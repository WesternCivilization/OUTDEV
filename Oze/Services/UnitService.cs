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
    public class UnitService
    {
       IOzeConnectionFactory _connectionData;

        public  UnitService()
        {
            OrmLiteConfig.DialectProvider = SqlServerDialect.Provider;
            OrmLiteConfig.DialectProvider.UseUnicode = true; // nhập tiếng việt

            _connectionData = new OzeConnectionFactory(ConfigurationManager.AppSettings["ConnectionString_OzeGeneral"], SqlServerOrmLiteDialectProvider.Instance);

        }
        public List<tbl_Unit> getAll(PagingModel page) 
        {
            if (page==null) page= new PagingModel(){offset=0,limit=100};
            if (page.search == null) page.search = "";

          //  ServiceStackHelper.Help();
          //  LicenseUtils.ActivatedLicenseFeatures();           
            //search again
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Unit>();
                if (!comm.IsSuperAdmin()) query = query.Where(e => e.SysHotelID == comm.GetHotelId());
                query.OrderByDescending(x => x.Id);

                int offset = 0; try { offset = page.offset; }
                catch { }

                int limit = 10;//int.Parse(Request.Params["limit"]);
                try { limit = page.limit; }
                catch { }

                List<tbl_Unit> rows = db.Select(query)
                    .Where(e => (e.Name1 ?? "").Contains(page.search))
                    .Skip(offset).Take(limit).ToList();
                return rows;                
            }
        }
        public long countAll(PagingModel page)
        {
            if (page.search == null) page.search = "";
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Unit>();
                if (!comm.IsSuperAdmin()) query = query.Where(e => e.SysHotelID == comm.GetHotelId());

                int offset = 0; try { offset = page.offset; }
                catch { }

                int limit = 10;//int.Parse(Request.Params["limit"]);
                try { limit = page.limit; }
                catch { }

                return db.Count(query.Where(e => e.Name1.Contains(page.search)));
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
        public int UpdateOrInsertUnit(tbl_Unit obj)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                if (obj.Id > 0)
                {

                    var query = db.From<tbl_Unit>().Where(e => e.Id == obj.Id);
                    var objUpdate = db.Select(query).SingleOrDefault();
                    if (objUpdate != null)
                    {
                        //bjUpdate.Code = obj.Code;
                        objUpdate.Name1 = obj.Name1;
                        objUpdate.Status = obj.Status;
                        objUpdate.Value = obj.Value;
                        objUpdate.CreateDate = DateTime.Now;
                        objUpdate.Createby = comm.GetUserId();
                        objUpdate.Description = obj.Description;
                    
                        return db.Update(objUpdate);
                    }
                    return -1;
                }
                else 
                {
                    var queryCount = db.From<tbl_Unit>().Where(e => e.Name1 == obj.Name1 && e.SysHotelID == comm.GetHotelId()).Select(e => e.Id);
                    var objCount = db.Count(queryCount);
                    if (objCount > 0) return comm.ERROR_EXIST;

                    obj.SysHotelID = comm.GetHotelId();
                    return (int)db.Insert(obj, selectIdentity: true);
                }
            }     
        }
        public int DeleteUnit(int Id)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Unit>().Where(e => e.Id == Id);
                //var objUpdate = db.Select(query).SingleOrDefault();
                return db.Delete(query);
            }
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
    }
}