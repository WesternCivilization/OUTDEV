using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using oze.data;

namespace Oze.Models.StoreModel
{
    public class StoreModel
    {
        public StoreModel()
        {
            ListItemStores = new List<Vw_Store>();
        }
        public string GroupName { get; set; }
        public int GroupId { get; set; }
        public List<Vw_Store> ListItemStores { get; set; }
    }

    public class SearchStoreModel
    {
        public string Search { get; set; }
        public int Start { get; set; }
        public int Lenght { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int StoreId { get; set; }

    }
}