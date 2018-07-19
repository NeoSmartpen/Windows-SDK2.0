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
            this.lbHistory = new System.Windows.Forms.ListBox();
            this.btnDisconnect = new System.Windows.Forms.Button();
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
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmPowerOffTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbFsrStep)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbDevices
            // 
            this.lbDevices.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbDevices.FormattingEnabled = true;
            this.lbDevices.ItemHeight = 12;
            this.lbDevices.Location = new System.Drawing.Point(11, 20);
            this.lbDevices.Name = "lbDevices";
            this.lbDevices.Size = new System.Drawing.Size(267, 134);
            this.lbDevices.TabIndex = 23;
            this.lbDevices.SelectedIndexChanged += new System.EventHandler(this.lbDevices_SelectedIndexChanged);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(284, 136);
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
            this.txtMacAddress.Location = new System.Drawing.Point(10, 167);
            this.txtMacAddress.Name = "txtMacAddress";
            this.txtMacAddress.Size = new System.Drawing.Size(178, 21);
            this.txtMacAddress.TabIndex = 19;
            this.txtMacAddress.Text = "9C7BD2FFF128";
            this.txtMacAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(194, 166);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(84, 23);
            this.btnConnect.TabIndex = 17;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(17, 720);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(540, 33);
            this.button2.TabIndex = 24;
            this.button2.Text = "Clear";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lbHistory);
            this.groupBox2.Controls.Add(this.btnDisconnect);
            this.groupBox2.Controls.Add(this.btnConnect);
            this.groupBox2.Controls.Add(this.lbDevices);
            this.groupBox2.Controls.Add(this.txtMacAddress);
            this.groupBox2.Controls.Add(this.btnSearch);
            this.groupBox2.Location = new System.Drawing.Point(14, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(540, 211);
            this.groupBox2.TabIndex = 32;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = " 1. Search and select your device ";
            // 
            // lbHistory
            // 
            this.lbHistory.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbHistory.FormattingEnabled = true;
            this.lbHistory.ItemHeight = 12;
            this.lbHistory.Items.AddRange(new object[] {
            "9C7BD200009A",
            "9C7BD2FFF014"});
            this.lbHistory.Location = new System.Drawing.Point(375, 21);
            this.lbHistory.Name = "lbHistory";
            this.lbHistory.Size = new System.Drawing.Size(159, 170);
            this.lbHistory.TabIndex = 25;
            this.lbHistory.SelectedIndexChanged += new System.EventHandler(this.lbHistory_SelectedIndexChanged);
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Enabled = false;
            this.btnDisconnect.Location = new System.Drawing.Point(283, 166);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(84, 23);
            this.btnDisconnect.TabIndex = 24;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
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
            this.groupBox3.Location = new System.Drawing.Point(14, 230);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(276, 224);
            this.groupBox3.TabIndex = 37;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Pen Information";
            // 
            // tbPenInfo
            // 
            this.tbPenInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbPenInfo.Location = new System.Drawing.Point(18, 26);
            this.tbPenInfo.Multiline = true;
            this.tbPenInfo.Name = "tbPenInfo";
            this.tbPenInfo.ReadOnly = true;
            this.tbPenInfo.Size = new System.Drawing.Size(241, 174);
            this.tbPenInfo.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.button1);
            this.groupBox4.Controls.Add(this.button3);
            this.groupBox4.Controls.Add(this.lbOfflineData);
            this.groupBox4.Enabled = false;
            this.groupBox4.Location = new System.Drawing.Point(12, 460);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(276, 244);
            this.groupBox4.TabIndex = 36;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Offline Data";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(195, 210);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Delete";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(7, 210);
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
            this.lbOfflineData.Location = new System.Drawing.Point(7, 20);
            this.lbOfflineData.Name = "lbOfflineData";
            this.lbOfflineData.Size = new System.Drawing.Size(263, 182);
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
            this.cbPenCapPower.Location = new System.Drawing.Point(22, 90);
            this.cbPenCapPower.Name = "cbPenCapPower";
            this.cbPenCapPower.Size = new System.Drawing.Size(154, 16);
            this.cbPenCapPower.TabIndex = 1;
            this.cbPenCapPower.Text = "Pen Cap Power Control";
            this.cbPenCapPower.UseVisualStyleBackColor = true;
            this.cbPenCapPower.CheckedChanged += new System.EventHandler(this.cbPenCapPower_CheckedChanged);
            // 
            // cbPenTipPowerOn
            // 
            this.cbPenTipPowerOn.AutoSize = true;
            this.cbPenTipPowerOn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbPenTipPowerOn.Location = new System.Drawing.Point(22, 114);
            this.cbPenTipPowerOn.Name = "cbPenTipPowerOn";
            this.cbPenTipPowerOn.Size = new System.Drawing.Size(144, 16);
            this.cbPenTipPowerOn.TabIndex = 2;
            this.cbPenTipPowerOn.Text = "Power On By Pen Tip";
            this.cbPenTipPowerOn.UseVisualStyleBackColor = true;
            this.cbPenTipPowerOn.CheckedChanged += new System.EventHandler(this.cbPenTipPowerOn_CheckedChanged);
            // 
            // cbBeep
            // 
            this.cbBeep.AutoSize = true;
            this.cbBeep.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbBeep.Location = new System.Drawing.Point(22, 136);
            this.cbBeep.Name = "cbBeep";
            this.cbBeep.Size = new System.Drawing.Size(90, 16);
            this.cbBeep.TabIndex = 3;
            this.cbBeep.Text = "Beep Sound";
            this.cbBeep.UseVisualStyleBackColor = true;
            this.cbBeep.CheckedChanged += new System.EventHandler(this.cbBeep_CheckedChanged);
            // 
            // cbOfflineData
            // 
            this.cbOfflineData.AutoSize = true;
            this.cbOfflineData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbOfflineData.Location = new System.Drawing.Point(22, 158);
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
            this.label1.Size = new System.Drawing.Size(122, 12);
            this.label1.TabIndex = 8;
            this.label1.Text = "Auto PowerOff Time ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 51);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "FSR step";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(24, 216);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 11;
            this.label4.Text = "Power";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 245);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 12);
            this.label5.TabIndex = 12;
            this.label5.Text = "Storage";
            // 
            // prgPower
            // 
            this.prgPower.Location = new System.Drawing.Point(68, 210);
            this.prgPower.Name = "prgPower";
            this.prgPower.Size = new System.Drawing.Size(175, 23);
            this.prgPower.TabIndex = 13;
            // 
            // prgStorage
            // 
            this.prgStorage.Location = new System.Drawing.Point(68, 239);
            this.prgStorage.Name = "prgStorage";
            this.prgStorage.Size = new System.Drawing.Size(175, 23);
            this.prgStorage.TabIndex = 14;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(19, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(24, 12);
            this.label6.TabIndex = 15;
            this.label6.Text = "Old";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(17, 51);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(31, 12);
            this.label7.TabIndex = 16;
            this.label7.Text = "New";
            // 
            // tbOldPassword
            // 
            this.tbOldPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbOldPassword.Location = new System.Drawing.Point(51, 20);
            this.tbOldPassword.Name = "tbOldPassword";
            this.tbOldPassword.Size = new System.Drawing.Size(122, 21);
            this.tbOldPassword.TabIndex = 0;
            // 
            // tbNewPassword
            // 
            this.tbNewPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbNewPassword.Location = new System.Drawing.Point(51, 47);
            this.tbNewPassword.Name = "tbNewPassword";
            this.tbNewPassword.Size = new System.Drawing.Size(122, 21);
            this.tbNewPassword.TabIndex = 0;
            // 
            // btnPwdChange
            // 
            this.btnPwdChange.Location = new System.Drawing.Point(178, 47);
            this.btnPwdChange.Name = "btnPwdChange";
            this.btnPwdChange.Size = new System.Drawing.Size(69, 23);
            this.btnPwdChange.TabIndex = 17;
            this.btnPwdChange.Text = "Submit";
            this.btnPwdChange.UseVisualStyleBackColor = true;
            this.btnPwdChange.Click += new System.EventHandler(this.btnPwdChange_Click);
            // 
            // btnUpgrade
            // 
            this.btnUpgrade.Location = new System.Drawing.Point(170, 52);
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
            this.tbFirmwarePath.Size = new System.Drawing.Size(181, 21);
            this.tbFirmwarePath.TabIndex = 19;
            this.tbFirmwarePath.Click += new System.EventHandler(this.tbFirmwarePath_Click);
            // 
            // tbFirmwareVersion
            // 
            this.tbFirmwareVersion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbFirmwareVersion.Location = new System.Drawing.Point(64, 52);
            this.tbFirmwareVersion.Name = "tbFirmwareVersion";
            this.tbFirmwareVersion.Size = new System.Drawing.Size(100, 21);
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
            this.groupBox5.Location = new System.Drawing.Point(296, 230);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(258, 278);
            this.groupBox5.TabIndex = 35;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Pen Setting";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.btnPwdChange);
            this.groupBox6.Controls.Add(this.label7);
            this.groupBox6.Controls.Add(this.tbOldPassword);
            this.groupBox6.Controls.Add(this.label6);
            this.groupBox6.Controls.Add(this.tbNewPassword);
            this.groupBox6.Enabled = false;
            this.groupBox6.Location = new System.Drawing.Point(297, 514);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(258, 84);
            this.groupBox6.TabIndex = 38;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Password";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.label2);
            this.groupBox7.Controls.Add(this.label8);
            this.groupBox7.Controls.Add(this.btnUpgrade);
            this.groupBox7.Controls.Add(this.tbFirmwareVersion);
            this.groupBox7.Controls.Add(this.tbFirmwarePath);
            this.groupBox7.Enabled = false;
            this.groupBox7.Location = new System.Drawing.Point(297, 604);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(257, 100);
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
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1071, 765);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox5);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainForm";
            this.Text = "Form1";
            this.TransparencyKey = System.Drawing.Color.Blue;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
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
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
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
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.ListBox lbHistory;
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
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label label2;
    }
}

