using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Oze.Models
{
    public class RoomModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string RoomCode { get; set; }
        public string RoomTypeCode { get; set; }
        public string Floor { get; set; }
        public string Postion { get; set; }
        public int RoomType_ID { get; set; }
        public int RoomLevel_ID { get; set; }
        public int Building_ID { get; set; }
        public string Temp { get; set; }
        public int Active { get; set; }

        public RoomModel()
        {
            this.ID = -1;
            this.Name = "";
            this.RoomCode = "";
            this.RoomTypeCode = "";
            this.Floor = "";
            this.Postion = "";
            this.RoomType_ID = 0;
            this.RoomLevel_ID = 0;
            this.Building_ID = 0;
            this.Temp = "";
            this.Active = 1;
        }
    }
}