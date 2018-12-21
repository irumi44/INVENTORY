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
    public partial class ucReturnProcess : UserControl
    {
        public bool IS_FULL { set; get; }
        string ORDER_ID;
        public ucReturnProcess()
        {
            InitializeComponent();
        }
        
        private void btnOK_Click_1(object sender, EventArgs e)
        {
            if (ORDER_ID == null || ORDER_ID != txtOrderNo.Text)
            {
                MessageBox.Show(this, "麻煩先查詢訂單 / 訂單號碼好像不對", "錯了啦");
                return;
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                DataTable dt = (DataTable)dataGridView1.DataSource;
                foreach (DataRow dr in dt.Rows)
                {
                    sb.Append(dr[clsConstants.Tables.ORDER_DETAIL.ITEM_ID] + " * " + dr["this_quant"] + "\n");
                }
                string msg = sb.ToString();
                if (msg.Length > 0) msg = msg.Substring(0, msg.Length - 1);
                if (MessageBox.Show(this, "確定這些?\n" + msg, "確定?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    return_process();
                }
                else return;
                
            }
            
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            show_order_details();
        }

        private void txtOrderNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter && !string.IsNullOrEmpty(txtOrderNo.Text))
            {
                show_order_details();
            }
        }

        private void show_order_details()
        {
            string order = txtOrderNo.Text;
            if (order.Length == 0) return;
            Server.Service sv = new Server.Service();
            DataTable dtOrder = sv.GetOrderDetailForReturn(order, out string msg);

            if (dtOrder is null || dtOrder.Rows.Count == 0)
            {
                MessageBox.Show(this, msg, "失敗");
                return;
            }
            ORDER_ID = order;
            
            for (int i = 0; i < dtOrder.Columns.Count; i++)
                dtOrder.Columns[i].ReadOnly = true;

            dtOrder.Columns.Add("this_quant", typeof(Int32));
            foreach (DataRow dr in dtOrder.Rows)
            {
                dr["this_quant"] = 0;
            }

            dataGridView1.DataSource = dtOrder;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.Columns[clsConstants.Tables.ORDER_LIST.ORDER_ID].Visible = false;

            dataGridView1.Columns[clsConstants.Tables.ORDER_DETAIL.ITEM_ID].HeaderCell.Value = "商品編號";
            dataGridView1.Columns[clsConstants.Tables.ITEM_INFO.BARCODE].HeaderCell.Value = "barcode";
            dataGridView1.Columns[clsConstants.Tables.ITEM_INFO.ITEM_NAME].HeaderCell.Value = "商品名稱";
            dataGridView1.Columns[clsConstants.Tables.ORDER_DETAIL.ITEM_OPTION].HeaderCell.Value = "商品選項";
            dataGridView1.Columns[clsConstants.Tables.ORDER_DETAIL.ITEM_QUANT].HeaderCell.Value = "數量";
            dataGridView1.Columns[clsConstants.Tables.ORDER_DETAIL.RETURN_QUANT].HeaderCell.Value = "已退回數量";
            dataGridView1.Columns["this_quant"].HeaderCell.Value = "本次退回數量";

            dataGridView1.Columns[clsConstants.Tables.ITEM_INFO.ITEM_NAME].Width = 150;
            dataGridView1.Columns[clsConstants.Tables.ORDER_DETAIL.ITEM_ID].Width = 150;
            dataGridView1.Columns["this_quant"].Width = 150;

            dataGridView1.RowTemplate.Height = 40;
            
            foreach (DataGridViewColumn c in dataGridView1.Columns)
            {
                c.ReadOnly = true;
            }
            dataGridView1.Columns["this_quant"].ReadOnly = false;

            foreach (DataGridViewColumn c in dataGridView1.Columns)
            {
                c.CellTemplate.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                c.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

        }

        private void return_process()
        {
            dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            DataTable dt = new DataTable();
            dt = (DataTable)dataGridView1.DataSource;
            bool not_all_zero = false;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr.Field<int>("this_quant") + dr.Field<int>(clsConstants.Tables.ORDER_DETAIL.RETURN_QUANT) >
                    dr.Field<int>(clsConstants.Tables.ORDER_DETAIL.ITEM_QUANT))
                {
                    MessageBox.Show(this, "總退回數量不能超過總數量", "失敗");
                    return;
                }
                if (dr.Field<int>("this_quant") > 0) not_all_zero = true;
            }
            if (!not_all_zero)
            {
                MessageBox.Show(this, "本次退回數量好像都是零, 到底要退什麼貨?", "失敗");
                return;
            }
            Server.Service sv = new Server.Service();
            if (!sv.ReturnProcess(dt, ORDER_ID, txtReason.Text, IS_FULL, out string msg))
            {
                MessageBox.Show(this, msg, "失敗");
            }
            else
            {
                MessageBox.Show(this, "退貨完成", "成功");
            }
        }

        private void txtBarcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                AddOne();
                txtBarcode.SelectAll();
                txtBarcode.Focus();
            }
        }

        private void AddOne()
        {
            DataTable dt = (DataTable)dataGridView1.DataSource;
            string bar_code = txtBarcode.Text;

            foreach (DataRow dr in dt.Rows)
            {
                if (dr[clsConstants.Tables.ITEM_INFO.BARCODE].ToString() == txtBarcode.Text)
                {
                    int i = dr.Field<int>("this_quant");
                    dr["this_quant"] = i + 1;
                }
            }
        }

        private void DataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].DataPropertyName == "this_quant")
            {
                if (!int.TryParse(e.FormattedValue.ToString(), out int i))
                {
                    MessageBox.Show(this, "麻煩給我個數字", "英文4ni?");
                    e.Cancel = true;
                }
            }
        }

    }
    }
