using Keebee.AAT.EventLogging;
using Keebee.AAT.RESTClient;
using Keebee.AAT.Display.Helpers;
using Keebee.AAT.Display.UserControls;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Keebee.AAT.Display.Caregiver
{
    public partial class ActivityPlayer : Form
    {
        private EventLogger _eventLogger;
        public EventLogger EventLogger
        {
            set { _eventLogger = value; }
        }

        private OperationsClient _opsClient;
        public OperationsClient OperationsClient
        {
            set { _opsClient = value; }
        }

        private string[] _files;
        public string[] Files
        {
            set { _files = value; }
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

        private int _residentId;
        public int ResidentId
        {
            set { _residentId = value; }
        }

        private readonly GamingEventLogger _gamingEventLogger;

#if DEBUG
        private const int ActivityNameLabelFontSize = 24;
#endif
#if !DEBUG
        private const int ActivityNameLabelFontSize = 30;
#endif

        public ActivityPlayer()
        {
            InitializeComponent();
            ConfigureComponents();
            InitializeStartupPosition();

            // gaming event logger
            _gamingEventLogger = new GamingEventLogger();

            matchingGame1.LogGamingEventEvent += LogGamingEvent;
        }

        private void ConfigureComponents()
        {
            panel1.Dock = DockStyle.Fill;
            matchingGame1.Dock = DockStyle.Fill;
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

        private void ActivityPlayerShown(object sender, EventArgs e)
        {
            lblActivityName.Text = _activityName;
            lblActivityName.Font = new Font("Microsoft Sans Serif", ActivityNameLabelFontSize);
            _gamingEventLogger.EventLogger = _eventLogger;
            _gamingEventLogger.OperationsClient = _opsClient;
            matchingGame1.EventLogger = _eventLogger;
            matchingGame1.Play(_files, _difficultyLevel, false);
        }

        private void LogGamingEvent(object sender, EventArgs e)
        {
            try
            {
                var args = (MatchingGame.LogGamingEventEventArgs)e;
                _gamingEventLogger.Add(_residentId, args.EventLogEntryTypeId, args.DifficultyLevel, args.Success, args.Description);
            }
            catch (Exception ex)
            {
                _eventLogger.WriteEntry($"Main.LogGamingEvent: {ex.Message}", EventLogEntryType.Error);
            }
        }
    }
}
