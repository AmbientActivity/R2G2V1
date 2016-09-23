﻿using Keebee.AAT.MessageQueuing;
using Keebee.AAT.Shared;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.ServiceModels;
using Keebee.AAT.RESTClient;
using Phidgets;
using Phidgets.Events;
using System.Configuration;
using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Web.Script.Serialization;

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

        // operations REST client
        private readonly IOperationsClient _opsClient;

#if DEBUG
        private readonly CustomMessageQueue _messageQueuePhidgetMonitor;
        private bool _phidgetMonitorIsActive;
#endif

        // message queue sender
        private readonly CustomMessageQueue _messageQueuePhidget;

        // event logger
        private readonly SystemEventLogger _systemEventLogger;

        // sensor value
        private const int StepTolerance = 10;
        private const int DefaultTouchSensorThreshold = 990;
        private readonly int _sensorThreshold;

        // active config
        private ConfigMessage _activeConfig;

        // display state
        private bool _displayIsActive;

        public PhidgetService()
        {
            InitializeComponent();

            _systemEventLogger = new SystemEventLogger(SystemEventLogType.PhidgetService);
            _opsClient = new OperationsClient { SystemEventLogger = _systemEventLogger };
            _sensorThreshold = ValidateSensorThreshold(ConfigurationManager.AppSettings["TouchSensorThreshold"]);

            var interfaceKit = new InterfaceKit();
            interfaceKit.SensorChange += SensorChange;
            interfaceKit.InputChange += InputChange;

            // message queue senders
            _messageQueuePhidget = new CustomMessageQueue(new CustomMessageQueueArgs
                                                          {
                                                              QueueName = MessageQueueType.Phidget
                                                          }) { SystemEventLogger = _systemEventLogger };

#if DEBUG
            _messageQueuePhidgetMonitor = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.PhidgetMonitor
            });
#endif
            // message queue listeners
            InitializeMessageQueueListeners();

            // open the phidget using the command line arguments
            openCmdLine(interfaceKit);
        }

        private void InitializeMessageQueueListeners()
        {
            var q1 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.ConfigPhidget,
                MessageReceivedCallback = MessageReceiveConfigPhidget
            })

            { SystemEventLogger = _systemEventLogger };

            var q2 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.DisplayPhidget,
                MessageReceivedCallback = MessageReceivedDisplayPhidget
            })
            { SystemEventLogger = _systemEventLogger };

#if DEBUG
            var q3 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.PhidgetMonitorState,
                MessageReceivedCallback = PhidgetMonitorMessageReceived
            });
#endif
        }

        private int ValidateSensorThreshold(string threshold)
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

            _systemEventLogger.WriteEntry($"Invalid SensorThreshold value: {threshold}", EventLogEntryType.Error);
            return DefaultTouchSensorThreshold;
        }

        private void SensorChange(object sender, SensorChangeEventArgs e)
        {
            try
            {
#if DEBUG
                if (_phidgetMonitorIsActive)
                {
                    var message = CreateMessageBodyFromSensor(e.Index, e.Value);
                    _messageQueuePhidgetMonitor.Send(message);
                }
#endif
                if (!_displayIsActive) return;

                int sensorId;
                int sensorValue = e.Value;

                var isValid = int.TryParse(Convert.ToString(e.Index), out sensorId);
                if (!isValid) return;

                if (_activeConfig.ConfigDetails.All(cd => cd.PhidgetTypeId != sensorId + 1))
                    return;

                var configDetail = _activeConfig.ConfigDetails.Single(cd => cd.PhidgetTypeId == sensorId + 1);

                switch (configDetail.PhidgetStyleTypeId)
                {
                    case PhidgetStyleTypeIdId.Touch:
                        if (sensorValue >= _sensorThreshold)
                            _messageQueuePhidget.Send(CreateMessageBodyFromSensor(sensorId, sensorValue));
                        break;
                    case PhidgetStyleTypeIdId.MultiTurn:
                    case PhidgetStyleTypeIdId.StopTurn:
                    case PhidgetStyleTypeIdId.Slider:
                        var stepValue = GetStepValue(sensorValue);
                        if (stepValue > 0) 
                            _messageQueuePhidget.Send(CreateMessageBodyFromSensor(sensorId, stepValue));
                        break;
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"SensorChange: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void InputChange(object sender, InputChangeEventArgs e)
        {
            try
            {
                if (!e.Value) return;

                if (e.Index < 0 || e.Index > 7)
                    throw new Exception($"Invalid InputId: {e.Index}");

                int sensorId;
                var isValid = int.TryParse(Convert.ToString(e.Index), out sensorId);
                if (!isValid) return;

                // input0 => sensor8
                // input1 => sensor9
                // etc
                _messageQueuePhidget.Send(CreateMessageBodyFromSensor(sensorId + 8, 999));
  
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"InputChange: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private static int GetStepValue(int val)
        {
            var returnValue = -1;

            if (val >= (int)RotationSensorStep.Value1 - StepTolerance / 2 && val <= (int)RotationSensorStep.Value1 + StepTolerance / 2)
                returnValue = (int)RotationSensorStep.Value1;
            else if (val >= (int)RotationSensorStep.Value2 - StepTolerance / 2 && val <= (int)RotationSensorStep.Value2 + StepTolerance / 2)
                returnValue = (int)RotationSensorStep.Value2;
            else if (val >= (int)RotationSensorStep.Value3 - StepTolerance / 2 && val <= (int)RotationSensorStep.Value3 + StepTolerance / 2)
                returnValue = (int)RotationSensorStep.Value3;
            else if (val >= (int)RotationSensorStep.Value4 - StepTolerance / 2 && val <= (int)RotationSensorStep.Value4 + StepTolerance / 2)
                returnValue = (int)RotationSensorStep.Value4;
            else if (val >= (int)RotationSensorStep.Value5 - StepTolerance)
                return (int)RotationSensorStep.Value5;

            return returnValue;
        }

        private static string CreateMessageBodyFromSensor(int sensorId, int sensorValue)
        {
            var phidgetMessage = new PhidgetMessage {SensorId = sensorId, SensorValue = sensorValue};
                
            var serializer = new JavaScriptSerializer();
            var messageBody = serializer.Serialize(phidgetMessage);
            return messageBody;
        }

        private void MessageReceiveConfigPhidget(object source, MessageEventArgs e)
        {
            try
            {
                _activeConfig = GetConfigFromMessageBody(e.MessageBody);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"MessageReceiveConfigPhidget{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
            }
        }

        private void MessageReceivedDisplayPhidget(object source, MessageEventArgs e)
        {
            try
            {
                var displayMessage = GetDisplayStateFromMessageBody(e.MessageBody);
                _displayIsActive = displayMessage.IsActive;

                LoadConfig();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"MessageReceivedDisplayPhidget{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
            }
        }

        private static ConfigMessage GetConfigFromMessageBody(string messageBody)
        {
            var serializer = new JavaScriptSerializer();
            var config = serializer.Deserialize<ConfigMessage>(messageBody);
            return config;
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
        }

#if DEBUG
        private void PhidgetMonitorMessageReceived(object sender, MessageEventArgs e)
        {
            var message = (e.MessageBody);

            var activeState = Convert.ToInt16(message);
            _phidgetMonitorIsActive = activeState > 0;
        }
#endif
        protected override void OnStart(string[] args)
        {
            _systemEventLogger.WriteEntry("In OnStart");
        }

        protected override void OnStop()
        {
            _systemEventLogger.WriteEntry("In OnStop");
        }

        // phidget command line open functions
        #region Command line open functions

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
