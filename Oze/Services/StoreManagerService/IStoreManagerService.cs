using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using oze.data;
using Oze.Models;
using Oze.Models.StoreModel;

namespace Oze.Services.StoreManagerService
{
    public interface IStoreManagerService
    {
        List<Vw_Store> Search(PagingModel page, SearchStoreModel model, out int total);
        List<tbl_Store> SearchMinibar(PagingModel page, out int total);
        List<tbl_Room> GetRoomByHotel();
        List<Vw_StoreConfig> GetProductAllByStore(int storeId);
        List<tbl_Product> GetProductByStore();
        tbl_Store GetItem(int storeId);
        int InsertOrUpdateStore(MinibarModel model);
        List<StoreModel> SearchAll(PagingModel page, string fromDate, string toDate, out int total);
        int DeleteStore(int id);
    }
}