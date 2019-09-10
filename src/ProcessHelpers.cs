using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Cyotek.QuickScan
{
  internal static class ProcessHelpers
  {
    #region Public Methods

    public static void OpenFolderInExplorer(string folderName)
    {
      if (string.IsNullOrEmpty(folderName))
      {
        MessageBox.Show("Folder not specified.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
      }
      else if (!Directory.Exists(folderName))
      {
        MessageBox.Show(string.Format("Folder '{0}' does not exist.", folderName), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
      }
      else
      {
        ProcessHelpers.StartProcess(string.Format("{0}\\explorer.exe", Environment.ExpandEnvironmentVariables("%windir%")), string.Format("/n,\"{0}\"", folderName));
      }
    }

    public static bool StartProcess(string processName, string arguments)
    {
      return StartProcess(processName, arguments, false);
    }

    public static bool StartProcess(string processName, string arguments, bool waitForExit)
    {
      bool result;

      try
      {
        ProcessStartInfo info;
        Process process;

        info = new ProcessStartInfo();
        info.FileName = processName;
        info.Arguments = arguments;

        process = new Process();
        process.StartInfo = info;

        process.Start();
        if (waitForExit)
        {
          process.WaitForExit();
        }

        result = true;
      }
      catch (Exception ex)
      {
        result = false;
        MessageBox.Show(string.Format("Failed to open {0}. {1}", processName, ex.GetBaseException().Message), "Open", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
      }

      return result;
    }

    #endregion Public Methods
  }
}