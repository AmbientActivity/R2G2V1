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
        private SystemEventLogger _systemEventLogger;
        public SystemEventLogger EventLogger
        {
            set { _systemEventLogger = value; }
        }

        private string[] _videos;
        public string[] Videos
        {
            set { _videos = value; }
        }

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
                axWindowsMediaPlayer1.URL = _videos[0];
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"VideoPlayer.Play: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            switch (e.newState)
            {
                case (int)WMPPlayState.wmppsMediaEnded:
                    Close();
                    break;
                case (int)WMPPlayState.wmppsReady:
                    break;
            }
        }

        private void VideoPlayerShown(object sender, EventArgs e)
        {
            Play();
        }

        private void CloseButtonClick(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.stop();
            Close();
        }
    }
}
