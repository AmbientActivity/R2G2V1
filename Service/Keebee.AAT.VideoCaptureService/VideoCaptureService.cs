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

        // message queue sender
        private readonly CustomMessageQueue _messageQueueVideoCaptureState;

        // config settings
        private readonly VideoEncodingQuality _encodingQuality;

        // media capture
        private MediaCapture _capture;
        private bool _isCapturing;

        // display state
        private bool _displayIsActive;

        public VideoCaptureService()
        {
            InitializeComponent();

            // message queue sender
            _messageQueueVideoCaptureState = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.VideoCaptureState
            });

            var q1 = new CustomMessageQueue(new CustomMessageQueueArgs
                {
                    QueueName = MessageQueueType.VideoCapture,
                    MessageReceivedCallback = MessageReceivedVideoCapture
                });

            var q2 = new CustomMessageQueue(new CustomMessageQueueArgs
                {
                    QueueName = MessageQueueType.DisplayVideoCapture,
                    MessageReceivedCallback = MessageReceivedDisplayVideoCapture
                });

            InitializeMediaCapture();

            var videoDuration = Convert.ToInt32(ConfigurationManager.AppSettings["VideoDuration"]);
            _encodingQuality = (VideoEncodingQuality)Convert.ToInt32(ConfigurationManager.AppSettings["VideoEncodingQuality"]);

            _timer = new Timer(videoDuration);
            _timer.Elapsed += OnTimerElapsed;
        }

        private async void InitializeMediaCapture()
        {
            try
            {
                SystemEventLogger.WriteEntry("Starting device", SystemEventLogType.VideoCaptureService);
                _capture = new MediaCapture();

                await _capture.InitializeAsync();

                if (_capture.MediaCaptureSettings.VideoDeviceId != string.Empty 
                    && _capture.MediaCaptureSettings.AudioDeviceId != string.Empty)
                {
                    SystemEventLogger.WriteEntry("Device initialized successfully", SystemEventLogType.VideoCaptureService);

                    _capture.RecordLimitationExceeded += RecordLimitationExceeded;
                    _capture.Failed += Failed;
                }
                else
                {
                    SystemEventLogger.WriteEntry("No VideoDevice/AudioDevice Found", SystemEventLogType.VideoCaptureService, EventLogEntryType.Warning);
                    _capture = null;
                }
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"InitializeEncoderDevices: {ex.Message}", SystemEventLogType.VideoCaptureService, EventLogEntryType.Error);
                _capture = null;
            }
        }

        private async void StartCapture()
        {
            try
            {
                if (_capture == null) return;

                _isCapturing = true;

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
            }
            catch (Exception ex)
            {
                _isCapturing = false;
                SystemEventLogger.WriteEntry($"StartCapture: {ex.Message}", SystemEventLogType.VideoCaptureService, EventLogEntryType.Error);
            }
        }

        private async void StopCapture()
        {
            try
            {
                if (_capture == null) return;
                if (!_isCapturing) return;

                await _capture.StopRecordAsync();

                _isCapturing = false;
            }
            catch (Exception ex)
            {
                _isCapturing = false;
                SystemEventLogger.WriteEntry($"StopCapture: {ex.Message}", SystemEventLogType.VideoCaptureService, EventLogEntryType.Error);
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
                SystemEventLogger.WriteEntry($"OnTimerElapsed: {ex.Message}", SystemEventLogType.VideoCaptureService, EventLogEntryType.Error);
            }
        }

        private void MessageReceivedVideoCapture(object source, MessageEventArgs e)
        {
            if (e.MessageBody != "1") return;
            if (!_displayIsActive || _isCapturing) return;

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
                SystemEventLogger.WriteEntry($"MessageReceivedDisplayVideoCapture{Environment.NewLine}{ex.Message}", SystemEventLogType.VideoCaptureService, EventLogEntryType.Error);
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
            SystemEventLogger.WriteEntry("RecordLimitationExceeded", SystemEventLogType.VideoCaptureService, EventLogEntryType.Warning);

            if (_capture == null) return;
            if (_isCapturing)
            {
                await _capture.StopRecordAsync();
            }
        }

        private void Failed(object sender, MediaCaptureFailedEventArgs e)
        {
            SystemEventLogger.WriteEntry("Failed", SystemEventLogType.VideoCaptureService, EventLogEntryType.Error);
            if (_isCapturing)
            {
                StopCapture();
            }
        }

        protected override void OnStart(string[] args)
        {
            SystemEventLogger.WriteEntry("In OnStart", SystemEventLogType.VideoCaptureService);
            _messageQueueVideoCaptureState.Send("1");
        }

        protected override void OnStop()
        {
            SystemEventLogger.WriteEntry("In OnStop", SystemEventLogType.VideoCaptureService);
            _messageQueueVideoCaptureState.Send("0");
            _timer.Stop();
            _timer.Dispose();
        }
    }
}
