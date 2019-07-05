﻿using System;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Keebee.AAT.BluetoothBeaconWatcherService.Beacon
{
    /// <summary>
    /// An Eddystone UID frame, according to the Google Specification from
    /// https://github.com/google/eddystone/tree/master/eddystone-uid
    /// </summary>
    public class UidEddystoneFrame : BeaconFrameBase
    {
        private sbyte _rangingData;

        /// <summary>
        /// Tx power level - the received power level at 0 m, in dBm.
        /// Values range from -100 to +20 dBM, with a resolution of 1 dBm.
        /// Signed 8 bit integer according to:
        /// https://developer.bluetooth.org/gatt/characteristics/Pages/CharacteristicViewer.aspx?u=org.bluetooth.characteristic.tx_power_level.xml
        /// Conversion between sbyte and byte works at runtime with simple casting for variables.
        /// In order to convert constants at compile time, you might need to run the conversion in
        /// an unchecked() context. For example:
        /// sbyte rangingData = unchecked((sbyte)0xEE);
        /// According to the documentation, this corresponds to the dBm value of -18.
        /// </summary>
        public sbyte RangingData
        {
            get { return _rangingData; }
            set
            {
                if (_rangingData == value) return;
                _rangingData = value;
                UpdatePayload();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 10-byte namespace, intended to ensure ID uniqueness accross multiple
        /// Eddystone implementers.
        /// For hints how to construct the namespace ID, see:
        /// https://github.com/google/eddystone/tree/master/eddystone-uid
        /// </summary>
        public byte[] NamespaceId
        {
            get { return NamespaceId1; }
            set
            {
                if (NamespaceId1 == value) return;
                if (value == null)
                {
                    NamespaceId1 = null;
                    return;
                }
                if (NamespaceId1 != null && NamespaceId1.SequenceEqual(value)) return;
                NamespaceId1 = new byte[value.Length];
                Array.Copy(value, NamespaceId1, value.Length);
                UpdatePayload();
                OnPropertyChanged();
                OnPropertyChanged(nameof(NamespaceIdAsNumber));
            }
        }

        /// <summary>
        /// Interprets the namespace ID as number.
        /// As it is 10 bytes, returned as BigInteger.
        /// </summary>
        public BigInteger NamespaceIdAsNumber => new BigInteger(NamespaceId.Reverse().ToArray());

        private byte[] _instanceId;

        /// <summary>
        /// 6-byte instance ID of the specific beacon belonging to a certain
        /// namespace.
        /// The ID can be assigned by your app in any custom way.
        /// </summary>
        public byte[] InstanceId
        {
            get { return _instanceId; }
            set
            {
                if (value == null)
                {
                    _instanceId = null;
                    return;
                }
                if (_instanceId != null && _instanceId.SequenceEqual(value)) return;
                _instanceId = new byte[value.Length];
                Array.Copy(value, _instanceId, value.Length);
                UpdatePayload();
                OnPropertyChanged();
                OnPropertyChanged(nameof(InstanceIdAsNumber));
            }
        }

        /// <summary>
        /// Return the instance ID as number.
        /// </summary>
        public ulong InstanceIdAsNumber
        {
            get
            {
                var tmpArray = (BitConverter.IsLittleEndian) ? InstanceId.Reverse().ToArray() : InstanceId;
                var tst = new byte[8];
                Array.Copy(tmpArray, 0, tst, 0, 6);

                return BitConverter.ToUInt64(tst, 0);
            }
        }

        public byte[] NamespaceId1 { get; set; }

        public UidEddystoneFrame(sbyte rangingData, byte[] namespaceId, byte[] instanceId)
        {
            _rangingData = rangingData;
            if (namespaceId != null && namespaceId.Length == 10)
            {
                NamespaceId1 = new byte[10];
                Array.Copy(namespaceId, NamespaceId1, namespaceId.Length);
            }
            if (instanceId != null && instanceId.Length == 6)
            {
                _instanceId = new byte[6];
                Array.Copy(instanceId, _instanceId, instanceId.Length);
            }
            UpdatePayload();
        }

        public UidEddystoneFrame(byte[] payload) : base(payload){}

        /// <summary>
        /// Parse the current payload into the properties exposed by this class.
        /// Has to be called if manually modifying the raw payload.
        /// </summary>
        public void ParsePayload()
        {
            if (!IsValid()) return;

            // The Ranging Data is the Tx power in dBm emitted by the beacon at 0 meters.
            // Note that this is different from other beacon protocol specifications that require the Tx power 
            // to be measured at 1 m.The value is an 8-bit integer as specified by the Tx Power Level Characteristic 
            // and ranges from -100 dBm to +20 dBm to a resolution of 1 dBm.
            var newRangingData = (sbyte)Payload[BeaconFrameHelper.EddystoneHeaderSize];
            if (newRangingData != RangingData)
            {
                _rangingData = newRangingData;
                OnPropertyChanged(nameof(RangingData));
            }

            // Namespace ID
            var newNamespaceId = new byte[10];
            Array.Copy(Payload, BeaconFrameHelper.EddystoneHeaderSize + 1, newNamespaceId, 0, 10);
            if (NamespaceId == null || !newNamespaceId.SequenceEqual(NamespaceId))
            {
                NamespaceId1 = newNamespaceId;
                OnPropertyChanged(nameof(NamespaceId));
                OnPropertyChanged(nameof(NamespaceIdAsNumber));
            }

            // Instance ID
            var newInstanceId = new byte[6];
            Array.Copy(Payload, BeaconFrameHelper.EddystoneHeaderSize + 11, newInstanceId, 0, 6);
            if (InstanceId == null || !newInstanceId.SequenceEqual(InstanceId))
            {
                _instanceId = newInstanceId;
                OnPropertyChanged(nameof(InstanceId));
                OnPropertyChanged(nameof(InstanceIdAsNumber));
            }
            _instanceId = newInstanceId;

            //Debug.WriteLine("Eddystone Uid Frame: Ranging data = "
            //    + RangingData + ", NS = " + BitConverter.ToString(NamespaceId) + ", Instance = " + BitConverter.ToString(InstanceId));
        }

        /// <summary>
        /// Update the raw payload when properties have changed.
        /// </summary>
        private void UpdatePayload()
        {
            if (NamespaceId == null || NamespaceId.Length != 10 ||
                InstanceId == null || InstanceId.Length != 6)
            {
                Payload = null;
                return;
            }

            var header = BeaconFrameHelper.CreateEddystoneHeader(BeaconFrameHelper.EddystoneFrameType.UidFrameType);
            using (var ms = new MemoryStream())
            {
                // Frame header
                ms.Write(header, 0, header.Length);
                // Ranging data
                ms.WriteByte((byte)RangingData);
                // Namespace ID
                ms.Write(NamespaceId, 0, NamespaceId.Length);
                // Instance ID
                ms.Write(InstanceId, 0, InstanceId.Length);
                // RFU (2 bytes, must be 0x00)
                ms.WriteByte(0x00);
                ms.WriteByte(0x00);
                // Save to payload (to direct array to prevent re-parsing and a potential endless loop of updating and parsing)
                Payload = ms.ToArray();
            }
        }

        /// <summary>
        /// Update the information stored in this frame with the information from the other frame.
        /// Useful for example when binding the UI to beacon information, as this will emit
        /// property changed notifications whenever a value changes - which would not be possible if
        /// you would overwrite the whole frame.
        /// </summary>
        /// <param name="otherFrame">Frame to use as source for updating the information in this beacon
        /// frame.</param>
        public override void Update(BeaconFrameBase otherFrame)
        {
            base.Update(otherFrame);
            ParsePayload();
        }

        /// <summary>
        /// Check if the contents of this frame are generally valid.
        /// Does not currently perform a deep analysis, but checks the header as well
        /// as the frame length.
        /// </summary>
        /// <returns>True if the frame is a valid Eddystone UID frame.</returns>
        public override bool IsValid()
        {
            if (!base.IsValid()) return false;

            // 2 bytes ID: AA FE
            // 1 byte frame type
            if (!Payload.IsEddystoneFrameType()) return false;

            // 1 byte ranging data
            // 10 bytes namespace id
            // 6 bytes instance id
            // 2 bytes RFU - have to be present according to specfication, but are
            //               sometimes omitted in practice
            return Payload.Length == BeaconFrameHelper.EddystoneHeaderSize + 17 ||
                Payload.Length == BeaconFrameHelper.EddystoneHeaderSize + 17 + 2;

        }
    }
}
