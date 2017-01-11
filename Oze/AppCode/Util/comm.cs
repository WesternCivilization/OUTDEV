using Oze.AppCode.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Oze.AppCode.Util
{
    public class comm
    {
        public static readonly int ERROR_NOT_EXIST = -3;
        public static readonly int ERROR_EXIST=-2;
        public static readonly int ERROR_GENERAL = -1;

       

        public static int GetUserId() 
        {
            if(HttpContext.Current==null) return -1;
            if(HttpContext.Current.Session[CConfig.SESSION_USERID]==null) return -1;

            return Int32.Parse(HttpContext.Current.Session[CConfig.SESSION_USERID].ToString());
        }
        public static String GetUserName()
        {
            if (HttpContext.Current == null) return "";
            if (HttpContext.Current.Session[CConfig.SESSION_USERNAME] == null) return "";

            return (HttpContext.Current.Session[CConfig.SESSION_USERNAME].ToString());
        }

        public static int GetHotelId()
        {
            if (HttpContext.Current == null) return -1;
            if (HttpContext.Current.Session[CConfig.SESSION_HOTELID] == null) return -1;

            return int.Parse(HttpContext.Current.Session[CConfig.SESSION_HOTELID].ToString());
      
        }
        public static string GetHotelCode()
        {
            if (HttpContext.Current == null) return "";
            if (HttpContext.Current.Session[CConfig.SESSION_HOTELCODE] == null) return "";

            return (HttpContext.Current.Session[CConfig.SESSION_HOTELCODE].ToString());

        }
        public static string GetRightCode()
        {
            if (HttpContext.Current == null) return "";
            if (HttpContext.Current.Session[CConfig.SESSION_GROUPCODE] == null) return "";
            string sCode= (HttpContext.Current.Session[CConfig.SESSION_GROUPCODE].ToString());
            return sCode;
        }
        public static bool IsSuperAdmin()
        {
            return (GetRightCode() == "SUPERADMIN");
        }
        public static bool IsKeToan()
        {
            return (GetRightCode() == "KETOAN");
        }
        public static bool IsLeTan()
        {
            return (GetRightCode() == "LETAN");
        }
        public static bool IsQuanLy()
        {
            return (GetRightCode() == "QUANLY");
        }
       
        public static bool IsKT()
        {
             return IsKeToan();
        }
        public static bool IsLT()
        {
            return IsLeTan();
        }
        public static bool IsQL()
        {
            return IsQuanLy();
        }
        public static bool IsSP()
        {
            return IsSuperAdmin();
        }
    }
}