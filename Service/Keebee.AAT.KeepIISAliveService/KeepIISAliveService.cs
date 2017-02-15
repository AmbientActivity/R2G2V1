using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Shared;
using System;
using System.Net;
using System.ServiceProcess;
using System.Threading;

namespace Keebee.AAT.KeepIISAliveService
{
    internal static class KeepAliveUrl
    {
        public const string OperationsApi = "http://localhost/Keebee.AAT.Operations/api/status";
        public const string AdministratorHome = "http://localhost/Keebee.AAT.Administrator";
    }

    partial class KeepIISAliveService : ServiceBase
    {
        // event logger
        private readonly SystemEventLogger _systemEventLogger;
        private readonly Thread _keepAliveThread;
        private readonly Thread _keepAliveThreadAdmin;

        public KeepIISAliveService()
        {
            InitializeComponent();

            _systemEventLogger = new SystemEventLogger(SystemEventLogType.KeepIISAliveService);

            _keepAliveThread = new Thread(KeepAliveOperations);
            _keepAliveThread.Start();

            _keepAliveThreadAdmin = new Thread(KeepAliveAdministrator);
            _keepAliveThreadAdmin.Start();
        }

        private void KeepAliveOperations()
        {
            while (true)
            {
                try
                {
                    var req = (HttpWebRequest)WebRequest.Create(KeepAliveUrl.OperationsApi);
                    var response = (HttpWebResponse)req.GetResponse();

                    if (response.StatusCode != HttpStatusCode.OK)
                        _systemEventLogger.WriteEntry(
                            $"Error accessing api.{Environment.NewLine}StatusCode: {response.StatusCode}");
                }
                catch (Exception ex)
                {
                    _systemEventLogger.WriteEntry(
                            $"Error accessing api.{Environment.NewLine}Error: {ex.Message}");
                }

                try
                {
                    Thread.Sleep(60000);
                }

                catch (ThreadAbortException)
                {
                    break;
                }
            }
        }

        private void KeepAliveAdministrator()
        {
            while (true)
            {
                try
                {
                    var req = (HttpWebRequest)WebRequest.Create(KeepAliveUrl.AdministratorHome);
                    var response = (HttpWebResponse)req.GetResponse();


                    if (response.StatusCode != HttpStatusCode.OK)
                        _systemEventLogger.WriteEntry(
                            $"Error accessing admin sitet.{Environment.NewLine}StatusCode: {response.StatusCode}");

                }
                catch (Exception ex)
                {
                    _systemEventLogger.WriteEntry(
                            $"Error accessing admin site.{Environment.NewLine}Error: {ex.Message}");
                }

                try
                {
                    Thread.Sleep(60000);
                }

                catch (ThreadAbortException)
                {
                    break;
                }
            }
        }

        protected override void OnStart(string[] args)
        {
            _systemEventLogger.WriteEntry("In OnStart");
        }

        protected override void OnStop()
        {
            _systemEventLogger.WriteEntry("In OnStop");
            _keepAliveThread.Abort();
            _keepAliveThreadAdmin.Abort();
        }
    }
}
