using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using System;
using System.Diagnostics;

namespace Keebee.AAT.Display.Helpers
{
    public class InteractiveActivityEventLogger
    {
        private readonly InteractiveActivityEventLogsClient _client = new InteractiveActivityEventLogsClient();

        public void Add(int residentId, int interactiveActivityTypeId, string description, int? difficultyLevel = null, bool? isSuccess = null)
        {
            try
            {
                var interactiveActivityEventLog = new InteractiveActivityEventLog
                                     {
                                         ResidentId = residentId > 0 ? residentId : (int?)null,
                                         InteractiveActivityTypeId = interactiveActivityTypeId,
                                         DifficultyLevel = difficultyLevel,
                                         IsSuccess = isSuccess,
                                         Description = description
                                     };

                _client.Post(interactiveActivityEventLog);
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"InteractiveActivityEventLogger.Add: {ex.Message}", SystemEventLogType.Display, EventLogEntryType.Error);
            }
        }
    }
}
