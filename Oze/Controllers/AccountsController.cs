using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Oze.Models;
using Oze.AppCode.BLL;
using System.Net.Mail;
using System.IO;
using Oze.Services;
using oze.data;

namespace Oze.Controllers
{
    public class AccountsController : Controller
    {
        public ActionResult Login(string type)
        {


            if (type != "logout" && type != "fgpw")
            {
                int retcode = -1;
                string retmesg = "";
                LoginModel mdlogin = new LoginModel();
                if (Request.Cookies["CUserName"] != null && Request.Cookies["CPassword"] != null)
                {
                    try
                    {
                        mdlogin.UserName = Request.Cookies["CUserName"].Value;
                        mdlogin.Password = Request.Cookies["CPassword"].Value;
                        mdlogin.Email = "";

                        tbl_SysUser userinfo = (new SysUserService()).CheckLogin(mdlogin.UserName, mdlogin.Password);

                        if (userinfo != null)
                        {

                            TempData["userinfo"] = userinfo;
                            TempData.Keep("userinfo");

                            Session[CConfig.SESSION_USERID] = retcode;
                            Session[CConfig.SESSION_USERNAME] = userinfo.UserName;
                            Session[CConfig.SESSION_FULLNAME] = userinfo.FullName;
                            Session[CConfig.SESSION_HOTELCODE] = "";
                            Session[CConfig.SESSION_HOTELNAME] = "";
                            Session[CConfig.SESSION_HOTELGROUPCODE] = "";
                            Session[CConfig.SESSION_PW] = userinfo.Password;

                            return RedirectToAction("index", "Home");
                        }
                        else
                        {
                            ViewBag.Mesg = retmesg;
                            return View();
                        }
                    }
                    catch (Exception ex)
                    {
                        return RedirectToAction("Index500", "Error");
                    }
                }
            }

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(String command, string chkRememberMe)
        {
            int retcode = -1;
            string retmesg = "";
            if (command == "SignInSub")
            {
                try
                {
                    LoginModel mdlogin = new LoginModel();

                    Response.Cookies["CUserName"].Value = Request.Form["UserName"].ToString().ToUpper().Trim();
                    Response.Cookies["CPassword"].Value = MD5.md5(Request.Form["Password"].ToString().Trim()).ToString();

                    mdlogin.UserName = Response.Cookies["CUserName"].Value.ToString().Trim();
                    mdlogin.Password = Response.Cookies["CPassword"].Value.ToString().Trim();
                    mdlogin.Email = "";

                    if (chkRememberMe == "on") //có check remember 
                    {
                        Response.Cookies["CUserName"].Expires = DateTime.Now.AddDays(30);
                        Response.Cookies["CPassword"].Expires = DateTime.Now.AddDays(30);
                    }
                    else
                    {
                        Response.Cookies["CUserName"].Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies["CPassword"].Expires = DateTime.Now.AddDays(-1);

                    }

                    tbl_SysUser userinfo = (new SysUserService()).CheckLogin(Request.Form["UserName"].ToString(), Request.Form["Password"].ToString());
                    if (userinfo != null)
                    {
                        
                       
                        /*Get thông tin user đăng nhập*/
                        //userinfo = data.AccountGetInfomation(mdlogin, retcode);
                                                
                        TempData["userinfo"] = userinfo;
                        TempData.Keep("userinfo");

                        Session[CConfig.SESSION_USERID] = retcode;
                        Session[CConfig.SESSION_USERNAME] = userinfo.UserName;
                        Session[CConfig.SESSION_FULLNAME] = userinfo.FullName;
                        Session[CConfig.SESSION_HOTELCODE] = userinfo.SysHotelID;
                        Session[CConfig.SESSION_HOTELNAME] = userinfo.SysHotelID.ToString();
                        Session[CConfig.SESSION_HOTELGROUPCODE] ="";
                        Session[CConfig.SESSION_PW] = userinfo.Password;
                        Session[CConfig.SESSION_HOTELID] = userinfo.SysHotelID;
                        tbl_GroupType oGroupType=(new SysUserService()).GetGroupTypeByUserID(userinfo.Id, userinfo.SysHotelID.Value);
                        if(oGroupType!=null) Session[CConfig.SESSION_GROUPCODE] = oGroupType.code;

                         return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.Mesg = retmesg;
                        return View();
                    }
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Index500", "Error");
                }
            }
            return View();
        }

        #region Logout
        public ActionResult Logout()
        {
            /*Xóa các session lưu thông tin đăng nhập*/
            //CDatabaseLam cursession = new CDatabaseLam();
            //cursession.CurrentSessionRemove();
            Session.Abandon();
            /*Xóa thông tin user*/
            TempData.Remove("userinfo");

            return RedirectToAction("Login", "Accounts", new { type = "logout" });
        }
        #endregion

        #region ForgotPassword
        /*
         * mode = 
         *  directfgpw: redrict từ controller Accounts/ForgotPassword
         */
        public ActionResult Index(string mode)
        {
            if (mode != "directfgpw")
            {
                ViewBag.MesgEmail = "Vui lòng nhập Tài khoản đăng nhập vào ô trống!";
            }
            else
            {
                ViewBag.MesgEmail = "Tài khoản đăng nhập không tồn tại!";
            }
            return View();
        }

        /*
         * pwmode = 
         *  
         */
        public ActionResult ForgotPassword(string pwmode)
        {            
            SysUserModel userinfo = new SysUserModel();
            LoginModel mdlogin = new LoginModel();

            ViewBag.Username = Request.QueryString["user"].ToUpper();
            ViewBag.MesgEmail = ViewBag.Username + ": Vui lòng nhập địa chỉ email vào ô trống!";

            //không nhập info thì redirect về chính nó
            if (ViewBag.Username == "")
            {
                return RedirectToAction("Index", "Accounts");
            }

            //Check exits username trong db
            try
            {
                mdlogin.UserName = ViewBag.Username;
                mdlogin.Email = "";
                /*Get thông tin user đăng nhập*/
                userinfo = null;// data.AccountGetInfomation(mdlogin, -1);

                if (userinfo == null)
                {
                    return RedirectToAction("Index", "Accounts", new { mode = "directfgpw" });
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index500", "Error");
            }

            Session[CConfig.SESSION_USERNAME] = ViewBag.Username;
            
            return View();
        }

        
        #endregion

        #region ChangePassword
        public ActionResult ChangePassword(string changepw)
        {
            if (changepw == "firstlogin")
            {
                ViewBag.Firstlogin = "Để đảm bảo an toàn thông tin. Vui lòng đổi lại Mật khẩu vừa được cấp mới!";
            }
            else
            {
                ViewBag.Firstlogin = "Vui lòng nhập đầy đủ thông tin vào ô trống.";
            }
            return View();
        }
       
        #endregion

        #region ImportData from excel to db
        public ActionResult ImportData()
        {
            return View();
        }

       
        #endregion

    }
}