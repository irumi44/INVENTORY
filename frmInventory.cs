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
    public partial class frmInventory : ChildForm
    {
       
        public frmInventory()
        {
            InitializeComponent();
            show_input_order();
            ucInputOrder1.DoneDelegate += new EventHandler(ucInputOrder1_Done);
            ucOrderDetail1.CantFindDelegate += new EventHandler(ucInputOrder1_Cantfind);
            ucOrderDetail1.AlreadyDoneDelegate += new EventHandler(ucInputOrder1_AlreadyDoneDelegate);
            ucOrderDetail1.DoneDelegate += new EventHandler(ucOrderDetail1_Done);
            ucOrderDetail1.ErrorDelegate += new EventHandler(ucOrderDetail1_Error);
            ucTransCode1.DoneDelegate += new EventHandler(ucTransCode1_Done);
            ucOrderDetail1.SetShippingIDDelegate += new EventHandler(ucOrderDetail1_SetShippingIDDelegate);

        }

        private void show_input_order()
        {
            btnBK.Enabled = false;
            this.ucInputOrder1.Visible = true;
            this.ucInputOrder1.SetGridview();
            this.ucOrderDetail1.Visible = false;
            this.ucTransCode1.Visible = false;

            ucInputOrder1.SetTextboxFocus();
        }

        private void show_order_detail()
        {

            if (!ucOrderDetail1.SetSource(ucInputOrder1.GetOrder())) return;
            this.ucInputOrder1.Visible = false;
            btnBK.Enabled = true;
            this.ucOrderDetail1.Visible = true;
            ucOrderDetail1.SetTextboxFocus();
        }

        private void ClearAll()
        {
            ucInputOrder1.ClearAll();
            ucOrderDetail1.ClearAll();
            ucTransCode1.ClearAll();
        }
        private void ucInputOrder1_Done(object sender, System.EventArgs e)
        {
            show_order_detail();
        }

        private void ucInputOrder1_Cantfind(object sender, System.EventArgs e)
        {
            MessageBox.Show(this, "找不到這筆訂單");
                      
            show_input_order();
        }
        private void ucInputOrder1_AlreadyDoneDelegate(object sender, System.EventArgs e)
        {
            MessageBox.Show(this, "此訂單已出貨");

            show_input_order();
        }
        private void ucOrderDetail1_SetShippingIDDelegate(object sender, System.EventArgs e)
        {
            ucTransCode1.ShippingID = ucOrderDetail1.ShippingID;
            ucTransCode1.Order_id = ucOrderDetail1.OrderID;
        }

        protected void ucOrderDetail1_Done(object sender, EventArgs e)
        {
            Server.Service sv = new Server.Service();
            if (!sv.InventoryMinus(ucOrderDetail1.OrderID, out string msg))
            {
                MessageBox.Show(this, "Internal error, nothing changed", "失敗");
                ClearAll();
                show_input_order();
                return;
            }

            frmConfirm frmConfirm1 = new frmConfirm("Well done! \nwill go back in ", "Go back now");
            frmConfirm1.Shown += new EventHandler(frmConfirm1_Shown);
            if (frmConfirm1.ShowDialog() == DialogResult.OK)
            {
                ClearAll();
                show_input_order();
            }
            
        }

        protected void ucOrderDetail1_Error(object sender, EventArgs e)
        {
            frmConfirm frmConfirm1 = new frmConfirm("錯誤! 請重刷! \nwill go back in ", "Go back now", true);
            frmConfirm1.Shown += new EventHandler(frmConfirm1_Shown);
            if (frmConfirm1.ShowDialog() == DialogResult.OK)
            {
                ClearAll();
                show_input_order();
            }

        }
        private void frmConfirm1_Shown(object sender, EventArgs e)
        {
            frmConfirm frm = (frmConfirm)sender;
            frm.startTime = DateTime.Now;
            frm.t.Enabled = true;
        }
        protected void ucTransCode1_Done(object sender, EventArgs e)
        {
            ClearAll();
            btnBK.Enabled = false;
            this.ucInputOrder1.Visible = true;
            this.ucOrderDetail1.Visible = false;
            this.ucTransCode1.Visible = false;
            ucInputOrder1.SetTextboxFocus();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (ucOrderDetail1.Visible == true)
            {
                ClearAll();
                show_input_order();
            }
            else if (ucTransCode1.Visible == true)
            {
                show_order_detail();
            }
        }
    }
}
