using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Forms;
using Keebee.AAT.Main.Extensions;
using Keebee.AAT.Shared;
using Keebee.AAT.SystemEventLogging;
using UserControl = System.Windows.Controls.UserControl;

namespace Keebee.AAT.Main.UserControls
{
    /// <summary>
    /// Interaction logic for MediaPlayer.xaml
    /// </summary>
    public partial class MediaPlayer : UserControl
    {
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

        private string[] _files;
        private int _currentPlaylisIndex;

        public MediaPlayer()
        {
            InitializeComponent();
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
                //axWindowsMediaPlayer1.URL = _files[_currentPlaylisIndex];

                //axWindowsMediaPlayer1.Ctlcontrols.play();
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
                //axWindowsMediaPlayer1.URL = _files[_currentPlaylisIndex];

                //axWindowsMediaPlayer1.Ctlcontrols.play();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"MediaPlayer.PlayPrevious: {ex.Message}", EventLogEntryType.Error);
            }
        }

        public void Stop()
        {
            //axWindowsMediaPlayer1.Ctlcontrols.stop();
        }

        private void ConfigureComponents()
        {
            //axWindowsMediaPlayer1.stretchToFit = true;
            //axWindowsMediaPlayer1.Dock = DockStyle.Fill;
        }

        private void ConfigureMediaPlayer()
        {
            MediaElement.LoadedBehavior = MediaState.Manual;
#if DEBUG
            //MediaElement.
#elif !DEBUG
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
            MediaElement.Width = SystemParameters.PrimaryScreenWidth;
            MediaElement.Height = SystemParameters.PrimaryScreenHeight;
#endif

#if DEBUG
            //axWindowsMediaPlayer1.uiMode = "full";
            //axWindowsMediaPlayer1.Ctlenabled = true;
            //axWindowsMediaPlayer1.enableContextMenu = true;
#elif !DEBUG
            axWindowsMediaPlayer1.uiMode = "none";
            axWindowsMediaPlayer1.Ctlenabled = false;
            axWindowsMediaPlayer1.enableContextMenu = false;
#endif
        }

        private void PlayMedia()
        {
            Dispatcher.Invoke(() =>
            {
                try
                {
                    //if (MediaElement.playState == WMPPlayState.wmppsPlaying)
                    MediaElement.Stop();

                    MediaElement.Source = new Uri(_files[0]);
                    MediaElement.Play();
                }
                catch (Exception ex)
                {
                    _systemEventLogger.WriteEntry($"MediaPlayer.PlayMedia: {ex.Message}", EventLogEntryType.Error);
                }
            });
        }

        private void RaiseMediaCompleteEvent()
        {
            Dispatcher.Invoke(() =>
            {
                MediaPlayerCompleteEvent?.Invoke(new object(), new EventArgs());
            });
        }

        private void RaiseLogVideoActivityEventEvent(string description)
        {
            Dispatcher.Invoke(() =>
            {
                var args = new LogVideoActivityEventEventArgs
                {
                    Description = description
                };
                LogVideoActivityEventEvent?.Invoke(new object(), args);
            });
        }

        private void MediaElement_MediaEnded(object sender, System.Windows.RoutedEventArgs e)
        {
            RaiseMediaCompleteEvent();
        }

        private void MediaElement_MediaOpened(object sender, System.Windows.RoutedEventArgs e)
        {
            //RaiseLogVideoActivityEventEvent(e.Source.ToString());
        }

        //private void PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        //{
        //    switch (e.newState)
        //    {
        //        case (int)WMPPlayState.wmppsPlaying:
        //            if (_isActiveEventLog)
        //                RaiseLogVideoActivityEventEvent(axWindowsMediaPlayer1.currentMedia.name);
        //            break;

        //        case (int)WMPPlayState.wmppsMediaEnded:
        //            if (_isLoop) return;
        //            RaiseMediaCompleteEvent();
        //            break;
        //    }
        //}
    }
}
