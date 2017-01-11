using oze.data;
using Oze.AppCode.BLL;
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
    public class SysUserService
    {
       IOzeConnectionFactory _connectionData;

        public  SysUserService()
        {
            OrmLiteConfig.DialectProvider = SqlServerDialect.Provider;
            OrmLiteConfig.DialectProvider.UseUnicode = true; // nhập tiếng việt

            _connectionData = new OzeConnectionFactory(ConfigurationManager.AppSettings["ConnectionString_OzeGeneral"], SqlServerOrmLiteDialectProvider.Instance);

        }
        public List<view_DetailUser> getAll(PagingModel page) 
        {
            if (page.search == null) page.search = "";

          //  ServiceStackHelper.Help();
          //  LicenseUtils.ActivatedLicenseFeatures();           
            //search again
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<view_DetailUser>();
                if (!comm.IsSuperAdmin()) query = query.Where(e => e.SysHotelID == comm.GetHotelId());
                query.OrderByDescending(x => x.ID);

                int offset = 0; try { offset = page.offset; }
                catch { }

                int limit = 10;//int.Parse(Request.Params["limit"]);
                try { limit = page.limit; }
                catch { }

                List<view_DetailUser> rows = db.Select(query)
                    .Where(e => (e.UserName ?? "").Contains(page.search))
                    .Skip(offset).Take(limit).ToList();
                return rows;                
            }
        }
        public long countAll(PagingModel page)
        {
            if (page.search == null) page.search = "";
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_SysUser>();
                if (!comm.IsSuperAdmin()) query = query.Where(e => e.SysHotelID == comm.GetHotelId());

                int offset = 0; try { offset = page.offset; }
                catch { }

                int limit = 10;//int.Parse(Request.Params["limit"]);
                try { limit = page.limit; }
                catch { }

                return db.Count(query.Where(e => e.UserName.Contains(page.search)));
            }        
        }

        public tbl_SysUser GetSysUserByID(string id)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_SysUser>().Where(e=>e.Id==int.Parse(id));
                return db.Select(query).SingleOrDefault();
            }     
        }
        public view_DetailUser GetViewUserByID(string id)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<view_DetailUser>().Where(e => e.ID == int.Parse(id));
                return db.Select(query).SingleOrDefault();
            }
        }
        public List<tbl_GroupType> GetAllGroupType()
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_GroupType>();
                return db.Select(query).ToList();
            }
        }
        public tbl_SysUser CheckLogin(string username,string password)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_SysUser>().Where(e => e.UserName == username && e.Password == MD5.md5(password));
                return db.Select(query).FirstOrDefault();
            }
        }
        public tbl_GroupType GetGroupTypeByUserID(int userid ,int hotelid)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query1 = db.From<tbl_SysUsers_GroupType_Hotel>().Where(e =>  e.hotelid == hotelid && e.userid == userid);
                var objUpdate1 = db.Single(query1);
                if(objUpdate1!=null)
                {
                    var query = db.From<tbl_GroupType>().Where(e => e.Id == objUpdate1.grouptypeid);
                    return db.Select(query).SingleOrDefault();
                }
                return null;
            }
        }
        public int UpdateOrInsertSysUser(view_DetailUser obj)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                //update
                if (obj.ID > 0)
                {

                    var query = db.From<tbl_SysUser>().Where(e => e.Id == obj.ID);
                    var objUpdate = db.Select(query).SingleOrDefault();
                    if (objUpdate != null)
                    {
                        int nx = 0;
                        using (var tran = db.OpenTransaction())//if need
                        {
                            //objUpdate.UserName = obj.UserName;
                            //objUpdate.Password = obj.Password;
                            objUpdate.IdentityNumber = obj.IdentityNumber;
                            objUpdate.FullName = obj.FullName;
                            objUpdate.Address = obj.Address;
                            objUpdate.Email = obj.Email;
                            objUpdate.Mobile = obj.Mobile;

                            objUpdate.SysHotelID = obj.SysHotelID;
                            objUpdate.Status = obj.Status;
                            objUpdate.IsActive = obj.IsActive;
                            


                            //objUpdate.FirstLogin = obj.FirstLogin;  
                            //objUpdate.Createby =comm.GetUserId();  
                            //objUpdate.CreateDate =DateTime.Now;  
                            objUpdate.Modifyby = comm.GetUserId();
                            nx= db.Update(objUpdate);

                            //delete for update
                            var query1 = db.From<tbl_SysUsers_GroupType_Hotel>().Where(e =>  e.hotelid == obj.SysHotelID && e.userid == obj.ID);
                            var objUpdate1 = db.Delete(query1);
                            
                            //insert again
                            var rightForHotel = new tbl_SysUsers_GroupType_Hotel();
                            rightForHotel.hotelid = objUpdate.SysHotelID;
                            rightForHotel.grouptypeid = obj.grouptypeid;
                            rightForHotel.userid = objUpdate.Id;

                            db.Insert(rightForHotel);
                            
                            tran.Commit();
                        }
                        return nx;
                    }
                    return -1;
                }
                //insert
                else 
                {
                    var queryCount = db.From<tbl_SysUser>().Where(e => e.UserName == obj.UserName && e.SysHotelID == comm.GetHotelId()).Select(e => e.Id);
                    var objCount = db.Count(queryCount);
                    if (objCount > 0) return comm.ERROR_EXIST;

                    int nx = 0;
                    using (var tran = db.OpenTransaction())//if need
                    {
                        tbl_SysUser obj1 = CloneFromView(obj);
                        obj1.Password = MD5.md5(obj.Password);
                        obj1.FirstLogin = obj.FirstLogin;
                        obj1.Createby = comm.GetUserId();
                        obj1.CreateDate = DateTime.Now;
                        obj1.Department = 0;
                        nx = (int)db.Insert(obj1, selectIdentity: true);

                        var query1 = db.From<tbl_SysUsers_GroupType_Hotel>().Where(e => e.grouptypeid == obj.grouptypeid && e.hotelid == obj.SysHotelID && e.userid == nx);
                        var objUpdate1 = db.Select(query1).SingleOrDefault();
                        if (objUpdate1 == null)
                        {
                            var rightForHotel = new tbl_SysUsers_GroupType_Hotel();
                            rightForHotel.hotelid = obj.SysHotelID;
                            rightForHotel.grouptypeid = obj.grouptypeid;
                            rightForHotel.userid = nx;

                            db.Insert(rightForHotel);
                        }
                        tran.Commit();
                    }
                    return nx;
                }
            }     
        }
        public int DeleteSysUser(int Id)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_SysUser>().Where(e => e.Id == Id);
                //var objUpdate = db.Select(query).SingleOrDefault();
                return db.Delete(query);
            }
        }
        public tbl_SysUser InitEmpty()
        {
            var objUpdate = new tbl_SysUser();
            objUpdate.UserName = "";
            objUpdate.Password = "";
            objUpdate.IdentityNumber = "";
            objUpdate.FullName = "";
            objUpdate.Address = "";
            objUpdate.Email = "";
            objUpdate.Mobile = "";

            objUpdate.SysHotelID = 0;
            objUpdate.Status = 1;
            objUpdate.IsActive = 1;

            return objUpdate;
        }
        public tbl_SysUser CloneFromView(view_DetailUser obj)
        {
            tbl_SysUser objUpdate = new tbl_SysUser();
            objUpdate = InitEmpty();
            objUpdate.UserName = obj.UserName;
            //objUpdate.Password = obj.Password;
            objUpdate.IdentityNumber = obj.IdentityNumber;
            objUpdate.FullName = obj.FullName;
            objUpdate.Address = obj.Address;
            objUpdate.Email = obj.Email;
            objUpdate.Mobile = obj.Mobile;

            objUpdate.SysHotelID = obj.SysHotelID;
            objUpdate.Status = obj.Status;
            objUpdate.IsActive = obj.IsActive;
            return objUpdate;
        }
    }
}