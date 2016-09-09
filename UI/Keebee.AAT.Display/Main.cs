using Keebee.AAT.RESTClient;
using Keebee.AAT.MessageQueuing;
using Keebee.AAT.Shared;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Display.UserControls;
using Keebee.AAT.Display.Caregiver;
using Keebee.AAT.Display.Helpers;
using System.Web.Script.Serialization;
using System.Diagnostics;
using System.Drawing;
using System;
using System.Windows.Forms;
using System.Linq;
using WMPLib;

namespace Keebee.AAT.Display
{
    internal enum ResponseValueChangeType
    {
        Increase = 0,
        Decrease = 1,
        NoDifference = 2
    }

    public partial class Main : Form
    {
        private OperationsClient _opsClient;
        public OperationsClient OperationsClient
        {
            set { _opsClient = value; }
        }

        private SystemEventLogger _systemEventLogger;
        public SystemEventLogger EventLogger
        {
            set { _systemEventLogger = value; }
        }

        private IWMPPlaylist _ambientPlaylist;
        public IWMPPlaylist AmbientPlaylist
        {
            set { _ambientPlaylist = value; }
        }

        // delegate
        private delegate void ResumeAmbientDelegate();
        private delegate void PlayMediaDelegate(int responseTypeId, int responseValue);
        private delegate void PlaySlideShowDelegate();
        private delegate void PlayMatchingGameDelegate();
        private delegate void ShowCaregiverDelegate();
        private delegate void KillDisplayDelegate();

        // message queue sender
        private readonly CustomMessageQueue _messageQueueDisplay;

        // message queue listener
        private readonly CustomMessageQueue _messageQueueResponse;

        // current sensor values
        private int _currentTelevisionSensorValue;
        private int _currentRadioSensorValue;

        // current activity/response
        private int _currenActivityTypeId;
        private int _currentResponseTypeId;

        // active profile
        private ActiveProfile _activeProfile;

        // flags
        private bool _isNewResponse;
        private bool _isMatchingGameTimeoutExpired;

        // caregiver interface
        private CaregiverInterface _caregiverInterface;

        // custom event loggers
        private readonly GameEventLogger _gameEventLogger;
        private readonly ActivityEventLogger _activityEventLogger;

        public Main()
        {
            InitializeComponent();
            ConfigureUserControls();

            // display message queue sender
            _messageQueueDisplay = new CustomMessageQueue(new CustomMessageQueueArgs
            { 
                QueueName = MessageQueueType.Display
            });

            // response message queue listener
            _messageQueueResponse = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.Response,
                MessageReceivedCallback = MessageReceived
            });

            // custom event loggers
            _gameEventLogger = new GameEventLogger();
            _activityEventLogger = new ActivityEventLogger();

            // response complete event handlers
            slideViewerFlash1.SlideShowCompleteEvent += SlideShowComplete;
            mediaPlayer1.MediaPlayerCompleteEvent += MediaPlayerComplete;
            matchingGame1.MatchingGameTimeoutExpiredEvent += MatchingGameTimeoutExpired;
            matchingGame1.LogGameEventEvent += LogGameEvent;
            mediaPlayer1.LogVideoActivityEventEvent += LogVideoActivityEvent;

            InitializeStartupPosition();

            _currentResponseTypeId = ResponseTypeId.Ambient;
        }

        #region initialization

        private void InitializeStartupPosition()
        {
            ShowInTaskbar = false;

#if DEBUG
            StartPosition = FormStartPosition.Manual;
            Location = new Point(0, 0);

            // set form size to 1/3 primary monitor size
            Width = SystemInformation.PrimaryMonitorSize.Width/3;
            Height = SystemInformation.PrimaryMonitorSize.Height/3;

#elif !DEBUG
            WindowState = FormWindowState.Maximized;
#endif
        }

        private void ConfigureUserControls()
        {
            ambient1.Dock = DockStyle.Fill;
            ambient1.Show();

            mediaPlayer1.Dock = DockStyle.Fill;
            mediaPlayer1.Hide();

            slideViewerFlash1.Dock = DockStyle.Fill;
            slideViewerFlash1.Hide();

            matchingGame1.Dock = DockStyle.Fill;
            matchingGame1.Hide();
        }

        private void SetPostLoadProperties()
        {
            // the _systemEventLogger is not available in the constructor - this gets called later in MainShown()
            _opsClient.SystemEventLogger = _systemEventLogger;
            _messageQueueDisplay.SystemEventLogger = _systemEventLogger;
            _messageQueueResponse.SystemEventLogger = _systemEventLogger;

            _gameEventLogger.OperationsClient = _opsClient;
            _gameEventLogger.SystemEventLogger = _systemEventLogger;
            _activityEventLogger.OperationsClient = _opsClient;
            _activityEventLogger.EventLogger = _systemEventLogger;

            ambient1.SystemEventLogger = _systemEventLogger;
            mediaPlayer1.SystemEventLogger = _systemEventLogger;
            slideViewerFlash1.SystemEventLogger = _systemEventLogger;
            matchingGame1.SystemEventLogger = _systemEventLogger;
        }

        #endregion

        private void ExecuteResponse(int responseTypeId, int sensorValue, bool isSystem)
        {
            if (!isSystem)
                if (!ShouldExecute(responseTypeId)) return;

            if (_currentResponseTypeId == ResponseTypeId.Caregiver) return;

            try
            {
                switch (responseTypeId)
                {
                    case ResponseTypeId.SlidShow:
                        PlaySlideShow();
                        break;
                    case ResponseTypeId.MatchingGame:
                        PlayMatchingGame();
                        break;
                    case ResponseTypeId.KillDisplay:
                        KillDisplay();
                        break;
                    case ResponseTypeId.Radio:
                    case ResponseTypeId.Television:
                    case ResponseTypeId.Cats:
                        PlayMedia(responseTypeId, sensorValue);
                        break;
                    case ResponseTypeId.Caregiver:
                        ShowCaregiver();
                        break;
                    case ResponseTypeId.Ambient:
                        ResumeAmbient();
                        break;
                }

                // save the current sensor values
                switch (responseTypeId)
                {
                    case ResponseTypeId.Radio:
                        _currentRadioSensorValue = sensorValue;
                        break;
                    case ResponseTypeId.Television:
                        _currentTelevisionSensorValue = sensorValue;
                        break;
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Main.ExecuteResponse: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private bool ShouldExecute(int responseTypeId)
        {
            // if it is a new activity/response type then execute it
            if (_isNewResponse) return true;

            // if it is a media player response type then execute it
            return (responseTypeId == ResponseTypeId.Television) || (responseTypeId == ResponseTypeId.Radio); 
        }

        private void StopCurrentResponse()
        {
            try
            {
                switch (_currentResponseTypeId)
                {
                    case ResponseTypeId.SlidShow:
                        slideViewerFlash1.Hide();
                        slideViewerFlash1.Stop();
                        break;
                    case ResponseTypeId.MatchingGame:
                        matchingGame1.Hide();
                        matchingGame1.Stop(_isMatchingGameTimeoutExpired);
                        break;
                    case ResponseTypeId.Radio:
                    case ResponseTypeId.Television:
                    case ResponseTypeId.Cats:
                        mediaPlayer1.Hide();
                        mediaPlayer1.Stop();
                        break;
                    case ResponseTypeId.Ambient:
                        ambient1.Hide();
                        ambient1.Pause();
                        break;
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Main.StopCurrentResponse: {ex.Message}", EventLogEntryType.Error);
            }
        }

        #region callback

        private void PlayMedia (int responseTypeId, int responseValue)
        {
            if (InvokeRequired)
            {
                Invoke(new PlayMediaDelegate(PlayMedia), responseTypeId, responseValue);
            }
            else
            {
                if (_isNewResponse)
                {
                    var mediaFiles = GetResponseFiles(responseTypeId);
                    if (!mediaFiles.Any()) return;

                    StopCurrentResponse();

                    mediaPlayer1.Show();
                    mediaPlayer1.Play(responseTypeId, mediaFiles);

                    _currentResponseTypeId = responseTypeId;  // radio or television
                }
                else
                {
                    var changeType = GetResponseValueChangeType(responseTypeId, responseValue);

                    switch (changeType)
                    {
                        case ResponseValueChangeType.Increase:
                            mediaPlayer1.PlayNext();
                            break;
                        case ResponseValueChangeType.Decrease:
                            mediaPlayer1.PlayPrevious();
                            break;
                        case ResponseValueChangeType.NoDifference:
                            break;
                    }
                }
            }
        }

        private ResponseValueChangeType GetResponseValueChangeType(int responseTypeId, int responseValue)
        {
            int currentResponseValue;

            switch (responseTypeId)
            {
                case ResponseTypeId.Radio:
                    currentResponseValue = _currentRadioSensorValue;
                    break;
                case ResponseTypeId.Television:
                    currentResponseValue = _currentTelevisionSensorValue;
                    break;
                default:
                    return ResponseValueChangeType.NoDifference;
            }

            // edge case 1 - going from Value5 to Value1 should be an INCREASE
            if ((currentResponseValue == (int)RotationSensorStep.Value5) && (responseValue == (int)RotationSensorStep.Value1))
                return ResponseValueChangeType.Increase;

            // edge case 2 - going from Value1 to Value5 should be a DECREASE
            if ((currentResponseValue == (int)RotationSensorStep.Value1) && (responseValue == (int)RotationSensorStep.Value5))
                return ResponseValueChangeType.Decrease;

            // no change
            if (responseValue == currentResponseValue)
                return ResponseValueChangeType.NoDifference;

            // all other scenarios
            return (responseValue > currentResponseValue)
                    ? ResponseValueChangeType.Increase
                    : ResponseValueChangeType.Decrease;
        }

        private void PlaySlideShow()
        {
            if (InvokeRequired)
            {
                Invoke(new PlaySlideShowDelegate(PlaySlideShow));
            }
            else
            {
                var images = GetResponseFiles(ResponseTypeId.SlidShow);
                if (!images.Any()) return;

                StopCurrentResponse();

                slideViewerFlash1.Show();
                slideViewerFlash1.Play(images);

                _activityEventLogger.Add(_activeProfile.ConfigId, _activeProfile.ResidentId, _currenActivityTypeId, ResponseTypeId.SlidShow);

                _currentResponseTypeId = ResponseTypeId.SlidShow;
            }
        }

        private void PlayMatchingGame()
        {
            if (InvokeRequired)
            {
                Invoke(new PlayMatchingGameDelegate(PlayMatchingGame));
            }
            else
            {
                var shapes = GetResponseFiles(ResponseTypeId.MatchingGame, "png");
                if (!shapes.Any()) return;

                StopCurrentResponse();
                _isMatchingGameTimeoutExpired = false;

                matchingGame1.Show();

                _activityEventLogger.Add(_activeProfile.ConfigId, _activeProfile.ResidentId, _currenActivityTypeId, ResponseTypeId.MatchingGame);
                _gameEventLogger.Add(_activeProfile.ResidentId, GameTypeId.MatchThePictures, _activeProfile.GameDifficultyLevel, null, "New game has been initiated");

                matchingGame1.Play(shapes, _activeProfile.GameDifficultyLevel, true);

                _currentResponseTypeId = ResponseTypeId.MatchingGame;
            }
        }

        private string[] GetResponseFiles(int responseTypeId, string fileType = null)
        {
            var files = _opsClient.GetProfileMediaForActivityResponseType(_activeProfile.Id, _currenActivityTypeId, responseTypeId)
                    .Where(x => x.FileType == fileType || fileType == null)
                    .OrderBy(x => x.FilePath)
                    .Select(x => x.FilePath)
                    .ToArray();

            // if no media found, load generic content
            if (!files.Any())
                files = _opsClient.GetProfileMediaForActivityResponseType(ProfileId.Generic, _currenActivityTypeId, responseTypeId)
                        .Where(x => x.FileType == fileType || fileType == null)
                        .OrderBy(x => x.FilePath)
                        .Select(x => x.FilePath)
                        .ToArray();

            return files;
        }

        private void ResumeAmbient()
        {
            if (InvokeRequired)
            {
                Invoke(new ResumeAmbientDelegate(ResumeAmbient));
            }
            else
            {
                StopCurrentResponse();
                ambient1.Show();
                ambient1.Resume();

                _currentResponseTypeId = ResponseTypeId.Ambient;
            }
        }

        private void ShowCaregiver()
        {
            if (InvokeRequired)
            {
                Invoke(new ShowCaregiverDelegate(ShowCaregiver));
            }
            else
            {
                StopCurrentResponse();
                var frmSplash = new Caregiver.Splash();
                frmSplash.Show();

                var genericProfile = _opsClient.GetProfileMedia(ProfileId.Generic);

                _caregiverInterface = new CaregiverInterface
                                         {
                                            EventLogger = _systemEventLogger,
                                            OperationsClient = _opsClient,
                                            GenericProfile = genericProfile
                };

                _caregiverInterface.CaregiverCompleteEvent += CaregiverComplete;

                frmSplash.Close();
                _caregiverInterface.Show();

                _currentResponseTypeId = ResponseTypeId.Caregiver;
            }
        }

        private void KillDisplay()
        {
            if (InvokeRequired)
            {
                Invoke(new KillDisplayDelegate(KillDisplay));
            }
            else
            {
                StopCurrentResponse();
                // alert the State Machine Service that the display is no longer active or idle
                _messageQueueDisplay.Send(CreateDisplayMessageBody(false));
                Application.Exit();
            }
        }

        private static string CreateDisplayMessageBody(bool isActive)
        {
            var displayMessage = new DisplayMessage
            {
                IsActive = isActive
            };

            var serializer = new JavaScriptSerializer();
            var displayMessageBody = serializer.Serialize(displayMessage);
            return displayMessageBody;
        }

        #endregion

        #region event handlers

        private void MessageReceived(object source, MessageEventArgs e)
        {
            var serializer = new JavaScriptSerializer();
            var response = serializer.Deserialize<ResponseMessage>(e.MessageBody);

            _isNewResponse = 
                (response.ResponseTypeId != _currentResponseTypeId) ||
                (response.ActivityTypeId != _currenActivityTypeId) ||
                (response.ActiveProfile.Id != _activeProfile.Id) ||
                (response.ActiveProfile.ResidentId != _activeProfile.ResidentId);

            _activeProfile = response.ActiveProfile;
            _currenActivityTypeId = response.ActivityTypeId;

            ExecuteResponse(response.ResponseTypeId, response.SensorValue, response.IsSystem);
        }

        private void SlideShowComplete(object sender, EventArgs e)
        {
            try
            {
                slideViewerFlash1.Hide();
                ResumeAmbient();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Main.SlideShowComplete: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void MediaPlayerComplete(object sender, EventArgs e)
        {
            try
            {
                mediaPlayer1.Hide();
                ResumeAmbient();
                _isNewResponse = true;
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Main.MediaPlayerComplete: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LogGameEvent(object sender, EventArgs e)
        {
            try
            {
                var args = (MatchingGame.LogGameEventEventArgs)e;
                _gameEventLogger.Add(_activeProfile.ResidentId, args.GameTypeId, args.DifficultyLevel, args.Success, args.Description);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Main.LogGameEvent: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void MatchingGameTimeoutExpired(object sender, EventArgs e)
        {
            try
            {
                _isMatchingGameTimeoutExpired = true;
                matchingGame1.Hide();
                ResumeAmbient();
                _isNewResponse = true;
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Main.MatchingGameTimeoutExpiredEvent: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LogVideoActivityEvent(object sender, EventArgs e)
        {
            try
            {
                var args = (MediaPlayer.LogVideoActivityEventEventArgs)e;
                _activityEventLogger.Add(_activeProfile.ConfigId, _activeProfile.ResidentId, _currenActivityTypeId, _currentResponseTypeId, args.Description);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Main.LogGameEvent: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void CaregiverComplete(object sender, EventArgs e)
        {
            try
            {
                ResumeAmbient();
                _isNewResponse = true;
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Main.CaregiverComplete: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void MainShown(object sender, EventArgs e)
        {
            try
            {
                // set any properties that can't be set in the constructor (because they are not initialized until now)
                SetPostLoadProperties();

                // inform the state machine service that the display is now active and idle
                _messageQueueDisplay.Send(CreateDisplayMessageBody(true));

                ambient1.Show();
                ambient1.Play(_ambientPlaylist);
            }
            catch(Exception ex)
            {
                _systemEventLogger.WriteEntry($"Main.MainShown: {ex.Message}", EventLogEntryType.Error);
            }
        }

        #endregion
    }
}
