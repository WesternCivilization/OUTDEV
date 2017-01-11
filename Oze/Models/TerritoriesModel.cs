using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Oze.Models
{
    public class TerritoriesModel
    {
        public string Activity { get; set; }

        [Required(ErrorMessage = "Chưa nhập Mã Tỉnh/Thành phố")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mã Tỉnh/Thành phố phải là dạng số")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Mã Tỉnh/Thành phố có độ dài là 2 ký tự")]
        [Display(Name = "Mã Tỉnh/Thành phố:")]
        public string ProvinceId { get; set; }

        [Required(ErrorMessage = "Chưa nhập Tên Tỉnh/Thành phố")]
        [Display(Name = "Tỉnh/Thành phố:")]
        public string ProvinceName { get; set; }

        [Required(ErrorMessage = "Chưa nhập Mã Quận/Huyện")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mã Quận/Huyện phải là dạng số")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Mã Quận/Huyện có độ dài là 3 ký tự")]
        [Display(Name = "Mã Quận/Huyện:")]
        public string DistrictId { get; set; }

        [Required(ErrorMessage = "Chưa nhập Tên Quận/Huyện")]
        [Display(Name = "Quận/Huyện:")]
        public string DistrictName { get; set; }

        [Required(ErrorMessage = "Chưa nhập Mã Phường/Xã")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mã Phường/Xã phải là dạng số")]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "Mã Phường/Xã có độ dài là 5 ký tự")]
        [Display(Name = "Mã Phường/Xã:")]
        public string WardsId { get; set; }

        [Required(ErrorMessage = "Chưa nhập Tên Phường/Xã")]
        [Display(Name = "Phường/Xã:")]
        public string WardsName { get; set; }

    }
}