// Cyotek Quick Scan
// https://github.com/cyotek/Cyotek.QuickScan

// Copyright © 2022 Cyotek Ltd.

// This work is licensed under the MIT License.
// See LICENSE.TXT for the full text

// Found this code useful?
// https://www.cyotek.com/contribute

using Cyotek.QuickScan;
using System;
using System.Windows.Forms;

namespace restartservice
{
  internal static class Program
  {
    #region Private Methods

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
      if (MessageBox.Show("Are you sure you want to restart the \"Windows Image Aquisition (WIA)\" service?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
      {
        ServiceUtilities.RestartService(ServiceUtilities.WiaServiceName, ServiceUtilities.DefaultTimeOut);
      }
    }

    #endregion Private Methods
  }
}