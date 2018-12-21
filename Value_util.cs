
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonResources
{
    public static class Value_util
    {

        public static string GetExcelDateTime(double d)
        {
            //09 / 30 / 2018  13:21:43
            DateTime dt = DateTime.FromOADate(d);
            string r = dt.ToString(clsConstants.VALUES.DATETIME_STRING);
            return r;
        }

        public static string GetExcelSlashDateTime(string s)
        {
            //2018/09/30
            string r = string.Empty;
            if (DateTime.TryParse(s, out DateTime dt))
            {
                r = dt.ToString(clsConstants.VALUES.DATETIME_STRING);
            }
            else if (DateTime.TryParseExact(s, "yyyy/MM/dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dt2))
            {
                r = dt2.ToString(clsConstants.VALUES.DATETIME_STRING);
            }

            return r;
        }
    }
}
