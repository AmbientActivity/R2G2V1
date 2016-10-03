using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.MessageQueuing;
using Keebee.AAT.Shared;
using System;
using System.Configuration;
using System.Threading;
using System.Web.Script.Serialization;
using System.ServiceProcess;
using System.Diagnostics;
using System.Linq;
using Microsoft.Expression.Encoder.Devices;
using Microsoft.Expression.Encoder.Live;

namespace Keebee.AAT.VideoCaptureService
{
    internal partial class VideoCaptureService : ServiceBase
    {
        private const int VideoDuration = 120; // in seconds
        // event logger
        private readonly SystemEventLogger _systemEventLogger;

        private readonly LiveJob _job;

        // display state
        private bool _displayIsActive;

        public VideoCaptureService()
        {
            InitializeComponent();

            _systemEventLogger = new SystemEventLogger(SystemEventLogType.VideoCaptureService);

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

            _job = new LiveJob();
            InitializeEncoderDevices();
        }

        private void InitializeEncoderDevices()
        {
            var videoDeviceName = ConfigurationManager.AppSettings["VideoDevice"];
            var audioDeviceName = ConfigurationManager.AppSettings["AudioDevice"];
            var videoDevice = EncoderDevices.FindDevices(EncoderDeviceType.Video).Single(x => x.Name == videoDeviceName);
            var audioDevice = EncoderDevices.FindDevices(EncoderDeviceType.Audio).Single(x => x.Name == audioDeviceName);
            var deviceSource = _job.AddDeviceSource(videoDevice, audioDevice);

            _job.ActivateSource(deviceSource);
        }

        private void StartCapture()
        {
            var fileOut = new FileArchivePublishFormat
            {
                OutputFileName = $@"C:\VideoCaptures\Capture_{DateTime.Now:yyyyMMdd_hhmmss}.wmv"
            };

            _job.PublishFormats.Add(fileOut);
            _job.StartEncoding();
        }

        private void MessageReceivedVideoCapture(object source, MessageEventArgs e)
        {
            try
            {
                // do nothing unless the display is active
                if (!_displayIsActive) return;

                if (!_job.IsCapturing)
                {
                    StartCapture();

                    for (var second = 0; second <= VideoDuration; second++)
                    {
                        if (_displayIsActive)
                            Thread.Sleep(1000);
                        else break;
                    }

                    _job.StopEncoding();
                    _job.PublishFormats.Clear();
                }
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
