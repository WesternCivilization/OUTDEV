using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Oze.AppCode.BLL;
using Oze.AppCode.DAL;
using Oze.Models;
using System.Data;
using System.Data.SqlClient;
using Oze.Services;
using Oze.Services.StoreManagerService;
using oze.data;
using Oze.AppCode.Util;

namespace Oze.Controllers
{
    public class SettingController : Controller
    {
        CSQLHelper helper = new CSQLHelper(CConfig.CONNECTIONSTRING_OZEGENERAL);
        CDatabaseNam db = new CDatabaseNam();
        
            //
        // GET: /Setting/
       
        public ActionResult Menu()
        {
            List<MenusModel> result = new List<MenusModel>();
            DataSet ds = new DataSet();
            try
            {
                //ds = helper.ExecuteQuery(CConfig.SP_GET_ALL_MENU);
                //foreach (DataRow row in ds.Tables[0].Rows)
                //{
                //    result.Add(new MenusModel {
                //        ID = Convert.ToInt32(row["ID"].ToString()),
                //        Name = row["Name"].ToString(),
                //        Link = row["Link"].ToString(),
                //        Controller = row["Controller"].ToString().Trim(),
                //        Action = row["Action"].ToString().Trim(),
                //        ParentID = Convert.ToInt32(row["ParentID"].ToString()),
                //        Level = Convert.ToInt32(row["Level"].ToString()),
                //        Order = Convert.ToInt32(row["Order"].ToString()),
                //        ModuleName = row["ModuleName"].ToString(),
                //        Status = Convert.ToInt32(row["Status"].ToString()),
                //        Icon = row["Icon"].ToString()
                //    });
                //}
            }
            catch (Exception ex)
            {
                ds = null;
                throw ex;
            }
            ViewData["menuList"] = result;
            return View();
        }
        /// <summary>
        /// 2016-11-09 11:10:35 NamLD
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Hotels() {
            List<HotelListModel> result = new List<HotelListModel>();
            DataSet ds = new DataSet();
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            try
            {
                result = db.getListHotels();
            }
            catch (Exception ex)
            {
                result = null;
                throw ex;
            }
            ViewData["hotelList"] = result;
            return View();
        }
        /// <summary>
        /// them moi khach san
        /// </summary>
        /// <returns></returns>
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult CreateHotels(HotelDefaultModel hotel)
        {
            object[] message = new object[2];
            DataSet ds = new DataSet();
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            try
            {
                if (hotel != null & ModelState.IsValid)
                {
                    ds = db.InsertHotel(hotel);
                    message[0] = ds.Tables[0].Rows[0].ItemArray;
                    if (ds.Tables.Count > 1)
                    {
                        message[1] = db.getObjectHotel(ds.Tables[1]);
                        tbl_Store tbl= new tbl_Store();
                        tbl.title="Kho chính";
                        tbl.SysHotelID = (new HotelService()).GetHotelByCode(ds.Tables[1].Rows[0]["code"].ToString()).Id;
                        tbl.typeStore=1;
                        tbl.datecreated = DateTime.Now;
                        tbl.creatorid = comm.GetUserId();                       
                        new StoreManagerService().InsertOrUpdateStoreGeneral(tbl);
                    }
                }
                   
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
            //ViewData["hotelList"] = result;
            return Json(new { mess = message }, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult UpdateHotels(HotelDefaultModel hotel)
        {
            object[] message = { };
            DataSet ds = new DataSet();
            try
            {
                if (hotel != null & ModelState.IsValid)
                {
                    ds = db.UpdateHotels(hotel);
                    message = ds.Tables[0].Rows[0].ItemArray;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //ViewData["hotelList"] = result;
            return Json(new { mess = message }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteHotels(string hotel)
        {
            object[] message = { };
            DataSet ds = new DataSet();
            try
            {
                if (hotel != null)
                {
                    ds = db.DeleteHotels(hotel);
                    message = ds.Tables[0].Rows[0].ItemArray;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //ViewData["hotelList"] = result;
            return Json(new { mess = message }, JsonRequestBehavior.AllowGet);
        }
    }
}