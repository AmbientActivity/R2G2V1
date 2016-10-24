using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Shared;
using Keebee.AAT.RESTClient;
using Keebee.AAT.MessageQueuing;
using System;
using System.Configuration.Install;
using System.Diagnostics;
using System.ServiceProcess;
using System.Linq;

namespace Keebee.AAT.BusinessRules
{
    public class MaintenanceRules
    {
        private OperationsClient _opsClient;
        public OperationsClient OperationsClient
        {
            set { _opsClient = value; }
        }

        private CustomMessageQueue _messageQueuePhidget;
        public CustomMessageQueue MsssageQueuePhidget
        {
            set { _messageQueuePhidget = value; }
        }

        private SystemEventLogger _systemEventLogger;
        public SystemEventLogger EventLogger
        {
            set { _systemEventLogger = value; }
        }

        public string RestartServices()
        {
            try
            {
                // state machine
                var service = new ServiceController(ServiceName.StateMachine);

                if (service.CanStop)
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped);
                    while (StateMachineIsInstalled()) { }
                }

                // rfid reader
                service = new ServiceController(ServiceName.RfidReader);

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running);

                // phidget
                service = new ServiceController(ServiceName.Phidget);

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running);

                // video
                service = new ServiceController(ServiceName.VideoCapture);

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"RestartService: {ex.Message}", EventLogEntryType.Error);
                return ex.Message;
            }

            return null;
        }

        public void KillDisplay()
        {
            var configDetails = _opsClient.GetActiveConfigDetails().ConfigDetails;
            var killDetails = configDetails.Where(c => c.ResponseType.Id == ResponseTypeId.KillDisplay);

            if (killDetails.Any())
            {
                var sensorId = killDetails.First().PhidgetType.Id - 1;
                _messageQueuePhidget.Send(string.Format("{0}\"SensorId\":{1},\"SensorValue\":{2}{3}", "{", sensorId, 999, "}"));
            }
        }

        public string ReinstallServices(string smsPath, string phidgetPath, string rfidPath, string videoCapturPath, string keepIISAlivePath)
        {
            var exePathSms = $@"{smsPath}\{ServiceName.StateMachineExe}";
            var exePathPhidget = $@"{phidgetPath}\{ServiceName.PhidgetExe}";
            var exePathRfidReader = $@"{rfidPath}\{ServiceName.RfidReaderExe}";
            var exePathVideoCapture = $@"{videoCapturPath}\{ServiceName.VideoCaptureExe}";
            var exePathKeepIISAlive = $@"{keepIISAlivePath}\{ServiceName.KeepIISAliveExe}";

            try
            {
                // uninstall
                var msg = ServiceInstaller.Uninstall(exePathKeepIISAlive)
                          ?? ServiceInstaller.Uninstall(exePathRfidReader)
                          ?? ServiceInstaller.Uninstall(exePathPhidget)
                          ?? ServiceInstaller.Uninstall(exePathVideoCapture);

                if (msg != null) return msg;

                msg = ServiceInstaller.Uninstall(exePathSms);

                // wait for these to completely uninstall
                while (StateMachineIsInstalled()) { }

                while (KeepIISAliveIsInstalled()) { }

                // install
                if (msg == null)
                    msg = ServiceInstaller.Install(exePathSms);

                if (msg == null)
                    msg = ServiceInstaller.Install(exePathPhidget);

                if (msg == null)
                    msg = ServiceInstaller.Install(exePathVideoCapture);

                if (msg == null)
                    msg = ServiceInstaller.Install(exePathRfidReader);

                if (msg == null)
                    msg = ServiceInstaller.Install(exePathKeepIISAlive);

                return msg;
            }

            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"ReinstallServices: {ex.Message}", EventLogEntryType.Error);
                return ex.Message;
            }
        }

        public string UninstallServices(string smsPath, string phidgetPath, string rfidPath, string videoCapturPath, string keepIISAlivePath)
        {
            var exePathSms = $@"{smsPath}\{ServiceName.StateMachineExe}";
            var exePathPhidget = $@"{phidgetPath}\{ServiceName.PhidgetExe}";
            var exePathRfidReader = $@"{rfidPath}\{ServiceName.RfidReaderExe}";
            var exePathVideoCapture = $@"{videoCapturPath}\{ServiceName.VideoCaptureExe}";
            var exePathKeepIISAlive = $@"{keepIISAlivePath}\{ServiceName.KeepIISAliveExe}";

            try
            {
                // uninstall
                var msg = ((ServiceInstaller.Uninstall(exePathKeepIISAlive) 
                        ?? ServiceInstaller.Uninstall(exePathRfidReader)) 
                        ?? ServiceInstaller.Uninstall(exePathPhidget)) 
                        ?? ServiceInstaller.Uninstall(exePathVideoCapture);

                if (msg != null) return msg;

                msg = ServiceInstaller.Uninstall(exePathSms);

                while (StateMachineIsInstalled()) { }
                while (KeepIISAliveIsInstalled()) { }

                return msg;
            }

            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"UninstallServices: {ex.Message}", EventLogEntryType.Error);
                return ex.Message;
            }
        }

        private static bool StateMachineIsInstalled()
        {
            var processes = Process.GetProcessesByName("Keebee.AAT.StateMachineService");
            return (processes.Any());
        }

        private static bool KeepIISAliveIsInstalled()
        {
            var processes = Process.GetProcessesByName("Keebee.AAT.KeebIISAlive");
            return (processes.Any());
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
                catch (Exception ex)
                {
                    return ex.InnerException?.Message ?? ex.Message;
                }

                return null;
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
                    return ex.InnerException?.Message ?? ex.Message;
                }

                return null;
            }
        }
    }
}
