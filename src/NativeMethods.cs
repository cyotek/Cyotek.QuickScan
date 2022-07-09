using System;
using System.Runtime.InteropServices;

// Cyotek Quick Scan
// https://github.com/cyotek/Cyotek.QuickScan

// Copyright © 2019-2022 Cyotek Ltd.

// This work is licensed under the MIT License.
// See LICENSE.TXT for the full text

// Found this code useful?
// https://www.cyotek.com/contribute

namespace Cyotek.QuickScan
{
  internal static class NativeMethods
  {
    #region Public Fields

    public const int MAX_PATH = 260;

    public const int SHGSI_ICON = 0x000000100;

    public const int SHGSI_SMALLICON = 0x000000001;

    public const int SIID_SHIELD = 77;

    #endregion Public Fields

    #region Public Methods

    [DllImport("user32.dll")]
    public static extern int DestroyIcon(IntPtr hIcon);

    [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
    public static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

    [DllImport("Shell32.dll", SetLastError = false)]
    public static extern int SHGetStockIconInfo(int siid, int uFlags, ref SHSTOCKICONINFO psii);

    #endregion Public Methods

    #region Public Structs

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct SHSTOCKICONINFO
    {
      public int cbSize;

      public IntPtr hIcon;

      public int iSysIconIndex;

      public int iIcon;

      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
      public string szPath;
    }

    #endregion Public Structs
  }
}