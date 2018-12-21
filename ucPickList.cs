using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;
using Microsoft.Reporting.WinForms;
using CommonResources;

namespace GUSHA.form.user_control
{
    public partial class ucPickList : UserControl
    {
        public event EventHandler NewPickDelegate;
        public event EventHandler OldPickDelegate;

        public string PickDt { get; set; }

        public ucPickList()
        {
            InitializeComponent();
            Server.Service sv = new Server.Service();
            setDataGrid(sv.ShowPickHistory());
        }


        private void setDataGrid(DataTable dt)
        {
            dataGridView1.DataSource = dt;
            dataGridView1.AllowUserToAddRows = false;

            dataGridView1.Columns[clsConstants.Tables.PICKING_HISTORY.PICK_DT].Visible = false;
            dataGridView1.Columns["pick_dt_dis"].DisplayIndex = 0;
            dataGridView1.Columns["pick_dt_dis"].HeaderCell.Value = "撿貨時間";
            dataGridView1.Columns["num"].HeaderCell.Value = "訂單數量";;

            foreach (DataGridViewColumn c in dataGridView1.Columns)
            {
                c.CellTemplate.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                c.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                c.ReadOnly = true;
            }

            dataGridView1.Columns["pick_dt_dis"].Width = 500;
            dataGridView1.Columns["num"].Width = 500;

            dataGridView1.RowTemplate.Height = 45;
            dataGridView1.Columns["pick_dt_dis"].DefaultCellStyle.Font = new Font("Times New Roman", 32, FontStyle.Bold);
            dataGridView1.Columns["num"].DefaultCellStyle.Font = new Font("Times New Roman", 32, FontStyle.Bold);
        }

        private void btnNewPick_Click(object sender, EventArgs e)
        {
            NewPickDelegate(this, e);
        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);

            var senderGrid = (DataGridView)sender;

            if (e.RowIndex > -1)
            {
                //PickDt = ((DataTable)dataGridView1.DataSource).Rows[e.RowIndex][clsConstants.Tables.PICKING_HISTORY.PICK_DT].ToString();
                PickDt = senderGrid.Rows[e.RowIndex].Cells[0].Value.ToString();
                OldPickDelegate(this, e);
            }

        }

        private void btnShowAll_Click(object sender, EventArgs e)
        {
            Server.Service sv = new Server.Service();
            setDataGrid(sv.ShowPickHistory(1000));
        }
    }
}
