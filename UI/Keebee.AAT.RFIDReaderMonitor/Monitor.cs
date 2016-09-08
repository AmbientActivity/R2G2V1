using Keebee.AAT.MessageQueuing;
using Keebee.AAT.Shared;
using System;
using System.Linq;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace Keebee.AAT.RfidReaderMonitor
{
    public partial class Monitor : Form
    {
        // delegate
        delegate void UpdateListViewDelegate(ListView lv, int readCount, int id);

#if DEBUG
        // message queue
        private readonly CustomMessageQueue _messageQueueRfidMonitorState;
#endif

        public Monitor()
        {
            InitializeComponent();
            ConfigureListViews();

#if DEBUG
            _messageQueueRfidMonitorState = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.RfidMonitorState
            });

            // listener
            var q = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.RfidMonitor,
                MessageReceivedCallback = MessageReceived
            });
#endif
            radOff.Checked = true;
        }

        private void ConfigureListViews()
        {
            lvReads.MultiSelect = false;
            lvReads.FullRowSelect = false;
            lvReads.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvReads.View = View.Details;

            lvReads.Columns.Add("Id", 0);
            lvReads.Columns.Add("Count", 50);
            lvReads.Columns.Add("Read Id", lvReads.Width - 150);
            lvReads.Columns.Add("Time", 95);

            lvResidents.MultiSelect = false;
            lvResidents.FullRowSelect = false;
            lvResidents.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvResidents.View = View.Details;

            lvResidents.Columns.Add("Id", 0);
            lvResidents.Columns.Add("Resident Id", lvReads.Width - 100);
            lvResidents.Columns.Add("Time", 80);
        }

        private void AddToListView(ListView lv, int readCount, int id)
        {
            if (InvokeRequired)
            {
                UpdateListViewDelegate d = AddToListView;
                Invoke(d, new object[] { lv, readCount, id });
            }
            else
            {
                if (lv.Name == "lvReads")
                    lvReads.Items.Add(new ListViewItem(new[] {string.Empty, readCount.ToString(), id.ToString(), DateTime.Now.ToLongTimeString()}));
                else
                {
                    lvReads.Items.Clear();
                    lvResidents.Items.Add(new ListViewItem(new[] {string.Empty, id.ToString(), DateTime.Now.ToLongTimeString()}));
                }
            }
        }

#if DEBUG
        private void MessageReceived(object sender, MessageEventArgs e)
        {
            var message = (e.MessageBody);
            var serializer = new JavaScriptSerializer();
            var readerMessage = serializer.Deserialize<RfidMonitorMessage>(message);

            AddToListView(readerMessage.IsFinal 
                ? lvResidents
                : lvReads, readerMessage.ReadCount, readerMessage.ResidentId);
        }
#endif

        private void MonitorFormClosing(object sender, FormClosingEventArgs e)
        {
#if DEBUG
            _messageQueueRfidMonitorState.Send("0");
#endif
        }

        private void MonitorCheckChanged(object sender, EventArgs e)
        {
#if DEBUG
            foreach (var result in from control in grpMonitor.Controls.OfType<RadioButton>()
                                   select control into radio
                                   where radio.Checked
                                   select radio.Text)
            {
                _messageQueueRfidMonitorState.Send(result == "On" ? "1" : "0");
            }
#endif
        }

        private void ClearButtonClick(object sender, EventArgs e)
        {
            lvResidents.Items.Clear();
        }
    }
}
