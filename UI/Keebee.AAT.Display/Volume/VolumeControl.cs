using Keebee.AAT.Display.Extensions;
using Keebee.AAT.Display.Helpers;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Keebee.AAT.Display.Volume
{
    public partial class VolumeControl : Form
    {
        // constants
        private const int VolumeIncrement = 3;

        // event handler
        public event EventHandler VolumeControlClosedEvent;

        // delegate
        private delegate void RaiseVolumeControlClosedEventDelegate();
        private delegate void CloseFormDelegate();

        // timer
        private readonly Timer _timer;
        public VolumeControl()
        {
            InitializeComponent();
            pbCurrentVolume.SetProgressNoAnimation(Convert.ToInt32(AudioManager.GetMasterVolume()));
            _timer = new Timer { Interval = 5000 };
            _timer.Tick += TimerTick;
            _timer.Start();
            InitializeStartupPosition();
        }

        private void InitializeStartupPosition()
        {
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
#if DEBUG
            var w = SystemInformation.PrimaryMonitorSize.Width / 3;
            var h = SystemInformation.PrimaryMonitorSize.Height / 3;
#elif !DEBUG
            var w = SystemInformation.PrimaryMonitorSize.Width;
            var h = SystemInformation.PrimaryMonitorSize.Height;
#endif
            Location = new Point(w / 2 - Width / 2, h / 2 - Height / 2);
        }

        private void ButtonUpClick(object sender, EventArgs e)
        {
            ResetTimer();
            AudioManager.StepMasterVolume(VolumeIncrement);
            pbCurrentVolume.SetProgressNoAnimation(Convert.ToInt32(AudioManager.GetMasterVolume()));
        }

        private void ButtonDownClick(object sender, EventArgs e)
        {
            ResetTimer();
            AudioManager.StepMasterVolume(VolumeIncrement * -1);
            pbCurrentVolume.SetProgressNoAnimation(Convert.ToInt32(AudioManager.GetMasterVolume()));
        }

        private void RaiseVolumeControlClosedEvent()
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                Invoke(new RaiseVolumeControlClosedEventDelegate(RaiseVolumeControlClosedEvent));
            }
            else
            {
                VolumeControlClosedEvent?.Invoke(new object(), new EventArgs());
            }
        }

        private void CloseForm()
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                Invoke(new CloseFormDelegate(CloseForm));
            }
            else
            {
                _timer.Stop();
                RaiseVolumeControlClosedEvent();
                Close();
            }
        }

        private void ButtonCloseClick(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void ResetTimer()
        {
            _timer.Stop();
            _timer.Start();
        }
    }
}
