using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Transactions;
using System.Web;
using oze.data;
using Oze.Models;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;

namespace Oze.Services
{
    public class StoreService
    {
        IOzeConnectionFactory _connectionData;

        public StoreService()
        {
            OrmLiteConfig.DialectProvider = SqlServerDialect.Provider;
            OrmLiteConfig.DialectProvider.UseUnicode = true; // nhập tiếng việt

            _connectionData = new OzeConnectionFactory(ConfigurationManager.AppSettings["ConnectionString_OzeGeneral"],
                SqlServerOrmLiteDialectProvider.Instance);

        }

        public List<tbl_Store> GetMainStore()
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                return db.Select<tbl_Store>(p => p.SysHotelID == CommService.GetHotelId() && p.typeStore == 1);
            }
        }

        public List<tbl_Store> GetAllStore()
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                return db.Select<tbl_Store>(p => p.SysHotelID == CommService.GetHotelId());
            }
        }


        public List<tbl_ProductCate> GetAllCategories()
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                return db.Select<tbl_ProductCate>().ToList();
            }
        }

        public List<tbl_Product> GetallProducts()
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                return db.Select<tbl_Product>(p =>p.UnitID !=0 && p.SysHotelID == CommService.GetHotelId()).ToList();
            }
        }

        public List<tbl_Supplier> GetAllSupplier()
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                return db.Select<tbl_Supplier>(p => p.SysHotelID == CommService.GetHotelId()).ToList();
            }
        }

        public List<tbl_Unit> GetAllUnit()
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                return db.Select<tbl_Unit>(p => p.SysHotelID == CommService.GetHotelId()).ToList();
            }
        }

        public tbl_Product GetProduct(int productId)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                return db.Select<tbl_Product>(p => p.Id == productId).FirstOrDefault();
            }
        }
        #region xuat kho

        public bool Xuatkho(tbl_PurchaseOrder order, List<tbl_PurchaseOrderDetail> orderDetails, ref string msg)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                // using (var ts = new TransactionScope())
                var tran = db.OpenTransaction();
                {
                    try
                    {
                        order.Id = Convert.ToInt32(db.Insert(order, true));
                        foreach (var item in orderDetails)
                        {
                            item.PurchaseID = order.Id;
                        }
                        db.InsertAll(orderDetails);
                        //Cập nhật tồn kho:
                        //Group theo StoreId
                        var storeIds = orderDetails.Select(p => p.StoreID).Distinct().ToList();
                        foreach (var storeId in storeIds)
                        {
                            var orderByStore = orderDetails.Where(p => p.StoreID == storeId);
                            //Group theo sản phẩm
                            var productGroup = orderByStore.GroupBy(p => p.itemid, (key, g) => new
                            {
                                ProductId = key,
                                Total = g.Sum(p => p.quantity)
                            });
                            foreach (var item in productGroup)
                            {
                                var product =
                                       db.Select<tbl_Product>(
                                               p => p.SysHotelID == CommService.GetHotelId() && p.Id == item.ProductId)
                                           .FirstOrDefault();
                                var existItem =
                                    db.Select<tbl_StoreProduct>(
                                        p => p.productid == item.ProductId && p.storeid == storeId).FirstOrDefault();
                                if (existItem == null || existItem.quantity < item.Total)
                                {
                                    msg = "Tạo phiếu không thành công. Sản phẩm " + product.Name + " không đủ hàng";
                                    tran.Rollback();
                                    return false;
                                }
                                else
                                {
                                    var log = new tbl_StoreProductLog
                                    {
                                        typeImportExport = 2,
                                        SysHotelID = CommService.GetHotelId(),
                                        datecreated = DateTime.Now,
                                        storeid = storeId,
                                        productid = item.ProductId,
                                        storeproductid = existItem.Id,
                                        unitid = product.UnitID,
                                        quantity = item.Total,
                                    };
                                    db.Insert(log);
                                    existItem.quantity = existItem.quantity - item.Total;
                                    db.Update(existItem);
                                }
                            }
                        }
                        tran.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                        return false;
                    }
                }
            }
        }

        #endregion


        #region  StoreInput
        public List<Vw_PurchaseOrderDetail> GetAll(PagingModel page, DateTime fromDate, DateTime Todate, string keyword, int type, out int count)
        {
            if (page.search == null) page.search = "";

            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<Vw_PurchaseOrderDetail>().Where(p => p.SysHotelID == CommService.GetHotelId() && p.InputDate >= fromDate && p.InputDate <= Todate.AddDays(1).AddSeconds(-1) && p.TypeOrder == type);

                if (!string.IsNullOrEmpty(keyword))
                    query.Where(x => x.ProductCode.Contains(keyword) || x.ProductName.Contains(keyword) || x.SupplierName.Contains(keyword));

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
                rows = rows.Skip(offset).Take(limit).ToList();

                return rows;
            }
        }

        public Vw_PurchaseOrderDetail GetDetail(int id)
        {
            //_tdate = _tdate.AddDays(1);
            using (var db = _connectionData.OpenDbConnection())
            {
                return db.Select<Vw_PurchaseOrderDetail>().FirstOrDefault(p => p.Id == id);
            }
        }

        public string GetOrderCode()
        {
            try
            {


                using (var db = _connectionData.OpenDbConnection())
                {
                    var holtel = db.Select<tbl_Hotel>(p => p.Id == CommService.GetHotelId()).FirstOrDefault();
                    var order = db.Select<tbl_PurchaseOrder>().OrderByDescending(p => p.Id).FirstOrDefault();
                    var id = order != null ? order.Id + 1 : 1;
                    return holtel.Code + "NK" + id.ToString("000000000");
                }
            }
            catch (Exception)
            {

                return DateTime.Now.ToString("ddMMyyHHmmssfff");
            }
        }


        public string GetOutOrderCode()
        {
            try
            {


                using (var db = _connectionData.OpenDbConnection())
                {
                    var holtel = db.Select<tbl_Hotel>(p => p.Id == CommService.GetHotelId()).FirstOrDefault();
                    var order = db.Select<tbl_PurchaseOrder>().OrderByDescending(p => p.Id).FirstOrDefault();
                    var id = order != null ? order.Id + 1 : 1;
                    return holtel.Code + "XK" + id.ToString("000000000");
                }
            }
            catch (Exception)
            {

                return DateTime.Now.ToString("ddMMyyHHmmssfff");
            }
        }


        public string GetTransferCode()
        {
            try
            {


                using (var db = _connectionData.OpenDbConnection())
                {
                    var holtel = db.Select<tbl_Hotel>(p => p.Id == CommService.GetHotelId()).FirstOrDefault();
                    var order = db.Select<tbl_TransferOrder>().OrderByDescending(p => p.Id).FirstOrDefault();
                    var id = order != null ? order.Id + 1 : 1;
                    return holtel.Code + "CK" + id.ToString("000000000");
                }
            }
            catch (Exception)
            {

                return DateTime.Now.ToString("ddMMyyHHmmssfff");
            }
        }
        public bool StoreInput(tbl_PurchaseOrder order, List<tbl_PurchaseOrderDetail> orderDetails)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                // using (var ts = new TransactionScope())
                var tran = db.OpenTransaction();
                {
                    try
                    {
                        order.Id = Convert.ToInt32(db.Insert(order, true));
                        foreach (var item in orderDetails)
                        {
                            item.PurchaseID = order.Id;
                        }
                        db.InsertAll(orderDetails);
                        //Cập nhật tồn kho:
                        //Group theo StoreId
                        var storeIds = orderDetails.Select(p => p.StoreID).Distinct().ToList();
                        foreach (var storeId in storeIds)
                        {
                            var orderByStore = orderDetails.Where(p => p.StoreID == storeId);
                            //Group theo sản phẩm
                            var productGroup = orderByStore.GroupBy(p => p.itemid, (key, g) => new
                            {
                                ProductId = key,
                                Total = g.Sum(p => p.quantity)
                            });
                            foreach (var item in productGroup)
                            {
                                var product =
                                       db.Select<tbl_Product>(
                                               p => p.SysHotelID == CommService.GetHotelId() && p.Id == item.ProductId)
                                           .FirstOrDefault();

                                var existItem =
                                    db.Select<tbl_StoreProduct>(
                                        p => p.productid == item.ProductId && p.storeid == storeId).FirstOrDefault();
                                if (existItem == null)
                                {

                                    existItem = new tbl_StoreProduct
                                    {
                                        quantity = item.Total,
                                        SysHotelID = CommService.GetHotelId(),
                                        datecreated = DateTime.Now,
                                        productid = item.ProductId,
                                        unitid = product.UnitID,
                                        storeid = storeId
                                    };
                                    var id = db.Insert(existItem, true);
                                    //Thêm bản ghi Log
                                    var log = new tbl_StoreProductLog
                                    {
                                        typeImportExport = 1,
                                        SysHotelID = CommService.GetHotelId(),
                                        datecreated = DateTime.Now,
                                        storeid = storeId,
                                        productid = item.ProductId,
                                        storeproductid = Convert.ToInt32(id),
                                        unitid = product.UnitID,
                                        quantity = item.Total,
                                    };
                                    db.Insert(log);
                                }
                                else
                                {
                                    var log = new tbl_StoreProductLog
                                    {
                                        typeImportExport = 1,
                                        SysHotelID = CommService.GetHotelId(),
                                        datecreated = DateTime.Now,
                                        storeid = storeId,
                                        productid = item.ProductId,
                                        storeproductid = existItem.Id,
                                        unitid = product.UnitID,
                                        quantity = item.Total,
                                    };
                                    db.Insert(log);
                                    existItem.quantity = existItem.quantity + item.Total;
                                    db.Update(existItem);
                                }
                            }

                        }
                        tran.Commit();
                        //   ts.Complete();
                        return true;
                    }
                    catch (Exception)
                    {
                        tran.Rollback();
                        return false;
                    }
                }
            }
        }

        #endregion
        #region Storetransfer

        public bool StoreTransfer(tbl_TransferOrder order, List<tbl_TransferOrderDetail> orderDetails, ref string msg)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                using (var ts = db.OpenTransaction())
                {
                    try
                    {
                        order.Id = Convert.ToInt32(db.Insert(order, true));
                        foreach (var item in orderDetails)
                        {
                            item.TransferID = order.Id;
                        }
                        db.InsertAll(orderDetails);
                        //Cập nhật tồn kho:
                        //Group theo StoreId
                        var srcStoreId = orderDetails.FirstOrDefault().FromStoreId;
                        var storeId = orderDetails.FirstOrDefault().StoreID;
                        //  foreach (var storeId in storeIds)

                        var orderByStore = orderDetails.Where(p => p.StoreID == storeId);
                        //Group theo sản phẩm
                        var productGroup = orderByStore.GroupBy(p => p.itemid, (key, g) => new
                        {
                            ProductId = key,
                            Total = g.Sum(p => p.quantity)
                        });
                        foreach (var item in productGroup)
                        {
                            //trừ tồn kho Kho gốc
                            var srcStoreProduct = db.Select<tbl_StoreProduct>(
                                    p => p.productid == item.ProductId && p.storeid == srcStoreId).FirstOrDefault();

                            var product =
                                   db.Select<tbl_Product>(
                                           p => p.SysHotelID == CommService.GetHotelId() && p.Id == item.ProductId)
                                       .FirstOrDefault();

                            if (srcStoreProduct == null || srcStoreProduct.quantity < item.Total)
                            {
                                //  var product = 
                                msg = "Tạo phiếu lỗi. Sản phẩm " + product.Name + " không đủ số lượng trong kho nguồn";
                                ts.Rollback();
                                return false;
                            }
                            srcStoreProduct.quantity = srcStoreProduct.quantity - item.Total;
                            db.Update(srcStoreProduct);
                            var existItem =
                                db.Select<tbl_StoreProduct>(
                                    p => p.productid == item.ProductId && p.storeid == storeId).FirstOrDefault();
                            if (existItem == null)
                            {

                                existItem = new tbl_StoreProduct
                                {
                                    quantity = item.Total,
                                    SysHotelID = CommService.GetHotelId(),
                                    datecreated = DateTime.Now,
                                    productid = item.ProductId,
                                    unitid = product.UnitID,
                                    storeid = storeId
                                };
                                var id = db.Insert(existItem, true);
                                //Thêm bản ghi Log
                                var log = new tbl_StoreProductLog
                                {
                                    typeImportExport = 3,
                                    SysHotelID = CommService.GetHotelId(),
                                    datecreated = DateTime.Now,
                                    storeid = storeId,
                                    productid = item.ProductId,
                                    storeproductid = Convert.ToInt32(id),
                                    unitid = product.UnitID,
                                    quantity = item.Total,
                                    fromstoreid = srcStoreId
                                };
                                db.Insert(log);
                            }
                            else
                            {
                                var log = new tbl_StoreProductLog
                                {
                                    typeImportExport = 3,
                                    SysHotelID = CommService.GetHotelId(),
                                    datecreated = DateTime.Now,
                                    storeid = storeId,
                                    productid = item.ProductId,
                                    storeproductid = existItem.Id,
                                    unitid = product.UnitID,
                                    quantity = item.Total,
                                    fromstoreid = srcStoreId
                                };
                                db.Insert(log);
                                existItem.quantity = existItem.quantity + item.Total;
                                db.Update(existItem);
                            }
                        }


                        ts.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        ts.Rollback();
                        return false;
                    }
                }
            }
        }

        public List<Vw_TransferOrder> GetALlTransferOrders(PagingModel page, DateTime fromDate, DateTime Todate, string keyword, out int count)
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
                var query = db.From<Vw_TransferOrder>().Where(p => p.SysHotelID == CommService.GetHotelId() && p.InputDate >= fromDate && p.InputDate <= Todate);

                if (!string.IsNullOrEmpty(keyword))
                    query.Where(x => x.ProductCode.Contains(keyword) || x.ProductName.Contains(keyword) || x.SupplierName.Contains(keyword));

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
                rows = rows.Skip(offset).Take(limit).ToList();

                return rows;
            }
        }
        public Vw_TransferOrder GetOrderDetail(int id)
        {
            //_tdate = _tdate.AddDays(1);
            using (var db = _connectionData.OpenDbConnection())
            {
                return db.Select<Vw_TransferOrder>().FirstOrDefault(p => p.Id == id);
            }
        }

        #endregion

        #region bù định mức

        public List<tbl_StoreProduct> GetStoreProductList(int storeId)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query =
                    db.From<tbl_StoreProduct>()
                        .Join<tbl_Store>((p, q) => p.storeid == q.Id)
                        .Where<tbl_Store>(p => p.typeStore == 2 && p.SysHotelID == CommService.GetHotelId());
                if (storeId != 0)
                {
                    query = query.Where(p => p.storeid == storeId);
                }
                return db.Select(query);
            }
        }
        /// <summary>
        /// lấy cấu hình định mức
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public List<tbl_StoreProduct_Config> GetStoreProductListConfig(int storeId)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query =
                    db.From<tbl_StoreProduct_Config>()
                        .Join<tbl_Store>((p, q) => p.storeid == q.Id)
                        .Where<tbl_Store>(p => p.typeStore == 2 && p.SysHotelID == CommService.GetHotelId());
                if (storeId != 0)
                {
                    query = query.Where(p => p.storeid == storeId);
                }
                return db.Select(query);
            }
        }
        public bool StoreTransfer(tbl_TransferOrder order, List<tbl_TransferOrderDetail> orderDetails)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                using (var ts = db.OpenTransaction())
                {
                    try
                    {
                        order.Id = Convert.ToInt32(db.Insert(order, true));
                        foreach (var item in orderDetails)
                        {
                            item.TransferID = order.Id;
                        }
                        var srcStoreId = orderDetails.FirstOrDefault().FromStoreId;
                        foreach (var item in orderDetails)
                        {
                            var product =
                                      db.Select<tbl_Product>(
                                              p => p.SysHotelID == CommService.GetHotelId() && p.Id == item.itemid)
                                          .FirstOrDefault();
                            var srcStoreProduct = db.Select<tbl_StoreProduct>(
                                    p => p.productid == item.itemid && p.storeid == srcStoreId).FirstOrDefault();
                            if (srcStoreProduct == null || srcStoreProduct.quantity == 0)
                            {
                                continue;
                            }

                            item.quantity = srcStoreProduct.quantity > item.quantity
                                ? item.quantity
                                : srcStoreProduct.quantity;

                            db.Insert(item);
                            srcStoreProduct.quantity = srcStoreProduct.quantity - item.quantity;
                            db.Update(srcStoreProduct);
                            var existItem =
                                db.Select<tbl_StoreProduct>(
                                    p => p.productid == item.itemid && p.storeid == item.StoreID).FirstOrDefault();
                            if (existItem == null)
                            {

                                existItem = new tbl_StoreProduct
                                {
                                    quantity = item.quantity,
                                    SysHotelID = CommService.GetHotelId(),
                                    datecreated = DateTime.Now,
                                    productid = item.itemid,
                                    unitid = product.UnitID,
                                    storeid = item.StoreID
                                };
                                var id = db.Insert(existItem, true);
                                //Thêm bản ghi Log
                                var log = new tbl_StoreProductLog
                                {
                                    typeImportExport = 3,
                                    SysHotelID = CommService.GetHotelId(),
                                    datecreated = DateTime.Now,
                                    storeid = item.StoreID,
                                    productid = item.itemid,
                                    storeproductid = Convert.ToInt32(id),
                                    unitid = product.UnitID,
                                    quantity = item.quantity,
                                    fromstoreid = srcStoreId
                                };
                                db.Insert(log);
                            }
                            else
                            {
                                var log = new tbl_StoreProductLog
                                {
                                    typeImportExport = 3,
                                    SysHotelID = CommService.GetHotelId(),
                                    datecreated = DateTime.Now,
                                    storeid = item.StoreID,
                                    productid = item.itemid,
                                    storeproductid = existItem.Id,
                                    unitid = product.UnitID,
                                    quantity = item.quantity,
                                    fromstoreid = srcStoreId
                                };
                                db.Insert(log);
                                existItem.quantity = existItem.quantity + item.quantity;
                                db.Update(existItem);
                            }
                        }
                        ts.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        ts.Rollback();
                        return false;
                    }
                }
            }
        }



        #endregion
    }

    public class ServiceBase
    {
        IOzeConnectionFactory _connectionData;

        public ServiceBase()
        {
            OrmLiteConfig.DialectProvider = SqlServerDialect.Provider;
            OrmLiteConfig.DialectProvider.UseUnicode = true; // nhập tiếng việt

            _connectionData = new OzeConnectionFactory(ConfigurationManager.AppSettings["ConnectionString_OzeGeneral"],
                SqlServerOrmLiteDialectProvider.Instance);

        }

    }
}