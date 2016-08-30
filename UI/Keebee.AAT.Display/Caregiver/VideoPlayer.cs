using Keebee.AAT.EventLogging;
using Keebee.AAT.Display.Extensions;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using WMPLib;

namespace Keebee.AAT.Display.Caregiver
{
    public partial class VideoPlayer : Form
    {
        private const string PlaylistCaregiver = "caregiver";

        private IWMPPlaylist _playlist;
        private string _currentPlaylistItem;
        private string _lastPlaylistItem;
        private int _maxIndex;

        private EventLogger _eventLogger;
        public EventLogger EventLogger
        {
            set { _eventLogger = value; }
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
            axWindowsMediaPlayer1.settings.volume = 100;
        }

        private void Play()
        {
            try
            {
                _maxIndex = _videos.Length - 1;
                _playlist = axWindowsMediaPlayer1.LoadPlaylist(PlaylistCaregiver, _videos);
                _lastPlaylistItem = _playlist.Item[_maxIndex].name;
                _currentPlaylistItem = _playlist.Item[0].name;

                axWindowsMediaPlayer1.currentPlaylist = _playlist;
            }
            catch (Exception ex)
            {
                _eventLogger.WriteEntry($"VideoPlayer.Play: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            switch (e.newState)
            {
                case (int)WMPPlayState.wmppsPlaying:
                    _currentPlaylistItem = axWindowsMediaPlayer1.currentMedia.name;
                    break;
                case (int)WMPPlayState.wmppsMediaEnded:

                    if (_currentPlaylistItem == _lastPlaylistItem)
                    {
                        Close();
                    }
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
