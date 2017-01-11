using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Oze.Models
{
    public class RoomPriceModel
    {
        public string Code { get; set; }
        public string SysHotelCode { get; set; }
        public string ReservationType { get; set; }
        public string DatetimeType { get; set; }
        public int TotalHours { get; set; }
        public int NumberPeople { get; set; }
        public float Price { get; set; }
        public string RoomTypeCode { get; set; }

        public RoomPriceModel()
        {
            this.Code = "";
            this.SysHotelCode = "";
            this.ReservationType = "";
            this.RoomTypeCode = "";
            this.DatetimeType = "";
            this.TotalHours = 0;
            this.NumberPeople = 0;
            this.Price = 0;
        }
    }
}