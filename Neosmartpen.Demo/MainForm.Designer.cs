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
            DevicesListbox = new System.Windows.Forms.ListBox();
            SearchButton = new System.Windows.Forms.Button();
            ConnectButton = new System.Windows.Forms.Button();
            groupBox2 = new System.Windows.Forms.GroupBox();
            DisconnectButton = new System.Windows.Forms.Button();
            groupBox1 = new System.Windows.Forms.GroupBox();
            GroupInfo = new System.Windows.Forms.GroupBox();
            PenInfoTextbox = new System.Windows.Forms.TextBox();
            GroupOffline = new System.Windows.Forms.GroupBox();
            OfflineDataDeleteButton = new System.Windows.Forms.Button();
            OfflineDataDownloadButton = new System.Windows.Forms.Button();
            OfflineDataListbox = new System.Windows.Forms.ListBox();
            colorDialog1 = new System.Windows.Forms.ColorDialog();
            PowerOffTimeInput = new System.Windows.Forms.NumericUpDown();
            PenCapPowerCheckbox = new System.Windows.Forms.CheckBox();
            PenTipPowerOnCheckbox = new System.Windows.Forms.CheckBox();
            BeepCheckbox = new System.Windows.Forms.CheckBox();
            OfflineDataCheckbox = new System.Windows.Forms.CheckBox();
            label1 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            PowerProgressBar = new System.Windows.Forms.ProgressBar();
            StorageProgressBar = new System.Windows.Forms.ProgressBar();
            btnUpgrade = new System.Windows.Forms.Button();
            FirmwarePathTextbox = new System.Windows.Forms.TextBox();
            FirmwareVersionTextbox = new System.Windows.Forms.TextBox();
            GroupConfig = new System.Windows.Forms.GroupBox();
            label3 = new System.Windows.Forms.Label();
            PressureSensorStep = new System.Windows.Forms.NumericUpDown();
            label11 = new System.Windows.Forms.Label();
            btnPwdChange = new System.Windows.Forms.Button();
            label7 = new System.Windows.Forms.Label();
            OldPasswordTextbox = new System.Windows.Forms.TextBox();
            label6 = new System.Windows.Forms.Label();
            NewPasswordTextbox = new System.Windows.Forms.TextBox();
            GroupUpdate = new System.Windows.Forms.GroupBox();
            label2 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            ConsoleTextbox = new System.Windows.Forms.TextBox();
            pictureBox = new System.Windows.Forms.PictureBox();
            gbJsonView = new System.Windows.Forms.GroupBox();
            btnConvertToJSON = new System.Windows.Forms.Button();
            tbSerializingResult = new System.Windows.Forms.TextBox();
            groupBox2.SuspendLayout();
            GroupInfo.SuspendLayout();
            GroupOffline.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PowerOffTimeInput).BeginInit();
            GroupConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PressureSensorStep).BeginInit();
            GroupUpdate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
            gbJsonView.SuspendLayout();
            SuspendLayout();
            // 
            // DevicesListbox
            // 
            DevicesListbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            DevicesListbox.FormattingEnabled = true;
            DevicesListbox.ItemHeight = 15;
            DevicesListbox.Location = new System.Drawing.Point(11, 25);
            DevicesListbox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            DevicesListbox.Name = "DevicesListbox";
            DevicesListbox.Size = new System.Drawing.Size(428, 197);
            DevicesListbox.TabIndex = 23;
            // 
            // SearchButton
            // 
            SearchButton.Location = new System.Drawing.Point(445, 25);
            SearchButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            SearchButton.Name = "SearchButton";
            SearchButton.Size = new System.Drawing.Size(84, 29);
            SearchButton.TabIndex = 22;
            SearchButton.Text = "Search";
            SearchButton.UseVisualStyleBackColor = true;
            SearchButton.Click += BtnSearch_Click;
            // 
            // ConnectButton
            // 
            ConnectButton.Location = new System.Drawing.Point(445, 158);
            ConnectButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            ConnectButton.Name = "ConnectButton";
            ConnectButton.Size = new System.Drawing.Size(84, 29);
            ConnectButton.TabIndex = 17;
            ConnectButton.Text = "Connect";
            ConnectButton.UseVisualStyleBackColor = true;
            ConnectButton.Click += ConnectButton_Click;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(DisconnectButton);
            groupBox2.Controls.Add(ConnectButton);
            groupBox2.Controls.Add(DevicesListbox);
            groupBox2.Controls.Add(SearchButton);
            groupBox2.Location = new System.Drawing.Point(14, 16);
            groupBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            groupBox2.Size = new System.Drawing.Size(540, 230);
            groupBox2.TabIndex = 32;
            groupBox2.TabStop = false;
            groupBox2.Text = " Search and select your device ";
            // 
            // DisconnectButton
            // 
            DisconnectButton.Location = new System.Drawing.Point(445, 194);
            DisconnectButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            DisconnectButton.Name = "DisconnectButton";
            DisconnectButton.Size = new System.Drawing.Size(84, 29);
            DisconnectButton.TabIndex = 24;
            DisconnectButton.Text = "Disconnect";
            DisconnectButton.UseVisualStyleBackColor = true;
            DisconnectButton.Click += DisconnectButton_Click;
            // 
            // groupBox1
            // 
            groupBox1.Location = new System.Drawing.Point(563, 16);
            groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            groupBox1.Size = new System.Drawing.Size(490, 876);
            groupBox1.TabIndex = 33;
            groupBox1.TabStop = false;
            groupBox1.Text = "Write something on your NCode paper ";
            // 
            // GroupInfo
            // 
            GroupInfo.Controls.Add(PenInfoTextbox);
            GroupInfo.Enabled = false;
            GroupInfo.Location = new System.Drawing.Point(14, 250);
            GroupInfo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            GroupInfo.Name = "GroupInfo";
            GroupInfo.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            GroupInfo.Size = new System.Drawing.Size(276, 191);
            GroupInfo.TabIndex = 37;
            GroupInfo.TabStop = false;
            GroupInfo.Text = "Information";
            // 
            // PenInfoTextbox
            // 
            PenInfoTextbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            PenInfoTextbox.Location = new System.Drawing.Point(18, 29);
            PenInfoTextbox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            PenInfoTextbox.Multiline = true;
            PenInfoTextbox.Name = "PenInfoTextbox";
            PenInfoTextbox.ReadOnly = true;
            PenInfoTextbox.Size = new System.Drawing.Size(241, 150);
            PenInfoTextbox.TabIndex = 0;
            // 
            // GroupOffline
            // 
            GroupOffline.Controls.Add(OfflineDataDeleteButton);
            GroupOffline.Controls.Add(OfflineDataDownloadButton);
            GroupOffline.Controls.Add(OfflineDataListbox);
            GroupOffline.Enabled = false;
            GroupOffline.Location = new System.Drawing.Point(14, 444);
            GroupOffline.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            GroupOffline.Name = "GroupOffline";
            GroupOffline.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            GroupOffline.Size = new System.Drawing.Size(276, 291);
            GroupOffline.TabIndex = 36;
            GroupOffline.TabStop = false;
            GroupOffline.Text = "Offline Data";
            // 
            // OfflineDataDeleteButton
            // 
            OfflineDataDeleteButton.Location = new System.Drawing.Point(195, 255);
            OfflineDataDeleteButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            OfflineDataDeleteButton.Name = "OfflineDataDeleteButton";
            OfflineDataDeleteButton.Size = new System.Drawing.Size(75, 29);
            OfflineDataDeleteButton.TabIndex = 2;
            OfflineDataDeleteButton.Text = "Delete";
            OfflineDataDeleteButton.UseVisualStyleBackColor = true;
            OfflineDataDeleteButton.Click += OfflineDataDeleteButton_Click;
            // 
            // OfflineDataDownloadButton
            // 
            OfflineDataDownloadButton.Location = new System.Drawing.Point(7, 255);
            OfflineDataDownloadButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            OfflineDataDownloadButton.Name = "OfflineDataDownloadButton";
            OfflineDataDownloadButton.Size = new System.Drawing.Size(75, 29);
            OfflineDataDownloadButton.TabIndex = 1;
            OfflineDataDownloadButton.Text = "Download";
            OfflineDataDownloadButton.UseVisualStyleBackColor = true;
            OfflineDataDownloadButton.Click += OfflineDataDownloadButton_Click;
            // 
            // OfflineDataListbox
            // 
            OfflineDataListbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            OfflineDataListbox.FormattingEnabled = true;
            OfflineDataListbox.ItemHeight = 15;
            OfflineDataListbox.Location = new System.Drawing.Point(7, 26);
            OfflineDataListbox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            OfflineDataListbox.Name = "OfflineDataListbox";
            OfflineDataListbox.Size = new System.Drawing.Size(263, 212);
            OfflineDataListbox.TabIndex = 0;
            // 
            // PowerOffTimeInput
            // 
            PowerOffTimeInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            PowerOffTimeInput.Location = new System.Drawing.Point(155, 23);
            PowerOffTimeInput.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            PowerOffTimeInput.Maximum = new decimal(new int[] { -1486618624, 232830643, 0, 0 });
            PowerOffTimeInput.Name = "PowerOffTimeInput";
            PowerOffTimeInput.Size = new System.Drawing.Size(83, 23);
            PowerOffTimeInput.TabIndex = 0;
            PowerOffTimeInput.ValueChanged += PowerOffTimeInput_ValueChanged;
            // 
            // PenCapPowerCheckbox
            // 
            PenCapPowerCheckbox.AutoSize = true;
            PenCapPowerCheckbox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            PenCapPowerCheckbox.Location = new System.Drawing.Point(22, 51);
            PenCapPowerCheckbox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            PenCapPowerCheckbox.Name = "PenCapPowerCheckbox";
            PenCapPowerCheckbox.Size = new System.Drawing.Size(145, 19);
            PenCapPowerCheckbox.TabIndex = 1;
            PenCapPowerCheckbox.Text = "Pen cap power control";
            PenCapPowerCheckbox.UseVisualStyleBackColor = true;
            PenCapPowerCheckbox.CheckedChanged += PenCapPowerCheckbox_CheckedChanged;
            // 
            // PenTipPowerOnCheckbox
            // 
            PenTipPowerOnCheckbox.AutoSize = true;
            PenTipPowerOnCheckbox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            PenTipPowerOnCheckbox.Location = new System.Drawing.Point(22, 75);
            PenTipPowerOnCheckbox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            PenTipPowerOnCheckbox.Name = "PenTipPowerOnCheckbox";
            PenTipPowerOnCheckbox.Size = new System.Drawing.Size(133, 19);
            PenTipPowerOnCheckbox.TabIndex = 2;
            PenTipPowerOnCheckbox.Text = "Power on by pen tip";
            PenTipPowerOnCheckbox.UseVisualStyleBackColor = true;
            PenTipPowerOnCheckbox.CheckedChanged += PenTipPowerOnCheckbox_CheckedChanged;
            // 
            // BeepCheckbox
            // 
            BeepCheckbox.AutoSize = true;
            BeepCheckbox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            BeepCheckbox.Location = new System.Drawing.Point(22, 100);
            BeepCheckbox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            BeepCheckbox.Name = "BeepCheckbox";
            BeepCheckbox.Size = new System.Drawing.Size(86, 19);
            BeepCheckbox.TabIndex = 3;
            BeepCheckbox.Text = "Beep sound";
            BeepCheckbox.UseVisualStyleBackColor = true;
            BeepCheckbox.CheckedChanged += BeepCheckbox_CheckedChanged;
            // 
            // OfflineDataCheckbox
            // 
            OfflineDataCheckbox.AutoSize = true;
            OfflineDataCheckbox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            OfflineDataCheckbox.Location = new System.Drawing.Point(22, 126);
            OfflineDataCheckbox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            OfflineDataCheckbox.Name = "OfflineDataCheckbox";
            OfflineDataCheckbox.Size = new System.Drawing.Size(86, 19);
            OfflineDataCheckbox.TabIndex = 5;
            OfflineDataCheckbox.Text = "Offline data";
            OfflineDataCheckbox.UseVisualStyleBackColor = true;
            OfflineDataCheckbox.CheckedChanged += OfflineDataCheckbox_CheckedChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(18, 26);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(121, 15);
            label1.TabIndex = 8;
            label1.Text = "Auto power off time ";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(15, 191);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(40, 15);
            label4.TabIndex = 11;
            label4.Text = "Power";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(8, 227);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(48, 15);
            label5.TabIndex = 12;
            label5.Text = "Storage";
            // 
            // PowerProgressBar
            // 
            PowerProgressBar.Location = new System.Drawing.Point(59, 183);
            PowerProgressBar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            PowerProgressBar.Name = "PowerProgressBar";
            PowerProgressBar.Size = new System.Drawing.Size(175, 29);
            PowerProgressBar.TabIndex = 13;
            // 
            // StorageProgressBar
            // 
            StorageProgressBar.Location = new System.Drawing.Point(59, 219);
            StorageProgressBar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            StorageProgressBar.Name = "StorageProgressBar";
            StorageProgressBar.Size = new System.Drawing.Size(175, 29);
            StorageProgressBar.TabIndex = 14;
            // 
            // btnUpgrade
            // 
            btnUpgrade.Location = new System.Drawing.Point(168, 72);
            btnUpgrade.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnUpgrade.Name = "btnUpgrade";
            btnUpgrade.Size = new System.Drawing.Size(75, 26);
            btnUpgrade.TabIndex = 18;
            btnUpgrade.Text = "Update";
            btnUpgrade.UseVisualStyleBackColor = true;
            btnUpgrade.Click += UpgradeButton_Click;
            // 
            // FirmwarePathTextbox
            // 
            FirmwarePathTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            FirmwarePathTextbox.Location = new System.Drawing.Point(69, 39);
            FirmwarePathTextbox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            FirmwarePathTextbox.Name = "FirmwarePathTextbox";
            FirmwarePathTextbox.Size = new System.Drawing.Size(174, 23);
            FirmwarePathTextbox.TabIndex = 19;
            FirmwarePathTextbox.Click += FirmwarePathTextbox_Click;
            // 
            // FirmwareVersionTextbox
            // 
            FirmwareVersionTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            FirmwareVersionTextbox.Location = new System.Drawing.Point(69, 72);
            FirmwareVersionTextbox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            FirmwareVersionTextbox.Name = "FirmwareVersionTextbox";
            FirmwareVersionTextbox.Size = new System.Drawing.Size(93, 23);
            FirmwareVersionTextbox.TabIndex = 20;
            // 
            // GroupConfig
            // 
            GroupConfig.Controls.Add(label3);
            GroupConfig.Controls.Add(PressureSensorStep);
            GroupConfig.Controls.Add(label11);
            GroupConfig.Controls.Add(btnPwdChange);
            GroupConfig.Controls.Add(PenCapPowerCheckbox);
            GroupConfig.Controls.Add(label4);
            GroupConfig.Controls.Add(label7);
            GroupConfig.Controls.Add(label5);
            GroupConfig.Controls.Add(OldPasswordTextbox);
            GroupConfig.Controls.Add(label1);
            GroupConfig.Controls.Add(label6);
            GroupConfig.Controls.Add(PowerProgressBar);
            GroupConfig.Controls.Add(NewPasswordTextbox);
            GroupConfig.Controls.Add(StorageProgressBar);
            GroupConfig.Controls.Add(OfflineDataCheckbox);
            GroupConfig.Controls.Add(BeepCheckbox);
            GroupConfig.Controls.Add(PenTipPowerOnCheckbox);
            GroupConfig.Controls.Add(PowerOffTimeInput);
            GroupConfig.Enabled = false;
            GroupConfig.Location = new System.Drawing.Point(296, 250);
            GroupConfig.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            GroupConfig.Name = "GroupConfig";
            GroupConfig.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            GroupConfig.Size = new System.Drawing.Size(258, 360);
            GroupConfig.TabIndex = 35;
            GroupConfig.TabStop = false;
            GroupConfig.Text = "Configuration";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(19, 155);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(107, 15);
            label3.TabIndex = 20;
            label3.Text = "Pressure sensitivity";
            // 
            // PressureSensorStep
            // 
            PressureSensorStep.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            PressureSensorStep.Location = new System.Drawing.Point(156, 151);
            PressureSensorStep.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            PressureSensorStep.Maximum = new decimal(new int[] { -1486618624, 232830643, 0, 0 });
            PressureSensorStep.Name = "PressureSensorStep";
            PressureSensorStep.Size = new System.Drawing.Size(83, 23);
            PressureSensorStep.TabIndex = 19;
            PressureSensorStep.ValueChanged += PressureSensorStep_ValueChanged;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new System.Drawing.Point(18, 258);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(104, 15);
            label11.TabIndex = 18;
            label11.Text = "* Setup password:";
            // 
            // btnPwdChange
            // 
            btnPwdChange.Location = new System.Drawing.Point(168, 312);
            btnPwdChange.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnPwdChange.Name = "btnPwdChange";
            btnPwdChange.Size = new System.Drawing.Size(70, 29);
            btnPwdChange.TabIndex = 17;
            btnPwdChange.Text = "Change";
            btnPwdChange.UseVisualStyleBackColor = true;
            btnPwdChange.Click += PasswordChangeButton_Click;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(23, 319);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(29, 15);
            label7.TabIndex = 16;
            label7.Text = "new";
            // 
            // OldPasswordTextbox
            // 
            OldPasswordTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            OldPasswordTextbox.Location = new System.Drawing.Point(57, 280);
            OldPasswordTextbox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            OldPasswordTextbox.Name = "OldPasswordTextbox";
            OldPasswordTextbox.Size = new System.Drawing.Size(105, 23);
            OldPasswordTextbox.TabIndex = 0;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(30, 285);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(24, 15);
            label6.TabIndex = 15;
            label6.Text = "old";
            // 
            // NewPasswordTextbox
            // 
            NewPasswordTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            NewPasswordTextbox.Location = new System.Drawing.Point(57, 314);
            NewPasswordTextbox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            NewPasswordTextbox.Name = "NewPasswordTextbox";
            NewPasswordTextbox.Size = new System.Drawing.Size(105, 23);
            NewPasswordTextbox.TabIndex = 0;
            // 
            // GroupUpdate
            // 
            GroupUpdate.Controls.Add(label2);
            GroupUpdate.Controls.Add(label8);
            GroupUpdate.Controls.Add(btnUpgrade);
            GroupUpdate.Controls.Add(FirmwareVersionTextbox);
            GroupUpdate.Controls.Add(FirmwarePathTextbox);
            GroupUpdate.Enabled = false;
            GroupUpdate.Location = new System.Drawing.Point(296, 612);
            GroupUpdate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            GroupUpdate.Name = "GroupUpdate";
            GroupUpdate.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            GroupUpdate.Size = new System.Drawing.Size(261, 122);
            GroupUpdate.TabIndex = 39;
            GroupUpdate.TabStop = false;
            GroupUpdate.Text = "Firmware Update";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(40, 44);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(25, 15);
            label2.TabIndex = 22;
            label2.Text = "File";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(17, 78);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(47, 15);
            label8.TabIndex = 21;
            label8.Text = "Version";
            // 
            // ConsoleTextbox
            // 
            ConsoleTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            ConsoleTextbox.Cursor = System.Windows.Forms.Cursors.No;
            ConsoleTextbox.Location = new System.Drawing.Point(14, 742);
            ConsoleTextbox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            ConsoleTextbox.Multiline = true;
            ConsoleTextbox.Name = "ConsoleTextbox";
            ConsoleTextbox.ReadOnly = true;
            ConsoleTextbox.Size = new System.Drawing.Size(543, 150);
            ConsoleTextbox.TabIndex = 40;
            // 
            // pictureBox
            // 
            pictureBox.BackColor = System.Drawing.Color.White;
            pictureBox.BackgroundImage = Properties.Resources.background;
            pictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            pictureBox.Location = new System.Drawing.Point(581, 44);
            pictureBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            pictureBox.Name = "pictureBox";
            pictureBox.Size = new System.Drawing.Size(457, 830);
            pictureBox.TabIndex = 27;
            pictureBox.TabStop = false;
            // 
            // gbJsonView
            // 
            gbJsonView.Controls.Add(btnConvertToJSON);
            gbJsonView.Controls.Add(tbSerializingResult);
            gbJsonView.Location = new System.Drawing.Point(1059, 16);
            gbJsonView.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            gbJsonView.Name = "gbJsonView";
            gbJsonView.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            gbJsonView.Size = new System.Drawing.Size(420, 876);
            gbJsonView.TabIndex = 34;
            gbJsonView.TabStop = false;
            gbJsonView.Text = "Transform it into JSON format";
            // 
            // btnConvertToJSON
            // 
            btnConvertToJSON.Location = new System.Drawing.Point(7, 811);
            btnConvertToJSON.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnConvertToJSON.Name = "btnConvertToJSON";
            btnConvertToJSON.Size = new System.Drawing.Size(407, 46);
            btnConvertToJSON.TabIndex = 1;
            btnConvertToJSON.Text = "To JSON";
            btnConvertToJSON.UseVisualStyleBackColor = true;
            btnConvertToJSON.Click += BtnConvertToJSON_Click;
            // 
            // tbSerializingResult
            // 
            tbSerializingResult.Location = new System.Drawing.Point(7, 25);
            tbSerializingResult.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            tbSerializingResult.Multiline = true;
            tbSerializingResult.Name = "tbSerializingResult";
            tbSerializingResult.ReadOnly = true;
            tbSerializingResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            tbSerializingResult.Size = new System.Drawing.Size(407, 778);
            tbSerializingResult.TabIndex = 0;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1491, 911);
            Controls.Add(gbJsonView);
            Controls.Add(ConsoleTextbox);
            Controls.Add(GroupUpdate);
            Controls.Add(pictureBox);
            Controls.Add(GroupInfo);
            Controls.Add(GroupOffline);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(GroupConfig);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            MaximizeBox = false;
            Name = "MainForm";
            TransparencyKey = System.Drawing.Color.Blue;
            FormClosed += MainForm_FormClosed;
            groupBox2.ResumeLayout(false);
            GroupInfo.ResumeLayout(false);
            GroupInfo.PerformLayout();
            GroupOffline.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)PowerOffTimeInput).EndInit();
            GroupConfig.ResumeLayout(false);
            GroupConfig.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)PressureSensorStep).EndInit();
            GroupUpdate.ResumeLayout(false);
            GroupUpdate.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
            gbJsonView.ResumeLayout(false);
            gbJsonView.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox gbJsonView;
        private System.Windows.Forms.Button btnConvertToJSON;
        private System.Windows.Forms.TextBox tbSerializingResult;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown PressureSensorStep;
    }
}

