using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Oze.Models;
using Oze.AppCode.DAL;
using System.Data;

namespace Oze.AppCode.BLL
{
    public class CRoom
    {
        CDatabase data = new CDatabase();
        public List<RoomModel> GetAllRoomHotel()
        {
            return data.GetAllRoom();
        }

        public string AddRoomHotel(string name,ref string rs)
        {
            RoomModel obj = new RoomModel();
            obj.Name = name;
            obj.RoomCode = new Locdau().LocDauChuoi(name).ToUpper();
            return data.CreateRooms(obj,ref rs);
        }

        //public string AddRoomHotel(string name, ref string rs)
        //{
        //    RoomModel obj = new RoomModel();
        //    obj.Name = name;
        //    obj.RoomCode = new Locdau().LocDauChuoi(name).ToUpper();
        //    return data.CreateRooms(obj, ref rs);
        //}

        public string DeleteRoomHotel(string id)
        {
            try
            {
                int idHotel = Int32.Parse(id);
                string mess = "";
                bool kq = data.DeleteRoomsID(idHotel, ref mess);
                return mess;
            }
            catch (Exception)
            {
                return "Có lỗi hệ thống không xoa được khách sạn";
            }
        }

        public string UpdateRoomHotel(string name,string id,ref int rs)
        {
            try
            {
                RoomModel obj = new RoomModel();
                obj.Name = name;
                obj.ID = Int32.Parse(id);
                string mess = "";
                bool kq = data.UpdateRoomsName(obj, ref mess, ref rs);
                if (kq)
                {
                    return mess;
                }
                else
                {
                    return "Chỉnh sửa thất bại";
                }
            }
            catch (Exception)
            {
                return "Có lỗi từ hệ thống";
            }    
            
        }

    }
}