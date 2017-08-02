using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Shared;
using Keebee.AAT.MessageQueuing;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace Keebee.AAT.BusinessRules
{
    public class MaintenanceRules
    {
        private CustomMessageQueue _messageQueueResponse;
        public CustomMessageQueue MessageQueueResponse
        {
            set { _messageQueueResponse = value; }
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
                SystemEventLogger.WriteEntry($"RestartService: {ex.Message}", SystemEventLogType.AdminInterface, EventLogEntryType.Error);
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
                    ConfigDetail = new ConfigDetailMessage
                    {
                        ResponseType = new ResponseTypeMessage
                        {
                            Id = ResponseTypeId.KillDisplay,
                            ResponseTypeCategoryId = ResponseTypeCategoryId.System
                        },
                    },
                    Resident = new ResidentMessage(),
                    IsActiveEventLog = false
                };

                var responseMessageBody = JsonConvert.SerializeObject(responseMessage);
                _messageQueueResponse.Send(responseMessageBody);
            }
            catch (Exception ex)
            {
                errormessage = ex.Message;
                SystemEventLogger.WriteEntry($"KillDisplay: {errormessage}", SystemEventLogType.AdminInterface, EventLogEntryType.Error);
            }

            return errormessage;
        }

        public string ClearServiceLogs()
        {
            var errormessage = string.Empty;

            try
            {
                SystemEventLogger.Clear(SystemEventLogType.KeepIISAliveService);
                SystemEventLogger.Clear(SystemEventLogType.StateMachineService);
                SystemEventLogger.Clear(SystemEventLogType.PhidgetService);
                SystemEventLogger.Clear(SystemEventLogType.BluetoothBeaconWatcherService);
                SystemEventLogger.Clear(SystemEventLogType.VideoCaptureService);
            }
            catch (Exception ex)
            {
                errormessage = ex.Message;
                SystemEventLogger.WriteEntry($"ClearEventLogs: {errormessage}", SystemEventLogType.AdminInterface, EventLogEntryType.Error);
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
                SystemEventLogger.WriteEntry($"ReinstallServices: {ex.Message}", SystemEventLogType.AdminInterface, EventLogEntryType.Error);
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
                SystemEventLogger.WriteEntry($"UninstallServices: {ex.Message}", SystemEventLogType.AdminInterface, EventLogEntryType.Error);
                return ex.Message;
            }
        }
    }
}
