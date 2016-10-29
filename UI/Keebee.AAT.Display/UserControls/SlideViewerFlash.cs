using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Display.Extensions;
using Keebee.AAT.Shared;
using AxShockwaveFlashObjects;
using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WMPLib;

namespace Keebee.AAT.Display.UserControls
{
    public partial class SlideViewerFlash : UserControl
    {
        private const int Interval = 8000;
        // event handler
        public event EventHandler SlideShowCompleteEvent;

        private SystemEventLogger _systemEventLogger;
        public SystemEventLogger SystemEventLogger
        {
            set { _systemEventLogger = value; }
        }

        // delegate
        private delegate void RaiseSlideShowCompleteEventDelegate();
        private delegate void PlayMusicDelegate(string[] files);

        // slide show
        private List<string> _images;
        private int _currentImageIndex;
        private int _totalImages;
        private bool _isFinite;
        private bool _isComplete;

        // media player
        private const string PlaylistProfile = PlaylistName.Profile;
        private IWMPPlaylist _playlist;
        private string _currentPlaylistItem;
        private string _lastPlaylistItem;
        private int _maxPlaylistIndex;
        private bool _isPlaylistComplete;
        private bool _isNewPlaylist;

        public SlideViewerFlash()
        {
            InitializeComponent();
            ConfigureComponents();
        }

        private void ConfigureComponents()
        {
            axShockwaveFlash1.Dock = DockStyle.Fill;
            timer1.Interval = Interval;
            axWindowsMediaPlayer1.Visible = false;
            axWindowsMediaPlayer1.uiMode = "invisible";
            axWindowsMediaPlayer1.settings.setMode("loop", true);
            axWindowsMediaPlayer1.PlayStateChange += PlayStateChange;
        }

        public void Play(string[] files, string[] music, bool autoStart, bool isFinite)
        {
            try
            {
                // ensure files are the correct type
                var validFiles = GetValidatedFiles(files);

                if (validFiles.Any())
                {
                    // slide show
                    _totalImages = validFiles.Count();
                    _currentImageIndex = 0;
                    _images = validFiles;

                    InitializeFlash();
                    DisplayImage();

                    if (autoStart) timer1.Start();
                    _isFinite = isFinite;
                }

                if (music == null) return;
                if (!music.Any()) return;

                // background music
                _isNewPlaylist = true;
                _isPlaylistComplete = false;

                _maxPlaylistIndex = music.Length - 1;

                if (music.Length > 1)
                    music.Shuffle();

                PlayMusic(music);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"SlideViewerFlash.Play: {ex.Message}", EventLogEntryType.Error);
            }
        }

        public void ShowPrevious()
        {
            _currentImageIndex--;

            if (_currentImageIndex < 0 && !_isFinite)
                _currentImageIndex = _totalImages - 1;

            HideImage();
        }

        public void ShowNext()
        {
            _currentImageIndex++;

            if (_currentImageIndex >= _totalImages && !_isFinite)
                _currentImageIndex = 0;

            HideImage();
        }

        public void StartTimer()
        {
            timer1.Start();
        }

        public void StopTimer()
        {
            timer1.Stop();
        }

        public void Stop()
        {
            axWindowsMediaPlayer1.Ctlcontrols.stop();
            timer1.Dispose();
            axShockwaveFlash1.CallFunction("<invoke name=\"stopImage\"></invoke>");
        }

        private List<string> GetValidatedFiles(IEnumerable<string> files)
        {
            var validFiles = new Collection<string>();

            try
            {
                ImageCodecInfo[] infos = ImageCodecInfo.GetImageEncoders();
                string ext = infos.Aggregate("", (current, info) => current + (info.FilenameExtension + ";"));

                foreach (var file in files)
                {
                    var extension = Path.GetExtension(file);
                    if (extension != null && ext.ToUpper().IndexOf(extension.ToUpper(), StringComparison.Ordinal) >= 0)
                    {
                        validFiles.Add(file);
                    }
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"SlideViewerFlash.GetValidatedFiles: {ex.Message}", EventLogEntryType.Error);
            }

            return validFiles.ToList();
        }

        private void InitializeFlash()
        {
            try
            {
                var swf = Path.Combine(Application.StartupPath, "SlideViewer.swf");
                axShockwaveFlash1.LoadMovie(0, swf);

                axShockwaveFlash1.CallFunction("<invoke name=\"initializeMovie\"></invoke>");
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"SlideViewerFlash.InitializeFlash: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void DisplayImage()
        {
            try
            {
                var filename = _images[_currentImageIndex];
                var xml = GetXmlString(filename);

                axShockwaveFlash1.CallFunction(
                    $"<invoke name=\"showImage\"><arguments><string>{xml}</string></arguments></invoke>");

            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"SlideViewerFlash.DisplayImage: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void HideImage()
        {
            try
            {
                axShockwaveFlash1.CallFunction("<invoke name=\"hideImage\"></invoke>");
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"SlideViewerFlash.HideImage: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private static string GetXmlString(string file)
        {
            var xmlBuilder = new StringBuilder();

            xmlBuilder.Append("<xml>");
            xmlBuilder.Append($"<images><pic>{file}</pic></images>");
            xmlBuilder.Append("</xml>");

           return xmlBuilder.ToString();
        }

        private void OnImageHidden()
        {
            ValidateNextImage();

            if (_isFinite)
            {
                if (_isComplete)
                    RaiseSlideShowCompleteEvent();
                else
                    DisplayImage();
            }
            else
            {
                if (_isComplete)
                {
                    // if ALL the images were deleted in mid-air, exit
                    if (!_images.Any())
                        RaiseSlideShowCompleteEvent();
                    else
                    {
                        if (!File.Exists(_images[0])) 
                            RaiseSlideShowCompleteEvent();
                    }
                    _currentImageIndex = 0;
                    _isComplete = false;
                }

                DisplayImage();
            }
        }

        private void ValidateNextImage()
        {
            while (true)
            {
                _isComplete = (_currentImageIndex >= _totalImages);
                if (_isComplete) break;

                var exists = File.Exists(_images[_currentImageIndex]);

                if (!exists)
                {
                    _images.RemoveAt(_currentImageIndex);
                    _totalImages = _images.Count;
                }
                else break;
            }
        }

        // raised by the shockwave activex component
        // let's us know when the image is finised being hidden
        private void FlashCall(object sender, _IShockwaveFlashEvents_FlashCallEvent e)
        {
            OnImageHidden();
        }

        private void RaiseSlideShowCompleteEvent()
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                Invoke(new RaiseSlideShowCompleteEventDelegate(RaiseSlideShowCompleteEvent));
            }
            else
            {
                SlideShowCompleteEvent?.Invoke(new object(), new EventArgs());
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            _currentImageIndex++;
            HideImage();
        }

        // media player

        private void PlayMusic(string[] files)
        {
            if (InvokeRequired)
            {
                Invoke(new PlayMusicDelegate(PlayMusic), new object[] {files});
            }
            else
            {
                try
                {
                    _playlist = axWindowsMediaPlayer1.LoadPlaylist(PlaylistProfile, files);
                    _lastPlaylistItem = _playlist.Item[_maxPlaylistIndex].name;
                    _currentPlaylistItem = _playlist.Item[0].name;

                    axWindowsMediaPlayer1.currentPlaylist = _playlist;
                }
                catch (Exception ex)
                {
                    _systemEventLogger.WriteEntry($"SlideViewerFlash.PlayMusic: {ex.Message}", EventLogEntryType.Error);
                }
            }
        }

        private void PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            switch (e.newState)
            {
                case (int)WMPPlayState.wmppsPlaying:
                    _currentPlaylistItem = axWindowsMediaPlayer1.currentMedia.name;
                    _isNewPlaylist = false;

                    break;

                case (int)WMPPlayState.wmppsMediaEnded:

                    if (_currentPlaylistItem == _lastPlaylistItem)
                    {
                        _isPlaylistComplete = true;
                    }
                    break;

                case (int)WMPPlayState.wmppsTransitioning:
                    if (_isPlaylistComplete) return;

                    if (axWindowsMediaPlayer1.currentMedia != null)
                    {
                        if (!File.Exists(axWindowsMediaPlayer1.currentMedia.sourceURL))
                        {
                             axWindowsMediaPlayer1.Ctlcontrols.next();

                            _playlist.removeItem(axWindowsMediaPlayer1.currentMedia);
                            _maxPlaylistIndex--;
                            _lastPlaylistItem = _playlist.Item[_maxPlaylistIndex].name;
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
    }
}
