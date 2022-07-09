using System;
using System.Windows.Forms;

// Cyotek Quick Scan
// https://github.com/cyotek/Cyotek.QuickScan

// Copyright © 2022 Cyotek Ltd.

// This work is licensed under the MIT License.
// See LICENSE.TXT for the full text

// Found this code useful?
// https://www.cyotek.com/contribute

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