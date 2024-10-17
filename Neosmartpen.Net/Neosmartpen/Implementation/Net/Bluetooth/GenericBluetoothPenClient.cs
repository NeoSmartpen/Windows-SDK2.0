using Neosmartpen.Net.Bluetooth.Le;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Radios;
using Windows.Foundation;
using Windows.Networking.Connectivity;

namespace Neosmartpen.Net.Bluetooth
{
    /// <summary>
    /// It provides client connectivity for common Bluetooth network services.
    /// (We recommend using this class under normal circumstances.)
    /// </summary>
    public class GenericBluetoothPenClient : IPenClient
    {
        private BluetoothPenClient classicPenClient;
        private BluetoothLePenClient lePenClient;

        /// <summary>
        /// It provides client connectivity for common Bluetooth network services.
        /// </summary>
        public IPenController PenController { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Indicates whether the pen is currently connected.
        /// </summary>
        public bool Alive
        {
            get
            {
                return lePenClient.Alive || classicPenClient.Alive;
            }
        }

        /// <summary>
        /// Indicates whether the currently connected pen is bluetooth low energy.
        /// </summary>
        public bool ConnectedDeviceIsLe
        {
            private set; get;
        }

        public GenericBluetoothPenClient(IPenController penCtrl)
        {
            PenController = penCtrl;
            classicPenClient = new BluetoothPenClient(penCtrl);
            lePenClient = new BluetoothLePenClient(penCtrl);

            lePenClient.onAddPenController += LePenClient_onAddPenController;
            lePenClient.onUpdatePenController += LePenClient_onUpdatePenController;
            lePenClient.onStopSearch += LePenClient_onStopSearch;

            ConnectedDeviceIsLe = true;

            NetworkInformation.NetworkStatusChanged += NetworkInformation_NetworkStatusChanged;
        }

        private void LePenClient_onStopSearch(IPenClient sender, BluetoothError args)
        {
            onStopSearch?.Invoke(this, args);
        }

        private void LePenClient_onUpdatePenController(IPenClient sender, PenUpdateInformation args)
        {
            onUpdatePenController?.Invoke(this, args);
        }

        private void LePenClient_onAddPenController(IPenClient sender, PenInformation args)
        {
            onAddPenController?.Invoke(this, args);
        }

        public void Dispose()
        {
            lePenClient.onAddPenController -= LePenClient_onAddPenController;
            lePenClient.onUpdatePenController -= LePenClient_onUpdatePenController;
            lePenClient.onStopSearch -= LePenClient_onStopSearch;

            NetworkInformation.NetworkStatusChanged -= NetworkInformation_NetworkStatusChanged;
        }

        /// <summary>
        /// Get the BluetoothAdapter object.
        /// </summary>
        /// <returns>instance of BluetoothAdapter</returns>
        public async Task<BluetoothAdapter> GetBluetoothAdapter()
        {
            BluetoothAdapter btAdapter = await BluetoothAdapter.GetDefaultAsync();
            if (btAdapter == null)
                return null;
            var radios = await Radio.GetRadiosAsync();
            if (radios.Count <= 0)
                return null;
            var bluetoothRadio = radios.Where(r => r.Kind == RadioKind.Bluetooth).FirstOrDefault();
            if (bluetoothRadio != null && bluetoothRadio.State == RadioState.On)
            {
                return btAdapter;
            }

            return null;
        }

        /// <summary>
        /// Indicates whether the system can use Bluetooth.
        /// </summary>
        /// <returns>True or false if Bluetooth is available</returns>
        public async Task<bool> GetBluetoothIsEnabledAsync()
        {
            BluetoothAdapter btAdapter = await BluetoothAdapter.GetDefaultAsync();
            if (btAdapter == null || !btAdapter.IsLowEnergySupported || !btAdapter.IsCentralRoleSupported)
                return false;
            var radios = await Radio.GetRadiosAsync();
            if (radios.Count <= 0)
                return false;
            var bluetoothRadio = radios.Where(r => r.Kind == RadioKind.Bluetooth).FirstOrDefault();
            return bluetoothRadio != null && bluetoothRadio.State == RadioState.On;
        }

        private async void NetworkInformation_NetworkStatusChanged(object sender)
        {
            if (!await GetBluetoothIsEnabledAsync())
            {
                await Disconnect();
                return;
            }
        }

        private SemaphoreSlim semaphreSlime = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Try to connect with the pen.
        /// </summary>
        /// <param name="penInformation">PenInformation instance that holds the pen's information</param>
        /// <returns>True or false if the connection is successful</returns>
        public async Task<bool> Connect(PenInformation penInformation)
        {
            try
            {
                await semaphreSlime.WaitAsync();

                if (penInformation == null || penInformation.Protocol == Protocols.NONE)
                    return false;

                if (penInformation.Protocol == Protocols.V2)
                {
                    ConnectedDeviceIsLe = true;
                    return await lePenClient.Connect(penInformation);
                }
                else
                {
                    ConnectedDeviceIsLe = false;
                    return await classicPenClient.Connect(penInformation.MacAddress);
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                semaphreSlime.Release();
            }
        }

        /// <summary>
        /// Try to connect with the pen in the bluetooth classic way.
        /// </summary>
        /// <param name="penInformation">PenInformation instance that holds the pen's information</param>
        /// <returns>True or false if the connection is successful</returns>
        public async Task<bool> ConnectByClassic(PenInformation penInformation)
        {
            try
            {
                await semaphreSlime.WaitAsync();

                if (penInformation == null || penInformation.Protocol == Protocols.NONE)
                    return false;
                ConnectedDeviceIsLe = false;
                return await classicPenClient.Connect(penInformation);
            }
            catch
            {
                return false;
            }
            finally
            {
                semaphreSlime.Release();
            }
        }

        /// <summary>
        /// Try to connect with the pen.
        /// </summary>
        /// <param name="macAddress">Mac address of the pen to connect</param>
        /// <returns>True or false if the connection is successful</returns>
        public async Task<bool> Connect(string macAddress)
        {
            try
            {
                await semaphreSlime.WaitAsync();

                PenInformation penInfo = GetPenInformationByAddress(macAddress);

                if (penInfo == null || penInfo.Protocol == Protocols.NONE)
                    return false;

                if (penInfo.Protocol == Protocols.V2)
                {
                    ConnectedDeviceIsLe = true;
                    return await lePenClient.Connect(penInfo);
                }
                else
                {
                    ConnectedDeviceIsLe = false;
                    return await classicPenClient.Connect(macAddress);
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                semaphreSlime.Release();
            }
        }

        /// <summary>
        /// Get the information of the pen to the pen's mac address.
        /// </summary>
        /// <param name="macAddress">Mac address of the pen to get the information.</param>
        /// <returns>PenInformation instance that holds the pen's information</returns>
        public PenInformation GetPenInformationByAddress(string macAddress)
        {
            return lePenClient.GetPenInformationByAddress(macAddress);
        }

        /// <summary>
        /// Disconnect the pen.
        /// </summary>
        /// <returns>True if disconnected false if failed</returns>
        public async Task Disconnect()
        {
            if (ConnectedDeviceIsLe)
            {
                await lePenClient.Disconnect();
            }
            else
            {
                classicPenClient.Disconnect();
            }
        }

        /// <summary>
        /// Discovers accessible smartpen device, both paired and in-range. 
        /// </summary>
        /// <returns>When this method completes successfully, it returns a List of PenInformation that represents the specified pen</returns>
        public async Task<List<PenInformation>> FindDevices()
        {
            return await classicPenClient.FindDevices();
        }

        /// <summary>
        /// To gets list of paired device
        /// </summary>
        /// <returns>When this method completes successfully, it returns a List of PenInformation that represents the specified pen</returns>
        public async Task<List<PenInformation>> FindPairedDevices()
        {
            return await classicPenClient.FindPairedDevices();
        }

        /// <summary>
        /// To gets list of unpaired device
        /// </summary>
        /// <returns>When this method completes successfully, it returns a List of PenInformation that represents the specified pen</returns>
        public async Task<PenInformation> FindUnpairedDevice(string macAddress)
        {
            return await classicPenClient.FindUnpairedDevice(macAddress);
        }

        /// <summary>
        /// To conduct unpairing with the specified device
        /// </summary>
        /// <param name="penInformation">The instance of PenInformation class</param>
        /// <returns>When this method completes successfully, it returns a boolean result</returns>
        public async Task<bool> UnPairing(PenInformation penInformation)
        {
            return await classicPenClient.UnPairing(penInformation);
        }

        public Task Unbind()
        {
            throw new NotImplementedException();
        }

        public void Write(byte[] data)
        {
            throw new NotImplementedException();
        }

        public void Read()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Notification for new Bluetooth LE advertisement events received.
        /// </summary>
        public event TypedEventHandler<IPenClient, PenInformation> onAddPenController;

        /// <summary>
        /// Notification for updated Bluetooth LE advertisement events received.
        /// </summary>
        public event TypedEventHandler<IPenClient, PenUpdateInformation> onUpdatePenController;

        /// <summary>
        /// Notification to the app that the Bluetooth LE scanning for advertisements has been cancelled or aborted either by the app or due to an error.
        /// </summary>
        public event TypedEventHandler<IPenClient, BluetoothError> onStopSearch;

        /// <summary>
        /// Start searching for Bluetooth Le advertisements.
        /// </summary>
        public void StartLEAdvertisementWatcher()
        {
            lePenClient.StartLEAdvertisementWatcher();
        }

        /// <summary>
        /// Stop searching for Bluetooth Le advertisements.
        /// </summary>
        public void StopLEAdvertisementWatcher()
        {
            lePenClient.StopLEAdvertisementWatcher();
        }
    }
}
