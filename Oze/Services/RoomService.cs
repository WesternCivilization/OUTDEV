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
    public class RoomService
    {
       IOzeConnectionFactory _connectionData;

        public  RoomService()
        {
            OrmLiteConfig.DialectProvider = SqlServerDialect.Provider;
            OrmLiteConfig.DialectProvider.UseUnicode = true; // nhập tiếng việt

            _connectionData = new OzeConnectionFactory(ConfigurationManager.AppSettings["ConnectionString_OzeGeneral"], SqlServerOrmLiteDialectProvider.Instance);

        }
        public List<tbl_Room> getAll(PagingModel page) 
        {
            if (page==null) page= new PagingModel(){offset=0,limit=100};
            if (page.search == null) page.search = "";

          //  ServiceStackHelper.Help();
          //  LicenseUtils.ActivatedLicenseFeatures();           
            //search again
            using (var db = _connectionData.OpenDbConnection())
            {
                int hotelid=comm.GetHotelId();
                var query = db.From<tbl_Room>();
                if(!comm.IsSuperAdmin()) query=query.Where(e => e.SysHotelID == hotelid);
                query.OrderByDescending(x => x.Name);
                int offset = 0; try { offset = page.offset; }
                catch { }

                int limit = 10;//int.Parse(Request.Params["limit"]);
                try { limit = page.limit; }
                catch { }

                List<tbl_Room> rows = db.Select(query)
                    .Where(e => (e.Name ?? "").Contains(page.search)).OrderBy(e=>e.Name)
                    .Skip(offset).Take(limit).ToList();
                return rows;                
            }
        }
        public List<Vw_SoDoPhong> getAllSoDoPhong(string search)
        {
            if (search == null) search = "";

            //  ServiceStackHelper.Help();
            //  LicenseUtils.ActivatedLicenseFeatures();   
            List<Vw_SoDoPhong> listSoDoPhong = new List<Vw_SoDoPhong>();
        
            //search again
            using (var db = _connectionData.OpenDbConnection())
            {
                int hotelid = comm.GetHotelId();
                var query = db.From<tbl_Room>();
                if (!comm.IsSuperAdmin()) query = query.Where(e => e.SysHotelID == hotelid);
                List<tbl_Room> rows = db.Select(query).OrderBy(e => e.Name).ToList<tbl_Room>();

                List<Vw_SoDoPhong> lstSoDoPhong = db.Select(db.From<Vw_SoDoPhong>().Where(e=>e.SysHotelID==hotelid));
                foreach (var room in rows) 
                {
                    Vw_SoDoPhong rowSoDoPhong =// db.Select(db.From<Vw_SoDoPhong>())
                        lstSoDoPhong.Where(e => e.id == room.Id)
                        .OrderByDescending(e => e.idusing).OrderByDescending(e=>(e.payer))
                        .FirstOrDefault();
                    listSoDoPhong.Add(rowSoDoPhong);
                }
            }
            return listSoDoPhong;
        }
        public long countAll(PagingModel page)
        {
            if (page.search == null) page.search = "";
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Room>();              

                int offset = 0; try { offset = page.offset; }
                catch { }

                int limit = 10;//int.Parse(Request.Params["limit"]);
                try { limit = page.limit; }
                catch { }

                return db.Count(query.Where(e => e.Name.Contains(page.search)));
            }        
        }

        public tbl_Room GetRoomByID(string id)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Room>().Where(e=>e.Id==int.Parse(id));
                return db.Select(query).SingleOrDefault();
            }     
        }
        public int UpdateOrInsertRoom(tbl_Room obj)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                if (obj.Id > 0)
                {


                    var query = db.From<tbl_Room>().Where(e => e.Id == obj.Id);
                    var objUpdate = db.Select(query).SingleOrDefault();
                    if (objUpdate != null)
                    {
                        //bjUpdate.Code = obj.Code;
                        objUpdate.Name = obj.Name;
                        objUpdate.Floor = obj.Floor;
                        objUpdate.RoomType_ID = obj.RoomType_ID;

                        return db.Update(objUpdate);
                    }
                    return -1;
                }
                else 
                {
                    var queryCount = db.From<tbl_Room>().Where(e => e.Name == obj.Name && e.SysHotelID == comm.GetHotelId()).Select(e=>e.Id);
                    var objCount = db.Count(queryCount);
                    if (objCount > 0) return comm.ERROR_EXIST;
                    obj.SysHotelID = comm.GetHotelId();
                    //obj.RoomType_ID = obj.RoomType_ID;

                    return (int)db.Insert(obj, selectIdentity: true);
                }
            }     
        }
        public int UpdateStatusRoom(int id,string status)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Room>().Where(e => e.Id == id);
                var objUpdate = db.Select(query).SingleOrDefault();
                if (objUpdate != null)
                {
                    objUpdate.status = int.Parse(status);
                    return db.Update(objUpdate);
                }
                return -1;
            }
        }
        public int UpdateStatusRoom1(int id, string status1)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Room>().Where(e => e.Id == id);
                var objUpdate = db.Select(query).SingleOrDefault();
                if (objUpdate != null)
                {
                    objUpdate.status1 = int.Parse(status1);
                    return db.Update(objUpdate);
                }
                return -1;
            }
        }
        public int DeleteRoom(int Id)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Room>().Where(e => e.Id == Id);
                //var objUpdate = db.Select(query).SingleOrDefault();
                return db.Delete(query);
            }
        }
        public tbl_Room InitEmpty()
        {
            var obj = new tbl_Room();
            obj.Id = 0;
            obj.Name = "";
            obj.Floor = "1";           
            return obj;
        }
    }
}