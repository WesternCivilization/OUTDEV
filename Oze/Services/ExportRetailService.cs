using System;
using System.Configuration;
using oze.data;
using Oze.AppCode.Util;
using Oze.Models;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;

namespace Oze.Services
{
    public class ExportRetailService
    {
        private IOzeConnectionFactory _connectionData;

        public ExportRetailService()
        {
            OrmLiteConfig.DialectProvider = SqlServerDialect.Provider;
            OrmLiteConfig.DialectProvider.UseUnicode = true; // nhập tiếng việt

            _connectionData = new OzeConnectionFactory(ConfigurationManager.AppSettings["ConnectionString_OzeGeneral"], SqlServerOrmLiteDialectProvider.Instance);
        }

        public tbl_SaleOrder InsertOrder(ExportRetailModel model)
        {
            try
            {
                using (var db = _connectionData.OpenDbConnection())
                {
                    var sysHotel = comm.GetHotelId();
                    #region Khách hàng

                    var sysCustomer = new tbl_Customer
                    {
                        Name = model.CustomerName,
                        Phone = model.CustomerPhone,
                        CreateDate = DateTime.Now,
                        Mobile = model.CustomerPhone,
                        SysHotelID = sysHotel,
                        CreateBy = comm.GetUserId(),
                        IdentifyNumber = model.CustomerPhone,
                        Status = true
                    };
                    var customerId = (int)db.Insert(sysCustomer, selectIdentity: true);
                    #endregion
                    #region Insert đơn
                    var objInsert = new tbl_SaleOrder
                    {
                        CustomerID = customerId,
                        SysHotelID = sysHotel,
                        AmountNoTax = model.TotalAmount,
                        Tax = model.SurchargeAmount,
                        TotalAmount = (model.TotalAmount - model.SurchargeAmount),
                        DateCreated = DateTime.Now,
                        DatePayment = DateTime.ParseExact(model.SPayTime, "dd/MM/yyyy hh:mm:ss", null),
                        PaymentTypeID = null,
                        TypeOrder = 1,
                        CreatorID = comm.GetUserId()
                    };
                    var iTblSaleOrderId = (int)db.Insert(objInsert, selectIdentity: true);
                    #endregion
                    #region Chi tiết đơn
                    if (model.ListRetailDetail != null)
                    {
                        foreach (var detail in model.ListRetailDetail)
                        {
                            var objDetail = new tbl_SaleOrderDetail
                            {
                                SysHotelID = sysHotel,
                                OrderID = iTblSaleOrderId,
                                catalogitem = detail.ProductName,
                                item = detail.ProductName,
                                itemid = detail.ProductId,
                                catalogitemid = detail.ProductId,
                                quantity = detail.Quantity,
                                Price = int.Parse(detail.Price + ""),
                                AmountNoTax = detail.Price,
                                Tax = 0,
                                TotalAmount = (detail.Price * detail.Quantity),
                                DateCreated = DateTime.Now,
                                DatePayment = DateTime.ParseExact(model.SPayTime, "dd/MM/yyyy hh:mm:ss", null),
                                TypeOrder = 1,
                                CreatorID = comm.GetUserId(),
                                StoreID = 0
                            };
                            db.Insert(objDetail, selectIdentity: true);
                        }

                    }
                    #endregion
                    return objInsert;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}