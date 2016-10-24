using Keebee.AAT.Shared;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Display.Extensions;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using WMPLib;

namespace Keebee.AAT.Display.UserControls
{
    public partial class MediaPlayer : UserControl
    {
        private const string PlaylistProfile = PlaylistName.Profile;

        private SystemEventLogger _systemEventLogger;
        public SystemEventLogger SystemEventLogger
        {
            set { _systemEventLogger = value; }
        }

        private bool _isActiveEventLog;

        // event handler
        public event EventHandler MediaPlayerCompleteEvent;
        public event EventHandler LogVideoActivityEventEvent;

        public class LogVideoActivityEventEventArgs : EventArgs
        {
            public string Description { get; set; }
        }

        private delegate void RaiseLogVideoActivityEventEventDelegate(string description);

        // delegate
        private delegate void PlayMediaDelegate(string[] files);
        private delegate void RaiseMediaCompleteEventDelegate();

        private string _currentPlaylistItem;
        private string _lastPlaylistItem;
        private int _maxIndex;
        private bool _isPlaylistComplete;
        private bool _isNewPlaylist;
        private bool _isPlayPrevious;
        
        private IWMPPlaylist _playlist;

        public MediaPlayer()
        {
            InitializeComponent();
            ConfigureComponents();

            axWindowsMediaPlayer1.PlayStateChange += PlayStateChange;
        }

        public void Play(int responseTypeId, string[] files, bool isActiveEventLog)
        {
            try
            {
                _isNewPlaylist = true;
                _isPlaylistComplete = false;

                _maxIndex = files.Length - 1;
                _isActiveEventLog = isActiveEventLog;

                if (files.Length > 1) 
                    files.Shuffle();

                ConfigureMediaPlayer(responseTypeId); 
                PlayMedia(files);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"MediaPlayer.Play: {ex.Message}", EventLogEntryType.Error);
            }
        }

        public void PlayNext()
        {
            try
            {
                if (_maxIndex == 0) return;

                _isPlayPrevious = false;
                axWindowsMediaPlayer1.Ctlcontrols.next();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"MediaPlayer.PlayNext: {ex.Message}", EventLogEntryType.Error);
            }
        }

        public void PlayPrevious()
        {
            try
            {
                if (_maxIndex == 0) return;

                _isPlayPrevious = true;
                axWindowsMediaPlayer1.Ctlcontrols.previous();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"MediaPlayer.PlayPrevious: {ex.Message}", EventLogEntryType.Error);
            }
        }

        public void Stop()
        {
            axWindowsMediaPlayer1.Ctlcontrols.stop();
        }

        private void ConfigureComponents()
        {
            axWindowsMediaPlayer1.stretchToFit = true;
            axWindowsMediaPlayer1.Dock = DockStyle.Fill;
            axWindowsMediaPlayer1.settings.volume = 70;

            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void ConfigureMediaPlayer(int responseTypeId)
        {
#if DEBUG
            pictureBox1.Hide();
            axWindowsMediaPlayer1.uiMode = "full";
#elif !DEBUG
            switch (responseTypeId)
            {
                case ResponseTypeId.Radio:
                    pictureBox1.Show();
                    axWindowsMediaPlayer1.uiMode = "invisible";
                    break;
                case ResponseTypeId.Television:
                case ResponseTypeId.Cats:
                    pictureBox1.Hide();
                    axWindowsMediaPlayer1.uiMode = "none";
                    break;

            }
#endif
            axWindowsMediaPlayer1.settings.setMode("loop", false);
        }

        private void PlayMedia(string[] files)
        {
            if (InvokeRequired)
            {
                Invoke(new PlayMediaDelegate(PlayMedia), new object[] { files });
            }
            else
            {
                try
                {
                    _playlist = axWindowsMediaPlayer1.LoadPlaylist(PlaylistProfile, files);
                    _lastPlaylistItem = _playlist.Item[_maxIndex].name;
                    _currentPlaylistItem = _playlist.Item[0].name;

                    axWindowsMediaPlayer1.currentPlaylist = _playlist;
                }
                catch (Exception ex)
                {
                    _systemEventLogger.WriteEntry($"MediaPlayer.PlayMedia: {ex.Message}", EventLogEntryType.Error);
                }
            }
        }

        private void RaiseMediaCompleteEvent()
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                Invoke(new RaiseMediaCompleteEventDelegate(RaiseMediaCompleteEvent));
            }
            else
            {
                MediaPlayerCompleteEvent?.Invoke(new object(), new EventArgs());
            }
        }

        private void RaiseLogVideoActivityEventEvent(string description)
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                Invoke(new RaiseLogVideoActivityEventEventDelegate(RaiseLogVideoActivityEventEvent));
            }
            else
            {
                var args = new LogVideoActivityEventEventArgs
                {
                    Description = description
                };
                LogVideoActivityEventEvent?.Invoke(new object(), args);
            }
        }

        private void PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            switch (e.newState)
            {
                case (int)WMPPlayState.wmppsPlaying:
                    _currentPlaylistItem = axWindowsMediaPlayer1.currentMedia.name;
                    _isNewPlaylist = false;

                    if (_isActiveEventLog)
                        RaiseLogVideoActivityEventEvent(axWindowsMediaPlayer1.currentMedia.name);
                    break;

                case (int)WMPPlayState.wmppsMediaEnded:
                    
                    if (_currentPlaylistItem == _lastPlaylistItem)
                    {
                        RaiseMediaCompleteEvent();
                        _isPlaylistComplete = true;
                    }
                    break;

                case (int)WMPPlayState.wmppsTransitioning:
                    if (_isPlaylistComplete) return;

                    if (axWindowsMediaPlayer1.currentMedia != null)
                    {
                        if (!File.Exists(axWindowsMediaPlayer1.currentMedia.sourceURL))
                        {
                            if (_isPlayPrevious)
                                axWindowsMediaPlayer1.Ctlcontrols.previous();
                            else
                                axWindowsMediaPlayer1.Ctlcontrols.next();

                            _playlist.removeItem(axWindowsMediaPlayer1.currentMedia);
                            _maxIndex--;
                            _lastPlaylistItem = _playlist.Item[_maxIndex].name;
                        }
                    }
                    break;

                case (int)WMPPlayState.wmppsReady:
                    // if a video was not found the player needs to be jump-started
                    if (!_isPlaylistComplete && !_isNewPlaylist)
                    {
                        axWindowsMediaPlayer1.Ctlcontrols.play();
                    }
                    break;
            }
        }
    }
}
