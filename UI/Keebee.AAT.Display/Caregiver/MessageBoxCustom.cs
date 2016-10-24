using System;
using System.Windows.Forms;

namespace Keebee.AAT.Display.Caregiver
{
    public partial class MessageBoxCustom : Form
    {
        private string _messageText;
        public string MessageText
        {
            set { _messageText = value; }
        }
        public MessageBoxCustom()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MessageBoxCustom_Shown(object sender, EventArgs e)
        {
            lblMessageText.Text = _messageText;
        }
    }
}
