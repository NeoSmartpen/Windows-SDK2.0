using System;
using System.Windows.Forms;

namespace PenDemo
{
    public partial class PasswordInputForm : Form
    {
        public delegate void OnEnterPassword(string password);

        private OnEnterPassword OnEntererdPassword;

        public PasswordInputForm(OnEnterPassword dele)
        {
            InitializeComponent();
            OnEntererdPassword = dele;
        }

        public void SetStatus(int retryCount, int resetCount)
        {
            StatusLabel.Text = retryCount + "/" + resetCount;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (PasswordTextbox.Text == "")
            {
                MessageBox.Show("Your device is locked.\r\nEnter your password to unlock it.");
                return;
            }
            
            Close();
            OnEntererdPassword(PasswordTextbox.Text);
            PasswordTextbox.Text = "";
        }
    }
}
