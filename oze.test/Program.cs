using oze.data;
using ServiceStack;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace oze.test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press any to start...");
            Console.ReadLine();

            ServiceStackHelper.Help();
            LicenseUtils.ActivatedLicenseFeatures();
            

            IOzeConnectionFactory _connectionData;
            OrmLiteConfig.DialectProvider = SqlServerDialect.Provider;
            OrmLiteConfig.DialectProvider.UseUnicode = true; // nhập tiếng việt
            _connectionData = new OzeConnectionFactory(ConfigurationManager.ConnectionStrings["OzeConnectionString"].ConnectionString, SqlServerOrmLiteDialectProvider.Instance);
            // _unit = new UnitFunction();
            //_log = LogManager.GetLogger("ProviderManageService");


            //insert
            using (var db = _connectionData.OpenDbConnection())
            {
                try
                {
                    using (var tran = db.OpenTransaction())//if need
                    {
                        var obj = new tbl_Province();
                        obj.NamEN = "province 1";
                       

                        long id = db.Insert(obj,selectIdentity:true);

                        Console.WriteLine("Result of insert: " + id.ToString());
                        tran.Commit();
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("Insert Error: " + e.InnerException, e.Message, e.StackTrace);

                }
            }

            //search
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Province>();
                query.OrderByDescending(x => x.Id);
                var lstResult= db.Select(query).ToList();

                Console.WriteLine("Total " + lstResult.Count.ToString());
                foreach (var s in lstResult) 
                {
                    Console.WriteLine("Title "+s.NamEN);
                }
            }

      
            

            //update        
            using (var db = _connectionData.OpenDbConnection())
            {
                try
                {
                    string id = "4";//id need to update
                    var query = db.From<tbl_Province>().Where(e=>e.Id==id);
                    var provider = db.Select(query).FirstOrDefault();
                    provider.NamEN = "title update";
                    bool isOK= db.Save(provider,true);
                    Console.WriteLine("Result of update :" + isOK.ToString());
                   
                }
                catch (Exception e)
                {
                    Console.WriteLine("update error : " + e.InnerException);
                }
            }

            //search again
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Province>();
                query.OrderByDescending(x => x.Id);
                var lstResult = db.Select(query).ToList();

                Console.WriteLine("Total " + lstResult.Count.ToString());
                foreach (var s in lstResult)
                {
                    Console.WriteLine("Title " + s.NamEN);
                }
            }
            /*
            //store sp
            using (var db = _connectionData.OpenDbConnection())
            {
                OrmLiteSPStatement query = null;//SPList.sp_Hotels_GetAll(db);
                List<tbl_Hotel> lstResult = query.ConvertToList<tbl_Hotel>();

                // query.OrderByDescending(x => x.Id);
                //var lstResult = db.Select(query).ToList();

                Console.WriteLine("Total " + lstResult.Count.ToString());
                foreach (var s in lstResult)
                {
                    Console.WriteLine("Title " + s.Name);
                }
            }
             */

            Console.WriteLine("Press any to stop...");
            Console.ReadLine();
            Console.ReadLine();
         


        }
    }
}
