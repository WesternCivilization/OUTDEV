using oze.data;
using Oze.AppCode.BLL;
using Oze.AppCode.DAL;
using Oze.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Oze.Controllers
{
    public class ReceptionistController : Controller
    {
        CDatabase data = new CDatabase();
        protected int UserID;
        protected static string HotelCode = "";
        
        //public ReceptionistController()
        //{
        //    HotelCode = Session[CConfig.SESSION_HOTELCODE] == null ? "OzeHotelxxxx" : Session[CConfig.SESSION_HOTELCODE].ToString();
        //}
        //
        // GET: /Receptionist/
        public ActionResult Index()
        {


            return View();
        }
        public ActionResult BookingList()
        {
            try
            {
                //Lấy UserID Đăng nhập
                UserID = Session[CConfig.SESSION_USERID] == null ? 1 : Convert.ToInt32(Session[CConfig.SESSION_USERID]);
                HotelCode = Session[CConfig.SESSION_HOTELCODE] == null ? "OzeHotelxxxx" : Session[CConfig.SESSION_HOTELCODE].ToString();
                //Lay danh sach phong
                ViewData["Room"] = new SelectList(data.GetAllRoom(), "ID", "Name");
                //Lấy list loai phong
                ViewData["hangphong"] = new SelectList(data.GetList_RoomType(UserID), "Value", "Name");
                //Lấy danh sách loại hình
                ViewData["loaihinh"] = new SelectList(data.GetList_ReservationType(UserID), "Value", "Name");
                //Lấy danh sách trạng thái đặt phòng
                ViewData["trangthai"] = new SelectList(data.GetList_ReservationStatus(UserID), "Value", "Name");
                //ThichPV Test 1234
            }
            catch (Exception ex)
            {
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
                return RedirectToAction("Index", "FloorPlan");
            }
            return View();
        }

        public ActionResult Onchage_RoomType(string RoomTypeID)
        {
            if(RoomTypeID != null)
            {
                //Lay danh sach phong theo hạng phòng
                ViewData["Room"] = new SelectList(data.Getview_GetRoomForRoomType(RoomTypeID), "ID", "Name");
            }
            return View();
        }

        public JsonResult SearchBooking(ReservationRoomModel mdReservationRoom, int CurrentPage, int NumInPage)
        {
            object[] message = new object[1];
            UserID = Session[CConfig.SESSION_USERID] == null ? 1 : Convert.ToInt32(Session[CConfig.SESSION_USERID]);
            HotelCode = Session[CConfig.SESSION_HOTELCODE] == null ? "OzeHotelxxxx" : Session[CConfig.SESSION_HOTELCODE].ToString();
            try
            {
                if (mdReservationRoom != null)
                {
                    message[0] = data.getObjectCustomer_Booking(mdReservationRoom, HotelCode, UserID, CurrentPage, NumInPage);
                }
            }
            catch (Exception ex)
            {
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }

            return Json(new { mess = message }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// get set room
        /// </summary>
        /// <param name="BookingCode"></param>
        /// <returns></returns>
        public JsonResult GetRoomForReservationRoom(string BookingCode)
        {
            List<string> json = new List<string>();
            DataSet ds = new DataSet();
            try
            {
                UserID = Session[CConfig.SESSION_USERID] == null ? 1 : Convert.ToInt32(Session[CConfig.SESSION_USERID]);
                HotelCode = Session[CConfig.SESSION_HOTELCODE].ToString();
                if (HotelCode == null)
                {
                    Response.Redirect("Receptionist/BookingList");
                }
                json = data.GetReservationRoom_Detail(BookingCode, HotelCode, UserID);
                ////ds = data.GetReservationRoom_Detail( BookingCode, HotelCode, UserID);
                ////Thông tin đặt phòng
                //ViewData["BookingInfo"] = json[0];
                //// thong tin phong
                //ViewData["SetRoom"] = json[1];
                ////Thông tin khách hàng
                //ViewData["CustomerInfo"] = json[2];
            }
            catch (Exception ex)
            {
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }
            return Json(new { mess = json }, JsonRequestBehavior.AllowGet);
        }
        // POST: /Receptionist/SetRoomForReservationRoom
        public JsonResult SetRoomForReservationRoom(tbl_Reservation_Room_tbl_Room_Rel model)
        {
            object[] message = new object[1];
            try
            {
                UserID = Session[CConfig.SESSION_USERID] == null ? 1 : Convert.ToInt32(Session[CConfig.SESSION_USERID]);
                model.HotelCode = HotelCode;

                if (model != null & ModelState.IsValid)
                {
                    message[0] = data.Create_RoomforReservationRoom(model, UserID);
                }
            }
            catch (Exception ex)
            {
                message[0] = "Gán phòng không thành công!";
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }

            return Json(new { mess = message }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Transfer_SetRoomForBooking(tbl_Reservation_Room ReserModel, tbl_Reservation_Room_tbl_Room_Rel Rel_model)
        {
            object message = new object();
            try
            {
                UserID = Session[CConfig.SESSION_USERID] == null ? 1 : Convert.ToInt32(Session[CConfig.SESSION_USERID]);

                if (ReserModel != null & Rel_model != null & ModelState.IsValid)
                {
                    message = data.Transfer_SetRoomForBooking(ReserModel,Rel_model);
                }
            }
            catch (Exception ex)
            {
                message = "Đổi phòng phòng không thành công!";
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }

            return Json(new { mess = message }, JsonRequestBehavior.AllowGet);
        }

        // GET: Group/Detail/5
        public ActionResult DetailBooking(string BookingCode)
        {
            List<string> json = new List<string>();
            DataSet ds = new DataSet();
            try
            {
                UserID = Session[CConfig.SESSION_USERID] == null ? 1 : Convert.ToInt32(Session[CConfig.SESSION_USERID]);
                HotelCode = Session[CConfig.SESSION_HOTELCODE].ToString();

                //Lấy danh sách giá thời điểm
                ViewData["giathoidiem"] = new SelectList(data.GetList_DefineCategory("GIATHOIDIEM",UserID), "Value", "Name");
                //Lấy danh sách khung giá phòng
                ViewData["khunggiaphong"] = new SelectList(data.GetList_DefineCategory("KHUNGGIAPHONG", UserID), "Value", "Name");

                if (HotelCode == null)
                {
                    Response.Redirect("Receptionist/BookingList");
                }
                json = data.GetReservationRoom_Detail(BookingCode, HotelCode, UserID);
                //ds = data.GetReservationRoom_Detail( BookingCode, HotelCode, UserID);
                //Thông tin đặt phòng
                ViewData["BookingInfo"] = json[0];
                // thong tin phong
                ViewData["SetRoom"] = json[1];
                //Thông tin khách hàng
                ViewData["CustomerInfo"] = json[2];
            }
            catch (Exception ex)
            {
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }
            return View();
        }
    }
}