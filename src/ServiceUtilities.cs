using System;
using System.ServiceProcess;

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
      }
      catch (Exception ex)
      {
        UiHelpers.ShowError("Failed to restart service.", ex);
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
        UiHelpers.ShowError("Failed to start service.", ex);
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
        UiHelpers.ShowError("Failed to stop service.", ex);
      }
    }

    #endregion Public Methods
  }
}