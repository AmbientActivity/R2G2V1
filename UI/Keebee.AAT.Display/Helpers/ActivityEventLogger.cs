using Keebee.AAT.SystemEventLogging;
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

        private SystemEventLogger _systemEventLogger;
        public SystemEventLogger EventLogger
        {
            set { _systemEventLogger = value; }
        }

        public void Add(int residentId, int activityTypeId, int responseTypeId, string description = null)
        {
            try
            { 
                var activityEventLog = new ActivityEventLog
                {
                    ResidentId = (residentId) > 0 ? residentId : (int?)null,
                    ActivityTypeId = activityTypeId,
                    ResponseTypeId = responseTypeId,
                    Description = description
                };

                _opsClient.PostActivityEventLog(activityEventLog);
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"ActivityEventLogger.Add: {ex.Message}", EventLogEntryType.Error);
            }
        }
    }
}
