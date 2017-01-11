using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Oze.Models
{
    public class ModulesModel
    {
        public int ID { get; set; }
        public string ModuleName { get; set; }
        public string ModuleDesc { get; set; }
        public int Createby { get; set; }
        public DateTime CreateDate { get; set; }
        public int Modifyby { get; set; }
        public DateTime ModifyDate { get; set; }
    }
}