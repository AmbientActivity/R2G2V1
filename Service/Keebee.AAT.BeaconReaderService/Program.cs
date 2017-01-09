using System.ServiceProcess;

namespace Keebee.AAT.BeaconReaderService
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
                new BeaconReaderService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
