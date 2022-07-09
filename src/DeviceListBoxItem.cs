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
  internal sealed class DeviceListBoxItem
  {
    #region Private Fields

    private string _deviceId;

    private string _name;

    #endregion Private Fields

    #region Public Constructors

    public DeviceListBoxItem(IDeviceInfo device)
    {
      _deviceId = device.DeviceID;
      _name = (string)device.Properties["Name"].get_Value();
    }

    #endregion Public Constructors

    #region Public Properties

    public string DeviceId
    {
      get => _deviceId;
      set => _deviceId = value;
    }

    public string Name
    {
      get => _name;
      set => _name = value;
    }

    #endregion Public Properties

    #region Public Methods

    public override string ToString()
    {
      return _name;
    }

    #endregion Public Methods
  }
}