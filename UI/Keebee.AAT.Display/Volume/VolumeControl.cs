using Keebee.AAT.Display.Extensions;
using Keebee.AAT.Display.Helpers;
using System;
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
