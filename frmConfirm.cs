using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.Reflection;

namespace GUSHA.form
{
    public partial class frmConfirm : Form
    {
        string lblString;
        string btnString;
        public DateTime startTime;
        private int secondsToWait = 3;
        public Timer t;
        bool ALARM;
        public frmConfirm(string lbl, string btn, bool alarm = false)
        {
            InitializeComponent();
            lblString = lbl;
            btnString = btn;
            lblDone.Text = lbl;
            btnOK.Text = btnString;
            t = new Timer();
            t.Interval = 500;
            t.Tick += new EventHandler(Timer1_Tick);
            t.Enabled = false;
            ALARM = alarm;
        }

        
        private void Timer1_Tick(object Sender, EventArgs e)
        {
            int elapsedSeconds = (int)(DateTime.Now - startTime).TotalSeconds;
            int remainingSeconds = secondsToWait - elapsedSeconds;

            if (remainingSeconds <= 0)
            {
                t.Stop();
                DialogResult = DialogResult.OK;
            }
            lblDone.Text = lblString + remainingSeconds.ToString() + " seconds";
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void FrmConfirm_Shown(object sender, System.EventArgs e)
        {
            Stream str;
            if (!ALARM)
            {
                str = File.OpenRead(@"success.wav");
            }
            else
            {
                str = File.OpenRead(@"err.wav");
            }
            SoundPlayer snd = new SoundPlayer(str);
            snd.Play();
        }
    }
}
