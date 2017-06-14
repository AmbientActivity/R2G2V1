using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Shared;
using Keebee.AAT.MessageQueuing;
using System;
using System.Diagnostics;
using System.ServiceProcess;
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
                var isInstalledBeaconWatcher = ServiceUtilities.IsInstalled(ServiceUtilities.ServiceType.BluetoothBeaconWatcher);
                var isInstalledVideoCapture = ServiceUtilities.IsInstalled(ServiceUtilities.ServiceType.VideoCapture);

                // state machine
                var service = new ServiceController(ServiceName.StateMachine);

                if (service.CanStop)
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped);
                    while (ServiceUtilities.IsInstalled(ServiceUtilities.ServiceType.VideoCapture)) { }
                }

                // bluetooth beacon watcher
                if (isInstalledBeaconWatcher)
                {
                    service = new ServiceController(ServiceName.BluetoothBeaconWatcher);
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running);
                }

                // phidget
                service = new ServiceController(ServiceName.Phidget);
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running);

                // video
                if (isInstalledVideoCapture)
                {
                    service = new ServiceController(ServiceName.VideoCapture);
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running);
                }
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

                eventLog = new SystemEventLogger(SystemEventLogType.BluetoothBeaconWatcherService);
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

        public string ReinstallServices(string smsPath, string phidgetPath, string bluetoothBeaconWatcherPath, string videoCapturPath, string keepIISAlivePath)
        {
            try
            {
                // uninstall
                var msg = ServiceUtilities.Install(ServiceUtilities.ServiceType.KeepIISAlive, keepIISAlivePath, false)
                          ?? ServiceUtilities.Install(ServiceUtilities.ServiceType.BluetoothBeaconWatcher, bluetoothBeaconWatcherPath, false)
                          ?? ServiceUtilities.Install(ServiceUtilities.ServiceType.Phidget, phidgetPath, false)
                          ?? ServiceUtilities.Install(ServiceUtilities.ServiceType.VideoCapture, videoCapturPath, false);

                if (msg != null) return msg;

                msg = ServiceUtilities.Install(ServiceUtilities.ServiceType.StateMachine, smsPath, false);

                // wait for these to completely uninstall
                while (ServiceUtilities.IsInstalled(ServiceUtilities.ServiceType.StateMachine)) { }

                while (ServiceUtilities.IsInstalled(ServiceUtilities.ServiceType.KeepIISAlive)) { }

                // install
                if (msg == null)
                    msg = ServiceUtilities.Install(ServiceUtilities.ServiceType.StateMachine, smsPath, true);

                if (msg == null)
                    msg = ServiceUtilities.Install(ServiceUtilities.ServiceType.Phidget, phidgetPath, true);

                if (msg == null)
                    msg = ServiceUtilities.Install(ServiceUtilities.ServiceType.VideoCapture, videoCapturPath, true);

                if (msg == null)
                    msg = ServiceUtilities.Install(ServiceUtilities.ServiceType.BluetoothBeaconWatcher, bluetoothBeaconWatcherPath, true);

                if (msg == null)
                    msg = ServiceUtilities.Install(ServiceUtilities.ServiceType.KeepIISAlive, keepIISAlivePath, true);

                return msg;
            }

            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"ReinstallServices: {ex.Message}", EventLogEntryType.Error);
                return ex.Message;
            }
        }

        public string UninstallServices(string smsPath, string phidgetPath, string bluetoothBeaconWatcherPath, string videoCapturPath, string keepIISAlivePath)
        {
            try
            {
                // uninstall
                var msg = ((ServiceUtilities.Install(ServiceUtilities.ServiceType.KeepIISAlive, keepIISAlivePath, false) 
                        ?? ServiceUtilities.Install(ServiceUtilities.ServiceType.BluetoothBeaconWatcher, bluetoothBeaconWatcherPath, false)) 
                        ?? ServiceUtilities.Install(ServiceUtilities.ServiceType.Phidget, phidgetPath, false)) 
                        ?? ServiceUtilities.Install(ServiceUtilities.ServiceType.VideoCapture, videoCapturPath, false);

                if (msg != null) return msg;

                msg = ServiceUtilities.Install(ServiceUtilities.ServiceType.StateMachine, smsPath, false);

                while (ServiceUtilities.IsInstalled(ServiceUtilities.ServiceType.StateMachine)) { }
                while (ServiceUtilities.IsInstalled(ServiceUtilities.ServiceType.KeepIISAlive)) { }

                return msg;
            }

            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"UninstallServices: {ex.Message}", EventLogEntryType.Error);
                return ex.Message;
            }
        }
    }
}
