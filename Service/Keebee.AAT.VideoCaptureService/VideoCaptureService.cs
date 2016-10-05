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
        private readonly Timer _timer;

        // output path
        private const string OutputPath = VideoCaptures.Path;

        // app.config settings
        private string _videoSourceName;
        private string _audioSourceName;
        private int _videoDuration;

        // expression 4 encoder
        private readonly LiveJob _job = new LiveJob();
        private LiveDeviceSource _deviceSource;
        private EncoderDevice _videoDevice;
        private EncoderDevice _audioDevice;

        // event logger
        private readonly SystemEventLogger _systemEventLogger;

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
                {SystemEventLogger = _systemEventLogger};

            var q2 = new CustomMessageQueue(new CustomMessageQueueArgs
                {
                    QueueName = MessageQueueType.DisplayVideoCapture,
                    MessageReceivedCallback = MessageReceivedDisplayVideoCapture
                })
                {SystemEventLogger = _systemEventLogger};

            InitializeEncoderDevices();

            _videoDuration = Convert.ToInt32(ConfigurationManager.AppSettings["VideoDuration"]);
            _timer = new Timer(_videoDuration);
            _timer.Elapsed += OnTimerElapsed;
        }

        private void InitializeEncoderDevices()
        {
            _videoSourceName = ConfigurationManager.AppSettings["VideoDevice"];
            _audioSourceName = ConfigurationManager.AppSettings["AudioDevice"];
            _videoDevice = EncoderDevices.FindDevices(EncoderDeviceType.Video).Single(x => x.Name == _videoSourceName);
            _audioDevice = EncoderDevices.FindDevices(EncoderDeviceType.Audio).Single(x => x.Name == _audioSourceName);
        }

        private void StartCapture()
        {
            try
            {
                var fileOut = new FileArchivePublishFormat
                {
                    OutputFileName = $@"{OutputPath}\Capture_{DateTime.Now:yyyyMMdd_hhmmss}.wmv"
                };

                _deviceSource = _job.AddDeviceSource(_videoDevice, _audioDevice);
                _job.ActivateSource(_deviceSource);
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
                _job.RemoveDeviceSource(_deviceSource);
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
            _job.Dispose();
        }
    }
}
