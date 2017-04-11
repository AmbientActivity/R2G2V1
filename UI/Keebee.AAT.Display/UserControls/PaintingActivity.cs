using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Display.Models;
using Keebee.AAT.Shared;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using AxShockwaveFlashObjects;

namespace Keebee.AAT.Display.UserControls
{
    public partial class PaintingActivity : UserControl
    {
        private const string SwfFilename = "PaintingActivity.swf";

        private SystemEventLogger _systemEventLogger;
        public SystemEventLogger SystemEventLogger
        {
            set { _systemEventLogger = value; }
        }

        // event handler
        public event EventHandler PaintingActivityTimeoutExpiredEvent;
        public event EventHandler LogInteractiveActivityEventEvent;
        public event EventHandler StartVideoCaptureEvent;

        private bool _isActiveEventLog;
        private bool _isAllowVideoCapture;

        // delegate
        private delegate void RaisePaintingActivityTimeoutExpiredDelegate();
        private delegate void RaiseLogInteractiveActvityEventEventDelegate(string description);
        private delegate void RaiseStartVideoCaptureDelegate();

        private bool _enableGameTimeout;

        public PaintingActivity()
        {
            InitializeComponent();
            ConfigureComponents();
        }

        private void ConfigureComponents()
        {
            axShockwaveFlash1.Dock = DockStyle.Fill;
        }

        public void Play(bool enableTimeout, bool isActiveEventLog, bool isAllowVideoCapture)
        {
            _enableGameTimeout = enableTimeout;
            _isActiveEventLog = isActiveEventLog;
            _isAllowVideoCapture = isAllowVideoCapture;
            PlayGame();
        }

        private void PlayGame()
        {
            try
            {
                var enableTimeout = _enableGameTimeout ? 1 : 0;
                var swf = Path.Combine(Application.StartupPath, SwfFilename);
                axShockwaveFlash1.LoadMovie(0, swf);

                axShockwaveFlash1.CallFunction("<invoke name=\"playPaintingActivity\"><arguments>" +
                    $"<number>{enableTimeout}</number></arguments></invoke>");

                axShockwaveFlash1.Show();
            }

            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Display.PaintingActivity.PlayGame{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
            }
        }

        public void Stop(bool isTimeoutExpired)
        {
            try
            {
                axShockwaveFlash1.Stop();
                axShockwaveFlash1.Hide();

                if (!isTimeoutExpired)
                    axShockwaveFlash1.CallFunction("<invoke name=\"stopPaintingActivity\"></invoke>");
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Display.PaintingActivity.Stop{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
            }

        }

        // called by the shockwave activex component
        private void FlashCall(object sender, _IShockwaveFlashEvents_FlashCallEvent e)
        {
            try
            {
                var request = e.request;

                // existence of arguments implies "log gaming event"
                if (e.request.Contains("<string>"))
                {
                    const string stringOpen = "<string>";
                    const string stringClose = "</string>";

                    // extract description
                    var description = request.Substring(request.IndexOf(stringOpen) + stringOpen.Length,
                        request.IndexOf(stringClose) - request.IndexOf(stringOpen) - stringOpen.Length);

                    // replace apostrophe escape characters with an apostrophe
                    description = description.Replace("&apos;", "'");

                    var isGameHasExpired = request.Contains("<true/>");
                    if (isGameHasExpired)
                        RaisePaintingActivityTimeoutExpired();

                    if (_isActiveEventLog)
                        RaiseLogInteractiveActivityEventEvent(description);

                    if (_isAllowVideoCapture)
                        RaiseStartVideoCaptureEvent();
                }

                // no arguments implies "raise game complete event"
                else
                {
                    RaisePaintingActivityTimeoutExpired();
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Display.PaintingActivity.FlashCall{Environment.NewLine}{ex.Message}",
                    EventLogEntryType.Error);
            }
        }

        private void RaisePaintingActivityTimeoutExpired()
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                Invoke(new RaisePaintingActivityTimeoutExpiredDelegate(RaisePaintingActivityTimeoutExpired));
            }
            else
            {
                PaintingActivityTimeoutExpiredEvent?.Invoke(new object(), new EventArgs());
            }
        }

        private void RaiseLogInteractiveActivityEventEvent(string description)
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                Invoke(new RaiseLogInteractiveActvityEventEventDelegate(RaiseLogInteractiveActivityEventEvent));
            }
            else
            {
                var args = new LogInteractiveActivityEventArgs
                {
                    InteractiveActivityTypeId = InteractiveActivityTypeId.PaintingActivity,
                    Description = description
                };

                LogInteractiveActivityEventEvent?.Invoke(new object(), args);
            }
        }

        private void RaiseStartVideoCaptureEvent()
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                Invoke(new RaiseStartVideoCaptureDelegate(RaiseStartVideoCaptureEvent));
            }
            else
            {
                StartVideoCaptureEvent?.Invoke(new object(), new EventArgs());
            }
        }
    }
}
