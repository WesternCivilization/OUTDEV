using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Oze.Models;
using Oze.Services;

namespace Oze.Controllers
{
    public class ReportController : Controller
    {
        RepostService _repostService = new RepostService();
        //
        // GET: /Report/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BaoCaoTienPhong()
        {
            
            return View();
        }

        [HttpGet]
        public ActionResult ListTienPhong(int length, int start, string search, ReportTienPhong model)
        {


            int recordsTotal = 0;
            double totalAmount = 0;
            var data = _repostService.BaoCaoTienPhong(new PagingModel() { offset = start, limit = length, search = "" }, Share.Todate(model.FromDate), Share.Todate(model.ToDate), model.Keyword,  out recordsTotal,out totalAmount);

            int recordsFiltered = recordsTotal;
            int draw = 1;
            try { draw = int.Parse(Request.Params["draw"]); }
            catch { }
            return Json(new
            {
                draw,
                recordsTotal,
                recordsFiltered,
                data,
                totalAmount

            }, JsonRequestBehavior.AllowGet);
        }
    }
}