using Keebee.AAT.Display.Extensions;
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
        // event handler
        public event EventHandler SlideShowCompleteEvent;

        private SystemEventLogger _systemEventLogger;
        public SystemEventLogger SystemEventLogger
        {
            set { _systemEventLogger = value; }
        }

        // delegate
        private delegate void RaiseSlideShowCompleteEventDelegate();

        public SlideViewerFlash()
        {
            InitializeComponent();
            ConfigureComponents();
        }

        private void ConfigureComponents()
        {
            axShockwaveFlash1.Dock = DockStyle.Fill;
        }   

        public void Play(string[] files)
        {
            try
            {
                var validFiles = GetValidatedFiles(files);
                if (!validFiles.Any()) return;

                var totalImages = validFiles.Count();

                if (totalImages > 1)
                    validFiles.Shuffle();

                PlaySlideShow(validFiles);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"SlideViewerFlash.Play: {ex.Message}", EventLogEntryType.Error);
            }
        }

        public void Stop()
        {
            axShockwaveFlash1.CallFunction("<invoke name=\"stopSlideShow\"></invoke>");
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

        private void PlaySlideShow(IEnumerable<string> files)
        {
            var swf = Path.Combine(Application.StartupPath, "SlideViewer.swf");
            axShockwaveFlash1.LoadMovie(0, swf);

            var xml = GetXmlString(files);
            axShockwaveFlash1.CallFunction(
                $"<invoke name=\"playSlideShow\"><arguments><string>{xml}</string></arguments></invoke>");
        }

        private static string GetXmlString(IEnumerable<string> files)
        {
            var xmlBuilder = new StringBuilder();

            xmlBuilder.Append("<xml>");
            foreach (var file in files)
            {
                xmlBuilder.Append($"<images><pic>{file}</pic></images>");
            }
            xmlBuilder.Append("</xml>");

           return xmlBuilder.ToString();
        }

        // raised by the shockwave activex component
        private void SlideShowComplete(object sender, _IShockwaveFlashEvents_FlashCallEvent e)
        {
            RaiseSlideShowCompleteEvent();
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
                SlideShowCompleteEvent(new object(), new EventArgs());
            }
        }
    }
}
