using System;
using System.Threading;
using System.Windows.Forms;

namespace Keebee.AAT.Simulator
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static readonly Mutex Mutex = new Mutex(true, "{8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE8F}");

        [STAThread]
        private static void Main()
        {
            if (Mutex.WaitOne(TimeSpan.Zero, true))
            {
                try
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new ControlPanel());
                }
                finally
                {
                    Mutex.ReleaseMutex();
                }
            }
            else
            {
                MessageBox.Show("Simulator is already running.");
            }
        }
    }
}
