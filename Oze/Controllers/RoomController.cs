using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Oze.Models;
using Oze.AppCode.DAL;
using Oze.AppCode.BLL;

namespace Oze.Controllers
{
    public class RoomController : Controller
    {
        CRoom room = new CRoom();
        //
        // GET: /Room/
        public ActionResult Index()
        {
            List<RoomModel> list = room.GetAllRoomHotel();
            return View(list);
        }

        public ActionResult Giatheophong()
        {
            List<RoomModel> list = room.GetAllRoomHotel();
            return View(list);
        }

        public ActionResult Giatheonguoi()
        {
            List<RoomModel> list = room.GetAllRoomHotel();
            return View(list);
        }

        public ActionResult GiaHangphong()
        {
            List<RoomModel> list = new CRoom().GetAllRoomHotel();
            return View(list);
        }

        //public JsonResult GetPhutroi(List<TestModel> obj)
        //{
        //    //CRoom cr = new CRoom();

        //    //obj = cr.GetPhuTroiQuaGioTheoNgay();
        //    return Json(new { mess = "", kq = "" }, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult CreateRoom(string roomName)
        {
            string erroValidate = "Vui lòng khồng để trống tên phòng";
            if (string.IsNullOrEmpty(roomName))
            {
                return Json(new { mess = erroValidate}, JsonRequestBehavior.AllowGet);
            }
            else
            {
                erroValidate = "";
                string rs = "";
                erroValidate = room.AddRoomHotel(roomName,ref rs);
                return Json(new { mess = erroValidate,kq = rs}, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UpdateRooms(string roomName, string id)
        {
            string erroValidate = "Vui lòng khồng để trống tên phòng";
            if (string.IsNullOrEmpty(roomName))
            {
                return Json(new { mess = erroValidate }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                erroValidate = "";
                int rs = 0;
                erroValidate = room.UpdateRoomHotel(roomName, id, ref rs);
                return Json(new { mess = erroValidate, kq = rs }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeleteRooms(string id)
        {
            string erroValidate = "Vui lòng khồng để trống tên phòng";
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { mess = erroValidate }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                erroValidate = "";
                //int rs = 0;
                erroValidate = room.DeleteRoomHotel(id);
                return Json(new { mess = erroValidate}, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CreateRoomType(RoomTypeModel objRoomType)
        {
            objRoomType.Code = new Locdau().LocDauChuoi(objRoomType.Name).ToUpper();
            objRoomType.DouldBed = objRoomType.DouldBed < 0 ? 0: objRoomType.DouldBed;
            objRoomType.SingBed = objRoomType.SingBed < 0 ? 0 : objRoomType.SingBed;
            objRoomType.UserLimit = objRoomType.UserLimit < 0 ? 0 : objRoomType.UserLimit;
            string mess = "";
            string roomtype = "";
            int retcode = 0;
            new CRoomType().CreateRoomType(objRoomType, ref roomtype, ref retcode, ref mess);
            return Json(new { notifi = mess }, JsonRequestBehavior.AllowGet);
        }
        //Giá theo ngày objGTN
        //Giá qua đêm objGQD
        //Giá theo tháng objGTT
        //giá theo tháng listGiathang
        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomType"></param>
        /// <param name="objGTN"></param>
        /// <param name="objGQD"></param>
        /// <param name="objGTT"></param>
        /// <param name="listGiathang"></param>
        /// <returns></returns>
        public JsonResult CreateRoomPrice(string roomType, RoomPriceModel objGTN, RoomPriceModel objGQD, RoomPriceModel objGTT, List<RoomPriceModel> listGiathang)
        {
            return Json(new { mess = "Thêm mới giá thất bại" }, JsonRequestBehavior.AllowGet);
        }

        //Phụ trội nhận phòng sớm theo ngày objPTNPSTN
        //Phụ trội quá giờ trả theo ngày objPTQGTTN
        //Phụ trội quá giờ trả theo đêm objPTQGTTTD
        //Phụ trội nhận phòng sớm theo đêm objNPSTD
        //Phụ trội quá số lượng người lớn objQSLNL
        public JsonResult CreatePhuTroi(string roomType,List<PhuTroiModel> objPTQGTTN, List<PhuTroiModel> objPTQGTTTD, List<PhuTroiModel> objPTNPSTN, List<PhuTroiModel> objNPSTD, List<PhuTroiModel> objQSLNL)
        {
            if (string.IsNullOrEmpty(roomType))
            {
                return Json(new { mess = "Thêm mới phụ trội thất bại" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { mess = "Thêm mới phụ trội thất bại" }, JsonRequestBehavior.AllowGet);

        }

        

    }
}