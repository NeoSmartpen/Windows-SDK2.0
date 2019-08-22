using Neosmartpen.Net.Usb.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;

namespace Neosmartpen.Net.Usb
{
    /// <summary>
    /// A class that implements functions related to USB connection
    /// </summary>
    public class UsbAdapter : IDisposable
    {
        private static readonly string USB_VID_PID_PREFIX = "USB\\VID_0E8D&PID_0023";

        private List<UsbPenComm> usbPenComms = new List<UsbPenComm>();

        /// <summary>
        /// List of currently connected pens
        /// </summary>
        public List<UsbPenComm> UsbPenComms { get { return usbPenComms.Where(p => p.IsActive && p.IsOpen).ToList(); } }

        private ManagementEventWatcher _plugInWatcher;
        private ManagementEventWatcher _unPlugWatcher;

        private static UsbAdapter usbAdapterInstance;

        /// <summary>
        /// Whether the watcher detects pen connection and disconnection
        /// </summary>
        public bool IsWatching { get; private set; }

        private UsbAdapter()
        {
        }

        /// <summary>
        /// Get an instance of UsbAdapter.
        /// </summary>
        /// <returns></returns>
        public static UsbAdapter GetInstance()
        {
            if (usbAdapterInstance == null)
                usbAdapterInstance = new UsbAdapter();

            return usbAdapterInstance;
        }

        /// <summary>
        /// Searches for the currently connected pen and starts communication.
        /// </summary>
        public void SearchAndConnect()
        {
            var ports = GetSerialPortList();

            foreach (var port in ports)
            {
                if (usbPenComms.Where(p=>p.PortName == port).Count() > 0)
                {
                    continue;
                }

                var penComm = new UsbPenComm(port);
                penComm.Open();

                if (penComm.IsOpen)
                {
                    penComm.Authenticated += PenComm_Authenticated;
                    penComm.ConnectionRefused += PenComm_ConnectionRefused;
                    penComm.Disconnected += PenComm_Disconnected;
                    usbPenComms.Add(penComm);
                    penComm.Connect();
                }
                else
                {
                    penComm.Dispose();
                }
            }

            ports.Clear();
            ports = null;
        }

        /// <summary>
        /// Disconnect the pen with the corresponding Mac address.
        /// </summary>
        /// <param name="macAddress">Mac address in text format</param>
        public void DisconnectByMacAddress(string macAddress)
        {
            var penComms = usbPenComms.Where(p => p.MacAddress.Replace(":", "").ToLower() == macAddress.Replace(":", "").ToLower()).ToList();
            if (penComms == null || penComms.Count <= 0)
                throw new Exceptions.NoSuchPenException();
            foreach(var penComm in penComms)
            {
                penComm.Dispose();
            }
        }

        private void PenComm_Authenticated(object sender, System.EventArgs e)
        {
            Connected?.Invoke(this, new ConnectionStatusChangedEventArgs((UsbPenComm)sender));
        }

        private void PenComm_Disconnected(object sender, EventArgs e)
        {
            var usbPenComm = sender as UsbPenComm;
            usbPenComm.Authenticated -= PenComm_Authenticated;
            usbPenComm.ConnectionRefused -= PenComm_ConnectionRefused;
            usbPenComm.Disconnected -= PenComm_Disconnected;
            usbPenComms.Remove(usbPenComm);

            Disconnected?.Invoke(this, new ConnectionStatusChangedEventArgs(usbPenComm));
        }

        private void PenComm_ConnectionRefused(object sender, System.EventArgs e)
        {
            UsbPenComm usbPenComm = (UsbPenComm)sender;
            usbPenComm.Authenticated -= PenComm_Authenticated;
            usbPenComm.ConnectionRefused -= PenComm_ConnectionRefused;
            usbPenComm.Disconnected -= PenComm_Disconnected;
            usbPenComm.Dispose();
            usbPenComms.Remove(usbPenComm);
        }

        /// <summary>
        /// Get the UsbPenComm instance corresponding to the Port Name.
        /// </summary>
        /// <param name="portName">Name of Serial Port</param>
        /// <returns>UsbPenComm instance</returns>
        public UsbPenComm GetUsbPenComm(string portName)
        {
            return usbPenComms.Where(p => p.PortName == portName).FirstOrDefault();
        }

        private void RemoveSerialPortComm(string portName)
        {
            var usbPenComm = GetUsbPenComm(portName);

            if (usbPenComm != null)
            {
                usbPenComm.Dispose();
            }
        }

        private void AddSerialPortComm(string portName)
        {
            var usbPenComm = GetUsbPenComm(portName);

            if (usbPenComm == null)
            {
                UsbPenComm newUsbPenComm = new UsbPenComm(portName);
                newUsbPenComm.Open();

                if (newUsbPenComm.IsOpen)
                {
                    newUsbPenComm.Authenticated += PenComm_Authenticated;
                    newUsbPenComm.ConnectionRefused += PenComm_ConnectionRefused;
                    newUsbPenComm.Disconnected += PenComm_Disconnected;
                    usbPenComms.Add(newUsbPenComm);
                    newUsbPenComm.Connect();
                }
                else
                {
                    newUsbPenComm.Dispose();
                }
            }
        }

        private List<string> GetSerialPortList()
        {
            List<string> results = new List<string>();

            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity WHERE PNPClass = 'Ports'");

                foreach (ManagementObject queryObj in searcher.Get())
                {
                    string Name = queryObj["Name"] as string;
                    string DeviceId = queryObj["DeviceId"] as string;
                    string PNPClass = queryObj["PNPClass"] as string;

                    if (Name != null && Name.Contains("(COM") && PNPClass == "Ports" && DeviceId.StartsWith(USB_VID_PID_PREFIX))
                    {
                        Regex reg = new Regex(@"COM[0-9]{1,2}", RegexOptions.IgnoreCase);
                        Match result = reg.Match(Name);

                        if (result.Success)
                        {
                            results.Add(result.Value);
                        }
                    }
                }
            }
            catch (ManagementException)
            {
            }

            return results.ToList();
        }

        /// <summary>
        /// Stop the UsbAdapter and release the allocated resources
        /// </summary>
        public void Dispose()
        {
            StopWatcher();

            foreach(var usbPenComm in usbPenComms)
            {
                usbPenComm.Authenticated -= PenComm_Authenticated;
                usbPenComm.ConnectionRefused -= PenComm_ConnectionRefused;
                usbPenComm.Disconnected -= PenComm_Disconnected;
                usbPenComm.Dispose();
            }

            usbPenComms.Clear();

            if (_plugInWatcher != null)
            {
                try
                {
                    _plugInWatcher.Dispose();
                    _plugInWatcher = null;
                }
                catch (Exception) { }
            }

            if (_unPlugWatcher != null)
            {
                try
                {
                    _unPlugWatcher.Dispose();
                    _unPlugWatcher = null;
                }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// Runs a watcher that detects a USB connection.
        /// </summary>
        public void StartWatcher()
        {
            if (IsWatching)
                return;

            IsWatching = true;
            // __InstanceOperationEvent
            const string plugInSql = "SELECT * FROM __InstanceCreationEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_PnPEntity'";
            const string unpluggedSql = "SELECT * FROM __InstanceDeletionEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_PnPEntity'";

            var scope = new ManagementScope("root\\CIMV2") { Options = { EnablePrivileges = true } };

            var pluggedInQuery = new WqlEventQuery(plugInSql);
            _plugInWatcher = new ManagementEventWatcher(scope, pluggedInQuery);
            _plugInWatcher.EventArrived += HandlePluggedInEvent;
            _plugInWatcher.Start();

            var unPluggedQuery = new WqlEventQuery(unpluggedSql);
            _unPlugWatcher = new ManagementEventWatcher(scope, unPluggedQuery);
            _unPlugWatcher.EventArrived += HandleUnPluggedEvent;
            _unPlugWatcher.Start();
        }

        /// <summary>
        /// Stop a watcher that detects a USB connection.
        /// </summary>
        public void StopWatcher()
        {
            if (!IsWatching)
                return;
            IsWatching = false;
            _plugInWatcher.Stop();
            _unPlugWatcher.Stop();
        }

        private void HandleUnPluggedEvent(object sender, EventArrivedEventArgs e)
        {
            var portName = GetSerialPortNameFromManagementBaseObject(e.NewEvent);

            if (!string.IsNullOrEmpty(portName))
            {
                RemoveSerialPortComm(portName);
            }
        }

        private void HandlePluggedInEvent(object sender, EventArrivedEventArgs e)
        {
            var portName = GetSerialPortNameFromManagementBaseObject(e.NewEvent);

            if (!string.IsNullOrEmpty(portName))
            {
                AddSerialPortComm(portName);
            }
        }

        private string GetSerialPortNameFromManagementBaseObject(ManagementBaseObject newEvent)
        {
            var targetInstanceData = newEvent.Properties["TargetInstance"];
            var targetInstanceObject = (ManagementBaseObject)targetInstanceData.Value;
            if (targetInstanceObject == null)
            {
                Console.WriteLine("object is null!!!");
                return null;
            }

            Console.WriteLine("=================== Get Infomation ====================");
            foreach (var p in targetInstanceObject.Properties)
            {
                if (p.Value is string[])
                {
                    string[] strs = p.Value as string[];
                    string str = string.Empty;
                    foreach (var s in strs)
                        str += (s + ", ");

                    Console.WriteLine($"Name : {p.Name}, Value : {str}, Qualifiers : {p.Qualifiers}, isArray : {p.IsArray}, isLocal : {p.IsLocal}, Origin : {p.Origin}");
                }
                else
                    Console.WriteLine($"Name : {p.Name}, Value : {p.Value}, Qualifiers : {p.Qualifiers}, isArray : {p.IsArray}, isLocal : {p.IsLocal}, Origin : {p.Origin}");
            }

            string name = targetInstanceObject.Properties["Name"].Value.ToString();
            string deviceId = targetInstanceObject.Properties["DeviceId"].Value.ToString();

            var match = Regex.Match(name.ToUpper(), "COM(\\d{1,3})");

            if (deviceId.StartsWith(USB_VID_PID_PREFIX) && match.Success)
            {
                return match.Value;
            }
            else
            {
                return null;
            }
        }

        #region event
        /// <summary>
        /// Event that occurs when a USB connection is established and communication with the pen is initiated.
        /// </summary>
        public event EventHandler<ConnectionStatusChangedEventArgs> Connected;
        /// <summary>
        /// Occurs when communication with the pen has ended.
        /// </summary>
        public event EventHandler<ConnectionStatusChangedEventArgs> Disconnected;
        #endregion event
    }
}
