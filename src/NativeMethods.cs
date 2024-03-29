using System;
using System.Runtime.InteropServices;

// Cyotek Quick Scan
// https://github.com/cyotek/Cyotek.QuickScan

// Copyright © 2019-2024 Cyotek Ltd.

// This work is licensed under the MIT License.
// See LICENSE.TXT for the full text

// Found this code useful?
// https://www.cyotek.com/contribute

namespace Cyotek.QuickScan
{
  internal static class NativeMethods
  {
    #region Public Fields

    public const int BM_CLICK = 0xF5;

    public const int GW_OWNER = 4;

    public const int MAX_PATH = 260;

    public const int SHGSI_ICON = 0x000000100;

    public const int SHGSI_SMALLICON = 0x000000001;

    public const int SIID_SHIELD = 77;

    public const int SWP_NOACTIVATE = 0X10;

    public const int SWP_NOZORDER = 0X4;

    #endregion Public Fields

    #region Public Methods

    [DllImport("user32.dll")]
    public static extern int DestroyIcon(IntPtr hIcon);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr GetWindow(IntPtr hWnd, int wCmd);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    public static extern IntPtr SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);

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