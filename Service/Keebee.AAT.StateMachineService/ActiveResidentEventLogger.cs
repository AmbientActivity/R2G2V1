using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using Keebee.AAT.SystemEventLogging;
using System;
using System.Diagnostics;

namespace Keebee.AAT.StateMachineService
{
    public class ActiveResidentEventLogger
    {
        private readonly IActiveResidentEventLogsClient _activeReisdentEventLogsClient;

        public ActiveResidentEventLogger()
        {
            _activeReisdentEventLogsClient = new ActiveResidentEventLogsClient();
        }

        public void Add(int residentId, string description)
        {
            try
            {
                var activeResidentEventLog = new ActiveResidentEventLog
                {
                    ResidentId = residentId > 0 ? residentId : (int?)null,
                    Description = description
                };

                _activeReisdentEventLogsClient.Post(activeResidentEventLog);
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"ActiveResidentEventLogger.Add: {ex.Message}", SystemEventLogType.StateMachineService, EventLogEntryType.Error);
            }
        }
    }
}