using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLM.Resource
{
    public class SLMUtil
    {
        public static int SafeInt(string str)
        {
            if (str == null) return 0;
            int r; int.TryParse(str.Replace(",",""), out r);
            return r;
        }

        public static decimal SafeDecimal(string str)
        {
            if (str == null) return 0;
            decimal d; decimal.TryParse(str.Replace(",", ""), out d);
            return d;
        }

        public static long SafeLong(string str)
        {
            if (str == null) return 0;
            long l; long.TryParse(str.Replace(",", ""), out l);
            return l;
        }

        public static double SafeDouble(string str)
        {
            if (str == null) return 0;
            double d; double.TryParse(str.Replace(",", ""), out d);
            return d;
        }

        //public static bool CheckDate(string dtString)
        //{
        //    var chk = dtString.Split('-');
        //    if (chk.Length != 3) return false;
        //    int d, m, y;
        //    int.TryParse(chk[0], out y);
        //    int.TryParse(chk[1], out m);
        //    int.TryParse(chk[2], out d);

        //    DateTime dt;
            

        //    if (y == 0 || m == 0 || d == 0 || m > 12 || !DateTime.TryParse(string.Format("{0}-{1}-{2}", y, m, d), out dt)) return false;
        //    else return true;

        //}

        public static bool CheckDate(string dtString)
        {
            var chk = dtString.Split('/');
            if (chk.Length != 3) return false;
            string year = chk[2].Substring(0, 4);
            int d, m, y;
            int.TryParse(chk[0], out d);
            int.TryParse(chk[1], out m);
            int.TryParse(year, out y);

            DateTime dt;


            if (y == 0 || m == 0 || d == 0 || m > 12 || !DateTime.TryParse(string.Format("{0}-{1}-{2}", y, m, d), out dt)) return false;
            else return true;

        }

        //public static DateTime? GetDateFromStr(string str)
        //{
        //    var tmp = str.Split('-');
        //    try
        //    {
        //        if (tmp.Length < 3) return null;
        //        int d, m, y;
        //        y = SafeInt(tmp[0]);
        //        m = SafeInt(tmp[1]);
        //        d = SafeInt(tmp[2]);

        //        return new DateTime(y, m, d);
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        public static DateTime? GetDateFromStr(string str)
        {
            var tmp = str.Split('/');
            try
            {
                if (tmp.Length < 3) return null;

                string year = tmp[2].Substring(0, 4);
                int d, m, y;
                y = SafeInt(year);
                m = SafeInt(tmp[1]);
                d = SafeInt(tmp[0]);

                return new DateTime(y, m, d);
            }
            catch
            {
                return null;
            }
        }

        public static bool ValidateEmail(string str)
        {
            string pattern = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(pattern);
            return reg.IsMatch(str.Trim());
        }


        public static string GetDateString(DateTime dt)
        {
            return string.Format(dt.ToString("{0}-MM-dd"), dt.Year);
        }

        public static string GetDBString(string str)
        {
            return str.Replace("'", "''");
        }

        public static DateTime? ConvertTimeZoneFromUtc(DateTime dateIn)
        {
            //เอาไว้ Convert TimeZone ของข้อมูลวันที่ที่ได้จาก CoreBank จาก UTC ให้เป็น Local
            return TimeZoneInfo.ConvertTime(dateIn, TimeZoneInfo.Utc, TimeZoneInfo.Local);
        }
    }
}
