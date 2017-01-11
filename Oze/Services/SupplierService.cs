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
    public class SupplierService
    {
       IOzeConnectionFactory _connectionData;

        public  SupplierService()
        {
            OrmLiteConfig.DialectProvider = SqlServerDialect.Provider;
            OrmLiteConfig.DialectProvider.UseUnicode = true; // nhập tiếng việt

            _connectionData = new OzeConnectionFactory(ConfigurationManager.AppSettings["ConnectionString_OzeGeneral"], SqlServerOrmLiteDialectProvider.Instance);

        }
        public List<tbl_Supplier> getAll(PagingModel page) 
        {
            if (page == null) page = new PagingModel() { offset = 0, limit = 100 };
            if (page.search == null) page.search = "";

          //  ServiceStackHelper.Help();
          //  LicenseUtils.ActivatedLicenseFeatures();           
            //search again
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Supplier>();
                if (!comm.IsSuperAdmin()) query = query.Where(e => e.SysHotelID == comm.GetHotelId());
                query.OrderByDescending(x => x.Id);

                int offset = 0; try { offset = page.offset; }
                catch { }

                int limit = 10;//int.Parse(Request.Params["limit"]);
                try { limit = page.limit; }
                catch { }

                List<tbl_Supplier> rows = db.Select(query)
                    .Where(e => (e.Name ?? "").Contains(page.search))
                    .Skip(offset).Take(limit).ToList();
                return rows;                
            }
        }
        public long countAll(PagingModel page)
        {
            if (page.search == null) page.search = "";
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Supplier>();
                if (!comm.IsSuperAdmin()) query = query.Where(e => e.SysHotelID == comm.GetHotelId());
                int offset = 0; try { offset = page.offset; }
                catch { }

                int limit = 10;//int.Parse(Request.Params["limit"]);
                try { limit = page.limit; }
                catch { }

                return db.Count(query.Where(e => e.Name.Contains(page.search)));
            }        
        }

        public tbl_Supplier GetSupplierByID(string id)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Supplier>().Where(e=>e.Id==int.Parse(id));
                return db.Select(query).SingleOrDefault();
            }     
        }
        public int UpdateOrInsertSupplier(tbl_Supplier obj)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                if (obj.Id > 0)
                {

                    var query = db.From<tbl_Supplier>().Where(e => e.Id == obj.Id);
                    var objUpdate = db.Select(query).SingleOrDefault();
                    if (objUpdate != null)
                    {
                        objUpdate.Address = obj.Address;
                        //objUpdate.Code = obj.Code;
                        objUpdate.Name = obj.Name;
                        objUpdate.Contact_Person_Email = obj.Contact_Person_Email;
                        objUpdate.Contact_Person_Mobile = obj.Contact_Person_Mobile;
                        objUpdate.Contact_Person_Name = obj.Contact_Person_Name;
                        objUpdate.Contact_Person_Phone = obj.Contact_Person_Phone;
                        objUpdate.Email = obj.Email;

                        return db.Update(objUpdate);
                    }
                    return -1;
                }
                else 
                {
                    var queryCount = db.From<tbl_Supplier>().Where(e => e.Name == obj.Name && e.SysHotelID == comm.GetHotelId()).Select(e => e.Id);
                    var objCount = db.Count(queryCount);
                    if (objCount > 0) return comm.ERROR_EXIST;
                    obj.SysHotelID = comm.GetHotelId();
                    return (int)db.Insert(obj, selectIdentity: true);
                }
            }     
        }
        public int DeleteSupplier(int Id)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Supplier>().Where(e => e.Id == Id);
                //var objUpdate = db.Select(query).SingleOrDefault();
                return db.Delete(query);
            }
        }
        public tbl_Supplier InitEmpty()
        {
            var obj = new tbl_Supplier();
            obj.Id = 0;
            obj.Address = "";
            obj.Code = "";
            obj.Contact_Person_Email = "";
            obj.Contact_Person_Mobile = "";
            obj.Contact_Person_Name = "";
            obj.Contact_Person_Phone = "";
            obj.Email = "";
            obj.Code = (new CommService()).GetSupplierCode();
            return obj;
        }
    }
}