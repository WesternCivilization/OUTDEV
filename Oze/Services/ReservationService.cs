using oze.data;
using Oze.AppCode.Util;
using Oze.Models;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Oze.Services
{
    public class ReservationService
    {
       IOzeConnectionFactory _connectionData;

       public ReservationService()
        {
            OrmLiteConfig.DialectProvider = SqlServerDialect.Provider;
            OrmLiteConfig.DialectProvider.UseUnicode = true; // nhập tiếng việt

            _connectionData = new OzeConnectionFactory(ConfigurationManager.AppSettings["ConnectionString_OzeGeneral"], SqlServerOrmLiteDialectProvider.Instance);

        }
        public List<view_Customer_DatPhong_Detail> getAll(PagingBookingModel page) 
        {
            if (page == null) page = new PagingBookingModel() { offset = 0, limit = 100 };
            if (page.search == null) page.search = "";

              //  ServiceStackHelper.Help();
              //  LicenseUtils.ActivatedLicenseFeatures();           
            //search again
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<view_Customer_DatPhong_Detail>();
                if (!comm.IsSuperAdmin()) query = query.Where(e => e.SysHotelID == comm.GetHotelId());
                if (page.bydate==1) 
                {
                    DateTime dtFrom = CommService.ConvertStringToDate(page.dtFrom);
                    DateTime dtTo = CommService.ConvertStringToDate(page.dtTo);
                }
                if (page.roomid > 0) query = query.Where(e => e.RoomID == page.roomid);//nếu theo số phòng
                if (page.roomtypeid > 0) query = query.Where(e => e.RoomTypeID == page.roomtypeid);//nếu theo hạng phòng
                if (!string.IsNullOrEmpty(page.search)) query = query.Where(e => e.CustomerName.Contains(page.search));//nếu theo tên kh
                if (!string.IsNullOrEmpty(page.code)) query = query.Where(e => e.BookingCode.Contains(page.code));//nếu theo mã đặt phòng
                if (page.status!=0) query = query.Where(e => e.StatusReservation == (page.status));//nếu theo mã đặt phòng



                int offset = 0; try { offset = page.offset; }
                catch { }

                int limit = 10;//int.Parse(Request.Params["limit"]);
                try { limit = page.limit; }
                catch { }

                List<view_Customer_DatPhong_Detail> rows = db.Select(query)
                    .Skip(offset).Take(limit).ToList();
                return rows;                
            }
        }
        public long countAll(PagingBookingModel page)
        {
           if (page == null) page = new PagingBookingModel() { offset = 0, limit = 100 };
            if (page.search == null) page.search = "";

              //  ServiceStackHelper.Help();
              //  LicenseUtils.ActivatedLicenseFeatures();           
            //search again
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<view_Customer_DatPhong_Detail>();
                if (!comm.IsSuperAdmin()) query = query.Where(e => e.SysHotelID == comm.GetHotelId());
                if (!string.IsNullOrEmpty(page.dtFrom) && !string.IsNullOrEmpty(page.dtTo)) 
                {
                    DateTime dtFrom = CommService.ConvertStringToDate(page.dtFrom);
                    DateTime dtTo = CommService.ConvertStringToDate(page.dtTo);
                }
                if (page.roomid > 0) query = query.Where(e => e.RoomID == page.roomid);//nếu theo số phòng
                if (page.roomtypeid > 0) query = query.Where(e => e.RoomTypeID == page.roomtypeid);//nếu theo hạng phòng
                if (!string.IsNullOrEmpty(page.search)) query = query.Where(e => e.CustomerName.Contains(page.search));//nếu theo tên kh
                if (!string.IsNullOrEmpty(page.code)) query = query.Where(e => e.BookingCode.Contains(page.code));//nếu theo mã đặt phòng



                int offset = 0; try { offset = page.offset; }
                catch { }

                int limit = 10;//int.Parse(Request.Params["limit"]);
                try { limit = page.limit; }
                catch { }

                return  db.Count(query);
            }
        }

        public tbl_Unit GetUnitByID(string id)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Unit>().Where(e=>e.Id==int.Parse(id));
                return db.Select(query).SingleOrDefault();
            }     
        }
        public string GetNextBookingCode()
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Reservation_Room>().OrderByDescending(e=>e.Id);
                var obj = db.Select(query).FirstOrDefault();
                if(obj==null) return "FOTEL"+comm.GetHotelCode()+"000001";
                return "Oze" + comm.GetHotelCode() + CommService.fitTo6(obj.Id+1);
            }
        }

        public JsonRs DatTruoc(ReservationRoomModel obj)
        {
            
           //nếu như thời gian là thời gian hiện tại thì ko cho phép
           if (CommService.ConvertStringToDate(obj.ArrivalDate).CompareTo(CommService.ConvertStringToDate(obj.LeaveDate)) >= 0) 
           {
               return  JsonRs.create(-2,"Thời gian đến ko thể lớn hơn hoặc bằng thời gian thời gian rời dự kiến");
           }
           if (obj.RoomTypeID == 0) return JsonRs.create(-2, "Bạn bắt buộc phải chọn hạng phòng");
           if (obj.customer.DOB == null) obj.customer.DOB = "01/01/1900";
           //thiết lập vào check các thông số trước khi vào vấn đề chính
            tbl_RoomPriceLevel price = (new CommService()).GetRoomPriceLevelById(obj.RoomLevelPriceID);
            using (var db = _connectionData.OpenDbConnection())
            {
                if (obj.ID > 0)//nếu là update
                {

                    //nếu có phòng rồi và khác reservationid hiện tại
                    if (obj.RoomID > 0)
                    {
                        bool isOkBooking = (new CommService()).checkRoomIsAvailableForBooking(obj.ID, obj.RoomID, CommService.ConvertStringToDate(obj.ArrivalDate), CommService.ConvertStringToDate(obj.LeaveDate));
                        if (isOkBooking)
                            return JsonRs.create(-2, "Phòng này đang có khách đặt chỗ trong khoảng thời gian này từ " + obj.ArrivalDate + " " + obj.LeaveDate);
                    }

                    var query = db.From<tbl_Reservation_Room>().Where(e => e.Id == obj.ID);
                    var objUpdate = db.Select(query).SingleOrDefault();
                    if (objUpdate != null)
                    {
                        using(var trans=db.OpenTransaction())
                        {

                            //update reservation
                            //bjUpdate.Code = obj.Code;
                            objUpdate.Reservation_Type_ID = obj.ReservationType;
                            objUpdate.Room_Level_ID = obj.Room_Level_ID;
                            objUpdate.Room_Type_ID = obj.RoomTypeID;
                            objUpdate.Payment_Type_ID = obj.Payment_Type_ID;
                            objUpdate.Arrive_Date =CommService.ConvertStringToDate(obj.ArrivalDate);
                            objUpdate.Leave_Date=CommService.ConvertStringToDate(obj.LeaveDate);
                            objUpdate.Number_People = obj.PeopelNumber;
                            objUpdate.Number_Children = obj.Children;
                            objUpdate.Note = obj.Note;

                            objUpdate.ModifyBy =comm.GetUserId();
                            objUpdate.ModifyDate = DateTime.Now;

                            objUpdate.Deduction = obj.Deduction;
                            objUpdate.Deposit = obj.Deposit;
                            objUpdate.Holiday = obj.Holiday;
                            objUpdate.KhungGio = obj.KhungGio;

                            objUpdate.ReservationStatus =(obj.RoomID>0? ReservationStatus.CONFIRM: ReservationStatus.WAITING);
                            
                            objUpdate.CreateDate = DateTime.Now;
                            objUpdate.CreateBy = comm.GetUserId();
                            objUpdate.Note = obj.Note;
                            objUpdate.RoomID = obj.RoomID;
                            objUpdate.titlePrice = price!=null ? price.title:"";

                            db.Update(objUpdate);

                            //cập nhật bảng quan hệ đặt phòng và khách hàng
                            var queryCustomer = db.From<tbl_Customer>();
                            queryCustomer.Join<tbl_Customer, tbl_Reservation_Customer_Rel>((e1, e2) => e1.Id == e2.customerid && e1.ReservationID==obj.ID);
                            var objCustomer = db.Select(queryCustomer).FirstOrDefault();
                            objCustomer.CreateDate = DateTime.Now;
                            //update customer
                            objCustomer.Name = obj.customer.FullName;
                            objCustomer.DOB = CommService.ConvertStringToDate(obj.customer.DOB);
                            objCustomer.IdentifyNumber = (obj.customer.IdentityNumber);
                            objCustomer.Sex = (obj.customer.Sex);
                            objCustomer.Email = (obj.customer.Email);
                            objCustomer.Phone = (obj.customer.Phone);
                            objCustomer.Mobile = (obj.customer.Mobile);
                            objCustomer.Fax = (obj.customer.Fax);
                            objCustomer.TaxCode = (obj.customer.TaxCode);
                            objCustomer.CitizenshipCode = (obj.customer.CitizenshipCode);
                            objCustomer.Company = (obj.customer.Company);
                            objCustomer.CreateDate = obj.customer.CreateDate;
                            objCustomer.CreateBy = comm.GetUserId();
                            objCustomer.HotelCode = comm.GetHotelCode();
                            objCustomer.SysHotelID = comm.GetHotelId();
                            objCustomer.ReservationID = objUpdate.Id;
                            objCustomer.ModifyDate = DateTime.Now;
                            objCustomer.CountryId = obj.customer.CountryID;
                            objCustomer.CreateDate = DateTime.Now;
                            objCustomer.Payer = obj.customer.Payer;
                            objCustomer.Leader = obj.customer.Leader;
                            objCustomer.Address = obj.customer.Address;
                            objUpdate.Price = obj.Price;

                            objCustomer.Status = true;
                            db.Update(objCustomer);

                            //cập nhật bảng quan hệ
                            var queryCustomerReversationRel = db.From<tbl_Reservation_Customer_Rel>()
                                .Where(e => e.customerid == objCustomer.Id && e.reservationID == objUpdate.Id);

                            var objCustomerReversationRel = db.Select(queryCustomerReversationRel).SingleOrDefault();
                            objCustomerReversationRel.roomid=obj.RoomID;
                            objCustomerReversationRel.datePlanFrom = objUpdate.Arrive_Date;
                            objCustomerReversationRel.datePlanTo = objUpdate.Leave_Date;
                            objCustomerReversationRel.status = objUpdate.ReservationStatus;
                            db.Update(objCustomerReversationRel);

                            if (obj.RoomID > 0)
                            {
                                var queryRoom = db.From<tbl_Room>().Where(e => e.Id == obj.RoomID);
                                tbl_Room objRoom = db.Select(queryRoom).SingleOrDefault();
                                if (objRoom.status != RoomStatus.LIVE)//nếu ko có thằng nào ở. cũng ko có thằng nào booking thì book
                                {
                                    objRoom.status = RoomStatus.BOOKING;
                                    objRoom.datePlanFrom = objUpdate.Arrive_Date;
                                    objRoom.datePlanTo = objUpdate.Leave_Date;

                                    objRoom.status = RoomStatus.BOOKING;
                                    //objRoom. = objUpdate.RoomID;
                                    db.Update(objRoom);
                                }
                            }

                            //commit transaction
                            trans.Commit();
                            
                        }
                        return JsonRs.create(objUpdate.Id,"");
                    }                    
                }
                else 
                {
                    //nếu có phòng rồi và khác reservationid hiện tại
                    if (obj.RoomID > 0)
                    {
                        bool isOkBooking = (new CommService()).checkRoomIsAvailableForBooking(obj.ID, obj.RoomID, CommService.ConvertStringToDate(obj.ArrivalDate), CommService.ConvertStringToDate(obj.LeaveDate));
                        if (isOkBooking)
                            return JsonRs.create(-2, "Phòng này đang có khách đặt chỗ trong khoảng thời gian này từ " + obj.ArrivalDate + " " + obj.LeaveDate);
                    }

                    /*
                    var queryCount = db.From<tbl_Unit>().Where(e => e.Name1 == obj.Name1 && e.SysHotelID == comm.GetHotelId()).Select(e => e.Id);
                    var objCount = db.Count(queryCount);
                    if (objCount > 0) return comm.ERROR_EXIST;
                    */
                    int reservationid = -1;
                    using(var trans=db.OpenTransaction())
                    {

                       
                        var objUpdate = new tbl_Reservation_Room();
                        objUpdate.SysHotelID = comm.GetHotelId();
                        objUpdate.HotelCode = comm.GetHotelCode();

                        //bjUpdate.Code = obj.Code;
                        objUpdate.Reservation_Type_ID = obj.ReservationType;
                        objUpdate.Room_Level_ID = obj.Room_Level_ID;
                        objUpdate.Room_Type_ID = obj.RoomTypeID;
                        objUpdate.Payment_Type_ID = obj.Payment_Type_ID;
                        objUpdate.Arrive_Date = CommService.ConvertStringToDate(obj.ArrivalDate);
                        objUpdate.Leave_Date = CommService.ConvertStringToDate(obj.LeaveDate);
                        objUpdate.Number_People = obj.Adult;
                        objUpdate.Number_Children = obj.Children;
                        objUpdate.Note = obj.Note;

                        objUpdate.ModifyBy = comm.GetUserId();
                        objUpdate.ModifyDate = DateTime.Now;

                        objUpdate.Deduction = obj.Deduction;
                        objUpdate.Deposit = obj.Deposit;
                        objUpdate.Holiday = obj.Holiday;
                        objUpdate.KhungGio = obj.KhungGio;
                       

                        objUpdate.CreateDate = DateTime.Now;
                        objUpdate.CreateBy = comm.GetUserId();
                        objUpdate.Note = obj.Note;
                        objUpdate.RoomID = obj.RoomID;
                        objUpdate.BookingCode = obj.ReservationCode;
                        objUpdate.ReservationStatus=(obj.RoomID>0? ReservationStatus.CONFIRM : ReservationStatus.WAITING);
                        objUpdate.Price = obj.Price;
                        reservationid =(int) db.Insert(objUpdate,selectIdentity:true);

                        var objCustomer = new tbl_Customer();
                        objCustomer.HotelCode = comm.GetHotelCode();
                        objCustomer.CustomerTypeID = obj.customer.CustomerTypeID;
                        objCustomer.Name = obj.customer.FullName;
                        objCustomer.DOB = CommService.ConvertStringToDate(obj.customer.DOB);
                        objCustomer.IdentifyNumber = (obj.customer.IdentityNumber);
                        objCustomer.Sex = (obj.customer.Sex);
                        objCustomer.Email = (obj.customer.Email);
                        objCustomer.Phone = (obj.customer.Phone);
                        objCustomer.Mobile = (obj.customer.Mobile);
                        objCustomer.Fax = (obj.customer.Fax);
                        objCustomer.TaxCode = (obj.customer.TaxCode);
                        objCustomer.CitizenshipCode = (obj.customer.CitizenshipCode);
                        objCustomer.Company = (obj.customer.Company);
                        objCustomer.CreateDate = DateTime.Now;
                        objCustomer.CreateBy = comm.GetUserId();
                        objCustomer.HotelCode = comm.GetHotelCode();
                        objCustomer.SysHotelID = comm.GetHotelId();
                        objCustomer.ReservationID = (int)reservationid;
                        objCustomer.CountryId = obj.customer.CountryID;
                        objCustomer.ModifyDate = DateTime.Now;
                        objCustomer.Address = obj.customer.Address;
                        objCustomer.Payer = obj.customer.Payer;
                        objCustomer.Leader = obj.customer.Leader;

                        long customerid = db.Insert(objCustomer,selectIdentity:true);

                        //cập nhật bảng quan hệ
                        var objCustomerReserver = new tbl_Reservation_Customer_Rel();
                        objCustomerReserver.SysHotelID = comm.GetHotelId();
                        objCustomerReserver.SysHotelCode = comm.GetHotelCode();
                        objCustomerReserver.customerid =(int) customerid;
                        objCustomerReserver.status = objUpdate.ReservationStatus;
                        objCustomerReserver.roomid = objUpdate.RoomID;
                        objCustomerReserver.reservationID = (int)reservationid;
                        objCustomerReserver.datePlanFrom = objUpdate.Arrive_Date;
                        objCustomerReserver.datePlanTo = objUpdate.Leave_Date;
                        objCustomerReserver.status = objUpdate.ReservationStatus;

                        db.Insert(objCustomerReserver);


                        if (objUpdate.RoomID > 0)
                        {
                            var queryRoom = db.From<tbl_Room>().Where(e => e.Id == obj.RoomID);
                            tbl_Room objRoom = db.Select(queryRoom).SingleOrDefault();
                            if (objRoom.status != RoomStatus.LIVE)//nếu ko có thằng nào ở. cũng ko có thằng nào booking thì book
                            {
                                objRoom.status = RoomStatus.BOOKING;
                                objRoom.datePlanFrom = objUpdate.Arrive_Date;
                                objRoom.datePlanTo = objUpdate.Leave_Date;

                                objRoom.status = RoomStatus.BOOKING;
                                //objRoom. = objUpdate.RoomID;
                                db.Update(objRoom);
                            }
                        }
                        trans.Commit();                            
                    }
                    return JsonRs.create(reservationid,"");
                }
            }
            return JsonRs.create(-1, "Có lỗi trong quá trình xử lý");
        }
        public JsonRs CheckInByReservationID(int reservationidzzz)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                using (var trans = db.OpenTransaction())
                {
                    //tra cứu đặt chỗ
                    var queryReservation = db.From<tbl_Reservation_Room>().Where(e => e.Id == reservationidzzz);
                    var obj = db.Select(queryReservation).SingleOrDefault();

                    if (obj == null) return JsonRs.create(comm.ERROR_NOT_EXIST,"Không tồn tại mã đặt chỗ này");

                    //khởi tạo checkin
                    var objUpdateCheckIn = new tbl_CheckIn();
                    objUpdateCheckIn.SysHotelID = comm.GetHotelId();
                    objUpdateCheckIn.HotelCode = comm.GetHotelCode();

                    //bjUpdate.Code = obj.Code;
                    objUpdateCheckIn.Reservation_Type_ID = obj.Reservation_Type_ID;
                    objUpdateCheckIn.Room_Level_ID = obj.Room_Level_ID;
                    objUpdateCheckIn.Room_Type_ID = obj.Room_Type_ID;
                    objUpdateCheckIn.Payment_Type_ID = obj.Payment_Type_ID;
                    objUpdateCheckIn.Arrive_Date = DateTime.Now;// (obj.Arrive_Date);
                    objUpdateCheckIn.Leave_Date = (obj.Leave_Date);
                    objUpdateCheckIn.Number_People = obj.Number_People;
                    objUpdateCheckIn.Number_Children = obj.Number_Children;
                    objUpdateCheckIn.Note = obj.Note;

                    objUpdateCheckIn.ModifyBy = comm.GetUserId();
                    objUpdateCheckIn.ModifyDate = DateTime.Now;

                    objUpdateCheckIn.Deduction = obj.Deduction;
                    objUpdateCheckIn.Deposit = obj.Deposit;
                    objUpdateCheckIn.Holiday = obj.Holiday;
                    objUpdateCheckIn.KhungGio = obj.KhungGio;


                    objUpdateCheckIn.CreateDate = DateTime.Now;
                    objUpdateCheckIn.CreateBy = comm.GetUserId();
                    objUpdateCheckIn.Note = obj.Note;

                    objUpdateCheckIn.BookingCode = obj.BookingCode;
                    objUpdateCheckIn.ReservationStatus = CheckInStatus.OK;
                    long checkinid = db.Insert(objUpdateCheckIn, selectIdentity: true);

                    //tra cứu và cập nhật trạng thái từ bảng đặt phòng
                    var queryCustomer = db.From<tbl_Reservation_Customer_Rel>();
                    queryCustomer.Join<tbl_Customer, tbl_Reservation_Customer_Rel>((e1, e2) => e1.Id==e2.customerid && e2.reservationID==obj.Id);
                    var objCustomerReservation = db.Select(queryCustomer).SingleOrDefault();
                    objCustomerReservation.status = ReservationStatus.TOCHECKIN;
                    db.Update(objCustomerReservation);
                    
                    // nếu là có người ở trong khoảng thời gian này thì thôi
                    if ((new CommService()).checkRoomNotAvailable(objCustomerReservation.roomid.Value, DateTime.Now, objUpdateCheckIn.Leave_Date.Value,db))
                    {
                        return JsonRs.create(-1, "Bạn không thể nhận phòng vì đã có người ở phòng trong khoảng thời gian này");
                    }
                  

                    //cập nhật bảng quan hệ khách hàng sử dụng phòng nào
                    var objRoomUsing = new tbl_RoomUsing();
                    objRoomUsing.SysHotelID = comm.GetHotelId();
                    objRoomUsing.SysHotelCode = comm.GetHotelCode();

                    //
                    objRoomUsing.status = objUpdateCheckIn.ReservationStatus;
                    objRoomUsing.CheckInID = (int)checkinid;
                    objRoomUsing.customerid = objCustomerReservation.customerid;
                    objRoomUsing.roomid = objCustomerReservation.roomid;
                    db.Insert(objRoomUsing);

                    //cập nhật bảng phòng
                    var queryRoom = db.From<tbl_Room>()
                        .Where(e => e.Id == obj.RoomID);
                    tbl_Room objRoom = db.Select(queryRoom).SingleOrDefault();
                    objRoom.status = RoomStatus.LIVE;
                    objRoom.datePlanFrom = objUpdateCheckIn.Arrive_Date;
                    objRoom.datePlanTo = objUpdateCheckIn.Leave_Date;


                    db.Update(objRoom);

                    //cập nhật bảng đặt chỗ là đã checkin
                    obj.ReservationStatus = ReservationStatus.TOCHECKIN;
                    db.Update(obj);

                    //commit transaction
                    trans.Commit();
                    return JsonRs.create(1, "");
                }
            }
        }
        /// <summary>
        /// check in trực tiếp
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public JsonRs CheckIn(ReservationRoomModel obj)
        {
            if (CommService.ConvertStringToDate(obj.ArrivalDate).CompareTo(CommService.ConvertStringToDate(obj.LeaveDate)) >= 0)
            {
                return JsonRs.create(-2, "Thời gian đến ko thể lớn hơn hoặc bằng thời gian thời gian rời dự kiến");
            }

            if (CommService.ConvertStringToDate(obj.ArrivalDate).CompareTo(DateTime.Now) >= 0)
            {
                return JsonRs.create(-2, "Thời gian nhận phòng trong thời gian tương lai");
            }
            if (obj.RoomTypeID == 0) return JsonRs.create(-2, "Bạn bắt buộc phải chọn hạng phòng");
            if (obj.RoomID == 0) return JsonRs.create(-2, "Bạn bắt buộc phải chọn phòng khi nhận phòng");

            if (new CommService().checkRoomNotAvailable(obj.RoomID,CommService.ConvertStringToDate(obj.ArrivalDate),CommService.ConvertStringToDate(obj.LeaveDate)))
            {
                return JsonRs.create(-2, "Phòng này đã có người đặt lịch/ở trong khoảng thời gian này");
            }
            
            if (obj.customer.DOB == null) obj.customer.DOB = "01/01/1900";
            using (var db = _connectionData.OpenDbConnection())
            {
                if (obj.ID > 0)
                {
                    //nếu đã có là chỉnh sửa
                    var query = db.From<tbl_CheckIn>().Where(e => e.Id == obj.ID);
                    var objUpdateCheckIn = db.Select(query).SingleOrDefault();
                    if (objUpdateCheckIn != null)
                    {
                       
                        using (var trans = db.OpenTransaction())
                        {
                            //update reservation
                            //bjUpdate.Code = obj.Code;
                            objUpdateCheckIn.Reservation_Type_ID = obj.ReservationType;
                            objUpdateCheckIn.Room_Level_ID = obj.Room_Level_ID;
                            objUpdateCheckIn.Room_Type_ID = obj.RoomTypeID;
                            objUpdateCheckIn.Payment_Type_ID = obj.Payment_Type_ID;
                            objUpdateCheckIn.Arrive_Date = CommService.ConvertStringToDate(obj.ArrivalDate);
                            objUpdateCheckIn.Leave_Date = CommService.ConvertStringToDate(obj.LeaveDate);
                            objUpdateCheckIn.Number_People = obj.PeopelNumber;
                            objUpdateCheckIn.Number_Children = obj.Children;
                            objUpdateCheckIn.Note = obj.Note;

                            objUpdateCheckIn.ModifyBy = comm.GetUserId();
                            objUpdateCheckIn.ModifyDate = DateTime.Now;

                            objUpdateCheckIn.Deduction = obj.Deduction;
                            objUpdateCheckIn.Deposit = obj.Deposit;
                            objUpdateCheckIn.Holiday = obj.Holiday;
                            objUpdateCheckIn.KhungGio = obj.KhungGio;
                            objUpdateCheckIn.Price = obj.Price;

                            objUpdateCheckIn.ReservationStatus =CheckInStatus.OK ;// (obj.RoomID > 0 ? ReservationStatus.CONFIRM : ReservationStatus.WAITING);

                            objUpdateCheckIn.CreateDate = DateTime.Now;
                            objUpdateCheckIn.CreateBy = comm.GetUserId();
                            objUpdateCheckIn.Note = obj.Note;
                            //objUpdateCheckIn.titlePrice = price != null ? price.title : "";

                            db.Update(objUpdateCheckIn);

                            var queryCustomer = db.From<tbl_Customer>();
                            queryCustomer.Join<tbl_Customer, tbl_RoomUsing>((e1, e2) => e1.Id == e2.customerid && e2.CheckInID == objUpdateCheckIn.Id);
                            var objCustomer = db.Select(queryCustomer).SingleOrDefault();

                            //update customer
                            objCustomer.Name = obj.customer.FullName;
                            objCustomer.DOB = CommService.ConvertStringToDate(obj.customer.DOB);
                            objCustomer.IdentifyNumber = (obj.customer.IdentityNumber);
                            objCustomer.Sex = (obj.customer.Sex);
                            objCustomer.Email = (obj.customer.Email);
                            objCustomer.Phone = (obj.customer.Phone);
                            objCustomer.Mobile = (obj.customer.Mobile);
                            objCustomer.Fax = (obj.customer.Fax);
                            objCustomer.TaxCode = (obj.customer.TaxCode);
                            objCustomer.CitizenshipCode = (obj.customer.CitizenshipCode);
                            objCustomer.Company = (obj.customer.Company);
                            objCustomer.CreateDate = obj.customer.CreateDate;
                            objCustomer.CreateBy = comm.GetUserId();
                            objCustomer.HotelCode = comm.GetHotelCode();
                            objCustomer.SysHotelID = comm.GetHotelId();
                            objCustomer.ReservationID = objUpdateCheckIn.Id;
                            objCustomer.ModifyDate = DateTime.Now;
                            objCustomer.Status = true;
                            objCustomer.Address = obj.customer.Address;
                            objCustomer.Payer = obj.customer.Payer;
                            objCustomer.Leader = obj.customer.Leader;

                            db.Update(objCustomer);

                            //cập nhật bảng quan hệ giữa khách và phòng đã sử dụng
                            var queryCustomerReversationRel = db.From<tbl_RoomUsing>()
                                .Where(e => e.customerid == objCustomer.Id && e.CheckInID == objUpdateCheckIn.Id);

                            var objCustomerReversationRel = db.Select(queryCustomerReversationRel).SingleOrDefault();
                            objCustomerReversationRel.roomid = obj.RoomID;
                            db.Update(objCustomerReversationRel);
                            
                            //commit transaction
                            trans.Commit();

                        }
                    }
                    return JsonRs.create(-1,"");
                }
                //thêm mới check in
                else
                {
                    if (obj.RoomID == 0) return JsonRs.create(-1, "Bạn không thể nhận phòng mà ko điền phòng");
                    //nếu là phòng đang ở thì ko thể checkin
                    if (new RoomService().GetRoomByID(obj.RoomID.ToString()).status==RoomStatus.LIVE) 
                    {
                        return JsonRs.create(-1, "Phòng này đã có người ở");
                    }
                    using (var trans = db.OpenTransaction())
                    {
                        
                        var objUpdateCheckIn = new tbl_CheckIn();
                        objUpdateCheckIn.SysHotelID = comm.GetHotelId();
                        objUpdateCheckIn.HotelCode = comm.GetHotelCode();

                        //bjUpdate.Code = obj.Code;
                        objUpdateCheckIn.Reservation_Type_ID = obj.ReservationType;
                        objUpdateCheckIn.Room_Level_ID = obj.Room_Level_ID;
                        objUpdateCheckIn.Room_Type_ID = obj.RoomTypeID;
                        objUpdateCheckIn.Payment_Type_ID = obj.Payment_Type_ID;
                        objUpdateCheckIn.Arrive_Date = CommService.ConvertStringToDate(obj.ArrivalDate);
                        objUpdateCheckIn.Leave_Date = CommService.ConvertStringToDate(obj.LeaveDate);
                        objUpdateCheckIn.Number_People = obj.Adult;
                        objUpdateCheckIn.Number_Children = obj.Children;
                        objUpdateCheckIn.Note = obj.Note;

                        objUpdateCheckIn.ModifyBy = comm.GetUserId();
                        objUpdateCheckIn.ModifyDate = DateTime.Now;

                        objUpdateCheckIn.Deduction = obj.Deduction;
                        objUpdateCheckIn.Deposit = obj.Deposit;
                        objUpdateCheckIn.Holiday = obj.Holiday;
                        objUpdateCheckIn.KhungGio = obj.KhungGio;


                        objUpdateCheckIn.CreateDate = DateTime.Now;
                        objUpdateCheckIn.CreateBy = comm.GetUserId();
                        objUpdateCheckIn.Note = obj.Note;
                       
                        objUpdateCheckIn.BookingCode = obj.ReservationCode;
                        objUpdateCheckIn.ReservationStatus = CheckInStatus.OK;// (obj.RoomID > 0 ? ReservationStatus.CONFIRM : ReservationStatus.WAITING);
                       

                        long reservationid = db.Insert(objUpdateCheckIn, selectIdentity: true);

                        //thêm mới khách hàng
                        var objCustomer = new tbl_Customer();
                        objCustomer.HotelCode = comm.GetHotelCode();
                        objCustomer.CustomerTypeID = obj.customer.CustomerTypeID;
                        objCustomer.Name = obj.customer.FullName;
                        objCustomer.DOB = CommService.ConvertStringToDate(obj.customer.DOB);
                        objCustomer.IdentifyNumber = (obj.customer.IdentityNumber);
                        objCustomer.Sex = (obj.customer.Sex);
                        objCustomer.Email = (obj.customer.Email);
                        objCustomer.Phone = (obj.customer.Phone);
                        objCustomer.Mobile = (obj.customer.Mobile);
                        objCustomer.Fax = (obj.customer.Fax);
                        objCustomer.TaxCode = (obj.customer.TaxCode);
                        objCustomer.CitizenshipCode = (obj.customer.CitizenshipCode);
                        objCustomer.Company = (obj.customer.Company);
                        objCustomer.CreateDate = DateTime.Now;
                        objCustomer.CreateBy = comm.GetUserId();
                        objCustomer.HotelCode = comm.GetHotelCode();
                        objCustomer.SysHotelID = comm.GetHotelId();
                        objCustomer.ReservationID = (int)reservationid;
                        objCustomer.Address = obj.customer.Address;
                        objCustomer.Payer = obj.customer.Payer;
                        objCustomer.Leader = obj.customer.Leader;
                      
                        objCustomer.ModifyDate = DateTime.Now;

                        long customerid = db.Insert(objCustomer,selectIdentity:true);

                        //cập nhật bảng quan hệ khách hàng và phòng
                        var objCustomerReserver = new tbl_RoomUsing();
                        objCustomerReserver.SysHotelID = comm.GetHotelId();
                        objCustomerReserver.SysHotelCode = comm.GetHotelCode();

                        objCustomerReserver.status = objUpdateCheckIn.ReservationStatus;
                        objCustomerReserver.CheckInID = (int)reservationid;
                        objCustomerReserver.roomid = obj.RoomID;
                        objCustomerReserver.customerid = (int)customerid;
                       
                        db.Insert(objCustomerReserver);

                        //cập nhật status của phòng: bảng phòng
                        var queryRoom = db.From<tbl_Room>()
                            .Where(e => e.Id == obj.RoomID);
                        tbl_Room objRoom = db.Select(queryRoom).SingleOrDefault();
                        objRoom.status = RoomStatus.LIVE;
                        objRoom.datePlanFrom = objUpdateCheckIn.Arrive_Date;
                        objRoom.datePlanTo = objUpdateCheckIn.Leave_Date;

                        db.Update(objRoom);

                        //commit transaction
                        trans.Commit();
                        return JsonRs.create(1,"");
                    }
                }
            }
            return JsonRs.create(-1, "");
        }
        public int AddToRoomMate(CustomerModel obj,int checkinid)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
               
                //tìm checkin id
                var queryCheckIn = db.From<tbl_CheckIn>().Where(e => e.Id == checkinid);
                var objUpdateCheckIn = db.Select(queryCheckIn).SingleOrDefault();
                if (objUpdateCheckIn == null) return comm.ERROR_NOT_EXIST;//nếu ko có thì báo lỗi
                if (obj.ID > 0)
                {
                    //tìm customer
                    var query = db.From<tbl_Customer>().Where(e => e.Id == obj.ID);
                    var objUpdateCustomer= db.Select(query).SingleOrDefault();
                    //nếu có rồi thì add vào tblRoom
                    if (objUpdateCustomer != null)
                    {
                        using (var trans = db.OpenTransaction())
                        {
                            //cập nhật bảng quan hệ
                            var queryCustomerReversationRel = db.From<tbl_RoomUsing>()
                                .Where(e => e.customerid == objUpdateCustomer.Id && e.CheckInID == objUpdateCheckIn.Id);

                            var objCustomerReversationRel = db.Select(queryCustomerReversationRel).SingleOrDefault();
                            objCustomerReversationRel.roomid = obj.RoomID;
                            db.Update(objCustomerReversationRel);


                            //commit transaction
                            trans.Commit();

                        }
                    }
                    return -1;
                }
                else
                {

                    //nếu chưa có thì chèn vào rồi mới add
                    using (var trans = db.OpenTransaction())
                    {
                        //thêm mới 1 customer
                        var objCustomer = new tbl_Customer();
                        objCustomer.HotelCode = comm.GetHotelCode();
                        objCustomer.CustomerTypeID = obj.CustomerTypeID;
                        objCustomer.Name = obj.FullName;
                        objCustomer.DOB = CommService.ConvertStringToDate(obj.DOB);
                        objCustomer.IdentifyNumber = (obj.IdentityNumber);
                        objCustomer.Sex = (obj.Sex);
                        objCustomer.Email = (obj.Email);
                        objCustomer.Phone = (obj.Phone);
                        objCustomer.Mobile = (obj.Mobile);
                        objCustomer.Fax = (obj.Fax);
                        objCustomer.TaxCode = (obj.TaxCode);
                        objCustomer.CitizenshipCode = (obj.CitizenshipCode);
                        objCustomer.Company = (obj.Company);
                        objCustomer.CreateDate = DateTime.Now;
                        objCustomer.CreateBy = comm.GetUserId();
                        objCustomer.HotelCode = comm.GetHotelCode();
                        objCustomer.SysHotelID = comm.GetHotelId();
                        objCustomer.ReservationID = (int)checkinid;
                        objCustomer.ModifyDate = DateTime.Now;

                        long customerid = db.Insert(objCustomer);

                        //cập nhật bảng quan hệ
                        var objCustomerReserver = new tbl_RoomUsing();
                        objCustomerReserver.SysHotelID = comm.GetHotelId();
                        objCustomerReserver.SysHotelCode = comm.GetHotelCode();

                        objCustomerReserver.status = objUpdateCheckIn.ReservationStatus;
                        objCustomerReserver.CheckInID = checkinid;
                        db.Insert(objCustomerReserver);

                        //cập nhật bảng phòng
                        var queryRoom = db.From<tbl_Room>()
                            .Where(e => e.Id == obj.RoomID);
                        tbl_Room objRoom = db.Select(queryRoom).SingleOrDefault();
                        objRoom.status = RoomStatus.LIVE;
                        db.Update(objRoom);

                        //commit transaction
                        trans.Commit();
                        return 1;
                    }
                }
            }
        }
        public JsonRs AssignRoom(int reservationid, int roomid)
        {
            //đầu tiên check xem phòng có okie ko
          
            using (var db = _connectionData.OpenDbConnection())
            {
                //tìm reservation id
                var queryCheckIn = db.From<tbl_Reservation_Room>().Where(e => e.Id == reservationid);
                var objUpdateReservation = db.Select(queryCheckIn).SingleOrDefault();
                 
                if (objUpdateReservation == null) return JsonRs.create(comm.ERROR_NOT_EXIST,"Không tồn tại mã booking này");//nếu ko có thì báo lỗi

                if (new CommService().checkRoomNotAvailable(roomid, objUpdateReservation.Arrive_Date.Value, objUpdateReservation.Leave_Date.Value,db))
                    return JsonRs.create(-1, "Phòng đã có người ở trong khoảng thời gian này");

                using (var trans = db.OpenTransaction())
                {
                    //cập nhật phòng và trạng thái của booking
                    objUpdateReservation.RoomID = roomid;
                    objUpdateReservation.ReservationStatus = ReservationStatus.CONFIRM;
                    db.Update(objUpdateReservation);

                    //cập nhật bảng quan hệ đặt phòng và khách hàng
                    var queryCustomer = db.From<tbl_Reservation_Customer_Rel>();
                    queryCustomer.Join<tbl_Customer, tbl_Reservation_Customer_Rel>((e1, e2) => e1.Id == e2.customerid && e2.reservationID == objUpdateReservation.Id);
                    var objCustomer = db.Select(queryCustomer).SingleOrDefault();
                    objCustomer.roomid = roomid;
                    db.Update(objCustomer);

                    //cập nhật bảng phòng
                    var queryRoom = db.From<tbl_Room>()
                        .Where(e => e.Id == roomid);
                    tbl_Room objRoom = db.Select(queryRoom).SingleOrDefault();
                    objRoom.status = RoomStatus.LIVE;
                    objRoom.datePlanTo = objUpdateReservation.Leave_Date;
                    objRoom.datePlanFrom = objUpdateReservation.Arrive_Date;
                    objRoom.status = RoomStatus.BOOKING;
                    db.Update(objRoom);

                    trans.Commit();
                    return JsonRs.create(1,"");
                }
                return JsonRs.create(-1, "Phòng đã có người ở trong khoảng thời gian này");       
            }
        }
        public int CancelReservation(int id,string note)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                
                var query = db.From<tbl_Reservation_Room>().Where(e => e.Id == id);
                var objUpdate = db.Select(query).SingleOrDefault();
                if (objUpdate != null)
                {
                    using (var trans = db.OpenTransaction())
                    {
                        //cập nhật bảng chính
                        objUpdate.ReservationStatus = ReservationStatus.CANCEL;
                        objUpdate.Reason = note;
                        //commit transaction
                        db.Update(objUpdate);

                        //cập nhật bảng quan hệ đặt phòng và khách hàng
                        var queryCustomer = db.From<tbl_Reservation_Customer_Rel>();
                        queryCustomer.Join<tbl_Customer, tbl_Reservation_Customer_Rel>((e1, e2) => e1.Id == e2.customerid && e2.reservationID == id);
                        var objCustomer = db.Select(queryCustomer).SingleOrDefault();
                        objCustomer.status = ReservationStatus.CANCEL;
                        db.Update(objCustomer);

                        trans.Commit();
                        return 1;
                    }
                }              
            }
            return -1;
        }
        public int CancelCheckIn(int id, string note)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
               
                    var query = db.From<tbl_CheckIn>().Where(e => e.Id == id);
                    var objUpdateCheckin = db.Select(query).SingleOrDefault();
                    if (objUpdateCheckin == null) return comm.ERROR_NOT_EXIST;
                    if (objUpdateCheckin != null)
                    {
                        using (var trans = db.OpenTransaction())
                        {
                            //cập nhật bảng check-in
                            objUpdateCheckin.ReservationStatus = ReservationStatus.CANCEL;
                            objUpdateCheckin.Reason = note;
                            db.Update(objUpdateCheckin);

                            //cập nhật bảng quan hệ
                            var queryCustomerReversationRel = db.From<tbl_RoomUsing>()
                                .Where(e => e.CheckInID == objUpdateCheckin.Id);
                            var objCustomerReversationRel = db.Select(queryCustomerReversationRel).FirstOrDefault();                            
                            objCustomerReversationRel.status = ReservationStatus.CANCEL;
                            db.Update(objCustomerReversationRel);

                            //trong truường hợp nhiều nguoi cùng ở
                            var lstCustomerReversationRel = db.Select(queryCustomerReversationRel).ToList();
                            if (lstCustomerReversationRel.Count > 1) 
                            {
                                foreach(var objCustomer1 in lstCustomerReversationRel)
                                {
                                    objCustomer1.status = ReservationStatus.CANCEL;
                                    db.Update(objCustomer1);
                                }
                            }

                            //cập nhật bảng room
                            var queryRoom = db.From<tbl_Room>()
                                .Where(e => e.Id == objCustomerReversationRel.roomid);
                            tbl_Room objRoom = db.Select(queryRoom).SingleOrDefault();
                            objRoom.status = RoomStatus.EMPTY;
                            db.Update(objRoom);

                            //commit transaction
                             db.Update(objUpdateCheckin);
                             trans.Commit();
                             return 1;
                        }
                   
                }
            }
            return -1;
        }
        public tbl_Unit InitEmpty()
        {
            var obj = new tbl_Unit();
            obj.Id = 0;
            obj.Name1 = "";
            obj.Name2 = "";
            obj.Description = "";
            obj.Status = 1;
            obj.Value = "1";
            return obj;
        }
        public  view_Customer_DatPhong_Detail GetDatTruocDetail(int id)
        {
            if (id == 0) return null;
            using (var db = _connectionData.OpenDbConnection())
            {

                var query = db.From<view_Customer_DatPhong_Detail>().Where(e => e.ID == id);
                var objUpdateCheckin = db.Select(query).SingleOrDefault();
                return objUpdateCheckin;
            }                   
        }
        public tbl_Customer GetRoomMateDetail(int customerid)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Customer>().Where(e => e.Id == customerid);
                var objUpdateCustomer = db.Select(query).SingleOrDefault();
                return objUpdateCustomer;
            }
        }

        public List<tbl_Room> getRoomAvailable(string sdateFrom,string sdateTo,int? typeRoomId=0)
        {
            DateTime dtFrom=CommService.ConvertStringToDate(sdateFrom);
            DateTime dtTo=CommService.ConvertStringToDate(sdateTo);

            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<vw_RoomAvailable>().Where(e => 
                    ((e.datePlanFrom <= dtFrom && e.datePlanTo >= dtFrom)
                    ||
                    (e.datePlanFrom <= dtTo &&  e.datePlanTo >= dtTo)));

                query.Where(e => e.SysHotelID == comm.GetHotelId());
                query=query.SelectDistinct(e=>e.roomid);

                var query1 = db.From<tbl_Room>().Where(e => !Sql.In(e.Id, query));
                query1.Where(e => e.SysHotelID == comm.GetHotelId());
                if (typeRoomId.HasValue && typeRoomId > 0) query1.Where(e => e.RoomType_ID == typeRoomId);
                var objUpdateCustomer = db.Select(query1).ToList();
                return objUpdateCustomer;
            }
        }
    }
}