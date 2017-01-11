using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Oze.AppCode.BLL;
using Oze.AppCode.DAL;
using Oze.Models;
using Oze.Services;
using oze.data;

namespace Oze.Controllers
{
    public class PriceEstimateController : Controller
    {
        // GET: Units
        [HttpGet]
        public ViewResult Index()
        {
            return View("Index");
        }
        // GET: trang đặt phòng
        [HttpGet]
        public ViewResult DatPhong()
        {
            return View("datphong");
        }
        // GET: trang ds đặt phòng
        [HttpGet]
        public ViewResult DanhSachDatPhong()
        {
            return View("danhsachdatphong1");
        }
        [HttpGet]
        public ViewResult DanhSachSeDen()
        {
            return View("danhsachdatphong2");
        }
        [HttpGet]
        public PartialViewResult AssignRoom(int id)
        {
            view_Customer_DatPhong_Detail result = new ReservationService().GetDatTruocDetail(id);
            return PartialView("AssignRoom", result);
        }
        [HttpGet]
        public PartialViewResult EditRoomMate(int id)
        {
            var objRoomate = new ReservationService().GetRoomMateDetail(id);
            return PartialView("EditRoomMate", objRoomate);
        }
        public ViewResult DatPhongDetail(int id)
        {
            view_Customer_DatPhong_Detail result = new ReservationService().GetDatTruocDetail(id);
            return View("datphongdetail", result);
        }
        [HttpPost]
        public ActionResult DatTruoc(ReservationRoomModel reservation)
        {
           // int result = new ReservationService().DatTruoc(reservation);
            return Json(new { result = 1, message="" }, JsonRequestBehavior.AllowGet);
        }        
        [HttpPost]
        public ActionResult CheckIn(ReservationRoomModel reservation)
        {
            JsonRs result = new ReservationService().CheckIn(reservation);
            return Json(new { result = int.Parse(result.Status), mess = result.Message }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CheckInByReservationID(int reservationid)
        {
            JsonRs result = new ReservationService().CheckInByReservationID(reservationid);
            return Json(new { result = result, message = "" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult HuyDatPhong( int id,string note)
        {
            int result = new ReservationService().CancelReservation(id,note);
            return Json(new { result = result, message = "" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CancelCheckIn(int id, string note)
        {
            int result = new ReservationService().CancelCheckIn(id, note);
            return Json(new { result = result, message = "" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AddRoomMate(CustomerModel obj, int checkinid)
        {
            int result = new ReservationService().AddToRoomMate(obj, checkinid);
            return Json(new { result = result, message = "" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GanPhong(int reservationid,int roomid)
        {
            JsonRs result = new ReservationService().AssignRoom(reservationid, roomid);
            return Json(new { result = int.Parse(result.Status), mess = result.Message }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult searchDanhsachdatphong(int length, int start, string search, string code, int status, int bydate, string dtFrom, string dtTo, int roomid, int roomtypeid)
        {
            ReservationService svrUnit = (new ReservationService());
           PagingBookingModel p= PagingBookingModel.initFrom(length, start, search, code, status, bydate, dtFrom, dtTo, roomid, roomtypeid);
            List<view_Customer_DatPhong_Detail> data = svrUnit.getAll(p);
            int recordsTotal = (int)svrUnit.countAll(p);
            int recordsFiltered = recordsTotal;
            int draw = 1;
            try { draw = int.Parse(Request.Params["draw"]); }
            catch { }
            return Json(new
            {
                draw,
                recordsTotal,
                recordsFiltered,
                data
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult estimatePrice(int length, int start, int typePrice, string dtFrom, string dtTo, int roomid, int roomtypeid,int adult,int children)
        {
           
            EstimatePrice1Service svrPrice = (new EstimatePrice1Service());
            PagingBookingModelPrice p = PagingBookingModelPrice.initNew(length, start, "", "", 0, 0, dtFrom, dtTo, roomid, roomtypeid,typePrice,adult,children);
            List<RoomPriceEstimateModel> data = svrPrice.getAll(p);
           

            int recordsTotal = data.Count;// (int)svrPrice.countAll(p);
            int recordsFiltered = recordsTotal;
            int draw = 1;
            try { draw = int.Parse(Request.Params["draw"]); }
            catch { }
            return Json(new
            {
                draw,
                recordsTotal,
                recordsFiltered,
                data
            }, JsonRequestBehavior.AllowGet);
        }
        /*
        public PartialViewResult Edit(string id)
        {
            tbl_U obj = (new UnitService()).GetUnitByID(id);
            if (obj == null) obj = (new UnitService()).InitEmpty();
            return PartialView("EditUnit", obj);
        }
        
        public PartialViewResult GetDetail(string id)
        {
            tbl_Unit obj = new UnitService().GetUnitByID(id);
            return PartialView("DetailUnit", obj);
        }
        public JsonResult Delete(string id)
        {
            int result = new UnitService().DeleteUnit(int.Parse(id));
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }
        public bool checkValidate(string username, string email, ref string kq)
        {
            bool rs = true;
            if (string.IsNullOrEmpty(username))
            {
                kq += string.Format(" + tên đăng nhập ", Environment.NewLine);
                rs = false;
            }
            if (string.IsNullOrEmpty(email))
            {
                kq += string.Format(" + email ", Environment.NewLine);
                rs = false;
            }
            return rs;
        }
         */
    }
}