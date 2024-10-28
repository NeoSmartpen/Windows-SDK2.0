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
            this.DevicesListbox = new System.Windows.Forms.ListBox();
            this.SearchButton = new System.Windows.Forms.Button();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.DisconnectButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.GroupInfo = new System.Windows.Forms.GroupBox();
            this.PenInfoTextbox = new System.Windows.Forms.TextBox();
            this.GroupOffline = new System.Windows.Forms.GroupBox();
            this.OfflineDataDeleteButton = new System.Windows.Forms.Button();
            this.OfflineDataDownloadButton = new System.Windows.Forms.Button();
            this.OfflineDataListbox = new System.Windows.Forms.ListBox();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.PowerOffTimeInput = new System.Windows.Forms.NumericUpDown();
            this.PenCapPowerCheckbox = new System.Windows.Forms.CheckBox();
            this.PenTipPowerOnCheckbox = new System.Windows.Forms.CheckBox();
            this.BeepCheckbox = new System.Windows.Forms.CheckBox();
            this.OfflineDataCheckbox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.PowerProgressBar = new System.Windows.Forms.ProgressBar();
            this.StorageProgressBar = new System.Windows.Forms.ProgressBar();
            this.btnUpgrade = new System.Windows.Forms.Button();
            this.FirmwarePathTextbox = new System.Windows.Forms.TextBox();
            this.FirmwareVersionTextbox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.GroupConfig = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.btnPwdChange = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.OldPasswordTextbox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.NewPasswordTextbox = new System.Windows.Forms.TextBox();
            this.GroupUpdate = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ConsoleTextbox = new System.Windows.Forms.TextBox();
            this.groupBox2.SuspendLayout();
            this.GroupInfo.SuspendLayout();
            this.GroupOffline.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PowerOffTimeInput)).BeginInit();
            this.GroupConfig.SuspendLayout();
            this.GroupUpdate.SuspendLayout();
            this.SuspendLayout();
            // 
            // DevicesListbox
            // 
            this.DevicesListbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DevicesListbox.FormattingEnabled = true;
            this.DevicesListbox.ItemHeight = 12;
            this.DevicesListbox.Location = new System.Drawing.Point(11, 20);
            this.DevicesListbox.Name = "DevicesListbox";
            this.DevicesListbox.Size = new System.Drawing.Size(428, 158);
            this.DevicesListbox.TabIndex = 23;
            // 
            // SearchButton
            // 
            this.SearchButton.Location = new System.Drawing.Point(445, 20);
            this.SearchButton.Name = "SearchButton";
            this.SearchButton.Size = new System.Drawing.Size(84, 23);
            this.SearchButton.TabIndex = 22;
            this.SearchButton.Text = "Search";
            this.SearchButton.UseVisualStyleBackColor = true;
            this.SearchButton.Click += new System.EventHandler(this.BtnSearch_Click);
            // 
            // ConnectButton
            // 
            this.ConnectButton.Location = new System.Drawing.Point(445, 126);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(84, 23);
            this.ConnectButton.TabIndex = 17;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.DisconnectButton);
            this.groupBox2.Controls.Add(this.ConnectButton);
            this.groupBox2.Controls.Add(this.DevicesListbox);
            this.groupBox2.Controls.Add(this.SearchButton);
            this.groupBox2.Location = new System.Drawing.Point(14, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(540, 197);
            this.groupBox2.TabIndex = 32;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = " Search and select your device ";
            // 
            // DisconnectButton
            // 
            this.DisconnectButton.Location = new System.Drawing.Point(445, 155);
            this.DisconnectButton.Name = "DisconnectButton";
            this.DisconnectButton.Size = new System.Drawing.Size(84, 23);
            this.DisconnectButton.TabIndex = 24;
            this.DisconnectButton.Text = "Disconnect";
            this.DisconnectButton.UseVisualStyleBackColor = true;
            this.DisconnectButton.Click += new System.EventHandler(this.DisconnectButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(563, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(490, 701);
            this.groupBox1.TabIndex = 33;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " Write something on your NCode paper ";
            // 
            // GroupInfo
            // 
            this.GroupInfo.Controls.Add(this.PenInfoTextbox);
            this.GroupInfo.Enabled = false;
            this.GroupInfo.Location = new System.Drawing.Point(14, 216);
            this.GroupInfo.Name = "GroupInfo";
            this.GroupInfo.Size = new System.Drawing.Size(276, 153);
            this.GroupInfo.TabIndex = 37;
            this.GroupInfo.TabStop = false;
            this.GroupInfo.Text = "Information";
            // 
            // PenInfoTextbox
            // 
            this.PenInfoTextbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.PenInfoTextbox.Location = new System.Drawing.Point(18, 23);
            this.PenInfoTextbox.Multiline = true;
            this.PenInfoTextbox.Name = "PenInfoTextbox";
            this.PenInfoTextbox.ReadOnly = true;
            this.PenInfoTextbox.Size = new System.Drawing.Size(241, 120);
            this.PenInfoTextbox.TabIndex = 0;
            // 
            // GroupOffline
            // 
            this.GroupOffline.Controls.Add(this.OfflineDataDeleteButton);
            this.GroupOffline.Controls.Add(this.OfflineDataDownloadButton);
            this.GroupOffline.Controls.Add(this.OfflineDataListbox);
            this.GroupOffline.Enabled = false;
            this.GroupOffline.Location = new System.Drawing.Point(14, 375);
            this.GroupOffline.Name = "GroupOffline";
            this.GroupOffline.Size = new System.Drawing.Size(278, 214);
            this.GroupOffline.TabIndex = 36;
            this.GroupOffline.TabStop = false;
            this.GroupOffline.Text = "Offline Data";
            // 
            // OfflineDataDeleteButton
            // 
            this.OfflineDataDeleteButton.Location = new System.Drawing.Point(195, 178);
            this.OfflineDataDeleteButton.Name = "OfflineDataDeleteButton";
            this.OfflineDataDeleteButton.Size = new System.Drawing.Size(75, 23);
            this.OfflineDataDeleteButton.TabIndex = 2;
            this.OfflineDataDeleteButton.Text = "Delete";
            this.OfflineDataDeleteButton.UseVisualStyleBackColor = true;
            this.OfflineDataDeleteButton.Click += new System.EventHandler(this.OfflineDataDeleteButton_Click);
            // 
            // OfflineDataDownloadButton
            // 
            this.OfflineDataDownloadButton.Location = new System.Drawing.Point(7, 178);
            this.OfflineDataDownloadButton.Name = "OfflineDataDownloadButton";
            this.OfflineDataDownloadButton.Size = new System.Drawing.Size(75, 23);
            this.OfflineDataDownloadButton.TabIndex = 1;
            this.OfflineDataDownloadButton.Text = "Download";
            this.OfflineDataDownloadButton.UseVisualStyleBackColor = true;
            this.OfflineDataDownloadButton.Click += new System.EventHandler(this.OfflineDataDownloadButton_Click);
            // 
            // OfflineDataListbox
            // 
            this.OfflineDataListbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.OfflineDataListbox.FormattingEnabled = true;
            this.OfflineDataListbox.ItemHeight = 12;
            this.OfflineDataListbox.Location = new System.Drawing.Point(7, 21);
            this.OfflineDataListbox.Name = "OfflineDataListbox";
            this.OfflineDataListbox.Size = new System.Drawing.Size(263, 146);
            this.OfflineDataListbox.TabIndex = 0;
            // 
            // pictureBox
            // 
            this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox.BackColor = System.Drawing.Color.White;
            this.pictureBox.BackgroundImage = global::PenDemo.Properties.Resources.background;
            this.pictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox.Location = new System.Drawing.Point(581, 35);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(457, 664);
            this.pictureBox.TabIndex = 27;
            this.pictureBox.TabStop = false;
            // 
            // PowerOffTimeInput
            // 
            this.PowerOffTimeInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PowerOffTimeInput.Location = new System.Drawing.Point(155, 30);
            this.PowerOffTimeInput.Maximum = new decimal(new int[] {
            -1486618624,
            232830643,
            0,
            0});
            this.PowerOffTimeInput.Name = "PowerOffTimeInput";
            this.PowerOffTimeInput.Size = new System.Drawing.Size(83, 21);
            this.PowerOffTimeInput.TabIndex = 0;
            this.PowerOffTimeInput.ValueChanged += new System.EventHandler(this.PowerOffTimeInput_ValueChanged);
            // 
            // PenCapPowerCheckbox
            // 
            this.PenCapPowerCheckbox.AutoSize = true;
            this.PenCapPowerCheckbox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PenCapPowerCheckbox.Location = new System.Drawing.Point(22, 54);
            this.PenCapPowerCheckbox.Name = "PenCapPowerCheckbox";
            this.PenCapPowerCheckbox.Size = new System.Drawing.Size(149, 16);
            this.PenCapPowerCheckbox.TabIndex = 1;
            this.PenCapPowerCheckbox.Text = "Pen cap power control";
            this.PenCapPowerCheckbox.UseVisualStyleBackColor = true;
            this.PenCapPowerCheckbox.CheckedChanged += new System.EventHandler(this.PenCapPowerCheckbox_CheckedChanged);
            // 
            // PenTipPowerOnCheckbox
            // 
            this.PenTipPowerOnCheckbox.AutoSize = true;
            this.PenTipPowerOnCheckbox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PenTipPowerOnCheckbox.Location = new System.Drawing.Point(22, 76);
            this.PenTipPowerOnCheckbox.Name = "PenTipPowerOnCheckbox";
            this.PenTipPowerOnCheckbox.Size = new System.Drawing.Size(135, 16);
            this.PenTipPowerOnCheckbox.TabIndex = 2;
            this.PenTipPowerOnCheckbox.Text = "Power on by pen tip";
            this.PenTipPowerOnCheckbox.UseVisualStyleBackColor = true;
            this.PenTipPowerOnCheckbox.CheckedChanged += new System.EventHandler(this.PenTipPowerOnCheckbox_CheckedChanged);
            // 
            // BeepCheckbox
            // 
            this.BeepCheckbox.AutoSize = true;
            this.BeepCheckbox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BeepCheckbox.Location = new System.Drawing.Point(22, 99);
            this.BeepCheckbox.Name = "BeepCheckbox";
            this.BeepCheckbox.Size = new System.Drawing.Size(89, 16);
            this.BeepCheckbox.TabIndex = 3;
            this.BeepCheckbox.Text = "Beep sound";
            this.BeepCheckbox.UseVisualStyleBackColor = true;
            this.BeepCheckbox.CheckedChanged += new System.EventHandler(this.BeepCheckbox_CheckedChanged);
            // 
            // OfflineDataCheckbox
            // 
            this.OfflineDataCheckbox.AutoSize = true;
            this.OfflineDataCheckbox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.OfflineDataCheckbox.Location = new System.Drawing.Point(22, 122);
            this.OfflineDataCheckbox.Name = "OfflineDataCheckbox";
            this.OfflineDataCheckbox.Size = new System.Drawing.Size(84, 16);
            this.OfflineDataCheckbox.TabIndex = 5;
            this.OfflineDataCheckbox.Text = "Offline data";
            this.OfflineDataCheckbox.UseVisualStyleBackColor = true;
            this.OfflineDataCheckbox.CheckedChanged += new System.EventHandler(this.OfflineDataCheckbox_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 12);
            this.label1.TabIndex = 8;
            this.label1.Text = "Auto power off time ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 150);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 11;
            this.label4.Text = "Power";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 179);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 12);
            this.label5.TabIndex = 12;
            this.label5.Text = "Storage";
            // 
            // PowerProgressBar
            // 
            this.PowerProgressBar.Location = new System.Drawing.Point(59, 144);
            this.PowerProgressBar.Name = "PowerProgressBar";
            this.PowerProgressBar.Size = new System.Drawing.Size(175, 23);
            this.PowerProgressBar.TabIndex = 13;
            // 
            // StorageProgressBar
            // 
            this.StorageProgressBar.Location = new System.Drawing.Point(59, 173);
            this.StorageProgressBar.Name = "StorageProgressBar";
            this.StorageProgressBar.Size = new System.Drawing.Size(175, 23);
            this.StorageProgressBar.TabIndex = 14;
            // 
            // btnUpgrade
            // 
            this.btnUpgrade.Location = new System.Drawing.Point(163, 52);
            this.btnUpgrade.Name = "btnUpgrade";
            this.btnUpgrade.Size = new System.Drawing.Size(75, 21);
            this.btnUpgrade.TabIndex = 18;
            this.btnUpgrade.Text = "Update";
            this.btnUpgrade.UseVisualStyleBackColor = true;
            this.btnUpgrade.Click += new System.EventHandler(this.UpgradeButton_Click);
            // 
            // FirmwarePathTextbox
            // 
            this.FirmwarePathTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FirmwarePathTextbox.Location = new System.Drawing.Point(64, 25);
            this.FirmwarePathTextbox.Name = "FirmwarePathTextbox";
            this.FirmwarePathTextbox.Size = new System.Drawing.Size(174, 21);
            this.FirmwarePathTextbox.TabIndex = 19;
            this.FirmwarePathTextbox.Click += new System.EventHandler(this.FirmwarePathTextbox_Click);
            // 
            // FirmwareVersionTextbox
            // 
            this.FirmwareVersionTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FirmwareVersionTextbox.Location = new System.Drawing.Point(64, 52);
            this.FirmwareVersionTextbox.Name = "FirmwareVersionTextbox";
            this.FirmwareVersionTextbox.Size = new System.Drawing.Size(93, 21);
            this.FirmwareVersionTextbox.TabIndex = 20;
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
            // GroupConfig
            // 
            this.GroupConfig.Controls.Add(this.label11);
            this.GroupConfig.Controls.Add(this.btnPwdChange);
            this.GroupConfig.Controls.Add(this.PenCapPowerCheckbox);
            this.GroupConfig.Controls.Add(this.label4);
            this.GroupConfig.Controls.Add(this.label7);
            this.GroupConfig.Controls.Add(this.label5);
            this.GroupConfig.Controls.Add(this.OldPasswordTextbox);
            this.GroupConfig.Controls.Add(this.label1);
            this.GroupConfig.Controls.Add(this.label6);
            this.GroupConfig.Controls.Add(this.PowerProgressBar);
            this.GroupConfig.Controls.Add(this.NewPasswordTextbox);
            this.GroupConfig.Controls.Add(this.StorageProgressBar);
            this.GroupConfig.Controls.Add(this.OfflineDataCheckbox);
            this.GroupConfig.Controls.Add(this.BeepCheckbox);
            this.GroupConfig.Controls.Add(this.PenTipPowerOnCheckbox);
            this.GroupConfig.Controls.Add(this.PowerOffTimeInput);
            this.GroupConfig.Enabled = false;
            this.GroupConfig.Location = new System.Drawing.Point(296, 216);
            this.GroupConfig.Name = "GroupConfig";
            this.GroupConfig.Size = new System.Drawing.Size(258, 280);
            this.GroupConfig.TabIndex = 35;
            this.GroupConfig.TabStop = false;
            this.GroupConfig.Text = "Configuration";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(18, 206);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(111, 12);
            this.label11.TabIndex = 18;
            this.label11.Text = "* Setup password:";
            // 
            // btnPwdChange
            // 
            this.btnPwdChange.Location = new System.Drawing.Point(168, 250);
            this.btnPwdChange.Name = "btnPwdChange";
            this.btnPwdChange.Size = new System.Drawing.Size(70, 23);
            this.btnPwdChange.TabIndex = 17;
            this.btnPwdChange.Text = "Change";
            this.btnPwdChange.UseVisualStyleBackColor = true;
            this.btnPwdChange.Click += new System.EventHandler(this.PasswordChangeButton_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(23, 255);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 12);
            this.label7.TabIndex = 16;
            this.label7.Text = "new";
            // 
            // OldPasswordTextbox
            // 
            this.OldPasswordTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.OldPasswordTextbox.Location = new System.Drawing.Point(57, 224);
            this.OldPasswordTextbox.Name = "OldPasswordTextbox";
            this.OldPasswordTextbox.Size = new System.Drawing.Size(105, 21);
            this.OldPasswordTextbox.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(30, 228);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(22, 12);
            this.label6.TabIndex = 15;
            this.label6.Text = "old";
            // 
            // NewPasswordTextbox
            // 
            this.NewPasswordTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.NewPasswordTextbox.Location = new System.Drawing.Point(57, 251);
            this.NewPasswordTextbox.Name = "NewPasswordTextbox";
            this.NewPasswordTextbox.Size = new System.Drawing.Size(105, 21);
            this.NewPasswordTextbox.TabIndex = 0;
            // 
            // GroupUpdate
            // 
            this.GroupUpdate.Controls.Add(this.label2);
            this.GroupUpdate.Controls.Add(this.label8);
            this.GroupUpdate.Controls.Add(this.btnUpgrade);
            this.GroupUpdate.Controls.Add(this.FirmwareVersionTextbox);
            this.GroupUpdate.Controls.Add(this.FirmwarePathTextbox);
            this.GroupUpdate.Enabled = false;
            this.GroupUpdate.Location = new System.Drawing.Point(296, 501);
            this.GroupUpdate.Name = "GroupUpdate";
            this.GroupUpdate.Size = new System.Drawing.Size(261, 88);
            this.GroupUpdate.TabIndex = 39;
            this.GroupUpdate.TabStop = false;
            this.GroupUpdate.Text = "Firmware Update";
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
            // ConsoleTextbox
            // 
            this.ConsoleTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ConsoleTextbox.Cursor = System.Windows.Forms.Cursors.No;
            this.ConsoleTextbox.Location = new System.Drawing.Point(14, 596);
            this.ConsoleTextbox.Multiline = true;
            this.ConsoleTextbox.Name = "ConsoleTextbox";
            this.ConsoleTextbox.ReadOnly = true;
            this.ConsoleTextbox.Size = new System.Drawing.Size(543, 118);
            this.ConsoleTextbox.TabIndex = 40;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1065, 729);
            this.Controls.Add(this.ConsoleTextbox);
            this.Controls.Add(this.GroupUpdate);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.GroupInfo);
            this.Controls.Add(this.GroupOffline);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.GroupConfig);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.TransparencyKey = System.Drawing.Color.Blue;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.groupBox2.ResumeLayout(false);
            this.GroupInfo.ResumeLayout(false);
            this.GroupInfo.PerformLayout();
            this.GroupOffline.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PowerOffTimeInput)).EndInit();
            this.GroupConfig.ResumeLayout(false);
            this.GroupConfig.PerformLayout();
            this.GroupUpdate.ResumeLayout(false);
            this.GroupUpdate.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox DevicesListbox;
        private System.Windows.Forms.Button SearchButton;
        private System.Windows.Forms.Button ConnectButton;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox GroupInfo;
        private System.Windows.Forms.GroupBox GroupOffline;
        private System.Windows.Forms.Button OfflineDataDeleteButton;
        private System.Windows.Forms.Button OfflineDataDownloadButton;
        private System.Windows.Forms.ListBox OfflineDataListbox;
        private System.Windows.Forms.TextBox PenInfoTextbox;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.NumericUpDown PowerOffTimeInput;
        private System.Windows.Forms.CheckBox PenCapPowerCheckbox;
        private System.Windows.Forms.CheckBox PenTipPowerOnCheckbox;
        private System.Windows.Forms.CheckBox BeepCheckbox;
        private System.Windows.Forms.CheckBox OfflineDataCheckbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ProgressBar PowerProgressBar;
        private System.Windows.Forms.ProgressBar StorageProgressBar;
        private System.Windows.Forms.Button btnUpgrade;
        private System.Windows.Forms.TextBox FirmwarePathTextbox;
        private System.Windows.Forms.TextBox FirmwareVersionTextbox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox GroupConfig;
        private System.Windows.Forms.GroupBox GroupUpdate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button DisconnectButton;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnPwdChange;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox OldPasswordTextbox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox NewPasswordTextbox;
        private System.Windows.Forms.TextBox ConsoleTextbox;
    }
}

