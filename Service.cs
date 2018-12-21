using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Security.Cryptography;
using CommonResources;

namespace Server
{
    public class Service
    {
        public static class SQLString
        {
            public const string CheckItemClean = "SELECT 1 FROM ITEM_INFO WHERE item_id = '{0}' ";

            public const string VendorList = "select vendor_code, vendor_name from VENDOR_INFO ";
            public const string CheckVendorName = "SELECT 1 FROM VENDOR_INFO WHERE vendor_code != '{0}' AND vendor_name = N'{1}' ";
            public const string GetVendor = "SELECT TOP 1 [vendor_code], [vendor_name], [tel], [address], [url], [contact_name] FROM VENDOR_INFO WHERE vendor_code = '{0}' ";
            
            public const string InsertVendor = "INSERT INTO VENDOR_INFO " +
                                               " ([vendor_code], [vendor_name], [tel], [address], [url], [contact_name]) " +
                                               " SELECT N'{0}', N'{1}', N'{2}', N'{3}', N'{4}', N'{5}' ";
            public const string UpdateVendor = "UPDATE VENDOR_INFO " +
                                               " SET vendor_name = N'{1}', tel = N'{2}', address = N'{3}', " +
                                               " url = N'{4}', contact_name = N'{5}' WHERE vendor_code = '{0}'";

            public const string UpdateItemId = ";UPDATE ITEM_INFO SET item_id = '{1}'  WHERE item_id = '{0}'" +
                                               ";UPDATE ORDER_DETAIL SET item_id = '{1}'  WHERE item_id = '{0}' " +
                                               ";UPDATE ITEM_OPTION SET item_id = '{1}'  WHERE item_id = '{0}' " +
                                               ";UPDATE RECEIVE_HISTORY SET item_id = '{1}'  WHERE item_id = '{0}' " +
                                               ";UPDATE STOCKTAKE_HISTORY SET item_id = '{1}'  WHERE item_id = '{0}' " +
                                               ";UPDATE RETURN_HISTORY SET item_id = '{1}'  WHERE item_id = '{0}' " +
                                               ";UPDATE RETURN_VENDOR_HISTORY SET item_id = '{1}'  WHERE item_id = '{0}' ";

            public const string ShowQueueQuery = "select [order_id], [order_date], [recipient], [tel], [shipping_id], [shipping_method], " +
                                                 " [status] from ORDER_LIST WHERE status IN ('U', 'P') ";
            public const string ShowQueryOrder = "select a.[order_id], a.[order_date], a.[recipient], a.[tel], a.[shipping_id], a.[shipping_method], " +
                                                 " a.[status], b.[item_id], c.[item_name], a.[pick_dt], b.[item_quant] from ORDER_LIST a JOIN ORDER_DETAIL b on a.order_id=b.order_id JOIN ITEM_INFO c on b.item_id = c.item_id " +
                                                 " where a.order_id IN (select c.order_id from ORDER_LIST c join ORDER_DETAIL d on c.order_id= d.order_id join ITEM_INFO e on d.item_id= e.item_id "+
                                                 " where (LEN('{0}') = 0 OR c.order_id = '{0}') AND(LEN('{1}') = 0 OR c.order_date >= '{1}') AND(LEN('{2}') = 0 OR c.order_date <= '{2}') AND(LEN('{3}') = 0 OR c.recipient = '{3}') AND " +
                                                 " (LEN('{4}') = 0 OR c.tel = '{4}') AND(LEN('{5}') = 0 OR c.shipping_id = '{5}') AND(LEN('{6}') = 0 OR c.shipping_method = '{6}') AND(LEN('{7}') = 0 OR c.status = '{7}')" +
                                                 " AND(LEN('{8}') = 0 OR d.item_id = '{8}') AND(LEN('{9}') = 0 OR e.barcode = '{9}') AND(LEN('{10}') = 0 OR e.item_name = N'{10}')) ORDER BY a.order_id  ";
            public const string ShowQueryInv = " select [item_id], [item_name], [item_location], [barcode], [inv], [item_mass_location], b.code_desc AS [status] " +
                                               " from ITEM_INFO a join SYS_CODESET b on a.status = b.code_value AND b.code_type = 'item_sys'" +
                                               " WHERE (LEN('{0}') = 0 OR item_id = '{0}') AND " +
                                               " (LEN('{1}') = 0 OR barcode = '{1}') AND " +
                                               " (LEN('{2}') = 0 OR item_location = '{2}') AND " +
                                               " (LEN('{3}') = 0 OR status = '{3}') AND " +
                                               " (LEN('{4}') = 0 OR item_name LIKE N'%{4}%') AND " +
                                               " (LEN('{6}') = 0 OR '{6}' < 0 OR CAST(inv AS CHAR) {5} '{6}') ";
            public const string RptInOut = " ;WITH cte1 AS (SELECT  st, date, item_id, item_name, id, quant, type, remarks from " +
                                               " (select 0 as st, a.pick_dt as date, b.item_id, b.item_name, a.order_id as id, -1 * b.item_quant as quant, N'出庫' as type, " +
                                               " a.remarks, 'OUT' AS t from ORDER_LIST a join ORDER_DETAIL b on a.order_id = b.order_id " +
                                               "  UNION ALL " +
                                               " select 0 as st, a.receive_dt as date, c.item_id, c.item_name, b.receive_no as id, a.quant as quant, N'入庫' as type, " +
                                               " b.remarks, 'IN' AS t from RECEIVE_HISTORY a join ITEM_INFO c on a.item_id = c.item_id " +
                                               " join RECEIVE_INFO b on a.receive_dt = b.receive_dt " +
                                               "  UNION ALL " +
                                               " select 0 as st, a.return_dt as date, c.item_id, c.item_name, b.order_id as id, a.quant as quant, N'可再銷售退貨入庫' as type, " +
                                               " b.remarks, 'IN' AS t from RETURN_HISTORY a join ITEM_INFO c on a.item_id = c.item_id " +
                                               " join RETURN_INFO b on a.return_dt = b.return_dt WHERE ISNULL(a.quant,0) > 0 " +
                                               "  UNION ALL " +
                                               " select 0 as st, a.return_dt as date, c.item_id, c.item_name, b.order_id as id, a.broken_quant as quant, N'不可再銷售退貨入庫' as type, " +
                                               " b.remarks, 'IN' AS t from RETURN_HISTORY a join ITEM_INFO c on a.item_id = c.item_id " +
                                               " join RETURN_INFO b on a.return_dt = b.return_dt WHERE ISNULL(a.broken_quant, 0) > 0 " +
                                               "  UNION ALL " +
                                               " select 0 as st, a.return_dt as date, c.item_id, c.item_name, b.return_dt as id, a.quant as quant, N'可再銷售退廠商出庫' as type, " +
                                               " b.remarks, 'OUT' AS t from RETURN_VENDOR_HISTORY a join ITEM_INFO c on a.item_id = c.item_id " +
                                               " join RETURN_VENDOR_INFO b on a.return_dt = b.return_dt WHERE ISNULL(a.quant, 0) > 0 " +
                                               "  UNION ALL " +
                                               " select 0 as st, a.return_dt as date, c.item_id, c.item_name, b.return_dt as id, a.broken_quant as quant, N'不可再銷售退廠商出庫' as type, " +
                                               " b.remarks, 'OUT' AS t from RETURN_VENDOR_HISTORY a join ITEM_INFO c on a.item_id = c.item_id " +
                                               " join RETURN_VENDOR_INFO b on a.return_dt = b.return_dt WHERE ISNULL(a.broken_quant, 0) > 0 " +
                                               " UNION ALL" +
                                               " select 1 as st, stocktake_dt as date, a.item_id, b.item_name, stocktake_dt as id, quant_after as quant, N'盤點' as type, " + 
                                               " '' as remarks, '' as t from STOCKTAKE_HISTORY a join item_info b on a.item_id=b.item_id " +
                                               " ) AS UA " +
                                               " WHERE (LEN('{0}') = 0 OR id = '{0}') AND " +
                                               " (LEN('{1}') = 0 OR item_id = '{1}') AND " +
                                               " (LEN('{2}') = 0 OR item_name LIKE N'%{2}%') AND " +
                                               " (LEN('{3}') = 0 OR date >= '{3}') AND " +
                                               " (LEN('{4}') = 0 OR date <= '{4}') AND " +
                                               " ('{5}' = 'ALL' OR t = '{5}') AND " +
                                               " (LEN('{6}') = 0 OR remarks LIKE N'%{6}%') " +
                                               " ), cte2 AS (SELECT date, item_id, item_name, id, quant, type, remarks, SUM(st) OVER (PARTITION BY item_id order by date) AS st" +
                                               "  FROM cte1 ) " +
                                               " SELECT SUBSTRING(date,1,4) + '-' + SUBSTRING(date,5,2) + '-' + SUBSTRING(date,7,2) as dt_dis, item_id" +
                                               "   item_name, id, quant, type, remarks, CASE WHEN st > 0  THEN " +
                                               "   CAST(sum(quant * CASE WHEN st > 0 THEN 1 ELSE 0 END) OVER (PARTITION BY item_id, st order by date) AS VARCHAR)" +
                                               "   ELSE '???' END AS bal FROM cte2" +
                                               " ORDER BY date, item_id ";
            
            public const string ShowPickHistory = " select distinct top {0} SUBSTRING(pick_dt,1,4) + '-' + SUBSTRING(pick_dt,5,2) + '-' + " +
                                    " SUBSTRING(pick_dt, 7, 2) + ' ' + SUBSTRING(pick_dt, 9, 2) + ':' + " +
                                    " SUBSTRING(pick_dt, 11, 2) + ':' + SUBSTRING(pick_dt, 13, 2) AS pick_dt_dis, pick_dt, SUM(1) OVER (PARTITION BY pick_dt) as num  from PICKING_HISTORY " +
                                                  " ORDER BY pick_dt DESC ";

            public const string ShowReceiveHistory = " select distinct top {0} SUBSTRING(a.receive_dt,1,4) + '-' + SUBSTRING(a.receive_dt,5,2) + '-' + " +
                                                     " SUBSTRING(a.receive_dt, 7, 2) + ' ' + SUBSTRING(a.receive_dt, 9, 2) + ':' + " +
                                                     " SUBSTRING(a.receive_dt, 11, 2) + ':' + SUBSTRING(a.receive_dt, 13, 2) AS receive_dt_dis, " +
                                                     " a.receive_dt, SUM(a.quant) OVER(PARTITION BY a.receive_dt) as num, b.receive_no, b.remarks from receive_history a " +
                                                     " JOIN RECEIVE_INFO b on a.receive_dt = b.receive_dt" +
                                                     " ORDER BY a.receive_dt DESC";

            public const string InsertReceiveInfo = " INSERT INTO RECEIVE_INFO" +
                                                    " ([receive_dt], [receive_no], [remarks], [vendor_code])" +
                                                    " select '{0}', '{1}', N'{2}', '{3}';";

            public const string InsertReturnVendorInfo = " INSERT INTO RETURN_VENDOR_INFO" +
                                                   " ([return_dt], [remarks], [vendor_code])" +
                                                   " select '{0}', N'{1}', N'{2}';";
            
            public const string ShowStockTakeHistory = " select distinct top {0} SUBSTRING(stocktake_dt,1,4) + '-' + SUBSTRING(stocktake_dt,5,2) + '-' + " +
                                                     " SUBSTRING(stocktake_dt, 7, 2) + ' ' + SUBSTRING(stocktake_dt, 9, 2) + ':' + " +
                                                     " SUBSTRING(stocktake_dt, 11, 2) + ':' + SUBSTRING(stocktake_dt, 13, 2) AS stocktake_dt_dis, " +
                                                     " stocktake_dt, SUM(1) OVER(PARTITION BY stocktake_dt) as num, " +
                                                     "  SUM(quant_after) OVER(PARTITION BY stocktake_dt) as num_all from stocktake_history " +
                                                     " ORDER BY stocktake_dt DESC";

            public const string GetSysCodeSet = " SELECT code_type, code_value, code_desc FROM SYS_CODESET " +
                                                " WHERE code_type = '{0}' AND (LEN('{1}') = 0 OR code_value = '{1}') ";

            public const string InsertNewPick = 
                " INSERT INTO PICKING_HISTORY" +
                " ([pick_dt], [order_id]) " +
                " SELECT '{0}', order_id " +
                " FROM ORDER_LIST " +
                " WHERE status = 'U'";

            public const string UpdateOrderTempLocation =
                " UPDATE f " +
                " SET f.temp_location = " +
                " g.temp_location FROM ORDER_DETAIL as f JOIN " +
                 " (SELECT a.order_id, a.item_id, " +
                 " (DENSE_RANK() OVER(ORDER BY c.item_location, a.item_id) + CAST(d.code_value AS INT) - 1) % CAST(e.code_value AS INT) + 1 AS temp_location " +
                 " from ORDER_DETAIL AS a " +
                 " LEFT JOIN ITEM_INFO AS c on a.item_id = c.item_id " +
                 " JOIN SYS_CODESET AS d on d.code_type = 'temp_location' " +
                 " JOIN SYS_CODESET AS e on e.code_type = 'temp_location_total' " +
                 " JOIN PICKING_HISTORY AS p ON p.order_id = a.order_id" +
                 " WHERE p.pick_dt = '{0}' ) AS g " +
                 " ON f.order_id = g.order_id and f.item_id = g.item_id";
            
            public const string StocktakeList = "select SUBSTRING(stocktake_dt,1,4) + '-' + SUBSTRING(stocktake_dt,5,2) + '-' +" +
                         " SUBSTRING(stocktake_dt, 7, 2) + ' ' + SUBSTRING(stocktake_dt, 9, 2) + ':' + " +
                         " SUBSTRING(stocktake_dt, 11, 2) + ':' + SUBSTRING(stocktake_dt, 13, 2) AS stocktake_dt_dis, a.item_id, b.item_name, b.barcode, b.item_location,a.quant_before , a.quant_after " +
                         " from STOCKTAKE_HISTORY a join ITEM_INFO b on a.item_id=b.item_id where a.stocktake_dt = '{0}'";

            public const string ReceiveList = "select SUBSTRING(a.receive_dt,1,4) + '-' + SUBSTRING(a.receive_dt,5,2) + '-' +" +
                         " SUBSTRING(a.receive_dt, 7, 2) + ' ' + SUBSTRING(a.receive_dt, 9, 2) + ':' + " +
                         " SUBSTRING(a.receive_dt, 11, 2) + ':' + SUBSTRING(a.receive_dt, 13, 2) AS receive_dt_dis, c.receive_no, c.remarks, a.item_id, b.item_name, b.barcode, b.item_location, a.quant AS item_quant from" +
                         " RECEIVE_HISTORY a join ITEM_INFO b on a.item_id=b.item_id JOIN RECEIVE_INFO c on a.receive_dt = c.receive_dt where a.receive_dt = '{0}'";

            public const string PickList =
                " select a.item_id, sum(a.item_quant) as item_quant, ISNULL(c.item_name, '') AS item_name " +
                " , ISNULL(c.item_location, '') + " +
                " CASE WHEN LEN(ISNULL(c.item_mass_location, '')) > 0 THEN N' 剩餘貨量在 ' + ISNULL(c.item_mass_location, '') " +
                "      ELSE '' END AS item_location, ISNULL(c.barcode, '') AS barcode, " +
                " a.temp_location " +
                " from ORDER_DETAIL AS a JOIN PICKING_HISTORY AS b on a.order_id = b.order_id " +
                " LEFT JOIN ITEM_INFO AS c on a.item_id = c.item_id " +
                " where b.pick_dt = '{0}' " +
                " GROUP BY a.item_id, c.item_name, c.item_location, c.barcode, a.temp_location, c.item_mass_location order by c.item_location, a.item_id ";

            public const string OrderList = " SELECT a.order_id, SUBSTRING(a.order_date,1,4) + '-' + SUBSTRING(a.order_date,5,2) + '-' + " +
                                    " SUBSTRING(a.order_date, 7, 2) + ' ' + SUBSTRING(a.order_date, 9, 2) + ':' + " +
                                    " SUBSTRING(a.order_date, 11, 2) + ':' + SUBSTRING(a.order_date, 13, 2) AS order_date ,a.recipient, " +
                                    " d.code_desc AS shipping_method, remarks " +
                                     " , a.receive_shop, a.address, CONVERT(VARBINARY(MAX), a.order_id) AS barcode," +
                                     " b.item_id, b.item_name, b.item_quant, b.temp_location" +
                                     "  FROM " +
                                     " ORDER_LIST a JOIN ORDER_DETAIL b on a.order_id = b.order_id " +
                                     " JOIN PICKING_HISTORY c ON a.order_id = c.order_id " +
                                    "  JOIN SYS_CODESET d ON a.shipping_method = d.code_value AND d.code_type = 'shipping_method' " +
                                    " WHERE c.pick_dt = '{0}' " +
                                     " ORDER BY a.shipping_method, a.order_date";

            public const string UpdateMaxTempLocation = "update a set a.code_value = (CAST(a.code_value AS INT) + CAST('{0}' AS INT) - 1) % CAST(b.code_value AS INT) + 1 " +
                                    "FROM SYS_CODESET a JOIN SYS_CODESET b ON 1=1 where a.code_type = 'temp_location' AND b.code_type = 'temp_location_total'";

            public const string UpdateOrderStatusToPick = "update ORDER_LIST SET status = 'P' FROM ORDER_LIST JOIN PICKING_HISTORY" +
                                                          " ON ORDER_LIST.order_id = PICKING_HISTORY.order_id " +
                                                          " WHERE PICKING_HISTORY.pick_dt = '{0}' ";

            public const string GetOrderDetails = "select a.[order_id], a.[item_id], ISNULL(c.[barcode],'') AS barcode, a.[item_name], a.[item_option], " +
                              " a.[item_quant], CAST(0 AS INT) AS [current_quant], a.[temp_location], " +
                              " CAST(0 AS BIT) As [complete] from ORDER_DETAIL a JOIN ORDER_LIST b" +
                              " ON a.order_id = b.order_id " +
                              " LEFT JOIN ITEM_INFO c " +
                              " ON a.item_id = c.item_id where b.status IN ('P', 'U') AND CASE WHEN b.shipping_method = 'B' THEN b.shipping_id " +
                              " ELSE SUBSTRING(b.shipping_id, LEN(b.shipping_id) - 8 + 1, 8) END = CASE WHEN b.shipping_method = 'B' THEN '{0}' ELSE SUBSTRING('{0}', LEN('{0}') - 8 + 1, 8) END";

            public const string GetOrderDetailsForReturn = 
                              " select a.[order_id], a.[item_id], ISNULL(c.[barcode],'') AS barcode, a.[item_name], a.[item_option], " +
                              " a.[item_quant], ISNULL(a.[return_quant], 0) AS return_quant " +
                              " from ORDER_DETAIL a JOIN ORDER_LIST b" +
                              " ON a.order_id = b.order_id " +
                              " LEFT JOIN ITEM_INFO c " +
                              " ON a.item_id = c.item_id where CASE WHEN b.shipping_method = 'B' THEN b.shipping_id " +
                              " ELSE SUBSTRING(b.shipping_id, LEN(b.shipping_id) - 8 + 1, 8) END = " +
                              " CASE WHEN b.shipping_method = 'B' THEN '{0}' ELSE SUBSTRING('{0}', LEN('{0}') - 8 + 1, 8) END";

            public const string GetOrderDetailsOrOrderId = " OR (b.order_id = '{0}')";

            public const string GetOrderMain = "select top 1 order_id, status, order_date, remarks, ISNULL(return_complete, 0) AS return_complete from ORDER_LIST WHERE " +
                "  CASE WHEN shipping_method = 'B' THEN shipping_id " +
                              " ELSE SUBSTRING(shipping_id, LEN(shipping_id) - 8 + 1, 8) END = CASE WHEN shipping_method = 'B' THEN '{0}' ELSE SUBSTRING('{0}', LEN('{0}') - 8 + 1, 8) END ";

            public const string GetOrderMainAndStatus = " AND status IN ('P', 'U') ";

            public const string GetOrderMainOrOrderId = " OR (order_id = '{0}')";

            public const string GetItem = " SELECT item_id, item_name, " +
                                          " SUBSTRING(item_location, 1, 2) + '-' + SUBSTRING(item_location, 3, 2) + '-' + " +
                                          " SUBSTRING(item_location, 5, 2) AS item_location, barcode, '1' AS current_quant, inv FROM" +
                                          " ITEM_INFO where barcode = '{0}' ";

            public const string GetItemById = " SELECT [item_id], [item_name], [item_location], [item_mass_location], [barcode], [last_mod_by], [last_mod_dt], [last_mod_st], [inv], [status] " +
                                              " FROM" +
                                              " ITEM_INFO where item_id = '{0}' ";
            
            public const string IsItemExisted = " SELECT 1 FROM" +
                                                " ITEM_INFO where item_id = '{0}' OR barcode = '{1}' ";

            public const string InsertItem = " INSERT INTO " +
                                             " ITEM_INFO ([item_id], [item_name], [item_location], [item_mass_location], [barcode], [last_mod_by], [last_mod_dt], [last_mod_st], [inv], [status]) " +
                                             " SELECT N'{0}', N'{1}', N'{2}', N'{3}', N'{4}', N'{5}', N'{6}', N'{7}', N'{8}', N'{9}'";
            public const string UpdateItem = " UPDATE ITEM_INFO" +
                                             " SET [item_name] = N'{1}', [item_location] = N'{2}', [item_mass_location] = N'{3}', " +
                                             " [barcode] = N'{4}', [last_mod_by] = N'{5}', [last_mod_dt] = N'{6}', " +
                                             " [last_mod_st] = N'{7}', [inv] = N'{8}', [status] = N'{9}' " +
                                             " WHERE item_id = N'{0}'";

            public const string GetPW = "SELECT 1 FROM SYS_CODESET WHERE code_type = '{0}' AND code_desc = 'ora' and code_value = '{1}' ";

            public const string UpdateOrderStatus = "UPDATE ORDER_LIST SET status = '{0}' WHERE order_id = '{1}'  OR CASE WHEN shipping_method = 'B' THEN shipping_id " +
                              " ELSE SUBSTRING(shipping_id, LEN(shipping_id) - 8 + 1, 8) END = CASE WHEN shipping_method = 'B' THEN '{1}' ELSE SUBSTRING('{1}', LEN('{1}') - 8 + 1, 8) END  ";

            public const string UpdateReturnItemInfo = " UPDATE ITEM_INFO SET inv = ISNULL(inv, 0) + {1} ," +
                                                       " broken_count = ISNULL(broken_count, 0) + {2} " +
                                                       " WHERE item_id = '{0}';";

            public const string UpdateReturnOrderDetail = " UPDATE ORDER_DETAIL SET return_quant  = ISNULL(return_quant, 0) + {2} " +
                                                          " WHERE order_id = '{0}' AND item_id = '{1}' ;";

            public const string UpdateReturnOrderInfo = " UPDATE a SET a.return_complete = 1 FROM ORDER_LIST a JOIN" +
                                                        " (SELECT order_id, SUM(item_quant) as i, SUM(ISNULL(return_quant,0)) as r " +
                                                        " FROM ORDER_DETAIL WHERE order_id = '{0}' GROUP BY order_id) b " +
                                                        " on a.order_id = b.order_id WHERE b.i=b.r ";

            public const string CheckReturnQuantMain = "SELECT 1 FROM ORDER_DETAIL WHERE 1=2 ";
            public const string CheckReturnQuantSub = " OR (order_id = '{0}' AND item_id = '{1}' AND " +
                                                   " ISNULL(return_quant, 0) + {2} > item_quant)";

            public const string ReturnOrderList = "UPDATE ORDER_LIST SET status = '{0}', return_reason = " +
                                                  " ISNULL(return_reason, '') + N'{2}' + '|' " +
                                                  " WHERE order_id = '{1}'  OR CASE WHEN shipping_method = 'B' THEN shipping_id " +
                              " ELSE SUBSTRING(shipping_id, LEN(shipping_id) - 8 + 1, 8) END = CASE WHEN shipping_method = 'B' THEN '{1}' ELSE SUBSTRING('{1}', LEN('{1}') - 8 + 1, 8) END  ";

            public const string ReturnInfo = " INSERT INTO RETURN_INFO " +
                                             " ([return_dt], [order_id], [remarks]) " +
                                             " SELECT '{0}', '{1}', N'{2}'";
            public const string ReturnHistoryBase = " INSERT INTO RETURN_HISTORY " +
                                             " ([return_dt], [item_id], [quant], [broken_quant]) ";
            public const string ReturnHistoryUnit = " SELECT '{0}', '{1}', {2}, {3} UNION ALL ";

            public const string MinusInventory = " UPDATE i SET i.inv = ISNULL(i.inv, 0) - d.item_quant" +
                                                 " FROM ITEM_INFO AS i JOIN ORDER_DETAIL AS d " +
                                                 " ON i.item_id = d.item_id WHERE d.order_id = '{0}'";
            public const string CheckOrderExistsAndPend = " SELECT CASE WHEN status IN ('P', 'U') THEN 1 " +
                                                          " WHEN status IN ('C') THEN 2 " +
                                                          " WHEN status IN ('A') THEN 3 " +
                                                          " WHEN status IN ('R') THEN 4 ELSE 5 END FROM ORDER_LIST WHERE order_id = '{0}'";

            public const string AddBackInventory = " UPDATE i SET i.inv = ISNULL(i.inv, 0) + d.item_quant" +
                                                 " FROM ITEM_INFO AS i JOIN ORDER_DETAIL AS d JOIN ORDER_LIST e ON d.order_id = e.order_id " +
                                                 " ON i.item_id = d.item_id WHERE d.order_id = '{0}' AND e.status = 'C'";

            public const string UpdatePickDt = "UPDATE ORDER_LIST SET pick_dt = format(getdate(),'yyyyMMddHHmmssf') WHERE order_id = '{0}'  ";

            public const string InitializeForImportOrder = " IF OBJECT_ID(\'tempdb..#ORDER_LIST\') IS NOT NULL DROP TABLE #ORDER_LIST; " +
                                                     " CREATE TABLE #ORDER_LIST ([order_id] varchar(50), [order_date] varchar(15), " +
                                       "[recipient] nvarchar(200), [tel] varchar(50), [shipping_id] varchar(50), " +
                                       "[shipping_fee] decimal(20,2), [attach_fee] decimal(20,2), [total_amt] decimal(20,2), " +
                                       " receive_shop nvarchar(200), address nvarchar(500), remarks nvarchar(500), return_reason NVARCHAR(max), " +
                                       " return_complete BIT, [discount] decimal(20,2), [shipping_method] nvarchar(500), " +
                                       "[last_mod_by] varchar(20), [last_mod_dt] varchar(15), [last_mod_st] varchar(2), [status] varchar(10) " +
                                       ",[pick_dt] varchar(15), [ship_dt] varchar(15), [return_dt] varchar(15));" +
                                       " IF OBJECT_ID(\'tempdb..#ORDER_DETAIL\') IS NOT NULL DROP TABLE #ORDER_DETAIL;" +
                                      " CREATE TABLE #ORDER_DETAIL ([order_id] varchar(50), [item_id] varchar(50), " +
                                   "[item_option] nvarchar(200), [item_quant] smallint, [price_original] decimal(20,2), " +
                                   "[price_on_sale] decimal(20,2), [last_mod_by] varchar(20), [last_mod_dt] varchar(15), " +
                                   "[last_mod_st] varchar(2), [item_name] nvarchar(200), temp_location varchar(5), return_quant SMALLINT); ";

            public const string InitializeForImportItem = " IF OBJECT_ID(\'tempdb..#ITEM_INFO\') IS NOT NULL DROP TABLE #ITEM_INFO; " +
                                       " CREATE TABLE #ITEM_INFO (item_id varchar(50), item_name nvarchar(200), " +
                                       "item_location NVARCHAR(50), item_mass_location NVARCHAR(50), barcode varchar(50), inv INT,last_mod_by varchar(20), " +
                                       "last_mod_dt varchar(15), last_mod_st varchar(2));";

            public const string InitializeForReceiveItem = " IF OBJECT_ID(\'tempdb..#ITEM_INFO\') IS NOT NULL DROP TABLE #ITEM_INFO; " +
                                       " CREATE TABLE #ITEM_INFO (item_id varchar(50), item_name NVARCHAR(500)," +
                                       " barcode VARCHAR(100), item_location VARCHAR(20), current_quant varchar(100));";

            public const string InitializeForStockTake = " IF OBJECT_ID(\'tempdb..#ITEM_INFO\') IS NOT NULL DROP TABLE #ITEM_INFO; " +
                                       " CREATE TABLE #ITEM_INFO (item_id varchar(50), item_name NVARCHAR(500)," +
                                       " barcode VARCHAR(100), item_location VARCHAR(20), inv varchar(10), current_quant varchar(100));";

        }
        private readonly string connectionString = svr_const.connetion_string;
        
        public DataTable ShowQueue()
        {
            DataTable dtOrderList = new DataTable(clsConstants.Tables.ORDER_LIST.TABLE_NAME);
            dtOrderList.Columns.Add(new DataColumn(clsConstants.Tables.ORDER_LIST.ORDER_ID, typeof(string)));
            dtOrderList.Columns.Add(new DataColumn(clsConstants.Tables.ORDER_LIST.ORDER_DATE, typeof(string)));
            dtOrderList.Columns.Add(new DataColumn(clsConstants.Tables.ORDER_LIST.RECIPIENT, typeof(string)));
            dtOrderList.Columns.Add(new DataColumn(clsConstants.Tables.ORDER_LIST.TEL, typeof(string)));
            dtOrderList.Columns.Add(new DataColumn(clsConstants.Tables.ORDER_LIST.SHIPPING_ID, typeof(string)));
            dtOrderList.Columns.Add(new DataColumn(clsConstants.Tables.ORDER_LIST.SHIPPING_METHOD, typeof(string)));
            dtOrderList.Columns.Add(new DataColumn(clsConstants.Tables.ORDER_LIST.STATUS, typeof(string)));

            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                SqlDataReader myReader = null;
                SqlCommand myCommand = new SqlCommand(SQLString.ShowQueueQuery, connection);
                myReader = myCommand.ExecuteReader();

                dtOrderList.Load(myReader);

            }
            catch (Exception ex)
            {
               
            }
            finally
            {
                connection.Close();
            }
            return dtOrderList;
        }
        
        public DataTable QueryOrder(string order_id, string date_from, string date_to, string recipient, string tel, string shipping_no,
                                    string shipping_method, string status, string item_id, string barcode, string item_name)
        {
            order_id = order_id.Replace("'", "''");
            date_from = date_from.Replace("'", "''");
            date_to = date_to.Replace("'", "''");
            recipient = recipient.Replace("'", "''");
            tel = tel.Replace("'", "''");
            shipping_no = shipping_no.Replace("'", "''");
            shipping_method = shipping_method.Replace("'", "''");
            status = status.Replace("'", "''");
            item_id = item_id.Replace("'", "''");
            barcode = barcode.Replace("'", "''");
            item_name = item_name.Replace("'", "''");

            DataTable dtOrderList = new DataTable(clsConstants.Tables.ORDER_LIST.TABLE_NAME);
            dtOrderList.Columns.Add(new DataColumn(clsConstants.Tables.ORDER_LIST.ORDER_ID, typeof(string)));
            dtOrderList.Columns.Add(new DataColumn(clsConstants.Tables.ORDER_LIST.ORDER_DATE, typeof(string)));
            dtOrderList.Columns.Add(new DataColumn(clsConstants.Tables.ORDER_LIST.RECIPIENT, typeof(string)));
            dtOrderList.Columns.Add(new DataColumn(clsConstants.Tables.ORDER_LIST.TEL, typeof(string)));
            dtOrderList.Columns.Add(new DataColumn(clsConstants.Tables.ORDER_LIST.SHIPPING_ID, typeof(string)));
            dtOrderList.Columns.Add(new DataColumn(clsConstants.Tables.ORDER_LIST.SHIPPING_METHOD, typeof(string)));
            dtOrderList.Columns.Add(new DataColumn(clsConstants.Tables.ORDER_LIST.STATUS, typeof(string)));
            dtOrderList.Columns.Add(new DataColumn(clsConstants.Tables.ORDER_LIST.PICK_DT, typeof(string)));
            dtOrderList.Columns.Add(new DataColumn(clsConstants.Tables.ORDER_DETAIL.ITEM_ID, typeof(string)));
            dtOrderList.Columns.Add(new DataColumn(clsConstants.Tables.ORDER_DETAIL.ITEM_NAME, typeof(string)));
            dtOrderList.Columns.Add(new DataColumn(clsConstants.Tables.ORDER_DETAIL.ITEM_QUANT, typeof(string)));

            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                SqlDataReader myReader = null;
                SqlCommand myCommand = new SqlCommand(string.Format(SQLString.ShowQueryOrder, order_id, date_from, 
                                                        date_to, recipient, tel, shipping_no, shipping_method, status, 
                                                        item_id, barcode, item_name), 
                                                        connection);
                myReader = myCommand.ExecuteReader();

                dtOrderList.Load(myReader);

            }
            catch (Exception ex)
            {

            }
            finally
            {
                connection.Close();
            }
            return dtOrderList;
        }

        public DataTable QueryEmptyInv()
        {
            DataTable dtItem = clsConstants.Tables.ITEM_INFO.BuildTable();

            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                SqlDataReader myReader = null;
                SqlCommand myCommand = new SqlCommand("[ItemInfo.FindWhereEmpty]", connection);
                myReader = myCommand.ExecuteReader();

                dtItem.Load(myReader);

            }
            catch (Exception ex)
            {

            }
            finally
            {
                connection.Close();
            }
            return dtItem;
        }

        public DataTable QueryInv(string item_id, string barcode, string location, string status, string item_name,string quantCompare, string quant)
        {
            DataTable dtItem = clsConstants.Tables.ITEM_INFO.BuildTable();

            if (!string.IsNullOrEmpty(location)) location.Replace("-", "");
            item_id = item_id.Replace("'", "''");
            barcode = barcode.Replace("'", "''");
            location = location.Replace("'", "''");
            status = status.Replace("'", "''");
            item_name = item_name.Replace("'", "''");
            quant = quant.Replace("'", "''");

            if (!int.TryParse(quant, out int q))
            {
                quant = string.Empty;
            }
            else quant = q.ToString();

            switch (quantCompare)
            {
                case clsConstants.VALUES.SYS_CODESET.NUM_COMPARE.EQUAL:
                    quantCompare = " = ";
                    break;
                case clsConstants.VALUES.SYS_CODESET.NUM_COMPARE.LARGER:
                    quantCompare = " > ";
                    break;
                case clsConstants.VALUES.SYS_CODESET.NUM_COMPARE.SMALLER:
                    quantCompare = " < ";
                    break;

                default:
                    quantCompare = " = ";
                    break;
            }
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                SqlDataReader myReader = null;
                SqlCommand myCommand = new SqlCommand(string.Format(SQLString.ShowQueryInv, item_id, barcode, location, status, item_name, quantCompare, quant), connection);
                myReader = myCommand.ExecuteReader();

                dtItem.Load(myReader);

            }
            catch (Exception ex)
            {

            }
            finally
            {
                connection.Close();
            }
            return dtItem;
        }
        
        public DataTable ShowStockTakeHistory(int num = 10)
        {
            DataTable dtReceive = new DataTable("stocktake_history");

            dtReceive.Columns.Add(new DataColumn("stocktake_dt", typeof(string)));
            dtReceive.Columns.Add(new DataColumn("stocktake_dt_dis", typeof(string)));
            dtReceive.Columns.Add(new DataColumn("num", typeof(string))); 
            dtReceive.Columns.Add(new DataColumn("num_all", typeof(string)));

            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                SqlDataReader myReader = null;
                SqlCommand myCommand = new SqlCommand(string.Format(SQLString.ShowStockTakeHistory, num.ToString()), connection);
                myReader = myCommand.ExecuteReader();

                dtReceive.Load(myReader);

            }
            catch (Exception ex)
            {

            }
            finally
            {
                connection.Close();
            }
            return dtReceive;
        }

        public DataTable ShowReceiveHistory(int num = 10)
        {
            DataTable dtReceive = new DataTable("receive_history");

            dtReceive.Columns.Add(new DataColumn("receive_dt", typeof(string)));
            dtReceive.Columns.Add(new DataColumn("receive_dt_dis", typeof(string)));
            dtReceive.Columns.Add(new DataColumn("receive_no", typeof(string)));
            dtReceive.Columns.Add(new DataColumn("remarks", typeof(string)));
            dtReceive.Columns.Add(new DataColumn("num", typeof(string)));

            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                SqlDataReader myReader = null;
                SqlCommand myCommand = new SqlCommand(string.Format(SQLString.ShowReceiveHistory, num.ToString()), connection);
                myReader = myCommand.ExecuteReader();

                dtReceive.Load(myReader);

            }
            catch (Exception ex)
            {

            }
            finally
            {
                connection.Close();
            }
            return dtReceive;
        }

        public DataTable ShowPickHistory(int num = 10)
        {
            DataTable dtPick = new DataTable("pick_history");

            dtPick.Columns.Add(new DataColumn("pick_dt", typeof(string)));
            dtPick.Columns.Add(new DataColumn("num", typeof(string)));

            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                SqlDataReader myReader = null;
                SqlCommand myCommand = new SqlCommand(string.Format(SQLString.ShowPickHistory, num.ToString()), connection);
                myReader = myCommand.ExecuteReader();

                dtPick.Load(myReader);

            }
            catch (Exception ex)
            {

            }
            finally
            {
                connection.Close();
            }
            return dtPick;
        }

        public DataSet GetPickingTable(string pick_dt = "")
        {
            bool IsNew = pick_dt == "";
            if (IsNew) pick_dt = DateTime.Now.ToString(clsConstants.VALUES.DATETIME_STRING);
            DataSet dsReturn = new DataSet();
            
            DataTable dtPicking = new DataTable("pick_list");
            DataTable dtOrderList = new DataTable("order_list");

            dtPicking.Columns.Add(new DataColumn(clsConstants.Tables.ITEM_INFO.ITEM_ID, typeof(string)));
            dtPicking.Columns.Add(new DataColumn("item_quant", typeof(string)));
            dtPicking.Columns.Add(new DataColumn(clsConstants.Tables.ITEM_INFO.ITEM_NAME, typeof(string)));
            dtPicking.Columns.Add(new DataColumn(clsConstants.Tables.ITEM_INFO.ITEM_LOCATION, typeof(string)));
            dtPicking.Columns.Add(new DataColumn(clsConstants.Tables.ORDER_DETAIL.TEMP_LOCATION, typeof(string)));
            dtPicking.Columns.Add(new DataColumn(clsConstants.Tables.ITEM_INFO.BARCODE, typeof(string)));

            dtOrderList.Columns.Add(new DataColumn(clsConstants.Tables.ORDER_LIST.ORDER_ID, typeof(string)));
            dtOrderList.Columns.Add(new DataColumn(clsConstants.Tables.ORDER_LIST.ORDER_DATE, typeof(string)));
            dtOrderList.Columns.Add(new DataColumn(clsConstants.Tables.ORDER_LIST.RECIPIENT, typeof(string)));
            dtOrderList.Columns.Add(new DataColumn(clsConstants.Tables.ORDER_LIST.SHIPPING_METHOD, typeof(string)));
            dtOrderList.Columns.Add(new DataColumn(clsConstants.Tables.ORDER_LIST.RECEIVE_SHOP, typeof(string)));
            dtOrderList.Columns.Add(new DataColumn(clsConstants.Tables.ORDER_LIST.ADDRESS, typeof(string)));
            dtOrderList.Columns.Add(new DataColumn(clsConstants.Tables.ORDER_LIST.REMARKS, typeof(string)));
            dtOrderList.Columns.Add(new DataColumn(clsConstants.Tables.ITEM_INFO.BARCODE, typeof(byte[])));
            dtOrderList.Columns.Add(new DataColumn(clsConstants.Tables.ITEM_INFO.ITEM_ID, typeof(string)));
            dtOrderList.Columns.Add(new DataColumn(clsConstants.Tables.ITEM_INFO.ITEM_NAME, typeof(string)));
            dtOrderList.Columns.Add(new DataColumn("item_quant", typeof(string)));
            dtOrderList.Columns.Add(new DataColumn(clsConstants.Tables.ORDER_DETAIL.TEMP_LOCATION, typeof(string)));
            SqlTransaction trans = null;
            SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                SqlDataReader myReader = null;
                trans = connection.BeginTransaction("tran");
                command.Transaction = trans;
                if (IsNew)
                {
                    //command = new SqlCommand(string.Format(SQLString.InsertNewPick, pick_dt), connection);
                    command.CommandText = string.Format(SQLString.InsertNewPick, pick_dt);
                    command.ExecuteNonQuery();

                    //command = new SqlCommand(string.Format(SQLString.UpdateOrderTempLocation, pick_dt), connection, trans);
                    //command.Transaction = trans;
                    command.CommandText = string.Format(SQLString.UpdateOrderTempLocation, pick_dt);
                    command.ExecuteNonQuery();
                    //trans.Commit();
                }

                //command = new SqlCommand(string.Format(SQLString.PickList, pick_dt), connection);
                command.CommandText = string.Format(SQLString.PickList, pick_dt);
                myReader = command.ExecuteReader();
                dtPicking.Load(myReader);
                //command = new SqlCommand(string.Format(SQLString.OrderList, pick_dt), connection);
                command.CommandText = string.Format(SQLString.OrderList, pick_dt);
                myReader = command.ExecuteReader();
                dtOrderList.Load(myReader);

                if (IsNew)
                {
                    string i = dtPicking.Rows.Count.ToString();
                    //command = new SqlCommand(string.Format(SQLString.UpdateMaxTempLocation, i), connection, trans);
                    command.CommandText = string.Format(SQLString.UpdateMaxTempLocation, i);
                    command.ExecuteNonQuery();
                    //command = new SqlCommand(string.Format(SQLString.UpdateOrderStatusToPick, pick_dt), connection, trans);
                    command.CommandText = string.Format(SQLString.UpdateOrderStatusToPick, pick_dt);
                    command.ExecuteNonQuery();

                    
                }
                dsReturn.Merge(dtPicking);
                dsReturn.Merge(dtOrderList);
                trans.Commit();
            }
            catch (Exception ex)
            {
                if (trans != null) trans.Rollback();
            }
            finally
            {
                connection.Close();
            }
            return dsReturn;
        }
        
        public DataSet GetStocktakeTable(string stocktake_dt)
        {
            DataSet dsReturn = new DataSet();

            DataTable dtReceive = new DataTable("stocktake_list");

            dtReceive.Columns.Add(new DataColumn("stocktake_dt_dis", typeof(string)));
            dtReceive.Columns.Add(new DataColumn(clsConstants.Tables.ITEM_INFO.ITEM_ID, typeof(string)));
            dtReceive.Columns.Add(new DataColumn(clsConstants.Tables.STOCKTAKE_HISTORY.QUANT_BEFORE, typeof(string)));
            dtReceive.Columns.Add(new DataColumn(clsConstants.Tables.STOCKTAKE_HISTORY.QUANT_AFTER, typeof(string)));
            dtReceive.Columns.Add(new DataColumn(clsConstants.Tables.ITEM_INFO.ITEM_NAME, typeof(string)));
            dtReceive.Columns.Add(new DataColumn(clsConstants.Tables.ITEM_INFO.ITEM_LOCATION, typeof(string)));
            dtReceive.Columns.Add(new DataColumn(clsConstants.Tables.ITEM_INFO.BARCODE, typeof(string)));

            SqlTransaction trans = null;
            SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
                SqlCommand command = null;
                SqlDataReader myReader = null;

                command = new SqlCommand(string.Format(SQLString.StocktakeList, stocktake_dt), connection);
                myReader = command.ExecuteReader();
                dtReceive.Load(myReader);

                dsReturn.Merge(dtReceive);
            }
            catch (Exception ex)
            {
                if (trans != null) trans.Rollback();
            }
            finally
            {
                connection.Close();
            }
            return dsReturn;
        }
        
        public DataSet GetReceiveTable(string receive_dt)
        {
            DataSet dsReturn = new DataSet();

            DataTable dtReceive = new DataTable("receive_list");

            dtReceive.Columns.Add(new DataColumn("receive_dt_dis", typeof(string)));
            dtReceive.Columns.Add(new DataColumn(clsConstants.Tables.ITEM_INFO.ITEM_ID, typeof(string)));
            dtReceive.Columns.Add(new DataColumn("item_quant", typeof(string)));
            dtReceive.Columns.Add(new DataColumn(clsConstants.Tables.ITEM_INFO.ITEM_NAME, typeof(string)));
            dtReceive.Columns.Add(new DataColumn(clsConstants.Tables.ITEM_INFO.ITEM_LOCATION, typeof(string)));
            dtReceive.Columns.Add(new DataColumn(clsConstants.Tables.ITEM_INFO.BARCODE, typeof(string)));

            SqlTransaction trans = null;
            SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
                SqlCommand command = null;
                SqlDataReader myReader = null;

                command = new SqlCommand(string.Format(SQLString.ReceiveList, receive_dt), connection);
                myReader = command.ExecuteReader();
                dtReceive.Load(myReader);
                
                dsReturn.Merge(dtReceive);
            }
            catch (Exception ex)
            {
                if (trans != null) trans.Rollback();
            }
            finally
            {
                connection.Close();
            }
            return dsReturn;
        }

        public bool ReturnProcess(DataTable dt, string order_id, string reason, bool IS_FULL, out string msg)
        {
            int i = -1;
            msg = string.Empty;
            string return_dt = DateTime.Now.ToString(clsConstants.VALUES.DATETIME_STRING);
            SqlConnection connection = new SqlConnection(connectionString);
            SqlTransaction trans = null;
            try
            {
                connection.Open();
                trans = connection.BeginTransaction("tran");
                SqlCommand myCommand = new SqlCommand(string.Format(SQLString.ReturnOrderList, 
                               clsConstants.VALUES.SYS_CODESET.ORDER_STATUS.RETURNED, order_id, reason), connection);
                myCommand.Transaction = trans;
                i = myCommand.ExecuteNonQuery();
                myCommand = new SqlCommand(string.Format(SQLString.ReturnInfo,
                               return_dt, order_id, reason), connection);
                myCommand.Transaction = trans;
                i = myCommand.ExecuteNonQuery();
                
                Dictionary<string, int> dic = new Dictionary<string, int>();
                foreach (DataRow dr in dt.Rows)
                {
                 if (!dic.ContainsKey(dr[clsConstants.Tables.ORDER_DETAIL.ITEM_ID].ToString()))
                    {
                        dic.Add(dr[clsConstants.Tables.ORDER_DETAIL.ITEM_ID].ToString(), dr.Field<int>("this_quant"));
                    }
                 else
                    {
                        dic[dr[clsConstants.Tables.ORDER_DETAIL.ITEM_ID].ToString()] += dr.Field<int>("this_quant");
                    }
                }

                string check = SQLString.CheckReturnQuantMain;
                string updateQuant = string.Empty;
                string inv = string.Empty;
                string return_history = SQLString.ReturnHistoryBase;
                foreach (KeyValuePair<string, int> kvp in dic)
                {
                    check = check + string.Format(SQLString.CheckReturnQuantSub, order_id, kvp.Key, kvp.Value);
                    updateQuant = updateQuant + string.Format(SQLString.UpdateReturnOrderDetail, order_id, kvp.Key, kvp.Value);
                    inv = inv + string.Format(SQLString.UpdateReturnItemInfo, kvp.Key, kvp.Value, IS_FULL ? 0 : kvp.Value);
                    return_history = return_history + string.Format(SQLString.ReturnHistoryUnit, return_dt, kvp.Key,
                        IS_FULL ? kvp.Value : 0, IS_FULL ? 0 : kvp.Value);
                }
                return_history = return_history.Substring(0, return_history.Length - 10);
                myCommand = new SqlCommand(check, connection);
                myCommand.Transaction = trans;
                SqlDataReader myReader = null;
                myReader = myCommand.ExecuteReader();
                DataTable dtCheck = new DataTable();
                dtCheck.Load(myReader);

                if (dtCheck != null && dtCheck.Rows.Count > 0) throw new Exception("資料庫已被其他人更新, 請重新再試");

                myCommand = new SqlCommand(updateQuant, connection);
                myCommand.Transaction = trans;
                i = myCommand.ExecuteNonQuery();

                //if (!IS_FULL)
                //{
                myCommand = new SqlCommand(inv, connection);
                myCommand.Transaction = trans;
                i = myCommand.ExecuteNonQuery();
                //}
                myCommand = new SqlCommand(return_history, connection);
                myCommand.Transaction = trans;
                i = myCommand.ExecuteNonQuery();
                
                myCommand = new SqlCommand(string.Format(SQLString.UpdateReturnOrderInfo, order_id), connection);
                myCommand.Transaction = trans;
                i = myCommand.ExecuteNonQuery();
                
                trans.Commit();
                
            }
            catch (Exception ex)
            {
                if (trans != null) trans.Rollback();
                msg = ex.Message;
                return false;
            }
            finally
            {
                connection.Close();
            }
            return i > -1;

        }

        public bool UpdateItemId(string oldId, string newId, out string msg)
        {
            int i = -1;
            msg = string.Empty;
            SqlConnection connection = new SqlConnection(connectionString);
            SqlTransaction trans = null;
            try
            {
                connection.Open();
                trans = connection.BeginTransaction("tran");
                SqlCommand myCommand = new SqlCommand(string.Format(SQLString.CheckItemClean, newId), connection);
                myCommand.Transaction = trans;
                SqlDataReader myReader = null;
                myReader = myCommand.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(myReader);
                if (dt != null && dt.Rows.Count > 0) throw new Exception("新編號已存在於資料庫");
                myCommand = new SqlCommand(string.Format(SQLString.UpdateItemId, oldId, newId), connection);
                myCommand.Transaction = trans;
                i = myCommand.ExecuteNonQuery();

                trans.Commit();
                
            }
            catch (Exception ex)
            {
                if (trans != null) trans.Rollback();
                msg = ex.Message;
                return false;
            }
            finally
            {
                connection.Close();
            }
            return i > -1;

        }
        
        public bool UpdateVendor(bool IsAdd, DataTable dtVendor, out string msg)
        {
            int i = -1;
            msg = string.Empty;
            SqlConnection connection = new SqlConnection(connectionString);
            SqlTransaction trans = null;
            try
            {
                //check name duplicated
                connection.Open();
                trans = connection.BeginTransaction("tran");
                SqlCommand myCommand = new SqlCommand(string.Format(SQLString.CheckVendorName,
                    dtVendor.Rows[0][clsConstants.Tables.VENDOR_INFO.VENDOR_CODE].ToString(),
                    dtVendor.Rows[0][clsConstants.Tables.VENDOR_INFO.VENDOR_NAME].ToString()), connection);
                myCommand.Transaction = trans;
                SqlDataReader myReader = null;
                myReader = myCommand.ExecuteReader();

                DataTable dt = new DataTable();
                dt.Load(myReader);
                if (dt != null && dt.Rows.Count > 0) throw new Exception("名稱重複!");

                if (IsAdd)
                {
                    //INSERT
                    myCommand = new SqlCommand(string.Format(SQLString.InsertVendor,
                        dtVendor.Rows[0][clsConstants.Tables.VENDOR_INFO.VENDOR_CODE].ToString(),
                        dtVendor.Rows[0][clsConstants.Tables.VENDOR_INFO.VENDOR_NAME].ToString(),
                        dtVendor.Rows[0][clsConstants.Tables.VENDOR_INFO.TEL].ToString(),
                        dtVendor.Rows[0][clsConstants.Tables.VENDOR_INFO.ADDRESS].ToString(),
                        dtVendor.Rows[0][clsConstants.Tables.VENDOR_INFO.URL].ToString(),
                        dtVendor.Rows[0][clsConstants.Tables.VENDOR_INFO.CONTACT_NAME].ToString()
                        ), connection);
                }
                else
                {
                    //UPDATE
                    myCommand = new SqlCommand(string.Format(SQLString.UpdateVendor,
                       dtVendor.Rows[0][clsConstants.Tables.VENDOR_INFO.VENDOR_CODE].ToString(),
                       dtVendor.Rows[0][clsConstants.Tables.VENDOR_INFO.VENDOR_NAME].ToString(),
                       dtVendor.Rows[0][clsConstants.Tables.VENDOR_INFO.TEL].ToString(),
                       dtVendor.Rows[0][clsConstants.Tables.VENDOR_INFO.ADDRESS].ToString(),
                       dtVendor.Rows[0][clsConstants.Tables.VENDOR_INFO.URL].ToString(),
                       dtVendor.Rows[0][clsConstants.Tables.VENDOR_INFO.CONTACT_NAME].ToString()
                       ), connection);
                }
                myCommand.Transaction = trans;
                i = myCommand.ExecuteNonQuery();

                trans.Commit();

            }
            catch (Exception ex)
            {
                if (trans != null) trans.Rollback();
                msg = ex.Message;
                return false;
            }
            finally
            {
                connection.Close();
            }
            return i > -1;

        }
        public DataTable FindVendors()
        {
            DataTable dtVendorList = new DataTable(clsConstants.Tables.VENDOR_INFO.TABLE_NAME);

            dtVendorList.Columns.Add(new DataColumn(clsConstants.Tables.VENDOR_INFO.VENDOR_CODE, typeof(string)));
            dtVendorList.Columns.Add(new DataColumn(clsConstants.Tables.VENDOR_INFO.VENDOR_NAME, typeof(string)));
            
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                SqlDataReader myReader = null;
                SqlCommand myCommand = new SqlCommand(SQLString.VendorList, connection);
                myReader = myCommand.ExecuteReader();

                dtVendorList.Load(myReader);

            }
            catch (Exception ex)
            {
               
            }
            finally
            {
                connection.Close();
            }
            return dtVendorList;
        }
        
        public DataTable GetVendor(string code)
        {
            DataTable dtVendor = clsConstants.Tables.VENDOR_INFO.BuildTable();

            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                SqlDataReader myReader = null;
                SqlCommand myCommand = new SqlCommand(string.Format(SQLString.GetVendor, code), connection);
                myReader = myCommand.ExecuteReader();

                dtVendor.Load(myReader);

            }
            catch (Exception ex)
            {

            }
            finally
            {
                connection.Close();
            }
            return dtVendor;
        }
        public DataSet GetRptInOut(string id, string item_id, string item_name, string datefrom, string dateto, string type, string remarks)
        {
            id = id.Replace("'", "''");
            item_id = item_id.Replace("'", "''");
            item_name = item_name.Replace("'", "''");
            datefrom = datefrom.Replace("'", "''");
            dateto = dateto.Replace("'", "''");
            type = type.Replace("'", "''");
            remarks = remarks.Replace("'", "''");

            DataSet dsReturn = new DataSet();

            DataTable dtReceive = new DataTable("rpt_inout");

            dtReceive.Columns.Add(new DataColumn("dt_dis", typeof(string)));
            dtReceive.Columns.Add(new DataColumn(clsConstants.Tables.ITEM_INFO.ITEM_ID, typeof(string)));
            dtReceive.Columns.Add(new DataColumn(clsConstants.Tables.ITEM_INFO.ITEM_NAME, typeof(string)));
            dtReceive.Columns.Add(new DataColumn("id", typeof(string)));
            dtReceive.Columns.Add(new DataColumn("quant", typeof(string)));
            dtReceive.Columns.Add(new DataColumn("type", typeof(string)));
            dtReceive.Columns.Add(new DataColumn(clsConstants.Tables.ORDER_LIST.REMARKS, typeof(string)));

            SqlTransaction trans = null;
            SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();
                SqlCommand command = null;
                SqlDataReader myReader = null;

                command = new SqlCommand(string.Format(SQLString.RptInOut, id, item_id, item_name, datefrom, dateto, type, remarks), connection);
                myReader = command.ExecuteReader();
                dtReceive.Load(myReader);

                dsReturn.Merge(dtReceive);
            }
            catch (Exception ex)
            {
                if (trans != null) trans.Rollback();
            }
            finally
            {
                connection.Close();
            }
            return dsReturn;
        }

        public DataTable FindDetail(string shipping_id, out bool is_done, out string order_id, out string date, out string remarks)
        {
            is_done = true;
            shipping_id = shipping_id.Replace("'", "''");
            string GetDetails = string.Format(SQLString.GetOrderDetails, shipping_id);
            string GetOrderMain = string.Format(SQLString.GetOrderMain + SQLString.GetOrderMainAndStatus, shipping_id);

            DataTable dtOrderDetail = new DataTable("order_detail");

            dtOrderDetail.Columns.Add(new DataColumn(clsConstants.Tables.ITEM_INFO.ITEM_ID, typeof(string)));
            dtOrderDetail.Columns.Add(new DataColumn(clsConstants.Tables.ITEM_INFO.ITEM_NAME, typeof(string)));
            dtOrderDetail.Columns.Add(new DataColumn(clsConstants.Tables.ORDER_DETAIL.ITEM_OPTION, typeof(string)));
            dtOrderDetail.Columns.Add(new DataColumn(clsConstants.Tables.ORDER_DETAIL.ITEM_QUANT, typeof(Int32)));
            dtOrderDetail.Columns.Add(new DataColumn("current_quant", typeof(Int32)));
            dtOrderDetail.Columns.Add(new DataColumn("complete", typeof(bool)));

            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                SqlDataReader myReader = null;
                SqlCommand myCommand = new SqlCommand(GetDetails, connection);
                myReader = myCommand.ExecuteReader();

                dtOrderDetail.Load(myReader);

                myCommand = new SqlCommand(GetOrderMain, connection);
                myReader = myCommand.ExecuteReader();
                DataTable e = new DataTable();
                e.Load(myReader);
                if (e != null && e.Rows.Count > 0 && e.Rows[0][clsConstants.Tables.ORDER_LIST.STATUS].ToString() != clsConstants.VALUES.SYS_CODESET.ORDER_STATUS.COMPLETE)
                {
                    //shipping_id = e.Rows[0][clsConstants.Tables.ORDER_LIST.SHIPPING_ID].ToString();
                    order_id = e.Rows[0][clsConstants.Tables.ORDER_LIST.ORDER_ID].ToString();
                    date = e.Rows[0][clsConstants.Tables.ORDER_LIST.ORDER_DATE].ToString();
                    remarks = e.Rows[0][clsConstants.Tables.ORDER_LIST.REMARKS].ToString();
                    is_done = false;
                }
                else
                {
                    //shipping_id = string.Empty;
                    order_id = string.Empty;
                    date = string.Empty;
                    remarks = string.Empty;
                    is_done = true;
                }

            }
            catch (Exception ex)
            {
                order_id = string.Empty;
                remarks = string.Empty;
                date = string.Empty;
                shipping_id = string.Empty;
            }
            finally
            {
                connection.Close();
            }
            return dtOrderDetail;
        }

        public DataTable GetOrderDetailForReturn(string shipping_id, out string msg)
        {
            msg = string.Empty;
            shipping_id = shipping_id.Replace("'", "''");
            string GetDetails = string.Format(SQLString.GetOrderDetailsForReturn + SQLString.GetOrderDetailsOrOrderId, shipping_id);
            string GetOrderMain = string.Format(SQLString.GetOrderMain + SQLString.GetOrderMainOrOrderId, shipping_id);

            DataTable dtOrderDetail = new DataTable("order_detail");

            dtOrderDetail.Columns.Add(new DataColumn(clsConstants.Tables.ITEM_INFO.ITEM_ID, typeof(string)));
            dtOrderDetail.Columns.Add(new DataColumn(clsConstants.Tables.ITEM_INFO.ITEM_NAME, typeof(string)));
            dtOrderDetail.Columns.Add(new DataColumn(clsConstants.Tables.ORDER_DETAIL.ITEM_OPTION, typeof(string)));
            dtOrderDetail.Columns.Add(new DataColumn(clsConstants.Tables.ORDER_DETAIL.ITEM_QUANT, typeof(Int32)));
            dtOrderDetail.Columns.Add(new DataColumn(clsConstants.Tables.ORDER_DETAIL.RETURN_QUANT, typeof(Int32)));

            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                SqlDataReader myReader = null;
                SqlCommand myCommand = new SqlCommand(GetDetails, connection);
                myReader = myCommand.ExecuteReader();

                dtOrderDetail.Load(myReader);

                myCommand = new SqlCommand(GetOrderMain, connection);
                myReader = myCommand.ExecuteReader();
                DataTable e = new DataTable();
                e.Load(myReader);
                if (e != null && e.Rows.Count > 0)
                {
                    string sts = e.Rows[0][clsConstants.Tables.ORDER_LIST.STATUS].ToString();
                    switch (sts)
                    {
                        case clsConstants.VALUES.SYS_CODESET.ORDER_STATUS.COMPLETE:
                            break;
                        case clsConstants.VALUES.SYS_CODESET.ORDER_STATUS.ABORT:
                            throw new Exception("這筆訂單已取消");
                        case clsConstants.VALUES.SYS_CODESET.ORDER_STATUS.PICKING:
                            throw new Exception("這筆訂單在撿貨中, 請使用取消訂單");
                        case clsConstants.VALUES.SYS_CODESET.ORDER_STATUS.RETURNED:
                            if (!e.Rows[0].Field<bool>(clsConstants.Tables.ORDER_LIST.RETURN_COMPLETE))
                                break;
                            throw new Exception("這筆訂單已被退貨");
                        case clsConstants.VALUES.SYS_CODESET.ORDER_STATUS.UNDONE:
                            throw new Exception("這筆訂單未完成, 請使用取消訂單");
                        default:
                            break;
                    }
                }
                else
                {
                    throw new Exception("找不到這筆訂單");    
                }

            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return null;
            }
            finally
            {
                connection.Close();
            }
            return dtOrderDetail;
        }
        public DataTable GetSysCodeset(string code_type, string code_value = "")
        {
            DataTable dtSysCodeset = clsConstants.Tables.SYS_CODESET.BuildTable();

            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                SqlDataReader myReader = null;
                SqlCommand myCommand = new SqlCommand(string.Format(SQLString.GetSysCodeSet, code_type, code_value), connection);
                myReader = myCommand.ExecuteReader();

                dtSysCodeset.Load(myReader);

            }
            catch (Exception ex)
            {
               
            }
            finally
            {
                connection.Close();
            }
            return dtSysCodeset;
        }

        public bool CheckPassword(string ac, string pswd, out string msg)
        {
            StringBuilder sb = new StringBuilder();
            HashAlgorithm algo = MD5.Create();
            foreach (byte b in algo.ComputeHash(Encoding.UTF8.GetBytes(pswd)))
            {
                sb.Append(b.ToString("X2"));
            }
            pswd = sb.ToString();
            msg = string.Empty;
            
            string Getpw = string.Format(SQLString.GetPW, ac, pswd);

            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                DataTable dt = new DataTable();
                connection.Open();
                SqlDataReader myReader = null;
                SqlCommand myCommand = new SqlCommand(Getpw, connection);
                myReader = myCommand.ExecuteReader();

                dt.Load(myReader);
                if (dt == null || dt.Rows.Count == 0)
                {
                    throw new Exception("Wrong password");
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }
            finally
            {
                connection.Close();
            }
            return true;
        }

        public bool InventoryMinus(string order_id, out string msg)
        {
            int i = -1;
            msg = string.Empty;
            order_id = order_id.Replace("'", "''");
            SqlConnection connection = new SqlConnection(connectionString);
            SqlTransaction trans = null;
            try
            {
                connection.Open();
                trans = connection.BeginTransaction("tran");
                SqlCommand myCommand = new SqlCommand(string.Format(SQLString.MinusInventory, order_id), connection);
                myCommand.Transaction = trans;
                i = myCommand.ExecuteNonQuery();
                myCommand = new SqlCommand(string.Format(SQLString.UpdateOrderStatus, clsConstants.VALUES.SYS_CODESET.ORDER_STATUS.COMPLETE, order_id), connection, trans);
                i = myCommand.ExecuteNonQuery();

                trans.Commit();
                if (i < 1)
                {
                    throw new Exception("cannot find this record");
                }

            }
            catch (Exception ex)
            {
                if (trans != null) trans.Rollback();
                msg = ex.Message;
                return false;
            }
            finally
            {
                connection.Close();
            }
            return i > -1;
        }

        public DataTable FindItem(string barcode)
        {
            DataTable dtItem = new DataTable("item");
            barcode = barcode.Replace("'", "''");
            dtItem.Columns.Add(new DataColumn(clsConstants.Tables.ITEM_INFO.ITEM_ID, typeof(string)));
            dtItem.Columns.Add(new DataColumn(clsConstants.Tables.ITEM_INFO.ITEM_NAME, typeof(string)));
            dtItem.Columns.Add(new DataColumn(clsConstants.Tables.ITEM_INFO.ITEM_LOCATION, typeof(string)));
            dtItem.Columns.Add(new DataColumn(clsConstants.Tables.ITEM_INFO.BARCODE, typeof(string)));
            dtItem.Columns.Add(new DataColumn(clsConstants.Tables.ITEM_INFO.INV, typeof(string)));
            dtItem.Columns.Add(new DataColumn("current_quant", typeof(string)));

            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                SqlDataReader myReader = null;
                SqlCommand myCommand = new SqlCommand(string.Format(SQLString.GetItem, barcode), connection);
                myReader = myCommand.ExecuteReader();

                dtItem.Load(myReader);

            }
            catch (Exception ex)
            {

            }
            finally
            {
                connection.Close();
            }
            return dtItem;
        }

        public DataTable FindItemById(string id)
        {
            DataTable dtItem = clsConstants.Tables.ITEM_INFO.BuildTable();

            id = id.Replace("'", "''");
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                SqlDataReader myReader = null;
                SqlCommand myCommand = new SqlCommand(string.Format(SQLString.GetItemById, id), connection);
                myReader = myCommand.ExecuteReader();

                dtItem.Load(myReader);

            }
            catch (Exception ex)
            {

            }
            finally
            {
                connection.Close();
            }
            return dtItem;
        }
        
        public bool InventoryOutVendor(DataTable dtOut, string remarks, string vendor, out string msg)
        {
            int i = -1;
            msg = string.Empty;
            remarks = remarks.Replace("'", "''");

            SqlConnection connection = new SqlConnection(connectionString);

            SqlTransaction trans = null;
            try
            {
                string out_dt = DateTime.Now.ToString(clsConstants.VALUES.DATETIME_STRING);
                connection.Open();

                SqlCommand myCommand = new SqlCommand(string.Format(SQLString.InsertReturnVendorInfo, out_dt, remarks, vendor), connection);
                i = myCommand.ExecuteNonQuery();


                myCommand = new SqlCommand(SQLString.InitializeForReceiveItem, connection);
                i = myCommand.ExecuteNonQuery();
                using (var bulkCopy = new SqlBulkCopy(connection))
                {
                    foreach (DataColumn col in dtOut.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                    }

                    bulkCopy.BulkCopyTimeout = 600;
                    bulkCopy.DestinationTableName = "dbo.#ITEM_INFO";
                    bulkCopy.WriteToServer(dtOut);
                }

                trans = connection.BeginTransaction("tran");
                using (var command = new SqlCommand("[ItemInfo.UpdateQuant]", connection)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    command.Parameters.Add("@dt", SqlDbType.VarChar).Value = out_dt;
                    command.Parameters.Add("@in", SqlDbType.Bit).Value = false;
                    command.Transaction = trans;
                    command.ExecuteNonQuery();
                }
                trans.Commit();

                return true;
            }
            catch (Exception ex)
            {
                if (trans != null) trans.Rollback();
                msg = ex.Message;
                return false;
            }
            finally
            {
                connection.Close();
            }
        }

        public bool InventoryAdd(DataTable dtIn, string receive_no, string remarks, string vendor, out string msg)
        {
            int i = -1;
            msg = string.Empty;
            receive_no = receive_no.Replace("'", "''");
            remarks = remarks.Replace("'", "''");

            SqlConnection connection = new SqlConnection(connectionString);

            SqlTransaction trans = null;
            try
            {
                string receive_dt = DateTime.Now.ToString(clsConstants.VALUES.DATETIME_STRING);
                connection.Open();

                SqlCommand myCommand = new SqlCommand(string.Format(SQLString.InsertReceiveInfo, receive_dt, receive_no, remarks, vendor), connection);
                i = myCommand.ExecuteNonQuery();


                myCommand = new SqlCommand(SQLString.InitializeForReceiveItem, connection);
                i = myCommand.ExecuteNonQuery();
                using (var bulkCopy = new SqlBulkCopy(connection))
                {
                    foreach (DataColumn col in dtIn.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                    }

                    bulkCopy.BulkCopyTimeout = 600;
                    bulkCopy.DestinationTableName = "dbo.#ITEM_INFO";
                    bulkCopy.WriteToServer(dtIn);
                }

                trans = connection.BeginTransaction("tran");
                using (var command = new SqlCommand("[ItemInfo.UpdateQuant]", connection)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    command.Parameters.Add("@in", SqlDbType.Bit).Value = true;
                    command.Parameters.Add("@dt", SqlDbType.VarChar).Value = receive_dt;
                    command.Transaction = trans;
                    command.ExecuteNonQuery();
                }
                trans.Commit();

                return true;
            }
            catch (Exception ex)
            {
                if (trans != null) trans.Rollback();
                msg = ex.Message;
                return false;
            }
            finally
            {
                connection.Close();
            }
        }

        public bool CancelOrderAddBack(string order_id, out string msg)
        {
            int i = -1;
            msg = string.Empty;

            order_id = order_id.Replace("'", "''");
            SqlConnection connection = new SqlConnection(connectionString);
            SqlTransaction trans = null;
            try
            {
                connection.Open();
                SqlCommand myCommand = new SqlCommand(string.Format(SQLString.CheckOrderExistsAndPend, order_id), connection);
                var result = myCommand.ExecuteScalar();
                i = result == null ? i : (int)result;
                if (i == -1) throw new Exception("找不到這筆訂單");
                if (i == 2) throw new Exception("這筆訂單已完成, 請使用退貨");
                if (i == 3) throw new Exception("這筆訂單已取消");
                if (i == 4) throw new Exception("這筆訂單已退貨");
                if (i == 5) throw new Exception("找不到這筆訂單");
                trans = connection.BeginTransaction("tran");
                myCommand = new SqlCommand(string.Format(SQLString.AddBackInventory, order_id), connection);
                myCommand.Transaction = trans;
                i = myCommand.ExecuteNonQuery();
                myCommand = new SqlCommand(string.Format(SQLString.UpdateOrderStatus, clsConstants.VALUES.SYS_CODESET.ORDER_STATUS.ABORT, order_id), connection, trans);
                i = myCommand.ExecuteNonQuery();

                trans.Commit();
                if (i < 1)
                {
                    throw new Exception("不明原因失敗");
                }

            }
            catch (Exception ex)
            {
                if (trans != null) trans.Rollback();
                msg = ex.Message;
                return false;
            }
            finally
            {
                connection.Close();
            }
            return i > -1;
        }
        
        public bool StockTake(DataTable dtIn, out string msg)
        {
            int i = -1;
            msg = string.Empty;

            SqlConnection connection = new SqlConnection(connectionString);

            SqlTransaction trans = null;
            try
            {

                connection.Open();
                SqlCommand myCommand = new SqlCommand(SQLString.InitializeForStockTake, connection);
                i = myCommand.ExecuteNonQuery();
                using (var bulkCopy = new SqlBulkCopy(connection))
                {
                    foreach (DataColumn col in dtIn.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                    }

                    bulkCopy.BulkCopyTimeout = 600;
                    bulkCopy.DestinationTableName = "dbo.#ITEM_INFO";
                    bulkCopy.WriteToServer(dtIn);
                }
                DataTable dtResult = new DataTable("result");
                dtResult.Columns.Add(new DataColumn("result", typeof(string)));
                dtResult.Columns.Add(new DataColumn("msg", typeof(string)));

                trans = connection.BeginTransaction("tran");
                using (var command = new SqlCommand("[ItemInfo.StockTake]", connection)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    SqlDataReader myReader = null;
                    command.Parameters.Add("@dt", SqlDbType.VarChar).Value = DateTime.Now.ToString(clsConstants.VALUES.DATETIME_STRING);
                    command.Transaction = trans;
                    myReader = command.ExecuteReader();
                    dtResult.Load(myReader);
                    //command.ExecuteNonQuery();
                }
                trans.Commit();
                if (dtResult.Rows.Count > 0 && dtResult.Rows[0]["result"].ToString() == "success")
                {
                    msg = dtResult.Rows[0]["msg"].ToString();
                    return true;
                }
                else
                {
                    trans.Rollback();
                    msg = dtResult.Rows[0]["msg"].ToString();
                    return false;
                }
            }
            catch (Exception ex)
            {
                if (trans != null) trans.Rollback();
                msg = ex.Message;
                return false;
            }
            finally
            {
                connection.Close();
            }
        }

        public bool AddItem(bool IsAdd, DataTable dtIn, out string msg)
        {
            SqlTransaction trans = null;
            SqlConnection connection = new SqlConnection(connectionString);
            msg = string.Empty;
            if (dtIn.Rows.Count == 0) return false;
            string item_id = dtIn.Rows[0][clsConstants.Tables.ITEM_INFO.ITEM_ID].ToString().Replace("'", "''");
            string item_name = dtIn.Rows[0][clsConstants.Tables.ITEM_INFO.ITEM_NAME].ToString().Replace("'", "''");
            string item_location = dtIn.Rows[0][clsConstants.Tables.ITEM_INFO.ITEM_LOCATION].ToString().Replace("'", "''");
            string item_mass_location = dtIn.Rows[0][clsConstants.Tables.ITEM_INFO.ITEM_MASS_LOCATION].ToString().Replace("'", "''");
            string inv = dtIn.Rows[0][clsConstants.Tables.ITEM_INFO.INV].ToString().Length == 0? "-1" : dtIn.Rows[0][clsConstants.Tables.ITEM_INFO.INV].ToString().Replace("'", "''");
            string status = dtIn.Rows[0][clsConstants.Tables.ITEM_INFO.STATUS].ToString().Replace("'", "''");
            string barcode = dtIn.Rows[0][clsConstants.Tables.ITEM_INFO.BARCODE].ToString().Replace("'", "''");
            string last_by = IsAdd ? "add_item" : "update_item";
            string last_dt = DateTime.Now.ToString(clsConstants.VALUES.DATETIME_STRING).Replace("'", "''");
            string last_st = IsAdd ? "I" : "U";
            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                SqlDataReader myReader = null;
                trans = connection.BeginTransaction("tran");
                command.Transaction = trans;

                if (IsAdd)
                {
                    DataTable dtExists = new DataTable();
                    //command = new SqlCommand(string.Format(SQLString.PickList, pick_dt), connection);
                    command.CommandText = string.Format(SQLString.IsItemExisted, item_id, barcode);
                    myReader = command.ExecuteReader();
                    dtExists.Load(myReader);
                    if (dtExists.Rows.Count > 0) throw new Exception("商品編號或條碼與資料庫內容重複");
                }
                
                //command = new SqlCommand(string.Format(SQLString.UpdateMaxTempLocation, i), connection, trans);
                //[item_id], [item_name], [item_location], [item_mass_location], [barcode], [last_mod_by], [last_mod_dt], [last_mod_st], [inv], [status]
                if (IsAdd) command.CommandText = string.Format(SQLString.InsertItem, item_id, item_name, item_location, item_mass_location, barcode, last_by,last_dt,last_st,inv, status);
                else command.CommandText = string.Format(SQLString.UpdateItem, item_id, item_name, item_location, item_mass_location, barcode, last_by, last_dt, last_st, inv, status);
                command.ExecuteNonQuery();

                trans.Commit();
                return true;
            }
            catch (Exception ex)
            {
                if (trans != null) trans.Rollback();
                msg = ex.Message;
                return false;
            }
            finally
            {
                connection.Close();
            }
            
        }

        public bool UpdatePickDt(string order_id)
        {
            int i = -1;
            order_id = order_id.Replace("'", "''");
            string UpdatePick =
                string.Format(SQLString.UpdatePickDt, order_id);

            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                SqlCommand myCommand = new SqlCommand(UpdatePick, connection);
                i = myCommand.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                connection.Close();
            }
            return i > -1;
        }
        public bool ImportOrder(DataTable dt,out string msg)
        {
            msg = string.Empty;
            SqlConnection connection = new SqlConnection(connectionString);

            SqlTransaction trans = null;
            try
            {   
                DataTable dtMain = null, dtDetail = null;
                Order_Separator.Separate(dt, out dtMain, out dtDetail, out string msgs);
                if (dtMain == null || dtDetail == null) throw new Exception(msgs);
                int i = -1;

                connection.Open();
                SqlCommand myCommand = new SqlCommand(SQLString.InitializeForImportOrder, connection);
                i = myCommand.ExecuteNonQuery();

                using (var bulkCopy = new SqlBulkCopy(connection))
                {
                    foreach (DataColumn col in dtMain.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                    }

                    bulkCopy.BulkCopyTimeout = 600;
                    bulkCopy.DestinationTableName = "dbo.#ORDER_LIST";
                    bulkCopy.WriteToServer(dtMain);
                }
                using (var bulkCopy = new SqlBulkCopy(connection))
                {
                    foreach (DataColumn col in dtDetail.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                    }

                    bulkCopy.BulkCopyTimeout = 600;
                    bulkCopy.DestinationTableName = "dbo.#ORDER_DETAIL";
                    bulkCopy.WriteToServer(dtDetail);
                }

                DataTable dtResult = new DataTable("result");
                dtResult.Columns.Add(new DataColumn("result", typeof(string)));
                dtResult.Columns.Add(new DataColumn("msg", typeof(string)));

                trans = connection.BeginTransaction("tran");
                using (var command = new SqlCommand("[OrderList.CheckAndInsertUpdate]", connection)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    SqlDataReader myReader = null;
                    
                    command.Transaction = trans;
                    myReader = command.ExecuteReader();
                    dtResult.Load(myReader);
                    //command.ExecuteNonQuery();
                }
                trans.Commit();
                if (dtResult.Rows.Count > 0 && dtResult.Rows[0]["result"].ToString() == "success")
                {
                    msg = dtResult.Rows[0]["msg"].ToString();
                    return true;
                }
                else
                {
                    trans.Rollback();
                    msg = dtResult.Rows[0]["msg"].ToString();
                    return false;
                }
            }
            catch (Exception ex)
            {
                if (trans != null) trans.Rollback();
                msg = ex.Message;
                if (ex.InnerException != null) msg += "\n" + ex.InnerException.Message;
                return false;
            }
            finally
            {
                connection.Close();
            }

        }
        public bool ImportItem(DataTable dt, bool ignore, out string msg)
        {
            msg = string.Empty;
            SqlConnection connection = new SqlConnection(connectionString);

            SqlTransaction trans = null;
            try
            {
                int i = -1;
                Order_Separator.FilterItem(ignore, ref dt);
                if (dt == null) throw new Exception("請確認商品編號唯一且不為空");
                
                connection.Open();
                SqlCommand myCommand = new SqlCommand(SQLString.InitializeForImportItem, connection);
                i = myCommand.ExecuteNonQuery();
                
                using (var bulkCopy = new SqlBulkCopy(connection))
                {
                    foreach (DataColumn col in dt.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                    }

                    bulkCopy.BulkCopyTimeout = 600;
                    bulkCopy.DestinationTableName = "dbo.#ITEM_INFO";
                    bulkCopy.WriteToServer(dt);
                }
                DataTable dtResult = new DataTable("result");
                dtResult.Columns.Add(new DataColumn("result", typeof(string)));
                dtResult.Columns.Add(new DataColumn("msg", typeof(string)));

                trans = connection.BeginTransaction("tran");
                using (var command = new SqlCommand("[ItemInfo.CheckAndInsertUpdate]", connection)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    SqlDataReader myReader = null;

                    command.Transaction = trans;
                    myReader = command.ExecuteReader();
                    dtResult.Load(myReader);
                    //command.Transaction = trans;
                    //command.ExecuteNonQuery();
                }
                trans.Commit();
                if (dtResult.Rows.Count > 0 && dtResult.Rows[0]["result"].ToString().StartsWith("success"))
                {
                    msg = dtResult.Rows[0]["msg"].ToString();
                    return true;
                }
                else
                {
                    msg = dtResult.Rows[0]["msg"].ToString();
                    return false;
                }
            }
            catch (Exception ex)
            {
                if (trans != null) trans.Rollback();
                msg = ex.Message;
                if (ex.InnerException != null) msg += "\n" +  ex.InnerException.Message;
                if (trans != null) trans.Rollback();
                return false;
            }
            finally
            {
                connection.Close();
            }

        }
        
    }
}
