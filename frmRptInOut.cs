using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonResources;
using Microsoft.Reporting.WinForms;

namespace GUSHA.form
{
    public partial class frmRptInOut : ChildForm
    {
        public frmRptInOut()
        {
            InitializeComponent();
            rboAll.Checked = true;            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataSet dsRpt;
            Server.Service sv = new Server.Service();
            dsRpt = sv.GetRptInOut(txtRcvNo.Text, txtItemId.Text,txtItemName.Text, calFrom.Value.ToString(clsConstants.VALUES.DATE_STRING) + 
                                  "0000000", calTo.Value.ToString(clsConstants.VALUES.DATE_STRING) + "2359599", 
                                  rboOut.Checked? "OUT" : rboIn.Checked ? "IN" : "ALL", txtRemarks.Text);
            ShowReport(dsRpt);
        }

        private void ShowReport(DataSet dsRpt)
        {
            if (dsRpt == null || dsRpt.Tables.Count == 0)
            {
                MessageBox.Show(this, "error in getting data", "失敗");
                return;
            }
            else if (dsRpt.Tables["rpt_inout"].Rows.Count ==0)
            {
                MessageBox.Show(this, "找不到出入庫資料", "找無");
                return;
            }

            DataTable dtCriteria = new DataTable("criteria");
            dtCriteria.Columns.Add("word");
            DataRow drNew = dtCriteria.NewRow();
            drNew["word"] = "搜尋條件:\n" +
                            "單據編號 " + txtRcvNo.Text + "\n" +
                            "商品編號 " + txtItemId.Text + "\n" +
                            "商品名稱 " + txtItemName.Text + "\n" +
                            "日期 " + calFrom.Text + " 至 " + calTo.Text + "\n" +
                            (rboAll.Checked ? rboAll.Text : rboIn.Checked ? rboIn.Text : rboOut.Text) + "\n" +
                            "備註 " + txtRemarks.Text;
            dtCriteria.Rows.Add(drNew);
            var reportDataSource2 = new ReportDataSource("DataSet2", dtCriteria);
            var reportDataSource1 = new ReportDataSource("DataSet1", dsRpt.Tables["rpt_inout"]);
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(reportDataSource1);
            reportViewer1.LocalReport.DataSources.Add(reportDataSource2);
            this.reportViewer1.SetDisplayMode(DisplayMode.PrintLayout);
            this.reportViewer1.RefreshReport();
        }

        private void frmRptInOut_Load(object sender, EventArgs e)
        {

            this.reportViewer1.RefreshReport();
        }

        private void panelUp_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
