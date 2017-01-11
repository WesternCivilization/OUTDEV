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
    public class ProductController : BaseController
    {
        // GET: Products
        [HttpGet]
        public ViewResult Index()
        {
            return View("Index");
        }

        [HttpGet]
        public ActionResult List(int length, int start, string search)
        {
            ProductService svrProducts = (new ProductService());
            List<view_DetailProduct> data = svrProducts.getAll(new PagingModel() { offset = start, limit = length, search = search });
            int recordsTotal = (int)svrProducts.countAll(new PagingModel() { offset = start, limit = length, search = search });
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
        public PartialViewResult Edit(string id)
        {
            view_DetailProduct obj = (new ProductService()).GetViewProductByID(id);
            if (obj == null) obj = new view_DetailProduct();
            return PartialView("EditProduct", obj);
        }
        public ActionResult Update(view_DetailProduct obj)
        {
            int result = (new ProductService()).UpdateOrInsertProduct(obj);
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult GetDetail(string id)
        {
            view_DetailProduct obj = new ProductService().GetViewProductByID(id);
            //List< tbl_Hotel> objCatalog = new HotelService().GetAll(new PagingModel() { offset = 0, limit = 100, search = "" });
            return PartialView("DetailProduct", obj);
        }
        public JsonResult Delete(string id)
        {
            int result = new ProductService().DeleteProduct(int.Parse(id));
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
    }
}