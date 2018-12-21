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
    public partial class frmChangeItemId : ChildForm
    {
        public frmChangeItemId()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string sOldId = txtId.Text;
            string sNewId = txtNewId.Text;

            Server.Service sv = new Server.Service();
            if (!sv.UpdateItemId(sOldId, sNewId, out string msg))
            {
                MessageBox.Show(this, msg, "失敗");
            }
            else
            {
                MessageBox.Show(this, "完成", "成功");
            }

        }
    }
}
