using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Oze.Models;
using Oze.AppCode.BLL;
using Oze.AppCode.DAL;
using System.Data;
using System.Globalization;
using oze.data;
using Oze.Services;
using System.Threading;
using Oze.AppCode.Util;
using Oze.Models.CustomerArriveManage;
using Oze.Models.CustomerManage;

namespace Oze.Controllers
{
    public class CustomerArriveManageController : BaseController
    {
        private readonly ICustomerArriveManageService _svustomerArrive;

        public CustomerArriveManageController()
        {
            _svustomerArrive = new CustomerArriveManageService();
        }
        // GET: Suppliers
        [HttpGet]
        public ViewResult Index()
        {
            var model = new ArriveDefault();
            model.ListRooms = _svustomerArrive.GeTblRoomsByType(comm.GetHotelId(), 0); ;
            model.listRoomTypes = _svustomerArrive.GetRoomTypes(comm.GetHotelId());
            return View("Index", model);
        }

        [HttpGet]
        public ActionResult List(int length, int start, string search, SearchCustomerArriveModel model)
        {
            int recordsTotal = 0;
            model.HotelID = comm.GetHotelId();
            List<Vw_CustomerArrive_Room> data = _svustomerArrive.getAll(new PagingModel() { offset = start, limit = length, search = search }, model, out recordsTotal);
            //int recordsTotal = (int)_svustomerArrive.countAll(new PagingModel() { offset = start, limit = length, search = search });
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
        public PartialViewResult GetInfomationRoomDetail(int id)
        {
            var model = _svustomerArrive.GetCustomerRoom(id);

            return PartialView("InfomationRoomDetail", model);
        }


        public PartialViewResult GetInfomationExChange(int id)
        {
            var model = _svustomerArrive.GetCustomerRoom(id);
            model.GetListRoom = _svustomerArrive.GeTblRoomsByType(model.SysHotelID.GetValueOrDefault(), 0).Where(x => x.Id != model.roomid.GetValueOrDefault() && x.status == 0).ToList();
            return PartialView("InfomationRoomDetail", model);
        }

        public JsonResult SelectCustomer(string search, int customerold)
        {
            var cus = _svustomerArrive.AutoCompleteCustomer(search, customerold);
            return Json(cus, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SelectRoomByType(int id)
        {
            var room = _svustomerArrive.GeTblRoomsByType(comm.GetHotelId(), id);
            return Json(room, JsonRequestBehavior.AllowGet);
        }


        #region popup thêm khách ở cùng

        public PartialViewResult AddUsingCustomers(int checkinID)
        {

            var obj = _svustomerArrive.GetRoomUsingCheckIn(checkinID);
            var model = new CustomeCheckInModel();
            model.CheckInID = obj.CheckInID.GetValueOrDefault();
            model.CustomerIdOld = obj.customerid.GetValueOrDefault();
            model.Roomid = obj.roomid.GetValueOrDefault();
            model.DOB = DateTime.Now.ToString("dd/MM/yyyy");
            model.SysHotelID = obj.SysHotelID.GetValueOrDefault();
            //model.Id = obj.Id;
            //model.Name = obj.Name;
            //model.CountryId = obj.CountryId;
            //model.DOB = obj.DOB.GetValueOrDefault().ToString("dd/MM/yyyy");
            //model.Address = obj.Address;
            //model.Sex = obj.Sex;
            //model.TeamSTT = obj.TeamSTT;
            //model.TeamMergeSTT = obj.TeamMergeSTT;
            //model.Email = obj.Email;
            //model.IdentifyNumber = obj.IdentifyNumber;
            //model.Phone = obj.Phone;
            //model.Company = obj.Company;
            //model.Payer = true;
            //model.Leader = true;
            model.ListCountry = _svustomerArrive.getAllCountry();
            return PartialView("AddUsingCustomer", model);
        }

        #endregion

        #region Event
        /// <summary>
        ///  thêm khách ở cùng
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ActionResult AddUsingRoom(CustomeCheckInModel obj)
        {
            //return Json(new { result = 0 }, JsonRequestBehavior.AllowGet);
            var rs = _svustomerArrive.AddUsingRoom(obj);
            return Json(rs, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        ///  Trả phòng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult UndoRoom(CustomeCheckInModel obj)
        {
            var rs = _svustomerArrive.UndoRoom(obj);
            return Json(rs, JsonRequestBehavior.AllowGet);
        }
        #endregion

        /// <summary>
        /// Đổi phòng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult ChangeRoom(int id, int CheckInID,string Note,string tdate)
        {
            DateTime _tdate;
            var rs= new JsonRs();           
            DateTime.TryParse(tdate, CultureInfo.GetCultureInfo("vi-vn"), DateTimeStyles.None, out _tdate);
            if (new CommService().checkRoomNotAvailable(id, _tdate, _tdate.AddDays(1)))
            {
                rs = JsonRs.create(-1, "Không thể đổi phòng, vì phòng này đã có người ở từ " + _tdate.ToString("dd/MM/yyyy HH:mm") + " đến " + _tdate.AddDays(1).ToString("dd/MM/yyyy HH:mm"));
            }
            else
            {
                rs = _svustomerArrive.ChangeRoom(id, CheckInID, Note, _tdate);
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

    }
}