using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Threading;
using AxShockwaveFlashObjects;
using Keebee.AAT.SystemEventLogging;
using UserControl = System.Windows.Controls.UserControl;

namespace Keebee.AAT.Main.UserControls
{
    /// <summary>
    /// Interaction logic for SlideViewer.xaml
    /// </summary>
    public partial class SlideViewer : UserControl
    {
        private const string SwfFilename = "SlideViewer.swf";
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
        private bool _isComplete;

        // timer
        private readonly DispatcherTimer _timer = new DispatcherTimer();
        readonly FlashAxControl _player = new FlashAxControl();

        public SlideViewer()
        {
            InitializeComponent();
            ConfigureComponents();
        }

        private void ConfigureComponents()
        {
            _timer.Interval = new TimeSpan(0, 0, 0, 0, Interval);
            _timer.Tick += TimerTick;
        }

        public void Play(string[] files, bool autoStart)
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

                    if (autoStart) _timer.Start();
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

            if (_currentImageIndex < 0)
                _currentImageIndex = _totalImages - 1;

            HideImage();
        }

        public void ShowNext()
        {
            _currentImageIndex++;

            if (_currentImageIndex >= _totalImages)
                _currentImageIndex = 0;

            HideImage();
        }

        public void StartTimer()
        {
            _timer.Start();
        }

        public void StopTimer()
        {
            _timer.Stop();
        }

        public void Stop()
        {
            _timer.Stop();
            _player.CallFunction("stopImage");
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
                var swf = Path.Combine(Application.StartupPath, SwfFilename);
                var host = new WindowsFormsHost {Child = _player};

                GridFlashAxPlayer.Children.Add(host);

                _player.FlashCallEvent += FlashCallHandler;
                _player.Width = (int)Width;
                _player.Height = (int)Height;

                _player.LoadMovie(swf);
                _player.CallFunction("initializeMovie");
                _player.Play();

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

                _player.CallFunction("showImage", $"<string>{xml}</string>");

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
                _player.CallFunction("hideImage");
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
        private void FlashCallHandler(object sender, EventArgs e)
        {
            var args = (FlashAxControl.FlashCallEventEventArgs) e;
            var request = args.Request;

            // check for no arguments       
            if (!request.Contains("<string>"))
            {
                OnImageHidden();
            }
            else  // existence of a string argument implies an error has occurred
            {

                const string stringOpen = "<string>";
                const string stringClose = "</string>";
                var message = request.Substring(request.IndexOf(stringOpen) + stringOpen.Length,
                    request.IndexOf(stringClose) - request.IndexOf(stringOpen) - stringOpen.Length);

                _systemEventLogger.WriteEntry($"SlideViewerFlash.DisplayImage: {message}", EventLogEntryType.Error);
            }
        }

        private void RaiseSlideShowCompleteEvent()
        {
            Dispatcher.Invoke(() =>
            {
                SlideShowCompleteEvent?.Invoke(new object(), new EventArgs());
            });
        }

        private void TimerTick(object sender, EventArgs e)
        {
            _currentImageIndex++;
            HideImage();
        }
    }
}
