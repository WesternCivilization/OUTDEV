using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Oze.Models;
using System.Data;
using Oze.AppCode.BLL;
using Oze.AppCode.DAL;

namespace Oze.AppCode.BLL
{
    public class CHotels
    {
        public List<HotelsModel> GetAllHotel()
        {
            List<HotelsModel> list = new List<HotelsModel>(); 
            try
            {
                DataTable dt = new CDatabase().GetAllHotels().Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    HotelsModel obj = new HotelsModel();
                    if (Int32.Parse(dt.Rows[i]["Status"].ToString())==1)
                    {
                        obj.ID = Int32.Parse(dt.Rows[i]["ID"].ToString());
                        obj.LogoUrl = dt.Rows[i]["LogoUrl"].ToString();
                        obj.Name = dt.Rows[i]["Name"].ToString();
                        obj.Phone = dt.Rows[i]["Phone"].ToString();
                        obj.Mobile = dt.Rows[i]["Mobile"].ToString();
                        obj.RoomCount = Int32.Parse(dt.Rows[i]["RoomCount"].ToString());
                        obj.Status = Int32.Parse(dt.Rows[i]["Status"].ToString());
                        obj.Website = dt.Rows[i]["Website"].ToString();
                        obj.Email = dt.Rows[i]["Email"].ToString();
                        obj.Code = dt.Rows[i]["Code"].ToString();
                        obj.Address = dt.Rows[i]["Address"].ToString();
                        obj.Description = dt.Rows[i]["Description"].ToString();

                        obj.Modifyby = string.IsNullOrEmpty(dt.Rows[i]["Modifyby"].ToString()) ? 0 : Int32.Parse(dt.Rows[i]["Modifyby"].ToString());
                        obj.Createby = string.IsNullOrEmpty(dt.Rows[i]["Createby"].ToString()) ? 0 : Int32.Parse(dt.Rows[i]["Createby"].ToString());
                        //obj.CreateDate =  dt.Rows[i][""].ToString();
                        //obj.ModifyDate = dt.Rows[i]["ModifyDate"].ToString();
                        list.Add(obj);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}