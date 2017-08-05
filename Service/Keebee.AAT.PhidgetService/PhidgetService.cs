using Keebee.AAT.MessageQueuing;
using Keebee.AAT.Shared;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.ApiClient.Clients;
using Newtonsoft.Json;
using Phidgets;
using Phidgets.Events;
using System.Configuration;
using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;

namespace Keebee.AAT.PhidgetService
{
    internal enum SensorType
    {
        Sensor0 = 0,
        Sensor1 = 1,
        Sensor2 = 2,
        Sensor3 = 3,
        Sensor4 = 4,
        Sensor5 = 5,
        Sensor6 = 6,
        Sensor7 = 7,
    }

    internal enum InputType
    {
        Input0 = 0,
        Input1 = 1,
        Input2 = 2,
        Input3 = 3,
        Input4 = 4,
        Input5 = 5,
        Input6 = 6,
        Input7 = 7
    }

    internal partial class PhidgetService : ServiceBase
    {
        #region declaration

        // api client
        private readonly IConfigsClient _configsClient;

        private readonly CustomMessageQueue _messageQueuePhidgetMonitor;
        private bool _phidgetMonitorIsActive;

        // message queue sender
        private readonly CustomMessageQueue _messageQueuePhidget;
        private readonly CustomMessageQueue _messageQueuePhidgetContinuousRadio;

        // sensor value
        private const int DefaultTouchSensorThreshold = 990;
        private readonly int _sensorThreshold;
        private readonly int _inputDebounceTime;
        private readonly int _incrementalDebounceTime;
        private RotationSensorStep _currentDiscreteStepValue = RotationSensorStep.Value5;

        // active config
        private ConfigMessage _activeConfig;

        // display state
        private bool _isDisplayActive;

        // current values
        private int _currentRadioSensorValue;

        // these are used at startup to prevent events from firing when the IK is first attaching to sensors and inputs
        private int _currentSensor;
        private int _currentInput;
        private int _totalSensors;
        private int _totalInputs;

        private readonly InterfaceKit _interfaceKit;

        #endregion

        public PhidgetService()
        {
            InitializeComponent();

            _configsClient = new ConfigsClient();
            _sensorThreshold = ValidateSensorThreshold(ConfigurationManager.AppSettings["TouchSensorThreshold"]);
            _inputDebounceTime = int.Parse(ConfigurationManager.AppSettings["InputDebounceTime"]);
            _incrementalDebounceTime = int.Parse(ConfigurationManager.AppSettings["IncrementalDebounceTime"]);

            _interfaceKit = new InterfaceKit();
            _interfaceKit.SensorChange += SensorChange;
            _interfaceKit.InputChange += InputChange;
            _interfaceKit.Attach += Attach;

            // message queue senders
            _messageQueuePhidget = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.Phidget
            });

            _messageQueuePhidgetContinuousRadio = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.PhidgetContinuousRadio
            });

            _messageQueuePhidgetMonitor = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.PhidgetMonitor
            });

            // message queue listeners
            InitializeMessageQueueListeners();

            // open the phidget using the command line arguments
            openCmdLine(_interfaceKit);
        }

        #region initialization

        private void InitializeMessageQueueListeners()
        {
            var q1 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.ConfigPhidget,
                MessageReceivedCallback = MessageReceiveConfigPhidget
            });

            var q2 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.DisplayPhidget,
                MessageReceivedCallback = MessageReceivedDisplayPhidget
            });

            var q3 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.PhidgetMonitorState,
                MessageReceivedCallback = PhidgetMonitorMessageReceived
            });
        }

        private static int ValidateSensorThreshold(string threshold)
        {

            int thresholdValue;

            var isValid = int.TryParse(threshold, out thresholdValue);
            if (isValid)
            {
                isValid = (thresholdValue > 0) && (thresholdValue < 1000);
            }

            if (isValid)
            {
                return thresholdValue;
            }

            SystemEventLogger.WriteEntry($"Invalid SensorThreshold value: {threshold}", SystemEventLogType.PhidgetService, EventLogEntryType.Error);
            return DefaultTouchSensorThreshold;
        }

        private void Attach(object sender, AttachEventArgs e)
        {
            _totalSensors = _interfaceKit.sensors.Count;
            _totalInputs = _interfaceKit.inputs.Count;
        }

        private void LoadConfig()
        {
            var config = _configsClient.GetActiveDetails();
            _activeConfig = new ConfigMessage
            {
                ConfigDetails = config.ConfigDetails
                    .Select(x => new
                    ConfigDetailMessage
                    {
                        PhidgetTypeId = x.PhidgetType.Id,
                        PhidgetStyleType = new PhidgetStyleTypeMessage
                        {
                            Id = x.PhidgetStyleType.Id,
                            IsIncremental = x.PhidgetStyleType.IsIncremental
                        },
                        ResponseType = new ResponseTypeMessage
                        {
                            Id = x.ResponseType.Id
                        }
                    })
            };
            SystemEventLogger.WriteEntry($"The configuration '{config.Description}' has been activated", SystemEventLogType.PhidgetService);
        }

        #endregion

        #region sensor/input change events

        private void ProcessTouchSensor(ConfigDetailMessage configDetail, int sensorId, int sensorValue)
        {
            try
            {
                if (sensorValue < _sensorThreshold) return;

                if (configDetail.ResponseType.Id != ResponseTypeId.Radio)
                {
                    _messageQueuePhidget.Send(CreateMessageBodyFromSensor(sensorId, sensorValue));
                }
                else
                {
                    SetDiscreteStepValue();
                    _messageQueuePhidget.Send(CreateMessageBodyFromSensor(sensorId, (int) _currentDiscreteStepValue));
                    _messageQueuePhidgetContinuousRadio.Send($"{(int) _currentDiscreteStepValue}");
                }
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"ProcessTouchSensor: {ex.Message}", SystemEventLogType.PhidgetService, EventLogEntryType.Error);
            }
        }

        // for debouncing
        private DateTime _latestSensorHit = DateTime.MinValue;
        private void ProcessIncrementalSensor(ConfigDetailMessage configDetail, int sensorId, int sensorValue)
        {
            try
            {
                // debounce - don't allow consecutive events < xx milliseconds apart
                if (DateTime.Now - _latestSensorHit < TimeSpan.FromMilliseconds(_incrementalDebounceTime))
                {
                    _latestSensorHit = DateTime.Now;
                    return; // too fast
                }

                // send continuous value
                if (configDetail.ResponseType.Id == ResponseTypeId.Radio)
                    _messageQueuePhidgetContinuousRadio.Send($"{sensorValue}");

                // send step value
                var stepValue = PhidgetUtility.GetSensorStepValue(sensorValue);
                if (stepValue > 0)
                {
                    if (configDetail.ResponseType.Id != ResponseTypeId.Radio)
                    {
                        _messageQueuePhidget.Send(CreateMessageBodyFromSensor(sensorId, stepValue));
                    }
                    else
                    {
                        if (_currentRadioSensorValue != stepValue)
                        {
                            _messageQueuePhidget.Send(CreateMessageBodyFromSensor(sensorId, stepValue));
                            _currentRadioSensorValue = stepValue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"ProcessIncrementalSensor: {ex.Message}", SystemEventLogType.PhidgetService, EventLogEntryType.Error);
            }
        }

        private void ProcessInputChange(InputChangeEventArgs e)
        {
            try
            {
                if (e.Index < 0 || e.Index > 7)
                    throw new Exception($"Invalid InputId: {e.Index}");

                // for the LED's
                _interfaceKit.outputs[e.Index] = e.Value;

                if (_activeConfig == null) return;
                if (!_isDisplayActive) return;

                int inputId;
                var isValid = int.TryParse(Convert.ToString(e.Index), out inputId);
                if (!isValid) return;

                // PhidgetTypeId = InputId + 9 (offset by 8 SensorIds, InputId is base 0)
                var phidgetTypeId = inputId + 9;

                if (_activeConfig.ConfigDetails.All(cd => cd.PhidgetTypeId != phidgetTypeId))
                    return;

                var configDetail = _activeConfig.ConfigDetails.Single(cd => cd.PhidgetTypeId == phidgetTypeId);

                SetDiscreteStepValue();

                _messageQueuePhidget.Send(CreateMessageBodyFromSensor(inputId + 8, (int)_currentDiscreteStepValue));

                if (configDetail.ResponseType.Id == ResponseTypeId.Radio)
                    _messageQueuePhidgetContinuousRadio.Send($"{(int)_currentDiscreteStepValue}");

            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"ProcessInputChange: {ex.Message}", SystemEventLogType.PhidgetService, EventLogEntryType.Error);
            }
        }

        // set discrete values for when inputs or touch sensors are used to navigate through a playlist
        private void SetDiscreteStepValue()
        {
            switch (_currentDiscreteStepValue)
            {
                case RotationSensorStep.Value1:
                    _currentDiscreteStepValue = RotationSensorStep.Value2;
                    break;
                case RotationSensorStep.Value2:
                    _currentDiscreteStepValue = RotationSensorStep.Value3;
                    break;
                case RotationSensorStep.Value3:
                    _currentDiscreteStepValue = RotationSensorStep.Value4;
                    break;
                case RotationSensorStep.Value4:
                    _currentDiscreteStepValue = RotationSensorStep.Value5;
                    break;
                default:
                    _currentDiscreteStepValue = RotationSensorStep.Value1;
                    break;
            }
        }

        #endregion

        #region message send/receive

        private void MessageReceiveConfigPhidget(object source, MessageEventArgs e)
        {
            try
            {
                var config = JsonConvert.DeserializeObject<ConfigMessage>(e.MessageBody);

                _activeConfig = new ConfigMessage
                {
                    ConfigDetails = config.ConfigDetails.Select(x => new ConfigDetailMessage
                    {
                        PhidgetTypeId = x.PhidgetTypeId,
                        PhidgetStyleType = new PhidgetStyleTypeMessage
                        {
                            Id = x.PhidgetStyleType.Id,
                            IsIncremental = x.PhidgetStyleType.IsIncremental
                        },
                        ResponseType = new ResponseTypeMessage
                        {
                            Id = x.ResponseType.Id
                        }
                    })
                };

                _isDisplayActive = config.IsDisplayActive;

                SystemEventLogger.WriteEntry($"The configuration '{_activeConfig.Description}' has been activated", SystemEventLogType.PhidgetService);
                
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"MessageReceiveConfigPhidget{Environment.NewLine}{ex.Message}", SystemEventLogType.PhidgetService, EventLogEntryType.Error);
            }
        }

        private void MessageReceivedDisplayPhidget(object source, MessageEventArgs e)
        {
            try
            {
                var displayMessage = JsonConvert.DeserializeObject<DisplayMessage>(e.MessageBody);
                _isDisplayActive = displayMessage.IsActive;

                if (!_isDisplayActive) return;

                LoadConfig();
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"MessageReceivedDisplayPhidget{Environment.NewLine}{ex.Message}", SystemEventLogType.PhidgetService, EventLogEntryType.Error);
            }
        }

        private void PhidgetMonitorMessageReceived(object sender, MessageEventArgs e)
        {
            var message = (e.MessageBody);

            var activeState = Convert.ToInt16(message);
            _phidgetMonitorIsActive = activeState > 0;
        }

        private static string CreateMessageBodyFromSensor(int sensorId, int sensorValue)
        {
            return JsonConvert.SerializeObject(new Tuple<int, int>(sensorId, sensorValue));
        }

        #endregion

        #region event handlers

        private void SensorChange(object sender, SensorChangeEventArgs e)
        {
            // if the interface kit is still attaching, exit  
            if (_currentSensor != _totalSensors)
            {
                _currentSensor++;
                return;
            }

            try
            {
                if (_phidgetMonitorIsActive)
                {
                    var message = CreateMessageBodyFromSensor(e.Index, e.Value);
                    _messageQueuePhidgetMonitor.Send(message);
                }

                if (!_isDisplayActive) return;

                int sensorId;
                int sensorValue = e.Value;

                var isValid = int.TryParse(Convert.ToString(e.Index), out sensorId);
                if (!isValid) return;

                // PhidgetTypeId = SensorId + 1 (SensorId is base 0)
                var phidgetTypeId = sensorId + 1;

                if (_activeConfig.ConfigDetails.All(cd => cd.PhidgetTypeId != phidgetTypeId))
                    return;

                var configDetail = _activeConfig.ConfigDetails.Single(cd => cd.PhidgetTypeId == phidgetTypeId);

                if (configDetail.PhidgetStyleType.IsIncremental)
                    ProcessIncrementalSensor(configDetail, sensorId, sensorValue);
                else
                    ProcessTouchSensor(configDetail, sensorId, sensorValue);
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"SensorChange: {ex.Message}", SystemEventLogType.PhidgetService, EventLogEntryType.Error);
            }
        }

        // for debouncing
        private DateTime _latestInputHit = DateTime.MinValue;
        private void InputChange(object sender, InputChangeEventArgs e)
        {
            // if the interface kit is still attaching, exit  
            if (_currentInput != _totalInputs)
            {
                _currentInput++;
                return;
            }

            if (_phidgetMonitorIsActive)
            {
                var message = CreateMessageBodyFromSensor(e.Index + 8, e.Value ? 1 : 0);
                _messageQueuePhidgetMonitor.Send(message);
            }

            // debounce the switch - don't allow consecutive events < xx milliseconds apart
            if (DateTime.Now - _latestInputHit < TimeSpan.FromMilliseconds(_inputDebounceTime))
            {
                _latestInputHit = DateTime.Now;
                return;  // too fast
            }
            _latestInputHit = DateTime.Now;
            ProcessInputChange(e);
        }

        protected override void OnStart(string[] args)
        {
            SystemEventLogger.WriteEntry("In OnStart", SystemEventLogType.PhidgetService);
        }

        protected override void OnStop()
        {
            SystemEventLogger.WriteEntry("In OnStop", SystemEventLogType.PhidgetService);
        }

        #endregion

        #region command line open functions

        private void openCmdLine(Phidget p, String pass = null)
        {
            int serial = -1;
            String logFile = null;
            int port = 5001;
            String host = null;
            bool remote = false, remoteIP = false;
            string[] args = Environment.GetCommandLineArgs();

            try
            { //Parse the flags
                for (int i = 1; i < args.Length; i++)
                {
                    if (args[i].StartsWith("-"))
                        switch (args[i].Remove(0, 1).ToLower())
                        {
                            case "l":
                                logFile = (args[++i]);
                                break;
                            case "n":
                                serial = int.Parse(args[++i]);
                                break;
                            case "r":
                                remote = true;
                                break;
                            case "s":
                                remote = true;
                                host = args[++i];
                                break;
                            case "p":
                                pass = args[++i];
                                break;
                            case "i":
                                remoteIP = true;
                                host = args[++i];
                                if (host.Contains(":"))
                                {
                                    port = int.Parse(host.Split(':')[1]);
                                    host = host.Split(':')[0];
                                }
                                break;
                        }
                }
                if (logFile != null)
                    Phidget.enableLogging(Phidget.LogLevel.PHIDGET_LOG_INFO, logFile);
                if (remoteIP)
                    p.open(serial, host, port, pass);
                else if (remote)
                    p.open(serial, host, pass);
                else
                    p.open(serial);
                return; //success
            }
            catch { }
        }

        #endregion
    }
}
