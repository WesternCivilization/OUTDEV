using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;
using oze.data;
using Oze.Models;
using Oze.Services;

namespace Oze.Controllers
{
    public class StoreInputController : Controller
    {
        StoreService service = new StoreService();
        // GET: StoreInput
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Input()
        {
            return View();
        }

        [HttpGet]
        public ActionResult List(int length, int start,string search, StoreInputSearchModels model)
        {
           
           
            int recordsTotal = 0;
            var data = service.GetAll(new PagingModel() { offset = start, limit = length, search = "" }, Share.Todate(model.FromDate), Share.Todate(model.ToDate), model.Keyword,1, out recordsTotal);
        
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


        public JsonResult GetProductInfo(int ProductId)
        {
            var product = service.GetProduct(ProductId);
            return Json(product, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Add(Order order, List<OrderDetail> orderdetails)
        {
            var tblorder = new tbl_PurchaseOrder
            {
                SysHotelID = CommService.GetHotelId(),
                CreatorID = CommService.GetUserId(),
                DateCreated = DateTime.Now,
                Status = 1,
                SupplierID = 0,
                OrderCode = order.SoPhieu,
                SupplierCode = order.SoChungTu,
                InputDate = Share.Todate(order.NgayNhapHD),
                DatePayment = Share.Todate(order.NgayChungTu) ,
                TypeOrder = 1
            };
            List<tbl_PurchaseOrderDetail> listDetail = new List<tbl_PurchaseOrderDetail>();
            foreach (var item in orderdetails)
            {
                listDetail.Add(new tbl_PurchaseOrderDetail
                {
                    SysHotelID = CommService.GetHotelId(),
                    item = item.ProductName,
                    catalogitemid = item.CateId,
                    catalogitem = item.CateName,
                    DateCreated = DateTime.Now,
                    CreatorID = CommService.GetUserId(),
                    quantity = item.Quantity,
                    itemid = item.ProductId,
                    StoreID = order.StoreId,
                    Price = item.Price,
                    ManufactureDate = Share.Todate(item.NgaySanXuat)  ,
                    ExpirationDate = Share.Todate(item.HanSuDung),

                });
            }
            var rs = service.StoreInput(tblorder, listDetail);
            return Json(new { ResponseCode = (rs ? "01" : "00"), Message = (rs ? "Tạo phiếu thành công" : "Tạo phiếu lỗi") }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDetail(int Id)
        {
            var data = service.GetDetail(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOrderCode()
        {
            return Json(service.GetOrderCode(), JsonRequestBehavior.AllowGet);
        }

    }
}