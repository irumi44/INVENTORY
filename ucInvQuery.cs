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
    public partial class ucInvQuery : UserControl
    {
        public ucInvQuery()
        {
            InitializeComponent();
            cboNumCompare.DataSource = new BindingSource(cls.clsCache.NumCompareSet, null);
            cboNumCompare.DisplayMember = clsConstants.VALUES.VALUE;
            cboNumCompare.ValueMember = clsConstants.VALUES.KEY;
            cboNumCompare.SelectedValue = clsConstants.VALUES.SYS_CODESET.NUM_COMPARE.LARGER;
            txtNum.Text = "-1";

            cboStatus.DataSource = new BindingSource(cls.clsCache.ItemStatusSet, null);
            cboStatus.DisplayMember = clsConstants.VALUES.VALUE;
            cboStatus.ValueMember = clsConstants.VALUES.KEY;
            txtBarcode.SelectAll();
            
        }
        public void SetFocus()
        {
            txtBarcode.Focus();
        }
        public void SetGridview()
        {
            Server.Service sv = new Server.Service();
            DataTable dtItem;
            if (chkFindEmpty.Checked) { dtItem = sv.QueryEmptyInv(); }
            else { dtItem = sv.QueryInv(txtItemId.Text, txtBarcode.Text, txtLocation.Text, 
                                        cboStatus.SelectedValue == null ? string.Empty : cboStatus.SelectedValue.ToString() , 
                                        txtName.Text, 
                                        cboNumCompare.SelectedValue == null ? string.Empty : cboNumCompare.SelectedValue.ToString(), 
                                        txtNum.Text); }
            
            if (dtItem is null)
            {
                return;
            }
            for (int i = 0; i < dtItem.Columns.Count; i++)
                dtItem.Columns[i].ReadOnly = true;
            if (dtItem.Rows.Count == 0)
            {
                DataRow drNew = dtItem.NewRow();
                drNew[clsConstants.Tables.ITEM_INFO.ITEM_ID] = "找不到";
                drNew[clsConstants.Tables.ITEM_INFO.ITEM_NAME] = "找不到";
                drNew[clsConstants.Tables.ITEM_INFO.ITEM_LOCATION] = "找不到";
                drNew[clsConstants.Tables.ITEM_INFO.ITEM_MASS_LOCATION] = "找不到";
                drNew[clsConstants.Tables.ITEM_INFO.STATUS] = "找不到";
                dtItem.Rows.Add(drNew);
            }
            dataGridView1.RowTemplate.Height = 50;
            dataGridView1.DataSource = dtItem;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.Columns[clsConstants.Tables.ITEM_INFO.ITEM_ID].HeaderCell.Value = "商品編號";
            dataGridView1.Columns[clsConstants.Tables.ITEM_INFO.ITEM_NAME].HeaderCell.Value = "商品名稱";
            dataGridView1.Columns[clsConstants.Tables.ITEM_INFO.ITEM_LOCATION].HeaderCell.Value = "商品位置";
            dataGridView1.Columns[clsConstants.Tables.ITEM_INFO.ITEM_MASS_LOCATION].HeaderCell.Value = "商品剩餘量位置";
            dataGridView1.Columns[clsConstants.Tables.ITEM_INFO.STATUS].HeaderCell.Value = "商品狀態";
            dataGridView1.Columns[clsConstants.Tables.ITEM_INFO.BARCODE].HeaderCell.Value = "商品barcode";
            dataGridView1.Columns[clsConstants.Tables.ITEM_INFO.INV].HeaderCell.Value = "商品庫存";

            dataGridView1.Columns[clsConstants.Tables.ITEM_INFO.ITEM_LOCATION].Width = 250;
            dataGridView1.Columns[clsConstants.Tables.ITEM_INFO.ITEM_LOCATION].DefaultCellStyle.Font = new Font("Times New Roman", 32, FontStyle.Bold);

            foreach (DataGridViewColumn c in dataGridView1.Columns)
            {
                c.CellTemplate.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                c.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            //dataGridView1.Columns["order_id"].Width = 150;
            //dataGridView1.Columns["order_date"].Width = 150;
            txtBarcode.SelectAll();
            txtBarcode.Focus();
        }

        private void CleanAndLockAll(bool enab)
        {
            foreach (TextBox t in new TextBox[] { txtItemId, txtBarcode, txtLocation, txtName, txtNum})
            {
                t.Text = string.Empty;
                t.Enabled = enab;
            }

            foreach (ComboBox t in new ComboBox[] { cboNumCompare, cboStatus })
            {
                t.SelectedValue = clsConstants.VALUES.SYS_CODESET.NUM_COMPARE.LARGER;
                t.Enabled = enab;
            }
            if (enab) txtNum.Text = "-1";
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            CleanAndLockAll(!chkFindEmpty.Checked);
            
        }

        private void btnOK_Click_1(object sender, EventArgs e)
        {
            SetGridview();
        }
        private void txtBarcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter && !string.IsNullOrEmpty(txtBarcode.Text))
            {
                SetGridview();
            }
        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);

            var senderGrid = (DataGridView)sender;

            if (e.RowIndex > -1)
            {
                frmModifyItem f = new frmModifyItem();
                f.LoadRecord(senderGrid.Rows[e.RowIndex].Cells[0].Value.ToString());
                f.ShowDialog();
                SetGridview();
            }

        }
    }
}
