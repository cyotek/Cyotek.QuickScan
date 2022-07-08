using System;
using System.Windows.Forms;

namespace Cyotek.QuickScan
{
  internal static class UiHelpers
  {
    #region Public Methods

    public static void ShowError(string message)
    {
      MessageBox.Show(message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    public static void ShowError(string message, Exception ex)
    {
      MessageBox.Show(message + " " + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    public static void ShowWarning(string message)
    {
      MessageBox.Show(message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
    }

    #endregion Public Methods
  }
}