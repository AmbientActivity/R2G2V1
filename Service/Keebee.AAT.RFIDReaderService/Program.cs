using System.ServiceProcess;

namespace Keebee.AAT.RfidReaderService
{
    internal static class Program
    {
        private static void Main()
        {
            var servicesToRun = new ServiceBase[] 
                                          { 
                                              new RfidReaderService() 
                                          };

            ServiceBase.Run(servicesToRun);
        }
    }
}
