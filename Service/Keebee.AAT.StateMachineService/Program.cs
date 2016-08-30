using System.ServiceProcess;

namespace Keebee.AAT.StateMachineService
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
                new StateMachineService() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
