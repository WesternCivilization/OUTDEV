using oze.data;
using Oze.AppCode.Util;
using Oze.Models;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace Oze.Services
{
    public class EstimatePrice1Service
    {
       IOzeConnectionFactory _connectionData;

       public EstimatePrice1Service()
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


               List<RoomPriceEstimateModel> lstPrice = caculatePrice(comm.GetHotelId(), khunggio,page.roomid, page.roomtypeid, dtFrom, dtTo,-1);
               
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
        public tbl_RoomPriceLevel getActivePrice(int roomTypeId, DateTime dtTime,IDbConnection db)
        {
            int hotelId = comm.GetHotelId();
            /*
            //lấy chính sách giá đang áp dụng cho ngày checkin cho hạng phòng này
            var queryPrice = db.From<tbl_RoomPriceLevel>()
                 .Where(e => e.RoomTypeID == roomTypeId && e.dateFrom <= dtTime && e.dateTo >= dtTime && e.SysHotelID==hotelId)
                 .OrderByDescending(e => e.Id);
            return db.Select(queryPrice).FirstOrDefault();
            */
            DayOfWeek dayOfWeek = dtTime.DayOfWeek;
            string iDayOfWeek = CommService.ConvertToInt(dayOfWeek).ToString();
            //search again
            //using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_RoomPriceLevel>()
                    .Where(e => e.RoomTypeID == roomTypeId
                        && e.Status == true
                        && e.dayOfWeeks.Contains(iDayOfWeek)
                        && (e.dateFrom <= dtTime)
                        && (e.dateTo >= dtTime)
                        )
                    .OrderByDescending(e => e.Id);

                tbl_RoomPriceLevel rows = db.Select<tbl_RoomPriceLevel>(query).FirstOrDefault();
                return rows;
            }
        }
        public RoomPriceEstimateModel caculatePriceMin(List<RoomPriceEstimateModel> lstObj)
        {
            RoomPriceEstimateModel firstObj = lstObj[0];
            foreach (RoomPriceEstimateModel o in lstObj) 
            {
                if (o.price <= firstObj.price) firstObj = o;
            }
            return firstObj;
        }
        /// <summary>
        /// Hàm tính giá, trả về list giá được tính
        /// </summary>
        /// <param name="typePrice">theo loại gì</param>
        /// <param name="roomtypeid">hạng phòng gì</param>
        /// <param name="dtFrom">từ ngày nào</param>
       /// <param name="dtTo">đến ngày nào</param>
        /// <returns></returns>
       public List<RoomPriceEstimateModel> caculatePrice_bak(int hotelid, int typePrice,int roomid, int roomtypeid, DateTime dtFrom, DateTime dtTo)
       {
           if (roomtypeid == 0) return new List<RoomPriceEstimateModel>();
            //giờ checkin
            int hourStartCheckIn = 12;
            //giờ checkout
            int hourStartCheckOut = 10;
           
            using (var db = _connectionData.OpenDbConnection())
            {
                #region "Theo h hoặc cùng 1 ngày và không cắt h checkout-checkin"
                   //nếu là theo h thì chia đều h ra
                   //hoặc là trong 1 ngày và trước giờ checkin và sau giờ check out hoặc trong khoảng checkin-checkout
                   if ((typePrice == RoomPriceType.HOUR)//bắt tính theo h ( khách sạn bắt tính
                       ||(dtTo.Day==dtFrom.Day && dtTo.Hour<hourStartCheckOut)//cùng ngày và trước giờ check out
                       || (dtTo.Day == dtFrom.Day && dtFrom.Hour > hourStartCheckIn)//cùng ngày và sau giờ check in
                       || (dtTo.Day == dtFrom.Day && dtFrom.Hour >= hourStartCheckOut)
                       || (dtTo.Hour > hourStartCheckIn && dtFrom.Hour < hourStartCheckOut && dtTo.Day == dtFrom.Day
                       && dtTo.Hour <= hourStartCheckIn))//cùng ngày và trong khoảng check in-out
                    {
                       //lấy chính sách giá active cho ngày ngày này
                        tbl_RoomPriceLevel oPrice = getActivePrice(roomtypeid,dtFrom,db);
                        if (oPrice != null)
                        {
                            //tính thời gian lệch
                            TimeSpan tm = dtTo.Subtract(dtFrom);
                            int totalHours = tm.Hours + ((tm.Minutes > 0) ? 1 : 0);

                            //truy vấn trong db theo h
                            var queryHours = db.From<tbl_RoomPriceLevel_Hour>()
                                .Where(e => e.RoomPriceLevelID == oPrice.Id && e.numberHours == totalHours);
                            tbl_RoomPriceLevel_Hour oPriceHours = db.Select(queryHours).FirstOrDefault();

                            double hourPrice = 0; //ko có thì trả về ko
                            //trả về kết quả nếu có
                            if (oPriceHours != null) hourPrice = oPriceHours.price.Value;
                            else oPriceHours = new tbl_RoomPriceLevel_Hour();

                            return new List<RoomPriceEstimateModel>(){ RoomPriceEstimateModel.createNew()
                            .setRoomId(roomid)
                            .setRoomTypeId(roomtypeid)
                            .setTypePrice(RoomPriceType.HOUR)
                            .setPrice(hourPrice)
                            .setDtFrom(dtFrom)
                            .setQuantity(1)
                            .setPricePolicyID(oPrice.Id)
                            .setPricePolicyName(oPrice.title)
                            .setDtTo(dtTo)};
                        }                      
                   }
                    #endregion
               
                #region "Trong cùng 1 ngày và cắt checkout-checkin"

                   // 1:Phụ trội quá giờ trả theo ngày
                   // 2:  Phụ trội quá giờ trả theo đêm 
                   // 3: Phụ trội nhận phòng sớm theo ngày
                   // 4:  Phụ trội nhận phòng sớm theo đêm
                   // 5 :Phụ trội quá số lượng người lớn
                   //nếu là trươc checkin và sau checkout trong cùng 1 ngày
                   if (dtTo.Hour > hourStartCheckIn && dtFrom.Hour < hourStartCheckOut && dtTo.Day == dtFrom.Day)
                   {
                       //lấy chính xác giá xác định cho này này
                       tbl_RoomPriceLevel oPrice = getActivePrice(roomtypeid, dtFrom, db);

                       //1.tính phụ trội sớm
                       var oPrice1 = caculateExtraEarly(typePrice, roomid, roomtypeid, dtFrom, dtTo, hourStartCheckIn, db, oPrice);

                       //2.tính phụ trội muộn
                       var oPrice2 = caculateExtraLate(typePrice, roomid, roomtypeid, dtFrom, dtTo, hourStartCheckOut, db, oPrice);
                      
                        //3.tính h giữa checkout và check in
                       int totalHours1 = hourStartCheckIn - hourStartCheckOut;
                        var oPrice3 = caculateByHours(roomid, roomtypeid, dtFrom, dtTo, db, oPrice, totalHours1);

                       //số tiền bằng phụ trội sơm+ checkin-to-checkout đến phụ trội muộn
                       return new List<RoomPriceEstimateModel>() { oPrice1, oPrice2, oPrice3 };
                   } 
                #endregion
                 #region "Xuyên 2 ngày và ko căt checkout-checkin"
                   //nếu là xuyên 2 ngày mà lại ko cắt check-in
                   if (dtTo.Day - dtFrom.Day == 1 &&  dtFrom.Hour>= hourStartCheckIn && dtTo.Hour<=hourStartCheckOut) 
                   {
                       //lấy chính sách giá active cho ngày ngày này
                       tbl_RoomPriceLevel oPrice = getActivePrice(roomtypeid, dtFrom, db);
                       if (oPrice != null)
                       {
                   
                           //1.đầu tiền tính theo giờ
                           //tính thời gian lệch
                           TimeSpan tm = dtTo.Subtract(dtFrom);
                           int totalHours = tm.Hours + ((tm.Minutes > 0) ? 1 : 0);

                           //truy vấn trong db theo h
                           var queryHours = db.From<tbl_RoomPriceLevel_Hour>()
                               .Where(e => e.RoomPriceLevelID == oPrice.Id && e.numberHours == totalHours);
                           tbl_RoomPriceLevel_Hour oPriceHours = db.Select(queryHours).FirstOrDefault();

                           double hourPrice = 0; //ko có thì trả về ko
                           //trả về kết quả nếu có
                           if (oPriceHours != null) hourPrice = oPriceHours.price.Value;


                           var oPriceCurrentHours = RoomPriceEstimateModel.createNew()
                            .setRoomId(roomid)
                            .setRoomTypeId(roomtypeid)
                            .setTypePrice(RoomPriceType.HOUR)
                            .setPrice(oPriceHours.price.Value)
                            .setDtFrom(dtFrom)
                            .setQuantity(1)
                            .setPricePolicyID(oPriceHours.RoomPriceLevelID.Value)
                            .setPricePolicyName(oPrice.title)
                            .setDtTo(dtTo);

                           //sau đó tính theo đêm                          
                           var oPriceCurrentNight = RoomPriceEstimateModel.createNew()
                             .setRoomId(roomid)
                             .setRoomTypeId(roomtypeid)
                             .setTypePrice(RoomPriceType.NIGHT)
                             .setPrice(oPrice.PriceNight.Value)
                             .setDtFrom(dtFrom)
                             .setQuantity(1)
                             .setPricePolicyID(oPriceHours.RoomPriceLevelID.Value)
                             .setPricePolicyName(oPrice.title)
                             .setDtTo(dtTo);

                           //rồi tính theo ngày
                           var oPriceCurrentDay = RoomPriceEstimateModel.createNew()
                            .setRoomId(roomid)
                            .setRoomTypeId(roomtypeid)
                            .setTypePrice(RoomPriceType.DAY)
                            .setPrice(oPrice.PriceNight.Value)
                            .setDtFrom(dtFrom)
                            .setQuantity(1)
                            .setPricePolicyID(oPriceHours.RoomPriceLevelID.Value)
                            .setPricePolicyName(oPrice.title)
                            .setDtTo(dtTo);

                           //lấy nhóc nhỏ nhất
                           var minPrice=caculatePriceMin(new List<RoomPriceEstimateModel>(){oPriceCurrentNight, oPriceCurrentHours, oPriceCurrentDay});
                           return new List<RoomPriceEstimateModel>() {minPrice};
                       }    
                   }
                   #endregion
                #region "Xuyên 2 ngày và căt checkout-checkin"    
                //các trường hợp còn lại:  xuyên qua 2 ngày mà lại  cắt check-in và check-out
                if (dtTo.Subtract(dtFrom).Days >= 1 &&  dtFrom.Hour< hourStartCheckOut && dtTo.Hour>=hourStartCheckIn) 
                {

                       List<RoomPriceEstimateModel> listResult = new List<RoomPriceEstimateModel>();

                       //tính số h phụ trội sớm
                       int hourExtraEarly=hourStartCheckIn- dtFrom.Hour;
                       var oPriceEarlyPolicy = getActivePrice(roomtypeid, dtFrom, db);

                       //tính giá này
                       var oPriceEarly = caculateExtraEarly(typePrice, roomid, roomtypeid, dtFrom, dtTo, hourStartCheckIn, db, oPriceEarlyPolicy);
                       listResult.Add(RoomPriceEstimateModel.createNew()
                            .setRoomId(roomid)
                            .setRoomTypeId(roomtypeid)
                            .setTypePrice(RoomPriceType.DAY)
                            .setPrice(oPriceEarly.price)
                            .setDtFrom(dtFrom)
                            .setQuantity(1)
                            .setPricePolicyID(oPriceEarlyPolicy.Id)
                            .setPricePolicyName(oPriceEarlyPolicy.title)
                            .setDtTo(dtTo));
                     

                       //tính số lượng giữa các ngày ở giữa
                       DateTime dtStart=new DateTime(dtFrom.Year,dtFrom.Month,dtFrom.Day,hourStartCheckIn,0,0);
                       DateTime dtStop=new DateTime(dtFrom.Year,dtFrom.Month,dtFrom.Day,hourStartCheckOut,0,0);


                       //khi mà ngày kết thúc còn lớn hơn ngày bắt đầu
                       while(dtStop.Subtract(dtTo).Days>=0)
                       {
                           //lấy chính sách giá ngày đang tính
                           var oPriceDayCurrent=getActivePrice(roomtypeid,dtStart,db);
                           //thêm vào danh sách
                           listResult.Add(RoomPriceEstimateModel.createNew()
                            .setRoomId(roomid)
                            .setRoomTypeId(roomtypeid)
                            .setTypePrice(RoomPriceType.DAY)
                            .setPrice(oPriceDayCurrent.Id)
                            .setDtFrom(dtFrom)
                            .setQuantity(1)
                            .setPricePolicyID(oPriceDayCurrent.Id)
                            .setPricePolicyName(oPriceDayCurrent.title)
                            .setDtTo(dtTo));
                            dtStart=dtStart.AddDays(1);
                       }

                       //tính số h phụ trội muộn
                       //tính giá này
                       int hourExtraLate=dtTo.Hour- hourStartCheckOut;
                       var oPriceLatePolicy = getActivePrice(roomtypeid, dtTo, db);
                       //tính giá này
                       var oPriceLate = caculateExtraLate(typePrice, roomid, roomtypeid, dtFrom, dtTo, hourStartCheckIn, db, oPriceLatePolicy);
                        //add vào danh sách
                       listResult.Add(RoomPriceEstimateModel.createNew()
                               .setRoomId(roomid)
                               .setRoomTypeId(roomtypeid)
                               .setTypePrice(RoomPriceType.DAY)
                               .setPrice(oPriceLate.price)
                               .setDtFrom(dtFrom)
                               .setQuantity(1)
                               .setPricePolicyID(oPriceLatePolicy.Id)
                               .setPricePolicyName(oPriceLatePolicy.title)
                               .setDtTo(dtTo));

                       return listResult;
                }
                #endregion
            }
            //mặc định là trả về ko có chính sách giá
            var oResult=RoomPriceEstimateModel.createNew()
                        .setRoomId(roomid)
                        .setRoomTypeId(roomtypeid)
                        .setTypePrice(RoomPriceType.HOUR)
                        .setPrice(0)
                        .setDtFrom(dtFrom)
                        .setQuantity(1)
                        .setPricePolicyID(0)
                        .setPricePolicyName("Chưa có chính sách giá")
                        .setDtTo(dtTo);
            //thực hiện trả về mặc định
            return new List<RoomPriceEstimateModel>(){oResult} ;
       }
       public List<RoomPriceEstimateModel> caculateExtraPeople(int roomtypeid, int roomid, DateTime dtFrom, DateTime dtTo, int numberAdult, int numberChildren) 
       {
           List <RoomPriceEstimateModel > lstResult= new List<RoomPriceEstimateModel>();
           using (var db = _connectionData.OpenDbConnection())
           {
               //lấy chính sách giá phụ trội người lớn và trẻ em cho ngày đầu tiên
               tbl_RoomPriceLevel oPriceActivePeople = getActivePrice(roomtypeid, dtFrom, db);
               if (oPriceActivePeople != null)
               {
                   if (oPriceActivePeople.Number_Adult < numberAdult)
                   {
                       RoomPriceEstimateModel oDataEarly =
                               caculateExtraByTotalHoursOrPeople(TypeExtra.OVER_NUMBER_ADULT, -1, roomid, roomtypeid, numberAdult - oPriceActivePeople.Number_Adult ?? 0, db, oPriceActivePeople);

                       oDataEarly.setDtFrom(dtFrom);
                       oDataEarly.setTypePrice(RoomPriceType.EXTRA_LATE_ADULT);
                       oDataEarly.updateTitlePrice();
                       oDataEarly.setDtTo(dtTo);
                       lstResult.Add(oDataEarly);
                   }//nếu quá người lớn
                   if (1 < numberChildren) //mặc định trẻ em chỉ cho 1
                   {
                       RoomPriceEstimateModel oDataEarly =
                               caculateExtraByTotalHoursOrPeople(TypeExtra.OVER_NUMBER_CHILDREN, -1, roomid, roomtypeid, numberChildren - 1, null, oPriceActivePeople);
                       oDataEarly.setDtFrom(dtFrom);
                       oDataEarly.setDtTo(dtTo);
                       oDataEarly.setTypePrice(RoomPriceType.EXTRA_LATE_CHIDLREN);
                       oDataEarly.updateTitlePrice();
                       lstResult.Add(oDataEarly);
                   }//nếu quá trẻ em
               }
           }
           return lstResult;
       }
       public List<RoomPriceEstimateModel> caculatePrice(int hotelid, int typePrice, int roomid, int roomtypeid, DateTime dtFrom, DateTime dtTo, int reservationID, int? numberAdult = 1, int? numberChildren = 1)
       {
           List<RoomPriceEstimateModel> priceRoom = caculatePriceOnRoom(hotelid, typePrice, roomid, roomtypeid, dtFrom, dtTo, reservationID);
           List<RoomPriceEstimateModel> pricePeopleExtra = caculateExtraPeople(roomtypeid,roomid, dtFrom, dtTo, numberAdult??1, numberChildren??1);
           return priceRoom.Union(pricePeopleExtra).ToList();
       }
       public List<RoomPriceEstimateModel> caculatePriceOnRoom(int hotelid, int typePrice, int roomid, int roomtypeid, DateTime dtFrom, DateTime dtTo, int reservationID)
       {
           if (roomtypeid == 0) return new List<RoomPriceEstimateModel>();
           //giờ checkin for day
           var oConfig = (new CommService()).getConfigHotel();
          
           int hourStartCheckIn = (int)oConfig.startCheckin;// 12;
           //giờ checkout for day and night
           int hourStartCheckOut = (int)oConfig.startCheckout;// 10; giờ checkout ngày
           //from for night hour
           int hourNight1 = (int)oConfig.startNight1;// 21;
           //to for night hour next day
           int hourNight2 = (int)oConfig.startNight2;// 05;
           //số phút làm tròn giờ
           int startRoundMain = (int)oConfig.startRoundMain;// 15;
           //to for night hour next day
           int startRoundExatra = (int)oConfig.startRoundExatra;// 15;

           int hourStartCheckOutNight = (int)oConfig.startCheckoutNight;// 10;  giờ checkout đêm


           using (var db = _connectionData.OpenDbConnection())
           {


               #region "Nhỏ hơn <= 7 tiêng: mặc định là tính theo giờ"
               TimeSpan tm = dtTo.Subtract(dtFrom);
               //int totalHours = tm.Hours + ((tm.Minutes > 0) ? 1 : 0);
               int totalHours = (int)tm.TotalHours + ((tm.Minutes > startRoundMain) ? 1 : 0);
               //nếu là theo h thì chia đều h ra
               //hoặc là trong 1 ngày và trước giờ checkin và sau giờ check out hoặc trong khoảng checkin-checkout
               if (totalHours<=7)//theo giờ, kiểu gì cũng tính theo ngày trừ khi khách sạn muốn kiểu riêng của mình
               {
                   //lấy chính sách giá active cho ngày ngày này
                   tbl_RoomPriceLevel oPrice = getActivePrice(roomtypeid, dtFrom, db);
                   if (oPrice != null)
                   {
                       RoomPriceEstimateModel oRoomHour = new RoomPriceEstimateModel();
                       RoomPriceEstimateModel oRoomDay = new RoomPriceEstimateModel();
                       RoomPriceEstimateModel oRoomNight = new RoomPriceEstimateModel();

                       //tính thời gian
                       //nếu là tính theo giờ 
                       if (typePrice == RoomPriceType.HOUR || typePrice == RoomPriceType.ALL)
                       {
                           //truy vấn trong db theo h
                           var queryHours = db.From<tbl_RoomPriceLevel_Hour>()
                               .Where(e => e.RoomPriceLevelID == oPrice.Id && e.numberHours == totalHours);
                           tbl_RoomPriceLevel_Hour oPriceHours = db.Select(queryHours).FirstOrDefault();

                           double hourPrice = 0; //ko có thì trả về ko
                           //trả về kết quả nếu có
                           if (oPriceHours != null) hourPrice = oPriceHours.price.Value;
                           else oPriceHours = new tbl_RoomPriceLevel_Hour();

                           oRoomHour= RoomPriceEstimateModel.createNew()
                            .setRoomId(roomid)
                            .setRoomTypeId(roomtypeid)
                            .setTypePrice(RoomPriceType.HOUR)
                            .setPrice(hourPrice)
                            .setDtFrom(dtFrom)
                            .setQuantity(1)
                            .setPricePolicyID(oPrice.Id)
                            .setPricePolicyName(oPrice.title)
                            .updateTitlePrice()
                            .setDtTo(dtTo);
                           if (typePrice == RoomPriceType.HOUR) return new List<RoomPriceEstimateModel>() { oRoomHour };
                       }
                       if (typePrice == RoomPriceType.NIGHT || typePrice == RoomPriceType.ALL)
                       {
                           oRoomNight= RoomPriceEstimateModel.createNew()
                            .setRoomId(roomid)
                            .setRoomTypeId(roomtypeid)
                            .setTypePrice(RoomPriceType.NIGHT)
                            .setPrice(oPrice.PriceNight.Value)
                            .setDtFrom(dtFrom)
                            .setQuantity(1)
                            .setPricePolicyID(oPrice.Id)
                            .setPricePolicyName(oPrice.title)
                            .updateTitlePrice()
                            .setDtTo(dtTo);
                           if (typePrice == RoomPriceType.NIGHT) return new List<RoomPriceEstimateModel>() { oRoomNight };
                       }
                       if (typePrice == RoomPriceType.DAY || typePrice == RoomPriceType.ALL)
                       {
                           oRoomDay= RoomPriceEstimateModel.createNew()
                            .setRoomId(roomid)
                            .setRoomTypeId(roomtypeid)
                            .setTypePrice(RoomPriceType.DAY)
                            .setPrice(oPrice.PriceDay.Value)
                            .setDtFrom(dtFrom)
                            .setQuantity(1)
                            .setPricePolicyID(oPrice.Id)
                            .setPricePolicyName(oPrice.title)
                            .updateTitlePrice()
                            .setDtTo(dtTo);
                           if (typePrice == RoomPriceType.DAY)return new List<RoomPriceEstimateModel>() { oRoomDay };
                       }
                       if (typePrice == RoomPriceType.ALL) //nếu là tự động thì default là theo giờ
                       {
                            return new List<RoomPriceEstimateModel>()
                                {caculatePriceMin(new List<RoomPriceEstimateModel>() { oRoomHour})};
                       }
                   }
               }
               #endregion
               #region "Trong ngày đầu tiên"
               else if ((totalHours > 7 && totalHours<=(24-(hourStartCheckIn-hourStartCheckOut))) && typePrice != RoomPriceType.ALL)//trong ngày đầu tiên và khách sạn ko muốn tính tự động
               {
                   //lấy chính sách giá active cho ngày ngày này
                   tbl_RoomPriceLevel oPrice = getActivePrice(roomtypeid, dtFrom, db);
                   if (oPrice != null)
                   {
                       RoomPriceEstimateModel oRoomHour = new RoomPriceEstimateModel();
                       RoomPriceEstimateModel oRoomDay = new RoomPriceEstimateModel();
                       RoomPriceEstimateModel oRoomNight = new RoomPriceEstimateModel();

                       //tính thời gian
                       //nếu là tính theo giờ 
                       if (typePrice == RoomPriceType.HOUR)
                       {
                           //truy vấn trong db theo h
                           var queryHours = db.From<tbl_RoomPriceLevel_Hour>()
                               .Where(e => e.RoomPriceLevelID == oPrice.Id && e.numberHours == totalHours);
                           tbl_RoomPriceLevel_Hour oPriceHours = db.Select(queryHours).FirstOrDefault();

                           double hourPrice = 0; //ko có thì trả về ko
                           //trả về kết quả nếu có
                           if (oPriceHours != null) hourPrice = oPriceHours.price.Value;
                           else oPriceHours = new tbl_RoomPriceLevel_Hour();

                           oRoomHour = RoomPriceEstimateModel.createNew()
                            .setRoomId(roomid)
                            .setRoomTypeId(roomtypeid)
                            .setTypePrice(RoomPriceType.HOUR)
                            .setPrice(hourPrice)
                            .setDtFrom(dtFrom)
                            .setQuantity(totalHours)
                            .setPricePolicyID(oPrice.Id)
                            .setPricePolicyName(oPrice.title)
                            .updateTitlePrice()
                            .setDtTo(dtTo);
                            return new List<RoomPriceEstimateModel>() { oRoomHour };
                       }
                       if (typePrice == RoomPriceType.NIGHT)
                       {
                           oRoomNight = RoomPriceEstimateModel.createNew()
                            .setRoomId(roomid)
                            .setRoomTypeId(roomtypeid)
                            .setTypePrice(RoomPriceType.NIGHT)
                            .setPrice(oPrice.PriceNight.Value)
                            .setDtFrom(dtFrom)
                            .setQuantity(1)
                            .setPricePolicyID(oPrice.Id)
                            .setPricePolicyName(oPrice.title)
                            .updateTitlePrice()
                            .setDtTo(dtTo);
                            return new List<RoomPriceEstimateModel>() { oRoomNight };
                       }
                       if (typePrice == RoomPriceType.DAY)
                       {
                           oRoomDay = RoomPriceEstimateModel.createNew()
                            .setRoomId(roomid)
                            .setRoomTypeId(roomtypeid)
                            .setTypePrice(RoomPriceType.DAY)
                            .setPrice(oPrice.PriceDay.Value)
                            .setDtFrom(dtFrom)
                            .setQuantity(1)
                            .setPricePolicyID(oPrice.Id)
                            .setPricePolicyName(oPrice.title)
                            .updateTitlePrice()
                            .setDtTo(dtTo);
                           return new List<RoomPriceEstimateModel>() { oRoomDay };
                       }                       
                   }
               }
               #endregion
               #region ">7 tiếng và ko phải tính tự động : chia làm 2 khung"
               ///khung 1:1. cắt từ thời gian checkin của khách đến giờ quy định checkout ngày gần nhất hoặc checkin đêm"
               ///khung 2:2. cắt từ thời gian checkin quy định đến giờ checkout của khách
               //if (totalHours > 7 && typePrice == RoomPriceType.ALL)
               else 
               {
                   List<RoomPriceEstimateModel> listResult = new List<RoomPriceEstimateModel>();
                   //lấy chính sách giá ngày đang tính là
                   // phụ trội sớm+ cách tính giá + phụ trội muôn: lặp liên tục
                   //nhảy từ  thời gian checkin đến thời gian checkout
                   DateTime dtStart = dtFrom;

                   DateTime dtEnd = dtTo;
                   int totalHourEarly = 0;
                   TimeSpan timeSpanEarly = new TimeSpan();

                   //start từ đêm, hay từ checkout, nếu từ checkout thì mốc sau phải lấy từ checkin
                   //start từ night2 thì lấy mốc từ nigth2
                   //=0: từ checkout
                   //=1: từ night1

                   int typePriceExtraStart = 0;//loại phụ sớm sớm đầu tiên là theo ngày hay đêm
                   int lastStart = RoomPriceType.ALL;//cái cuối cùng đang tình là theo khung gì: đêm hay ngày
                   int typePriceStart = 0;//loại đầu tiên đang tính là theo khung gì: đêm hay ngày
                  


                   //nếu là trong mốc : từ checkin đến start night1: phụ trội sớm đêm
                   if (dtStart.Hour < hourNight1 && dtStart.Hour > hourStartCheckIn)
                   {
                       totalHourEarly = (hourNight1 - dtStart.Hour);//phụ trội sớm của đêm
                       timeSpanEarly = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, hourNight1, 0, 0).Subtract(dtStart);

                       typePriceExtraStart = TypeExtra.EARLY_CHECKIN_NIGHT;
                       typePriceStart=RoomPriceType.EXTRA_EARLY_NIGHT;
                       lastStart = RoomPriceType.NIGHT;
                   }
                   //nếu là từ:  từ night2 đến checkout:phụ trội ngày sớm
                   if (dtStart.Hour >= hourNight2  &&  dtStart.Hour < hourStartCheckIn)
                   {
                       totalHourEarly = (hourStartCheckIn - dtStart.Hour);//phụ trội sớm của ngày 
                       timeSpanEarly = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, hourStartCheckIn, 0, 0).Subtract(dtStart);
   
                       typePriceExtraStart = TypeExtra.EARLY_CHECKIN_DAY;
                        typePriceStart=RoomPriceType.EXTRA_EARLY_DAY;
                       lastStart = RoomPriceType.DAY;
                   }
                   //////từ trong thời gian quy định đêm 1 đến sáng hôm sau 2: checkin theo đêm
                   if (dtStart.Hour >= hourNight1 || dtStart.Hour < hourNight2)
                   {
                       lastStart = RoomPriceType.NIGHT;
                   }

                   //////từ checkin ở thời điểm đêm night1:phụ trội đêm sớm
                   ////if (dtStart.Hour < hourNight1 && dtStart.Hour >= hourStartCheckIn) 
                   ////{
                   ////    totalHourEarly = (hourNight1 - dtStart.Hour);
                   ////    typePriceExtraStart = TypeExtra.EARLY_CHECKIN_NIGHT;//phụ trội sớm của đêm   
                   ////    typePriceStart=RoomPriceType.EXTRA_EARLY_NIGHT;
                   ////    lastStart = RoomPriceType.NIGHT;
                   ////}

                   //tính toán phụ trội sớm nếu cần: nếu như quá số giờ quy định thì thêm 1
                   totalHourEarly = (int)timeSpanEarly.TotalHours + ((timeSpanEarly.Minutes > (int)startRoundMain)?1:0);

                   if(totalHourEarly>0 )
                   {
                       //chỉ tính giá nếu số phút lớn hơn số h quy định                      
                        var oPriceDayCurrent = getActivePrice(roomtypeid, dtStart, db);
                        RoomPriceEstimateModel oDataEarly =
                            caculateExtraByTotalHoursOrPeople(typePriceExtraStart, typePriceStart, roomid, roomtypeid, totalHourEarly, db, oPriceDayCurrent);
                        oDataEarly.setDtFrom(dtStart);
                        oDataEarly.setDtTo(dtStart.Add(timeSpanEarly));

                        ////thêm vào danh sách
                        listResult.Add(oDataEarly);
                       
                       //cập nhật thời gian
                        dtStart = dtStart.Add(timeSpanEarly);//thêm để chạm mốc checkout hoặc bắt đầu checkin đêm(21h)
                   }
                   
                   //khi mà ngày kết thúc còn lớn hơn ngày bắt đầu thì tiếp tục đệ quy
                   //bắt đầu đệ quy bằng cách lấy 2 mốc checkout và mốc bắt đầu checkin đêm để cắt
                   while (dtEnd.CompareTo(dtStart) > 0)
                   {
                       //các điểm cắt là : ----->night2 -----01---->checkout-->checkin----02-->night1---->
                       //lấy chính sách giá ngày đang tính
                       
                       //nếu mốc thời gian mà đủ 1 ngày thì thực hiện nhảy
                       //cắt từ thời điểm start đến checkout của ngày hôm sau
                       //nếu vẫn đủ đến thời gian checkout thì cắt giữa
                       DateTime dtNextDayCHeckout;//xác định thời gian checkout kế tiếp
                       //nếu là tính theo ngay: 1 là trước đó theo ngày, 2 là đã có 2 rồi, 3 là có 1 rồi và ko phụ trội ( tức là trước đó ít nhất 1 đêm rồi)
                       if  ((lastStart == RoomPriceType.DAY || listResult.Count>=2 || (listResult.Count==1 && totalHourEarly==0)))
                       {
                           dtNextDayCHeckout = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, hourStartCheckOut - 1, 59, 59).AddDays(1);
                           //nếu là nhận đêm đầu tiên mà ở cùng ngày thi ko cần tăng ngày làm gì: check out chỉ trong  1 ngày
                           if (lastStart == RoomPriceType.NIGHT && dtStart.Hour <= hourNight2)//theo đêm mà giờ ở khoảng thứ 2:bắt đầu 1 ngày mới, ko phải của ngày hôm trước
                           {
                               dtNextDayCHeckout = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, hourStartCheckOut - 1, 59, 59);
                           }
                       }
                       else 
                       {
                           
                           dtNextDayCHeckout = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, hourStartCheckOutNight - 1, 59, 59).AddDays(1);
                           //nếu là nhận đêm đầu tiên mà ở cùng ngày thi ko cần tăng ngày làm gì: check out chỉ trong  1 ngày
                           if (lastStart == RoomPriceType.NIGHT && dtStart.Hour <= hourNight2)//theo đêm mà giờ ở khoảng thứ 2:bắt đầu 1 ngày mới, ko phải của ngày hôm trước
                           {
                               dtNextDayCHeckout = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, hourStartCheckOutNight - 1, 59, 59);
                           }
                       }


                       DateTime dtNextNight = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, hourNight2, 0, 0);
                       
                       //đủ qua thời gian checkout ngày hôm sau ko ?
                       if (dtEnd.CompareTo(dtNextDayCHeckout) > 0)
                       {
                           //nếu lần đầu tiên là phụ trội theo ngày thì lấy theo ngày
                           //hoặc là từ chú thứ 3 rồi: vì phụ trội đêm, đêm, thì phải đến ngày: đêm chỉ xuất hiện 1 lần đầu tiên mà thôi
                           //hoặc lần kế trước là ngày
                           if (lastStart == RoomPriceType.DAY || listResult.Count>=2 || (listResult.Count==1 && totalHourEarly==0))
                           {
                               var oPriceDayCurrent = getActivePrice(roomtypeid, dtStart, db);
                               ////thêm vào danh sách
                               listResult.Add(RoomPriceEstimateModel.createNew()
                                .setRoomId(roomid)
                                .setRoomTypeId(roomtypeid)
                                .setTypePrice(RoomPriceType.DAY)
                                .setPrice(oPriceDayCurrent.PriceDay.Value)
                                .setDtFrom(dtStart)
                                .setQuantity(1)
                                .setPricePolicyID(oPriceDayCurrent.Id)
                                .setPricePolicyName(oPriceDayCurrent.title)
                                .updateTitlePrice()
                                .setDtTo(dtNextDayCHeckout));

                               //cập nhật cả thời gian checkin và checkout
                               dtStart = dtNextDayCHeckout.AddHours(hourStartCheckIn-hourStartCheckOut).AddMinutes(1);//gán lại dtStart để cập nhật vòng lặp
                               lastStart = RoomPriceType.DAY;
                               continue;
                           }

                           //nếu lần đầu tiên là phụ trội theo đêm thì lấy theo đêm
                           if (lastStart == RoomPriceType.NIGHT)//nếu là đêm/hoặc phụ trội đêm trước đó
                           {
                               var oPriceDayCurrent = getActivePrice(roomtypeid, dtStart, db);
                               ////thêm vào danh sách
                               listResult.Add(RoomPriceEstimateModel.createNew()
                                .setRoomId(roomid)
                                .setRoomTypeId(roomtypeid)
                                .setTypePrice(RoomPriceType.NIGHT)
                                .setPrice(oPriceDayCurrent.PriceNight.Value)
                                .setDtFrom(dtStart)
                                .setQuantity(1)
                                .setPricePolicyID(oPriceDayCurrent.Id)
                                .setPricePolicyName(oPriceDayCurrent.title)
                                .updateTitlePrice()
                                .setDtTo(dtNextDayCHeckout));
                               //trừ thêm thời gian checkout night nữa
                               dtStart = dtNextDayCHeckout.AddHours(hourStartCheckIn -hourStartCheckOutNight).AddMinutes(1);// new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, hourNight1, 0, 0);//gán lại dtStart
                               lastStart = RoomPriceType.NIGHT;
                               continue;
                           }
                       }

                       //nếu ko đủ thì là tính phụ trội muộn hoặc tròn ngày,đêm của chú cuối cùng
                       else 
                       {
                           //xem đã có chưa(nếu là lần đầu tiên mà có phụ trội: thì phải tính là tròn 1 ngày, hoặc 1 đêm)
                           //còn không là tính theo phụ trội
                           if (totalHourEarly>0 && listResult.Count <= 1)//nếu có bản ghi rồi mà <=1 mà thừa giờ thì tức là có phụ trội rồi
                           {
                               var oPriceDayCurrent = getActivePrice(roomtypeid, dtStart, db);
                               //nếu đang tính phụ trội đêm  thì là tính giá đêm
                               if (lastStart == RoomPriceType.NIGHT) 
                               {
                                   var oData = RoomPriceEstimateModel.createNew()
                                   .setRoomId(roomid)
                                   .setRoomTypeId(roomtypeid)
                                   .setTypePrice(RoomPriceType.NIGHT)
                                   .setPrice(oPriceDayCurrent.PriceNight.Value)
                                   .setDtFrom(dtStart)
                                   .setQuantity(1)
                                   .setPricePolicyID(oPriceDayCurrent.Id)
                                   .setPricePolicyName(oPriceDayCurrent.title)
                                   .updateTitlePrice()
                                   .setDtTo(dtEnd);
                                   listResult.Add(oData);
                                   dtStart = dtEnd;
                               }
                               //nếu đang tính phụ trội ngày  thì là tính giá ngày
                               if (lastStart == RoomPriceType.DAY)
                               {
                                   var oData = RoomPriceEstimateModel.createNew()
                                       .setRoomId(roomid)
                                       .setRoomTypeId(roomtypeid)
                                       .setTypePrice(RoomPriceType.DAY)
                                       .setPrice(oPriceDayCurrent.PriceDay.Value)
                                       .setDtFrom(dtStart)
                                       .setQuantity(1)
                                       .setPricePolicyID(oPriceDayCurrent.Id)
                                       .setPricePolicyName(oPriceDayCurrent.title)
                                       .updateTitlePrice()
                                       .setDtTo(dtEnd);
                                   listResult.Add(oData);
                                   dtStart = dtEnd;
                               }                              
                           }
                           else
                           {

                               //tính xem là phụ trội muộn của đêm hay ngày
                               int typeExtra = 0;// để tính phụ trội
                               int typePriceEnd = 0;//để hiển thị title

                               //nếu ko phải lần đầu tiên: trước đó  đang tính theo là đêm hay là ngày rồi thì phụ trội tính theo cái đó
                               if (lastStart == RoomPriceType.NIGHT)
                               {
                                   typeExtra = TypeExtra.LATE_CHECKOUT_NIGHT;
                                   typePriceEnd = RoomPriceType.EXTRA_LATE_NIGHT;
                               }
                               else if (lastStart == RoomPriceType.DAY) { 
                                   typeExtra = TypeExtra.LATE_CHECKOUT_DAY;
                                   typePriceEnd = RoomPriceType.EXTRA_LATE_DAY;
                               }
                               //nếu là lần đầu tiên thì lấy theo typeStart: chưa có gì thì lấy theo ngày bắt đầu
                               else
                               {
                                   if (typePriceExtraStart == TypeExtra.EARLY_CHECKIN_DAY)
                                   {
                                       typeExtra = TypeExtra.LATE_CHECKOUT_DAY;
                                       typePriceEnd = RoomPriceType.EXTRA_LATE_DAY;
                                   }
                                   if (typePriceExtraStart == TypeExtra.EARLY_CHECKIN_NIGHT)
                                   {
                                       typeExtra = TypeExtra.LATE_CHECKOUT_NIGHT;
                                       typePriceEnd = RoomPriceType.EXTRA_LATE_NIGHT;
                                   }
                               }
                               //tính tổng thời gian bị muộn
                               TimeSpan tmRemain = dtEnd.Subtract(dtStart);
                               int totalHoursRemain = (int)tmRemain.TotalHours + (tmRemain.Minutes > startRoundExatra ? 1 : 0);

                               var oPriceDayCurrent = getActivePrice(roomtypeid, dtStart, db);
                               RoomPriceEstimateModel oDataLate
                                   = caculateExtraByTotalHoursOrPeople(typeExtra, typePriceEnd, roomid, roomtypeid, totalHoursRemain, db, oPriceDayCurrent);
                               
                               //nếu ko có phụ trội thì tính tròn là 1 ngày hoặc 1 đêm
                               //tròn đêm: nếu trong danh sách chưa có đêm
                               //tròn ngày: nếu trong danh sách chưa có ngày
                               if (oDataLate.price==0)
                               {
                                  
                                    if (typePriceEnd == RoomPriceType.EXTRA_LATE_NIGHT)
                                    {
                                        oDataLate.quantiy = 1;
                                        oDataLate.price = oPriceDayCurrent.PriceNight.Value;
                                        oDataLate.typePrice = RoomPriceType.NIGHT;
                                    }
                                    if (typePriceEnd == RoomPriceType.EXTRA_LATE_DAY)
                                    {
                                         oDataLate.price = oPriceDayCurrent.PriceDay.Value;
                                         oDataLate.quantiy = 1;
                                         oDataLate.typePrice = RoomPriceType.DAY;
                                    }

                                    //nếu đã tồn tại đêm rồi, thì chỉ tính là ngày thôi
                                   //kết quả luôn tồn tại duy nhất 1 đêm
                                    if(listResult.Where(e=>e.typePrice==RoomPriceType.NIGHT).Count()>0)
                                    {
                                        oDataLate.quantiy = 1;
                                        oDataLate.price = oPriceDayCurrent.PriceDay.Value;
                                        oDataLate.typePrice = RoomPriceType.DAY;
                                    }
                               }

                               oDataLate.setDtFrom(dtStart);
                               oDataLate.setDtTo(dtEnd);
                               dtStart = dtEnd;// dtStart.AddHours(totalHoursRemain);//cập nhật lại dtStart

                               ////thêm vào danh sách
                               listResult.Add(oDataLate);
                           }
                       }
                   }

                  //trả về kết quả
                   return listResult;
               }
                #endregion

               
           }
           //mặc định là trả về ko có chính sách giá
           var oResult = RoomPriceEstimateModel.createNew()
                       .setRoomId(roomid)
                       .setRoomTypeId(roomtypeid)
                       .setTypePrice(RoomPriceType.HOUR)
                       .setPrice(0)
                       .setDtFrom(dtFrom)
                       .setQuantity(1)
                       .setPricePolicyID(0)
                       .setPricePolicyName("Chưa có chính sách giá")
                       .updateTitlePrice()
                       .setDtTo(dtTo);
           //thực hiện trả về mặc định
           return new List<RoomPriceEstimateModel>() { oResult };


       }
       
        public RoomPriceEstimateModel caculateByHours(int roomid, int roomtypeid, DateTime dtFrom, DateTime dtTo, IDbConnection db, tbl_RoomPriceLevel oPrice, int totalHours1)
       {
           //tính thời gian lệch
           //truy vấn trong db theo h
           var queryHours = db.From<tbl_RoomPriceLevel_Hour>()
               .Where(e => e.RoomPriceLevelID == oPrice.Id && e.numberHours == totalHours1);
           tbl_RoomPriceLevel_Hour oPriceHours = db.Select(queryHours).FirstOrDefault();

           double hourPrice = 0; //ko có thì trả về ko
           //trả về kết quả nếu có
           if (oPriceHours != null) hourPrice = oPriceHours.price.Value;

           //trả về kết quả
           var oPrice3 = RoomPriceEstimateModel.createNew()
           .setRoomId(roomid)
           .setRoomTypeId(roomtypeid)
           .setTypePrice(RoomPriceType.HOUR)
           .setPrice(hourPrice)
           .setDtFrom(dtFrom)
           .setQuantity(1)
           .setPricePolicyID(oPriceHours.RoomPriceLevelID.Value)
           .setPricePolicyName(oPrice.title)
           .setDtTo(dtTo);
           return oPrice3;
       }

       private RoomPriceEstimateModel caculateExtraLate(int typePrice, int roomid, int roomtypeid, DateTime dtFrom, DateTime dtTo, int hourStartCheckOut, IDbConnection db, tbl_RoomPriceLevel oPrice)
       {
           tbl_RoomPriceLevel_Extra oPriceLate = new tbl_RoomPriceLevel_Extra();
           int numberHourLate = (hourStartCheckOut - dtTo.Hour) + 1;//phụ trội muộn
           //truy vấn trong db theo h phụ trội
           var queryLate = db.From<tbl_RoomPriceLevel_Extra>()
               .Where(e => e.RoomPriceLevelID == oPrice.Id && e.numberExtra == numberHourLate);

           //nếu xác định tính theo ngày hay theo đêm thì tính theo cái đó  thì tính theo đó
           if (typePrice == RoomPriceType.DAY) queryLate.Where(e => e.typeExtra == TypeExtra.EARLY_CHECKIN_DAY);
           if (typePrice == RoomPriceType.NIGHT) queryLate.Where(e => e.typeExtra == TypeExtra.EARLY_CHECKIN_NIGHT);
           else //ko thì thằng nào rẻ hơn lấy thằng đó
           {
               var queryLate1 = queryLate.Clone().Where(e => e.typeExtra == TypeExtra.LATE_CHECKOUT_DAY);
               var queryLate2 = queryLate.Clone().Where(e => e.typeExtra == TypeExtra.LATE_CHECKOUT_NIGHT);

               var oPriceLate1 = db.Select<tbl_RoomPriceLevel_Extra>(queryLate1).FirstOrDefault();
               var oPriceLate2 = db.Select<tbl_RoomPriceLevel_Extra>(queryLate2).FirstOrDefault();

               if (oPriceLate1 == null) oPriceLate1 = new tbl_RoomPriceLevel_Extra() { priceExtra = 0 };
               if (oPriceLate2 == null) oPriceLate2 = new tbl_RoomPriceLevel_Extra() { priceExtra = 0 };

               if (oPriceLate1.priceExtra.Value >= oPriceLate1.priceExtra.Value) oPriceLate = oPriceLate2;
               else oPriceLate = oPriceLate1;
           }

           var oPrice2 = RoomPriceEstimateModel.createNew()
                .setRoomId(roomid)
                .setRoomTypeId(roomtypeid)
                .setTypePrice(RoomPriceType.EXTRA_LATE_DAY)
                .setPrice(oPriceLate.priceExtra.Value)
                .setDtFrom(dtFrom)
                .setQuantity(1)
                .setPricePolicyID(oPrice.Id)
                .setPricePolicyName(oPrice.title)
                .setDtTo(dtTo);
           return oPrice2;
       }
        /// <summary>
        /// trả về hàm tính giá trị
        /// </summary>
        /// <param name="typePriceExtra"></param>
        /// <param name="roomid"></param>
        /// <param name="roomtypeid"></param>
        /// <param name="numberHourLate"></param>
        /// <param name="db"></param>
        /// <param name="oPrice"></param>
        /// <returns></returns>
       private RoomPriceEstimateModel caculateExtraByTotalHoursOrPeople(int typePriceExtra,int typePrice, int roomid, int roomtypeid, int numberHourLate, IDbConnection db, tbl_RoomPriceLevel oPrice)
       {
           tbl_RoomPriceLevel_Extra oPriceLate = new tbl_RoomPriceLevel_Extra();
           //truy vấn trong db theo h phụ trội
           var queryExtra = db.From<tbl_RoomPriceLevel_Extra>()
               .Where(e => e.RoomPriceLevelID == oPrice.Id && e.numberExtra == numberHourLate);
           queryExtra.Where(e => e.typeExtra == typePriceExtra);
           oPriceLate = db.Select<tbl_RoomPriceLevel_Extra>(queryExtra).FirstOrDefault();           
           if (oPriceLate == null)
           {
               oPriceLate = new tbl_RoomPriceLevel_Extra();
               oPriceLate.priceExtra = 0;
           }
           var oPrice2 = RoomPriceEstimateModel.createNew()
                .setRoomId(roomid)
                .setRoomTypeId(roomtypeid)
                .setTypePrice(typePrice)
                .setPrice(oPriceLate.priceExtra.Value)
                .setDtFrom(DateTime.Now)
                .setQuantity(numberHourLate)
                .setPricePolicyID(oPrice.Id)
                .setPricePolicyName(oPrice.title)
                .updateTitlePrice()
                .setDtTo(DateTime.Now);
           return oPrice2;
       }
       
       private RoomPriceEstimateModel caculateExtraEarly(int typePrice, int roomid, int roomtypeid, DateTime dtFrom, DateTime dtTo, int hourStartCheckIn, IDbConnection db, tbl_RoomPriceLevel oPrice)
       {
           tbl_RoomPriceLevel_Extra oPriceEarly = new tbl_RoomPriceLevel_Extra();
           int numberHourEarly = (hourStartCheckIn - dtFrom.Hour) + 1;//phụ trội sớm
           //truy vấn trong db theo h phụ trội
           var queryEarly = db.From<tbl_RoomPriceLevel_Extra>()
               .Where(e => e.RoomPriceLevelID == oPrice.Id && e.numberExtra == numberHourEarly);

           //nếu xác định tính rồi thì tính theo đó
           if (typePrice == RoomPriceType.DAY) queryEarly.Where(e => e.typeExtra == TypeExtra.EARLY_CHECKIN_DAY);
           if (typePrice == RoomPriceType.NIGHT) queryEarly.Where(e => e.typeExtra == TypeExtra.EARLY_CHECKIN_NIGHT);
           else //ko thì thằng nào rẻ hơn lấy thằng đó
           {
               var queryEarly1 = queryEarly.Clone().Where(e => e.typeExtra == TypeExtra.EARLY_CHECKIN_DAY);
               var queryEarly2 = queryEarly.Clone().Where(e => e.typeExtra == TypeExtra.EARLY_CHECKIN_NIGHT);

               var oPriceEarly1 = db.Select<tbl_RoomPriceLevel_Extra>(queryEarly1).FirstOrDefault();
               var oPriceEarly2 = db.Select<tbl_RoomPriceLevel_Extra>(queryEarly2).FirstOrDefault();

               if (oPriceEarly1 == null) oPriceEarly1 = new tbl_RoomPriceLevel_Extra() { priceExtra = 0 };
               if (oPriceEarly2 == null) oPriceEarly2 = new tbl_RoomPriceLevel_Extra() { priceExtra = 0 };

               if (oPriceEarly1.priceExtra.Value >= oPriceEarly2.priceExtra.Value) oPriceEarly = oPriceEarly2;
               else oPriceEarly = oPriceEarly1;

           }

           var oPrice1 = RoomPriceEstimateModel.createNew()
                .setRoomId(roomid)
                .setRoomTypeId(roomtypeid)
                .setTypePrice(RoomPriceType.EXTRA_EARLY_NIGHT)
                .setPrice(oPriceEarly.priceExtra.Value)
                .setDtFrom(dtFrom)
                .setQuantity(1)
                .setPricePolicyID(oPrice.Id)
                .setPricePolicyName(oPrice.title)
                .setDtTo(dtTo);
           return oPrice1;
       }
       public List<RoomPriceEstimateModel> getAll(PagingBookingModelPrice page) 
        {
            if (page == null) page = new PagingBookingModelPrice() { offset = 0, limit = 100 };
            if (page.search == null) page.search = "";

            //  ServiceStackHelper.Help();
            //  LicenseUtils.ActivatedLicenseFeatures();           
            //search again
            DateTime dtFrom = CommService.ConvertStringToDate(page.dtFrom);
            DateTime dtTo = CommService.ConvertStringToDate(page.dtTo);


            List<RoomPriceEstimateModel> rows = caculatePrice(comm.GetHotelId(), page.typePrice, page.roomid, page.roomtypeid, dtFrom, dtTo,-1,page.adult,page.children);
            return rows;                
        }
        
    }
}