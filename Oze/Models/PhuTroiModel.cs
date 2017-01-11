using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Oze.Models
{
    public class PhuTroiModel
    {
        
        public int ID { get; set; }
        public string Name { get; set; }
        public string CodePhuTroi { get; set; }
        public string ReservationType { get; set; }
        public int TotalHours { get; set; }
        public int NumberPeople { get; set; }
        public float Price { get; set; }
        public string RoomTypeCode { get; set; }

        public PhuTroiModel()
        {
            this.ID = 0;
            this.Name = "";
            this.CodePhuTroi = "";// code các CodePhuTroi sẽ được lấy theo name được viết hoa và lọc bỏ dấu 
            this.ReservationType = "";
            this.RoomTypeCode = "";
            this.TotalHours = 0;
            this.TotalHours = 0;
            this.NumberPeople = 0;
            this.Price = 0;
        }
    }
}