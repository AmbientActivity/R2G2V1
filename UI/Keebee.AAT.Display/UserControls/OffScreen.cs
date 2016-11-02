using System;
using System.Windows.Forms;

namespace Keebee.AAT.Display.UserControls
{
    public partial class OffScreen : UserControl
    {
        // event handler
        public event EventHandler OffScreenCompleteEvent;

        // delegate
        private delegate void RaiseOffScreenCompleteEventDelegate();

        private readonly Timer _timer;
        private const int WaitInterval = 60000; // 1 minute

        public OffScreen()
        {
            InitializeComponent();

            _timer = new Timer { Interval = WaitInterval };
            _timer.Tick += TimerTick;
        }

        public void Play()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            RaiseOffScreenCompleteEvent();
        }

        private void RaiseOffScreenCompleteEvent()
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                Invoke(new RaiseOffScreenCompleteEventDelegate(RaiseOffScreenCompleteEvent));
            }
            else
            {
                OffScreenCompleteEvent?.Invoke(new object(), new EventArgs());
            }
        }
    }
}
