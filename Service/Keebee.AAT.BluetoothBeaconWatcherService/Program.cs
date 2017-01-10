using System.ServiceProcess;

namespace Keebee.AAT.BluetoothBeaconWatcherService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new BluetoothBeaconWatcherService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
