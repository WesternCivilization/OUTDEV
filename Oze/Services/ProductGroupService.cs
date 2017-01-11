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
    public class ProductGroupService
    {
       IOzeConnectionFactory _connectionData;

        public  ProductGroupService()
        {
            OrmLiteConfig.DialectProvider = SqlServerDialect.Provider;
            OrmLiteConfig.DialectProvider.UseUnicode = true; // nhập tiếng việt

            _connectionData = new OzeConnectionFactory(ConfigurationManager.AppSettings["ConnectionString_OzeGeneral"], SqlServerOrmLiteDialectProvider.Instance);

        }
        public List<tbl_ProductGroup> getAll(PagingModel page) 
        {
            if (page==null) page= new PagingModel(){offset=0,limit=100};
            if (page.search == null) page.search = "";

          //  ServiceStackHelper.Help();
          //  LicenseUtils.ActivatedLicenseFeatures();           
            //search again
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_ProductGroup>();
                if (!comm.IsSuperAdmin()) query = query.Where(e => e.SysHotelID == comm.GetHotelId());
                query.OrderByDescending(x => x.Id);

                int offset = 0; try { offset = page.offset; }
                catch { }

                int limit = 10;//int.Parse(Request.Params["limit"]);
                try { limit = page.limit; }
                catch { }

                List<tbl_ProductGroup> rows = db.Select(query)
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
                var query = db.From<tbl_ProductGroup>();
                if (!comm.IsSuperAdmin()) query = query.Where(e => e.SysHotelID == comm.GetHotelId());

                int offset = 0; try { offset = page.offset; }
                catch { }

                int limit = 10;//int.Parse(Request.Params["limit"]);
                try { limit = page.limit; }
                catch { }

                return db.Count(query.Where(e => e.Name.Contains(page.search)));
            }        
        }

        public tbl_ProductGroup GetProductGroupByID(string id)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_ProductGroup>().Where(e=>e.Id==int.Parse(id));
                return db.Select(query).SingleOrDefault();
            }     
        }
        public int UpdateOrInsertProductGroup(tbl_ProductGroup obj)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                if (obj.Id > 0)
                {

                    var query = db.From<tbl_ProductGroup>().Where(e => e.Id == obj.Id);
                    var objUpdate = db.Select(query).SingleOrDefault();
                    if (objUpdate != null)
                    {
                        //bjUpdate.Code = obj.Code;
                        objUpdate.Name = obj.Name;
                        objUpdate.Status = obj.Status;
                        objUpdate.CreateDate = DateTime.Now;
                        objUpdate.Createby = comm.GetUserId();
                        objUpdate.Description = obj.Description;
                        return db.Update(objUpdate);
                    }
                    return comm.ERROR_GENERAL;
                }
                else 
                {
                    var queryCount = db.From<tbl_ProductGroup>().Where(e => e.Name == obj.Name && e.SysHotelID == comm.GetHotelId()).Select(e => e.Id);
                    var objCount = db.Count(queryCount);
                    if (objCount > 0) return comm.ERROR_EXIST;

                    obj.SysHotelID = comm.GetHotelId();
                    return (int)db.Insert(obj, selectIdentity: true);
                }
            }     
        }
        public int DeleteProductGroup(int Id)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_ProductGroup>().Where(e => e.Id == Id);
                //var objUpdate = db.Select(query).SingleOrDefault();
                return db.Delete(query);
            }
        }
        public tbl_ProductGroup InitEmpty()
        {
            var obj = new tbl_ProductGroup();
            obj.Id = 0;
            obj.Name= "";
            obj.Description = "";
            obj.Status = 1;
            return obj;
        }
    }
}