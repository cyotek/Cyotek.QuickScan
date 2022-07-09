using Cyotek.QuickScan;
using Cyotek.Windows.Forms;
using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows.Forms;

namespace Cyotek.Tools.DuplicateFileFinder
{
  internal static class ElevationHelper
  {
    #region Public Properties

    public static bool IsElevated => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

    #endregion Public Properties

    #region Public Methods

    public static void Elevate() => ElevationHelper.Elevate(string.Empty);

    public static void Elevate(string arguments)
    {
      ProcessStartInfo startInfo;

      startInfo = new ProcessStartInfo
      {
        FileName = Application.ExecutablePath,
        Arguments = arguments,
        UseShellExecute = true,
        Verb = "runas"
      };

      try
      {
        Process.Start(startInfo);
        Application.Exit();
      }
      catch (Exception ex)
      {
        UiHelpers.ShowError("Failed to elevate process.", ex);
      }
    }

    #endregion Public Methods
  }
}