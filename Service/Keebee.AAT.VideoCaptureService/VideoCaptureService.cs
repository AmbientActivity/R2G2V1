using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.MessageQueuing;
using Keebee.AAT.Shared;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using System;
using System.Configuration;
using System.Web.Script.Serialization;
using System.ServiceProcess;
using System.Diagnostics;
using System.IO;
using System.Timers;
using Timer = System.Timers.Timer;


namespace Keebee.AAT.VideoCaptureService
{
    internal partial class VideoCaptureService : ServiceBase
    {
        private readonly Timer _timer;

        // app.config settings
        private readonly int _videoDuration;
        private readonly VideoEncodingQuality _encodingQuality;

        // media capture
        private MediaCapture _capture;
        private bool _isRecording;

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

            InitializeMediaCapture();

            _videoDuration = Convert.ToInt32(ConfigurationManager.AppSettings["VideoDuration"]);
            _encodingQuality = (VideoEncodingQuality)Convert.ToInt32(ConfigurationManager.AppSettings["VideoEncodingQuality"]);

            _timer = new Timer(_videoDuration);
            _timer.Elapsed += OnTimerElapsed;
        }

        private async void InitializeMediaCapture()
        {
            try
            {
                _systemEventLogger.WriteEntry("Starting device");
                _capture = new MediaCapture();

                await _capture.InitializeAsync();

                if (_capture.MediaCaptureSettings.VideoDeviceId != string.Empty 
                    && _capture.MediaCaptureSettings.AudioDeviceId != string.Empty)
                {
                    _systemEventLogger.WriteEntry("Device initialized successfully");

                    _capture.RecordLimitationExceeded += RecordLimitationExceeded;
                    _capture.Failed += Failed;
                }
                else
                {
                    _systemEventLogger.WriteEntry("No VideoDevice/AudioDevice Found", EventLogEntryType.Warning);
                    _capture = null;
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"InitializeEncoderDevices: {ex.Message}", EventLogEntryType.Error);
                _capture = null;
            }
        }

        private async void StartCapture()
        {
            try
            {
                if (_capture == null) return;

                var now = DateTime.Now.ToString("yyyy-MM-dd");
                var rootFolder = $@"{VideoCaptures.Path}\{now}";
                if (!Directory.Exists(rootFolder))
                    Directory.CreateDirectory(rootFolder);

                var filename = $"Capture_{DateTime.Now:yyyyMMdd_hhmmss}.mp4";
                var storageFolder = await StorageFolder.GetFolderFromPathAsync(rootFolder);

                if (File.Exists($@"{rootFolder}\{filename}")) return;

                var recordStorageFile = await storageFolder.CreateFileAsync(filename);
                var recordProfile = MediaEncodingProfile.CreateMp4(_encodingQuality);

                await _capture.StartRecordToStorageFileAsync(recordProfile, recordStorageFile);

                _isRecording = true;
            }
            catch (Exception ex)
            {
                _isRecording = false;
                _systemEventLogger.WriteEntry($"StartCapture: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private async void StopCapture()
        {
            try
            {
                if (_capture == null) return;
                if (!_isRecording) return;

                await _capture.StopRecordAsync();

                _isRecording = false;
            }
            catch (Exception ex)
            {
                _isRecording = false;
                _systemEventLogger.WriteEntry($"StopCapture: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void OnTimerElapsed(object source, ElapsedEventArgs e)
        {
            try
            {
                StopCapture();
                _timer.Stop();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"OnTimerElapsed: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void MessageReceivedVideoCapture(object source, MessageEventArgs e)
        {
            if (e.MessageBody != "1") return;
            if (!_displayIsActive || _isRecording) return;

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
                    _timer?.Stop();
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

        private async void RecordLimitationExceeded(MediaCapture currentCaptureObject)
        {
            _systemEventLogger.WriteEntry("RecordLimitationExceeded", EventLogEntryType.Warning);

            if (_capture == null) return;
            if (_isRecording)
            {
                await _capture.StopRecordAsync();
            }
        }

        private void Failed(object sender, MediaCaptureFailedEventArgs e)
        {
            _systemEventLogger.WriteEntry("Failed", EventLogEntryType.Error);
            if (_isRecording)
            {
                StopCapture();
            }
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
