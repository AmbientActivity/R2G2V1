using Keebee.AAT.SystemEventLogging;
using System;
using System.Diagnostics;
using System.Messaging;

namespace Keebee.AAT.MessageQueuing
{
    public enum MessageQueueType
    {
        DisplaySms = 1,
        DisplayPhidget = 2,
        DisplayVideoCapture = 3,
        DisplayBluetoothBeaconWatcher = 4,
        Phidget = 5,
        Response = 6,
        BluetoothBeaconWatcher = 7,
        BluetoothBeaconWatcherReload = 8,
        VideoCapture = 9,
        ConfigSms = 10,
        ConfigPhidget = 11,
        PhidgetContinuousRadio = 12,
        BeaconMonitor = 13,
        BeaconMonitorResident = 14,
        BeaconMonitorState = 15,
        VideoCaptureState = 16
#if DEBUG
        , PhidgetMonitor = 17
        , PhidgetMonitorState = 18
#endif
    }

    public class MessageEventArgs : EventArgs
    {
        public string MessageBody { get; set; }
    }

    public class CustomMessageQueueArgs
    {
        public MessageQueueType QueueName { get; set; }
        public EventHandler<MessageEventArgs> MessageReceivedCallback { get; set; }
    }

    public class CustomMessageQueue
    {
        private const string QueueNameDisplaySms = "Display-SMS";
        private const string QueueNameDisplayPhidget = "Display-Phidget";
        private const string QueueNameDisplayVideoCapture = "Display-Video-Capture";
        private const string QueueNameDisplayBluetoothBeaconWatcher = "Display-Bluetooth-Beacon-Watcher";
        private const string QueueNamePhidget = "Phidget";
        private const string QueueNameResponse = "Response";
        private const string QueueNameBluetoothBeaconWatcher = "Bluetooth-Beacon-Watcher";
        private const string QueueNameBluetoothBeaconWatcherReload = "Bluetooth-Beacon-Watcher-Reload";
        private const string QueueNameVideoCapture = "Video-Capture";
        private const string QueueNameConfigSms = "Config-SMS";
        private const string QueueNameConfigPhidget = "Config-Phidget";
        private const string QueueNamePhidgetContinuousRadio = "Phidget-Continuous-Radio";
        private const string QueueNameBeaconMonitor = "Beacon-Monitor";
        private const string QueueNameBeaconMonitorResident = "Beacon-Monitor-Resident";
        private const string QueueNameBeaconMonitorState = "Beacon-Monitor-State";
        private const string QueueNameVideoCaptureState = "Video-Capture-State";
#if DEBUG
        private const string QueueNamePhidgetMonitor = "Phidget-Monitor";
        private const string QueueNamePhidgetMonitorState = "Phidget-Monitor-State";
#endif

        private SystemEventLogger _systemEventLogger;
        public SystemEventLogger SystemEventLogger
        {
            set { _systemEventLogger = value; }
        }

        public EventHandler<MessageEventArgs> MessageReceivedCallback;

        private readonly MessageQueue _messageQueue;

        public EventHandler<MessageEventArgs> MessageRecevied { get; set; }

        public CustomMessageQueue(CustomMessageQueueArgs args)
        {
            _messageQueue = Create(args.QueueName);

            if (args.MessageReceivedCallback == null) return;

            MessageReceivedCallback += args.MessageReceivedCallback;
            _messageQueue.ReceiveCompleted += QueueMessageReceived;
            _messageQueue.BeginReceive();
        }

        public void QueueMessageReceived(object source, ReceiveCompletedEventArgs e)
        {
            var sourceQueue = (MessageQueue)source;

            try
            {
                var queueMessage = sourceQueue.EndReceive(e.AsyncResult);

                queueMessage.Formatter = new XmlMessageFormatter(new[] { "System.String,mscorlib" });

                MessageReceivedCallback(source, new MessageEventArgs { MessageBody = queueMessage.Body.ToString() });
            }

            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"CustomMessageQueue.Send: {ex.Message}", EventLogEntryType.Error);
            }

            finally
            {
                sourceQueue?.BeginReceive();
            }
        }

        public void Send(string message)
        {
            try
            {
                _messageQueue.Send(message);
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"CustomMessageQueue.Send: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private static string QueueNameLiteral(MessageQueueType queueName)
        {
            var literal = string.Empty;

            switch (queueName)
            {
                case MessageQueueType.DisplaySms:
                    literal = QueueNameDisplaySms;
                    break;
                case MessageQueueType.DisplayPhidget:
                    literal = QueueNameDisplayPhidget;
                    break;
                case MessageQueueType.DisplayVideoCapture:
                    literal = QueueNameDisplayVideoCapture;
                    break;
                case MessageQueueType.DisplayBluetoothBeaconWatcher:
                    literal = QueueNameDisplayBluetoothBeaconWatcher;
                    break;
                case MessageQueueType.Phidget:
                    literal = QueueNamePhidget;
                    break;
                case MessageQueueType.Response:
                    literal = QueueNameResponse;
                    break;
                case MessageQueueType.BluetoothBeaconWatcher:
                    literal = QueueNameBluetoothBeaconWatcher;
                    break;
                case MessageQueueType.BluetoothBeaconWatcherReload:
                    literal = QueueNameBluetoothBeaconWatcherReload;
                    break;
                case MessageQueueType.VideoCapture:
                    literal = QueueNameVideoCapture;
                    break;
                case MessageQueueType.ConfigSms:
                    literal = QueueNameConfigSms;
                    break;
                case MessageQueueType.ConfigPhidget:
                    literal = QueueNameConfigPhidget;
                    break;
                case MessageQueueType.PhidgetContinuousRadio:
                    literal = QueueNamePhidgetContinuousRadio;
                    break;
                case MessageQueueType.BeaconMonitor:
                    literal = QueueNameBeaconMonitor;
                    break;
                case MessageQueueType.BeaconMonitorResident:
                    literal = QueueNameBeaconMonitorResident;
                    break;
                case MessageQueueType.BeaconMonitorState:
                    literal = QueueNameBeaconMonitorState;
                    break;
                case MessageQueueType.VideoCaptureState:
                    literal = QueueNameVideoCaptureState;
                    break;
#if DEBUG
                case MessageQueueType.PhidgetMonitor:
                    literal = QueueNamePhidgetMonitor;
                    break;
                case MessageQueueType.PhidgetMonitorState:
                    literal = QueueNamePhidgetMonitorState;
                    break;
#endif
            }
            return literal;
        }

        private MessageQueue Create(MessageQueueType queueNameType)
        {
            MessageQueue messageQueue;

            try
            {
                var queueName = QueueNameLiteral(queueNameType);
                if (queueName.Length == 0) return null;

                if (MessageQueue.Exists(@".\Private$\" + queueName))
                {
                    messageQueue = new MessageQueue(@".\Private$\" + queueName);
                }
                else
                {
                    MessageQueue.Create(@".\Private$\" + queueName);
                    messageQueue = new MessageQueue(@".\Private$\" + queueName);
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"CustomMessageQueue.Create: {ex.Message}", EventLogEntryType.Error);
                return null;
            }

            return messageQueue;
        }
    }
}
