using Keebee.AAT.ServiceModels;
using Keebee.AAT.MessageQueuing;
using Keebee.AAT.Shared;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.ServiceProcess;
using System.Diagnostics;
using System.Linq;

namespace Keebee.AAT.StateMachineService
{
    public partial class StateMachineService : ServiceBase
    {
        #region declaration

        // api client
        private readonly IActiveResidentClient _activeResidentClient;
        private readonly IConfigsClient _configsClient;
        private readonly IResponseTypesClient _responseTypesClient;

        // message queue sender
        private readonly CustomMessageQueue _messageQueueResponse;
        private readonly CustomMessageQueue _messageQueueConfigPhidget;
        private readonly CustomMessageQueue _messageQueueVideoCapture;

        // active config
        private ConfigMessage _activeConfig;

        // active profile
        private ResidentMessage _activeResident;

        // random response types (for "on/off" toggle)
        private ResponseTypeMessage[] _randomResponseTypes;

        // current response type (needed for the "on/off" toggle)
        private int _currentResponseTypeId = ResponseTypeId.Ambient;


        // display state
        private bool _isDisplayActive;

        // video capture
        private bool _isInstalledVideoCapture;

        #endregion

        public StateMachineService()
        {
            InitializeComponent();

            _activeResidentClient = new ActiveResidentClient();
            _configsClient = new ConfigsClient();
            _responseTypesClient = new ResponseTypesClient();

            InitializeMessageQueueListeners();

            // message queue sender
            _messageQueueResponse = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.Response
            });

            _messageQueueConfigPhidget = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.ConfigPhidget
            });

            _messageQueueVideoCapture = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.VideoCapture
            });
        }

        private void InitializeMessageQueueListeners()
        {
            var q1 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.Phidget,
                MessageReceivedCallback = MessageReceivedPhidget
            });

            var q2 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.ConfigSms,
                MessageReceivedCallback = MessageReceivedConfigSms
            });

            var q3 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.DisplaySms,
                MessageReceivedCallback = MessageReceivedDisplaySms
            });

            var q4 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.BluetoothBeaconWatcher,
                MessageReceivedCallback = MessageReceivedBluetoothBeaconWatcher
            });

            var q5 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.VideoCaptureState,
                MessageReceivedCallback = MessageReceivedVideoCaptureState
            });
        }

        private void ExecuteResponse(int phidgetTypeId, int sensorValue)
        {
            try
            {
                // if the activity is not defined in the config then exit
                if (_activeConfig.ConfigDetails.All(x => x.PhidgetTypeId != phidgetTypeId))
                    return;

                var configDetail = _activeConfig.ConfigDetails
                    .Single(cd => cd.PhidgetTypeId == phidgetTypeId);

                // for the OffScreen, need to alternate between the OffScreen and a 'random' response
                if (configDetail.ResponseType.Id == ResponseTypeId.OffScreen  
                    && _currentResponseTypeId == ResponseTypeId.OffScreen)
                    configDetail.ResponseType = GetOffScreenResponse();

                var responseMessage = new ResponseMessage
                {
                    SensorValue = sensorValue,
                    ConfigDetail = configDetail,
                    Resident = _activeResident,
                    IsActiveEventLog = _activeConfig.IsActiveEventLog
                };

                if (_isInstalledVideoCapture)
                {
                    if (!configDetail.ResponseType.IsSystem)
                    {
                        if (_activeResident.AllowVideoCapturing)
                            // send a signal to the video capture service to start recording
                            _messageQueueVideoCapture.Send("1");
                    }
                }

                var serializer = new JavaScriptSerializer();
                var responseMessageBody = serializer.Serialize(responseMessage);
                _messageQueueResponse.Send(responseMessageBody);

                _currentResponseTypeId = configDetail.ResponseType.Id;
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"ExecuteResponse: {ex.Message}", SystemEventLogType.StateMachineService, EventLogEntryType.Error);
            }
        }

        private int _currentSequentialResponseTypeIndex = -1;
        private ResponseTypeMessage GetOffScreenResponse()
        {
            if (_currentSequentialResponseTypeIndex < _randomResponseTypes.Length - 1)
                _currentSequentialResponseTypeIndex++;
            else
                _currentSequentialResponseTypeIndex = 0;

            return _randomResponseTypes[_currentSequentialResponseTypeIndex];
        }

        private void LoadConfig()
        {
            if (_activeConfig != null) return;
            try
            {
                var config = _configsClient.GetActiveDetails();
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
                            ResponseType = new ResponseTypeMessage
                            {
                                Id = x.ResponseType.Id,
                                ResponseTypeCategoryId = x.ResponseType.ResponseTypeCategory.Id,
                                IsSystem = x.ResponseType.IsSystem,
                                IsRandom = x.ResponseType.IsRandom,
                                IsRotational = x.ResponseType.IsRotational,
                                IsUninterrupted = x.ResponseType.IsUninterrupted,
                                InteractiveActivityTypeId = x.ResponseType.InteractiveActivityType?.Id ?? 0,
                                SwfFile = x.ResponseType.InteractiveActivityType?.SwfFile ?? string.Empty
                            },    
                            ResponseTypeCategoryId = x.ResponseType.ResponseTypeCategory.Id,
                            PhidgetTypeId = x.PhidgetType.Id,
                            PhidgetStyleTypeId = x.PhidgetStyleType.Id
                        })
                };

                // reload random response types
                _randomResponseTypes = _responseTypesClient.GetRandomTypes()
                    .Select( r => new ResponseTypeMessage
                    {
                        Id = r.Id,
                        ResponseTypeCategoryId = r.ResponseTypeCategory.Id,
                        IsSystem = r.IsSystem,
                        IsRotational = r.IsRotational,
                        IsUninterrupted = r.IsUninterrupted,
                        InteractiveActivityTypeId = r.InteractiveActivityType?.Id ?? 0,
                        SwfFile = r.InteractiveActivityType?.SwfFile ?? string.Empty
                    }).ToArray();

                SystemEventLogger.WriteEntry($"The configuration '{config.Description}' has been activated", SystemEventLogType.StateMachineService);
            }

            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"LoadConfig{Environment.NewLine}{ex.Message}", SystemEventLogType.StateMachineService, EventLogEntryType.Error);
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
                SystemEventLogger.WriteEntry($"MessageReceivedPhidget: {ex.Message}", SystemEventLogType.StateMachineService, EventLogEntryType.Error);
            }
        }

        private void MessageReceivedBluetoothBeaconWatcher(object source, MessageEventArgs e)
        {
            try
            {
                var resident = GetResidentFromMessageBody(e.MessageBody);

                if (resident.Id > 0)
                {
                    if (_activeResident?.Id == resident.Id) return;
                    LogActiveResidentEvent(resident.Id, "New active resident");
                    SetActiveResident(_isDisplayActive ? resident.Id : (int?)null);
                }
                else
                {
                    if (_activeResident?.Id == PublicProfileSource.Id) return;
                    LogActiveResidentEvent(PublicProfileSource.Id, "Active resident is public");
                    SetActiveResident(null);
                }

                _activeResident = resident;
            }

            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"MessageReceivedBluetoothBeaconWatcher{Environment.NewLine}{ex.Message}", SystemEventLogType.StateMachineService, EventLogEntryType.Error);
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
                    Id = PublicProfileSource.Id,
                    Name = PublicProfileSource.Name,
                    GameDifficultyLevel = 1,
                    AllowVideoCapturing = false
                };

                LoadConfig();
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"MessageReceivedDisplaySms{Environment.NewLine}{ex.Message}", SystemEventLogType.StateMachineService, EventLogEntryType.Error);
            }
        }

        private void MessageReceivedConfigSms(object source, MessageEventArgs e)
        {
            try
            {
                _activeConfig = GetConfigFromMessageBody(e.MessageBody);
                _isDisplayActive = _activeConfig.IsDisplayActive;

                if (!_isDisplayActive) return;
                SystemEventLogger.WriteEntry($"The configuration '{_activeConfig.Description}' has been activated", SystemEventLogType.StateMachineService);
                _messageQueueConfigPhidget.Send(e.MessageBody);
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"MessageReceiveConfigSms{Environment.NewLine}{ex.Message}", SystemEventLogType.StateMachineService, EventLogEntryType.Error);
            }
        }

        private void MessageReceivedVideoCaptureState(object source, MessageEventArgs e)
        {
            try
            {
                _isInstalledVideoCapture = e.MessageBody == "1";
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"MessageReceivedVideoCaptureState{Environment.NewLine}{ex.Message}", SystemEventLogType.StateMachineService, EventLogEntryType.Error);
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

        #endregion

        protected override void OnStart(string[] args)
        {
            SystemEventLogger.WriteEntry("In OnStart", SystemEventLogType.StateMachineService);
        }

        protected override void OnStop()
        {
            SystemEventLogger.WriteEntry("In OnStop", SystemEventLogType.StateMachineService);
        }

        private void LogActiveResidentEvent(int residentId, string description)
        {
            try
            {
                if (!_isDisplayActive) return;
                if (_activeConfig == null) return;
                if (!_activeConfig.IsActiveEventLog) return;

                var activeResdientEventLogger = new ActiveResidentEventLogger();
                activeResdientEventLogger.Add(residentId, description);
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"LogActiveResidentEvent: {ex.Message}", SystemEventLogType.StateMachineService, EventLogEntryType.Error);
            }
        }

        private void SetActiveResident(int? residentId)
        {
            try
            {
                var resident = new ActiveResidentEdit { ResidentId = residentId };
                _activeResidentClient.Patch(resident);
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"SetActiveResident: {ex.Message}", SystemEventLogType.StateMachineService, EventLogEntryType.Error);
            }
        }
    }
}
