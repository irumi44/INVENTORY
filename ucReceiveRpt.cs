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
    public partial class ucReceiveRpt : UserControl
    {
        public ucReceiveRpt()
        {
            InitializeComponent();
        }
        public void LoadOldReceive(string receive_dt)
        {
            DataSet dsRpt;
            Server.Service sv = new Server.Service();
            dsRpt = sv.GetReceiveTable(receive_dt);
            ShowReport(dsRpt);
        }

        private void ShowReport(DataSet dsRpt)
        {
            if (dsRpt == null || dsRpt.Tables.Count == 0)
            {
                MessageBox.Show(this, "error in getting data", "失敗");
                return;
            }
            
            var reportDataSource1 = new ReportDataSource("dsReceive", dsRpt.Tables["receive_list"]);
            reportViewer1.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer1.SetDisplayMode(DisplayMode.PrintLayout);
            this.reportViewer1.RefreshReport();
        }
    }
}
