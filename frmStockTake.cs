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
    public partial class frmStockTake : ChildForm
    {
        public frmStockTake()
        {
            InitializeComponent();
            ucStockTakeProcess1.Visible = false;
            ucStockTakeRpt1.Visible = false;
            ucStockTakeList1.Visible = true;
            ucStockTakeList1.ResetNewGrid();
            ucStockTakeList1.NewStockTakeDelegate += ucStockTakeList1_NewStockTakeDelegate;
            ucStockTakeList1.OldStockTakeDelegate += ucStockTakeList1_OldStockTakeDelegate;
        }

        private void ucStockTakeList1_OldStockTakeDelegate(object sender, EventArgs e)
        {
            ucStockTakeList1.Visible = false;
            ucStockTakeRpt1.Visible = true;
            ucStockTakeProcess1.Visible = false;
            ucStockTakeRpt1.LoadOldStocktake(ucStockTakeList1.StockTakeDt);
        }

        private void ucStockTakeList1_NewStockTakeDelegate(object sender, EventArgs e)
        {
            ucStockTakeList1.Visible = false;
            ucStockTakeRpt1.Visible = false;
            ucStockTakeProcess1.Visible = true;
            ucStockTakeProcess1.RefreshAll();
        }
    }
}
