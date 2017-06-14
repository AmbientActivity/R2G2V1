using Keebee.AAT.Main.Helpers;
using Keebee.AAT.SystemEventLogging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Keebee.AAT.Main.UserControls
{
    /// <summary>
    /// Interaction logic for AmbientPlayer.xaml
    /// </summary>
    public partial class AmbientPlayer : UserControl
    {
        // event logger
        private SystemEventLogger _systemEventLogger;
        public SystemEventLogger SystemEventLogger
        {
            set { _systemEventLogger = value; }
        }

        // event handler
        public event EventHandler ScreenTouchedEvent;

        // invitation messages
        private int _currentInvitationMessageIndex = -1;
        private IList<AmbientInvitationMessage> _invitationMessages;
        public IList<AmbientInvitationMessage> InvitationMessages
        {
            set { _invitationMessages = value; }
        }

        private bool _isInvitaionShown;

        public class ScreenTouchedEventEventArgs : EventArgs
        {
            public int ResponseTypeId { get; set; }
        }

        // timers
        private Timer _timerInvitation;
        private Timer _timerVideo;

        // media
        private string[] _playlist;

        private int _currentIndex;
        private int _maxIndex;

        public AmbientPlayer()
        {
            InitializeComponent();
            ConfigureMediaPlayer();
            ConfigureInvitationMessage();
        }

        public void Play(string[] playlist)
        {
            try
            {
                _playlist = playlist;
                _maxIndex = _playlist.Length - 1;
                _currentIndex = 0;
                PlayAmbient();
                _timerVideo.Start();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"AmbientPlayer.Play: {ex.Message}", EventLogEntryType.Error);
            }
        }

        public void Pause()
        {
            MediaElement.Pause();
            _timerVideo.Stop();
            _timerInvitation.Stop();
        }

        public void Resume()
        {
            DisplayContent(showInvitation: false);
            MediaElement.Play();
            _timerVideo.Start();
        }

        private void ConfigureMediaPlayer()
        {
            MediaElement.LoadedBehavior = MediaState.Manual;
#if !DEBUG
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
            MediaElement.Width = SystemParameters.PrimaryScreenWidth;
            MediaElement.Height = SystemParameters.PrimaryScreenHeight;
#endif
        }

        private void ConfigureInvitationMessage()
        {
            Invitation.Visibility = Visibility.Hidden;
            Invitation.FontFamily = new FontFamily("Microsoft Sans Serif");

#if DEBUG
            //Invitation.FontSize = 36;        
#elif !DEBUG
            //Invitation.FontSize = 120;
#endif
        }

        public void InitializeTimers(int durationInvitation, int durationVideo)
        {
            _timerInvitation = new Timer { Interval = durationInvitation };
            _timerInvitation.Elapsed += TimerMessageElapsed;

            _timerVideo = new Timer { Interval = durationVideo };
            _timerVideo.Elapsed += TimerVideoElapsed;
        }

        private void PlayAmbient()
        {
            try
            {
                MediaElement.Source = new Uri(_playlist[0]);
                MediaElement.Play();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"AmbientPlayer.PlayAmbient: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void TimerMessageElapsed(object sender, EventArgs e)
        {
            _timerInvitation.Stop();
            DisplayContent(showInvitation: !_isInvitaionShown);
            _timerVideo.Start();
        }

        private void TimerVideoElapsed(object sender, EventArgs e)
        {
            if (_invitationMessages.Count == 0) return;

            _timerVideo.Stop();
            DisplayContent(showInvitation: !_isInvitaionShown);
            _timerInvitation.Start();
        }

        private void DisplayContent(bool showInvitation)
        {
            Dispatcher.Invoke(() =>
            {
                if (showInvitation)
                {
                    if (_currentInvitationMessageIndex >= _invitationMessages.Count() - 1)
                        _currentInvitationMessageIndex = 0;
                    else
                        _currentInvitationMessageIndex++;

                    var message = _invitationMessages[_currentInvitationMessageIndex].Message;

                    MediaElement.Pause();
                    MediaElement.Visibility = Visibility.Hidden;

                    Invitation.Content = message;
                    Invitation.Visibility = Visibility.Visible;

                    _isInvitaionShown = true;
                }
                else
                {
                    Invitation.Visibility = Visibility.Hidden;
                    MediaElement.Visibility = Visibility.Visible;
                    MediaElement.Play();

                    _isInvitaionShown = false;
                }
            });
        }

        private void RaiseScreenTouchedEvent(int responseTypeId)
        {
            var args = new ScreenTouchedEventEventArgs
            {
                ResponseTypeId = responseTypeId
            };
            ScreenTouchedEvent?.Invoke(new object(), args);
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (_currentIndex >= _maxIndex)
                _currentIndex = 0;
            else
                _currentIndex++;

            MediaElement.Source = new Uri(_playlist[_currentIndex]);
            MediaElement.Play();
        }

        private void Invitation_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var responseTypeId = _invitationMessages[_currentInvitationMessageIndex].ResponseTypeId;

            if (responseTypeId > 0)
                RaiseScreenTouchedEvent(responseTypeId);
        }
    }
}
