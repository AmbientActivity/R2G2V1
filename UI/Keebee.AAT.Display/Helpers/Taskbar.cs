using System;
using System.Runtime.InteropServices;

namespace Keebee.AAT.Display.Helpers
{
    internal static class UnsafeNativeMethods
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr FindWindow(string className, string windowText);
        [DllImport("user32.dll")]
        internal static extern int ShowWindow(IntPtr hwnd, int command);
    }

    public class Taskbar
    {
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 1;

        protected static IntPtr Handle => UnsafeNativeMethods.FindWindow("Shell_TrayWnd", "");

        private Taskbar()
        {
            // hide constructor
        }

        public static void Show()
        {
            UnsafeNativeMethods.ShowWindow(Handle, SW_SHOW);
        }

        public static void Hide()
        {
            UnsafeNativeMethods.ShowWindow(Handle, SW_HIDE);
        }
    }
}
