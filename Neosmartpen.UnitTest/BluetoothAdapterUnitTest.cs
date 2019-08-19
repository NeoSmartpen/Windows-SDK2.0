using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neosmartpen.Net;
using Neosmartpen.Net.Bluetooth;
using Neosmartpen.Net.Metadata.Model;
using Neosmartpen.Net.Protocol.v2;
using System.Collections.Generic;
using System.Threading;

namespace Neosmartpen.UnitTest
{
    [TestClass]
    public class BluetoothAdapterUnitTest : PenCommV2Callbacks
    {
        private BluetoothAdapter _btAdt;

        private PenCommV2 _penComm;

        public const string MAC = "9C7BD2FFF10E";

        public const int TEST_TIMEOUT = 15000;

        [TestInitialize]
        public void SetUp()
        {
            _btAdt = new BluetoothAdapter();
            _penComm = new PenCommV2(this);
        }

        [TestCleanup]
        public void SetDown()
        {
            _btAdt.Disconnect();
        }

        public bool ConnectAndBind()
        {
            return _btAdt.Connect(MAC, delegate (uint deviceClass)
            {
                _btAdt.Bind(_penComm);
            });
        }

        [TestMethod]
        [Timeout(TEST_TIMEOUT)]
        public void TestConnectionAndBinding()
        {
            Assert.IsTrue(ConnectAndBind());
        }

        [TestMethod]
        [Timeout(TEST_TIMEOUT)]
        public void TestDisconnection()
        {
            ConnectAndBind();

            bool result = _btAdt.Disconnect();
            Assert.IsTrue(result);
        }

        [TestMethod]
        [Timeout(20000)]
        public void TestFindAllDevices()
        {
            var results = _btAdt.FindAllDevices();
            Assert.IsTrue(results != null && results.Length > 0);
        }

        #region callback

        void PenCommV2Callbacks.onConnected(IPenComm sender, string macAddress, string deviceName, string fwVersion, string protocolVersion, string subName, int maxForce)
        {
        }

        void PenCommV2Callbacks.onPenAuthenticated(IPenComm sender)
        {
        }

        void PenCommV2Callbacks.onDisconnected(IPenComm sender)
        {
        }

        void PenCommV2Callbacks.onReceiveDot(IPenComm sender, Dot dot, ImageProcessingInfo info)
        {
        }

        void PenCommV2Callbacks.onReceiveOfflineDataList(IPenComm sender, params OfflineDataInfo[] offlineNotes)
        {
        }

        void PenCommV2Callbacks.onStartOfflineDownload(IPenComm sender)
        {
        }

        void PenCommV2Callbacks.onFinishedOfflineDownload(IPenComm sender, bool result)
        {
        }

        void PenCommV2Callbacks.onRemovedOfflineData(IPenComm sender, bool result)
        {
        }

        void PenCommV2Callbacks.onReceivePenStatus(IPenComm sender, bool locked, int passwdMaxReTryCount, int passwdRetryCount, long timestamp, short autoShutdownTime, int maxForce, int battery, int usedmem, bool useOfflineData, bool autoPowerOn, bool penCapPower, bool hoverMode, bool beep, short penSensitivity, PenCommV2.UsbMode usbmode, bool downsampling, string btLocalName, PenCommV2.DataTransmissionType dataTransmissionType)
        {
        }

        void PenCommV2Callbacks.onPenPasswordRequest(IPenComm sender, int retryCount, int resetCount)
        {
        }

        void PenCommV2Callbacks.onPenPasswordSetUpResponse(IPenComm sender, bool result)
        {
        }

        void PenCommV2Callbacks.onPenOfflineDataSetUpResponse(IPenComm sender, bool result)
        {
        }

        void PenCommV2Callbacks.onPenTimestampSetUpResponse(IPenComm sender, bool result)
        {
        }

        void PenCommV2Callbacks.onPenSensitivitySetUpResponse(IPenComm sender, bool result)
        {
        }

        void PenCommV2Callbacks.onPenAutoShutdownTimeSetUpResponse(IPenComm sender, bool result)
        {
        }

        void PenCommV2Callbacks.onPenAutoPowerOnSetUpResponse(IPenComm sender, bool result)
        {
        }

        void PenCommV2Callbacks.onPenCapPowerOnOffSetupResponse(IPenComm sender, bool result)
        {
        }

        void PenCommV2Callbacks.onPenBeepSetUpResponse(IPenComm sender, bool result)
        {
        }

        void PenCommV2Callbacks.onPenHoverSetUpResponse(IPenComm sender, bool result)
        {
        }

        void PenCommV2Callbacks.onPenColorSetUpResponse(IPenComm sender, bool result)
        {
        }

        void PenCommV2Callbacks.onPenUsbModeSetUpResponse(IPenComm sender, bool result)
        {
        }

        void PenCommV2Callbacks.onPenDownSamplingSetUpResponse(IPenComm sender, bool result)
        {
        }

        void PenCommV2Callbacks.onPenBtLocalNameSetUpResponse(IPenComm sender, bool result)
        {
        }

        void PenCommV2Callbacks.onPenFscSensitivitySetUpResponse(IPenComm sender, bool result)
        {
        }

        void PenCommV2Callbacks.onPenDataTransmissionTypeSetUpResponse(IPenComm sender, bool result)
        {
        }

        void PenCommV2Callbacks.onPenBeepAndLightResponse(IPenComm sender, bool result)
        {
        }

        void PenCommV2Callbacks.onReceiveFirmwareUpdateStatus(IPenComm sender, int total, int amountDone)
        {
        }

        void PenCommV2Callbacks.onReceiveFirmwareUpdateResult(IPenComm sender, bool result)
        {
        }

        void PenCommV2Callbacks.onReceiveBatteryAlarm(IPenComm sender, int battery)
        {
        }

        void PenCommV2Callbacks.onErrorDetected(IPenComm sender, ErrorType errorType, long timestamp, Dot dot, string extraData, ImageProcessErrorInfo imageProcessErrorInfo)
        {
        }

        void PenCommV2Callbacks.onAvailableNoteAccepted(IPenComm sender, bool result)
        {
        }

        void PenCommV2Callbacks.onPenProfileReceived(IPenComm sender, PenProfileReceivedCallbackArgs args)
        {
        }

        void PenCommV2Callbacks.onSymbolDetected(IPenComm sender, List<Symbol> symbols)
        {
        }

        void PenCommV2Callbacks.onReceiveOfflineDataPageList(IPenComm sender, int section, int owner, int note, int[] pageNumbers)
        {
        }

        void PenCommV2Callbacks.onReceiveOfflineStrokes(IPenComm sender, int total, int amountDone, Stroke[] strokes, Symbol[] symbols)
        {
        }

        #endregion callback
    }
}
