using System;
using System.Diagnostics;
using System.Linq;

namespace Keebee.AAT.SystemEventLogging
{
    public enum SystemEventLogType
    {
        Display = 1,
        Simulator = 2,
        StateMachineService = 3,
        BluetoothBeaconWatcherService = 4,
        PhidgetService = 5,
        VideoCaptureService = 6,
        KeepIISAliveService = 7,
        EventLog = 8,
        AutomatedExport = 9,
        AdminInterface = 10
    }

    public static class SystemEventLogger
    {
        private const string EventLogDisplay = "ABBY Display";
        private const string EventLogStateMachineService = "ABBY State Machine Service";
        private const string EventLogBluetoothBeaconWatcherService = "ABBY Bluetooth Beacon Watcher Service";
        private const string EventLogPhidgetService = "ABBY Phidget Service";
        private const string EventLogVideoCaptureService = "ABBY Video Capture Service";
        private const string EventLogKeepIISAliveService = "ABBY Keep IIS Alive Service";
        private const string EventLogEventLog = "ABBY Event Log";
        private const string EventLogAutomatedExport = "ABBY Automated Export";
        private const string EventLogAdminInterface = "ABBY Administrator Interface";

        public static void WriteEntry(string message, SystemEventLogType systemeEntryType, EventLogEntryType type = EventLogEntryType.Information)
        {
            using (var eventLog = new EventLog())
            {
                var eventId = Initialize(eventLog, EventLogLiteral(systemeEntryType)) + 1;
                eventLog.WriteEntry(message, type, eventId);
            }
        }

        public static void Clear(SystemEventLogType systemeEntryType)
        {
            using (var eventLog = new EventLog())
            {
                var eventId = Initialize(eventLog, EventLogLiteral(systemeEntryType)) + 1;
                eventLog.Clear();
            }
        }

        private static string EventLogLiteral(SystemEventLogType queueName)
        {
            var literal = string.Empty;

            switch (queueName)
            {
                case SystemEventLogType.Display:
                    literal = EventLogDisplay;
                    break;
                case SystemEventLogType.StateMachineService:
                    literal = EventLogStateMachineService;
                    break;
                case SystemEventLogType.PhidgetService:
                    literal = EventLogPhidgetService;
                    break;
                case SystemEventLogType.BluetoothBeaconWatcherService:
                    literal = EventLogBluetoothBeaconWatcherService;
                    break;
                case SystemEventLogType.VideoCaptureService:
                    literal = EventLogVideoCaptureService;
                    break;
                case SystemEventLogType.EventLog:
                    literal = EventLogEventLog;
                    break;
                case SystemEventLogType.AutomatedExport:
                    literal = EventLogAutomatedExport;
                    break;
                case SystemEventLogType.AdminInterface:
                    literal = EventLogAdminInterface;
                    break;
                case SystemEventLogType.KeepIISAliveService:
                    literal = EventLogKeepIISAliveService;
                    break;
            }
            return literal;
        }

        private static int Initialize(EventLog eventLog, string name)
        {
            // remove spaces for the source
            var source = string.Join("", name.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));

            if (!EventLog.SourceExists(source))
            {
                EventLog.CreateEventSource(source, name);
            }
            eventLog.Source = source;
            eventLog.Log = name;

            UInt16 nextEventId = 1;
            try
            {
                var entries = eventLog.Entries.Cast<EventLogEntry>().ToArray();
                if (entries.Any())
                {
                    long lastEventId = eventLog.Entries.Cast<EventLogEntry>().Last().InstanceId;

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
