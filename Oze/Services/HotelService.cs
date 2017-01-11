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
    public class HotelService
    {
       IOzeConnectionFactory _connectionData;

        public  HotelService()
        {
            OrmLiteConfig.DialectProvider = SqlServerDialect.Provider;
            OrmLiteConfig.DialectProvider.UseUnicode = true; // nhập tiếng việt

            _connectionData = new OzeConnectionFactory(ConfigurationManager.AppSettings["ConnectionString_OzeGeneral"], SqlServerOrmLiteDialectProvider.Instance);

        }
        public List<tbl_Hotel> GetAll(PagingModel page) 
        {
            if (page == null) page = new PagingModel() { offset = 0, limit = 500 };
            if (page.search == null) page.search = "";

          //  ServiceStackHelper.Help();
          //  LicenseUtils.ActivatedLicenseFeatures();           
            //search again
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Hotel>();
                if (!comm.IsSuperAdmin()) query.Where(e => e.Id == comm.GetHotelId());

                int offset = 0; try { offset = page.offset; }
                catch { }

                int limit = 10;//int.Parse(Request.Params["limit"]);
                try { limit = page.limit; }
                catch { }

                query=query
                    .Where(e => (e.Name.Contains(page.search)))
                    .Skip(offset).Take(limit);
                 query.OrderByDescending(x => x.Id);

                List<tbl_Hotel> rows = db.Select(query).ToList();
                return rows;                
            }
        }

        public long countAll(PagingModel page)
        {
            if (page.search == null) page.search = "";
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Hotel>();
                if (!comm.IsSuperAdmin()) query.Where(e => e.Id == comm.GetHotelId());

                int offset = 0; try { offset = page.offset; }
                catch { }

                int limit = 10;//int.Parse(Request.Params["limit"]);
                try { limit = page.limit; }
                catch { }

                return db.Count(query.Where(e => e.Name.Contains(page.search)));
            }        
        }

        public tbl_Hotel GetHotelByID(string id)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Hotel>().Where(e=>e.Id==int.Parse(id));
                return db.Select(query).SingleOrDefault();
            }     
        }
        public tbl_Hotel GetHotelByCode(string code)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Hotel>().Where(e => e.Code == code);
                return db.Select(query).SingleOrDefault();
            }
        }
        public int UpdateOrInsertHotel(tbl_Hotel obj)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                if (obj.Id > 0)
                {

                    var query = db.From<tbl_Hotel>().Where(e => e.Id == obj.Id);
                    var objUpdate = db.Select(query).SingleOrDefault();
                    if (objUpdate != null)
                    {
                        objUpdate.Address = obj.Address;
                        //objUpdate.Code = obj.Code;
                        objUpdate.Name = obj.Name;
                        objUpdate.Code = obj.Code;
                        objUpdate.Address = obj.Address;
                        objUpdate.Email = obj.Email;
                        objUpdate.Mobile = obj.Mobile;
                        objUpdate.Phone = obj.Phone;

                        return db.Update(objUpdate);
                    }
                    return -1;
                }
                else 
                {
                    return (int)db.Insert(obj, selectIdentity: true);
                }
            }     
        }
        public int DeleteHotel(int Id)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Hotel>().Where(e => e.Id == Id);
                //var objUpdate = db.Select(query).SingleOrDefault();
                return db.Delete(query);
            }
        }
        public tbl_Hotel InitEmpty()
        {
            var obj = new tbl_Hotel();
            obj.Id = 0;
            obj.Address = "";
            obj.Code = "";
            obj.Name = "";
            obj.Mobile = "";
            obj.Phone = "";
            obj.Email = "";
            return obj;
        }
    }
}