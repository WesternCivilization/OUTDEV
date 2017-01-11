using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.DataAnnotations;

namespace oze.data.Entity
{
  public  class Vw_RoomActive : data.Vw_RoomActive
    {
        [Ignore]
        public int IdActive { get; set; }
    }
}
