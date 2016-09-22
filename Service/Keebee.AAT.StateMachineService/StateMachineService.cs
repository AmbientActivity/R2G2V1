using Keebee.AAT.ServiceModels;
using Keebee.AAT.RESTClient;
using Keebee.AAT.MessageQueuing;
using Keebee.AAT.Shared;
using Keebee.AAT.SystemEventLogging;
using System.Net;
using System.Threading;
using System;
using System.Web.Script.Serialization;
using System.ServiceProcess;
using System.Diagnostics;
using System.Linq;

namespace Keebee.AAT.StateMachineService
{
    public partial class StateMachineService : ServiceBase
    {
        private const string UrlKeepAlive = "http://localhost/Keebee.AAT.Operations/api/status";

        // operations REST client
        private readonly IOperationsClient _opsClient;

        // message queue sender
        private readonly CustomMessageQueue _messageQueueResponse;

        // event logger
        private readonly SystemEventLogger _systemEventLogger;

        // active config
        private Config _activeConfig;
        private bool _reloadActiveConfig = true;

        // active profile
        private Resident _activeResident;

        // display state
        private bool _displayIsActive;

        public StateMachineService()
        {
            InitializeComponent();

            _systemEventLogger = new SystemEventLogger(SystemEventLogType.StateMachineService);
            _opsClient = new OperationsClient { SystemEventLogger = _systemEventLogger };
 
            InitializeMessageQueueListeners();

            // message queue sender
            _messageQueueResponse = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.Response
            }) {SystemEventLogger = _systemEventLogger};

            var keepAliveThread = new Thread(KeepAlive);
            keepAliveThread.Start();
        }

        private void KeepAlive()
        {
            while (true)
            {
                if (_activeResident != null)
                {
                    var req = (HttpWebRequest) WebRequest.Create(UrlKeepAlive);
                    var response = (HttpWebResponse) req.GetResponse();

                    if (response.StatusCode != HttpStatusCode.OK)
                        _systemEventLogger.WriteEntry(
                            $"Error accessing web host.{Environment.NewLine}StatusCode: {response.StatusCode}");
                }

                try
                {
                    Thread.Sleep(60000);
                }

                catch (ThreadAbortException)
                {
                    break;
                }
            }
        }

        private void InitializeMessageQueueListeners()
        {
            var q1 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.Phidget,
                MessageReceivedCallback = MessageReceivedPhidget
            }) { SystemEventLogger = _systemEventLogger };

            var q2 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.Video,
                MessageReceivedCallback = MessageReceivedVideo
            }) { SystemEventLogger = _systemEventLogger };

            var q3 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.Rfid,
                MessageReceivedCallback = MessageReceivedRfid
            }) { SystemEventLogger = _systemEventLogger };

            var q4 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.Display,
                MessageReceivedCallback = MessageReceivedDisplay
            }) { SystemEventLogger = _systemEventLogger };

            var q5 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.ConfigSms,
                MessageReceivedCallback = MessageReceiveConfigSms
            })
            { SystemEventLogger = _systemEventLogger };
        }

        private void ExecuteResponse(int phidgetTypeId, int sensorValue)
        {
            try
            {
                LoadResident();
                LoadActiveConfig();

                // if the activity type is not defined in this config then exit
                if (_activeConfig.ConfigDetails.All(x => x.PhidgetType.Id != phidgetTypeId))
                    return;

                var configDetail =
                    _activeConfig.ConfigDetails
                    .Single(cd => cd.PhidgetType.Id == phidgetTypeId);

                var responseMessage = new ResponseMessage
                {
                    SensorValue = sensorValue,

                    ActiveConfigDetail = new ActiveConfigDetail
                        {
                            Id = configDetail.Id,
                            PhidgetTypeId = configDetail.PhidgetType.Id,
                            ResponseTypeId = configDetail.ResponseType.Id,
                            IsSystem = configDetail.ResponseType.IsSystem
                        },

                    ActiveResident = new ActiveResident
                        {
                            Id = _activeResident.Id,
                            ConfigId = _activeConfig.Id,
                            GameDifficultyLevel = _activeResident.GameDifficultyLevel
                        },

                    IsActiveEventLog = _activeConfig.IsActiveEventLog
                };

                var serializer = new JavaScriptSerializer();
                var responseMessageBody = serializer.Serialize(responseMessage);
                _messageQueueResponse.Send(responseMessageBody);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"ExecuteResponse: {ex.Message}", EventLogEntryType.Error); 
            }
        }

        private void LoadResident()
        {
            if (_activeResident == null)
                _activeResident = _opsClient.GetGenericDetails();
        }

        private void LoadActiveConfig()
        {
            if (!_reloadActiveConfig && _activeConfig != null) return;

            _activeConfig = _opsClient.GetActiveConfigDetails();
            _reloadActiveConfig = false;
        }

        #region message received event handlers

        private void MessageReceivedPhidget(object source, MessageEventArgs e)
        {
            try
            {
                // do nothing unless the display is active
                if (!_displayIsActive) return;

                var phidget = GetPhidgetFromMessageBody(e.MessageBody);
                if (phidget == null) return;

                var sensorValue = phidget.SensorValue;

                // sensorId's are base 0 - convert to base 1 for PhidgetTypeId
                var phidgetTypeId = phidget.SensorId + 1;

                ExecuteResponse(phidgetTypeId, sensorValue);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"QueueMessageReceivedPhidget: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private static PhidgetMessage GetPhidgetFromMessageBody(string messageBody)
        {
            var serializer = new JavaScriptSerializer();
            var phidget = serializer.Deserialize<PhidgetMessage>(messageBody);
            return phidget;
        }

        private void MessageReceivedRfid(object source, MessageEventArgs e)
        {
            try
            {
                int residentId;
                var isValid = int.TryParse(e.MessageBody, out residentId);
                if (!isValid) return;

                LoadActiveConfig();

                if (residentId > 0)
                {
                    if (_activeResident?.Id == residentId) return;
                    _activeResident = _opsClient.GetResident(residentId);
                    LogRfidEvent(residentId, "New active resident");
                }
                else
                {
                    if (_activeResident?.Id == 0) return;
                    _activeResident = _opsClient.GetGenericDetails();

                    LogRfidEvent(-1, "Active resident is generic");
                }
            }

            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"MessageReceivedRfid{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
            }
        }

        private void MessageReceivedVideo(object source, MessageEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry(ex.Message, EventLogEntryType.Error);
            }
        }

        private void MessageReceivedDisplay(object source, MessageEventArgs e)
        {
            try
            {
                var displayMessage = GetDisplayFromMessageBody(e.MessageBody);

                _displayIsActive = displayMessage.IsActive;

                if (!_displayIsActive) return;

                if (_activeResident == null)
                    _activeResident = _opsClient.GetGenericDetails();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"MessageReceivedDisplay{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
            }
        }

        private void MessageReceiveConfigSms(object source, MessageEventArgs e)
        {
            try
            {
                if (e.MessageBody != "1") return;
                _reloadActiveConfig = true;
                _systemEventLogger.WriteEntry($"{_activeConfig.Description} has been activated");
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"MessageReceiveConfigSms{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
            }
        }

        private static DisplayMessage GetDisplayFromMessageBody(string messageBody)
        {
            var serializer = new JavaScriptSerializer();
            var display = serializer.Deserialize<DisplayMessage>(messageBody);
            return display;
        }

        #endregion

        protected override void OnStart(string[] args)
        {
            _systemEventLogger.WriteEntry("In OnStart");
        }

        protected override void OnStop()
        {
            _systemEventLogger.WriteEntry("In OnStop");
        }

        private void LogRfidEvent(int residentId, string description)
        {
            try
            {
                if (!_activeConfig.IsActiveEventLog) return;

                var rfidEventLogger = new RfidEventLogger()
                {
                    OperationsClient = _opsClient,
                    SystemEventLogger = _systemEventLogger
                };
                rfidEventLogger.Add(residentId, description);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"RfidReaderService.LogRfidEvent: {ex.Message}", EventLogEntryType.Error);
            }
        }
    }
}
