using System;
using System.ComponentModel.DataAnnotations;

namespace Oze.Models
{
    public class DefineCategoryModel
    {
        public int ID { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public int Createby { get; set; }
        public DateTime CreateDate { get; set; }
    }
    /// <summary>
    /// THICHPV 2016/12/06 
    /// List danh mục dùng chung
    /// </summary>
    public class DefineCategoryListModel
    {
        public int ID { get; set; }

        [StringLength(50, ErrorMessage = "Loại có độ dài tối đa là 50 ký tự")]
        [Display(Name = "Loại danh mục:")]
        [Required(ErrorMessage = "Loại danh mục không để trống")]
        public string Type { get; set; }

        [StringLength(100, ErrorMessage = "Giá trị có độ dài tối đa là 100 ký tự")]
        [Display(Name = "Giá trị:")]
        [Required(ErrorMessage = "Giá trị không để trống")]
        public string Value { get; set; }

        [StringLength(200, ErrorMessage = "Tên danh mục có độ dài tối đa là 200 ký tự")]
        [Display(Name = "Tên danh mục:")]
        [Required(ErrorMessage = "Tên danh mục không để trống")]
        public string Name { get; set; }

        [Display(Name = "Sắp xếp:")]
        [Required(ErrorMessage = "Sắp sếp không để trống")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Sắp sếp thứ tự phải là dạng số")]
        public int Order { get; set; }

        [Display(Name = "Người tạo:")]
        [Required(ErrorMessage = "Không thấy thông tin tài khoản")]
        public string CreateByUser { get; set; }

        [Display(Name = "Người tạo:")]
        [Required(ErrorMessage = "Không thấy thông tin tài khoản")]
        public int Createby { get; set; }

        [Display(Name = "Ngày tạo:")]
        [Required(ErrorMessage = "Không thấy thông tin ngày tạo")]
        public DateTime CreateDate { get; set; }
    }
}