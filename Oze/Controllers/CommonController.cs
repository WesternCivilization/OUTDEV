using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Oze.Models;
using Oze.AppCode.BLL;
using Oze.AppCode.DAL;
using System.Data;
using oze.data;
using Oze.Services;
using System.Threading;

namespace Oze.Controllers
{
    public class CommonController : BaseController
    {
        // GET: Units
        [HttpGet]
        public ViewResult Index()
        {
            return View("Index");
        }
        /// <summary>
        /// lấy checkin theo phòng đang ở
        /// </summary>
        /// <param name="roomtypeid"></param>
        /// <returns></returns>
        public JsonResult GetCheckInIDByRoomID(int roomid)
        {
            tbl_RoomUsing lst = new CommService().GetCheckInIDByRoomID(roomid);
            return Json(new { result = lst }, JsonRequestBehavior.AllowGet);
        }
       
        /// <summary>
        /// lấy reservationid theo phòng đang đặt
        /// </summary>
        /// <param name="roomtypeid"></param>
        /// <returns></returns>
        public JsonResult GetReservationIDByRoomID(int roomid)
        {
            tbl_Reservation_Room lst = new CommService().GetReservationIDByRoomID(roomid);
            return Json(new { result = lst }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// lấy ds phòng theo hạng phòng
        /// </summary>
        /// <param name="roomtypeid"></param>
        /// <returns></returns>
        public JsonResult GetAllRoomByRoomTypeID(int roomtypeid)
        {
            List<tbl_Room> lst = new CommService().GetAllRoomByTypeId(roomtypeid);
            return Json(new { result = lst }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// lấy giá api theo thời gian và hạng phòng
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="roomtypeid"></param>
        /// <returns></returns>
        public JsonResult GetPriceByDateInAndRoomType(string datetime, int roomtypeid)
        {
            tbl_RoomPriceLevel result = new CommService().GetPrice(roomtypeid, CommService.ConvertStringToDate(datetime));
            tbl_RoomPriceLevel_Hour hours = null;
            if (result != null) hours = new CommService().GetPriceHour(result.Id).FirstOrDefault();

            return Json(new { result = result, hours = hours }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// lấy giá theo thời gian và id phòng
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="roomid"></param>
        /// <returns></returns>
        public JsonResult GetPriceByDateInAndRoom(string datetime, int roomid)
        {
            tbl_Room_Type o = new CommService().GetRoomTypeByRoomID(roomid);
            if (o == null) return Json(new { }, JsonRequestBehavior.AllowGet);
            else
            {
                return GetPriceByDateInAndRoomType(datetime, o.Id);
            }
        }
        /// <summary>
        /// lấy hạng phòng theo id phòng
        /// </summary>
        /// <param name="roomid"></param>
        /// <returns></returns>
        public JsonResult GetRoomTypeByRoomID(int roomid)
        {
            tbl_Room_Type o = new CommService().GetRoomTypeByRoomID(roomid);
            return Json(new { result=o}, JsonRequestBehavior.AllowGet);
        }
    }
}