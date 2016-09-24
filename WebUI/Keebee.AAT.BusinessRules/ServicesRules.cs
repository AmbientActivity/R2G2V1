using Keebee.AAT.SystemEventLogging;
using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace Keebee.AAT.BusinessRules
{
    public class ServicesRules
    {
        private SystemEventLogger _systemEventLogger;
        public SystemEventLogger EventLogger
        {
            set { _systemEventLogger = value; }
        }

        public void StartService(string serviceName, int timeoutMilliseconds)
        {
            var service = new ServiceController(serviceName);
            try
            {
                var timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"StartService: {ex.Message}", EventLogEntryType.Error);
            }
        }

        public void StopService(string serviceName, int timeoutMilliseconds)
        {
            var service = new ServiceController(serviceName);
            try
            {
                var timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"StopService: {ex.Message}", EventLogEntryType.Error);
            }
        }

        public void RestartService(string serviceName, int timeoutMilliseconds)
        {
            var service = new ServiceController(serviceName);
            try
            {
                var millisec1 = Environment.TickCount;
                var timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                // count the rest of the timeout
                var millisec2 = Environment.TickCount;
                timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec2 - millisec1));

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"RestartService: {ex.Message}", EventLogEntryType.Error);
            }
        }
    }
}
