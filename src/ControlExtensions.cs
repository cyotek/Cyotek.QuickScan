using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

// Cyotek Quick Scan
// https://github.com/cyotek/Cyotek.QuickScan

// Copyright © 2019-2022 Cyotek Ltd.

// This work is licensed under the MIT License.
// See LICENSE.TXT for the full text

// Found this code useful?
// https://www.cyotek.com/contribute

namespace Cyotek.QuickScan
{
  internal static class ControlExtensions
  {
    #region Public Methods

    [DebuggerStepThrough]
    public static bool IsDesignTime(this Control target)
    {
      bool result;

      if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
      {
        result = true;
      }
      else
      {
        Control control;

        result = false;
        control = target;

        while (control != null)
        {
          if (control.Site != null && control.Site.DesignMode)
          {
            result = true;
            break;
          }

          control = control.Parent;
        }
      }

      return result;
    }

    #endregion Public Methods
  }
}