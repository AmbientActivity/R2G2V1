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
        private const string UrlKeepAliveAdmin = "http://localhost/Keebee.AAT.Administrator";

        // operations REST client
        private readonly IOperationsClient _opsClient;

        // message queue sender
        private readonly CustomMessageQueue _messageQueueResponse;
        private readonly CustomMessageQueue _messageQueueConfigPhidget;

        // event logger
        private readonly SystemEventLogger _systemEventLogger;

        // active config
        private ConfigMessage _activeConfig;

        // active profile
        private ResidentMessage _activeResident;

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

            _messageQueueConfigPhidget = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.ConfigPhidget
            })
            { SystemEventLogger = _systemEventLogger };

            var keepAliveThread = new Thread(KeepAlive);
            keepAliveThread.Start();

            var keepAliveThreadAdmin = new Thread(KeepAliveAdmin);
            keepAliveThreadAdmin.Start();
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

        private void KeepAliveAdmin()
        {
            while (true)
            {
                var req = (HttpWebRequest)WebRequest.Create(UrlKeepAliveAdmin);
                var response = (HttpWebResponse)req.GetResponse();

                if (_activeResident != null)
                {
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
                QueueName = MessageQueueType.DisplaySms,
                MessageReceivedCallback = MessageReceivedDisplaySms
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
                // if the activity is not defined in the config then exit
                if (_activeConfig.ConfigDetails.All(x => x.PhidgetTypeId != phidgetTypeId))
                    return;

                var configDetail =
                    _activeConfig.ConfigDetails
                    .Single(cd => cd.PhidgetTypeId == phidgetTypeId);

                var responseMessage = new ResponseMessage
                {
                    SensorValue = sensorValue,
                    ConfigDetail = configDetail,
                    Resident =_activeResident,
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

        private void MessageReceivedRfid(object source, MessageEventArgs e)
        {
            try
            {
                var resident = GetResidentFromMessageBody(e.MessageBody);

                if (resident.Id > 0)
                {
                    if (_activeResident?.Id == resident.Id) return;
                    LogRfidEvent(resident.Id, "New active resident");
                }
                else
                {
                    if (_activeResident?.Id == PublicMediaSource.Id) return;
                    LogRfidEvent(PublicMediaSource.Id, "Active resident is public");
                }

                _activeResident = resident;
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

        private void MessageReceivedDisplaySms(object source, MessageEventArgs e)
        {
            try
            {
                var displayMessage = GetDisplayStateFromMessageBody(e.MessageBody);
                _displayIsActive = displayMessage.IsActive;

                if (!_displayIsActive) return;

                LoadResident();
                LoadConfig();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"MessageReceivedDisplaySms{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
            }
        }

        private void MessageReceiveConfigSms(object source, MessageEventArgs e)
        {
            try
            {
                _activeConfig = GetConfigFromMessageBody(e.MessageBody);
                _messageQueueConfigPhidget.Send(e.MessageBody);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"MessageReceiveConfigSms{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
            }
        }

        private static PhidgetMessage GetPhidgetFromMessageBody(string messageBody)
        {
            var serializer = new JavaScriptSerializer();
            var phidget = serializer.Deserialize<PhidgetMessage>(messageBody);
            return phidget;
        }

        private static ConfigMessage GetConfigFromMessageBody(string messageBody)
        {
            var serializer = new JavaScriptSerializer();
            var config = serializer.Deserialize<ConfigMessage>(messageBody);
            return config;
        }

        private ResidentMessage GetResidentFromMessageBody(string messageBody)
        {
            var resident = new ResidentMessage {Id = PublicMediaSource.Id, GameDifficultyLevel = 1};
            try
            {
                var serializer = new JavaScriptSerializer();
                resident = serializer.Deserialize<ResidentMessage>(messageBody);
               
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"GetResidentFromMessageBody{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
            }
            return resident;
        }

        private static DisplayMessage GetDisplayStateFromMessageBody(string messageBody)
        {
            var serializer = new JavaScriptSerializer();
            var display = serializer.Deserialize<DisplayMessage>(messageBody);
            return display;
        }

        private void LoadConfig()
        {
            var config = _opsClient.GetActiveConfigDetails();
            _activeConfig = new ConfigMessage
            {
                Id = config.Id,
                Description = config.Description,
                IsActiveEventLog = config.IsActiveEventLog,
                ConfigDetails = config.ConfigDetails
                                    .Select(x => new
                                    ConfigDetailMessage
                                    {
                                        Id = x.Id,
                                        ResponseTypeId = x.ResponseType.Id,
                                        PhidgetTypeId = x.PhidgetType.Id,
                                        PhidgetStyleTypeId = x.PhidgetStyleType.Id
                                    }
                                    )
            };
            _systemEventLogger.WriteEntry($"'{config.Description}' has been activated");
        }

        private void LoadResident()
        {
            // only for the first time
            if (_activeResident == null)
                _activeResident = new ResidentMessage { Id = PublicMediaSource.Id, GameDifficultyLevel = 1 };
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
                if (!_displayIsActive) return;
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
