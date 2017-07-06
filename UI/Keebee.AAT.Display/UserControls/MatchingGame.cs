using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Display.Helpers;
using Keebee.AAT.Display.Models;
using Keebee.AAT.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AxShockwaveFlashObjects;

namespace Keebee.AAT.Display.UserControls
{
    public partial class MatchingGame : UserControl
    {
        private string _swfFile;

        private SystemEventLogger _systemEventLogger;
        public SystemEventLogger SystemEventLogger
        {
            set { _systemEventLogger = value; }
        }

        // event handler
        public event EventHandler MatchingGameTimeoutExpiredEvent;
        public event EventHandler LogInteractiveActivityEventEvent;
        public event EventHandler StartVideoCaptureEvent;

        private bool _isActiveEventLog;
        private bool _isAllowVideoCapture;

        // delegate
        private delegate void RaiseMatchingGameTimeoutExpiredEventDelegate();
        private delegate void RaiseLogInteractiveActivityEventEventDelegate(string description, int difficultyLevel, bool? success);
        private delegate void RaiseStartVideoCaptureEventDelegate();

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

        public void Play(string[] shapes, string[] sounds, int initialDifficultyLevel, bool enableTimeout, bool isActiveEventLog, bool isAllowVideoCapture, string swfFile)
        {
            _initialDifficultyLevel = initialDifficultyLevel;
            _isActiveEventLog = isActiveEventLog;
            _isAllowVideoCapture = isAllowVideoCapture;
            _enableGameTimeout = enableTimeout;
            _swfFile = swfFile;

            PlayGame(shapes, sounds);
        }

        private void PlayGame(ICollection<string> shapes, ICollection<string> sounds)
        {
            try
            {
                if (axShockwaveFlash1.Movie == null)
                {
                    var swf = Path.Combine(Application.StartupPath, _swfFile);
                    axShockwaveFlash1.LoadMovie(0, swf);
                }

                if (!shapes.Any()) return;
                if (!sounds.Any()) return;

                var xmlShapes = GetXmlString(shapes);
                var enableTimeout = _enableGameTimeout ? 1 : 0;

                var wouldYouLikeToMatchThePictures = sounds.Single(s => Path.GetFileName(s) == MatchingGameConfig.WouldYouListToMatchThePictures);
                var wouldYouLikeToMatchThePairs = sounds.Single(s => Path.GetFileName(s) == MatchingGameConfig.WouldYouListToMatchThePairs);
                var correct = sounds.Single(s => Path.GetFileName(s) == MatchingGameConfig.Correct);
                var goodJob = sounds.Single(s => Path.GetFileName(s) == MatchingGameConfig.GoodJob);
                var wellDone = sounds.Single(s => Path.GetFileName(s) == MatchingGameConfig.WellDone);
                var tryAgain = sounds.Single(s => Path.GetFileName(s) == MatchingGameConfig.TryAgain);
                var letsTryAgain = sounds.Single(s => Path.GetFileName(s) == MatchingGameConfig.LetsTryAgain);
                var letsTrySomethingDifferent = sounds.Single(s => Path.GetFileName(s) == MatchingGameConfig.LetsTrySomethingDifferent);

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
                    // extract difficultyLevel
                    const string numberOpen = "<number>";
                    const string numberClose = "</number>";
                    var difficultyLevel = Convert.ToInt32(request.Substring(request.IndexOf(numberOpen) + numberOpen.Length,
                            request.IndexOf(numberClose) - request.IndexOf(numberOpen) - numberOpen.Length));

                    // remove difficultyLevel from the request string
                    request = request.Replace($"{numberOpen}{difficultyLevel}{numberClose}", string.Empty);

                    // extract success
                    const string stringOpen = "<string>";
                    const string stringClose = "</string>";
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
                        RaiseMatchingGameTimeoutExpiredEvent();

                    if (_isActiveEventLog)
                        RaiseLogInteractiveActivityEventEvent(description, difficultyLevel, isSuccess);

                    if (_isAllowVideoCapture)
                        RaiseStartVideoCaptureEvent();
                }

                // no arguments implies "raise game complete event"
                else
                {
                    RaiseMatchingGameTimeoutExpiredEvent();
                }
            }

            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Display.MatchingGame.FlashCall{Environment.NewLine}{ex.Message}", EventLogEntryType.Error);
            }
        }

        private void RaiseMatchingGameTimeoutExpiredEvent()
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                Invoke(new RaiseMatchingGameTimeoutExpiredEventDelegate(RaiseMatchingGameTimeoutExpiredEvent));
            }
            else
            {
                MatchingGameTimeoutExpiredEvent?.Invoke(new object(), new EventArgs());
            }
        }

        private void RaiseLogInteractiveActivityEventEvent(string description, int difficultyLevel, bool? success)
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                Invoke(new RaiseLogInteractiveActivityEventEventDelegate(RaiseLogInteractiveActivityEventEvent));
            }
            else
            {
                var args = new LogInteractiveActivityEventArgs
                {
                    InteractiveActivityTypeId = InteractiveActivityTypeId.MatchingGame,
                    DifficultyLevel = difficultyLevel,
                               Success = success,
                               Description = description
                           };

                LogInteractiveActivityEventEvent?.Invoke(new object(), args);
            }
        }

        private void RaiseStartVideoCaptureEvent()
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                Invoke(new RaiseStartVideoCaptureEventDelegate(RaiseStartVideoCaptureEvent));
            }
            else
            {
                StartVideoCaptureEvent?.Invoke(new object(), new EventArgs());
            }
        }
    }
}
