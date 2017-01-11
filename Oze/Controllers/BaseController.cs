using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Oze.Models;
using Oze.AppCode.DAL;
using Oze.AppCode.BLL;


namespace Oze.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
        //
        // GET: /HotelManager/Base/

        /*TrungND Lấy danh sách khách sạn*/
        public List<SelectListItem> GetComboxHotel()
        {
            List<SelectListItem> li = new List<SelectListItem>();
            try
            {
                li.Add(new SelectListItem { Text = "Chọn Khách sạn", Value = "0", Selected = true });
                List<HotelsModel> list = new CHotels().GetAllHotel();
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Status == 1)
                    {
                        li.Add(new SelectListItem { Text = list[i].Name, Value = list[i].ID.ToString() });
                    }
                }

            }
            catch (Exception)
            {
            }
            return li;
        }

        public List<SelectListItem> GetComboxSysUser()
        {
            List<SelectListItem> li = new List<SelectListItem>();
            try
            {
                li.Add(new SelectListItem { Text = "Chọn User cha", Value = "0", Selected = true });
                List<SysUserModel> list = new CsysUser().GetAllUser();
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].IsActive != 0)
                    {
                        li.Add(new SelectListItem { Text = list[i].FullName, Value = list[i].ID.ToString() });
                    }
                }

            }
            catch (Exception)
            {
            }
            return li;
        }

        public List<SelectListItem> GetComboxDepartment()
        {
            List<SelectListItem> li = new List<SelectListItem>();
            try
            {
                li.Add(new SelectListItem { Text = "Chọn Bộ Phận", Value = "0", Selected = true });
                List<HotelsModel> list = new CHotels().GetAllHotel();
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Status == 1)
                    {
                        li.Add(new SelectListItem { Text = list[i].Name, Value = list[i].ID.ToString() });
                    }
                }

            }
            catch (Exception)
            {
            }
            return li;
        }

        //public string checkphutroi()
        //{
        //    PhuTroiModel model = new PhuTroiModel();

        //}
    }
}