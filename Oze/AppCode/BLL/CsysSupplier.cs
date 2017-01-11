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
    public class CSupplier
    {
        /*TrungND 21/10/16 gán thông tin cho khách sạn*/
        public List<SupplierModel> GetAllUser()
        {
            List<SupplierModel> list = new List<SupplierModel>();
            try
            {
                DataTable dt = new CDatabase().GetAllSupplier().Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    SupplierModel obj = new SupplierModel();
                    obj.ID = Int32.Parse(dt.Rows[0]["ID"].ToString());
                    obj.Name = dt.Rows[i]["FullName"].ToString();
                    obj.ContactName = dt.Rows[i]["UserName"].ToString();
                    obj.Mobile = dt.Rows[i]["Mobile"].ToString().Trim();
                    obj.Address = dt.Rows[i]["IdentityNumber"].ToString().Trim();
                    obj.Email = dt.Rows[i]["Email"].ToString();
                    obj.Note = dt.Rows[i]["Note"].ToString();
                    list.Add(obj);
                }
                return list;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string CreateSupplier(SupplierModel obj,ref bool kq)
        {
            try
            {
                DataSet ds = new CDatabase().CreateSupplier(obj);
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

        public string UpdateSupplier(SupplierModel obj,ref bool kq)
        {
            try
            {
                kq = new CDatabase().UpdateSupplier(obj);
                return kq == true ? "Chỉnh sửa thành công" : "Chỉnh sửa thất bại";
            }
            catch (Exception)
            {
                return "Có lỗi hệ thống trong quá trình thêm mới";
            }
        }

        public string DeleteSupplier(string id)
        {
            try
            {
                bool kq = new CDatabase().DeleteSupplier(id);
                //bool kq = true;
                return kq == true ? "Xóa sửa thành công" : "Xóa sửa thất bại";
            }
            catch (Exception)
            {
                return "Có lỗi hệ thống trong quá trình thêm mới";
            }
        }

        public SupplierModel GetInforSupplierByID(string id)
        {
            SupplierModel obj = new SupplierModel();
            try
            {
                DataSet ds = new CDatabase().GetInforSupplierById(id);
                if (ds.Tables.Count > 0)
                {
                    obj.ID = Int32.Parse(ds.Tables[0].Rows[0]["ID"].ToString());
                    obj.Name = ds.Tables[0].Rows[0]["FullName"].ToString();
                    obj.ContactName = ds.Tables[0].Rows[0]["UserName"].ToString();
                    obj.Mobile = ds.Tables[0].Rows[0]["Mobile"].ToString().Trim();
                    obj.Address = ds.Tables[0].Rows[0]["IdentityNumber"].ToString().Trim();
                    obj.Email = ds.Tables[0].Rows[0]["Email"].ToString();
                    obj.Note = ds.Tables[0].Rows[0]["Note"].ToString();
                
                    if (ds.Tables[0].Rows[0]["CreateDate"] != null)
                    {
                        obj.CreateDate = DateTime.Parse(ds.Tables[0].Rows[0]["CreateDate"].ToString());
                    }
                    if (ds.Tables[0].Rows[0]["ModifyDate"] != null)
                    {
                        obj.ModifiedDate = DateTime.Parse(ds.Tables[0].Rows[0]["ModifiedDate"].ToString());
                    }
                }
                return obj;
            }
            catch (Exception ex)
            {
                return obj;
            }
        }

        public List<SupplierModel> ListInforSupplier()
        {
            List<SupplierModel> list = new List<SupplierModel>();
            try
            {
                DataSet ds = new CDatabase().GetInforSupplier();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    SupplierModel obj = new SupplierModel();
                    obj.ID = Int32.Parse(ds.Tables[0].Rows[i]["ID"].ToString());
                    obj.Name = ds.Tables[0].Rows[i]["FullName"].ToString();
                    obj.ContactName = ds.Tables[0].Rows[i]["UserName"].ToString();
                    obj.Mobile = ds.Tables[0].Rows[i]["Mobile"].ToString().Trim();
                    obj.Address = ds.Tables[0].Rows[i]["IdentityNumber"].ToString().Trim();
                    obj.Email = ds.Tables[0].Rows[i]["Email"].ToString();
                    obj.Note = ds.Tables[0].Rows[i]["Note"].ToString();

                    if (ds.Tables[0].Rows[0]["CreateDate"] != null)
                    {
                        obj.CreateDate = DateTime.Parse(ds.Tables[0].Rows[i]["CreateDate"].ToString());
                    }
                    if (ds.Tables[0].Rows[0]["ModifyDate"] != null)
                    {
                        obj.ModifiedDate = DateTime.Parse(ds.Tables[0].Rows[i]["ModifiedDate"].ToString());
                    }
                   
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