using Keebee.AAT.SystemEventLogging;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using WMPLib;

namespace Keebee.AAT.Display.UserControls
{
    public partial class AmbientPlayer : UserControl
    {
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
            axWindowsMediaPlayer1.settings.volume = 70;
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

        private void AmbientPlayerVisibleChanged(object sender, EventArgs e)
        {
            lblAmbientPlayer.Hide();
        }
    }
}
