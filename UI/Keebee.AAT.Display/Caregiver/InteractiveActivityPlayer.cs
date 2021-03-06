﻿using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Display.Helpers;
using Keebee.AAT.Display.Models;
using Keebee.AAT.Shared;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Keebee.AAT.Display.Caregiver
{
    public partial class InteractiveActivityPlayer : Form
    {
        #region declarations

        // matching game shapes
        private string[] _shapes;
        public string[] Shapes
        {
            set { _shapes = value; }
        }

        // matching game sounds
        private string[] _sounds;
        public string[] Sounds
        {
            set { _sounds = value; }
        }

        private int _difficultyLevel;
        public int DifficultyLevel
        {
            set { _difficultyLevel = value; }
        }

        private bool _isActiveEventLog;
        public bool IsActiveEventLog
        {
            set { _isActiveEventLog = value; }
        }

        private int _residentId;
        public int ResidentId
        {
            set { _residentId = value; }
        }

        private int _interactiveActivityTypeId;
        public int InteractiveActivityId
        {
            set { _interactiveActivityTypeId = value; }
        }

        private string _swfFile;
        public string SwfFile
        {
            set { _swfFile = value; }
        }

        private readonly InteractiveActivityEventLogger _interactiveActivityEventLogger;

        public bool IsTimeoutExpired { get; private set; }

#if DEBUG
        private const int ActivityNameLabelFontSize = 24;
#elif !DEBUG
        private const int ActivityNameLabelFontSize = 30;
#endif
        #endregion

        #region initialization

        public InteractiveActivityPlayer()
        {
            InitializeComponent();
            ConfigureComponents();
            InitializeStartupPosition();

            // gaming event logger
            _interactiveActivityEventLogger = new InteractiveActivityEventLogger();

            matchingGame1.LogInteractiveActivityEventEvent += LogInteractiveActivityEvent;
        }

        private void ConfigureComponents()
        {
            btnExit.BringToFront();

            matchingGame1.Dock = DockStyle.Fill;
            matchingGame1.Hide();

            activityPlayer1.Hide();
            activityPlayer1.Dock = DockStyle.Fill;
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

        #endregion

        #region event handlers

        private void CloseButtonClick(object sender, EventArgs e)
        {
            IsTimeoutExpired = false;
            Close();
        }

        private void InteractiveActivityPlayerShown(object sender, EventArgs e)
        {
            switch (_interactiveActivityTypeId)
            {
                case InteractiveActivityTypeId.MatchingGame:
                    matchingGame1.MatchingGameTimeoutExpiredEvent += TimeoutExpiredEvent;
                    matchingGame1.Show();
                    matchingGame1.Play(_shapes, _sounds, _difficultyLevel, true, _isActiveEventLog, false, _swfFile);
                    matchingGame1.Select();
                    break;
                default:
                    activityPlayer1.ActivityPlayerTimeoutExpiredEvent += TimeoutExpiredEvent;
                    activityPlayer1.Show();
                    activityPlayer1.Play(_interactiveActivityTypeId, _swfFile, true, _isActiveEventLog, false);
                    activityPlayer1.Select();
                    break;
            }
        }

        private void TimeoutExpiredEvent(object sender, EventArgs e)
        {
            IsTimeoutExpired = true;
            Close();
        }

        private void LogInteractiveActivityEvent(object sender, EventArgs e)
        {
            try
            {
                var args = (LogInteractiveActivityEventArgs)e;
                _interactiveActivityEventLogger.Add(_residentId, args.InteractiveActivityTypeId, args.Description, args.DifficultyLevel, args.Success);
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"Main.LogInteractiveActivityEvent: {ex.Message}", SystemEventLogType.Display, EventLogEntryType.Error);
            }
        }

        #endregion
    }
}
