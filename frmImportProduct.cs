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
    public partial class frmImportProduct : ChildForm
    {
        public frmImportProduct()
        {
            InitializeComponent();
        }
        public void SetDefault()
        {
            lblProcessing.Visible = false;
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

        private void btnChoose2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ImportItem(openFileDialog1.FileName);
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter && !string.IsNullOrEmpty(textBox2.Text))
            {
                ImportItem(textBox2.Text);
            }
        }

        private void ImportItem(string file_path)
        {
            if (string.IsNullOrEmpty(file_path) || (!file_path.EndsWith("xls") && (!file_path.EndsWith("xlsx"))))
            {
                MessageBox.Show(this, "Please select an Excel file or pay more money", "失敗");
                return;
            }
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                lblProcessing.Visible = true;
                DataTable dt = GetDataItem(file_path);
                if (dt == null || dt.Rows.Count == 0) throw new Exception("沒吃到任何一條DATA");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (Regex.IsMatch(dt.Rows[i][j].ToString(), CommonResources.clsConstants.VALUES.EXCEL_NUM, RegexOptions.IgnoreCase))
                        {
                            throw new Exception(string.Format("在第{0}列, 第{1}行資料被EXCEL認成科學數字了, 麻煩請TEXT( ,\"0\")", (i + 2).ToString(), (j+1).ToString()));
                        }
                    }
                }
                Server.Service sv = new Server.Service();
                if (sv.ImportItem(dt, chkIgnore.Checked,out string msg))
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
                    m += s.Replace("$", "") + ", ";
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

        private DataTable GetDataItem(string file_path)
        {
            string sheet = CheckSheet(file_path, 6);
            if (string.IsNullOrEmpty(sheet)) return null;

            var dt = new DataTable();

            var query = "SELECT F1 As item_id, F2 As item_location, F3 As item_name, CStr(F4) As barcode, F5 As inv, F6 As item_mass_location FROM [product$]" +
                        " WHERE NOT IsNull(F1) OR NOT IsNull(F2) OR NOT IsNull(F3) OR NOT IsNull(F4) OR NOT IsNull(F5) ";
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
                string[] dbHeader = new string[] {clsConstants.Tables.ITEM_INFO.ITEM_ID, clsConstants.Tables.ITEM_INFO.ITEM_LOCATION,
                clsConstants.Tables.ITEM_INFO.ITEM_NAME, clsConstants.Tables.ITEM_INFO.BARCODE, clsConstants.Tables.ITEM_INFO.INV,
                clsConstants.Tables.ITEM_INFO.ITEM_MASS_LOCATION};

                string[] shouldHeader = new string[] {"id", "loca", "nam", "bar", "n", "mass"};

                bool suspicious = false;
                string susMT = "你確定這欄{0}是{1}?\n";
                string susM = string.Empty;
                for (int i = 0; i < dbHeader.Length; i++)
                {
                    if (!dt.Rows[0][dbHeader[i]].ToString().ToLower().Contains(shouldHeader[i]))
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
        
    }
}
