using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Oze.Models
{
    public class SupplierModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string ContactName { get; set; }
        public string Address { get; set; }
      
       
        public string Email { get; set; }
        public string Mobile { get; set; }      
        public string Note { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }


        
        public SupplierModel()
        {
            this.ID = -1;
            this.Name = "";
            this.ContactName = "";
            this.Address = "";

            
            this.Email = "";
            this.Mobile = "";
            this.Note = "";
            this.CreateDate = DateTime.Now;
        }
    }
}