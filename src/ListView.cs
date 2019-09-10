using System;

namespace Cyotek.QuickScan
{
  internal class ListView : System.Windows.Forms.ListView
  {
    #region Protected Methods

    protected override void OnHandleCreated(EventArgs e)
    {
      base.OnHandleCreated(e);

      if (!this.IsDesignTime() && Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6)
      {
        NativeMethods.SetWindowTheme(this.Handle, "explorer", null);
      }
    }

    #endregion Protected Methods
  }
}