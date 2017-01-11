using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Oze.Models
{
    public class MenusModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public int ParentID { get; set; }
        public int Level { get; set; }
        public int Order { get; set; }
        public int Status { get; set; }
        public string ModuleName { get; set; }
        public int countChild { get; set; }
        public string Icon { get; set; }
    }
    public class ParentMenu {
        public ParentMenu() {
            MenuViewModel = new List<MenusModel>();
        }
        public List<MenusModel> MenuViewModel { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public int ParentID { get; set; }
        public int Level { get; set; }
        public int Order { get; set; }
        public int Status { get; set; }
        public string ModuleName { get; set; }
        public int countChild { get; set; }
        public string Icon { get; set; }

    }

}