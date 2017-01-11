using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using oze.data;
using Oze.AppCode.Util;
using Oze.Models;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;

namespace Oze.Services.RoomLevelService
{
    public class RoomLevelService : IRoomLevelService
    {
        private readonly IOzeConnectionFactory _connectionData;

        public RoomLevelService()
        {
            OrmLiteConfig.DialectProvider = SqlServerDialect.Provider;
            OrmLiteConfig.DialectProvider.UseUnicode = true; // nhập tiếng việt

            _connectionData = new OzeConnectionFactory(ConfigurationManager.AppSettings["ConnectionString_OzeGeneral"], SqlServerOrmLiteDialectProvider.Instance);

        }
        public List<tbl_Room_Type> Search(PagingModel page, out int total)
        {
            if (page == null) page = new PagingModel() { offset = 0, limit = 100 };
            if (page.search == null) page.search = "";
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Room_Type>();
                if (!comm.IsSuperAdmin()) query = query.Where(e => e.HotelID == comm.GetHotelId());
                query.OrderByDescending(x => x.Id);
                if (!string.IsNullOrEmpty(page.search))
                {
                    query = query.Where(x => x.Code.Contains(page.search.Trim()) || x.Name.Contains(page.search.Trim()));
                }
                var count = query.ToCountStatement();
                total = db.Select<int>(count).FirstOrDefault();
                var offset = 0; try { offset = page.offset; }
                catch
                {
                    // ignored
                }

                int limit = 10;//int.Parse(Request.Params["limit"]);
                try { limit = page.limit; }
                catch
                {
                    // ignored
                }

                var lst = db.Select(query).Skip(offset).Take(limit).ToList();
                return lst;
            }
        }

        public tbl_Room_Type GetRoomTypeById(string id)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Room_Type>().Where(e => e.Id == int.Parse(id));
                return db.Select(query).SingleOrDefault();
            }
        }

        public int UpdateOrInsertRoomType(tbl_Room_Type obj)
        {
            try
            {
                using (var db = _connectionData.OpenDbConnection())
                {
                    if (obj.Id > 0)
                    {

                        var query = db.From<tbl_Room_Type>().Where(e => e.Id == obj.Id);
                        var objUpdate = db.Select(query).SingleOrDefault();
                        if (objUpdate != null)
                        {
                            //objUpdate.Code = obj.Code;
                            objUpdate.Name = obj.Name;
                            objUpdate.Note = obj.Note;

                            //objUpdate.HotelID =comm.GetHotelId();
                            return db.Update(objUpdate);
                        }
                        return -1;
                    }
                    var rd = new Random();
                    var id = rd.Next(1, 10);
                    var code = comm.GetHotelCode().Trim() + id;
                    obj.Code = code;
                    obj.HotelID = comm.GetHotelId();
                    return (int)db.Insert(obj, true);
                }
            }
            catch (Exception)
            {
                return -1;
            }

        }

        public int DeleteRoomLevel(int id)
        {
            try
            {
                using (var db = _connectionData.OpenDbConnection())
                {
                    var query = db.From<tbl_Room_Type>().Where(e => e.Id == id);
                    var item = db.Select(query).FirstOrDefault();
                    return db.Delete(item);
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}