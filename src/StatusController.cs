using System;
using System.Windows.Forms;

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