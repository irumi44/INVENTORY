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
    public partial class frmItemLocationList : ChildForm
    {
        public frmItemLocationList()
        {
            InitializeComponent();
            ucPickList1.Visible = true;
            ucPickRpt1.Visible = false;

            ucPickList1.NewPickDelegate += UcPickList1_NewPickDelegate;
            ucPickList1.OldPickDelegate += UcPickList1_OldPickDelegate;
            ucPickRpt1.NonePickDelegate += UcPickRpt1_NonePickDelegate;
        }

        private void UcPickRpt1_NonePickDelegate(object sender, EventArgs e)
        {
            ucPickList1.Visible = true;
            ucPickRpt1.Visible = false;
            MessageBox.Show(this, "沒有待撿貨的訂單", "恭喜");
        }

        private void UcPickList1_OldPickDelegate(object sender, EventArgs e)
        {

            ucPickList1.Visible = false;
            ucPickRpt1.Visible = true;
            ucPickRpt1.LoadOldPick(ucPickList1.PickDt);
        }

        private void UcPickList1_NewPickDelegate(object sender, EventArgs e)
        {

            ucPickList1.Visible = false;
            ucPickRpt1.Visible = true;
            ucPickRpt1.LoadNewPick();
        }
    }
}
