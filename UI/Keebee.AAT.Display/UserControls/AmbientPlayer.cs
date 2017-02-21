using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Shared;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WMPLib;

namespace Keebee.AAT.Display.UserControls
{
    public partial class AmbientPlayer : UserControl
    {
        internal class InvitationMessage
        {
            public int Id { get; set; }
            public string Message { get; set; }
            public int ResponseTypeId { get; set; }
        }

        // event logger
        private SystemEventLogger _systemEventLogger;
        public SystemEventLogger SystemEventLogger
        {
            set { _systemEventLogger = value; }
        }

        // event handler
        public event EventHandler ScreenTouchedEvent;

        // delegate
        private delegate void PlayAmbientDelegate();
        private delegate void RaiseScreenTouchedEventDelegate(int responseTypeId);

        // invitation message
        private int _currentInvitationMessageIndex = -1;
        private IList<InvitationMessage> _invitationMessages;

        private bool _isInvitaionShown;

        // invitation messages
        private readonly string _invitationMessage1;
        private readonly string _invitationMessage2;
        private readonly string _invitationMessage3;
        private readonly string _invitationMessage4;
        private readonly string _invitationMessage5;

        // invitation response types
        private readonly int _invitation1ResponseTypeId;
        private readonly int _invitation2ResponseTypeId;
        private readonly int _invitation3ResponseTypeId;
        private readonly int _invitation4ResponseTypeId;
        private readonly int _invitation5ResponseTypeId;

        public class ScreenTouchedEventEventArgs : EventArgs
        {
            public int ResponseTypeId { get; set; }
        }

        // timers
        private readonly Timer _timerInvitation;
        private readonly Timer _timerVideo;

        private IWMPPlaylist _playlist;

        public AmbientPlayer()
        {
            InitializeComponent();
            ConfigureMediaPlayer();
            ConfigureInvitationMessage();

            var durationInvitation = Convert.ToInt32(ConfigurationManager.AppSettings["AmbientInvitationDuration"].Trim());
            _timerInvitation = new Timer { Interval = durationInvitation };
            _timerInvitation.Tick += TimerMessageTick;

            var durationVideo = Convert.ToInt32(ConfigurationManager.AppSettings["AmbientVideoDuration"].Trim());
            _timerVideo = new Timer { Interval = durationVideo };
            _timerVideo.Tick += TimerVideoTick;

            // invitation messages
            _invitationMessage1 = ConfigurationManager.AppSettings["InvitationMessage1"].Trim();
            _invitationMessage2 = ConfigurationManager.AppSettings["InvitationMessage2"].Trim();
            _invitationMessage3 = ConfigurationManager.AppSettings["InvitationMessage3"].Trim();
            _invitationMessage4 = ConfigurationManager.AppSettings["InvitationMessage4"].Trim();
            _invitationMessage5 = ConfigurationManager.AppSettings["InvitationMessage5"].Trim();

            // invitation response types
            _invitation1ResponseTypeId = Convert.ToInt32(ConfigurationManager.AppSettings["Invitation1ResponseTypeId"].Trim());
            _invitation2ResponseTypeId = Convert.ToInt32(ConfigurationManager.AppSettings["Invitation2ResponseTypeId"].Trim());
            _invitation3ResponseTypeId = Convert.ToInt32(ConfigurationManager.AppSettings["Invitation3ResponseTypeId"].Trim());
            _invitation4ResponseTypeId = Convert.ToInt32(ConfigurationManager.AppSettings["Invitation4ResponseTypeId"].Trim());
            _invitation5ResponseTypeId = Convert.ToInt32(ConfigurationManager.AppSettings["Invitation5ResponseTypeId"].Trim());

            LoadInvitationMessages();
        }

        public void Play(IWMPPlaylist playlist)
        {
            try
            {
                _playlist = playlist;
                PlayAmbient();
                _timerVideo.Start();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"AmbientPlayer.PlayAmbient: {ex.Message}", EventLogEntryType.Error);
            }
        }

        public void Pause()
        {
            axWindowsMediaPlayer1.Ctlcontrols.pause();
            _timerVideo.Stop();
            _timerInvitation.Stop();
        }

        public void Resume()
        {
            DisplayContent(showInvitation: false);
            _timerVideo.Start();
        }

        private void ConfigureMediaPlayer()
        {
            axWindowsMediaPlayer1.stretchToFit = true;
            axWindowsMediaPlayer1.Dock = DockStyle.Fill;
            axWindowsMediaPlayer1.uiMode = "none";
            axWindowsMediaPlayer1.settings.setMode("loop", true);
            axWindowsMediaPlayer1.settings.volume = MediaPlayerControl.DefaultVolume;
            axWindowsMediaPlayer1.enableContextMenu = false;
            axWindowsMediaPlayer1.Ctlenabled = false;
        }

        private void ConfigureInvitationMessage()
        {
            lblInvitation.Hide();
#if DEBUG
            lblInvitation.Font = new Font("Microsoft Sans Serif", 36);
#elif !DEBUG
            lblInvitation.Font = new Font("Microsoft Sans Serif", 72);
#endif
        }

        private void PlayAmbient()
        {
            if (InvokeRequired)
            {
                Invoke(new PlayAmbientDelegate(PlayAmbient));
            }
            else
            {
                try
                {
                    axWindowsMediaPlayer1.currentPlaylist = _playlist;
                }
                catch (Exception ex)
                {
                    _systemEventLogger.WriteEntry($"AmbientPlayer.PlayAmbient: {ex.Message}", EventLogEntryType.Error);
                }
            }
        }

        private void TimerMessageTick(object sender, EventArgs e)
        {
            _timerInvitation.Stop();
            DisplayContent(showInvitation: !_isInvitaionShown);
            _timerVideo.Start();
        }

        private void TimerVideoTick(object sender, EventArgs e)
        {
            _timerVideo.Stop();
            DisplayContent(showInvitation: !_isInvitaionShown);
            _timerInvitation.Start();
        }

        private void DisplayContent(bool showInvitation)
        {
            if (showInvitation)
            {
                if (_currentInvitationMessageIndex >= _invitationMessages.Count() - 1)
                    _currentInvitationMessageIndex = 0;
                else
                    _currentInvitationMessageIndex++;

                var  message = _invitationMessages[_currentInvitationMessageIndex].Message;

                axWindowsMediaPlayer1.Ctlcontrols.pause();
                axWindowsMediaPlayer1.Hide();
                lblInvitation.Text = message;
                lblInvitation.Show();

                _isInvitaionShown = true;
            }
            else
            {
                lblInvitation.Hide();
                axWindowsMediaPlayer1.Show();
                axWindowsMediaPlayer1.Ctlcontrols.play();

                _isInvitaionShown = false;
            }
        }

        private static int ValidateResponseType(int responseTypeId)
        {
            switch (responseTypeId)
            {
                case ResponseTypeId.SlideShow:
                    return ResponseTypeId.SlideShow;
                case ResponseTypeId.MatchingGame:
                    return ResponseTypeId.MatchingGame;
                case ResponseTypeId.Cats:
                    return ResponseTypeId.Cats;
                case ResponseTypeId.Radio:
                    return ResponseTypeId.Radio;
                case ResponseTypeId.Television:
                    return ResponseTypeId.Television;
                default:
                    return 0;
            }
        }

        private void LoadInvitationMessages()
        {
           _invitationMessages = new List<InvitationMessage>();

            if (_invitationMessage1.Length > 0)
                _invitationMessages.Add(new InvitationMessage
                {
                    Id = 1,
                    Message = _invitationMessage1,
                    ResponseTypeId = ValidateResponseType(_invitation1ResponseTypeId)
                });

            if (_invitationMessage2.Length > 0)
                _invitationMessages.Add(new InvitationMessage
                {
                    Id = 2,
                    Message = _invitationMessage2,
                    ResponseTypeId = ValidateResponseType(_invitation2ResponseTypeId)
                });

            if (_invitationMessage3.Length > 0)
                _invitationMessages.Add(new InvitationMessage
                {
                    Id = 3,
                    Message = _invitationMessage3,
                    ResponseTypeId = ValidateResponseType(_invitation3ResponseTypeId)
                });

            if (_invitationMessage4.Length > 0)
                _invitationMessages.Add(new InvitationMessage
                {
                    Id = 4,
                    Message = _invitationMessage4,
                    ResponseTypeId = ValidateResponseType(_invitation4ResponseTypeId)
                });

            if (_invitationMessage5.Length > 0)
                _invitationMessages.Add(new InvitationMessage
                {
                    Id = 5,
                    Message = _invitationMessage5,
                    ResponseTypeId = ValidateResponseType(_invitation5ResponseTypeId)
                });
        }

        private void RaiseScreenTouchedEvent(int responseTypeId)
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                Invoke(new RaiseScreenTouchedEventDelegate(RaiseScreenTouchedEvent), responseTypeId);
            }
            else
            {
                var args = new ScreenTouchedEventEventArgs
                {
                    ResponseTypeId = responseTypeId
                };
                ScreenTouchedEvent?.Invoke(new object(), args);
            }
        }

        private void InvitationClick(object sender, EventArgs e)
        {
            var responseTypeId = _invitationMessages[_currentInvitationMessageIndex].ResponseTypeId;

            if (responseTypeId > 0)
                RaiseScreenTouchedEvent(responseTypeId);
        }
    }
}
