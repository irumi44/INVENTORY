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
    public partial class ucStockTakeList : UserControl
    {
        public event EventHandler NewStockTakeDelegate;
        public event EventHandler OldStockTakeDelegate;

        public string StockTakeDt { get; set; }
        public ucStockTakeList()
        {
            InitializeComponent();
        }

        public void ResetNewGrid()
        {
            Server.Service sv = new Server.Service();
            setDataGrid(sv.ShowStockTakeHistory());
        }

        private void setDataGrid(DataTable dt)
        {
            dataGridView1.DataSource = dt;
            dataGridView1.AllowUserToAddRows = false;

            dataGridView1.Columns[CommonResources.clsConstants.Tables.STOCKTAKE_HISTORY.STOCKTAKE_DT].Visible = false;
            dataGridView1.Columns["stocktake_dt_dis"].DisplayIndex = 0;
            dataGridView1.Columns["stocktake_dt_dis"].HeaderCell.Value = "盤點時間";
            dataGridView1.Columns["num"].HeaderCell.Value = "商品種類數量";
            dataGridView1.Columns["num_all"].HeaderCell.Value = "商品總數";

            foreach (DataGridViewColumn c in dataGridView1.Columns)
            {
                c.CellTemplate.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                c.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                c.ReadOnly = true;
            }

            dataGridView1.Columns["stocktake_dt_dis"].Width = 500;
            dataGridView1.Columns["num"].Width = 500;
            dataGridView1.Columns["num_all"].Width = 500;

            dataGridView1.RowTemplate.Height = 45;
            dataGridView1.Columns["stocktake_dt_dis"].DefaultCellStyle.Font = new Font("Times New Roman", 32, FontStyle.Bold);
            dataGridView1.Columns["num"].DefaultCellStyle.Font = new Font("Times New Roman", 32, FontStyle.Bold);
            dataGridView1.Columns["num_all"].DefaultCellStyle.Font = new Font("Times New Roman", 32, FontStyle.Bold);
        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);

            var senderGrid = (DataGridView)sender;

            if (e.RowIndex > -1)
            {
                //StockTakeDt = ((DataTable)dataGridView1.DataSource).Rows[e.RowIndex][CommonResources.clsConstants.Tables.STOCKTAKE_HISTORY.STOCKTAKE_DT].ToString();
                StockTakeDt = senderGrid.Rows[e.RowIndex].Cells[0].Value.ToString();
                OldStockTakeDelegate(this, e);
            }

        }

        private void btnNewStockTake_Click_1(object sender, EventArgs e)
        {
            NewStockTakeDelegate(this, e);
        }

        private void btnShowAll_Click(object sender, EventArgs e)
        {
            Server.Service sv = new Server.Service();
            setDataGrid(sv.ShowStockTakeHistory(1000));
        }

    }
}
