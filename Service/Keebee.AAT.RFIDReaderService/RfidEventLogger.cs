using Keebee.AAT.RESTClient;
using Keebee.AAT.SystemEventLogging;
using System;
using System.Diagnostics;

namespace Keebee.AAT.RfidReaderService
{
    public class RfidEventLogger
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

        public void Add(int residentId, string description)
        {
            try
            {
                var rfidEventLog = new RfidEventLog
                {
                    ResidentId = residentId > 0 ? residentId : (int?)null,
                    Description = description
                };

                _opsClient.PostRfidEventLog(rfidEventLog);
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"RfidEventLogger.Add: {ex.Message}", EventLogEntryType.Error);
            }
        }
    }
}