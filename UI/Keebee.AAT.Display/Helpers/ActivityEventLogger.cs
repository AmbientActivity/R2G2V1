using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using System;
using System.Diagnostics;

namespace Keebee.AAT.Display.Helpers
{
    public class ActivityEventLogger
    {
        private readonly ActivityEventLogsClient _activityEventLogsClient;

        public ActivityEventLogger()
        {
            _activityEventLogsClient = new ActivityEventLogsClient();
        }

        public void Add(int configId, int configDetailId, int residentId, string description = null)
        {
            try
            {
                var activityEventLog = new ActivityEventLog
                {
                    ConfigId = configId,
                    ConfigDetailId = configDetailId,
                    ResidentId = (residentId) > 0 ? residentId : (int?)null,
                    Description = description
                };

                _activityEventLogsClient.Post(activityEventLog);
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"ActivityEventLogger.Add: {ex.Message}", SystemEventLogType.Display, EventLogEntryType.Error);
            }
        }
    }
}
