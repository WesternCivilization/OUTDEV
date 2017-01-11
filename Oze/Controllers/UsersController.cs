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
    public class UsersController : BaseController
    {
        // GET: SysUsers
        [HttpGet]
        public ViewResult Index()
        {
            return View("Index");
        }

        [HttpGet]
        public ActionResult List(int length, int start,string search)
        {
            SysUserService svrSysUser= (new SysUserService()) ;
            List<view_DetailUser> data = svrSysUser.getAll(new PagingModel() { offset = start, limit = length, search = search });
            int recordsTotal = (int)svrSysUser.countAll(new PagingModel() { offset = start, limit = length, search = search });
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
        public PartialViewResult Edit(string id)
        {
            view_DetailUser obj = (new SysUserService()).GetViewUserByID(id);
            if (obj == null) obj = new view_DetailUser() { IsActive=1};
            return PartialView("EditUser", obj);
        }       
        public ActionResult Update(view_DetailUser  obj)
        {

            int result = (new SysUserService()).UpdateOrInsertSysUser(obj);
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult GetDetail(string id)
        {
            view_DetailUser obj = new SysUserService().GetViewUserByID(id);
            //List< tbl_Hotel> objCatalog = new HotelService().GetAll(new PagingModel() { offset = 0, limit = 100, search = "" });
            return PartialView("DetailUser",obj);
        }
        public JsonResult Delete(string id)
        {
            int result = new SysUserService().DeleteSysUser(int.Parse(id));
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