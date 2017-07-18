using Keebee.AAT.MessageQueuing;
using Keebee.AAT.Shared;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Configuration;
using System.Diagnostics;
using System.Web.Script.Serialization;
using Timer = System.Threading.Timer;

namespace Keebee.AAT.Simulator
{
    internal enum StepDirectionType
    {
        Left = 0,
        Right = 1
    }

    public partial class ControlPanel : Form
    {
        // for the random response types when the on/off button is clicked
        private readonly IEnumerable<RandomResponseTypeMessage> _randomResponseTypes = new Collection<RandomResponseTypeMessage>
            {
                GetResponseTypeMessage(ResponseTypeId.SlideShow),
                GetResponseTypeMessage(ResponseTypeId.MatchingGame),
                GetResponseTypeMessage(ResponseTypeId.PaintingActivity),
                GetResponseTypeMessage(ResponseTypeId.BalloonPoppingGame)
            };

        #region declaration

        // constant
        private const string MatchingGame = "MatchingGame.swf";
        private const string PaintingActivity = "PaintingActivity.swf";
        private const string BalloonPoppingGame = "BalloonPoppingGame.swf";

        // data
        private readonly IResidentsClient _residentsClient;
        private Resident[] _residents;

        // message queue sender
        private readonly CustomMessageQueue _messageQueuePhidget;
        private readonly CustomMessageQueue _messageQueueBluetoothBeaconWatcher;
        private readonly CustomMessageQueue _messageQueueResponse;
        private readonly CustomMessageQueue _messageQueuePhidgetContinuousRadio;

        // timer
        private readonly int _autoResponseInterval;
        private Timer _timerSensor;
        private readonly int _autoResidentInterval;
        private Timer _timerResident;

        // activity
        private int _currentSensorId = -1;
        private const int MaxSensorId = 3;

        // resident
        private int _currentResidentIndex = -1;
        private readonly int _totalResidents;
        private Resident _currentResident;
        private readonly Resident _publicResident = new Resident
        {
            Id = PublicProfileSource.Id,
            FirstName = PublicProfileSource.Description,
            GameDifficultyLevel = 1,
            AllowVideoCapturing = false
        };

        // step action
        private const int MaxValue = 1000;
        private const int StepIncrement = 200;

        private int _currentRadio4Value;
        private int _currentTelevsion5Value;

        #endregion

        public ControlPanel()
        {
            InitializeComponent();

            _autoResponseInterval = Convert.ToInt32(ConfigurationManager.AppSettings["AutoSensorInterval"]);
            _autoResidentInterval = Convert.ToInt32(ConfigurationManager.AppSettings["AutoResidentInterval"]);

            _residentsClient = new ResidentsClient();

            // message queue senders
            _messageQueuePhidget = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.Phidget

            });

            _messageQueueBluetoothBeaconWatcher = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.BluetoothBeaconWatcher

            });

            _messageQueueResponse = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.Response

            });

            _messageQueuePhidgetContinuousRadio = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.PhidgetContinuousRadio
            });

            LoadResidentDropDown();
            _totalResidents = _residents.Count();
        }

        #region initialization

        private void LoadResidentDropDown()
        {
            var arrayList = new ArrayList();
            
            _residents = new List<Resident> { _publicResident }
            .Union(_residentsClient.Get()
                .Select(r => new Resident
                    {
                        Id = r.Id,
                        FirstName = r.FirstName,
                        LastName = r.LastName,
                        Gender = r.Gender,
                        GameDifficultyLevel = r.GameDifficultyLevel,
                        AllowVideoCapturing = r.AllowVideoCapturing
                    })).OrderBy(o => o.LastName).ToArray();

            foreach (var r in _residents)
            {
                arrayList.Add(new {r.Id, Name = r.LastName == null 
                    ? $"{r.FirstName}" 
                    : $"{r.LastName}, {r.FirstName}" });
            }

            cboResident.ValueMember = "Id";
            cboResident.DisplayMember = "Name";
            cboResident.DataSource = arrayList;

            // make public library active
            _messageQueueBluetoothBeaconWatcher.Send(CreateMessageBodyForBluetoothBeaconWatcher(new Resident { Id = PublicProfileSource.Id, GameDifficultyLevel =  1}));
        }

        #endregion

        #region button click

        private void KillDisplayButtonClick(object sender, EventArgs e)
        {
            ExecuteResponse(ResponseTypeId.KillDisplay, PhidgetTypeId.Sensor0, MaxValue - 1, true);
        }

        private void SlideShowButtonClick(object sender, EventArgs e)
        {
            ExecuteResponse(ResponseTypeId.SlideShow, PhidgetTypeId.Sensor0);
        }

        private void MatchingGameButtonClick(object sender, EventArgs e)
        {
            ExecuteResponse(ResponseTypeId.MatchingGame, PhidgetTypeId.Sensor0);
        }

        private void CatsButtonClick(object sender, EventArgs e)
        {
            ExecuteResponse(ResponseTypeId.Cats, PhidgetTypeId.Sensor0);
        }

        private void RadioRightButtonClick(object sender, EventArgs e)
        {
            _currentRadio4Value = GetCurrentStepValue(ResponseTypeId.Radio, StepDirectionType.Right);

            var valueToSend = (_currentRadio4Value == MaxValue)
                ? _currentRadio4Value - 1 : _currentRadio4Value;

            if (valueToSend <= 0) return;

            _messageQueuePhidgetContinuousRadio.Send($"{valueToSend}");
            ExecuteResponse(ResponseTypeId.Radio, PhidgetTypeId.Sensor0, valueToSend);
        }

        private void RadioLeftButtonClick(object sender, EventArgs e)
        {
            _currentRadio4Value = GetCurrentStepValue(ResponseTypeId.Radio, StepDirectionType.Left);

            var valueToSend = (_currentRadio4Value == MaxValue)
                ? _currentRadio4Value - 1 : _currentRadio4Value;

            if (valueToSend <= 0) return;

            _messageQueuePhidgetContinuousRadio.Send($"{valueToSend}");
            ExecuteResponse(ResponseTypeId.Radio, PhidgetTypeId.Sensor0, valueToSend);
        }

        private void TelevisionRightButtonClick(object sender, EventArgs e)
        {
            _currentTelevsion5Value = GetCurrentStepValue(ResponseTypeId.Television, StepDirectionType.Right);

            var valueToSend = (_currentTelevsion5Value == MaxValue)
                ? _currentTelevsion5Value - 1 : _currentTelevsion5Value;

            if (valueToSend <= 0) return;

            ExecuteResponse(ResponseTypeId.Television, PhidgetTypeId.Sensor0, valueToSend);
        }

        private void TelevisionLeftButtonClick(object sender, EventArgs e)
        {
            _currentTelevsion5Value = GetCurrentStepValue(ResponseTypeId.Television, StepDirectionType.Left);

            var valueToSend = (_currentTelevsion5Value == MaxValue)
                ? _currentTelevsion5Value - 1 : _currentTelevsion5Value;

            if (valueToSend <= 0) return;

            ExecuteResponse(ResponseTypeId.Television, PhidgetTypeId.Sensor0, valueToSend);
        }

        private int GetCurrentStepValue(int responseType, StepDirectionType direction)
        {
            var currentValue = -1;

            switch (responseType)
            {
                case ResponseTypeId.Radio:
                    currentValue = _currentRadio4Value;
                    break;
                case ResponseTypeId.Television:
                    currentValue = _currentTelevsion5Value;
                    break;
            }

            if (currentValue < 0) return -1;

            switch (direction)
            {
                case StepDirectionType.Left:
                    if ((currentValue - StepIncrement) > 0)
                        currentValue -= StepIncrement;
                    else
                        currentValue = MaxValue;
                    break;

                case StepDirectionType.Right:
                    if ((currentValue + StepIncrement) <= MaxValue)
                        currentValue += StepIncrement;
                    else
                        currentValue = StepIncrement;
                    break;
            }

            return currentValue;
        }

        private void CaregiverButtonClick(object sender, EventArgs e)
        {
            ExecuteResponse(ResponseTypeId.Caregiver, PhidgetTypeId.Sensor0, MaxValue - 1, true);
        }

        private void AmbientButtonClick(object sender, EventArgs e)
        {
            ExecuteResponse(ResponseTypeId.Ambient, PhidgetTypeId.Sensor0, MaxValue - 1, true);
        }

        private void PaintingActivityClick(object sender, EventArgs e)
        {
            ExecuteResponse(ResponseTypeId.PaintingActivity, PhidgetTypeId.Sensor0);
        }

        private void BalloonPoppingGameClick(object sender, EventArgs e)
        {
            ExecuteResponse(ResponseTypeId.BalloonPoppingGame, PhidgetTypeId.Sensor0);
        }

        private void VolumeControlClick(object sender, EventArgs e)
        {
            ExecuteResponse(ResponseTypeId.VolumeControl, PhidgetTypeId.Sensor0);
        }

        private void OffScreenButtonClick(object sender, EventArgs e)
        {
            ExecuteResponse(ResponseTypeId.OffScreen, PhidgetTypeId.Sensor4, MaxValue - 1, false);
        }

        private static RandomResponseTypeMessage GetResponseTypeMessage(int responseTypeId)
        {
            switch (responseTypeId)
            {
                case ResponseTypeId.SlideShow:
                    return new RandomResponseTypeMessage
                    {
                        Id = responseTypeId,
                        InteractiveActivityTypeId = 0,
                        SwfFile = null
                    };
                case ResponseTypeId.MatchingGame:
                    return new RandomResponseTypeMessage
                    {
                        Id = responseTypeId,
                        InteractiveActivityTypeId = InteractiveActivityTypeId.MatchingGame,
                        SwfFile = "MatchingGame.swf"
                    };
                case ResponseTypeId.PaintingActivity:
                    return new RandomResponseTypeMessage
                    {
                        Id = responseTypeId,
                        InteractiveActivityTypeId = InteractiveActivityTypeId.MatchingGame,
                        SwfFile = "PaintingActivity.swf"
                    };
                case ResponseTypeId.BalloonPoppingGame:
                    return new RandomResponseTypeMessage
                    {
                        Id = responseTypeId,
                        InteractiveActivityTypeId = InteractiveActivityTypeId.MatchingGame,
                        SwfFile = "BalloonPoppingGame.swf"
                    };
                default:
                    return null;
            }
        }

        private void NatureButtonClick(object sender, EventArgs e)
        {
            ExecuteResponse(ResponseTypeId.Nature, PhidgetTypeId.Sensor0);
        }

        private void SportsButtonClick(object sender, EventArgs e)
        {
            ExecuteResponse(ResponseTypeId.Sports, PhidgetTypeId.Sensor0);
        }

        private void ActivateResidentClick(object sender, EventArgs e)
        {
            var id = Convert.ToInt32(cboResident.SelectedValue.ToString());

            var resident = (id == PublicProfileSource.Id)
                ? new Resident { Id = PublicProfileSource.Id, GameDifficultyLevel = 1, AllowVideoCapturing = false }
                : _residents.Single(x => x.Id == id);

            CreateMessageBodyForBluetoothBeaconWatcher(resident);
            _messageQueueBluetoothBeaconWatcher.Send(CreateMessageBodyForBluetoothBeaconWatcher(resident));
        }

        #endregion

        #region timer

        private void TimerSensorTick(object sender)
        {
            if (_timerSensor == null) return;

            if (_currentSensorId < MaxSensorId)
                _currentSensorId++;
            else
                _currentSensorId = 0;

            _messageQueuePhidget.Send(string.Format("{0}\"SensorId\":{1},\"SensorValue\":{2}{3}", "{", _currentSensorId, MaxValue - 1, "}"));
        }

        private void TimerResidentTick(object sender)
        {
            if (_timerResident == null) return;

            if (_currentResidentIndex < (_totalResidents - 1))
                _currentResidentIndex++;
            else
                _currentResidentIndex = 0;

            var id = _residents[_currentResidentIndex].Id;

            var resident = (id == PublicProfileSource.Id)
                ? _publicResident
                : _residents.Single(x => x.Id == id);

            CreateMessageBodyForBluetoothBeaconWatcher(resident);
            _messageQueueBluetoothBeaconWatcher.Send(CreateMessageBodyForBluetoothBeaconWatcher(resident));
        }

        #endregion

        #region event handlers

        private void ControlPanelClosing(object sender, FormClosingEventArgs e)
        {
            _timerSensor?.Dispose();
            _timerResident?.Dispose();
        }

        private void AutoResponseCheckChanged(object sender, EventArgs e)
        {
            // Search Auto Activity GroupBox
            foreach (var result in from control in grpAutoSensor.Controls.OfType<RadioButton>() 
                                   select control into radio where radio.Checked select radio.Text)
            {
                if (result == "On")
                {
                    if (_timerSensor == null)
                    {
                        _timerSensor = new Timer(TimerSensorTick, null, 0, _autoResponseInterval);
                    }
                }
                else
                {
                    if (_timerSensor == null) continue;

                    _timerSensor.Dispose();
                    _timerSensor = null;
                }
            }
        }

        private void AutoResidentCheckChanged(object sender, EventArgs e)
        {
            foreach (var result in from control in grpAutoResident.Controls.OfType<RadioButton>() 
                                   select control into radio where radio.Checked select radio.Text)
            {
                if (result == "On")
                {
                    if (_timerResident == null)
                    {
                        _timerResident = new Timer(TimerResidentTick, null, 0, _autoResidentInterval);
                    }
                }
                else
                {
                    if (_timerResident == null) continue;

                    _timerResident.Dispose();
                    _timerResident = null;
                }
            }
        }

        private void ResidentSelectedIndexChanged(object sender, EventArgs e)
        {
            var id = Convert.ToInt32(cboResident.SelectedValue.ToString());
            var resident = _residents.Single(x => x.Id == id);

            _currentResident = new Resident
            {
                Id = resident.Id,
                FirstName = resident.FirstName,
                LastName = resident.LastName,
                Gender = resident.Gender,
                GameDifficultyLevel = resident.GameDifficultyLevel,
                AllowVideoCapturing = resident.AllowVideoCapturing
            };
        }

        #endregion

        #region helper

        private void ExecuteResponse(int responseTypeId, int phidgetTypeId, int sensorValue = MaxValue - 1, bool isSystem = false)
        {
#if !DEBUG
            if (Process.GetProcessesByName($"{AppSettings.Namespace}.{AppSettings.DisplayAppName}").Any())
#endif
            _messageQueueResponse.Send(CreateMessageBodyForResponse(responseTypeId, phidgetTypeId, isSystem, sensorValue));
        }

        private static int GetInteractiveActivityTypeId(int responseTypeId)
        {
            switch (responseTypeId)
            {
                case ResponseTypeId.MatchingGame:
                    return InteractiveActivityTypeId.MatchingGame;
                case ResponseTypeId.PaintingActivity:
                    return InteractiveActivityTypeId.PaintingActivity;
                case ResponseTypeId.BalloonPoppingGame:
                    return InteractiveActivityTypeId.BalloonPoppingGame;
                default:
                    return 0;
            }
        }

        private static string GetInteractiveActivityTypeSwf(int responseTypeId)
        {
            switch (responseTypeId)
            {
                case ResponseTypeId.MatchingGame:
                    return MatchingGame;
                case ResponseTypeId.PaintingActivity:
                    return PaintingActivity;
                case ResponseTypeId.BalloonPoppingGame:
                    return BalloonPoppingGame;
                default:
                    return null;
            }
        }

        private static string CreateMessageBodyForBluetoothBeaconWatcher(Resident resident)
        {
            var residentMessage = new ResidentMessage
            {
                Id = resident.Id,
                Name = $"{resident.FirstName} {resident.LastName}".Trim(),
                GameDifficultyLevel = resident.GameDifficultyLevel,
                AllowVideoCapturing = resident.AllowVideoCapturing
            };

            var serializer = new JavaScriptSerializer();
            var messageBody = serializer.Serialize(residentMessage);

            return messageBody;
        }

        private string CreateMessageBodyForResponse(int responseTypeId, int phidgetTypeId, bool isSystem, int sensorValue)
        {
            var responseMessage = new ResponseMessage
            {
                SensorValue = sensorValue,
                ConfigDetail = new ConfigDetailMessage
                {
                    Id = 1,
                    ResponseTypeId = responseTypeId,
                    PhidgetTypeId = phidgetTypeId,
                    IsSystemReponseType = isSystem,
                    ConfigId = ConfigId.Default,
                    InteractiveActivityTypeId = GetInteractiveActivityTypeId(responseTypeId),
                    SwfFile = GetInteractiveActivityTypeSwf(responseTypeId)
                },
                Resident = new ResidentMessage
                {
                    Id = _currentResident.Id,
                    Name = _currentResident.Id == PublicProfileSource.Id 
                            ? PublicProfileSource.Name
                            : $"{_currentResident.FirstName} {_currentResident.LastName}".Trim(),
                    GameDifficultyLevel = _currentResident.GameDifficultyLevel,
                    AllowVideoCapturing = _currentResident.AllowVideoCapturing
                },
                IsActiveEventLog = false,
                RandomResponseTypes = _randomResponseTypes.ToArray()
            };

            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(responseMessage);
        }

        #endregion
    }
}
