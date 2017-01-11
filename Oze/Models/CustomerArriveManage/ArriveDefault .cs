using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using oze.data;

namespace Oze.Models.CustomerArriveManage
{
    public class ArriveDefault
    {
        public List<tbl_Room> ListRooms { get; set; }
        public List<tbl_Room_Type> listRoomTypes { get; set; }
    }
}