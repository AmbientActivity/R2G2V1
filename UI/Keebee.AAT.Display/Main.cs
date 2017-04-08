using Keebee.AAT.MessageQueuing;
using Keebee.AAT.Shared;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Display.UserControls;
using Keebee.AAT.Display.Caregiver;
using Keebee.AAT.Display.Helpers;
using Keebee.AAT.Display.Volume;
using Keebee.AAT.Display.Extensions;
using Keebee.AAT.Display.Models;
using Keebee.AAT.ApiClient.Clients;
using System.Web.Script.Serialization;
using System.Diagnostics;
using System.Drawing;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows.Forms;
using System.Linq;
using AxWMPLib;
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
        // api client
        private readonly IPublicMediaFilesClient _publicMediaFilesClient;
        private readonly IConfigsClient _configsClient;

        #region declaration

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
        private delegate void ShowOffScreenDelegate();
        private delegate void ShowVolumeControlDelegate();
        private delegate void PlayPaintingActivityDelegate();

        // message queue sender
        private readonly CustomMessageQueue _messageQueueDisplaySms;
        private readonly CustomMessageQueue _messageQueueDisplayPhidget;
        private readonly CustomMessageQueue _messageQueueDisplayVideoCapture;
        private readonly CustomMessageQueue _messageQueueDisplayBluetoothBeaconWatcher;

        // message queue listener
        private readonly CustomMessageQueue _messageQueueResponse;

        // current sensor values
        private int _currentRadioSensorValue;
        private int _currentTelevisionSensorValue;

        // current activity/response types
        private int _currentResponseTypeId;
        private int _currentPhidgetTypeId;

        // active event logging
        private bool _currentIsActiveEventLog;

        // active activity/response
        private ConfigDetailMessage _activeConfigDetail;

        // list of all active response type ids
        private int[] _activeResponseTypeIds;

        // active profile
        private ResidentMessage _activeResident;

        // flags
        private bool _isNewResponse;
        private bool _isMatchingGameTimeoutExpired;
        private bool _isPaintingActivityTimeoutExpired;

        // caregiver interface
        private CaregiverInterface _caregiverInterface;

        // custom event loggers
        private readonly InteractiveActivityEventLogger _interactiveActivityEventLogger;
        private readonly ActivityEventLogger _activityEventLogger;

        // opacity layer (for volume control or any other modal dialogs)
        private readonly OpaqueLayer _opaqueLayer;

        // active resident display timer
        private readonly Timer _residentDisplayTimer;

        #endregion

        public Main()
        {
            InitializeComponent();
            ConfigureUserControls();
            InitializeAmbientPlayer();

            _opaqueLayer = new OpaqueLayer { Dock = DockStyle.Fill };
            _residentDisplayTimer = new Timer { Interval = 3000 };
            _residentDisplayTimer.Tick += ActiveResidentTimerTick;

            _publicMediaFilesClient = new PublicMediaFilesClient();
            _configsClient = new ConfigsClient();

            #region message queue inititialization

            // display-sms message queue sender
            _messageQueueDisplaySms = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.DisplaySms
            });

            // display-phidget message queue sender
            _messageQueueDisplayPhidget = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.DisplayPhidget
            });

            // display-video-capture message queue sender
            _messageQueueDisplayVideoCapture = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.DisplayVideoCapture
            });

            // display-bluetooth-beacon-watcher message queue sender
            _messageQueueDisplayBluetoothBeaconWatcher = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.DisplayBluetoothBeaconWatcher
            });

            // response message queue listener
            _messageQueueResponse = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.Response,
                MessageReceivedCallback = MessageReceivedResponse
            });

            #endregion

            // custom event loggers
            _interactiveActivityEventLogger = new InteractiveActivityEventLogger();
            _activityEventLogger = new ActivityEventLogger();

            // response complete event handlers
            ambientPlayer1.ScreenTouchedEvent += AmbientScreenTouched;
            slideViewerFlash1.SlideShowCompleteEvent += SlideShowComplete;
            mediaPlayer1.MediaPlayerCompleteEvent += MediaPlayerComplete;
            offScreen1.OffScreenCompleteEvent += OffScreenComplete;
            matchingGame1.MatchingGameTimeoutExpiredEvent += MatchingGameTimeoutExpired;
            matchingGame1.LogInteractiveActivityEventEvent += LogInteractiveActivityEvent;
            mediaPlayer1.LogVideoActivityEventEvent += LogVideoActivityEvent;
            paintingActivity1.PaintingActivityTimeoutExpiredEvent += PaintingActivityTimeoutExpired;
            paintingActivity1.LogInteractiveActivityEventEvent += LogInteractiveActivityEvent;

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
            Width = SystemInformation.PrimaryMonitorSize.Width / 3;
            Height = SystemInformation.PrimaryMonitorSize.Height / 3;

#elif !DEBUG
            WindowState = FormWindowState.Maximized;
#endif
        }

        private void InitializeAmbientPlayer()
        {
            // ambient invitation messages
            var durationInvitation = Convert.ToInt32(ConfigurationManager.AppSettings["AmbientInvitationDuration"].Trim());
            var durationVideo = Convert.ToInt32(ConfigurationManager.AppSettings["AmbientVideoDuration"].Trim());

            var invitationMessage1 = ConfigurationManager.AppSettings["InvitationMessage1"].Trim();
            var invitationMessage2 = ConfigurationManager.AppSettings["InvitationMessage2"].Trim();
            var invitationMessage3 = ConfigurationManager.AppSettings["InvitationMessage3"].Trim();
            var invitationMessage4 = ConfigurationManager.AppSettings["InvitationMessage4"].Trim();
            var invitationMessage5 = ConfigurationManager.AppSettings["InvitationMessage5"].Trim();
            var invitationMessage6 = ConfigurationManager.AppSettings["InvitationMessage6"].Trim();
            var invitationMessage7 = ConfigurationManager.AppSettings["InvitationMessage7"].Trim();
            var invitationMessage8 = ConfigurationManager.AppSettings["InvitationMessage8"].Trim();
            var invitationMessage9 = ConfigurationManager.AppSettings["InvitationMessage9"].Trim();
            var invitationMessage10 = ConfigurationManager.AppSettings["InvitationMessage10"].Trim();

            // ambient invitation response types
            var invitationResponse1 = Convert.ToInt32(ConfigurationManager.AppSettings["InvitationResponse1"].Trim());
            var invitationResponse2 = Convert.ToInt32(ConfigurationManager.AppSettings["InvitationResponse2"].Trim());
            var invitationResponse3 = Convert.ToInt32(ConfigurationManager.AppSettings["InvitationResponse3"].Trim());
            var invitationResponse4 = Convert.ToInt32(ConfigurationManager.AppSettings["InvitationResponse4"].Trim());
            var invitationResponse5 = Convert.ToInt32(ConfigurationManager.AppSettings["InvitationResponse5"].Trim());
            var invitationResponse6 = Convert.ToInt32(ConfigurationManager.AppSettings["InvitationResponse6"].Trim());
            var invitationResponse7 = Convert.ToInt32(ConfigurationManager.AppSettings["InvitationResponse7"].Trim());
            var invitationResponse8 = Convert.ToInt32(ConfigurationManager.AppSettings["InvitationResponse8"].Trim());
            var invitationResponse9 = Convert.ToInt32(ConfigurationManager.AppSettings["InvitationResponse9"].Trim());
            var invitationResponse10 = Convert.ToInt32(ConfigurationManager.AppSettings["InvitationResponse10"].Trim());

            ambientPlayer1.InvitationMessages = AmbientInvitationMessages.Load(
                new[]
                {
                    invitationMessage1, invitationMessage2, invitationMessage3, invitationMessage4, invitationMessage5,
                    invitationMessage6, invitationMessage7, invitationMessage8, invitationMessage9, invitationMessage10
                },
                new[]
                {
                    invitationResponse1, invitationResponse2, invitationResponse3, invitationResponse4, invitationResponse5,
                    invitationResponse6, invitationResponse7, invitationResponse8, invitationResponse9, invitationResponse10
                });

            ambientPlayer1.InitializeTimers(durationInvitation, durationVideo);
        }

        private void ConfigureUserControls()
        {
            ambientPlayer1.Dock = DockStyle.Fill;
            ambientPlayer1.BringToFront();
            ambientPlayer1.Show();

            mediaPlayer1.Dock = DockStyle.Fill;
            mediaPlayer1.SendToBack();
            mediaPlayer1.Hide();

            radioControl1.Dock = DockStyle.Fill;
            radioControl1.SendToBack();
            radioControl1.Hide();

            slideViewerFlash1.Dock = DockStyle.Fill;
            slideViewerFlash1.SendToBack();
            slideViewerFlash1.Hide();

            matchingGame1.Dock = DockStyle.Fill;
            matchingGame1.SendToBack();
            matchingGame1.Hide();

            paintingActivity1.Dock = DockStyle.Fill;
            paintingActivity1.SendToBack();
            paintingActivity1.Hide();

            offScreen1.Dock = DockStyle.Fill;
            offScreen1.SendToBack();
            offScreen1.Hide();

#if DEBUG
            var screenWidth = SystemInformation.PrimaryMonitorSize.Width / 3;
            lblActiveResident.Font = new Font(FontFamily.GenericSansSerif, 13);
#elif !DEBUG
            var screenWidth = SystemInformation.PrimaryMonitorSize.Width;
            lblActiveResident.Font = new Font(FontFamily.GenericSansSerif, 17);
#endif
            lblActiveResident.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lblActiveResident.SendToBack();
            lblActiveResident.Hide();
        }

        private void SetPostLoadProperties()
        {
            // the _systemEventLogger is not available in the constructor - this gets called later in MainShown()
            _messageQueueDisplaySms.SystemEventLogger = _systemEventLogger;
            _messageQueueDisplayPhidget.SystemEventLogger = _systemEventLogger;
            _messageQueueResponse.SystemEventLogger = _systemEventLogger;

            _interactiveActivityEventLogger.SystemEventLogger = _systemEventLogger;
            _activityEventLogger.EventLogger = _systemEventLogger;

            ambientPlayer1.SystemEventLogger = _systemEventLogger;
            mediaPlayer1.SystemEventLogger = _systemEventLogger;
            slideViewerFlash1.SystemEventLogger = _systemEventLogger;
            matchingGame1.SystemEventLogger = _systemEventLogger;
        }

        #endregion

        private void ExecuteResponse(int responseTypeId, int sensorValue, bool isSystem)
        {
            if (!isSystem)
                if (!ShouldExecute(responseTypeId)) return;

            if (_currentResponseTypeId == ResponseTypeId.Caregiver && 
                responseTypeId != ResponseTypeId.VolumeControl)  
                return;

            try
            {
                switch (responseTypeId)
                {
                    case ResponseTypeId.SlideShow:
                        PlaySlideShow();
                        break;
                    case ResponseTypeId.MatchingGame:
                        PlayMatchingGame();
                        break;
                    case ResponseTypeId.PaintingActivity:
                        PlayPaintingActivity();
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
                    case ResponseTypeId.VolumeControl:
                        ShowVolumeControl();
                        break;
                    case ResponseTypeId.OffScreen:
                        ShowOffScreen();
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

            // if it is a' media player' or 'off screen' response type then execute it
            return (responseTypeId == ResponseTypeId.Television)
                || (responseTypeId == ResponseTypeId.Radio)
                || (responseTypeId == ResponseTypeId.OffScreen);
        }

        private void StopCurrentResponse(int newResponseTypeid = -1)
        {
            try
            {
                switch (_currentResponseTypeId)
                {
                    case ResponseTypeId.SlideShow:
                        slideViewerFlash1.Hide();
                        slideViewerFlash1.SendToBack();
                        slideViewerFlash1.Stop();
                        mediaPlayer1.Stop();
                        break;
                    case ResponseTypeId.MatchingGame:
                        matchingGame1.Hide();
                        matchingGame1.SendToBack();
                        matchingGame1.Stop(_isMatchingGameTimeoutExpired);
                        break;
                    case ResponseTypeId.PaintingActivity:
                        paintingActivity1.Hide();
                        paintingActivity1.SendToBack();
                        paintingActivity1.Stop(_isPaintingActivityTimeoutExpired);
                        break;
                    case ResponseTypeId.Radio:
                    case ResponseTypeId.Television:
                    case ResponseTypeId.Cats:
                        radioControl1.Hide();
                        radioControl1.SendToBack();
                        if (newResponseTypeid != ResponseTypeId.Television &&
                            newResponseTypeid != ResponseTypeId.Cats)
                        {
                            mediaPlayer1.SendToBack();
                            mediaPlayer1.Hide();
                        }
                        mediaPlayer1.Stop();
                        break;
                    case ResponseTypeId.Ambient:
                        ambientPlayer1.Hide();
                        ambientPlayer1.SendToBack();
                        ambientPlayer1.Pause();
                        break;
                    case ResponseTypeId.OffScreen:
                        offScreen1.Hide();
                        offScreen1.SendToBack();
                        offScreen1.Stop();
                        break;
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Main.StopCurrentResponse: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private int _currentSequentialResponseTypeIndex = -1;
        private void ExecuteSequential(IEnumerable<int> responseTypeIds)
        {
            var activeIds = responseTypeIds.Where(x => _activeResponseTypeIds.Contains(x)).ToArray();

            if (!activeIds.Any())
            {
                ResumeAmbient();
            }
            else
            {
                if (_currentSequentialResponseTypeIndex < activeIds.Length - 1)
                    _currentSequentialResponseTypeIndex++;
                else
                    _currentSequentialResponseTypeIndex = 0;

                var responseTypeId = activeIds[_currentSequentialResponseTypeIndex];

                switch (responseTypeId)
                {
                    case ResponseTypeId.MatchingGame:
                        PlayMatchingGame();
                        break;
                    case ResponseTypeId.PaintingActivity:
                        PlayPaintingActivity();
                        break;
                    case ResponseTypeId.SlideShow:
                        PlaySlideShow();
                        break;
                    case ResponseTypeId.Radio:
                        PlayMedia(ResponseTypeId.Radio, _currentRadioSensorValue);
                        break;
                    case ResponseTypeId.Television:
                        PlayMedia(ResponseTypeId.Television, _currentTelevisionSensorValue);
                        break;
                    case ResponseTypeId.Cats:
                        PlayMedia(ResponseTypeId.Cats, 0);
                        break;
                }
            }
        }

        private void DisplayActiveResident()
        {
            lblActiveResident.Hide();
            lblActiveResident.SendToBack();
            _residentDisplayTimer.Stop();

            lblActiveResident.Text = _activeResident.Name;
            lblActiveResident.Left = Width - lblActiveResident.Width;
            lblActiveResident.BringToFront();
            lblActiveResident.Show();

            _residentDisplayTimer.Start();
        }

        #region callback

        private void PlayMedia(int responseTypeId, int responseValue)
        {
            if (InvokeRequired)
            {
                Invoke(new PlayMediaDelegate(PlayMedia), responseTypeId, responseValue);
            }
            else
            {
                if (_isNewResponse)
                {
                    var mediaFileQuery = new MediaFileQuery();                      
                    var mediaFiles = mediaFileQuery.GetFilesForResponseType(_activeResident.Id, responseTypeId);

                    if (!mediaFiles.Any()) return;

                    mediaFiles.Shuffle();
                    StopCurrentResponse(responseTypeId);

                    switch (responseTypeId)
                    {
                        case ResponseTypeId.Radio:
#if !DEBUG
                            radioControl1.BringToFront();
                            radioControl1.Show();
#elif DEBUG
                            mediaPlayer1.BringToFront();
                            mediaPlayer1.Show();
#endif
                            break;
                        default:
                            mediaPlayer1.BringToFront();
                            mediaPlayer1.Show();
                            break;
                    }

                    mediaPlayer1.Play(responseValue, mediaFiles, _currentIsActiveEventLog, false);
                    DisplayActiveResident();

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
                var mediaFileQuery = new MediaFileQuery();
                var images = mediaFileQuery.GetFilesForResponseType(_activeResident.Id, ResponseTypeId.SlideShow);
                if (!images.Any()) return;

                var music = mediaFileQuery.GetFilesForResponseType(_activeResident.Id, ResponseTypeId.Radio, MediaPathTypeId.Music);
                if (!music.Any()) return;

                StopCurrentResponse();

                images.Shuffle();
                slideViewerFlash1.BringToFront();
                slideViewerFlash1.Show();
                slideViewerFlash1.Play(images, autoStart: true);

                music.Shuffle();
                mediaPlayer1.Play(0, new[] { music.First() }, false, false);

                DisplayActiveResident();

                if (_currentIsActiveEventLog)
                    _activityEventLogger.Add(_activeConfigDetail.ConfigId, _activeConfigDetail.Id, _activeResident.Id);

                _currentResponseTypeId = ResponseTypeId.SlideShow;
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
                var mediaFileQuery = new MediaFileQuery();
                var shapes = mediaFileQuery.GetFilesForResponseType(_activeResident.Id, ResponseTypeId.MatchingGame, MediaPathTypeId.MatchingGameShapes);
                var sounds = mediaFileQuery.GetFilesForResponseType(_activeResident.Id, ResponseTypeId.MatchingGame, MediaPathTypeId.MatchingGameSounds);

                // ensure there are enough shapes and sounds to play the game
                var gameSetup = new MatchingGameSetup();
                var totalShapes = gameSetup.GetTotalShapes(shapes);
                var totalSounds = gameSetup.GetTotalSounds(sounds);

                totalShapes.Shuffle();

                StopCurrentResponse();
                _isMatchingGameTimeoutExpired = false;

                matchingGame1.BringToFront();
                matchingGame1.Show();
                DisplayActiveResident();

                if (_currentIsActiveEventLog)
                {
                    _activityEventLogger.Add(_activeConfigDetail.ConfigId, _activeConfigDetail.Id, _activeResident.Id);
                    _interactiveActivityEventLogger.Add(_activeResident.Id, InteractiveActivityTypeId.MatchingGame, "New game has been initiated", _activeResident.GameDifficultyLevel, null);
                }
    
                matchingGame1.Play(totalShapes, totalSounds, _activeResident.GameDifficultyLevel, true, _currentIsActiveEventLog);

                _currentResponseTypeId = ResponseTypeId.MatchingGame;
            }
        }

        private void PlayPaintingActivity()
        {
            if (InvokeRequired)
            {
                Invoke(new PlayPaintingActivityDelegate(PlayPaintingActivity));
            }
            else
            {
                StopCurrentResponse();
                _isPaintingActivityTimeoutExpired = false;

                paintingActivity1.BringToFront();
                paintingActivity1.Show();
                DisplayActiveResident();

                if (_currentIsActiveEventLog)
                {
                    _activityEventLogger.Add(_activeConfigDetail.ConfigId, _activeConfigDetail.Id, _activeResident.Id);
                }

                paintingActivity1.Play(true, _currentIsActiveEventLog);

                _currentResponseTypeId = ResponseTypeId.PaintingActivity;
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
                ambientPlayer1.BringToFront();
                ambientPlayer1.Show();
                ambientPlayer1.Resume();

                _currentResponseTypeId = ResponseTypeId.Ambient;
            }
        }

        private void ShowVolumeControl()
        {
            if (InvokeRequired)
            {
                Invoke(new ShowVolumeControlDelegate(ShowVolumeControl));
            }
            else
            {
                var volumeControl = new VolumeControl();
                if (volumeControl.IsOpen()) return;

                _opaqueLayer.Show();
                volumeControl.VolumeControlClosedEvent += VolumentControlClosed;
                volumeControl.Show();
            }
        }

        private void ShowOffScreen()
        {
            if (InvokeRequired)
            {
                Invoke(new ShowOffScreenDelegate(ShowOffScreen));
            }
            else
            {
                if (_currentResponseTypeId != ResponseTypeId.OffScreen)
                {
                    StopCurrentResponse();
                    offScreen1.BringToFront();
                    offScreen1.Show();
                    offScreen1.Play();
                    DisplayActiveResident();

                    _currentResponseTypeId = ResponseTypeId.OffScreen;
                }
                else
                {
                    offScreen1.Hide();
                    offScreen1.Stop();

                    // randomly execute one of the following reponse types
                    ExecuteSequential(new [] {
                        ResponseTypeId.MatchingGame,
                        ResponseTypeId.PaintingActivity,
                        ResponseTypeId.SlideShow});

                    DisplayActiveResident();
                }
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

                var mediaResponseTypes = _publicMediaFilesClient.Get();
                var config = _configsClient.GetActiveDetails();

                _caregiverInterface = new CaregiverInterface
                {
                    EventLogger = _systemEventLogger,
                    Config = config,
                    PublicMediaFiles = mediaResponseTypes
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
                _ambientPlaylist.clear();
                // alert the State Machine Service that the display is no longer active or idle
                _messageQueueDisplaySms.Send(CreateDisplayMessageBody(false));
                _messageQueueDisplayPhidget.Send(CreateDisplayMessageBody(false));
                _messageQueueDisplayVideoCapture.Send(CreateDisplayMessageBody(false));
                _messageQueueDisplayBluetoothBeaconWatcher.Send(CreateDisplayMessageBody(false));
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

        private void MessageReceivedResponse(object source, MessageEventArgs e)
        {
            try
            {
                var serializer = new JavaScriptSerializer();
                var response = serializer.Deserialize<ResponseMessage>(e.MessageBody);

                _isNewResponse =
                     (response.ConfigDetail.ResponseTypeId != _currentResponseTypeId) ||
                     (response.ConfigDetail.PhidgetTypeId != _currentPhidgetTypeId) ||
                     (response.Resident.Id != _activeResident?.Id);

                _activeResident = response.Resident;
                _activeConfigDetail = response.ConfigDetail;
                _currentIsActiveEventLog = response.IsActiveEventLog;
                _activeResponseTypeIds = response.ResponseTypeIds;
                _currentPhidgetTypeId = response.ConfigDetail.PhidgetTypeId;

                ExecuteResponse(response.ConfigDetail.ResponseTypeId, response.SensorValue, response.ConfigDetail.IsSystemReponseType);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Main.MessageReceivedResponse: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void AmbientScreenTouched(object sender, EventArgs e)
        {
            try
            {
                var args = (AmbientPlayer.ScreenTouchedEventEventArgs)e;
                var responseTypeId = args.ResponseTypeId;

                StopCurrentResponse();
                switch (responseTypeId)
                {
                    case ResponseTypeId.SlideShow:
                        PlaySlideShow();
                        break;
                    case ResponseTypeId.MatchingGame:
                        PlayMatchingGame();
                        break;
                    case ResponseTypeId.Cats:
                    case ResponseTypeId.Radio:
                    case ResponseTypeId.Television:
                        _isNewResponse = true;
                        PlayMedia(responseTypeId, 0);
                        break;
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Main.AmbientScreenTouched: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void SlideShowComplete(object sender, EventArgs e)
        {
            try
            {
                slideViewerFlash1.Hide();
                mediaPlayer1.Stop();
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

        private void OffScreenComplete(object sender, EventArgs e)
        {
            try
            {
                offScreen1.Hide();
                ResumeAmbient();
                _isNewResponse = true;
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Main.MediaPlayerComplete: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void VolumentControlClosed(object sender, EventArgs e)
        {
            _opaqueLayer.Hide();
        }

        private void LogInteractiveActivityEvent(object sender, EventArgs e)
        {
            try
            {
                var args = (LogInteractiveActivityEventArgs)e;
                _interactiveActivityEventLogger.Add(_activeResident.Id, args.InteractiveActivityTypeId, args.Description, args.DifficultyLevel, args.Success);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Main.LogInteractiveActivityEvent: {ex.Message}", EventLogEntryType.Error);
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

        private void PaintingActivityTimeoutExpired(object sender, EventArgs e)
        {
            try
            {
                _isPaintingActivityTimeoutExpired = true;
                paintingActivity1.Hide();
                ResumeAmbient();
                _isNewResponse = true;
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Main.PaintingActivityTimeoutExpiredEvent: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LogVideoActivityEvent(object sender, EventArgs e)
        {
            try
            {
                var args = (MediaPlayer.LogVideoActivityEventEventArgs)e;
                _activityEventLogger.Add(_activeConfigDetail.ConfigId, _activeConfigDetail.Id, _activeResident.Id, args.Description);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Main.LogVideoActivityEvent: {ex.Message}", EventLogEntryType.Error);
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

                // inform the services that the display is now active
                _messageQueueDisplaySms.Send(CreateDisplayMessageBody(true));
                _messageQueueDisplayPhidget.Send(CreateDisplayMessageBody(true));
                _messageQueueDisplayVideoCapture.Send(CreateDisplayMessageBody(true));
                _messageQueueDisplayBluetoothBeaconWatcher.Send(CreateDisplayMessageBody(true));

                _activeResident = new ResidentMessage
                {
                    Id = PublicProfileSource.Id,
                    Name = PublicProfileSource.Name,
                    AllowVideoCapturing = false,
                    GameDifficultyLevel = PublicProfileSource.GameDifficultyLevel
                };

                ambientPlayer1.Show();
                ambientPlayer1.Play(_ambientPlaylist);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Main.MainShown: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void MainFormClosing(object sender, FormClosingEventArgs e)
        {
            ambientPlayer1.Dock = DockStyle.None;
            slideViewerFlash1.Dock = DockStyle.None;
            matchingGame1.Dock = DockStyle.None;
            paintingActivity1.Dock = DockStyle.None;
            mediaPlayer1.Dock = DockStyle.None;
            radioControl1.Dock = DockStyle.None;
            offScreen1.Dock = DockStyle.None;
        }

        private void ActiveResidentTimerTick(object sender, EventArgs e)
        {
            lblActiveResident.Hide();
            lblActiveResident.SendToBack();
        }

        #endregion
    }
}
