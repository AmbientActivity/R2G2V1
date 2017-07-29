using Keebee.AAT.MessageQueuing;
using Keebee.AAT.Shared;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Windows.Forms;

namespace Keebee.AAT.Simulator
{
    internal enum StepDirectionType
    {
        Left = 0,
        Right = 1
    }

    public partial class ControlPanel : Form
    {
        internal class RotationalResponse
        {
            public int Id { get; set; }
            public int SensorValue { get; set; }
        }

        // for the random response types when the on/off button is clicked
        private readonly ResponseTypeMessage[]  _randomResponseTypes;

        #region declaration

        // data
        private readonly IResidentsClient _residentsClient;
        private Resident[] _residents;

        // message queue sender
        private readonly CustomMessageQueue _messageQueueBluetoothBeaconWatcher;
        private readonly CustomMessageQueue _messageQueueResponse;
        private readonly CustomMessageQueue _messageQueuePhidgetContinuousRadio;

        // responses
        private readonly ResponseTypeMessage[] _responseTypes;
        private int _currentResponseTypeId = ResponseTypeId.Ambient;

        // resident
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

        // current sensor values (for 'rotational' response types)
        private readonly RotationalResponse[] _rotationalResponses;

        #endregion

        public ControlPanel()
        {
            InitializeComponent();

            _residentsClient = new ResidentsClient();

            // message queue senders
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

            IResponseTypesClient responseTypesClient = new ResponseTypesClient();
            _responseTypes = responseTypesClient.Get()
                .Select(r => new ResponseTypeMessage
                {
                    Id = r.Id,
                    ResponseTypeCategoryId = r.ResponseTypeCategory.Id,
                    IsSystem = r.IsSystem,
                    IsRandom = r.IsRandom,
                    IsRotational = r.IsRotational,
                    IsUninterrupted = r.IsUninterrupted,
                    InteractiveActivityTypeId = r.InteractiveActivityType?.Id ?? 0,
                    SwfFile = r.InteractiveActivityType?.SwfFile ?? string.Empty
                }).ToArray();

            _randomResponseTypes = _responseTypes.Where(r => r.IsRandom).ToArray();

            // initialize rotational response sensor values
            _rotationalResponses = responseTypesClient.GeRotationalTypes()
                .Select(r => new RotationalResponse { Id = r.Id, SensorValue = 0 })
                .ToArray();
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

        // image
        private void SlideShowButtonClick(object sender, EventArgs e)
        {
            ExecuteResponse(ResponseTypeId.SlideShow, PhidgetTypeId.Sensor0);
        }

        // rotational
        private void RadioRightButtonClick(object sender, EventArgs e)
        {
            var currentValue = GetCurrentStepValue(ResponseTypeId.Radio, StepDirectionType.Right);

            var valueToSend = (currentValue == MaxValue)
                ? currentValue - 1 : currentValue;

            if (valueToSend <= 0) return;

            _messageQueuePhidgetContinuousRadio.Send($"{valueToSend}");
            ExecuteResponse(ResponseTypeId.Radio, PhidgetTypeId.Sensor0, valueToSend);
        }

        private void RadioLeftButtonClick(object sender, EventArgs e)
        {
            var currentValue = GetCurrentStepValue(ResponseTypeId.Radio, StepDirectionType.Left);

            var valueToSend = (currentValue == MaxValue)
                ? currentValue - 1 : currentValue;

            if (valueToSend <= 0) return;

            _messageQueuePhidgetContinuousRadio.Send($"{valueToSend}");
            ExecuteResponse(ResponseTypeId.Radio, PhidgetTypeId.Sensor0, valueToSend);
        }

        private void TelevisionRightButtonClick(object sender, EventArgs e)
        {
            var currentValue = GetCurrentStepValue(ResponseTypeId.Television, StepDirectionType.Right);

            var valueToSend = (currentValue == MaxValue)
                ? currentValue - 1 : currentValue;

            if (valueToSend <= 0) return;

            ExecuteResponse(ResponseTypeId.Television, PhidgetTypeId.Sensor0, valueToSend);
        }

        private void TelevisionLeftButtonClick(object sender, EventArgs e)
        {
            var currentValue = GetCurrentStepValue(ResponseTypeId.Television, StepDirectionType.Left);

            var valueToSend = (currentValue == MaxValue)
                ? currentValue - 1 : currentValue;

            if (valueToSend <= 0) return;

            ExecuteResponse(ResponseTypeId.Television, PhidgetTypeId.Sensor0, valueToSend);
        }

        private void NatureRightButtonClick(object sender, EventArgs e)
        {
            var currentValue = GetCurrentStepValue(ResponseTypeId.Nature, StepDirectionType.Right);

            var valueToSend = (currentValue == MaxValue)
                ? currentValue - 1 : currentValue;

            if (valueToSend <= 0) return;

            ExecuteResponse(ResponseTypeId.Nature, PhidgetTypeId.Sensor0, valueToSend);
        }

        private void NatureLeftButtonClick(object sender, EventArgs e)
        {
            var currentValue = GetCurrentStepValue(ResponseTypeId.Nature, StepDirectionType.Left);

            var valueToSend = (currentValue == MaxValue)
                ? currentValue - 1 : currentValue;

            if (valueToSend <= 0) return;

            ExecuteResponse(ResponseTypeId.Nature, PhidgetTypeId.Sensor0, valueToSend);
        }

        private void SportsRightButtonClick(object sender, EventArgs e)
        {
            var currentValue = GetCurrentStepValue(ResponseTypeId.Sports, StepDirectionType.Right);

            var valueToSend = (currentValue == MaxValue)
                ? currentValue - 1 : currentValue;

            if (valueToSend <= 0) return;

            ExecuteResponse(ResponseTypeId.Sports, PhidgetTypeId.Sensor0, valueToSend);
        }

        private void SportsLeftButtonClick(object sender, EventArgs e)
        {
            var currentValue = GetCurrentStepValue(ResponseTypeId.Sports, StepDirectionType.Left);

            var valueToSend = (currentValue == MaxValue)
                ? currentValue - 1 : currentValue;

            if (valueToSend <= 0) return;

            ExecuteResponse(ResponseTypeId.Sports, PhidgetTypeId.Sensor0, valueToSend);
        }

        private void MachineryRightButtonClick(object sender, EventArgs e)
        {
            var currentValue = GetCurrentStepValue(ResponseTypeId.Machinery, StepDirectionType.Right);

            var valueToSend = (currentValue == MaxValue)
                ? currentValue - 1 : currentValue;

            if (valueToSend <= 0) return;

            ExecuteResponse(ResponseTypeId.Machinery, PhidgetTypeId.Sensor0, valueToSend);
        }

        private void MachineryLeftButtonClick(object sender, EventArgs e)
        {
            var currentValue = GetCurrentStepValue(ResponseTypeId.Machinery, StepDirectionType.Left);

            var valueToSend = (currentValue == MaxValue)
                ? currentValue - 1 : currentValue;

            if (valueToSend <= 0) return;

            ExecuteResponse(ResponseTypeId.Machinery, PhidgetTypeId.Sensor0, valueToSend);
        }

        private void AnimalsRightButtonClick(object sender, EventArgs e)
        {
            var currentValue = GetCurrentStepValue(ResponseTypeId.Animals, StepDirectionType.Right);

            var valueToSend = (currentValue == MaxValue)
                ? currentValue - 1 : currentValue;

            if (valueToSend <= 0) return;

            ExecuteResponse(ResponseTypeId.Animals, PhidgetTypeId.Sensor0, valueToSend);
        }

        private void AnimalsLeftButtonClick(object sender, EventArgs e)
        {
            var currentValue = GetCurrentStepValue(ResponseTypeId.Animals, StepDirectionType.Left);

            var valueToSend = (currentValue == MaxValue)
                ? currentValue - 1 : currentValue;

            if (valueToSend <= 0) return;

            ExecuteResponse(ResponseTypeId.Animals, PhidgetTypeId.Sensor0, valueToSend);
        }

        private void CuteRightButtonClick(object sender, EventArgs e)
        {
            var currentValue = GetCurrentStepValue(ResponseTypeId.Cute, StepDirectionType.Right);

            var valueToSend = (currentValue == MaxValue)
                ? currentValue - 1 : currentValue;

            if (valueToSend <= 0) return;

            ExecuteResponse(ResponseTypeId.Cute, PhidgetTypeId.Sensor0, valueToSend);
        }

        private void CuteLeftButtonClick(object sender, EventArgs e)
        {
            var currentValue = GetCurrentStepValue(ResponseTypeId.Cute, StepDirectionType.Left);

            var valueToSend = (currentValue == MaxValue)
                ? currentValue - 1 : currentValue;

            if (valueToSend <= 0) return;

            ExecuteResponse(ResponseTypeId.Cute, PhidgetTypeId.Sensor0, valueToSend);
        }

        private int GetCurrentStepValue(int responseTypeId, StepDirectionType direction)
        {
            var currentValue = _rotationalResponses.Single(r => r.Id == responseTypeId).SensorValue;

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

        // system
        private void KillDisplayButtonClick(object sender, EventArgs e)
        {
            ExecuteResponse(ResponseTypeId.KillDisplay, PhidgetTypeId.Sensor0, MaxValue - 1, true);
        }

        private void CatsButtonClick(object sender, EventArgs e)
        {
            ExecuteResponse(ResponseTypeId.Cats, PhidgetTypeId.Sensor0);
        }

        private void CaregiverButtonClick(object sender, EventArgs e)
        {
            ExecuteResponse(ResponseTypeId.Caregiver, PhidgetTypeId.Sensor0, MaxValue - 1, true);
        }

        private void AmbientButtonClick(object sender, EventArgs e)
        {
            ExecuteResponse(ResponseTypeId.Ambient, PhidgetTypeId.Sensor0, MaxValue - 1, true);
        }

        private void VolumeControlClick(object sender, EventArgs e)
        {
            ExecuteResponse(ResponseTypeId.VolumeControl, PhidgetTypeId.Sensor0);
        }

        private void OffScreenButtonClick(object sender, EventArgs e)
        {
            ExecuteResponse(ResponseTypeId.OffScreen, PhidgetTypeId.Sensor4, MaxValue - 1, false);
        }

        // interactive activity
        private void MatchingGameButtonClick(object sender, EventArgs e)
        {
            ExecuteResponse(ResponseTypeId.MatchingGame, PhidgetTypeId.Sensor0);
        }
        
        private void PaintingActivityClick(object sender, EventArgs e)
        {
            ExecuteResponse(ResponseTypeId.PaintingActivity, PhidgetTypeId.Sensor0);
        }

        private void BalloonPoppingGameClick(object sender, EventArgs e)
        {
            ExecuteResponse(ResponseTypeId.BalloonPoppingGame, PhidgetTypeId.Sensor0);
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

        #region event handlers

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
            if (!Process.GetProcessesByName($"{AppSettings.Namespace}.{AppSettings.DisplayAppName}").Any())
                return;
#endif
            var responseToExecute = _responseTypes.Single(r => r.Id == responseTypeId);

            // for the OffScreen, need to alternate between the OffScreen and a 'random' response
            if (responseTypeId == ResponseTypeId.OffScreen && _currentResponseTypeId == ResponseTypeId.OffScreen)
                responseToExecute = GetOffScreenResponse();

            _messageQueueResponse.Send(CreateMessageBodyForResponse(responseToExecute, phidgetTypeId, sensorValue));
            _currentResponseTypeId = responseToExecute.Id;
            SaveCurrentRotationalSensorValue(responseTypeId, sensorValue);
#if !DEBUG
#endif
        }

        private void SaveCurrentRotationalSensorValue(int responseTypeId, int sensorValue)
        {
            if (_rotationalResponses.Any(r => r.Id == responseTypeId))
            {
                _rotationalResponses.Single(r => r.Id == responseTypeId)
                    .SensorValue = sensorValue;
            }
        }

        private int _currentSequentialResponseTypeIndex = -1;
        private ResponseTypeMessage GetOffScreenResponse()
        {
            if (_currentSequentialResponseTypeIndex < _randomResponseTypes.Length - 1)
                _currentSequentialResponseTypeIndex++;
            else
                _currentSequentialResponseTypeIndex = 0;

            return _randomResponseTypes[_currentSequentialResponseTypeIndex];
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

            var messageBody = JsonConvert.SerializeObject(residentMessage);

            return messageBody;
        }

        private string CreateMessageBodyForResponse(ResponseTypeMessage responseType, int phidgetTypeId, int sensorValue)
        {


            var responseMessage = new ResponseMessage
            {
                SensorValue = sensorValue,
                ConfigDetail = new ConfigDetailMessage
                {
                    Id = 1,
                    ResponseType = responseType,
                    PhidgetTypeId = phidgetTypeId,
                    ConfigId = ConfigId.Default,   
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
                IsActiveEventLog = false
            };

            return JsonConvert.SerializeObject(responseMessage);
        }

#endregion
    }
}
