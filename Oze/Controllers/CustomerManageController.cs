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
using Oze.Models.CustomerManage;

namespace Oze.Controllers
{
    public class CustomerManageController : BaseController
    {
        private readonly ICustomerManageService _customerManage;

        public CustomerManageController()
        {
            _customerManage = new CustomerManageService();

        }
        // GET: Suppliers
        [HttpGet]
        public ViewResult Index()
        {
            return View("Index");
        }

        [HttpGet]
        public ActionResult List(int length, int start, string search, SearchCustomerModel model)
        {
            int recordsTotal = 0;
            List<tbl_Customer> data = _customerManage.getAll(new PagingModel() { offset = start, limit = length, search = search }, model, out recordsTotal);
            //int recordsTotal = (int)_customerManage.countAll(new PagingModel() { offset = start, limit = length, search = search });
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
            var model = _customerManage.GetCustomerRoom(id);
        
            return PartialView("InfomationRoomDetail", model);
        }
        #region Bạn cùng phòng
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PartialViewResult GetRoomMateDetail(int customerid)
        {
            var model = _customerManage.GetRoomMate(customerid);
            return PartialView("RoomMateCustomer", model);
        }
        #endregion
        #region Lịch sử đặt phòng
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PartialViewResult GetHistoryDetail(int id)
        {
            var model = _customerManage.GetHistory(id);
        
            return PartialView("HistoryCustomer", model);
        }

        #endregion
        #region lấy thông tin khách hàng

        public PartialViewResult GetDetail(int id)
        {
            var obj = _customerManage.GetCustomer(id);
            var model = new CustomeDetail();
            model.Id = obj.Id;
            model.Name = obj.Name;
            model.CountryId = obj.CountryId;
            model.DOB = obj.DOB.GetValueOrDefault().ToString("dd/MM/yyyy");
            model.Address = obj.Address;
            model.Sex = obj.Sex;
            model.TeamSTT = obj.TeamSTT;
            model.TeamMergeSTT = obj.TeamMergeSTT;
            model.Email = obj.Email;
            model.IdentifyNumber = obj.IdentifyNumber;
            model.Phone = obj.Phone;
            model.Mobile = obj.Mobile;
            model.Company = obj.Company;
            model.Payer = true;
            model.Leader = true;
            model.ListCountry = _customerManage.getAllCountry();
            return PartialView("DetailCustomer", model);
        }

        #endregion
        #region lấy thông tin khách hàng

        public PartialViewResult GetEdit(int id)
        {
            var obj = _customerManage.GetCustomer(id);
            var model = new CustomeDetail();
            model.Id = obj.Id;
            model.Name = obj.Name;
            model.CountryId = obj.CountryId;
            model.DOB = obj.DOB.GetValueOrDefault().ToString("dd/MM/yyyy");
            model.Address = obj.Address;
            model.Sex = obj.Sex;
            model.TeamSTT = obj.TeamSTT;
            model.TeamMergeSTT = obj.TeamMergeSTT;
            model.Email = obj.Email;
            model.IdentifyNumber = obj.IdentifyNumber;
            model.Phone = obj.Phone;
            model.Mobile = obj.Mobile;

            model.Company = obj.Company;
            model.Payer = true;
            model.Leader = true;
            model.ListCountry = _customerManage.getAllCountry();
            return PartialView("EditCustomer", model);
        }

        #endregion
        #region Event
        /// <summary>
        /// Cập nhật thông tin khách hàng
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ActionResult Update(CustomeDetail obj)
        {
            int result = _customerManage.updateCustome(obj);
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        ///  cập nhtaaj trạng thái khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult Delete(string id)
        {
            int result = _customerManage.deleteCustome(int.Parse(id));
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}