using Keebee.AAT.RESTClient;
using Keebee.AAT.MessageQueuing;
using Keebee.AAT.Shared;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Windows.Forms;
using System.Configuration;
using Timer = System.Threading.Timer;

namespace Keebee.AAT.Simulator
{

    enum SensorDirectionType
    {
        Left = 0,
        Right = 1
    }

    public partial class ControlPanel : Form
    {
        // delegate
        delegate void UpdateLabelDelegate(string text);

        // data
        private readonly IOperationsClient _opsClient;
        private Resident[] _residents;

        // message queue sender
        private readonly CustomMessageQueue _messageQueuePhidget;
        private readonly CustomMessageQueue _messageQueueRfid;
        private readonly CustomMessageQueue _messageQueueConfig;

        // timer
        private readonly int _autoSensorInterval;
        private Timer _timerSensor;
        private readonly int _autoProfileInterval;
        private Timer _timerProfile;

        // activity
        private int _currentSensorId = -1;
        private const int MaxSensorId = 3;

        // profile
        private int _currentProfileIndex = -1;
        private readonly int _totalProfiles;

        // turn dial sensors
        private const int MaxSensorValue = 1000;
        private const int SensorStepIncrement = 200;

        private int _currentRadio4Value;
        private int _currentTelevsion5Value;

        public ControlPanel()
        {
            InitializeComponent();

            _autoSensorInterval = Convert.ToInt32(ConfigurationManager.AppSettings["AutoSensorInterval"]);
            _autoProfileInterval = Convert.ToInt32(ConfigurationManager.AppSettings["AutoProfileInterval"]);

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

            _messageQueueConfig = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.Config

            });

            LoadProfileDropDown();
            _totalProfiles = _residents.Count();
        }

        private void LoadProfileDropDown()
        {

            var arrayList = new ArrayList();
            _residents = _opsClient.GetResidents().ToArray();

            var profiles = new List<Resident> { new Resident {Id = 0, FirstName = "Generic" }}
                .Union(_residents.Select(r => new Resident
                                                {
                                                    Id = r.Id,
                                                    FirstName = r.FirstName,
                                                    LastName = r.LastName,
                                                })).ToList();

            foreach (var p in profiles)
            {
                arrayList.Add(new {p.Id, Name = p.LastName == null 
                    ? $"{p.FirstName}" 
                    : $"{p.FirstName} {p.LastName}" });
            }

            cboProfile.ValueMember = "Id";
            cboProfile.DisplayMember = "Name";
            cboProfile.DataSource = arrayList;

            // sending resident "0" will make Generic the active profile
            _messageQueueRfid.Send("0");
        }

        private void UpdatProfileLabel(string text)
        {
            if (InvokeRequired)
            {
                UpdateLabelDelegate d = UpdatProfileLabel;
                Invoke(d, new object[] { text });
            }
            else
            {
                lblProfile.Text = text;
            }
        }

        private void UpdateSensorLabel(string text)
        {
            if (InvokeRequired)
            {
                UpdateLabelDelegate d = UpdateSensorLabel;
                Invoke(d, new object[] { text });
            }
            else
            {
                lblSensor.Text = text;
            }
        }

        private void UpdateValueLabel(string text)
        {
            if (InvokeRequired)
            {
                UpdateLabelDelegate d = UpdateValueLabel;
                Invoke(d, new object[] { text });
            }
            else
            {
                lblValue.Text = text;
            }
        }

        private void ReloadConfigClick(object sender, EventArgs e)
        {
            _messageQueueConfig.Send("1");
        }

        private void SlideShowSensorClick(object sender, EventArgs e)
        {
            UpdateSensorLabel("0");
            UpdateValueLabel(Convert.ToString(MaxSensorValue - 1));
            _messageQueuePhidget.Send(string.Format("{0}\"SensorId\":0,\"SensorValue\":{1}{2}", "{", MaxSensorValue - 1, "}"));
        }

        private void MatchinggameSensorClick(object sender, EventArgs e)
        {
            UpdateSensorLabel("1");
            UpdateValueLabel(Convert.ToString(MaxSensorValue - 1));
            _messageQueuePhidget.Send(string.Format("{0}\"SensorId\":1,\"SensorValue\":{1}{2}", "{", MaxSensorValue - 1, "}"));
        }

        private void CatsSensorClick(object sender, EventArgs e)
        {
            UpdateSensorLabel("1");
            UpdateValueLabel(Convert.ToString(MaxSensorValue - 1));
            _messageQueuePhidget.Send(string.Format("{0}\"SensorId\":2,\"SensorValue\":{1}{2}", "{", MaxSensorValue - 1, "}"));
        }

        private void KillDisplaySensorClick(object sender, EventArgs e)
        {
            UpdateSensorLabel("3");
            UpdateValueLabel(Convert.ToString(MaxSensorValue - 1));
            _messageQueuePhidget.Send(string.Format("{0}\"SensorId\":3,\"SensorValue\":{1}{2}", "{", MaxSensorValue - 1, "}"));
        }

        private void RadioSensorRightClick(object sender, EventArgs e)
        {
            UpdateSensorLabel("4");

            _currentRadio4Value = GetRotationCurrentValue(ResponseTypeId.Radio, SensorDirectionType.Right);

            var valueToSend = (_currentRadio4Value == MaxSensorValue)
                ? _currentRadio4Value - 1 : _currentRadio4Value;

            UpdateValueLabel(Convert.ToString(valueToSend));
            if (valueToSend <= 0) return;

            _messageQueuePhidget.Send(string.Format("{0}\"SensorId\":4,\"SensorValue\":{1}{2}", "{", valueToSend, "}"));
        }

        private void RadioSensorLeftClick(object sender, EventArgs e)
        {
            UpdateSensorLabel("4");

            _currentRadio4Value = GetRotationCurrentValue(ResponseTypeId.Radio, SensorDirectionType.Left);

            var valueToSend = (_currentRadio4Value == MaxSensorValue)
                ? _currentRadio4Value - 1 : _currentRadio4Value;

            UpdateValueLabel(Convert.ToString(valueToSend));
            if (valueToSend <= 0) return;

            _messageQueuePhidget.Send(string.Format("{0}\"SensorId\":4,\"SensorValue\":{1}{2}", "{", valueToSend, "}"));
        }

        private void TelevisionSensorRightClick(object sender, EventArgs e)
        {
            UpdateSensorLabel("5");

            _currentTelevsion5Value = GetRotationCurrentValue(ResponseTypeId.Television, SensorDirectionType.Right);

            var valueToSend = (_currentTelevsion5Value == MaxSensorValue)
                ? _currentTelevsion5Value - 1 : _currentTelevsion5Value;

            UpdateValueLabel(Convert.ToString(valueToSend));
            if (valueToSend <= 0) return;

            _messageQueuePhidget.Send(string.Format("{0}\"SensorId\":5,\"SensorValue\":{1}{2}", "{", valueToSend, "}"));
        }

        private void TelevisionSensorLeftClick(object sender, EventArgs e)
        {
            UpdateSensorLabel("5");

            _currentTelevsion5Value = GetRotationCurrentValue(ResponseTypeId.Television, SensorDirectionType.Left);

            var valueToSend = (_currentTelevsion5Value == MaxSensorValue)
                ? _currentTelevsion5Value - 1 : _currentTelevsion5Value;

            UpdateValueLabel(Convert.ToString(valueToSend));
            if (valueToSend <= 0) return;

            _messageQueuePhidget.Send(string.Format("{0}\"SensorId\":5,\"SensorValue\":{1}{2}", "{", valueToSend, "}"));
        }

        private int GetRotationCurrentValue(int responseType, SensorDirectionType direction)
        {
            int currentValue = -1;

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
                case SensorDirectionType.Left:
                    if ((currentValue - SensorStepIncrement) > 0)
                        currentValue -= SensorStepIncrement;
                    else
                        currentValue = MaxSensorValue;
                    break;

                case SensorDirectionType.Right:
                    if ((currentValue + SensorStepIncrement) <= MaxSensorValue)
                        currentValue += SensorStepIncrement;
                    else
                        currentValue = SensorStepIncrement;
                    break;
            }

            return currentValue;
        }

        private void CaregiverSensorClick(object sender, EventArgs e)
        {
            UpdateSensorLabel("6");
            _messageQueuePhidget.Send(string.Format("{0}\"SensorId\":6,\"SensorValue\":{1}{2}", "{", MaxSensorValue - 1, "}"));
        }

        private void AmbientSensorClick(object sender, EventArgs e)
        {
            UpdateSensorLabel("7");
            _messageQueuePhidget.Send(string.Format("{0}\"SensorId\":7,\"SensorValue\":{1}{2}", "{", MaxSensorValue - 1, "}"));
        }

        private void SlideShowInputClick(object sender, EventArgs e)
        {
            UpdateSensorLabel("0");
            UpdateValueLabel(Convert.ToString(MaxSensorValue - 1));
            _messageQueuePhidget.Send(string.Format("{0}\"SensorId\":8,\"SensorValue\":{1}{2}", "{", MaxSensorValue - 1, "}"));
        }

        private void MatchingGameInputClick(object sender, EventArgs e)
        {
            UpdateSensorLabel("1");
            UpdateValueLabel(Convert.ToString(MaxSensorValue - 1));
            _messageQueuePhidget.Send(string.Format("{0}\"SensorId\":9,\"SensorValue\":{1}{2}", "{", MaxSensorValue - 1, "}"));
        }

        private void RadioInputClick(object sender, EventArgs e)
        {
            UpdateSensorLabel("0");
            UpdateValueLabel(Convert.ToString(MaxSensorValue - 1));
            _messageQueuePhidget.Send(string.Format("{0}\"SensorId\":12,\"SensorValue\":{1}{2}", "{", MaxSensorValue - 1, "}"));
        }

        private void TelevisionInputClick(object sender, EventArgs e)
        {
            UpdateSensorLabel("1");
            UpdateValueLabel(Convert.ToString(MaxSensorValue - 1));
            _messageQueuePhidget.Send(string.Format("{0}\"SensorId\":13,\"SensorValue\":{1}{2}", "{", MaxSensorValue - 1, "}"));
        }
        
        private void ActivateProfileClick(object sender, EventArgs e)
        {
            _messageQueueRfid.Send(cboProfile.SelectedValue.ToString());
            lblProfile.Text = cboProfile.Text;
        }

        private void TimerSensorTick(object sender)
        {
            if (_timerSensor == null) return;

            if (_currentSensorId < MaxSensorId)
                _currentSensorId++;
            else
                _currentSensorId = 0;

            UpdateSensorLabel(Convert.ToString(_currentSensorId));
            UpdateValueLabel(Convert.ToString(MaxSensorValue - 1));
            _messageQueuePhidget.Send(string.Format("{0}\"SensorId\":{1},\"SensorValue\":{2}{3}", "{", _currentSensorId, MaxSensorValue - 1, "}"));
        }

        private void TimerProfileTick(object sender)
        {
            if (_timerProfile == null) return;

            if (_currentProfileIndex < (_totalProfiles - 1))
                _currentProfileIndex++;
            else
                _currentProfileIndex = 0;

            var residentId = _residents[_currentProfileIndex].Id;
            var firstName = _residents[_currentProfileIndex].FirstName;
            UpdatProfileLabel(firstName);

            _messageQueueRfid.Send(Convert.ToString(residentId));
        }

        private void ControlPanelClosing(object sender, FormClosingEventArgs e)
        {
            if (_timerSensor != null)
                _timerSensor.Dispose();

            if (_timerProfile != null)
                _timerProfile.Dispose();
        }

        private void AutoSensorCheckChanged(object sender, EventArgs e)
        {
            // Search Auto Activity GroupBox
            foreach (var result in from control in grpAutoSensor.Controls.OfType<RadioButton>() 
                                   select control into radio where radio.Checked select radio.Text)
            {
                if (result == "On")
                {
                    if (_timerSensor == null)
                    {
                        _timerSensor = new Timer(TimerSensorTick, null, 0, _autoSensorInterval);
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

        private void AutoProfileCheckChanged(object sender, EventArgs e)
        {
            foreach (var result in from control in grpAutoProfile.Controls.OfType<RadioButton>() 
                                   select control into radio where radio.Checked select radio.Text)
            {
                if (result == "On")
                {
                    if (_timerProfile == null)
                    {
                        _timerProfile = new Timer(TimerProfileTick, null, 0, _autoProfileInterval);
                    }
                }
                else
                {
                    if (_timerProfile == null) continue;

                    _timerProfile.Dispose();
                    _timerProfile = null;
                }
            }
        }
    }
}
