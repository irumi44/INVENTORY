using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUSHA.cls
{
    public static class  clsCache
{
        private static List<string> opening_forms { set; get; }
        public static Dictionary<string, string> StatusSet { set; get; }
        public static Dictionary<string, string> ItemStatusSet { set; get; }
        public static Dictionary<string, string> ShippingSet { set; get; }
        public static Dictionary<string, string> NumCompareSet { set; get; }
        public static Dictionary<string, string> VendorSet { set; get; }

        public static void SetOpen(form.ChildForm frm)
        {
            if (opening_forms == null) opening_forms = new List<string>();
            opening_forms.Add(frm.Name);
        }

        public static void SetClose(form.ChildForm frm)
        {
            if (opening_forms == null) opening_forms = new List<string>();
            if (opening_forms.Contains(frm.Name)) opening_forms.Remove(frm.Name);
        }

        public static bool IS_OPENING(form.ChildForm frm)
        {
            if (frm == null) return false;
            return opening_forms.Contains(frm.Name);
        }

        public static void SetShippingSet(DataTable dtSys)
        {
            ShippingSet = new Dictionary<string, string>();
            foreach (DataRow dr in dtSys.Rows)
            {
               ShippingSet.Add(dr[CommonResources.clsConstants.Tables.SYS_CODESET.CODE_VALUE].ToString(),
                               dr[CommonResources.clsConstants.Tables.SYS_CODESET.CODE_DESC].ToString());
            }
        }

        public static void SetStatusSet(DataTable dtSys)
        {
            StatusSet = new Dictionary<string, string>();
            foreach (DataRow dr in dtSys.Rows)
            {
                StatusSet.Add(dr[CommonResources.clsConstants.Tables.SYS_CODESET.CODE_VALUE].ToString(),
                              dr[CommonResources.clsConstants.Tables.SYS_CODESET.CODE_DESC].ToString());
            }
        }

        public static void SetItemStatusSet(DataTable dtSys)
        {
            ItemStatusSet = new Dictionary<string, string>();
            foreach (DataRow dr in dtSys.Rows)
            {
                ItemStatusSet.Add(dr[CommonResources.clsConstants.Tables.SYS_CODESET.CODE_VALUE].ToString(),
                                  dr[CommonResources.clsConstants.Tables.SYS_CODESET.CODE_DESC].ToString());
            }
        }
        
        public static void SetNumCompareSet(DataTable dtSys)
        {
            NumCompareSet = new Dictionary<string, string>();
            foreach (DataRow dr in dtSys.Rows)
            {
                NumCompareSet.Add(dr[CommonResources.clsConstants.Tables.SYS_CODESET.CODE_VALUE].ToString(),
                                  dr[CommonResources.clsConstants.Tables.SYS_CODESET.CODE_DESC].ToString());
            }
        }

       
    }
}
