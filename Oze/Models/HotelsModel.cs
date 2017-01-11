using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Oze.Models
{
    public class HotelsModel
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string LogoUrl { get; set; }
        public int RoomCount { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public int Createby { get; set; }
        public DateTime CreateDate { get; set; }
        public int Modifyby { get; set; }
        public DateTime ModifyDate { get; set; }
    }
    /// <summary>
    /// NamLD 11/13/2016 09:12:41
    /// CreateByUser, ModifyByUser la lay ten nguoi cap nhat
    /// </summary>
    public class HotelListModel
    {
        public int ID { get; set; }

        [StringLength(50, ErrorMessage = "Mã KS có độ dài tối đa là 50 ký tự")]
        [Display(Name = "Mã KS:")]
        [Required(ErrorMessage ="Mã KS không để trống")]
        [RegularExpression("^ozehotel([0-9]{3})$", ErrorMessage ="Sai định dạng (OzeHotelXXX, xxx la chữ số)")]
        public string Code { get; set; }

        [StringLength(500, ErrorMessage = "Tên KS có độ dài tối đa là 500 ký tự")]
        [Display(Name = "Tên KS:")]
        [Required(ErrorMessage = "Tên KS không để trống")]
        public string Name { get; set; }

        [StringLength(200, ErrorMessage = "Địa chỉ có độ dài tối đa là 200 ký tự")]
        [Display(Name = "Địa chỉ:")]
        public string Address { get; set; }

        [StringLength(11, ErrorMessage = "Máy bàn có độ dài tối đa là 11 ký tự")]
        [Display(Name = "Máy bàn:")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Số điện thoại phải là dạng số")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Di động không để trống")]
        [StringLength(11, ErrorMessage = "Di động có độ dài tối đa là 11 ký tự")]
        [Display(Name = "Di động:")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Số điện thoại phải là dạng số")]
        public string Mobile { get; set; }

        [Display(Name = "Email:")]
        [StringLength(200, ErrorMessage = "Độ dài tối đa Email là 200 ký tự")]
        [DataType(DataType.EmailAddress,ErrorMessage ="Sai định dạng Email")]
        public string Email { get; set; }

        [Display(Name = "Website:")]
        public string Website { get; set; }

        [Display(Name = "Logo:")]
        public string LogoUrl { get; set; }

        [Display(Name = "Số phòng:")]
        [Required(ErrorMessage = "Số phòng không để trống")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Số phòng phải là dạng số")]
        public int RoomCount { get; set; }

        [Display(Name = "Mô tả:")]
        public string Description { get; set; }

        [Display(Name = "Trạng thái:")]
        [Required(ErrorMessage = "Trạng thái không để trống")]
        public Status Status { get; set; }

        [Display(Name = "Người tạo:")]
        [Required(ErrorMessage = "Không thấy thông tin tài khoản")]
        public string CreateByUser { get; set; }

        [Display(Name = "Người tạo:")]
        [Required(ErrorMessage = "Không thấy thông tin tài khoản")]
        public int Createby { get; set; }

        [Display(Name = "Ngày tạo:")]
        [Required(ErrorMessage = "Không thấy thông tin ngày tạo")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Người sửa:")]
        [Required(ErrorMessage = "Không thấy thông tin tài khoản")]
        public string ModifyByUser { get; set; }

        [Display(Name = "Người sửa:")]
        [Required(ErrorMessage = "Không thấy thông tin tài khoản")]
        public int Modifyby { get; set; }

        [Display(Name = "Ngày sửa:")]
        [Required(ErrorMessage = "Không thấy thông tin ngày sửa")]
        public DateTime? ModifyDate { get; set; }
    }
    /// <summary>
    /// NamLD 11/13/2016 09:12:41
    /// default CreateByUser, ModifyByUser la lay id
    /// </summary>
    public class HotelDefaultModel
    {
        public string Code { get; set; }

        [StringLength(500, ErrorMessage = "Tên KS có độ dài tối đa là 500 ký tự")]
        [Display(Name = "Tên KS:")]
        [Required(ErrorMessage = "Tên KS không để trống")]
        public string Name { get; set; }

        [StringLength(200, ErrorMessage = "Địa chỉ có độ dài tối đa là 200 ký tự")]
        [Display(Name = "Địa chỉ:")]
        public string Address { get; set; }

        [StringLength(11, ErrorMessage = "Máy bàn có độ dài tối đa là 11 ký tự")]
        [Display(Name = "Máy bàn:")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Số điện thoại phải là dạng số")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Di động không để trống")]
        [StringLength(11, ErrorMessage = "Di động có độ dài tối đa là 11 ký tự")]
        [Display(Name = "Di động:")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Số điện thoại phải là dạng số")]
        public string Mobile { get; set; }

        [Display(Name = "Email:")]
        [StringLength(200, ErrorMessage = "Độ dài tối đa Email là 200 ký tự")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Sai định dạng Email")]
        public string Email { get; set; }

        [Display(Name = "Website:")]
        public string Website { get; set; }

        [Display(Name = "Logo:")]
        public string LogoUrl { get; set; }

        [Display(Name = "Số phòng:")]
        [Required(ErrorMessage = "Số phòng không để trống")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Số phòng phải là dạng số")]
        public int RoomCount { get; set; }

        [Display(Name = "Mô tả:")]
        public string Description { get; set; }

        [Display(Name = "Trạng thái:")]
        [Required(ErrorMessage = "Trạng thái không để trống")]
        public Status Status { get; set; }

        [Display(Name = "Người tạo:")]
        [Required(ErrorMessage = "Không thấy thông tin tài khoản")]
        public int Createby { get; set; }

        [Display(Name = "Người sửa:")]
        [Required(ErrorMessage = "Không thấy thông tin tài khoản")]
        public int Modifyby { get; set; }

    }
}