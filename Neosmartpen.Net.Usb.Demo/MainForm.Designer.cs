namespace Neosmartpen.Net.Usb.Demo
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lvUsbPens = new System.Windows.Forms.ListView();
            this.Port = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Model = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Mac = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gbSelectedPen = new System.Windows.Forms.GroupBox();
            this.lvPenFiles = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tvLocalDir = new System.Windows.Forms.TreeView();
            this.lvLocalFiles = new System.Windows.Forms.ListView();
            this.Title = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Size = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Type = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Date = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnDownload = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnFormat = new System.Windows.Forms.Button();
            this.btnPowerOff = new System.Windows.Forms.Button();
            this.btnFirmwareUpdate = new System.Windows.Forms.Button();
            this.cbAutoPowerOn = new System.Windows.Forms.CheckBox();
            this.nudAutoPowerOffTime = new System.Windows.Forms.NumericUpDown();
            this.cbPenCapOff = new System.Windows.Forms.CheckBox();
            this.cbBeep = new System.Windows.Forms.CheckBox();
            this.cbSaveOfflineData = new System.Windows.Forms.CheckBox();
            this.cbDownsampling = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pbStorage = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.pbBattery = new System.Windows.Forms.ProgressBar();
            this.lbFirmwareVersion = new System.Windows.Forms.Label();
            this.lbDateTime = new System.Windows.Forms.Label();
            this.btnShowOfflineFile = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.gbSelectedPen.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudAutoPowerOffTime)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lvUsbPens);
            this.groupBox1.Location = new System.Drawing.Point(22, 16);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(444, 151);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "USB Pens";
            // 
            // lvUsbPens
            // 
            this.lvUsbPens.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Port,
            this.Model,
            this.Mac});
            this.lvUsbPens.FullRowSelect = true;
            this.lvUsbPens.HideSelection = false;
            this.lvUsbPens.Location = new System.Drawing.Point(12, 21);
            this.lvUsbPens.MultiSelect = false;
            this.lvUsbPens.Name = "lvUsbPens";
            this.lvUsbPens.Size = new System.Drawing.Size(417, 114);
            this.lvUsbPens.TabIndex = 0;
            this.lvUsbPens.UseCompatibleStateImageBehavior = false;
            this.lvUsbPens.View = System.Windows.Forms.View.Details;
            this.lvUsbPens.SelectedIndexChanged += new System.EventHandler(this.lvUsbPens_SelectedIndexChanged);
            // 
            // Port
            // 
            this.Port.Text = "Port";
            this.Port.Width = 80;
            // 
            // Model
            // 
            this.Model.Text = "Model";
            this.Model.Width = 120;
            // 
            // Mac
            // 
            this.Mac.Text = "Mac";
            this.Mac.Width = 200;
            // 
            // gbSelectedPen
            // 
            this.gbSelectedPen.Controls.Add(this.lbFirmwareVersion);
            this.gbSelectedPen.Controls.Add(this.lbDateTime);
            this.gbSelectedPen.Controls.Add(this.label3);
            this.gbSelectedPen.Controls.Add(this.pbBattery);
            this.gbSelectedPen.Controls.Add(this.label2);
            this.gbSelectedPen.Controls.Add(this.pbStorage);
            this.gbSelectedPen.Controls.Add(this.label1);
            this.gbSelectedPen.Controls.Add(this.cbDownsampling);
            this.gbSelectedPen.Controls.Add(this.cbSaveOfflineData);
            this.gbSelectedPen.Controls.Add(this.cbBeep);
            this.gbSelectedPen.Controls.Add(this.cbPenCapOff);
            this.gbSelectedPen.Controls.Add(this.nudAutoPowerOffTime);
            this.gbSelectedPen.Controls.Add(this.cbAutoPowerOn);
            this.gbSelectedPen.Controls.Add(this.lvPenFiles);
            this.gbSelectedPen.Enabled = false;
            this.gbSelectedPen.Location = new System.Drawing.Point(21, 220);
            this.gbSelectedPen.Name = "gbSelectedPen";
            this.gbSelectedPen.Size = new System.Drawing.Size(445, 358);
            this.gbSelectedPen.TabIndex = 1;
            this.gbSelectedPen.TabStop = false;
            this.gbSelectedPen.Text = "Selected Pen";
            // 
            // lvPenFiles
            // 
            this.lvPenFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lvPenFiles.Enabled = false;
            this.lvPenFiles.FullRowSelect = true;
            this.lvPenFiles.HideSelection = false;
            this.lvPenFiles.Location = new System.Drawing.Point(185, 26);
            this.lvPenFiles.MultiSelect = false;
            this.lvPenFiles.Name = "lvPenFiles";
            this.lvPenFiles.Size = new System.Drawing.Size(246, 316);
            this.lvPenFiles.TabIndex = 0;
            this.lvPenFiles.UseCompatibleStateImageBehavior = false;
            this.lvPenFiles.View = System.Windows.Forms.View.Details;
            this.lvPenFiles.SelectedIndexChanged += new System.EventHandler(this.lvPenFiles_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Title";
            this.columnHeader1.Width = 150;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Type";
            this.columnHeader2.Width = 70;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnShowOfflineFile);
            this.groupBox3.Controls.Add(this.tvLocalDir);
            this.groupBox3.Controls.Add(this.lvLocalFiles);
            this.groupBox3.Location = new System.Drawing.Point(575, 16);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(468, 561);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Local";
            // 
            // tvLocalDir
            // 
            this.tvLocalDir.Location = new System.Drawing.Point(15, 21);
            this.tvLocalDir.Name = "tvLocalDir";
            this.tvLocalDir.Size = new System.Drawing.Size(436, 194);
            this.tvLocalDir.TabIndex = 1;
            this.tvLocalDir.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvLocalDir_BeforeExpand);
            this.tvLocalDir.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvLocalDir_AfterSelect);
            // 
            // lvLocalFiles
            // 
            this.lvLocalFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Title,
            this.Size,
            this.Type,
            this.Date});
            this.lvLocalFiles.FullRowSelect = true;
            this.lvLocalFiles.HideSelection = false;
            this.lvLocalFiles.Location = new System.Drawing.Point(15, 230);
            this.lvLocalFiles.MultiSelect = false;
            this.lvLocalFiles.Name = "lvLocalFiles";
            this.lvLocalFiles.Size = new System.Drawing.Size(437, 273);
            this.lvLocalFiles.TabIndex = 0;
            this.lvLocalFiles.UseCompatibleStateImageBehavior = false;
            this.lvLocalFiles.View = System.Windows.Forms.View.Details;
            // 
            // Title
            // 
            this.Title.Text = "Title";
            this.Title.Width = 200;
            // 
            // Size
            // 
            this.Size.Text = "Size";
            // 
            // Type
            // 
            this.Type.Text = "Type";
            // 
            // Date
            // 
            this.Date.Text = "Date";
            this.Date.Width = 100;
            // 
            // btnDownload
            // 
            this.btnDownload.Location = new System.Drawing.Point(498, 303);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(49, 48);
            this.btnDownload.TabIndex = 3;
            this.btnDownload.Text = "->";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(498, 441);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(49, 48);
            this.btnDelete.TabIndex = 5;
            this.btnDelete.Text = "Del";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnFormat
            // 
            this.btnFormat.Location = new System.Drawing.Point(23, 173);
            this.btnFormat.Name = "btnFormat";
            this.btnFormat.Size = new System.Drawing.Size(121, 34);
            this.btnFormat.TabIndex = 6;
            this.btnFormat.Text = "Format";
            this.btnFormat.UseVisualStyleBackColor = true;
            this.btnFormat.Click += new System.EventHandler(this.btnFormat_Click);
            // 
            // btnPowerOff
            // 
            this.btnPowerOff.Location = new System.Drawing.Point(168, 173);
            this.btnPowerOff.Name = "btnPowerOff";
            this.btnPowerOff.Size = new System.Drawing.Size(121, 34);
            this.btnPowerOff.TabIndex = 7;
            this.btnPowerOff.Text = "Power Off";
            this.btnPowerOff.UseVisualStyleBackColor = true;
            this.btnPowerOff.Click += new System.EventHandler(this.btnPowerOff_Click);
            // 
            // btnFirmwareUpdate
            // 
            this.btnFirmwareUpdate.Location = new System.Drawing.Point(310, 173);
            this.btnFirmwareUpdate.Name = "btnFirmwareUpdate";
            this.btnFirmwareUpdate.Size = new System.Drawing.Size(156, 34);
            this.btnFirmwareUpdate.TabIndex = 8;
            this.btnFirmwareUpdate.Text = "Firmware Update";
            this.btnFirmwareUpdate.UseVisualStyleBackColor = true;
            this.btnFirmwareUpdate.Click += new System.EventHandler(this.btnFirmwareUpdate_Click);
            // 
            // cbAutoPowerOn
            // 
            this.cbAutoPowerOn.AutoSize = true;
            this.cbAutoPowerOn.Location = new System.Drawing.Point(13, 137);
            this.cbAutoPowerOn.Name = "cbAutoPowerOn";
            this.cbAutoPowerOn.Size = new System.Drawing.Size(109, 16);
            this.cbAutoPowerOn.TabIndex = 1;
            this.cbAutoPowerOn.Text = "Auto Power On";
            this.cbAutoPowerOn.UseVisualStyleBackColor = true;
            this.cbAutoPowerOn.CheckedChanged += new System.EventHandler(this.cbAutoPowerOn_CheckedChanged);
            // 
            // nudAutoPowerOffTime
            // 
            this.nudAutoPowerOffTime.Location = new System.Drawing.Point(13, 103);
            this.nudAutoPowerOffTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudAutoPowerOffTime.Name = "nudAutoPowerOffTime";
            this.nudAutoPowerOffTime.Size = new System.Drawing.Size(120, 21);
            this.nudAutoPowerOffTime.TabIndex = 2;
            this.nudAutoPowerOffTime.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudAutoPowerOffTime.ValueChanged += new System.EventHandler(this.nudAutoPowerOffTime_ValueChanged);
            // 
            // cbPenCapOff
            // 
            this.cbPenCapOff.AutoSize = true;
            this.cbPenCapOff.Location = new System.Drawing.Point(13, 159);
            this.cbPenCapOff.Name = "cbPenCapOff";
            this.cbPenCapOff.Size = new System.Drawing.Size(92, 16);
            this.cbPenCapOff.TabIndex = 3;
            this.cbPenCapOff.Text = "Pen Cap Off";
            this.cbPenCapOff.UseVisualStyleBackColor = true;
            this.cbPenCapOff.CheckedChanged += new System.EventHandler(this.cbPenCapOff_CheckedChanged);
            // 
            // cbBeep
            // 
            this.cbBeep.AutoSize = true;
            this.cbBeep.Location = new System.Drawing.Point(13, 181);
            this.cbBeep.Name = "cbBeep";
            this.cbBeep.Size = new System.Drawing.Size(53, 16);
            this.cbBeep.TabIndex = 4;
            this.cbBeep.Text = "Beep";
            this.cbBeep.UseVisualStyleBackColor = true;
            this.cbBeep.CheckedChanged += new System.EventHandler(this.cbBeep_CheckedChanged);
            // 
            // cbSaveOfflineData
            // 
            this.cbSaveOfflineData.AutoSize = true;
            this.cbSaveOfflineData.Location = new System.Drawing.Point(13, 203);
            this.cbSaveOfflineData.Name = "cbSaveOfflineData";
            this.cbSaveOfflineData.Size = new System.Drawing.Size(120, 16);
            this.cbSaveOfflineData.TabIndex = 5;
            this.cbSaveOfflineData.Text = "Save Offline Data";
            this.cbSaveOfflineData.UseVisualStyleBackColor = true;
            this.cbSaveOfflineData.CheckedChanged += new System.EventHandler(this.cbSaveOfflineData_CheckedChanged);
            // 
            // cbDownsampling
            // 
            this.cbDownsampling.AutoSize = true;
            this.cbDownsampling.Location = new System.Drawing.Point(13, 225);
            this.cbDownsampling.Name = "cbDownsampling";
            this.cbDownsampling.Size = new System.Drawing.Size(108, 16);
            this.cbDownsampling.TabIndex = 6;
            this.cbDownsampling.Text = "Downsampling";
            this.cbDownsampling.UseVisualStyleBackColor = true;
            this.cbDownsampling.CheckedChanged += new System.EventHandler(this.cbDownsampling_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 82);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "Auto Power Off Minute";
            // 
            // pbStorage
            // 
            this.pbStorage.Location = new System.Drawing.Point(13, 317);
            this.pbStorage.Name = "pbStorage";
            this.pbStorage.Size = new System.Drawing.Size(120, 22);
            this.pbStorage.Step = 1;
            this.pbStorage.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 302);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "Storage";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 256);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 12);
            this.label3.TabIndex = 12;
            this.label3.Text = "Battery";
            // 
            // pbBattery
            // 
            this.pbBattery.Location = new System.Drawing.Point(13, 271);
            this.pbBattery.Name = "pbBattery";
            this.pbBattery.Size = new System.Drawing.Size(120, 22);
            this.pbBattery.Step = 1;
            this.pbBattery.TabIndex = 11;
            // 
            // lbFirmwareVersion
            // 
            this.lbFirmwareVersion.AutoSize = true;
            this.lbFirmwareVersion.Location = new System.Drawing.Point(11, 54);
            this.lbFirmwareVersion.Name = "lbFirmwareVersion";
            this.lbFirmwareVersion.Size = new System.Drawing.Size(0, 12);
            this.lbFirmwareVersion.TabIndex = 14;
            // 
            // lbDateTime
            // 
            this.lbDateTime.AutoSize = true;
            this.lbDateTime.Location = new System.Drawing.Point(11, 29);
            this.lbDateTime.Name = "lbDateTime";
            this.lbDateTime.Size = new System.Drawing.Size(0, 12);
            this.lbDateTime.TabIndex = 13;
            // 
            // btnShowOfflineFile
            // 
            this.btnShowOfflineFile.Location = new System.Drawing.Point(295, 512);
            this.btnShowOfflineFile.Name = "btnShowOfflineFile";
            this.btnShowOfflineFile.Size = new System.Drawing.Size(156, 34);
            this.btnShowOfflineFile.TabIndex = 9;
            this.btnShowOfflineFile.Text = "Show Offline File";
            this.btnShowOfflineFile.UseVisualStyleBackColor = true;
            this.btnShowOfflineFile.Click += new System.EventHandler(this.btnShowOfflineFile_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1061, 596);
            this.Controls.Add(this.btnFirmwareUpdate);
            this.Controls.Add(this.btnPowerOff);
            this.Controls.Add(this.btnFormat);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.gbSelectedPen);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "UsbDemoTool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.gbSelectedPen.ResumeLayout(false);
            this.gbSelectedPen.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudAutoPowerOffTime)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox gbSelectedPen;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnFormat;
        private System.Windows.Forms.Button btnPowerOff;
        private System.Windows.Forms.Button btnFirmwareUpdate;
        private System.Windows.Forms.ListView lvUsbPens;
        private System.Windows.Forms.ListView lvPenFiles;
        private System.Windows.Forms.TreeView tvLocalDir;
        private System.Windows.Forms.ListView lvLocalFiles;
        private System.Windows.Forms.ColumnHeader Title;
        private System.Windows.Forms.ColumnHeader Size;
        private System.Windows.Forms.ColumnHeader Type;
        private System.Windows.Forms.ColumnHeader Date;
        private System.Windows.Forms.ColumnHeader Port;
        private System.Windows.Forms.ColumnHeader Model;
        private System.Windows.Forms.ColumnHeader Mac;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ProgressBar pbBattery;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ProgressBar pbStorage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbDownsampling;
        private System.Windows.Forms.CheckBox cbSaveOfflineData;
        private System.Windows.Forms.CheckBox cbBeep;
        private System.Windows.Forms.CheckBox cbPenCapOff;
        private System.Windows.Forms.NumericUpDown nudAutoPowerOffTime;
        private System.Windows.Forms.CheckBox cbAutoPowerOn;
        private System.Windows.Forms.Label lbFirmwareVersion;
        private System.Windows.Forms.Label lbDateTime;
        private System.Windows.Forms.Button btnShowOfflineFile;
    }
}