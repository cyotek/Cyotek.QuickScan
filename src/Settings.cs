using Cyotek.Data.Ini;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using WIA;

// Cyotek Quick Scan
// https://github.com/cyotek/Cyotek.QuickScan

// Copyright © 2019-2022 Cyotek Ltd.

// This work is licensed under the MIT License.
// See LICENSE.TXT for the full text

// Found this code useful?
// https://www.cyotek.com/contribute

namespace Cyotek.QuickScan
{
  internal class Settings
  {
    #region Private Fields

    private const string _defaultNextScanSound = "sounds\\gmae.wav";

    private bool _addMetadata;

    private bool _autoSave;

    private string _baseFileName;

    private bool _continuousScan;

    private int _counter;

    private string _deviceId;

    private bool _estimateFileSizes;

    private Guid _format;

    private bool _ignoreUpdates;

    private WiaImageIntent _imageIntent;

    private Orientation _layoutOrientation;

    private IDictionary<PropertyTag, Tuple<PropertyTagType, string>> _metadata;

    private string _nextScanSound;

    private int _optionsSplitterSize;

    private string _outputFolder;

    private bool _playSounds;

    private bool _promptForDevice;

    private int _quality;

    private bool _saveSettingsOnExit;

    private int _scanDpi;

    private bool _showPixelGrid;

    private bool _showPreview;

    private Unit _unit;

    private bool _useCounter;

    private string _windowPosition;

    #endregion Private Fields

    #region Public Constructors

    public Settings()
    {
      _quality = 97;
      _imageIntent = WiaImageIntent.ColorIntent;
      _format = WiaFormatId.Bmp;
      _counter = 1;
      _estimateFileSizes = true;
      _showPixelGrid = true;
      _layoutOrientation = Orientation.Vertical;
      _showPreview = true;
      _unit = Unit.Pixel;
      _metadata = new Dictionary<PropertyTag, Tuple<PropertyTagType, string>>();
      _saveSettingsOnExit = true;
      _nextScanSound = _defaultNextScanSound;
    }

    #endregion Public Constructors

    #region Public Properties

    public bool AddMetadata
    {
      get => _addMetadata;
      set => this.UpdateValue(ref _addMetadata, value);
    }

    public bool AutoSave
    {
      get => _autoSave;
      set => this.UpdateValue(ref _autoSave, value);
    }

    public string BaseFileName
    {
      get => _baseFileName;
      set => this.UpdateValue(ref _baseFileName, value);
    }

    public bool ContinuousScan
    {
      get => _continuousScan;
      set => this.UpdateValue(ref _continuousScan, value);
    }

    public int Counter
    {
      get => _counter;
      set => this.UpdateValue(ref _counter, value);
    }

    public string DeviceId
    {
      get => _deviceId;
      set => this.UpdateValue(ref _deviceId, value);
    }

    public bool EstimateFileSizes
    {
      get => _estimateFileSizes;
      set => this.UpdateValue(ref _estimateFileSizes, value);
    }

    public Guid Format
    {
      get => _format;
      set => this.UpdateValue(ref _format, value);
    }

    public string FormatString =>
      // WIA throws a COM exception if the GUID isn't formatted just-so
      _format.ToString("B");

    public bool IgnoreUpdates
    {
      get => _ignoreUpdates;
      set => _ignoreUpdates = value;
    }

    public WiaImageIntent ImageIntent
    {
      get => _imageIntent;
      set => this.UpdateValue(ref _imageIntent, value);
    }

    public Orientation LayoutOrientation
    {
      get => _layoutOrientation;
      set => this.UpdateValue(ref _layoutOrientation, value);
    }

    public IDictionary<PropertyTag, Tuple<PropertyTagType, string>> Metadata => _metadata;

    public string NextScanSound
    {
      get => _nextScanSound;
      set => this.UpdateValue(ref _nextScanSound, value);
    }

    public int OptionsSplitterSize
    {
      get => _optionsSplitterSize;
      set => this.UpdateValue(ref _optionsSplitterSize, value);
    }

    public string OutputFolder
    {
      get => _outputFolder;
      set => this.UpdateValue(ref _outputFolder, value);
    }

    public bool PlaySounds
    {
      get => _playSounds;
      set => this.UpdateValue(ref _playSounds, value);
    }

    public bool PromptForDevice
    {
      get => _promptForDevice;
      set => this.UpdateValue(ref _promptForDevice, value);
    }

    public int Quality
    {
      get => _quality;
      set => this.UpdateValue(ref _quality, value);
    }

    public bool SaveSettingsOnExit
    {
      get => _saveSettingsOnExit;
      set => this.UpdateValue(ref _saveSettingsOnExit, value);
    }

    public int ScanDpi
    {
      get => _scanDpi;
      set => this.UpdateValue(ref _scanDpi, value);
    }

    public bool ShowPixelGrid
    {
      get => _showPixelGrid;
      set => this.UpdateValue(ref _showPixelGrid, value);
    }

    public bool ShowPreview
    {
      get => _showPreview;
      set => this.UpdateValue(ref _showPreview, value);
    }

    public Unit Unit
    {
      get => _unit;
      set => this.UpdateValue(ref _unit, value);
    }

    public bool UseCounter
    {
      get => _useCounter;
      set => this.UpdateValue(ref _useCounter, value);
    }

    public string WindowPosition
    {
      get => _windowPosition;
      set => this.UpdateValue(ref _windowPosition, value);
    }

    #endregion Public Properties

    #region Private Properties

    private string DefaultIniFileName => Path.ChangeExtension(Application.ExecutablePath, "default.ini");

    private string IniFileName => Path.ChangeExtension(Application.ExecutablePath, "ini");

    #endregion Private Properties

    #region Public Methods

    public void Load()
    {
      string fileName;

      fileName = this.GetLoadFileName();

      if (File.Exists(fileName))
      {
        IniDocument data;
        IniSectionToken settings;

        data = new IniDocument(fileName);

        settings = (IniSectionToken)data.CreateSection("Settings");
        this.ReadBool(ref _estimateFileSizes, settings["EstimateFileSizes"]);
        this.ReadBool(ref _saveSettingsOnExit, settings[nameof(this.SaveSettingsOnExit)]);

        settings = (IniSectionToken)data.CreateSection("Device");
        _deviceId = settings.GetValue("DeviceId");
        this.ReadBool(ref _promptForDevice, settings["Prompt"]);
        this.ReadBool(ref _continuousScan, settings["Continuous"]);

        settings = (IniSectionToken)data.CreateSection("Scan");
        this.ReadEnum(ref _imageIntent, settings["Intent"]);
        this.ReadInt(ref _scanDpi, settings["Dpi"]);

        settings = (IniSectionToken)data.CreateSection("Output");
        this.ReadGuid(ref _format, settings["Format"]);
        this.ReadInt(ref _quality, settings["Quality"]);
        _outputFolder = settings["Folder"];
        _baseFileName = settings["FileName"];
        this.ReadInt(ref _counter, settings["Counter"]);
        this.ReadBool(ref _useCounter, settings["UseCounter"]);
        this.ReadBool(ref _autoSave, settings["AutoSave"]);
        this.ReadBool(ref _addMetadata, settings["AddMetaData"]);

        settings = (IniSectionToken)data.CreateSection("UI");
        this.ReadBool(ref _showPreview, settings["Preview"]);
        this.ReadBool(ref _showPixelGrid, settings["PixelGrid"]);
        this.ReadEnum(ref _layoutOrientation, settings["Orientation"]);
        this.ReadEnum(ref _unit, settings["Unit"]);

        settings = this.CreateMachineSection(data, "UI");
        this.ReadInt(ref _optionsSplitterSize, settings[nameof(this.OptionsSplitterSize)]);
        _windowPosition = settings[nameof(this.WindowPosition)];

        settings = (IniSectionToken)data.CreateSection("Sounds");
        this.ReadBool(ref _playSounds, settings[nameof(this.PlaySounds)]);
        _nextScanSound = settings.GetValue(nameof(this.NextScanSound), _defaultNextScanSound);

        this.LoadMetadata(data);
      }
    }

    public void Save()
    {
      IniDocument data;
      IniSectionToken settings;

      data = new IniDocument(this.GetLoadFileName());

      settings = (IniSectionToken)data.CreateSection("Settings");
      settings["EstimateFileSizes"] = _estimateFileSizes.ToString(CultureInfo.InvariantCulture);
      settings[nameof(this.SaveSettingsOnExit)] = _saveSettingsOnExit.ToString(CultureInfo.InvariantCulture);

      settings = (IniSectionToken)data.CreateSection("Device");
      settings["DeviceId"] = _deviceId;
      settings["Prompt"] = _promptForDevice.ToString(CultureInfo.InvariantCulture);
      settings["Continuous"] = _continuousScan.ToString(CultureInfo.InvariantCulture);

      settings = (IniSectionToken)data.CreateSection("Scan");
      settings["Intent"] = _imageIntent.ToString();
      settings["Dpi"] = _scanDpi.ToString(CultureInfo.InvariantCulture);

      settings = (IniSectionToken)data.CreateSection("Output");
      settings["Format"] = _format.ToString();
      settings["Quality"] = _quality.ToString(CultureInfo.InvariantCulture);
      settings["Folder"] = _outputFolder;
      settings["FileName"] = _baseFileName;
      settings["Counter"] = _counter.ToString(CultureInfo.InvariantCulture);
      settings["UseCounter"] = _useCounter.ToString(CultureInfo.InvariantCulture);
      settings["AutoSave"] = _autoSave.ToString(CultureInfo.InvariantCulture);

      settings = (IniSectionToken)data.CreateSection("UI");
      settings["Unit"] = _unit.ToString();
      settings["Preview"] = _showPreview.ToString(CultureInfo.InvariantCulture);
      settings["PixelGrid"] = _showPixelGrid.ToString(CultureInfo.InvariantCulture);
      settings["Orientation"] = _layoutOrientation.ToString();

      settings = this.CreateMachineSection(data, "UI");
      settings[nameof(this.OptionsSplitterSize)] = _optionsSplitterSize.ToString(CultureInfo.InvariantCulture);
      settings[nameof(this.WindowPosition)] = _windowPosition;

      settings = (IniSectionToken)data.CreateSection("Sounds");
      settings[nameof(this.PlaySounds)] = _playSounds.ToString();

      data.Save(this.IniFileName);
    }

    #endregion Public Methods

    #region Private Methods

    private IniSectionToken CreateMachineSection(IniDocument document, string name)
    {
      name = Environment.MachineName + "-" + name;

      return (IniSectionToken)document.CreateSection(name);
    }

    private string GetLoadFileName()
    {
      string fileName;

      fileName = this.IniFileName;

      if (!File.Exists(fileName))
      {
        // custom file doesn't exist, check for a default
        fileName = this.DefaultIniFileName;
      }

      return fileName;
    }

    private void LoadMetadata(IniDocument data)
    {
      IniSectionToken settings;

      _metadata.Clear();

      settings = (IniSectionToken)data.CreateSection("Metadata");

      foreach (IniToken token in settings.ChildTokens)
      {
        if (token is IniValueToken setting && !string.IsNullOrEmpty(setting.Name) && !string.IsNullOrEmpty(setting.Value))
        {
          int typePos;
          PropertyTag tag;
          PropertyTagType type;
          string value;

          typePos = setting.Value.IndexOf(',');
          tag = (PropertyTag)Enum.Parse(typeof(PropertyTag), setting.Name, true);
          type = (PropertyTagType)Enum.Parse(typeof(PropertyTagType), setting.Value.Substring(0, typePos), true);
          value = setting.Value.Substring(typePos + 1);

          _metadata.Add(tag, Tuple.Create(type, value));
        }
      }
    }

    private void ReadBool(ref bool setting, string value)
    {
      if (!string.IsNullOrEmpty(value))
      {
        setting = Convert.ToBoolean(value);
      }
    }

    private void ReadEnum<T>(ref T setting, string value)
              where T : struct
    {
      if (!string.IsNullOrEmpty(value))
      {
        setting = (T)Enum.Parse(typeof(T), value, true);
      }
    }

    private void ReadGuid(ref Guid setting, string value)
    {
      if (!string.IsNullOrEmpty(value))
      {
        setting = new Guid(value);
      }
    }

    private void ReadInt(ref int setting, string value)
    {
      if (!string.IsNullOrEmpty(value))
      {
        setting = Convert.ToInt32(value);
      }
    }

    private void UpdateValue<T>(ref T field, T value)
    {
      if (!_ignoreUpdates && !EqualityComparer<T>.Default.Equals(field, value))
      {
        field = value;
      }
    }

    #endregion Private Methods
  }
}