
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using CommonResources;
namespace CommonResources
{
    public static class Order_Separator
    {
        public static void FilterItem(bool ignore, ref DataTable dt)
        {
            try
            {
                DataRow[] drs = (from DataRow dr in dt.Rows
                                 where dr[clsConstants.Tables.ITEM_INFO.ITEM_ID] == DBNull.Value || dr[clsConstants.Tables.ITEM_INFO.ITEM_ID].ToString().Length == 0
                                 select dr).ToArray();
                if (drs.Count() > 0)
                {
                    dt = null;
                    return;
                }

                if (!ignore)
                {
                    drs = (from DataRow dr in dt.Rows
                           join DataRow dr2 in dt.Copy().Rows
                           on dr[clsConstants.Tables.ITEM_INFO.ITEM_ID].ToString() equals dr2[clsConstants.Tables.ITEM_INFO.ITEM_ID].ToString()
                           select dr).ToArray();

                    if (drs.Count() > dt.Rows.Count)
                    {
                        dt = null;
                        return;
                    }
                }
                List<string> ids = new List<string>();
                for (int i = dt.Rows.Count - 1; i >= 0; i--)
                {
                    dt.Rows[i][clsConstants.Tables.ITEM_INFO.ITEM_ID] = dt.Rows[i][clsConstants.Tables.ITEM_INFO.ITEM_ID].ToString().Trim();
                    if (ids.Contains(dt.Rows[i][clsConstants.Tables.ITEM_INFO.ITEM_ID]))
                        dt.Rows.Remove(dt.Rows[i]);
                    else
                        ids.Add(dt.Rows[i][clsConstants.Tables.ITEM_INFO.ITEM_ID].ToString());
                }

            }
            catch (Exception ex)
            {

            }

        }

        public static void Separate(DataTable dt, out DataTable dtMain, out DataTable dtDetail, out string msg)
        {
            try
            {
                msg = string.Empty;
                dtMain = clsConstants.Tables.ORDER_LIST.BuildTable();
                dtDetail = clsConstants.Tables.ORDER_DETAIL.BuildTable();

                dtMain.Columns.Add(new DataColumn(clsConstants.VALUES.LAST_MOD_DT, typeof(string)));
                dtMain.Columns.Add(new DataColumn(clsConstants.VALUES.LAST_MOD_ST, typeof(string)));
                dtMain.Columns.Add(new DataColumn(clsConstants.VALUES.LAST_MOD_BY, typeof(string)));

                dtDetail.Columns.Add(new DataColumn(clsConstants.VALUES.LAST_MOD_DT, typeof(string)));
                dtDetail.Columns.Add(new DataColumn(clsConstants.VALUES.LAST_MOD_ST, typeof(string)));
                dtDetail.Columns.Add(new DataColumn(clsConstants.VALUES.LAST_MOD_BY, typeof(string)));

                DataRow[] drs = (from DataRow dr in dt.Rows
                                 where dr[clsConstants.Tables.ORDER_LIST.ORDER_ID] != DBNull.Value && dr[clsConstants.Tables.ORDER_LIST.ORDER_ID].ToString().Length > 0
                                 orderby clsConstants.Tables.ORDER_LIST.ORDER_ID
                                 select dr).ToArray();

                foreach (DataRow dr in drs)
                {
                    dr[clsConstants.Tables.ORDER_LIST.ORDER_ID] = dr[clsConstants.Tables.ORDER_LIST.ORDER_ID].ToString().StartsWith("#") ? dr[clsConstants.Tables.ORDER_LIST.ORDER_ID].ToString().Substring(1, dr[clsConstants.Tables.ORDER_LIST.ORDER_ID].ToString().Length - 1) :
                                        dr[clsConstants.Tables.ORDER_LIST.ORDER_ID].ToString();
                    DataRow drNew = dtMain.NewRow();
                    if (dtMain.Select(clsConstants.Tables.ORDER_LIST.ORDER_ID + " = '" + dr[clsConstants.Tables.ORDER_LIST.ORDER_ID].ToString() + "'").Count() == 0)
                    {
                        drNew[clsConstants.Tables.ORDER_LIST.ORDER_ID] = dr[clsConstants.Tables.ORDER_LIST.ORDER_ID];
                        if (double.TryParse(dr[clsConstants.Tables.ORDER_LIST.ORDER_DATE].ToString(), out double excelDate))
                            drNew[clsConstants.Tables.ORDER_LIST.ORDER_DATE] = Value_util.GetExcelDateTime(excelDate);
                        else
                            drNew[clsConstants.Tables.ORDER_LIST.ORDER_DATE] = Value_util.GetExcelSlashDateTime(dr[clsConstants.Tables.ORDER_LIST.ORDER_DATE].ToString());
                        if (string.IsNullOrEmpty(drNew[clsConstants.Tables.ORDER_LIST.ORDER_DATE].ToString()))
                            throw new Exception("有資料的日期不大正確");

                        drNew[clsConstants.Tables.ORDER_LIST.RECIPIENT] = dr[clsConstants.Tables.ORDER_LIST.RECIPIENT];
                        drNew[clsConstants.Tables.ORDER_LIST.TEL] = dr[clsConstants.Tables.ORDER_LIST.TEL];
                        drNew[clsConstants.Tables.ORDER_LIST.SHIPPING_ID] = dr[clsConstants.Tables.ORDER_LIST.SHIPPING_ID];
                        drNew[clsConstants.Tables.ORDER_LIST.TOTAL_AMT] = dr[clsConstants.Tables.ORDER_LIST.TOTAL_AMT];
                        drNew[clsConstants.Tables.ORDER_LIST.SHIPPING_FEE] = dr[clsConstants.Tables.ORDER_LIST.SHIPPING_FEE];
                        drNew[clsConstants.Tables.ORDER_LIST.ATTACH_FEE] = dr[clsConstants.Tables.ORDER_LIST.ATTACH_FEE];
                        drNew[clsConstants.Tables.ORDER_LIST.DISCOUNT] = dr[clsConstants.Tables.ORDER_LIST.DISCOUNT];
                        drNew[clsConstants.Tables.ORDER_LIST.ADDRESS] = dr[clsConstants.Tables.ORDER_LIST.ADDRESS];
                        drNew[clsConstants.Tables.ORDER_LIST.REMARKS] = dr[clsConstants.Tables.ORDER_LIST.REMARKS];
                        drNew[clsConstants.Tables.ORDER_LIST.RECEIVE_SHOP] = dr[clsConstants.Tables.ORDER_LIST.RECEIVE_SHOP];
                        drNew[clsConstants.Tables.ORDER_LIST.SHIPPING_METHOD] =  dr[clsConstants.Tables.ORDER_LIST.SHIPPING_METHOD];
                        drNew[clsConstants.Tables.ORDER_LIST.STATUS] = clsConstants.VALUES.SYS_CODESET.ORDER_STATUS.UNDONE;
                        dtMain.Rows.Add(drNew);
                    }
                    if (dtDetail.Select(clsConstants.Tables.ORDER_DETAIL.ORDER_ID + 
                        " = '" + dr[clsConstants.Tables.ORDER_DETAIL.ORDER_ID].ToString() + "' AND " +
                        clsConstants.Tables.ORDER_DETAIL.ITEM_ID +
                        " = '" + dr[clsConstants.Tables.ORDER_DETAIL.ITEM_ID].ToString() + "'").Count() > 0)
                    {
                        throw new Exception("貨號在同一筆訂單裡重複, 請不要降");
                    }

                    DataRow drDetailNew = dtDetail.NewRow();
                    drDetailNew[clsConstants.Tables.ORDER_DETAIL.ORDER_ID] = dr[clsConstants.Tables.ORDER_DETAIL.ORDER_ID];
                    drDetailNew[clsConstants.Tables.ORDER_DETAIL.ITEM_ID] = dr[clsConstants.Tables.ORDER_DETAIL.ITEM_ID];
                    drDetailNew[clsConstants.Tables.ORDER_DETAIL.ITEM_NAME] = dr[clsConstants.Tables.ORDER_DETAIL.ITEM_NAME];
                    drDetailNew[clsConstants.Tables.ORDER_DETAIL.ITEM_OPTION] = dr[clsConstants.Tables.ORDER_DETAIL.ITEM_OPTION];
                    drDetailNew[clsConstants.Tables.ORDER_DETAIL.ITEM_QUANT] = dr[clsConstants.Tables.ORDER_DETAIL.ITEM_QUANT];
                    drDetailNew[clsConstants.Tables.ORDER_DETAIL.PRICE_ORIGINAL] = dr[clsConstants.Tables.ORDER_DETAIL.PRICE_ORIGINAL];
                    drDetailNew[clsConstants.Tables.ORDER_DETAIL.PRICE_ON_SALE] = dr[clsConstants.Tables.ORDER_DETAIL.PRICE_ON_SALE];

                    dtDetail.Rows.Add(drDetailNew);
                }

            }
            catch (Exception ex)
            {
                msg = ex.Message;
                dtMain = null;
                dtDetail = null;
            }

        }


    }
}

