using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Shared;
using Keebee.AAT.RESTClient;
using Keebee.AAT.MessageQueuing;
using System;
using System.Configuration.Install;
using System.Diagnostics;
using System.ServiceProcess;
using System.Linq;
using System.Web.Script.Serialization;

namespace Keebee.AAT.BusinessRules
{
    public class MaintenanceRules
    {
        private CustomMessageQueue _messageQueueResponse;
        public CustomMessageQueue MessageQueueResponse
        {
            set { _messageQueueResponse = value; }
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

        public string KillDisplay()
        {
            var errormessage = string.Empty;

            try
            {
                var responseMessage = new ResponseMessage
                {
                    SensorValue = 999,
                    ConfigDetail = new ConfigDetailMessage { ResponseTypeId = ResponseTypeId.KillDisplay, IsSystemReponseType = true },
                    Resident = new ResidentMessage(),
                    IsActiveEventLog = false,
                    ResponseTypeIds = null
                };

                var serializer = new JavaScriptSerializer();
                var responseMessageBody = serializer.Serialize(responseMessage);
                _messageQueueResponse.Send(responseMessageBody);
            }
            catch (Exception ex)
            {
                errormessage = ex.Message;
                _systemEventLogger.WriteEntry($"KillDisplay: {errormessage}", EventLogEntryType.Error);
            }

            return errormessage;
        }

        public string ClearServiceLogs()
        {
            var errormessage = string.Empty;

            try
            {
                var eventLog = new SystemEventLogger(SystemEventLogType.KeepIISAliveService);
                eventLog.Clear();

                eventLog = new SystemEventLogger(SystemEventLogType.StateMachineService);
                eventLog.Clear();

                eventLog = new SystemEventLogger(SystemEventLogType.PhidgetService);
                eventLog.Clear();

                eventLog = new SystemEventLogger(SystemEventLogType.RfidReaderService);
                eventLog.Clear();

                eventLog = new SystemEventLogger(SystemEventLogType.VideoCaptureService);
                eventLog.Clear();
            }
            catch (Exception ex)
            {
                errormessage = ex.Message;
                _systemEventLogger.WriteEntry($"ClearEventLogs: {errormessage}", EventLogEntryType.Error);
            }

            return errormessage;
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
