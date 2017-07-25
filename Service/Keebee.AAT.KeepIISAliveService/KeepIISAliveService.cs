using Keebee.AAT.SystemEventLogging;
using System;
using System.Diagnostics;
using System.Net;
using System.ServiceProcess;
using System.Threading;

namespace Keebee.AAT.KeepIISAliveService
{
    partial class KeepIISAliveService : ServiceBase
    {
        private readonly Thread _keepAliveThread;
        private const string KeepAlivewUrl = "http://localhost/Keebee.AAT.Administrator/Home/Ping";

        public KeepIISAliveService()
        {
            InitializeComponent();

            _keepAliveThread = new Thread(KeepIISAlive);
            _keepAliveThread.Start();
        }

        private static void KeepIISAlive()
        {
            while (true)
            {
                try
                {
                    var req = (HttpWebRequest)WebRequest.Create(KeepAlivewUrl);
                    var response = (HttpWebResponse)req.GetResponse();

                    if (response.StatusCode != HttpStatusCode.OK)
                        SystemEventLogger.WriteEntry(
                            $"Error accessing the web site.{Environment.NewLine}StatusCode: {response.StatusCode}", SystemEventLogType.KeepIISAliveService, EventLogEntryType.Error);

                }
                catch (Exception ex)
                {
                    var message = ex.InnerException?.Message ?? ex.Message;
                    SystemEventLogger.WriteEntry(
                            $"Error accessing the web site.{Environment.NewLine}Message: {message}", SystemEventLogType.KeepIISAliveService, EventLogEntryType.Error);
                }

                try
                {
                    Thread.Sleep(120000);
                }

                catch (ThreadAbortException)
                {
                    break;
                }
            }
        }

        protected override void OnStart(string[] args)
        {
            SystemEventLogger.WriteEntry("In OnStart", SystemEventLogType.KeepIISAliveService);
        }

        protected override void OnStop()
        {
            SystemEventLogger.WriteEntry("In OnStop", SystemEventLogType.KeepIISAliveService);
            _keepAliveThread.Abort();
        }
    }
}
