using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using oze.data;
using Oze.Models;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;

namespace Oze.Services
{
    public class RepostService
    {
        IOzeConnectionFactory _connectionData;

        public RepostService()
        {
            OrmLiteConfig.DialectProvider = SqlServerDialect.Provider;
            OrmLiteConfig.DialectProvider.UseUnicode = true; // nhập tiếng việt

            _connectionData = new OzeConnectionFactory(ConfigurationManager.AppSettings["ConnectionString_OzeGeneral"],
                SqlServerOrmLiteDialectProvider.Instance);

        }

        public List<Vw_Report_TienPhong> BaoCaoTienPhong(PagingModel page, DateTime fromDate, DateTime Todate, string keyword, out int count, out double total)
        {
            if (page.search == null) page.search = "";

            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<Vw_Report_TienPhong>().Where(p => p.SysHotelID == CommService.GetHotelId() && p.DateCreated >= fromDate && p.DateCreated <= Todate.AddDays(1).AddSeconds(-1));

                if (!string.IsNullOrEmpty(keyword))
                    query.Where(x => x.Customername.Contains(keyword));

                query.OrderByDescending(x => x.id);


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
                total = rows.Sum(p => p.TotalAmount ?? 0);
                rows = rows.Skip(offset).Take(limit).ToList();
                return rows;
            }
        }
    }
}