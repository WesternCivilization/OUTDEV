using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using oze.data;

namespace Oze.Models.CustomerArriveManage
{
    public class PaymentCheckOutModel
    {
        public List<RoomPriceEstimateModel> GetListPriceEstimate { get; set; }
        public oze.data.Entity.Vw_InforCustomer InforCustomer { get; set; }
        public List<tbl_Room> GetListRoom { get; set; }
        public List<Vw_ProductService> GetListCustomerServices { get; set; }
        public List<Vw_ProductService> GetListCustomerOtherServices { get; set; }
        public List<tbl_Product> GetProductList { get; set; }
    }
}