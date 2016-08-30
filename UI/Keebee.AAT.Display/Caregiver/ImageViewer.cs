using Keebee.AAT.EventLogging;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Keebee.AAT.Display.Caregiver
{
    public partial class ImageViewer : Form
    {
        private EventLogger _eventLogger;
        public EventLogger EventLogger
        {
            set { _eventLogger = value; }
        }

        private string[] _images;
        public string[] Images
        {
            set { _images = value; }
        }

#if DEBUG
        private const int AutoModeLabelFontSize = 8;

        private const int TableLayoutPanelColTwoWidth = 70;
        private const int TableLayoutPanelColFiveidth = 70;

        private const int AutoModeLabeMarginTop = 30;
#endif
#if !DEBUG
        private const int AutoModeLabelFontSize = 16;

        private const int TableLayoutPanelColTwoWidth = 150;
        private const int TableLayoutPanelColFiveidth = 150;

        private const int AutoModeLabeMarginTop = 20;
#endif

        public ImageViewer()
        {
            InitializeComponent();
            ConfigureComponents();
            InitializeStartupPosition();
        }

        private void ConfigureComponents()
        {
            panel1.Dock = DockStyle.Fill;
            slideViewerSimple1.Dock = DockStyle.Fill;
            lblAutoMode.Font = new Font("Microsoft Sans Serif", AutoModeLabelFontSize);
            lblAutoMode.Margin = new Padding(3, AutoModeLabeMarginTop, 0, 0);
            tableLayoutPanel1.ColumnStyles[1].Width = TableLayoutPanelColTwoWidth;
            tableLayoutPanel1.ColumnStyles[4].Width = TableLayoutPanelColFiveidth;
        }

        private void InitializeStartupPosition()
        {
            ShowInTaskbar = false;

#if DEBUG
            StartPosition = FormStartPosition.Manual;
            Location = new Point(0, 0);

            // set form size to 1/3 primary monitor size
            Width = SystemInformation.PrimaryMonitorSize.Width / 3;
            Height = SystemInformation.PrimaryMonitorSize.Height / 3;

#elif !DEBUG
            WindowState = FormWindowState.Maximized;
#endif
        }

        #region even handlers

        private void PreviousButtonClick(object sender, EventArgs e)
        {
            slideViewerSimple1.ShowPrevious();
        }

        private void NextButtonClick(object sender, EventArgs e)
        {
            slideViewerSimple1.ShowNext();
        }

        private void PlayButtonClick(object sender, EventArgs e)
        {
            if (btnPlay.ImageIndex == 0)
            {
                slideViewerSimple1.StartTimer();
                btnPlay.ImageIndex = 1;
            }
            else
            {
                slideViewerSimple1.StopTimer();
                btnPlay.ImageIndex = 0;
            }
        }

        private void CloseButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        private void ImageViewerShown(object sender, EventArgs e)
        {
            slideViewerSimple1.EventLogger = _eventLogger;
            slideViewerSimple1.Show(_images);
        }

        #endregion
    }
}
