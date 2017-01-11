using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Oze.Models
{
    public class RoomPriceType 
    {
        public static int HOUR = 0;
        public static int DAY = 1;
        public static int NIGHT = 2;
        public static int ALL = -1;

        public static int EXTRA_EARLY_NIGHT = 5;
        public static int EXTRA_LATE_NIGHT = 6;
        public static int EXTRA_EARLY_DAY = 7;
        public static int EXTRA_LATE_DAY = 8;
        public static int EXTRA_LATE_ADULT = 9;
        public static int EXTRA_LATE_CHIDLREN = 10;


        //1:Phụ trội quá giờ trả theo ngày
        //    / 2:  Phụ trội quá giờ trả theo đêm / 3: Phụ trội nhận phòng sớm theo ngày/ 4:  Phụ trội nhận phòng sớm theo đêm
        //    / 5 :Phụ trội quá số lượng người lớn
    }
    public class TypeExtra
    {
        public static int EARLY_CHECKIN_DAY = 3; // 3: Phụ trội nhận phòng sớm theo ngày
        public static int EARLY_CHECKIN_NIGHT = 4;  // 4:  Phụ trội nhận phòng sớm theo đêm
        public static int LATE_CHECKOUT_DAY = 1; //1:Phụ trội quá giờ trả theo ngày
        public static int LATE_CHECKOUT_NIGHT = 2; // 2:  Phụ trội quá giờ trả theo đêm 
        public static int OVER_NUMBER_ADULT = 5;// 5 :Phụ trội quá số lượng người lớn
        public static int OVER_NUMBER_CHILDREN = 6;// 6 :Phụ trội quá số lượng trẻ em


    }
    public class RoomPriceEstimateModel
    {
        public int roomid { get; set; }
        public string roomName { get; set; }

        public int roomtypeid { get; set; }
        public string roomtypeName { get; set; }

        public int typePrice { get; set; }//loại giá:0:giờ;1: ngày; 2:đêm
        public int quantiy { get; set; }//số lượng 
        public int pricePolicyId { get; set; }//chính sách giá
        public string pricePolicyName { get; set; }//chính sách giá

        public double price { get; set; }//chính sách giá
        public DateTime dtFrom { get; set; }//từ
        public DateTime dtTo { get; set; }//đến
        public string titlePrice { get; set; }//chính sách giá

        public void setTitlePrice()
        {
            titlePrice = getTitlePrice();
        }
        public string getTitlePrice()
        {
            string sContent;
            if (typePrice == 0) sContent = "Theo giờ";
            else if (typePrice == 1) sContent = "Theo ngày";
            else if (typePrice == 2) sContent = "Theo đêm";

            else if (typePrice == 5) sContent = "Phụ trội sớm đêm";
            else if (typePrice == 6) sContent = "Phụ trội trễ đêm";

            else if (typePrice == 7) sContent = "Phụ trội sớm ngày";
            else if (typePrice == 8) sContent = "Phụ trội trễ ngày";
            else if (typePrice == 9) sContent = "Phụ trội người lớn";
            else if (typePrice == 10) sContent = "Phụ trội trẻ em";

            else sContent = "Không xác định";    
            return sContent;
        }

        public RoomPriceEstimateModel()
        {
            this.roomid =0;
            this.roomName = "";
            this.roomtypeid = 0;
            this.roomtypeName = "";
            this.typePrice = -1;
            this.quantiy = 0;
            this.pricePolicyId = 0;
            this.price = 0;
            this.dtFrom = DateTime.Now;
            this.dtTo = DateTime.Now;
        }
        /// <summary>
        /// design pattern for builder
        /// </summary>
        /// <returns></returns>
        public static RoomPriceEstimateModel createNew()
        {
            return new RoomPriceEstimateModel();
        }
        public  RoomPriceEstimateModel setRoomId(int roomid)
        {
            this.roomid = roomid;
            return this;
        }
        public RoomPriceEstimateModel setRoomName(string roomName)
        {
            this.roomName = roomName;
            return this;
        }
        public RoomPriceEstimateModel setRoomTypeId(int roomTypeId)
        {
            this.roomtypeid = roomTypeId;
            return this;
        }
         public RoomPriceEstimateModel setTypePrice(int typePrice)
        {
            this.typePrice = typePrice;
            return this;
        }
         public RoomPriceEstimateModel setQuantity(int quantiy)
        {
            this.quantiy = quantiy;
            return this;
        }
         public RoomPriceEstimateModel setPricePolicyID(int pricePolicyID)
        {
            this.pricePolicyId = pricePolicyID;
            return this;
        }
         public RoomPriceEstimateModel setPrice(double price)
        {
            this.price = price;
            return this;
        }
         public RoomPriceEstimateModel setDtFrom(DateTime dtFromX)
        {
            this.dtFrom = dtFromX;
            return this;
        }
         public RoomPriceEstimateModel setDtTo(DateTime dtToX)
        {
            this.dtTo = dtToX;
            return this;
        }
         public RoomPriceEstimateModel setPricePolicyName(string pricePolicyName)
         {
             this.pricePolicyName = pricePolicyName;
             return this;
         }
         public RoomPriceEstimateModel updateTitlePrice()
         {
             this.titlePrice = this.getTitlePrice();
             return this;
         }
    }
}