using System;
using System.ServiceProcess;

namespace Keebee.AAT.VideoCaptureService
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0].Equals("/i", StringComparison.InvariantCultureIgnoreCase))
                {
                    System.Configuration.Install.ManagedInstallerClass.InstallHelper(new string[]
                    {
                        System.Reflection.Assembly.GetExecutingAssembly().Location
                    });
                }
                else if (args[0].Equals("/u", StringComparison.InvariantCultureIgnoreCase))
                {
                    System.Configuration.Install.ManagedInstallerClass.InstallHelper(new string[]
                    {
                        "/u", System.Reflection.Assembly.GetExecutingAssembly().Location
                    });
                }
            }
            else
            {
                var servicesToRun = new ServiceBase[]
                {
                    new VideoCaptureService()
                };
                ServiceBase.Run(servicesToRun);
            }
        }
    }
}
