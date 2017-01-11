using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using oze.data;

namespace Oze.Models.CustomerArriveManage
{
    public class PricePay
    {
        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }
        public string dayOfWeeks { get; set; }
        public decimal PriceDay { get; set; }
        public decimal PriceNight { get; set; }
        public decimal PriceMonth { get; set; }
        public int TypePrice { get; set; }
        public decimal tbl_RoomPriceLevel_HoursPriceHour { get; set; }
        public int numberHours { get; set; }
    }
}