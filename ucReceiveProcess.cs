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
    public partial class ucReceiveProcess : UserControl
    {
        public ucReceiveProcess()
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

            DataTable dtInItem = new DataTable("in_item");
            dtInItem.Columns.Add(new DataColumn(clsConstants.Tables.ITEM_INFO.ITEM_ID, typeof(string)));
            dtInItem.Columns.Add(new DataColumn(clsConstants.Tables.ITEM_INFO.ITEM_NAME, typeof(string)));
            dtInItem.Columns.Add(new DataColumn(clsConstants.Tables.ITEM_INFO.BARCODE, typeof(string)));
            dtInItem.Columns.Add(new DataColumn(clsConstants.Tables.ITEM_INFO.ITEM_LOCATION, typeof(string)));
            dtInItem.Columns.Add(new DataColumn("current_quant", typeof(string)));
            dataGridView1.DataSource = dtInItem;

            dataGridView1.AllowUserToAddRows = false;
            foreach (DataGridViewColumn c in dataGridView1.Columns)
            {
                c.ReadOnly = true;
                c.CellTemplate.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                c.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            dataGridView1.Columns["current_quant"].ReadOnly = false;

            dataGridView1.Columns[clsConstants.Tables.ITEM_INFO.ITEM_NAME].Width = 450;
            dataGridView1.Columns[clsConstants.Tables.ITEM_INFO.ITEM_LOCATION].Width = 450;

            dataGridView1.RowTemplate.Height = 40;
            dataGridView1.Columns[clsConstants.Tables.ITEM_INFO.ITEM_LOCATION].DefaultCellStyle.Font = new Font("Times New Roman", 32, FontStyle.Bold);

            dataGridView1.Columns[clsConstants.Tables.ITEM_INFO.ITEM_ID].HeaderCell.Value = "商品編號";
            dataGridView1.Columns[clsConstants.Tables.ITEM_INFO.ITEM_NAME].HeaderCell.Value = "商品名稱";
            dataGridView1.Columns[clsConstants.Tables.ITEM_INFO.BARCODE].HeaderCell.Value = "barcode";
            dataGridView1.Columns[clsConstants.Tables.ITEM_INFO.ITEM_LOCATION].HeaderCell.Value = "商品位置";
            dataGridView1.Columns["current_quant"].HeaderCell.Value = "進倉總數";

            textBox1.SelectAll();
            textBox1.Focus();
        }

        public void RefreshAll()
        {
            ((DataTable)dataGridView1.DataSource).Clear();
            textBox1.Text = string.Empty;
            textBox1.Focus();
        }
        private void find_item()
        {
            string barcode = textBox1.Text;
            if (string.IsNullOrEmpty(barcode)) return;
            DataTable dt = (DataTable)dataGridView1.DataSource;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["barcode"].ToString() == barcode && int.TryParse(dr["current_quant"].ToString(), out int i))
                {
                    dr["current_quant"] = (i + 1).ToString();
                    return;
                }
            }
            Server.Service sv = new Server.Service();
            DataTable dtItem = sv.FindItem(barcode);
            if (dtItem.Rows.Count > 0)
            {
                DataRow drNew = dt.NewRow();
                drNew[clsConstants.Tables.ITEM_INFO.ITEM_ID] = dtItem.Rows[0][clsConstants.Tables.ITEM_INFO.ITEM_ID];
                drNew[clsConstants.Tables.ITEM_INFO.ITEM_NAME] = dtItem.Rows[0][clsConstants.Tables.ITEM_INFO.ITEM_NAME];
                drNew[clsConstants.Tables.ITEM_INFO.BARCODE] = dtItem.Rows[0][clsConstants.Tables.ITEM_INFO.BARCODE];
                drNew["current_quant"] = dtItem.Rows[0]["current_quant"];
                drNew[clsConstants.Tables.ITEM_INFO.ITEM_LOCATION] = dtItem.Rows[0][clsConstants.Tables.ITEM_INFO.ITEM_LOCATION];
                dt.Rows.Add(drNew);
            }
            else
            {
                MessageBox.Show(this, "資料庫中找不到這個商品, 請直接匯入", "錯誤");
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                find_item();
                textBox1.SelectAll();
                textBox1.Focus();
            }
        }
        private void ItemsIn()
        {
            try
            {
                Server.Service sv = new Server.Service();
                if (sv.InventoryAdd((DataTable)dataGridView1.DataSource, txtRcvNo.Text, txtRemarks.Text, 
                    cboVendor.SelectedValue == null ? string.Empty : cboVendor.SelectedValue.ToString(), out string msg))
                {
                    MessageBox.Show(this, "進貨完成", "成功");
                    DataTable dt = (DataTable)dataGridView1.DataSource;
                    dt.Clear();
                    textBox1.Text = string.Empty;
                    txtRcvNo.Text = string.Empty;
                    txtRemarks.Text = string.Empty;
                    cboVendor.SelectedIndex = -1;
                }
                else
                {
                    throw new Exception(msg);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
        }

        private void btnOK_Click_1(object sender, EventArgs e)
        {
            find_item();
            textBox1.SelectAll();
            textBox1.Focus();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            txtRcvNo.Text = txtRcvNo.Text.Replace("#", "");
            ItemsIn();
        }
    }
}
