using Keebee.AAT.RESTClient;
using Keebee.AAT.MessageQueuing;
using Keebee.AAT.Shared;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Windows.Forms;
using System.Configuration;
using System.Diagnostics;
using System.Web.Script.Serialization;
using Timer = System.Threading.Timer;

namespace Keebee.AAT.Simulator
{

    enum StepDirectionType
    {
        Left = 0,
        Right = 1
    }

    public partial class ControlPanel : Form
    {
        // data
        private readonly IOperationsClient _opsClient;
        private Resident[] _residents;

        // message queue sender
        private readonly CustomMessageQueue _messageQueuePhidget;
        private readonly CustomMessageQueue _messageQueueRfid;
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

        // step action
        private const int MaxValue = 1000;
        private const int StepIncrement = 200;

        private int _currentRadio4Value;
        private int _currentTelevsion5Value;

        public ControlPanel()
        {
            InitializeComponent();

            _autoResponseInterval = Convert.ToInt32(ConfigurationManager.AppSettings["AutoSensorInterval"]);
            _autoResidentInterval = Convert.ToInt32(ConfigurationManager.AppSettings["AutoResidentInterval"]);

            _opsClient = new OperationsClient ();

            // message queue senders
            _messageQueuePhidget = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.Phidget

            });

            _messageQueueRfid = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.Rfid

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

        private void LoadResidentDropDown()
        {

            var arrayList = new ArrayList();
            
            _residents = new List<Resident>
            {
                new Resident
                {
                    Id = PublicMediaSource.Id,
                    FirstName = PublicMediaSource.Description,
                    GameDifficultyLevel = 1,
                    AllowVideoCapturing = false
                }
            }
            .Union(_opsClient.GetResidents()
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
            _messageQueueRfid.Send(CreateMessageBodyForRfid(new Resident { Id = PublicMediaSource.Id, GameDifficultyLevel =  1}));
        }

        private void KillDisplayButtonClick(object sender, EventArgs e)
        {
            ExecuteResponse(ResponseTypeId.KillDisplay, PhidgetTypeId.Input1, MaxValue - 1, true);
        }

        private void SlideShowButtonClick(object sender, EventArgs e)
        {
            ExecuteResponse(ResponseTypeId.SlideShow, PhidgetTypeId.Sensor0);
        }

        private void MatchingGameButtonClick(object sender, EventArgs e)
        {
            ExecuteResponse(ResponseTypeId.MatchingGame, PhidgetTypeId.Sensor1);
        }

        private void CatsButtonClick(object sender, EventArgs e)
        {
            ExecuteResponse(ResponseTypeId.Cats, PhidgetTypeId.Sensor3);
        }

        private void OffScreenButtonClick(object sender, EventArgs e)
        {
            ExecuteResponse(ResponseTypeId.OffScreen, PhidgetTypeId.Sensor4, MaxValue - 1, false, new [] {ResponseTypeId.MatchingGame, ResponseTypeId.SlideShow});
        }

        private void RadioRightButtonClick(object sender, EventArgs e)
        {
            _currentRadio4Value = GetCurrentStepValue(ResponseTypeId.Radio, StepDirectionType.Right);

            var valueToSend = (_currentRadio4Value == MaxValue)
                ? _currentRadio4Value - 1 : _currentRadio4Value;

            if (valueToSend <= 0) return;

            _messageQueuePhidgetContinuousRadio.Send($"{valueToSend}");
            ExecuteResponse(ResponseTypeId.Radio, PhidgetTypeId.Sensor5, valueToSend);
        }

        private void RadioLeftButtonClick(object sender, EventArgs e)
        {
            _currentRadio4Value = GetCurrentStepValue(ResponseTypeId.Radio, StepDirectionType.Left);

            var valueToSend = (_currentRadio4Value == MaxValue)
                ? _currentRadio4Value - 1 : _currentRadio4Value;

            if (valueToSend <= 0) return;

            _messageQueuePhidgetContinuousRadio.Send($"{valueToSend}");
            ExecuteResponse(ResponseTypeId.Radio, PhidgetTypeId.Sensor5, valueToSend);
        }

        private void TelevisionRightButtonClick(object sender, EventArgs e)
        {
            _currentTelevsion5Value = GetCurrentStepValue(ResponseTypeId.Television, StepDirectionType.Right);

            var valueToSend = (_currentTelevsion5Value == MaxValue)
                ? _currentTelevsion5Value - 1 : _currentTelevsion5Value;

            if (valueToSend <= 0) return;

            ExecuteResponse(ResponseTypeId.Television, PhidgetTypeId.Sensor6, valueToSend);
        }

        private void TelevisionLeftButtonClick(object sender, EventArgs e)
        {
            _currentTelevsion5Value = GetCurrentStepValue(ResponseTypeId.Television, StepDirectionType.Left);

            var valueToSend = (_currentTelevsion5Value == MaxValue)
                ? _currentTelevsion5Value - 1 : _currentTelevsion5Value;

            if (valueToSend <= 0) return;

            ExecuteResponse(ResponseTypeId.Television, PhidgetTypeId.Sensor6, valueToSend);
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
            ExecuteResponse(ResponseTypeId.Caregiver, PhidgetTypeId.Input0, MaxValue - 1, true);
        }

        private void AmbientButtonClick(object sender, EventArgs e)
        {
            ExecuteResponse(ResponseTypeId.Ambient, PhidgetTypeId.Sensor7, MaxValue - 1, true);
        }

        private void ActivateResidentClick(object sender, EventArgs e)
        {
            var id = Convert.ToInt32(cboResident.SelectedValue.ToString());

            var resident = (id == PublicMediaSource.Id)
                ? new Resident { Id = PublicMediaSource.Id, GameDifficultyLevel = 1, AllowVideoCapturing = false }
                : _residents.Single(x => x.Id == id);

            CreateMessageBodyForRfid(resident);
            _messageQueueRfid.Send(CreateMessageBodyForRfid(resident));
        }

        private static string CreateMessageBodyForRfid(Resident resident)
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

            var residentId = _residents[_currentResidentIndex].Id;

            _messageQueueRfid.Send(Convert.ToString(residentId));
        }

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

        private void ExecuteResponse(int responseTypeId, int phidgetTypeId, int sensorValue = MaxValue - 1, bool isSystem = false, int[] reponseTypeIds = null)
        {
            if (Process.GetProcessesByName(ApplicationName.DisplayApp).Any())
                _messageQueueResponse.Send(CreateMessageBodyForResponse(responseTypeId, phidgetTypeId, isSystem, sensorValue, reponseTypeIds));
        }

        private string CreateMessageBodyForResponse(int responseTypeId, int phidgetTypeId, bool isSystem, int sensorValue, int[] reponseTypeIds)
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
                    ConfigId = ConfigId.Default
                },
                Resident = new ResidentMessage
                {
                    Id = _currentResident.Id,
                    Name = $"{_currentResident.FirstName} {_currentResident.LastName}".Trim(),
                    GameDifficultyLevel = _currentResident.GameDifficultyLevel,
                    AllowVideoCapturing = _currentResident.AllowVideoCapturing
                },
                IsActiveEventLog = false,
                ResponseTypeIds = reponseTypeIds
            };

            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(responseMessage);
        }
    }
}
