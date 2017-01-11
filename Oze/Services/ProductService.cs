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
    public class ProductService
    {
       IOzeConnectionFactory _connectionData;

        public  ProductService()
        {
            OrmLiteConfig.DialectProvider = SqlServerDialect.Provider;
            OrmLiteConfig.DialectProvider.UseUnicode = true; // nhập tiếng việt

            _connectionData = new OzeConnectionFactory(ConfigurationManager.AppSettings["ConnectionString_OzeGeneral"], SqlServerOrmLiteDialectProvider.Instance);

        }
        public List<view_DetailProduct> getAll(PagingModel page) 
        {
            if (page.search == null) page.search = "";

          //  ServiceStackHelper.Help();
          //  LicenseUtils.ActivatedLicenseFeatures();           
            //search again
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<view_DetailProduct>();
                if (!comm.IsSuperAdmin()) query.Where(e => e.SysHotelID == comm.GetHotelId());
                query.OrderByDescending(x => x.ID);

                int offset = 0; try { offset = page.offset; }
                catch { }

                int limit = 10;//int.Parse(Request.Params["limit"]);
                try { limit = page.limit; }
                catch { }

                List<view_DetailProduct> rows = db.Select(query)
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
                var query = db.From<tbl_Product>();
                if (!comm.IsSuperAdmin()) query.Where(e => e.SysHotelID == comm.GetHotelId());

                int offset = 0; try { offset = page.offset; }
                catch { }

                int limit = 10;//int.Parse(Request.Params["limit"]);
                try { limit = page.limit; }
                catch { }

                return db.Count(query.Where(e => e.Name.Contains(page.search)));
            }        
        }

        public tbl_Product GetProductByID(string id)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Product>().Where(e=>e.Id==int.Parse(id));
                return db.Select(query).SingleOrDefault();
            }     
        }
        public view_DetailProduct GetViewProductByID(string id)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<view_DetailProduct>().Where(e => e.ID == int.Parse(id));
                return db.Select(query).SingleOrDefault();
            }
        }
        public int UpdateOrInsertProduct(view_DetailProduct objView)
        {
            tbl_Product obj = CloneFromView(objView);
            using (var db = _connectionData.OpenDbConnection())
            {
                if (obj.Id > 0)
                {

                    var query = db.From<tbl_Product>().Where(e => e.Id == obj.Id);
                    var objUpdate = db.Select(query).SingleOrDefault();
                    if (objUpdate != null)
                    {
                        objUpdate.Name = obj.Name;
                        objUpdate.Code = obj.Code;
                        objUpdate.Description = obj.Description;
                        objUpdate.PriceOrder = obj.PriceOrder;

                        objUpdate.SalePrice = obj.SalePrice;
                        objUpdate.Status = obj.Status;
                        objUpdate.SupplierID = obj.SupplierID;
                        objUpdate.UnitID = obj.UnitID;

                        objUpdate.QuotaMinimize = obj.QuotaMinimize;
                        objUpdate.ProductGroupID = obj.ProductGroupID;

                        
                        return db.Update(objUpdate);
                    }
                    return -1;
                }
                else 
                {
                    var queryCount = db.From<tbl_Unit>().Where(e => e.Name1 == obj.Name && e.SysHotelID == comm.GetHotelId()).Select(e => e.Id);
                    var objCount = db.Count(queryCount);
                    if (objCount > 0) return comm.ERROR_EXIST;

                    obj.SysHotelID = comm.GetHotelId();
                    return (int)db.Insert(obj, selectIdentity: true);
                }
            }     
        }
        public int DeleteProduct(int Id)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Product>().Where(e => e.Id == Id);
                //var objUpdate = db.Select(query).SingleOrDefault();
                return db.Delete(query);
            }
        }
        public tbl_Product InitEmpty()
        {
            var obj = new tbl_Product();
            obj.Id = 0;
            obj.Name = "";
            obj.SysHotelID = -1;
            obj.PictureUrl = "";
            obj.PriceOrder= 0;
            obj.SalePrice = 0;
            obj.Status = 1;
            obj.SupplierID =0;
            obj.SysHotelID = 0;
            obj.QuotaMinimize = 0;

            obj.ProductGroupID = 0;
            obj.Code = (new CommService()).GetProductCode();
            return obj;
        }
        public tbl_Product CloneFromView(view_DetailProduct objView)
        {
            var obj = InitEmpty();
            obj.Id = objView.ID;
            obj.Name = objView.Name;
            obj.Code = objView.Code;

            obj.SysHotelID = objView.SysHotelID;
            obj.PictureUrl = objView.PictureUrl;
            obj.PriceOrder = objView.PriceOrder;
            obj.SalePrice = objView.SalePrice;
            obj.Status = objView.Status;
            obj.SysHotelID = objView.SysHotelID;
            obj.UnitID = objView.UnitID;
            obj.Description = objView.Description;
            obj.SupplierID = objView.SupplierID;
            obj.ProductGroupID = objView.ProductGroupID;
            obj.QuotaMinimize = objView.QuotaMinimize;

            return obj;
        }
    }
}