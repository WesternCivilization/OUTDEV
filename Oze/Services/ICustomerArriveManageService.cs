using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using oze.data;
using Oze.Models;
using Oze.Models.CustomerArriveManage;
using Oze.Models.CustomerManage;


namespace Oze.Services
{
    interface ICustomerArriveManageService
    {
        List<Vw_CustomerArrive_Room> getAll(PagingModel page, SearchCustomerArriveModel model, out int count);
        List<tbl_Country> getAllCountry();
        tbl_RoomUsing GetRoomUsingCheckIn(int id);
        List<HistoryCustomer> GetHistory(int id);
        JsonRs AddUsingRoom(CustomeCheckInModel obj);
        JsonRs UndoRoom(CustomeCheckInModel obj);
        JsonRs ChangeRoom(int id, int CheckInID,string note, DateTime _tdate);
        List<tbl_Customer> AutoCompleteCustomer(string text, int customerold);
        List<tbl_Room_Type> GetRoomTypes(int hotelid);
        List<tbl_Room> GeTblRoomsByType(int hotelid, int typeid);
        oze.data.Entity.Vw_InforCustomer GetCustomerRoom(int id);
        oze.data.Entity.Vw_InforCustomer GetCustomerRoomByCheckInID(int CheckInid);

        List<tbl_Product> GetlistProduct(int SysHotelID,int roomid);

        #region viết cho phần checkout

        List<Vw_ProductService> GetListCustomerServices(int checkinID);
        JsonRs InsertCustomeServer(int productId, int checkinID, int hotelID, int customerid, int quantity);
        JsonRs InsertNewOtherService(string name, int checkinID, int hotelID, int customerid, double price);
        JsonRs DeleteCustomeServer(int cussvID);
        JsonRs PaymentCheckOut(int checkinID, string Tdate,int khunggio);
        JsonRs PayBillCheckOut(int checkinID, string Tdate, int khunggio);
        bool SendMailHepelink(string files, string to, string subject, string body);
        Vw_HotelsConfig GetConfig(int hotelID);

        int UpdateOrInsert(tbl_HotelsConfig obj);

        #endregion


    }
}
