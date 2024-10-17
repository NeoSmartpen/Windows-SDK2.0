using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Devices.Enumeration;

namespace Neosmartpen.Net.Bluetooth
{
    /// <summary>
    /// Represents Information of smartpen device
    /// </summary>
	public class PenInformation : INotifyPropertyChanged
    {
        public static PenInformation Create(DeviceInformation devInfo, string name, ulong vMac, string mac, int rssi, int protocol, bool isLe = false)
        {
            return new PenInformation(devInfo, isLe)
            {
                name = name,
                VirtualMacAddress = vMac,
                MacAddress = mac,
                Rssi = rssi,
                Protocol = protocol
            };
        }
        internal PenInformation(DeviceInformation devInfo, bool isLe = false)
        {
            deviceInformation = devInfo;
            IsLe = isLe;
        }

        public bool IsLe { get; internal set; }


        /// <summary>
        /// Gets the device identifier
        /// </summary>
        public string Id
        {
            get
            {
                if (deviceInformation == null)
                    return string.Empty;
                return deviceInformation.Id;
            }
        }

        /// <summary>
        /// Gets a common name of a device
        /// </summary>
        public string Name
        {
            get
            {
                if (IsLe)
                {
                    return name;
                }
                else
                {
                    if (deviceInformation == null)
                        return string.Empty;
                    return deviceInformation.Name;
                }
            }
        }
        private string name;

        /// <summary>
        /// Gets the signal strength for the Bluetooth connection with the peer device
        /// </summary>
		public int Rssi
        {
            get { return rssi; }
            internal set
            {
                rssi = value;
                NotifyPropertyChanged();
            }
        }
        private int rssi;

        /// <summary>
        /// Gets the device's mac address
        /// </summary>
        public string MacAddress { get; internal set; }
        public ulong BluetoothAddress
        {
            get
            {
                string temp = MacAddress.Replace(":", "");

                return Convert.ToUInt64(temp, 16);
            }
        }

        public int Protocol
        {
            get; set;
        }

        public void Update(PenUpdateInformation update)
        {
            if (IsLe)
            {
                Rssi = update.Rssi;
                if (!string.IsNullOrEmpty(update.ModelName))
                {
                    name = update.ModelName;
                    NotifyPropertyChanged("Name");
                }
            }
            else
            {
                if (deviceInformation != null && update.deviceInformationUpdate != null)
                {
                    deviceInformation.Update(update.deviceInformationUpdate);
                }
            }
        }

        public override string ToString()
        {
            return "[" + Name + "] " + MacAddress;
        }

        internal DeviceInformation deviceInformation;
        public ulong VirtualMacAddress { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class PenUpdateInformation
    {
        public static PenUpdateInformation Create(DeviceInformationUpdate update, bool isLe = false)
        {
            return new PenUpdateInformation(update, isLe);
        }
        public static PenUpdateInformation Create(string id, bool isLe = false)
        {
            return new PenUpdateInformation(null, isLe)
            {
                id = id
            };
        }
        internal PenUpdateInformation(DeviceInformationUpdate update, bool isLe = false)
        {
            deviceInformationUpdate = update;
            IsLe = isLe;
        }
        public bool IsLe { get; internal set; }

        internal string id;
        public string Id
        {
            get
            {
                if (!string.IsNullOrEmpty(id))
                    return id;
                if (deviceInformationUpdate != null)
                    return deviceInformationUpdate.Id;
                return string.Empty;
            }
        }
        public string ModelName { get; set; }
        public int Rssi { get; set; }

        internal DeviceInformationUpdate deviceInformationUpdate;

    }
}
