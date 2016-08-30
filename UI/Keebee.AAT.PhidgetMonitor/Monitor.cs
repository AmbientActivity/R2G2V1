using System;
using System.Windows.Forms;
using Keebee.AAT.MessageQueuing;
using System.Web.Script.Serialization;

namespace Keebee.AAT.PhidgetMonitor
{
    public partial class Monitor : Form
    {
        delegate void UpdateLabelDelegate(int sensorId, string text);

#if DEBUG
        private readonly CustomMessageQueue _messageQueuePhidgetMonitorState;
#endif

        public Monitor()
        {
            InitializeComponent();

#if DEBUG
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
#endif
        }

#if DEBUG
        private void MessageReceived(object sender, MessageEventArgs e)
        {
            var message = (e.MessageBody);
            var serializer = new JavaScriptSerializer();
            var phidgetMessage = serializer.Deserialize<PhidgetMessage>(message);

            UpdateSensorValueLabel(phidgetMessage.SensorId, Convert.ToString(phidgetMessage.SensorValue));
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
#endif

        private void MonitorShown(object sender, EventArgs e)
        {
#if DEBUG
            _messageQueuePhidgetMonitorState.Send("1");
#endif
        }

        private void MonitorFormClosing(object sender, FormClosingEventArgs e)
        {
#if DEBUG
            _messageQueuePhidgetMonitorState.Send("0");
#endif
        }
    }
}
