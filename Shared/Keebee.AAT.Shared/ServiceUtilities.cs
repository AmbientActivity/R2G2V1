using System;
using System.Diagnostics;
using System.Linq;

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
            var processes = Process.GetProcessesByName(GetServiceName(type));
            return (processes.Any());
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
                        FileName = $@"{path}\{GetServiceName(type)}.exe",
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
                    return $"{AppSettings.Namespace}.{ServiceName.StateMachine}";
                case ServiceType.Phidget:
                    return $"{AppSettings.Namespace}.{ServiceName.Phidget}";
                case ServiceType.KeepIISAlive:
                    return $"{AppSettings.Namespace}.{ServiceName.KeepIISAlive}";
                case ServiceType.BluetoothBeaconWatcher:
                    return $"{AppSettings.Namespace}.{ServiceName.BluetoothBeaconWatcher}";
                case ServiceType.VideoCapture:
                    return $"{AppSettings.Namespace}.{ServiceName.VideoCapture}";
                default:
                    return string.Empty;
            }

        }
    }
}
