using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Oze.Models.CustomerManage
{
    public class HistoryCustomer
    {
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string tbl_CustomerName { get; set; }
        public string tbl_Room_StatusName { get; set; }
        public int ReservationStatus { get; set; }
        
        public string BookingCode { get; set; }
        //public string StatusName { get { return ReservationStatus==1?"11111":"9999"; } set { Name = value; } }
        public DateTime Arrive_Date { get; set; }
        public DateTime Leave_Date { get; set; }

    }
}