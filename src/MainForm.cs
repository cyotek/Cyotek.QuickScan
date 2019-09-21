﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WIA;
using CommonDialog = WIA.CommonDialog;

namespace Cyotek.QuickScan
{
  internal partial class MainForm : BaseForm
  {
    #region Private Fields

    private IDeviceManager _deviceManager;

    private Bitmap _image;

    private Settings _settings;

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

      if (!e.Cancel && saveSettingsOnExitToolStripMenuItem.Checked)
      {
        this.SaveSettings();
      }
    }

    protected override void OnShown(EventArgs e)
    {
      base.OnShown(e);

      this.LoadSettings();

      try
      {
        _settings.IgnoreUpdates = true;
        this.LoadTypes();
        this.LoadFormats();
        this.LoadDevices();
      }
      finally
      {
        _settings.IgnoreUpdates = false;
      }

      this.ApplySettings();
      this.UpdateUi();
    }

    #endregion Protected Methods

    #region Private Methods

    private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
      AboutDialog.ShowAboutDialog();
    }

    private void ActualSizeToolStripMenuItem_Click(object sender, EventArgs e)
    {
      previewImageBox.ActualSize();
    }

    private void ApplySettings()
    {
      this.SetDevice(_settings.DeviceId);
      devicePromptCheckBox.Checked = _settings.PromptForDevice;
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

      previewImageBox.ShowPixelGrid = _settings.ShowPixelGrid;
      this.SetOrientation(_settings.LayoutOrientation);
      this.SetPreview(_settings.ShowPreview);
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
          fileSizeToolStripStatusLabel.Text = "(Calculating)";
          fileSizeBackgroundWorker.RunWorkerAsync(this.GetImageInfo());
        }
      }
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

      _settings.DeviceId = deviceComboBox.SelectedItem is DeviceListBoxItem device ? device.DeviceId : null;

      if (!_settings.PromptForDevice)
      {
        WIA.Properties properties;
        Property xDpi;
        Property yDpi;
        int min;
        int max;

        properties = this.GetSelectedDevice().Items[1].Properties;
        xDpi = properties.GetProperty(WiaPropertyId.WIA_IPS_XRES);
        yDpi = properties.GetProperty(WiaPropertyId.WIA_IPS_YRES);

        min = Math.Max(xDpi.SubTypeMin, yDpi.SubTypeMin);
        max = Math.Min(xDpi.SubTypeMax, yDpi.SubTypeMax);

        dpiNumericUpDown.Minimum = min;
        dpiNumericUpDown.Maximum = max;
        dpiNumericUpDown.Value = max;
      }
    }

    private void DevicePromptCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      _settings.PromptForDevice = devicePromptCheckBox.Checked;

      this.UpdateUi();
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
        Image = _image
        Image = _image.Copy()
      };
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
          result = deviceInfo.Connect();
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

    private void HorizontalLayoutToolStripButton_Click(object sender, EventArgs e)
    {
      this.SetOrientation(horizontalLayoutToolStripButton.Checked ? Orientation.Vertical : Orientation.Horizontal);
    }

    private void HorizontalToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.SetOrientation(Orientation.Horizontal);
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

      formatComboBox.SelectedIndex = 0;
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

    private void OpenFolderButton_Click(object sender, EventArgs e)
    {
      ProcessHelpers.OpenFolderInExplorer(_settings.OutputFolder);
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
        MessageBox.Show("No device selected.", "Device Properties", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
      }
    }

    private void PerformDeviceInfoAction(Action<DeviceInfo> action)
    {
      DeviceInfo device;

      device = this.GetSelectedDeviceInfo();

      if (device != null)
      {
        action(device);
      }
      else
      {
        MessageBox.Show("No device selected.", "Device Properties", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
      }
    }

    private void PerformImageAction(Func<Bitmap, Bitmap> action)
    {
      if (_image != null)
      {
        Bitmap newImage;
        Size size;

        size = _image.Size;

        newImage = action(_image);

        if (newImage != null)
        {
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

        this.CalculateFileSize();
      }
      else
      {
        SystemSounds.Beep.Play();
      }
    }

    private void PixelGridToolStripMenuItem_Click(object sender, EventArgs e)
    {
      _settings.ShowPixelGrid = !_settings.ShowPixelGrid;

      this.ApplySettings();
    }

    private void PreviewButton_Click(object sender, EventArgs e)
    {
      this.RunScanLoop((_, dialog) => dialog.ShowAcquireImage(WiaDeviceType.ScannerDeviceType, _settings.ImageIntent, WiaImageBias.MaximizeQuality, _settings.FormatString, false, true, false));
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
      if (_image != null)
      {
        ImageCodecInfo codec;
        EncoderParameters parameters;
        Guid format;
        int quality;

        using (this.CreateStatusController("Creating preview image..."))
        {
          format = _settings.Format;
          quality = _settings.Quality;
          codec = ImageCodecHelpers.GetImageCodec(format);
          parameters = ImageCodecHelpers.GetEncoderParameters(codec, quality);

          using (Bitmap copy = _image)
          {
            using (MemoryStream stream = new MemoryStream())
            {
              copy.Save(stream, codec, parameters);

              stream.Position = 0;

              using (Bitmap loaded = (Bitmap)Image.FromStream(stream))
              {
                ImagePreviewWindow.ShowImagePreview(loaded);
              }
            }
          }
        }
      }
      else
      {
        MessageBox.Show("Please scan an image first.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
    }

    private void QualityNumericUpDown_ValueChanged(object sender, EventArgs e)
    {
      _settings.Quality = (int)qualityNumericUpDown.Value;

      if (_settings.Format == WiaFormatId.Jpeg && _image != null)
      {
        this.CalculateFileSize();
      }
    }

    private void RefreshDeviceListToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.LoadDevices();
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

    private void RunScanLoop(Func<Device, CommonDialog, ImageFile> getImage)
    {
      bool done;

      done = false;

      while (!done)
      {
        ImageFile image;

        image = this.GetImage(getImage);

        if (image != null)
        {
          string fileName;

          this.SetImage(image);

          fileName = _settings.AutoSave ? this.SaveImage() : null;

          done = !(_settings.AutoSave && _settings.ContinuousScan && !string.IsNullOrEmpty(fileName) && MessageBox.Show("Continue scanning?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes);
        }
        else
        {
          done = true;
        }
      }

      this.CalculateFileSize();
    }

    private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.SaveImageAs();
    }

    private void SaveButton_Click(object sender, EventArgs e)
    {
      string fileName;

      fileName = this.SaveImage();

      if (!string.IsNullOrEmpty(fileName))
      {
        MessageBox.Show(string.Format("File saved to {0}.", fileName), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
    }

    private string SaveImage()
    {
      string path;
      string result;

      path = _settings.OutputFolder;
      result = null;

      if (_image == null)
      {
        MessageBox.Show("Please scan an image first.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
      }
      else if (string.IsNullOrEmpty(path))
      {
        MessageBox.Show("Output folder not specified.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
      }
      else if (!Directory.Exists(path))
      {
        MessageBox.Show(string.Format("Output folder '{0}' not found.", path), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
      }
      else
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
          _image.Save(fileName, codecInfo, encoderParameters);
          result = fileName;
        }

        this.ApplySettings();
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
              MessageBox.Show(string.Format("Failed to save file. {0}", ex.GetBaseException().Message), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
          }
        }

        return null;
      });
    }

    private void SaveSettings()
    {
      _settings.Save();
    }

    private void SaveSettingsNowToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.SaveSettings();
    }

    private void SaveSettingsOnExitToolStripMenuItem_Click(object sender, EventArgs e)
    {
      saveSettingsOnExitToolStripMenuItem.Checked = !saveSettingsOnExitToolStripMenuItem.Checked;
    }

    private void ScanButton_Click(object sender, EventArgs e)
    {
      this.RunScanLoop((device, dialog) =>
      {
        Item item;

        item = device.Items[1];

        item.Properties.SetPropertyValue(WiaPropertyId.WIA_IPS_CUR_INTENT, _settings.ImageIntent);

        item.Properties.SetPropertyValue(WiaPropertyId.WIA_IPS_XRES, (int)dpiNumericUpDown.Value);
        item.Properties.SetPropertyValue(WiaPropertyId.WIA_IPS_YRES, (int)dpiNumericUpDown.Value);

        item.Properties.SetPropertyMaximum(WiaPropertyId.WIA_IPS_XEXTENT);
        item.Properties.SetPropertyMaximum(WiaPropertyId.WIA_IPS_YEXTENT);

        //PropertiesDialog.ShowPropertiesDialog(item.Properties);

        return dialog.ShowTransfer(item, _settings.FormatString, false);
      });
    }

    private void ScanPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.PerformDeviceAction(device =>
      {
        PropertiesDialog.ShowPropertiesDialog(device.Items[1].Properties);
      });
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

    private void SetDpi(int dpi)
    {
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
    }

    private void SetImage(ImageFile image)
    {
      formatToolStripStatusLabel.Text = image.FileExtension;

      this.SetImage(image.ToBitmap(), true);
    }

    private void SetImage(Bitmap image, bool resetZoom)
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

      sizeToolStripStatusLabel.Text = string.Format("{0} x {1}", image.Width, image.Height);

      if (resetZoom)
      {
        previewImageBox.ZoomToFit();
      }

      previewImageBox.EndUpdate();
    }

    private void SetIntent(WiaImageIntent imageIntent)
    {
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

    private void ShowPreviewToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.SetPreview(!_settings.ShowPreview);
    }

    private void TypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
      _settings.ImageIntent = typeComboBox.SelectedItem is KeyValueListBoxItem<WiaImageIntent> item ? item.Value : WiaImageIntent.UnspecifiedIntent;
    }

    private void UpdateUi()
    {
      bool directDevice;
      bool canScan;

      directDevice = !_settings.PromptForDevice;
      canScan = directDevice || deviceComboBox.SelectedIndex != -1;

      deviceComboBox.Enabled = directDevice;
      continuousCheckBox.Enabled = directDevice;
      devicePropertiesButton.Enabled = directDevice && canScan;
      scanButton.Enabled = canScan;
      previewButton.Enabled = canScan;

      if (!directDevice && _settings.ContinuousScan)
      {
        continuousCheckBox.Checked = false;
      }
    }

    private void UseCounterCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      _settings.UseCounter = useCounterCheckBox.Checked;
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