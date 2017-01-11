using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Oze.Models
{
    public class AccountsModel
    {
    }
    public class LoginModel
    {
        [Required(ErrorMessage = "Nhập Tên đăng nhập.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Nhập Mật khẩu.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Email { get; set; }
        public int FirstLogin { get; set; }
    }


    public class RegisterModel
    {
        [Required(ErrorMessage = "Nhập Tên đăng nhập.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email không đúng.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Nhập Mật khẩu.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Nhập Mật khẩu xác nhận.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không đúng.")]
        public string ConfirmPassword { get; set; }

        public string IdentityNumber { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public int Department { get; set; }
        public int ParentID { get; set; }
        public int SysHotelID { get; set; }
        public int Status { get; set; }
        public int IsActive { get; set; }
        public int Createby { get; set; }
        public DateTime CreateDate { get; set; }
        public int Modifyby { get; set; }
        public DateTime ModifyDate { get; set; }
        public int RetCode { get; set; }
        public string RetMesg { get; set; }
    }
   
}