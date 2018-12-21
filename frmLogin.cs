using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUSHA.form
{
    public partial class frmLogin : ChildForm
    {
        public event EventHandler EnableDelegate;
        int tries = 0;
        Server.Service sv;
        public frmLogin()
        {
            InitializeComponent();
            sv = new Server.Service();

            RefreshAll();
        }
        
        private void RefreshAll()
        {
            txtPasswd.Text = string.Empty;
            txtAc.Text = string.Empty;
            chkRob.Checked = false;
        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter && !string.IsNullOrEmpty(txtPasswd.Text))
            {
                CheckPasswd();
            }
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            CheckPasswd();
        }
        private void CheckPasswd()
        {

            if (!chkRob.Checked)
            {
                RefreshAll();
                MessageBox.Show(this, "請勾 \"" + chkRob.Text + "\" \n", "失敗");
                return;
            }
            if (!sv.CheckPassword(txtAc.Text, txtPasswd.Text, out string msg))
            {
                tries += 1;
                MessageBox.Show(this, msg, "失敗");
                if (tries % 3 == 0) { MessageBox.Show(this, "已經輸入3次失敗, 獲得另外3次機會!!!", "恭喜中獎"); }
                RefreshAll();
            }
            else
            {
                EnableDelegate(this, new EventArgs());
                this.Close();
            }
        }
        
    }
}
