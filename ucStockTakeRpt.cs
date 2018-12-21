using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace GUSHA.form.user_control
{
    public partial class ucStockTakeRpt : UserControl
    {
        public ucStockTakeRpt()
        {
            InitializeComponent();
        }

        public void LoadOldStocktake(string stocktake_dt)
        {
            DataSet dsRpt;
            Server.Service sv = new Server.Service();
            dsRpt = sv.GetStocktakeTable(stocktake_dt);
            ShowReport(dsRpt);
        }

        private void ShowReport(DataSet dsRpt)
        {
            if (dsRpt == null || dsRpt.Tables.Count == 0)
            {
                MessageBox.Show(this, "error in getting data", "失敗");
                return;
            }

            var reportDataSource1 = new ReportDataSource("dsStockTake", dsRpt.Tables["stocktake_list"]);
            reportViewer1.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer1.SetDisplayMode(DisplayMode.PrintLayout);
            this.reportViewer1.RefreshReport();
        }
    }
}
