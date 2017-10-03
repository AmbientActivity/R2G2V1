using System;
using System.Collections;
using System.Windows.Forms;

namespace Keebee.AAT.Display.Caregiver.CustomControls
{
    public partial class ComboBoxLarge : UserControl
    {
        // event handler
        public event EventHandler SelectedIndexChanged;
        public event EventHandler ComboClick;

        // delegate
        public delegate void SelectedIndexChangedDelegate(object sender, EventArgs e);
        public delegate void ComboClickDelegate(object sender, EventArgs e);

        public string ValueMember
        {
            set { comboBox1.ValueMember = value; }
        }

        public string DisplayMember
        {
            set { comboBox1.DisplayMember = value; }
        }

        public ArrayList DataSource
        {
            set { comboBox1.DataSource = value; }
        }

        public object SelectedValue
        {
            set { comboBox1.SelectedValue = value; }
            get { return comboBox1.SelectedValue; } 
        }

        public ComboBoxLarge()
        {
            InitializeComponent();
            comboBox1.SelectedIndexChanged += RaiseCSelectedIndexChanged;
            comboBox1.Click += RaiseComboClick;
        }

        private void RaiseCSelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                Invoke(new SelectedIndexChangedDelegate(RaiseCSelectedIndexChanged), new { sender, e });
            }
            else
            {
                SelectedIndexChanged?.Invoke(sender, e);
            }
        }

        private void RaiseComboClick(object sender, EventArgs e)
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                Invoke(new ComboClickDelegate(RaiseComboClick), new { sender, e });
            }
            else
            {
                ComboClick?.Invoke(sender, e);
            }
        }

        private void ButtonClick(object sender, EventArgs e)
        {
            comboBox1.DroppedDown = true;
        }
    }
}
