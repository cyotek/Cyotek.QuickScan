using System;
using System.Runtime.InteropServices;

namespace Cyotek.QuickScan
{
  internal static class NativeMethods
  {
    #region Public Methods

    [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
    public extern static int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

    #endregion Public Methods
  }
}