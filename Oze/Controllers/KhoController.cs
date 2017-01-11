using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using oze.data;
using Oze.Models;
using Oze.Services;
using Oze.Services.StoreManagerService;

namespace Oze.Controllers
{
    public class KhoController : Controller
    {
        StoreService service = new StoreService();
        // GET: Store
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetProductInfo(int ProductId)
        {
            var product = service.GetProduct(ProductId);
            return Json(product, JsonRequestBehavior.AllowGet);
        }

        #region Xuất kho
        public ActionResult DanhSachPhieuXuat()
        {
            return View();
        }
        public ActionResult XuatKho()
        {
            return View();
        }

        public JsonResult ThemPhieuXuat(Order order, List<OrderDetail> orderdetails)
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
                DatePayment = Share.Todate(order.NgayChungTu),
                TypeOrder = 2
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
                    ManufactureDate = Share.Todate(item.NgaySanXuat),
                    ExpirationDate = Share.Todate(item.HanSuDung),

                });
            }
            var msg = "";
            var rs = service.Xuatkho(tblorder, listDetail, ref msg);
            if (string.IsNullOrEmpty(msg))
                msg = "Tạo phiếu lỗi";
            return Json(new { ResponseCode = (rs ? "01" : "00"), Message = (rs ? "Tạo phiếu thành công" : msg) }, JsonRequestBehavior.AllowGet);
            return null;

        }

        [HttpGet]
        public ActionResult DanhSachXuatKho(int length, int start, string search, StoreInputSearchModels model)
        {


            int recordsTotal = 0;
            var data = service.GetAll(new PagingModel() { offset = start, limit = length, search = "" }, Share.Todate(model.FromDate), Share.Todate(model.ToDate), model.Keyword, 2, out recordsTotal);

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

        public JsonResult MaPhieuXuat()
        {
            return Json(service.GetOutOrderCode(), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Nhập kho
        public ActionResult DanhSachPhieuNhap()
        {
            return View();
        }

        public ActionResult NhapKho()
        {
            return View();
        }

        [HttpGet]
        public ActionResult DanhSachNhapKho(int length, int start, string search, StoreInputSearchModels model)
        {


            int recordsTotal = 0;
            var data = service.GetAll(new PagingModel() { offset = start, limit = length, search = "" }, Share.Todate(model.FromDate), Share.Todate(model.ToDate), model.Keyword, 1, out recordsTotal);

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
        public JsonResult GetDetail(int Id)
        {
            var data = service.GetDetail(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOrderCode()
        {
            return Json(service.GetOrderCode(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ThemPhieuNhap(Order order, List<OrderDetail> orderdetails)
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
                DatePayment = Share.Todate(order.NgayChungTu),
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
                    ManufactureDate = Share.Todate(item.NgaySanXuat),
                    ExpirationDate = Share.Todate(item.HanSuDung),

                });
            }
            var rs = service.StoreInput(tblorder, listDetail);
            return Json(new { ResponseCode = (rs ? "01" : "00"), Message = (rs ? "Tạo phiếu thành công" : "Tạo phiếu lỗi") }, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region  Chuyen kho

        public JsonResult GetTranferDetail(int Id)
        {
            var data = service.GetOrderDetail(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult DanhSachChuyenKho(int length, int start, string search, StoreInputSearchModels model)
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

        public JsonResult GetTransferCode()
        {
            return Json(service.GetTransferCode(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DanhSachPhieuChuyen()
        {
            return View();
        }

        public ActionResult ChuyenKho()
        {
            return View();
        }


        public JsonResult ThemPhieuChuyenKho(Order order, List<OrderDetail> orderdetails)
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


        #endregion

        #region Bù định mức tự động

        public ActionResult BuDinhMuc()
        {
            return View();
        }
        public JsonResult LayTonKho(int storeId, int productId)
        {
            var products = service.GetallProducts();//lấy tất cả các sản phẩm
            if (productId != 0)
            {
                products = products.Where(p => p.Id == productId).ToList();
            }
            var stores = service.GetAllStore().Where(p => p.typeStore == 2);//lấy tất cả các minibar

            if (storeId != 0)
                stores = stores.Where(p => p.Id == storeId);
            var item = service.GetStoreProductList(storeId);//lấy tồn kho
            var itemConfig = service.GetStoreProductListConfig(storeId);//lấy cấu hình

            if (storeId != 0)
                itemConfig = itemConfig.Where(p => p.storeid == storeId).ToList();//nếu lọc theo kho

            List<dynamic> Itemlist = new List<dynamic>();
            foreach (var store in stores)//duyệt qua các minibar
            {
                var productConfig = new StoreManagerService().GetProductAllByStore(store.Id);//lấy các sản phẩm thuộc minibar này
                productConfig.Where(e => e.Status == 1);//chỉ lấy các chú thuộc

                if (store.minQuality == null)
                    store.minQuality = 0;
                var tonkho = item.Where(p => p.storeid == store.Id).ToList();//lấy tốn kho theo minibar này
                List<dynamic> item2 = new List<dynamic>();

                foreach (var product in productConfig)//duyệt quá các sản phẩm trong minibar đó
                {
                    var itemTonkho = tonkho.FirstOrDefault(p => p.productid == product.ID);//lấy tồn kho theo sp này
                    var itemConfigDinhMuc = itemConfig.FirstOrDefault(p => p.productid == product.ID);//item config của sản phẩm trong kho này
                    int dinhmuc = 0;
                    if (itemConfigDinhMuc != null) dinhmuc = itemConfigDinhMuc.minimize ?? 0;

                    if (itemTonkho != null)//nếu !=null thì đưa vào bằng cách trừ đi, ko có thì thôi
                    {
                        item2.Add(new
                        {
                            StoreId = store.Id,
                            ProductId = product.ID,
                            Quantity = itemTonkho.quantity,
                            ProductName = product.Name,
                            Thieu = (itemTonkho.quantity >= dinhmuc ? 0 : dinhmuc - itemTonkho.quantity)
                        });
                    }
                    else
                    {
                        item2.Add(new
                        {
                            StoreId = store.Id,
                            ProductId = product.ID,
                            Quantity = 0,
                            ProductName = product.Name,
                            Thieu = dinhmuc
                        });
                    }
                }

                Itemlist.Add(new { Store = store, TonKho = item2 });
            }
            return Json(Itemlist, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ThemPhieuBuDinhMuc(int storeid, List<BukhoModel> products)
        {
            {
                var tblorder = new tbl_TransferOrder
                {
                    SysHotelID = CommService.GetHotelId(),
                    CreatorID = CommService.GetUserId(),
                    DateCreated = DateTime.Now,
                    Status = 1,
                    SupplierID = 0,
                    OrderCode = service.GetTransferCode(),
                    //  SupplierCode = order.SoChungTu,
                    InputDate = DateTime.Today,
                };
                List<tbl_TransferOrderDetail> listDetail = new List<tbl_TransferOrderDetail>();

                var product = service.GetallProducts();
                var cate = service.GetAllCategories();
                foreach (var item in products)
                {
                    var p1 = product.FirstOrDefault(p => p.Id == item.ProductId);
                    var c = cate.FirstOrDefault(p => p.Id == p1?.ProductCateID);
                    listDetail.Add(new tbl_TransferOrderDetail
                    {
                        SysHotelID = CommService.GetHotelId(),
                        item = p1 != null ? p1.Name : "",
                        catalogitemid = p1?.ProductCateID,
                        catalogitem = c != null ? c.Name : "",
                        DateCreated = DateTime.Now,
                        CreatorID = CommService.GetUserId(),
                        quantity = item.Quantity,
                        itemid = item.ProductId,
                        StoreID = item.StoreId,
                        FromStoreId = storeid,
                        Price = Convert.ToInt32(p1?.PriceOrder),



                    });
                }
                var msg = "";
                var rs = service.StoreTransfer(tblorder, listDetail);
                if (string.IsNullOrEmpty(msg))
                    msg = "Tạo phiếu lỗi";
                return Json(new { ResponseCode = (rs ? "01" : "00"), Message = (rs ? "Tạo phiếu thành công" : msg) }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}