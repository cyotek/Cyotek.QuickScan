// Cyotek Quick Scan
// https://github.com/cyotek/Cyotek.QuickScan

// Copyright © 2022 Cyotek Ltd.

// This work is licensed under the MIT License.
// See LICENSE.TXT for the full text

// Found this code useful?
// https://www.cyotek.com/contribute

using System;
using System.ServiceProcess;
using System.Windows.Forms;

namespace Cyotek.QuickScan
{
  internal static class ServiceUtilities
  {
    #region Public Fields

    public static readonly TimeSpan DefaultTimeOut = TimeSpan.FromSeconds(10);

    public static readonly string WiaServiceName = "stisvc";

    #endregion Public Fields

    #region Public Methods

    public static void RestartService(string serviceName, TimeSpan timeout)
    {
      try
      {
        using (ServiceController serviceController = new ServiceController(serviceName))
        {
          int currentTicks;
          int updatedTicks;

          currentTicks = Environment.TickCount;

          if (serviceController.Status != ServiceControllerStatus.Stopped)
          {
            serviceController.Stop();
            serviceController.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
          }

          updatedTicks = Environment.TickCount;

          timeout = TimeSpan.FromMilliseconds(timeout.TotalMilliseconds - (updatedTicks - currentTicks));

          serviceController.Start();
          serviceController.WaitForStatus(ServiceControllerStatus.Running, timeout);
        }

        MessageBox.Show("Service successfully restarted.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
      catch (Exception ex)
      {
        MessageBox.Show(string.Format("Failed to restart service. {0}", ex.Message), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    public static void StartService(string serviceName, TimeSpan timeout)
    {
      try
      {
        using (ServiceController serviceController = new ServiceController(serviceName))
        {
          serviceController.Start();
          serviceController.WaitForStatus(ServiceControllerStatus.Running, timeout);
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(string.Format("Failed to start service. {0}", ex.Message), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    public static void StopService(string serviceName, TimeSpan timeout)
    {
      try
      {
        using (ServiceController serviceController = new ServiceController(serviceName))
        {
          serviceController.Stop();
          serviceController.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(string.Format("Failed to stop service. {0}", ex.Message), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    #endregion Public Methods
  }
}