using CommonResources;
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
    public partial class frmVendor : ChildForm
    {
        private bool IsAdd = false;
        private string code;
        public frmVendor()
        {
            InitializeComponent();

            Server.Service sv = new Server.Service();
            DataTable dtVendor = sv.FindVendors();
            Dictionary<string, string> vendorSet = new Dictionary<string, string>();
            foreach (DataRow dr in dtVendor.Rows)
            {
                vendorSet.Add(dr[clsConstants.Tables.VENDOR_INFO.VENDOR_CODE].ToString(),
                                  dr[clsConstants.Tables.VENDOR_INFO.VENDOR_NAME].ToString());
            }
            cls.clsCache.VendorSet = vendorSet;


            cboVendor.DisplayMember = clsConstants.VALUES.VALUE;
            cboVendor.ValueMember = clsConstants.VALUES.KEY;
            cboVendor.DataSource = new BindingSource(cls.clsCache.VendorSet, null);
            cboVendor.SelectedIndex = -1;

            ClearAll();
            view_mode(true);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Length == 0)
            {
                MessageBox.Show(this, "名子咧?", "失敗");
                return;
            }
            DataTable dtVendor = clsConstants.Tables.VENDOR_INFO.BuildTable();
            DataRow drNew = dtVendor.NewRow();
            drNew[clsConstants.Tables.VENDOR_INFO.VENDOR_CODE] = IsAdd ? Guid.NewGuid().ToString(): code;
            drNew[clsConstants.Tables.VENDOR_INFO.VENDOR_NAME] = txtName.Text;
            drNew[clsConstants.Tables.VENDOR_INFO.TEL] = txtTel.Text;
            drNew[clsConstants.Tables.VENDOR_INFO.ADDRESS] = txtAddr.Text;
            drNew[clsConstants.Tables.VENDOR_INFO.URL] = txtUrl.Text;
            drNew[clsConstants.Tables.VENDOR_INFO.CONTACT_NAME] = txtContact.Text;
            dtVendor.Rows.Add(drNew);
            Server.Service sv = new Server.Service();
            if (!sv.UpdateVendor(IsAdd, dtVendor, out string msg))
            {
                MessageBox.Show(this, msg, "失敗");
                ClearAll();
            }
            else
            {
                MessageBox.Show(this, "完成", "成功");
                DataTable dt = sv.FindVendors();
                Dictionary<string, string> vendorSet = new Dictionary<string, string>();
                foreach (DataRow dr in dt.Rows)
                {
                    vendorSet.Add(dr[clsConstants.Tables.VENDOR_INFO.VENDOR_CODE].ToString(),
                                      dr[clsConstants.Tables.VENDOR_INFO.VENDOR_NAME].ToString());
                }
                cls.clsCache.VendorSet = vendorSet;


                cboVendor.DisplayMember = clsConstants.VALUES.VALUE;
                cboVendor.ValueMember = clsConstants.VALUES.KEY;
                cboVendor.DataSource = new BindingSource(cls.clsCache.VendorSet, null);
                cboVendor.SelectedIndex = -1;
            }
            view_mode(true);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearAll();
            view_mode(true);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            IsAdd = true;
            ClearAll();
            view_mode(false);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            IsAdd = false;
            view_mode(false);
        }

        private void ClearAll()
        {
            txtName.Text = string.Empty;
            txtTel.Text = string.Empty;
            txtUrl.Text = string.Empty;
            txtAddr.Text = string.Empty;
            txtContact.Text = string.Empty;
        }
        private void view_mode(bool is_view)
        {
            btnOK.Enabled = !is_view;
            btnCancel.Enabled = !is_view;
            btnAdd.Enabled = is_view;
            btnEdit.Enabled = is_view;
            cboVendor.Enabled = is_view;
            txtName.Enabled = !is_view;
            txtTel.Enabled = !is_view;
            txtUrl.Enabled = !is_view;
            txtAddr.Enabled = !is_view;
            txtContact.Enabled = !is_view;
        }

        private void cboVendor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboVendor.SelectedValue == null) return;
            code = cboVendor.SelectedValue.ToString();
            Server.Service sv = new Server.Service();
            DataTable dtVendor = sv.GetVendor(code);
            if (dtVendor != null && dtVendor.Rows.Count > 0)
            {
                txtName.Text = dtVendor.Rows[0][clsConstants.Tables.VENDOR_INFO.VENDOR_NAME].ToString();
                txtTel.Text = dtVendor.Rows[0][clsConstants.Tables.VENDOR_INFO.TEL].ToString();
                txtUrl.Text = dtVendor.Rows[0][clsConstants.Tables.VENDOR_INFO.URL].ToString();
                txtAddr.Text = dtVendor.Rows[0][clsConstants.Tables.VENDOR_INFO.ADDRESS].ToString();
                txtContact.Text = dtVendor.Rows[0][clsConstants.Tables.VENDOR_INFO.CONTACT_NAME].ToString();
            }
        }
    }
}

