using Keebee.AAT.SystemEventLogging;
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

        // slide show
        private List<string> _images;

        private int _currentImageIndex;
        private int _totalImages;

        private bool _isFinite;
        private bool _isComplete;

        public SlideViewerFlash()
        {
            InitializeComponent();
            ConfigureComponents();
        }

        private void ConfigureComponents()
        {
            axShockwaveFlash1.Dock = DockStyle.Fill;

            timer1.Interval = Interval;
        }   

        public void Play(string[] files, bool autoStart, bool isFinite)
        {
            try
            {
                // ensure files are the correct type
                var validFiles = GetValidatedFiles(files);

                if (validFiles.Any())
                {
                    _totalImages = validFiles.Count();
                    _currentImageIndex = 0;
                    _images = validFiles;

                    InitializeFlash();
                    DisplayImage();

                    if (autoStart) timer1.Start();
                    _isFinite = isFinite;

                }
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
    }
}
