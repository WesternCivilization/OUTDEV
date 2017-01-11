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
using Oze.Models.CustomerManage;
using Vw_RoomPriceLevel = oze.data.Entity.Vw_RoomPriceLevel;

namespace Oze.Controllers
{
    public class RoomPriceLevelController : BaseController
    {
        private readonly IRoomPriceLevelService _priceLevelService;

        public RoomPriceLevelController()
        {
            _priceLevelService = new RoomPriceLevelService();


        }
        // GET: Suppliers
        [HttpGet]
        public ViewResult Index()
        {
            //var room = _priceLevelService.GetRooms();
            return View("Index");
        }

        [HttpGet]
        public ActionResult List(int length, int start, string search)
        {
            //comm.GetHotelId();
            var hotelID = 0;
            if (!comm.IsSuperAdmin())
                hotelID = comm.GetHotelId();
            int recordsTotal = 0;
            List<Vw_RoomPriceLevel> data = _priceLevelService.getAll(new PagingModel() { offset = start, limit = length, search = search }, hotelID, out recordsTotal);
            //int recordsTotal = (int)_priceLevelService.countAll(new PagingModel() { offset = start, limit = length, search = search });
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
        //public PartialViewResult GetInfomationRoomDetail(int id)
        //{
        //    var model = _priceLevelService.GetCustomerRoom(id);

        //    return PartialView("DetailCustomer");
        //}



        #region lấy thêm mới giá

        public PartialViewResult ViewInsert()
        {


            var model = new Vw_RoomPriceLevel();


            model.IsSuperAdmin = comm.IsSuperAdmin();
            if (!comm.IsSuperAdmin())
            {
                var hotel = _priceLevelService.GetHotelsByid(comm.GetHotelId());
                model.HotelName = hotel == null ? "" : hotel.Name;
                model.SysHotelID = comm.GetHotelId();

            }
            else
            {
                model.ListHotel = _priceLevelService.GetHotels();

            }
            model.ListRoomType = _priceLevelService.GeTblRoomTypes(model.SysHotelID.GetValueOrDefault());

            DateTime _fdate;
            DateTime _tdate;

            DateTime.TryParse(DateTime.Now.ToString("dd/MM/yyyy"), CultureInfo.GetCultureInfo("vi-vn"), DateTimeStyles.None, out _fdate);
            DateTime.TryParse(DateTime.Now.ToString("dd/MM/yyyy 23:59"), CultureInfo.GetCultureInfo("vi-vn"), DateTimeStyles.None, out _tdate);
            //_tdate = _tdate.AddDays(1);
            model.dateFrom = _fdate;
            model.dateTo = _tdate;
            return PartialView("PriceByRoom", model);
        }

        #endregion
        #region lấy thông tin chi tiêts
        public PartialViewResult GetDetail(int id)
        {
            var model = _priceLevelService.GetPriceLevel(id);

            model.Listxtraday = _priceLevelService.GeTblRoomPriceLevelExtras(id, 1);
            model.Listxtranight = _priceLevelService.GeTblRoomPriceLevelExtras(id, 2);
            model.xEarlyDay = _priceLevelService.GeTblRoomPriceLevelExtras(id, 3);
            model.xEarlyNight = _priceLevelService.GeTblRoomPriceLevelExtras(id, 4);
            model.limitPerson = _priceLevelService.GeTblRoomPriceLevelExtras(id, 5);
            model.limitPerson_child = _priceLevelService.GeTblRoomPriceLevelExtras(id, 6);
            model.ListConfig = _priceLevelService.GetRoomActives(model.RoomTypeID.GetValueOrDefault(), model.SysHotelID.GetValueOrDefault());
            model.listHour = _priceLevelService.GeLevelHourses(id);
            model.IsSuperAdmin = comm.IsSuperAdmin();
            var hotel = _priceLevelService.GetHotelsByid(comm.GetHotelId());
            model.HotelName = hotel == null ? "" : hotel.Name;
            model.SysHotelID = comm.GetHotelId();
            model.ListRoomType = _priceLevelService.GeTblRoomTypes(model.SysHotelID.GetValueOrDefault());
            return PartialView("ViewPriceByRoom", model);


        }

        #endregion
        #region lấy sửa giá
        /// <summary>
        ///    1:Phụ trội quá giờ trả theo ngày / 
        //     2:  Phụ trội quá giờ trả theo đêm /
        //     3: Phụ trội nhận phòng sớm theo ngày /
        //     4:  Phụ trội nhận phòng sớm theo đêm /
        //     5 :Phụ trội quá số lượng người lớn
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PartialViewResult GetEditDetail(int id)
        {

            var model = _priceLevelService.GetPriceLevel(id);


            model.Listxtraday = _priceLevelService.GeTblRoomPriceLevelExtras(id, 1);
            model.Listxtranight = _priceLevelService.GeTblRoomPriceLevelExtras(id, 2);
            model.xEarlyDay = _priceLevelService.GeTblRoomPriceLevelExtras(id, 3);
            model.xEarlyNight = _priceLevelService.GeTblRoomPriceLevelExtras(id, 4);
            model.limitPerson = _priceLevelService.GeTblRoomPriceLevelExtras(id, 5);
            model.limitPerson_child = _priceLevelService.GeTblRoomPriceLevelExtras(id, 6);
            model.ListConfig = _priceLevelService.GetRoomActives(model.RoomTypeID.GetValueOrDefault(), model.SysHotelID.GetValueOrDefault());
            model.listHour = _priceLevelService.GeLevelHourses(id);
            model.IsSuperAdmin = comm.IsSuperAdmin();
            var hotel = _priceLevelService.GetHotelsByid(comm.GetHotelId());
            model.HotelName = hotel == null ? "" : hotel.Name;
        model.SysHotelID = comm.GetHotelId();
            model.ListRoomType = _priceLevelService.GeTblRoomTypes(model.SysHotelID.GetValueOrDefault());
            return PartialView("EditPriceByRoom", model);
        }

        #endregion

        #region Event
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listRoom"></param>
        /// <param name="model"></param>
        /// <param name="listXtraDay"></param>
        /// <param name="listPriceDay"></param>
        /// <param name="listXtraNight"></param>
        /// <param name="listEarlyDay"></param>
        /// <param name="listEarlyNight"></param>
        /// <param name="listLimitPerson"></param>
        /// <returns></returns>
        public ActionResult InsertOrUpdate(List<tbl_Room> listRoom, tbl_RoomPriceLevel model, string fDate, string tDate,
            List<tbl_RoomPriceLevel_Extra> listXtraDay,
             List<tbl_RoomPriceLevel_Hour> listPriceDay,
             List<tbl_RoomPriceLevel_Extra> listXtraNight,
             List<tbl_RoomPriceLevel_Extra> listEarlyDay,
             List<tbl_RoomPriceLevel_Extra> listEarlyNight,
             List<tbl_RoomPriceLevel_Extra> listLimitPerson,
                List<tbl_RoomPriceLevel_Extra> listLimitPerson_Child
            )
        {

            if (model.Id > 0)
            {
                var result = _priceLevelService.UpdateRoomPriceLevel(listRoom, model,
                    listXtraDay,
                    listPriceDay,
                    listXtraNight,
                    listEarlyDay,
                    listEarlyNight,
                    listLimitPerson, listLimitPerson_Child);
                return Json(new { result = result, Message = result ? "Cập nhật giá hạng phòng thành công." : "Cập nhật giá hạng phòng thất bại!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                DateTime _fdate;
                DateTime _tdate;

                DateTime.TryParse(fDate, CultureInfo.GetCultureInfo("vi-vn"), DateTimeStyles.None, out _fdate);
                DateTime.TryParse(tDate, CultureInfo.GetCultureInfo("vi-vn"), DateTimeStyles.None, out _tdate);
                model.dateFrom = _fdate;// từ
                model.dateTo = _tdate;// tơi
                if (_priceLevelService.checkDate(_fdate, _tdate))
                    return Json(new { result = false, Message = "Khoảng thời gian đa tồn tại trên hệ thống. Vui lòng kiểm tra lai!" }, JsonRequestBehavior.AllowGet);
                if (_fdate >= _tdate)
                    return Json(new { result = false, Message = "Thời gian từ phải nhỏ hơn thời gian tới. Vui lòng kiểm tra lai!" }, JsonRequestBehavior.AllowGet);

                var result = _priceLevelService.InsertRoomPriceLevel(listRoom, model,
          listXtraDay,
           listPriceDay,
          listXtraNight,
            listEarlyDay,
            listEarlyNight,
           listLimitPerson, listLimitPerson_Child);
                return Json(new { result = result, Message = result ? "Thêm mới giá hạng phòng thành công." : "Thêm mới giá hạng phòng Thất bại." }, JsonRequestBehavior.AllowGet);
            }

        }
        /// <summary>
        /// Cập nhật thông tin khách hàng
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ActionResult Update(CustomeDetail obj)
        {
            int result = 0;
            //int result = _priceLevelService.updateCustome(obj);
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        ///  cập nhtaaj trạng thái khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult Delete(string id)
        {
            int result = 0;
            result = _priceLevelService.LockRoom(int.Parse(id));
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region lấy thông tin khách hàng

        public PartialViewResult GetRoom(int id, int hotelid, int typeRoom)
        {

            var model = _priceLevelService.GetRooms(id, hotelid, typeRoom);
            if (id > 0)
            {
                model[0].IdActive = typeRoom;
                return PartialView("RoomSelect", model);
            }

            return PartialView("RoomSelectAdd", model);

        }

        #endregion

        public JsonResult GetRoomActives(int id, int hotelid)
        {
            var data = _priceLevelService.GetRoomActives(id, hotelid);

            return Json(new { status = data.Count > 0, Data = data }, JsonRequestBehavior.AllowGet);

        }
    }
}