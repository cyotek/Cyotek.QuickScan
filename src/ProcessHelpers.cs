using System;
using System.Diagnostics;
using System.IO;

// Cyotek Quick Scan
// https://github.com/cyotek/Cyotek.QuickScan

// Copyright © 2019-2022 Cyotek Ltd.

// This work is licensed under the MIT License.
// See LICENSE.TXT for the full text

// Found this code useful?
// https://www.cyotek.com/contribute

namespace Cyotek.QuickScan
{
  internal static class ProcessHelpers
  {
    #region Public Methods

    public static void OpenFolderInExplorer(string folderName)
    {
      if (string.IsNullOrEmpty(folderName))
      {
        UiHelpers.ShowWarning("Folder not specified.");
      }
      else if (!Directory.Exists(folderName))
      {
        UiHelpers.ShowWarning(string.Format("Folder '{0}' does not exist.", folderName));
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
        UiHelpers.ShowError(string.Format("Failed to open {0}.", processName), ex);
      }

      return result;
    }

    #endregion Public Methods
  }
}