using oze.data;
using Oze.Models;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using Oze.Models.CustomerManage;
using Oze.AppCode.Util;

namespace Oze.Services
{
    public class CustomerManageService : ICustomerManageService
    {
        IOzeConnectionFactory _connectionData;

        public CustomerManageService()
        {
            OrmLiteConfig.DialectProvider = SqlServerDialect.Provider;
            OrmLiteConfig.DialectProvider.UseUnicode = true; // nhập tiếng việt
                        _connectionData = new OzeConnectionFactory(ConfigurationManager.AppSettings["ConnectionString_OzeGeneral"],
                SqlServerOrmLiteDialectProvider.Instance);

        }

        public List<tbl_Customer> getAll(PagingModel page, SearchCustomerModel model, out int count)
        {
            if (page.search == null) page.search = "";

            //  ServiceStackHelper.Help();
            //  LicenseUtils.ActivatedLicenseFeatures();           
            //search again
            DateTime _fdate;
            DateTime _tdate;

            DateTime.TryParse(model.Fdate, CultureInfo.GetCultureInfo("vi-vn"), DateTimeStyles.None, out _fdate);
            DateTime.TryParse(model.Tdate, CultureInfo.GetCultureInfo("vi-vn"), DateTimeStyles.None, out _tdate);
            _tdate = _tdate.AddDays(1);
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Customer>();
                if (!string.IsNullOrEmpty(model.Name))
                    query.Where(x => x.Name.Contains(model.Name));
                if (!string.IsNullOrEmpty(model.Email))
                    query.Where(x => x.Email == model.Email.Trim());

                if (!string.IsNullOrEmpty(model.Phone))
                    query.Where(x => x.Phone == model.Phone.Trim());

                if (!string.IsNullOrEmpty(model.Name))
                    query.Where(x => x.Name.Contains(model.Name));
                if (model.CheckDate)
                {
                    query.Where(x => x.CreateDate >= _fdate && x.CreateDate <= _tdate);
                }
                //query.Where(x => x.Status == true);
                if (!comm.IsSuperAdmin()) query.Where(x => x.SysHotelID==comm.GetHotelId());
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
             rows=   rows.Where(e => (e.Name ?? "").Contains(page.search)).Skip(offset).Take(limit).ToList();

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


        public tbl_Customer GetCustomer(int id)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.Single<tbl_Customer>(x => x.Id == id);
                return query;
            }
        }


        public int updateCustome(CustomeDetail obj)
        {


            using (var db = _connectionData.OpenDbConnection())
            {

                using (var tran = db.OpenTransaction())
                {

                    DateTime DOB;

                    DateTime.TryParse(obj.DOB, CultureInfo.GetCultureInfo("vi-vn"), DateTimeStyles.None, out DOB);
                    try
                    {
                        var model = db.Single<tbl_Customer>(x => x.Id == obj.Id);
                        model.Name = obj.Name;
                        model.CountryId = obj.CountryId;
                        model.DOB = DOB;
                        model.Address = obj.Address;
                        model.Sex = obj.Sex;
                        model.TeamSTT = obj.TeamSTT;
                        model.TeamMergeSTT = obj.TeamMergeSTT;
                        model.Email = obj.Email;
                        model.IdentifyNumber = obj.IdentifyNumber;
                        model.Phone = obj.Phone;
                        model.Mobile = obj.Mobile;

                        model.Company = obj.Company;
                        //var g = db.Single<tbl_CustomerGroup>(x => x.Customer_ID == obj.Id);
                        //g.Doan_ID = obj.TeamSTT;
                        //g.Gop_Doan_ID = obj.TeamMergeSTT;
                        db.Update(model);
                        //db.Update(g);
                        tran.Commit();
                        return 1;
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                        return 0;
                    }
                }
            }

        }




        public List<HistoryCustomer> GetHistory(int id)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                /*
                var query = db.From<tbl_CustomerGroup>();
                query.Join<tbl_Reservation_Room, tbl_CustomerGroup>((x, y) => x.Id == y.ReserID);
                query.Join<tbl_CustomerGroup, tbl_Customer>((x, y) => x.Customer_ID == y.Id);
                query.Join<tbl_Reservation_Room, tbl_Room_Status>((x, y) => x.ReservationStatus == y.Id);
                query.Where<tbl_CustomerGroup>(x => x.Customer_ID == id);
                var rows = db.Select<HistoryCustomer>(query).ToList();
                return rows;
                */
                var query = db.From<tbl_Customer>();
                query.Join<tbl_Reservation_Customer_Rel, tbl_Customer>((x, y) => x.customerid == y.Id && x.customerid==id);
                query.Join<tbl_Reservation_Customer_Rel, tbl_Reservation_Room>((x, y) => x.reservationID == y.Id);
                var rows = db.Select<HistoryCustomer>(query).ToList();
                return rows;
            }
        }
        public List<HistoryCustomer> GetRoomMate(int customerid)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                /*
                var query = db.From<tbl_CustomerGroup>();
                query.Join<tbl_Reservation_Room, tbl_CustomerGroup>((x, y) => x.Id == y.ReserID);
                query.Join<tbl_CustomerGroup, tbl_Customer>((x, y) => x.Customer_ID == y.Id);
                query.Join<tbl_Reservation_Room, tbl_Room_Status>((x, y) => x.ReservationStatus == y.Id);
                query.Where<tbl_CustomerGroup>(x => x.Customer_ID == id);
                var rows = db.Select<HistoryCustomer>(query).ToList();
                return rows;
                */
                var tblCheckInUsing = db.Single<tbl_RoomUsing>(e => e.customerid == customerid);
                if (tblCheckInUsing == null) return new List<HistoryCustomer>();
                var query = db.From<tbl_Customer>();
                query.Join<tbl_RoomUsing, tbl_Customer>((x, y) => x.customerid == y.Id );
                query.Join<tbl_RoomUsing, tbl_CheckIn>((x, y) => x.CheckInID == y.Id && x.CheckInID == tblCheckInUsing.CheckInID);
                var rows = db.Select<HistoryCustomer>(query).ToList();
                return rows;
            }
        }

        public int deleteCustome(int id)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                try
                {
                    var obj = db.Single<tbl_Customer>(x => x.Id == id);
                    obj.Status = false;
                    db.Update(obj);
                    return id;
                }
                catch (Exception)
                {
                    return 0;
                    throw;
                }
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
    }
}