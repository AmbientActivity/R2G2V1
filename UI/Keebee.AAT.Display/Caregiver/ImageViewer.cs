﻿using Keebee.AAT.SystemEventLogging;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Keebee.AAT.Display.Caregiver
{
    public partial class ImageViewer : Form
    {
        private SystemEventLogger _systemEventLogger;
        public SystemEventLogger EventLogger
        {
            set { _systemEventLogger = value; }
        }

        private string[] _images;
        public string[] Images
        {
            set { _images = value; }
        }

        private bool _isPlaying;
        private const bool AutoStart = true;


#if DEBUG
        private const int AutoModeLabelFontSize = 8;

        private const int TableLayoutPanelColTwoWidth = 70;
        private const int TableLayoutPanelColFiveidth = 70;

        private const int AutoModeLabeMarginTop = 30;
#elif !DEBUG
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

            slideViewerFlash1.SlideShowCompleteEvent += SlideShowComplete;
        }

        private void ConfigureComponents()
        {
            panel1.Dock = DockStyle.Fill;
            slideViewerFlash1.Dock = DockStyle.Fill;
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

        #region event handlers

        private void PreviousButtonClick(object sender, EventArgs e)
        {
            slideViewerFlash1.ShowPrevious();
        }

        private void NextButtonClick(object sender, EventArgs e)
        {
            slideViewerFlash1.ShowNext();
        }

        private void PlayButtonClick(object sender, EventArgs e)
        {
            if (!_isPlaying)
                Play();
            else
                Pause();
        }

        private void Play()
        {
            slideViewerFlash1.ShowNext();
            slideViewerFlash1.StartTimer();
            btnPlay.BackgroundImage = imageList1.Images[1];
            _isPlaying = true;
        }

        private void Pause()
        {
            slideViewerFlash1.StopTimer();
            btnPlay.BackgroundImage = imageList1.Images[0];
            _isPlaying = false;
        }

        private void CloseButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        private void ImageViewerShown(object sender, EventArgs e)
        {
            slideViewerFlash1.SystemEventLogger = _systemEventLogger;
            btnPlay.BackgroundImage = imageList1.Images[1];
            _isPlaying = true;
            slideViewerFlash1.Play(_images, autoStart: AutoStart);
        }

        private void ImageViewerFormClosing(object sender, FormClosingEventArgs e)
        {
            slideViewerFlash1.Stop();
        }

        private void SlideShowComplete(object sender, EventArgs e)
        {
            Close();
        }

        #endregion
    }
}
