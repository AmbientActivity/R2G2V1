using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Shared;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using WMPLib;

namespace Keebee.AAT.Display.Caregiver
{
    public partial class VideoPlayer : Form
    {
        // event handler
        public event EventHandler TimeoutExpiredEvent;

        // delegate
        private delegate void RaiseTimeoutExpiredEventDelegate();

        private string _video;
        public string Video
        {
            set { _video = value; }
        }

        private int _timeout;
        public int Timeout
        {
            set { _timeout = value; }
        }

        private Timer _timer;

        public VideoPlayer()
        {
            InitializeComponent();
            ConfigureComponents();
            InitializeStartupPosition();
        }

        private void InitializeStartupPosition()
        {
            ShowInTaskbar = false;

#if DEBUG
            StartPosition = FormStartPosition.Manual;
            Location = new Point(0, 0);

            // set form size to 1/3 primary monitor size
            Width = SystemInformation.PrimaryMonitorSize.Width / 3;
            Height = SystemInformation.PrimaryMonitorSize.Height / 3;

#elif !DEBUG
            WindowState = FormWindowState.Maximized;
#endif
        }

        private void ConfigureComponents()
        {
            axWindowsMediaPlayer1.stretchToFit = true;
            axWindowsMediaPlayer1.Dock = DockStyle.Fill;
            axWindowsMediaPlayer1.uiMode = "full";
            axWindowsMediaPlayer1.settings.setMode("loop", false);
            axWindowsMediaPlayer1.settings.volume = MediaPlayerControl.DefaultVolume;
            axWindowsMediaPlayer1.enableContextMenu = false;
        }

        private void Play()
        {
            try
            {
                axWindowsMediaPlayer1.URL = _video;
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"VideoPlayer.Play: {ex.Message}", SystemEventLogType.Display, EventLogEntryType.Error);
            }
        }

        #region event handlers

        private void PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            switch (e.newState)
            {
                case (int)WMPPlayState.wmppsPlaying:
                    _timer.Stop();
                    break;
                case (int)WMPPlayState.wmppsMediaEnded:
                    Close();
                    break;
                case (int)WMPPlayState.wmppsPaused:
                    _timer.Start();
                    break;
                case (int)WMPPlayState.wmppsReady:
                    break;
            }
        }

        private void VideoPlayerShown(object sender, EventArgs e)
        {
            _timer = new Timer { Interval = _timeout };
            _timer.Tick += Tick;

            Play();
        }

        private void CloseButtonClick(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.stop();
            Close();
        }

        private void Tick(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.stop();
            RaiseVideoPlayerTimeoutExpired();
        }

        private void RaiseVideoPlayerTimeoutExpired()
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                Invoke(new RaiseTimeoutExpiredEventDelegate(RaiseVideoPlayerTimeoutExpired));
            }
            else
            {
                TimeoutExpiredEvent?.Invoke(new object(), new EventArgs());
            }
        }

        #endregion
    }
}
