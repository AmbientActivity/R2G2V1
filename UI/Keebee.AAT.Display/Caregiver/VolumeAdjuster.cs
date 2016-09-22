using Keebee.AAT.Display.Extensions;
using Keebee.AAT.Display.Helpers;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Shared;
using Keebee.AAT.RESTClient;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using WMPLib;

namespace Keebee.AAT.Display.Caregiver
{
    public partial class VolumeAdjuster : Form
    {
        private const string PlaylistCaregiver = "caregiver";

        // use Cats video for sample audio clip

        private SystemEventLogger _systemEventLogger;
        public SystemEventLogger EventLogger
        {
            set { _systemEventLogger = value; }
        }

        private OperationsClient _opsClient;
        public OperationsClient OperationsClient
        {
            set { _opsClient = value; }
        }

        private bool _isMusicPlaying;
        public bool IsMusicPlaying
        {
            set { _isMusicPlaying = value; }
        }

        // media path
        private readonly MediaSourcePath _mediaPath = new MediaSourcePath();

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
                var mediaPath = _opsClient.GetPublicMediaFilesForResponseType(ResponseTypeId.Cats)
                    .MediaFiles.Single().Paths.First();

                var mediaPathType = mediaPath.MediaPathType.Description;
                var filename = mediaPath.Files.First().Filename;

                var fullPath = $@"{_mediaPath.ProfileRoot}\{PublicMediaSource.Id}\{mediaPathType}\{filename}";

                _playlist = axWindowsMediaPlayer1.LoadPlaylist(PlaylistCaregiver, new [] { fullPath });
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
