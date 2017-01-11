using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aspose.Cells;
using oze.data;
using oze.data.Entity;
using Oze.Models;
using Oze.Models.CustomerArriveManage;
using Oze.Services;

namespace Oze.Controllers
{
    public class RoomCheckOutController : Controller
    {
        private readonly ICustomerArriveManageService _svustomerArrive;
        public RoomCheckOutController()
        {
            _svustomerArrive = new CustomerArriveManageService();///okie
        }
        // GET: RoomCheckOut
        public ActionResult Index(int checkinID)
        {
            //checkinID = 9;
            var model = new PaymentCheckOutModel();
            //thông tin khách hàng
            model.InforCustomer = _svustomerArrive.GetCustomerRoomByCheckInID(checkinID);
            if (model.InforCustomer == null)
            {
                return Redirect("CustomerPayCheckOutFalse");
            }
            var listRoomPrice = new List<RoomPriceEstimateModel>();
            //okie
            DateTime dateToArrive = DateTime.Now;
            //DateTime dateToArrive= model.InforCustomer.Leave_Date.GetValueOrDefault(DateTime.Now);
            //if (dateToArrive.CompareTo(DateTime.Now) > 0) dateToArrive = DateTime.Now;
            model.InforCustomer.Leave_Date = DateTime.Now;
            listRoomPrice = new EstimatePrice1Service().caculatePrice(model.InforCustomer.SysHotelID.GetValueOrDefault(0), model.InforCustomer.KhungGio.GetValueOrDefault(0),
                model.InforCustomer.roomid.GetValueOrDefault(0), model.InforCustomer.Room_Type_ID.GetValueOrDefault(0), model.InforCustomer.Arrive_Date.GetValueOrDefault(DateTime.Now),
                dateToArrive, -1);
            //danh sách giá phòng
            model.GetListPriceEstimate = listRoomPrice;

            var cService = _svustomerArrive.GetListCustomerServices(checkinID);
            // danh sách dịnh vụ đã sử dụng
            model.GetListCustomerServices = cService.Where(x => x.UnitID > 0).ToList();

            // danh sách dịnh vụ khác đã sử dụng
            model.GetListCustomerOtherServices = cService.Where(x => x.UnitID == 0).ToList();
            // danh sách dịch vụ
            model.GetProductList = _svustomerArrive.GetlistProduct(model.InforCustomer.SysHotelID.GetValueOrDefault(), model.InforCustomer.roomid.GetValueOrDefault(0));
            return View(model);
        }

        public ActionResult CustomerPayCheckOutFalse()
        {
            return View();
        }

        public JsonResult InsertService(int productId, int checkinID, int hotelID, int customerid, int Quantity)
        {
            //checkinID = 9;
            var rs = _svustomerArrive.InsertCustomeServer(productId, checkinID, hotelID, customerid, Quantity);
            //rs.Data=new Vw_ProductService();
            return Json(rs, JsonRequestBehavior.AllowGet);
        }
        public JsonResult InsertNewOtherService(string name, int checkinID, int hotelID, int customerid, double price)
        {
            //checkinID = 9;
            var rs = _svustomerArrive.InsertNewOtherService(name, checkinID, hotelID, customerid, price);
            //rs.Data=new Vw_ProductService();
            return Json(rs, JsonRequestBehavior.AllowGet);
        }
        public JsonResult LockCustomerService(int cussvID)
        {

            var rs = _svustomerArrive.DeleteCustomeServer(cussvID);
            //rs.Data=new Vw_ProductService();
            return Json(rs, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult ViewPrintPay(int checkinID, bool IsboolService, bool IsboolRoom,string tDate)
        {

            //[Ignore]
            //checkinID = 9;
            var model = new PaymentCheckOutModel();
            //thông tin khách hàng
            model.InforCustomer = _svustomerArrive.GetCustomerRoomByCheckInID(checkinID);
            DateTime dateToLeave;
            DateTime.TryParse(tDate, CultureInfo.GetCultureInfo("vi-vn"), DateTimeStyles.None, out dateToLeave);
            //danh sách giá phòng
            model.GetListPriceEstimate = new EstimatePrice1Service().caculatePrice(model.InforCustomer.SysHotelID.GetValueOrDefault(), model.InforCustomer.KhungGio.GetValueOrDefault(0),
                 model.InforCustomer.roomid.GetValueOrDefault(0), model.InforCustomer.Room_Type_ID.GetValueOrDefault(0), model.InforCustomer.Arrive_Date.GetValueOrDefault(),
                dateToLeave, -1); 
            model.InforCustomer.IsboolRoom = IsboolRoom;
            model.InforCustomer.IsboolService = IsboolService;
            var cService = _svustomerArrive.GetListCustomerServices(checkinID);
            // danh sách dịnh vụ đã sử dụng
            model.GetListCustomerServices = cService.ToList();

            // danh sách dịnh vụ khác đã sử dụng
            //model.GetListCustomerOtherServices = cService.Where(x => x.UnitID == 0).ToList();
            //var model = _svustomerArrive.GetCustomerRoomByCheckInID(id);
            //model.GetListCustomerServices = _svustomerArrive.GetListCustomerServices(id);
            Session["RoomByCheckInID"] = model;

            return PartialView("PrintPay", model);
        }
        public JsonResult PrintPay(int checkinID, string tDate)
        {
            //checkinID = 9;
            var model = new PaymentCheckOutModel();
            //thông tin khách hàng
            model.InforCustomer = _svustomerArrive.GetCustomerRoomByCheckInID(checkinID);

            DateTime dateToLeave;
            DateTime.TryParse(tDate, CultureInfo.GetCultureInfo("vi-vn"), DateTimeStyles.None, out dateToLeave);
            //danh sách giá phòng
            model.GetListPriceEstimate = new EstimatePrice1Service().caculatePrice(model.InforCustomer.SysHotelID.GetValueOrDefault(), model.InforCustomer.KhungGio.GetValueOrDefault(0),
                 model.InforCustomer.roomid.GetValueOrDefault(0), model.InforCustomer.Room_Type_ID.GetValueOrDefault(0), model.InforCustomer.Arrive_Date.GetValueOrDefault(),
                dateToLeave, -1);

            var cService = _svustomerArrive.GetListCustomerServices(checkinID);
            // danh sách dịnh vụ đã sử dụng
            model.GetListCustomerServices = cService.ToList();
        
            // danh sách dịnh vụ khác đã sử dụng
            //model.GetListCustomerOtherServices = cService.Where(x => x.UnitID == 0).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }


        public JsonResult PaymentCheckOut(int checkInid, string Tdate)
        {

            var model = _svustomerArrive.PaymentCheckOut(checkInid, Tdate);
            //model.GetListCustomerServices = _svustomerArrive.GetListCustomerServices(id);

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PayBilltCheckOut(int checkInid, string Tdate)
        {

            var model = _svustomerArrive.PayBillCheckOut(checkInid, Tdate);
            //model.GetListCustomerServices = _svustomerArrive.GetListCustomerServices(id);

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PayRoomPrice(string Tdate, int checkinID)
        {
            var listRoomPrice = new List<RoomPriceEstimateModel>();
            //okie
            DateTime dateToLeave;
            DateTime.TryParse(Tdate, CultureInfo.GetCultureInfo("vi-vn"), DateTimeStyles.None, out dateToLeave);
            var InforCustomer = _svustomerArrive.GetCustomerRoomByCheckInID(checkinID);
            listRoomPrice = new EstimatePrice1Service().caculatePrice(InforCustomer.SysHotelID.GetValueOrDefault(), InforCustomer.KhungGio.GetValueOrDefault(0),
                InforCustomer.roomid.GetValueOrDefault(0), InforCustomer.Room_Type_ID.GetValueOrDefault(0), InforCustomer.Arrive_Date.GetValueOrDefault(),
                dateToLeave, -1);
            var service = _svustomerArrive.GetListCustomerServices(checkinID);
            var checkin = _svustomerArrive.GetCustomerRoomByCheckInID(checkinID);
            var total = service.Sum(x => x.TotalSale.GetValueOrDefault()) + listRoomPrice.Sum(x => x.price);
            var totalpay = total - checkin.Deduction.GetValueOrDefault() - checkin.Deduction;
            var rs = new {Total= total,totalpay=totalpay, listRoomPrice = listRoomPrice};
            return Json(rs, JsonRequestBehavior.AllowGet);
        }
        #region xuất file hungpv
        public ActionResult Export(int CheckInIDExport)
        {

            // =======================================================================================================================================
            WorkbookDesigner designer = new WorkbookDesigner();
            var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Download/"), "FileMauPayCheckOut.xlsx");
            designer.Workbook = new Workbook(path);
            var worksheet = designer.Workbook.Worksheets[0];
            var lis = Session["RoomByCheckInID"] as PaymentCheckOutModel;

            worksheet.Cells["D2"].PutValue(lis.InforCustomer.CustomerName);
            worksheet.Cells["D3"].PutValue(lis.InforCustomer.Phone);
            worksheet.Cells["D4"].PutValue(DateTime.Now.ToString("dd/MM/yyyy hh:mm"));
            worksheet.Cells["G2"].PutValue(lis.InforCustomer.HotelName);
            //worksheet.Cells["A12"].PutValue("Fax  : ");
            worksheet.Cells["F14"].PutValue(lis.GetListCustomerServices.Sum(x => x.TotalSale));
            worksheet.Cells["F15"].PutValue(lis.InforCustomer.Deposit);
            worksheet.Cells["F16"].PutValue(lis.InforCustomer.Deduction);
            worksheet.Cells["F17"].PutValue(lis.GetListCustomerServices.Sum(x => x.TotalSale) - lis.InforCustomer.Deposit - lis.InforCustomer.Deduction);
            var listnewRoom = new List<Vw_ProductService>();
            var listnew = new List<Vw_ProductService>();
            var stt = 0;
            for (int i = 0; i < lis.GetListPriceEstimate.Count; i++)
            {
                listnewRoom.Add(new Vw_ProductService()
                {
                    cussvID = stt + 1,
                    Quantity = lis.GetListPriceEstimate[i].quantiy,
                    Name = lis.GetListPriceEstimate[i].dtFrom.ToString("dd/MM/yyyy hh:mm") + "-" + lis.GetListPriceEstimate[i].dtTo.ToString("dd/MM/yyyy hh:mm"),
                    SalePrice = lis.GetListPriceEstimate[i].price,
                    TotalSale = lis.GetListPriceEstimate[i].price,
                    UnitName = lis.GetListPriceEstimate[i].pricePolicyName,
                });
            }
            for (int i = 0; i < lis.GetListCustomerServices.Count; i++)
            {
                listnew.Add(new Vw_ProductService()
                {
                    cussvID = stt + 1,
                    Quantity = lis.GetListCustomerServices[i].Quantity,
                    Name = lis.GetListCustomerServices[i].Name,
                    SalePrice = lis.GetListCustomerServices[i].SalePrice,
                    TotalSale = lis.GetListCustomerServices[i].TotalSale,
                    UnitName = lis.GetListCustomerServices[i].UnitName,
                });
            }


            designer.SetDataSource("ImportRoom", listnewRoom);
            designer.SetDataSource("Import", listnew);
            //Process the markers
            designer.Process(false);
            //Save the Excel file.

            System.IO.Stream stream = Response.OutputStream;

            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-disposition", "attachment; filename=" + "thanhtoanchokhachhang" + lis.InforCustomer.CustomerName + "" + ".pdf");
            Response.Clear();
            Response.BufferOutput = true;
            designer.Workbook.Save(stream, SaveFormat.Pdf);

            Response.Flush();
            return new EmptyResult();
        }
        #endregion

        #region sendmail

        public JsonResult SendMail()
        {

            var rs = new JsonRs();

            // =======================================================================================================================================
            WorkbookDesigner designer = new WorkbookDesigner();
            var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Download/"), "FileMauPayCheckOut.xlsx");
            designer.Workbook = new Workbook(path);
            var worksheet = designer.Workbook.Worksheets[0];
            var lis = Session["RoomByCheckInID"] as PaymentCheckOutModel;

            worksheet.Cells["D2"].PutValue(lis.InforCustomer.CustomerName);
            worksheet.Cells["D3"].PutValue(lis.InforCustomer.Phone);
            worksheet.Cells["D4"].PutValue(DateTime.Now.ToString("dd/MM/yyyy hh:mm"));
            worksheet.Cells["G2"].PutValue(lis.InforCustomer.HotelName);
            //worksheet.Cells["A12"].PutValue("Fax  : ");ok
            worksheet.Cells["F14"].PutValue(lis.GetListCustomerServices.Sum(x => x.TotalSale));
            worksheet.Cells["F15"].PutValue(lis.InforCustomer.Deposit);
            worksheet.Cells["F16"].PutValue(lis.InforCustomer.Deduction);
            worksheet.Cells["F17"].PutValue(lis.GetListCustomerServices.Sum(x => x.TotalSale)- lis.InforCustomer.Deposit- lis.InforCustomer.Deduction);
            var listnewRoom = new List<Vw_ProductService>();
            var listnew = new List<Vw_ProductService>();
            var stt = 0;
            for (int i = 0; i < lis.GetListPriceEstimate.Count; i++)
            {
                listnewRoom.Add(new Vw_ProductService()
                {
                    cussvID = stt + 1,
                    Quantity = lis.GetListPriceEstimate[i].quantiy,
                    Name = lis.GetListPriceEstimate[i].dtFrom.ToString("dd/MM/yyyy hh:mm") + "-" + lis.GetListPriceEstimate[i].dtTo.ToString("dd/MM/yyyy hh:mm"),
                    SalePrice = lis.GetListPriceEstimate[i].price,
                    TotalSale = lis.GetListPriceEstimate[i].price,
                    UnitName = lis.GetListPriceEstimate[i].pricePolicyName,
                });
            }
            for (int i = 0; i < lis.GetListCustomerServices.Count; i++)
            {
                listnew.Add(new Vw_ProductService()
                {
                    cussvID = stt + 1,
                    Quantity = lis.GetListCustomerServices[i].Quantity,
                    Name = lis.GetListCustomerServices[i].Name,
                    SalePrice = lis.GetListCustomerServices[i].SalePrice,
                    TotalSale = lis.GetListCustomerServices[i].TotalSale,
                    UnitName = lis.GetListCustomerServices[i].UnitName,
                });
            }


            designer.SetDataSource("ImportRoom", listnewRoom);
            designer.SetDataSource("Import", listnew);

            //Process the markers
            designer.Process(false);
            //Save the Excel file.

            System.IO.Stream stream = Response.OutputStream;

            //Response.ContentType = "application/vnd.ms-excel";
            //Response.AddHeader("Content-disposition", "attachment; filename=" + "thanhtoanchokhachhang" + lis.CustomerName + "" + ".pdf");
            //Response.Clear();
            //Response.BufferOutput = true;
            var pathsend = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Download/"), "Hoadonthanhtoan_" + lis.InforCustomer.CustomerName + "" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".pdf");
            designer.Workbook.Save(pathsend, SaveFormat.Pdf);
            if (!string.IsNullOrEmpty(lis.InforCustomer.Email))
            {
                _svustomerArrive.SendMailHepelink(pathsend, lis.InforCustomer.Email,
                    "Vv/ Gửi Mai báo cáo thanh toán cho khách hàng", "Cảm ơn quý khách sử dụng dịch vụ của chúng tôi");
                //System.IO.File.Delete(pathsend);
                rs.Status = "01";
                rs.Message = "Gửi thành công hóa đơn cho khách hàng : " + lis.InforCustomer.CustomerName;
                return Json(rs, JsonRequestBehavior.AllowGet);
            }
            else
            {
                rs.Status = "00";
                rs.Message = "Không có thông tin email nên không thể gửi cho khách hàng " + lis.InforCustomer.CustomerName;
                return Json(rs, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion
    }
}