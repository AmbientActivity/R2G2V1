using System.ServiceProcess;

namespace Keebee.AAT.VideoCaptureService
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
                new VideoCaptureService() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
