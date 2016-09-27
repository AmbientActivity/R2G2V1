using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Shared;
using System;
using System.Net;
using System.ServiceProcess;
using System.Threading;

namespace Keebee.AAT.VideoCaptureService
{
    partial class VideoCaptureService : ServiceBase
    {
        // event logger
        private readonly SystemEventLogger _systemEventLogger;

        public VideoCaptureService()
        {
            InitializeComponent();

            _systemEventLogger = new SystemEventLogger(SystemEventLogType.VideoCaptureService);
        }

        protected override void OnStart(string[] args)
        {
            _systemEventLogger.WriteEntry("In OnStart");
        }

        protected override void OnStop()
        {
            _systemEventLogger.WriteEntry("In OnStop");
        }
    }
}
