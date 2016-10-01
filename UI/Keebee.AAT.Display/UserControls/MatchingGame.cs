﻿using Keebee.AAT.SystemEventLogging;
using AxShockwaveFlashObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Keebee.AAT.Display.Helpers;
using Keebee.AAT.Shared;

namespace Keebee.AAT.Display.UserControls
{
    public partial class MatchingGame : UserControl
    {
        private SystemEventLogger _systemEventLogger;
        public SystemEventLogger SystemEventLogger
        {
            set { _systemEventLogger = value; }
        }

        // event handler
        public event EventHandler MatchingGameTimeoutExpiredEvent;
        public event EventHandler LogGameEventEvent;

        private bool _isActiveEventLog;

        public class LogGameEventEventArgs : EventArgs
        {
            public int GameTypeId { get; set; }
            public string Description { get; set; }
            public int DifficultyLevel { get; set; }
            public bool? Success { get; set; }
        }

        // delegate
        private delegate void RaiseMatchingGameTimeoutExpiredDelegate();
        private delegate void RaiseLogGameEventEventDelegate(int eventLogEntryTypeId, int difficultyLevel, bool? success, string description);

        private int _initialDifficultyLevel;
        private bool _enableGameTimeout;

        public MatchingGame()
        {
            InitializeComponent();
            ConfigureComponents();
        }

        private void ConfigureComponents()
        {
            axShockwaveFlash1.Dock = DockStyle.Fill;
        }

        public void Play(string[] shapes, string[] sounds, int initialDifficultyLevel, bool enableTimeout, bool isActiveEventLog)
        {
            _initialDifficultyLevel = initialDifficultyLevel;
            _isActiveEventLog = isActiveEventLog;
            _enableGameTimeout = enableTimeout;
            PlayGame(shapes, sounds);
        }

        private void PlayGame(ICollection<string> shapes, ICollection<string> sounds)
        {
            try
            {
                var swf = Path.Combine(Application.StartupPath, "MatchingGame.swf");
                axShockwaveFlash1.LoadMovie(0, swf);

                if (!shapes.Any()) return;
                if (!sounds.Any()) return;

                var xmlShapes = GetXmlString(shapes);
                var enableTimeout = _enableGameTimeout ? 1 : 0;

                var wouldYouLikeToMatchThePictures = sounds.Single(s => s.Contains(MatchingGameConfig.WouldYouListToMatchThePictures));
                var wouldYouLikeToMatchThePairs = sounds.Single(s => s.Contains(MatchingGameConfig.WouldYouListToMatchThePairs));
                var correct = sounds.Single(s => s.Contains(MatchingGameConfig.Correct));
                var goodJob = sounds.Single(s => s.Contains(MatchingGameConfig.GoodJob));
                var wellDone = sounds.Single(s => s.Contains(MatchingGameConfig.WellDone));
                var tryAgain = sounds.Single(s => s.Contains(MatchingGameConfig.TryAgain));
                var letsTryAgain = sounds.Single(s => s.Contains(MatchingGameConfig.LetsTryAgain));
                var letsTrySomethingDifferent = sounds.Single(s => s.Contains(MatchingGameConfig.LetsTrySomethingDifferent));

                axShockwaveFlash1.CallFunction(
                    "<invoke name=\"loadMedia\"><arguments>" +
                    $"<string>{xmlShapes}</string>" + 
                    $"<string>{wouldYouLikeToMatchThePictures}</string>" +
                    $"<string>{wouldYouLikeToMatchThePairs}</string>" +
                    $"<string>{correct}</string>" +
                    $"<string>{goodJob}</string>" +
                    $"<string>{wellDone}</string>" +
                    $"<string>{tryAgain}</string>" +
                    $"<string>{letsTryAgain}</string>" +
                    $"<string>{letsTrySomethingDifferent}</string>" +
                    $"<number>{_initialDifficultyLevel}</number>" +
                    $"<number>{enableTimeout}</number></arguments></invoke>");
                axShockwaveFlash1.CallFunction("<invoke name=\"playMatchingGame\"></invoke>");

                axShockwaveFlash1.Show();
            }

            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Display.MatchingGame.PlayGame{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
            }
        }

        public void Stop(bool isTimeoutExpired)
        {
            try
            {
                axShockwaveFlash1.Stop();
                axShockwaveFlash1.Hide();

                if (!isTimeoutExpired)
                    axShockwaveFlash1.CallFunction("<invoke name=\"stopMatchingGame\"></invoke>");
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Display.MatchingGame.Stop{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
            }
            
        }

        private string GetXmlString(IEnumerable<string> files)
        {
            var xmlBuilder = new StringBuilder();

            try
            {
                xmlBuilder.Append("<images>");
                foreach (var file in files)
                {
                    xmlBuilder.Append($"<image><name>{file}</name></image>");
                }
                xmlBuilder.Append("</images>");
            }

            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Display.MatchingGame.GetXmlString{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
            }

            return xmlBuilder.ToString();
        }

        // called by the shockwave activex component
        private void FlashCall(object sender, _IShockwaveFlashEvents_FlashCallEvent e)
        {
            var request = e.request;

            try
            {
                // existence of arguments implies "log gaming event"
                if (e.request.Contains("<number>"))
                {
                    // extract eventLogEntryTypeId
                    const string numberOpen = "<number>";
                    const string numberClose = "</number>";
                    var eventLogEntryTypeId = Convert.ToInt32(request.Substring(request.IndexOf(numberOpen) + numberOpen.Length,
                            request.IndexOf(numberClose) - request.IndexOf(numberOpen) - numberOpen.Length));

                    const string stringOpen = "<string>";
                    const string stringClose = "</string>";

                    // extract difficultyLevel
                    var difficultyLevel = Convert.ToInt32(request.Substring(request.IndexOf(stringOpen) + stringOpen.Length,
                            request.IndexOf(stringClose) - request.IndexOf(stringOpen) - stringOpen.Length));

                    // remove difficultyLevel from the request string
                    request = request.Replace($"{stringOpen}{difficultyLevel}{stringClose}", string.Empty);

                    // extract success
                    var successDesc = request.Substring(request.IndexOf(stringOpen) + stringOpen.Length,
                            request.IndexOf(stringClose) - request.IndexOf(stringOpen) - stringOpen.Length);

                    bool? isSuccess = null;

                    switch (successDesc)
                    {
                        case "TRUE":
                            isSuccess = true;
                            break;
                        case "FALSE":
                            isSuccess = false;
                            break;
                    }

                    // remove success from the request string
                    request = request.Replace($"{stringOpen}{successDesc}{stringClose}", string.Empty);

                    // extract description
                    var description = request.Substring(request.IndexOf(stringOpen) + stringOpen.Length,
                        request.IndexOf(stringClose) - request.IndexOf(stringOpen) - stringOpen.Length);

                    // replace apostrophe escape characters with an apostrophe
                    description = description.Replace("&apos;", "'");

                    var isGameHasExpired = request.Contains("<true/>");
                    if (isGameHasExpired)
                        RaiseMatchingGameTimeoutExpired();

                    if (_isActiveEventLog)
                        RaiseLogGameEventEvent(eventLogEntryTypeId, difficultyLevel, isSuccess, description);
                }

                // no arguments implies "raise game complete event"
                else
                {
                    RaiseMatchingGameTimeoutExpired();
                }
            }

            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Display.MatchingGame.FlashCall{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
            }
        }

        private void RaiseMatchingGameTimeoutExpired()
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                Invoke(new RaiseMatchingGameTimeoutExpiredDelegate(RaiseMatchingGameTimeoutExpired));
            }
            else
            {
                MatchingGameTimeoutExpiredEvent?.Invoke(new object(), new EventArgs());
            }
        }

        private void RaiseLogGameEventEvent(int gameTypeId, int difficultyLevel, bool? success, string description)
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                Invoke(new RaiseLogGameEventEventDelegate(RaiseLogGameEventEvent));
            }
            else
            {
                var args = new LogGameEventEventArgs
                           {
                               GameTypeId = gameTypeId,
                               DifficultyLevel = difficultyLevel,
                               Success = success,
                               Description = description
                           };

                LogGameEventEvent?.Invoke(new object(), args);
            }
        }
    }
}
