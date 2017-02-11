using Keebee.AAT.ApiClient;
using Keebee.AAT.SystemEventLogging;
using System;
using System.Diagnostics;

namespace Keebee.AAT.Display.Helpers
{
    public class InteractiveActivityEventLogger
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

                _opsClient.PostInteractiveActivityEventLog(interactiveActivityEventLog);
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"InteractiveActivityEventLogger.Add: {ex.Message}", EventLogEntryType.Error);
            }
        }
    }
}
