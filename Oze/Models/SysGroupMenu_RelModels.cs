using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Oze.Models
{
    public class SysGroupMenu_RelModels: MenusModel
    { 
        public int SysMenuID { get; set; }
        public int SysGroupID { get; set; }
        public string SysGroupName { get; set; }
        public int UserID { get; set; }
    }
}