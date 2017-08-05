using Keebee.AAT.MessageQueuing;
using Keebee.AAT.Shared;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using Newtonsoft.Json;
using System;
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

        #region initialization

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

                var responseMessage = new ResponseMessage
                {
                    SensorValue = sensorValue,
                    ConfigDetail = configDetail,
                    Resident = _activeResident,
                    IsActiveEventLog = _activeConfig.IsActiveEventLog
                };

                // for the OffScreen, need to alternate between the OffScreen and a 'random' response
                if (configDetail.ResponseType.Id == ResponseTypeId.OffScreen &&
                    _currentResponseTypeId == ResponseTypeId.OffScreen)
                {
                    responseMessage.ConfigDetail.ResponseType = GetOffScreenResponse();
                    _currentResponseTypeId = responseMessage.ConfigDetail.ResponseType.Id;
                }
                else
                    _currentResponseTypeId = configDetail.ResponseType.Id;

                if (_isInstalledVideoCapture)
                {
                    if (configDetail.ResponseType.ResponseTypeCategoryId != ResponseTypeCategoryId.System)
                    {
                        if (_activeResident.AllowVideoCapturing)
                            // send instruction to the video capture service to start capturing
                            _messageQueueVideoCapture.Send("1");
                    }
                }

                _messageQueueResponse.Send(JsonConvert.SerializeObject(responseMessage));
                
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"ExecuteResponse: {ex.Message}", SystemEventLogType.StateMachineService, EventLogEntryType.Error);
                _currentResponseTypeId = 0;
            }
        }

        private int _currentSequentialResponseTypeIndex = -1;
        private ResponseTypeMessage GetOffScreenResponse()
        {
            ResponseTypeMessage responseType = null;
            try
            {
                if (LoadRandomResponseTypes())
                {
                    if (_currentSequentialResponseTypeIndex < _randomResponseTypes.Length - 1)
                        _currentSequentialResponseTypeIndex++;
                    else
                        _currentSequentialResponseTypeIndex = 0;

                    responseType = _randomResponseTypes[_currentSequentialResponseTypeIndex];
                }
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"GetOffScreenResponse: {ex.Message}", SystemEventLogType.StateMachineService, EventLogEntryType.Error);
            }

            return responseType;
        }

        private void LoadConfig()
        {
            if (_activeConfig != null) return;
            try
            {
                var config = _configsClient.GetActiveDetails();
                _activeConfig = GetConfig(config);

                SystemEventLogger.WriteEntry($"The configuration '{config.Description}' has been activated", SystemEventLogType.StateMachineService);
            }

            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"LoadConfig{Environment.NewLine}{ex.Message}", SystemEventLogType.StateMachineService, EventLogEntryType.Error);
            }
        }

        private static ConfigMessage GetConfig(Config config)
        {
            return new ConfigMessage
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
                        PhidgetTypeId = x.PhidgetType.Id,
                        PhidgetStyleType = new PhidgetStyleTypeMessage
                        {
                            Id = x.PhidgetStyleType.Id,
                            IsIncremental = x.PhidgetStyleType.IsIncremental
                        },
                        ResponseType = new ResponseTypeMessage
                        {
                            Id = x.ResponseType.Id,
                            ResponseTypeCategoryId = x.ResponseType.ResponseTypeCategory.Id,
                            IsRandom = x.ResponseType.IsRandom,
                            IsRotational = x.ResponseType.IsRotational,
                            IsUninterrupted = x.ResponseType.IsUninterrupted,
                            InteractiveActivityTypeId = x.ResponseType.InteractiveActivityType?.Id ?? 0,
                            SwfFile = x.ResponseType.InteractiveActivityType?.SwfFile ?? string.Empty
                        }
                    })
            };
        }

        private bool LoadRandomResponseTypes()
        {
            try
            {
                if (_randomResponseTypes != null) return true;

                var responseTypesClient = new ResponseTypesClient();
                _randomResponseTypes = responseTypesClient.GetRandomTypes()
                    .Select(r => new ResponseTypeMessage
                    {
                        Id = r.Id,
                        ResponseTypeCategoryId = r.ResponseTypeCategory.Id,
                        IsRotational = r.IsRotational,
                        IsUninterrupted = r.IsUninterrupted,
                        InteractiveActivityTypeId = r.InteractiveActivityType?.Id ?? 0,
                        SwfFile = r.InteractiveActivityType?.SwfFile ?? string.Empty
                    }).ToArray();
              
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"LoadRandomResponseTypes{Environment.NewLine}{ex.Message}", SystemEventLogType.StateMachineService, EventLogEntryType.Error);
            }

            return (_randomResponseTypes != null);
        }

        #endregion

        #region message received event handlers

        private void MessageReceivedPhidget(object source, MessageEventArgs e)
        {
            try
            {
                // do nothing unless the display is active
                if (!_isDisplayActive) return;

                var phidget = JsonConvert.DeserializeObject<Tuple<int, int>>(e.MessageBody);
                if (phidget == null) return;

                // sensorId's are base 0 - convert to base 1 for PhidgetTypeId
                var phidgetTypeId = phidget.Item1 + 1;
                var sensorValue = phidget.Item2;

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
                var resident = JsonConvert.DeserializeObject<ResidentMessage>(e.MessageBody);

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
                var displayMessage = JsonConvert.DeserializeObject<DisplayMessage>(e.MessageBody);
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
                LoadRandomResponseTypes();
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
                var config = JsonConvert.DeserializeObject<ConfigMessage>(e.MessageBody);

                _activeConfig = GetUpdatedConfig(config);
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

        private static ConfigMessage GetUpdatedConfig(ConfigMessage config)
        {
            return new ConfigMessage
            {
                Id = config.Id,
                Description = config.Description,
                IsActiveEventLog = config.IsActiveEventLog,
                IsDisplayActive = config.IsDisplayActive,
                ConfigDetails = config.ConfigDetails
                    .Select(x => new
                    ConfigDetailMessage
                    {
                        Id = x.Id,
                        ConfigId = x.ConfigId,
                        PhidgetTypeId = x.PhidgetTypeId,
                        PhidgetStyleType = new PhidgetStyleTypeMessage
                        {
                            Id = x.PhidgetStyleType.Id,
                            IsIncremental = x.PhidgetStyleType.IsIncremental
                        },
                        ResponseType = new ResponseTypeMessage
                        {
                            Id = x.ResponseType.Id,
                            ResponseTypeCategoryId = x.ResponseType.ResponseTypeCategoryId,
                            IsRandom = x.ResponseType.IsRandom,
                            IsRotational = x.ResponseType.IsRotational,
                            IsUninterrupted = x.ResponseType.IsUninterrupted,
                            InteractiveActivityTypeId = x.ResponseType.InteractiveActivityTypeId,
                            SwfFile = x.ResponseType.SwfFile
                        }
                    })
            };
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

        #endregion

        #region event handlers

        protected override void OnStart(string[] args)
        {
            SystemEventLogger.WriteEntry("In OnStart", SystemEventLogType.StateMachineService);
        }

        protected override void OnStop()
        {
            SystemEventLogger.WriteEntry("In OnStop", SystemEventLogType.StateMachineService);
        }

        #endregion

        #region active resident

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

        #endregion
    }
}
