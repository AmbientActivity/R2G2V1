using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using System;
using System.Diagnostics;

namespace Keebee.AAT.Main.Helpers
{
    public class ActivityEventLogger
    {
        private readonly ActivityEventLogsClient _activityEventLogsClient;

        private SystemEventLogger _systemEventLogger;
        public SystemEventLogger EventLogger
        {
            set { _systemEventLogger = value; }
        }

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
                _systemEventLogger?.WriteEntry($"ActivityEventLogger.Add: {ex.Message}", EventLogEntryType.Error);
            }
        }
    }
}
