using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.System.Threading;

namespace Neosmartpen.Net.Bluetooth.Le
{
    /// <summary>
    /// It provides client connectivity for Bluetooth Le network services.
    /// </summary>
    public class BluetoothLePenClient : IPenClient
    {
        #region Constants
        public static readonly Guid OldServiceUuidV2 = new Guid(0x19f1, 0x00, 0x1000, 0x80, 0x00, 0x00, 0x80, 0x5F, 0x9B, 0x34, 0xFB);
        public static readonly Guid OldWriteCharacteristicsUuidV2 = new Guid(0x2BA0, 0x00, 0x1000, 0x80, 0x00, 0x00, 0x80, 0x5F, 0x9B, 0x34, 0xFB);
        public static readonly Guid OldIndicateCharacteristicsUuidV2 = new Guid(0x2BA1, 0x00, 0x1000, 0x80, 0x00, 0x00, 0x80, 0x5F, 0x9B, 0x34, 0xFB);

        public static readonly Guid ServiceUuidV2 = new Guid("4f99f138-9d53-5bfa-9e50-b147491afe68");
        public static readonly Guid WriteCharacteristicsUuidV2 = new Guid("8bc8cc7d-88ca-56b0-af9a-9bf514d0d61a");
        public static readonly Guid IndicateCharacteristicsUuidV2 = new Guid("64cd86b1-2256-5aeb-9f04-2caf6c60ae57");

        public static readonly string ALLOWED_MAC_PREFIX = "9C:7B:D";
        #endregion

        #region Variables
        /// <summary>
        /// Indicates whether the pen is currently connected.
        /// </summary>
        public bool Alive
        {
            get
            {
				return gattDeviceService?.Session?.SessionStatus == GattSessionStatus.Active && PenController?.Protocol != Protocols.NONE;
            }
            internal set { }
        }

        /// <summary>
        /// It provides client connectivity for common Bluetooth network services.
        /// </summary>
        public IPenController PenController { get; set; }

        public string Name { get; set; }

        public BluetoothLePenClient(IPenController penctrl)
        {
            PenController = penctrl;
        }

        private SemaphoreSlim semaphreSlime = new SemaphoreSlim(1, 1);

        private BluetoothLEDevice bluetoothLEDevice;
        private GattDeviceService gattDeviceService;
        private GattCharacteristic writeCharacteristic;
        private GattCharacteristic indicateCharacteristic;
        private ushort mtuSize;

        private Task writeTask;

        private AutoResetEvent autoResetEvent = new AutoResetEvent(false);
        #endregion

        /// <summary>
        /// Try to connect with the pen.
        /// </summary>
        /// <param name="penInformation">PenInformation instance that holds the pen's information</param>
        /// <returns>True or false if the connection is successful</returns>
        public async Task<bool> Connect(PenInformation penInformation)
        {
            try
            {
                if (penInformation.Protocol != Protocols.V2)
                    throw new NotSupportedException("Not supported protocol version");

                await semaphreSlime.WaitAsync();

                Debug.WriteLine("");

                bluetoothLEDevice = await BluetoothLEDevice.FromIdAsync(penInformation.Id);

                var status = await bluetoothLEDevice.RequestAccessAsync();
                Debug.WriteLine("RequestAccessAsync result is " + status.ToString());

                if (status != Windows.Devices.Enumeration.DeviceAccessStatus.Allowed)
                {
                    throw new Exception();
                }

                GattDeviceServicesResult result = await bluetoothLEDevice.GetGattServicesAsync(BluetoothCacheMode.Uncached);

                if (result.Status != GattCommunicationStatus.Success || await Bind(result.Services) == false)
                {
                    Debug.WriteLine("GetGattServicesAsync status is " + result.Status.ToString());
                    throw new Exception();
                }

                bluetoothLEDevice.ConnectionStatusChanged += BluetoothLEDevice_ConnectionStatusChanged;

                return true;
            }
            catch
            {
                DisposeBluetoothResource();
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
                    return await Connect(penInfo);

                return false;
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
            try
            {
                PenInformation penInfo = null;

                var bleAdWatcher = CreateLEAdvertisementWatcher();

                bleAdWatcher.Received += async (BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args) =>
                {
                    try
                    {
                        if (args.AdvertisementType == BluetoothLEAdvertisementType.ConnectableUndirected)
                        {
                            int protocol = Protocols.NONE;
                            if (args.Advertisement.ServiceUuids.Contains(BluetoothLePenClient.OldServiceUuidV2))
                                protocol = Protocols.V2;
                            else if (args.Advertisement.ServiceUuids.Contains(BluetoothLePenClient.ServiceUuidV2))
                                protocol = Protocols.V2;
                            else if (args.Advertisement.ServiceUuids.Contains(BluetoothPenClient.ServiceUuidV1))
                                protocol = Protocols.V1;

                            if (penInfo != null || protocol == Protocols.NONE)
                            {
                                return;
                            }

                            BluetoothLEDevice bleDevice = await BluetoothLEDevice.FromBluetoothAddressAsync(args.BluetoothAddress);

                            string mac = string.Empty;

                            foreach (var data in args.Advertisement.DataSections)
                            {
                                if (data.DataType == 0xFF)
                                {
                                    mac = BufferToMacAddress(data.Data);
                                    break;
                                }
                            }

                            if (mac.ToLower() == macAddress.ToLower())
                            {
                                penInfo = PenInformation.Create(bleDevice.DeviceInformation, bleDevice.Name, args.BluetoothAddress, mac, args.RawSignalStrengthInDBm, protocol, true);

                                bleAdWatcher?.Stop();
                                bleAdWatcher = null;
                            }
                        }
                    }
                    catch
                    {
                    }
                };

                bleAdWatcher.Stopped += (BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementWatcherStoppedEventArgs args) =>
                {
                    bleAdWatcher?.Stop();
                    bleAdWatcher = null;

                    autoResetEvent.Set();
                };

                bleAdWatcher.Start();

                ThreadPoolTimer.CreateTimer((ThreadPoolTimer timer) => {
                    bleAdWatcher?.Stop();
                    bleAdWatcher = null;
                }, TimeSpan.FromSeconds(5));

                autoResetEvent.WaitOne();

                if (penInfo == null || penInfo.Protocol == Protocols.NONE)
                    return null;

                return penInfo;
            }
            catch
            {
                return null;
            }
        }

        private void DisposeBluetoothResource()
        {
            if (indicateCharacteristic != null)
            {
                indicateCharacteristic.ValueChanged -= IndicateCharacteristic_ValueChanged;
                try
                {
                    indicateCharacteristic?.Service?.Dispose();
                }
                catch { }
            }

            try
            {
                gattDeviceService?.Dispose();
            }
            catch { }

            if (bluetoothLEDevice != null)
            {
                bluetoothLEDevice.ConnectionStatusChanged -= BluetoothLEDevice_ConnectionStatusChanged;
                try
                {
                    bluetoothLEDevice?.Dispose();
                }
                catch { }
            }

            writeCharacteristic = null;
			indicateCharacteristic = null;
			gattDeviceService = null;
			bluetoothLEDevice = null;

			// abolutely necessary.
			GC.Collect();
			GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// Disconnect the pen.
        /// </summary>
        /// <returns>True if disconnected false if failed</returns>
        public async Task Disconnect()
        {
            await onDisconnect();
        }

        /// <summary>
        /// Bind the inquired GattDeviceService to send and receive data to and from the pen.
        /// (It is not used unless it is a special case.)
        /// </summary>
        /// <param name="gattDeviceServices">GattDeviceService found</param>
        /// <returns>True on success false on failure</returns>
        public async Task<bool> Bind(IReadOnlyList<GattDeviceService> gattDeviceServices)
        {
            Debug.WriteLine("Bind Try");
            try
            {
                foreach (var gds in gattDeviceServices)
                {
                    if (gds.Uuid.Equals(OldServiceUuidV2) || gds.Uuid.Equals(ServiceUuidV2))
                    {
                        gattDeviceService = gds;
                        mtuSize = gattDeviceService.Session.MaxPduSize;
                        var result = await gds.GetCharacteristicsAsync();
                        if (result.Status == GattCommunicationStatus.Success)
                        {
                            var characteristics = result.Characteristics;

                            await InitCharacteristics(characteristics);

                            if (writeCharacteristic != null && indicateCharacteristic != null)
                            {
                                writeTask = Task.Run(GattSendDataTask);
                                PenController.PenClient = this;
                                PenController.Protocol = Protocols.V2;
                                PenController.OnConnected();
                                return true;
                            }
                            else
                            {
                                // TODO error
                                Debug.WriteLine("Bind cannot init Characteristics");
                            }
                        }
                        else
                        {
                            // todo error
                            Debug.WriteLine("Bind GattCommunicationStatus is not Success");
                        }

                        break;
                    }
                }
                return false;
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Bind Exception occured : " + exp.Message);
                return false;
            }
        }

        private void BluetoothLEDevice_ConnectionStatusChanged(BluetoothLEDevice sender, object args)
        {
            Debug.WriteLine("BLE Status Change");
            Debug.WriteLine($"Status : {sender.ConnectionStatus}");
            switch (sender.ConnectionStatus)
            {
                case BluetoothConnectionStatus.Disconnected:
                    PenController.OnDisconnected();
                    break;
            }
        }

        private async Task InitCharacteristics(IReadOnlyList<GattCharacteristic> gattCharacteristics)
        {
            foreach (var c in gattCharacteristics)
            {
                if (c.Uuid.Equals(OldWriteCharacteristicsUuidV2) || c.Uuid.Equals(WriteCharacteristicsUuidV2))
                {
                    writeCharacteristic = c;
                }
                else if (c.Uuid.Equals(OldIndicateCharacteristicsUuidV2) || c.Uuid.Equals(IndicateCharacteristicsUuidV2))
                {
                    var cccdValue = GattClientCharacteristicConfigurationDescriptorValue.None;
                    if (c.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Indicate))
                    {
                        cccdValue = GattClientCharacteristicConfigurationDescriptorValue.Indicate;
                    }
                    else if (c.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify))
                    {
                        cccdValue = GattClientCharacteristicConfigurationDescriptorValue.Notify;
                    }

                    if (cccdValue != GattClientCharacteristicConfigurationDescriptorValue.None)
                    {
                        var status = await c.WriteClientCharacteristicConfigurationDescriptorAsync(cccdValue);
                        if (status == GattCommunicationStatus.Success)
                        {
                            c.ValueChanged += IndicateCharacteristic_ValueChanged;
                            indicateCharacteristic = c;
                        }
                    }
                }
            }
        }

        private void IndicateCharacteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var value = args.CharacteristicValue;

            //Debug.WriteLine($"Read data : {BitConverter.ToString(value.ToArray())}");
            //Debug.WriteLine($"Read data : {value.Length}");

            byte[] readBytes = new byte[value.Length];

            using (DataReader reader = DataReader.FromBuffer(value))
            {
                reader.ReadBytes(readBytes);
            }

            PenController.OnDataReceived(readBytes);
        }

        public void Read()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Releases communication-related resources allocated for disconnecting from the pen. (It is not used unless it is a special case.)
        /// </summary>
        /// <returns>True on success false on failure</returns>
        public async Task Unbind()
		{
			semaphreSlime.Wait(3000);
            Debug.WriteLine("Unbind Try");
            try
			{
                try
                {
                    if (bluetoothLEDevice?.ConnectionStatus == BluetoothConnectionStatus.Connected)
                    {
                        var result = await indicateCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.None);
                    }
                }
                catch { }

                DisposeBluetoothResource();

                writeDataQueue.Add(new byte[0]);

                Debug.WriteLine("Unbind Finished");
            }
			catch
            {
                Debug.WriteLine("Unbind Exception");
            }
			finally
			{
				semaphreSlime.Release();
			}
		}

		private async Task<bool> WriteCharacteristic(byte[] data)
        {
            if (!Alive)
                return false;
            try
            {
                //Debug.WriteLine($"Write data : {BitConverter.ToString(data)}");
                var result = await writeCharacteristic.WriteValueAsync(data.AsBuffer());
                if (result == GattCommunicationStatus.Success)
                {
                    return true;
                }
                else
                {
                    Debug.WriteLine($"Write Characteristic Error : {result}");
                    return false;
                }
            }
            catch (Exception exp)
            {
                Debug.WriteLine($"writeCharacteristic exception {exp.Message}\n[{exp.StackTrace}]");
                return false;
            }
        }

        private BlockingCollection<byte[]> writeDataQueue = new BlockingCollection<byte[]>();
        private int writeIndex;
        private async Task GattSendDataTask()
        {
            // 해당 Thread를 Alive로 해두었으나, 위험할 수도있다는 생각이 든다. 
            // 해당 부분을 thread live용 변수를 따로 두는것에 대하여 생각을 해봐야할듯하다.
            while (Alive)
            {
                var data = writeDataQueue.Take();
                if (data == null || data.Length <= 0)
                    continue;

                if (mtuSize - 3 <= 0)
                {
                    Debug.WriteLine("mtu size is too small");
                    continue;
                }

                writeIndex = 0;
                while (writeIndex < data.Length)
                {
                    var partialData = data.Skip(writeIndex).Take(mtuSize - 3).ToArray();
                    writeIndex += (mtuSize - 3);
                    if (await WriteCharacteristic(partialData) == false)
                    {
                        Debug.WriteLine("write failed");
                        break;
                    }
                }
            }

            Debug.WriteLine("SendDataTask Finished");
        }

        public void Write(byte[] data)
        {
            writeDataQueue.Add(data);
        }

        private async Task onDisconnect()
        {
            PenController.OnDisconnected();
        }

        #region Advertisement Watcher
        public event TypedEventHandler<IPenClient, PenInformation> onAddPenController;
        public event TypedEventHandler<IPenClient, PenUpdateInformation> onUpdatePenController;
        //public event TypedEventHandler<BluetoothLePenClient, PenUpdateInformation> onRemovePenController;
        public event TypedEventHandler<IPenClient, BluetoothError> onStopSearch;

        private BluetoothLEAdvertisementWatcher bluetoothLEAdvertisementWatcher;
        List<PenInformation> penList = new List<PenInformation>();

        private BluetoothLEAdvertisementWatcher CreateLEAdvertisementWatcher()
        {
            var bluetoothLEAdvertisementWatcher = new BluetoothLEAdvertisementWatcher();

            //bluetoothLEAdvertisementWatcher.AdvertisementFilter.Advertisement.ServiceUuids.Add(MakeServiceUuid(0x18F1));
            //bluetoothLEAdvertisementWatcher.AdvertisementFilter.Advertisement.ServiceUuids.Add(MakeServiceUuid(0x19F1));

            bluetoothLEAdvertisementWatcher.SignalStrengthFilter.InRangeThresholdInDBm = -70;
            bluetoothLEAdvertisementWatcher.SignalStrengthFilter.OutOfRangeThresholdInDBm = -75;
            bluetoothLEAdvertisementWatcher.SignalStrengthFilter.OutOfRangeTimeout = TimeSpan.FromMilliseconds(2000);
            bluetoothLEAdvertisementWatcher.ScanningMode = BluetoothLEScanningMode.Active;

            return bluetoothLEAdvertisementWatcher;
        }

        private void InitLEAdvertisementWatcher()
        {
            if (bluetoothLEAdvertisementWatcher == null)
                bluetoothLEAdvertisementWatcher = CreateLEAdvertisementWatcher();

            bluetoothLEAdvertisementWatcher.Received += BluetoothLEAdvertisementWatcher_Received;
            bluetoothLEAdvertisementWatcher.Stopped += BluetoothLEAdvertisementWatcher_Stopped;
        }

        private void DeleteLEAdvertisementWatcher()
        {
            if (bluetoothLEAdvertisementWatcher == null)
                return;
            bluetoothLEAdvertisementWatcher.Received -= BluetoothLEAdvertisementWatcher_Received;
            bluetoothLEAdvertisementWatcher.Stopped -= BluetoothLEAdvertisementWatcher_Stopped;
        }

        /// <summary>
        /// Start searching for Bluetooth Le advertisements.
        /// </summary>
        public void StartLEAdvertisementWatcher()
        {
            InitLEAdvertisementWatcher();
            if (bluetoothLEAdvertisementWatcher?.Status != BluetoothLEAdvertisementWatcherStatus.Started)
            {
                penList.Clear();
                bluetoothLEAdvertisementWatcher.Start();
            }
        }

        /// <summary>
        /// Stop searching for Bluetooth Le advertisements.
        /// </summary>
        public void StopLEAdvertisementWatcher()
        {
            if (bluetoothLEAdvertisementWatcher == null)
                return;

            if (bluetoothLEAdvertisementWatcher?.Status != BluetoothLEAdvertisementWatcherStatus.Stopped)
            {
                penList.Clear();
                bluetoothLEAdvertisementWatcher.Stop();
            }
        }

        private void BluetoothLEAdvertisementWatcher_Stopped(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementWatcherStoppedEventArgs args)
        {
            onStopSearch?.Invoke(this, args.Error);
            DeleteLEAdvertisementWatcher();
        }

        object deviceLock = new object();

        private async void BluetoothLEAdvertisementWatcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            try
            {
                if (args.AdvertisementType == BluetoothLEAdvertisementType.ConnectableUndirected)
                {
                    int protocol = Protocols.NONE;
                    if (args.Advertisement.ServiceUuids.Contains(BluetoothLePenClient.OldServiceUuidV2))
                        protocol = Protocols.V2;
                    else if (args.Advertisement.ServiceUuids.Contains(BluetoothLePenClient.ServiceUuidV2))
                        protocol = Protocols.V2;
                    else if (args.Advertisement.ServiceUuids.Contains(BluetoothPenClient.ServiceUuidV1))
                        protocol = Protocols.V1;

                    // mac address를 안쓰고 virtual mac address를 쓴이유는 advertisement는 굉장히 많이 들어오는 값이기 떄문에 좀더 빠르게 검색하기 위해
                    //if (penList.Exists(x => x.virtualMacAddress == args.BluetoothAddress))
                    //	return;

                    var penInformation = penList.Where(x => x.VirtualMacAddress == args.BluetoothAddress).FirstOrDefault();
                    if (penInformation == null)
                    {
                        BluetoothLEDevice bleDevice = await BluetoothLEDevice.FromBluetoothAddressAsync(args.BluetoothAddress);
                        string mac = string.Empty;
                        foreach (var data in args.Advertisement.DataSections)
                        {
                            if (data.DataType == 0xFF)
                            {
                                mac = BufferToMacAddress(data.Data);
                                break;
                            }
                        }

                        if (ValidateMacAddress(mac))
                        {
                            lock (deviceLock)
                            {
                                if (penList.Where(x => x.VirtualMacAddress == args.BluetoothAddress).Count() <= 0)
                                {
                                    penInformation = PenInformation.Create(bleDevice.DeviceInformation, bleDevice.Name, args.BluetoothAddress, mac, args.RawSignalStrengthInDBm, protocol, true);
                                    penList.Add(penInformation);
                                    onAddPenController(this, penInformation);
                                }
                            }
                        }
                    }
                    else
                    {
                        BluetoothLEDevice bleDevice = await BluetoothLEDevice.FromBluetoothAddressAsync(args.BluetoothAddress);
                        var update = PenUpdateInformation.Create(bleDevice?.DeviceId, true);
                        update.Rssi = args.RawSignalStrengthInDBm;

                        onUpdatePenController?.Invoke(this, update);
                    }
                }
                else if (args.AdvertisementType == BluetoothLEAdvertisementType.ScanResponse)
                {
                    // ScanResponse에 name이 추가정보로 오는데 DeviceInformaion을 가져오면 Name을 알 수 있기 때문에 추가로 받아 처리할 필요는 아직까진 없다.
                    var penInformation = penList.Where(x => x.VirtualMacAddress == args.BluetoothAddress).FirstOrDefault();
                    if (penInformation == null)
                        return;

                    BluetoothLEDevice bleDevice = await BluetoothLEDevice.FromBluetoothAddressAsync(args.BluetoothAddress);

                    var update = PenUpdateInformation.Create(bleDevice?.DeviceId, true);
                    update.Rssi = args.RawSignalStrengthInDBm;
                    if (!string.IsNullOrEmpty(bleDevice.Name))
                        update.ModelName = bleDevice.Name;
                    else
                    {
                        foreach (var data in args.Advertisement.DataSections)
                        {
                            if (data.DataType == 0x09)
                            {
                                var b = new byte[data.Data.Length];
                                using (var reader = DataReader.FromBuffer(data.Data))
                                {
                                    reader.ReadBytes(b);
                                }
                                update.ModelName = Encoding.UTF8.GetString(b);
                                break;
                            }
                        }
                    }

                    onUpdatePenController?.Invoke(this, update);
                }
            }
            catch { }
        }
        #endregion

        #region Utils
        public bool ValidateMacAddress(string mac)
        {
            if (string.IsNullOrEmpty(mac))
                return false;
            return mac.ToUpper().StartsWith(BluetoothLePenClient.ALLOWED_MAC_PREFIX);
        }

        public string BufferToMacAddress(IBuffer buffer)
        {
            string mac = string.Empty;
            var b = new byte[6];
            using (var reader = DataReader.FromBuffer(buffer))
            {
                reader.ReadBytes(b);
            }
            mac = BitConverter.ToString(b);
            mac = mac.Replace('-', ':');

            return mac;
        }
        #endregion
    }
}
