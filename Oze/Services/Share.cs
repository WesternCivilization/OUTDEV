using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Oze.Services
{
    public static class Share
    {
        public static DateTime TodateTime(string date)
        {
            try
            {
                return DateTime.ParseExact(date, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {

                return DateTime.MinValue;
            }
        }
        public static DateTime Todate(string date)
        {
            try
            {
                return DateTime.ParseExact(date.Split(' ')[0], "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {

                return DateTime.MinValue;
            }
        }
    }
}