using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Keebee.AAT.Display.Caregiver.CustomControls
{
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

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hwnd, int msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern uint GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern void SetWindowLong(IntPtr hwnd, int index, uint value);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X,
              int Y, int cx, int cy, uint uFlags);

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
            SendMessage(listView1.Handle, LB_SETHORIZONTALEXTENT, 1000, 0);
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
            uint windowStyle = GetWindowLong(handle, GWL_STYLE);

            // Modify style
            SetWindowLong(handle, GWL_STYLE, windowStyle | addStyle);

            // Let the window know of the changes
            SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOZORDER | SWP_NOSIZE | SWP_FRAMECHANGED);
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
