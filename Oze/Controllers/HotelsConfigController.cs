using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using oze.data;
using Oze.AppCode.Util;
using Oze.Services;

namespace Oze.Controllers
{
    public class HotelsConfigController : Controller
    {
        private readonly ICustomerArriveManageService _svustomerArrive;

        public HotelsConfigController()
        {
            _svustomerArrive = new CustomerArriveManageService();
        }
        // GET: HotelsConfig
        public ActionResult Index()
        {
            var mode = new Vw_HotelsConfig();
            mode.startCheckin = 12;
            mode.startCheckout = 10;
            mode.startNight1 = 21;
            mode.startNight2 = 5;
            mode.startRoundMain = 15;
            mode.startRoundExatra = 15;


            var   datamode = _svustomerArrive.GetConfig(comm.GetHotelId());
          
            return View(datamode ?? mode);
        }

        public ActionResult UpdateOrInsert(tbl_HotelsConfig obj)
        {
            int result = 0;
             result = _svustomerArrive.UpdateOrInsert(obj);
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }
    }
}