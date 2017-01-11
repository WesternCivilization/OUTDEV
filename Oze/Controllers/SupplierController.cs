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
    public class SupplierController : BaseController
    {
        // GET: Suppliers
        [HttpGet]
        public ViewResult Index()
        {
            return View("Index");
        }

        [HttpGet]
        public ActionResult List(int length, int start,string search)
        {
            SupplierService svrSupplier= (new SupplierService()) ;
            List<tbl_Supplier> data = svrSupplier.getAll(new PagingModel() { offset = start, limit = length, search = search });
            int recordsTotal = (int)svrSupplier.countAll(new PagingModel() { offset = start, limit = length, search = search });
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
            tbl_Supplier obj = (new SupplierService()).GetSupplierByID(id);
            if (obj == null) obj = (new SupplierService()).InitEmpty();
            return PartialView("EditSupplier", obj);
        }       
        public ActionResult Update(tbl_Supplier obj)
        {
            int result = new SupplierService().UpdateOrInsertSupplier(obj);
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult GetDetail(string id)
        {
            tbl_Supplier obj = new SupplierService().GetSupplierByID(id);
            return PartialView("DetailSupplier",obj);
        }
        public JsonResult Delete(string id)
        {
            int result = new SupplierService().DeleteSupplier(int.Parse(id));
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