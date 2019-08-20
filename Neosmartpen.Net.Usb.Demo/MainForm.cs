using Neosmartpen.Net.Usb.Exceptions;
using System;
using System.IO;
using System.Windows.Forms;

namespace Neosmartpen.Net.Usb.Demo
{
    public partial class MainForm : Form
    {
        private string LocalDirectory = null;

        private UsbAdapter usbAdapter;

        private UpdateForm updateForm;

        public MainForm()
        {
            InitializeComponent();

            // Create a UsbAdapter instance.
            usbAdapter = UsbAdapter.GetInstance();

            // Implement an event handler to receive pen connection events.
            usbAdapter.Connected += UsbAdapter_Connected;
            usbAdapter.Disconnected += UsbAdapter_Disconnected;

            TreeNode root = this.tvLocalDir.Nodes.Add("PC");
            string[] drives = Directory.GetLogicalDrives();
            foreach (string drive in drives)
            {
                TreeNode node = root.Nodes.Add(drive);
                node.Nodes.Add("@%");
            }
        }

        private void UsbAdapter_Connected(object sender, Events.ConnectionStatusChangedEventArgs e)
        {
            // When the pen is connected, it implements event handlers for the pen's various functions.
            var usbPenComm = e.UsbPenComm;
            usbPenComm.OfflineFileListReceived += UsbPenComm_FileListReceived;
            usbPenComm.LogFileListReceived += UsbPenComm_FileListReceived;
            usbPenComm.FileInfoReceived += UsbPenComm_FileInfoReceived;
            usbPenComm.FileDownloadProgressChanged += UsbPenComm_FileDownloadProgressChanged;
            usbPenComm.FileDownloadResultReceived += UsbPenComm_FileDownloadResultReceived;
            usbPenComm.DeleteFileResultReceived += UsbPenComm_DeleteFileResultReceived;
            usbPenComm.BatteryStatusReceived += UsbPenComm_BatteryStatusReceived;
            usbPenComm.StorageStatusReceived += UsbPenComm_StorageStatusReceived;
            usbPenComm.DateTimeReceived += UsbPenComm_DateTimeReceived;
            usbPenComm.ConfigSetupResultReceived += UsbPenComm_ConfigSetupResultReceived;
            usbPenComm.FormatResultReceived += UsbPenComm_FormatResultReceived;
            usbPenComm.PowerOffResultReceived += UsbPenComm_PowerOffResultReceived;
            usbPenComm.UpdateProgressChanged += UsbPenComm_UpdateProgressChanged;
            usbPenComm.UpdateResultReceived += UsbPenComm_UpdateResultReceived;

            this.BeginInvoke(new MethodInvoker(delegate ()
            {
                ListViewItem item = new ListViewItem(new string[]
                {
                    usbPenComm.PortName,
                    usbPenComm.DeviceName,
                    usbPenComm.MacAddress
                });
                item.Tag = usbPenComm.PortName;
                lvUsbPens.Items.Add(item);
            }));
        }

        private void UsbAdapter_Disconnected(object sender, Events.ConnectionStatusChangedEventArgs e)
        {
            // When the pen is disconnected, you must remove the attached event handler.
            var usbPenComm = e.UsbPenComm;
            usbPenComm.OfflineFileListReceived -= UsbPenComm_FileListReceived;
            usbPenComm.LogFileListReceived -= UsbPenComm_FileListReceived;
            usbPenComm.FileInfoReceived -= UsbPenComm_FileInfoReceived;
            usbPenComm.FileDownloadProgressChanged -= UsbPenComm_FileDownloadProgressChanged;
            usbPenComm.FileDownloadResultReceived -= UsbPenComm_FileDownloadResultReceived;
            usbPenComm.DeleteFileResultReceived -= UsbPenComm_DeleteFileResultReceived;
            usbPenComm.BatteryStatusReceived -= UsbPenComm_BatteryStatusReceived;
            usbPenComm.StorageStatusReceived -= UsbPenComm_StorageStatusReceived;
            usbPenComm.DateTimeReceived -= UsbPenComm_DateTimeReceived;
            usbPenComm.ConfigSetupResultReceived -= UsbPenComm_ConfigSetupResultReceived;
            usbPenComm.FormatResultReceived -= UsbPenComm_FormatResultReceived;
            usbPenComm.PowerOffResultReceived -= UsbPenComm_PowerOffResultReceived;
            usbPenComm.UpdateProgressChanged -= UsbPenComm_UpdateProgressChanged;
            usbPenComm.UpdateResultReceived -= UsbPenComm_UpdateResultReceived;

            this.BeginInvoke(new MethodInvoker(delegate ()
            {
                for (int i = 0; i < lvUsbPens.Items.Count; i++)
                {
                    if ((string)lvUsbPens.Items[i].Tag == e.UsbPenComm.PortName)
                    {
                        lvUsbPens.Items.RemoveAt(i);
                        break;
                    }
                }

                lvPenFiles.Items.Clear();
                lvPenFiles.Enabled = false;
                lbFirmwareVersion.Text = "";
                lbDateTime.Text = "";
                nudAutoPowerOffTime.Value = 5;
                cbAutoPowerOn.Checked = false;
                cbBeep.Checked = false;
                cbDownsampling.Checked = false;
                cbPenCapOff.Checked = false;
                cbSaveOfflineData.Checked = false;
                pbBattery.Value = 0;
                gbSelectedPen.Enabled = false;

                if (updateForm != null && updateForm.Visible)
                {
                    updateForm.Hide();
                    updateForm = null;
                }
            }));
        }

        private void UsbPenComm_UpdateResultReceived(object sender, Events.UpdateResultReceivedEventArgs e)
        {
            if (e.Result == Usb.Events.UpdateResultReceivedEventArgs.ResultType.Complete)
                MessageBox.Show("Firmware updated");
            else
                MessageBox.Show("Firmware update failed");
        }

        private void UsbPenComm_UpdateProgressChanged(object sender, Events.ProgressChangedEventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(delegate ()
            {
                if (updateForm != null)
                {
                    updateForm.SetStatus(100, e.Progress);
                }
            }));
        }

        private void UsbPenComm_PowerOffResultReceived(object sender, Events.ResultReceivedEventArgs e)
        {
            if (e.Result == Usb.Events.ResultReceivedEventArgs.ResultType.Success)
                MessageBox.Show("Power off");
            else
                MessageBox.Show("Power off failed");
        }

        private void UsbPenComm_FormatResultReceived(object sender, Events.ResultReceivedEventArgs e)
        {
            if (e.Result == Usb.Events.ResultReceivedEventArgs.ResultType.Success)
                MessageBox.Show("Pen is formatted");
            else
                MessageBox.Show("Format failed");
        }

        private void UsbPenComm_ConfigSetupResultReceived(object sender, Events.ConfigSetupResultReceivedEventArgs e)
        {
            UsbPenComm penComm = sender as UsbPenComm;

            this.BeginInvoke(new MethodInvoker(delegate ()
            {
                var pen = GetSelectedPen();

                if (pen == null || penComm.PortName != pen.PortName)
                    return;

                switch (e.Type)
                {
                    case ConfigType.AutoPowerOffTime:
                        nudAutoPowerOffTime.Value = (short)penComm.AutoPowerOffTime.TotalMinutes;
                        break;
                    case ConfigType.AutoPowerOn:
                        cbAutoPowerOn.Checked = penComm.IsAutoPowerOnEnabled;
                        break;

                    case ConfigType.PenCapOff:
                        cbPenCapOff.Checked = penComm.IsPenCapOffEnabled;
                        break;

                    case ConfigType.Beep:
                        cbBeep.Checked = penComm.IsBeepEnabled;
                        break;

                    case ConfigType.SaveOfflineData:
                        cbSaveOfflineData.Checked = penComm.CanSaveOfflineData;
                        break;

                    case ConfigType.DownSampling:
                        cbDownsampling.Checked = penComm.IsDownsamplingEnabled;
                        break;
                }
            }));
        }

        private void UsbPenComm_DateTimeReceived(object sender, Events.DateTimeReceivedEventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(delegate ()
            {
                lbDateTime.Text = e.DateTime.ToString();
            }));
        }

        private void UsbPenComm_StorageStatusReceived(object sender, Events.StorageStatusReceivedEventArgs e)
        {
        }

        private void UsbPenComm_BatteryStatusReceived(object sender, Events.BatteryStatusReceivedEventArgs e)
        {
            UsbPenComm penComm = sender as UsbPenComm;

            this.BeginInvoke(new MethodInvoker(delegate ()
            {
                var pen = GetSelectedPen();

                if (pen == null || penComm.PortName != pen.PortName)
                    return;

                pbBattery.Value = e.Battery;
            }));
        }

        private void UsbPenComm_DeleteFileResultReceived(object sender, Events.ResultReceivedEventArgs e)
        {
            if (e.Result == Usb.Events.ResultReceivedEventArgs.ResultType.Success)
            {
                this.BeginInvoke(new MethodInvoker(delegate ()
                {
                    string file;
                    UsbPenComm pen = sender as UsbPenComm;

                    if ((file = GetSelectedPenFile()) == null)
                    {
                        return;
                    }
                    if (file.EndsWith(".csv"))
                        pen.GetLogFileListRequest();
                    else
                        pen.GetOfflineFileListRequest();
                }));
                MessageBox.Show("File is deleted");
            }
            else
                MessageBox.Show("File delete failed");
        }

        private void UsbPenComm_FileDownloadResultReceived(object sender, Events.FileDownloadResultReceivedEventArgs e)
        {
            if (e.Result == Usb.Events.FileDownloadResultReceivedEventArgs.ResultType.Success)
            {
                this.BeginInvoke(new MethodInvoker(delegate ()
                {
                    InitLocalFileList();
                }));
                MessageBox.Show("File downloaded");
            }
            else
                MessageBox.Show("File download failed");
        }

        private void UsbPenComm_FileDownloadProgressChanged(object sender, Events.ProgressChangedEventArgs e)
        {
        }

        private void UsbPenComm_FileInfoReceived(object sender, Events.FileInfoReceivedEventArgs e)
        {
        }

        private void UsbPenComm_FileListReceived(object sender, Events.FileListReceivedEventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(delegate ()
            {
                lvPenFiles.Items.Clear();
                ListViewItem item2 = new ListViewItem(new string[]
                {
                "..", ""
                });
                item2.Tag = "Parent";
                lvPenFiles.Items.Add(item2);

                foreach (var file in e.Files)
                {
                    ListViewItem item3 = new ListViewItem(new string[]
                    {
                        file, "file"
                    });
                    item3.Tag = file;
                    lvPenFiles.Items.Add(item3);
                }
            }));
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Allows you to search and communicate with a pen connected via USB.
            usbAdapter.SearchAndConnect();

            // Launch Watcher to detect when the pen is connected or disconnected via USB.
            usbAdapter.StartWatcher();
        }

        private void InitLocalFileList()
        {
            try
            {
                this.lvLocalFiles.Items.Clear();

                string[] files = Directory.GetFiles(LocalDirectory);

                foreach (string file in files)
                {
                    FileInfo info = new FileInfo(file);

                    ListViewItem item = new ListViewItem(new string[]
                    {
                        info.Name,
                        info.Length.ToString(),
                        info.Extension,
                        info.LastWriteTime.ToString()
                    });
                    item.Tag = info.Name;
                    lvLocalFiles.Items.Add(item);
                }
            }
            catch (Exception)
            {
            }
        }

        private void tvLocalDir_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode current = e.Node;
            if (current.Level < 1) return;
            string path = current.FullPath;
            LocalDirectory = path.Substring(path.IndexOf("\\") + 1) + "\\";

            InitLocalFileList();
        }

        private void tvLocalDir_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode current = e.Node;

            if (current.Nodes.Count == 1 && current.Nodes[0].Text.Equals("@%"))
            {
                current.Nodes.Clear();

                String path = current.FullPath.Substring(current.FullPath.IndexOf("\\") + 1);

                try
                {
                    string[] directories = Directory.GetDirectories(path);
                    foreach (string directory in directories)
                    {
                        TreeNode newNode = current.Nodes.Add(
                            directory.Substring(
                            directory.LastIndexOf("\\") + 1));
                        newNode.Nodes.Add("@%");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void lvUsbPens_SelectedIndexChanged(object sender, EventArgs e)
        {
            gbSelectedPen.Enabled = true;
            lvPenFiles.Enabled = true;
            InitPenFileList();
            InitPenConfig();
        }

        private void InitPenConfig()
        {
            var pen = GetSelectedPen();

            if (pen == null)
                return;

            nudAutoPowerOffTime.Value = (short)pen.AutoPowerOffTime.TotalMinutes;
            cbAutoPowerOn.Checked = pen.IsAutoPowerOnEnabled;
            cbPenCapOff.Checked = pen.IsPenCapOffEnabled;
            cbBeep.Checked = pen.IsBeepEnabled;
            cbSaveOfflineData.Checked = pen.CanSaveOfflineData;
            cbDownsampling.Checked = pen.IsDownsamplingEnabled;

            pen.GetDateTimeRequest();
            lbFirmwareVersion.Text = "F/W Ver. " + pen.FirmwareVersion;
            pen.GetBatteryStatusRequest();
            pen.GetStorageStatusRequest();
        }

        private void InitPenFileList()
        {
            lvPenFiles.Items.Clear();
            ListViewItem item2 = new ListViewItem(new string[]
            {
                "data",
                "folder"
            });
            item2.Tag = "Data";
            lvPenFiles.Items.Add(item2);
            ListViewItem item3 = new ListViewItem(new string[]
            {
                "log",
                "folder"
            });
            item3.Tag = "Log";
            lvPenFiles.Items.Add(item3);
        }

        private void lvPenFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvPenFiles.SelectedItems.Count <= 0)
                return;

            string tag = (string)lvPenFiles.SelectedItems[0].Tag;

            if (tag == "Data" || tag == "Log")
            {
                string port = (string)lvUsbPens.SelectedItems[0].Tag;

                var penComm = usbAdapter.GetUsbPenComm(port);

                if (penComm != null)
                {
                    if (tag == "Data")
                        penComm.GetOfflineFileListRequest();
                    else
                        penComm.GetLogFileListRequest();
                }
            }
            else if (tag == "Parent")
            {
                InitPenFileList();
            }
        }

        private UsbPenComm GetSelectedPen()
        {
            if (lvUsbPens.SelectedItems.Count <= 0)
            {
                return null;
            }
            else
            {
                var port = (string)lvUsbPens.SelectedItems[0].Tag;
                return usbAdapter.GetUsbPenComm(port);
            }
        }

        private string GetSelectedPenFile()
        {
            if (lvPenFiles.SelectedItems.Count <= 0)
            {
                return null;
            }
            else
            {
                return (string)lvPenFiles.SelectedItems[0].Tag;
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            string file;
            UsbPenComm pen;

            if ((pen = GetSelectedPen()) == null || (file = GetSelectedPenFile()) == null)
            {
                MessageBox.Show("Please select pen and select file");
                return;
            }

            if (string.IsNullOrEmpty(LocalDirectory))
            {
                MessageBox.Show("Please select local folder");
                return;
            }

            pen.GetFileDataRequest(file.EndsWith(".csv") ? FileType.Log : FileType.Data, file, LocalDirectory);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string file;
            UsbPenComm pen;

            if ((pen = GetSelectedPen()) == null || (file = GetSelectedPenFile()) == null)
            {
                MessageBox.Show("Please select pen and select file");
                return;
            }

            pen.DeleteFileRequest(file.EndsWith(".csv") ? FileType.Log : FileType.Data, file);
        }

        private void btnFormat_Click(object sender, EventArgs e)
        {
            UsbPenComm pen;

            if ((pen = GetSelectedPen()) == null)
            {
                MessageBox.Show("Please select pen");
                return;
            }

            pen.FormatRequest();
        }

        private void btnPowerOff_Click(object sender, EventArgs e)
        {
            UsbPenComm pen;

            if ((pen = GetSelectedPen()) == null)
            {
                MessageBox.Show("Please select pen");
                return;
            }

            pen.PowerOffRequest();
        }

        private void btnFirmwareUpdate_Click(object sender, EventArgs e)
        {
            UsbPenComm pen;

            if ((pen = GetSelectedPen()) == null)
            {
                MessageBox.Show("Please select pen");
                return;
            }

            updateForm = new UpdateForm(pen.PortName, (string portName, string filePath, string firmwareVersion) => {
                try
                {
                    pen.UpdateRequest(firmwareVersion, filePath);
                }
                catch (FirmwareVersionIsTooLongException)
                {
                    MessageBox.Show("Firmware version is not invalid");
                }
                catch (FileCannotLoadException)
                {
                    MessageBox.Show("Firmware file cannot open");
                }
            });
            updateForm.ShowDialog();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Handle when app is closed
            // You should retrieve all UsbPenComm and UsbAdapter objects that exist and disconnect events and Dispose.
            usbAdapter.Connected -= UsbAdapter_Connected;
            usbAdapter.Disconnected -= UsbAdapter_Disconnected;

            foreach(var usbPenComm in usbAdapter.UsbPenComms)
            {
                usbPenComm.OfflineFileListReceived -= UsbPenComm_FileListReceived;
                usbPenComm.LogFileListReceived -= UsbPenComm_FileListReceived;
                usbPenComm.FileInfoReceived -= UsbPenComm_FileInfoReceived;
                usbPenComm.FileDownloadProgressChanged -= UsbPenComm_FileDownloadProgressChanged;
                usbPenComm.FileDownloadResultReceived -= UsbPenComm_FileDownloadResultReceived;
                usbPenComm.DeleteFileResultReceived -= UsbPenComm_DeleteFileResultReceived;
                usbPenComm.BatteryStatusReceived -= UsbPenComm_BatteryStatusReceived;
                usbPenComm.StorageStatusReceived -= UsbPenComm_StorageStatusReceived;
                usbPenComm.DateTimeReceived -= UsbPenComm_DateTimeReceived;
                usbPenComm.ConfigSetupResultReceived -= UsbPenComm_ConfigSetupResultReceived;
                usbPenComm.FormatResultReceived -= UsbPenComm_FormatResultReceived;
                usbPenComm.PowerOffResultReceived -= UsbPenComm_PowerOffResultReceived;
                usbPenComm.UpdateProgressChanged -= UsbPenComm_UpdateProgressChanged;
                usbPenComm.UpdateResultReceived -= UsbPenComm_UpdateResultReceived;
                usbPenComm.Dispose();
            }

            usbAdapter.Dispose();
        }

        private void cbAutoPowerOn_CheckedChanged(object sender, EventArgs e)
        {
            var pen = GetSelectedPen();

            if (pen == null)
                return;

            if (cbAutoPowerOn.Checked != pen.IsAutoPowerOnEnabled)
                pen.SetIsAutoPowerOnEnabledRequest(cbAutoPowerOn.Checked);
        }

        private void cbPenCapOff_CheckedChanged(object sender, EventArgs e)
        {
            var pen = GetSelectedPen();

            if (pen == null)
                return;

            if (cbPenCapOff.Checked != pen.IsPenCapOffEnabled)
                pen.SetIsPenCapOffEnabledRequest(cbPenCapOff.Checked);
        }

        private void cbBeep_CheckedChanged(object sender, EventArgs e)
        {
            var pen = GetSelectedPen();

            if (pen == null)
                return;

            if (cbBeep.Checked != pen.IsBeepEnabled)
                pen.SetIsBeepEnabledRequest(cbBeep.Checked);
        }

        private void cbSaveOfflineData_CheckedChanged(object sender, EventArgs e)
        {
            var pen = GetSelectedPen();

            if (pen == null)
                return;

            if (cbSaveOfflineData.Checked != pen.CanSaveOfflineData)
                pen.SetCanSaveOfflineDataRequest(cbSaveOfflineData.Checked);
        }

        private void cbDownsampling_CheckedChanged(object sender, EventArgs e)
        {
            var pen = GetSelectedPen();

            if (pen == null)
                return;

            if (cbDownsampling.Checked != pen.IsDownsamplingEnabled)
                pen.SetIsDownsamplingEnabledRequest(cbDownsampling.Checked);
        }

        private void nudAutoPowerOffTime_ValueChanged(object sender, EventArgs e)
        {
            var pen = GetSelectedPen();

            if (pen == null)
                return;

            if ((short)nudAutoPowerOffTime.Value != pen.AutoPowerOffTime.Minutes)
                pen.SetAutoPowerOffTimeRequest(TimeSpan.FromMinutes((short)nudAutoPowerOffTime.Value));
        }

        private void btnShowOfflineFile_Click(object sender, EventArgs e)
        {
            if (lvLocalFiles.SelectedItems.Count <= 0)
            {
                MessageBox.Show("Please select offline data file");
                return;
            }

            var fileName = (string)lvLocalFiles.SelectedItems[0].Tag;

            string baseName = fileName.Substring(0, fileName.Length - 1);
            string strokeFile, statusFile, dotFile;

            strokeFile = LocalDirectory + "\\" + baseName + "t";
            statusFile = LocalDirectory + "\\" + baseName + "o";
            dotFile = LocalDirectory + "\\" + baseName + "c";
            
            if (!File.Exists(strokeFile))
            {
                MessageBox.Show("*t file is not exists");
                return;
            }

            if (!File.Exists(statusFile))
            {
                MessageBox.Show("*o file is not exists");
                return;
            }

            if (!File.Exists(dotFile))
            {
                MessageBox.Show("*c file is not exists");
                return;
            }

            DrawingView mview = new DrawingView(new string[] { strokeFile });
            mview.Show();
        }
    }
}
