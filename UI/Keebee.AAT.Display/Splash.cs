using Keebee.AAT.Display.Properties;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Display.Extensions;
using Keebee.AAT.Shared;
using Keebee.AAT.Display.Helpers;
using System;
using System.Linq;
using System.Diagnostics;
using System.Drawing;
using System.ServiceProcess;
using System.Windows.Forms;

namespace Keebee.AAT.Display
{
    public partial class Splash : Form
    {
        private const string SqlExpressServiceName = "MSSQL$SQLEXPRESS";

        private Timer _timer;

        // ambient playlist
        private string[] _ambientPlaylist;

        public Splash()
        {
            InitializeComponent();

            Location = new Point(
                (Screen.PrimaryScreen.WorkingArea.Width - Width) / 2,
                (Screen.PrimaryScreen.WorkingArea.Height - Height) / 2);
        }

        private void LaunchMain()
        {
            var main = new Main()
            {
                AmbientPlaylist = _ambientPlaylist
            };
            main.Show();
            Hide();
        }

        private const int MaxNumAttempts = 50;
        private int _numAttempt;

        private bool LoadAmbientMediaPlaylist()
        {
            // just in case the playlist has already been loaded
            if (_ambientPlaylist != null) return true;

            try
            {
                if (!StartSqlExpressService())
                    return false;

                var mediaFileQuery = new MediaFileQuery();
                _ambientPlaylist = mediaFileQuery.GetFilesForResponseType(PublicProfileSource.Id, ResponseTypeId.Ambient, MediaPathTypeId.Ambient);
                if (!_ambientPlaylist.Any())
                {
                    Hide();
                    MessageBox.Show("No Ambient Video content found", "No Content Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                }

                if (_ambientPlaylist.Length > 1)
                    _ambientPlaylist.Shuffle();

                if (_ambientPlaylist == null)
                {
                    SystemEventLogger.WriteEntry(
                        $"Splash.LoadAmbientMediaPlaylist{Environment.NewLine}Failed to read the database.{Environment.NewLine}Number of attempts {_numAttempt}"
                        , SystemEventLogType.Display
                        , EventLogEntryType.Warning);

                    return false;
                }
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"Splash.LoadAmbientMediaPlaylist: {ex.Message}"
                    , SystemEventLogType.Display
                    , EventLogEntryType.Error);
                return false;       // fail - will need to try again
            }

            return true;            // success - splash can be closed
        }

        private void TimerTick(object sender, EventArgs e)
        {
            try
            {
                _timer.Stop();
                _numAttempt++;

                var success = LoadAmbientMediaPlaylist();

                if (!success)
                {
                    if (_numAttempt < MaxNumAttempts)
                    {
                        _timer.Start();
                        return;
                    }

                    // exceeded the max number of database read attempts - abort
                    Hide();
                    MessageBox.Show(Resources.DatabaseReadErrorMessage, Resources.DatabaseReadErrorCaption,
                        MessageBoxButtons.OK);
                    Application.Exit();
                }
                else
                {
                    LaunchMain();
                }
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"Splash.TimerTick: {ex.Message}", SystemEventLogType.Display, EventLogEntryType.Error);
                _timer.Start();
            }
        }

        private static bool StartSqlExpressService()
        {
            try
            {
                var controller = new ServiceController(SqlExpressServiceName);

                if (controller.Status == ServiceControllerStatus.Stopped)
                    controller.Start();
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"Splash.TimerTick: {ex.Message}", SystemEventLogType.Display, EventLogEntryType.Warning);
                return false;
            }
            return true;
        }

        private void SplashShown(object sender, EventArgs e)
        {
            try
            {
                _timer = new Timer { Interval = 1000 };
                _timer.Start();
                _timer.Tick += TimerTick;
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"Splash.SplashShown: {ex.Message}", SystemEventLogType.Display, EventLogEntryType.Error);
            }
        }
    }
}
