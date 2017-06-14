using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.Main.Helpers;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Main.Extensions;
using Keebee.AAT.Main.UserControls;
using Keebee.AAT.MessageQueuing;
using Keebee.AAT.Shared;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Keebee.AAT.Main
{
    internal enum ResponseValueChangeType
    {
        Increase = 0,
        Decrease = 1,
        NoDifference = 2
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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

        private string[] _ambientPlaylist;
        public string[] AmbientPlaylist
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
        private readonly CustomMessageQueue _messageQueueVideoCapture;

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
        //private CaregiverInterface _caregiverInterface;

        // custom event loggers
        private readonly InteractiveActivityEventLogger _interactiveActivityEventLogger;
        private readonly ActivityEventLogger _activityEventLogger;

        // opacity layer (for volume control or any other modal dialogs)
        //private readonly OpaqueLayer _opaqueLayer;

        // active resident display timer
        private readonly Timer _residentDisplayTimer;

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            ConfigureUserControls();
            InitializeAmbientPlayer();

            _residentDisplayTimer = new Timer { Interval = 3000 };
            _residentDisplayTimer.Elapsed += ActiveResidentTimerElapsed;

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

            // video-capture message queue sender
            _messageQueueVideoCapture = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.VideoCapture
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

            AmbientPlayer.ScreenTouchedEvent += AmbientScreenTouched;

            _currentResponseTypeId = ResponseTypeId.Ambient;

            InitializeStartupPosition();

            LoadAmbientMediaPlaylist();
        }

        #region initialization

        private void InitializeStartupPosition()
        {
            ShowInTaskbar = false;

            Top = 0;
            Left = 0;
#if DEBUG
            // set form size to 1/3 primary monitor size
            Width = SystemParameters.PrimaryScreenWidth / 3;
            Height = SystemParameters.PrimaryScreenHeight / 3;

#elif !DEBUG
            WindowState = WindowState.Maximized;
#endif
        }

        private void ConfigureUserControls()
        {
            AmbientPlayer.Visibility = Visibility.Visible;
            //mediaPlayer1.Dock = DockStyle.Fill;
            //mediaPlayer1.SendToBack();
            //mediaPlayer1.Hide();

            //radioControl1.Dock = DockStyle.Fill;
            //radioControl1.SendToBack();
            //radioControl1.Hide();

            //slideViewerFlash1.Dock = DockStyle.Fill;
            //slideViewerFlash1.SendToBack();
            //slideViewerFlash1.Hide();

            //matchingGame1.Dock = DockStyle.Fill;
            //matchingGame1.SendToBack();
            //matchingGame1.Hide();

            //paintingActivity1.Dock = DockStyle.Fill;
            //paintingActivity1.SendToBack();
            //paintingActivity1.Hide();

            //offScreen1.Dock = DockStyle.Fill;
            //offScreen1.SendToBack();
            //offScreen1.Hide();
            ActiveResidentLabel.FontFamily = new FontFamily("Generic Sans Serif");
#if DEBUG       
            ActiveResidentLabel.FontSize = 13;
#elif !DEBUG
            ActiveResidentLabel.FontSize = 17;
#endif
            ActiveResidentLabel.Visibility = Visibility.Hidden;
        }

        private void SetPostLoadProperties()
        {
            // the _systemEventLogger is not available in the constructor - this gets called later in MainShown()
            _messageQueueDisplaySms.SystemEventLogger = _systemEventLogger;
            _messageQueueDisplayPhidget.SystemEventLogger = _systemEventLogger;
            _messageQueueResponse.SystemEventLogger = _systemEventLogger;

            _interactiveActivityEventLogger.SystemEventLogger = _systemEventLogger;
            _activityEventLogger.EventLogger = _systemEventLogger;

            AmbientPlayer.SystemEventLogger = _systemEventLogger;
            //mediaPlayer1.SystemEventLogger = _systemEventLogger;
            //slideViewerFlash1.SystemEventLogger = _systemEventLogger;
            //matchingGame1.SystemEventLogger = _systemEventLogger;
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

            AmbientPlayer.InvitationMessages = AmbientInvitationMessages.Load(
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

            AmbientPlayer.InitializeTimers(durationInvitation, durationVideo);
        }

        #endregion


        private void LoadAmbientMediaPlaylist()
        {
            try
            {
                // create a temporary media player for loading the ambient playlist
                var mediaFileQuery = new MediaFileQuery();
                var ambientFiles = mediaFileQuery.GetFilesForResponseType(PublicProfileSource.Id, ResponseTypeId.Ambient, MediaPathTypeId.Ambient);

                if (ambientFiles.Length > 1)
                    ambientFiles.Shuffle();

                _ambientPlaylist = ambientFiles;

            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Main.LoadAmbientMediaPlaylist: {ex.Message}", EventLogEntryType.Error);
            }
        }

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
                       // PlayMatchingGame();
                        break;
                    case ResponseTypeId.PaintingActivity:
                        //PlayPaintingActivity();
                        break;
                    case ResponseTypeId.KillDisplay:
                        //KillDisplay();
                        break;
                    case ResponseTypeId.Radio:
                    case ResponseTypeId.Television:
                    case ResponseTypeId.Cats:
                        //PlayMedia(responseTypeId, sensorValue);
                        break;
                    case ResponseTypeId.Caregiver:
                        //ShowCaregiver();
                        break;
                    case ResponseTypeId.Ambient:
                        ResumeAmbient();
                        break;
                    case ResponseTypeId.VolumeControl:
                        //ShowVolumeControl();
                        break;
                    case ResponseTypeId.OffScreen:
                        //ShowOffScreen();
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
                        SlideViewer.Visibility = Visibility.Hidden;
                        SlideViewer.Stop();
                        //mediaPlayer1.Stop();
                        break;
                    case ResponseTypeId.MatchingGame:
                        //matchingGame1.Hide();
                        //matchingGame1.Stop(_isMatchingGameTimeoutExpired);
                        break;
                    case ResponseTypeId.PaintingActivity:
                        //paintingActivity1.Hide();
                        //paintingActivity1.Stop(_isPaintingActivityTimeoutExpired);
                        break;
                    case ResponseTypeId.Radio:
                    case ResponseTypeId.Television:
                    case ResponseTypeId.Cats:
                        //radioControl1.Hide();
                        if (newResponseTypeid != ResponseTypeId.Television &&
                            newResponseTypeid != ResponseTypeId.Cats)
                        {
                            //mediaPlayer1.Hide();
                        }
                        //mediaPlayer1.Stop();
                        break;
                    case ResponseTypeId.Ambient:
                        AmbientPlayer.Visibility = Visibility.Hidden;
                        AmbientPlayer.Pause();
                        break;
                    case ResponseTypeId.OffScreen:
                        //offScreen1.Hide();
                        //offScreen1.Stop();
                        break;
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Main.StopCurrentResponse: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void DisplayActiveResident()
        {
            //Dispatcher.Invoke(() =>
            //{
            ActiveResidentLabel.Visibility = Visibility.Hidden;
            _residentDisplayTimer.Stop();

            ActiveResidentLabel.Content = _activeResident.Name;
            //ActiveResidentLabel.Left = Width - lblActiveResident.Width;
            ActiveResidentLabel.Visibility = Visibility.Visible;

            //var grid = MainGrid;
            //Panel.SetZIndex(ActiveResidentLabel, 5);
            _residentDisplayTimer.Start();
            //});
        }

        #region callback

        private void ResumeAmbient()
        {
            StopCurrentResponse();
            AmbientPlayer.Visibility = Visibility.Visible;
            AmbientPlayer.Resume();

            _currentResponseTypeId = ResponseTypeId.Ambient;
        }

        private void PlaySlideShow()
        {
            Dispatcher.Invoke(() =>
            {
                var mediaFileQuery = new MediaFileQuery();
                var images = mediaFileQuery.GetFilesForResponseType(_activeResident.Id, ResponseTypeId.SlideShow);
                if (!images.Any()) return;

                var music = mediaFileQuery.GetFilesForResponseType(_activeResident.Id, ResponseTypeId.Radio,
                    MediaPathTypeId.Music);
                if (!music.Any()) return;

                StopCurrentResponse();

                images.Shuffle();
                SlideViewer.Visibility = Visibility.Visible;
                SlideViewer.Play(images, autoStart: true);
                //Panel.SetZIndex(SlideViewer, 3);

                music.Shuffle();
                //mediaPlayer1.Play(0, new[] {music.First()}, false, false);

                DisplayActiveResident();

                if (_currentIsActiveEventLog)
                    _activityEventLogger.Add(_activeConfigDetail.ConfigId, _activeConfigDetail.Id, _activeResident.Id);

                _currentResponseTypeId = ResponseTypeId.SlideShow;
            });

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

        private void ActiveResidentTimerElapsed(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                ActiveResidentLabel.Visibility = Visibility.Hidden;
            });
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
                        //PlaySlideShow();
                        break;
                    case ResponseTypeId.MatchingGame:
                        //PlayMatchingGame();
                        break;
                    case ResponseTypeId.Cats:
                    case ResponseTypeId.Radio:
                    case ResponseTypeId.Television:
                        _isNewResponse = true;
                        //PlayMedia(responseTypeId, 0);
                        break;
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Main.AmbientScreenTouched: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
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
                AmbientPlayer.Visibility = Visibility.Visible;
                AmbientPlayer.Play(_ambientPlaylist);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Main.MainShown: {ex.Message}", EventLogEntryType.Error);
            }
        }

        #endregion

    }
}
