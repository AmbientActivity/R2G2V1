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
        // api client
        private readonly IActiveResidentClient _activeResidentClient;
        private readonly IConfigsClient _configsClient;
        private readonly IResponseTypesClient _responseTypesClient;

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

        // random response types (for "on/off" toggle)
        private IEnumerable<ResponseType> _randomResponseTypes;

        // display state
        private bool _isDisplayActive;

        // video capture
        private bool _isInstalledVideoCapture;

        public StateMachineService()
        {
            InitializeComponent();

            _systemEventLogger = new SystemEventLogger(SystemEventLogType.StateMachineService);
            _activeResidentClient = new ActiveResidentClient();
            _configsClient = new ConfigsClient();
            _responseTypesClient = new ResponseTypesClient();

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
                QueueName = MessageQueueType.DisplaySms,
                MessageReceivedCallback = MessageReceivedDisplaySms
            })
            { SystemEventLogger = _systemEventLogger };

            var q4 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.BluetoothBeaconWatcher,
                MessageReceivedCallback = MessageReceivedBluetoothBeaconWatcher
            })
            { SystemEventLogger = _systemEventLogger };

            var q5 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.VideoCaptureState,
                MessageReceivedCallback = MessageReceivedVideoCaptureState
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
                    RandomResponseTypes = _randomResponseTypes
                        .Select(r => new ResponseTypeMessage
                        {
                            Id = r.Id,
                            ResponseTypeCategoryId = r.ResponseTypeCategory.Id,
                            IsSystem = r.IsSystem,
                            IsAdvanceable = r.IsAdvanceable,
                            IsUninterrupted = r.IsUninterrupted,
                            InteractiveActivityTypeId = r.InteractiveActivityType?.Id ?? 0,
                            SwfFile = r.InteractiveActivityType?.SwfFile ?? string.Empty
                        }).Distinct().ToArray()
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
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"ExecuteResponse: {ex.Message}", EventLogEntryType.Error);
            }
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
                                IsAdvanceable = x.ResponseType.IsAdvanceable,
                                InteractiveActivityTypeId = x.ResponseType.InteractiveActivityType?.Id ?? 0,
                                SwfFile = x.ResponseType.InteractiveActivityType?.SwfFile ?? string.Empty
                            },    
                            ResponseTypeCategoryId = x.ResponseType.ResponseTypeCategory.Id,
                            PhidgetTypeId = x.PhidgetType.Id,
                            PhidgetStyleTypeId = x.PhidgetStyleType.Id
                        })
                };

                // reload random response types
                _randomResponseTypes = _responseTypesClient.GetRandomTypes();

                _systemEventLogger.WriteEntry($"The configuration '{config.Description}' has been activated");
            }

            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"LoadConfig{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
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
                _systemEventLogger.WriteEntry($"MessageReceivedBluetoothBeaconWatcher{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
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

        private void MessageReceivedVideoCaptureState(object source, MessageEventArgs e)
        {
            try
            {
                _isInstalledVideoCapture = e.MessageBody == "1";
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"MessageReceivedVideoCaptureState{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
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
            _systemEventLogger.WriteEntry("In OnStart");
        }

        protected override void OnStop()
        {
            _systemEventLogger.WriteEntry("In OnStop");
        }

        private void LogActiveResidentEvent(int residentId, string description)
        {
            try
            {
                if (!_isDisplayActive) return;
                if (_activeConfig == null) return;
                if (!_activeConfig.IsActiveEventLog) return;

                var activeResdientEventLogger = new ActiveResidentEventLogger()
                {
                    SystemEventLogger = _systemEventLogger
                };
                activeResdientEventLogger.Add(residentId, description);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"LogActiveResidentEvent: {ex.Message}", EventLogEntryType.Error);
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
                _systemEventLogger?.WriteEntry($"SetActiveResident: {ex.Message}", EventLogEntryType.Error);
            }
        }
    }
}
