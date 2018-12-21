using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;
using System.Text.RegularExpressions;
using CommonResources;

namespace GUSHA.form
{
    public partial class frmImportOrder : ChildForm
    {
        public frmImportOrder()
        {
            InitializeComponent();
        }
        public void SetDefault()
        {
            lblProcessing.Visible = false;
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ImportOrder(openFileDialog1.FileName); 
            }            
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter && !string.IsNullOrEmpty(textBox1.Text))
            {
                ImportOrder(textBox1.Text);
            }
        }

        private void ImportOrder(string file_path)
        {
            if (string.IsNullOrEmpty(file_path) || (!file_path.EndsWith("xls") && (!file_path.EndsWith("xlsx"))))
            {
                MessageBox.Show(this, "Please select an Excel file or pay more money");
                return;
            }
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                lblProcessing.Visible = true;
                DataTable dt = GetDataOrder(file_path);
                if (dt == null || dt.Rows.Count == 0) throw new Exception("沒吃到任何一條DATA");
                for (int i = 0; i<dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (Regex.IsMatch(dt.Rows[i][j].ToString(), CommonResources.clsConstants.VALUES.EXCEL_NUM, RegexOptions.IgnoreCase))
                        {
                            throw new Exception(string.Format("在第{0}列, 第{1}行資料被EXCEL認成科學數字了, 麻煩請TEXT( ,\"0\")",(i+2).ToString(),(j+1).ToString()));
                        }
                    }
                }
                Server.Service sv = new Server.Service();
                if (sv.ImportOrder(dt,out string msg))
                {
                    string ms = string.Empty;
                    if (!string.IsNullOrEmpty(msg)) ms += msg;
                    MessageBox.Show(this, "Import succeeded \n 匯入成功\n" + ms, "成功");
                }
                else
                {
                    throw new Exception(msg);
                }
            }
            catch (Exception ex)
            {
                string m = string.Empty;
                if (ex.InnerException != null) m += ex.InnerException.Message + "\n";
                m += ex.Message + "\n";
                MessageBox.Show(this, m + "Import failed  \n 匯入失敗", "失敗");
            }
            finally
            {

                Cursor.Current = Cursors.Default;
                lblProcessing.Visible = false;
            }
            
        }

        private string CheckSheet(string file_path, int col_num)
        {
            DataTable schemaTable = null;
            DataTable colTable = null;
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = ConnectionString(file_path, "No") })
            {
                cn.Open();
                schemaTable = cn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new Object[] { null, null, null, "TABLE" });
                colTable = cn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, null);
                cn.Close();
            }
            if (schemaTable == null || colTable == null)
            {
                return null;
            }
            List<string> sheetList = new List<string>();
            foreach (DataRow dr in schemaTable.Rows)
            {
                string p = (from DataRow drCol in colTable.Rows
                            where drCol["TABLE_NAME"].ToString() == dr["TABLE_NAME"].ToString()
                            orderby drCol["ORDINAL_POSITION"] descending
                            select drCol["ORDINAL_POSITION"].ToString()).FirstOrDefault();
                if (p == col_num.ToString())
                {
                    sheetList.Add(dr["TABLE_NAME"].ToString());
                }
            }
            if (sheetList.Count > 1)
            {
                string m = "這些sheets 都長很像, 我還沒聰明到瞭解你想用哪個sheet, 麻煩刪一刪\n";
                foreach (string s in sheetList)
                {
                    m += s.Replace("$","") + ", ";
                }
                m = m.Substring(0, m.Length - 2);
                MessageBox.Show(this, m, "失敗");
                return null;
            }
            else if (sheetList.Count == 0)
            {
                MessageBox.Show(this, "我找不到資料行數符合的sheet, 請填好填滿" + col_num.ToString() + "個", "失敗");
                return null;
            }

            return sheetList[0];
        }

        private DataTable GetDataOrder(string file_path)
        {
            string sheet = CheckSheet(file_path, 19);
            if (string.IsNullOrEmpty(sheet)) return null;

            var dt = new DataTable();
            
            string query = "SELECT F1 As order_id, F2 As order_date, F3 As recipient, F4 As tel, F5 As item_id, " +
                " F6 As item_name, F7 As item_option, F8 As item_quant, F9 As shipping_id, F10 As total_amt, " +
                " F11 As shipping_fee, F12 As attach_fee, F13 As discount, F14 As price_original, " +
                " F15 As price_on_sale, F16 As shipping_method, F17 AS receive_shop, F18 AS address, F19 AS remarks FROM " +
                " [" + sheet + "]" +
                " WHERE  NOT IsNull(F1) OR NOT IsNull(F2) OR NOT IsNull(F3) OR NOT IsNull(F4) OR NOT IsNull(F5) OR NOT IsNull(F6) OR NOT " +
                " IsNull(F7) OR NOT IsNull(F8) OR  NOT IsNull(F9) OR NOT IsNull(F10) OR NOT IsNull(F11) OR NOT IsNull(F12) OR NOT " +
                " IsNull(F13) OR NOT IsNull(F14) OR NOT IsNull(F15) OR NOT IsNull(F16) OR NOT IsNull(F17)";
            using (OleDbConnection cn = new OleDbConnection { ConnectionString = ConnectionString(file_path, "No") })
            {
                using (OleDbCommand cmd = new OleDbCommand { CommandText = query, Connection = cn })
                {
                    try
                    {
                        cn.Open();
                        
                        OleDbDataReader dr = cmd.ExecuteReader();
                        dt.Load(dr);
                    }
                    catch (Exception ex)
                    {
                    }
                    finally
                    {
                        cn.Close();
                    }
                }
            }
            if (dt.Rows.Count > 1)
            {
                //check each column header
                string[] dbHeader = new string[] {clsConstants.Tables.ORDER_LIST.ORDER_ID, clsConstants.Tables.ORDER_LIST.ORDER_DATE,
                clsConstants.Tables.ORDER_LIST.RECIPIENT, clsConstants.Tables.ORDER_LIST.TEL, clsConstants.Tables.ORDER_DETAIL.ITEM_ID,
                clsConstants.Tables.ORDER_DETAIL.ITEM_NAME,clsConstants.Tables.ORDER_DETAIL.ITEM_OPTION,clsConstants.Tables.ORDER_DETAIL.ITEM_QUANT,
                clsConstants.Tables.ORDER_LIST.SHIPPING_ID, clsConstants.Tables.ORDER_LIST.TOTAL_AMT, clsConstants.Tables.ORDER_LIST.SHIPPING_FEE,
                clsConstants.Tables.ORDER_LIST.ATTACH_FEE, clsConstants.Tables.ORDER_LIST.DISCOUNT, clsConstants.Tables.ORDER_DETAIL.PRICE_ORIGINAL,
                clsConstants.Tables.ORDER_DETAIL.PRICE_ON_SALE, clsConstants.Tables.ORDER_LIST.SHIPPING_METHOD, clsConstants.Tables.ORDER_LIST.RECEIVE_SHOP,
                clsConstants.Tables.ORDER_LIST.ADDRESS, clsConstants.Tables.ORDER_LIST.REMARKS};

                string[] shouldHeader = new string[] {"訂單", "訂單日", "收", "電", "商", "名", "選", "數", "貨", "總", "運", "附"
                , "折", "原", "特", "送", "門", "地", ""};

                bool suspicious = false;
                string susMT = "你確定這欄{0}是{1}?\n";
                string susM = string.Empty;
                for (int i = 0;i<dbHeader.Length;i++)
                {
                    if (!dt.Rows[0][dbHeader[i]].ToString().Contains(shouldHeader[i]))
                    {
                        suspicious = true;
                        susM += string.Format(susMT, dt.Rows[0][dbHeader[i]].ToString(), dbHeader[i]);
                    }
                }
                if (suspicious && MessageBox.Show(this, susM, "真的?", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                {
                    return null;
                }
                dt.Rows[0].Delete();
            }
            dt.AcceptChanges();

            return dt;
        }
        public string ConnectionString(string FileName, string Header)
        {
            OleDbConnectionStringBuilder Builder = new OleDbConnectionStringBuilder();
            if (Path.GetExtension(FileName).ToUpper() == ".XLS")
            {
                Builder.Provider = "Microsoft.Jet.OLEDB.4.0";
                Builder.Add("Extended Properties", string.Format("Excel 8.0;IMEX=1;HDR={0};", Header));
            }
            else
            {
                Builder.Provider = "Microsoft.ACE.OLEDB.12.0";
                Builder.Add("Extended Properties", string.Format("Excel 12.0;IMEX=1;HDR={0};", Header));
            }
            
            Builder.DataSource = FileName;

            return Builder.ConnectionString;
        }
        
    }
}
