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
    public class SodophongController : BaseController
    {
        // GET: Rooms
        [HttpGet]
        public ViewResult Index()
        {
            return View("Index");
        }
        public PartialViewResult sodophong()
        {
            return PartialView("sodophong");
        }

        [HttpGet]
        public ActionResult List(int length, int start,string search)
        {
            //phòng ko cần, vì phòng rất it<100
            start = 0;
            length = 100;
            search = "";
            RoomService svrRoom= (new RoomService()) ;
            List<tbl_Room> data = svrRoom.getAll(new PagingModel() { offset = start, limit = length, search = search });
            int recordsTotal = (int)svrRoom.countAll(new PagingModel() { offset = start, limit = length, search = search });
            int recordsFiltered = recordsTotal;
            int draw = 1;
            try { draw = int.Parse(Request.Params["draw"]); }catch { }
            return Json(new
            {
                draw,
                recordsTotal,
                recordsFiltered,
                data
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public PartialViewResult ListRoom()
        {
            return PartialView("ListRoom");
        }
        public PartialViewResult Edit(string id)
        {
            tbl_Room obj = (new RoomService()).GetRoomByID(id);
            if (obj == null) obj = (new RoomService()).InitEmpty();
            return PartialView("EditRoom", obj);
        }       
        public ActionResult Update(tbl_Room obj)
        {
            int result = new RoomService().UpdateOrInsertRoom(obj);
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult GetDetail(string id)
        {
            tbl_Room obj = new RoomService().GetRoomByID(id);
            return PartialView("DetailRoom",obj);
        }
        public JsonResult Delete(string id)
        {
            int result = new RoomService().DeleteRoom(int.Parse(id));
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }
        public bool checkValidate(string username,string email,ref string kq){
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
    }
}