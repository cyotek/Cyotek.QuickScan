using Cyotek.Data.Ini;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using WIA;

namespace Cyotek.QuickScan
{
  internal class Settings
  {
    #region Private Fields

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

    private string _outputFolder;

    private bool _promptForDevice;

    private int _quality;

    private int _scanDpi;

    private bool _showPixelGrid;

    private bool _showPreview;

    private Unit _unit;

    private bool _useCounter;

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
      get { return _autoSave; }
      set { this.UpdateValue(ref _autoSave, value); }
    }

    public string BaseFileName
    {
      get { return _baseFileName; }
      set { this.UpdateValue(ref _baseFileName, value); }
    }

    public bool ContinuousScan
    {
      get { return _continuousScan; }
      set { this.UpdateValue(ref _continuousScan, value); }
    }

    public int Counter
    {
      get { return _counter; }
      set { this.UpdateValue(ref _counter, value); }
    }

    public string DeviceId
    {
      get { return _deviceId; }
      set { this.UpdateValue(ref _deviceId, value); }
    }

    public bool EstimateFileSizes
    {
      get { return _estimateFileSizes; }
      set { this.UpdateValue(ref _estimateFileSizes, value); }
    }

    public Guid Format
    {
      get { return _format; }
      set { this.UpdateValue(ref _format, value); }
    }

    public string FormatString
    {
      get
      {
        // WIA throws a COM exception if the GUID isn't formatted just-so
        return _format.ToString("B");
      }
    }

    public bool IgnoreUpdates
    {
      get { return _ignoreUpdates; }
      set { _ignoreUpdates = value; }
    }

    public WiaImageIntent ImageIntent
    {
      get { return _imageIntent; }
      set { this.UpdateValue(ref _imageIntent, value); }
    }

    public Orientation LayoutOrientation
    {
      get { return _layoutOrientation; }
      set { this.UpdateValue(ref _layoutOrientation, value); }
    }

    public IDictionary<PropertyTag, Tuple<PropertyTagType, string>> Metadata => _metadata;

    public string OutputFolder
    {
      get { return _outputFolder; }
      set { this.UpdateValue(ref _outputFolder, value); }
    }

    public bool PromptForDevice
    {
      get { return _promptForDevice; }
      set { this.UpdateValue(ref _promptForDevice, value); }
    }

    public int Quality
    {
      get { return _quality; }
      set { this.UpdateValue(ref _quality, value); }
    }

    public int ScanDpi
    {
      get { return _scanDpi; }
      set { this.UpdateValue(ref _scanDpi, value); }
    }

    public bool ShowPixelGrid
    {
      get { return _showPixelGrid; }
      set { this.UpdateValue(ref _showPixelGrid, value); }
    }

    public bool ShowPreview
    {
      get { return _showPreview; }
      set { this.UpdateValue(ref _showPreview, value); }
    }

    public Unit Unit
    {
      get { return _unit; }
      set { this.UpdateValue(ref _unit, value); }
    }

    public bool UseCounter
    {
      get { return _useCounter; }
      set { this.UpdateValue(ref _useCounter, value); }
    }

    #endregion Public Properties

    #region Private Properties

    private string IniFileName
    {
      get
      {
        return Path.ChangeExtension(Application.ExecutablePath, "ini");
      }
    }

    #endregion Private Properties

    #region Public Methods

    public void Load()
    {
      string fileName;

      fileName = this.IniFileName;

      if (File.Exists(fileName))
      {
        IniDocument data;
        IniSectionToken settings;

        data = new IniDocument(fileName);

        settings = (IniSectionToken)data.CreateSection("Settings");
        this.ReadBool(ref _estimateFileSizes, settings["EstimateFileSizes"]);

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

        this.LoadMetadata(data);
      }
    }

    public void Save()
    {
      IniDocument data;
      IniSectionToken settings;
      string fileName;

      fileName = this.IniFileName;

      data = new IniDocument(fileName);

      settings = (IniSectionToken)data.CreateSection("Settings");
      settings["EstimateFileSizes"] = _estimateFileSizes.ToString();

      settings = (IniSectionToken)data.CreateSection("Device");
      settings["DeviceId"] = _deviceId;
      settings["Prompt"] = _promptForDevice.ToString();
      settings["Continuous"] = _continuousScan.ToString();

      settings = (IniSectionToken)data.CreateSection("Scan");
      settings["Intent"] = _imageIntent.ToString();
      settings["Dpi"] = _scanDpi.ToString();

      settings = (IniSectionToken)data.CreateSection("Output");
      settings["Format"] = _format.ToString();
      settings["Quality"] = _quality.ToString();
      settings["Folder"] = _outputFolder;
      settings["FileName"] = _baseFileName;
      settings["Counter"] = _counter.ToString();
      settings["UseCounter"] = _useCounter.ToString();
      settings["AutoSave"] = _autoSave.ToString();

      settings = (IniSectionToken)data.CreateSection("UI");
      settings["Unit"] = _unit.ToString();
      settings["Preview"] = _showPreview.ToString();
      settings["PixelGrid"] = _showPixelGrid.ToString();
      settings["Orientation"] = _layoutOrientation.ToString();

      data.Save();
    }

    #endregion Public Methods

    #region Private Methods

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