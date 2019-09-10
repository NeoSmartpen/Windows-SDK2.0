namespace PenDemo
{
    partial class MainForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbDevices = new System.Windows.Forms.ListBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtMacAddress = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tbPenInfo = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.lbOfflineData = new System.Windows.Forms.ListBox();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.nmPowerOffTime = new System.Windows.Forms.NumericUpDown();
            this.cbPenCapPower = new System.Windows.Forms.CheckBox();
            this.cbPenTipPowerOn = new System.Windows.Forms.CheckBox();
            this.cbBeep = new System.Windows.Forms.CheckBox();
            this.cbOfflineData = new System.Windows.Forms.CheckBox();
            this.tbFsrStep = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.prgPower = new System.Windows.Forms.ProgressBar();
            this.prgStorage = new System.Windows.Forms.ProgressBar();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tbOldPassword = new System.Windows.Forms.TextBox();
            this.tbNewPassword = new System.Windows.Forms.TextBox();
            this.btnPwdChange = new System.Windows.Forms.Button();
            this.btnUpgrade = new System.Windows.Forms.Button();
            this.tbFirmwarePath = new System.Windows.Forms.TextBox();
            this.tbFirmwareVersion = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.tbCertificateFilePath = new System.Windows.Forms.TextBox();
            this.btnSelectPrivateKey = new System.Windows.Forms.Button();
            this.tbPrivateKeyFilePath = new System.Windows.Forms.TextBox();
            this.btnUpdateCertificate = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.tbSerialNumber = new System.Windows.Forms.TextBox();
            this.btnDeleteCertificate = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmPowerOffTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbFsrStep)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbDevices
            // 
            this.lbDevices.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbDevices.FormattingEnabled = true;
            this.lbDevices.ItemHeight = 12;
            this.lbDevices.Location = new System.Drawing.Point(11, 20);
            this.lbDevices.Name = "lbDevices";
            this.lbDevices.Size = new System.Drawing.Size(257, 158);
            this.lbDevices.TabIndex = 23;
            this.lbDevices.SelectedIndexChanged += new System.EventHandler(this.lbDevices_SelectedIndexChanged);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(11, 184);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(84, 23);
            this.btnSearch.TabIndex = 22;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtMacAddress
            // 
            this.txtMacAddress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMacAddress.Location = new System.Drawing.Point(11, 218);
            this.txtMacAddress.Name = "txtMacAddress";
            this.txtMacAddress.Size = new System.Drawing.Size(158, 21);
            this.txtMacAddress.TabIndex = 19;
            this.txtMacAddress.Text = "9C7BD20555EF";
            this.txtMacAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(175, 218);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(93, 23);
            this.btnConnect.TabIndex = 17;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 720);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(545, 33);
            this.button2.TabIndex = 24;
            this.button2.Text = "Clear";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnConnect);
            this.groupBox2.Controls.Add(this.lbDevices);
            this.groupBox2.Controls.Add(this.txtMacAddress);
            this.groupBox2.Controls.Add(this.btnSearch);
            this.groupBox2.Location = new System.Drawing.Point(14, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(276, 249);
            this.groupBox2.TabIndex = 32;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = " 1. Search and select your device ";
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(563, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(505, 740);
            this.groupBox1.TabIndex = 33;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " 2. Write something on your ncode note ";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tbPenInfo);
            this.groupBox3.Enabled = false;
            this.groupBox3.Location = new System.Drawing.Point(14, 266);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(276, 153);
            this.groupBox3.TabIndex = 37;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Pen Information";
            // 
            // tbPenInfo
            // 
            this.tbPenInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbPenInfo.Location = new System.Drawing.Point(18, 23);
            this.tbPenInfo.Multiline = true;
            this.tbPenInfo.Name = "tbPenInfo";
            this.tbPenInfo.ReadOnly = true;
            this.tbPenInfo.Size = new System.Drawing.Size(241, 120);
            this.tbPenInfo.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.button1);
            this.groupBox4.Controls.Add(this.button3);
            this.groupBox4.Controls.Add(this.lbOfflineData);
            this.groupBox4.Enabled = false;
            this.groupBox4.Location = new System.Drawing.Point(12, 518);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(278, 196);
            this.groupBox4.TabIndex = 36;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Offline Data";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(195, 162);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Delete";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(7, 162);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 1;
            this.button3.Text = "Download";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // lbOfflineData
            // 
            this.lbOfflineData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbOfflineData.FormattingEnabled = true;
            this.lbOfflineData.ItemHeight = 12;
            this.lbOfflineData.Location = new System.Drawing.Point(7, 21);
            this.lbOfflineData.Name = "lbOfflineData";
            this.lbOfflineData.Size = new System.Drawing.Size(263, 134);
            this.lbOfflineData.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.BackgroundImage = global::PenDemo.Properties.Resources.background;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(575, 35);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(484, 709);
            this.pictureBox1.TabIndex = 27;
            this.pictureBox1.TabStop = false;
            // 
            // nmPowerOffTime
            // 
            this.nmPowerOffTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nmPowerOffTime.Location = new System.Drawing.Point(155, 21);
            this.nmPowerOffTime.Maximum = new decimal(new int[] {
            -1486618624,
            232830643,
            0,
            0});
            this.nmPowerOffTime.Name = "nmPowerOffTime";
            this.nmPowerOffTime.Size = new System.Drawing.Size(83, 21);
            this.nmPowerOffTime.TabIndex = 0;
            this.nmPowerOffTime.ValueChanged += new System.EventHandler(this.nmPowerOffTime_ValueChanged);
            // 
            // cbPenCapPower
            // 
            this.cbPenCapPower.AutoSize = true;
            this.cbPenCapPower.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbPenCapPower.Location = new System.Drawing.Point(22, 83);
            this.cbPenCapPower.Name = "cbPenCapPower";
            this.cbPenCapPower.Size = new System.Drawing.Size(149, 16);
            this.cbPenCapPower.TabIndex = 1;
            this.cbPenCapPower.Text = "Pen cap power control";
            this.cbPenCapPower.UseVisualStyleBackColor = true;
            this.cbPenCapPower.CheckedChanged += new System.EventHandler(this.cbPenCapPower_CheckedChanged);
            // 
            // cbPenTipPowerOn
            // 
            this.cbPenTipPowerOn.AutoSize = true;
            this.cbPenTipPowerOn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbPenTipPowerOn.Location = new System.Drawing.Point(22, 107);
            this.cbPenTipPowerOn.Name = "cbPenTipPowerOn";
            this.cbPenTipPowerOn.Size = new System.Drawing.Size(135, 16);
            this.cbPenTipPowerOn.TabIndex = 2;
            this.cbPenTipPowerOn.Text = "Power on by pen tip";
            this.cbPenTipPowerOn.UseVisualStyleBackColor = true;
            this.cbPenTipPowerOn.CheckedChanged += new System.EventHandler(this.cbPenTipPowerOn_CheckedChanged);
            // 
            // cbBeep
            // 
            this.cbBeep.AutoSize = true;
            this.cbBeep.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbBeep.Location = new System.Drawing.Point(22, 129);
            this.cbBeep.Name = "cbBeep";
            this.cbBeep.Size = new System.Drawing.Size(89, 16);
            this.cbBeep.TabIndex = 3;
            this.cbBeep.Text = "Beep sound";
            this.cbBeep.UseVisualStyleBackColor = true;
            this.cbBeep.CheckedChanged += new System.EventHandler(this.cbBeep_CheckedChanged);
            // 
            // cbOfflineData
            // 
            this.cbOfflineData.AutoSize = true;
            this.cbOfflineData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbOfflineData.Location = new System.Drawing.Point(22, 151);
            this.cbOfflineData.Name = "cbOfflineData";
            this.cbOfflineData.Size = new System.Drawing.Size(84, 16);
            this.cbOfflineData.TabIndex = 5;
            this.cbOfflineData.Text = "Offline data";
            this.cbOfflineData.UseVisualStyleBackColor = true;
            this.cbOfflineData.CheckedChanged += new System.EventHandler(this.cbOfflineData_CheckedChanged);
            // 
            // tbFsrStep
            // 
            this.tbFsrStep.LargeChange = 1;
            this.tbFsrStep.Location = new System.Drawing.Point(142, 47);
            this.tbFsrStep.Maximum = 4;
            this.tbFsrStep.Name = "tbFsrStep";
            this.tbFsrStep.Size = new System.Drawing.Size(104, 45);
            this.tbFsrStep.TabIndex = 7;
            this.tbFsrStep.ValueChanged += new System.EventHandler(this.tbFsrStep_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 12);
            this.label1.TabIndex = 8;
            this.label1.Text = "Auto power off time ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 51);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "Force step";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(24, 185);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 11;
            this.label4.Text = "Power";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 214);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 12);
            this.label5.TabIndex = 12;
            this.label5.Text = "Storage";
            // 
            // prgPower
            // 
            this.prgPower.Location = new System.Drawing.Point(68, 179);
            this.prgPower.Name = "prgPower";
            this.prgPower.Size = new System.Drawing.Size(175, 23);
            this.prgPower.TabIndex = 13;
            // 
            // prgStorage
            // 
            this.prgStorage.Location = new System.Drawing.Point(68, 208);
            this.prgStorage.Name = "prgStorage";
            this.prgStorage.Size = new System.Drawing.Size(175, 23);
            this.prgStorage.TabIndex = 14;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 141);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(22, 12);
            this.label6.TabIndex = 15;
            this.label6.Text = "old";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 168);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 12);
            this.label7.TabIndex = 16;
            this.label7.Text = "new";
            // 
            // tbOldPassword
            // 
            this.tbOldPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbOldPassword.Location = new System.Drawing.Point(40, 137);
            this.tbOldPassword.Name = "tbOldPassword";
            this.tbOldPassword.Size = new System.Drawing.Size(125, 21);
            this.tbOldPassword.TabIndex = 0;
            // 
            // tbNewPassword
            // 
            this.tbNewPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbNewPassword.Location = new System.Drawing.Point(40, 164);
            this.tbNewPassword.Name = "tbNewPassword";
            this.tbNewPassword.Size = new System.Drawing.Size(125, 21);
            this.tbNewPassword.TabIndex = 0;
            // 
            // btnPwdChange
            // 
            this.btnPwdChange.Location = new System.Drawing.Point(172, 164);
            this.btnPwdChange.Name = "btnPwdChange";
            this.btnPwdChange.Size = new System.Drawing.Size(75, 23);
            this.btnPwdChange.TabIndex = 17;
            this.btnPwdChange.Text = "Change";
            this.btnPwdChange.UseVisualStyleBackColor = true;
            this.btnPwdChange.Click += new System.EventHandler(this.btnPwdChange_Click);
            // 
            // btnUpgrade
            // 
            this.btnUpgrade.Location = new System.Drawing.Point(193, 52);
            this.btnUpgrade.Name = "btnUpgrade";
            this.btnUpgrade.Size = new System.Drawing.Size(75, 21);
            this.btnUpgrade.TabIndex = 18;
            this.btnUpgrade.Text = "Update";
            this.btnUpgrade.UseVisualStyleBackColor = true;
            this.btnUpgrade.Click += new System.EventHandler(this.btnUpgrade_Click);
            // 
            // tbFirmwarePath
            // 
            this.tbFirmwarePath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbFirmwarePath.Location = new System.Drawing.Point(64, 25);
            this.tbFirmwarePath.Name = "tbFirmwarePath";
            this.tbFirmwarePath.Size = new System.Drawing.Size(204, 21);
            this.tbFirmwarePath.TabIndex = 19;
            this.tbFirmwarePath.Click += new System.EventHandler(this.tbFirmwarePath_Click);
            // 
            // tbFirmwareVersion
            // 
            this.tbFirmwareVersion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbFirmwareVersion.Location = new System.Drawing.Point(64, 52);
            this.tbFirmwareVersion.Name = "tbFirmwareVersion";
            this.tbFirmwareVersion.Size = new System.Drawing.Size(123, 21);
            this.tbFirmwareVersion.TabIndex = 20;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(14, 56);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(48, 12);
            this.label8.TabIndex = 21;
            this.label8.Text = "Version";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cbPenCapPower);
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Controls.Add(this.label5);
            this.groupBox5.Controls.Add(this.label1);
            this.groupBox5.Controls.Add(this.prgPower);
            this.groupBox5.Controls.Add(this.tbFsrStep);
            this.groupBox5.Controls.Add(this.prgStorage);
            this.groupBox5.Controls.Add(this.cbOfflineData);
            this.groupBox5.Controls.Add(this.cbBeep);
            this.groupBox5.Controls.Add(this.cbPenTipPowerOn);
            this.groupBox5.Controls.Add(this.nmPowerOffTime);
            this.groupBox5.Enabled = false;
            this.groupBox5.Location = new System.Drawing.Point(296, 266);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(258, 246);
            this.groupBox5.TabIndex = 35;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Pen Setting";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.label2);
            this.groupBox7.Controls.Add(this.label8);
            this.groupBox7.Controls.Add(this.btnUpgrade);
            this.groupBox7.Controls.Add(this.tbFirmwareVersion);
            this.groupBox7.Controls.Add(this.tbFirmwarePath);
            this.groupBox7.Enabled = false;
            this.groupBox7.Location = new System.Drawing.Point(14, 424);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(276, 88);
            this.groupBox7.TabIndex = 39;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Firmware Update";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(37, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 12);
            this.label2.TabIndex = 22;
            this.label2.Text = "File";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.btnDeleteCertificate);
            this.groupBox6.Controls.Add(this.label12);
            this.groupBox6.Controls.Add(this.tbSerialNumber);
            this.groupBox6.Controls.Add(this.label11);
            this.groupBox6.Controls.Add(this.btnPwdChange);
            this.groupBox6.Controls.Add(this.label9);
            this.groupBox6.Controls.Add(this.label7);
            this.groupBox6.Controls.Add(this.btnUpdateCertificate);
            this.groupBox6.Controls.Add(this.tbOldPassword);
            this.groupBox6.Controls.Add(this.tbCertificateFilePath);
            this.groupBox6.Controls.Add(this.label6);
            this.groupBox6.Controls.Add(this.tbNewPassword);
            this.groupBox6.Enabled = false;
            this.groupBox6.Location = new System.Drawing.Point(296, 518);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(257, 196);
            this.groupBox6.TabIndex = 40;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Security";
            // 
            // tbCertificateFilePath
            // 
            this.tbCertificateFilePath.Location = new System.Drawing.Point(10, 34);
            this.tbCertificateFilePath.Name = "tbCertificateFilePath";
            this.tbCertificateFilePath.ReadOnly = true;
            this.tbCertificateFilePath.Size = new System.Drawing.Size(155, 21);
            this.tbCertificateFilePath.TabIndex = 0;
            // 
            // btnSelectPrivateKey
            // 
            this.btnSelectPrivateKey.Location = new System.Drawing.Point(179, 38);
            this.btnSelectPrivateKey.Name = "btnSelectPrivateKey";
            this.btnSelectPrivateKey.Size = new System.Drawing.Size(69, 23);
            this.btnSelectPrivateKey.TabIndex = 1;
            this.btnSelectPrivateKey.Text = "Browse";
            this.btnSelectPrivateKey.UseVisualStyleBackColor = true;
            this.btnSelectPrivateKey.Click += new System.EventHandler(this.btnSelectPrivateKey_Click);
            // 
            // tbPrivateKeyFilePath
            // 
            this.tbPrivateKeyFilePath.Location = new System.Drawing.Point(12, 39);
            this.tbPrivateKeyFilePath.Name = "tbPrivateKeyFilePath";
            this.tbPrivateKeyFilePath.ReadOnly = true;
            this.tbPrivateKeyFilePath.Size = new System.Drawing.Size(161, 21);
            this.tbPrivateKeyFilePath.TabIndex = 2;
            // 
            // btnUpdateCertificate
            // 
            this.btnUpdateCertificate.Location = new System.Drawing.Point(172, 33);
            this.btnUpdateCertificate.Name = "btnUpdateCertificate";
            this.btnUpdateCertificate.Size = new System.Drawing.Size(75, 23);
            this.btnUpdateCertificate.TabIndex = 3;
            this.btnUpdateCertificate.Text = "Update";
            this.btnUpdateCertificate.UseVisualStyleBackColor = true;
            this.btnUpdateCertificate.Click += new System.EventHandler(this.btnUpdateCertificate_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(10, 17);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(117, 12);
            this.label9.TabIndex = 4;
            this.label9.Text = "* Certificate update:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(10, 22);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(205, 12);
            this.label10.TabIndex = 5;
            this.label10.Text = "* Private key file for authentication: ";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.tbPrivateKeyFilePath);
            this.groupBox8.Controls.Add(this.label10);
            this.groupBox8.Controls.Add(this.btnSelectPrivateKey);
            this.groupBox8.Location = new System.Drawing.Point(296, 13);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(258, 249);
            this.groupBox8.TabIndex = 26;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Authentication Parameter";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(10, 119);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(111, 12);
            this.label11.TabIndex = 18;
            this.label11.Text = "* Setup password:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(11, 68);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(113, 12);
            this.label12.TabIndex = 20;
            this.label12.Text = "* Certificate delete:";
            // 
            // tbSerialNumber
            // 
            this.tbSerialNumber.Location = new System.Drawing.Point(11, 85);
            this.tbSerialNumber.Name = "tbSerialNumber";
            this.tbSerialNumber.Size = new System.Drawing.Size(155, 21);
            this.tbSerialNumber.TabIndex = 19;
            // 
            // btnDeleteCertificate
            // 
            this.btnDeleteCertificate.Location = new System.Drawing.Point(172, 84);
            this.btnDeleteCertificate.Name = "btnDeleteCertificate";
            this.btnDeleteCertificate.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteCertificate.TabIndex = 21;
            this.btnDeleteCertificate.Text = "Delete";
            this.btnDeleteCertificate.UseVisualStyleBackColor = true;
            this.btnDeleteCertificate.Click += new System.EventHandler(this.btnDeleteCertificate_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1071, 765);
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox5);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.TransparencyKey = System.Drawing.Color.Blue;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmPowerOffTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbFsrStep)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lbDevices;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtMacAddress;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ListBox lbOfflineData;
        private System.Windows.Forms.TextBox tbPenInfo;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.NumericUpDown nmPowerOffTime;
        private System.Windows.Forms.CheckBox cbPenCapPower;
        private System.Windows.Forms.CheckBox cbPenTipPowerOn;
        private System.Windows.Forms.CheckBox cbBeep;
        private System.Windows.Forms.CheckBox cbOfflineData;
        private System.Windows.Forms.TrackBar tbFsrStep;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ProgressBar prgPower;
        private System.Windows.Forms.ProgressBar prgStorage;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbOldPassword;
        private System.Windows.Forms.TextBox tbNewPassword;
        private System.Windows.Forms.Button btnPwdChange;
        private System.Windows.Forms.Button btnUpgrade;
        private System.Windows.Forms.TextBox tbFirmwarePath;
        private System.Windows.Forms.TextBox tbFirmwareVersion;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnUpdateCertificate;
        private System.Windows.Forms.TextBox tbPrivateKeyFilePath;
        private System.Windows.Forms.Button btnSelectPrivateKey;
        private System.Windows.Forms.TextBox tbCertificateFilePath;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnDeleteCertificate;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox tbSerialNumber;
    }
}

