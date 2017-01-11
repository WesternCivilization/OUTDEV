using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using oze.data;
using Oze.Models;
using Oze.Services;

namespace Oze.Controllers
{
    public class StoreTransferController : Controller
    {
        StoreService service = new StoreService();
        // GET: StoreTransfer
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Transfer()
        {
            return View();
        }

        public JsonResult GetTransferCode()
        {
            return Json(service.GetTransferCode(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDetail(int Id)
        {
            var data = service.GetOrderDetail(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult List(int length, int start, string search, StoreInputSearchModels model)
        {


            int recordsTotal = 0;
            var data = service.GetALlTransferOrders(new PagingModel() { offset = start, limit = length, search = "" }, Share.Todate(model.FromDate), Share.Todate(model.ToDate), model.Keyword, out recordsTotal);

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
        public JsonResult Add(Order order, List<OrderDetail> orderdetails)
        {
            var tblorder = new tbl_TransferOrder
            {
                SysHotelID = CommService.GetHotelId(),
                CreatorID = CommService.GetUserId(),
                DateCreated = DateTime.Now,
                Status = 1,
                SupplierID = 0,
                OrderCode = order.SoPhieu,
                //  SupplierCode = order.SoChungTu,
                InputDate = Share.Todate(order.NgayNhapHD),
                //  DatePayment = DateTime.ParseExact(order.NgayChungTu, "dd/MM/yyyy", CultureInfo.InvariantCulture)
            };
            List<tbl_TransferOrderDetail> listDetail = new List<tbl_TransferOrderDetail>();
            foreach (var item in orderdetails)
            {
                listDetail.Add(new tbl_TransferOrderDetail
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
                    FromStoreId = order.SrcStoreId,
                    Price = item.Price,



                });
            }
            var msg = "";
            var rs = service.StoreTransfer(tblorder, listDetail, ref msg);
            if (string.IsNullOrEmpty(msg))
                msg = "Tạo phiếu lỗi";
            return Json(new { ResponseCode = (rs ? "01" : "00"), Message = (rs ? "Tạo phiếu thành công" : msg) }, JsonRequestBehavior.AllowGet);
        }


    }
}