using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUSHA.form.user_control
{
    public partial class ucStockTakeProcess : UserControl
    {
        public ucStockTakeProcess()
        {
            InitializeComponent();

            DataTable dtInItem = new DataTable("stocktake_item");
            dtInItem.Columns.Add(new DataColumn(CommonResources.clsConstants.Tables.ITEM_INFO.ITEM_ID, typeof(string)));
            dtInItem.Columns.Add(new DataColumn(CommonResources.clsConstants.Tables.ITEM_INFO.ITEM_NAME, typeof(string)));
            dtInItem.Columns.Add(new DataColumn(CommonResources.clsConstants.Tables.ITEM_INFO.BARCODE, typeof(string)));
            dtInItem.Columns.Add(new DataColumn(CommonResources.clsConstants.Tables.ITEM_INFO.ITEM_LOCATION, typeof(string)));
            dtInItem.Columns.Add(new DataColumn(CommonResources.clsConstants.Tables.ITEM_INFO.INV, typeof(string)));
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

            dataGridView1.Columns[CommonResources.clsConstants.Tables.ITEM_INFO.ITEM_NAME].Width = 450;
            dataGridView1.Columns[CommonResources.clsConstants.Tables.ITEM_INFO.ITEM_LOCATION].Width = 450;

            dataGridView1.RowTemplate.Height = 40;
            dataGridView1.Columns[CommonResources.clsConstants.Tables.ITEM_INFO.ITEM_LOCATION].DefaultCellStyle.Font = new Font("Times New Roman", 32, FontStyle.Bold);

            dataGridView1.Columns[CommonResources.clsConstants.Tables.ITEM_INFO.ITEM_ID].HeaderCell.Value = "商品編號";
            dataGridView1.Columns[CommonResources.clsConstants.Tables.ITEM_INFO.ITEM_NAME].HeaderCell.Value = "商品名稱";
            dataGridView1.Columns[CommonResources.clsConstants.Tables.ITEM_INFO.BARCODE].HeaderCell.Value = "barcode";
            dataGridView1.Columns[CommonResources.clsConstants.Tables.ITEM_INFO.ITEM_LOCATION].HeaderCell.Value = "商品位置";
            dataGridView1.Columns[CommonResources.clsConstants.Tables.ITEM_INFO.INV].HeaderCell.Value = "盤點前數量";
            dataGridView1.Columns["current_quant"].HeaderCell.Value = "盤點後數量";

            txtBarcode.SelectAll();
            txtBarcode.Focus();
        }

        public void RefreshAll()
        {
            ((DataTable)dataGridView1.DataSource).Clear();
            txtBarcode.Text = string.Empty;
            txtBarcode.Focus();
        }
        private void find_item()
        {
            string barcode = txtBarcode.Text;
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
                drNew[CommonResources.clsConstants.Tables.ITEM_INFO.ITEM_ID] = dtItem.Rows[0][CommonResources.clsConstants.Tables.ITEM_INFO.ITEM_ID];
                drNew[CommonResources.clsConstants.Tables.ITEM_INFO.ITEM_NAME] = dtItem.Rows[0][CommonResources.clsConstants.Tables.ITEM_INFO.ITEM_NAME];
                drNew[CommonResources.clsConstants.Tables.ITEM_INFO.BARCODE] = dtItem.Rows[0][CommonResources.clsConstants.Tables.ITEM_INFO.BARCODE];
                drNew["current_quant"] = dtItem.Rows[0]["current_quant"];
                drNew[CommonResources.clsConstants.Tables.ITEM_INFO.ITEM_LOCATION] = dtItem.Rows[0][CommonResources.clsConstants.Tables.ITEM_INFO.ITEM_LOCATION];
                drNew[CommonResources.clsConstants.Tables.ITEM_INFO.INV] = dtItem.Rows[0][CommonResources.clsConstants.Tables.ITEM_INFO.INV];
                dt.Rows.Add(drNew);
            }
            else
            {
                MessageBox.Show(this, "資料庫中找不到這個商品, 請直接匯入", "錯誤");
            }
        }

        private void txtBarcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                find_item();
                txtBarcode.SelectAll();
                txtBarcode.Focus();
            }
        }
        private void ItemsIn()
        {
            try
            {
                Server.Service sv = new Server.Service();
                if (sv.StockTake((DataTable)dataGridView1.DataSource, out string msg))
                {
                    MessageBox.Show(this, "盤點完成" + msg, "成功");
                    DataTable dt = (DataTable)dataGridView1.DataSource;
                    dt.Clear();
                    txtBarcode.Text = string.Empty;
                }
                else
                {
                    throw new Exception(msg);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "盤點失敗, " + ex.Message, "失敗");
            }
            finally
            {

            }
        }

        private void buttonComplete_Click(object sender, EventArgs e)
        {
            ItemsIn();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            find_item();
            txtBarcode.SelectAll();
            txtBarcode.Focus();
        }
    }
}
