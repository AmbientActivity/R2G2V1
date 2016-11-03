using Keebee.AAT.ServiceModels;
using Keebee.AAT.RESTClient;
using Keebee.AAT.MessageQueuing;
using Keebee.AAT.Shared;
using Keebee.AAT.SystemEventLogging;
using System;
using System.Web.Script.Serialization;
using System.ServiceProcess;
using System.Diagnostics;
using System.Linq;

namespace Keebee.AAT.StateMachineService
{
    public partial class StateMachineService : ServiceBase
    {
        // operations REST client
        private readonly IOperationsClient _opsClient;

        // message queue sender
        private readonly CustomMessageQueue _messageQueueResponse;
        private readonly CustomMessageQueue _messageQueueConfigPhidget;
        private readonly CustomMessageQueue _messageQueueVideoCapture;

        // event logger
        private readonly SystemEventLogger _systemEventLogger;

        // active config
        private ConfigMessage _activeConfig;

        // active profile
        private ResidentMessage _activeResident;

        // display state
        private bool _isDisplayActive;

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
            })
            { SystemEventLogger = _systemEventLogger };

            _messageQueueConfigPhidget = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.ConfigPhidget
            })
            { SystemEventLogger = _systemEventLogger };

            _messageQueueVideoCapture = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.VideoCapture
            })
            { SystemEventLogger = _systemEventLogger };
        }

        private void InitializeMessageQueueListeners()
        {
            var q1 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.Phidget,
                MessageReceivedCallback = MessageReceivedPhidget
            })
            { SystemEventLogger = _systemEventLogger };

            var q2 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.ConfigSms,
                MessageReceivedCallback = MessageReceivedConfigSms
            })
            { SystemEventLogger = _systemEventLogger };

            var q3 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.Rfid,
                MessageReceivedCallback = MessageReceivedRfid
            })
            { SystemEventLogger = _systemEventLogger };

            var q4 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.DisplaySms,
                MessageReceivedCallback = MessageReceivedDisplaySms
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
                    Resident = _activeResident,
                    IsActiveEventLog = _activeConfig.IsActiveEventLog,
                    ResponseTypeIds = _activeConfig.ConfigDetails
                        .Select(c => c.ResponseTypeId)
                        .Distinct().ToArray()
                };

                if (!configDetail.IsSystemReponseType)
                {
                    if (_activeResident.AllowVideoCapturing)
                        // send a signal to the video capture service to start recording
                        _messageQueueVideoCapture.Send("1");
                }

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
                if (!_isDisplayActive) return;

                var phidget = GetPhidgetFromMessageBody(e.MessageBody);
                if (phidget == null) return;

                var sensorValue = phidget.SensorValue;

                // sensorId's are base 0 - convert to base 1 for PhidgetTypeId
                var phidgetTypeId = phidget.SensorId + 1;

                ExecuteResponse(phidgetTypeId, sensorValue);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"MessageReceivedPhidget: {ex.Message}", EventLogEntryType.Error);
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
                    SetActiveResident(_isDisplayActive ? resident.Id : (int?)null);
                }
                else
                {
                    if (_activeResident?.Id == PublicMediaSource.Id) return;
                    LogRfidEvent(PublicMediaSource.Id, "Active resident is public");
                    SetActiveResident(null);
                }

                _activeResident = resident;
                if (_activeResident == null) return;

                // TODO: should it stop recording if a new resident becomes active
                // TODO: who has not agreed to be captured?
                //if (!_activeResident.AllowVideoCapturing)
                //    _messageQueueVideoCapture.Send("0");
            }

            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"MessageReceivedRfid{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
            }
        }
         
        private void MessageReceivedDisplaySms(object source, MessageEventArgs e)
        {
            try
            {
                var displayMessage = GetDisplayStateFromMessageBody(e.MessageBody);
                _isDisplayActive = displayMessage.IsActive;

                if (!_isDisplayActive)
                {
                    SetActiveResident(null);
                    return;
                }

                _activeResident = new ResidentMessage
                {
                    Id = PublicMediaSource.Id,
                    GameDifficultyLevel = 1,
                    AllowVideoCapturing = false
                };

                LoadConfig();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"MessageReceivedDisplaySms{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
            }
        }

        private void MessageReceivedConfigSms(object source, MessageEventArgs e)
        {
            try
            {
                _activeConfig = GetConfigFromMessageBody(e.MessageBody);
                _isDisplayActive = _activeConfig.IsDisplayActive;

                if (!_isDisplayActive) return;
                _systemEventLogger.WriteEntry($"The configuration '{_activeConfig.Description}' has been activated");
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

        private static ResidentMessage GetResidentFromMessageBody(string messageBody)
        {
            var serializer = new JavaScriptSerializer();
            var resident = serializer.Deserialize<ResidentMessage>(messageBody);

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
            if (_activeConfig != null) return;
            try
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
                                            ConfigId = x.ConfigId,
                                            ResponseTypeId = x.ResponseType.Id,
                                            PhidgetTypeId = x.PhidgetType.Id,
                                            PhidgetStyleTypeId = x.PhidgetStyleType.Id,
                                            IsSystemReponseType = x.ResponseType.IsSystem
                                        }
                                        )
                };

                _systemEventLogger.WriteEntry($"The configuration '{config.Description}' has been activated");
            }

            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"LoadConfig{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
            }
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
                if (!_isDisplayActive) return;
                if (_activeConfig == null) return;
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
                _systemEventLogger.WriteEntry($"LogRfidEvent: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void SetActiveResident(int? residentId)
        {
            try
            {
                var resident = new ActiveResidentEdit { ResidentId = residentId };
                _opsClient.PatchActiveResident(resident);
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"SetActiveResident: {ex.Message}", EventLogEntryType.Error);
            }
        }
    }
}
