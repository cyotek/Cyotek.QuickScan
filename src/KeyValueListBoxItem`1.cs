// Cyotek Quick Scan
// https://github.com/cyotek/Cyotek.QuickScan

// Copyright © 2019-2022 Cyotek Ltd.

// This work is licensed under the MIT License.
// See LICENSE.TXT for the full text

// Found this code useful?
// https://www.cyotek.com/contribute

namespace Cyotek.QuickScan
{
  internal sealed class KeyValueListBoxItem<TValue>
  {
    #region Private Fields

    private string _name;

    private TValue _value;

    #endregion Private Fields

    #region Public Constructors

    public KeyValueListBoxItem(string name, TValue value)
    {
      _name = name;
      _value = value;
    }

    #endregion Public Constructors

    #region Public Properties

    public string Name
    {
      get => _name;
      set => _name = value;
    }

    public TValue Value
    {
      get => _value;
      set => _value = value;
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