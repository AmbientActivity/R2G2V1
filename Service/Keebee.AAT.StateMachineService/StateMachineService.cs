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

        // active profile
        private Profile _activeProfile;

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
                // only execute if the display is active and the current active profile is "Generic"
                if (_displayIsActive && _activeProfile?.ResidentId == 0)
                {
                    var req = (HttpWebRequest)WebRequest.Create(UrlKeepAlive);
                    var response = (HttpWebResponse)req.GetResponse();

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
                QueueName = MessageQueueType.Config,
                MessageReceivedCallback = MessageReceiveConfig
            })
            { SystemEventLogger = _systemEventLogger };
        }

        private void ExecuteResponse(int phidgetTypeId, int sensorValue)
        {
            try
            {
                // if the activity type is not defined in this config then exit
                if (_activeConfig.ConfigDetails.All(x => x.PhidgetType.Id != phidgetTypeId))
                    return;

                var responseType =
                    _activeConfig.ConfigDetails
                    .Single(cd => cd.PhidgetType.Id == phidgetTypeId)
                    .ResponseType;

                var responseMessage = new ResponseMessage
                {
                    PhidgetTypeId = phidgetTypeId,
                    ResponseTypeId = responseType.Id,
                    SensorValue = sensorValue,
                    IsSystem = responseType.IsSystem,
                    ActiveProfile = new ActiveProfile
                        {
                            Id = _activeProfile.Id,
                            ConfigId = _activeConfig.Id,
                            ResidentId = _activeProfile.ResidentId,
                            GameDifficultyLevel = _activeProfile.GameDifficultyLevel
                        }
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

                if (residentId > 0)
                {
                    if (_activeProfile?.ResidentId == residentId) return;
                    _activeProfile = _opsClient.GetResidentProfile(residentId);
                    LogRfidEvent(residentId, "New active profile");
                }
                else
                {
                    if (_activeProfile?.ResidentId == 0) return;
                    _activeProfile = _opsClient.GetGenericProfile();

                    LogRfidEvent(-1, "Active profile is generic");

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

                if (_activeProfile == null)
                    _activeProfile = _opsClient.GetGenericProfile();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"MessageReceivedDisplay{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
            }
        }

        private void MessageReceiveConfig(object source, MessageEventArgs e)
        {
            try
            {
                if (e.MessageBody != "1") return;
                _activeConfig = _opsClient.GetActiveConfigDetails();
                _systemEventLogger.WriteEntry($"{_activeConfig.Description} has been activated");
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"MessageReceivedDisplay{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
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
            _activeConfig = _opsClient.GetActiveConfigDetails();
        }

        protected override void OnStop()
        {
            _systemEventLogger.WriteEntry("In OnStop");
        }

        private void LogRfidEvent(int residentId, string description)
        {
            try
            {
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
