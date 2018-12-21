
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonResources
{
    public static class clsConstants
    {
        public static class Tables
        {
            public static class SYS_CODESET
            {
                public const string TABLE_NAME = "SYS_CODESET";
                public const string CODE_TYPE = "code_type";
                public const string CODE_VALUE = "code_value";
                public const string CODE_DESC = "code_desc";

                public static DataTable BuildTable()
                {
                    DataTable dt = new DataTable(TABLE_NAME);
                    dt.Columns.Add(new DataColumn(CODE_TYPE, typeof(string)));
                    dt.Columns.Add(new DataColumn(CODE_VALUE, typeof(string)));
                    dt.Columns.Add(new DataColumn(CODE_DESC, typeof(string)));
                    return dt;
                }
            }
            public static class ITEM_INFO
            {
                public const string TABLE_NAME = "ITEM_INFO";
                public const string ITEM_ID = "item_id";
                public const string ITEM_NAME = "item_name";
                public const string ITEM_LOCATION = "item_location";
                public const string ITEM_MASS_LOCATION = "item_mass_location";
                public const string BARCODE = "barcode";
                public const string INV = "inv";
                public const string STATUS = "status";
                public const string BROKEN_COUNT = "broken_count";

                public static DataTable BuildTable()
                {
                    DataTable dt = new DataTable(TABLE_NAME);
                    dt.Columns.Add(new DataColumn(ITEM_ID, typeof(string)));
                    dt.Columns.Add(new DataColumn(ITEM_NAME, typeof(string)));
                    dt.Columns.Add(new DataColumn(ITEM_LOCATION, typeof(string)));
                    dt.Columns.Add(new DataColumn(ITEM_MASS_LOCATION, typeof(string)));
                    dt.Columns.Add(new DataColumn(BARCODE, typeof(string)));
                    dt.Columns.Add(new DataColumn(INV, typeof(string)));
                    dt.Columns.Add(new DataColumn(STATUS, typeof(string)));
                    dt.Columns.Add(new DataColumn(BROKEN_COUNT, typeof(string)));

                    return dt;
                }
            }
            public static class ORDER_DETAIL
            {
                public const string TABLE_NAME = "ORDER_DETAIL";
                public const string ORDER_ID = "order_id";
                public const string ITEM_ID = "item_id";
                public const string ITEM_OPTION = "item_option";
                public const string ITEM_QUANT = "item_quant";
                public const string PRICE_ORIGINAL = "price_original";
                public const string PRICE_ON_SALE = "price_on_sale";
                public const string ITEM_NAME = "item_name";
                public const string TEMP_LOCATION = "temp_location";
                public const string RETURN_QUANT = "return_quant";

                public static DataTable BuildTable()
                {
                    DataTable dt = new DataTable(TABLE_NAME);
                    dt.Columns.Add(new DataColumn(ORDER_ID, typeof(string)));
                    dt.Columns.Add(new DataColumn(ITEM_ID, typeof(string)));
                    dt.Columns.Add(new DataColumn(ITEM_OPTION, typeof(string)));
                    dt.Columns.Add(new DataColumn(ITEM_QUANT, typeof(string)));
                    dt.Columns.Add(new DataColumn(PRICE_ORIGINAL, typeof(string)));
                    dt.Columns.Add(new DataColumn(PRICE_ON_SALE, typeof(string)));
                    dt.Columns.Add(new DataColumn(ITEM_NAME, typeof(string)));
                    dt.Columns.Add(new DataColumn(TEMP_LOCATION, typeof(string)));
                    dt.Columns.Add(new DataColumn(RETURN_QUANT, typeof(string)));

                    return dt;
                }
            }
            public static class ORDER_LIST
            {
                public const string TABLE_NAME = "ORDER_LIST";
                public const string ORDER_ID = "order_id";
                public const string ORDER_DATE = "order_date";
                public const string RECIPIENT = "recipient";
                public const string TEL = "tel";
                public const string SHIPPING_ID = "shipping_id";
                public const string SHIPPING_FEE = "shipping_fee";
                public const string ATTACH_FEE = "attach_fee";
                public const string TOTAL_AMT = "total_amt";
                public const string DISCOUNT = "discount";
                public const string SHIPPING_METHOD = "shipping_method";
                public const string STATUS = "status";
                public const string PICK_DT = "pick_dt";
                public const string SHIP_DT = "ship_dt";
                public const string RETURN_DT = "return_dt";
                public const string RECEIVE_SHOP = "receive_shop";
                public const string ADDRESS = "address";
                public const string REMARKS = "remarks";
                public const string RETURN_REASON = "return_reason";
                public const string RETURN_COMPLETE = "return_complete";

                public static DataTable BuildTable()
                {
                    DataTable dt = new DataTable(TABLE_NAME);
                    dt.Columns.Add(new DataColumn(ORDER_ID, typeof(string)));
                    dt.Columns.Add(new DataColumn(ORDER_DATE, typeof(string)));
                    dt.Columns.Add(new DataColumn(RECIPIENT, typeof(string)));
                    dt.Columns.Add(new DataColumn(TEL, typeof(string)));
                    dt.Columns.Add(new DataColumn(SHIPPING_ID, typeof(string)));
                    dt.Columns.Add(new DataColumn(SHIPPING_FEE, typeof(string)));
                    dt.Columns.Add(new DataColumn(ATTACH_FEE, typeof(string)));
                    dt.Columns.Add(new DataColumn(TOTAL_AMT, typeof(string)));
                    dt.Columns.Add(new DataColumn(DISCOUNT, typeof(string)));
                    dt.Columns.Add(new DataColumn(SHIPPING_METHOD, typeof(string)));
                    dt.Columns.Add(new DataColumn(STATUS, typeof(string)));
                    dt.Columns.Add(new DataColumn(PICK_DT, typeof(string)));
                    dt.Columns.Add(new DataColumn(SHIP_DT, typeof(string)));
                    dt.Columns.Add(new DataColumn(RETURN_DT, typeof(string)));
                    dt.Columns.Add(new DataColumn(RECEIVE_SHOP, typeof(string)));
                    dt.Columns.Add(new DataColumn(ADDRESS, typeof(string)));
                    dt.Columns.Add(new DataColumn(REMARKS, typeof(string)));
                    dt.Columns.Add(new DataColumn(RETURN_REASON, typeof(string)));
                    dt.Columns.Add(new DataColumn(RETURN_COMPLETE, typeof(string)));

                    return dt;
                }
            }
            public static class PICKING_HISTORY
            {
                public const string TABLE_NAME = "PICKING_HISTORY";
                public const string PICK_DT = "pick_dt";
                public const string ORDER_ID = "order_id";

                public static DataTable BuildTable()
                {
                    DataTable dt = new DataTable(TABLE_NAME);
                    dt.Columns.Add(new DataColumn(PICK_DT, typeof(string)));
                    dt.Columns.Add(new DataColumn(ORDER_ID, typeof(string)));

                    return dt;
                }
            }
            public static class RECEIVE_HISTORY
            {
                public const string TABLE_NAME = "RECEIVE_HISTORY";
                public const string RECEIVE_DT = "receive_dt";
                public const string ITEM_ID = "item_id";
                public const string QUANT = "quant";

                public static DataTable BuildTable()
                {
                    DataTable dt = new DataTable(TABLE_NAME);
                    dt.Columns.Add(new DataColumn(RECEIVE_DT, typeof(string)));
                    dt.Columns.Add(new DataColumn(ITEM_ID, typeof(string)));
                    dt.Columns.Add(new DataColumn(QUANT, typeof(string)));

                    return dt;
                }
            }
            public static class STOCKTAKE_HISTORY
            {
                public const string TABLE_NAME = "STOCKTAKE_HISTORY";
                public const string STOCKTAKE_DT = "stocktake_dt";
                public const string ITEM_ID = "item_id";
                public const string QUANT_BEFORE = "quant_before";
                public const string QUANT_AFTER = "quant_after";

                public static DataTable BuildTable()
                {
                    DataTable dt = new DataTable(TABLE_NAME);
                    dt.Columns.Add(new DataColumn(STOCKTAKE_DT, typeof(string)));
                    dt.Columns.Add(new DataColumn(ITEM_ID, typeof(string)));
                    dt.Columns.Add(new DataColumn(QUANT_BEFORE, typeof(string)));
                    dt.Columns.Add(new DataColumn(QUANT_AFTER, typeof(string)));

                    return dt;
                }
            }
            public static class VENDOR_INFO
            {
                public const string TABLE_NAME = "VENDOR_INFO";
                public const string VENDOR_CODE = "vendor_code";
                public const string VENDOR_NAME = "vendor_name";
                public const string TEL = "tel";
                public const string ADDRESS = "address";
                public const string URL = "url";
                public const string CONTACT_NAME = "contact_name";

                public static DataTable BuildTable()
                {
                    DataTable dt = new DataTable(TABLE_NAME);
                    dt.Columns.Add(new DataColumn(VENDOR_CODE, typeof(string)));
                    dt.Columns.Add(new DataColumn(VENDOR_NAME, typeof(string)));
                    dt.Columns.Add(new DataColumn(TEL, typeof(string)));
                    dt.Columns.Add(new DataColumn(ADDRESS, typeof(string)));
                    dt.Columns.Add(new DataColumn(URL, typeof(string)));
                    dt.Columns.Add(new DataColumn(CONTACT_NAME, typeof(string)));

                    return dt;
                }
            }

        }

        public static class VALUES
        {
            public const string LAST_MOD_DT = "last_mod_dt";
            public const string LAST_MOD_ST = "last_mod_st";
            public const string LAST_MOD_BY = "last_mod_by";
            public const string VALUE = "Value";
            public const string KEY = "Key";
            public const string DATE_STRING = "yyyyMMdd";
            public const string DATETIME_STRING = "yyyyMMddHHmmssf";
            public const string DATETIME_DISPLAY_STRING = "yyyy-MM-dd HH:mm:ss";
            public const string EXCEL_NUM = "\\d\\.\\d+e\\+";
            public static class SYS_CODESET
            {
                public static class ORDER_STATUS
                {
                    public const string CODE_TYPE = "order_sts";
                    public const string COMPLETE = "C";
                    public const string UNDONE = "U";
                    public const string PICKING = "P";
                    public const string ABORT = "A";
                    public const string RETURNED = "R";
                }
                public static class ITEM_STATUS
                {
                    public const string CODE_TYPE = "item_sys";
                    public const string ACTIVE = "A";
                    public const string INACTIVE = "I";
                }
                public static class NUM_COMPARE
                {
                    public const string CODE_TYPE = "num_comp";
                    public const string LARGER = "L";
                    public const string EQUAL = "E";
                    public const string SMALLER = "S";
                }

                public static class SHIPPING_METHOD
                {
                    public const string CODE_TYPE = "shipping_method";
                    public const string SEVEN = "S";
                    public const string BLACK_CAT = "B";
                    public const string FAMILY = "F";
                    public const string POST = "P";
                    public const string SF = "SF";
                }
                public static class TEMP_LOCATION
                {
                    public const string CODE_TYPE = "temp_location";
                }
                public static class TEMP_LOCATION_TOTAL
                {
                    public const string CODE_TYPE = "temp_location_total";
                }
            }
        }

    }
}
