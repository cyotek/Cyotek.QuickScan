﻿using System;
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
  internal partial class PropertiesDialog : BaseForm
  {
    #region Private Fields

    private readonly WIA.Properties _properties;

    #endregion Private Fields

    #region Public Constructors

    public PropertiesDialog()
    {
      InitializeComponent();
    }

    public PropertiesDialog(WIA.Properties properties)
      : this()
    {
      _properties = properties;
    }

    #endregion Public Constructors

    #region Private Enums

    private enum ColumnType
    {
      Name,

      Id,

      Value,

      Min,

      Max,

      Step,

      Values
    }

    #endregion Private Enums

    #region Public Methods

    public static void ShowPropertiesDialog(WIA.Properties properties)
    {
      using (PropertiesDialog dialog = new PropertiesDialog(properties))
      {
        dialog.ShowDialog();
      }
    }

    #endregion Public Methods

    #region Protected Methods

    protected override void OnShown(EventArgs e)
    {
      base.OnShown(e);

      if (_properties != null)
      {
        listView.BeginUpdate();

        for (int i = 0; i < _properties.Count; i++)
        {
          Property property;
          ListViewItem item;
          string value;
          WiaPropertyType type;

          property = _properties[i + 1];
          type = (WiaPropertyType)property.Type;

          try
          {
            value = property.GetValueString();
          }
          catch (Exception ex)
          {
            value = "{" + ex.Message + "}";
          }

          item = new ListViewItem();

          for (int j = 0; j < listView.Columns.Count; j++)
          {
            item.SubItems.Add(string.Empty);
          }

          item.SubItems[(int)ColumnType.Name].Text = property.Name;
          item.SubItems[(int)ColumnType.Id].Text = property.PropertyID.ToString();
          item.SubItems[(int)ColumnType.Value].Text = value;

          if (type > WiaPropertyType.UnsupportedPropertyType && type <= WiaPropertyType.CurrencyPropertyType)
          {
            if (property.SubType == WiaSubType.RangeSubType)
            {
              item.SubItems[(int)ColumnType.Min].Text = property.SubTypeMin.ToString();
              item.SubItems[(int)ColumnType.Max].Text = property.SubTypeMax.ToString();
              item.SubItems[(int)ColumnType.Step].Text = property.SubTypeStep.ToString();
            }
            else if (property.SubType == WiaSubType.ListSubType)
            {
              item.SubItems[(int)ColumnType.Values].Text = property.SubTypeValues.ToSeparatedString();
            }
          }

          listView.Items.Add(item);
        }

        listView.EndUpdate();
      }
    }

    #endregion Protected Methods

    #region Private Methods

    private void CloseButton_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    #endregion Private Methods
  }
}