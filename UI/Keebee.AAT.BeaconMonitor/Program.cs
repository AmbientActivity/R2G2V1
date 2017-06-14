using Keebee.AAT.Shared;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Keebee.AAT.BeaconMonitor
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static readonly Mutex Mutex = new Mutex(true, "{8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE8F}");

        [STAThread]
        private static void Main()
        {
            if (Mutex.WaitOne(TimeSpan.Zero, true))
            {
                try
                {
                    if (IsBeaconWatcherInstalled())
                    {
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Application.Run(new Monitor());
                    }
                    else
                    {
                        MessageBox.Show("Bluetooth Beacon Watcher Service is not installed.");
                    }
                }
                finally
                {
                    Mutex.ReleaseMutex();
                }
            }
            else
            {
                MessageBox.Show("Beacon Montitor is already running.");
            }
        }

        public static bool IsBeaconWatcherInstalled()
        {
            var processes = Process.GetProcessesByName($@"{AppSettings.Namespace}.{ServiceName.BluetoothBeaconWatcher}");
            return (processes.Any());
        }
    }
}
