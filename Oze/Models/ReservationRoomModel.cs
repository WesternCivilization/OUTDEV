using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Oze.Models
{
    public class ReservationStatus
    {
        public static int WAITING = 1;
        public static int CONFIRM=2;
        public static int TOCHECKIN = 3;
        public static int CANCEL = 4;
        public static int CHECKOUT = 5;

    }
    public class CheckInStatus
    {
        public static int OK = 1;
        public static int CHECKOUT = 2;
        public static int CANCEL = 3;
    }
    public class RoomStatus
    {
        public static int EMPTY = 0;
        public static int LIVE = 1;
        public static int CANCEL = 2;
        public static int CHECKOUT =3;
        public static int BOOKING = 4;


        public static int DIRTY = 10;
        public static int BROKEN = 11;
        public static int FIXING = 12;
    }
    public class ReservationRoomModel
    {
        public string Activity { get; set; }
        public int ID { get; set; } //ID
        public int CustID { get; set; } //ID Khách đặt phòng (là người đặt phòng)
        public string CustomerName { get; set; } //Ten khach dat phong
        public string ReservationCode { get; set; } //mã đặt phòng
        public string HotelCode { get; set; }//mã KS
        public int ReservationType { get; set; }  //loại đặt phòng (theo người, theo phòng)
        public int RoomID { get; set; } //ID phòng 
        public int RoomTypeID { get; set; } //ID loại phòng
        public int Room_Level_ID { get; set; } //ID hạng phòng
        public int ReservationStatus { get; set; } //Trang thai dat phong
        public int Payment_Type_ID { get; set; } //ID phương thức thanh toán
        public float Deposit { get; set; }  //tiền cọc
        public float Price { get; set; }  //giá phòng (tính theo loại đặt phòng)
        public int Holiday { get; set; }  //giá phòng theo dịp (tính theo ngày thường, ngày lễ)
        public int KhungGio { get; set; }  //giá phòng theo khung (tính theo giờ, giá cả ngày, giá đêm)
        public float Discount { get; set; } //giảm trừ
        public int PeopelNumber { get; set; }  //số người
        public int Adult { get; set; } //số người lớn
        public int Children { get; set; } //số trẻ nhỏ
        public float Deduction { get; set; }  //số người
        public float Tax { get; set; }  //số người
        public string ArrivalDate { get; set; }  //thời gian đến (booking)
        public string LeaveDate { get; set; }  //thời gian đi (booking)
        public string CheckInDate { get; set; }  //thời gian checkin
        public string CheckOutDate { get; set; }  //thời gian chechkout

        public string Note { get; set; }  //ghi chú 
        public string Old_Room { get; set; } //phòng cũ (phòng trước khi đổi)
        public string RoomTemp { get; set; } //phòng tạm (TH: ko có phòng thỏa mãn các tiêu chí booking {loại phòng, hàng phòng, số phòng})
        public string Reason { get; set; } //lý do 

        public int CreateResBy { get; set; } //người nhận khách (checkin)
        public string CreateResDate { get; set; } //thời gian nhận khách
        public int ModifyResby { get; set; }  //người sửa, cập nhật thông tin
        public string ModifyResDate { get; set; }   //thời gian sửa, cập nhật thông tin
        public int RoomLevelPriceID { get; set; }
        public  CustomerModel customer {get;set;}
        public int CountryId { get; set; }
    }

    public class ReservationHist
    {
        public int ID { get; set; } //ID
        public int CustID { get; set; } //ID Khách đặt phòng (là người đặt phòng)
        public string ReservationCode { get; set; } //mã đặt phòng
        public int OrgLead { get; set; } //ID khách Trưởng đoàn
        public int OrgCode { get; set; } //Code đoàn (nếu là đặt phòng đi theo đoàn)

        public int Createby { get; set; } //người nhận khách (checkin)
        public string CreateDate { get; set; } //thời gian nhận khách

    }

    public class CustomerModel
    {
        public string Activity { get; set; }
        public int ID { get; set; } //ID
        public int SourceID { get; set; } //là KH của KS nào
        public int CustomerTypeID { get; set; } //loại KH 
        public int GroupID { get; set; } //ID đoàn 
        public int GroupJoinID { get; set; } //ID đoàn ghép
        public int RoomID { get; set; } //ID phòng 
        public string FullName { get; set; }  //Tên đầy đủ
        public int Sex { get; set; } //giới tính
        public string DOB { get; set; } //ngày tháng năm sinh dd/mm/yyyy
        public string IdentityNumber { get; set; }  // CMND/Hộ chiếu ....
        public int CitizenshipCode { get; set; }  // Quốc tịch   
        //public string Protector { get; set; } //người bảo hộ (đối với KH nhỏ hơn 18 tuổi)
        public string Company { get; set; } //Company
        public string Address { get; set; } //địa chỉ
        public string Email { get; set; }  //email
        public string Phone { get; set; } //Phone
        public string Mobile { get; set; } //mobile
        public string Fax { get; set; } //Phone
        public string TaxCode { get; set; } //mobile
        public string HotelCode { get; set; }//mã KS
        public string ReserCode { get; set; } //mã đặt phòng
        public int Leader { get; set; } //ID phòng 
        public int Payer { get; set; } //ID phòng 
        public int Createby { get; set; } //người nhận khách (checkin)
        public DateTime CreateDate { get; set; } //thời gian nhận khách
        public int Modifyby { get; set; }  //người sửa, cập nhật thông tin
        public DateTime ModifyDate { get; set; }   //thời gian sửa, cập nhật thông tin
        public int CountryID { get; set; } //CountryID phòng 

    }

    public class ListReserCustModel
    {
        public List<ReservationRoomModel> ReservationRomlst { get; set; } //Khởi tạo list Đặt phòng 
        public List<ReservationHist> ReservationHistorylst { get; set; }  //Khởi tạo list Lịch sử đặt phòng
        public List<CustomerModel> Customerlst { get; set; }  //Khởi tạo list khách hàng
    }
    public class DetailBooking {
         public int ID { get; set; } 
         public string HotelName { get; set; } 
         public string BookingCode { get; set; }
         public int RoomID { get; set; } //ID phòng 
         public string RoomName { get; set; } //so phong
         public int Room_Status_ID { get; set; } 
         public string Room_Status_Name { get; set; } 
         public int Reservation_Type_ID { get; set; } 
         public string Reservation_Type_Name { get; set; } 
         public int Payment_Type_ID { get; set; }
         public string PaymentTypeName { get; set; }
         public float Deposit { get; set; }
         public int Number_People { get; set; }
         public int Number_Children { get; set; }
         public float Deduction { get; set; }
         public string Arrive_Date { get; set; }
         public string Leave_Date { get; set; }
         public string khunggio { get; set; }
         public string Holiday { get; set; }
         public string Room_Type_ID { get; set; }
    }

    public class Customer_Booking
    {
        public int Id { get; set; }
        public int? SourceID { get; set; }
        public string HotelCode { get; set; }
        public int CustomerTypeID { get; set; }
        public int? Room_ID { get; set; }
        public int? Group_ID { get; set; }
        public int? JoinGroup_ID { get; set; }
        public string Name { get; set; }
        public DateTime? DOB { get; set; }
        public string IdentifyNumber { get; set; }
        public int? Sex { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Fax { get; set; }
        public string TaxCode { get; set; }
        public int? CitizenshipCode { get; set; }
        public int? Leader { get; set; }
        public int? Payer { get; set; }
        public string Company { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? CreateBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public int? ModifyBy { get; set; }
        public string CusType { get; set; }
    }
}