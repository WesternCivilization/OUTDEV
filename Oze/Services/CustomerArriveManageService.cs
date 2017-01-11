using oze.data;
using Oze.Models;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Web;
using Oze.Models.CustomerArriveManage;
using Oze.Models.CustomerManage;
using Vw_InforCustomer = oze.data.Entity.Vw_InforCustomer;
using oze.data.Entity;
using Oze.AppCode.Util;

namespace Oze.Services
{
    public class CustomerArriveManageService : ICustomerArriveManageService
    {
        IOzeConnectionFactory _connectionData;

        public CustomerArriveManageService()
        {
            OrmLiteConfig.DialectProvider = SqlServerDialect.Provider;
            OrmLiteConfig.DialectProvider.UseUnicode = true; // nhập tiếng việt
            _connectionData = new OzeConnectionFactory(ConfigurationManager.AppSettings["ConnectionString_OzeGeneral"],
    SqlServerOrmLiteDialectProvider.Instance);

        }

        public List<Vw_CustomerArrive_Room> getAll(PagingModel page, SearchCustomerArriveModel model, out int count)
        {
            if (page.search == null) page.search = "";

            //  ServiceStackHelper.Help();
            //  LicenseUtils.ActivatedLicenseFeatures();           
            //search again
            //DateTime _fdate;
            //DateTime _tdate;

            //DateTime.TryParse(model.Fdate, CultureInfo.GetCultureInfo("vi-vn"), DateTimeStyles.None, out _fdate);
            //DateTime.TryParse(model.Tdate, CultureInfo.GetCultureInfo("vi-vn"), DateTimeStyles.None, out _tdate);
            //_tdate = _tdate.AddDays(1);
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<Vw_CustomerArrive_Room>();
                if (!string.IsNullOrEmpty(model.Name))
                    query.Where(x => x.CustomerName.Contains(model.Name));
                if (!string.IsNullOrEmpty(model.Email))
                    query.Where(x => x.Email == model.Email.Trim());

                if (!string.IsNullOrEmpty(model.Phone))
                    query.Where(x => x.Phone == model.Phone.Trim());

                if (!string.IsNullOrEmpty(model.Name))
                    query.Where(x => x.CustomerName.Contains(model.Name));

                if (model.RooTypeID > 0)
                    query.Where(x => x.Room_Type_ID == model.RooTypeID);

                if (model.RoomID > 0)
                    query.Where(x => x.roomid == model.RoomID);
                if (model.HotelID > 0)
                    query.Where(x => x.SysHotelID == model.HotelID);
                //if (model.CheckDate)
                //{
                //    query.Where(x => x.CreateDate >= _fdate && x.CreateDate <= _tdate);
                //}
                //query.Where(x => x.Status == true);
                query.OrderByDescending(x => x.Id);

                int offset = 0;
                try
                {
                    offset = page.offset;
                }
                catch
                {
                }

                int limit = 10; //int.Parse(Request.Params["limit"]);
                try
                {
                    limit = page.limit;
                }
                catch
                {
                }

                var rows = db.Select(query);
                count = rows.Count;
                rows = rows.Skip(offset).Take(limit).ToList();

                return rows;
            }
        }




        public List<tbl_Country> getAllCountry()
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Country>();
                var rows = db.Select(query);
                return rows;
            }
        }


        public tbl_RoomUsing GetRoomUsingCheckIn(int id)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.Single<tbl_RoomUsing>(x => x.CheckInID == id);
                return query;
            }
        }


        public JsonRs AddUsingRoom(CustomeCheckInModel obj)
        {
            var rs = new JsonRs();


            using (var db = _connectionData.OpenDbConnection())
            {
                if (obj.CustomerId > 0)
                {
                    try
                    {

                        var usingroom = new tbl_RoomUsing();
                        usingroom.SysHotelID = comm.GetHotelId();// obj.SysHotelID;
                        usingroom.CheckInID = obj.CheckInID;
                        usingroom.customerid = obj.CustomerId;
                        usingroom.datecreated = DateTime.Now;
                        usingroom.roomid = obj.Roomid;
                        usingroom.status = 2;
                        db.Insert(usingroom);
                        rs.Status = "01";
                        rs.Message = "Thêm mới khách ở cùng thàng công.";
                        return rs;
                    }
                    catch (Exception e)
                    {
                        rs.Status = "00";
                        rs.Message = "Thêm mới khách ở cùng thất bại!";
                        return rs;
                    }

                }
                else
                {
                    using (var tran = db.OpenTransaction())
                    {

                        DateTime DOB;
                        DateTime.TryParse(obj.DOB, CultureInfo.GetCultureInfo("vi-vn"), DateTimeStyles.None, out DOB);
                        try
                        {
                            var Customer = new tbl_Customer();
                            Customer.Name = obj.Name;
                            Customer.CountryId = obj.CountryId;
                            Customer.DOB = DOB;
                            Customer.Address = obj.Address;
                            Customer.Sex = obj.Sex;
                            Customer.TeamSTT = obj.TeamSTT;
                            Customer.TeamMergeSTT = obj.TeamMergeSTT;
                            Customer.Email = obj.Email;
                            Customer.IdentifyNumber = obj.IdentifyNumber;
                            Customer.Phone = obj.Phone;
                            Customer.Address = obj.Address;
                            Customer.Company = obj.Company;
                            Customer.Status = true;
                            Customer.TeamMergeSTT = obj.TeamMergeSTT;
                            Customer.TeamSTT = obj.TeamSTT;
                            Customer.Payer = 0;
                            Customer.Status = true;
                            Customer.ReservationID = obj.CheckInID;
                            Customer.HotelCode = comm.GetHotelCode();
                            Customer.SysHotelID = comm.GetHotelId();

                            long idCustomer=  db.Insert(Customer, true);

                            var usingroom = new tbl_RoomUsing();
                            usingroom.SysHotelID = comm.GetHotelId();//Customer.SysHotelID;
                            usingroom.SysHotelCode = comm.GetHotelCode();
                            usingroom.CheckInID = obj.CheckInID;
                            usingroom.customerid = (int)idCustomer;// Customer.Id;
                            usingroom.datecreated = DateTime.Now;
                            usingroom.roomid = obj.Roomid;
                            usingroom.status = 2;
                            db.Insert(usingroom);
                            /*
                            var group = new tbl_CustomerGroup();
                            group.Doan_ID = obj.TeamSTT;
                            group.Gop_Doan_ID = obj.TeamMergeSTT;
                            group.Customer_ID = Customer.Id;
                            db.Insert(group);
                            */

                            tran.Commit();
                            rs.Status = "01";
                            rs.Message = "Thêm mới khách ở cùng thàng công.";
                            return rs;
                        }
                        catch (Exception)
                        {
                            tran.Rollback();
                            rs.Status = "00";
                            rs.Message = "Thêm mới khách ở cùng thất bại!";
                            return rs;

                        }
                    }

                }

            }

        }




        public List<HistoryCustomer> GetHistory(int id)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_CustomerGroup>();
                query.Join<tbl_Reservation_Room, tbl_CustomerGroup>((x, y) => x.Id == y.ReserID);
                query.Join<tbl_CustomerGroup, tbl_Customer>((x, y) => x.Customer_ID == y.Id);
                query.Join<tbl_Reservation_Room, tbl_Room_Status>((x, y) => x.ReservationStatus == y.Id);
                query.Where<tbl_CustomerGroup>(x => x.Customer_ID == id);
                var rows = db.Select<HistoryCustomer>(query).ToList();
                return rows;
            }
        }



        public oze.data.Entity.Vw_InforCustomer GetCustomerRoom(int id)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var obj = db.Single<Vw_InforCustomer>(x => x.Id == id);
                return obj;
            }
        }

        public oze.data.Entity.Vw_InforCustomer GetCustomerRoomByCheckInID(int CheckInid)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var obj = db.Single<Vw_InforCustomer>(x => x.CheckInID == CheckInid);
                return obj;
            }
        }


        public List<tbl_Customer> AutoCompleteCustomer(string text, int customerold)
        {
            try
            {
                using (var db = _connectionData.OpenDbConnection())
                {
                    var obj = db.Select<tbl_Customer>(x => x.Name.Contains(text) && x.Id != customerold);
                    return obj;
                }
            }
            catch (Exception e)
            {
                return new List<tbl_Customer>();
                Console.WriteLine(e);
                throw;
            }
        }

        public List<tbl_Room_Type> GetRoomTypes(int hotelid)
        {

            try
            {
                using (var db = _connectionData.OpenDbConnection())
                {
                    var query = db.From<tbl_Room_Type>();
                    if (hotelid > 0)
                        query.Where(x => x.HotelID == hotelid);

                    return db.Select<tbl_Room_Type>(query);
                }
            }
            catch (Exception e)
            {
                return new List<tbl_Room_Type>();
                Console.WriteLine(e);
                throw;
            }
        }

        public List<tbl_Room> GeTblRoomsByType(int hotelid, int typeid)
        {
            try
            {
                using (var db = _connectionData.OpenDbConnection())
                {
                    var query = db.From<tbl_Room>();
                    if (hotelid > 0)
                        query.Where(x => x.SysHotelID == hotelid);
                    if (typeid > 0)
                        query.Where(x => x.RoomType_ID == typeid);
                    db.Select<tbl_Room>(x => x.SysHotelID == hotelid && x.RoomType_ID == typeid);
                    return db.Select<tbl_Room>(query);
                }
            }
            catch (Exception e)
            {
                return new List<tbl_Room>();
                Console.WriteLine(e);
                throw;
            }
        }

        public JsonRs UndoRoom(CustomeCheckInModel obj)
        {
            var rs = new JsonRs();


            using (var db = _connectionData.OpenDbConnection())
            {
                try
                {
                    var usingroom =
                        db.Single<tbl_RoomUsing>(
                            x => x.status == 1 && x.CheckInID == obj.CheckInID && x.roomid == obj.Roomid);
                    if (usingroom != null)
                    {
                        usingroom.status = 2;
                        db.Update(usingroom);
                        rs.Status = "01";
                        rs.Message = "Trả phòng thành công.";
                        return rs;
                    }
                    else
                    {
                        rs.Status = "00";
                        rs.Message = "Không tìm thấy phòng của khách hàng. Vui lòng kiểm tra lại.";
                    }


                }
                catch (Exception e)
                {
                    rs.Status = "00";
                    rs.Message = "Trả phòng thất bại!";
                    return rs;
                }
            }

            return rs;
        }

        public JsonRs ChangeRoom(int roomid, int CheckInID, string note, DateTime _tdate)
        {

            var rs = new JsonRs();


            using (var db = _connectionData.OpenDbConnection())
            {
                using (var tran = db.OpenTransaction())
                {
                    try
                    {
                        //khi doi phong capnhat trang thai cu ve5 va them 1 thằng mới Stt=2
                        var checkin = db.Single<tbl_CheckIn>(x => x.Id == CheckInID);
                        checkin.Leave_Date = _tdate;
                        var Listchangeroom = new List<tbl_RoomChange>();
                        var listusingroonew = new List<tbl_RoomUsing>();
                        var listusingroomold =
                            db.Select<tbl_RoomUsing>(x => x.status == 1 && x.CheckInID == CheckInID);
                        if (listusingroomold.Count == 0)
                        {
                            rs.Status = "00";
                            rs.Message = "Không tìm thấy phòng củ vui lòng kiểm tra lại!";
                            return rs;
                        }

                        foreach (tbl_RoomUsing t in listusingroomold)
                        {
                            t.status = 5;
                            var changeroom = new tbl_RoomChange()
                            {
                                SysHotelID = t.SysHotelID,
                                creatorid = 1,
                                customercode = t.customercode,
                                customerid = t.customerid,
                                datecreated = t.datecreated,
                                newroomid = t.SysHotelID,
                                oldeoomid = t.Id,
                            };
                            Listchangeroom.Add(changeroom);


                            var usingroomnew = new tbl_RoomUsing()
                            {
                                SysHotelID = t.SysHotelID,
                                customercode = t.customercode,
                                customerid = t.customerid,
                                datecreated = t.datecreated,
                                roomid = roomid,
                                status = 1,
                                CheckInID = t.CheckInID,
                                roomcode = t.roomcode,
                                SysHotelCode = t.SysHotelCode,
                                Note = note

                            };
                            listusingroonew.Add(usingroomnew);
                        }
                        //cập nhật trạng thái phòng mới
                        var listroom = new List<tbl_Room>();
                        var roomnew = db.Single<tbl_Room>(x => x.Id == roomid);
                        roomnew.status =1;
                        roomnew.datePlanFrom = checkin.Arrive_Date;//cập nhật ngày đến
                        roomnew.datePlanTo = checkin.Leave_Date;// cập nhật ngày đi

                        //phòng cũ
                        var roomold = db.Single<tbl_Room>(x => x.Id == listusingroomold[0].roomid);
                        roomold.status = 0;
                        listroom.Add(roomnew);
                        listroom.Add(roomold);

                        //cập nhật lai trạng thái hai phòng mới
                        db.UpdateAll(listroom);

                        //cập nhật lại tragj Thái Using củ với Status về bằng 5
                        db.UpdateAll(listusingroomold);
                        
                        // Thêm UsingRoommowis với Stt mới bằng 1
                        db.InsertAll(listusingroonew);
                        
                        //thêm Using Change
                        db.InsertAll(Listchangeroom);
                        db.Update(checkin);
                        tran.Commit();
                        rs.Status = "01";
                        rs.Message = "Đổi phòng cho khách hàng thành công. Lưu ý tất cả khách hàng trong phòng sẻ được chuyển tới phòng mới là : " + roomnew.Name;


                    }
                    catch (Exception e)
                    {
                        rs.Status = "00";
                        rs.Message = "Đổi phòng thất bại. Vui lòng thực hiện lại!";
                        return rs;
                    }
                }
            }

            return rs;
        }

        public List<Vw_ProductService> GetListCustomerServices(int checkinID)
        {
            try
            {
                using (var db = _connectionData.OpenDbConnection())
                {
                    var query = db.From<Vw_ProductService>();

                    query.Where(x => x.CheckinID == checkinID && x.status == 1);

                    return db.Select<Vw_ProductService>(query);
                }
            }
            catch (Exception e)
            {
                return new List<Vw_ProductService>();
                Console.WriteLine(e);
                throw;
            }
        }

        public List<tbl_Product> GetlistProduct(int SysHotelID, int roomid)
        {
            try
            {
                using (var db = _connectionData.OpenDbConnection())
                {
                    var query = db.From<tbl_Product>();
                    query.Join<tbl_StoreProduct_Config, tbl_Product>((x, y) => x.productid == y.Id);
                    query.Join<tbl_Store, tbl_StoreProduct_Config>((x, y) => x.Id == y.storeid && x.roomid == roomid);
                    //query.Where<tbl_Store>(x => x.roomid == roomid);
                    query.Where(x => x.SysHotelID == SysHotelID && x.UnitID != 0);
                    List<tbl_Product> lst = db.Select<tbl_Product>(query);
                    
                    if (lst.Count ==0)//nếu ko có thì lấy từ kho tổng
                    {
                        query = db.From<tbl_Product>();
                        query.Join<tbl_Store, tbl_Product>((x, y) => x.SysHotelID == y.SysHotelID);
                        query.Where<tbl_Store>(x => x.roomid == roomid);
                        query.Where(x => x.SysHotelID == SysHotelID && x.UnitID != 0);
                        lst = db.Select<tbl_Product>(query);
                    }
                    return lst;
                }
            }
            catch (Exception e)
            {
                return new List<tbl_Product>();
                Console.WriteLine(e);
                throw;
            }
        }

        public JsonRs InsertCustomeServer(int productId, int checkinID, int hotelID, int customerid, int quantity)
        {
            var rs = new JsonRs();
            try
            {
                using (var db = _connectionData.OpenDbConnection())
                {
                    using (var tran = db.OpenTransaction())
                    {
                        //if (db.Single<Vw_ProductService>(x => x.CheckinID == checkinID && x.productid == productId && x.status == 1) != null)
                        //{
                        //    rs.Status = "00";
                        //    rs.Message = "Dịch vụ đã tồn tại!";
                        //    return rs;
                        //}
                        var sv = new tbl_CustomerService()
                        {
                            customerid = customerid,
                            CheckInID = checkinID,
                            SysHotelID = hotelID,
                            productid = productId,
                            productcode = "",
                            datecreated = DateTime.Now,
                            Quantity = quantity,
                            status = 1,
                        };
                        db.Save(sv, true);

                        //trừ tồn kho tương ứng
                        //trừ tồn kho tương ứng
                        //tìm minibar ứng với phòng đang ở
                        var usingroom = db.Single<tbl_RoomUsing>(x => x.CheckInID == checkinID);

                        var queryStore = db.From<tbl_Store>().Where(e => e.roomid == usingroom.roomid);
                        var firstStore = db.Select(queryStore).FirstOrDefault();//nếu có kho

                        if (firstStore == null)//nếu ko có kho thì trừ từ kho tổng
                        {
                            firstStore = db.Select(db.From<tbl_Store>()
                                .Where(e => e.typeStore == 1 && e.SysHotelID == comm.GetHotelId()))
                                .FirstOrDefault();
                        }
                        if (firstStore != null)//nếu  có kho thì tiến hành trừ
                        {
                            //duyệt qua các sản phẩm
                            //foreach (var oProduct in product)
                            {
                                var queryProductStore = db.From<tbl_StoreProduct>()
                                    .Where(e => e.productid == productId && e.storeid == firstStore.Id)
                                    .OrderBy(e => e.quantity);
                                var firstProductInStore = db.Select(queryProductStore).FirstOrDefault();
                                if (firstProductInStore != null)
                                {
                                    firstProductInStore.quantity = firstProductInStore.quantity - quantity;
                                    db.Update(firstProductInStore);
                                }
                            }
                        }

                        tran.Commit();
                        rs.Status = "01";
                        rs.Data = db.Single<Vw_ProductService>(x => x.CheckinID == checkinID && x.productid == productId && x.cussvID == sv.Id);
                    }
                    return rs;
                }
            }
            catch (Exception e)
            {
                rs.Status = "00";
                rs.Message = "Thêm mới dịch vụ thất bại";
                return rs;
                throw;
            }
        }
        public JsonRs InsertNewOtherService(string name, int checkinID, int hotelID, int customerid, double price)
        {
            var rs = new JsonRs();

            using (var db = _connectionData.OpenDbConnection())
            {
                using (var tran = db.OpenTransaction())
                {
                    try
                    {
                        if (
                      db.Single<tbl_Product>(x => x.Name.ToUpper() == name.Trim().ToUpper()) != null)
                        {
                            rs.Status = "00";
                            rs.Message = "Dịch vụ có thể đã tồn tại trên hệ thống!";
                        }
                        var p = new tbl_Product()
                        {
                            Status = 1,
                            Name = name,
                            Code = "",
                            CreateDate = DateTime.Now,
                            Createby = 1,
                            DateOrder = DateTime.Now,
                            Description = "dịch vụ khác",
                            ModifyDate = DateTime.Now,
                            ProductCateID = 0,
                            ProductGroupID = 0,
                            SalePrice = price,
                            PriceOrder = price,
                            SupplierID = 0,
                            SysHotelID = hotelID,
                            Modifyby = 1,
                            PictureUrl = "",
                            UnitID = 0,
                            QuotaMinimize = 1,

                        };
                        db.Save(p, true);
                        var sv = new tbl_CustomerService()
                        {
                            customerid = customerid,
                            CheckInID = checkinID,
                            SysHotelID = hotelID,
                            productid = p.Id,
                            productcode = "",
                            datecreated = DateTime.Now,
                            Quantity = 1,
                            status = 1,
                        };
                        db.Save(sv, true);
                        rs.Data = sv;

                        tran.Commit();
                        rs.Status = "01";
                        return rs;
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        rs.Status = "00";
                        rs.Message = "Thêm mới dịch vụ thất bại";
                        return rs;
                        throw;
                    }
                }

            }

        }

        public JsonRs DeleteCustomeServer(int cussvID)
        {
            var rs = new JsonRs();
            try
            {
                using (var db = _connectionData.OpenDbConnection())
                {
                    using (var trans = db.OpenTransaction())
                    {
                       
                        var cus = db.Single<tbl_CustomerService>(
                            x => x.Id == cussvID && x.status == 1);
                        cus.status = 0;
                        
                       
                        db.Update(cus);

                        //cộng trả lại kho cho nó
                        var usingroom = db.Single<tbl_RoomUsing>(x => x.CheckInID == cus.CheckInID);
                        var queryStore = db.From<tbl_Store>().Where(e => e.roomid == usingroom.roomid);
                        var firstStore = db.Select(queryStore).FirstOrDefault();//nếu có kho

                        if (firstStore == null)//nếu ko có kho thì trừ từ kho tổng
                        {
                            firstStore = db.Select(db.From<tbl_Store>()
                                .Where(e => e.typeStore == 1 && e.SysHotelID == comm.GetHotelId()))
                                .FirstOrDefault();
                        }
                        if (firstStore != null)//nếu  có kho thì tiến hành trừ
                        {
                            //duyệt qua các sản phẩm
                            //foreach (var oProduct in product)
                            {
                                var queryProductStore = db.From<tbl_StoreProduct>()
                                    .Where(e => e.productid == cus.productid && e.storeid == firstStore.Id)
                                    .OrderBy(e => e.quantity);
                                var firstProductInStore = db.Select(queryProductStore).FirstOrDefault();
                                if (firstProductInStore != null)
                                {
                                    firstProductInStore.quantity = firstProductInStore.quantity + cus.Quantity;
                                    db.Update(firstProductInStore);
                                }
                            }
                        }
                        trans.Commit();
                        rs.Status = "01";
                        rs.Message = "Xóa dịch vụ thành công";
                        return rs;
                    }
                }
            }
            catch (Exception ex)
            {
                rs.Status = "00";
                rs.Message = "Thêm mới dịch vụ thất bại";
                return rs;
                throw;
            }
        }

        public JsonRs PaymentCheckOut(int checkinID, string Tdate, int khunggio)
        {

            var rs = new JsonRs();

            using (var db = _connectionData.OpenDbConnection())
            {
                using (var tran = db.OpenTransaction())
                {
                    try
                    {
                        DateTime dateToLeave;
                        DateTime.TryParse(Tdate, CultureInfo.GetCultureInfo("vi-vn"), DateTimeStyles.None, out dateToLeave);
                        var check = db.Select<tbl_SaleOrder>(x => x.CheckInID == checkinID).FirstOrDefault();

                        if (check != null)
                        {
                            rs.Status = "00";
                            rs.Message = "Khách hàng này đã ghi nhận thanh toán lúc : " + check.DateCreated.GetValueOrDefault().ToString("dd/MM/yyyy mm:ss");
                            return rs;
                        }
                        //lấy thời gian checkin
                        var checkin = db.Single<tbl_CheckIn>(x => x.Id == checkinID);
                        checkin.Leave_Date = dateToLeave;
                        checkin.KhungGio = khunggio;
                        if (checkin.Leave_Date.Value.CompareTo(DateTime.Now.AddMinutes(10)) > 0)
                            return JsonRs.create(-1, "Thời gian đi không thể quá thời gian hiện tại");

                        if (checkin.Leave_Date.Value.CompareTo(checkin.Arrive_Date) <= 0)
                            return JsonRs.create(-1, "Thời gian đi không thể nhỏ hơn hoặc bằng thời gian đến");


                        //khi doi phong capnhat trang thai cu ve5 va them 1 thằng mới Stt=2
                        var itemcheckin = GetCustomerRoomByCheckInID(checkinID);
                        var usingroom = db.Single<tbl_RoomUsing>(x => x.CheckInID == checkinID);
                        usingroom.status = 3;
                        
                        //cập nhật bảng đặt phòng là đã checkout
                        /**/
                        var reservation = db.Select<tbl_Reservation_Room>().Where(e => e.BookingCode == itemcheckin.BookingCode).FirstOrDefault();
                        if ( reservation!= null) 
                        {
                             reservation.ReservationStatus = ReservationStatus.CHECKOUT;
                             var reservationRel = db.Select<tbl_Reservation_Customer_Rel>().Where(e => e.reservationID == reservation.Id).FirstOrDefault();
                             if (reservationRel != null)
                             {
                                 reservationRel.status = ReservationStatus.CHECKOUT;
                                 db.Update(reservationRel);
                             }
                             db.Update(reservation);
                        }
                        
                        #region lay lai chinh sach gia
                        var Estimate = new EstimatePrice1Service().caculatePrice(itemcheckin.SysHotelID.GetValueOrDefault(),khunggio,// itemcheckin.KhungGio.GetValueOrDefault(0),
                        itemcheckin.roomid.GetValueOrDefault(0), itemcheckin.Room_Type_ID.GetValueOrDefault(0), itemcheckin.Arrive_Date.GetValueOrDefault(),
                        dateToLeave, -1,itemcheckin.Number_People,itemcheckin.Number_Children);


                        #endregion
                        var room = db.Single<tbl_Room>(x => x.Id == usingroom.roomid);
                        room.status = 0;
                        checkin.ReservationStatus = 3;
                        if (itemcheckin == null)
                        {
                            rs.Status = "00";
                            rs.Message = "Lỗi không tìm thấy khách hàng đang ở!";
                            return rs;
                        }

                        //đến việc xử lý các dịch vụ
                        var product = GetListCustomerServices(checkinID);

                        ////trừ tồn kho tương ứng
                        ////tìm minibar ứng với phòng đang ở
                        //var queryStore=db.From<tbl_Store>().Where(e=>e.roomid==usingroom.roomid);
                        //var firstStore=db.Select(queryStore).FirstOrDefault();//nếu có kho

                        //if (firstStore == null)//nếu ko có kho thì trừ từ kho tổng
                        //{
                        //    firstStore = db.Select(db.From<tbl_Store>()
                        //        .Where(e => e.typeStore==1 && e.SysHotelID==comm.GetHotelId()))
                        //        .FirstOrDefault();
                        //}
                        //if (firstStore != null)//nếu  có kho thì tiến hành trừ
                        //{
                        //    //duyệt qua các sản phẩm
                        //    foreach (var oProduct in product)
                        //    {
                        //        var queryProductStore = db.From<tbl_StoreProduct>()
                        //            .Where(e => e.productid == oProduct.productid && e.storeid == firstStore.Id)
                        //            .OrderBy(e => e.quantity);
                        //        var firstProductInStore = db.Select(queryProductStore).FirstOrDefault();
                        //        if (firstProductInStore != null)
                        //        {
                        //            firstProductInStore.quantity = oProduct.Quantity - 1;
                        //            db.Update(firstProductInStore);
                        //        }
                        //    }
                        //}

                        var total = product.Sum(x => x.TotalSale);
                        var totalroom = Estimate.Sum(x => x.price);
                        //var totalquantity = product.Sum(x => x.Quantity);

                        var saleorder = new tbl_SaleOrder()
                        {
                            SysHotelID = itemcheckin.SysHotelID,
                            DateCreated = DateTime.Now,
                            DatePayment = DateTime.Now,
                            PaymentTypeID = 1,
                            TotalAmount = total + totalroom,
                            CheckInID = itemcheckin.CheckInID,
                            CustomerID = itemcheckin.Id,
                            CreatorID = 0,
                            CustomerCode = "",
                            TypeOrder = 1,
                            Deposit = itemcheckin.Deposit.GetValueOrDefault(0),
                            Deduction = itemcheckin.Deduction.GetValueOrDefault(0),
                            Discount = itemcheckin.Discount.GetValueOrDefault(0),
                            Tax = 0
                        };
                        db.Save(saleorder, true);
                        var list = product.Select(t => new tbl_SaleOrderDetail()
                        {
                            SysHotelID = t.SysHotelID,
                            DateCreated = DateTime.Now,
                            DatePayment = DateTime.Now,
                            PaymentTypeID = 1,
                            TotalAmount = t.TotalSale,
                            Price = 0,
                            AmountNoTax = 0,
                            Tax = 0,
                            item = "",
                            catalogitem = "",
                            CreatorID = 0,
                            quantity = t.Quantity,
                            itemid = t.productid,
                            StoreID = 0,
                            TypeOrder = 2, // dịch vụ bằng 2 giá phòng bằng 1
                            catalogitemid = t.productid,
                            OrderID = saleorder.Id
                        }).ToList();


                        foreach (var price in Estimate)
                        {
                            list.Add(new tbl_SaleOrderDetail()
                            {
                                SysHotelID = itemcheckin.SysHotelID,
                                DateCreated = DateTime.Now,
                                DatePayment = DateTime.Now,
                                PaymentTypeID = 1,
                                TotalAmount = price.price,
                                Price = price.quantiy,
                                AmountNoTax = 0,
                                Tax = 0,
                                item = price.dtFrom.ToString("dd/MM/yyyy hh:mm") + "|" + price.dtTo.ToString("dd/MM/yyyy hh:mm"),
                                catalogitem = "",
                                CreatorID = 0,
                                quantity = price.quantiy,
                                itemid = price.roomid,
                                StoreID = 0,
                                TypeOrder = 1, // dịch vụ bằng 2 giá phòng bằng 1
                                catalogitemid = price.pricePolicyId,
                                OrderID = saleorder.Id,
                            });
                        }
                        if (list.Count > 0)
                            db.InsertAll(list);
                        db.Update(room);
                        db.Update(usingroom);
                        db.Update(checkin);
                        tran.Commit();
                        rs.Status = "01";
                        rs.Message = "Thanh toán thành công cho khách hàng :" + itemcheckin.CustomerName;
                        return rs;
                    }
                    catch (Exception e)
                    {
                        rs.Status = "00";
                        rs.Message = "Thanh toán thất bại. Vui lòng thực hiện lại!";
                        return rs;
                    }

                }
            }

        }
        public JsonRs PayBillCheckOut(int checkinID, string Tdate,int khunggio)
        {

            var rs = new JsonRs();

            using (var db = _connectionData.OpenDbConnection())
            {
                using (var tran = db.OpenTransaction())
                {
                    try
                    {
                        DateTime dateToLeave;
                        DateTime.TryParse(Tdate, CultureInfo.GetCultureInfo("vi-vn"), DateTimeStyles.None, out dateToLeave);
                        //danh sách giá phòng

                        var check = db.Select<tbl_SaleOrder>(x => x.CheckInID == checkinID).FirstOrDefault();
                        if (check != null)
                        {
                            rs.Status = "00";
                            rs.Message = "Khách hàng này đã ghi nhận thanh toán lúc : " + check.DateCreated.GetValueOrDefault().ToString("dd/MM/yyyy mm:ss");
                            return rs;
                        }
                        //khi doi phong capnhat trang thai cu ve5 va them 1 thằng mới Stt=2
                        var itemcheckin = GetCustomerRoomByCheckInID(checkinID);

                        #region lay lai chinh sach gia
                        var Estimate = new EstimatePrice1Service().caculatePrice(itemcheckin.SysHotelID.GetValueOrDefault(),khunggio,// itemcheckin.KhungGio.GetValueOrDefault(0),
                     itemcheckin.roomid.GetValueOrDefault(0), itemcheckin.Room_Type_ID.GetValueOrDefault(0), itemcheckin.Arrive_Date.GetValueOrDefault(),
                     dateToLeave, -1,itemcheckin.Number_People,itemcheckin.Number_Children);
                        #endregion
                        var usingroom = db.Single<tbl_RoomUsing>(x => x.CheckInID == checkinID);
                        usingroom.status = 3;
                        var checkin = db.Single<tbl_CheckIn>(x => x.Id == checkinID);
                        checkin.Leave_Date = dateToLeave;
                        checkin.KhungGio = khunggio;
                        var room = db.Single<tbl_Room>(x => x.Id == usingroom.roomid);
                        room.status = 0;
                        checkin.ReservationStatus = 3;
                        if (itemcheckin == null)
                        {
                            rs.Status = "00";
                            rs.Message = "Lỗi không tìm thấy khách hàng đang ở!";
                            return rs;
                        }
                        var product = GetListCustomerServices(checkinID);

                        var total = product.Sum(x => x.TotalSale);
                        var totalroom = Estimate.Sum(x => x.price);
                        //var totalquantity = product.Sum(x => x.Quantity);
                        #region thêm hóa đơn theo dịch vụ và giá phòng

                        // giá phòng
                        var saleorderrom = new tbl_SaleOrder()
                        {
                            SysHotelID = itemcheckin.SysHotelID,
                            DateCreated = DateTime.Now,
                            DatePayment = DateTime.Now,
                            PaymentTypeID = 1,
                            TotalAmount = totalroom,
                            CheckInID = itemcheckin.CheckInID,
                            CustomerID = itemcheckin.Id,
                            CreatorID = 0,
                            CustomerCode = "",
                            TypeOrder = 1,
                            Tax = 0,
                            Deposit = itemcheckin.Deposit.GetValueOrDefault(0),
                            Deduction = itemcheckin.Deduction.GetValueOrDefault(0),
                            Discount = itemcheckin.Discount.GetValueOrDefault(0),
                        };
                        // dịch vụ
                        var orderService = new tbl_SaleOrder()
                        {
                            SysHotelID = itemcheckin.SysHotelID,
                            DateCreated = DateTime.Now,
                            DatePayment = DateTime.Now,
                            PaymentTypeID = 1,
                            TotalAmount = total,
                            CheckInID = itemcheckin.CheckInID,
                            CustomerID = itemcheckin.Id,
                            CreatorID = 0,
                            CustomerCode = "",
                            TypeOrder = 2,
                            Tax = 0


                        };
                        // giá phòng
                        db.Save(saleorderrom, true);
                        // dịch vụ
                        db.Save(orderService, true);

                        #endregion
                        #region chi tiết hóa đơn theo dịch vụ và giá phòng
                        // giá phòng
                        var listroomdetail = Estimate.Select(t => new tbl_SaleOrderDetail()
                        {
                            SysHotelID = itemcheckin.SysHotelID,
                            DateCreated = DateTime.Now,
                            DatePayment = DateTime.Now,
                            PaymentTypeID = 1,
                            TotalAmount = t.price,
                            Price = 0,
                            AmountNoTax = 0,
                            Tax = 0,
                            item = "",
                            catalogitem = "",
                            CreatorID = 0,
                            quantity = t.quantiy,
                            itemid = t.roomid,
                            StoreID = 0,
                            TypeOrder = 2, // dịch vụ bằng 2 giá phòng bằng 1
                            catalogitemid = 0,
                            OrderID = saleorderrom.Id
                        }).ToList();
                        // dịch vụ
                        var listservicedetail = product.Select(t => new tbl_SaleOrderDetail()
                        {
                            SysHotelID = t.SysHotelID,
                            DateCreated = DateTime.Now,
                            DatePayment = DateTime.Now,
                            PaymentTypeID = 1,
                            TotalAmount = t.TotalSale,
                            Price = t.Quantity,
                            AmountNoTax = 0,
                            Tax = 0,
                            item = "",
                            catalogitem = "",
                            CreatorID = 0,
                            quantity = t.Quantity,
                            itemid = t.productid,
                            StoreID = 0,
                            TypeOrder = 2, // dịch vụ bằng 2 giá phòng bằng 1
                            catalogitemid = t.productid,
                            OrderID = orderService.Id
                        }).ToList();

                        // giá phòng
                        db.InsertAll(listroomdetail);
                        // dịch vụ
                        db.InsertAll(listservicedetail);
                        #endregion
                        db.Update(room);
                        db.Update(usingroom);
                        db.Update(checkin);
                        tran.Commit();
                        rs.Status = "01";
                        rs.Message = "Thanh toán thành công cho khách hàng :" + itemcheckin.CustomerName;
                        return rs;
                    }
                    catch (Exception e)
                    {
                        rs.Status = "00";
                        rs.Message = "Thanh toán thất bại. Vui lòng thực hiện lại!";
                        return rs;
                    }

                }
            }

        }
        public bool SendMailHepelink(string files, string to, string subject, string body)
        {
            try
            {
                string fromMail = ConfigurationManager.AppSettings["MailServerUser"];
                string mailAddress = ConfigurationManager.AppSettings["MailServerAddress"];
                string password = ConfigurationManager.AppSettings["MailServerPassword"];
                //string fromMail = "hungpv@zopost.vn";
                //string mailAddress = "210.245.85.208";
                //string password = "anhlaanh";
                var mail = new Magnum.Mail.SmtpMailServer(mailAddress, fromMail, password);
                var m = new MailMessage();
                m.From = new MailAddress(fromMail);
                m.To.Add(new MailAddress(to));
                m.Subject = subject;
                m.Body = body;
                m.IsBodyHtml = true;
                m.Attachments.Add(new Attachment(files));
                mail.Send(m);
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw new Exception();
            }


        }

        public Vw_HotelsConfig GetConfig(int hotelID)
        {
            try
            {
                using (var db = _connectionData.OpenDbConnection())
                {
                    var cus = db.Select<Vw_HotelsConfig>(x => x.SysHotelID == hotelID).FirstOrDefault();
                    return cus;
                }
            }
            catch (Exception ex)
            {
                return new Vw_HotelsConfig();
                throw;
            }
        }

        public int UpdateOrInsert(tbl_HotelsConfig obj)
        {
            try
            {
                using (var db = _connectionData.OpenDbConnection())
                {
                    if (obj.Id > 0)
                    {
                        db.Update(obj);
                    }
                    else
                    {
                        db.Insert(obj);
                    }

                    return 1;
                }
            }
            catch (Exception ex)
            {
                return 0;
                throw;
            }
        }


    }
}