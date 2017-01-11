using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.DataAnnotations;

namespace oze.data.Entity
{
    public class Vw_InforCustomer : data.Vw_InforCustomer
    {
        [Ignore]
        public List<tbl_Room> GetListRoom { get; set; }
        [Ignore]
        public List<Vw_ProductService> GetListCustomerServices { get; set; }

        [Ignore]
        public List<tbl_Product> GetProductList { get; set; }

        [Ignore]
        public bool IsboolRoom { get; set; }
        [Ignore]
        public bool IsboolService { get; set; }
        //[Ignore]
        //public List<o> GetProductList2 { get; set; }
        //List<RoomPriceEstimateModel>()
    }
}

