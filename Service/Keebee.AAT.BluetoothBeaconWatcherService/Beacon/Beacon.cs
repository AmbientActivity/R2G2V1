﻿using Keebee.AAT.BluetoothBeaconWatcherService.Annotations;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Bluetooth.Advertisement;

namespace Keebee.AAT.BluetoothBeaconWatcherService.Beacon
{
    /// <summary>
    /// Represents a single unique beacon that has a specified Bluetooth MAC address.
    /// Construction and updates are usually handled by the BeaconManager.
    /// 
    /// Construct this class based on a Bluetooth advertisement received from the
    /// Windows Bluetooth API. When further advertisements are received for this beacon,
    /// call its UpdateBeacon() method to update the frames.
    /// </summary>
    public class Beacon : INotifyPropertyChanged
    {
        private readonly Guid _eddystoneGuid = new Guid("0000FEAA-0000-1000-8000-00805F9B34FB");

        public enum BeaconTypeEnum
        {
            /// <summary>
            /// Bluetooth LE advertisment that is not recognized as one of the beacon formats
            /// supported by this library.
            /// </summary>
            Unknown,
            /// <summary>
            /// Beacon conforming to the Google Eddystone specification.
            /// </summary>
            Eddystone,
            /// <summary>
            /// Beacon conforming to the Apple iBeacon specification.
            /// iBeacon is a Trademark of Apple Inc.
            /// Note: the beacon broadcast payload is not parsed by this library.
            /// </summary>
            iBeacon
        }

        /// <summary>
        /// Type of this beacon.
        /// Defines how the beacon will parse the individual frames to extract information from the
        /// advertisements.
        /// </summary>
        public BeaconTypeEnum BeaconType { get; set; } = BeaconTypeEnum.Unknown;

        /// <summary>
        /// List of all the different frames that have been observed for this beacon so far.
        /// If a new frame with the same type is collected, it replaces the previous frame.
        /// </summary>
        public ObservableCollection<BeaconFrameBase> BeaconFrames { get; set; } = new ObservableCollection<BeaconFrameBase>();

        private short _rssi;
        /// <summary>
        /// Raw signal strength in dBM.
        /// If a new advertisement is received for the same beacon (with the same
        /// Bluetooth MAC address), always the latest signal strength is recorded.
        /// </summary>
        public short Rssi
        {
            get { return _rssi; }
            set
            {
                if (_rssi == value) return;
                _rssi = value;
                OnPropertyChanged();
            }
        }

        private ulong _bluetoothAddress;
        /// <summary>
        /// The Bluetooth MAC address.
        /// Used to cluster the different received Bluetooth advertisements and to
        /// collect multiple advertisements for unique beacons.
        /// </summary>
        public ulong BluetoothAddress
        {
            get { return _bluetoothAddress; }
            set
            {
                if (_bluetoothAddress == value) return;
                _bluetoothAddress = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(BluetoothAddressAsString));
            }
        }

        /// <summary>
        /// Retrieves the Bluetooth MAC address formatted as a hex string.
        /// </summary>
        public string BluetoothAddressAsString
        {
            get
            {
                return string.Join(":", BitConverter.GetBytes(BluetoothAddress).Reverse().Select(b => b.ToString("X2"))).Substring(6);
            }
        }

        private DateTimeOffset _timestamp;
        /// <summary>
        /// Timestamp when the last advertisement was received for this beacon.
        /// </summary>
        public DateTimeOffset Timestamp
        {
            get { return _timestamp; }
            set
            {
                if (_timestamp == value) return;
                _timestamp = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Construct a new Bluetooth beacon based on the received advertisement.
        /// Tries to find out if it's a known type, and then parses the contents accordingly.
        /// </summary>
        /// <param name="btAdv">Bluetooth advertisement to parse, as received from
        /// the Windows Bluetooth LE API.</param>
        public Beacon(BluetoothLEAdvertisementReceivedEventArgs btAdv)
        {
            BluetoothAddress = btAdv.BluetoothAddress;
            UpdateBeacon(btAdv);
        }

        /// <summary>
        /// Manually create a new Beacon instance.
        /// </summary>
        /// <param name="beaconType">Beacon type to use for this manually constructed beacon.</param>
        public Beacon(BeaconTypeEnum beaconType)
        {
            BeaconType = beaconType;
        }

        /// <summary>
        /// Received a new advertisement for this beacon.
        /// If the Bluetooth address of the new advertisement matches this beacon,
        /// it will parse the contents of the advertisement and extract known frames.
        /// </summary>
        /// <param name="btAdv">Bluetooth advertisement to parse, as received from
        /// the Windows Bluetooth LE API.</param>
        public void UpdateBeacon(BluetoothLEAdvertisementReceivedEventArgs btAdv)
        {
            if (btAdv == null) return;

            if (btAdv.BluetoothAddress != BluetoothAddress)
            {
                throw new BeaconException("Bluetooth address of beacon does not match - not updating beacon information");
            }

            Rssi = btAdv.RawSignalStrengthInDBm;
            Timestamp = btAdv.Timestamp;

            // Check if beacon advertisement contains any actual usable data
            if (btAdv.Advertisement == null) return;

            if (btAdv.Advertisement.ServiceUuids.Any())
            {
                foreach (var serviceUuid in btAdv.Advertisement.ServiceUuids)
                {
                    // If we have multiple service UUIDs and already recognized a beacon type, 
                    // don't overwrite it with another service Uuid.
                    if (BeaconType == BeaconTypeEnum.Unknown)
                    {
                        BeaconType = serviceUuid.Equals(_eddystoneGuid)
                            ? BeaconTypeEnum.Eddystone
                            : BeaconTypeEnum.Unknown;
                    }
                }
            }

            // Data sections
            if (btAdv.Advertisement.DataSections.Any())
            {
                if (BeaconType == BeaconTypeEnum.Eddystone)
                {
                    // This beacon is according to the Eddystone specification - parse data
                    ParseEddystoneData(btAdv);
                }
            }

            // Manufacturer data - currently unused
            if (btAdv.Advertisement.ManufacturerData.Any())
            {
                foreach (var manufacturerData in btAdv.Advertisement.ManufacturerData)
                {
                    // Print the company ID + the raw data in hex format
                    var manufacturerDataArry = manufacturerData.Data.ToArray();
                    if (manufacturerData.CompanyId == 0x4C && manufacturerData.Data.Length >= 23 &&
                        manufacturerDataArry[0] == 0x02)
                    {
                        BeaconType = BeaconTypeEnum.iBeacon;
                        // Only one relevant data frame for iBeacons
                        if (BeaconFrames.Any())
                        {
                            BeaconFrames[0].Payload = manufacturerDataArry;
                        }
                        else
                        {
                            BeaconFrames.Add(new UnknownBeaconFrame(manufacturerDataArry));
                        }
                    }
                }
            }
        }

        private void ParseEddystoneData(BluetoothLEAdvertisementReceivedEventArgs btAdv)
        {
            // Parse Eddystone data
            foreach (var dataSection in btAdv.Advertisement.DataSections)
            {
                //Debug.WriteLine("Beacon data: " + dataSection.DataType + " = " +
                //                BitConverter.ToString(dataSection.Data.ToArray()));
                //+ " (" + Encoding.UTF8.GetString(dataSection.Data.ToArray()) + ")\n");

                // Relvant data of Eddystone is in data section 0x16
                // Windows receives: 0x01 = 0x06
                //                   0x03 = 0xAA 0xFE
                //                   0x16 = 0xAA 0xFE [type] [data]
                if (dataSection.DataType == 0x16)
                {
                    var beaconFrame = dataSection.Data.ToArray().CreateEddystoneBeaconFrame();
                    if (beaconFrame == null) continue;

                    var found = false;

                    for (var i = 0; i < BeaconFrames.Count; i++)
                    {
                        if (BeaconFrames[i].GetType() == beaconFrame.GetType())
                        {
                            var updateFrame = false;
                            if (beaconFrame.GetType() == typeof(UnknownBeaconFrame))
                            {
                                // Unknown frame - also compare eddystone type
                                var existingEddystoneFrameType =
                                    BeaconFrames[i].Payload.GetEddystoneFrameType();
                                var newEddystoneFrameType = beaconFrame.Payload.GetEddystoneFrameType();
                                if (existingEddystoneFrameType != null &&
                                    existingEddystoneFrameType == newEddystoneFrameType)
                                {
                                    updateFrame = true;
                                }
                            }
                            else
                            {
                                updateFrame = true;
                            }
                            if (updateFrame)
                            {
                                BeaconFrames[i].Update(beaconFrame);
                                found = true;
                                break;  // Don't analyze any other known frames of this beacon
                            }
                        }
                    }
                    if (!found)
                    {
                        BeaconFrames.Add(beaconFrame);
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
