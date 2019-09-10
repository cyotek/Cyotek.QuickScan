using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cyotek.QuickScan
{
  internal static class ControlExtensions
  {
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
  }
}
