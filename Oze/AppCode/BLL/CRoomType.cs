using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Oze.Models;
using Oze.AppCode.DAL;
using System.Data;


namespace Oze.AppCode.BLL
{
    public class CRoomType
    {
        CDatabase data = new CDatabase();
        public void CreateRoomType(RoomTypeModel obj, ref string roomtype, ref int retcode, ref string retmesg)
        {
            try
            {
                data.CreateRoomType(obj, ref roomtype, ref retcode, ref retmesg);
            }
            catch (Exception)
            {
            }
        }
    }
}