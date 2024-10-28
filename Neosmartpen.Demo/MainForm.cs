using Neosmartpen.Net;
using Neosmartpen.Net.Bluetooth;
using Neosmartpen.Net.Support;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
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

        private ProgressForm progressForm;
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

        private void log(string message)
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
                    bool result = await bluetooth.Connect(selected);
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
                if (bluetooth != null)
                {
                    if (bluetooth.Alive)
                    {
                        await bluetooth.Disconnect();
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
            if (bluetooth != null)
            {
                if (bluetooth.Alive)
                {
                    await bluetooth.Disconnect();
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
            log("connected");
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

            bluetooth.StopLEAdvertisementWatcher();
        }

        private void Authenticated(IPenClient sender, object args)
        {
            log("authenticated");
            controller.RequestPenStatus();
            controller.AddAvailableNote();
            controller.RequestOfflineDataList();
        }

        private void Disconnected(IPenClient sender, object args)
        {
            log("disconnected");
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
            log("Configuration is changed");
            controller.RequestPenStatus();
        }

        private void BatteryAlarmReceived(IPenClient sender, BatteryAlarmReceivedEventArgs args)
        {
            log("Low power warning. current power level is " + args.Battery);
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
            controller.SetPassword(OldPasswordTextbox.Text, NewPasswordTextbox.Text);
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
            controller.RequestFirmwareInstallation(file, FirmwareVersionTextbox.Text);
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
