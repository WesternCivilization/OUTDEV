using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Oze.AppCode.BLL;
using Oze.AppCode.DAL;
using Oze.Models;

namespace Oze.Controllers
{
    public class FloorPlanController : Controller
    {
        public SysUserModel userinformation;
        CDatabaseLam data = new CDatabaseLam();
        //CDatabaseNam db = new CDatabaseNam();
        //
        // GET: /FloorPlan
        public ActionResult Index()
        {
            userinformation = TempData["userinfo"] as SysUserModel;
            if (TempData["userinfo"] == null)
            {
                try
                {
                    LoginModel mdlogin = new LoginModel();
                    mdlogin.UserName = "";
                    mdlogin.Email = "";

                    /*Get thông tin user đăng nhập*/
                    userinformation = data.AccountGetInfomation(mdlogin, Int32.Parse(Session[CConfig.SESSION_USERID].ToString()));
                }
                catch(Exception ex)
                {
                    return RedirectToAction("Index500", "Error");
                }

                TempData["userinfo"] = userinformation;
                TempData.Keep("userinfo");
            }

            TempData["FullName"] = userinformation.FullName;
            TempData["HotelCode"] = userinformation.CodeSysHotel;
            TempData["HotelName"] = userinformation.NameSysHotel;
            return View();
                
        }
        public ActionResult Receptionist()
        {
            //List<ParentMenu> listMenu = new List<ParentMenu>();
            //listMenu = db.getAllMenu();
            return View();
        }
    }
}