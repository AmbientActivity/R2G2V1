using Keebee.AAT.Shared;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Display.Extensions;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Keebee.AAT.Display.Properties;
using Keebee.AAT.MessageQueuing;
using WMPLib;

namespace Keebee.AAT.Display.UserControls
{
    public partial class MusicPlayer : UserControl
    {
        private const string PlaylistProfile = PlaylistName.Profile;

        private SystemEventLogger _systemEventLogger;
        public SystemEventLogger SystemEventLogger
        {
            set { _systemEventLogger = value; }
        }

        // delegate
        private delegate void PlayMediaDelegate(string[] files);

        private string _currentPlaylistItem;
        private string _lastPlaylistItem;
        private int _maxIndex;
        private bool _isPlaylistComplete;
        private bool _isNewPlaylist;
        private IWMPPlaylist _playlist;

        public MusicPlayer()
        {
            InitializeComponent();
            ConfigureMediaPlayer();

            axWindowsMediaPlayer1.PlayStateChange += PlayStateChange;
        }

        public void Play(string[] files)
        {
            try
            {
                _isNewPlaylist = true;
                _isPlaylistComplete = false;
                _maxIndex = files.Length - 1;

                ConfigureMediaPlayer();

                if (files.Length > 1)
                    files.Shuffle();

                PlayMedia(files);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"MusicPlayer.Play: {ex.Message}", EventLogEntryType.Error);
            }
        }

        public void Stop()
        {
            axWindowsMediaPlayer1.Ctlcontrols.stop();
        }

        private void ConfigureMediaPlayer()
        {
            axWindowsMediaPlayer1.settings.volume = 70;
            axWindowsMediaPlayer1.settings.mute = false;
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
                    if (axWindowsMediaPlayer1.playState == WMPPlayState.wmppsPlaying)
                        axWindowsMediaPlayer1.Ctlcontrols.stop();

                    _playlist = axWindowsMediaPlayer1.LoadPlaylist(PlaylistProfile, files);
                    _lastPlaylistItem = _playlist.Item[_maxIndex].name;
                    _currentPlaylistItem = _playlist.Item[0].name;

                    axWindowsMediaPlayer1.currentPlaylist = _playlist;
                }
                catch (Exception ex)
                {
                    _systemEventLogger.WriteEntry($"MusicPlayer.PlayMedia: {ex.Message}", EventLogEntryType.Error);
                }
            }
        }

        private void PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            switch (e.newState)
            {
                case (int)WMPPlayState.wmppsPlaying:
                    _currentPlaylistItem = axWindowsMediaPlayer1.currentMedia.name;
                    _isNewPlaylist = false;

                    break;

                case (int)WMPPlayState.wmppsMediaEnded:
                    if (_currentPlaylistItem == _lastPlaylistItem)
                        _isPlaylistComplete = true;

                    break;

                case (int)WMPPlayState.wmppsTransitioning:
                    if (_isPlaylistComplete) return;

                    if (axWindowsMediaPlayer1.currentMedia != null)
                    {
                        if (!File.Exists(axWindowsMediaPlayer1.currentMedia.sourceURL))
                        {
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
