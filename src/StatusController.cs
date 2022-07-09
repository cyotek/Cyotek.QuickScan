using System;
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
  internal sealed class StatusController : IDisposable
  {
    #region Private Fields

    private readonly ToolStripStatusLabel _label;

    #endregion Private Fields

    #region Public Constructors

    public StatusController(ToolStripStatusLabel label, string message)
    {
      Form owner;

      _label = label;

      label.Text = message;
      label.Owner.Refresh();

      owner = label.Owner.FindForm();
      owner.UseWaitCursor = true;

      Cursor.Current = Cursors.WaitCursor;
    }

    #endregion Public Constructors

    #region Public Methods

    public void Dispose()
    {
      if (_label != null)
      {
        Form owner;

        _label.Text = string.Empty;

        owner = _label.Owner.FindForm();
        owner.UseWaitCursor = false;

        Cursor.Current = Cursors.Default;
      }
    }

    #endregion Public Methods
  }
}