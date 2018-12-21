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
    public partial class ucOrderQuery : UserControl
    {
        public ucOrderQuery()
        {
            InitializeComponent();

            comboBox1.DataSource = new BindingSource(cls.clsCache.ShippingSet, null);
            comboBox1.DisplayMember = clsConstants.VALUES.VALUE;
            comboBox1.ValueMember = clsConstants.VALUES.KEY;

            comboBox2.DataSource = new BindingSource(cls.clsCache.StatusSet, null);
            comboBox2.DisplayMember = clsConstants.VALUES.VALUE;
            comboBox2.ValueMember = clsConstants.VALUES.KEY;

            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;

        }

        public void SetGridview()
        {
            Server.Service sv = new Server.Service();
            DataTable dtOrder = sv.QueryOrder(textBox1.Text, dateTimePicker1.Value.ToString(clsConstants.VALUES.DATE_STRING) + "0000000",
                                              dateTimePicker2.Value.ToString(clsConstants.VALUES.DATE_STRING) + "2359599",
                                              textBox3.Text, textBox4.Text, textBox2.Text, 
                                              comboBox1.SelectedValue == null ? string.Empty : comboBox1.SelectedValue.ToString(), 
                                              comboBox2.SelectedValue == null ? string.Empty : comboBox2.SelectedValue.ToString()
                                              , textBox7.Text, textBox8.Text, textBox9.Text);

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
                if (DateTime.TryParseExact(dr[clsConstants.Tables.ORDER_LIST.PICK_DT].ToString(), clsConstants.VALUES.DATETIME_STRING, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dtp))
                {
                    dr[clsConstants.Tables.ORDER_LIST.PICK_DT] = dtp.ToString(clsConstants.VALUES.DATETIME_DISPLAY_STRING);
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
            dataGridView1.Columns[clsConstants.Tables.ORDER_LIST.PICK_DT].HeaderCell.Value = "最後出貨時間";
            dataGridView1.Columns[clsConstants.Tables.ORDER_DETAIL.ITEM_ID].HeaderCell.Value = "商品編號";
            dataGridView1.Columns[clsConstants.Tables.ORDER_DETAIL.ITEM_NAME].HeaderCell.Value = "商品名稱";
            dataGridView1.Columns[clsConstants.Tables.ORDER_DETAIL.ITEM_QUANT].HeaderCell.Value = "商品數量";

            foreach (DataGridViewColumn c in dataGridView1.Columns)
            {
                c.CellTemplate.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                c.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            dataGridView1.Columns[clsConstants.Tables.ORDER_LIST.ORDER_ID].Width = 150;
            dataGridView1.Columns[clsConstants.Tables.ORDER_LIST.ORDER_DATE].Width = 150;
        }

        private void btnOK_Click_1(object sender, EventArgs e)
        {
            SetGridview();
        }
    }
}
