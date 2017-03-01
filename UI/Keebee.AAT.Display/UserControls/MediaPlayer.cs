using Keebee.AAT.Shared;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Display.Extensions;
using System;
using System.Diagnostics;
using System.Linq;
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
        private delegate void PlayMediaDelegate();
        private delegate void RaiseMediaCompleteEventDelegate();

        private int _maxIndex;
        private bool _isLoop;
        private IWMPPlaylist _playlist;

        private string[] _files;
        private int _currentPlaylisIndex;

        public MediaPlayer()
        {
            InitializeComponent();
            ConfigureComponents();

            axWindowsMediaPlayer1.PlayStateChange += PlayStateChange;
        }

        public void Play(int responseValue, string[] files, bool isActiveEventLog, bool isLoop)
        {
            try
            {
                _isLoop = isLoop;
                _maxIndex = files.Length - 1;
                _isActiveEventLog = isActiveEventLog;
                _currentPlaylisIndex = 0;
                _files = files;

                ConfigureMediaPlayer();

                if (_files.Length > 1)
                    _files.Shuffle();

                PlayMedia();
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
                _currentPlaylisIndex = (_currentPlaylisIndex >= _maxIndex) ? 0 : _currentPlaylisIndex + 1;
                axWindowsMediaPlayer1.currentMedia = _playlist.Item[_currentPlaylisIndex];

                axWindowsMediaPlayer1.Ctlcontrols.play();
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
                _currentPlaylisIndex = (_currentPlaylisIndex <= 0) ? _maxIndex : _currentPlaylisIndex - 1;
                axWindowsMediaPlayer1.currentMedia = _playlist.Item[_currentPlaylisIndex];

                axWindowsMediaPlayer1.Ctlcontrols.play();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"MediaPlayer.PlayPrevious: {ex.Message}", EventLogEntryType.Error);
            }
        }

        public void Stop()
        {
            axWindowsMediaPlayer1.Ctlcontrols.stop();
            _playlist.clear();
        }

        private void ConfigureComponents()
        {
            axWindowsMediaPlayer1.stretchToFit = true;
            axWindowsMediaPlayer1.Dock = DockStyle.Fill;
        }

        private void ConfigureMediaPlayer()
        {
            axWindowsMediaPlayer1.stretchToFit = true;
            axWindowsMediaPlayer1.Dock = DockStyle.Fill;
            axWindowsMediaPlayer1.settings.setMode("loop", _isLoop);
            axWindowsMediaPlayer1.settings.volume = MediaPlayerControl.DefaultVolume;

#if DEBUG
            axWindowsMediaPlayer1.uiMode = "full";
            axWindowsMediaPlayer1.Ctlenabled = true;
            axWindowsMediaPlayer1.enableContextMenu = true;           
#elif !DEBUG
            axWindowsMediaPlayer1.uiMode = "none";
            axWindowsMediaPlayer1.Ctlenabled = false;
            axWindowsMediaPlayer1.enableContextMenu = false;
#endif
        }

        private void PlayMedia()
        {
            if (InvokeRequired)
            {
                Invoke(new PlayMediaDelegate(PlayMedia));
            }
            else
            {
                try
                {
                    if (axWindowsMediaPlayer1.playState == WMPPlayState.wmppsPlaying)
                        axWindowsMediaPlayer1.Ctlcontrols.stop();

                    _playlist = axWindowsMediaPlayer1.LoadPlaylist(PlaylistProfile, _files);

                    axWindowsMediaPlayer1.currentMedia = _playlist.Item[0];
                    axWindowsMediaPlayer1.Ctlcontrols.play();
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
                    if (_isActiveEventLog)
                        RaiseLogVideoActivityEventEvent(axWindowsMediaPlayer1.currentMedia.name);
                    break;

                case (int)WMPPlayState.wmppsMediaEnded:
                    if (_isLoop) return;
                    RaiseMediaCompleteEvent();
                    break;
            }
        }
    }
}
