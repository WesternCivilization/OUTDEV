using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Oze.Models
{
    public class AccessRightModels
    {
        public Nullable<int> RuleID { get; set; }
        public int GroupID { get; set; }
        public string GroupName { get; set; }
        public int ModelID { get; set; }
        public string ModelName { get; set; }
        public bool Read { get; set; }
        public bool Write { get; set; }
        public bool Create { get; set; }
        public bool Delete { get; set; }
        public int UserID { get; set; }
    }
}