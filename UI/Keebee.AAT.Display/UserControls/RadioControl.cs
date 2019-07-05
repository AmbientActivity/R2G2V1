using Keebee.AAT.Display.Properties;
using Keebee.AAT.MessageQueuing;
using Keebee.AAT.SystemEventLogging;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Keebee.AAT.Display.UserControls
{
    public partial class RadioControl : UserControl
    {
        public class LogVideoActivityEventEventArgs : EventArgs
        {
            public string Description { get; set; }
        }

        // delegate
        private delegate void UpdateDialDelegate(int value);

        private int _minDial;
        private int _maxDial;
        private int _rangeDial;
        private double _divisorDial;

        public RadioControl()
        {
            InitializeComponent();
            ConfigureComponents();

            var q = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.PhidgetContinuousRadio,
                MessageReceivedCallback = MessageReceivedPhidgetContinuousRadio
            });
        }

        private void ConfigureComponents()
        {
            pbRadioPanel.Image = Resources.radio_panel;
            pbRadioPanel.SizeMode = PictureBoxSizeMode.StretchImage;
            pbRadioPanel.Dock = DockStyle.Fill;
            pbDial.Show();
#if DEBUG
            pbDial.Width = 5;
            pbDial.Height = SystemInformation.PrimaryMonitorSize.Height / 3;

            _maxDial = (SystemInformation.PrimaryMonitorSize.Width / 3) - 30;
            _minDial = 120;
#elif !DEBUG
            pbDial.Width = 8;
            pbDial.Height = SystemInformation.PrimaryMonitorSize.Height;
            _maxDial = (SystemInformation.PrimaryMonitorSize.Width) - 30;
            _minDial = 300;
#endif
            _rangeDial = _maxDial - _minDial;
            _divisorDial = (double)1000 / _rangeDial;
        }

        private void MessageReceivedPhidgetContinuousRadio(object source, MessageEventArgs e)
        {
            try
            {
                int value;
                var isValid = int.TryParse(e.MessageBody, out value);
                if (!isValid) return;

                UpdateDial(value);
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"RadioControl.MessageReceivedPhidgetContinuousRadio: {ex.Message}", SystemEventLogType.Display, EventLogEntryType.Error);
            }
        }

        private void UpdateDial(int value)
        {
            if (InvokeRequired)
            {
                Invoke(new UpdateDialDelegate(UpdateDial), value);
            }
            else
            {
                pbDial.Left = (int)(value / _divisorDial) + _minDial;
            }
        }
    }
}
