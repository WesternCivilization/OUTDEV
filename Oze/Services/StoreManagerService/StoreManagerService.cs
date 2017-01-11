using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using oze.data;
using Oze.AppCode.Util;
using Oze.Models;
using Oze.Models.StoreModel;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;

namespace Oze.Services.StoreManagerService
{
    public class StoreManagerService : IStoreManagerService
    {
        private readonly IOzeConnectionFactory _connectionData;

        public StoreManagerService()
        {
            OrmLiteConfig.DialectProvider = SqlServerDialect.Provider;
            OrmLiteConfig.DialectProvider.UseUnicode = true; // nhập tiếng việt
            _connectionData = new OzeConnectionFactory(ConfigurationManager.AppSettings["ConnectionString_OzeGeneral"], SqlServerOrmLiteDialectProvider.Instance);

        }

        public List<Vw_Store> Search(PagingModel page, SearchStoreModel search, out int total)
        {
            if (page == null) page = new PagingModel { offset = 0, limit = 100 };
            if (page.search == null) page.search = "";
            using (var db = _connectionData.OpenDbConnection())
            {
                List<Vw_Store> lstProductStore = new List<Vw_Store>();
                if (search.StoreId == 0)
                {
                    var query = db.From<Vw_Store>();
                    if (!comm.IsSuperAdmin()) query = query.Where(e => e.SysHotelID == comm.GetHotelId());

                    DateTime fDateTime;
                    DateTime tDateTime;
                    //if (!string.IsNullOrEmpty(search.FromDate) && !string.IsNullOrEmpty(search.ToDate))
                    //{
                    //    fDateTime = CommService.ConvertStringToDate(search.FromDate);
                    //    tDateTime = CommService.ConvertStringToDate(search.ToDate);
                    //    query = query.Where(x => x.CreateDate >= fDateTime && x.CreateDate <= tDateTime);
                    //}
                    query.OrderByDescending(x => x.ProductGroupID);
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
                    lstProductStore = lst;
                }
                else 
                {
                    var query = db.From<Vw_Store1>();
                    if (!comm.IsSuperAdmin()) query = query.Where(e => e.SysHotelID == comm.GetHotelId());
                    query.Where(e => e.storeid == search.StoreId);
                    DateTime fDateTime;
                    DateTime tDateTime;
                    //if (!string.IsNullOrEmpty(search.FromDate) && !string.IsNullOrEmpty(search.ToDate))
                    //{
                    //    fDateTime = CommService.ConvertStringToDate(search.FromDate);
                    //    tDateTime = CommService.ConvertStringToDate(search.ToDate);
                    //    query = query.Where(x => x.CreateDate >= fDateTime && x.CreateDate <= tDateTime);
                    //}
                    query.OrderByDescending(x => x.ProductGroupID);
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
                    foreach (var obj in lst) 
                    {
                        var obj1 = new Vw_Store();
                        obj1.Code = obj.Code;
                        obj1.Createby = obj.Createby;
                        obj1.CreateDate = obj.CreateDate;
                        obj1.DateOrder = obj.DateOrder;
                        obj1.Description = obj.Description;
                        obj1.GroupName = obj.GroupName;
                        obj1.ID = obj.ID;
                        obj1.Modifyby = obj.Modifyby;
                        obj1.ModifyDate = obj.ModifyDate;
                        obj1.Name = obj.Name;
                        obj1.ProductGroupID = obj.ProductGroupID;
                        obj1.QuotaMinimize = obj.QuotaMinimize;
                        obj1.SalePrice = obj.SalePrice;
                        obj1.Status = obj.Status;
                        obj1.SupplierID = obj.SupplierID;
                        obj1.SupplierName = obj.SupplierName;
                        obj1.SysHotelID = obj.SysHotelID;
                        obj1.TonKho = obj.TonKho;
                        obj1.UnitID = obj.UnitID;
                        obj1.UnitName = obj.UnitName;
                        obj1.XuatKho = obj.XuatKho;
                        lstProductStore.Add(obj1);
                    }
                    //clone to that
                    //lstProductStore = lst;
                }                    
                return lstProductStore;
            }
        }

        public List<tbl_Store> SearchMinibar(PagingModel page, out int total)
        {
            if (page == null) page = new PagingModel() { offset = 0, limit = 100 };
            if (page.search == null) page.search = "";
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Store>();
                query = query.Where(x => x.typeStore == 2);
                if (!comm.IsSuperAdmin()) query = query.Where(e => e.SysHotelID == comm.GetHotelId());
                query.OrderByDescending(x => x.Id);
                if (!string.IsNullOrEmpty(page.search))
                {
                    query = query.Where(x => x.title.Contains(page.search.Trim()));
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

        public List<tbl_Room> GetRoomByHotel()
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Room>().Where(e => e.SysHotelID == comm.GetHotelId());
                var lst = db.Select(query);
                return lst;
            }
        }

        public List<Vw_StoreConfig> GetProductAllByStore(int storeId)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                //các sản phẩm thuộc kho
                var query = db.From<Vw_StoreConfig>().Where(e => e.SysHotelID == comm.GetHotelId() && e.UnitID != 0);
                if (storeId != 0)
                {
                    query = query.Where(x => (x.storeid??0) == storeId);
                }
                var lst = db.Select(query);

                //các sản phẩm ko thuộc kho này
                var query1 = db.From<Vw_StoreConfig>().Where(e => e.SysHotelID == comm.GetHotelId() && e.UnitID != 0);
                if (storeId != 0)
                {
                    query1 = query1.Where(x => (x.storeid ?? 0) != storeId);
                }
                for (int jx = 0; jx <= lst.Count - 1; jx++)
                {
                    lst[jx].Status = 1;
                }
                //đánh dấu các chú đã thuộc rồi
                var lst1= db.Select(query1);
                for (int jx = 0; jx <= lst1.Count - 1;jx++ )
                {
                    if (lst.Where(e => e.Name == lst1[jx].Name).Count() == 0) lst.Add(lst1[jx]);
                }
                return lst;
            }
        }
        public List<Vw_Store_Pro> GetProductAllByStore_old(int storeId)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                //các sản phẩm thuộc kho
                var query = db.From<Vw_Store_Pro>().Where(e => e.SysHotelID == comm.GetHotelId() && e.UnitID != 0);
                if (storeId != 0)
                {
                    query = query.Where(x => x.StoreId == storeId);
                }
                var lst = db.Select(query);

                //các sản phẩm ko thuộc kho này
                var query1 = db.From<Vw_Store_Pro>().Where(e => e.SysHotelID == comm.GetHotelId() && e.UnitID != 0);
                if (storeId != 0)
                {
                    query1 = query1.Where(x => (x.StoreId ?? 0) != storeId);
                }
                for (int jx = 0; jx <= lst.Count - 1; jx++)
                {
                    lst[jx].Status = 1;
                }
                //đánh dấu các chú đã thuộc rồi
                var lst1 = db.Select(query1);
                for (int jx = 0; jx <= lst1.Count - 1; jx++)
                {
                    if (lst.Where(e => e.Name == lst1[jx].Name).Count() == 0) lst.Add(lst1[jx]);
                }
                return lst;
            }
        }
        public List<tbl_Product> GetProductByStore()
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Product>().Where(e => e.SysHotelID == comm.GetHotelId() && e.UnitID != 0);
                var lst = db.Select(query);
                return lst;
            }
        }

        public tbl_Store GetItem(int storeId)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Store>().Where(e => e.Id == storeId);
                return db.Select(query).SingleOrDefault();
            }
        }

        public int InsertOrUpdateStore(MinibarModel model)
        {
            try
            {
                using (var db = _connectionData.OpenDbConnection())
                {
                    using (var tran = db.OpenTransaction())
                    {
                        try
                        {
                            if (model.Id > 0)
                            {
                                var query = db.From<tbl_Store>().Where(e => e.Id == model.Id);
                                var up = db.Select(query).SingleOrDefault();
                                if (up != null)//update store
                                {
                                    up.title = model.Name;
                                    up.roomid = model.RoomId;
                                    db.Update(up);
                                }
                                var strDel = db.From<tbl_StoreProduct_Config>().Where(e => e.storeid == model.Id);
                                var del = db.Select(strDel);
                                if (del.Any())
                                {
                                    db.DeleteAll(del);
                                }
                                if (model.Item.Any())//update store product config
                                {
                                    var list = model.Item.Select(item => new tbl_StoreProduct_Config
                                    {
                                        productid = item.ProductId,
                                        minimize = item.Limit,
                                        SysHotelID = comm.GetHotelId(),
                                        datecreated = DateTime.Now,
                                        storeid = model.Id
                                    });
                                    db.InsertAll(list);
                                }
                                tran.Commit();
                                return 1;

                            }
                            //Insert-------------------------
                            var check = db.From<tbl_Store>().Where(e => e.roomid == model.RoomId);
                            var isExit = db.Select(check).FirstOrDefault();
                            if (isExit == null) //Nếu roomID tồn tại rồi thì k cho insert nữa
                            {
                                var store = new tbl_Store
                                {
                                    title = model.Name,
                                    roomid = model.RoomId,
                                    creatorid = comm.GetUserId(),
                                    SysHotelID = comm.GetHotelId(),
                                    datecreated = DateTime.Now,
                                    typeStore = 2
                                };
                                var idStore = db.Insert(store, true);//insert product
                                if (model.Item.Any())//insert store product
                                {
                                    var list = model.Item.Select(item => new tbl_StoreProduct_Config
                                    {
                                        productid = item.ProductId,
                                        minimize = item.Limit,
                                        SysHotelID = comm.GetHotelId(),
                                        datecreated = DateTime.Now,
                                        storeid = int.Parse(idStore.ToString())
                                    });
                                    db.InsertAll(list);
                                    /*
                                    var list = model.Item.Select(item => new tbl_StoreProduct
                                    {
                                        productid = item.ProductId,
                                        quantity = item.Limit,
                                        SysHotelID = comm.GetHotelId(),
                                        datecreated = DateTime.Now,
                                        storeid = int.Parse(idStore.ToString())
                                    });
                                    db.InsertAll(list);
                                     */
                                }
                                tran.Commit();
                                return 1;
                            }
                            return -100;
                        }
                        catch (Exception)
                        {
                            tran.Rollback();
                            return -1;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }
        public int InsertOrUpdateStoreGeneral(tbl_Store store)
        {
            try
            {
                using (var db = _connectionData.OpenDbConnection())
                {
                    return (int)db.Insert(store, selectIdentity: true);
                }
            }
            catch (Exception)
            {
                return -1;
            }

        }
        public List<StoreModel> SearchAll(PagingModel page, string fromDate, string toDate, out int total)
        {
            if (page == null) page = new PagingModel { offset = 0, limit = 100 };
            if (page.search == null) page.search = "";
            var model = new List<StoreModel>();
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<Vw_Store>();
                //anh co the viet ntn. em chua biet. truoc hay sau phan tran anh nhỉ.
                // tat nhien la sau han trang riu 
                //vi du 
                // cai group no nam o dau
                if (!comm.IsSuperAdmin()) query = query.Where(e => e.SysHotelID == comm.GetHotelId());
                query.OrderByDescending(x => x.ID);
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
                //cho nay ra lít da phan anhaj

                //var test = lst.FirstOrDefault();
                //test.//goi face nc anh nhe ok
                var group = lst.Select(x => new { x.ProductGroupID, x.GroupName }).GroupBy(x => x).ToList();
                // model.AddRange(group.Select(x=>new StoreModel {GroupId = x.Key.ProductGroupID.Value,GroupName = x.Key.GroupName}).ToList());
                // get item theo grupId
                foreach (var item in group)
                {
                    var gr = new StoreModel
                    {
                        GroupId = item.Key.ProductGroupID.Value,
                        GroupName = item.Key.GroupName
                    };
                    gr.ListItemStores.AddRange(lst.Where(x => x.ProductGroupID == item.Key.ProductGroupID));
                    model.Add(gr);

                }
                return model;
            }
        }

        public int DeleteStore(int id)
        {
            try
            {
                using (var db = _connectionData.OpenDbConnection())
                {
                    using (var tran = db.OpenTransaction())
                    {
                        try
                        {
                            var rs = -1;
                            var query = db.From<tbl_Store>().Where(e => e.Id == id);
                            var item = db.Select(query).FirstOrDefault();
                            if (item != null)
                            {
                                /*
                                //xoa storeProduct
                                var strdel = db.From<tbl_StoreProduct>().Where(e => e.storeid == item.Id);
                                var lstDel = db.Select(strdel);
                                db.DeleteAll(lstDel);
                                 */
                                var strdel = db.From<tbl_StoreProduct_Config>().Where(e => e.storeid == item.Id);
                                var lstDel = db.Select(strdel);
                                db.DeleteAll(lstDel);
                                
                                //xoa Store
                                rs = db.Delete(item);
                            }
                            tran.Commit();
                            return rs;
                        }
                        catch (Exception)
                        {
                            tran.Rollback();
                            return -1;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}