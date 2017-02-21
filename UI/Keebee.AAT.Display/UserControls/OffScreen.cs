using System;
using System.Drawing;
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
#if DEBUG
            lblOff.Font = new Font("Microsoft Sans Serif", 36);
#elif !DEBUG
            lblOff.Font = new Font("Microsoft Sans Serif", 120);
#endif
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
