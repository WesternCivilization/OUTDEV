using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.DataAnnotations;

namespace oze.data.Entity
{
    public class Vw_InforCustomer_Room : data.Vw_InforCustomer_Room
    {
        [Ignore]
        public List<tbl_Country> ListCountry { get; set; }
    }
}
