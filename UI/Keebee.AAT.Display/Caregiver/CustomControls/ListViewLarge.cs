using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Keebee.AAT.Display.Caregiver.CustomControls
{
    internal static class NativeMethods
    {
        [DllImport("user32.dll")]
        internal static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        internal static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll")]
        internal static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter,
                        int x, int y, int width, int height, uint flags);

        [DllImport("user32.dll")]
        internal static extern IntPtr SendMessage(IntPtr hwnd, uint msg,
                        IntPtr wParam, IntPtr lParam);
    }

    public partial class ListViewLarge : UserControl
    {
        const int LB_GETHORIZONTALEXTENT = 0x0193;
        const int LB_SETHORIZONTALEXTENT = 0x0194;

        const long WS_HSCROLL = 0x00100000L;

        const int SWP_FRAMECHANGED = 0x0020;
        const int SWP_NOMOVE = 0x0002;
        const int SWP_NOSIZE = 0x0001;
        const int SWP_NOZORDER = 0x0004;

        const int GWL_STYLE = (-16);

        // event handler
        public event ColumnWidthChangingEventHandler ColumnWidthChanging;
        public event EventHandler ItemClicked;

        // delegate
        public delegate void ColumnWidthChangingDelegate(object sender, ColumnWidthChangingEventArgs e);
        public delegate void ClickDelegate(object sender, EventArgs e);

        // exposed properties
        public ListView.ColumnHeaderCollection Columns => listView1.Columns;
        public bool GridLines { get { return listView1.GridLines; } set { listView1.GridLines = value; } }
        public bool MultiSelect { get { return listView1.MultiSelect; } set { listView1.MultiSelect = value; } }
        public bool FullRowSelect { get { return listView1.FullRowSelect; } set { listView1.FullRowSelect = value; } }
        public ColumnHeaderStyle HeaderStyle { get { return listView1.HeaderStyle; } set { listView1.HeaderStyle = value; } }
        public View View { get { return listView1.View; } set { listView1.View = value; } }
        public ImageList SmallImageList { get { return listView1.SmallImageList; } set { listView1.SmallImageList = value; } }
        public ListView.ListViewItemCollection Items => listView1.Items;
        public ListView.SelectedListViewItemCollection SelectedItems => listView1.SelectedItems;
        public ListView.SelectedIndexCollection SelectedIndices => listView1.SelectedIndices;

        public ListViewLarge()
        {
            InitializeComponent();
            listView1.Click += RaiseClick;
            listView1.ColumnWidthChanging += RaiseColumnWidthChanging;
            SetStyle(ControlStyles.DoubleBuffer, true);
            AddStyle(listView1.Handle, (uint)WS_HSCROLL);
            NativeMethods.SendMessage(listView1.Handle, LB_SETHORIZONTALEXTENT, (IntPtr)1000, (IntPtr)0);
        }

        private void RaiseColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                Invoke(new ColumnWidthChangingDelegate(RaiseColumnWidthChanging), new { sender, e });
            }
            else
            {
                ColumnWidthChanging?.Invoke(sender, e);
            }
        }

        private void RaiseClick(object sender, EventArgs e)
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                Invoke(new ClickDelegate(RaiseClick), new { sender, e });
            }
            else
            {
                ItemClicked?.Invoke(sender, e);
            }
        }

        private static void AddStyle(IntPtr handle, uint addStyle)
        {
            // Get current window style
            int windowStyle = NativeMethods.GetWindowLong(handle, GWL_STYLE);

            // Modify style
            NativeMethods.SetWindowLong(handle, GWL_STYLE, windowStyle | (int)addStyle);

            // Let the window know of the changes
            NativeMethods.SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOZORDER | SWP_NOSIZE | SWP_FRAMECHANGED);
        }

        //List view header formatters
        public void ColorListViewHeader(Color backColor, Color foreColor)
        {
            listView1.OwnerDraw = true;
            listView1.DrawColumnHeader +=
                new DrawListViewColumnHeaderEventHandler
                (
                    (sender, e) => HeaderDraw(sender, e, backColor, foreColor)
                );
            listView1.DrawItem += new DrawListViewItemEventHandler(BodyDraw);
        }

        private static void HeaderDraw(object sender, DrawListViewColumnHeaderEventArgs e, Color backColor, Color foreColor)
        {
            e.Graphics.FillRectangle(new SolidBrush(backColor), e.Bounds);
            e.Graphics.DrawString(e.Header.Text, e.Font, new SolidBrush(foreColor), e.Bounds);
        }

        private static void BodyDraw(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }
    }
}
