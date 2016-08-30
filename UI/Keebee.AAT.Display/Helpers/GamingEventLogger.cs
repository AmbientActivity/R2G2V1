using Keebee.AAT.RESTClient;
using Keebee.AAT.EventLogging;
using System;
using System.Diagnostics;

namespace Keebee.AAT.Display.Helpers
{
    public class GamingEventLogger
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

        public void Add(int residentId, int eventLogEntryTypeId, int difficultyLevel, bool success, string description)
        {
            try
            {
                var gamingEventLog = new GamingEventLog
                                     {
                                         ResidentId = residentId > 0 ? residentId : (int?)null,
                                         EventLogEntryTypeId = eventLogEntryTypeId,
                                         DifficultyLevel = difficultyLevel,
                                         IsSuccess = success,
                                         Description = description
                                     };

                _opsClient.PostGamingEventLog(gamingEventLog);
            }
            catch (Exception ex)
            {
                _eventLogger?.WriteEntry($"GamingEventLogger.Add: {ex.Message}", EventLogEntryType.Error);
            }
        }
    }
}
