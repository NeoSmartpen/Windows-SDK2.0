using System;
using System.Text;
using Windows.Foundation;
using Windows.Storage;

namespace Neosmartpen.Net
{
    public class PenController : IPenController, IPenControllerEvent
    {
        private object objectLock = new object();
        private PenClientParserV1 mClientV1;
        private PenClientParserV2 mClientV2;

        public int Protocol { get; set; }

        public PenController()
        {
            mClientV1 = new PenClientParserV1();
            mClientV2 = new PenClientParserV2();

            Protocol = Protocols.NONE;
        }

        private IPenClient penClient = null;
        public IPenClient PenClient
        {
            get
            {
                return penClient;
            }
            set
            {
                penClient = value;
                mClientV1.PenClient = penClient;
                mClientV2.PenClient = penClient;
            }
        }

        public void OnDataReceived(byte[] buff)
        {
            if (Protocol == Protocols.V1)
            {
                mClientV1.ProtocolParse(buff, buff.Length);
            }
            else
            {
                mClientV2.ProtocolParse(buff, buff.Length);
            }
        }

        #region Event Property
        /// <summary>
        /// Occurs when a connection is made
        /// </summary>
        public event TypedEventHandler<IPenClient, ConnectedEventArgs> Connected
        {
            add
            {
                lock (objectLock)
                {
                    mClientV1.Connected += value;
                    mClientV2.Connected += value;
                }
            }
            remove
            {
                lock (objectLock)
                {
                    mClientV1.Connected -= value;
                    mClientV2.Connected -= value;
                }
            }
        }

        /// <summary>
        /// Occurs when a connection is closed
        /// </summary>
        public event TypedEventHandler<IPenClient, object> Disconnected
        {
            add
            {
                lock (objectLock)
                {
                    mClientV1.Disconnected += value;
                    mClientV2.Disconnected += value;
                }
            }
            remove
            {
                lock (objectLock)
                {
                    mClientV1.Disconnected -= value;
                    mClientV2.Disconnected -= value;
                }
            }
        }

        /// <summary>
        /// Occurs when finished offline data downloading
        /// </summary>
        public event TypedEventHandler<IPenClient, SimpleResultEventArgs> OfflineDownloadFinished
        {
            add
            {
                lock (objectLock)
                {
                    mClientV1.OfflineDownloadFinished += value;
                    mClientV2.OfflineDownloadFinished += value;
                }
            }
            remove
            {
                lock (objectLock)
                {
                    mClientV1.OfflineDownloadFinished -= value;
                    mClientV2.OfflineDownloadFinished -= value;
                }
            }
        }

        /// <summary>
        /// Occurs when authentication is complete, the password entered has been verified.
        /// </summary>
        public event TypedEventHandler<IPenClient, object> Authenticated
        {
            add
            {
                lock (objectLock) { mClientV1.Authenticated += value; mClientV2.Authenticated += value; }
            }
            remove
            {
                lock (objectLock) { mClientV1.Authenticated -= value; mClientV2.Authenticated -= value; }
            }
        }

        /// <summary>
        /// Occurs when the note information to be used is added
        /// </summary>
        public event TypedEventHandler<IPenClient, object> AvailableNoteAdded
        {
            add
            {
                lock (objectLock) { mClientV1.AvailableNoteAdded += value; mClientV2.AvailableNoteAdded += value; }
            }
            remove
            {
                lock (objectLock) { mClientV1.AvailableNoteAdded -= value; mClientV2.AvailableNoteAdded -= value; }
            }
        }

        /// <summary>
        /// Occurs when the power-on setting is applied when the pen tip is pressed
        /// </summary>
        public event TypedEventHandler<IPenClient, SimpleResultEventArgs> AutoPowerOnChanged
        {
            add
            {
                lock (objectLock) { mClientV1.AutoPowerOnChanged += value; mClientV2.AutoPowerOnChanged += value; }
            }
            remove
            {
                lock (objectLock) { mClientV1.AutoPowerOnChanged -= value; mClientV2.AutoPowerOnChanged -= value; }
            }
        }

        /// <summary>
        /// Occurs when the power-off setting is applied when there is no input for a certain period of time
        /// </summary>
        public event TypedEventHandler<IPenClient, SimpleResultEventArgs> AutoPowerOffTimeChanged
        {
            add
            {
                lock (objectLock) { mClientV1.AutoPowerOffTimeChanged += value; mClientV2.AutoPowerOffTimeChanged += value; }
            }
            remove
            {
                lock (objectLock) { mClientV1.AutoPowerOffTimeChanged -= value; mClientV2.AutoPowerOffTimeChanged -= value; }
            }
        }

        /// <summary>
        /// Occurs when the beep setting is applied
        /// </summary>
        public event TypedEventHandler<IPenClient, SimpleResultEventArgs> BeepSoundChanged
        {
            add
            {
                lock (objectLock) { mClientV1.BeepSoundChanged += value; mClientV2.BeepSoundChanged += value; }
            }
            remove
            {
                lock (objectLock) { mClientV1.BeepSoundChanged -= value; mClientV2.BeepSoundChanged -= value; }
            }
        }

        /// <summary>
        /// Occurs when the cap is closed and the power-on and power-off setting is applied
        /// </summary>
        public event TypedEventHandler<IPenClient, SimpleResultEventArgs> PenCapPowerOnOffChanged
        {
            add
            {
                lock (objectLock) { mClientV1.PenCapPowerOnOffChanged += value; mClientV2.PenCapPowerOnOffChanged += value; }
            }
            remove
            {
                lock (objectLock) { mClientV1.PenCapPowerOnOffChanged -= value; mClientV2.PenCapPowerOnOffChanged -= value; }
            }
        }

        /// <summary>
        /// Occurs when the pen's new LED color value is applied
        /// </summary>
        public event TypedEventHandler<IPenClient, SimpleResultEventArgs> PenColorChanged
        {
            add
            {
                lock (objectLock) { mClientV1.PenColorChanged += value; mClientV2.PenColorChanged += value; }
            }
            remove
            {
                lock (objectLock) { mClientV1.PenColorChanged -= value; mClientV2.PenColorChanged -= value; }
            }
        }

        public event TypedEventHandler<IPenClient, SimpleResultEventArgs> HoverChanged
        {
            add
            {
                lock (objectLock) { mClientV1.HoverChanged += value; mClientV2.HoverChanged += value; }
            }
            remove
            {
                lock (objectLock) { mClientV2.HoverChanged -= value; mClientV2.HoverChanged -= value; }
            }
        }

        /// <summary>
        /// Occurs when settings to store offline data are applied
        /// </summary>
        public event TypedEventHandler<IPenClient, SimpleResultEventArgs> OfflineDataChanged
        {
            add
            {
                lock (objectLock) { mClientV1.OfflineDataChanged += value; mClientV2.OfflineDataChanged += value; }
            }
            remove
            {
                lock (objectLock) { mClientV1.OfflineDataChanged -= value; mClientV2.OfflineDataChanged -= value; }
            }
        }

        /// <summary>
        /// Occurs when requesting a password when the pen is locked with a password
        /// </summary>
        public event TypedEventHandler<IPenClient, PasswordRequestedEventArgs> PasswordRequested
        {
            add
            {
                lock (objectLock) { mClientV1.PasswordRequested += value; mClientV2.PasswordRequested += value; }
            }
            remove
            {
                lock (objectLock) { mClientV1.PasswordRequested -= value; mClientV2.PasswordRequested -= value; }
            }
        }

        /// <summary>
        /// Occurs when the pen's new password is applied
        /// </summary>
        public event TypedEventHandler<IPenClient, SimpleResultEventArgs> PasswordChanged
        {
            add
            {
                lock (objectLock) { mClientV1.PasswordChanged += value; mClientV2.PasswordChanged += value; }
            }
            remove
            {
                lock (objectLock) { mClientV1.PasswordChanged -= value; mClientV2.PasswordChanged -= value; }
            }
        }

        /// <summary>
        /// Occurs when the pen's new fsr sensitivity setting is applied
        /// </summary>
        public event TypedEventHandler<IPenClient, SimpleResultEventArgs> SensitivityChanged
        {
            add
            {
                lock (objectLock) { mClientV1.SensitivityChanged += value; mClientV2.SensitivityChanged += value; }
            }
            remove
            {
                lock (objectLock) { mClientV1.SensitivityChanged -= value; mClientV2.SensitivityChanged -= value; }
            }
        }

        /// <summary>
        /// Occurs when the pen's new fsc sensitivity setting is applied
        /// </summary>
        public event TypedEventHandler<IPenClient, SimpleResultEventArgs> FscSensitivityChanged
        {
            add
            {
                lock (objectLock) { mClientV1.FscSensitivityChanged += value; mClientV2.FscSensitivityChanged += value; }
            }
            remove
            {
                lock (objectLock) { mClientV1.FscSensitivityChanged -= value; mClientV2.FscSensitivityChanged -= value; }
            }
        }

        /// <summary>
        /// Occurs when pen's RTC time is applied
        /// </summary>
        public event TypedEventHandler<IPenClient, SimpleResultEventArgs> RtcTimeChanged
        {
            add
            {
                lock (objectLock) { mClientV1.RtcTimeChanged += value; mClientV2.RtcTimeChanged += value; }
            }
            remove
            {
                lock (objectLock) { mClientV1.RtcTimeChanged -= value; mClientV2.RtcTimeChanged -= value; }
            }
        }

        /// <summary>
        /// Occurs when pen's beep and light is applied
        /// </summary>
        public event TypedEventHandler<IPenClient, SimpleResultEventArgs> BeepAndLightChanged
        {
            add
            {
                lock (objectLock) { mClientV1.BeepAndLightChanged += value; mClientV2.BeepAndLightChanged += value; }
            }
            remove
            {
                lock (objectLock) { mClientV1.BeepAndLightChanged -= value; mClientV2.BeepAndLightChanged -= value; }
            }
        }

        /// <summary>
        /// Occurs when the pen's new bt local name setting is applied
        /// </summary>
        public event TypedEventHandler<IPenClient, SimpleResultEventArgs> BtLocalNameChanged
        {
            add
            {
                lock (objectLock) { mClientV1.BtLocalNameChanged += value; mClientV2.BtLocalNameChanged += value; }
            }
            remove
            {
                lock (objectLock) { mClientV1.BtLocalNameChanged -= value; mClientV2.BtLocalNameChanged -= value; }
            }
        }

        /// <summary>
        /// Occurs when the pen's new data transmission type setting is applied
        /// </summary>
        public event TypedEventHandler<IPenClient, SimpleResultEventArgs> DataTransmissionTypeChanged
        {
            add
            {
                lock (objectLock) { mClientV1.DataTransmissionTypeChanged += value; mClientV2.DataTransmissionTypeChanged += value; }
            }
            remove
            {
                lock (objectLock) { mClientV1.DataTransmissionTypeChanged -= value; mClientV2.DataTransmissionTypeChanged -= value; }
            }
        }


        /// <summary>
        /// Occurs when the pen's new down sampling setting is applied
        /// </summary>
        public event TypedEventHandler<IPenClient, SimpleResultEventArgs> DownSamplingChanged
        {
            add
            {
                lock (objectLock) { mClientV1.DownSamplingChanged += value; mClientV2.DownSamplingChanged += value; }
            }
            remove
            {
                lock (objectLock) { mClientV2.DownSamplingChanged -= value; mClientV2.DownSamplingChanged -= value; }
            }
        }

        /// <summary>
        /// Occurs when the pen's new usb mode setting is applied
        /// </summary>
        public event TypedEventHandler<IPenClient, SimpleResultEventArgs> UsbModeChanged
        {
            add
            {
                lock (objectLock) { mClientV1.UsbModeChanged += value; mClientV2.UsbModeChanged += value; }
            }
            remove
            {
                lock (objectLock) { mClientV1.UsbModeChanged -= value; mClientV2.UsbModeChanged -= value; }
            }
        }

        /// <summary>
        /// Occurs when the pen's battery status changes
        /// </summary>
        public event TypedEventHandler<IPenClient, BatteryAlarmReceivedEventArgs> BatteryAlarmReceived
        {
            add
            {
                lock (objectLock) { mClientV1.BatteryAlarmReceived += value; mClientV2.BatteryAlarmReceived += value; }
            }
            remove
            {
                lock (objectLock) { mClientV1.BatteryAlarmReceived -= value; mClientV2.BatteryAlarmReceived -= value; }
            }
        }

        /// <summary>
        /// Occurs when new coordinate data is received
        /// </summary>
        public event TypedEventHandler<IPenClient, DotReceivedEventArgs> DotReceived
        {
            add
            {
                lock (objectLock) { mClientV1.DotReceived += value; mClientV2.DotReceived += value; }
            }
            remove
            {
                lock (objectLock) { mClientV1.DotReceived -= value; mClientV2.DotReceived -= value; }
            }
        }

        /// <summary>
        /// Occurs when firmware installation is complete
        /// </summary>
        public event TypedEventHandler<IPenClient, SimpleResultEventArgs> FirmwareInstallationFinished
        {
            add
            {
                lock (objectLock) { mClientV1.FirmwareInstallationFinished += value; mClientV2.FirmwareInstallationFinished += value; }
            }
            remove
            {
                lock (objectLock) { mClientV1.FirmwareInstallationFinished -= value; mClientV2.FirmwareInstallationFinished -= value; }
            }
        }

        /// <summary>
        /// Occurs when firmware installation is started
        /// </summary>
        public event TypedEventHandler<IPenClient, object> FirmwareInstallationStarted
        {
            add
            {
                lock (objectLock) { mClientV1.FirmwareInstallationStarted += value; mClientV2.FirmwareInstallationStarted += value; }
            }
            remove
            {
                lock (objectLock) { mClientV1.FirmwareInstallationStarted -= value; mClientV2.FirmwareInstallationStarted -= value; }
            }
        }

        /// <summary>
        /// Notice the progress while the firmware installation is in progress
        /// </summary>
        public event TypedEventHandler<IPenClient, ProgressChangeEventArgs> FirmwareInstallationStatusUpdated
        {
            add
            {
                lock (objectLock) { mClientV1.FirmwareInstallationStatusUpdated += value; mClientV2.FirmwareInstallationStatusUpdated += value; }
            }
            remove
            {
                lock (objectLock) { mClientV1.FirmwareInstallationStatusUpdated -= value; mClientV2.FirmwareInstallationStatusUpdated -= value; }
            }
        }

        /// <summary>
        /// Occurs when a list of offline data is received
        /// </summary>
        public event TypedEventHandler<IPenClient, OfflineDataListReceivedEventArgs> OfflineDataListReceived
        {
            add
            {
                lock (objectLock) { mClientV1.OfflineDataListReceived += value; mClientV2.OfflineDataListReceived += value; }
            }
            remove
            {
                lock (objectLock) { mClientV1.OfflineDataListReceived -= value; mClientV2.OfflineDataListReceived -= value; }
            }
        }

        /// <summary>
        /// Occurs when an offline stroke is received
        /// </summary>
        public event TypedEventHandler<IPenClient, OfflineStrokeReceivedEventArgs> OfflineStrokeReceived
        {
            add
            {
                lock (objectLock) { mClientV1.OfflineStrokeReceived += value; mClientV2.OfflineStrokeReceived += value; }
            }
            remove
            {
                lock (objectLock) { mClientV1.OfflineStrokeReceived -= value; mClientV2.OfflineStrokeReceived -= value; }
            }
        }

        /// <summary>
        /// Occurs when a status of pen is received
        /// </summary>
        public event TypedEventHandler<IPenClient, PenStatusReceivedEventArgs> PenStatusReceived
        {
            add
            {
                lock (objectLock) { mClientV1.PenStatusReceived += value; mClientV2.PenStatusReceived += value; }
            }
            remove
            {
                lock (objectLock) { mClientV1.PenStatusReceived -= value; mClientV2.PenStatusReceived -= value; }
            }
        }

        /// <summary>
        /// Occurs when an offline data is removed
        /// </summary>
        public event TypedEventHandler<IPenClient, SimpleResultEventArgs> OfflineDataRemoved
        {
            add
            {
                lock (objectLock) { mClientV1.OfflineDataRemoved += value; mClientV2.OfflineDataRemoved += value; }
            }
            remove
            {
                lock (objectLock) { mClientV1.OfflineDataRemoved -= value; mClientV2.OfflineDataRemoved -= value; }
            }
        }

        /// <summary>
        /// Occurs when offline downloading starts
        /// </summary>
        public event TypedEventHandler<IPenClient, object> OfflineDataDownloadStarted
        {
            add
            {
                lock (objectLock) { mClientV1.OfflineDataDownloadStarted += value; mClientV2.OfflineDataDownloadStarted += value; }
            }
            remove
            {
                lock (objectLock) { mClientV1.OfflineDataDownloadStarted -= value; mClientV2.OfflineDataDownloadStarted -= value; }
            }
        }

        /// <summary>
        /// Occurs when a response to an operation request for a pen profile is received
        /// </summary>
        public event TypedEventHandler<IPenClient, PenProfileReceivedEventArgs> PenProfileReceived
        {
            add
            {
                lock (objectLock) { mClientV1.PenProfileReceived += value; mClientV2.PenProfileReceived += value; }
            }
            remove
            {
                lock (objectLock) { mClientV1.PenProfileReceived -= value; mClientV2.PenProfileReceived -= value; }
            }
        }


        /// <summary>
        /// Occurs when error received
        /// </summary>
        public event TypedEventHandler<IPenClient, ErrorDetectedEventArgs> ErrorDetected
        {
            add
            {
                lock (objectLock) { mClientV1.ErrorDetected += value; mClientV2.ErrorDetected += value; }
            }
            remove
            {
                lock (objectLock) { mClientV1.ErrorDetected -= value; mClientV2.ErrorDetected -= value; }
            }
        }
        #endregion

        #region Request
        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="oldone">old password</param>
        /// <param name="newone">new password</param>
        public bool SetPassword(string oldone, string newone = "")
        {
            return Request(() => { return mClientV1.ReqSetUpPassword(oldone, newone); }, () => { return mClientV2.ReqSetUpPassword(oldone, newone); });
        }

        /// <summary>
        /// If you request a password when the pen is locked, enter it
        /// </summary>
        /// <param name="password">Specifies the password for authentication. password is a string, maximum length is 16 bytes</param>
        public void InputPassword(string password)
        {
            Request(() => mClientV1.ReqInputPassword(password), () => mClientV2.ReqInputPassword(password));
        }

        /// <summary>
        /// Request the status of the pen
        /// </summary>
        public void RequestPenStatus()
        {
            Request(() => mClientV1.ReqPenStatus(), () => mClientV2.ReqPenStatus());
        }

        /// <summary>
        /// Sets the pen's RTC timestamp
        /// </summary>
        /// <param name="timetick">milisecond timestamp tick (from 1970-01-01)</param>
        public void SetRtcTime(long timetick)
        {
            Request(null, () => mClientV2.ReqSetupTime(timetick));
        }

        /// <summary>
        /// Sets the power-off setting when there is no input for a certain period of time
        /// </summary>
        /// <param name="minute">minute of maximum idle time, staying power on (0~)</param>
        public void SetAutoPowerOffTime(short minute)
        {
            Request(() => mClientV1.ReqSetupPenAutoShutdownTime(minute), () => mClientV2.ReqSetupPenAutoShutdownTime(minute));
        }

        /// <summary>
        /// Sets the property that can be control by cap of pen
        /// </summary>
        /// <param name="enable">true if you want enable setting; otherwise, false</param>
        public void SetPenCapPowerOnOffEnable(bool enable)
        {
            Request(null, () => mClientV2.ReqSetupPenCapPower(enable));
        }

        /// <summary>
        /// Sets the power-on setting when the pen tip is pressed
        /// </summary>
        /// <param name="enable">true if you want enable setting; otherwise, false</param>
        public void SetAutoPowerOnEnable(bool enable)
        {
            Request(() => mClientV1.ReqSetupPenAutoPowerOn(enable), () => mClientV2.ReqSetupPenAutoPowerOn(enable));
        }

        /// <summary>
        /// Sets the status of the beep sound property
        /// </summary>
        /// <param name="enable">true if you want enable setting; otherwise, false</param>
        public void SetBeepSoundEnable(bool enable)
        {
            Request(() => mClientV1.ReqSetupPenBeep(enable), () => mClientV2.ReqSetupPenBeep(enable));
        }

        private void SetHoverEnable(bool enable)
        {
            Request(() => mClientV1.ReqSetupHoverMode(enable), () => mClientV2.ReqSetupHoverMode(enable));
        }

        /// <summary>
        /// Sets the usage of offline data
        /// </summary>
        /// <param name="enable">true if you want enable setting; otherwise, false</param>
        public void SetOfflineDataEnable(bool enable)
        {
            Request(null, () => mClientV2.ReqSetupOfflineData(enable));
        }

        /// <summary>
        /// Sets the color of LED
        /// </summary>
        /// <param name="color">integer type color formatted 0xAARRGGBB</param>
        public void SetColor(int color)
        {
            Request(() => mClientV1.ReqSetupPenColor(color), () => mClientV2.ReqSetupPenColor(color));
        }

        /// <summary>
        /// Sets the value of the pen's sensitivity property that controls the force sensor of pen
        /// </summary>
        /// <param name="step">the value of sensitivity. (0~4, 0 means maximum sensitivity)</param>
        public void SetSensitivity(short step)
        {
            Request(() => mClientV1.ReqSetupPenSensitivity(step), () => mClientV2.ReqSetupPenSensitivity(step));
        }

        /// <summary>
        /// Sets the status of usb mode property that determine if usb mode is disk or bulk.
        /// You can choose between Disk mode, which is used as a removable disk, and Bulk mode, which is capable of high volume data communication, when connected with usb
        /// </summary>
        /// <param name="mode">enum of UsbMode</param>
        public void SetUsbMode(UsbMode mode)
        {
            Request(null, () => mClientV2.ReqSetupUsbMode(mode));
        }

        /// <summary>
        /// Sets the status of the down sampling property.
        /// Downsampling is a function of avoiding unnecessary data communication by omitting coordinates at the same position.
        /// </summary>
        /// <param name="enable">true if you want to enable down sampling, otherwise false.</param>
        public void SetDownSampling(bool enable)
        {
            Request(null, () => mClientV2.ReqSetupDownSampling(enable));
        }

        /// <summary>
        /// Sets the local name of the bluetooth device property.
        /// </summary>
        /// <param name="btLocalName">Bluetooth local name to set</param>
        public void SetBtLocalName(string btLocalName)
        {
            Request(null, () => mClientV2.ReqSetupBtLocalName(btLocalName));
        }

        /// <summary>
        /// Sets the value of the pen's sensitivity property that controls the force sensor(c-type) of pen.
        /// </summary>
        /// <param name="step">the value of sensitivity. (0~4, 0 means maximum sensitivity)</param>
        public void SetFscSensitivity(short step)
        {
            Request(null, () => mClientV2.ReqSetupPenFscSensitivity(step));
        }

        /// <summary>
        /// Sets the status of data transmission type property that determine if data transmission type is event or request-response.
        /// </summary>
        /// <param name="mode">enum of DataTransmissionType</param>
        public void SetDataTransmissionType(DataTransmissionType mode)
        {
            Request(null, () => mClientV2.ReqSetupDataTransmissionType(mode));
        }

        /// <summary>
        /// Request Beeps and light on.
        /// </summary>
        public void RequestBeepAndLight()
        {
            Request(null, () => mClientV2.ReqBeepAndLight());
        }

        /// <summary>
        /// Sets the available notebook type
        /// </summary>
        public void AddAvailableNote()
        {
            Request(() => mClientV1.ReqAddUsingNote(), () => mClientV2.ReqAddUsingNote());
        }

        /// <summary>
        /// Sets the available notebook type 
        /// </summary>
        /// <param name="section">The section Id of the paper</param>
        /// <param name="owner">The owner Id of the paper</param>
        /// <param name="notes">The array of notebook Id list</param>
        public void AddAvailableNote(int section, int owner, int[] notes = null)
        {
            Request(() => mClientV1.ReqAddUsingNote(section, owner, notes), () => mClientV2.ReqAddUsingNote(section, owner, notes));
        }

        /// <summary>
        /// Sets the available notebook types
        /// </summary>
        /// <param name="section">The array of section Id of the paper list</param>
        /// <param name="owner">The array of owner Id of the paper list</param>
        public void AddAvailableNote(int[] section, int[] owner)
        {
            if (section == null)
                throw new ArgumentNullException("section");
            if (owner == null)
                throw new ArgumentNullException("onwer");
            if (section.Length != owner.Length)
                throw new ArgumentOutOfRangeException("section, owner", "The number of section and owner does not match");

            Request(() => mClientV1.ReqAddUsingNote(section, owner), () => mClientV2.ReqAddUsingNote(section, owner));
        }

        //public bool RequestOfflineDataList(int section, int owner, int note);

        /// <summary>
        /// Requests the list of Offline data
        /// </summary>
        public void RequestOfflineDataList()
        {
            Request(() => mClientV1.ReqOfflineDataList(), () => mClientV2.ReqOfflineDataList());
        }

        /// <summary>
        /// Request to remove offline data in device.
        /// </summary>
        /// <param name="section">The Section Id of the paper</param>
        /// <param name="owner">The Owner Id of the paper</param>
        /// <param name="notes">The Note Id's array of the paper</param>
        public void RequestRemoveOfflineData(int section, int owner, int[] notes)
        {
            Request(null, () => mClientV2.ReqRemoveOfflineData(section, owner, notes));
        }

        /// <summary>
        /// Requests the transmission of offline data 
        /// </summary>
        /// <param name="section">The section Id of the paper</param>
        /// <param name="owner">The owner Id of the paper</param>
        /// <param name="notes">The array of notebook Id list</param>
        /// <param name="deleteOnFinished">delete offline data when transmission is finished</param>
        /// <param name="pages">The array of page's number</param>
        public bool RequestOfflineData(int section, int owner, int note, bool deleteOnFinished = true, int[] pages = null)
        {
            return Request(() => { return mClientV1.ReqOfflineData(new OfflineDataInfo(section, owner, note, pages)); }, () => { return mClientV2.ReqOfflineData(section, owner, note, deleteOnFinished, pages); });
        }

        /// <summary>
        /// Requests the transmission of offline data 
        /// </summary>
        /// <param name="section">The section Id of the paper</param>
        /// <param name="owner">The owner Id of the paper</param>
        /// <param name="notes">The array of notebook Id list</param>
        public bool RequestOfflineData(int section, int owner, int[] notes)
        {
            return Request(() => { return mClientV1.ReqOfflineData(new OfflineDataInfo(section, owner, notes[0])); }, () => { return mClientV2.ReqOfflineData(section, owner, notes[0]); });
        }

        /// <summary>
        /// Requests the transmission of offline data 
        /// </summary>
        /// <param name="section">The section Id of the paper</param>
        /// <param name="owner">The owner Id of the paper</param>
        public void RequestOfflineData(int section, int owner)
        {
            Request(() => mClientV1.ReqPenStatus(), () => mClientV2.ReqPenStatus());
        }

        /// <summary>
        /// Requests the firmware installation 
        /// </summary>
        /// <param name="file">Represents a binary file of firmware</param>
        /// <param name="version">Version of firmware typed string</param>
        /// <param name="forceWithCompression">force upload compressed file</param>
        public void RequestFirmwareInstallation(string file, string version, Compressible? forceCompression = null)
        {
            if (forceCompression != null && Protocol == Protocols.V1)
            {
                throw new NotSupportedException($"forceCompression is not supported at this device");
            }
            Request(() => mClientV1.ReqPenSwUpgrade(file), () => { mClientV2.ReqPenSwUpgrade(file, version, forceCompression); });
        }

        /// <summary>
        /// Request to suspend firmware installation
        /// </summary>
        public void SuspendFirmwareInstallation()
        {
            Request(() => mClientV1.SuspendSwUpgrade(), () => mClientV2.SuspendSwUpgrade());
        }

        public bool IsSupportPenProfile()
        {
            if (PenClient == null || !PenClient.Alive || Protocol == -1)
            {
                throw new RequestIsUnreached();
            }

            if (Protocol == Protocols.V1)
            {
                return mClientV1.IsSupportPenProfile();
            }
            else
            {
                return mClientV2.IsSupportPenProfile();
            }
        }

        /// <summary>
        /// Request to create profile
        /// </summary>
        /// <param name="profileName">Name of the profile to be created</param>
        /// <param name="password">Password of profile</param>
        //public void CreateProfile(string profileName, string password)
        public void CreateProfile(string profileName, byte[] password)
        {
            if (IsSupportPenProfile())
            {
                if (string.IsNullOrEmpty(profileName))
                    throw new ArgumentNullException("profileName");
                if (password == null)
                    throw new ArgumentNullException("password");

                byte[] profileNameBytes = Encoding.UTF8.GetBytes(profileName);
                //byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                if (profileNameBytes.Length > PenProfile.LIMIT_BYTE_LENGTH_PROFILE_NAME)
                    throw new ArgumentOutOfRangeException("profileName", $"profileName byte length must be {PenProfile.LIMIT_BYTE_LENGTH_PROFILE_NAME} or less");
                else if (password.Length != PenProfile.LIMIT_BYTE_LENGTH_PASSWORD)
                    throw new ArgumentOutOfRangeException("password", $"password byte length must be {PenProfile.LIMIT_BYTE_LENGTH_PASSWORD}");

                Request(() => mClientV1.ReqCreateProfile(profileNameBytes, password), () => mClientV2.ReqCreateProfile(profileNameBytes, password));
            }
            else
                throw new NotSupportedException($"CreateProfile is not supported at this pen firmware version");

        }

        /// <summary>
        /// Request to delete profile
        /// </summary>
        /// <param name="profileName">Name of the profile to be deleted</param>
        /// <param name="password">password of profile</param>
        public void DeleteProfile(string profileName, byte[] password)
        {
            if (IsSupportPenProfile())
            {
                if (string.IsNullOrEmpty(profileName))
                    throw new ArgumentNullException("profileName");
                if (password == null)
                    throw new ArgumentNullException("password");

                byte[] profileNameBytes = Encoding.UTF8.GetBytes(profileName);
                //byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                if (profileNameBytes.Length > PenProfile.LIMIT_BYTE_LENGTH_PROFILE_NAME)
                    throw new ArgumentOutOfRangeException("profileName", $"profileName byte length must be {PenProfile.LIMIT_BYTE_LENGTH_PROFILE_NAME} or less");
                else if (password.Length != PenProfile.LIMIT_BYTE_LENGTH_PASSWORD)
                    throw new ArgumentOutOfRangeException("password", $"password byte length must be {PenProfile.LIMIT_BYTE_LENGTH_PASSWORD}");

                Request(() => mClientV1.ReqDeleteProfile(profileNameBytes, password), () => mClientV2.ReqDeleteProfile(profileNameBytes, password));
            }
            else
                throw new NotSupportedException($"CreateProfile is not supported at this pen firmware version");
        }

        /// <summary>
        /// Request information of the profile
        /// </summary>
        /// <param name="profileName">profile's name</param>
        public void GetProfileInfo(string profileName)
        {
            if (IsSupportPenProfile())
            {
                if (string.IsNullOrEmpty(profileName))
                    throw new ArgumentNullException("profileName");

                byte[] profileNameBytes = Encoding.UTF8.GetBytes(profileName);
                if (profileNameBytes.Length > PenProfile.LIMIT_BYTE_LENGTH_PROFILE_NAME)
                    throw new ArgumentOutOfRangeException("profileName", $"profileName byte length must be {PenProfile.LIMIT_BYTE_LENGTH_PROFILE_NAME} or less");

                Request(() => mClientV1.ReqProfileInfo(profileNameBytes), () => mClientV2.ReqProfileInfo(profileNameBytes));
            }
            else
                throw new NotSupportedException($"CreateProfile is not supported at this pen firmware version");
        }

        /// <summary>
        /// Request to get data from profile
        /// </summary>
        /// <param name="profileName">profile name</param>
        /// <param name="keys">key array</param>
        public void ReadProfileValues(string profileName, string[] keys)
        {
            if (IsSupportPenProfile())
            {
                if (string.IsNullOrEmpty(profileName))
                    throw new ArgumentNullException("profileName");
                if (keys == null)
                    throw new ArgumentNullException("keys");

                byte[] profileNameBytes = Encoding.UTF8.GetBytes(profileName);
                if (profileNameBytes.Length > PenProfile.LIMIT_BYTE_LENGTH_PROFILE_NAME)
                    throw new ArgumentOutOfRangeException("profileName", $"profileName byte length must be {PenProfile.LIMIT_BYTE_LENGTH_PROFILE_NAME} or less");

                byte[][] keysBytes = new byte[keys.Length][];
                for (int i = 0; i < keys.Length; ++i)
                {
                    keysBytes[i] = Encoding.UTF8.GetBytes(keys[i]);
                    if (keysBytes[i].Length > PenProfile.LIMIT_BYTE_LENGTH_KEY)
                        throw new ArgumentOutOfRangeException("keys", $"key byte length must be {PenProfile.LIMIT_BYTE_LENGTH_KEY} or less");
                }

                Request(() => mClientV1.ReqReadProfileValue(profileNameBytes, keysBytes), () => mClientV2.ReqReadProfileValue(profileNameBytes, keysBytes));
            }
            else
                throw new NotSupportedException($"CreateProfile is not supported at this pen firmware version");
        }

        /// <summary>
        /// Request to write data
        /// </summary>
        /// <param name="profileName">profile name</param>
        /// <param name="password">password</param>
        /// <param name="keys">key array</param>
        /// <param name="data">data</param>
        public void WriteProfileValues(string profileName, byte[] password, string[] keys, byte[][] data)
        {
            if (IsSupportPenProfile())
            {
                if (string.IsNullOrEmpty(profileName))
                    throw new ArgumentNullException("profileName");
                if (password == null)
                    throw new ArgumentNullException("password");
                if (keys == null)
                    throw new ArgumentNullException("keys");
                if (data == null)
                    throw new ArgumentNullException("data");
                if (keys.Length != data.Length)
                    throw new ArgumentOutOfRangeException("keys, data", "The number of keys and data does not match");

                byte[] profileNameBytes = Encoding.UTF8.GetBytes(profileName);
                //byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                if (profileNameBytes.Length > PenProfile.LIMIT_BYTE_LENGTH_PROFILE_NAME)
                    throw new ArgumentOutOfRangeException("profileName", $"profileName byte length must be {PenProfile.LIMIT_BYTE_LENGTH_PROFILE_NAME} or less");
                else if (password.Length != PenProfile.LIMIT_BYTE_LENGTH_PASSWORD)
                    throw new ArgumentOutOfRangeException("password", $"password byte length must be {PenProfile.LIMIT_BYTE_LENGTH_PASSWORD}");

                byte[][] keysBytes = new byte[keys.Length][];
                for (int i = 0; i < keys.Length; ++i)
                {
                    keysBytes[i] = Encoding.UTF8.GetBytes(keys[i]);
                    if (keysBytes[i].Length > PenProfile.LIMIT_BYTE_LENGTH_KEY)
                        throw new ArgumentOutOfRangeException("keys", $"key byte length must be {PenProfile.LIMIT_BYTE_LENGTH_KEY} or less");
                }

                Request(() => mClientV1.ReqWriteProfileValue(profileNameBytes, password, keysBytes, data), () => mClientV2.ReqWriteProfileValue(profileNameBytes, password, keysBytes, data));
            }
            else
                throw new NotSupportedException($"CreateProfile is not supported at this pen firmware version");
        }

        /// <summary>
        /// Request to delete data
        /// </summary>
        /// <param name="profileName">profile name</param>
        /// <param name="password">password</param>
        /// <param name="keys">key array</param>
        public void DeleteProfileValues(string profileName, byte[] password, string[] keys)
        {
            if (IsSupportPenProfile())
            {
                if (string.IsNullOrEmpty(profileName))
                    throw new ArgumentNullException("profileName");
                if (password == null)
                    throw new ArgumentNullException("password");
                if (keys == null)
                    throw new ArgumentNullException("keys");

                byte[] profileNameBytes = Encoding.UTF8.GetBytes(profileName);
                //byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                if (profileNameBytes.Length > PenProfile.LIMIT_BYTE_LENGTH_PROFILE_NAME)
                    throw new ArgumentOutOfRangeException("profileName", $"profileName byte length must be {PenProfile.LIMIT_BYTE_LENGTH_PROFILE_NAME} or less");
                else if (password.Length != PenProfile.LIMIT_BYTE_LENGTH_PASSWORD)
                    throw new ArgumentOutOfRangeException("password", $"password byte length must be {PenProfile.LIMIT_BYTE_LENGTH_PASSWORD}");

                byte[][] keysBytes = new byte[keys.Length][];
                for (int i = 0; i < keys.Length; ++i)
                {
                    keysBytes[i] = Encoding.UTF8.GetBytes(keys[i]);
                    if (keysBytes[i].Length > PenProfile.LIMIT_BYTE_LENGTH_KEY)
                        throw new ArgumentOutOfRangeException("keys", $"key byte length must be {PenProfile.LIMIT_BYTE_LENGTH_KEY} or less");
                }

                Request(() => mClientV1.ReqDeleteProfileValue(profileNameBytes, password, keysBytes), () => mClientV2.ReqDeleteProfileValue(profileNameBytes, password, keysBytes));
            }
            else
                throw new NotSupportedException($"CreateProfile is not supported at this pen firmware version");
        }

        public void OnConnected()
        {
            if (Protocol != Protocols.V1)
            {
                mClientV2.ReqVersionTask();
            }
        }

        public void OnDisconnected()
        {
            if (Protocol == Protocols.V1)
                mClientV1.OnDisconnected();
            else
                mClientV2.OnDisconnected();
        }

        public delegate void RequestDele();

        private void Request(RequestDele requestToV1, RequestDele requestToV2)
        {
            if (PenClient == null || !PenClient.Alive || Protocol == -1)
            {
                throw new RequestIsUnreached();
            }

            if (Protocol == Protocols.V1)
            {
                if (requestToV1 == null) throw new UnavailableRequest();

                requestToV1();
            }
            else
            {
                if (requestToV2 == null) throw new UnavailableRequest();

                requestToV2();
            }
        }

        public delegate bool RequestDeleReturnBool();

        private bool Request(RequestDeleReturnBool requestToV1, RequestDeleReturnBool requestToV2)
        {
            if (PenClient == null || !PenClient.Alive || Protocol == -1)
            {
                throw new RequestIsUnreached();
            }

            if (Protocol == Protocols.V1)
            {
                if (requestToV1 == null) throw new UnavailableRequest();

                return requestToV1();
            }
            else
            {
                if (requestToV2 == null) throw new UnavailableRequest();

                return requestToV2();
            }
        }

        #endregion

        public void SetPressureCalibrateFactor(int cPX1, int cPY1, int cPX2, int cPY2, int cPX3, int cPY3)
        {
            Support.PressureCalibration.Instance.MakeFactor(cPX1, cPY1, cPX2, cPY2, cPX3, cPY3);
        }

        public float GetPressureCalibrationFactor(int index)
        {
            if (index < 0 || index > Support.PressureCalibration.Instance.MAX_FACTOR)
                return -1;
            return Support.PressureCalibration.Instance.Factor[index];
        }
    }

    /// <summary>
    /// This exception is thrown when a request is submitted to the PenController and the request fails.
    /// </summary>
    public class PenRequestException : Exception
    {
    }

    /// <summary>
    /// This exception is thrown when the connection is abnormal when requesting the pen.
    /// </summary>
    public class RequestIsUnreached : PenRequestException
    {
    }

    /// <summary>
    /// This exception is thrown when requesting a function that is not in the pen
    /// </summary>
    public class UnavailableRequest : PenRequestException
    {
    }
}