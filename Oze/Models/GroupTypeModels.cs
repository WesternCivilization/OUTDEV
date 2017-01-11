using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Oze.Models
{
    public class GroupTypeModels
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter an group name.")]
        [Display(Name = "Tên loại nhóm VN")]
        public string NameVN { get; set; }
        [Display(Name = "Tên loại nhóm EN")]
        public string NameEN { get; set; }
        
        [Display(Name = "Thứ tự")]
        public int Order { get; set; }
    }
}