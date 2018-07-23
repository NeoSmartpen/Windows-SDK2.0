using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neosmartpen.Net.Bluetooth;
using Neosmartpen.Net.Protocol.v1;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Neosmartpen.UnitTest
{
    [TestClass]
    public class ProtocolVersion1UnitTest
    {
        private BluetoothAdapter _btAdt;

        private PenCommV1 _penComm;

        private PenCommV1CallbacksImpl _callbackObj;

        private AutoResetEvent _autoResetEvent = new AutoResetEvent(false);

        public const int TEST_TIMEOUT = 15000;

        public const string MAC = "9C7BD202300F";

        public const string PASSWORD = "1234";

        public const int DEFAULT_SECTION = 3;
        public const int DEFAULT_OWNER = 27;
        public const int DEFAULT_NOTE = 603;

        public const string FIRMWARE_FILEPATH = "E:\\vs_workplace\\WINSDK\\Neosmartpen.UnitTest\\N2_1.07.0162._v_";

        [TestInitialize]
        public void SetUp()
        {
            _btAdt = new BluetoothAdapter();

            _callbackObj = new PenCommV1CallbacksImpl();

            _callbackObj.Authenticated = delegate (object sender, object[] args)
            {
                _autoResetEvent.Set();
            };

            _callbackObj.PasswordRequest = delegate (object sender, object[] args)
            {
                _penComm.ReqInputPassword(PASSWORD);
            };

            _penComm = new PenCommV1(_callbackObj);

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
            bool result = false;

            _callbackObj.OfflineDataListReceived = delegate (object sender, object[] args)
            {
                result = true;
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                _penComm.ReqOfflineDataList();
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
                _penComm.ReqOfflineData(new Net.OfflineDataInfo(DEFAULT_SECTION, DEFAULT_OWNER, DEFAULT_NOTE));
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
            bool result1 = _penComm.ReqSetUpPassword(null,null);

            Assert.IsFalse(result1);

            bool result2 = _penComm.ReqSetUpPassword("0000", PASSWORD);

            Assert.IsFalse(result2);

            bool result3 = _penComm.ReqSetUpPassword(PASSWORD, "0000");

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
                _penComm.ReqSetUpPassword(PASSWORD, "");
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


            bool fourthCheck = false;

            _callbackObj.AvailableNoteAccepted = delegate (object sender, object[] args)
            {
                fourthCheck = (bool)args[0];
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                _penComm.ReqAddUsingNote(DEFAULT_SECTION, DEFAULT_OWNER, DEFAULT_NOTE);
            });

            _autoResetEvent.WaitOne();

            bool fifthCheck = false;

            _callbackObj.AvailableNoteAccepted = delegate (object sender, object[] args)
            {
                fifthCheck = (bool)args[0];
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                _penComm.ReqAddUsingNote(DEFAULT_SECTION, DEFAULT_OWNER, null);
            });

            _autoResetEvent.WaitOne();

            bool sixthCheck = false;

            _callbackObj.AvailableNoteAccepted = delegate (object sender, object[] args)
            {
                sixthCheck = (bool)args[0];
                _autoResetEvent.Set();
            };

            Task.Factory.StartNew(() =>
            {
                _penComm.ReqAddUsingNote(DEFAULT_SECTION, DEFAULT_OWNER, new int[] { 601, 602, 603, 604, 605, 606, 607, 608, 609, 610 });
            });

            _autoResetEvent.WaitOne();

            Assert.IsTrue(firstCheck && secondCheck && thirdCheck && fourthCheck && fifthCheck && sixthCheck);
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
                requestResult = _penComm.ReqPenSwUpgrade(FIRMWARE_FILEPATH);

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
                requestResult = _penComm.ReqPenSwUpgrade(FIRMWARE_FILEPATH);

                if (!requestResult)
                {
                    _autoResetEvent.Set();
                }
            });

            _autoResetEvent.WaitOne();

            return requestResult && !result;
        }

        #endregion
    }
}
