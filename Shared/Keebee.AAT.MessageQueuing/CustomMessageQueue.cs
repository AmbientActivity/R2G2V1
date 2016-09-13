﻿using Keebee.AAT.SystemEventLogging;
using System;
using System.Diagnostics;
using System.Messaging;

namespace Keebee.AAT.MessageQueuing
{
    public enum MessageQueueType
    {
        Display = 1,
        Phidget = 2,
        Response = 3,
        Rfid = 4,
        Video = 5,
        ConfigSms = 6,
        ConfigPhidget = 7
#if DEBUG
        , PhidgetMonitor = 8,
        PhidgetMonitorState = 9,
        RfidMonitor = 10,
        RfidMonitorState = 11
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
        private const string QueueNameDisplay = "Display";
        private const string QueueNamePhidget = "Phidget";
        private const string QueueNameResponse = "Response";
        private const string QueueNameRfid = "RFID";
        private const string QueueNameVideo = "Video";
        private const string QueueNameConfigSms = "Config-SMS";
        private const string QueueNameConfigPhidget = "Config-Phidget";
#if DEBUG
        private const string QueueNamePhidgetMonitor = "Phidget-Monitor";
        private const string QueueNamePhidgetMonitorState = "Phidget-Monitor-State";

        private const string QueueNameRFIDMonitor = "RFID-Monitor";
        private const string QueueNameRFIDMonitorState = "RFID-Monitor-State";
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

                queueMessage.Formatter = new XmlMessageFormatter(new[] {"System.String,mscorlib"});

                MessageReceivedCallback(source, new MessageEventArgs { MessageBody = queueMessage.Body.ToString() });
            }

            catch(Exception ex)
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
                case MessageQueueType.Display:
                    literal = QueueNameDisplay;
                    break;
                case MessageQueueType.Phidget:
                    literal = QueueNamePhidget;
                    break;
                case MessageQueueType.Response:
                    literal = QueueNameResponse;
                    break;
                case MessageQueueType.Rfid:
                    literal = QueueNameRfid;
                    break;
                case MessageQueueType.Video:
                    literal = QueueNameVideo;
                    break;
                case MessageQueueType.ConfigSms:
                    literal = QueueNameConfigSms;
                    break;
                case MessageQueueType.ConfigPhidget:
                    literal = QueueNameConfigPhidget;
                    break;
#if DEBUG
                case MessageQueueType.PhidgetMonitor:
                    literal = QueueNamePhidgetMonitor;
                    break;
                case MessageQueueType.PhidgetMonitorState:
                    literal = QueueNamePhidgetMonitorState;
                    break;
                case MessageQueueType.RfidMonitor:
                    literal = QueueNameRFIDMonitor;
                    break;
                case MessageQueueType.RfidMonitorState:
                    literal = QueueNameRFIDMonitorState;
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
