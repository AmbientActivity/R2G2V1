﻿using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Display.Helpers;
using Keebee.AAT.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WMPLib;

namespace Keebee.AAT.Display.UserControls
{
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

        // delegate
        private delegate void PlayAmbientDelegate();
        private delegate void RaiseScreenTouchedEventDelegate(int responseTypeId);

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

        private IWMPPlaylist _playlist;

        public AmbientPlayer()
        {
            InitializeComponent();
            ConfigureMediaPlayer();
            ConfigureInvitationMessage();
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
            lblInvitation.Font = new Font("Microsoft Sans Serif", 120);
#endif
        }

        public void InitializeTimers(int durationInvitation, int durationVideo)
        {
            _timerInvitation = new Timer { Interval = durationInvitation };
            _timerInvitation.Tick += TimerMessageTick;

            _timerVideo = new Timer { Interval = durationVideo };
            _timerVideo.Tick += TimerVideoTick;
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
            if (_invitationMessages.Count == 0) return;

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

                //axWindowsMediaPlayer1.Ctlcontrols.pause();
                axWindowsMediaPlayer1.settings.volume = 0;
                axWindowsMediaPlayer1.Hide();

                lblInvitation.Text = message;
                lblInvitation.Show();

                _isInvitaionShown = true;
            }
            else
            {
                lblInvitation.Hide();
                axWindowsMediaPlayer1.Show();
                //axWindowsMediaPlayer1.Ctlcontrols.play();
                axWindowsMediaPlayer1.settings.volume = MediaPlayerControl.DefaultVolume;

                _isInvitaionShown = false;
            }
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
