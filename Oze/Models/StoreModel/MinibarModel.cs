using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using oze.data;

namespace Oze.Models.StoreModel
{
    public class MinibarModel
    {
        public List<tbl_Room> ListRooms { get; set; }
        public List<Vw_StoreConfig> ListProduct { get; set; }
        public List<tbl_Product> ListProductsAdd { get; set; }
        public int RoomId { get; set; }
        public string Name { get; set; }
        public List<Item> Item { get; set; }
        public int Id { get; set; }
        public string Action { get; set; }
    }

    public class Item
    {
        public int ProductId { get; set; }
        public int Limit { get; set; }
        public string Note { get; set; }
    }
}