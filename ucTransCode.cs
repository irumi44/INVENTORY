using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUSHA.form.user_control
{
    public partial class ucTransCode : UserControl
    {
        private DateTime startTime;
        private int secondsToWait = 3;
        private Timer t;
        public event EventHandler DoneDelegate;
        public string Order_id { get; set; }
        public ucTransCode()
        {
            InitializeComponent();
            lblMSG.Visible = false;
            lblCountDown.Visible = false;
            t = new Timer();
            t.Interval = 500;
            t.Tick += new EventHandler(Timer1_Tick);
            t.Enabled = false;

        }
        public string ShippingID { set; get; }
        public void ClearAll()
        {
            lblMSG.Visible = false;
            textBox1.Text = "";
            lblCountDown.Visible = false;
        }
        private void btnInputDone_Click(object sender, EventArgs e)
        {
            ValidateTransCode(textBox1.Text);
        }

        public void SetTextboxFocus()
        {
            textBox1.Focus();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                ValidateTransCode(textBox1.Text);
            }
        }

        private void Timer1_Tick(object Sender, EventArgs e)
        {
            int elapsedSeconds = (int)(DateTime.Now - startTime).TotalSeconds;
            int remainingSeconds = secondsToWait - elapsedSeconds;

            if (remainingSeconds <= 0)
            {
                t.Stop();
                DoneDelegate(this, e);
            }
            lblCountDown.Text = "will back in " + remainingSeconds.ToString() + " seconds";
        }

        private void ValidateTransCode(string code)
        {
            ////validation
            //Server.Service sv = new Server.Service();
            //if (code == ShippingID && sv.UpdateStatus(Order_id, code))
            //{
            //    lblMSG.Text = "Correct";
            //    lblCountDown.Visible = true;
            //    t.Start();
            //    startTime = DateTime.Now;

            //}
            //else
            //{
            //    lblMSG.Text = "Incorrect";
            //}

            //lblMSG.Visible = true;
        }
    }
}
