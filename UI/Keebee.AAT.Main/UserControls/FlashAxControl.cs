using System;
using System.Windows.Forms;
using AxShockwaveFlashObjects;

namespace Keebee.AAT.Main.UserControls
{
    public partial class FlashAxControl : UserControl
    {
        // event handler
        public event EventHandler FlashCallEvent;
        public class FlashCallEventEventArgs : EventArgs
        {
            public string Request { get; set; }
        }

        public FlashAxControl()
        {
            InitializeComponent();
        }

        public new int Width
        {
            get { return axShockwaveFlash.Width; }
            set { axShockwaveFlash.Width = value; }
        }
        public new int Height
        {
            get { return axShockwaveFlash.Height; }
            set { axShockwaveFlash.Height = value; }
        }

        public void LoadMovie(string strPath)
        {
            axShockwaveFlash.LoadMovie(0, strPath);
        }

        public void Play()
        {
            axShockwaveFlash.Play();      
        }

        public void Stop()
        {
            axShockwaveFlash.Stop();    
        }

        public void CallFunction(string name, string args = null)
        {
            var arguments = string.Empty;

            if (args != null)
            {
                arguments = $"<arguments>{args}</arguments>";
            }
            axShockwaveFlash.CallFunction($"<invoke name=\"{name}\">{arguments}</invoke>");
        }

        public void FlashCall(object sender, _IShockwaveFlashEvents_FlashCallEvent e)
        {
            var args = new FlashCallEventEventArgs
            {
                Request = e.request
            };

            FlashCallEvent?.Invoke(new object(), args);
        }
    }
}
