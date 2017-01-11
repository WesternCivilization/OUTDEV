using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Oze.Models
{
    public class PagingModel
    {
        public int offset;
        public int limit;
        public string search;
    }
    public class PagingBookingModel:PagingModel
    {
        public string dtFrom;
        public string dtTo;
        public int status;
        public int roomid;
        public int roomtypeid;
        public string code;
        public int bydate;

        public static PagingBookingModel initFrom(int length, int start, string search, string code,int status, int bydate, string dtFrom, string dtTo, int roomid,int roomtypeid)
        {
            return new PagingBookingModel() { limit = length, offset = start, search = search, code = code, status = status, bydate = bydate, dtFrom = dtFrom, dtTo = dtTo, roomid = roomid, roomtypeid = roomtypeid };
        }
    }
    public class PagingBookingModelPrice : PagingBookingModel
    {   
        public int typePrice;
        public int children;
        public int adult;

        public static PagingBookingModelPrice initNew(int length, int start, string search, string code, int status, int bydate, string dtFrom, string dtTo, int roomid, int roomtypeid, int typePrice, int adult,int children)
        {
            return new PagingBookingModelPrice() { limit = length, offset = start, search = search, code = code, status = status, bydate = bydate, dtFrom = dtFrom, dtTo = dtTo, roomid = roomid, roomtypeid = roomtypeid, typePrice = typePrice,adult=adult,children=children };
        }
    }
}