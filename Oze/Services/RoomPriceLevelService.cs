using oze.data;
using Oze.Models;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Globalization;
using System.Linq;
using System.Web;
using Oze.Models.CustomerManage;
using Vw_RoomPriceLevel = oze.data.Entity.Vw_RoomPriceLevel;
using oze.data.Entity;
using Oze.AppCode.Util;

namespace Oze.Services
{
    public class RoomPriceLevelService : IRoomPriceLevelService
    {
        IOzeConnectionFactory _connectionData;

        public RoomPriceLevelService()
        {

            OrmLiteConfig.DialectProvider = SqlServerDialect.Provider;
            OrmLiteConfig.DialectProvider.UseUnicode = true; // nhập tiếng việt

            _connectionData = new OzeConnectionFactory(ConfigurationManager.AppSettings["ConnectionString_OzeGeneral"],
                SqlServerOrmLiteDialectProvider.Instance);

        }

        public List<Vw_RoomPriceLevel> getAll(PagingModel page, int hotelid, out int count)
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
                var query = db.From<Vw_RoomPriceLevel>();
                if (hotelid > 0)
                    query.Where(x => x.SysHotelID == hotelid);

                query.OrderByDescending(x => x.id);


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





        public oze.data.Entity.Vw_InforCustomer_Room GetCustomerRoom(int id)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var obj = db.Single<oze.data.Entity.Vw_InforCustomer_Room>(x => x.Id == id);
                return obj;
            }
        }

        public List<tbl_Room_Type> GeTblRoomTypes(int hotelid)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Room_Type>();
                if (hotelid > 0)
                    query.Where(x => x.HotelID == hotelid);

                return db.Select<tbl_Room_Type>(query);
            }
        }

        public List<tbl_Hotel> GetHotels()
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var obj = db.Select<tbl_Hotel>();
                return obj;
            }
        }

        public List<oze.data.Entity.Vw_RoomActive> GetRooms(int id, int hotelid, int typeRoom)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<oze.data.Entity.Vw_RoomActive>();
                query.Where(x => x.SysHotelID == hotelid);
                //if (id > 0)
                query.Where(x => x.RoomLevel_ID == typeRoom || x.RoomLevel_ID == null);

                return db.Select<oze.data.Entity.Vw_RoomActive>(query);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listRoom"></param>
        /// <param name="model"></param>
        /// <param name="listXtraDay"></param>
        /// <param name="listPriceDay"></param>
        /// <param name="listXtraNight"></param>
        /// <param name="listEarlyDay"></param>
        /// <param name="listEarlyNight"></param>
        /// <param name="listLimitPerson"></param>
        /// <returns></returns>
        public bool InsertRoomPriceLevel(List<tbl_Room> listRoom, tbl_RoomPriceLevel model,
            List<tbl_RoomPriceLevel_Extra> listXtraDay, List<tbl_RoomPriceLevel_Hour> listPriceDay,
            List<tbl_RoomPriceLevel_Extra> listXtraNight, List<tbl_RoomPriceLevel_Extra> listEarlyDay,
            List<tbl_RoomPriceLevel_Extra> listEarlyNight, List<tbl_RoomPriceLevel_Extra> listLimitPerson, List<tbl_RoomPriceLevel_Extra> listLimitPerson_Child)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                using (var tran = db.OpenTransaction())
                {
                    try
                    {
                        //  model.DateCreate = DateTime.Now;
                        //var roomtype = new tbl_Room_Type()
                        //{
                        //    Name = RoomTypeName.Trim(),
                        //    Code = "",
                        //    DouldBed = model.Twin,
                        //    SingBed = model._Single,
                        //    Note = "",
                        //    HotelID = model.SysHotelID,
                        //    UserLimit = model.CreateID
                        //};
                        //db.Save(roomtype, true);
                        //model.RoomTypeID = roomtype.Id;

                        model.DateCreate = DateTime.Now;
                        model.Status = true;
                        db.Save(model, true);
                        //db.Delete<tbl_Room_Type_Room_Rel>(p => p.LevelID == model.RoomTypeID);
                        //phụ trội muộn theo ngày
                        if (listXtraDay != null)
                        {
                            var xtraday =
                                listXtraDay.Select(
                                    x =>
                                        new tbl_RoomPriceLevel_Extra()
                                        {
                                            RoomPriceLevelID = model.Id,
                                            numberExtra = x.numberExtra,
                                            priceExtra = x.priceExtra,
                                            typeExtra = x.typeExtra
                                        });
                            db.InsertAll(xtraday);
                        }
                        //phụ trội muộn theo đêm
                        if (listXtraNight != null)
                        {
                            var xtranight =
                                listXtraNight.Select(
                                    x =>
                                        new tbl_RoomPriceLevel_Extra()
                                        {
                                            RoomPriceLevelID = model.Id,
                                            numberExtra = x.numberExtra,
                                            priceExtra = x.priceExtra,
                                            typeExtra = x.typeExtra
                                        });
                            db.InsertAll(xtranight);
                        } //Gias theo ngayf
                        if (listPriceDay != null)
                        {
                            var xpriceday =
                                listPriceDay.Select(
                                    x =>
                                        new tbl_RoomPriceLevel_Hour()
                                        {
                                            RoomPriceLevelID = model.Id,
                                            numberHours = x.numberHours,
                                            price = x.price
                                        });
                            db.InsertAll(xpriceday);

                        } //phụ trội sowms theo ngày
                        if (listEarlyDay != null)
                        {
                            var xEarlyDay =
                                listEarlyDay.Select(
                                    x =>
                                        new tbl_RoomPriceLevel_Extra()
                                        {
                                            RoomPriceLevelID = model.Id,
                                            numberExtra = x.numberExtra,
                                            priceExtra = x.priceExtra,
                                            typeExtra = x.typeExtra
                                        });
                            db.InsertAll(xEarlyDay);
                        } //phụ trội som theo dem
                        if (listEarlyNight != null)
                        {
                            var xEarlyNight =
                                listEarlyNight.Select(
                                    x =>
                                        new tbl_RoomPriceLevel_Extra()
                                        {
                                            RoomPriceLevelID = model.Id,
                                            numberExtra = x.numberExtra,
                                            priceExtra = x.priceExtra,
                                            typeExtra = x.typeExtra
                                        });
                            //vuot qua so ng
                            db.InsertAll(xEarlyNight);
                        }
                        //vuot qua so ng
                        if (listLimitPerson != null)
                        {
                            var limitPerson =
                                listLimitPerson.Select(
                                    x =>
                                        new tbl_RoomPriceLevel_Extra()
                                        {
                                            RoomPriceLevelID = model.Id,
                                            numberExtra = x.numberExtra,
                                            priceExtra = x.priceExtra,
                                            typeExtra = x.typeExtra
                                        });
                            db.InsertAll(limitPerson);
                        }
                        if (listLimitPerson_Child != null)
                        {
                            var limitPerson_child =
                                listLimitPerson_Child.Select(
                                    x =>
                                        new tbl_RoomPriceLevel_Extra()
                                        {
                                            RoomPriceLevelID = model.Id,
                                            numberExtra = x.numberExtra,
                                            priceExtra = x.priceExtra,
                                            typeExtra = x.typeExtra
                                        });
                            db.InsertAll(limitPerson_child);
                        }
                        // cau hinhs
                        //if (listRoom != null)
                        //{
                        //    var romconfig =
                        //        listRoom.Select(
                        //            x => new tbl_Room_Type_Room_Rel() { LevelID = model.RoomTypeID, roomid = x.Id });
                        //    db.InsertAll(romconfig);
                        //}
                        tran.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        return false;
                        throw;
                    }
                }

            }
        }

        public Vw_RoomPriceLevel GetPriceLevel(int id)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var obj = db.Single<Vw_RoomPriceLevel>(x => x.id == id);
                return obj;
            }
        }

        public List<tbl_RoomPriceLevel_Extra> GeTblRoomPriceLevelExtras(int RoomPriceLevelID, int type)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var obj =
                    db.Select<tbl_RoomPriceLevel_Extra>(
                        x => x.RoomPriceLevelID == RoomPriceLevelID && x.typeExtra == type);
                return obj;
            }
        }

        public List<oze.data.Entity.Vw_RoomActive> GetRoomActives(int LevelID, int hotelid)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var obj = db.Select<oze.data.Entity.Vw_RoomActive>(x => x.RoomLevel_ID == LevelID && x.SysHotelID == hotelid);
                return obj;
            }
        }

        public List<tbl_RoomPriceLevel_Hour> GeLevelHourses(int RoomPriceLevelID)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var obj = db.Select<tbl_RoomPriceLevel_Hour>(x => x.RoomPriceLevelID == RoomPriceLevelID);
                return obj;
            }
        }

        /// <summary>
        /// Caapj nhatja
        /// </summary>
        /// <param name="listRoom"></param>
        /// <param name="model"></param>
        /// <param name="listXtraDay"></param>
        /// <param name="listPriceDay"></param>
        /// <param name="listXtraNight"></param>
        /// <param name="listEarlyDay"></param>
        /// <param name="listEarlyNight"></param>
        /// <param name="listLimitPerson"></param>
        /// <returns></returns>
        public bool UpdateRoomPriceLevel(List<tbl_Room> listRoom, tbl_RoomPriceLevel model,
            List<tbl_RoomPriceLevel_Extra> listXtraDay, List<tbl_RoomPriceLevel_Hour> listPriceDay,
            List<tbl_RoomPriceLevel_Extra> listXtraNight, List<tbl_RoomPriceLevel_Extra> listEarlyDay,
            List<tbl_RoomPriceLevel_Extra> listEarlyNight, List<tbl_RoomPriceLevel_Extra> listLimitPerson, List<tbl_RoomPriceLevel_Extra> listLimitPerson_Child)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                using (var tran = db.OpenTransaction())
                {
                    try
                    {
                        //model.DateCreate = DateTime.Now;
                        var obj = db.Single<tbl_RoomPriceLevel>(x => x.Id == model.Id);
                        var rel = new tbl_RoomPriceLevel_Hi()
                        {
                            CreateID = obj.CreateID,
                            Twin = obj.Twin,
                            _Single = obj._Single,
                            Note = obj.Note,
                            Number_Adult = obj.Number_Adult,
                            PriceNight = obj.PriceNight,
                            PriceDay = obj.PriceDay,

                            PriceMonth = obj.PriceMonth,
                            dayOfWeeks = obj.dayOfWeeks,
                            DateCreate = obj.DateCreate,
                            DatePromotion = obj.DatePromotion,
                            RoomPriceLevelID = obj.Id,
                            RoomTypeID = obj.RoomTypeID,
                            Status = obj.Status,
                            SysHotelID = obj.SysHotelID,
                            TypePrice = obj.TypePrice,
                            dateFrom = obj.dateFrom,
                            dateTo = obj.dateTo
                        };
                        obj.Twin = model.Twin;
                        obj._Single = model._Single;
                        obj.Note = model.Note;
                        obj.Number_Adult = model.Number_Adult;
                        obj.PriceNight = model.PriceNight;
                        obj.PriceDay = model.PriceDay;
                        obj.PriceMonth = model.PriceMonth;
                        obj.dayOfWeeks = model.dayOfWeeks;



                        //model.DateCreate = obj.DateCreate;
                        db.Update(obj);
                        db.Insert(rel);
                        //db.Delete<tbl_RoomPriceLevel_Extra>(x=>x.==1)
                        db.Delete<tbl_RoomPriceLevel_Extra>(p => p.RoomPriceLevelID == model.Id);
                        //db.Delete<tbl_Room_Type_Room_Rel>(p => p.LevelID == model.RoomTypeID);
                        db.Delete<tbl_RoomPriceLevel_Hour>(p => p.RoomPriceLevelID == model.Id);
                        //phụ trội muộn theo ngày
                        if (listXtraDay != null)
                        {
                            var xtraday =
                                listXtraDay.Select(
                                    x =>
                                        new tbl_RoomPriceLevel_Extra()
                                        {
                                            RoomPriceLevelID = model.Id,
                                            numberExtra = x.numberExtra,
                                            priceExtra = x.priceExtra,
                                            typeExtra = x.typeExtra
                                        });
                            db.InsertAll(xtraday);
                        }
                        //phụ trội muộn theo đêm
                        if (listXtraNight != null)
                        {
                            var xtranight =
                                listXtraNight.Select(
                                    x =>
                                        new tbl_RoomPriceLevel_Extra()
                                        {
                                            RoomPriceLevelID = model.Id,
                                            numberExtra = x.numberExtra,
                                            priceExtra = x.priceExtra,
                                            typeExtra = x.typeExtra
                                        });
                            db.InsertAll(xtranight);
                        } //Gias theo ngayf
                        if (listPriceDay != null)
                        {
                            var xpriceday =
                                listPriceDay.Select(
                                    x =>
                                        new tbl_RoomPriceLevel_Hour()
                                        {
                                            RoomPriceLevelID = model.Id,
                                            numberHours = x.numberHours,
                                            price = x.price
                                        });
                            db.InsertAll(xpriceday);

                        } //phụ trội sowms theo ngày
                        if (listEarlyDay != null)
                        {
                            var xEarlyDay =
                                listEarlyDay.Select(
                                    x =>
                                        new tbl_RoomPriceLevel_Extra()
                                        {
                                            RoomPriceLevelID = model.Id,
                                            numberExtra = x.numberExtra,
                                            priceExtra = x.priceExtra,
                                            typeExtra = x.typeExtra
                                        });
                            db.InsertAll(xEarlyDay);
                        } //phụ trội som theo dem
                        if (listEarlyNight != null)
                        {
                            var xEarlyNight =
                                listEarlyNight.Select(
                                    x =>
                                        new tbl_RoomPriceLevel_Extra()
                                        {
                                            RoomPriceLevelID = model.Id,
                                            numberExtra = x.numberExtra,
                                            priceExtra = x.priceExtra,
                                            typeExtra = x.typeExtra
                                        });
                            //vuot qua so ng
                            db.InsertAll(xEarlyNight);
                        }
                        //vuot qua so ng
                        if (listLimitPerson != null)
                        {
                            var limitPerson =
                                listLimitPerson.Select(
                                    x =>
                                        new tbl_RoomPriceLevel_Extra()
                                        {
                                            RoomPriceLevelID = model.Id,
                                            numberExtra = x.numberExtra,
                                            priceExtra = x.priceExtra,
                                            typeExtra = x.typeExtra
                                        });
                            db.InsertAll(limitPerson);
                        }
                        //Phuj trooij trẻ em
                        if (listLimitPerson_Child != null)
                        {
                            var limitPerson_child =
                                listLimitPerson_Child.Select(
                                    x =>
                                        new tbl_RoomPriceLevel_Extra()
                                        {
                                            RoomPriceLevelID = model.Id,
                                            numberExtra = x.numberExtra,
                                            priceExtra = x.priceExtra,
                                            typeExtra = x.typeExtra
                                        });
                            db.InsertAll(limitPerson_child);
                        }// cau hinhs
                        //if (listRoom != null)
                        //{
                        //    var romconfig =
                        //        listRoom.Select(
                        //            x => new tbl_Room_Type_Room_Rel() { LevelID = model.Id, roomid = x.Id });
                        //    db.InsertAll(romconfig);
                        //}


                        tran.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        return false;
                        throw;
                    }
                }

            }
        }



        public int LockRoom(int id)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                try
                {
                    var obj = db.Single<tbl_RoomPriceLevel>(x => x.Id == id);
                    obj.Status = false;
                    db.Update(obj);
                    return 1;
                }
                catch (Exception)
                {
                    return 0;
                    throw;
                }

            }
        }


        public tbl_Hotel GetHotelsByid(int id)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var obj = db.Single<tbl_Hotel>(x => x.Id == id);
                return obj;
            }
        }

        public bool checkDate(DateTime fdate, DateTime tdate)
        {
            int currentHotelID = comm.GetHotelId();
            using (var db = _connectionData.OpenDbConnection())
            {
                var obj = db.Select<tbl_RoomPriceLevel>().Where(x => x.SysHotelID==currentHotelID && x.dateFrom == fdate && x.dateTo == tdate).FirstOrDefault();
                return obj != null;
            }
        }
    }
}