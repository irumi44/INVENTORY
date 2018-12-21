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
    public partial class ucReceiveList : UserControl
    {
        public event EventHandler NewReceiveDelegate;
        public event EventHandler OldReceiveDelegate;

        public string ReceiveDt { get; set; }

        public ucReceiveList()
        {
            InitializeComponent();
        }

        public void ResetNewGrid()
        {
            Server.Service sv = new Server.Service();
            setDataGrid(sv.ShowReceiveHistory());
        }

        private void setDataGrid(DataTable dt)
        {
            dataGridView1.DataSource = dt;
            dataGridView1.AllowUserToAddRows = false;

            dataGridView1.Columns[CommonResources.clsConstants.Tables.RECEIVE_HISTORY.RECEIVE_DT].Visible = false;
            dataGridView1.Columns["receive_dt_dis"].DisplayIndex = 0;
            dataGridView1.Columns["receive_no"].DisplayIndex = 1;
            dataGridView1.Columns["remarks"].DisplayIndex = 2;
            dataGridView1.Columns["num"].DisplayIndex = 3;
            dataGridView1.Columns["receive_dt_dis"].HeaderCell.Value = "進貨時間";
            dataGridView1.Columns["receive_no"].HeaderCell.Value = "單據號碼";
            dataGridView1.Columns["remarks"].HeaderCell.Value = "備註";
            dataGridView1.Columns["num"].HeaderCell.Value = "商品數量"; ;

            foreach (DataGridViewColumn c in dataGridView1.Columns)
            {
                c.CellTemplate.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                c.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                c.ReadOnly = true;
            }

            dataGridView1.Columns["receive_dt_dis"].Width = 400;
            dataGridView1.Columns["receive_no"].Width = 100;
            dataGridView1.Columns["remarks"].Width = 100;
            dataGridView1.Columns["num"].Width = 400;

            dataGridView1.RowTemplate.Height = 45;
            dataGridView1.Columns["receive_dt_dis"].DefaultCellStyle.Font = new Font("Times New Roman", 32, FontStyle.Bold);
            dataGridView1.Columns["num"].DefaultCellStyle.Font = new Font("Times New Roman", 32, FontStyle.Bold);
        }
        
        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);

            var senderGrid = (DataGridView)sender;

            if (e.RowIndex > -1)
            {
                //ReceiveDt = ((DataTable)dataGridView1.DataSource).Rows[e.RowIndex][CommonResources.clsConstants.Tables.RECEIVE_HISTORY.RECEIVE_DT].ToString();
                ReceiveDt = senderGrid.Rows[e.RowIndex].Cells[0].Value.ToString();
                OldReceiveDelegate(this, e);
            }

        }

        private void btnNewReceive_Click(object sender, EventArgs e)
        {
            NewReceiveDelegate(this, e);
        }

        private void btnShowAll_Click_1(object sender, EventArgs e)
        {
            Server.Service sv = new Server.Service();
            setDataGrid(sv.ShowReceiveHistory(1000));
        }
    }
}
