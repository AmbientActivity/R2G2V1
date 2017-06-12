using Keebee.AAT.SystemEventLogging;
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

        private SystemEventLogger _systemEventLogger;
        public SystemEventLogger SystemEventLogger
        {
            set { _systemEventLogger = value; }
        }

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

        private string _activityName;
        public string ActivityName
        {
            set { _activityName = value; }
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

        private readonly InteractiveActivityEventLogger _interactiveActivityEventLogger;

#if DEBUG
        private const int ActivityNameLabelFontSize = 24;
#elif !DEBUG
        private const int ActivityNameLabelFontSize = 30;
#endif
        #endregion

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
            btnClose.BringToFront();

            matchingGame1.Dock = DockStyle.Fill;
            matchingGame1.Hide();

            paintingActivity1.Hide();
            paintingActivity1.Dock = DockStyle.Fill;
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

        private void CloseButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        private void InteractiveActivityPlayerShown(object sender, EventArgs e)
        {
            _interactiveActivityEventLogger.SystemEventLogger = _systemEventLogger;

            switch (_interactiveActivityTypeId)
            {
                case InteractiveActivityTypeId.MatchingGame:
                    matchingGame1.Show();
                    matchingGame1.SystemEventLogger = _systemEventLogger;
                    matchingGame1.Play(_shapes, _sounds, _difficultyLevel, false, _isActiveEventLog, false);
                    matchingGame1.Select();
                    break;

                case InteractiveActivityTypeId.PaintingActivity:
                    paintingActivity1.Show();
                    paintingActivity1.SystemEventLogger = _systemEventLogger;
                    paintingActivity1.Play(false, _isActiveEventLog, false);
                    paintingActivity1.Select();
                    break;
            }
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
                _systemEventLogger.WriteEntry($"Main.LogInteractiveActivityEvent: {ex.Message}", EventLogEntryType.Error);
            }
        }
    }
}
