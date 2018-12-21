using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonResources;

namespace GUSHA.form.user_control
{
    public partial class ucItemInfo : UserControl
    {
        public ucItemInfo()
        {
            InitializeComponent();
            cboStatus.DataSource = new BindingSource(cls.clsCache.ItemStatusSet, null);
            cboStatus.DisplayMember = clsConstants.VALUES.VALUE;
            cboStatus.ValueMember = clsConstants.VALUES.KEY;
            cboStatus.SelectedValue = clsConstants.VALUES.SYS_CODESET.ITEM_STATUS.ACTIVE;
            cboStatus.Enabled = false;
            txtinv.Text = "0";
            txtBarcode.SelectAll();
            txtBarcode.Focus();
        }

        private void LockForEditMode(bool IsEdit)
        {
            txtinv.Enabled = !IsEdit;
            txtId.Enabled = !IsEdit;

            txtBarcode.Enabled = true;
            txtName.Enabled = true;
            txtLocation.Enabled = true;
            txtMassLocation.Enabled = true;
            cboStatus.Enabled = true;
        }

        public void LoadRecord(string iid = null)
        {
            if (iid == null)
            {
                LockForEditMode(false);
                ClearFields();
                txtBarcode.SelectAll();
                txtBarcode.Focus();
            }
            else
            {
                Server.Service sv = new Server.Service();
                DataTable dt = sv.FindItemById(iid);
                if (dt != null && dt.Rows.Count > 0)
                {
                    FillInValues(dt.Rows[0]);
                }
                
                LockForEditMode(true);
            }
        }

        private void FillInValues(DataRow dr)
        {
            txtBarcode.Text = dr[clsConstants.Tables.ITEM_INFO.BARCODE] == null ? string.Empty: dr[clsConstants.Tables.ITEM_INFO.BARCODE].ToString();
            txtId.Text = dr[clsConstants.Tables.ITEM_INFO.ITEM_ID] == null ? string.Empty : dr[clsConstants.Tables.ITEM_INFO.ITEM_ID].ToString();
            txtName.Text = dr[clsConstants.Tables.ITEM_INFO.ITEM_NAME] == null ? string.Empty : dr[clsConstants.Tables.ITEM_INFO.ITEM_NAME].ToString();
            txtLocation.Text = dr[clsConstants.Tables.ITEM_INFO.ITEM_LOCATION] == null ? string.Empty : dr[clsConstants.Tables.ITEM_INFO.ITEM_LOCATION].ToString();
            txtMassLocation.Text = dr[clsConstants.Tables.ITEM_INFO.ITEM_MASS_LOCATION] == null ? string.Empty : dr[clsConstants.Tables.ITEM_INFO.ITEM_MASS_LOCATION].ToString();
            txtinv.Text = dr[clsConstants.Tables.ITEM_INFO.INV] == null ? string.Empty : dr[clsConstants.Tables.ITEM_INFO.INV].ToString();
            cboStatus.SelectedText = dr[clsConstants.Tables.ITEM_INFO.STATUS] == null ? string.Empty : dr[clsConstants.Tables.ITEM_INFO.STATUS].ToString();
        }

        private void ClearFields()
        {
            txtBarcode.Clear();
            txtId.Clear();
            txtName.Clear();
            txtLocation.Clear();
            txtMassLocation.Clear();
            txtinv.Clear();
            txtinv.Text = "0";
            cboStatus.SelectedValue = clsConstants.VALUES.SYS_CODESET.ITEM_STATUS.ACTIVE;
        }

        private bool IsDataValid(out string msg)
        {
            msg = string.Empty;
            if (string.IsNullOrEmpty(txtBarcode.Text))
            {
                msg = "請輸入Barcode";
                txtBarcode.SelectAll();
                txtBarcode.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txtId.Text))
            {
                msg = "請輸入商品編號";
                txtId.SelectAll();
                txtId.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txtLocation.Text))
            {
                msg = "請輸入商品位置";
                txtLocation.SelectAll();
                txtLocation.Focus();
                return false;
            }
            if (txtLocation.Text.Length != 6 || !int.TryParse(txtLocation.Text, out int a))
            {
                msg = "商品位置目前只支援6位數字";
                txtLocation.SelectAll();
                txtLocation.Focus();
                return false;
            }
            if (txtinv.Text.ToString().Length > 0 && !int.TryParse(txtinv.Text, out int b))
            {
                msg = "庫存只支援整數";
                txtinv.SelectAll();
                txtinv.Focus();
                return false;
            }

            return true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!IsDataValid(out string msg))
            {
                MessageBox.Show(this, msg, "失敗");
                return;
            }
            Server.Service sv = new Server.Service();
            DataTable dtItem = clsConstants.Tables.ITEM_INFO.BuildTable();
            DataRow drNew = dtItem.NewRow();
            drNew[clsConstants.Tables.ITEM_INFO.BARCODE] = txtBarcode.Text.Replace("'", "''");
            drNew[clsConstants.Tables.ITEM_INFO.ITEM_ID] = txtId.Text.Replace("'", "''");
            drNew[clsConstants.Tables.ITEM_INFO.ITEM_NAME] = txtName.Text.Replace("'", "''");
            drNew[clsConstants.Tables.ITEM_INFO.ITEM_LOCATION] = txtLocation.Text.Replace("'", "''");
            drNew[clsConstants.Tables.ITEM_INFO.ITEM_MASS_LOCATION] = txtMassLocation.Text.Replace("'", "''");
            drNew[clsConstants.Tables.ITEM_INFO.INV] = txtinv.Text.Replace("'", "''");
            drNew[clsConstants.Tables.ITEM_INFO.STATUS] = cboStatus.SelectedValue == null ? clsConstants.VALUES.SYS_CODESET.ITEM_STATUS.ACTIVE : cboStatus.SelectedValue.ToString();
            dtItem.Rows.Add(drNew);

            if (!sv.AddItem(txtId.Enabled, dtItem, out string msgServer))
            {
                MessageBox.Show(this, msgServer, "失敗");
            }
            else
            {
                MessageBox.Show(this, "成功", "成功");
                ClearFields();
            }

            txtBarcode.SelectAll();
            txtBarcode.Focus();
        }

    }
}
