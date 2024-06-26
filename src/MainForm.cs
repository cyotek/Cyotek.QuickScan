﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows.Forms;
using WIA;
using CommonDialog = WIA.CommonDialog;
using Timer = System.Timers.Timer;
using WiaProperties = WIA.Properties;

// Cyotek Quick Scan
// https://github.com/cyotek/Cyotek.QuickScan

// Copyright © 2019-2024 Cyotek Ltd.

// This work is licensed under the MIT License.
// See LICENSE.TXT for the full text

// Found this code useful?
// https://www.cyotek.com/contribute

namespace Cyotek.QuickScan
{
  internal partial class MainForm : BaseForm
  {
    #region Private Fields

    private bool _acceptSplitterChanges;

    private DialogResult _continuationResult;

    private IDeviceManager _deviceManager;

    private Timer _dialogResizeTimer;

    private int _findWindowRetryCount;

    private IntPtr _handle;

    private Image _image;

    private ImageFile _imageFile;

    private bool _isLimitedUi;

    private DateTimeOffset _lastScanDateTime;

    private int _lastScanHeight;

    private int _lastScanWidth;

    private int _lastScanXPos;

    private int _lastScanYPos;

    private string _selectedDeviceName;

    private Settings _settings;

    private SoundPlayer _soundPlayer;

    private string _tempFileName;

    #endregion Private Fields

    #region Public Constructors

    public MainForm()
    {
      this.InitializeComponent();
    }

    #endregion Public Constructors

    #region Protected Methods

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
      base.OnFormClosing(e);

      if (!e.Cancel)
      {
        if (_isLimitedUi)
        {
          _continuationResult = DialogResult.Cancel;
        }

        this.CleanUp();

        if (_settings.SaveSettingsOnExit)
        {
          this.SaveSettings();
        }
      }
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      this.LoadSettings();

      this.PrepareElevationItems();

      this.LoadFormats();

      this.ApplyWindowSettings();
      this.ApplySettings();
      this.UpdateUi();
    }

    protected override void OnShown(EventArgs e)
    {
      base.OnShown(e);

      try
      {
        _settings.IgnoreUpdates = true;
        this.LoadTypes();
        this.LoadDevices();
      }
      finally
      {
        _settings.IgnoreUpdates = false;
      }
    }

    #endregion Protected Methods

    #region Private Methods

    private static WiaImageIntent GetIntent(WiaProperties properties)
    {
      Property property;
      WiaImageIntent intent;

      property = properties.GetProperty(WiaPropertyId.WIA_IPA_DATATYPE);

      if (property != null)
      {
        switch ((WiaDataType)property.get_Value())
        {
          case WiaDataType.WIA_DATA_COLOR:
          case WiaDataType.WIA_DATA_COLOR_DITHER:
            intent = WiaImageIntent.ColorIntent;
            break;

          case WiaDataType.WIA_DATA_GRAYSCALE:
            intent = WiaImageIntent.GrayscaleIntent;
            break;

          case WiaDataType.WIA_DATA_THRESHOLD:
          case WiaDataType.WIA_DATA_COLOR_THRESHOLD:
            intent = WiaImageIntent.TextIntent;
            break;

          default:
            intent = WiaImageIntent.ColorIntent;
            break;
        }
      }
      else
      {
        intent = (WiaImageIntent)properties.GetPropertyInt32Value(WiaPropertyId.WIA_IPS_CUR_INTENT);
      }

      return intent;
    }

    private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
      AboutDialog.ShowAboutDialog();
    }

    private void ActualSizeToolStripMenuItem_Click(object sender, EventArgs e)
    {
      previewImageBox.ActualSize();
    }

    private void AddMetadata(Image image)
    {
      foreach (KeyValuePair<PropertyTag, Tuple<PropertyTagType, string>> pair in _settings.Metadata)
      {
        image.SetPropertyItem(pair.Key, pair.Value.Item1, pair.Value.Item2.MailMerge('{', '}', this.EvaluateMergeToken));
      }
    }

    private void ApplySettings()
    {
      this.SetDevice(_settings.DeviceId);
      continuousCheckBox.Checked = _settings.ContinuousScan;

      this.SetIntent(_settings.ImageIntent);
      this.SetDpi(_settings.ScanDpi);

      this.SetFormat(_settings.Format);
      this.SetQuality(_settings.Quality);
      folderTextBox.Text = _settings.OutputFolder;
      fileNameTextBox.Text = _settings.BaseFileName;
      this.SetCounter(_settings.Counter);
      useCounterCheckBox.Checked = _settings.UseCounter;
      autoSaveCheckBox.Checked = _settings.AutoSave;

      estimateFileSizesToolStripMenuItem.Checked = _settings.EstimateFileSizes;
      fileSizeToolStripStatusLabel.Visible = _settings.EstimateFileSizes;
      playSoundsToolStripMenuItem.Checked = _settings.PlaySounds;
      inlinePromptToolStripMenuItem.Checked = _settings.InlineScanPrompt;
      showProgressToolStripMenuItem.Checked = _settings.ShowProgress;
      maximizeScanWindowToolStripMenuItem.Checked = _settings.MaximizeScanDialog;
      automaticallySelectCustomToolStripMenuItem.Checked = _settings.AutoSelectCustom;

      continuationPanel.Visible = _settings.InlineScanPrompt;

      previewImageBox.ShowPixelGrid = _settings.ShowPixelGrid;
      this.SetOrientation(_settings.LayoutOrientation);
      this.SetPreview(_settings.ShowPreview);
      this.SetUnit(_settings.Unit);

      saveSettingsOnExitToolStripMenuItem.Checked = _settings.SaveSettingsOnExit;
    }

    private void ApplyWindowSettings()
    {
      _acceptSplitterChanges = false;

      UiHelpers.ApplyWindowPosition(this, _settings.WindowPosition);
      UiHelpers.SetSplitterSize(splitContainer, _settings.OptionsSplitterSize);

      _acceptSplitterChanges = true;
    }

    private void AutomaticallySelectCustomToolStripMenuItem_Click(object sender, EventArgs e)
    {
      automaticallySelectCustomToolStripMenuItem.Checked = !automaticallySelectCustomToolStripMenuItem.Checked;

      _settings.AutoSelectCustom = automaticallySelectCustomToolStripMenuItem.Checked;
    }

    private void AutoSaveCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      _settings.AutoSave = autoSaveCheckBox.Checked;
    }

    private void BrowseButton_Click(object sender, EventArgs e)
    {
      using (FolderBrowserDialog dialog = new FolderBrowserDialog
      {
        Description = "Select output &folder:",
        SelectedPath = _settings.OutputFolder,
        ShowNewFolderButton = true
      })
      {
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
          folderTextBox.Text = dialog.SelectedPath;
        }
      }
    }

    private string BuildFileName(string path, string name, string extension)
    {
      return Path.Combine(path, name + extension);
    }

    private void CalculateFileSize()
    {
      if (_settings.EstimateFileSizes)
      {
        if (fileSizeBackgroundWorker.IsBusy)
        {
          fileSizeBackgroundWorker.CancelAsync();
        }
        else if (_image == null)
        {
          fileSizeToolStripStatusLabel.Text = "(Unknown)";
        }
        else
        {
          ImageInfo imageInfo;

          fileSizeToolStripStatusLabel.Text = "(Calculating)";

          try
          {
            imageInfo = this.GetImageInfo();
          }
          catch
          {
            fileSizeToolStripStatusLabel.Text = "(Failed)";
            imageInfo = null;
          }

          if (imageInfo != null)
          {
            fileSizeBackgroundWorker.RunWorkerAsync(imageInfo);
          }
        }
      }
    }

    private void CancelScanButton_Click(object sender, EventArgs e)
    {
      _continuationResult = DialogResult.Cancel;
    }

    private void CentimetersToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.SetUnit(Unit.Centimeter);
    }

    private void CleanUp()
    {
      if (_dialogResizeTimer != null)
      {
        _dialogResizeTimer.Stop();
        _dialogResizeTimer.Dispose();
        _dialogResizeTimer = null;
      }

      previewImageBox.Image = null;

      if (_image != null)
      {
        _image.Dispose();
        _image = null;
      }

      FileUtilities.DeleteFile(_tempFileName);
    }

    private void ContinuousCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      _settings.ContinuousScan = continuousCheckBox.Checked;
    }

    private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.PerformImageAction(i => { ClipboardUtilities.CopyImage(i); return null; });
    }

    private void CounterNumericUpDown_ValueChanged(object sender, EventArgs e)
    {
      _settings.Counter = (int)counterNumericUpDown.Value;
    }

    private StatusController CreateStatusController(string message)
    {
      return new StatusController(statusToolStripStatusLabel, message);
    }

    private void DeviceComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.UpdateUi();

      _settings.DeviceId = deviceComboBox.SelectedItem is DeviceListBoxItem deviceInfo
        ? deviceInfo.DeviceId
        : null;

      if (!_settings.PromptForDevice)
      {
        Device device;

        device = this.GetSelectedDevice();

        if (device != null)
        {
          this.SetDpi(device);
        }
      }
    }

    private void DevicePropertiesButton_Click(object sender, EventArgs e)
    {
      this.PerformDeviceAction(device =>
      {
        CommonDialog dialog;

        dialog = new CommonDialog();

        dialog.ShowDeviceProperties(device);
      });
    }

    private void DevicePropertiesToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.PerformDeviceAction(device =>
      {
        PropertiesDialog.ShowPropertiesDialog(device.Properties);
      });
    }

    private void DialogResizeTimerElapsed(object sender, ElapsedEventArgs e)
    {
      this.ResizeWiaWindow();
    }

    private void DpiNumericUpDown_ValueChanged(object sender, EventArgs e)
    {
      _settings.ScanDpi = (int)dpiNumericUpDown.Value;
    }

    private void EstimateFileSizesToolStripMenuItem_Click(object sender, EventArgs e)
    {
      estimateFileSizesToolStripMenuItem.Checked = !estimateFileSizesToolStripMenuItem.Checked;

      _settings.EstimateFileSizes = estimateFileSizesToolStripMenuItem.Checked;

      this.ApplySettings();
    }

    private string EvaluateMergeToken(string token)
    {
      string result;

      if (token.EqualsIgnoreCase("now"))
      {
        result = _lastScanDateTime.ToString("s");
      }
      else if (token.EqualsIgnoreCase("now:exif"))
      {
        result = _lastScanDateTime.ToString("yyyy:MM:dd HH:mm:ss");
      }
      else if (token.EqualsIgnoreCase("year"))
      {
        result = _lastScanDateTime.Year.ToString(CultureInfo.InvariantCulture);
      }
      else if (token.EqualsIgnoreCase("appname"))
      {
        result = Application.ProductName;
      }
      else if (token.EqualsIgnoreCase("appversion"))
      {
        result = Application.ProductVersion;
      }
      else if (token != null && token.Length > 1 && token[0] == '#')
      {
        result = null;
        token = token.Substring(1);

        // TODO: This will be wrong if multiple devices are present and the user
        // chooses a different one from the dialog versus this selection
        this.PerformDeviceAction(device =>
        {
          foreach (Property property in device.Properties)
          {
            if (property.Name.EqualsIgnoreCase(token))
            {
              result = property.GetValueString();
              break;
            }
          }
        });
      }
      else
      {
        result = null;
      }

      return result;
    }

    private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void FileNameTextBox_TextChanged(object sender, EventArgs e)
    {
      _settings.BaseFileName = fileNameTextBox.Text;
    }

    private void FileSizeBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
    {
      ImageCodecInfo codec;
      EncoderParameters parameters;
      ImageInfo data;

      data = (ImageInfo)e.Argument;

      codec = ImageCodecHelpers.GetImageCodec(data.Format);
      parameters = ImageCodecHelpers.GetEncoderParameters(codec, data.Quality);

      using (MemoryStream stream = new MemoryStream())
      {
        data.Image.Save(stream, codec, parameters);

        e.Result = stream.Length;
      }
    }

    private void FileSizeBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      if (e.Error == null)
      {
        long size;

        size = Convert.ToInt64(e.Result);

        fileSizeToolStripStatusLabel.Text = size.ToFileSizeString();
      }
      else
      {
        Trace.WriteLine(e.Error.ToString());
        fileSizeToolStripStatusLabel.Text = "(Unknown)";
      }
    }

    private void FileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.PerformImageAction(i =>
      {
        i.RotateFlip(RotateFlipType.RotateNoneFlipY);
        return null;
      });
    }

    private void FlipHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.PerformImageAction(i =>
      {
        i.RotateFlip(RotateFlipType.RotateNoneFlipX);
        return null;
      });
    }

    private void FolderTextBox_TextChanged(object sender, EventArgs e)
    {
      _settings.OutputFolder = folderTextBox.Text;
    }

    private void FormatComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
      _settings.Format = formatComboBox.SelectedItem is KeyValueListBoxItem<Guid> item ? item.Value : Guid.Empty;

      if (formatComboBox.SelectedIndex != -1 && _image != null)
      {
        this.CalculateFileSize();
      }
    }

    private DialogResult GetContinuationAction()
    {
      return _settings.InlineScanPrompt
        ? this.GetInlineContinuationAction()
        : MessageBox.Show("Do you want to continue scanning using the current image size?", Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
    }

    private ImageFile GetImage(Func<Device, CommonDialog, ImageFile> getImage)
    {
      Device device;
      ImageFile image;

      device = this.GetSelectedDevice();

      if (device != null)
      {
        using (this.CreateStatusController("Scanning..."))
        {
          CommonDialog dialog;

          dialog = new CommonDialog();
          image = getImage(device, dialog);
        }
      }
      else
      {
        image = null;
      }

      return image;
    }

    private ImageInfo GetImageInfo()
    {
      // fun fact - WIA must be using GDI+, so all the codec GUID's are the same

      return new ImageInfo
      {
        Format = _settings.Format,
        Quality = _settings.Quality,
        Image = _image.Copy()
      };
    }

    private DialogResult GetInlineContinuationAction()
    {
      _continuationResult = DialogResult.None;

      this.ShowContinuationBar();

      do
      {
        Application.DoEvents(); // HACK
      } while (_continuationResult == DialogResult.None);

      this.HideContinuationBar();

      return _continuationResult;
    }

    private Device GetSelectedDevice()
    {
      Device result;

      if (_settings.PromptForDevice)
      {
        ICommonDialog dialog;

        dialog = new WIA.CommonDialog();

        try
        {
          result = dialog.ShowSelectDevice(WiaDeviceType.ScannerDeviceType, true, false);
        }
        catch (COMException ex) when (ex.HResult == -2145320939) // 0x80210015 no devices?
        {
          result = null;
        }
      }
      else
      {
        DeviceInfo deviceInfo;

        deviceInfo = this.GetSelectedDeviceInfo();

        if (deviceInfo != null)
        {
          try
          {
            using (this.CreateStatusController("Connecting to device..."))
            {
              result = deviceInfo.Connect();
            }
          }
          catch (COMException ex) when (ex.HResult == (int)WiaError.WIA_ERROR_OFFLINE || ex.HResult == (int)WiaError.E_FAIL)
          {
            result = null;
            UiHelpers.ShowError("Unable to connect to device; it may be offline.");
          }
          catch (Exception ex)
          {
            result = null;
            UiHelpers.ShowError("Unable to connect to device.", ex);
          }
        }
        else
        {
          result = null;
        }
      }

      return result;
    }

    private DeviceInfo GetSelectedDeviceInfo()
    {
      DeviceInfo deviceInfo;

      if (deviceComboBox.SelectedItem is DeviceListBoxItem deviceListBoxItem)
      {
        deviceInfo = _deviceManager.DeviceInfos[deviceListBoxItem.DeviceId];
      }
      else
      {
        deviceInfo = null;
      }

      return deviceInfo;
    }

    private string GetSelectedDeviceName()
    {
      return deviceComboBox.SelectedItem is DeviceListBoxItem deviceListBoxItem
        ? deviceListBoxItem.Name
        : null;
    }

    private void HideContinuationBar()
    {
      this.SetContinuationBarState(false);
    }

    private void HorizontalLayoutToolStripButton_Click(object sender, EventArgs e)
    {
      this.SetOrientation(horizontalLayoutToolStripButton.Checked ? Orientation.Vertical : Orientation.Horizontal);
    }

    private void HorizontalToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.SetOrientation(Orientation.Horizontal);
    }

    private void InchesToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.SetUnit(Unit.Inch);
    }

    private void InlinePromptToolStripMenuItem_Click(object sender, EventArgs e)
    {
      inlinePromptToolStripMenuItem.Checked = !inlinePromptToolStripMenuItem.Checked;

      _settings.InlineScanPrompt = inlinePromptToolStripMenuItem.Checked;

      continuationPanel.Visible = _settings.InlineScanPrompt;
    }

    private void LoadDevices()
    {
      IDeviceInfos devices;

      _deviceManager = new DeviceManager();

      devices = _deviceManager.DeviceInfos;

      deviceComboBox.Items.Clear();

      for (int i = 0; i < devices.Count; i++)
      {
        IDeviceInfo device;

        device = devices[i + 1];

        if (device.Type == WiaDeviceType.ScannerDeviceType)
        {
          deviceComboBox.Items.Add(new DeviceListBoxItem(device));
        }
      }

      if (deviceComboBox.Items.Count != 0)
      {
        deviceComboBox.Sorted = true;
        deviceComboBox.Sorted = false;
        deviceComboBox.SelectedIndex = 0;
      }
    }

    private void LoadFormats()
    {
      formatComboBox.Items.Add(new KeyValueListBoxItem<Guid>("Windows bitmap (*.bmp)", WiaFormatId.Bmp));
      formatComboBox.Items.Add(new KeyValueListBoxItem<Guid>("Joint Photographic Experts Group (*.jpg)", WiaFormatId.Jpeg));
      formatComboBox.Items.Add(new KeyValueListBoxItem<Guid>("Portable Network Graphics (*.png)", WiaFormatId.Png));
      formatComboBox.Items.Add(new KeyValueListBoxItem<Guid>("Tagged Image File Format (*.tiff)", WiaFormatId.Tiff));
      formatComboBox.Items.Add(new KeyValueListBoxItem<Guid>("Graphics Interchange Format (*.gif)", WiaFormatId.Gif));
    }

    private void LoadSettings()
    {
      _settings = new Settings();
      _settings.Load();
    }

    private void LoadTypes()
    {
      typeComboBox.Items.Add(new KeyValueListBoxItem<WiaImageIntent>("Color", WiaImageIntent.ColorIntent));
      typeComboBox.Items.Add(new KeyValueListBoxItem<WiaImageIntent>("Grayscale", WiaImageIntent.GrayscaleIntent));
      typeComboBox.Items.Add(new KeyValueListBoxItem<WiaImageIntent>("Monochrome", WiaImageIntent.TextIntent));
      typeComboBox.SelectedIndex = 0;
    }

    private void MaximizeScanWindowToolStripMenuItem_Click(object sender, EventArgs e)
    {
      maximizeScanWindowToolStripMenuItem.Checked = !maximizeScanWindowToolStripMenuItem.Checked;

      _settings.MaximizeScanDialog = maximizeScanWindowToolStripMenuItem.Checked;
    }

    private void NextScanButton_Click(object sender, EventArgs e)
    {
      _continuationResult = DialogResult.Yes;
    }

    private void OpenFolderButton_Click(object sender, EventArgs e)
    {
      ProcessHelpers.OpenFolderInExplorer(_settings.OutputFolder);
    }

    private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Image image;

      image = ClipboardUtilities.GetImage();

      if (image != null)
      {
        this.SetImage(image, true);
      }
      else
      {
        SystemSounds.Beep.Play();
      }
    }

    private void PerformDeviceAction(Action<Device> action)
    {
      Device device;

      device = this.GetSelectedDevice();

      if (device != null)
      {
        action(device);
      }
      else
      {
        UiHelpers.ShowWarning("No device selected.");
      }
    }

    private void PerformImageAction(Func<Image, Image> action)
    {
      if (_image != null)
      {
        Image newImage;
        Size size;

        size = _image.Size;

        newImage = action(_image);

        if (newImage != null)
        {
          this.CalculateFileSize();
          this.SetImage(newImage, false);
        }
        else if (_image.Size == size)
        {
          previewImageBox.Invalidate();
        }
        else
        {
          newImage = _image;

          this.SetImage(newImage, false);
        }
      }
      else
      {
        SystemSounds.Beep.Play();
      }
    }

    private ImageFile PerformNewScan(Device device, CommonDialog dialog)
    {
      Items items;
      ImageFile result;

      this.PrepareResizeDialogTimer();

      // if you use ShowAcquireImage, the resulting device properties don't
      // reflect the original scan. For example, WIA_IPS_XPOS and WIA_IPS_YPOS
      // will both be zero, even if co-ordinates were originally offset. To
      // allow chain scanning when using non-zero co-ordinates, we need to use
      // ShowSelectItems to get the "definition" of the pending scan, extract
      // what we need, then manually request the scan with.
      //
      // Happily, this also allows me to fix a previous bug where if the DPI of
      // the scan didn't match the UI DPI, subsequent scans would either be
      // invalid or fail.
      //
      // Intent seems to be wrong if user chooses Grayscale or Black
      // and White in the GUI. This actually maps to WIA_IPA_DATATYPE (4103)
      // with values including WIA_DATA_COLOR_DITHER, WIA_DATA_GRAYSCALE and
      // WIA_DATA_THRESHOLD, so I'm now ignoring WIA_IPS_CUR_INTENT and instead
      // looking at WIA_IPA_DATATYPE

      items = dialog.ShowSelectItems(device, Intent: _settings.ImageIntent, Bias: WiaImageBias.MaximizeQuality, SingleSelect: true, UseCommonUI: true, CancelError: false);

      if (items != null && items.Count == 1)
      {
        WiaProperties properties;

        properties = items[1].Properties;

        _lastScanXPos = properties.GetPropertyInt32Value(WiaPropertyId.WIA_IPS_XPOS);
        _lastScanYPos = properties.GetPropertyInt32Value(WiaPropertyId.WIA_IPS_YPOS);
        _lastScanWidth = properties.GetPropertyInt32Value(WiaPropertyId.WIA_IPS_XEXTENT);
        _lastScanHeight = properties.GetPropertyInt32Value(WiaPropertyId.WIA_IPS_YEXTENT);

        _settings.DeviceId = device.DeviceID;
        this.SetDevice(device.DeviceID);
        this.SetIntent(GetIntent(properties));
        this.SetDpi(properties.GetPropertyInt32Value(WiaPropertyId.WIA_IPS_XRES));

        result = this.RepeatLastScan(device, dialog);
      }
      else
      {
        result = null;
      }

      return result;
    }

    private void PixelGridToolStripMenuItem_Click(object sender, EventArgs e)
    {
      _settings.ShowPixelGrid = !_settings.ShowPixelGrid;

      this.ApplySettings();
    }

    private void PixelsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.SetUnit(Unit.Pixel);
    }

    private void PlaySound(string fileName)
    {
      if (_settings.PlaySounds && !string.IsNullOrWhiteSpace(fileName))
      {
        fileName = Path.Combine(Application.StartupPath, fileName);

        if (File.Exists(fileName))
        {
          try
          {
            if (_soundPlayer == null)
            {
              _soundPlayer = new SoundPlayer();
            }

            _soundPlayer.SoundLocation = fileName;
            _soundPlayer.Play();
          }
          catch
          {
            // don't care
          }
        }
      }
    }

    private void PlaySoundsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      playSoundsToolStripMenuItem.Checked = !playSoundsToolStripMenuItem.Checked;

      _settings.PlaySounds = playSoundsToolStripMenuItem.Checked;
    }

    private void PrepareElevationItems()
    {
      NativeMethods.SHSTOCKICONINFO info;

      info = new NativeMethods.SHSTOCKICONINFO
      {
        cbSize = Marshal.SizeOf<NativeMethods.SHSTOCKICONINFO>()
      };

      NativeMethods.SHGetStockIconInfo(NativeMethods.SIID_SHIELD, NativeMethods.SHGSI_ICON | NativeMethods.SHGSI_SMALLICON, ref info);

      restartWIAServiceToolStripMenuItem.Image = Icon.FromHandle(info.hIcon).ToBitmap();

      NativeMethods.DestroyIcon(info.hIcon);
    }

    private void PrepareResizeDialogTimer()
    {
      if (_settings.MaximizeScanDialog)
      {
        // this is a bit hacky. Can't use a WinForms Timer as this is blocked
        // until the WIA dialog is dismissed, which kind of defeats the purpose.
        // I'm using a System.Timers.Timer instead as this fires on a different
        // thread. The problem with _that_ is that means I can't access UI
        // properties such as the window handle or the selected device name. Can't
        // invoke or synchronise as that too is blocked. So I'm storing the values
        // up front to work around this.

        _handle = this.Handle;
        _selectedDeviceName = string.Format(_settings.ScanDialogTitle, this.GetSelectedDeviceName());

        _findWindowRetryCount = 0;

        if (_dialogResizeTimer == null)
        {
          _dialogResizeTimer = new Timer { AutoReset = true, Interval = 100 };
          _dialogResizeTimer.Elapsed += this.DialogResizeTimerElapsed;
        }

        _dialogResizeTimer.Start();
      }
    }

    private void PreviewImageBox_ShowPixelGridChanged(object sender, EventArgs e)
    {
      bool showGrid;

      showGrid = previewImageBox.ShowPixelGrid;

      previewImageBox.ShowPixelGrid = showGrid;

      pixelGridToolStripButton.Checked = showGrid;
      pixelGridToolStripMenuItem.Checked = showGrid;
    }

    private void PreviewLinkLabel_Click(object sender, EventArgs e)
    {
      this.ShowImagePreview();
    }

    private void QualityNumericUpDown_ValueChanged(object sender, EventArgs e)
    {
      _settings.Quality = (int)qualityNumericUpDown.Value;

      if (_settings.Format == WiaFormatId.Jpeg && _image != null)
      {
        this.CalculateFileSize();
      }
    }

    private void ReconfigureScanButton_Click(object sender, EventArgs e)
    {
      _continuationResult = DialogResult.No;
    }

    private void RefreshDeviceListToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.LoadDevices();
    }

    private ImageFile RepeatLastScan(Device device, CommonDialog dialog)
    {
      Item item;

      item = device.Items[1];

      this.SetDeviceProperties(item.Properties, _lastScanXPos, _lastScanYPos, _lastScanWidth, _lastScanHeight);

      return _settings.ShowProgress
        ? dialog.ShowTransfer(item, _settings.FormatString, false)
        : item.Transfer(_settings.FormatString);
    }

    [DebuggerStepThrough]
    private void ResizeWiaWindow()
    {
      try
      {
        IntPtr handle;

        // try and find a window with the right title
        // if we find a match, see if it's owned by us
        // and if so, resize it to fit the display space

        // note: sticking a break point here doesn't go down well

        handle = NativeMethods.FindWindow(null, _selectedDeviceName);

        if (handle != IntPtr.Zero)
        {
          IntPtr ownerHandle;

          ownerHandle = NativeMethods.GetWindow(handle, NativeMethods.GW_OWNER);

          if (ownerHandle == _handle)
          {
            Rectangle area;

            area = Screen.FromHandle(_handle).WorkingArea;

            NativeMethods.SetWindowPos(handle, IntPtr.Zero, area.X, area.Y, area.Width, area.Height, NativeMethods.SWP_NOZORDER | NativeMethods.SWP_NOACTIVATE);

            if (_settings.AutoSelectCustom)
            {
              this.SelectCustomScanOption(handle);
            }

            _dialogResizeTimer.Stop();
          }
        }

        // automatically abort if we don't find a match quickly
        if (_findWindowRetryCount++ > 10)
        {
          _dialogResizeTimer.Stop();
        }
      }
      catch
      {
        // no-op
        _dialogResizeTimer.Stop();
      }
    }

    private void RestartWIAServiceToolStripMenuItem_Click(object sender, EventArgs e)
    {
      string fileName;

      fileName = Path.Combine(Application.StartupPath, "rstrtwia.exe");

      try
      {
        Process.Start(fileName);
      }
      catch (Exception ex)
      {
        UiHelpers.ShowError("Failed to execute helper program.", ex);
      }
    }

    private void Rotate90ClockwiseToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.PerformImageAction(i =>
      {
        i.RotateFlip(RotateFlipType.Rotate90FlipNone);
        return null;
      });
    }

    private void Rotate90CounterClockwiseToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.PerformImageAction(i =>
      {
        i.RotateFlip(RotateFlipType.Rotate270FlipNone);
        return null;
      });
    }

    private void RunScanLoop()
    {
      if (this.ValidateOutputOptions(true))
      {
        bool done;
        bool keepSize;

        done = false;
        keepSize = false;

        while (!done)
        {
          ImageFile image;

          image = keepSize
            ? this.GetImage(this.RepeatLastScan)
            : this.GetImage(this.PerformNewScan);

          if (image != null)
          {
            string fileName;

            _lastScanDateTime = DateTimeOffset.UtcNow;

            this.SetImage(image);

            fileName = _settings.AutoSave
              ? this.SaveImage()
              : null;

            if (_settings.AutoSave && _settings.ContinuousScan && !string.IsNullOrEmpty(fileName))
            {
              this.PlaySound(_settings.NextScanSound);

              switch (this.GetContinuationAction())
              {
                case DialogResult.Yes:
                  keepSize = true;
                  break;

                case DialogResult.No:
                  keepSize = false;
                  break;

                default:
                  done = true;
                  break;
              }
            }
            else
            {
              done = true;
            }
          }
          else
          {
            done = true;
          }
        }

        if (!_settings.AutoSave)
        {
          this.CalculateFileSize();
        }
      }
    }

    private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.SaveImageAs();
    }

    private string SaveImage()
    {
      string path;
      string result;

      path = _settings.OutputFolder;
      result = null;

      if (this.ValidateOutputOptions(false))
      {
        string baseName;
        string fileName;
        string extension;
        ImageCodecInfo codecInfo;
        EncoderParameters encoderParameters;

        codecInfo = ImageCodecHelpers.GetImageCodec(_settings.Format);
        encoderParameters = ImageCodecHelpers.GetEncoderParameters(codecInfo, _settings.Quality);
        extension = ImageCodecHelpers.GetSuggestedExtension(codecInfo);

        baseName = _settings.BaseFileName;

        if (string.IsNullOrEmpty(baseName))
        {
          baseName = DateTime.Now.ToString("yyyyMMdd HHmm");
        }

        if (_settings.UseCounter || File.Exists(this.BuildFileName(path, baseName, extension)))
        {
          do
          {
            fileName = this.BuildFileName(path, baseName + " (" + _settings.Counter + ")", extension);
            _settings.Counter++;
          } while (File.Exists(fileName));
        }
        else
        {
          fileName = this.BuildFileName(path, baseName, extension);
        }

        using (this.CreateStatusController("Saving image..."))
        {
          if (_settings.AddMetadata)
          {
            this.AddMetadata(_image);
          }

          try
          {
            _image.Save(fileName, codecInfo, encoderParameters);
          }
          catch
          {
            fileName = this.SaveImageDirect(fileName);
          }

          result = fileName;
        }
      }

      return result;
    }

    private void SaveImageAs()
    {
      this.PerformImageAction(i =>
      {
        using (SaveFileDialog dialog = new SaveFileDialog
        {
          Title = "Save Image As",
          Filter = "Portable Network Graphics (*.png)|*.png|All Files (*.*)|*.*",
          DefaultExt = "png"
        })
        {
          if (dialog.ShowDialog() == DialogResult.OK)
          {
            try
            {
              i.Save(dialog.FileName, ImageFormat.Png);
            }
            catch (Exception ex)
            {
              UiHelpers.ShowError("Failed to save file.", ex);
            }
          }
        }

        return null;
      });
    }

    private string SaveImageDirect(string fileName)
    {
      fileName = Path.ChangeExtension(fileName, _imageFile.FileExtension);

      try
      {
        _imageFile.SaveFileEx(fileName);

        UiHelpers.ShowWarning(string.Format("Failed to save image in requested format. Image saved to '{0}' without customisation.", fileName));
      }
      catch (Exception ex)
      {
        UiHelpers.ShowError("Failed to save image.", ex);
      }

      return fileName;
    }

    private void SaveSettings()
    {
      _settings.WindowPosition = UiHelpers.GetWindowPosition(this);
      _settings.Save();
    }

    private void SaveSettingsNowToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.SaveSettings();
    }

    private void SaveSettingsOnExitToolStripMenuItem_Click(object sender, EventArgs e)
    {
      _settings.SaveSettingsOnExit = !_settings.SaveSettingsOnExit;

      saveSettingsOnExitToolStripMenuItem.Checked = _settings.SaveSettingsOnExit;
    }

    private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
    {
      string fileName;

      fileName = this.SaveImage();

      if (!string.IsNullOrEmpty(fileName))
      {
        MessageBox.Show(string.Format("File saved to {0}.", fileName), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
    }

    private void ScanPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.PerformDeviceAction(device =>
      {
        WiaProperties properties;

        properties = device.Items[1].Properties;

        PropertiesDialog.ShowPropertiesDialog(properties);
      });
    }

    private void ScanToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.RunScanLoop();
    }

    private void SelectCustomScanOption(IntPtr parentHandle)
    {
      IntPtr handle;

      handle = NativeMethods.FindWindowEx(parentHandle, IntPtr.Zero, "Button", _settings.AutoSelectCustomTitle);

      if (handle != IntPtr.Zero)
      {
        NativeMethods.SendMessage(handle, NativeMethods.BM_CLICK, IntPtr.Zero, IntPtr.Zero);
      }
    }

    private void SetContinuationBarState(bool enabled)
    {
      _isLimitedUi = enabled;

      deviceGroupBox.Enabled = !enabled;
      deviceSettingsGroupBox.Enabled = !enabled;
      this.UpdateUi();

      continueScanningLabel.Enabled = enabled;
      nextScanButton.Enabled = enabled;
      reconfigureScanButton.Enabled = enabled;
      cancelScanButton.Enabled = enabled;
    }

    private void SetCounter(int counter)
    {
      if (counter >= counterNumericUpDown.Minimum && counter <= counterNumericUpDown.Maximum)
      {
        counterNumericUpDown.Value = counter;
      }
    }

    private void SetDevice(string deviceId)
    {
      for (int i = 0; i < deviceComboBox.Items.Count; i++)
      {
        if (deviceComboBox.Items[i] is DeviceListBoxItem item && string.Equals(item.DeviceId, deviceId, StringComparison.OrdinalIgnoreCase))
        {
          deviceComboBox.SelectedIndex = i;
          break;
        }
      }
    }

    private void SetDeviceProperties(WiaProperties deviceProperties, int left, int top, int width, int height)
    {
      if (_settings.ScanDpi <= 0)
      {
        _settings.ScanDpi = (int)dpiNumericUpDown.Value;
      }

      deviceProperties.SetPropertyValue(WiaPropertyId.WIA_IPS_CUR_INTENT, _settings.ImageIntent); // set this first as it resets a bunch of other properties

      deviceProperties.SetPropertyValue(WiaPropertyId.WIA_IPS_XRES, _settings.ScanDpi);
      deviceProperties.SetPropertyValue(WiaPropertyId.WIA_IPS_YRES, _settings.ScanDpi);

      deviceProperties.SetPropertyValue(WiaPropertyId.WIA_IPS_XPOS, left);
      deviceProperties.SetPropertyValue(WiaPropertyId.WIA_IPS_YPOS, top);

      if (width < 0)
      {
        deviceProperties.SetPropertyMaximum(WiaPropertyId.WIA_IPS_XEXTENT);
      }
      else
      {
        deviceProperties.SetPropertyValue(WiaPropertyId.WIA_IPS_XEXTENT, width);
      }

      if (height < 0)
      {
        deviceProperties.SetPropertyMaximum(WiaPropertyId.WIA_IPS_YEXTENT);
      }
      else
      {
        deviceProperties.SetPropertyValue(WiaPropertyId.WIA_IPS_YEXTENT, height);
      }
    }

    private void SetDpi(Device device)
    {
      WiaProperties properties;
      Property xDpi;
      Property yDpi;
      int min;
      int max;

      properties = device.Items[1].Properties;
      xDpi = properties.GetProperty(WiaPropertyId.WIA_IPS_XRES);
      yDpi = properties.GetProperty(WiaPropertyId.WIA_IPS_YRES);

      try
      {
        min = Math.Max(xDpi.SubTypeMin, yDpi.SubTypeMin);
      }
      catch (COMException)
      {
        min = 150;
      }

      try
      {
        max = Math.Min(xDpi.SubTypeMax, yDpi.SubTypeMax);
      }
      catch (COMException)
      {
        max = 4800;
      }

      dpiNumericUpDown.Minimum = min;
      dpiNumericUpDown.Maximum = max;
      dpiNumericUpDown.Value = max;
    }

    private void SetDpi(int dpi)
    {
      _settings.ScanDpi = dpi;

      if (dpi >= dpiNumericUpDown.Minimum && dpi <= dpiNumericUpDown.Maximum)
      {
        dpiNumericUpDown.Value = dpi;
      }
    }

    private void SetFormat(Guid format)
    {
      for (int i = 0; i < formatComboBox.Items.Count; i++)
      {
        if (formatComboBox.Items[i] is KeyValueListBoxItem<Guid> item && item.Value == format)
        {
          formatComboBox.SelectedIndex = i;
          break;
        }
      }

      if (formatComboBox.SelectedIndex == -1)
      {
        formatComboBox.SelectedIndex = 0;
      }
    }

    private void SetImage(ImageFile image)
    {
      _imageFile = image;

      formatToolStripStatusLabel.Text = image.FileExtension;

      previewImageBox.Image = null;

      if (_image != null)
      {
        _image.Dispose();
        _image = null;
      }

      if (string.IsNullOrEmpty(_tempFileName))
      {
        _tempFileName = Path.GetTempFileName();
      }

      image.SaveFileEx(_tempFileName);

      this.SetImage(Image.FromFile(_tempFileName), true);
    }

    private void SetImage(Image image, bool resetZoom)
    {
      previewImageBox.BeginUpdate();

      previewImageBox.Image = null;

      if (_image != null && !object.ReferenceEquals(_image, image))
      {
        _image.Dispose();
        _image = null;
      }

      _image = image;

      previewImageBox.Image = image;

      this.UpdateSize();

      if (resetZoom)
      {
        previewImageBox.ZoomToFit();
      }

      previewImageBox.EndUpdate();
    }

    private void SetIntent(WiaImageIntent imageIntent)
    {
      _settings.ImageIntent = imageIntent;

      for (int i = 0; i < typeComboBox.Items.Count; i++)
      {
        if (typeComboBox.Items[i] is KeyValueListBoxItem<WiaImageIntent> item && item.Value == imageIntent)
        {
          typeComboBox.SelectedIndex = i;
          break;
        }
      }
    }

    private void SetOrientation(Orientation orientation)
    {
      _settings.LayoutOrientation = orientation;

      splitContainer.Orientation = orientation;

      horizontalToolStripMenuItem.Checked = orientation == Orientation.Horizontal;
      horizontalLayoutToolStripButton.Checked = orientation == Orientation.Horizontal;
      verticalToolStripMenuItem.Checked = orientation == Orientation.Vertical;
    }

    private void SetPreview(bool show)
    {
      _settings.ShowPreview = show;

      showPreviewToolStripMenuItem.Checked = show;
      imagePreviewToolStripButton.Checked = show;
      splitContainer.Panel2Collapsed = !show;
    }

    private void SetQuality(int quality)
    {
      if (quality >= qualityNumericUpDown.Minimum && quality <= qualityNumericUpDown.Maximum)
      {
        qualityNumericUpDown.Value = quality;
      }
    }

    private void SetUnit(Unit unit)
    {
      _settings.Unit = unit;

      pixelsToolStripMenuItem.Checked = unit == Unit.Pixel;
      statusPixelsToolStripMenuItem.Checked = unit == Unit.Pixel;
      inchesToolStripMenuItem.Checked = unit == Unit.Inch;
      statusInchesToolStripMenuItem.Checked = unit == Unit.Inch;
      centimetersToolStripMenuItem.Checked = unit == Unit.Centimeter;
      statusCentimetersToolStripMenuItem.Checked = unit == Unit.Centimeter;

      switch (unit)
      {
        case Unit.Pixel:
          unitToolStripSplitButton.Text = "px";
          break;

        case Unit.Inch:
          unitToolStripSplitButton.Text = "in";
          break;

        case Unit.Centimeter:
          unitToolStripSplitButton.Text = "cm";
          break;
      }

      this.UpdateSize();
    }

    private void ShowContinuationBar()
    {
      this.SetContinuationBarState(true);
    }

    private void ShowImagePreview()
    {
      if (_image != null)
      {
        using (this.CreateStatusController("Creating preview image..."))
        {
          Guid format;
          int quality;
          ImageCodecInfo codec;
          EncoderParameters parameters;

          format = _settings.Format;
          quality = _settings.Quality;
          codec = ImageCodecHelpers.GetImageCodec(format);
          parameters = ImageCodecHelpers.GetEncoderParameters(codec, quality);

          using (Image copy = _image)
          {
            using (MemoryStream stream = new MemoryStream())
            {
              copy.Save(stream, codec, parameters);

              stream.Position = 0;

              using (Image loaded = Image.FromStream(stream))
              {
                ImagePreviewWindow.ShowImagePreview(loaded);
              }
            }
          }
        }
      }
      else
      {
        UiHelpers.ShowWarning("Please scan an image first.");
      }
    }

    private void ShowImagePreviewButton_Click(object sender, EventArgs e)
    {
      this.ShowImagePreview();
    }

    private void ShowPreviewToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.SetPreview(!_settings.ShowPreview);
    }

    private void ShowProgressToolStripMenuItem_Click(object sender, EventArgs e)
    {
      showProgressToolStripMenuItem.Checked = !showProgressToolStripMenuItem.Checked;

      _settings.ShowProgress = showProgressToolStripMenuItem.Checked;
    }

    private void SplitContainer_SplitterMoved(object sender, SplitterEventArgs e)
    {
      if (_settings != null && _acceptSplitterChanges)
      {
        _settings.OptionsSplitterSize = splitContainer.SplitterDistance;
      }
    }

    private void TypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
      _settings.ImageIntent = typeComboBox.SelectedItem is KeyValueListBoxItem<WiaImageIntent> item ? item.Value : WiaImageIntent.UnspecifiedIntent;
    }

    private void UnitToolStripSplitButton_ButtonClick(object sender, EventArgs e)
    {
      Unit unit;

      unit = _settings.Unit + 1;

      if (unit > Unit.Centimeter)
      {
        unit = Unit.Pixel;
      }

      this.SetUnit(unit);
    }

    private void UpdateSize()
    {
      if (_image != null)
      {
        float w;
        float h;

        switch (_settings.Unit)
        {
          case Unit.Pixel:
            w = _image.Width;
            h = _image.Height;
            break;

          case Unit.Inch:
            w = _image.Width / _image.VerticalResolution;
            h = _image.Height / _image.HorizontalResolution;
            break;

          case Unit.Centimeter:
            w = _image.Width * 2.54F / _image.VerticalResolution;
            h = _image.Height * 2.54F / _image.HorizontalResolution;
            break;

          default:
            w = _image.Width;
            h = _image.Height;
            break;
        }

        sizeToolStripStatusLabel.Text = string.Format("{0:F2} x {1:F2}", w, h);
      }
      else
      {
        sizeToolStripStatusLabel.Text = "(Unknown)";
      }
    }

    private void UpdateUi()
    {
      bool canScan;

      canScan = deviceComboBox.SelectedIndex != -1 && !_isLimitedUi;

      devicePropertiesButton.Enabled = canScan;
      scanToolStripButton.Enabled = canScan;
      scanToolStripMenuItem.Enabled = canScan;
    }

    private void UseCounterCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      _settings.UseCounter = useCounterCheckBox.Checked;
    }

    private bool ValidateOutputOptions(bool isPreScan)
    {
      bool result;
      string path;

      result = false;
      path = _settings.OutputFolder;

      if (!isPreScan && _image == null)
      {
        UiHelpers.ShowWarning("Please scan an image first.");
      }
      else if (_settings.AutoSave || !isPreScan)
      {
        if (string.IsNullOrEmpty(path))
        {
          UiHelpers.ShowWarning("Output folder not specified.");
        }
        else if (!Directory.Exists(path))
        {
          UiHelpers.ShowWarning(string.Format("Output folder '{0}' not found.", path));
        }
        else
        {
          result = true;
        }
      }
      else
      {
        result = true;
      }

      return result;
    }

    private void VerticalToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.SetOrientation(Orientation.Vertical);
    }

    private void ZoomToWindowToolStripMenuItem_Click(object sender, EventArgs e)
    {
      previewImageBox.ZoomToFit();
    }

    #endregion Private Methods
  }
}