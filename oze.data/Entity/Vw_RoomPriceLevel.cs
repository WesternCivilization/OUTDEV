﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.DataAnnotations;

namespace oze.data.Entity
{
    public class Vw_RoomPriceLevel : data.Vw_RoomPriceLevel
    {
        [Ignore]
        public List<tbl_Hotel> ListHotel { get; set; }
        [Ignore]
        public List<tbl_RoomPriceLevel_Extra> Listxtraday { get; set; }
        [Ignore]
        public List<tbl_RoomPriceLevel_Extra> Listxtranight { get; set; }
        [Ignore]
        public List<tbl_RoomPriceLevel_Extra> xEarlyDay { get; set; }
        [Ignore]
        public List<tbl_RoomPriceLevel_Extra> xEarlyNight { get; set; }
        [Ignore]
        public List<tbl_RoomPriceLevel_Extra> limitPerson { get; set; }
        [Ignore]
        public List<tbl_RoomPriceLevel_Extra> limitPerson_child { get; set; }
        [Ignore]
        public List<tbl_RoomPriceLevel_Hour> listHour { get; set; }
        [Ignore]
        public List<tbl_Room_Type> ListRoomType { get; set; }
        [Ignore]
        public List<Vw_RoomActive> ListConfig { get; set; }
        [Ignore]
        public bool IsSuperAdmin { get; set; }
      //hunginnit
    }
}
