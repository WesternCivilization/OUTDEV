using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Oze.AppCode.DAL;
using Oze.AppCode.BLL;
using Oze.Models;
namespace Oze.AppCode.BLL
{
    public class CsysUser
    {
        /*TrungND 21/10/16 gán thông tin cho khách sạn*/
        public List<SysUserModel> GetAllUser()
        {
            List<SysUserModel> list = new List<SysUserModel>();
            try
            {
                DataTable dt = new CDatabase().GetAllSysUser().Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    SysUserModel obj = new SysUserModel();
                    obj.ID = Int32.Parse(dt.Rows[i]["ID"].ToString());
                    obj.FullName = dt.Rows[i]["FullName"].ToString();
                    obj.Address = dt.Rows[i]["Address"].ToString();
                    obj.IdentityNumber = dt.Rows[i]["IdentityNumber"].ToString();
                    obj.Mobile = dt.Rows[i]["Mobile"].ToString();
                    obj.ParentID = Int32.Parse(dt.Rows[i]["ParentID"].ToString());
                    obj.Status = Int32.Parse(dt.Rows[i]["Status"].ToString());
                    obj.Department = Int32.Parse(dt.Rows[i]["Department"].ToString());
                    obj.IsActive = dt.Rows[i]["IsActive"].ToString() == "" ? 0 : Int32.Parse(dt.Rows[i]["IsActive"].ToString());

                    //obj.ModifyDate = string.IsNullOrEmpty(dt.Rows[i]["ModifyDate"].ToString()) ? null : DateTime.Parse(dt.Rows[i]["ModifyDate"].ToString());
                    //obj.Modifyby = Int32.Parse(dt.Rows[i]["Modifyby"].ToString());
                    //obj.Createby = Int32.Parse(dt.Rows[i]["Createby"].ToString());
                    //obj.CreateDate = DateTime.Parse(dt.Rows[i]["CreateDate"].ToString());
                    list.Add(obj);
                }
                return list;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string CreateSysUser(SysUserModel obj,ref bool kq)
        {
            try
            {
                DataSet ds = new CDatabase().CreateSysUser(obj);
                int rs = Int32.Parse(ds.Tables[0].Rows[0][1].ToString());
                kq = false;
                if (rs==1)
                {
                    kq = true;
                }
                return ds.Tables[0].Rows[0][0].ToString();
            }
            catch (Exception)
            {
                return "Có lỗi hệ thống trong quá trình thêm mới";
            }
        }

        public string UpdateSysUser(SysUserModel obj,ref bool kq)
        {
            try
            {
                kq = new CDatabase().UpdateSysUser(obj);
                return kq == true ? "Chỉnh sửa thành công" : "Chỉnh sửa thất bại";
            }
            catch (Exception)
            {
                return "Có lỗi hệ thống trong quá trình thêm mới";
            }
        }

        public string DeleteSysUser(string id)
        {
            try
            {
                bool kq = new CDatabase().DeleteSysUser(id);
                //bool kq = true;
                return kq == true ? "Xóa sửa thành công" : "Xóa sửa thất bại";
            }
            catch (Exception)
            {
                return "Có lỗi hệ thống trong quá trình thêm mới";
            }
        }

        public SysUserModel GetInforSysUserByID(string id)
        {
            SysUserModel obj = new SysUserModel();
            try
            {
                DataSet ds = new CDatabase().GetInforSysUserByID(id);
                if (ds.Tables.Count > 0)
                {
                    obj.ID = Int32.Parse(ds.Tables[0].Rows[0]["ID"].ToString());
                    obj.FullName = ds.Tables[0].Rows[0]["FullName"].ToString();
                    obj.UserName = ds.Tables[0].Rows[0]["UserName"].ToString();
                    obj.Mobile = ds.Tables[0].Rows[0]["Mobile"].ToString().Trim();
                    obj.IdentityNumber = ds.Tables[0].Rows[0]["IdentityNumber"].ToString().Trim();
                    obj.Status = Int32.Parse(ds.Tables[0].Rows[0]["Status"].ToString());
                    obj.IsActive = Int32.Parse(ds.Tables[0].Rows[0]["IsActive"].ToString());
                    obj.NameSysHotelID = ds.Tables[0].Rows[0]["Name"].ToString();
                    obj.NameModifyby = ds.Tables[0].Rows[0]["EditName"].ToString();
                    //obj.NameParentID = ds.Tables[0].Rows[0]["ParentID"].ToString();
                    obj.NameCreateby = ds.Tables[0].Rows[0]["CreateName"].ToString();
                    obj.NameSysHotel = ds.Tables[0].Rows[0]["Name"].ToString();
                    obj.Email = ds.Tables[0].Rows[0]["Email"].ToString();
                    obj.Address = ds.Tables[0].Rows[0]["Address"].ToString();
                    obj.SysHotelID = Int32.Parse(ds.Tables[0].Rows[0]["SysHotelID"].ToString());
                    obj.ParentID = Int32.Parse(ds.Tables[0].Rows[0]["ParentID"].ToString());

                    if (ds.Tables[0].Rows[0]["CreateDate"] != null)
                    {
                        obj.CreateDate = DateTime.Parse(ds.Tables[0].Rows[0]["CreateDate"].ToString());
                    }
                    if (ds.Tables[0].Rows[0]["ModifyDate"] != null)
                    {
                        obj.ModifyDate = DateTime.Parse(ds.Tables[0].Rows[0]["ModifyDate"].ToString());
                    }
                }
                return obj;
            }
            catch (Exception ex)
            {
                return obj;
            }
        }

        public List<SysUserModel> ListInforSysUser()
        {
            List<SysUserModel> list = new List<SysUserModel>();
            try
            {
                DataSet ds = new CDatabase().GetInforSysUser();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    SysUserModel obj = new SysUserModel();
                    obj.ID = Int32.Parse(ds.Tables[0].Rows[i]["ID"].ToString());
                    obj.FullName = ds.Tables[0].Rows[i]["FullName"].ToString();
                    obj.UserName = ds.Tables[0].Rows[i]["UserName"].ToString();
                    obj.Mobile = ds.Tables[0].Rows[i]["Mobile"].ToString();
                    obj.IdentityNumber = ds.Tables[0].Rows[i]["IdentityNumber"].ToString();
                    obj.Status = Int32.Parse(ds.Tables[0].Rows[i]["Status"].ToString());
                    obj.IsActive = Int32.Parse(ds.Tables[0].Rows[i]["IsActive"].ToString());
                    obj.NameSysHotelID = ds.Tables[0].Rows[i]["Name"].ToString();
                    obj.NameModifyby = ds.Tables[0].Rows[i]["EditName"].ToString();
                    //obj.NameParentID = ds.Tables[0].Rows[0]["ParentID"].ToString();
                    obj.NameCreateby = ds.Tables[0].Rows[i]["CreateName"].ToString();
                    if (obj.IsActive != 0)
                    {
                        list.Add(obj);
                    }
                }
                return list;
            }
            catch (Exception)
            {
                return list;
            }
        }

        public List<string> ListGroupNameByUser(string username)
        {
            List<string> list = new List<string>();
            try
            {
                DataSet ds = new CDatabase().GetAllGroupsByUser(username);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    string groupName = ds.Tables[0].Rows[i]["GroupName"].ToString();
                    list.Add(groupName);
                }
                return list;
            }
            catch (Exception)
            {
                return list;
            }
        }
    }
}