using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Oze.Models
{
    public class SysUserModel
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string IdentityNumber { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public int Department { get; set; }
        public int ParentID { get; set; }
        public int SysHotelID { get; set; }
        public int Status { get; set; }
        public int IsActive { get; set; }
        public int FirstLogin { get; set; }
        public int Createby { get; set; }
        public DateTime CreateDate { get; set; }
        public int Modifyby { get; set; }
        public string NameModifyby { get; set; }
        public string NameCreateby { get; set; }
        public string NameSysHotelID { get; set; }
        public string NameParentID { get; set; }
        public DateTime ModifyDate { get; set; }
        public string CodeSysHotel { get; set; }
        public string NameSysHotel { get; set; }
        public string HotelsGroupCode { get; set; }


        public SysUserModel()
        {
            this.ID = -1;
            this.UserName = "";
            this.Password = "";
            this.IdentityNumber = "";
            this.FullName = "";
            this.Address = "";
            this.Email = "";
            this.Mobile = "";
            this.Department = -1;
            this.ParentID = -1;
            this.SysHotelID = -1;
            this.Status = -1;
            this.IsActive = -1;
            this.FirstLogin = -1;
            this.Createby = -1;
            this.CreateDate = DateTime.Now.AddYears(-1000);
            this.Modifyby = -1;
            this.ModifyDate = DateTime.Now.AddYears(-1000);
            this.CodeSysHotel = "";
            this.NameSysHotel = "";
            this.HotelsGroupCode = "";
        }
    }
}