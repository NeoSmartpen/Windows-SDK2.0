using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Neosmartpen.Net.Bluetooth
{
    /// <summary>
    /// Provides client connections for Bluetooth RFCOMM network services. 
    /// </summary>
	public class BluetoothPenClient : PenClient
	{
		// sample
		//Bluetooth#Bluetooth00:09:dd:42:7b:a0-9c:7b:d2:ff:fb:5b#RFCOMM:00000000:{00001101-0000-1000-8000-00805f9b34fb}
		//BluetoothLE#BluetoothLE00:09:dd:42:7b:a0-d4:00:9c:d4:c2:18
		public static readonly uint ClassOfDeviceV1 = 0x0500;
        public static readonly uint ClassOfDeviceV2 = 0x2510;
        public static readonly Guid ServiceUuidV1 = new Guid(0x18f1, 0x00, 0x1000, 0x80, 0x00, 0x00, 0x80, 0x5F, 0x9B, 0x34, 0xFB);
        public static readonly Guid ServiceUuidV2 = new Guid(0x19f1, 0x00, 0x1000, 0x80, 0x00, 0x00, 0x80, 0x5F, 0x9B, 0x34, 0xFB);
		private readonly string ALLOWED_MAC_PREFIX = "9C:7B:D";

		public BluetoothPenClient(IPenController penctrl) : base(penctrl)
		{
		}

		~BluetoothPenClient()
		{
			DeleteWatcher();
			DeleteLEAdvertisementWatcher();
		}

		private StreamSocket streamSocket;

		private SemaphoreSlim semaphreSlime = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Connect the client to a remote bluetooth host using mac address
        /// </summary>
        /// <param name="macAddress">The mac address of the remote host</param>
        /// <returns>When this method completes successfully, it returns a boolean result</returns>
		public async Task<bool> Connect(string macAddress)
		{
			List<PenInformation> penList = await FindPairedDevices();
			PenInformation find = penList.Find(x => x.MacAddress.ToUpper() == macAddress.ToUpper());

			if (find == null)
			{
				find = await FindUnpairedDevice(macAddress);
			}

			if (find == null)
			{
				return false;
			}

			return await Connect(find);
		}

        /// <summary>
        /// Connect the client to a remote bluetooth host using PenInformation instance
        /// </summary>
        /// <param name="penInformation">The instance of PenInformation class</param>
        /// <returns>When this method completes successfully, it returns a boolean result</returns>
		public async Task<bool> Connect(PenInformation penInformation)
		{
			try
			{
                // lock try 블럭 안으로 이동
                await semaphreSlime.WaitAsync();

				if (Alive)
				{
					return false;
				}

				bool ret = await Pairing(penInformation);
				if (ret == false)
				{
					return false;
				}

				// 소켓을 멤버 변수로 가지고 있게끔
				streamSocket = new StreamSocket();

				//BluetoothDevice bluetoothDevice = await BluetoothDevice.FromIdAsync(penInformation.deviceInformation.Id);
				// le의 deviceinformation id를 이용해 BluetoothDevice를 가져올 수 없기 때문에 이런식으로 우회함
				BluetoothDevice bluetoothDevice = await BluetoothDevice.FromBluetoothAddressAsync(penInformation.BluetoothAddress);

				if (bluetoothDevice == null)
				{
					return false;
				}

                //var rfcommServices = await bluetoothDevice.GetRfcommServicesForIdAsync(RfcommServiceId.SerialPort, BluetoothCacheMode.Uncached);
                var rfcommServices = await bluetoothDevice.GetRfcommServicesForIdAsync(RfcommServiceId.FromUuid(RfcommServiceId.SerialPort.Uuid), BluetoothCacheMode.Uncached);

                RfcommDeviceService chatService = null;

				if (rfcommServices.Services.Count > 0)
				{
					chatService = rfcommServices.Services[0];
				}
				else
				{
					return false;
				}

				await streamSocket.ConnectAsync(bluetoothDevice.HostName, chatService.ConnectionServiceName);

				// 여기가 좀 지저분함
				PenController.Protocol = penInformation.Protocol == Protocols.V2 || bluetoothDevice.ClassOfDevice.RawValue == ClassOfDeviceV2 ? Protocols.V2 : Protocols.V1;

				await Task.Delay(200);

				Bind(streamSocket);
			}
			catch (Exception ex)
			{
				switch ((uint)ex.HResult)
				{
					case (0x80070490): // ERROR_ELEMENT_NOT_FOUND
						return false;
					case (0x800710DF): // ERROR_DEVICE_NOT_AVAILABLE
						return false;
					default:
						Debug.WriteLine($"Exception : {ex.Message}");
						Debug.WriteLine($"Exception : {ex.StackTrace}");
						return false;
				}
			}
			finally
			{
				semaphreSlime.Release();
			}

			return true;
		}

        /// <summary>
        /// To conduct pairing with the specified device
        /// </summary>
        /// <param name="penInformation">The instance of PenInformation class</param>
        /// <returns>When this method completes successfully, it returns a boolean result</returns>
		public async Task<bool> Pairing(PenInformation penInformation)
		{
			if (penInformation.deviceInformation == null)
				return false;

			var result = await penInformation.deviceInformation.Pairing.PairAsync();
			switch (result.Status)
			{
				case DevicePairingResultStatus.Paired:
				case DevicePairingResultStatus.AlreadyPaired:
					return true;
				//case DevicePairingResultStatus.AccessDenied:
				//case DevicePairingResultStatus.AuthenticationFailure:
				//case DevicePairingResultStatus.AuthenticationNotAllowed:
				//case DevicePairingResultStatus.AuthenticationTimeout:
				//case DevicePairingResultStatus.ConnectionRejected:
				//case DevicePairingResultStatus.Failed:
				//case DevicePairingResultStatus.HardwareFailure:
				//case DevicePairingResultStatus.InvalidCeremonyData:
				//case DevicePairingResultStatus.NoSupportedProfiles:
				//case DevicePairingResultStatus.NotPaired:
				//case DevicePairingResultStatus.NotReadyToPair:
				//case DevicePairingResultStatus.OperationAlreadyInProgress:
				//case DevicePairingResultStatus.PairingCanceled:
				//case DevicePairingResultStatus.ProtectionLevelCouldNotBeMet:
				//case DevicePairingResultStatus.RejectedByHandler:
				//case DevicePairingResultStatus.RemoteDeviceHasAssociation:
				//case DevicePairingResultStatus.RequiredHandlerNotRegistered:
				//case DevicePairingResultStatus.TooManyConnections:
				//	return false;
				default:
					return false;
			}
		}

        /// <summary>
        /// To conduct unpairing with the specified device
        /// </summary>
        /// <param name="penInformation">The instance of PenInformation class</param>
        /// <returns>When this method completes successfully, it returns a boolean result</returns>
		public async Task<bool> UnPairing(PenInformation penInformation)
		{
			if (penInformation.deviceInformation == null)
				return false;

			var result = await penInformation.deviceInformation.Pairing.UnpairAsync();
			switch (result.Status)
			{
				case DeviceUnpairingResultStatus.Unpaired:
				case DeviceUnpairingResultStatus.AlreadyUnpaired:
					return true;
				//case DeviceUnpairingResultStatus.AccessDenied:
				//case DeviceUnpairingResultStatus.Failed:
				//case DeviceUnpairingResultStatus.OperationAlreadyInProgress:
				//	return false;
				default:
					return false;
			}
		}

        /// <summary>
        /// Closes the Bluetooth socket and the underlying connection. 
        /// </summary>
		public void Disconnect()
		{
			semaphreSlime.Wait();

			// disconnect 할때 소켓을 릴리즈 함
			if (streamSocket != null)
			{
				streamSocket.Dispose();
				streamSocket = null;
			}

			semaphreSlime.Release();
		}

        /// <summary>
        /// Discovers accessible smartpen device, both paired and in-range. 
        /// </summary>
        /// <returns>When this method completes successfully, it returns a List of PenInformation that represents the specified pen</returns>
		public async Task<List<PenInformation>> FindDevices()
		{
			string selector = string.Format(SELECTOR, ProtocolBluetooth);
			return await SearchDevicesWithoutWatcher(selector);
		}

        /// <summary>
        /// To gets list of paired device
        /// </summary>
        /// <returns>When this method completes successfully, it returns a List of PenInformation that represents the specified pen</returns>
		public async Task<List<PenInformation>> FindPairedDevices()
		{
			return await SearchDevicesWithoutWatcher(BluetoothDevice.GetDeviceSelector());
		}
		public async Task<PenInformation> FindUnpairedDevice(string macAddress)
		{
			return await SearchDeviceWithoutWatcher(macAddress);
		}

		#region Find Device
		private readonly string FIND_UNPAIRED_SELECTOR = "System.Devices.DevObjectType:=5 AND System.Devices.Aep.ProtocolId:=\"{E0CBF06C-CD8B-4647-BB8A-263B43F0F974}\" AND (System.Devices.Aep.IsPaired:=System.StructuredQueryType.Boolean#False OR System.Devices.Aep.Bluetooth.IssueInquiry:=System.StructuredQueryType.Boolean#False)";

		private async Task<PenInformation> SearchDeviceWithoutWatcher(string macAddress)
		{
			string deviceSelector = FIND_UNPAIRED_SELECTOR + $" AND System.Devices.Aep.DeviceAddress:=\"{macAddress.ToUpper()}\"";
			var deviceInfo = await DeviceInformation.FindAllAsync(deviceSelector, RequestedProperties);
			var info = deviceInfo.FirstOrDefault();
			if (info == null)
				return null;

			object value;
			int rssi = 0;
			info.Properties.TryGetValue("System.Devices.Aep.SignalStrength", out value);
			if (value != null)
			{
				rssi = (int)value;
			}

            PenInformation penInfo = new PenInformation(info);
            penInfo.MacAddress = macAddress;
			penInfo.Rssi = rssi;
			return penInfo;
		}

		private async Task<List<PenInformation>> SearchDevicesWithoutWatcher(string deviceSelector)
		{
			if (penList == null)
				penList = new List<PenInformation>();
			penList.Clear();
			var deviceInfo = await DeviceInformation.FindAllAsync(deviceSelector, RequestedProperties);

			foreach (var info in deviceInfo)
			{
				if ( info != null)
				{
					object value;

					if (info.Properties != null && info.Properties.TryGetValue("System.Devices.Aep.DeviceAddress", out value))
					{
						if (ValidateMacAddress(value.ToString()))
						{
							object valueRssi;
							int rssi = 0;
							info.Properties.TryGetValue("System.Devices.Aep.SignalStrength", out valueRssi);
							if (valueRssi != null)
							{
								rssi = (int)valueRssi;
							}

							PenInformation penInfo = new PenInformation(info);
							penInfo.MacAddress = value.ToString();
							penInfo.Rssi = rssi;

							penList.Add(penInfo);
						}
					}
				}
			}

			return penList;
		}
		#endregion

		#region Device Watcher Function
		//private readonly string SELECTOR = "System.Devices.DevObjectType:=5 AND System.Devices.Aep.ProtocolId:=\"{0}\"";
		//System.Devices.DevObjectType:=5 AND System.Devices.Aep.ProtocolId:="{E0CBF06C-CD8B-4647-BB8A-263B43F0F974}"
		private readonly string SELECTOR = "System.Devices.DevObjectType:=5 AND System.Devices.Aep.ProtocolId:=\"{0}\"";
		private readonly string ProtocolBluetooth = "{E0CBF06C-CD8B-4647-BB8A-263B43F0F974}";

        private readonly string[] RequestedProperties = new string[] { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected", "System.Devices.Aep.SignalStrength" };

        private DeviceWatcher deviceWatcher = null;
		List<PenInformation> penList = new List<PenInformation>();
		private void CreateWatcher()
		{
			if (deviceWatcher == null)
			{
				string selector = string.Format(SELECTOR, ProtocolBluetooth);
				deviceWatcher = DeviceInformation.CreateWatcher(selector,
                                                                RequestedProperties,
																DeviceInformationKind.AssociationEndpoint);
			}

			deviceWatcher.Added += DeviceWatcher_Added;
			deviceWatcher.Removed += DeviceWatcher_Removed;
			deviceWatcher.Updated += DeviceWatcher_Updated;
			deviceWatcher.Stopped += DeviceWatcher_Stopped;
			deviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
		}
		private void DeleteWatcher()
		{
			if (deviceWatcher == null)
				return;
			deviceWatcher.Added -= DeviceWatcher_Added;
			deviceWatcher.Removed -= DeviceWatcher_Removed;
			deviceWatcher.Updated -= DeviceWatcher_Updated;
			deviceWatcher.Stopped -= DeviceWatcher_Stopped;
			deviceWatcher.EnumerationCompleted -= DeviceWatcher_EnumerationCompleted;
		}
		public void StartWatcher()
		{
			CreateWatcher();
			if (deviceWatcher?.Status != DeviceWatcherStatus.Started)
			{
				penList.Clear();
				deviceWatcher.Start();
			}
		}

		public void StopWatcher()
		{
			if (deviceWatcher == null)
				return;

			if (deviceWatcher.Status != DeviceWatcherStatus.Stopped)
			{
				deviceWatcher.Stop();
			}
		}

		public event TypedEventHandler<BluetoothPenClient, PenInformation> onAddPenController;
		public event TypedEventHandler<BluetoothPenClient, PenUpdateInformation> onUpdatePenController;
		public event TypedEventHandler<BluetoothPenClient, PenUpdateInformation> onRemovePenController;
		public event TypedEventHandler<BluetoothPenClient, BluetoothError> onStopSearch;

		// 람다식을 써도되지만 함수로 굳이 쓴 이유는 가독성
		private void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
		{
			string mac = GetMacAddressFromDeviceID(args.Id);
			if (penList.Exists(x => x.MacAddress == mac))
				return;
			if (ValidateMacAddress(mac))
			{
				object value;
				int rssi = 0;
				args.Properties.TryGetValue("System.Devices.Aep.SignalStrength", out value);
				if ( value != null)
				{
					rssi = (int)value;
				}
				PenInformation penInfo = new PenInformation(args);
				penInfo.MacAddress = mac;
				penInfo.Rssi = rssi;
				onAddPenController(this, penInfo);
			}
		}

		private void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
		{
			if (ValidateMacAddress(GetMacAddressFromDeviceID(args.Id)))
			{
				onRemovePenController(this, new PenUpdateInformation(args));
			}
		}

		private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
		{
			if (ValidateMacAddress(GetMacAddressFromDeviceID(args.Id)))
			{
				onUpdatePenController(this, new PenUpdateInformation(args));
			}
		}
		private void DeviceWatcher_Stopped(DeviceWatcher sender, object args)
		{
			onStopSearch(this, BluetoothError.Success);

			DeleteWatcher();
		}

		private void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
		{
			sender.Stop();
		}

		#endregion

		#region Advertisement Watcher

		private BluetoothLEAdvertisementWatcher bluetoothLEAdvertisementWatcher;
		private void CreateLEAdvertisementWatcher()
		{
			if (bluetoothLEAdvertisementWatcher == null)
				bluetoothLEAdvertisementWatcher = new BluetoothLEAdvertisementWatcher();

			bluetoothLEAdvertisementWatcher.AdvertisementFilter.Advertisement.ServiceUuids.Add(MakeServiceUuid(0x18F1));
			bluetoothLEAdvertisementWatcher.AdvertisementFilter.Advertisement.ServiceUuids.Add(MakeServiceUuid(0x19F1));

			bluetoothLEAdvertisementWatcher.SignalStrengthFilter.InRangeThresholdInDBm = -70;
			bluetoothLEAdvertisementWatcher.SignalStrengthFilter.OutOfRangeThresholdInDBm = -75;
			bluetoothLEAdvertisementWatcher.SignalStrengthFilter.OutOfRangeTimeout = TimeSpan.FromMilliseconds(2000);
			bluetoothLEAdvertisementWatcher.ScanningMode = BluetoothLEScanningMode.Active;

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

		private void StartLEAdvertisementWatcher()
		{
			if (bluetoothLEAdvertisementWatcher.Status != BluetoothLEAdvertisementWatcherStatus.Started)
			{
				penList.Clear();
				bluetoothLEAdvertisementWatcher.Start();
			}
		}

		private void StopLEAdvertisementWatcher()
		{
			if (bluetoothLEAdvertisementWatcher.Status != BluetoothLEAdvertisementWatcherStatus.Stopped)
			{
				penList.Clear();
				bluetoothLEAdvertisementWatcher.Stop();
			}
		}

		private void BluetoothLEAdvertisementWatcher_Stopped(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementWatcherStoppedEventArgs args)
		{
            onStopSearch(this, args.Error);
		}

		private async void BluetoothLEAdvertisementWatcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
		{
			if ( args.AdvertisementType == BluetoothLEAdvertisementType.ConnectableUndirected )
			{
				// 여기서 사용하는 uuid도 상수로 뺄지 고민해볼 필요있음
				if ( !args.Advertisement.ServiceUuids.Contains(ServiceUuidV1) && !args.Advertisement.ServiceUuids.Contains(ServiceUuidV2) )
				{
					return;
				}

				// mac address를 안쓰고 virtual mac address를 쓴이유는 advertisement는 굉장히 많이 들어오는 값이기 떄문에 좀더 빠르게 검색하기 위해
				if (penList.Exists(x => x.VirtualMacAddress == args.BluetoothAddress))
					return;

				BluetoothLEDevice bleDevice = await BluetoothLEDevice.FromBluetoothAddressAsync(args.BluetoothAddress);
				PenInformation penCtrl = new PenInformation(bleDevice.DeviceInformation);
				string mac = string.Empty;
				foreach ( var data in args.Advertisement.DataSections )
				{
					if ( data.DataType == 0xFF )
					{
						var b = new byte[data.Data.Length];
						using (var reader = DataReader.FromBuffer(data.Data))
						{
							reader.ReadBytes(b);
						}
						mac = BitConverter.ToString(b);
						mac.Replace('-', ':');
						break;
					}
				}

				// 현재 방식에서는 필요가 없어서 일단 제거해둠
				//Protocol protocol = Client.Protocol.PenCommV1;
				//if ( args.Advertisement.ServiceUuids.Contains(MakeServiceUuid(0x19f1)) )
				//{
				//	protocol = Client.Protocol.PenCommV2;
				//}
				//else if ( args.Advertisement.ServiceUuids.Contains(MakeServiceUuid(0x18f1)) )
				//{
				//	protocol = Client.Protocol.PenCommV1;
				//}
				//else { /* error */ }

				//penInfo.protocol = protocol;
				penCtrl.MacAddress = mac;
				penList.Add(penCtrl);
				onAddPenController(this, penCtrl);

			}
			else if ( args.AdvertisementType == BluetoothLEAdvertisementType.ScanResponse )
			{
				// ScanResponse에 name이 추가정보로 오는데 DeviceInformaion을 가져오면 Name을 알 수 있기 때문에 추가로 받아 처리할 필요는 아직까진 없다.
			}
		}
		#endregion

		#region Util
		private bool ValidateMacAddress(string mac)
		{
			if (string.IsNullOrEmpty(mac))
				return false;
			return mac.ToUpper().StartsWith(ALLOWED_MAC_PREFIX);
		}
		private string GetMacAddressFromDeviceID(string id)
		{
			return id.Substring(id.Length - 17);
		}
		private string MakeSericeName(string deviceID)
		{
            return deviceID + "{0}#RFCOMM:00000000:{00001101-0000-1000-8000-00805f9b34fb}";
		}

		private Guid MakeServiceUuid(int id)
		{
			// id를 제외한 부분은 fixed 임
			/*
			Guid uuid = new Guid(id, 0x00, 0x1000, 0x80, 0x00, 0x00, 0x80, 0x5F, 0x9B, 0x34, 0xFB);
			Debug.WriteLine($"Debug]] make uuid : {uuid}");
			return uuid;
			*/
			return new Guid(id, 0x00, 0x1000, 0x80, 0x00, 0x00, 0x80, 0x5F, 0x9B, 0x34, 0xFB);
		}
		#endregion
	}
}
