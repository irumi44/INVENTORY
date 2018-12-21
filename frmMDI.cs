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
    public partial class frmMDI : Form
    {
        frmInventory frmInv = null;
        frmImportProduct frmImpProduct = null;
        frmImportOrder frmImpOrder = null;
        frmReturn frmRetF = null;
        frmReturn frmRetC = null;
        frmItemLocationList frmItemList = null;
        frmCancelOrder frmCancel = null;
        frmRecieve frmReceiv = null;
        frmQueryItem frmQuery = null;
        frmQueryOrder frmQueryOrd = null;
        frmStockTake frmStockTak = null;
        frmAddItem frmAddIt = null;
        frmRptInOut frmInOut = null;
        frmChangeItemId frmChgId = null;
        frmVendor frmVend = null;
        frmReturnVendor frmRetV = null;

        public frmMDI()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            Server.Service sv = new Server.Service();
            cls.clsCache.SetShippingSet(sv.GetSysCodeset(CommonResources.clsConstants.VALUES.SYS_CODESET.SHIPPING_METHOD.CODE_TYPE));
            cls.clsCache.SetStatusSet(sv.GetSysCodeset(CommonResources.clsConstants.VALUES.SYS_CODESET.ORDER_STATUS.CODE_TYPE));
            cls.clsCache.SetItemStatusSet(sv.GetSysCodeset(CommonResources.clsConstants.VALUES.SYS_CODESET.ITEM_STATUS.CODE_TYPE));
            cls.clsCache.SetNumCompareSet(sv.GetSysCodeset(CommonResources.clsConstants.VALUES.SYS_CODESET.NUM_COMPARE.CODE_TYPE));
            
        }

        private void tBtnInventory_Click(object sender, EventArgs e)
        {
            if (!cls.clsCache.IS_OPENING(frmInv))
            {
                frmInv = new frmInventory();
                frmInv.MdiParent = this;
                cls.clsCache.SetOpen(frmInv);
            }
            frmInv.WindowState = FormWindowState.Maximized;
            frmInv.Show();
        }

        private void frmMDI_Load(object sender, EventArgs e)
        {
            lock_buttons(true);
            frmLogin f = new frmLogin();
            f.MdiParent = this;
            f.WindowState = FormWindowState.Normal;
            f.StartPosition = FormStartPosition.CenterScreen;
            f.EnableDelegate += F_EnableDelegate;
            f.Show();
            
        }

        private void F_EnableDelegate(object sender, EventArgs e)
        {
            lock_buttons(false);
        }

        private void lock_buttons(bool k)
        {
            foreach (ToolStripDropDownButton c in toolStrip1.Items)
            {
                c.Enabled = !k;
            }
        }

        private void tBtnItemList_Click(object sender, EventArgs e)
        {
            if (!cls.clsCache.IS_OPENING(frmItemList))
            {
                frmItemList = new frmItemLocationList();
                frmItemList.MdiParent = this;
                cls.clsCache.SetOpen(frmItemList);
            }
            frmItemList.WindowState = FormWindowState.Maximized;
            frmItemList.Show();
        }

        private void tBtnCancel_Click(object sender, EventArgs e)
        {
            if (!cls.clsCache.IS_OPENING(frmCancel))
            {
                frmCancel = new frmCancelOrder();
                frmCancel.MdiParent = this;
                cls.clsCache.SetOpen(frmCancel);
            }
            frmCancel.WindowState = FormWindowState.Maximized;
            frmCancel.Show();
        }

        private void tBtnInvIn_Click(object sender, EventArgs e)
        {
            if (!cls.clsCache.IS_OPENING(frmReceiv))
            {
                frmReceiv = new frmRecieve();
                frmReceiv.MdiParent = this;
                cls.clsCache.SetOpen(frmReceiv);
            }
            frmReceiv.WindowState = FormWindowState.Maximized;
            frmReceiv.Show();
        }

        private void tBtnInvQuery_Click(object sender, EventArgs e)
        {
            if (!cls.clsCache.IS_OPENING(frmQuery))
            {
                frmQuery = new frmQueryItem();
                frmQuery.MdiParent = this;
                cls.clsCache.SetOpen(frmQuery);
            }
            frmQuery.WindowState = FormWindowState.Maximized;
            frmQuery.Show();
        }
        
        private void tBtnStockTake_Click(object sender, EventArgs e)
        {
            if (!cls.clsCache.IS_OPENING(frmStockTak))
            {
                frmStockTak = new frmStockTake();
                frmStockTak.MdiParent = this;
                cls.clsCache.SetOpen(frmStockTak);
            }
            frmStockTak.WindowState = FormWindowState.Maximized;
            frmStockTak.Show();
        }

        private void tBtnAddItem_Click(object sender, EventArgs e)
        {
            if (!cls.clsCache.IS_OPENING(frmAddIt))
            {
                frmAddIt = new frmAddItem();
                frmAddIt.MdiParent = this;
                cls.clsCache.SetOpen(frmAddIt);
            }
            frmAddIt.WindowState = FormWindowState.Maximized;
            frmAddIt.Show();
        }

        private void tBtnImp_Click(object sender, EventArgs e)
        {
            if (!cls.clsCache.IS_OPENING(frmImpProduct))
            {
                frmImpProduct = new frmImportProduct();
                frmImpProduct.MdiParent = this;
                cls.clsCache.SetOpen(frmImpProduct);
            }
            frmImpProduct.SetDefault();
            frmImpProduct.WindowState = FormWindowState.Maximized;
            frmImpProduct.Show();
        }

        private void tBtnInOut_Click(object sender, EventArgs e)
        {
            if (!cls.clsCache.IS_OPENING(frmInOut))
            {
                frmInOut = new frmRptInOut();
                frmInOut.MdiParent = this;
                cls.clsCache.SetOpen(frmInOut);
            }
            frmInOut.WindowState = FormWindowState.Maximized;
            frmInOut.Show();
        }

        private void tBtnImportOrder_Click(object sender, EventArgs e)
        {
            if (!cls.clsCache.IS_OPENING(frmImpOrder))
            {
                frmImpOrder = new frmImportOrder();
                frmImpOrder.MdiParent = this;
                cls.clsCache.SetOpen(frmImpOrder);
            }
            frmImpOrder.SetDefault();
            frmImpOrder.WindowState = FormWindowState.Maximized;
            frmImpOrder.Show();
        }

        private void tBtnOrderQuery_Click(object sender, EventArgs e)
        {
            if (!cls.clsCache.IS_OPENING(frmQueryOrd))
            {
                frmQueryOrd = new frmQueryOrder();
                frmQueryOrd.MdiParent = this;
                cls.clsCache.SetOpen(frmQueryOrd);
            }
            frmQueryOrd.WindowState = FormWindowState.Maximized;
            frmQueryOrd.Show();
        }

        private void tBtnModifyItem_Click(object sender, EventArgs e)
        {
            if (!cls.clsCache.IS_OPENING(frmQuery))
            {
                frmQuery = new frmQueryItem();
                frmQuery.MdiParent = this;
                cls.clsCache.SetOpen(frmQuery);
            }
            frmQuery.WindowState = FormWindowState.Maximized;
            frmQuery.Show();
        }

        private void tBtnChangeItemId_Click(object sender, EventArgs e)
        {
            if (!cls.clsCache.IS_OPENING(frmChgId))
            {
                frmChgId = new frmChangeItemId();
                frmChgId.MdiParent = this;
                cls.clsCache.SetOpen(frmChgId);
            }
            frmChgId.WindowState = FormWindowState.Maximized;
            frmChgId.Show();
        }

        private void tBtnReturnVender_Click(object sender, EventArgs e)
        {

            if (!cls.clsCache.IS_OPENING(frmRetV))
            {
                frmRetV = new frmReturnVendor();
                frmRetV.MdiParent = this;
                cls.clsCache.SetOpen(frmRetV);
            }
            frmRetV.WindowState = FormWindowState.Maximized;
            frmRetV.Show();
        }

        private void tBtnFullRet_Click(object sender, EventArgs e)
        {
            if (!cls.clsCache.IS_OPENING(frmRetF))
            {
                frmRetF = new frmReturn(true);
                frmRetF.Name = frmRetF.Name + "F";
                frmRetF.MdiParent = this;
                cls.clsCache.SetOpen(frmRetF);
            }
            frmRetF.WindowState = FormWindowState.Maximized;
            frmRetF.Show();
        }

        private void tBtnCustRet_Click(object sender, EventArgs e)
        {
            if (!cls.clsCache.IS_OPENING(frmRetC))
            {
                frmRetC = new frmReturn(false);
                frmRetC.Name = frmRetC.Name + "C";
                frmRetC.MdiParent = this;
                cls.clsCache.SetOpen(frmRetC);
            }
            frmRetC.WindowState = FormWindowState.Maximized;
            frmRetC.Show();
        }

        private void tBtnVendor_Click(object sender, EventArgs e)
        {
            if (!cls.clsCache.IS_OPENING(frmVend))
            {
                frmVend = new frmVendor();
                frmVend.MdiParent = this;
                cls.clsCache.SetOpen(frmVend);
            }
            frmVend.WindowState = FormWindowState.Maximized;
            frmVend.Show();
        }
    }
}
