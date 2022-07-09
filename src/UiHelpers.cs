using System;
using System.Globalization;
using System.Windows.Forms;

// Cyotek Quick Scan
// https://github.com/cyotek/Cyotek.QuickScan

// Copyright © 2022 Cyotek Ltd.

// This work is licensed under the MIT License.
// See LICENSE.TXT for the full text

// Found this code useful?
// https://www.cyotek.com/contribute

namespace Cyotek.QuickScan
{
  internal static class UiHelpers
  {
    #region Private Fields

    private static readonly char[] _separators = { ',' };

    #endregion Private Fields

    #region Public Methods

    public static void ApplyWindowPosition(Form form, string position)
    {
      if (!string.IsNullOrWhiteSpace(position))
      {
        string[] parts;

        parts = position.Split(_separators);

        if (parts.Length >= 4)
        {
          if (int.TryParse(parts[0], out int x)
              && int.TryParse(parts[1], out int y)
              && int.TryParse(parts[2], out int w)
              && int.TryParse(parts[3], out int h)
              && int.TryParse(parts[4], out int state)
             )
          {
            try
            {
              form.SetBounds(x, y, w, h);

              if (state == 0 || state == 2)
              {
                form.WindowState = (FormWindowState)state;
              }
            }
            catch
            {
              // don't care
            }
          }
        }
      }
    }

    public static string GetWindowPosition(Form form)
    {
      // TODO: This will return wrong values for maximized windows, need to dig
      // out the interop code from Cyotek.Win32
      return string.Format("{0}, {1}, {2}, {3}, {4}",
        form.Left.ToString(CultureInfo.InvariantCulture),
        form.Top.ToString(CultureInfo.InvariantCulture),
        form.Width.ToString(CultureInfo.InvariantCulture),
        form.Height.ToString(CultureInfo.InvariantCulture),
        ((int)form.WindowState).ToString(CultureInfo.InvariantCulture));
    }

    public static void ShowError(string message)
    {
      MessageBox.Show(message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    public static void ShowError(string message, Exception ex)
    {
      MessageBox.Show(message + " " + ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    public static void ShowWarning(string message)
    {
      MessageBox.Show(message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
    }

    #endregion Public Methods
  }
}