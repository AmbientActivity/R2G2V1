using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Display.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using AxShockwaveFlashObjects;

namespace Keebee.AAT.Display.UserControls
{
    public partial class ActivityPlayer : UserControl
    {
        // event handler
        public event EventHandler ActivityPlayerTimeoutExpiredEvent;
        public event EventHandler LogInteractiveActivityEventEvent;
        public event EventHandler StartVideoCaptureEvent;

        private int _activityTypeId;
        private string _swfFile;
        private bool _isActiveEventLog;
        private bool _isAllowVideoCapture;
        private bool _enableGameTimeout;

        // delegate
        private delegate void RaiseActivityPlayerTimeoutExpiredDelegate();
        private delegate void RaiseLogInteractiveActvityEventEventDelegate(string description);
        private delegate void RaiseStartVideoCaptureDelegate();

        public ActivityPlayer()
        {
            InitializeComponent();
            ConfigureComponents();
        }

        private void ConfigureComponents()
        {
            axShockwaveFlash1.Dock = DockStyle.Fill;
        }

        private void ConfigureFlashActiveX()
        {
            Controls.Remove(axShockwaveFlash1);
            axShockwaveFlash1 = new AxShockwaveFlash();
            axShockwaveFlash1.BeginInit();
            axShockwaveFlash1.Dock = DockStyle.Fill;
            axShockwaveFlash1.Name = "axShockwaveFlash1";
            axShockwaveFlash1.FlashCall += FlashCall;
            axShockwaveFlash1.EndInit();
            Controls.Add(axShockwaveFlash1);
        }

        public void Play(int activityTypeId, string swfFile, bool enableTimeout, bool isActiveEventLog, bool isAllowVideoCapture)
        {
            _enableGameTimeout = enableTimeout;
            _isActiveEventLog = isActiveEventLog;
            _isAllowVideoCapture = isAllowVideoCapture;
            _activityTypeId = activityTypeId;
            _swfFile = swfFile;

            PlayActivity();
        }

        private void PlayActivity()
        {
            try
            {
                ConfigureFlashActiveX();

                var enableTimeout = _enableGameTimeout ? 1 : 0;
                var swf = Path.Combine(Application.StartupPath, _swfFile);

                axShockwaveFlash1.LoadMovie(0, swf);

                axShockwaveFlash1.CallFunction("<invoke name=\"playActivity\"><arguments>" +
                    $"<number>{enableTimeout}</number></arguments></invoke>");

                axShockwaveFlash1.Show();
            }

            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"Display.ActivityPlayer.PlayActivity{Environment.NewLine}{ex.Message}", SystemEventLogType.Display, EventLogEntryType.Error);
            }
        }

        public void Stop(bool isTimeoutExpired)
        {
            try
            {
                axShockwaveFlash1.Stop();
                axShockwaveFlash1.Hide();

                if (!isTimeoutExpired)
                    axShockwaveFlash1.CallFunction("<invoke name=\"stopActivity\"></invoke>");
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"Display.ActivityPlayer.Stop{Environment.NewLine}{ex.Message}", SystemEventLogType.Display, EventLogEntryType.Error);
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
                        RaiseActivityPlayerTimeoutExpired();

                    if (_isActiveEventLog)
                        RaiseLogInteractiveActivityEventEvent(description);

                    if (_isAllowVideoCapture)
                        RaiseStartVideoCaptureEvent();
                }

                // no arguments implies "raise game complete event"
                else
                {
                    RaiseActivityPlayerTimeoutExpired();
                }
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"Display.ActivityPlayer.FlashCall{Environment.NewLine}{ex.Message}", SystemEventLogType.Display,
                    EventLogEntryType.Error);
            }
        }

        private void RaiseActivityPlayerTimeoutExpired()
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                Invoke(new RaiseActivityPlayerTimeoutExpiredDelegate(RaiseActivityPlayerTimeoutExpired));
            }
            else
            {
                ActivityPlayerTimeoutExpiredEvent?.Invoke(new object(), new EventArgs());
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
                    InteractiveActivityTypeId = _activityTypeId,
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
