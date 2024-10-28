using Neosmartpen.Net;
using Neosmartpen.Net.Bluetooth;
using Neosmartpen.Net.Support;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using Windows.Storage;
using System.Threading.Tasks;

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

        private ProgressForm progressForm;
        private readonly PasswordInputForm passwordInputForm;

        private readonly GenericBluetoothPenClient bluetoothClient;
        private readonly PenController penController;

        public const string PROGRESS_TITLE_OFFLINE = "Download Offline Data";
        public const string PROGRESS_TITLE_UPDATE = "Firmware Update";

        private readonly object progressLock = new object();

        public MainForm()
        {
            InitializeComponent();

            canvasWidth = pictureBox.Width;
            canvasHeight = pictureBox.Height;

            canvasBitmap = new Bitmap(pictureBox.Width, pictureBox.Height);

            passwordInputForm = new PasswordInputForm(OnInputPassword);

            // create PenController instance.
            penController = new PenController();

            // Create BluetoothPenClient instance. and bind PenController.
            // BluetoothPenClient is implementation of bluetooth function.
            bluetoothClient = new GenericBluetoothPenClient(penController);

            // bluetooth advertisement event
            bluetoothClient.onStopSearch += OnStopSearch;
            bluetoothClient.onUpdatePenController += OnUpdatePenController;
            bluetoothClient.onAddPenController += OnAddPenController;

            // pen controller event
            penController.PenStatusReceived += PenStatusReceived;
            penController.Connected += Connected;
            penController.Disconnected += Disconnected;
            penController.Authenticated += Authenticated;
            penController.DotReceived += DotReceived;
            penController.PasswordRequested += PasswordRequested;
            penController.OfflineDataListReceived += OfflineDataListReceived;

            penController.AutoPowerOffTimeChanged += ConfigurationChanged;
            penController.AutoPowerOnChanged += ConfigurationChanged;
            penController.RtcTimeChanged += ConfigurationChanged;
            penController.SensitivityChanged += ConfigurationChanged;
            penController.BeepSoundChanged += ConfigurationChanged;
            penController.PenColorChanged += ConfigurationChanged;

            penController.PasswordChanged += PasswordChanged;

            penController.BatteryAlarmReceived += BatteryAlarmReceived;

            penController.OfflineDataDownloadStarted += OfflineDataDownloadStarted;
            penController.OfflineStrokeReceived += OfflineStrokeReceived;
            penController.OfflineDownloadFinished += OfflineDownloadFinished;
            penController.OfflineDataRemoved += OfflineDataRemoved;

            penController.FirmwareInstallationStatusUpdated += FirmwareInstallationStatusUpdated;
            penController.FirmwareInstallationFinished += FirmwareInstallationFinished;
        }

        private void AppendLog(string message)
        {
            this.BeginInvoke(new MethodInvoker(delegate ()
            {
                if (ConsoleTextbox.Text != "")
                {
                    ConsoleTextbox.AppendText("\r\n");
                }
                ConsoleTextbox.AppendText(message);
            }));
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            SearchButton.Enabled = false;
            DevicesListbox.Items.Clear();
            bluetoothClient.StartLEAdvertisementWatcher();
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
            }));        }

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
                    bool result = await bluetoothClient.Connect(selected);
                    if (!result)
                    {
                        MessageBox.Show("Connection failed.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("conection exception : " + ex.Message);
                    Debug.WriteLine("conection exception : " + ex.StackTrace);
                }
            }
        }

        private async void DisconnectButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (bluetoothClient != null)
                {
                    if (bluetoothClient.Alive)
                    {
                        await bluetoothClient.Disconnect();
                    }
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
            if (bluetoothClient != null)
            {
                if (bluetoothClient.Alive)
                {
                    await bluetoothClient.Disconnect();
                }
            }
        }

        private void DisplayProgress(string title, int total, int amountDone)
        {
            this.BeginInvoke(new MethodInvoker(delegate ()
            {
                lock (progressLock)
                {
                    if (progressForm == null)
                    {
                        progressForm = new ProgressForm();
                    }

                    progressForm?.SetStatus(title, total, amountDone);

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
                    if (progressForm != null && progressForm.Visible)
                    {
                        progressForm.Close();
                        progressForm.Dispose();
                        progressForm = null;
                    }
                }
            }));
        }

        #region Pen Event 

        private void Connected(IPenClient sender, ConnectedEventArgs args)
        {
            AppendLog("connected");
            this.BeginInvoke(new MethodInvoker(delegate ()
            {
                ConnectButton.Enabled = false;
                PenInfoTextbox.Text = String.Format(
                    format: "Mac: {0}\r\n\r\nName: {1}\r\n\r\nSubName: {2}\r\n\r\nFirmware Version: {3}\r\n\r\nProtocol Version: {4}", 
                    args.MacAddress, 
                    args.DeviceName, 
                    args.SubName, 
                    args.FirmwareVersion,
                    args.ProtocolVersion
                );
                SetGroupVisiblity(true);
            }));

            bluetoothClient.StopLEAdvertisementWatcher();
        }

        private void Authenticated(IPenClient sender, object args)
        {
            AppendLog("authenticated");
            penController.RequestPenStatus();
            penController.AddAvailableNote();
            penController.RequestOfflineDataList();
        }

        private void Disconnected(IPenClient sender, object args)
        {
            AppendLog("disconnected");
            this.BeginInvoke(new MethodInvoker(delegate ()
            {
                OfflineDataListbox.Items.Clear();
                ConnectButton.Enabled = true;
                PenInfoTextbox.Text = "";
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
            penController.InputPassword(password);
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
            penController.RequestOfflineDataList();
        }

        private void OfflineDataRemoved(IPenClient sender, SimpleResultEventArgs args)
        {
            penController.RequestOfflineDataList();
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
            AppendLog("configuration is changed");
            penController.RequestPenStatus();
        }

        private void BatteryAlarmReceived(IPenClient sender, BatteryAlarmReceivedEventArgs args)
        {
            AppendLog("low power warning. current power level is " + args.Battery);
            penController.RequestPenStatus();
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
            penController.RequestOfflineData(d.Section, d.Owner, d.Note);
        }

        private void OfflineDataDeleteButton_Click(object sender, EventArgs e)
        {
            if (!(OfflineDataListbox.SelectedItem is OfflineDataInfo))
            {
                MessageBox.Show("Select an offline data item.");
                return;
            }
            OfflineDataInfo data = OfflineDataListbox.SelectedItem as OfflineDataInfo;
            penController.RequestRemoveOfflineData(data.Section, data.Owner, new int[] { data.Note });
        }

        private void PowerOffTimeInput_ValueChanged(object sender, EventArgs e)
        {
            penController.SetAutoPowerOffTime((short)PowerOffTimeInput.Value);
        }

        private void PenCapPowerCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            penController.SetPenCapPowerOnOffEnable(PenCapPowerCheckbox.Checked);
        }

        private void PenTipPowerOnCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            penController.SetAutoPowerOnEnable(PenTipPowerOnCheckbox.Checked);
        }

        private void BeepCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            penController.SetBeepSoundEnable(BeepCheckbox.Checked);
        }

        private void OfflineDataCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            penController.SetOfflineDataEnable(OfflineDataCheckbox.Checked);
        }

        private void PasswordChangeButton_Click(object sender, EventArgs e)
        {
            penController.SetPassword(OldPasswordTextbox.Text, NewPasswordTextbox.Text);
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
            var file = await StorageFile.GetFileFromPathAsync(FirmwarePathTextbox.Text);
            penController.RequestFirmwareInstallation(file, FirmwareVersionTextbox.Text);
        }

        private void FirmwarePathTextbox_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                Filter = "Firmware Files|*._v_",
                Title = "Select a Firmware File"
            };

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FirmwarePathTextbox.Text = openFileDialog1.FileName;
            }
        }

        #endregion
    }
}
