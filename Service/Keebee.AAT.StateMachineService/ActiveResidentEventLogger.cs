using Keebee.AAT.RESTClient;
using Keebee.AAT.SystemEventLogging;
using System;
using System.Diagnostics;

namespace Keebee.AAT.StateMachineService
{
    public class ActiveResidentEventLogger
    {
        private IOperationsClient _opsClient;
        public IOperationsClient OperationsClient
        {
            set { _opsClient = value; }
        }

        private SystemEventLogger _systemEventLogger;
        public SystemEventLogger SystemEventLogger
        {
            set { _systemEventLogger = value; }
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

                _opsClient.PostActiveResidentEventLog(activeResidentEventLog);
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"ActiveResidentEventLogger.Add: {ex.Message}", EventLogEntryType.Error);
            }
        }
    }
}