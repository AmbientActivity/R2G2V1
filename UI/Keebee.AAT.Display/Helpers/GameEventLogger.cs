using Keebee.AAT.RESTClient;
using Keebee.AAT.SystemEventLogging;
using System;
using System.Diagnostics;

namespace Keebee.AAT.Display.Helpers
{
    public class GameEventLogger
    {
        private OperationsClient _opsClient;
        public OperationsClient OperationsClient
        {
            set { _opsClient = value; }
        }

        private SystemEventLogger _systemEventLogger;
        public SystemEventLogger SystemEventLogger
        {
            set { _systemEventLogger = value; }
        }

        public void Add(int residentId, int gameTypeId, int difficultyLevel, bool? isSuccess, string description)
        {
            try
            {
                var gamingEventLog = new GameEventLog
                                     {
                                         ResidentId = residentId > 0 ? residentId : (int?)null,
                                         GameTypeId = gameTypeId,
                                         DifficultyLevel = difficultyLevel,
                                         IsSuccess = isSuccess,
                                         Description = description
                                     };

                _opsClient.PostGameEventLog(gamingEventLog);
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"GameEventLogger.Add: {ex.Message}", EventLogEntryType.Error);
            }
        }
    }
}
