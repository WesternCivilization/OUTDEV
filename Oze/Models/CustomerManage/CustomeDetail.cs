using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using oze.data;

namespace Oze.Models.CustomerManage
{
    public class CustomeDetail
    {
        public int Id { get; set; }
        public int? SourceID { get; set; }
        public string HotelCode { get; set; }
       
        public int CustomerTypeID { get; set; }
        public string Name { get; set; }
        public string DOB { get; set; }
      
        public string IdentifyNumber { get; set; }
        public int? Sex { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Fax { get; set; }
        public string TaxCode { get; set; }
        public int? CitizenshipCode { get; set; }
        public string Company { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? CreateBy { get; set; }
        public DateTime? ModifyDate { get; set; }
        public int? ModifyBy { get; set; }
        public bool Leader { get; set; }
        public bool Payer { get; set; }
        public int? CountryId { get; set; }
        public int? TeamSTT { get; set; }
        public int? TeamMergeSTT { get; set; }
        public List<tbl_Country> ListCountry { get; set; }
    }
}