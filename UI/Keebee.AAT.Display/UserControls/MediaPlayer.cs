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
        private delegate void UpdateDialDelegate(int value);

        private string _currentPlaylistItem;
        private string _lastPlaylistItem;
        private int _maxIndex;
        private bool _isPlaylistComplete;
        private bool _isNewPlaylist;
        private bool _isPlayPrevious;
        private bool _isLoop;
        private IWMPPlaylist _playlist;

        private int _minDial;
        private int _maxDial;
        private int _rangeDial;
        private double _divisorDial;

        private int _responseTypeId;

        public MediaPlayer()
        {
            InitializeComponent();
            ConfigureComponents();

            var q = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.PhidgetContinuousRadio,
                MessageReceivedCallback = MessageReceivedPhidgetContinuousRadio
            });

            axWindowsMediaPlayer1.PlayStateChange += PlayStateChange;
        }

        public void Play(int responseTypeId, int responseValue, string[] files, bool isActiveEventLog, bool isLoop)
        {
            try
            {
                _isNewPlaylist = true;
                _isPlaylistComplete = false;
                _isLoop = isLoop;
                _responseTypeId = responseTypeId;
                _maxIndex = files.Length - 1;
                _isActiveEventLog = isActiveEventLog;

                ShowHideResponseControls();
                ConfigureMediaPlayer();
                UpdateDial(responseValue);

                if (files.Length > 1) 
                    files.Shuffle();

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
            //_televisionTestScreenTimer.Stop();
            axWindowsMediaPlayer1.settings.mute = false;
            axWindowsMediaPlayer1.Ctlcontrols.stop();
            pbRadioPanel.Hide();
            //pbTestPattern.Hide();
        }

        private void ConfigureComponents()
        {
            axWindowsMediaPlayer1.stretchToFit = true;
            axWindowsMediaPlayer1.Dock = DockStyle.Fill;

            pbRadioPanel.Image = Resources.radio_panel;
            pbRadioPanel.SizeMode = PictureBoxSizeMode.StretchImage;
            pbRadioPanel.Dock = DockStyle.Fill;
            pbDial.Hide();
#if DEBUG
            pbDial.Width = 5;
            pbDial.Height = SystemInformation.PrimaryMonitorSize.Height / 3;
            
            _maxDial = (SystemInformation.PrimaryMonitorSize.Width / 3) - 30;
            _minDial = 120;
#elif !DEBUG
            pbDial.Width = 8;
            pbDial.Height = SystemInformation.PrimaryMonitorSize.Height;
            _maxDial = (SystemInformation.PrimaryMonitorSize.Width) - 30;
            _minDial = 300;
#endif
            _rangeDial = _maxDial - _minDial;
            _divisorDial = (double)1000 / _rangeDial;
        }

        private void ConfigureMediaPlayer()
        {
            if (_responseTypeId == ResponseTypeId.Radio)
            {
                axWindowsMediaPlayer1.uiMode = "invisible";
            }
            else
            {
#if DEBUG
                axWindowsMediaPlayer1.uiMode = "full";
#elif !DEBUG
                axWindowsMediaPlayer1.uiMode = "none";
#endif
            }

            axWindowsMediaPlayer1.settings.volume = 70;
            axWindowsMediaPlayer1.settings.mute = false;
            axWindowsMediaPlayer1.settings.setMode("loop", _isLoop);
        }

        private void ShowHideResponseControls()
        {
            switch (_responseTypeId)
            {
                case ResponseTypeId.Radio:
                    pbDial.Show();
                    pbRadioPanel.Show();
                    break;
                case ResponseTypeId.Television:
                case ResponseTypeId.Cats:
                    pbRadioPanel.Hide();
                    pbDial.Hide();
                    break;
            }
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

                    if (_isLoop) return;

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

        private void MessageReceivedPhidgetContinuousRadio(object source, MessageEventArgs e)
        {
            if (_responseTypeId != ResponseTypeId.Radio)
                return;

            try
            {
                int value;
                var isValid = int.TryParse(e.MessageBody, out value);
                if (!isValid) return;

                UpdateDial(value);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"MediaPlayer.MessageReceivedPhidgetContinuousRadio: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void UpdateDial(int value)
        {
            if (InvokeRequired)
            {
                Invoke(new UpdateDialDelegate(UpdateDial), value);
            }
            else
            {
                pbDial.Left = (int)(value / _divisorDial) + _minDial;
            }
        }
    }
}
