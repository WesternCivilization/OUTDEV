using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Oze.Models;
using Oze.AppCode.BLL;
using Oze.AppCode.DAL;
using System.Data;

namespace Oze.Controllers
{
    public class SysUsersController : BaseController
    {
        // GET: SysUsers
        [HttpGet]
        public ActionResult Index()
        {
            CHotels hotel = new CHotels();
            //ViewBag.Mess = "0";
            //Session[CConfig.SESSIONMESS] = null;
            ViewBag.SelectHotel = GetComboxHotel();
            
            ViewBag.UsertHotel = GetComboxSysUser();

            //Get all sysUser và lấy 1 vài thông tin cần thiết
            List<SysUserModel> list = new List<SysUserModel>();
            list = new CsysUser().ListInforSysUser();
            ViewBag.ListSysUser = list;
            return View();
        }

        [HttpGet]
        public ActionResult CreateUser()
        {
            CHotels hotel = new CHotels();
            //ViewBag.Mess = "0";
            //Session[CConfig.SESSIONMESS] = null;
            ViewBag.SelectHotel = GetComboxHotel();

            ViewBag.UsertHotel = GetComboxSysUser();

            //Get all sysUser và lấy 1 vài thông tin cần thiết
            List<SysUserModel> list = new List<SysUserModel>();
            list = new CsysUser().ListInforSysUser();
            ViewBag.ListSysUser = list;
            return View();
        }
        
        public JsonResult CreateUser(string txtname, string txtusername, string txtpass,string txtphone,string txtcmt,string txtAddress,string cboHotelname,string cbxChonUser,string cboactive, string email)
        {
            bool kq = false;
            string message = "";
            string erroValidate = "Vui lòng khồng để trống ";
            if (!checkValidate(txtusername,email,ref erroValidate))
            {
                if (string.IsNullOrEmpty(txtpass))
                {
                    message += string.Format(" mật khẩu ", Environment.NewLine);
                }
                return Json(new { mess = erroValidate, rs = kq }, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(txtpass)){
                    message += string.Format(" mật khẩu ", Environment.NewLine);
                    return Json(new { mess = erroValidate, rs = kq }, JsonRequestBehavior.AllowGet);
                }

            SysUserModel obj = new SysUserModel();
            obj.FullName = txtname;
            obj.UserName = txtusername.ToUpper().Trim();
            obj.Password = MD5.md5(txtpass.ToUpper().Trim());
            obj.Mobile = txtphone;
            obj.IdentityNumber = txtcmt;
            obj.Address = txtAddress;
            obj.Email = email;

            obj.Createby = Int32.Parse(Session[CConfig.SESSION_USERID].ToString());
            obj.CreateDate = DateTime.Now;

            obj.Department = 0;
            int parent = string.IsNullOrEmpty(cbxChonUser) ? 0 : Int32.Parse(cbxChonUser.ToString());
            obj.ParentID = parent;
            int sysHotel = string.IsNullOrEmpty(cboHotelname) ? 0 : Int32.Parse(cboHotelname.ToString());
            obj.SysHotelID = sysHotel;
            obj.IsActive = Int32.Parse(cboactive);//khi thêm mới luôn để isactive =1 trong trường hợp xóa isactive=0
            obj.Status = 1;
            message = new CsysUser().CreateSysUser(obj,ref kq);
            return Json(new { mess = message,rs= kq }, JsonRequestBehavior.AllowGet);
            
        }
        
        public ActionResult UpdateUser()
        {
            if (Request.QueryString["UserID"] != null)
            {
                string userID = Request.QueryString["UserID"].ToString();
                CHotels hotel = new CHotels();
                ViewBag.SelectHotel = GetComboxHotel();
                ViewBag.UsertHotel = GetComboxSysUser();

                //Get all sysUser và lấy 1 vài thông tin cần thiết
                List<SysUserModel> list = new List<SysUserModel>();
                list = new CsysUser().ListInforSysUser();
                ViewBag.ListSysUser = list;

                //lấy ra thông tin của user
                SysUserModel objSysUser = new SysUserModel();
                objSysUser = new CsysUser().GetInforSysUserByID(userID);
                return View(objSysUser);
            }
            else
            {
                return View();
            }
            
           
        }

        public JsonResult GetDetailSysUser(string id)
        {
            SysUserModel obj = new SysUserModel();
            obj = new CsysUser().GetInforSysUserByID(id);
            List<string> listGroup = new CsysUser().ListGroupNameByUser(obj.UserName);
            return Json(new { obj = obj, listGroupUser = listGroup }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult Update_User(string id,string txtname, string txtpass, string txtphone, string txtcmt, string txtAddress, string cboHotelname, string cbxChonUser, string cboactive, string email)
        {
            bool kq = false;
            string message = "";
            string erroValidate = "Vui lòng khồng để trống ";
            if (!checkValidate("username", email, ref erroValidate))
            {
                if (string.IsNullOrEmpty(txtpass))
                {
                    message += string.Format(" + mật khẩu ", Environment.NewLine);
                }
                return Json(new { mess = erroValidate, rs = kq }, JsonRequestBehavior.AllowGet);
            }
            
            SysUserModel obj = new SysUserModel();
            obj.ID = Int32.Parse(id);
            obj.FullName = txtname;
            if (!txtpass.Equals(""))
	        {
                obj.Password = MD5.md5(txtpass.ToUpper().Trim());		 
	        }
            obj.Mobile = txtphone;
            obj.IdentityNumber = txtcmt;
            obj.Address = txtAddress;
            obj.Email = email;

            obj.Modifyby = Int32.Parse(Session[CConfig.SESSION_USERID].ToString());
            obj.ModifyDate = DateTime.Now;

            obj.Department = 0;
            int parent = string.IsNullOrEmpty(cbxChonUser) ? 0 : Int32.Parse(cbxChonUser.ToString());
            obj.ParentID = parent;
            int sysHotel = string.IsNullOrEmpty(cboHotelname) ? 0 : Int32.Parse(cboHotelname.ToString());
            obj.SysHotelID = sysHotel;

            //obj.Status = Int32.Parse(cboactive);
            obj.IsActive = Int32.Parse(cboactive);
            
            message = new CsysUser().UpdateSysUser(obj,ref kq);
            return Json(new { mess = message, rs = kq }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult Delete_User(string id)
        {
            string message = new CsysUser().DeleteSysUser(id);
            return Json(new { mess = message}, JsonRequestBehavior.AllowGet);
        }

        public bool checkValidate(string username,string email,ref string kq){
            bool rs = true;
            if (string.IsNullOrEmpty(username))
            {
                kq += string.Format(" + tên đăng nhập ", Environment.NewLine);
                rs = false;
            }
            if (string.IsNullOrEmpty(email))
            {
                kq += string.Format(" + email ", Environment.NewLine);
                rs = false;
            }
            return rs;
        }


        public ActionResult DetailSysUser(string id)
        {
            CHotels hotel = new CHotels();
            List<string> listGroup = null;
            //ds Hotels
            TempData["SelectHotel"] = GetComboxHotel();
            TempData.Keep("SelectHotel");

            SysUserModel obj = new SysUserModel();
            if (Session[CConfig.SESSION_USERID] != null)
            {
                //thong tin nv
                obj = new CsysUser().GetInforSysUserByID(Session[CConfig.SESSION_USERID].ToString());
                //Nhom quyen
                listGroup = new CsysUser().ListGroupNameByUser(obj.UserName);
            }

            ViewData["listGroup"] = listGroup;
            return View("DetailSysUser", obj);
        }

        public JsonResult UserUpdate(SysUserModel user)
        {
            object[] message = new object[2];
            int code = -1;
            string mess = "";
            CDatabaseLam db = new CDatabaseLam();
            DataSet ds = new DataSet();
            try
            {
                if (user != null)
                {
                    if(Session[CConfig.SESSION_USERID] != null)
                    {
                        user.ID = Int32.Parse(Session[CConfig.SESSION_USERID].ToString());
                        user.Modifyby = Int32.Parse(Session[CConfig.SESSION_USERID].ToString());

                        db.UserUpdate(user, ref code, ref mess);
                        message[0] = mess;
                        message[1] = code;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Json(new { mess = message }, JsonRequestBehavior.AllowGet);
        }
    }
}