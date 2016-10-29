using System.Drawing;
using System.Windows.Forms;

namespace Keebee.AAT.Display.Volume
{
    public partial class OpaqueLayer : Form
    {
        public OpaqueLayer()
        {
            InitializeComponent();
            InitializeStartupPosition();
        }

        private void InitializeStartupPosition()
        {
            ShowInTaskbar = false;

#if DEBUG
            StartPosition = FormStartPosition.Manual;
            Location = new Point(0, 0);

            // set form size to 1/3 primary monitor size
            Width = SystemInformation.PrimaryMonitorSize.Width / 3;
            Height = SystemInformation.PrimaryMonitorSize.Height / 3;

#elif !DEBUG
            WindowState = FormWindowState.Maximized;
#endif
        }
    }
}
