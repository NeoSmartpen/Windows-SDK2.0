using System;
using System.Windows.Forms;

namespace PenDemo
{
    public partial class ProgressForm : Form
    {
        public ProgressForm()
        {
            InitializeComponent();
        }

        public void SetStatus( string title, int total, int amountDone )
        {
            Text = title;

            progressBar1.Maximum = total;
            progressBar1.Value = amountDone > total ? total : amountDone;

            label1.Text = total == 0 && amountDone == 0 ? "" : String.Format( "( {0} / {1} )", amountDone, total );
        }
    }
}
