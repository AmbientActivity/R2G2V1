using Keebee.AAT.Display.Helpers;
using System;
using System.Threading;
using System.Windows.Forms;

namespace Keebee.AAT.Display
{
    static class Program
    {
        static readonly Mutex Mutex = new Mutex(true, "{8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE8E}");

        [STAThread]
        static void Main()
        {

            if (Mutex.WaitOne(TimeSpan.Zero, true))
            {
                try
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
                finally
                {
                    Mutex.ReleaseMutex();
                }
            }
            else
            {
                MessageBox.Show("Display App is already running.");
            }
        }

        static void ApplicationExit(object sender, EventArgs e)
        {
            Taskbar.Show();
            Cursor.Show();
        }
    }
}
