using System.Web.Mvc;
using Oze.Models;
using Oze.AppCode.DAL;
using System.Data;
using System;
using System.Web;
using Oze.AppCode.BLL;
using System.Collections.Generic;
using System.Web.Script.Serialization;
 

namespace Oze.Controllers
{
    public class GroupController : Controller
    {
        CDatabase data = new CDatabase();
        CDatabaseNam dataNam = new CDatabaseNam();
        CRightPermissions permission = new CRightPermissions();
        protected int UserID;
        protected int GroupID;
        protected string SysHotelCode = "";

        // GET: Group
        public ActionResult Index()
        {
            return View();
        }


        // GET: Group/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Group/Create
        [HttpGet]
        public ActionResult Create()
        {
            
            UserID = Session[CConfig.SESSION_USERID] == null ? 1 : Convert.ToInt32(Session[CConfig.SESSION_USERID]);
            try
            {
                DataTable dtPermission = new DataTable();
                dtPermission = permission.tbUserHasPermission();
                ViewData["CheckPermission"] = dtPermission;
                SysHotelCode = Session[CConfig.SESSION_HOTELCODE].ToString();

                if (permission.checkPermission(dtPermission, "GroupTypeModels", "Read"))
                {
                    ViewData["ListGroupType"] = new SelectList(data.GetGroupTypeList(UserID), "ID", "NameVN");
                    ViewData["ListGroupTypeALL"] = data.GetGroupTypeList(UserID);
                }
                    
                if(permission.checkPermission(dtPermission, "HotelsModel","Read"))
                {
                    ViewData["Hotel"] = new SelectList(dataNam.getListHotels(), "Code", "Name", 1);
                    ViewData["ListGroupTypeForGroup"] = data.GetListGroupTypeforGroup(SysHotelCode);
                }
                    
                if (permission.checkPermission(dtPermission, "GroupModels", "Read"))
                {
                    ViewData["ListGroup"] = data.GetListGroup(SysHotelCode);
                }

                return View();
            }
            catch(Exception)
            {
                return RedirectToAction("Index", "FloorPlan");
            }
        }

        // POST: Group/Create
        public JsonResult CreateGroup(GroupDetailModel Group)
        {
            DataSet ds = new DataSet();
            object[] message = new object[1];
            try
            {
                if(Group != null & ModelState.IsValid)
                {
                    ds = data.CreateGroup(Group);
                    message[0] = ds.Tables[0].Rows[0]["sMess"].ToString();
                }
            }
            catch (Exception ex)
            {
                message[0] = "Error: Không thêm được nhóm quyền!";
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }

            return Json(new { mess = message}, JsonRequestBehavior.AllowGet);
        }

        // GET: Group/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                GroupModels Group = new GroupModels();
                UserID = Session[CConfig.SESSION_USERID] == null ? 1 : Convert.ToInt32(Session[CConfig.SESSION_USERID]);

                Group = data.GetGroupByID(id);
                ViewData["ListRule"] = data.GetAccessRights(id);
                TempData["MenuList"] = new SelectList(data.GetListMenu(), "ID", "Name");
                TempData.Keep("MenuList");

                ViewData["ListGroupType"] = new SelectList(data.GetGroupTypeList(UserID), "ID", "NameVN", Group.GroupType);
                ViewData["Hotel"] = new SelectList(dataNam.getListHotels(), "Code", "Name", Group.SysHotelCode);

                TempData["ListGroupMenu"] = data.GetListGroupMenu(id);
                TempData.Keep("ListGroupMenu");
                return View(Group);
            }
            catch(Exception ex)
            {
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
                return View();
            }
            
        }

        // POST: Group/Edit/5
        [HttpPost]
        public ActionResult UpdateGroup(string id, string strObject)
        {
            try
            {
                // TODO: Add update logic here
                string[] arrComm = strObject.Split(new string[] { "," }, System.StringSplitOptions.None);
                DataSet ds = new DataSet();
                int Group = GroupID;
                string rule = arrComm[0].ToString();
                string read = arrComm[1].ToString();
                string write = arrComm[2].ToString();
                string create = arrComm[3].ToString();
                string delete = arrComm[4].ToString();

                ds = data.CreateAccessRight(Group, rule, read, write, create, delete);

                ViewBag.Mess = ds.Tables[0].Rows[0]["sMess"].ToString();
                if (ds.Tables[0].Rows[0]["iCode"].ToString() == "1")
                {
                    Session[CConfig.SESSION_RIGHT_PERMISSION] = null;
                }
                return RedirectToAction("Edit", "Group", new { id = GroupID });
            }
            catch(Exception ex)

            {
                var exs = ex;
                return View();
            }
        }

        // GET: Group/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Group/Delete/5
        //[HttpPost]
        public JsonResult DeleteGroup(int id = 0)
        {
            object[] message = { };
            DataSet ds = new DataSet();
            try
            {
                // TODO: Add delete logic here
                if(id != 0)
                {
                    //ds = data.DeleteGroupMenuRel
                }
                //return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
                //return View();
            }
            return Json(new { mess = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Ajax form: Create Group Menu 
        /// </summary>
        /// <param name="SysMenuID"></param>
        /// <param name="SysGroupID"></param>
        /// <returns></returns>
        public JsonResult CreateGroupMenu(string SysMenuID, string SysGroupID)
        {
            string result;
            string Count = "0";
            try
            {
                SysGroupMenu_RelModels objGroupMenu = new SysGroupMenu_RelModels();
                DataSet ds = new DataSet();
                int MenuID = string.IsNullOrEmpty(SysMenuID) ? 0 : Int32.Parse(SysMenuID.ToString());
                int GroupID = string.IsNullOrEmpty(SysGroupID) ? 0 : Int32.Parse(SysGroupID.ToString());
                int UserID = Session[CConfig.SESSION_USERID].ToString() == "" ? 1 : Convert.ToInt32(Session[CConfig.SESSION_USERID]);
                objGroupMenu.SysMenuID = MenuID;
                objGroupMenu.SysGroupID = GroupID;
                objGroupMenu.UserID = UserID;

                ds = data.CreateGroupMenuRel(objGroupMenu);

                if (ds.Tables[1].Rows.Count > 0)
                {
                    Count = ds.Tables[1].Rows[0][0].ToString();
                    var row = ds.Tables[1].Rows[0];
                    List<SysGroupMenu_RelModels> list = new List<SysGroupMenu_RelModels>();
                    list.Add(new SysGroupMenu_RelModels { 
                        ID = Convert.ToInt32(row["ID"].ToString().Trim()),
                        SysGroupID = Convert.ToInt32(row["SysGroupID"].ToString().Trim()),
                        Name = row["Name"].ToString().Trim(),
                        Level = Convert.ToInt32(row["Level"].ToString().Trim())
                    });
                    ViewData["ListGroupMenu"] = list;// ds.Tables[1];
                }
                result = ds.Tables[0].Rows[0][0].ToString();
            }
            catch(Exception ex)
            {
                result = "Error: Không thêm được menu vào group";
                Count = null;
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }
            
            return Json(new { mess = result, Count = Count, data = ViewData["ListGroupMenu"] == null ? null : ViewData["ListGroupMenu"] }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteGroupMenu(string ID)
        {
            string result, resultC;
            try
            {
                DataSet ds = new DataSet();
                int iID = string.IsNullOrEmpty(ID) ? 0 : Int32.Parse(ID.ToString());
                int UserID = Session[CConfig.SESSION_USERID].ToString() == "" ? 1 : Convert.ToInt32(Session[CConfig.SESSION_USERID]);

                ds = data.DeleteGroupMenuRel(iID, UserID);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    result = ds.Tables[0].Rows[0][0].ToString();
                    resultC = ds.Tables[0].Rows[0][1].ToString();
                }
                else
                {
                    result = "Error: Xóa Menu không thành công";
                    resultC = "-1";
                }
            }
            catch (Exception ex)
            {
                result = "Error: Không thêm được menu vào group";
                resultC = "";
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }

            return Json(new { mess = result, code = resultC }, JsonRequestBehavior.AllowGet);
        }
    }
}
