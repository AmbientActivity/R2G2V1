using Keebee.AAT.SystemEventLogging;
using System;
using System.Configuration.Install;
using System.Diagnostics;
using System.ServiceProcess;
using Keebee.AAT.Shared;
using System.Linq;

namespace Keebee.AAT.BusinessRules
{
    public class UtilityRules
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

        public string RestartServices()
        {
            try
            {
                var millisec1 = Environment.TickCount;
                var timeout = TimeSpan.FromMilliseconds(60000 * millisec1);

                // state machine
                var service = new ServiceController(ServiceName.StateMachine);

                if (service.CanStop)
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                }

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);

                // wait for the duplicate to disappear
                while (StateMachineIsMultiple()) { }

                // state machine
                service = new ServiceController(ServiceName.RfidReader);

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);

                service = new ServiceController(ServiceName.Phidget);

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"RestartService: {ex.Message}", EventLogEntryType.Error);
                return ex.Message;
            }

            return string.Empty;
        }

        public string ReinstallServices()
        {
            var servicePath = new ServicesPath();
            var exePathSms = servicePath.StateMachine;
            var exePathPhidget = servicePath.Phidget;
            var exePathRfidReader = servicePath.RfidReader;

            try
            {
                var msg = ServiceInstaller.Uninstall(exePathRfidReader);

                if (msg.Length == 0)
                    msg = ServiceInstaller.Uninstall(exePathPhidget);

                if (msg.Length == 0)
                    ServiceInstaller.Uninstall(exePathSms);

                if (msg.Length == 0)
                    msg = ServiceInstaller.Install(exePathSms);

                if (msg.Length == 0)
                    ServiceInstaller.Install(exePathPhidget);

                if (msg.Length == 0)
                    ServiceInstaller.Install(exePathRfidReader);

                // wait for the duplicate to disappear
                while (StateMachineIsMultiple()) { }

                return msg;
            }

            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"ReinstallServices: {ex.Message}", EventLogEntryType.Error);
                return ex.Message;
            }
        }

        private static bool StateMachineIsMultiple()
        {
            var processes = Process.GetProcessesByName("Keebee.AAT.StateMachineService");
            return (processes.Count() > 1);
        }

        private static class ServiceInstaller
        {

            public static string Install(string exePath)
            {
                try
                {
                    ManagedInstallerClass.InstallHelper(
                        new string[] { exePath });
                }
                catch(Exception ex)
                {
                    return ex.Message;
                }
                return string.Empty;
            }

            public static string Uninstall(string exePath)
            {
                try
                {
                    ManagedInstallerClass.InstallHelper(
                        new string[] { "/u", exePath });
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
                return string.Empty;
            }
        }
    }
}
