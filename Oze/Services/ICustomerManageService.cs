using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using oze.data;
using Oze.Models;
using Oze.Models.CustomerManage;

namespace Oze.Services
{
    interface ICustomerManageService
    {
        List<tbl_Customer> getAll(PagingModel page, SearchCustomerModel model,out int count);
        List<tbl_Country> getAllCountry();
        tbl_Customer GetCustomer(int id);
        oze.data.Entity.Vw_InforCustomer_Room GetCustomerRoom(int id);
        List<HistoryCustomer> GetHistory(int id);
        List<HistoryCustomer> GetRoomMate(int customerid);
        int updateCustome(CustomeDetail obj);
        int deleteCustome(int id);
    }
}
