using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Oze.Models
{
    public class GroupModels:GroupTypeModels
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter an group name.")]
        [Display(Name = "Tên nhóm")]
        public string GroupName { get; set; }
        [Display(Name = "Loại nhóm")]
        public int GroupType { get; set; }
        [Display(Name = "Mô tả")]
        public string Description { get; set; }
        [Display(Name = "Khách sạn")]
        public string SysHotelCode { get; set; }
        [Display(Name = "Trạng thái")]
        public int Status { get; set; }
        public int UserID { get; set; }
    }

    /// <summary>
    /// ThichPV 11/29/2016 18:12:41
    /// Lấy ra tên KS, tên Loại nhóm, Tên User, tên Trạng thái
    /// </summary>
    public class GroupDetailModel
    {
        [Display(Name = "Mã nhóm:")]
        public int ID { get; set; }

        [StringLength(500, ErrorMessage = "Tên nhóm quyền có độ dài tối đa là 500 ký tự")]
        [Display(Name = "Tên nhóm:")]
        [Required(ErrorMessage = "Tên nhóm không để trống")]
        public string GroupName { get; set; }

        [Display(Name = "Loại nhóm:")]
        [Required(ErrorMessage = "Loại nhóm không để trống")]
        public int GroupType { get; set; }
        [Display(Name = "Loại nhóm:")]
        public string GroupTypeName { get; set; }

        [Display(Name = "Khách sạn:")]
        [Required(ErrorMessage = "Khách sạn không để trống")]
        public string SysHotelCode { get; set; }
        [Display(Name = "Khách sạn:")]
        public string SysHotelName { get; set; }

        [Display(Name = "Mô tả:")]
        public string Description { get; set; }

        [Display(Name = "Trạng thái:")]
        [Required(ErrorMessage = "Trạng thái không để trống")]
        public Status Status { get; set; }

        [Display(Name = "Người tạo ID:")]
        [Required(ErrorMessage = "Không thấy thông tin tài khoản")]
        public int UserID { get; set; }

        [Display(Name = "Người tạo:")]
        [Required(ErrorMessage = "Không thấy thông tin tài khoản")]
        public string UserName { get; set; }
    }
}