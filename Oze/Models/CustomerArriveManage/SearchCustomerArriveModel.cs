using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Oze.Models.CustomerArriveManage
{
    public class SearchCustomerArriveModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Fdate { get; set; }
        public string Tdate { get; set; }
        public string Tax_Passpor { get; set; }
        public bool CheckDate { get; set; }
        public int RoomID { get; set; }
        public int RooTypeID { get; set; }
        public int HotelID { get; set; }
    }
}