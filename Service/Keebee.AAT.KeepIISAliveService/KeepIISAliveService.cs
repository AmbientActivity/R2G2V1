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
        // event logger
        private readonly SystemEventLogger _systemEventLogger;
        private readonly Thread _keepAliveThread;
        private const string KeepAlivewUrl = "http://localhost/Keebee.AAT.Administrator/Home/Ping";

        public KeepIISAliveService()
        {
            InitializeComponent();

            _systemEventLogger = new SystemEventLogger(SystemEventLogType.KeepIISAliveService);

            _keepAliveThread = new Thread(KeepIISAlive);
            _keepAliveThread.Start();
        }

        private void KeepIISAlive()
        {
            while (true)
            {
                try
                {
                    var req = (HttpWebRequest)WebRequest.Create(KeepAlivewUrl);
                    var response = (HttpWebResponse)req.GetResponse();

                    if (response.StatusCode != HttpStatusCode.OK)
                        _systemEventLogger.WriteEntry(
                            $"Error accessing the web site.{Environment.NewLine}StatusCode: {response.StatusCode}", EventLogEntryType.Error);

                }
                catch (Exception ex)
                {
                    var message = ex.InnerException?.Message ?? ex.Message;
                    _systemEventLogger.WriteEntry(
                            $"Error accessing the web site.{Environment.NewLine}Message: {message}", EventLogEntryType.Error);
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
            _systemEventLogger.WriteEntry("In OnStart");
        }

        protected override void OnStop()
        {
            _systemEventLogger.WriteEntry("In OnStop");
            _keepAliveThread.Abort();
        }
    }
}
