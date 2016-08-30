using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Keebee.AAT.Display.Caregiver.CustomControls
{
    public partial class RepeatButton : Button
    {
        private Timer timerRepeater;      //timer to measure repeat intervals wait.
        //private IContainer _components;    //Components collection of this control (timer)
        //private bool _disposed = false;    //flag used to prevent multiple disposing in Dispose method
        private MouseEventArgs _mouseDownArgs;  //muse down arguments; used by timer when repeating events.

        [DefaultValue(400)]
        [Category("Enhanced")]
        [Description("Initial delay. Time in milliseconds between button press and first repeat action.")]
        public int InitialDelay { set; get; }

        [DefaultValue(62)]
        [Category("Enhanced")]
        [Description("Repeat Interval. Repeat between each repeat action while button is hold pressed.")]
        public int RepeatInterval { set; get; }

        public RepeatButton()
        {
            InitializeComponent();
            InitialDelay = 400;
            RepeatInterval = 62;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            //Save arguments
            _mouseDownArgs = e;
            timerRepeater.Enabled = false;
            RepeaterTick(null, EventArgs.Empty);
        }

        private void RepeaterTick(object sender, EventArgs e)
        {

            base.OnMouseDown(_mouseDownArgs);
            timerRepeater.Interval = timerRepeater.Enabled 
                ? RepeatInterval 
                : InitialDelay;

            timerRepeater.Enabled = true;
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            timerRepeater.Enabled = false;
        }
    }
}
