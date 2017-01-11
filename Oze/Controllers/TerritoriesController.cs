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

namespace Oze.Controllers
{
    public class TerritoriesController : Controller
    {
        CSQLHelper helper = new CSQLHelper(CConfig.CONNECTIONSTRING_OZEGENERAL);
        CDatabaseLam data = new CDatabaseLam();
        CDatabaseNam dataNam = new CDatabaseNam();
        
        // GET: /Territories/   
        [HttpGet]
        public ActionResult Territories()
        {
            int RetCode = 1;
            string RetMesg = "";
            DataSet ds = new DataSet();
            List<TerritoriesModel> result = new List<TerritoriesModel>();
            TerritoriesModel mdterr = new TerritoriesModel();
            mdterr.Activity = "SELECT";
            mdterr.ProvinceId = "";
            mdterr.DistrictId = "";
            mdterr.WardsId = "";
            
            try
            {
                result = data.TerritoriesGet(mdterr, ref RetCode, ref RetMesg);
            }
            catch (Exception ex)
            {
                result = null;
                throw ex;
            }
            ViewData["TerritoriesList"] = result;
            return View();
        }

        public JsonResult Create(TerritoriesModel mdterr) 
        {
            object[] message = new object[3];
            DataTable dt = new DataTable();
            int RetCode = 1;
            string RetMesg = "";
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            try
            {
                mdterr.Activity = "INSERT";
                if (mdterr != null & ModelState.IsValid)
                {
                    dt = data.InsertTerri(mdterr, ref RetCode, ref RetMesg);
                    message[0] = RetMesg;
                    message[1] = RetCode;
                }  
                //if(dt.Rows.Count > 0)
                //{
                //    message[2] = dt;
                //}
            }
            catch (Exception ex)
            {
                dt = null;
                throw ex;
            }
            return Json(new { mess = message}, JsonRequestBehavior.AllowGet);
            //return View();
        }

        public JsonResult Update(TerritoriesModel mdterr)
        {
            bool result = false;
            object[] message = new object[3];
            int RetCode = 1;
            string RetMesg = "";
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            try
            {
                mdterr.Activity = "UPDATE";
                if (mdterr != null & ModelState.IsValid)
                {
                    result = data.UpdDelTerri(mdterr, ref RetCode, ref RetMesg);
                    message[0] = RetMesg;
                    message[1] = RetCode;
                }
            }
            catch (Exception ex)
            {
                result = false;
                throw ex;
            }
            //ViewData["TerritoriesList"] = result;
            return Json(new { mess = RetMesg, code = RetCode }, JsonRequestBehavior.AllowGet);
            //return View();
        }

        public JsonResult Delete(TerritoriesModel mdterr)
        {
            bool result = false;
            object[] message = new object[3];
            int RetCode = 1;
            string RetMesg = "";
            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            try
            {
                mdterr.Activity = "DELETE";
                if (mdterr != null & ModelState.IsValid)
                {
                    result = data.UpdDelTerri(mdterr, ref RetCode, ref RetMesg);
                    message[0] = RetMesg;
                    message[1] = RetCode;
                }
            }
            catch (Exception ex)
            {
                result = false;
                throw ex;
            }
            //ViewData["TerritoriesList"] = result;
            return Json(new { mess = RetMesg, code = RetCode }, JsonRequestBehavior.AllowGet);
            //return View();
        }
	}
}