using Keebee.AAT.Display.Extensions;
using Keebee.AAT.Display.Helpers;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Shared;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using WMPLib;

namespace Keebee.AAT.Display.Caregiver
{
    public partial class VolumeAdjuster : Form
    {
        private const string PlaylistCaregiver = "caregiver";

        // use Cats video for sample audio clip
        private readonly string _catsVideo = $@"\\{Environment.MachineName}\{MediaPath.CatsVideo}";

        private SystemEventLogger _systemEventLogger;
        public SystemEventLogger EventLogger
        {
            set { _systemEventLogger = value; }
        }

        private bool _isMusicPlaying;
        public bool IsMusicPlaying
        {
            set { _isMusicPlaying = value; }
        }

        private IWMPPlaylist _playlist;

        public VolumeAdjuster()
        {
            InitializeComponent();
            ConfigureMediaPlayer();
            InitializeStartupPosition();
            pbCurrentVolume.SetProgressNoAnimation(Convert.ToInt32(AudioManager.GetMasterVolume()));
        }

        private void InitializeStartupPosition()
        {
            ShowInTaskbar = false;
#if DEBUG
            StartPosition = FormStartPosition.CenterParent;
#elif !DEBUG
            StartPosition = FormStartPosition.CenterScreen;
#endif
        }

        private void ConfigureMediaPlayer()
        {
            axWindowsMediaPlayer1.uiMode = "invisible";
            axWindowsMediaPlayer1.settings.setMode("loop", true);
        }

        private void PlayAudio()
        {
            try
            {
                
                _playlist = axWindowsMediaPlayer1.LoadPlaylist(PlaylistCaregiver, new [] { _catsVideo });
                axWindowsMediaPlayer1.currentPlaylist = _playlist;
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.VolumeAdjuster.Play: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void VolumeUpButtonMouseDown(object sender, MouseEventArgs e)
        {
            AudioManager.StepMasterVolume(2);
            pbCurrentVolume.SetProgressNoAnimation(Convert.ToInt32(AudioManager.GetMasterVolume()));
        }

        private void VolumeDownButtonMouseDown(object sender, MouseEventArgs e)
        {
            AudioManager.StepMasterVolume(-2);
            pbCurrentVolume.SetProgressNoAnimation(Convert.ToInt32(AudioManager.GetMasterVolume()));
        }

        private void VolumeAdjusterShown(object sender, EventArgs e)
        {
            if (!_isMusicPlaying)
                PlayAudio();
        }

        private void VolumeAdjusterFormClosing(object sender, FormClosingEventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.stop();
        }

        private void CloseButtonClick(object sender, EventArgs e)
        {
            Close();
        }
    }
}
