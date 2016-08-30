using Keebee.AAT.EventLogging;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Keebee.AAT.Display.UserControls
{
    public partial class SlideViewerSimple : UserControl
    {
        private const int Interval = 4000;

        private EventLogger _eventLogger;
        public EventLogger EventLogger
        {
            set { _eventLogger = value; }
        }

        // slide show
        private string[] _images;

        private int _currentImageIndex;
        private int _totalImages;

        public SlideViewerSimple()
        {
            InitializeComponent();
            ConfigureComponents();
        }

        private void ConfigureComponents()
        {
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            timer1.Interval = Interval;
        }

        public void Show(string[] files)
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

                    DisplayImage();
                } 
            }
            catch (Exception ex)
            {
                _eventLogger.WriteEntry($"SlideViewerSimple.Play: {ex.Message}", EventLogEntryType.Error);
            }
        }

        public void ShowPrevious()
        {
            _currentImageIndex--;

            if (_currentImageIndex < 0)
                _currentImageIndex = _totalImages - 1;

            DisplayImage();
        }

        public void ShowNext()
        {
            _currentImageIndex++;

            if (_currentImageIndex >= _totalImages)
                _currentImageIndex = 0;

            DisplayImage();
        }

        public void StartTimer()
        {
            timer1.Start();
        }

        public void StopTimer()
        {
            timer1.Stop();
        }

        private static string[] GetValidatedFiles(IEnumerable<string> files)
        {
            var validFiles = new Collection<string>();

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

            return validFiles.ToArray();
        }

        private void DisplayImage()
        {
            var fileName = _images[_currentImageIndex];
            using (var fileStream = new FileStream(fileName, FileMode.Open))
            {
                pictureBox1.Image = new Bitmap(fileStream);
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            ShowNext();
        }
    }
}
