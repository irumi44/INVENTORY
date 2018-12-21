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
    public partial class frmModifyItem : ChildForm
    {
        public frmModifyItem()
        {
            InitializeComponent();
        }



        public void LoadRecord(string iid)
        {
            this.ucItemInfo1.LoadRecord(iid);
        }
    }
}
