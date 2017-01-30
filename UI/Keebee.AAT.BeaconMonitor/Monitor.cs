using Keebee.AAT.MessageQueuing;
using Keebee.AAT.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace Keebee.AAT.BeaconMonitor
{
    public partial class Monitor : Form
    {
        // delegate
        private delegate void UpdateLabelActiveResidentDelegate(string residentName, int rssi);

        private delegate void AddListViewBeaconDelegate(string beaconType, ulong address, short rssi, DateTimeOffset updated, string payload);
        private delegate void UpdateListViewBeaconDelegate(ulong address, short rssi, DateTimeOffset updated);
        private delegate void RemoveListViewBeaconDelegate(ulong address);

        // message queue
        private readonly CustomMessageQueue _messageQueueBeaconMonitorState;

        private readonly ICollection<ulong> _beaconAddresses = new Collection<ulong>();
        public Monitor()
        {
            InitializeComponent();
            ConfigureListViews();

#if DEBUG
            _messageQueueBeaconMonitorState = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.BeaconMonitorState
            });

            // listener
            var q1 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.BeaconMonitor,
                MessageReceivedCallback = MessageReceivedBeacon
            });

            var q2 = new CustomMessageQueue(new CustomMessageQueueArgs
            {
                QueueName = MessageQueueType.BeaconMonitorResident,
                MessageReceivedCallback = MessageReceivedBeaconResident
            });
#endif
            radOff.Checked = true;
        }

        private void ConfigureListViews()
        {
            lvBeacons.MultiSelect = false;
            lvBeacons.FullRowSelect = false;
            lvBeacons.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvBeacons.View = View.Details;

            lvBeacons.Columns.Add("Id", 0);
            lvBeacons.Columns.Add("Type", 60);
            lvBeacons.Columns.Add("Address", 110);
            lvBeacons.Columns.Add("Rssi", 35);
            lvBeacons.Columns.Add("Updated", 170);
            lvBeacons.Columns.Add("Payload", 280);
        }

        private void UpdateLabelActiveResident(string residentName, int rssi)
        {
            if (InvokeRequired)
            {
                UpdateLabelActiveResidentDelegate d = UpdateLabelActiveResident;
                Invoke(d, new object[] { residentName, rssi });
            }
            else
            {
                lblActiveResident.Text = residentName;
                lblRssi.Text = rssi.ToString();
            }
        }

        private void AddListViewBeacon(string beaconType, ulong address, short rssi, DateTimeOffset updated, string payload)
        {
            if (InvokeRequired)
            {
                AddListViewBeaconDelegate d = AddListViewBeacon;
                Invoke(d, new object[] { beaconType, address, rssi, updated, payload });
            }
            else
            {
                lvBeacons.Items.Add(new ListViewItem(new[] { string.Empty, beaconType, address.ToString(), rssi.ToString(), updated.ToString(), payload }));
            }
        }

        private void UpdateListViewBeacon(ulong address, short rssi, DateTimeOffset updated)
        {
            if (InvokeRequired)
            {
                UpdateListViewBeaconDelegate d = UpdateListViewBeacon;
                Invoke(d, new object[] { address, rssi, updated });
            }
            else
            {
                var item = lvBeacons.FindItemWithText(address.ToString());
                item.SubItems[3].Text = rssi.ToString();
                item.SubItems[4].Text = updated.ToString();
            }
        }

        private void RemoveListViewBeacon(ulong address)
        {
            if (InvokeRequired)
            {
                RemoveListViewBeaconDelegate d = RemoveListViewBeacon;
                Invoke(d, new object[] { address });
            }
            else
            {
                var item = lvBeacons.FindItemWithText(address.ToString());
                item.Remove();
            }
        }

        private void MessageReceivedBeacon(object sender, MessageEventArgs e)
        {
            var message = (e.MessageBody);
            var serializer = new JavaScriptSerializer();
            var beaconMessages = serializer.Deserialize<IEnumerable<BeaconMonitorMessage>>(message).ToArray();

            foreach (var beaconMessage in beaconMessages)
            {
                if (_beaconAddresses.All(b => b != beaconMessage.Address))
                {
                    AddListViewBeacon(beaconMessage.BeaconType, beaconMessage.Address, beaconMessage.Rssi.First(), beaconMessage.TimeStamp, beaconMessage.Payload);
                    _beaconAddresses.Add(beaconMessage.Address);
                }
                else
                {
                    UpdateListViewBeacon(beaconMessage.Address, beaconMessage.Rssi.First(), beaconMessage.TimeStamp);
                }
            }

            var addresses = _beaconAddresses;
            foreach (var address in addresses)
            {
                if (beaconMessages.Any(b => b.Address == address)) continue;

                RemoveListViewBeacon(address);
                _beaconAddresses.Remove(address);
            }
        }

        private void MessageReceivedBeaconResident(object sender, MessageEventArgs e)
        {
            var message = (e.MessageBody);
            var serializer = new JavaScriptSerializer();
            var residentMessage = serializer.Deserialize<BeaconMonitorResidentMessage>(message);

            UpdateLabelActiveResident(residentMessage.ResidentName, residentMessage.Rssi);
        }

        private void MonitorFormClosing(object sender, FormClosingEventArgs e)
        {
            _messageQueueBeaconMonitorState.Send("0");
        }

        private void MonitorCheckChanged(object sender, EventArgs e)
        {
#if DEBUG
            foreach (var result in from control in grpMonitor.Controls.OfType<RadioButton>()
                                   select control into radio
                                   where radio.Checked
                                   select radio.Text)
            {
                _messageQueueBeaconMonitorState.Send(result == "On" ? "1" : "0");
            }
#endif
        }

        private void ClearButtonClick(object sender, EventArgs e)
        {
            lvBeacons.Items.Clear();
            _beaconAddresses.Clear();
        }
    }
}
