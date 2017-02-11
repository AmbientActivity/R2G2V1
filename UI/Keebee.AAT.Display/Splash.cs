using Keebee.AAT.Display.Properties;
using Keebee.AAT.ApiClient;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Display.Extensions;
using Keebee.AAT.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Drawing;
using System.ServiceProcess;
using System.Windows.Forms;
using AxWMPLib;
using WMPLib;

namespace Keebee.AAT.Display
{
    public partial class Splash : Form
    {
        private const string SqlExpressServiceName = "MSSQL$SQLEXPRESS";
        private const string PlaylistAmbient = PlaylistName.Ambient;

        private Timer _timer;

        // operations REST client
        private readonly OperationsClient _opsClient;

        // ambient playlist
        private IWMPPlaylist _ambientPlaylist;

        // event logger
        private readonly SystemEventLogger _systemEventLogger;

        // media path
        private readonly MediaSourcePath _mediaPath = new MediaSourcePath();

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
                if (!StartSqlExpressService())
                    return false;

                // create a temporary media player for loading the ambient playlist
                using (var mediaPlayer = new AxWindowsMediaPlayer())
                {
                    Controls.Add(mediaPlayer);

                    var mediaPathType = _opsClient.GetMediaPathType(MediaPathTypeId.Ambient);
                    var ambientMediaPaths = _opsClient.GetSystemMediaFilesForResponseType(ResponseTypeId.Ambient)
                        .MediaResponseType.Paths;

                    var files = ambientMediaPaths.SelectMany(p => p.Files)
                        .Select(f => $@"{_mediaPath.MediaRoot}\{_mediaPath.SystemLibrary}\{mediaPathType.Path}\{f.Filename}")
                        .ToArray();

                    if (files.Any())
                    {
                        if (files.Length > 1)
                        files.Shuffle();

                        _ambientPlaylist = mediaPlayer.LoadPlaylist(PlaylistAmbient, files);
                    }

                    Controls.Remove(mediaPlayer);

                    if (_ambientPlaylist == null)
                    {
                        _systemEventLogger.WriteEntry(
                            $"Splash.LoadAmbientMediaPlaylist{Environment.NewLine}Failed to read the database.{Environment.NewLine}Number of attempts {_numAttempt}", EventLogEntryType.Warning);

                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Splash.LoadAmbientMediaPlaylist: {ex.Message}", EventLogEntryType.Error);
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
                _systemEventLogger.WriteEntry($"Splash.TimerTick: {ex.Message}", EventLogEntryType.Error);
                _timer.Start();
            }
        }

        private bool StartSqlExpressService()
        {
            try
            {
                var controller = new ServiceController(SqlExpressServiceName);

                if (controller.Status == ServiceControllerStatus.Stopped)
                    controller.Start();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Splash.TimerTick: {ex.Message}", EventLogEntryType.Warning);
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
                _systemEventLogger.WriteEntry($"Splash.SplashShown: {ex.Message}", EventLogEntryType.Error);
            }
        }
    }
}
