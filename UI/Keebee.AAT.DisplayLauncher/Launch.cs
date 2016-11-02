using System;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Forms;

namespace Keebee.AAT.DisplayLauncher
{
    public partial class Launch : Form
    {
        private delegate void UpdateMessageDelegate();

        private int _secondsRemaining;
        private readonly string _exePath;

        public Launch()
        {
            InitializeComponent();

            _exePath = ConfigurationManager.AppSettings["ExecutablePath"];
            var waitTime = Convert.ToInt32(ConfigurationManager.AppSettings["WaitTime"]);
            _secondsRemaining = waitTime;

            var waitTimer = new Timer{ Interval = 1000 };
            waitTimer.Tick += TimerTick;
            waitTimer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (_secondsRemaining <= 0)
            {
                Process.Start(_exePath);
                Close();
            }
            else
            {
                _secondsRemaining--;
                UpdateMessage();
            }
        }

        private void UpdateMessage()
        {
            if (InvokeRequired)
            {
                Invoke(new UpdateMessageDelegate(UpdateMessage));
            }
            else
            {
                lblMessage.Text = $"AAT System will launch in {_secondsRemaining} seconds...";
            }
        }
    }
}
