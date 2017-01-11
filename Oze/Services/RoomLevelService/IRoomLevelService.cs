using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using oze.data;
using Oze.Models;

namespace Oze.Services.RoomLevelService
{
    public interface IRoomLevelService
    {
        List<tbl_Room_Type> Search(PagingModel page, out int total);
        tbl_Room_Type GetRoomTypeById(string id);
        int UpdateOrInsertRoomType(tbl_Room_Type obj);
        int DeleteRoomLevel(int id);


    }
}