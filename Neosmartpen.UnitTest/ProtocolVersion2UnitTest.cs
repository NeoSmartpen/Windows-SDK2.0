using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neosmartpen.Net.Bluetooth;
using Neosmartpen.Net.Protocol.v2;
using Neosmartpen.Net.Support;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Neosmartpen.UnitTest
{
    [TestClass]
    public class ProtocolVersion2UnitTest
    {
        private BluetoothAdapter _btAdt;

        private PenCommV2 _penComm;

        private PenCommV2CallbacksImpl _callbackObj;

        private static AutoResetEvent _autoResetEvent = new AutoResetEvent(false);

        public const int TEST_TIMEOUT = 15000;

        public const string MAC = "9C7BD2EEE021";

        public const string PASSWORD = "1234";

        public const int DEFAULT_SECTION = 3;
        public const int DEFAULT_OWNER = 27;
        public const int DEFAULT_NOTE = 603;

        public const string FIRMWARE_FILEPATH = "E:\\vs_workplace\\WINSDK\\Neosmartpen.UnitTest\\NWP-F70_1.00.0105._v_";
        public const string FIRMWARE_VERSION = "1.01";

        [TestInitialize]
        public void SetUp()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            _btAdt = new BluetoothAdapter();

            _callbackObj = new PenCommV2CallbacksImpl();

            _callbackObj.Authenticated = delegate (object sender, object[] args)
            {
                _autoResetEvent.Set();
            };

            _callbackObj.PasswordRequest = delegate (object sender, object[] args)
            {
                _penComm.ReqInputPassword(PASSWORD);
            };

            _penComm = new PenCommV2(_callbackObj);

            bool result = _btAdt.Connect(MAC, delegate (uint deviceClass)
            {
                _btAdt.Bind(_penComm);
            });

            if (!result)
            {
                Assert.Fail("connection failed");
                return;
            }

            _autoResetEvent.WaitOne();
        }

        [TestCleanup]
        public void SetDown()
        {
            _callbackObj.Dispose();
            _btAdt.Disconnect();
        }

        #region offline data test

        [TestMethod]
        [Timeout(TEST_TIMEOUT)]
        public void TestOfflineDataList()
        {
            bool firstCheck = false;

            _callbackObj.OfflineDataListReceived = delegate (object sender, object[] args)
            {
                firstCheck = true;
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                _penComm.ReqOfflineDataList();
            });

            _autoResetEvent.WaitOne();

            bool secondCheck = false;

            _callbackObj.OfflineDataListReceived = delegate (object sender, object[] args)
            {
                secondCheck = true;
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                _penComm.ReqOfflineDataList(DEFAULT_SECTION, DEFAULT_OWNER);
            });

            _autoResetEvent.WaitOne();

            bool thirdCheck = false;

            _callbackObj.OfflineDataListReceived = delegate (object sender, object[] args)
            {
                thirdCheck = true;
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                _penComm.ReqOfflineDataList(DEFAULT_SECTION, DEFAULT_OWNER, DEFAULT_NOTE);
            });

            _autoResetEvent.WaitOne();

            Assert.IsTrue(firstCheck && secondCheck && thirdCheck);
        }

        [TestMethod]
        [Timeout(TEST_TIMEOUT)]
        public void TestOfflineDataRemove()
        {
            bool result = false;

            _callbackObj.OfflineDataRemoved = delegate (object sender, object[] args)
            {
                result = (bool)args[0];
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                _penComm.ReqRemoveOfflineData(DEFAULT_SECTION, DEFAULT_OWNER, new int[] { DEFAULT_NOTE });
            });

            _autoResetEvent.WaitOne();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestOfflineDataDownload()
        {
            bool result = false;

            _callbackObj.OfflineDataDownloadingFinished = (object sender, object[] args) =>
            {
                result = (bool)args[0];
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                _penComm.ReqOfflineData(DEFAULT_SECTION, DEFAULT_OWNER, DEFAULT_NOTE);
            });

            _autoResetEvent.WaitOne();

            Assert.IsTrue(result);
        }

        #endregion

        #region setting

        [TestMethod]
        [Timeout(TEST_TIMEOUT)]
        public void TestAutoPowerOnSetup()
        {
            bool result = false;

            _callbackObj.AutoPowerOnChanged = delegate (object sender, object[] args) 
            {
                result = (bool)args[0];
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                _penComm.ReqSetupPenAutoPowerOn(false);
            });

            _autoResetEvent.WaitOne();

            Assert.IsTrue(result);
        }

        [TestMethod]
        [Timeout(TEST_TIMEOUT)]
        public void TestAutoShutdownTimeSetup()
        {
            bool result = false;

            _callbackObj.AutoShutdownTimeChanged = delegate (object sender, object[] args)
            {
                result = (bool)args[0];
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                _penComm.ReqSetupPenAutoShutdownTime(20);
            });

            _autoResetEvent.WaitOne();

            Assert.IsTrue(result);
        }

        [TestMethod]
        [Timeout(TEST_TIMEOUT)]
        public void TestPenBeepAndLightSetup()
        {
            bool result = false;

            _callbackObj.PenBeepAndLightChanged = delegate (object sender, object[] args)
            {
                result = (bool)args[0];
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                _penComm.ReqBeepAndLight();
            });

            _autoResetEvent.WaitOne();

            Assert.IsTrue(result);
        }

        [TestMethod]
        [Timeout(TEST_TIMEOUT)]
        public void TestPenBeepSetup()
        {
            bool result = false;

            _callbackObj.PenBeepChanged = delegate (object sender, object[] args)
            {
                result = (bool)args[0];
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                _penComm.ReqSetupPenBeep(true);
            });

            _autoResetEvent.WaitOne();

            Assert.IsTrue(result);
        }

        [TestMethod]
        [Timeout(TEST_TIMEOUT)]
        public void TestPenBtLocalNameSetup()
        {
            bool result = false;

            _callbackObj.PenBtLocalNameChanged = delegate (object sender, object[] args)
            {
                result = (bool)args[0];
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                _penComm.ReqSetupBtLocalName("NeosmartpenLINE");
            });

            _autoResetEvent.WaitOne();

            Assert.IsTrue(result);
        }

        [TestMethod]
        [Timeout(TEST_TIMEOUT)]
        public void TestPenCapPowerOnOffSetup()
        {
            bool result = false;

            _callbackObj.PenCapPowerOnOffChanged = delegate (object sender, object[] args)
            {
                result = (bool)args[0];
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                _penComm.ReqSetupPenCapPower(true);
            });

            _autoResetEvent.WaitOne();

            Assert.IsTrue(result);
        }

        [TestMethod]
        [Timeout(TEST_TIMEOUT)]
        public void TestPenColorSetup()
        {
            bool result = false;

            _callbackObj.PenColorChanged = delegate (object sender, object[] args)
            {
                result = (bool)args[0];
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                _penComm.ReqSetupPenColor(0x00000000);
            });

            _autoResetEvent.WaitOne();

            Assert.IsTrue(result);
        }

        [TestMethod]
        [Timeout(TEST_TIMEOUT)]
        public void TestPenDataTransmissionTypeSetup()
        {
            bool result = false;

            _callbackObj.PenDataTransmissionTypeChanged = delegate (object sender, object[] args)
            {
                result = (bool)args[0];
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                _penComm.ReqSetupDataTransmissionType(PenCommV2.DataTransmissionType.Event);
            });

            _autoResetEvent.WaitOne();

            Assert.IsTrue(result);
        }

        [TestMethod]
        [Timeout(TEST_TIMEOUT)]
        public void TestPenDownSamplingSetup()
        {
            bool result = false;

            _callbackObj.PenDownSamplingChanged = delegate (object sender, object[] args)
            {
                result = (bool)args[0];
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                _penComm.ReqSetupDownSampling(false);
            });

            _autoResetEvent.WaitOne();

            Assert.IsTrue(result);
        }

        [TestMethod]
        [Timeout(TEST_TIMEOUT)]
        public void TestPenFscSensitivitySetup()
        {
            bool result = false;

            _callbackObj.PenFscSensitivityChanged = delegate (object sender, object[] args)
            {
                result = (bool)args[0];
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                _penComm.ReqSetupPenFscSensitivity(1);
            });

            _autoResetEvent.WaitOne();

            Assert.IsTrue(result);
        }

        [TestMethod]
        [Timeout(TEST_TIMEOUT)]
        public void TestPenOfflineDataSetup()
        {
            bool result = false;

            _callbackObj.PenOfflineDataChanged = delegate (object sender, object[] args)
            {
                result = (bool)args[0];
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                _penComm.ReqSetupOfflineData(true);
            });

            _autoResetEvent.WaitOne();

            Assert.IsTrue(result);
        }

        [TestMethod]
        [Timeout(TEST_TIMEOUT)]
        public void TestPenSensitivitySetup()
        {
            bool result = false;

            _callbackObj.PenSensitivityChanged = delegate (object sender, object[] args)
            {
                result = (bool)args[0];
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                _penComm.ReqSetupPenSensitivity(0);
            });

            _autoResetEvent.WaitOne();

            Assert.IsTrue(result);
        }

        [TestMethod]
        [Timeout(TEST_TIMEOUT)]
        public void TestPenTimestampSetup()
        {
            bool result = false;

            _callbackObj.PenTimestampChanged = delegate (object sender, object[] args)
            {
                result = (bool)args[0];
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                _penComm.ReqSetupTime(Time.GetUtcTimeStamp());
            });

            _autoResetEvent.WaitOne();

            Assert.IsTrue(result);
        }

        [TestMethod]
        [Timeout(TEST_TIMEOUT)]
        public void TestPenUsbModeSetup()
        {
            bool result = false;

            _callbackObj.PenUsbModeChanged = delegate (object sender, object[] args)
            {
                result = (bool)args[0];
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                _penComm.ReqSetupUsbMode(PenCommV2.UsbMode.Disk);
            });

            _autoResetEvent.WaitOne();

            Assert.IsTrue(result);
        }

        [TestMethod]
        [Timeout(TEST_TIMEOUT)]
        public void TestPenStatus()
        {
            bool result = false;

            _callbackObj.PenStatusReceived = delegate (object sender, object[] args) 
            {
                result = true;
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                _penComm.ReqPenStatus();
            });

            _autoResetEvent.WaitOne();

            Assert.IsTrue(result);
        }

        #endregion

        #region password

        [TestMethod]
        [Timeout(TEST_TIMEOUT)]
        public void TestSetUpPassword()
        {
            bool result1 = _penComm.ReqSetUpPassword(null);

            Assert.IsFalse(result1);

            bool result2 = _penComm.ReqSetUpPassword("0000");

            Assert.IsFalse(result2);

            bool result3 = _penComm.ReqSetUpPassword("0001", "0000");

            Assert.IsFalse(result3);

            bool firstChange = false;

            _callbackObj.PasswordChanged = delegate (object sender, object[] arg)
            {
                firstChange = true;
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                //1234로 비밀번호 변경
                _penComm.ReqSetUpPassword("", PASSWORD);
            });

            _autoResetEvent.WaitOne();

            bool secondChange = false;

            _callbackObj.PasswordChanged = delegate (object sender, object[] arg)
            {
                secondChange = true;
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                //비밀번호 삭제
                _penComm.ReqSetUpPassword(PASSWORD);
            });

            _autoResetEvent.WaitOne();

            Assert.IsTrue(firstChange && secondChange);
        }

        [TestMethod]
        [Timeout(TEST_TIMEOUT)]
        public void TestInputPassword()
        {
            bool result = false;
            
            _callbackObj.PasswordChanged = delegate (object sender, object[] args)
            {
                _btAdt.Disconnect();
                _autoResetEvent.Set();
            };

            _callbackObj.Authenticated = delegate (object sender, object[] args)
            {
                result = true;
                _autoResetEvent.Set();
            };

            _callbackObj.PasswordRequest = delegate (object sender, object[] args)
            {
                _penComm.ReqInputPassword(PASSWORD);
            };

            Task.Factory.StartNew(() =>
            {
                //1234로 비밀번호 변경
                _penComm.ReqSetUpPassword("", PASSWORD);
            });

            _autoResetEvent.WaitOne();

            bool connResult = _btAdt.Connect(MAC, delegate (uint deviceClass)
            {
                _btAdt.Bind(_penComm);
            });

            _autoResetEvent.WaitOne();

            Assert.IsTrue(result && connResult);
        }

        #endregion

        #region note

        [TestMethod]
        [Timeout(TEST_TIMEOUT)]
        public void TestAvailableNoteRequest()
        {
            bool firstCheck = false;

            _callbackObj.AvailableNoteAccepted = delegate (object sender, object[] args)
            {
                firstCheck = (bool)args[0];
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                _penComm.ReqAddUsingNote();
            });

            _autoResetEvent.WaitOne();

            bool secondCheck = false;

            _callbackObj.AvailableNoteAccepted = delegate (object sender, object[] args)
            {
                secondCheck = (bool)args[0];
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                _penComm.ReqAddUsingNote(DEFAULT_SECTION, DEFAULT_OWNER, new int[] { DEFAULT_NOTE });
            });

            _autoResetEvent.WaitOne();

            bool thirdCheck = false;

            _callbackObj.AvailableNoteAccepted = delegate (object sender, object[] args)
            {
                thirdCheck = (bool)args[0];
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                _penComm.ReqAddUsingNote(new int[] { DEFAULT_SECTION }, new int[] { DEFAULT_OWNER });
            });

            _autoResetEvent.WaitOne();

            Assert.IsTrue(firstCheck && secondCheck && thirdCheck);
        }

        #endregion

        #region fw update

        [TestMethod]
        [Ignore]
        public void TestUpdateFirmware()
        {
            Assert.IsTrue(TestUpdateCancel());
            Assert.IsTrue(TestUpdate());
        }

        private bool TestUpdate()
        {
            bool started = false;
            bool result = false;
            bool requestResult = false;

            _callbackObj.FirmwareUpdateProgressReceived = delegate (object sender, object[] args)
            {
                started = true;
                Debug.WriteLine(args[0] + " / " + args[1]);
            };

            _callbackObj.FirmwareUpdateResultReceived = delegate (object sender, object[] args)
            {
                result = (bool)args[0];
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                requestResult = _penComm.ReqPenSwUpgrade(FIRMWARE_FILEPATH, FIRMWARE_VERSION);

                if (!requestResult)
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            return requestResult && started && result;
        }

        private bool TestUpdateCancel()
        {
            bool result = false;
            bool requestResult = false;

            _callbackObj.FirmwareUpdateProgressReceived = delegate (object sender, object[] args)
            {
                _penComm.SuspendSwUpgrade();
            };

            _callbackObj.FirmwareUpdateResultReceived = delegate (object sender, object[] args)
            {
                result = (bool)args[0];
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                requestResult = _penComm.ReqPenSwUpgrade(FIRMWARE_FILEPATH, FIRMWARE_VERSION);

                if (!requestResult)
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            return requestResult && !result;
        }

        #endregion

        #region pen profile

        public readonly static string PROFILE_NAME = "neolab_t";
        public readonly static string PROFILE_NAME_LONG = "aaaaaaaaaaaa";
        public readonly static string PROFILE_NAME_INVALID = "abcd";

        public readonly static byte[] PROFILE_PASS = new byte[] { 0x3E, 0xD5, 0x95, 0x25, 0x06, 0xF7, 0x83, 0xDD };
        public readonly static byte[] PROFILE_PASS_LONG = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        public readonly static byte[] PROFILE_PASS_INVALID = new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        public readonly static string[] PROFILE_VALUE_KEYS = new string[] { "harry", "sally" };
        public readonly static string[] PROFILE_VALUE_KEYS_INVALID = new string[] { "john", "doe" };
        public readonly static byte[][] PROFILE_VALUE_VALUES = new byte[][] { Encoding.UTF8.GetBytes("harrys password"), Encoding.UTF8.GetBytes("sally password") };

        [TestMethod]
        public void TestPenProfile()
        {
            // 프로파일 생성
            Assert.IsTrue(PenProfileCreateParamNullTest());
            Assert.IsTrue(PenProfileCreateParamLongTest());
            Assert.IsTrue(PenProfileCreatePermissionDeniedTest());
            Assert.IsTrue(PenProfileCreateSuccessTest());
            Assert.IsTrue(PenProfileCreateAlreadyExistsTest());

            // 프로파일 조회
            Assert.IsTrue(PenProfileInfoParamTest());
            Assert.IsTrue(PenProfileInfoTest());

            // 프로파일 값 생성
            Assert.IsTrue(PenProfileWriteValueParamNullTest());
            Assert.IsTrue(PenProfileWriteValueParamLongTest());
            Assert.IsTrue(PenProfileWriteValuePermissionDeniedTest());
            Assert.IsTrue(PenProfileWriteValueSuccessTest());

            // 프로파일 값 조회
            Assert.IsTrue(PenProfileReadValueParamTest());
            Assert.IsTrue(PenProfileReadValueTest());

            // 프로파일 값 삭제
            Assert.IsTrue(PenProfileDeleteValueParamNullTest());
            Assert.IsTrue(PenProfileDeleteValueParamLongTest());
            Assert.IsTrue(PenProfileDeleteValueInvalidPasswordTest());
            Assert.IsTrue(PenProfileDeleteValueProfileNotExistsTest());
            Assert.IsTrue(PenProfileDeleteValueNotExistsTest());
            Assert.IsTrue(PenProfileDeleteValueSuccessTest());

            // 프로파일 삭제
            Assert.IsTrue(PenProfileDeleteParamNullTest());
            Assert.IsTrue(PenProfileDeleteParamLongTest());
            Assert.IsTrue(PenProfileDeleteInvalidPasswordTest());    
            Assert.IsTrue(PenProfileDeleteSuccessTest());
            Assert.IsTrue(PenProfileDeleteNameNotExistsTest());
        }

        #region pen profile create test

        private bool PenProfileCreateParamNullTest()
        {
            bool resultPassNull = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.CreateProfile(PROFILE_NAME, null);
                }
                catch (ArgumentNullException)
                {
                    resultPassNull = true;
                    _autoResetEvent.Set();
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            bool resultNameNull = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.CreateProfile(null, PROFILE_PASS);
                }
                catch (ArgumentNullException)
                {
                    resultNameNull = true;
                    _autoResetEvent.Set();
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            return resultPassNull && resultNameNull;
        }

        private bool PenProfileCreateParamLongTest()
        {
            bool resultNameLong = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.CreateProfile(PROFILE_NAME_LONG, PROFILE_PASS);
                }
                catch (ArgumentOutOfRangeException)
                {
                    resultNameLong = true;
                    _autoResetEvent.Set();
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            bool resultPassLong = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.CreateProfile(PROFILE_NAME, PROFILE_PASS_LONG);
                }
                catch (ArgumentOutOfRangeException)
                {
                    resultPassLong = true;
                    _autoResetEvent.Set();
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            return resultNameLong && resultPassLong;
        }

        private bool PenProfileCreatePermissionDeniedTest()
        {
            bool result = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                PenProfileReceivedCallbackArgs arg = args[0] as PenProfileReceivedCallbackArgs;

                if (arg.Status == PenProfile.PROFILE_STATUS_NO_PERMISSION)
                {
                    result = true;
                }

                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.CreateProfile(PROFILE_NAME, PROFILE_PASS_INVALID);
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            return result;
        }

        private bool PenProfileCreateSuccessTest()
        {
            bool result = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                PenProfileReceivedCallbackArgs arg = args[0] as PenProfileReceivedCallbackArgs;

                if (arg.Result == PenProfileReceivedCallbackArgs.ResultType.Success && arg.Status == PenProfile.PROFILE_STATUS_SUCCESS)
                {
                    result = true;
                }

                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.CreateProfile(PROFILE_NAME, PROFILE_PASS);
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            return result;
        }

        private bool PenProfileCreateAlreadyExistsTest()
        {
            bool result = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                PenProfileReceivedCallbackArgs arg = args[0] as PenProfileReceivedCallbackArgs;

                if (arg.Result == PenProfileReceivedCallbackArgs.ResultType.Success && arg.Status == PenProfile.PROFILE_STATUS_EXIST_PROFILE_ALREADY)
                {
                    result = true;
                }

                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.CreateProfile(PROFILE_NAME, PROFILE_PASS);
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            return result;
        }

        #endregion

        #region pen profile info test

        private bool PenProfileInfoParamTest()
        {
            bool resultNameNull = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.GetProfileInfo(null);
                }
                catch (ArgumentNullException)
                {
                    resultNameNull = true;
                    _autoResetEvent.Set();
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            bool resultNameLong = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.GetProfileInfo(PROFILE_NAME_LONG);
                }
                catch (ArgumentOutOfRangeException)
                {
                    resultNameLong = true;
                    _autoResetEvent.Set();
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            return resultNameNull && resultNameLong;
        }

        private bool PenProfileInfoTest()
        {
            bool resultNotExists = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                PenProfileReceivedCallbackArgs arg = args[0] as PenProfileReceivedCallbackArgs;

                if (arg.Result == PenProfileReceivedCallbackArgs.ResultType.Success && arg.Status == PenProfile.PROFILE_STATUS_NO_EXIST_PROFILE)
                {
                    resultNotExists = true;
                }

                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.GetProfileInfo(PROFILE_NAME_INVALID);
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            bool resultExists = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                PenProfileReceivedCallbackArgs arg = args[0] as PenProfileReceivedCallbackArgs;

                if (arg.Result == PenProfileReceivedCallbackArgs.ResultType.Success && arg.Status == PenProfile.PROFILE_STATUS_SUCCESS)
                {
                    resultExists = true;
                }

                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.GetProfileInfo(PROFILE_NAME);
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            return resultNotExists && resultExists;
        }

        #endregion

        #region pen profile delete test

        private bool PenProfileDeleteParamNullTest()
        {
            bool resultPassNull = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.DeleteProfile(PROFILE_NAME, null);
                }
                catch (ArgumentNullException)
                {
                    resultPassNull = true;
                    _autoResetEvent.Set();
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            bool resultNameNull = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.DeleteProfile(null, PROFILE_PASS);
                }
                catch (ArgumentNullException)
                {
                    resultNameNull = true;
                    _autoResetEvent.Set();
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            return resultPassNull && resultNameNull;
        }

        private bool PenProfileDeleteParamLongTest()
        {
            bool resultNameLong = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.DeleteProfile(PROFILE_NAME_LONG, PROFILE_PASS);
                }
                catch (ArgumentOutOfRangeException)
                {
                    resultNameLong = true;
                    _autoResetEvent.Set();
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            bool resultPassLong = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.DeleteProfile(PROFILE_NAME, PROFILE_PASS_LONG);
                }
                catch (ArgumentOutOfRangeException)
                {
                    resultPassLong = true;
                    _autoResetEvent.Set();
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            return resultNameLong && resultPassLong;
        }

        private bool PenProfileDeleteNameNotExistsTest()
        {
            bool result = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                PenProfileReceivedCallbackArgs arg = args[0] as PenProfileReceivedCallbackArgs;

                if (arg.Result == PenProfileReceivedCallbackArgs.ResultType.Success && arg.Status == PenProfile.PROFILE_STATUS_NO_EXIST_PROFILE)
                {
                    result = true;
                }

                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.DeleteProfile(PROFILE_NAME, PROFILE_PASS);
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            return result;
        }

        private bool PenProfileDeleteInvalidPasswordTest()
        {
            bool result = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                PenProfileReceivedCallbackArgs arg = args[0] as PenProfileReceivedCallbackArgs;

                if (arg.Result == PenProfileReceivedCallbackArgs.ResultType.Success && arg.Status == PenProfile.PROFILE_STATUS_NO_PERMISSION)
                {
                    result = true;
                }

                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.DeleteProfile(PROFILE_NAME, PROFILE_PASS_INVALID);
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            return result;
        }

        private bool PenProfileDeleteSuccessTest()
        {
            bool result = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                PenProfileReceivedCallbackArgs arg = args[0] as PenProfileReceivedCallbackArgs;

                if (arg.Result == PenProfileReceivedCallbackArgs.ResultType.Success && arg.Status == PenProfile.PROFILE_STATUS_SUCCESS)
                {
                    result = true;
                }

                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.DeleteProfile(PROFILE_NAME, PROFILE_PASS);
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            return result;
        }

        #endregion

        #region pen profile write value test

        private bool PenProfileWriteValueParamNullTest()
        {
            bool resultPassNull = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.WriteProfileValues(PROFILE_NAME, null, PROFILE_VALUE_KEYS, PROFILE_VALUE_VALUES);
                }
                catch (ArgumentNullException)
                {
                    resultPassNull = true;
                    _autoResetEvent.Set();
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            bool resultNameNull = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.WriteProfileValues(null, PROFILE_PASS, PROFILE_VALUE_KEYS, PROFILE_VALUE_VALUES);
                }
                catch (ArgumentNullException)
                {
                    resultNameNull = true;
                    _autoResetEvent.Set();
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            return resultPassNull && resultNameNull;
        }

        private bool PenProfileWriteValueParamLongTest()
        {
            bool resultNameLong = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.WriteProfileValues(PROFILE_NAME_LONG, PROFILE_PASS, PROFILE_VALUE_KEYS, PROFILE_VALUE_VALUES);
                }
                catch (ArgumentOutOfRangeException)
                {
                    resultNameLong = true;
                    _autoResetEvent.Set();
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            bool resultPassLong = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.WriteProfileValues(PROFILE_NAME, PROFILE_PASS_LONG, PROFILE_VALUE_KEYS, PROFILE_VALUE_VALUES);
                }
                catch (ArgumentOutOfRangeException)
                {
                    resultPassLong = true;
                    _autoResetEvent.Set();
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            return resultNameLong && resultPassLong;
        }

        private bool PenProfileWriteValuePermissionDeniedTest()
        {
            bool result = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                PenProfileWriteValueCallbackArgs arg = args[0] as PenProfileWriteValueCallbackArgs;

                if (arg.Result != PenProfileReceivedCallbackArgs.ResultType.Failed)
                {
                    foreach (var d in arg.Data)
                    {
                        if (d.Status == PenProfile.PROFILE_STATUS_NO_PERMISSION)
                        {
                            result = true;
                            continue;
                        }
                    }
                }

                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.WriteProfileValues(PROFILE_NAME, PROFILE_PASS_INVALID, PROFILE_VALUE_KEYS, PROFILE_VALUE_VALUES);
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            return result;
        }

        private bool PenProfileWriteValueSuccessTest()
        {
            bool result = true;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                PenProfileWriteValueCallbackArgs arg = args[0] as PenProfileWriteValueCallbackArgs;

                if (arg.Result != PenProfileReceivedCallbackArgs.ResultType.Failed)
                {
                    foreach (var d in arg.Data)
                    {
                        if (d.Status != PenProfile.PROFILE_STATUS_SUCCESS)
                        {
                            result = false;
                            continue;
                        }
                    }
                }

                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.WriteProfileValues(PROFILE_NAME, PROFILE_PASS, PROFILE_VALUE_KEYS, PROFILE_VALUE_VALUES);
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            return result;
        }

        #endregion pen profile write value test

        #region pen profile read value test

        private bool PenProfileReadValueParamTest()
        {
            bool resultNameNull = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.ReadProfileValues(null, PROFILE_VALUE_KEYS);
                }
                catch (ArgumentNullException)
                {
                    resultNameNull = true;
                    _autoResetEvent.Set();
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            bool resultNameLong = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.ReadProfileValues(PROFILE_NAME_LONG, PROFILE_VALUE_KEYS);
                }
                catch (ArgumentOutOfRangeException)
                {
                    resultNameLong = true;
                    _autoResetEvent.Set();
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            return resultNameNull && resultNameLong;
        }

        private bool PenProfileReadValueTest()
        {
            // 프로파일 명이 올바르지 않을때

            bool resultProfileNotExists = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                PenProfileReadValueCallbackArgs arg = args[0] as PenProfileReadValueCallbackArgs;

                if (arg.Result == PenProfileReceivedCallbackArgs.ResultType.Success)
                {
                    foreach (var d in arg.Data)
                    {
                        if (d.Status == PenProfile.PROFILE_STATUS_NO_EXIST_PROFILE)
                        {
                            resultProfileNotExists = true;
                            continue;
                        }
                    }
                }

                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.ReadProfileValues(PROFILE_NAME_INVALID, PROFILE_VALUE_KEYS);
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();


            // 프로파일의 키가 존재 하지 않을때

            bool resultKeyNotExists = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                PenProfileReadValueCallbackArgs arg = args[0] as PenProfileReadValueCallbackArgs;

                if (arg.Result == PenProfileReceivedCallbackArgs.ResultType.Success)
                {
                    foreach (var d in arg.Data)
                    {
                        if (d.Status == PenProfile.PROFILE_STATUS_NO_EXIST_KEY)
                        {
                            resultKeyNotExists = true;
                            continue;
                        }
                    }
                }

                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.ReadProfileValues(PROFILE_NAME, PROFILE_VALUE_KEYS_INVALID);
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();


            // 프로파일 키가 존재하여 값을 잘 얻어올때

            bool resultKeyExists = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                PenProfileReadValueCallbackArgs arg = args[0] as PenProfileReadValueCallbackArgs;

                if (arg.Result == PenProfileReceivedCallbackArgs.ResultType.Success)
                {
                    resultKeyExists = true;

                    foreach (var d in arg.Data)
                    {
                        if (d.Status != PenProfile.PROFILE_STATUS_SUCCESS)
                        {
                            resultKeyExists = false;
                            continue;
                        }
                    }
                }

                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.ReadProfileValues(PROFILE_NAME, PROFILE_VALUE_KEYS);
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            return resultProfileNotExists && resultKeyNotExists &&resultKeyExists;
        }

        #endregion

        #region pen profile delete value test

        private bool PenProfileDeleteValueParamNullTest()
        {
            bool resultPassNull = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.DeleteProfileValues(PROFILE_NAME, null, PROFILE_VALUE_KEYS);
                }
                catch (ArgumentNullException)
                {
                    resultPassNull = true;
                    _autoResetEvent.Set();
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            bool resultNameNull = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.DeleteProfileValues(null, PROFILE_PASS, PROFILE_VALUE_KEYS);
                }
                catch (ArgumentNullException)
                {
                    resultNameNull = true;
                    _autoResetEvent.Set();
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            return resultPassNull && resultNameNull;
        }

        private bool PenProfileDeleteValueParamLongTest()
        {
            bool resultNameLong = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.DeleteProfileValues(PROFILE_NAME_LONG, PROFILE_PASS, PROFILE_VALUE_KEYS);
                }
                catch (ArgumentOutOfRangeException)
                {
                    resultNameLong = true;
                    _autoResetEvent.Set();
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            bool resultPassLong = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.DeleteProfileValues(PROFILE_NAME, PROFILE_PASS_LONG, PROFILE_VALUE_KEYS);
                }
                catch (ArgumentOutOfRangeException)
                {
                    resultPassLong = true;
                    _autoResetEvent.Set();
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            return resultNameLong && resultPassLong;
        }

        private bool PenProfileDeleteValueProfileNotExistsTest()
        {
            bool result = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                PenProfileDeleteValueCallbackArgs arg = args[0] as PenProfileDeleteValueCallbackArgs;

                if (arg.Result == PenProfileReceivedCallbackArgs.ResultType.Success)
                {
                    foreach (var v in arg.Data)
                    {
                        if (v.Status == PenProfile.PROFILE_STATUS_NO_EXIST_PROFILE)
                        {
                            result = true;
                            continue;
                        }
                    }
                }

                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.DeleteProfileValues(PROFILE_NAME_INVALID, PROFILE_PASS, PROFILE_VALUE_KEYS);
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            return result;
        }

        private bool PenProfileDeleteValueInvalidPasswordTest()
        {
            bool result = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                PenProfileDeleteValueCallbackArgs arg = args[0] as PenProfileDeleteValueCallbackArgs;

                if (arg.Result == PenProfileReceivedCallbackArgs.ResultType.Success)
                {
                    foreach (var v in arg.Data)
                    {
                        if (v.Status == PenProfile.PROFILE_STATUS_NO_PERMISSION)
                        {
                            result = true;
                            continue;
                        }
                    }
                }

                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.DeleteProfileValues(PROFILE_NAME, PROFILE_PASS_INVALID, PROFILE_VALUE_KEYS);
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            return result;
        }

        private bool PenProfileDeleteValueSuccessTest()
        {
            bool result = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                PenProfileDeleteValueCallbackArgs arg = args[0] as PenProfileDeleteValueCallbackArgs;

                if (arg.Result == PenProfileReceivedCallbackArgs.ResultType.Success)
                {
                    result = true;

                    foreach (var v in arg.Data)
                    {
                        if (v.Status != PenProfile.PROFILE_STATUS_SUCCESS)
                        {
                            result = false;
                            continue;
                        }
                    }
                }

                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.DeleteProfileValues(PROFILE_NAME, PROFILE_PASS, PROFILE_VALUE_KEYS);
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            return result;
        }

        private bool PenProfileDeleteValueNotExistsTest()
        {
            bool result = false;

            _callbackObj.PenProfileReceived = delegate (object sender, object[] args)
            {
                PenProfileDeleteValueCallbackArgs arg = args[0] as PenProfileDeleteValueCallbackArgs;

                if (arg.Result == PenProfileReceivedCallbackArgs.ResultType.Success)
                {
                    foreach (var v in arg.Data)
                    {
                        if (v.Status == PenProfile.PROFILE_STATUS_NO_EXIST_KEY)
                        {
                            result = true;
                            continue;
                        }
                    }
                }

                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                try
                {
                    _penComm.DeleteProfileValues(PROFILE_NAME, PROFILE_PASS, PROFILE_VALUE_KEYS_INVALID);
                }
                catch
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            return result;
        }

        #endregion

        #endregion
    }
}
