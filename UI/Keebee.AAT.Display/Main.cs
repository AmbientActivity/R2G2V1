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
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using Keebee.AAT.Display.Extensions;
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
        private int _currentResponseTypeId;

        // active event logging
        private bool _currentIsActiveEventLog;

        // active activity/response
        private ActiveConfigDetail _activeConfigDetail;

        // active profile
        private ActiveResident _activeResident;

        // flags
        private bool _isNewResponse;
        private bool _isMatchingGameTimeoutExpired;

        // caregiver interface
        private CaregiverInterface _caregiverInterface;

        // custom event loggers
        private readonly GameEventLogger _gameEventLogger;
        private readonly ActivityEventLogger _activityEventLogger;

        // media path
        private readonly MediaSourcePath _mediaPath = new MediaSourcePath();

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

            _currentResponseTypeId = ResponseTypeId.Ambient;

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

            // if it is a media player or caregiver response type then execute it
            return (responseTypeId == ResponseTypeId.Television)
                || (responseTypeId == ResponseTypeId.Radio); 
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

        private string[] GetFilesForResponseType(int responseTypeId, int mediaPathTypeId)
        {
            var files = new string[0];
            var mediaFiles = _activeResident.Id == PublicMediaSource.Id
                ? _opsClient.GetPublicMediaFiles().MediaFiles.ToArray()
                : _opsClient.GetResidentMediaFilesForResident(_activeResident.Id).MediaFiles.ToArray();

            var numResponseTypes = mediaFiles.Count(x => x.ResponseType.Id == responseTypeId);
       
            if (numResponseTypes != 1) return files;
            {
                files = GetFiles(mediaFiles, responseTypeId, mediaPathTypeId);

                if (files.Any()) return files;
            }

            return files;
        }

        private string[] GetFiles(ICollection<MediaResponseType> mediaFiles, int responseTypeId, int mediaPathTypeId)
        {
            var pathRoot = $@"{_mediaPath.ProfileRoot}\{_activeResident.Id}";

            var mediaPaths = mediaFiles
                .Single(x => x.ResponseType.Id == responseTypeId).Paths.ToArray();

            var mediaPathType = mediaPaths
                .Single(x => x.MediaPathType.Id == mediaPathTypeId)
                .MediaPathType.Description;

            var fileList = mediaFiles
                .Single(x => x.ResponseType.Id == responseTypeId).Paths
                .Where(x => x.MediaPathType.Id == mediaPathTypeId)
                .SelectMany(x => x.Files)
                .OrderBy(x => x.Filename)
                .Select(x => $@"{pathRoot}\{mediaPathType}\{x.Filename}")
                .ToArray();

            return fileList.Any() ? fileList : new string[0];
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
                    var mediaPathTypeId = 0;
                    switch (responseTypeId)
                    {
                        case ResponseTypeId.Radio:
                            mediaPathTypeId = MediaPathTypeId.Music;
                            break;
                        case ResponseTypeId.Television:
                            mediaPathTypeId = MediaPathTypeId.Videos;
                            break;
                        case ResponseTypeId.Cats:
                            mediaPathTypeId = MediaPathTypeId.Videos;
                            break;
                    }

                    var mediaFiles = GetFilesForResponseType(responseTypeId, mediaPathTypeId);
                    if (!mediaFiles.Any()) return;

                    mediaFiles.Shuffle();
                    StopCurrentResponse();

                    mediaPlayer1.Show();
                    mediaPlayer1.Play(responseTypeId, mediaFiles, _currentIsActiveEventLog);

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
                var images = GetFilesForResponseType(ResponseTypeId.SlidShow, MediaPathTypeId.Images);
                if (!images.Any()) return;

                images.Shuffle();
                StopCurrentResponse();

                slideViewerFlash1.Show();
                slideViewerFlash1.Play(images, true, true);

                if (_currentIsActiveEventLog)
                    _activityEventLogger.Add(_activeResident.ConfigId, _activeConfigDetail.Id, _activeResident.Id);

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
                var shapes = GetFilesForResponseType(ResponseTypeId.MatchingGame, MediaPathTypeId.Shapes);
                if (!shapes.Any()) return;

                shapes.Shuffle();
                StopCurrentResponse();

                _isMatchingGameTimeoutExpired = false;

                matchingGame1.Show();

                if (_currentIsActiveEventLog)
                {
                    _activityEventLogger.Add(_activeResident.ConfigId, _activeConfigDetail.Id, _activeResident.Id);
                    _gameEventLogger.Add(_activeResident.Id, GameTypeId.MatchThePictures, _activeResident.GameDifficultyLevel, null, "New game has been initiated");
                }

                matchingGame1.Play(shapes, _activeResident.GameDifficultyLevel, true, _currentIsActiveEventLog);

                _currentResponseTypeId = ResponseTypeId.MatchingGame;
            }
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

                var publicMedia = _opsClient.GetPublicMediaFiles().MediaFiles;
                var config = _opsClient.GetActiveConfigDetails();

                _caregiverInterface = new CaregiverInterface
                                         {
                                            EventLogger = _systemEventLogger,
                                            OperationsClient = _opsClient,
                                            Config = config,
                                            MediaFiles = publicMedia
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
            try
            {
                var serializer = new JavaScriptSerializer();
                var response = serializer.Deserialize<ResponseMessage>(e.MessageBody);

                _isNewResponse = 
                     (response.ActiveConfigDetail.ResponseTypeId != _currentResponseTypeId) ||
                     (response.ActiveResident.Id != _activeResident?.Id);

                _activeResident = response.ActiveResident;
                _activeConfigDetail = response.ActiveConfigDetail;
                _currentIsActiveEventLog = response.IsActiveEventLog;

                ExecuteResponse(response.ActiveConfigDetail.ResponseTypeId, response.SensorValue, response.IsSystem);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Main.MessageReceived: {ex.Message}", EventLogEntryType.Error);
            }
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
                _gameEventLogger.Add(_activeResident.Id, args.GameTypeId, args.DifficultyLevel, args.Success, args.Description);
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
                _activityEventLogger.Add(_activeResident.ConfigId, _activeConfigDetail.Id, _activeResident.Id, args.Description);
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
