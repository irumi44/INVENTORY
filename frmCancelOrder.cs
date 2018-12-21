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
    public partial class frmCancelOrder : ChildForm
    {
        Server.Service sv;
        public frmCancelOrder()
        {
            InitializeComponent();
            sv = new Server.Service();

            RefreshAll();
        }
        
        private void RefreshAll()
        {
            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
            panelDown.Visible = true;
            panelUp.Visible = false;
            chkRob.Checked = false;
        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
        //    if (e.KeyChar == (char)Keys.Enter && !string.IsNullOrEmpty(textBox1.Text))
        //    {
        //        CheckPasswd();
        //    }
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
        //    CheckPasswd();
        }
        //private void CheckPasswd()
        //{
            
        //    if (!chkRob.Checked)
        //    {
        //        RefreshAll();
        //        MessageBox.Show(this, "Please click on \"I'm NOT robot\" \n", "失敗");
        //        return;
        //    }
        //    if (!sv.CheckPassword(textBox1.Text, out string msg))
        //    {
        //        MessageBox.Show(this, msg, "失敗");
        //        RefreshAll();
        //    }
        //    else
        //    {
        //        RefreshAll();
        //        panelUp.Visible = false;
        //        panelDown.Visible = true;
        //        textBox2.Focus();
        //    }
        //}

        private void btnOK_Click(object sender, EventArgs e)
        {
            CancelThis();
        }
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter && !string.IsNullOrEmpty(textBox2.Text))
            {
                CancelThis();
            }
        }        
        private void CancelThis()
        {
            if (!sv.CancelOrderAddBack(textBox2.Text, out string msg))
            {
                MessageBox.Show(this, msg, "失敗");
                textBox2.Focus();
            }
            else
            {
                RefreshAll();
                //panelUp.Visible = true;
                //panelDown.Visible = false;
                MessageBox.Show(this, "完成取消", "成功");
            }
        }
    }
}
