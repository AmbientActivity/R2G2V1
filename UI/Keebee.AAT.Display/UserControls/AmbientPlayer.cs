using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Display.Properties;
using Keebee.AAT.ApiClient;
using Keebee.AAT.Shared;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using WMPLib;

namespace Keebee.AAT.Display.UserControls
{
    public partial class AmbientPlayer : UserControl
    {
        private OperationsClient _opsClient;
        public OperationsClient OperationsClient
        {
            set { _opsClient = value; }
        }

        // event logger
        private SystemEventLogger _systemEventLogger;
        public SystemEventLogger SystemEventLogger
        {
            set { _systemEventLogger = value; }
        }

        // delegate
        private delegate void PlayAmbientDelegate();

        private IWMPPlaylist _playlist;

        public AmbientPlayer()
        {
            InitializeComponent();
            ConfigureMediaPlayer();
        }

        public void Play(IWMPPlaylist playlist)
        {
            try
            {
                _playlist = playlist;
                PlayAmbient();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"AmbientPlayer.PlayAmbient: {ex.Message}", EventLogEntryType.Error);
            }
        }

        public void Pause()
        {
            axWindowsMediaPlayer1.Ctlcontrols.pause();
        }

        public void Resume()
        {
            axWindowsMediaPlayer1.Ctlcontrols.play();
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
    }
}
