using System.ServiceProcess;

namespace Keebee.AAT.PhidgetService
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
                new PhidgetService() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
