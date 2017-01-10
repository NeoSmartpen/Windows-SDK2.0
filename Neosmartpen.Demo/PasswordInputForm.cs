using System;
using System.Windows.Forms;

namespace PenDemo
{
    public partial class PasswordInputForm : Form
    {
        public delegate void OnEnterPassword( string password );

        OnEnterPassword mDelegate;

        public PasswordInputForm( OnEnterPassword dele )
        {
            InitializeComponent();

            mDelegate = dele;
        }

        private void btnSubmit_Click( object sender, EventArgs e )
        {
            if ( tbPassword.Text == "" )
            {
                MessageBox.Show( "Your pen is locked.\r\nPlease input your password." );
                return;
            }
            
            Close();

            mDelegate( tbPassword.Text );

            tbPassword.Text = "";
        }
    }
}
