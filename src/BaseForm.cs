using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

// Cyotek Quick Scan
// https://github.com/cyotek/Cyotek.QuickScan

// Copyright © 2019-2022 Cyotek Ltd.

// This work is licensed under the MIT License.
// See LICENSE.TXT for the full text

// Found this code useful?
// https://www.cyotek.com/contribute

namespace Cyotek.QuickScan
{
  internal class BaseForm : System.Windows.Forms.Form
  {
    #region Protected Constructors

    protected BaseForm()
    {
      base.MaximizeBox = false;
      base.MinimizeBox = false;
      base.ShowIcon = false;
      base.ShowInTaskbar = false;
      base.StartPosition = FormStartPosition.CenterParent;
      base.FormBorderStyle = FormBorderStyle.FixedDialog;
    }

    #endregion Protected Constructors

    #region Public Properties

    [DefaultValue(typeof(FormBorderStyle), "FixedDialog")]
    public new FormBorderStyle FormBorderStyle
    {
      get => base.FormBorderStyle;
      set => base.FormBorderStyle = value;
    }

    [DefaultValue(false)]
    public new bool MaximizeBox
    {
      get => base.MaximizeBox;
      set => base.MaximizeBox = value;
    }

    [DefaultValue(false)]
    public new bool MinimizeBox
    {
      get => base.MinimizeBox;
      set => base.MinimizeBox = value;
    }

    [DefaultValue(false)]
    public new bool ShowIcon
    {
      get => base.ShowIcon;
      set => base.ShowIcon = value;
    }

    [DefaultValue(false)]
    public new bool ShowInTaskbar
    {
      get => base.ShowInTaskbar;
      set => base.ShowInTaskbar = value;
    }

    [DefaultValue(typeof(FormStartPosition), "CenterParent")]
    public new FormStartPosition StartPosition
    {
      get => base.StartPosition;
      set => base.StartPosition = value;
    }

    #endregion Public Properties

    #region Protected Methods

    protected override void OnLoad(EventArgs e)
    {
      if (!this.DesignMode)
      {
        this.Font = SystemFonts.MessageBoxFont;
      }

      base.OnLoad(e);
    }

    protected override void OnShown(EventArgs e)
    {
      base.OnShown(e);

      Cursor.Current = Cursors.Default;
    }

    #endregion Protected Methods
  }
}