using System;
using System.Diagnostics;
using System.Linq;

namespace Keebee.AAT.EventLogging
{
    public enum EventLogType
    {
        Display = 1,
        Simulator = 2,
        KeebeeMessageQueuing = 3,
        StateMachineService = 4,
        RfidReaderService = 5,
        PhidgetService = 6
    }

    public class EventLogger
    {
        private const string EventLogDisplay = "Keebee Display";
        private const string EventLogSimulator = "Keebee Activity Simulator";
        private const string EventLogMessageQueuing = "Keebee Message Queuing";
        private const string EventLogStateMachineService = "Keebee State Machine Service";
        private const string EventLogRfidReaderService = "Keebee RFID Reader Service";
        private const string EventLogPhidgetService = "Keebee Phidget Service";

        private EventLog _eventLog;
        public EventLog EventLog
        {
            set { _eventLog = value; }
        }

        public int EventId { get; private set; }

        public EventLogger(EventLogType eventLogType)
        {
            _eventLog = new EventLog();
            EventId = Initialize(EventLogLiteral(eventLogType));
        }

        public void WriteEntry(string message, EventLogEntryType type = EventLogEntryType.Information)
        {
            _eventLog.WriteEntry(message, type, EventId++);
        }

        private static string EventLogLiteral(EventLogType queueName)
        {
            var literal = string.Empty;

            switch (queueName)
            {
                case EventLogType.Display:
                    literal = EventLogDisplay;
                    break;
                case EventLogType.Simulator:
                    literal = EventLogSimulator;
                    break;
                case EventLogType.KeebeeMessageQueuing:
                    literal = EventLogMessageQueuing;
                    break;
                case EventLogType.StateMachineService:
                    literal = EventLogStateMachineService;
                    break;
                case EventLogType.RfidReaderService:
                    literal = EventLogRfidReaderService;
                    break;
                case EventLogType.PhidgetService:
                    literal = EventLogPhidgetService;
                    break;
            }
            return literal;
        }

        private int Initialize(string name)
        {
            // remove spaces for the source
            var source = string.Join("", name.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));

            if (!EventLog.SourceExists(source))
            {
                EventLog.CreateEventSource(source, name);
            }
            _eventLog.Source = source;
            _eventLog.Log = name;

            UInt16 nextEventId = 1;
            try
            {
                var entries = _eventLog.Entries.Cast<EventLogEntry>().ToArray();
                if (entries.Any())
                {
                    long lastEventId = _eventLog.Entries.Cast<EventLogEntry>().Last().InstanceId;

                    // increment the eventId if it is less than the max value for UInt16
                    // otherwise reset it to 1
                    if (lastEventId < UInt16.MaxValue)
                        nextEventId = (UInt16)(lastEventId + 1);
                    else
                        nextEventId = 1;
                }
            }

            catch (Exception)
            {
                return 1;
            }

            return nextEventId;
        }
    }
}
