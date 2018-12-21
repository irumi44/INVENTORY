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

namespace GUSHA.form.user_control
{
    public partial class ucPickRpt : UserControl
    {

        public event EventHandler NonePickDelegate;
        public ucPickRpt()
        {
            InitializeComponent();
        }

        public void LoadOldPick(string pick_dt)
        {
            DataSet dsRpt;
            Server.Service sv = new Server.Service();
            dsRpt = sv.GetPickingTable(pick_dt);
            ShowReport(dsRpt);
        }

        public void LoadNewPick()
        {
            DataSet dsRpt;
            Server.Service sv = new Server.Service();
            dsRpt = sv.GetPickingTable();
            ShowReport(dsRpt);
        }

        private void ShowReport(DataSet dsRpt)
        {
            if (dsRpt == null || dsRpt.Tables.Count == 0)
            {
                MessageBox.Show(this, "error in getting data", "錯誤");
                return;
            }
            else if (dsRpt.Tables["order_list"].Rows.Count == 0 )
            {
                NonePickDelegate(this, new EventArgs());
                return;
            }
            BarcodeWriter<Bitmap> bw = new BarcodeWriter<Bitmap>();
            ZXing.Common.EncodingOptions op = new ZXing.Common.EncodingOptions { Height = 100, Width = 400 };
            op.GS1Format = true;
            op.PureBarcode = false;

            bw.Options = op;
            bw.Format = BarcodeFormat.CODE_128;
            ZXing.Rendering.BitmapRenderer rd = new ZXing.Rendering.BitmapRenderer();
            bw.Renderer = rd;
            ImageConverter converter = new ImageConverter();

            foreach (DataRow dr in dsRpt.Tables["order_list"].Rows)
            {
                dr["barcode"] = (byte[])converter.ConvertTo(bw.Write(dr[CommonResources.clsConstants.Tables.ORDER_LIST.ORDER_ID].ToString()), typeof(byte[]));
            }

            var reportDataSource1 = new ReportDataSource("DataSet1", dsRpt.Tables["pick_list"]);
            reportViewer1.LocalReport.DataSources.Add(reportDataSource1);
            var reportDataSource2 = new ReportDataSource("DataSet2", dsRpt.Tables["order_list"]);
            reportViewer1.LocalReport.DataSources.Add(reportDataSource2);
            this.reportViewer1.SetDisplayMode(DisplayMode.PrintLayout);
            this.reportViewer1.RefreshReport();
        }
    }
}
