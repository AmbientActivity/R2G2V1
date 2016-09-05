using Keebee.AAT.Display.Properties;
using Keebee.AAT.RESTClient;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Display.Extensions;
using System;
using System.Linq;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using AxWMPLib;
using WMPLib;

namespace Keebee.AAT.Display
{
    public partial class Splash : Form
    {
        private const string PlaylistAmbient = "ambient";

        private Timer _timer;

        // operations REST client
        private readonly OperationsClient _opsClient;

        // ambient playlist
        private IWMPPlaylist _ambientPlaylist;

        // event logger
        private readonly SystemEventLogger _systemEventLogger;

        public Splash()
        {
            InitializeComponent();
            _systemEventLogger = new SystemEventLogger(SystemEventLogType.Display);
            _opsClient = new OperationsClient();

            Location = new Point(
                (Screen.PrimaryScreen.WorkingArea.Width - Width) / 2,
                (Screen.PrimaryScreen.WorkingArea.Height - Height) / 2);
        }

        private void LaunchMain()
        {
            var main = new Main()
                       {
                           AmbientPlaylist = _ambientPlaylist,
                           EventLogger = _systemEventLogger,
                           OperationsClient = _opsClient
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
                // stop the timer while attempting to read the data
                _timer.Stop();

                // create a temporary media player for loading the ambient playlist
                using (var mediaPlayer = new AxWindowsMediaPlayer())
                {
                    Controls.Add(mediaPlayer);

                    var responses = _opsClient.GetAmbientResponses();
                    if (responses != null)
                    {
                        var ambientMediaFiles = responses
                            .OrderBy(x => x.FilePath)
                            .Select(x => x.FilePath)
                            .ToArray();

                        if (ambientMediaFiles.Any())
                        {
                            if (ambientMediaFiles.Length > 1)
                                ambientMediaFiles.Shuffle();

                            _ambientPlaylist = mediaPlayer.LoadPlaylist(PlaylistAmbient, ambientMediaFiles);
                        }
                    }
                    
                    Controls.Remove(mediaPlayer);

                    if (_ambientPlaylist == null)
                    {
                        _systemEventLogger.WriteEntry(
                            $"Splash.LoadAmbientMediaPlaylist{Environment.NewLine}Failed to read the database.{Environment.NewLine}Number of attempts {_numAttempt}", EventLogEntryType.Warning);

                        _timer.Start();
                        return false;
                    }
                }
            }
            catch(Exception ex)
            {
                _systemEventLogger.WriteEntry($"Splash.LoadAmbientMediaPlaylist: {ex.Message}", EventLogEntryType.Error);
                _timer.Start();
                return false;       // fail - will need to try again
            }

            _timer.Start();
            return true;            // success - splash can be closed
        }

        private void TimerTick(object sender, EventArgs e)
        {
            try
            {
                _numAttempt++;
                var success = LoadAmbientMediaPlaylist();

                if (!success)
                {
                    if (_numAttempt < MaxNumAttempts) return;

                    // exceeded the max number of database read attempts - abort
                    _timer.Stop();
                    Hide();

                    MessageBox.Show(
                        Resources.DatabaseReadErrorMessage, Resources.DatabaseReadErrorCaption, MessageBoxButtons.OK);

                    Application.Exit();
                }
                else
                {
                    _timer.Stop();
                    LaunchMain(); 
                }

            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Splash.TimerTick: {ex.Message}", EventLogEntryType.Error);
            }
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
                _systemEventLogger.WriteEntry($"Splash.SplashShown: {ex.Message}", EventLogEntryType.Error);
            }
        }
    }
}
