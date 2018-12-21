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
    public partial class ucInputOrder : UserControl
    {
        public event EventHandler DoneDelegate;
        public ucInputOrder()
        {
            InitializeComponent();
        }
        public void ClearAll()
        {
            textBox1.Text = "";
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter && !string.IsNullOrEmpty(textBox1.Text))
            {
                DoneDelegate(this, e);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text)) DoneDelegate(this, e);
        }

        public void SetTextboxFocus()
        {
            textBox1.SelectAll();
            textBox1.Focus();
        }

        public void SetGridview()
        {
            Server.Service sv = new Server.Service();
            DataTable dtOrder = sv.ShowQueue();

            if (dtOrder is null)
            {
                return;
            }

            foreach (DataRow dr in dtOrder.Rows)
            {
                if (DateTime.TryParseExact(dr[clsConstants.Tables.ORDER_LIST.ORDER_DATE].ToString(), clsConstants.VALUES.DATETIME_STRING, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dt))
                {
                    dr[clsConstants.Tables.ORDER_LIST.ORDER_DATE] = dt.ToString(clsConstants.VALUES.DATETIME_DISPLAY_STRING);
                }
                dr[clsConstants.Tables.ORDER_LIST.SHIPPING_METHOD] = cls.clsCache.ShippingSet[dr[clsConstants.Tables.ORDER_LIST.SHIPPING_METHOD].ToString()];
                dr[clsConstants.Tables.ORDER_LIST.STATUS] = cls.clsCache.StatusSet[dr[clsConstants.Tables.ORDER_LIST.STATUS].ToString()];
            }
            for (int i = 0; i < dtOrder.Columns.Count; i++)
                dtOrder.Columns[i].ReadOnly = true;

            dataGridView1.DataSource = dtOrder;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.Columns[clsConstants.Tables.ORDER_LIST.ORDER_ID].HeaderCell.Value = "訂單編號";
            dataGridView1.Columns[clsConstants.Tables.ORDER_LIST.ORDER_DATE].HeaderCell.Value = "訂單日期";
            dataGridView1.Columns[clsConstants.Tables.ORDER_LIST.RECIPIENT].HeaderCell.Value = "收貨人";
            dataGridView1.Columns[clsConstants.Tables.ORDER_LIST.TEL].HeaderCell.Value = "電話";
            dataGridView1.Columns[clsConstants.Tables.ORDER_LIST.SHIPPING_ID].HeaderCell.Value = "運單編號";
            dataGridView1.Columns[clsConstants.Tables.ORDER_LIST.SHIPPING_METHOD].HeaderCell.Value = "運送方法";
            dataGridView1.Columns[clsConstants.Tables.ORDER_LIST.STATUS].HeaderCell.Value = "狀態";
            
            foreach (DataGridViewColumn c in dataGridView1.Columns)
            {
                c.CellTemplate.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                c.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            dataGridView1.Columns[clsConstants.Tables.ORDER_LIST.ORDER_ID].Width = 150;
            dataGridView1.Columns[clsConstants.Tables.ORDER_LIST.ORDER_DATE].Width = 150;
        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);

            var senderGrid = (DataGridView)sender;

            if (e.RowIndex > -1)
            {
                //this.textBox1.Text = ((DataTable)dataGridView1.DataSource).Rows[e.RowIndex][clsConstants.Tables.ORDER_LIST.ORDER_ID].ToString();
                this.textBox1.Text = senderGrid.Rows[e.RowIndex].Cells[4].Value.ToString();
                DoneDelegate(this, e);
            }
            
        }

        public string GetOrder()
        {
            return textBox1.Text;
        }

    }
}
