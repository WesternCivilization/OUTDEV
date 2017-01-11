using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Oze.Models
{
    public class Order
    {
        public string SoPhieu { get; set; }
        public string SoChungTu { get; set; }
        public string NgayNhapHD { get; set; }
        public string NgayChungTu { get; set; }
        public int StoreId { get; set; }
        public int SrcStoreId { get; set; }
    }

    public class BukhoModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int StoreId { get; set; }
    }
    public class OrderDetail
    {
        public int ID { get; set; }
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public int CateId { get; set; }
        public int Price { get; set; }
        public int SupplierId { get; set; }
        public int UnitId { get; set; }
        public string HanSuDung { get; set; }
        public string NgaySanXuat { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; }
        public string CateName { get; set; }
    }

    public class StoreInputSearchModels
    {
        public string Keyword { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
}