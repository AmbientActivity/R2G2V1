using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.MessageQueuing;
using Keebee.AAT.Shared;
using System;
using System.Configuration;
using System.Web.Script.Serialization;
using System.ServiceProcess;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using Microsoft.Expression.Encoder.Devices;
using Microsoft.Expression.Encoder.Live;

namespace Keebee.AAT.VideoCaptureService
{
    internal partial class VideoCaptureService : ServiceBase
    {
        private const string OutputPath = @"C:\VideoCaptures";
        private readonly Timer _timer;
        private const int VideoDuration = 120000; // in milliseconds

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

            _timer = new Timer(VideoDuration);
            _timer.Elapsed += OnTimerElapsed;
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
            try
            {
                var fileOut = new FileArchivePublishFormat
                {
                    OutputFileName = $@"{OutputPath}\{DateTime.Now:yyyyMMdd_hhmmss}.wmv"
                };

                _job.PublishFormats.Add(fileOut);
                _job.StartEncoding();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"StartCapture: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void StopCapture()
        {
            try
            {
                if (!_job.IsCapturing) return;

                _job.StopEncoding();
                _job.PublishFormats.Clear();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"StopCapture: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void OnTimerElapsed(object source, ElapsedEventArgs e)
        {
            StopCapture();
            _timer.Stop();
        }

        private void MessageReceivedVideoCapture(object source, MessageEventArgs e)
        {
            if (!_displayIsActive || _job.IsCapturing) return;

            StartCapture();
            _timer.Start();
        }

        private void MessageReceivedDisplayVideoCapture(object source, MessageEventArgs e)
        {
            try
            {
                var displayMessage = GetDisplayStateFromMessageBody(e.MessageBody);
                _displayIsActive = displayMessage.IsActive;

                if (!_displayIsActive)
                {
                    _timer.Stop();
                    StopCapture();
                }
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
            _timer.Stop();
            _timer.Dispose();
        }
    }
}
