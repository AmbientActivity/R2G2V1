using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;

namespace Keebee.AAT.Shared
{
    public class ServiceUtilities
    {
        public enum ServiceType
        {
            StateMachine = 0,
            Phidget = 1,
            KeepIISAlive = 2,
            BluetoothBeaconWatcher = 4,
            VideoCapture = 5
        }

        public static bool IsInstalled(ServiceType type)
        {
            var svc = ServiceController.GetServices()
                .FirstOrDefault(s => s.ServiceName == GetServiceName(type));

            return (svc != null);
        }

        public static string Install(ServiceType type, string path, bool install)
        {
            try
            {
                var args = install ? "/i" : "/u";
                var p = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = $@"{path}\{AppSettings.Namespace}.{GetServiceName(type)}.exe",
                        Arguments = args
                    }
                };

                p.Start();
                p.WaitForExit();
            }
            catch (Exception ex)
            {
                return ex.InnerException?.Message ?? ex.Message;
            }

            return null;
        }

        private static string GetServiceName(ServiceType type)
        {
            switch (type)
            {
                case ServiceType.StateMachine:
                    return ServiceName.StateMachine;
                case ServiceType.Phidget:
                    return ServiceName.Phidget;
                case ServiceType.KeepIISAlive:
                    return ServiceName.KeepIISAlive;
                case ServiceType.BluetoothBeaconWatcher:
                    return ServiceName.BluetoothBeaconWatcher;
                case ServiceType.VideoCapture:
                    return ServiceName.VideoCapture;
                default:
                    return string.Empty;
            }

        }
    }
}
