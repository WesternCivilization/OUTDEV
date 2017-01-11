using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using oze.data;
using Oze.Models;
using Oze.Models.CustomerManage;
using Vw_RoomActive = oze.data.Entity.Vw_RoomActive;
using Vw_RoomPriceLevel = oze.data.Entity.Vw_RoomPriceLevel;

namespace Oze.Services
{
    interface IRoomPriceLevelService
    {
        List<Vw_RoomPriceLevel> getAll(PagingModel page,int hotelid, out int count);

        int LockRoom(int id);



        oze.data.Entity.Vw_InforCustomer_Room GetCustomerRoom(int id);

        List<tbl_Room_Type> GeTblRoomTypes(int hotelid);

        List<tbl_Hotel> GetHotels();
        tbl_Hotel GetHotelsByid(int id);
        List<oze.data.Entity.Vw_RoomActive> GetRooms(int id,int hotelid,int typeRoom);
        bool InsertRoomPriceLevel(List<tbl_Room> listRoom, tbl_RoomPriceLevel model,
            List<tbl_RoomPriceLevel_Extra> listXtraDay,
             List<tbl_RoomPriceLevel_Hour> listPriceDay,
             List<tbl_RoomPriceLevel_Extra> listXtraNight,
             List<tbl_RoomPriceLevel_Extra> listEarlyDay,
             List<tbl_RoomPriceLevel_Extra> listEarlyNight,
             List<tbl_RoomPriceLevel_Extra> listLimitPerson, List<tbl_RoomPriceLevel_Extra> listLimitPerson_Child);
        bool UpdateRoomPriceLevel(List<tbl_Room> listRoom, tbl_RoomPriceLevel model,
           List<tbl_RoomPriceLevel_Extra> listXtraDay,
            List<tbl_RoomPriceLevel_Hour> listPriceDay,
            List<tbl_RoomPriceLevel_Extra> listXtraNight,
            List<tbl_RoomPriceLevel_Extra> listEarlyDay,
            List<tbl_RoomPriceLevel_Extra> listEarlyNight,
            List<tbl_RoomPriceLevel_Extra> listLimitPerson, List<tbl_RoomPriceLevel_Extra> listLimitPerson_Child);

        oze.data.Entity.Vw_RoomPriceLevel GetPriceLevel(int id);
        List<tbl_RoomPriceLevel_Extra> GeTblRoomPriceLevelExtras(int RoomPriceLevelID,int type);
        List<oze.data.Entity.Vw_RoomActive> GetRoomActives(int  LevelID, int hotelid);
        List<tbl_RoomPriceLevel_Hour> GeLevelHourses(int RoomPriceLevelID);
        bool checkDate(DateTime fdate, DateTime tdate);
    }
}
