namespace Neosmartpen.Net.Usb.Demo
{
    partial class UpdateForm
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
            this.pbUpdateProgress = new System.Windows.Forms.ProgressBar();
            this.tbFirmwareFilePath = new System.Windows.Forms.TextBox();
            this.tbFirmwareVersion = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.tbFirmwareVersion);
            this.groupBox1.Controls.Add(this.tbFirmwareFilePath);
            this.groupBox1.Location = new System.Drawing.Point(15, 16);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(388, 134);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Firmware Information";
            // 
            // pbUpdateProgress
            // 
            this.pbUpdateProgress.Location = new System.Drawing.Point(15, 161);
            this.pbUpdateProgress.Name = "pbUpdateProgress";
            this.pbUpdateProgress.Size = new System.Drawing.Size(387, 31);
            this.pbUpdateProgress.Step = 1;
            this.pbUpdateProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbUpdateProgress.TabIndex = 1;
            // 
            // tbFirmwareFilePath
            // 
            this.tbFirmwareFilePath.Location = new System.Drawing.Point(16, 24);
            this.tbFirmwareFilePath.Name = "tbFirmwareFilePath";
            this.tbFirmwareFilePath.ReadOnly = true;
            this.tbFirmwareFilePath.Size = new System.Drawing.Size(358, 21);
            this.tbFirmwareFilePath.TabIndex = 0;
            this.tbFirmwareFilePath.Text = "Click here to select new firmware file";
            this.tbFirmwareFilePath.Click += new System.EventHandler(this.tbFirmwareFilePath_Click);
            // 
            // tbFirmwareVersion
            // 
            this.tbFirmwareVersion.Location = new System.Drawing.Point(16, 52);
            this.tbFirmwareVersion.Name = "tbFirmwareVersion";
            this.tbFirmwareVersion.Size = new System.Drawing.Size(358, 21);
            this.tbFirmwareVersion.TabIndex = 1;
            this.tbFirmwareVersion.Text = "Enter new firmware version";
            this.tbFirmwareVersion.Enter += new System.EventHandler(this.tbFirmwareVersion_Enter);
            this.tbFirmwareVersion.Leave += new System.EventHandler(this.tbFirmwareVersion_Leave);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(16, 86);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(358, 32);
            this.button1.TabIndex = 2;
            this.button1.Text = "Update";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // UpdateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 207);
            this.Controls.Add(this.pbUpdateProgress);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdateForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Firmware Update";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox tbFirmwareVersion;
        private System.Windows.Forms.TextBox tbFirmwareFilePath;
        private System.Windows.Forms.ProgressBar pbUpdateProgress;
    }
}