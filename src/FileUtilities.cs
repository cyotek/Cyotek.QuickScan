using System.IO;

// Cyotek Quick Scan
// https://github.com/cyotek/Cyotek.QuickScan

// Copyright © 2022 Cyotek Ltd.

// This work is licensed under the MIT License.
// See LICENSE.TXT for the full text

// Found this code useful?
// https://www.cyotek.com/contribute

namespace Cyotek.QuickScan
{
  internal static class FileUtilities
  {
    #region Public Methods

    public static void DeleteFile(string fileName)
    {
      if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
      {
        File.Delete(fileName);
      }
    }

    #endregion Public Methods
  }
}