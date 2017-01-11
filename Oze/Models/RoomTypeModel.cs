using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Oze.Models
{
    public class RoomTypeModel
    {
        //trungND
        public int ID { get; set; }
        //mã của roomtype dùng để gán cho room và phụ trội
        public string Code { get; set; }
        public string Name { get; set; }
        //số lượng giường đơn
        public int SingBed { get; set; }
        
        //số lượng giường đôi
        public int DouldBed { get; set; }
        //số lượng người lớn tiêu chuẩn trong phòng
        public int UserLimit { get; set; }
        public string Note { get; set; }
        public RoomTypeModel()
        {
            this.ID = -1;
            this.Name = "";
            this.Code = "";// code các Roomtype sẽ được lấy theo name được viết hoa và lọc bỏ dấu 
            this.DouldBed = 0;
            this.SingBed = 0;
            this.UserLimit = 0;
            this.Note = "";
        }
    }
}