using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.MessageQueuing;
using Keebee.AAT.Shared;
using DirectX.Capture;
using System;
using System.Threading;
using System.Web.Script.Serialization;
using System.ServiceProcess;
using System.Diagnostics;

namespace Keebee.AAT.VideoCaptureService
{
    partial class VideoCaptureService : ServiceBase
    {
        // event logger
        private readonly SystemEventLogger _systemEventLogger;

        private readonly Capture _capture;
        private readonly Filters _filters = new Filters();

        // display state
        private bool _displayIsActive;

        public VideoCaptureService()
        {
            InitializeComponent();

            _systemEventLogger = new SystemEventLogger(SystemEventLogType.VideoCaptureService);

            _capture = new Capture(_filters.VideoInputDevices[0], _filters.AudioInputDevices[0]);
            _capture.CaptureComplete += new EventHandler(OnCaptureComplete);

            var q1 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.VideoCapture,
                MessageReceivedCallback = MessageReceivedVideoCapture
            })
            { SystemEventLogger = _systemEventLogger };

            var q2 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.DisplayVideoCapture,
                MessageReceivedCallback = MessageReceivedDisplayVideoCapture
            })
            { SystemEventLogger = _systemEventLogger };
        }

        private void StartCapture()
        {
            try
            {
                var mediaPath = new MediaSourcePath();
                //if (!_capture.Cued)
                var filename = $"Capture_{DateTime.Now.Ticks}.avi";

                //_capture.Filename = $@"{mediaPath.MediaRoot}\VideoCaptures\{filename}";
                _capture.Filename = $@"C:\VideoCaptures\{filename}";
                _capture.Start();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"StartCapture: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void StopCapture()
        {
            _capture.Stop();
        }

        private void MessageReceivedVideoCapture(object source, MessageEventArgs e)
        {
            try
            {
                // do nothing unless the display is active
                if (!_displayIsActive) return;

                //if (_capture.Stopped)
                //{
                //    StartCapture();
                //    Thread.Sleep(5000);
                //    StopCapture();
                //}

            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"MessageReceivedVideoCapture: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void MessageReceivedDisplayVideoCapture(object source, MessageEventArgs e)
        {
            try
            {
                var displayMessage = GetDisplayStateFromMessageBody(e.MessageBody);
                _displayIsActive = displayMessage.IsActive;
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"MessageReceivedDisplayVideoCapture{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
            }
        }

        private static DisplayMessage GetDisplayStateFromMessageBody(string messageBody)
        {
            var serializer = new JavaScriptSerializer();
            var display = serializer.Deserialize<DisplayMessage>(messageBody);
            return display;
        }

        private static void OnCaptureComplete(object sender, EventArgs e)
        {
        }

        protected override void OnStart(string[] args)
        {
            _systemEventLogger.WriteEntry("In OnStart");
        }

        protected override void OnStop()
        {
            _systemEventLogger.WriteEntry("In OnStop");

            if (_capture != null)
                _capture.Stop();
        }
    }
}
