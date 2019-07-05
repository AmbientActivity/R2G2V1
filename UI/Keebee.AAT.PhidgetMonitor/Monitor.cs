using Keebee.AAT.MessageQueuing;
using Newtonsoft.Json;
using System;
using System.Windows.Forms;

namespace Keebee.AAT.PhidgetMonitor
{
    public partial class Monitor : Form
    {
        private delegate void UpdateLabelDelegate(int sensorId, string text);

        private readonly CustomMessageQueue _messageQueuePhidgetMonitorState;

        public Monitor()
        {
            InitializeComponent();

            _messageQueuePhidgetMonitorState = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.PhidgetMonitorState
            });

            // listener
            var q = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.PhidgetMonitor,
                MessageReceivedCallback = MessageReceived
            });
        }

        private void MessageReceived(object sender, MessageEventArgs e)
        {
            var phidget = JsonConvert.DeserializeObject<Tuple<int, int>>(e.MessageBody);
            if (phidget == null) return;

            // sensorId's are base 0 - convert to base 1 for PhidgetTypeId
            var sensorId = phidget.Item1;
            var sensorValue = phidget.Item2;

            if (sensorId < 8)
                UpdateSensorValueLabel(sensorId, Convert.ToString(sensorValue));
            else
                UpdateInputValueLabel(sensorId, sensorValue == 1 ? "On" : "Off");
        }

        private void UpdateSensorValueLabel(int sensorId, string text)
        {
            if (InvokeRequired)
            {
                UpdateLabelDelegate d = UpdateSensorValueLabel;
                Invoke(d, new object[] { sensorId, text });
            }
            else
            {
                switch (sensorId)
                {
                    case 0:
                        lblSensor0Value.Text = text;
                        break;
                    case 1:
                        lblSensor1Value.Text = text;
                        break;
                    case 2:
                        lblSensor2Value.Text = text;
                        break;
                    case 3:
                        lblSensor3Value.Text = text;
                        break;
                    case 4:
                        lblSensor4Value.Text = text;
                        break;
                    case 5:
                        lblSensor5Value.Text = text;
                        break;
                    case 6:
                        lblSensor6Value.Text = text;
                        break;
                    case 7:
                        lblSensor7Value.Text = text;
                        break;
                }
            }
        }

        private void UpdateInputValueLabel(int inputId, string text)
        {
            if (InvokeRequired)
            {
                UpdateLabelDelegate d = UpdateInputValueLabel;
                Invoke(d, new object[] { inputId, text });
            }
            else
            {
                switch (inputId)
                {
                    case 8:
                        lblInput0Value.Text = text;
                        break;
                    case 9:
                        lblInput1Value.Text = text;
                        break;
                    case 10:
                        lblInput2Value.Text = text;
                        break;
                    case 11:
                        lblInput3Value.Text = text;
                        break;
                    case 12:
                        lblInput4Value.Text = text;
                        break;
                    case 13:
                        lblInput5Value.Text = text;
                        break;
                    case 14:
                        lblInput6Value.Text = text;
                        break;
                    case 15:
                        lblInput7Value.Text = text;
                        break;
                }
            }
        }

        private void MonitorShown(object sender, EventArgs e)
        {
            _messageQueuePhidgetMonitorState.Send("1");
        }

        private void MonitorFormClosing(object sender, FormClosingEventArgs e)
        {
            _messageQueuePhidgetMonitorState.Send("0");
        }
    }
}
