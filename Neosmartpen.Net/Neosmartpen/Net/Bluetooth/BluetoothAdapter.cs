using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace Neosmartpen.Net.Bluetooth
{
    /// <summary>
    /// Class describing the devices discovered.
    /// </summary>
    public class PenDevice
    {
        /// <summary>
        /// Gets a name of a device.
        /// </summary>
        public string Name { internal set; get; }

        /// <summary>
        /// Gets the device identifier.
        /// </summary>
        public string Address { internal set; get; }

        /// <summary>
        /// Specifies whether the device is authenticated, paired, or bonded. All authenticated devices are remembered. 
        /// </summary>
        public bool Authenticated { internal set; get; }

        /// <summary>
        /// Date and Time this device was last seen by the system. 
        /// </summary>
        public DateTime LastSeen { internal set; get; }

        /// <summary>
        /// Date and Time this device was last used by the system.
        /// </summary>
        public DateTime LastUsed { internal set; get; }

        /// <summary>
        /// Specifies whether the device is a remembered device. Not all remembered devices are authenticated.
        /// </summary>
        public bool Remembered { internal set; get; }
        
        /// <summary>
        /// Returns the signal strength for the Bluetooth connection with the peer device. 
        /// </summary>
        public int Rssi { internal set; get; }

        /// <summary>
        /// Returns the Class of Device of the remote device.
        /// </summary>
        public uint ClassOfDevice { internal set; get; }

        internal PenDevice( BluetoothDeviceInfo device )
        {
            Name = device.DeviceName;
            Address = device.DeviceAddress.ToString();
            Authenticated = device.Authenticated;
            LastSeen = device.LastSeen;
            LastUsed = device.LastUsed;
            Remembered = device.Remembered;
            Rssi = device.Rssi;
            ClassOfDevice = device.ClassOfDevice.Value;
        }

        /// <summary>
        /// Returns a String that represents the current Object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return Address + "[" + Name + "]";
        }
    }

    /// <summary>
    /// Provides client connections for Bluetooth RFCOMM network services. 
    /// </summary>
    public class BluetoothAdapter
    {
        /// <summary>
        /// The delegate models a callback to get Class of Device when connection is established.
        /// </summary>
        /// <param name="deviceClass">Class of Device of the remote device</param>
        public delegate void OnConnected( uint deviceClass );

        private BluetoothClient mBtClient;

        private readonly string ALLOWED_MAC_PREFIX = "9C7BD";

        private readonly bool ALLOW_OTHER_CONNECTION = true;

        private object mConnLock = new object();

        /// <summary>
        /// Returns the Class of Device of the remote device.
        /// </summary>
        public uint DeviceClass { private set; get; }

        /// <summary>
        /// Gets a name of a device.
        /// </summary>
        public string DeviceName { private set; get; }

        /// <summary>
        /// Gets the device identifier.
        /// </summary>
        public string DeviceAddress { private set; get; }

        /// <summary>
        /// A constructor that constructs a BluetoothAdapter
        /// </summary>
        public BluetoothAdapter()
        {
        }

        /// <summary>
        /// Specifies whether the device is connected. 
        /// </summary>
        public bool Connected
        {
            get { return mBtClient != null && mBtClient.Connected; }
        }

        /// <summary>
        /// Specifies whether the bluetooth adapter is enabled. 
        /// </summary>
        public bool Enabled
        {
            get
            {
                try
                {
                    BluetoothClient bc = new BluetoothClient();
                    return true;
                }
                catch ( System.PlatformNotSupportedException )
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Discovers accessible Bluetooth devices, both remembered and in-range, and returns their names and addresses.  
        /// </summary>
        /// <returns>An array of PenDevice objects describing the devices discovered.</returns>
        public PenDevice[] FindAllDevices()
        {
            List<PenDevice> devices = new List<PenDevice>();

            try
            {
                BluetoothClient bc = new BluetoothClient();
                BluetoothDeviceInfo[] array = bc.DiscoverDevices();

                foreach ( BluetoothDeviceInfo device in array )
                {
                    if ( ValidateAddress( device.DeviceAddress.ToString() ) )
                    {
                        devices.Add( new PenDevice( device ) );
                    }
                }
            }
            catch ( System.PlatformNotSupportedException )
            {
                return null;
            }

            return devices.ToArray();
        }

        private bool ValidateAddress( string mac )
        {
            return mac.StartsWith( ALLOWED_MAC_PREFIX );
        }

        private uint FindDeviceClass( string address )
        {
            RegistryKey registryKey = Registry.CurrentUser.CreateSubKey( "Software" ).CreateSubKey( "NeoLAB PenSDK" ).CreateSubKey( "Devices" );
            
            foreach ( string addr in registryKey.GetSubKeyNames() )
            {
                if ( addr == address )
                {
                    RegistryKey devicekey = registryKey.OpenSubKey( address );

                    uint cod = Convert.ToUInt32( devicekey.GetValue( "COD", 0, RegistryValueOptions.None ) );

                    if ( cod != 0 )
                    {
                        return cod;
                    }
                }
            }

            return 0;
        }

        private void WriteDeviceClass( string address, uint deviceClass )
        {
            RegistryKey registryKey = Registry.CurrentUser.CreateSubKey( "Software" ).CreateSubKey( "NeoLAB PenSDK" ).CreateSubKey( "Devices" );

            registryKey.DeleteSubKey( address, false );
            
            RegistryKey devicekey = registryKey.CreateSubKey( address );

            devicekey.SetValue( "COD", deviceClass );

            devicekey.Flush();
            devicekey.Close();
        }

        /// <summary>
        /// Connects the client to a remote Bluetooth host using the specified mac address and OnConnected handler. 
        /// </summary>
        /// <param name="mac">The mac address of the remote host.</param>
        /// <param name="handler">The delegate to handle connecting event.</param>
        /// <returns>true if the BluetoothAdapter was connected to a remote resource; otherwise, false.</returns>
        public bool Connect( string mac, OnConnected handler )
        {
            BluetoothAddress bta = new BluetoothAddress( Convert.ToInt64( mac, 16 ) );
            BluetoothEndPoint rep = new BluetoothEndPoint( bta, BluetoothService.SerialPort );

            lock ( mConnLock )
            {
                try
                {
                    if ( ( Connected && !ALLOW_OTHER_CONNECTION ) || !ValidateAddress( mac ) )
                    {
                        return false;
                    }

                    Disconnect();

                    RemovePairedDevice( mac );

                    EventHandler<BluetoothWin32AuthenticationEventArgs> authHandler = new EventHandler<BluetoothWin32AuthenticationEventArgs>( handleRequests );
                    BluetoothWin32Authentication authenticator = new BluetoothWin32Authentication( authHandler );
                    
                    // 페어링 요청
                    BluetoothSecurity.PairRequest( bta, null );
                    
                    mBtClient = new BluetoothClient();
                    mBtClient.Connect( rep );

                    authenticator.Dispose();

                    DeviceClass = DeviceClass != 0 ? DeviceClass : FindDeviceClass( mac );

                    if ( DeviceClass == 0 )
                    {
                        mBtClient.Dispose();
                        return false;
                    }

                    WriteDeviceClass( mac, DeviceClass );

                    DeviceAddress = mac;

                    handler( DeviceClass );
                }
                catch
                {
                    return false;
                }

                return true;
            }
        }

        private void handleRequests( Object thing, BluetoothWin32AuthenticationEventArgs args )
        {
            System.Console.WriteLine( "BluetoothWin32AuthenticationEventArgs ( AuthenticationMethod : {0}, AuthenticationRequirements : {1}, NumberOrPasskey : {2}, COD : {3:X} )", args.AuthenticationMethod, args.AuthenticationRequirements, args.NumberOrPasskey, args.Device.ClassOfDevice.Value );
            DeviceClass = args.Device.ClassOfDevice.Value;
            DeviceName = args.Device.DeviceName;
            //args.Confirm = true;
        }

        /// <summary>
        /// Closes the Bluetooth socket and the underlying connection. 
        /// </summary>
        /// <returns>true if the BluetoothAdapter was disconnected to a remote resource; otherwise, false.</returns>
        public bool Disconnect()
        {
            lock ( mConnLock )
            {
                if ( Connected )
                {
                    mBtClient.Close();
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Remove the pairing with the specified device.
        /// </summary>
        /// <param name="addr">Remote device's mac address with which to remove pairing.</param>
        public void RemovePairedDevice( string addr )
        {
            BluetoothAddress bta = new BluetoothAddress( Convert.ToInt64( addr, 16 ) );
            BluetoothSecurity.RemoveDevice( bta );
        }

        /// <summary>
        /// Bind the bluetooth socket with IPenComm class.
        /// </summary>
        /// <param name="comm">Provides data handling for Neosmartpen Devices. </param>
        /// <param name="name">The name of IPenComm instance </param>
        public void Bind( IPenComm comm, string name = null )
        {
            comm.Bind( mBtClient.Client, name == null ? DeviceAddress : name );
        }
    }
}
