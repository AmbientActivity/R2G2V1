using System;
using System.Diagnostics;
using System.Linq;

namespace Keebee.AAT.SystemEventLogging
{
    public enum SystemEventLogType
    {
        Display = 1,
        Simulator = 2,
        KeebeeMessageQueuing = 3,
        StateMachineService = 4,
        RfidReaderService = 5,
        PhidgetService = 6,
        EventLog = 7,
        AutomatedExport = 8,
        FileManager = 9
    }

    public class SystemEventLogger
    {
        private const string EventLogDisplay = "R2G2 Display";
        private const string EventLogSimulator = "R2G2 Activity Simulator";
        private const string EventLogMessageQueuing = "R2G2 Message Queuing";
        private const string EventLogStateMachineService = "R2G2 State Machine Service";
        private const string EventLogRfidReaderService = "R2G2 RFID Reader Service";
        private const string EventLogPhidgetService = "R2G2 Phidget Service";
        private const string EventLogEventLog = "R2G2 Event Log";
        private const string EventLogAutomatedExport = "R2G2 Automated Export";
        private const string EventLogFileManager = "R2G2 File Manager";

        private EventLog _eventLog;
        public EventLog EventLog
        {
            set { _eventLog = value; }
        }

        public int EventId { get; private set; }

        public SystemEventLogger(SystemEventLogType systemEventLogType)
        {
            _eventLog = new EventLog();
            EventId = Initialize(EventLogLiteral(systemEventLogType));
        }

        public void WriteEntry(string message, EventLogEntryType type = EventLogEntryType.Information)
        {
            _eventLog.WriteEntry(message, type, EventId++);
        }

        private static string EventLogLiteral(SystemEventLogType queueName)
        {
            var literal = string.Empty;

            switch (queueName)
            {
                case SystemEventLogType.Display:
                    literal = EventLogDisplay;
                    break;
                case SystemEventLogType.Simulator:
                    literal = EventLogSimulator;
                    break;
                case SystemEventLogType.KeebeeMessageQueuing:
                    literal = EventLogMessageQueuing;
                    break;
                case SystemEventLogType.StateMachineService:
                    literal = EventLogStateMachineService;
                    break;
                case SystemEventLogType.RfidReaderService:
                    literal = EventLogRfidReaderService;
                    break;
                case SystemEventLogType.PhidgetService:
                    literal = EventLogPhidgetService;
                    break;
                case SystemEventLogType.EventLog:
                    literal = EventLogEventLog;
                    break;
                case SystemEventLogType.AutomatedExport:
                    literal = EventLogAutomatedExport;
                    break;
                case SystemEventLogType.FileManager:
                    literal = EventLogFileManager;
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
