using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Shared;
using System.Diagnostics;
using System;

namespace Keebee.AAT.BusinessRules
{
    public class ServicesRules
    {
        private SystemEventLogger _systemEventLogger;
        public SystemEventLogger EventLogger
        {
            set { _systemEventLogger = value; }
        }

        public string Install(ServiceUtilities.ServiceType type, string path, bool install)
        {
            try
            {
                var isInstalled = ServiceUtilities.IsInstalled(type);
                string msg = null;

                if ((install && !isInstalled) || (!install && isInstalled))
                {
                    msg = ServiceUtilities.Install(type, path, install);
                }

                return msg;
            }

            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"ServicesRules.Install: {ex.Message}", EventLogEntryType.Error);
                return ex.Message;
            }
        }

        public static bool IsInstalled(ServiceUtilities.ServiceType type)
        {
            switch (type)
            {
                case ServiceUtilities.ServiceType.BluetoothBeaconWatcher:
                    return ServiceUtilities.IsInstalled(ServiceUtilities.ServiceType.BluetoothBeaconWatcher);
                case ServiceUtilities.ServiceType.VideoCapture:
                    return ServiceUtilities.IsInstalled(ServiceUtilities.ServiceType.VideoCapture);
                default:
                    return false;
            }          
        }
    }
}
