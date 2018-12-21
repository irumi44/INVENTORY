using System;
using System.Data.SqlClient;
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
    public partial class ucOrderDetail : UserControl
    {
        public event EventHandler DoneDelegate;
        public event EventHandler CantFindDelegate;
        public event EventHandler AlreadyDoneDelegate;
        public event EventHandler SetShippingIDDelegate;
        public event EventHandler ErrorDelegate;
        int rowCount = -1;
        int col_index_total = 3;
        int col_index_current = 4;
        public string ShippingID { set; get; }
        public string OrderID { set; get; }

        public ucOrderDetail()
        {
            InitializeComponent();
            dataGridView1.CellFormatting += DataGridView1_CellFormatting;
            dataGridView1.ColumnDisplayIndexChanged += DataGridView1_ColumnDisplayIndexChanged;

        }

        private void DataGridView1_ColumnDisplayIndexChanged(object sender, DataGridViewColumnEventArgs e)
        {
            //if (dataGridView1.DataSource == null) return;
            //foreach (DataGridViewColumn c in dataGridView1.Columns)
            //{
            //    if (c.HeaderText == "數量")
            //    {
            //        col_index_total = c.DisplayIndex;
            //    }
            //    else if (c.HeaderText == "已刷數量")
            //    {
            //        col_index_current = c.DisplayIndex;
            //    }
            //}
        }

        private void DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == col_index_current && ((int)dataGridView1.Rows[e.RowIndex].Cells[col_index_total].Value) > 1)
            {
                DataGridViewCellStyle s = e.CellStyle;
                s.ForeColor = Color.Red;
                s.Font = new Font("Times New Roman", 32, FontStyle.Bold);
                e.CellStyle = s;
            }

        }

        public void ClearAll()
        {
            ((DataTable)dataGridView1.DataSource).Clear();
        }

        public bool SetSource(string shipping_id)
        {
            Server.Service sv = new Server.Service();
            DataTable dtOrder = sv.FindDetail(shipping_id, out bool is_done, out string order_id, out string date, out string remarks);
            
            if (dtOrder is null || dtOrder.Rows.Count == 0)
            {
                CantFindDelegate(this, new EventArgs());
                return false;
            }
            if (is_done)
            {
                AlreadyDoneDelegate(this, new EventArgs());
                return false;
            }
            OrderID = dtOrder.Rows[0][clsConstants.Tables.ORDER_LIST.ORDER_ID].ToString();

            if (!sv.UpdatePickDt(OrderID))
            {
                MessageBox.Show(this, "Internal Error", "錯誤");
                return false;
            }
            ShippingID = shipping_id;
            SetShippingIDDelegate(this, new EventArgs());
            this.lblNumber.Text = order_id;
            if (DateTime.TryParseExact(date, clsConstants.VALUES.DATETIME_STRING, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dt))
            {
                this.lblDate.Text = dt.ToString(clsConstants.VALUES.DATETIME_DISPLAY_STRING);
            }
            else
            {
                this.lblDate.Text = date;
            }
                
            this.lblTrans.Text = shipping_id;

            textBox2.Text = remarks;

            for (int i = 0;i<dtOrder.Columns.Count;i++)
                dtOrder.Columns[i].ReadOnly = false;

            rowCount = dtOrder.Rows.Count;
            dataGridView1.DataSource = dtOrder;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.Columns[clsConstants.Tables.ORDER_LIST.ORDER_ID].Visible = false;

            dataGridView1.Columns[clsConstants.Tables.ORDER_DETAIL.ITEM_ID].HeaderCell.Value = "商品編號";
            dataGridView1.Columns[clsConstants.Tables.ITEM_INFO.BARCODE].HeaderCell.Value = "barcode";
            dataGridView1.Columns[clsConstants.Tables.ITEM_INFO.ITEM_NAME].HeaderCell.Value = "商品名稱";
            dataGridView1.Columns[clsConstants.Tables.ORDER_DETAIL.ITEM_OPTION].HeaderCell.Value = "商品選項";
            dataGridView1.Columns[clsConstants.Tables.ORDER_DETAIL.ITEM_QUANT].HeaderCell.Value = "數量";
            dataGridView1.Columns["current_quant"].HeaderCell.Value = "已刷數量";
            dataGridView1.Columns[clsConstants.Tables.ORDER_DETAIL.TEMP_LOCATION].HeaderCell.Value = "暫存位置";
            dataGridView1.Columns["complete"].HeaderCell.Value = "完成";

            foreach (DataGridViewColumn c in dataGridView1.Columns)
            {
                c.CellTemplate.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                c.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            dataGridView1.Columns[clsConstants.Tables.ITEM_INFO.ITEM_NAME].Width = 150;
            dataGridView1.Columns[clsConstants.Tables.ORDER_DETAIL.TEMP_LOCATION].Width = 150;
            dataGridView1.Columns[clsConstants.Tables.ORDER_DETAIL.ITEM_ID].Width = 150;

            dataGridView1.RowTemplate.Height = 40;
            dataGridView1.Columns[clsConstants.Tables.ORDER_DETAIL.TEMP_LOCATION].DefaultCellStyle.Font = new Font("Times New Roman", 32, FontStyle.Bold);

            foreach (DataGridViewColumn c in dataGridView1.Columns)
            {
                c.ReadOnly = false;
            }
            //dataGridView1.Columns["complete"].ReadOnly = false;

            return true;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {            
            dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);

            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns["complete"] is DataGridViewCheckBoxColumn &&
                count_current())
            {
                this.DoneDelegate(this, e);
            }
        }

        private bool count_current()
        {
            int CurrentRowCount = 0;
            DataTable dt = (DataTable)dataGridView1.DataSource;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr.Field<bool>("complete")) CurrentRowCount++;

            }
            return CurrentRowCount == rowCount;
        }

        public void SetTextboxFocus()
        {
            textBox1.Focus();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                EnterItemCode();
                SetTextboxFocus();
                textBox1.SelectAll();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            EnterItemCode();
            SetTextboxFocus();
            textBox1.SelectAll();
        }
        //ErrorDelegate
        private bool IsItemAvailable()
        {
            DataTable dt = (DataTable)dataGridView1.DataSource;
            string bar_code = textBox1.Text;
            DataRow[] drs = (from DataRow drr in dt.Rows
                             where drr[clsConstants.Tables.ITEM_INFO.BARCODE].ToString() == bar_code
                             select drr).ToArray();
            foreach (DataRow dr in drs)
            {
                if (dr[clsConstants.Tables.ITEM_INFO.BARCODE].ToString() == textBox1.Text)
                {
                    if ((bool)dr["complete"] == true)
                    {
                        //MessageBox.Show(this, "This item is already checked!!!");
                        return false;
                    }
                    else
                    {
                        dr["current_quant"] = int.Parse(dr["current_quant"].ToString()) + 1;
                        if (int.Parse(dr["current_quant"].ToString()) == int.Parse(dr[clsConstants.Tables.ORDER_DETAIL.ITEM_QUANT].ToString()))
                            dr["complete"] = true;
                        if (count_current())
                        {
                            this.DoneDelegate(this, new EventArgs());
                        }
                        textBox1.Text = string.Empty;
                        return true;
                    }
                }
            }
            return false;
            //MessageBox.Show(this, "This item does not belong to the order!!!");
        }

        private void EnterItemCode()
        {
            if (!IsItemAvailable())
            {

                ErrorDelegate(this, new EventArgs());
            }
        }
        
    }
}
