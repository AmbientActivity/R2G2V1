using Keebee.AAT.RESTClient;
using Keebee.AAT.MessageQueuing;
using Keebee.AAT.Constants;
using Keebee.AAT.EventLogging;
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

        private EventLogger _eventLogger;
        public EventLogger EventLogger
        {
            set { _eventLogger = value; }
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

        // current activity values
        private bool _isNewResponse;
        private int _currenActivityTypeId;
        private int _currentResponseId;
        private int _currentResponseTypeId;
        private int _currentTelevisionResponseValue;
        private int _currentRadioResponseValue;
        private int _gameDifficultyLevel;
        private bool _isMatchingGameTimeoutExpired;

        // current resident
        private int _currentResidentId;

        // caregiver interface
        private CaregiverInterface _caregiverInterface;

        // custom event loggers
        private readonly GamingEventLogger _gamingEventLogger;
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
            _gamingEventLogger = new GamingEventLogger();
            _activityEventLogger = new ActivityEventLogger();

            // response complete event handlers
            slideViewerFlash1.SlideShowCompleteEvent += SlideShowComplete;
            mediaPlayer1.MediaPlayerCompleteEvent += MediaPlayerComplete;
            matchingGame1.MatchingGameTimeoutExpiredEvent += MatchingGameTimeoutExpired;
            matchingGame1.LogGamingEventEvent += LogGamingEvent;
            mediaPlayer1.LogActivityEventEvent += LogActivityEvent;
            InitializeStartupPosition();
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
            // the _eventLogger is not available in the constructor - this gets called later in MainShown()
            _opsClient.EventLogger = _eventLogger;
            _messageQueueDisplay.EventLogger = _eventLogger;
            _messageQueueResponse.EventLogger = _eventLogger;

            _gamingEventLogger.OperationsClient = _opsClient;
            _gamingEventLogger.EventLogger = _eventLogger;
            _activityEventLogger.OperationsClient = _opsClient;
            _activityEventLogger.EventLogger = _eventLogger;

            ambient1.EventLogger = _eventLogger;
            mediaPlayer1.EventLogger = _eventLogger;
            slideViewerFlash1.EventLogger = _eventLogger;
            matchingGame1.EventLogger = _eventLogger;
        }

        #endregion

        private void ExecuteSystemResponse(int responseValue)
        {
            switch (responseValue)
            {
                case SystemResponseType.KillDisplay:
                    KillDisplay();
                    break;
                case SystemResponseType.Caregiver:
                    ShowCaregiver();
                    break;
                case SystemResponseType.Ambient:
                    ResumeAmbient();
                    break;
            }
        }

        private void ExecuteUserResponse(int responseTypeId, int responseValue)
        {
            switch (responseTypeId)
            {
                case UserResponseType.SlidShow:
                    PlaySlideShow();
                    break;
                case UserResponseType.MatchingGame:
                    PlayMatchingGame();
                    break;
                case UserResponseType.Radio:
                case UserResponseType.Television:
                case UserResponseType.Cats:
                    PlayMedia(responseTypeId, responseValue);
                    break;
                case UserResponseType.Ambient:
                    ResumeAmbient();
                    break;
            }
        }

        private void ExecuteResponse(int responseId, int responseTypeId, int responseValue)
        {
            if (!IsValidResponse(responseId, responseTypeId)) return;
            if (_currentResponseId == SystemResponseType.Caregiver) return;

            try
            {
                _currentResponseId = responseId;

                if (responseId > 0)
                    ExecuteUserResponse(responseTypeId, responseValue);
                else
                    ExecuteSystemResponse(responseId);

                SetCurrentResponseValues(responseTypeId, responseValue);
            }
            catch (Exception ex)
            {
                _eventLogger.WriteEntry($"Main.ExecuteResponse: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void SetCurrentResponseValues(int responseTypeId, int responseValue)
        {
            switch (responseTypeId)
            {
                case UserResponseType.Radio:
                    _currentRadioResponseValue = responseValue;
                    break;
                case UserResponseType.Television:
                    _currentTelevisionResponseValue = responseValue;
                    break;
            }
        }

        private bool IsValidResponse(int responseId, int responseTypeId)
        {
            // if the responseId has changed then execute it
            _isNewResponse = (responseId != _currentResponseId);
            if (_isNewResponse) return true;

            // if it is a user response
            if (responseId > 0)
            {
                // if it is not a media player response type - do nothing
                if ((responseTypeId != UserResponseType.Television) &&
                    (responseTypeId != UserResponseType.Radio))
                    return false;
            }
            else return false;

            // media player response type - execute it
            return true;
        }

        private static ResponseMessage GetResponseFromMessageBody(string messageBody)
        {
            var serializer = new JavaScriptSerializer();
            var response = serializer.Deserialize<ResponseMessage>(messageBody);
            return response;
        }

        private void StopCurrentResponse()
        {
            switch (_currentResponseTypeId)
            {
                case UserResponseType.SlidShow:
                    slideViewerFlash1.Hide();
                    slideViewerFlash1.Stop();
                    break;
                case UserResponseType.MatchingGame:
                    matchingGame1.Hide();
                    matchingGame1.Stop(logEvent: !_isMatchingGameTimeoutExpired);
                    break;
                case UserResponseType.Radio:
                case UserResponseType.Television:
                case UserResponseType.Cats:
                    mediaPlayer1.Hide();
                    mediaPlayer1.Stop();
                    break;
                case UserResponseType.Ambient:
                    ambient1.Hide();
                    ambient1.Pause();
                    break;
            }
        }

        #region callback

        private void PlayMedia (int responseTypeId, int responseValue)
        {
            if (InvokeRequired)
            {
                Invoke(new PlayMediaDelegate(PlayMedia), new object[] { responseTypeId, responseValue });
            }
            else
            {
                if (_isNewResponse)
                {
                    var mediaFiles = GetResponseFiles(_currentResponseId);
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
                case UserResponseType.Radio:
                    currentResponseValue = _currentRadioResponseValue;
                    break;
                case UserResponseType.Television:
                    currentResponseValue = _currentTelevisionResponseValue;
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
                var images = GetResponseFiles(_currentResponseId);
                if (!images.Any()) return;

                StopCurrentResponse();
                slideViewerFlash1.Show();
                slideViewerFlash1.Play(images);
                _currentResponseTypeId = UserResponseType.SlidShow;
                _activityEventLogger.Add(_currentResidentId, _currenActivityTypeId, _currentResponseTypeId);
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
                var shapes = GetResponseFiles(_currentResponseId, "png");
                if (!shapes.Any()) return;

                StopCurrentResponse();
                _isMatchingGameTimeoutExpired = false;

                matchingGame1.Show();
                matchingGame1.Play(shapes, _gameDifficultyLevel, true);

                _currentResponseTypeId = UserResponseType.MatchingGame;
                _activityEventLogger.Add(_currentResidentId, _currenActivityTypeId, _currentResponseTypeId);
            }
        }

        private string[] GetResponseFiles(int profileDetailId, string fileType = null)
        {
            var files = _opsClient.GetProfileResponses(profileDetailId)
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
                _currentResponseTypeId = UserResponseType.Ambient;
                _currentResponseId = UserResponse.Ambient;
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
                var frmSplash = new Caregiver.Splash();
                frmSplash.Show();

                var genericProfile = _opsClient.GetProfileMedia(UserProfile.Generic);

                StopCurrentResponse();
                _caregiverInterface = new CaregiverInterface
                                         {
                                            EventLogger = _eventLogger,
                                            OperationsClient = _opsClient,
                                            GenericProfile = genericProfile
                };

                _caregiverInterface.CaregiverCompleteEvent += CaregiverComplete;

                frmSplash.Close();
                _caregiverInterface.Show();

                _currentResponseTypeId = SystemResponseType.Caregiver;
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
            var message = e.MessageBody;

            var response = GetResponseFromMessageBody(message);

            _currentResidentId = response.ResidentId;
            _currenActivityTypeId = response.ActivityTypeId;
            _gameDifficultyLevel = response.GameDifficultyLevel;

            ExecuteResponse(response.ProfileDetailId, response.ResponseTypeId, response.ResponseValue);
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
                _eventLogger.WriteEntry($"Main.SlideShowComplete: {ex.Message}", EventLogEntryType.Error);
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
                _eventLogger.WriteEntry($"Main.MediaPlayerComplete: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LogGamingEvent(object sender, EventArgs e)
        {
            try
            {
                var args = (MatchingGame.LogGamingEventEventArgs)e;
                _gamingEventLogger.Add(_currentResidentId, args.EventLogEntryTypeId, args.DifficultyLevel, args.Success, args.Description);
            }
            catch (Exception ex)
            {
                _eventLogger.WriteEntry($"Main.LogGamingEvent: {ex.Message}", EventLogEntryType.Error);
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
                _eventLogger.WriteEntry($"Main.MatchingGameTimeoutExpiredEvent: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LogActivityEvent(object sender, EventArgs e)
        {
            try
            {
                var args = (MediaPlayer.LogActivityEventEventArgs)e;
                _activityEventLogger.Add(_currentResidentId, _currenActivityTypeId, _currentResponseTypeId, args.Description);
            }
            catch (Exception ex)
            {
                _eventLogger.WriteEntry($"Main.LogGamingEvent: {ex.Message}", EventLogEntryType.Error);
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
                _eventLogger.WriteEntry($"Main.CaregiverComplete: {ex.Message}", EventLogEntryType.Error);
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
                _eventLogger.WriteEntry($"Main.MainShown: {ex.Message}", EventLogEntryType.Error);
            }
        }

        #endregion
    }
}
