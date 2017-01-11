using System;
using System.Collections.Generic;
using System.Web;
using Oze.AppCode.BLL;
using Oze.Models;
using System.Data;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Oze.AppCode.DAL
{
    public class CDatabaseNam
    {
        CSQLHelper helper = new CSQLHelper(CConfig.CONNECTIONSTRING_OZEGENERAL);
        List<MenusModel> getListMenu(int parentID, int status, DataSet list)
        {
            List<MenusModel> result = new List<MenusModel>();
            try
            {
                foreach (DataRow menu in list.Tables[0].Rows)
                {
                    int prID = Convert.ToInt32(menu["ParentID"].ToString().Trim());
                    int st = Convert.ToInt32(menu["Status"].ToString().Trim());
                    if (prID == parentID && st == status)
                    {
                        result.Add(new MenusModel
                        {
                            ID = prID,
                            Name = menu["Name"].ToString().Trim(),
                            Link = menu["Link"].ToString().Trim(),
                            Controller = menu["Controller"].ToString().Trim(),
                            Action = menu["Action"].ToString().Trim(),
                            ParentID = Convert.ToInt32(menu["ParentID"].ToString().Trim()),
                            Level = Convert.ToInt32(menu["Level"].ToString().Trim()),
                            Order = Convert.ToInt32(menu["Order"].ToString().Trim()),
                            ModuleName = menu["ModuleName"].ToString().Trim(),
                            Status = st,
                            Icon = menu["Icon"].ToString().Trim()
                        });
                    }
                }
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }
        DataSet getListMenuByUser(int UserID)
        {
            
            DataSet ds = new DataSet();
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@UserID", SqlDbType.Int);
                para[0].Direction = ParameterDirection.Input;
                para[0].Value = UserID;
                ds = helper.ExecuteQuery(CConfig.SP_MENU_GET_BY_USER, para, CommandType.StoredProcedure);
            }
            catch (Exception)
            {
                ds = null;
            }
            return ds;
        }

        public void getAllMenu()
        {
            string user = "";
            if (HttpContext.Current.Session[CConfig.SESSION_USERID] != null)
            {
                user = HttpContext.Current.Session[CConfig.SESSION_USERID].ToString().Trim();

                List<ParentMenu> list = new List<ParentMenu>();
                List<MenusModel> listMenu = new List<MenusModel>();
                List<MenusModel> listChild = new List<MenusModel>();
                DataSet ds = new DataSet();
                try
                {
                    if (user != null)
                    {
                        int userID = Convert.ToInt32(user);
                        ds = getListMenuByUser(userID);
                        listMenu = getListMenu(0, 1, ds);
                        {
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                List<MenusModel> dsChild = new List<MenusModel>();
                                dsChild = getListMenu(Convert.ToInt32(row["ID"].ToString()), 1, ds);

                                foreach (var rowChild in dsChild)
                                {

                                    listChild.Add(new MenusModel
                                    {
                                        ID = rowChild.ID,
                                        Name = rowChild.Name,
                                        Link = rowChild.Link,
                                        Controller = rowChild.Controller,
                                        Action = rowChild.Action,
                                        ParentID = rowChild.ParentID,
                                        Level = rowChild.Level,
                                        Order = rowChild.Order,
                                        ModuleName = rowChild.ModuleName,
                                        Status = rowChild.Status,
                                        Icon = rowChild.Icon

                                    });
                                }
                                if (dsChild.Count == 0)
                                {
                                    list.Add(new ParentMenu
                                    {
                                        ID = Convert.ToInt32(row["ID"].ToString()),
                                        Name = row["Name"].ToString().Trim(),
                                        Link = row["Link"].ToString().Trim(),
                                        Controller = row["Controller"].ToString().Trim(),
                                        Action = row["Action"].ToString().Trim(),
                                        ParentID = Convert.ToInt32(row["ParentID"].ToString()),
                                        Level = Convert.ToInt32(row["Level"].ToString()),
                                        Order = Convert.ToInt32(row["Order"].ToString()),
                                        ModuleName = row["ModuleName"].ToString().Trim(),
                                        Status = Convert.ToInt32(row["Status"].ToString()),
                                        Icon = row["Icon"].ToString().Trim()

                                    });
                                }
                                else
                                {
                                    list.Add(new ParentMenu
                                    {
                                        ID = Convert.ToInt32(row["ID"].ToString()),
                                        Name = row["Name"].ToString().Trim(),
                                        Link = row["Link"].ToString().Trim(),
                                        Controller = row["Controller"].ToString().Trim(),
                                        Action = row["Action"].ToString().Trim(),
                                        ParentID = Convert.ToInt32(row["ParentID"].ToString()),
                                        Level = Convert.ToInt32(row["Level"].ToString()),
                                        Order = Convert.ToInt32(row["Order"].ToString()),
                                        ModuleName = row["ModuleName"].ToString().Trim(),
                                        Status = Convert.ToInt32(row["Status"].ToString()),
                                        Icon = row["Icon"].ToString().Trim(),
                                        MenuViewModel = listChild
                                    });
                                }

                            }
                        }
                    }
                    //else {
                    //    HttpContext.Current.Response.Redirect("/Accounts/Login");
                    //}
                }
                catch (Exception ex)
                {
                    list = null;
                    throw ex;
                }
                HttpContext.Current.Session[CConfig.SESSION_MENU_LIST] = list;
            }
        }

        /*
         * 2016-11-09 11:52:53 NamLD
         * Get hotel return dataset
         */
        DataSet getHotels() {
            DataSet ds = new DataSet();
            try
            {
                
                ds = helper.ExecuteQuery(CConfig.SP_HOTELS_GET_LIST);
            }
            catch (Exception)
            {
                ds = null;
            }
            return ds;
        }
        /*
         * 2016-11-09 11:52:53 NamLD
         * Get hotel return list<Hotel>
         */
        public List<HotelListModel> getListHotels()
        {
            List<HotelListModel> result = new List<HotelListModel>();
            DataSet ds = new DataSet();
            try
            {
                Nullable<DateTime> dt = null;
                ds = getHotels();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    result.Add(new HotelListModel
                    {
                        ID = Convert.ToInt32(row["ID"].ToString().Trim()),
                        Code = row["Code"].ToString().Trim(),
                        Name = row["Name"].ToString().Trim(),
                        Address = row["Address"].ToString().Trim(),
                        Phone = row["Phone"].ToString().Trim(),
                        Mobile = row["Mobile"].ToString().Trim(),
                        Email = row["Email"].ToString().Trim(),
                        Website = row["Website"].ToString().Trim(),
                        LogoUrl = row["LogoUrl"].ToString().Trim(),
                        RoomCount = Convert.ToInt32(row["RoomCount"].ToString()),
                        Description = row["Description"].ToString().Trim(),
                        Status = Convert.ToInt32(row["Status"].ToString()) == 1? Status.Open:Status.Close,
                        CreateByUser = row["CreateByUser"].ToString().Trim(),
                        CreateDate = DateTime.Parse(row["CreateDate"].ToString()),
                        ModifyByUser = row["ModifyByUser"].ToString().Trim(),
                        ModifyDate = row["ModifyDate"].ToString().Trim() =="" ? dt : DateTime.Parse(row["ModifyDate"].ToString().Trim())
                    });
                }
            }
            catch (Exception ex)
            {
                result = null;
                throw ex;
            }
            return result;
        }
        /**
         * 2016-11-11 11:05:54 NamLD
         * insert hotel
         */
        public DataSet InsertHotel(HotelDefaultModel hotel) {
            string user = HttpContext.Current.Session[CConfig.SESSION_USERID].ToString().Trim();
            List<string> error = new List<string>();
            DataSet ds = new DataSet();
            try
            {

                DateTime date = DateTime.Now;
                SqlParameter[] para = new SqlParameter[11];

                para[0] = new SqlParameter("@Name", SqlDbType.NVarChar);
                para[0].Direction = ParameterDirection.Input;
                para[0].Value = hotel.Name.Trim();
                // validate
                //Validate.validStr(hotel.Name, 500, 0, "Code", false, error);
                para[1] = new SqlParameter("@Address", SqlDbType.NVarChar);
                para[1].Direction = ParameterDirection.Input;
                para[1].Value = hotel.Address == null ? "" : hotel.Address;

                para[2] = new SqlParameter("@Phone", SqlDbType.NChar);
                para[2].Direction = ParameterDirection.Input;
                para[2].Value = hotel.Phone == null? "" : hotel.Phone;
                // validate
                //Validate.validStrNumber(hotel.Phone, 10, 10, "Phone", true, error);

                para[3] = new SqlParameter("@Mobile", SqlDbType.NChar);
                para[3].Direction = ParameterDirection.Input;
                para[3].Value = hotel.Mobile == null ? "" : hotel.Mobile;
                // validate
                //Validate.validStrNumber(hotel.Mobile, 11, 10, "Mobile", true, error);

                para[4] = new SqlParameter("@Email", SqlDbType.NVarChar);
                para[4].Direction = ParameterDirection.Input;
                para[4].Value = hotel.Email == null ? "" : hotel.Email;
                // validate
                //Validate.validStrNumber(hotel.Email, 200, 0, "Email", false, error);

                para[5] = new SqlParameter("@Website", SqlDbType.NVarChar);
                para[5].Direction = ParameterDirection.Input;
                para[5].Value = hotel.Website == null ? "" : hotel.Website;

                para[6] = new SqlParameter("@LogoUrl", SqlDbType.NVarChar);
                para[6].Direction = ParameterDirection.Input;
                para[6].Value = hotel.LogoUrl == null ? "" : hotel.LogoUrl;

                para[7] = new SqlParameter("@RoomCount", SqlDbType.Int);
                para[7].Direction = ParameterDirection.Input;
                para[7].Value = hotel.RoomCount;
                // validate
                // Validate.validStrNumber(hotel.Email, 1000 , 1, "Email", false, error);

                para[8] = new SqlParameter("@Description", SqlDbType.NVarChar);
                para[8].Direction = ParameterDirection.Input;
                para[8].Value = hotel.Description == null ? "" : hotel.Description;

                para[9] = new SqlParameter("@Status", SqlDbType.Int);
                para[9].Direction = ParameterDirection.Input;
                para[9].Value = (int)hotel.Status;
                // validate
                //Validate.validEmail(hotel.Email, "Email", false, error);

                para[10] = new SqlParameter("@Createby", SqlDbType.Int);
                para[10].Direction = ParameterDirection.Input;
                para[10].Value = Int32.Parse(user);


               
                ds = helper.ExecuteQuery(CConfig.SP_HOTELS_INSERT, para, CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }
        /**
         * 2016-11-11 11:05:54 NamLD
         * insert hotel
         */
        public DataSet UpdateHotels(HotelDefaultModel hotel)
        {
            string user = HttpContext.Current.Session[CConfig.SESSION_USERID].ToString().Trim();
            List<string> error = new List<string>();
            DataSet ds = new DataSet();
            try
            {

                DateTime date = DateTime.Now;
                SqlParameter[] para = new SqlParameter[12];
                para[0] = new SqlParameter("@Code", SqlDbType.NVarChar);
                para[0].Direction = ParameterDirection.Input;
                para[0].Value = hotel.Code.Trim();

                para[1] = new SqlParameter("@Name", SqlDbType.NVarChar);
                para[1].Direction = ParameterDirection.Input;
                para[1].Value = hotel.Name.Trim();
                // validate
                //Validate.validStr(hotel.Name, 500, 0, "Code", false, error);
                para[2] = new SqlParameter("@Address", SqlDbType.NVarChar);
                para[2].Direction = ParameterDirection.Input;
                para[2].Value = hotel.Address == null ? "" : hotel.Address;

                para[3] = new SqlParameter("@Phone", SqlDbType.NChar);
                para[3].Direction = ParameterDirection.Input;
                para[3].Value = hotel.Phone == null ? (object)DBNull.Value : hotel.Phone;
                // validate
                //Validate.validStrNumber(hotel.Phone, 10, 10, "Phone", true, error);

                para[4] = new SqlParameter("@Mobile", SqlDbType.NChar);
                para[4].Direction = ParameterDirection.Input;
                para[4].Value = hotel.Mobile == null ? "" : hotel.Mobile;
                // validate
                //Validate.validStrNumber(hotel.Mobile, 11, 10, "Mobile", true, error);

                para[5] = new SqlParameter("@Email", SqlDbType.NVarChar);
                para[5].Direction = ParameterDirection.Input;
                para[5].Value = hotel.Email == null ? "" : hotel.Email;
                // validate
                //Validate.validStrNumber(hotel.Email, 200, 0, "Email", false, error);

                para[6] = new SqlParameter("@Website", SqlDbType.NVarChar);
                para[6].Direction = ParameterDirection.Input;
                para[6].Value = hotel.Website == null ? "" : hotel.Website;

                para[7] = new SqlParameter("@LogoUrl", SqlDbType.NVarChar);
                para[7].Direction = ParameterDirection.Input;
                para[7].Value = hotel.LogoUrl == null ? "" : hotel.LogoUrl;

                para[8] = new SqlParameter("@RoomCount", SqlDbType.Int);
                para[8].Direction = ParameterDirection.Input;
                para[8].Value = hotel.RoomCount;
                // validate
                // Validate.validStrNumber(hotel.Email, 1000 , 1, "Email", false, error);

                para[9] = new SqlParameter("@Description", SqlDbType.NVarChar);
                para[9].Direction = ParameterDirection.Input;
                para[9].Value = hotel.Description == null ? "" : hotel.Description;

                para[10] = new SqlParameter("@Status", SqlDbType.Int);
                para[10].Direction = ParameterDirection.Input;
                para[10].Value = (int)hotel.Status;
                // validate
                //Validate.validEmail(hotel.Email, "Email", false, error);

                para[11] = new SqlParameter("@Modifyby", SqlDbType.Int);
                para[11].Direction = ParameterDirection.Input;
                para[11].Value = Int32.Parse(user);



                ds = helper.ExecuteQuery(CConfig.SP_HOTELS_UPDATE, para, CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"> table can convert sang object</param>
        /// <returns></returns>
        public List<object> getObjectHotel(DataTable dt)
        {
            List<object> result = new List<object>();
            foreach (DataRow rw in dt.AsEnumerable())
            {
                result.Add( new {
                        Code = rw["Code"].ToString().Trim(),
                        Createby = rw["Createby"].ToString().Trim(),
                        CreateDate = String.Format("{0:dd/MM/yyyy HH:mm:ss}", rw["CreateDate"]),
                        Modifyby = rw["Modifyby"].ToString() == "" ? "" : rw["Modifyby"],
                        ModifyDate = rw["ModifyDate"].ToString() == "" ? "" : String.Format("{0:dd/MM/yyyy HH:mm:ss}", rw["ModifyDate"])
                    });
            }

            return result;
        }
        /// <summary>
        /// NamLD 11/22/2016 11:10:33
        /// Xoa khách sạn
        /// </summary>
        /// <param name="hotel">Mã Code khách sạn</param>
        /// <returns></returns>
        public DataSet DeleteHotels(string hotel) {
            DataSet ds = new DataSet();
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@Code", SqlDbType.NVarChar);
                para[0].Direction = ParameterDirection.Input;
                para[0].Value = hotel.Trim();

                ds = helper.ExecuteQuery(CConfig.SP_HOTELS_DELETE, para, CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }
    }

}