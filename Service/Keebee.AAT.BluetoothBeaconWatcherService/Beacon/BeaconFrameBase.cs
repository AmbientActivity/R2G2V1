using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Keebee.AAT.BluetoothBeaconWatcherService.Annotations;

namespace Keebee.AAT.BluetoothBeaconWatcherService.Beacon
{
    /// <summary>
    /// Abstract class for every Bluetooth Beacon frame.
    /// Common is that every frame has a payload / data, which derived classes can
    /// further parse and make it more accessible through custom properties depending
    /// on the beacon specification.
    /// </summary>
    public abstract class BeaconFrameBase : INotifyPropertyChanged
    {
        protected byte[] _payload;

        /// <summary>
        /// The raw byte payload of this beacon frame.
        /// Derived classes can add additional functionality to parse or update the payload.
        /// 
        /// Note: directly setting the payload does not lead to the class re-parsing the payload
        /// into its instance variables (if applicable in the derived class).
        /// Call ParsePayload() manually from the derived class if you wish to enable this behavior.
        /// </summary>
        public byte[] Payload
        {
            get { return _payload; }
            set
            {
                if (value == null)
                {
                    _payload = null;
                    return;
                }
                if (_payload != null && _payload.SequenceEqual(value)) return;
                _payload = new byte[value.Length];
                Array.Copy(value, _payload, value.Length);
                OnPropertyChanged();
            }
        }

        protected BeaconFrameBase()
        {

        }

        protected BeaconFrameBase(byte[] payload)
        {
            Payload = payload;
        }

        protected BeaconFrameBase(BeaconFrameBase other)
        {
            Payload = other.Payload;
        }

        public virtual void Update(BeaconFrameBase otherFrame)
        {
            Payload = otherFrame.Payload;
        }

        public virtual bool IsValid()
        {
            return Payload != null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
