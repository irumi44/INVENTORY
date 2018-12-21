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
    public partial class frmRecieve : ChildForm
    {
        public frmRecieve()
        {
            InitializeComponent();
            ucReceiveProcess1.Visible = false;
            ucReceiveRpt1.Visible = false;
            ucReceiveList1.Visible = true;
            ucReceiveList1.ResetNewGrid();
            ucReceiveList1.NewReceiveDelegate += UcReceiveList1_NewReceiveDelegate;
            ucReceiveList1.OldReceiveDelegate += UcReceiveList1_OldReceiveDelegate;
        }

        private void UcReceiveList1_OldReceiveDelegate(object sender, EventArgs e)
        {
            ucReceiveList1.Visible = false;
            ucReceiveRpt1.Visible = true;
            ucReceiveProcess1.Visible = false;
            ucReceiveRpt1.LoadOldReceive(ucReceiveList1.ReceiveDt);
        }

        private void UcReceiveList1_NewReceiveDelegate(object sender, EventArgs e)
        {
            ucReceiveList1.Visible = false;
            ucReceiveRpt1.Visible = false;
            ucReceiveProcess1.Visible = true;
            ucReceiveProcess1.RefreshAll();
        }
    }
}
