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
    public class EstimatePriceService
    {
       IOzeConnectionFactory _connectionData;

       public EstimatePriceService()
        {
            OrmLiteConfig.DialectProvider = SqlServerDialect.Provider;
            OrmLiteConfig.DialectProvider.UseUnicode = true; // nhập tiếng việt

            _connectionData = new OzeConnectionFactory(ConfigurationManager.AppSettings["ConnectionString_OzeGeneral"], SqlServerOrmLiteDialectProvider.Instance);

        }
       public List<view_Customer_DatPhong_Detail> getPrice(PagingBookingModel page,int khunggio)
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
              
               DateTime dtFrom = CommService.ConvertStringToDate(page.dtFrom);
               DateTime dtTo = CommService.ConvertStringToDate(page.dtTo);
               
               if (page.roomid > 0) query = query.Where(e => e.RoomID == page.roomid);//nếu theo số phòng
               if (page.roomtypeid > 0) query = query.Where(e => e.RoomTypeID == page.roomtypeid);//nếu theo hạng phòng


               List<RoomPriceEstimateModel> lstPrice = caculatePrice(comm.GetHotelId(), khunggio, page.roomtypeid, dtFrom, dtTo);
               
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
        /// <summary>
        /// Hàm tính giá, trả về list giá được tính
        /// </summary>
        /// <param name="typePrice">theo loại gì</param>
        /// <param name="roomtypeid">hạng phòng gì</param>
        /// <param name="dtFrom">từ ngày nào</param>
       /// <param name="dtTo">đến ngày nào</param>
        /// <returns></returns>
       public List<RoomPriceEstimateModel> caculatePrice(int hotelid, int typePrice,int roomid, int roomtypeid, DateTime dtFrom, DateTime dtTo)
        {
            //giờ checkin
            int hourStartCheckIn = 10;
            //giờ checkout
            int hourStartCheckOut = 12;

            
           
            using (var db = _connectionData.OpenDbConnection())
            {
                //lấy chính sách giá đang áp dụng cho ngày checkin cho hạng phòng này
               var queryPrice= db.From<tbl_RoomPriceLevel>()
                    .Where(e => e.RoomTypeID == roomtypeid && e.dateFrom <= dtFrom && e.dateTo <= dtFrom)
                    .OrderByDescending(e => e.Id);

               tbl_RoomPriceLevel oPrice = db.Select<tbl_RoomPriceLevel>(queryPrice).FirstOrDefault();           
               if (oPrice != null) 
               {
                    //nếu là theo h thì chia đều h ra
                   //hoặc là trong 1 ngày và trước giờ checkin và sau giờ check out hoặc trong khoảng checkin-checkout
                   if ((typePrice == RoomPriceType.HOUR)//bắt tính theo h ( khách sạn bắt tính
                       ||(dtTo.Day==dtFrom.Day && dtTo.Hour<hourStartCheckIn)//cùng ngày và trước giờ check in
                       || (dtTo.Day == dtFrom.Day && dtFrom.Hour >= hourStartCheckOut)//cùng ngày và sau giờ check out
                    )
                   {
                       //tính thời gian lệch
                       TimeSpan tm = dtTo.Subtract(dtTo);
                       int totalHours = tm.Hours + ((tm.Minutes > 0) ? 1 : 0);

                       //truy vấn trong db theo h
                       var queryHours = db.From<tbl_RoomPriceLevel_Hour>()
                           .Where(e => e.RoomPriceLevelID == oPrice.Id && e.numberHours==totalHours);
                       tbl_RoomPriceLevel_Hour oPriceHours = db.Select(queryHours).FirstOrDefault();

                       int hourPrice = 0; //ko có thì trả về ko
                       //trả về kết quả nếu có
                       if (oPriceHours != null) hourPrice = oPriceHours.numberHours.Value;
                       
                       return new List<RoomPriceEstimateModel>(){ RoomPriceEstimateModel.createNew()
                            .setRoomId(roomid)
                            .setRoomTypeId(roomtypeid)
                            .setTypePrice(RoomPriceType.HOUR)
                            .setPrice(oPriceHours.numberHours.Value)
                            .setDtFrom(dtFrom)
                            .setQuantity(1)
                            .setPricePolicyID(oPriceHours.RoomPriceLevelID.Value)
                            .setPricePolicyName(oPrice.title)
                            .setDtTo(dtTo)};
                      
                   }
                   // 1:Phụ trội quá giờ trả theo ngày
                   // 2:  Phụ trội quá giờ trả theo đêm 
                   // 3: Phụ trội nhận phòng sớm theo ngày
                   // 4:  Phụ trội nhận phòng sớm theo đêm
                   // 5 :Phụ trội quá số lượng người lớn
                   //nếu là trươc checkin và sau checkout trong cùng 1 ngày
                   if (dtTo.Hour > hourStartCheckOut && dtFrom.Hour < hourStartCheckIn && dtTo.Day == dtFrom.Day)
                   {
                       //1.tính phụ trội sớm
                       tbl_RoomPriceLevel_Extra oPriceEarly;
                       int numberHourEarly = (hourStartCheckIn - dtFrom.Hour) + 1;//phụ trội sớm
                       //truy vấn trong db theo h phụ trội
                       var queryEarly = db.From<tbl_RoomPriceLevel_Extra>()
                           .Where(e => e.RoomPriceLevelID == oPrice.Id && e.numberExtra == numberHourEarly);

                       //nếu xác định tính rồi thì tính theo đó
                       if (typePrice == RoomPriceType.DAY) queryEarly.Where(e => e.typeExtra == TypeExtra.EARLY_CHECKIN_DAY);
                       if (typePrice == RoomPriceType.NIGHT) queryEarly.Where(e => e.typeExtra == TypeExtra.EARLY_CHECKIN_NIGHT);
                       else //ko thì thằng nào rẻ hơn lấy thằng đó
                       {
                           var queryEarly1 = queryEarly.Where(e => e.typeExtra == TypeExtra.EARLY_CHECKIN_DAY);
                           var queryEarly2 = queryEarly.Where(e => e.typeExtra == TypeExtra.EARLY_CHECKIN_NIGHT);

                           var oPriceEarly1 = db.Select<tbl_RoomPriceLevel_Extra>(queryEarly1).FirstOrDefault();
                           var oPriceEarly2 = db.Select<tbl_RoomPriceLevel_Extra>(queryEarly2).FirstOrDefault();
                           if (oPriceEarly1.priceExtra.Value >= oPriceEarly2.priceExtra.Value) oPriceEarly = oPriceEarly2;
                           else oPriceEarly = oPriceEarly1;

                       }

                       var oPrice1=   RoomPriceEstimateModel.createNew()
                            .setRoomId(roomid)
                            .setRoomTypeId(roomtypeid)
                            .setTypePrice(RoomPriceType.EXTRA_EARLY_NIGHT)
                            .setPrice(oPriceEarly.priceExtra.Value)
                            .setDtFrom(dtFrom)
                            .setQuantity(1)
                            .setPricePolicyID(oPrice.Id)
                            .setPricePolicyName(oPrice.title)
                            .setDtTo(dtTo);

                       //2.tính phụ trội muộn
                       tbl_RoomPriceLevel_Extra oPriceLate;
                       int numberHourLate = (hourStartCheckOut - dtTo.Hour) + 1;//phụ trội muộn
                       //truy vấn trong db theo h phụ trội
                       var queryLate = db.From<tbl_RoomPriceLevel_Extra>()
                           .Where(e => e.RoomPriceLevelID == oPrice.Id && e.numberExtra == numberHourLate);

                       //nếu xác định tính theo ngày hay theo đêm thì tính theo cái đó  thì tính theo đó
                       if (typePrice == RoomPriceType.DAY) queryLate.Where(e => e.typeExtra == TypeExtra.EARLY_CHECKIN_DAY);
                       if (typePrice == RoomPriceType.NIGHT) queryLate.Where(e => e.typeExtra == TypeExtra.EARLY_CHECKIN_NIGHT);
                       else //ko thì thằng nào rẻ hơn lấy thằng đó
                       {
                           var queryLate1 = queryLate.Where(e => e.typeExtra == TypeExtra.LATE_CHECKOUT_DAY);
                           var queryLate2 = queryLate.Where(e => e.typeExtra == TypeExtra.LATE_CHECKOUT_NIGHT);

                           var oPriceLate1 = db.Select<tbl_RoomPriceLevel_Extra>(queryLate1).FirstOrDefault();
                           var oPriceLate2 = db.Select<tbl_RoomPriceLevel_Extra>(queryLate2).FirstOrDefault();
                           if (oPriceLate1.priceExtra.Value >= oPriceLate1.priceExtra.Value) oPriceLate = oPriceLate2;
                           else oPriceLate = oPriceLate1;
                       }
                       var oPrice2=   RoomPriceEstimateModel.createNew()
                            .setRoomId(roomid)
                            .setRoomTypeId(roomtypeid)
                            .setTypePrice(RoomPriceType.EXTRA_LATE_DAY)
                            .setPrice(oPriceLate.priceExtra.Value)
                            .setDtFrom(dtFrom)
                            .setQuantity(1)
                            .setPricePolicyID(oPrice.RoomPriceLevelID.Value)
                            .setPricePolicyName(oPrice.title)
                            .setDtTo(dtTo)};

                       //3.tính h giữa checkout và check in
                       int totalHours=hourStartCheckOut - hourStartCheckIn;
                       //tính thời gian lệch
                       //truy vấn trong db theo h
                       var queryHours = db.From<tbl_RoomPriceLevel_Hour>()
                           .Where(e => e.RoomPriceLevelID == oPrice.Id && e.numberHours == totalHours);
                       tbl_RoomPriceLevel_Hour oPriceHours = db.Select(queryHours).FirstOrDefault();

                       int hourPrice = 0; //ko có thì trả về ko
                       //trả về kết quả nếu có
                       if (oPriceHours != null) hourPrice = oPriceHours.numberHours.Value;

                       //trả về kết quả
                        var oPrice3=   RoomPriceEstimateModel.createNew()
                        .setRoomId(roomid)
                        .setRoomTypeId(roomtypeid)
                        .setTypePrice(RoomPriceType.HOUR)
                        .setPrice(oPriceHours.numberHours.Value)
                        .setDtFrom(dtFrom)
                        .setQuantity(1)
                        .setPricePolicyID(oPriceHours.RoomPriceLevelID.Value)
                        .setPricePolicyName(oPrice.title)
                        .setDtTo(dtTo);

                   }
               }
               //thực hiện trả về mặc định
               return new List<RoomPriceEstimateModel>(){ RoomPriceEstimateModel.createNew()
                            .setRoomId(roomid)
                            .setRoomTypeId(roomtypeid)
                            .setTypePrice(RoomPriceType.HOUR)
                            .setPrice(0)
                            .setDtFrom(dtFrom)
                            .setQuantity(1)
                            .setPricePolicyID(0)
                            .setPricePolicyName("Chưa có chính sách giá")
                            .setDtTo(dtTo)};
            }

            //nếu là theo ngày

            return new List<RoomPriceEstimateModel>();
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
                if(obj==null) return "Oze"+comm.GetHotelCode()+"000001";
                return "Oze" + comm.GetHotelCode() + CommService.fitTo6(obj.Id+1);
            }
        }

        public int DatTruoc(ReservationRoomModel obj)
        {
            //thiết lập vào check các thông số trước khi vào vấn đề chính
           tbl_RoomPriceLevel price=   (new CommService()).GetRoomPriceLevelById(obj.RoomLevelPriceID);

            using (var db = _connectionData.OpenDbConnection())
            {
                if (obj.ID > 0)
                {

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
                            queryCustomer.Join<tbl_Customer, tbl_Reservation_Customer_Rel>((e1, e2) => e1.Id == e2.customerid);
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
                            objCustomer.ReservationID = objUpdate.Id;
                            objCustomer.ModifyDate = DateTime.Now;
                            objCustomer.CountryId = obj.customer.CountryID;

                            db.Update(objCustomer);

                            //cập nhật bảng quan hệ
                            var queryCustomerReversationRel = db.From<tbl_Reservation_Customer_Rel>()
                                .Where(e => e.customerid == objCustomer.Id && e.reservationID == objUpdate.Id);

                            var objCustomerReversationRel = db.Select(queryCustomerReversationRel).SingleOrDefault();
                            objCustomerReversationRel.roomid=obj.RoomID;
                            db.Update(objCustomerReversationRel);

                            if (obj.RoomID > 0)
                            {
                                var queryRoom = db.From<tbl_Room>().Where(e => e.Id == obj.RoomID);
                                tbl_Room objRoom = db.Select(queryRoom).SingleOrDefault();
                                objRoom.status = RoomStatus.LIVE;
                                objRoom.datePlanTo = objUpdate.Arrive_Date;
                                objRoom.datePlanFrom = objUpdate.Leave_Date;
                                objRoom.status = RoomStatus.BOOKING;
                                db.Update(objRoom);
                            }

                            //commit transaction
                            trans.Commit();
                            
                        }
                        return objUpdate.Id;
                    }
                    return -1;
                }
                else 
                {
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

                        long customerid = db.Insert(objCustomer,selectIdentity:true);

                        //cập nhật bảng quan hệ
                        var objCustomerReserver = new tbl_Reservation_Customer_Rel();
                        objCustomerReserver.SysHotelID = comm.GetHotelId();
                        objCustomerReserver.SysHotelCode = comm.GetHotelCode();
                        objCustomerReserver.customerid =(int) customerid;
                        objCustomerReserver.status = objUpdate.ReservationStatus;
                        objCustomerReserver.reservationID = (int)reservationid;
                        db.Insert(objCustomerReserver);

                        if (objUpdate.RoomID > 0)
                        {
                            var queryRoom = db.From<tbl_Room>().Where(e => e.Id == obj.RoomID);
                            tbl_Room objRoom = db.Select(queryRoom).SingleOrDefault();
                            objRoom.status = RoomStatus.LIVE;
                            objRoom.datePlanTo = objUpdate.Arrive_Date;
                            objRoom.datePlanFrom = objUpdate.Leave_Date;
                            objRoom.status = RoomStatus.BOOKING;
                            db.Update(objRoom);
                        }
                        trans.Commit();                            
                    }
                    return reservationid;
                }
            }     
        }
        public int CheckInByReservationID(int reservationidzzz)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                using (var trans = db.OpenTransaction())
                {
                    //tra cứu đặt chỗ
                    var queryReservation = db.From<tbl_Reservation_Room>().Where(e => e.Id == reservationidzzz);
                    var obj = db.Select(queryReservation).SingleOrDefault();

                    if (obj == null) return comm.ERROR_NOT_EXIST;

                    //khởi tạo checkin
                    var objUpdateCheckIn = new tbl_CheckIn();
                    objUpdateCheckIn.SysHotelID = comm.GetHotelId();
                    objUpdateCheckIn.HotelCode = comm.GetHotelCode();

                    //bjUpdate.Code = obj.Code;
                    objUpdateCheckIn.Reservation_Type_ID = obj.Reservation_Type_ID;
                    objUpdateCheckIn.Room_Level_ID = obj.Room_Level_ID;
                    objUpdateCheckIn.Room_Type_ID = obj.Room_Type_ID;
                    objUpdateCheckIn.Payment_Type_ID = obj.Payment_Type_ID;
                    objUpdateCheckIn.Arrive_Date = (obj.Arrive_Date);
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

                    //tra cứu khách hàng từ bảng đặt phòng
                    var queryCustomer = db.From<tbl_Reservation_Customer_Rel>();
                    queryCustomer.Join<tbl_Customer, tbl_Reservation_Customer_Rel>((e1, e2) => e1.Id==e2.customerid && e2.reservationID==obj.Id);
                    var objCustomerReservation = db.Select(queryCustomer).SingleOrDefault();

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
                    objRoom.datePlanTo = objUpdateCheckIn.Arrive_Date;
                    objRoom.datePlanFrom = objUpdateCheckIn.Leave_Date;

                    db.Update(objRoom);

                    //cập nhật bảng đặt chỗ là đã checkin
                    obj.ReservationStatus = ReservationStatus.TOCHECKIN;
                    db.Update(obj);

                    //commit transaction
                    trans.Commit();
                    return 1;
                }
            }
        }
        /// <summary>
        /// check in trực tiếp
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CheckIn(ReservationRoomModel obj)
        {
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

                            objUpdateCheckIn.ReservationStatus = (obj.RoomID > 0 ? ReservationStatus.CONFIRM : ReservationStatus.WAITING);

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
                    return -1;
                }
                //thêm mới check in
                else
                {
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
                        objUpdateCheckIn.ReservationStatus = (obj.RoomID > 0 ? ReservationStatus.CONFIRM : ReservationStatus.WAITING);
                       

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
                      
                        objCustomer.ModifyDate = DateTime.Now;

                        long customerid = db.Insert(objCustomer);

                        //cập nhật bảng quan hệ khách hàng và phòng
                        var objCustomerReserver = new tbl_RoomUsing();
                        objCustomerReserver.SysHotelID = comm.GetHotelId();
                        objCustomerReserver.SysHotelCode = comm.GetHotelCode();

                        objCustomerReserver.status = objUpdateCheckIn.ReservationStatus;
                        objCustomerReserver.CheckInID = (int)reservationid;
                        db.Insert(objCustomerReserver);

                        //cập nhật status của phòng: bảng phòng
                        var queryRoom = db.From<tbl_Room>()
                            .Where(e => e.Id == obj.RoomID);
                        tbl_Room objRoom = db.Select(queryRoom).SingleOrDefault();
                        objRoom.status = RoomStatus.LIVE;
                        db.Update(objRoom);

                        //commit transaction
                        trans.Commit();
                        return 1;
                    }
                    return -1;
                }
            }
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
                    return -1;
                }
            }
        }
        public int AssignRoom(int reservationid, int roomid)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                //tìm reservation id
                var queryCheckIn = db.From<tbl_Reservation_Room>().Where(e => e.Id == reservationid);
                var objUpdateReservation = db.Select(queryCheckIn).SingleOrDefault();
                if (objUpdateReservation == null) return comm.ERROR_NOT_EXIST;//nếu ko có thì báo lỗi
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
                    objRoom.datePlanTo = objUpdateReservation.Arrive_Date;
                    objRoom.datePlanFrom = objUpdateReservation.Leave_Date;
                    objRoom.status = RoomStatus.BOOKING;
                    db.Update(objRoom);

                    trans.Commit();
                    return 1;
                }
                return -1;                
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
                    objUpdate.ReservationStatus = ReservationStatus.CANCEL;
                    objUpdate.Reason = note;
                    //commit transaction
                    return db.Update(objUpdate);
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
                            var objCustomerReversationRel = db.Select(queryCustomerReversationRel).SingleOrDefault();
                            objCustomerReversationRel.status = ReservationStatus.CANCEL;
                            db.Update(objCustomerReversationRel);


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
    }
}