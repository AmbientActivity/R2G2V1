using Keebee.AAT.Constants;
using Keebee.AAT.EventLogging;
using Keebee.AAT.RESTClient;
using System;
using System.Diagnostics;

namespace Keebee.AAT.Display.Helpers
{
    public class ActivityEventLogger
    {

        private OperationsClient _opsClient;
        public OperationsClient OperationsClient
        {
            set { _opsClient = value; }
        }

        private EventLogger _eventLogger;
        public EventLogger EventLogger
        {
            set { _eventLogger = value; }
        }

        public void Add(int residentId, int activityTypeId, int responseTypeId, string description = null)
        {
            try
            { 
            var eventLog = new RESTClient.EventLog
            {
                ResidentId = (residentId) > 0 ? residentId : (int?)null,
                EventLogEntryTypeId = UserEventLogEntryType.SensorActivated,
                ActivityTypeId = activityTypeId,
                ResponseTypeId = responseTypeId,
                Description = description
            };

            _opsClient.PostEventLog(eventLog);
            }
            catch (Exception ex)
            {
                _eventLogger?.WriteEntry($"ActivityEventLogger.Add: {ex.Message}", EventLogEntryType.Error);
            }
        }
    }
}
