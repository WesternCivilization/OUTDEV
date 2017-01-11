using oze.data;
using Oze.AppCode.BLL;
using Oze.AppCode.Util;
using Oze.Models;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace Oze.Services
{
    public class CommService
    {
        IOzeConnectionFactory _connectionData;

        public CommService()
        {
            OrmLiteConfig.DialectProvider = SqlServerDialect.Provider;
            OrmLiteConfig.DialectProvider.UseUnicode = true; // nhập tiếng việt

            _connectionData = new OzeConnectionFactory(ConfigurationManager.AppSettings["ConnectionString_OzeGeneral"], SqlServerOrmLiteDialectProvider.Instance);

        }
        public static int GetUserId()
        {
            if (HttpContext.Current == null) return -1;
            if (HttpContext.Current.Session[CConfig.SESSION_USERID] == null) return -1;

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
        public static string GetRightCode()
        {
            if (HttpContext.Current == null) return "";
            if (HttpContext.Current.Session[CConfig.SESSION_GROUPCODE] == null) return "";
            return (HttpContext.Current.Session[CConfig.SESSION_GROUPCODE].ToString());

        }
        public static string fitTo6(int p)
        {
            string result = p.ToString();
            if (result.Length == 6) return result;
            if (result.Length == 5) return "0" + result;
            if (result.Length == 4) return "00" + result;
            if (result.Length == 3) return "000" + result;
            if (result.Length == 2) return "0000" + result;
            if (result.Length == 1) return "000000" + result;
            return result;
        }
        public static bool IsSuperAdmin()
        {
            return (GetRightCode() == "SUPERADMIN");
        }
        public List<tbl_Country> GetAllCountry()
        {
            //search again
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Country>().OrderBy(e=>e.Name);
                List<tbl_Country> rows = db.Select(query)
                   .ToList();
                return rows;
            }
        }
        public tbl_RoomPriceLevel GetRoomPriceLevelById(int id)
        {
            //search again
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_RoomPriceLevel>().Where(e => e.Id==id);
                tbl_RoomPriceLevel rows = db.Select(query).FirstOrDefault();
                return rows;
            }
        }
        public List<tbl_Room_Type> GetAllRoomType()
        {
            //search again
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Room_Type>().OrderBy(e => e.Id);
                if (!comm.IsSuperAdmin()) query = query.Where(e => e.HotelID == comm.GetHotelId());
                List<tbl_Room_Type> rows = db.Select(query)
                   .ToList();
                return rows;
            }
        }
        public tbl_Room_Type GetRoomTypeByRoomID(int roomid)
        {
            //search again
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Room_Type>().Join<tbl_Room>((e1,e2)=>e1.Id==e2.RoomType_ID && e2.Id==roomid);

                tbl_Room_Type rows = db.Select<tbl_Room_Type>(query).SingleOrDefault();
                return rows;
            }
        }
        /// <summary>
        /// thực hiện tìm chính sách gia cho 1 loại phòng nào đấy: tính theo ngày check-in (ngày đến thôi)
        /// </summary>
        /// <param name="roomid"></param>
        /// <param name="roomid"></param>
        /// <returns>Trả về giá theo ngày, đếm tháng</returns>

        public tbl_RoomPriceLevel GetPrice(int roomTypeID, DateTime dtIn)
        {
            DayOfWeek dayOfWeek= dtIn.DayOfWeek;
            string iDayOfWeek=CommService.ConvertToInt(dayOfWeek).ToString();
            //search again
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_RoomPriceLevel>()
                    .Where(e => e.RoomTypeID == roomTypeID 
                        && e.Status==true 
                        && e.dayOfWeeks.Contains(iDayOfWeek) 
                        && (e.dateFrom <= dtIn) 
                        && (e.dateTo >= dtIn)
                        )
                    .OrderByDescending(e => e.Id);

                tbl_RoomPriceLevel rows = db.Select<tbl_RoomPriceLevel>(query).FirstOrDefault();
                return rows;
            }
        }
        /// <summary>
        /// thực hiện tìm chính sách gia cho 1 loại phòng nào đấy: tính theo ngày check-in (ngày đến thôi)
        /// </summary>
        /// <param name="roomid"></param>
        /// <param name="roomid"></param>
        /// <returns>Trả về giá theo giờ</returns>

        public List<tbl_RoomPriceLevel_Hour> GetPriceHour(int priceID)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_RoomPriceLevel_Hour>()
                    .Where(e => e.RoomPriceLevelID == priceID)
                    .OrderBy(e => e.Id);
                return db.Select<tbl_RoomPriceLevel_Hour>(query).ToList();
            }
        }
        public static int ConvertToInt(DayOfWeek dayOfWeek)
        {
            if (dayOfWeek == DayOfWeek.Monday) return 2;
            if (dayOfWeek == DayOfWeek.Tuesday) return 3;
            if (dayOfWeek == DayOfWeek.Wednesday) return 4;
            if (dayOfWeek == DayOfWeek.Thursday) return 5;
            if (dayOfWeek == DayOfWeek.Friday) return 6;
            if (dayOfWeek == DayOfWeek.Saturday) return 7;
            if (dayOfWeek == DayOfWeek.Sunday) return 8;
            return 0;
        }
        public List<tbl_Room> GetAllRoomByTypeId(int roomtypeid)
        {
            //search again
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Room>().Where(e2=>e2.RoomType_ID==roomtypeid);
                return db.Select(query).ToList<tbl_Room>();
            }
        }
        public tbl_RoomUsing GetCheckInIDByRoomID(int roomid)
        {
            //search again
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_RoomUsing>().Where(e2=>e2.status==CheckInStatus.OK && e2.roomid==roomid);
                return db.Select(query).FirstOrDefault();
            }
        }
        
        public static DateTime ConvertStringToDate(string p)
        {
            try
            { 
               if (p.IndexOf(":") != -1) return DateTime.ParseExact(p, "dd/MM/yyyy HH:mm", null);
                return DateTime.ParseExact(p, "dd/MM/yyyy",null);
            }
            catch (Exception ex) { return new DateTime(1900,01,01); }
        }
        public tbl_Reservation_Room  GetReservationIDByRoomID(int roomid)
        {
            //search again
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Reservation_Room>().
                    Where(e2 => e2.RoomID == roomid 
                        && e2.SysHotelID==comm.GetHotelId()
                        && (e2.ReservationStatus == ReservationStatus.CONFIRM || e2.ReservationStatus == ReservationStatus.WAITING));
                return db.Select(query).FirstOrDefault();
            }
        }
        /// <summary>
        /// check xem 1 booking đã okie chưa?
        /// </summary>
        /// <param name="roomid"></param>
        /// <param name="dtFrom"></param>
        /// <param name="dtTo"></param>
        /// <returns></returns>
        public bool checkRoomIsAvailableForBooking(int reservationid, int roomid, DateTime dtFrom, DateTime dtTo, IDbConnection db1 = null)
        {
            //throw new NotImplementedException();tbl_Reservation_Customer_Rel
             //search again
            using (var db = db1?? _connectionData.OpenDbConnection())
            {
                if (reservationid == 0)//nếu là thêm mới
                {
                    //check về lịch book
                    var query = db.From<tbl_Reservation_Customer_Rel>().Where(e2 => e2.status == ReservationStatus.CONFIRM
                        && e2.roomid == roomid
                        && ((e2.datePlanFrom <= dtFrom && e2.datePlanTo >= dtFrom)
                            || (e2.datePlanFrom <= dtTo && e2.datePlanTo >= dtTo)
                            || (e2.datePlanFrom >= dtFrom && e2.datePlanTo <= dtTo)
                            ));

                    bool isOK = db.Select(query).Count() > 0;
                    if (isOK) return isOK;


                    //check về người đang ở                   
                    var queryRoom = db.From<tbl_Room>().Where(e2 => e2.status == RoomStatus.LIVE
                        && e2.Id == roomid
                         && ((e2.datePlanFrom <= dtFrom && e2.datePlanTo >= dtFrom)
                            || (e2.datePlanFrom <= dtTo && e2.datePlanTo >= dtTo)));
                    return db.Select(queryRoom).Count() > 0;

                   
                }
                else //nếu là cập nhật
                {
                    var query = db.From<tbl_Reservation_Customer_Rel>().Where(e2 => e2.reservationID!=reservationid && e2.status == ReservationStatus.CONFIRM
                           && e2.roomid == roomid
                            && ((e2.datePlanFrom <= dtFrom && e2.datePlanTo >= dtFrom)
                            || (e2.datePlanFrom <= dtTo && e2.datePlanTo >= dtTo)
                            || (e2.datePlanFrom >= dtFrom && e2.datePlanTo <= dtTo)
                            ));
                    bool isOK = db.Select(query).Count() > 0;
                    if (isOK) return isOK;


                    //check về người đang ở                   
                    var queryRoom = db.From<tbl_Room>().Where(e2 => e2.status == RoomStatus.LIVE
                        && e2.Id == roomid
                        && ((e2.datePlanFrom <= dtFrom && e2.datePlanTo >= dtFrom)
                            || (e2.datePlanFrom <= dtTo && e2.datePlanTo >= dtTo)
                            || (e2.datePlanFrom >= dtFrom && e2.datePlanTo <= dtTo)
                            ));
                    return db.Select(queryRoom).Count() > 0;
                }
            }
        }
        public bool checkRoomNotAvailable(int roomid, DateTime dtFrom, DateTime dtTo,IDbConnection db1=null)
        {
            //throw new NotImplementedException();tbl_Reservation_Customer_Rel
            //search again
            if (db1 == null)
            {
                using (var db =  _connectionData.OpenDbConnection())
                {
                    //check về lịch book
                    var query = db.From<tbl_Reservation_Customer_Rel>().Where(e2 => e2.status == ReservationStatus.CONFIRM
                        && e2.roomid == roomid);
                    var list1 = db.Select(query);

                    bool isOK = list1.Where(e2 => ((e2.datePlanFrom <= dtFrom && e2.datePlanTo >= dtFrom)
                        || (e2.datePlanFrom <= dtTo && e2.datePlanTo >= dtTo)
                        || (e2.datePlanFrom >= dtFrom && e2.datePlanTo <= dtTo)
                        )).Count() > 0;

                    //bool isOK = db.Select(query).Count() > 0;
                    if (isOK) return isOK;


                    //check về người đang ở                   
                    var queryRoom = db.From<tbl_Room>().Where(e2 => e2.status == RoomStatus.LIVE
                        && e2.Id == roomid);
                    var list2 = db.Select(queryRoom);

                    isOK = list2.Where(e2 => ((e2.datePlanFrom <= dtFrom && e2.datePlanTo >= dtFrom)
                       || (e2.datePlanFrom <= dtTo && e2.datePlanTo >= dtTo)
                       || (e2.datePlanFrom >= dtFrom && e2.datePlanTo <= dtTo)
                       )).Count() > 0;
                    //return db.Select(queryRoom).Count() > 0;       
                    return isOK;
                }
            }
            else 
            {
                var db = db1;
                //check về lịch book
                var query = db.From<tbl_Reservation_Customer_Rel>().Where(e2 => e2.status == ReservationStatus.CONFIRM
                    && e2.roomid == roomid);
                var list1 = db.Select(query);

                bool isOK = list1.Where(e2 => ((e2.datePlanFrom <= dtFrom && e2.datePlanTo >= dtFrom)
                    || (e2.datePlanFrom <= dtTo && e2.datePlanTo >= dtTo)
                    || (e2.datePlanFrom >= dtFrom && e2.datePlanTo <= dtTo)
                    )).Count() > 0;

                if (isOK) return isOK;


                //check về người đang ở                   
                var queryRoom = db.From<tbl_Room>().Where(e2 => e2.status == RoomStatus.LIVE
                    && e2.Id == roomid);
                var list2 = db.Select(queryRoom);

                isOK = list2.Where(e2 => ((e2.datePlanFrom <= dtFrom && e2.datePlanTo >= dtFrom)
                    || (e2.datePlanFrom <= dtTo && e2.datePlanTo >= dtTo)
                    || (e2.datePlanFrom >= dtFrom && e2.datePlanTo <= dtTo)
                    || (e2.datePlanFrom >= dtFrom && e2.datePlanTo <= dtTo)
                    )).Count() > 0;
                //return db.Select(queryRoom).Count() > 0;       
                return isOK;
            }
        }
        public string GetSupplierCode()
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Supplier>().OrderByDescending(e => e.Id);
                var obj = db.Select(query).FirstOrDefault();
                if (obj == null) return "FS" + comm.GetHotelCode() + "000001";
                return "FS" + comm.GetHotelCode() + CommService.fitTo6(obj.Id + 1);
            }
        }
        public string GetProductCode()
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Product>().OrderByDescending(e => e.Id);
                var obj = db.Select(query).FirstOrDefault();
                if (obj == null) return "FP" + comm.GetHotelCode() + "000001";
                return "FP" + comm.GetHotelCode() + CommService.fitTo6(obj.Id + 1);
            }
        }
        public tbl_HotelsConfig getConfigHotel()
        {
            tbl_HotelsConfig oConfig = null;
            //search again
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_HotelsConfig>().Where(e2 =>e2.SysHotelID==comm.GetHotelId());
                oConfig= db.Select(query).FirstOrDefault();
            }
            if (oConfig == null) oConfig = new tbl_HotelsConfig()
            {
                startCheckin = 12,
                startCheckout = 10,
                startNight1 = 21,
                startNight2 = 5,
                startRoundMain = 15,
                startRoundExatra = 15
            };
            return oConfig;
        }
    }
}