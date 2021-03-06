﻿using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Shared;
using System.Diagnostics;
using System;

namespace Keebee.AAT.BusinessRules
{
    public class ServicesRules
    {
        public string Install(ServiceUtilities.ServiceType type, string path, bool install)
        {
            try
            {
                if (!ServiceUtilities.IsInstalled(ServiceUtilities.ServiceType.StateMachine))
                {
                    return "The State Machine Service is not installed";
                }

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
                SystemEventLogger.WriteEntry($"ServicesRules.Install: {ex.Message}", SystemEventLogType.AdminInterface, EventLogEntryType.Error);
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
