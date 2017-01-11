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
    public class ProductGroupController : BaseController
    {
        // GET: ProductGroups
        [HttpGet]
        public ViewResult Index()
        {
            return View("Index");
        }

        [HttpGet]
        public ActionResult List(int length, int start,string search)
        {
            ProductGroupService svrProductGroup= (new ProductGroupService()) ;
            List<tbl_ProductGroup> data = svrProductGroup.getAll(new PagingModel() { offset = start, limit = length, search = search });
            int recordsTotal = (int)svrProductGroup.countAll(new PagingModel() { offset = start, limit = length, search = search });
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
            tbl_ProductGroup obj = (new ProductGroupService()).GetProductGroupByID(id);
            if (obj == null) obj = (new ProductGroupService()).InitEmpty();
            return PartialView("EditProductGroup", obj);
        }       
        public ActionResult Update(tbl_ProductGroup obj)
        {
            int result = new ProductGroupService().UpdateOrInsertProductGroup(obj);
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult GetDetail(string id)
        {
            tbl_ProductGroup obj = new ProductGroupService().GetProductGroupByID(id);
            return PartialView("DetailProductGroup",obj);
        }
        public JsonResult Delete(string id)
        {
            int result = new ProductGroupService().DeleteProductGroup(int.Parse(id));
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