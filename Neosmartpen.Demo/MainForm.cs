using Neosmartpen.Net;
using Neosmartpen.Net.Bluetooth;
using Neosmartpen.Net.Support;
using System;
using System.Drawing;
using System.Windows.Forms;
using Windows.Storage;

namespace PenDemo
{
    public partial class MainForm : Form
    {
        public static string DEFAULT_PASSWORD = "0000";

        private readonly Bitmap canvasBitmap;
        private Stroke currentStroke;

        private readonly int canvasWidth;
        private readonly int canvasHeight;

        private readonly object drawLock = new object();

        private readonly ProgressForm progressForm;
        private readonly PasswordInputForm passwordInputForm;

        private readonly GenericBluetoothPenClient bluetooth;
        private readonly PenController controller;

        public const string PROGRESS_TITLE_OFFLINE = "Download Offline Data";
        public const string PROGRESS_TITLE_UPDATE = "Firmware Update";

        private readonly object progressLock = new object();

        public MainForm()
        {
            InitializeComponent();

            canvasWidth = pictureBox.Width;
            canvasHeight = pictureBox.Height;

            canvasBitmap = new Bitmap(pictureBox.Width, pictureBox.Height);

            progressForm = new ProgressForm();
            passwordInputForm = new PasswordInputForm(OnInputPassword);

            // create PenController instance.
            controller = new PenController();

            // Create BluetoothPenClient instance. and bind PenController.
            // BluetoothPenClient is implementation of bluetooth function.
            bluetooth = new GenericBluetoothPenClient(controller);

            // bluetooth advertisement event
            bluetooth.onStopSearch += OnStopSearch;
            bluetooth.onUpdatePenController += OnUpdatePenController;
            bluetooth.onAddPenController += OnAddPenController;

            // pen controller event
            controller.PenStatusReceived += PenStatusReceived;
            controller.Connected += Connected;
            controller.Disconnected += Disconnected;
            controller.Authenticated += Authenticated;
            controller.DotReceived += DotReceived;
            controller.PasswordRequested += PasswordRequested;
            controller.OfflineDataListReceived += OfflineDataListReceived;

            controller.AutoPowerOffTimeChanged += ConfigurationChanged;
            controller.AutoPowerOnChanged += ConfigurationChanged;
            controller.RtcTimeChanged += ConfigurationChanged;
            controller.SensitivityChanged += ConfigurationChanged;
            controller.BeepSoundChanged += ConfigurationChanged;
            controller.PenColorChanged += ConfigurationChanged;

            controller.PasswordChanged += PasswordChanged;

            controller.BatteryAlarmReceived += BatteryAlarmReceived;

            controller.OfflineDataDownloadStarted += OfflineDataDownloadStarted;
            controller.OfflineStrokeReceived += OfflineStrokeReceived;
            controller.OfflineDownloadFinished += OfflineDownloadFinished;
            controller.OfflineDataRemoved += OfflineDataRemoved;

            controller.FirmwareInstallationStatusUpdated += FirmwareInstallationStatusUpdated;
            controller.FirmwareInstallationFinished += FirmwareInstallationFinished;
        }

        private void ConsoleWrite(string message)
        {
            this.BeginInvoke(new MethodInvoker(delegate ()
            {
                if (ConsoleTextbox.Text != "")
                {
                    ConsoleTextbox.AppendText("\r\n");
                }
                ConsoleTextbox.AppendText(DateTime.Now.ToString("hh:mm:ss - ") + message);
            }));
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            SearchButton.Enabled = false;
            DevicesListbox.Items.Clear();
            bluetooth.StartLEAdvertisementWatcher();
        }

        #region Bluetooth Advertisement Event

        private void OnAddPenController(IPenClient sender, PenInformation args)
        {
            this.BeginInvoke(new MethodInvoker(delegate ()
            {
                DevicesListbox.Items.Add(args);
            }));
        }

        private void OnUpdatePenController(IPenClient sender, PenUpdateInformation args)
        {
            this.BeginInvoke(new MethodInvoker(delegate ()
            {
                foreach (PenInformation device in DevicesListbox.Items)
                {
                    if (device.Id == args.Id)
                    {
                        device.Update(args);
                    }
                }
            }));
        }

        private void OnStopSearch(IPenClient sender, Windows.Devices.Bluetooth.BluetoothError args)
        {
            this.BeginInvoke(new MethodInvoker(delegate ()
            {
                SearchButton.Enabled = true;
            }));        
        }

        #endregion

        private async void ConnectButton_Click(object sender, EventArgs e)
        {
            ConnectButton.Enabled = false;

            if (!(DevicesListbox.SelectedItem is PenInformation selected))
            {
                MessageBox.Show("Select your device.");
            }
            else
            {
                try
                {
                    if (!await bluetooth.Connect(selected))
                    {
                        MessageBox.Show("Connection failed.");
                    }
                }
                catch
                {
                }
            }
        }

        private async void DisconnectButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (bluetooth.Alive)
                {
                    await bluetooth.Disconnect();
                }
            }
            catch
            {
            }

            ConnectButton.Enabled = true;
        }

        private void ProcessDot(Dot dot)
        {
            // TODO: Drawing sample code
            if (dot.DotType == DotTypes.PEN_DOWN)
            {
                currentStroke = new Stroke(dot.Section, dot.Owner, dot.Note, dot.Page);
                currentStroke.Add(dot);
            }
            else if (dot.DotType == DotTypes.PEN_MOVE)
            {
                currentStroke.Add(dot);
            }
            else if (dot.DotType == DotTypes.PEN_UP)
            {
                currentStroke.Add(dot);
                DrawStroke(currentStroke);
            }
        }

        private void DrawStroke(Stroke stroke)
        {
            this.Invoke(new MethodInvoker(delegate ()
            {
                lock (drawLock)
                {
                    //603 Ring Note Height  5.52  5.41 	63.46 	88.88 
                    int dx = (int)((5.52 * canvasWidth) / 63.46);
                    int dy = (int)((5.41 * canvasHeight) / 88.88);
                    Renderer.Draw(canvasBitmap, stroke, (float)(canvasWidth / 63.46f), (float)(canvasHeight / 88.88f), -dx, -dy, 1, Color.FromArgb(200, Color.Blue));
                    pictureBox.Image = canvasBitmap;
                }
            }));
        }

        private async void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (bluetooth.Alive)
            {
                await bluetooth.Disconnect();
            }
            passwordInputForm.Dispose();
            progressForm.Dispose();

            bluetooth.onStopSearch -= OnStopSearch;
            bluetooth.onUpdatePenController -= OnUpdatePenController;
            bluetooth.onAddPenController -= OnAddPenController;

            controller.PenStatusReceived -= PenStatusReceived;
            controller.Connected -= Connected;
            controller.Disconnected -= Disconnected;
            controller.Authenticated -= Authenticated;
            controller.DotReceived -= DotReceived;
            controller.PasswordRequested -= PasswordRequested;
            controller.OfflineDataListReceived -= OfflineDataListReceived;

            controller.AutoPowerOffTimeChanged -= ConfigurationChanged;
            controller.AutoPowerOnChanged -= ConfigurationChanged;
            controller.RtcTimeChanged -= ConfigurationChanged;
            controller.SensitivityChanged -= ConfigurationChanged;
            controller.BeepSoundChanged -= ConfigurationChanged;
            controller.PenColorChanged -= ConfigurationChanged;

            controller.PasswordChanged -= PasswordChanged;

            controller.BatteryAlarmReceived -= BatteryAlarmReceived;

            controller.OfflineDataDownloadStarted -= OfflineDataDownloadStarted;
            controller.OfflineStrokeReceived -= OfflineStrokeReceived;
            controller.OfflineDownloadFinished -= OfflineDownloadFinished;
            controller.OfflineDataRemoved -= OfflineDataRemoved;

            controller.FirmwareInstallationStatusUpdated -= FirmwareInstallationStatusUpdated;
            controller.FirmwareInstallationFinished -= FirmwareInstallationFinished;

            controller.PenProfileReceived += PenProfileReceived;
        }

        private void DisplayProgress(string title, int total, int amountDone)
        {
            this.BeginInvoke(new MethodInvoker(delegate ()
            {
                lock (progressLock)
                {
                    progressForm.SetStatus(title, total, amountDone);
                    if (!progressForm.Visible)
                    {
                        progressForm.ShowDialog();
                    }
                }
            }));
        }

        private void CloseProgress()
        {
            this.BeginInvoke(new MethodInvoker(delegate ()
            {
                lock (progressLock)
                {
                    if (progressForm.Visible)
                    {
                        progressForm.Close();
                    }
                }
            }));
        }

        #region Pen Event 

        private void Connected(IPenClient sender, ConnectedEventArgs args)
        {
            ConsoleWrite("Successful connection with device");
            this.BeginInvoke(new MethodInvoker(delegate ()
            {
                ConnectButton.Enabled = false;
                PenInfoTextbox.Text = $"Mac: {args.MacAddress}\r\n\r\n" +
                $"Name: {args.DeviceName}\r\n\r\n" +
                $"SubName: {args.SubName}\r\n\r\n" +
                $"Firmware Version: {args.FirmwareVersion}\r\n\r\n" +
                $"Protocol Version: {args.ProtocolVersion}";
                SetGroupVisiblity(true);
            }));

            bluetooth.StopLEAdvertisementWatcher();
        }

        private void Authenticated(IPenClient sender, object args)
        {
            ConsoleWrite("Device authentication completed");
            controller.RequestPenStatus();
            controller.AddAvailableNote();
            controller.RequestOfflineDataList();

            if (!controller.IsSupportPenProfile())
            {
                ConsoleWrite("Pen profile is not supported on this device.");
            }
            else
            {
                ConsoleWrite("Pen profile is supported on this device.");
            }
            controller.GetProfileInfo(PEN_PROFILE_TEST_NAME);
        }

        private void Disconnected(IPenClient sender, object args)
        {
            ConsoleWrite("Terminated device connection");
            this.BeginInvoke(new MethodInvoker(delegate ()
            {
                OfflineDataListbox.Items.Clear();
                ConnectButton.Enabled = true;
                PenInfoTextbox.Text = "";
                PowerOffTimeInput.Value = 0;
                BeepCheckbox.Checked = false;
                OfflineDataCheckbox.Checked = false;
                PenCapPowerCheckbox.Checked = false;
                PenTipPowerOnCheckbox.Checked = false;
                PowerProgressBar.Value = 0;
                StorageProgressBar.Value =0;
                SetGroupVisiblity(false);
            }));

            CloseProgress();
        }

        private void DotReceived(IPenClient sender, DotReceivedEventArgs args)
        {
            ProcessDot(args.Dot);
        }

        private void PasswordRequested(IPenClient sender, PasswordRequestedEventArgs args)
        {
            ConsoleWrite($"Request password input. Number of attempts: {args.RetryCount}, Number of input limits: {args.ResetCount}");
            if (args.RetryCount >= args.ResetCount)
            {
                MessageBox.Show("The devices's data has been initialized.");
                return;
            }

            this.BeginInvoke(new MethodInvoker(delegate ()
            {
                passwordInputForm.SetStatus(args.RetryCount, args.ResetCount);
                passwordInputForm.ShowDialog();
            }));
        }

        private void OnInputPassword(string password)
        {
            ConsoleWrite($"Password '{password}' entered.");
            controller.InputPassword(password);
        }

        private void PenStatusReceived(IPenClient sender, PenStatusReceivedEventArgs args)
        {
            this.BeginInvoke(new MethodInvoker(delegate ()
            {
                PowerOffTimeInput.Value = args.AutoShutdownTime;
                BeepCheckbox.Checked = args.Beep;
                OfflineDataCheckbox.Checked = args.UseOfflineData;
                PenCapPowerCheckbox.Checked = args.PenCapPower;
                PenTipPowerOnCheckbox.Checked = args.AutoPowerOn;

                PowerProgressBar.Maximum = 100;
                PowerProgressBar.Value = args.Battery > 100 ? 100 : args.Battery;

                StorageProgressBar.Maximum = 100;
                StorageProgressBar.Value = args.UsedMem;
            }));
        }

        private void OfflineDataListReceived(IPenClient sender, OfflineDataListReceivedEventArgs args)
        {
            this.BeginInvoke(new MethodInvoker(delegate ()
            {
                OfflineDataListbox.Items.Clear();
                foreach (OfflineDataInfo item in args.OfflineNotes)
                {
                    OfflineDataListbox.Items.Add(item);
                }
            }));
        }

        private void OfflineDataDownloadStarted(IPenClient sender, object args)
        {
            DisplayProgress(PROGRESS_TITLE_OFFLINE, 100, 0);
        }

        private void OfflineStrokeReceived(IPenClient sender, OfflineStrokeReceivedEventArgs args)
        {
            foreach (Stroke stroke in args.Strokes)
            {
                DrawStroke(stroke);
            }
            DisplayProgress(PROGRESS_TITLE_OFFLINE, args.Total, args.AmountDone);
        }

        private void OfflineDownloadFinished(IPenClient sender, SimpleResultEventArgs args)
        {
            CloseProgress();
            controller.RequestOfflineDataList();
        }

        private void OfflineDataRemoved(IPenClient sender, SimpleResultEventArgs args)
        {
            controller.RequestOfflineDataList();
        }

        private void FirmwareInstallationFinished(IPenClient sender, SimpleResultEventArgs args)
        {
            CloseProgress();
        }

        private void FirmwareInstallationStatusUpdated(IPenClient sender, ProgressChangeEventArgs args)
        {
            DisplayProgress(PROGRESS_TITLE_UPDATE, args.Total, args.AmountDone);
        }

        private void PasswordChanged(IPenClient sender, SimpleResultEventArgs args)
        {
            MessageBox.Show("Setting the device password " + (args.Result ? "complete" : "failed"));
        }

        private void ConfigurationChanged(IPenClient sender, SimpleResultEventArgs args)
        {
            ConsoleWrite("Device's configuration is changed.");
            controller.RequestPenStatus();
        }

        private void BatteryAlarmReceived(IPenClient sender, BatteryAlarmReceivedEventArgs args)
        {
            ConsoleWrite("Low power warning. current power level is " + args.Battery);
            controller.RequestPenStatus();
        }

        #endregion

        #region pencontrol
        private void OfflineDataDownloadButton_Click(object sender, EventArgs e)
        {
            if (!(OfflineDataListbox.SelectedItem is OfflineDataInfo d))
            {
                MessageBox.Show("Select an offline data item.");
                return;
            }
            controller.RequestOfflineData(d.Section, d.Owner, d.Note);
        }

        private void OfflineDataDeleteButton_Click(object sender, EventArgs e)
        {
            if (!(OfflineDataListbox.SelectedItem is OfflineDataInfo))
            {
                MessageBox.Show("Select an offline data item.");
                return;
            }
            OfflineDataInfo data = OfflineDataListbox.SelectedItem as OfflineDataInfo;
            controller.RequestRemoveOfflineData(data.Section, data.Owner, new int[] { data.Note });
        }

        private void PowerOffTimeInput_ValueChanged(object sender, EventArgs e)
        {
            controller.SetAutoPowerOffTime((short)PowerOffTimeInput.Value);
        }

        private void PenCapPowerCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            controller.SetPenCapPowerOnOffEnable(PenCapPowerCheckbox.Checked);
        }

        private void PenTipPowerOnCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            controller.SetAutoPowerOnEnable(PenTipPowerOnCheckbox.Checked);
        }

        private void BeepCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            controller.SetBeepSoundEnable(BeepCheckbox.Checked);
        }

        private void OfflineDataCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            controller.SetOfflineDataEnable(OfflineDataCheckbox.Checked);
        }

        private void PasswordChangeButton_Click(object sender, EventArgs e)
        {
            controller.SetPassword(OldPasswordTextbox.Text ?? "1111", NewPasswordTextbox.Text);
        }

        private void SetGroupVisiblity(bool visible)
        {
            GroupInfo.Enabled = visible;
            GroupConfig.Enabled = visible;
            GroupOffline.Enabled = visible;
            GroupUpdate.Enabled = visible;
        }

        private async void UpgradeButton_Click(object sender, EventArgs e)
        {
            if (FirmwarePathTextbox.Text == "" || FirmwareVersionTextbox.Text == "")
            {
                MessageBox.Show("Select firmware file and enter firmware version.");
                return;
            }

            Compressible? compression = null;
            if (CompressOkRadio.Checked)
            {
                compression = Compressible.Enabled;
            }
            if (CompressNoRadio.Checked)
            {
                compression = Compressible.Unabled;
            }

            controller.RequestFirmwareInstallation(
                file: await StorageFile.GetFileFromPathAsync(FirmwarePathTextbox.Text), 
                version: FirmwareVersionTextbox.Text,
                forceCompression: compression
            );
        }

        private void FirmwarePathTextbox_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                Filter = "Firmware Files (*._v_)|*._v_|Firmware Files (*.bin)|*.bin|All Files|*",
                Title = "Select a Firmware File"
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FirmwarePathTextbox.Text = openFileDialog1.FileName;
            }
        }

        #endregion

        private static readonly string PEN_PROFILE_TEST_NAME = "neolab_t";
        private static readonly byte[] PEN_PROFILE_TEST_PASSWORD = { 0x3E, 0xD5, 0x95, 0x25, 0x06, 0xF7, 0x83, 0xDD };

        private PenProfileReceivedEventArgs lastArgs;

        private void PenProfileReceived(IPenClient sender, PenProfileReceivedEventArgs args)
        {
            if (args.Result == PenProfileReceivedEventArgs.ResultType.Failed)
            {
                ConsoleWrite("PenProfile Failed");
                return;
            }

            switch (args.Type)
            {
                case PenProfileReceivedEventArgs.PenProfileType.Create:
                    CreateProfileResultReceived(args);
                    break;
                case PenProfileReceivedEventArgs.PenProfileType.Delete:
                    DeleteProfileResultReceived(args);
                    break;
                case PenProfileReceivedEventArgs.PenProfileType.Info:
                    ProfileInfoReceived(args);
                    break;
                case PenProfileReceivedEventArgs.PenProfileType.ReadValue:
                    ReadProfileValueResultReceived(args);
                    break;
                case PenProfileReceivedEventArgs.PenProfileType.WriteValue:
                    WriteProfileValueResultReceived(args);
                    break;
                case PenProfileReceivedEventArgs.PenProfileType.DeleteValue:
                    DeleteProfileValueResultReceived(args);
                    break;
            }
        }

        private void CreateProfileResultReceived(PenProfileReceivedEventArgs penProfileReceivedEventArgs)
        {
            switch (penProfileReceivedEventArgs.Status)
            {
                case PenProfile.PROFILE_STATUS_SUCCESS:
                    ConsoleWrite($"Create Success:{penProfileReceivedEventArgs.ProfileName}");
                    GroupProfile.Enabled = true;
                    break;
                case PenProfile.PROFILE_STATUS_FAILURE:
                    ConsoleWrite($"Create Failure:{penProfileReceivedEventArgs.ProfileName}");
                    break;
                case PenProfile.PROFILE_STATUS_EXIST_PROFILE_ALREADY:
                    ConsoleWrite($"Already existed profile name:{penProfileReceivedEventArgs.ProfileName}");
                    break;
                case PenProfile.PROFILE_STATUS_NO_PERMISSION:
                    ConsoleWrite("Permission Denied. Check your password");
                    break;
                default:
                    ConsoleWrite("Create Error " + penProfileReceivedEventArgs.Status);
                    break;
            }
        }

        private void DeleteProfileResultReceived(PenProfileReceivedEventArgs penProfileReceivedEventArgs)
        {
            switch (penProfileReceivedEventArgs.Status)
            {
                case PenProfile.PROFILE_STATUS_SUCCESS:
                    ConsoleWrite($"Delete Success:{penProfileReceivedEventArgs.ProfileName}");
                    break;
                case PenProfile.PROFILE_STATUS_FAILURE:
                    ConsoleWrite($"Delete Failure:{penProfileReceivedEventArgs.ProfileName}");
                    break;
                case PenProfile.PROFILE_STATUS_NO_EXIST_PROFILE:
                    ConsoleWrite($"Do not exist profile:{penProfileReceivedEventArgs.ProfileName}");
                    break;
                case PenProfile.PROFILE_STATUS_NO_PERMISSION:
                    ConsoleWrite("Permission Denied. Check your password");
                    break;
                default:
                    ConsoleWrite("Delete error " + penProfileReceivedEventArgs.Status);
                    break;
            }
        }

        private void ProfileInfoReceived(PenProfileReceivedEventArgs penProfileReceivedEventArgs)
        {
            switch (penProfileReceivedEventArgs.Status)
            {
                case PenProfile.PROFILE_STATUS_SUCCESS:
                    {
                        var args = penProfileReceivedEventArgs as PenProfileInfoEventArgs;
                        System.Text.StringBuilder strs = new System.Text.StringBuilder();
                        strs.Append($"Total Section Count : {args.TotalSectionCount}");
                        strs.Append(Environment.NewLine);
                        strs.Append($"Section Size : {args.SectionSize}");
                        strs.Append(Environment.NewLine);
                        strs.Append($"Using Section Count : {args.UseSectionCount}");
                        strs.Append(Environment.NewLine);
                        strs.Append($"using Key count : {args.UseKeyCount}");
                        ConsoleWrite(strs.ToString());
                    }
                    GroupProfile.Enabled = true;
                    break;
                case PenProfile.PROFILE_STATUS_FAILURE:
                    ConsoleWrite($"Get Info Failure:{penProfileReceivedEventArgs.ProfileName}");
                    break;
                case PenProfile.PROFILE_STATUS_NO_EXIST_PROFILE:
                    ConsoleWrite($"Do not exist profile:{penProfileReceivedEventArgs.ProfileName}");
                    controller.CreateProfile(PEN_PROFILE_TEST_NAME, PEN_PROFILE_TEST_PASSWORD);
                    break;
                default:
                    ConsoleWrite("Info Error " + penProfileReceivedEventArgs.Status);
                    break;
            }
        }

        private void ReadProfileValueResultReceived(PenProfileReceivedEventArgs penProfileReceivedEventArgs)
        {
            var args = penProfileReceivedEventArgs as PenProfileReadValueEventArgs;
            foreach (var value in args.Data)
            {
                switch (value.Status)
                {
                    case PenProfile.PROFILE_STATUS_SUCCESS:
                        ConsoleWrite($"key : {value.Key}, Value : {System.Text.Encoding.UTF8.GetString(value.Data)}");
                        break;
                    case PenProfile.PROFILE_STATUS_FAILURE:
                        ConsoleWrite($"Read value Failure:key[{value.Key}]");
                        break;
                    case PenProfile.PROFILE_STATUS_NO_EXIST_PROFILE:
                        ConsoleWrite($"Do not exist profile:{penProfileReceivedEventArgs.ProfileName}");
                        break;
                    case PenProfile.PROFILE_STATUS_NO_EXIST_KEY:
                        ConsoleWrite($"Do not exist key:[{value.Key}]");
                        break;
                    case PenProfile.PROFILE_STATUS_NO_PERMISSION:
                        ConsoleWrite("Permission Denied. Check your password");
                        break;
                    default:
                        ConsoleWrite("Read value Error " + penProfileReceivedEventArgs.Status);
                        break;
                }
            }
        }

        private void WriteProfileValueResultReceived(PenProfileReceivedEventArgs penProfileReceivedEventArgs)
        {
            var args = penProfileReceivedEventArgs as PenProfileWriteValueEventArgs;
            foreach (var value in args.Data)
            {
                switch (value.Status)
                {
                    case PenProfile.PROFILE_STATUS_SUCCESS:
                        ConsoleWrite($"Write Success key:[{value.Key}]");
                        break;
                    case PenProfile.PROFILE_STATUS_FAILURE:
                        ConsoleWrite($"Write value Failure key:[{value.Key}]");
                        break;
                    case PenProfile.PROFILE_STATUS_NO_EXIST_PROFILE:
                        ConsoleWrite($"Do not exist profile:{penProfileReceivedEventArgs.ProfileName}");
                        break;
                    case PenProfile.PROFILE_STATUS_NO_EXIST_KEY:
                        ConsoleWrite($"Do not exist key:[{value.Key}]");
                        break;
                    case PenProfile.PROFILE_STATUS_NO_PERMISSION:
                        ConsoleWrite("Permission Denied. Check your password");
                        break;
                    default:
                        ConsoleWrite("Write value Error " + penProfileReceivedEventArgs.Status);
                        break;
                }
            }
        }

        private void DeleteProfileValueResultReceived(PenProfileReceivedEventArgs penProfileReceivedEventArgs)
        {
            var args = penProfileReceivedEventArgs as PenProfileDeleteValueEventArgs;
            foreach (var value in args.Data)
            {
                switch (value.Status)
                {
                    case PenProfile.PROFILE_STATUS_SUCCESS:
                        ConsoleWrite($"Delete Success key:[{value.Key}]");
                        break;
                    case PenProfile.PROFILE_STATUS_FAILURE:
                        ConsoleWrite($"Delete value Failure key:[{value.Key}]");
                        break;
                    case PenProfile.PROFILE_STATUS_NO_EXIST_PROFILE:
                        ConsoleWrite($"Do not exist profile:{penProfileReceivedEventArgs.ProfileName}");
                        break;
                    case PenProfile.PROFILE_STATUS_NO_EXIST_KEY:
                        ConsoleWrite($"Do not exist key:[{value.Key}]");
                        break;
                    case PenProfile.PROFILE_STATUS_NO_PERMISSION:
                        ConsoleWrite("Permission Denied. Check your password");
                        break;
                    default:
                        ConsoleWrite("Delete value Error " + penProfileReceivedEventArgs.Status);
                        break;
                }
            }
        }

        private void ProfileGetButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ProfileKeyTextbox.Text) )
            {
                MessageBox.Show("Enter the key of the profile you want to read.");
            }
            controller.ReadProfileValues(PEN_PROFILE_TEST_NAME, new string[] { ProfileKeyTextbox.Text });
        }

        private void ProfileAddButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ProfileKeyTextbox.Text) || string.IsNullOrEmpty(ProfileValueTextbox.Text))
            {
                MessageBox.Show("Enter the key and value of the profile you want to create.");
            }
            controller.WriteProfileValues(PEN_PROFILE_TEST_NAME, PEN_PROFILE_TEST_PASSWORD, new string[] { ProfileKeyTextbox.Text }, new byte[][] { System.Text.Encoding.UTF8.GetBytes(ProfileValueTextbox.Text) });
        }

        private void ProfileDeleteButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ProfileKeyTextbox.Text))
            {
                MessageBox.Show("Enter the key of the profile you want to delete.");
            }
            controller.DeleteProfileValues(PEN_PROFILE_TEST_NAME, PEN_PROFILE_TEST_PASSWORD, new string[] { ProfileKeyTextbox.Text });
        }
    }
}
