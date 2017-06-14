using System;
using System.Threading;
using System.Windows;

namespace Keebee.AAT.Main
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static readonly Mutex Mutex = new Mutex(true, "{8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE8E}");

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (Mutex.WaitOne(TimeSpan.Zero, true))
            {
                try
                {
#if !DEBUG
                  //Taskbar.Hide();
                  //Cursor.Hide();
#endif
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
    }
}
