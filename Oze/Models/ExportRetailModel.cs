using System.Collections.Generic;
using oze.data;

namespace Oze.Models
{
    public class ExportRetailModel
    {
        public IEnumerable<view_DetailProduct> ListProducts { get; set; } // danh sách sản phẩm lúc load trang
        public string ErCode { get; set; }// mã phiếu
        public string CustomerName { get; set; } // 
        public string SPayTime { get; set; }// thời gian thanh toán
        public string CustomerPhone { get; set; } // số DT
        public List<RetailDetail> ListRetailDetail { get; set; } // danh sách sản phẩm
        public float TotalAmount { get; set; } // tổng tiền hoá đơn
        public float SurchargeAmount { get; set; } // phụ thu        
        public float PayTotal { get; set; }
        public float PayLeft { get; set; }
    }

    public class RetailDetail
    {
        public int ProductId { get; set; } // mã sản phẩm
        public string ProductName { get; set; } // mã sản phẩm
        public string Unit { get; set; } //
        public string Description { get; set; }
        public string Note { get; set; }
        public int Quantity { get; set; }//số lượng
        public float Price { get; set; } // giá
        public float TotalPrice { get; set; } // tổng giá ( giá * số lượng) từng sản phẩm
    }
}