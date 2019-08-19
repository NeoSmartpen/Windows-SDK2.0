using System;
using System.Windows.Forms;

namespace Neosmartpen.Net.Usb.Demo
{
    public partial class UpdateForm : Form
    {
        public string PortName { get; set; }

        public delegate void OnClickedUpdate(string portName, string filePath, string firmwareVersion);

        private OnClickedUpdate onClickedUpdate;

        public UpdateForm(string portName, OnClickedUpdate onClickedUpdate)
        {
            PortName = portName;
            this.onClickedUpdate = onClickedUpdate;
            InitializeComponent();
        }

        public void SetStatus(int total, int amountDone)
        {
            pbUpdateProgress.Maximum = total;
            pbUpdateProgress.Value = amountDone > total ? total : amountDone;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbFirmwareFilePath.Text) || tbFirmwareFilePath.Text == "Click here to select new firmware file")
            {
                MessageBox.Show("Please select new firmware file");
                return;
            }

            if (string.IsNullOrEmpty(tbFirmwareVersion.Text) || tbFirmwareVersion.Text == "Enter new firmware version")
            {
                MessageBox.Show("Please enter version of new firmware file");
                return;
            }

            tbFirmwareFilePath.Enabled = false;
            tbFirmwareVersion.Enabled = false;
            button1.Enabled = false;

            this.onClickedUpdate?.Invoke(PortName, tbFirmwareFilePath.Text, tbFirmwareVersion.Text);
        }

        private void tbFirmwareFilePath_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Firmware Files|*._v_";
            openFileDialog1.Title = "Select a Firmware File";

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tbFirmwareFilePath.Text = openFileDialog1.FileName;
            }
        }

        private void tbFirmwareVersion_Enter(object sender, EventArgs e)
        {
            if (tbFirmwareVersion.Text == "Enter new firmware version")
                tbFirmwareVersion.Text = "";
        }

        private void tbFirmwareVersion_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbFirmwareVersion.Text))
                tbFirmwareVersion.Text = "Enter new firmware version";
        }
    }
}
