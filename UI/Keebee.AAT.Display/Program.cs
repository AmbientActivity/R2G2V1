using Keebee.AAT.Display.Helpers;
using System;
using System.Windows.Forms;

namespace Keebee.AAT.Display
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
#if !DEBUG
            Taskbar.Hide();
            //Cursor.Hide();
#endif
            Application.ApplicationExit += ApplicationExit;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new Splash());
        }

        static void ApplicationExit(object sender, EventArgs e)
        {
            Taskbar.Show();
            Cursor.Show();
        }
    }
}
