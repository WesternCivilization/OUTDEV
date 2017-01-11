using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Oze.AppCode.BLL;
using Oze.Models;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Data.OleDb;
using System.Net.Mail;
using System.IO;
using System.Configuration;

namespace Oze.AppCode.DAL
{
    public class CDatabaseLam
    {
        CSQLHelper helper = new CSQLHelper(CConfig.CONNECTIONSTRING_OZEGENERAL);
        //CSQLHelper connmember = new CSQLHelper(CConfig.CONNECTIONSTRING_OZMEMBER);
        public static string SendMKEmail = ConfigurationManager.AppSettings["SendMKEmail"].ToString();
        /// <summary>
        /// Register Account login System
        /// </summary>
        /// <param name="Group"></param>
        /// <returns></returns>
        #region Register Account login System
        public void AccountRegister(RegisterModel Register, ref int retcode, ref string retmesg)
        {
            try
            {
                SqlParameter[] para = new SqlParameter[15];
                para[0] = new SqlParameter("@UserName", SqlDbType.NVarChar, 20);
                para[0].Direction = ParameterDirection.Input;
                para[0].Value = Register.UserName;
                para[1] = new SqlParameter("@Password", SqlDbType.NVarChar, 50);
                para[1].Direction = ParameterDirection.Input;
                para[1].Value = Register.Password;
                para[2] = new SqlParameter("@IdentityNumber", SqlDbType.NVarChar, 20);
                para[2].Direction = ParameterDirection.Input;
                para[2].Value = Register.IdentityNumber;
                para[3] = new SqlParameter("@FullName", SqlDbType.NVarChar, 100);
                para[3].Direction = ParameterDirection.Input;
                para[3].Value = Register.FullName;
                para[4] = new SqlParameter("@Address", SqlDbType.NVarChar, 200);
                para[4].Direction = ParameterDirection.Input;
                para[4].Value = Register.Address;
                para[5] = new SqlParameter("@Email", SqlDbType.NVarChar, 100);
                para[5].Direction = ParameterDirection.Input;
                para[5].Value = Register.Email;
                para[6] = new SqlParameter("@Mobile", SqlDbType.NVarChar, 11);
                para[6].Direction = ParameterDirection.Input;
                para[6].Value = Register.Mobile;
                para[7] = new SqlParameter("@Department", SqlDbType.Int);
                para[7].Direction = ParameterDirection.Input;
                para[7].Value = Register.Department;
                para[8] = new SqlParameter("@ParentID", SqlDbType.Int);
                para[8].Direction = ParameterDirection.Input;
                para[8].Value = Register.ParentID;
                para[9] = new SqlParameter("@SysHotelID", SqlDbType.Int);
                para[9].Direction = ParameterDirection.Input;
                para[9].Value = Register.SysHotelID;
                para[10] = new SqlParameter("@Status", SqlDbType.Int);
                para[10].Direction = ParameterDirection.Input;
                para[10].Value = Register.Status;
                para[11] = new SqlParameter("@IsActive", SqlDbType.Int);
                para[11].Direction = ParameterDirection.Input;
                para[11].Value = Register.IsActive;
                para[12] = new SqlParameter("@Createby", SqlDbType.Int);
                para[12].Direction = ParameterDirection.Input;
                para[12].Value = Register.Createby;
                para[13] = new SqlParameter("@RetCode", SqlDbType.Int);
                para[13].Direction = ParameterDirection.Output;
                para[13].Value = Register.RetCode;
                para[14] = new SqlParameter("@RetMesg", SqlDbType.NVarChar, 500);
                para[14].Direction = ParameterDirection.Output;
                para[14].Value = Register.RetMesg;

                helper.ExecuteQuery(CConfig.SP_ACCOUNT_REGISTER, para, CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        /// <summary>
        /// Check username, pw and login system
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <param name="RetCode"></param>
        /// <param name="RetMesg"></param>
        /// <returns></returns>
        #region Check username, pw and login system
        public void AccountLogin(LoginModel login, ref int retcode, ref string retmesg)
        {
            try
            {
                SqlParameter[] para = new SqlParameter[4];
                para[0] = new SqlParameter("@UserName", SqlDbType.NVarChar, 20);
                para[0].Direction = ParameterDirection.Input;
                para[0].Value = login.UserName;
                para[1] = new SqlParameter("@Password", SqlDbType.NVarChar, 50);
                para[1].Direction = ParameterDirection.Input;
                para[1].Value = login.Password;
                para[2] = new SqlParameter("@RetCode", SqlDbType.Int);
                para[2].Direction = ParameterDirection.Output;
                para[2].Value = DBNull.Value;
                para[3] = new SqlParameter("@RetMesg", SqlDbType.NVarChar, 500);
                para[3].Direction = ParameterDirection.Output;
                para[3].Value = DBNull.Value;

                helper.ExecuteQuery(CConfig.SP_ACCOUNT_LOGIN, para, CommandType.StoredProcedure);

                retcode = Int32.Parse(para[2].Value.ToString());
                retmesg = para[3].Value.ToString();
                return;
            }
            catch (Exception ex)
            {
                retcode = -1;
                retmesg = ex.Message;
                return;
            }

        }
        #endregion

        /// <summary>
        /// Get CustomerInfo
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        #region Get CustomerInfo
        public SysUserModel AccountGetInfomation(LoginModel mdlogin, int UserID)
        {
            SysUserModel AccInfo = new SysUserModel();
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] para = new SqlParameter[3];
                para[0] = new SqlParameter("@UserID", SqlDbType.Int);
                para[0].Direction = ParameterDirection.Input;
                para[0].Value = UserID;
                para[1] = new SqlParameter("@UserName", SqlDbType.NVarChar);
                para[1].Direction = ParameterDirection.Input;
                para[1].Value = mdlogin.UserName;
                para[2] = new SqlParameter("@Email", SqlDbType.NVarChar);
                para[2].Direction = ParameterDirection.Input;
                para[2].Value = mdlogin.Email;

                ds = helper.ExecuteQuery(CConfig.SP_ACCOUNT_GET_INFO, para, CommandType.StoredProcedure);
                dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    AccInfo = new SysUserModel
                    {
                        ID = Convert.ToInt32(dt.Rows[0]["ID"].ToString()),
                        UserName = dt.Rows[0]["UserName"].ToString(),
                        Password = dt.Rows[0]["Password"].ToString(),
                        IdentityNumber = dt.Rows[0]["IdentityNumber"].ToString(),
                        FullName = dt.Rows[0]["FullName"].ToString(),
                        Address = dt.Rows[0]["Address"].ToString(),
                        Email = dt.Rows[0]["Email"].ToString(),
                        Mobile = dt.Rows[0]["Mobile"].ToString(),
                        Department = Convert.ToInt32(dt.Rows[0]["Department"].ToString()),
                        ParentID = Convert.ToInt32(dt.Rows[0]["ParentID"].ToString()),
                        SysHotelID = Convert.ToInt32(dt.Rows[0]["SysHotelID"].ToString()),
                        Status = Convert.ToInt32(dt.Rows[0]["Status"].ToString()),
                        IsActive = Convert.ToInt32(dt.Rows[0]["IsActive"].ToString()),
                        FirstLogin = Convert.ToInt32(dt.Rows[0]["FirstLogin"].ToString()),
                        CodeSysHotel = dt.Rows[0]["CodeSysHotel"].ToString(),
                        NameSysHotel = dt.Rows[0]["NameSysHotel"].ToString(),
                        HotelsGroupCode = dt.Rows[0]["HotelsGroupCode"].ToString(),
                    };
                }
                else
                    AccInfo = null;
            }
            catch (Exception ex)
            {
                AccInfo = null;
            }
            return AccInfo;
        }
        #endregion

        /// <summary>
        /// Remove All current session
        /// Use for sign out
        /// </summary>
        /// <returns></returns>
        #region Remove All current session
        public void CurrentSessionRemove()
        {
            HttpContext.Current.Session.Remove(CConfig.SESSION_USERID);
            HttpContext.Current.Session.Remove(CConfig.SESSION_USERNAME);
            HttpContext.Current.Session.Remove(CConfig.SESSION_FULLNAME);
            HttpContext.Current.Session.Remove(CConfig.SESSION_HOTELCODE);
            HttpContext.Current.Session.Remove(CConfig.SESSION_HOTELNAME);
        }
        #endregion

        /// <summary>
        /// Import data from excel to sql
        /// </summary>
        /// <returns></returns>
        #region Import data from excel to sql
        OleDbConnection excelConnection;
        public void ImportDataFromExcel(ref string mesg)
        {
            try
            {
                string _sheetname = ""; //"Wards";
                if (HttpContext.Current.Request.Files["FileUpload1"].ContentLength > 0)
                {
                    string extension = System.IO.Path.GetExtension(HttpContext.Current.Request.Files["FileUpload1"].FileName);
                    string path1 = string.Format("{0}/{1}", HttpContext.Current.Server.MapPath("~/Content/UploadedFolder"), HttpContext.Current.Request.Files["FileUpload1"].FileName);
                    if (System.IO.File.Exists(path1))
                        System.IO.File.Delete(path1);

                    HttpContext.Current.Request.Files["FileUpload1"].SaveAs(path1);

                    //SQL Server Connection String
                    //string sqlConnectionString = @"Data Source=.;UID=sa;PWD=sa;Database=DB-BookStore;Trusted_Connection=true;Persist Security Info=True";
                    string sqlConnectionString = CConfig.CONNECTIONSTRING_OZEGENERAL;

                    //A 32-bit provider which enables the use of
                    string excelConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path1 + ";Extended Properties=Excel 12.0;Persist Security Info=False";

                    //Create Connection String to Excel work book
                    excelConnection = new OleDbConnection(excelConnectionString);

                    //Open Connection String excel
                    excelConnection.Open();

                    //Get all sheetname of excel file 
                    DataTable dtsheet = new DataTable();
                    dtsheet = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                    for (int i = 0; i < dtsheet.Rows.Count; i++)
                    {
                        _sheetname = dtsheet.Rows[i]["Table_Name"].ToString();

                        //Create OleDbCommand to fetch data from Excel
                        OleDbCommand cmd = new OleDbCommand("Select * from [" + _sheetname + "]", excelConnection);

                        OleDbDataReader dReader;
                        dReader = cmd.ExecuteReader();

                        SqlBulkCopy sqlBulk = new SqlBulkCopy(sqlConnectionString);
                        //Give your Destination table name
                        sqlBulk.DestinationTableName = "tbl_" + _sheetname.Replace("$", "");
                        sqlBulk.WriteToServer(dReader);
                    }

                    excelConnection.Close();
                    mesg = "Import Success.";
                }
                else
                {
                    mesg = "Vui lòng Chọn tệp cần import";
                    return;
                }
            }
            catch (Exception ex)
            {
                excelConnection.Close();
                mesg = ex.Message;
            }
        }
        #endregion

        /// <summary>
        /// Send Email mật khẩu cho user
        /// </summary>
        /// <returns></returns>
        #region Send Email
        //////////////////////////////////////////////// SEND EMAIL ///////////////////////////////////////////////////
        //Populating the HTML Formatted Body
        private string PopulateBody(string userName, string dpid, string newpw)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/Content/Templates/passwordTemplate.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{fullname}", userName);
            body = body.Replace("{dpid}", dpid);
            body = body.Replace("{loginpass}", newpw);
            return body;
        }

        //Email Sending Method
        private void SendHtmlFormattedEmail(string recepientEmail, string subject, string body)
        {
            using (MailMessage mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress("fcs@fpts.com.vn", "Phong Dich vu Khach hang – FPTS");
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;
                mailMessage.To.Add(new MailAddress(recepientEmail));
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.fpts.com.vn";
                //smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);
                System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                NetworkCred.UserName = "fcs";
                NetworkCred.Password = "fcs1307";
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                //smtp.Port = int.Parse(ConfigurationManager.AppSettings["Port"]);
                smtp.Send(mailMessage);
            }
        }

        //Sending the Formatted HTML email
        public bool SendEmailFgpw(string CusEmail, ref string mesg)
        {
            try
            {
                string strSubject = "OZE Mat khau / OZE password";
                string strBody = "";

                strBody = this.PopulateBody(HttpContext.Current.Session[CConfig.SESSION_FULLNAME].ToString(),
                    HttpContext.Current.Session[CConfig.SESSION_USERNAME].ToString(),
                    HttpContext.Current.Session[CConfig.SESSION_PW].ToString());
                string email = string.IsNullOrEmpty(SendMKEmail) ? CusEmail : SendMKEmail;
                this.SendHtmlFormattedEmail(email,
                    strSubject,
                    strBody);
                mesg = "Vui lòng Kiểm tra hòm mail để cập nhật mật khẩu!";
                return true;
            }
            catch (Exception ex)
            {
                mesg = "Gửi mail thất bại!";
                return false;
            }
        }
        #endregion

        /// <summary>
        /// Update mật khẩu cho user vào db
        /// </summary>
        /// <returns></returns>
        #region Update MK cho user
        public void AccountUpdatePw(LoginModel mdlogin, ref int retcode)
        {
            try
            {
                SqlParameter[] para = new SqlParameter[4];
                para[0] = new SqlParameter("@UserName", SqlDbType.NVarChar, 20);
                para[0].Direction = ParameterDirection.Input;
                para[0].Value = mdlogin.UserName;
                para[1] = new SqlParameter("@Email", SqlDbType.NVarChar, 50);
                para[1].Direction = ParameterDirection.Input;
                para[1].Value = mdlogin.Email;
                para[2] = new SqlParameter("@Password", SqlDbType.NVarChar, 50);
                para[2].Direction = ParameterDirection.Input;
                para[2].Value = mdlogin.Password;
                para[3] = new SqlParameter("@FirstLogin", SqlDbType.Int);
                para[3].Direction = ParameterDirection.Input;
                para[3].Value = mdlogin.FirstLogin;

                helper.ExecuteQuery(CConfig.SP_ACCOUNT_UPDATE_PW, para, CommandType.StoredProcedure);
                retcode = 1;
                return;
            }
            catch (Exception ex)
            {
                retcode = -1;
                return;
            }
        }

        #endregion

        /// <summary>
        /// Get Territories
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        #region Get Territories
        public List<TerritoriesModel> TerritoriesGet(TerritoriesModel mdterr, ref int RetCode, ref string RetMesg)
        {
            List<TerritoriesModel> result = new List<TerritoriesModel>();
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] para = new SqlParameter[6];
                para[0] = new SqlParameter("@Activity", SqlDbType.NVarChar, 20);
                para[0].Direction = ParameterDirection.Input;
                para[0].Value = mdterr.Activity;
                para[1] = new SqlParameter("@ProvincesId", SqlDbType.NVarChar, 2);
                para[1].Direction = ParameterDirection.Input;
                para[1].Value = mdterr.ProvinceId;
                para[2] = new SqlParameter("@DistrictId", SqlDbType.NVarChar, 3);
                para[2].Direction = ParameterDirection.Input;
                para[2].Value = mdterr.DistrictId;
                para[3] = new SqlParameter("@WardsId", SqlDbType.NVarChar, 5);
                para[3].Direction = ParameterDirection.Input;
                para[3].Value = mdterr.WardsId;
                para[4] = new SqlParameter("@RetCode", SqlDbType.Int);
                para[4].Direction = ParameterDirection.Output;
                para[4].Value = DBNull.Value;
                para[5] = new SqlParameter("@RetMesg", SqlDbType.NVarChar, 100);
                para[5].Direction = ParameterDirection.Output;
                para[5].Value = DBNull.Value;

                ds = helper.ExecuteQuery(CConfig.SP_TERRITORIES, para, CommandType.StoredProcedure);
                dt = ds.Tables[0];
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        result.Add(new TerritoriesModel
                        {
                            ProvinceId = row["ProvinceId"].ToString().Trim(),
                            ProvinceName = row["ProvinceName"].ToString().Trim(),
                            DistrictId = row["DistrictId"].ToString().Trim(),
                            DistrictName = row["DistrictName"].ToString().Trim(),
                            WardsId = row["WardsId"].ToString().Trim(),
                            WardsName = row["WardsName"].ToString().Trim()
                        });
                    }
                }
                else { result = null; }

                RetCode = Int32.Parse(para[4].Value.ToString());
                RetMesg = "Có " + dt.Rows.Count + " thông tin được tìm thấy.";
            }
            catch (Exception ex)
            {
                RetCode = -1;
                RetMesg = ex.Message;
                result = null;
            }
            return result;
        }
        #endregion

        #region Insert Territories
        public DataTable InsertTerri(TerritoriesModel mdterr, ref int RetCode, ref string RetMesg)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] para = new SqlParameter[9];

                para[0] = new SqlParameter("@Activity", SqlDbType.NVarChar, 20);
                para[0].Direction = ParameterDirection.Input;
                para[0].Value = mdterr.Activity;
                para[1] = new SqlParameter("@ProvincesName", SqlDbType.NVarChar, 50);
                para[1].Direction = ParameterDirection.Input;
                para[1].Value = mdterr.ProvinceName;
                para[2] = new SqlParameter("@DistrictName", SqlDbType.NVarChar, 50);
                para[2].Direction = ParameterDirection.Input;
                para[2].Value = mdterr.DistrictName;
                para[3] = new SqlParameter("@WardsName", SqlDbType.NVarChar, 50);
                para[3].Direction = ParameterDirection.Input;
                para[3].Value = mdterr.WardsName;
                para[4] = new SqlParameter("@ProvincesId", SqlDbType.NVarChar, 2);
                para[4].Direction = ParameterDirection.Input;
                para[4].Value = mdterr.ProvinceId;
                para[5] = new SqlParameter("@DistrictId", SqlDbType.NVarChar, 3);
                para[5].Direction = ParameterDirection.Input;
                para[5].Value = mdterr.DistrictId;
                para[6] = new SqlParameter("@WardsId", SqlDbType.NVarChar, 5);
                para[6].Direction = ParameterDirection.Input;
                para[6].Value = mdterr.WardsId;
                para[7] = new SqlParameter("@RetCode", SqlDbType.Int);
                para[7].Direction = ParameterDirection.Output;
                para[7].Value = DBNull.Value;
                para[8] = new SqlParameter("@RetMesg", SqlDbType.NVarChar, 100);
                para[8].Direction = ParameterDirection.Output;
                para[8].Value = DBNull.Value;

                ds = helper.ExecuteQuery(CConfig.SP_TERRITORIES, para, CommandType.StoredProcedure);

                RetCode = Int32.Parse(para[7].Value.ToString());
                RetMesg = para[8].Value.ToString();

            }
            catch (Exception ex)
            {
                RetCode = -1;
                RetMesg = ex.Message;
                ds = null;
                throw ex;
            }
            return ds.Tables[0];
        }
        #endregion

        /// <summary>
        /// Delete/Update Territories
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        #region Update/Delete Territories
        public bool UpdDelTerri(TerritoriesModel mdterr, ref int RetCode, ref string RetMesg)
        {
            bool result = false;
            List<string> error = new List<string>();
            DataSet ds = new DataSet();
            try
            {
                SqlParameter[] para = new SqlParameter[9];

                para[0] = new SqlParameter("@Activity", SqlDbType.NVarChar, 20);
                para[0].Direction = ParameterDirection.Input;
                para[0].Value = mdterr.Activity;
                para[1] = new SqlParameter("@ProvincesName", SqlDbType.NVarChar, 50);
                para[1].Direction = ParameterDirection.Input;
                para[1].Value = mdterr.ProvinceName;
                para[2] = new SqlParameter("@DistrictName", SqlDbType.NVarChar, 50);
                para[2].Direction = ParameterDirection.Input;
                para[2].Value = mdterr.DistrictName;
                para[3] = new SqlParameter("@WardsName", SqlDbType.NVarChar, 50);
                para[3].Direction = ParameterDirection.Input;
                para[3].Value = mdterr.WardsName;
                para[4] = new SqlParameter("@ProvincesId", SqlDbType.NVarChar, 2);
                para[4].Direction = ParameterDirection.Input;
                para[4].Value = mdterr.ProvinceId;
                para[5] = new SqlParameter("@DistrictId", SqlDbType.NVarChar, 3);
                para[5].Direction = ParameterDirection.Input;
                para[5].Value = mdterr.DistrictId;
                para[6] = new SqlParameter("@WardsId", SqlDbType.NVarChar, 5);
                para[6].Direction = ParameterDirection.Input;
                para[6].Value = mdterr.WardsId;
                para[7] = new SqlParameter("@RetCode", SqlDbType.Int);
                para[7].Direction = ParameterDirection.Output;
                para[7].Value = DBNull.Value;
                para[8] = new SqlParameter("@RetMesg", SqlDbType.NVarChar, 100);
                para[8].Direction = ParameterDirection.Output;
                para[8].Value = DBNull.Value;

                helper.ExecuteNonQuery(CConfig.SP_TERRITORIES, para, CommandType.StoredProcedure);

                RetCode = Int32.Parse(para[7].Value.ToString());
                RetMesg = para[8].Value.ToString();

                result = true;
            }
            catch (Exception ex)
            {
                RetCode = -1;
                RetMesg = ex.Message;
                result = false;
            }
            return result;
        }
        #endregion

        /// <summary>
        /// User Update
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        #region Update/Delete Territories
        public bool UserUpdate(SysUserModel obj, ref int RetCode, ref string RetMesg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[5];
                para[0] = new SqlParameter("@ID", SqlDbType.Int);
                para[0].Direction = ParameterDirection.Input;
                para[0].Value = obj.ID;
                para[1] = new SqlParameter("@Address", SqlDbType.NVarChar, 200);
                para[1].Direction = ParameterDirection.Input;
                para[1].Value = obj.Address;
                para[2] = new SqlParameter("@Email", SqlDbType.NVarChar, 50);
                para[2].Direction = ParameterDirection.Input;
                para[2].Value = obj.Email;
                para[3] = new SqlParameter("@Mobile", SqlDbType.VarChar, 12);
                para[3].Direction = ParameterDirection.Input;
                para[3].Value = obj.Mobile;
                para[4] = new SqlParameter("@Modifyby", SqlDbType.Int);
                para[4].Direction = ParameterDirection.Input;
                para[4].Value = obj.Modifyby;

                helper.ExecuteNonQuery(CConfig.SP_USERDETAIL_UPDATE, para, CommandType.StoredProcedure);

                RetCode = 1;
                RetMesg = "Cập nhật thành công!";

                result = true;
            }
            catch (Exception ex)
            {
                RetCode = -1;
                RetMesg = "Cập nhật không thành công. " + ex.Message;

                return result;
            }

            return result;
        }
        #endregion

        #region Insert ReservationRoomModel reservation
        public DataSet InsertReservationRoom(CustomerModel customer, ReservationRoomModel reservation, ref int RetCode, ref string RetMesg)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlParameter[] para = new SqlParameter[41];

                //THONG TIN KH
                para[0] = new SqlParameter("@ActivityC", SqlDbType.NVarChar, 20);
                para[0].Direction = ParameterDirection.Input;
                para[0].Value = customer.Activity;

                para[1] = new SqlParameter("@FullName", SqlDbType.NVarChar, 50);
                para[1].Direction = ParameterDirection.Input;
                para[1].Value = customer.FullName;

                para[2] = new SqlParameter("@Sex", SqlDbType.Int);
                para[2].Direction = ParameterDirection.Input;
                para[2].Value = customer.Sex;

                para[3] = new SqlParameter("@DOB", SqlDbType.NVarChar, 20);
                para[3].Direction = ParameterDirection.Input;
                para[3].Value = customer.DOB;

                para[4] = new SqlParameter("@IdentityNumber", SqlDbType.NVarChar, 20);
                para[4].Direction = ParameterDirection.Input;
                para[4].Value = customer.IdentityNumber;

                para[5] = new SqlParameter("@CitizenshipCode", SqlDbType.Int);
                para[5].Direction = ParameterDirection.Input;
                para[5].Value = customer.CitizenshipCode;

                para[6] = new SqlParameter("@Company", SqlDbType.NVarChar, 150);
                para[6].Direction = ParameterDirection.Input;
                para[6].Value = customer.Company;

                para[7] = new SqlParameter("@Address", SqlDbType.NVarChar, 200);
                para[7].Direction = ParameterDirection.Input;
                para[7].Value = customer.Address;

                para[8] = new SqlParameter("@Email", SqlDbType.NVarChar, 100);
                para[8].Direction = ParameterDirection.Input;
                para[8].Value = customer.Email;

                para[9] = new SqlParameter("@Mobile", SqlDbType.NVarChar, 20);
                para[9].Direction = ParameterDirection.Input;
                para[9].Value = customer.Mobile;

                para[10] = new SqlParameter("@HotelCode", SqlDbType.NVarChar, 50);
                para[10].Direction = ParameterDirection.Input;
                para[10].Value = customer.HotelCode;

                para[11] = new SqlParameter("@RoomID", SqlDbType.Int);
                para[11].Direction = ParameterDirection.Input;
                para[11].Value = customer.RoomID;

                para[12] = new SqlParameter("@GroupID", SqlDbType.Int);
                para[12].Direction = ParameterDirection.Input;
                para[12].Value = customer.GroupID;

                para[13] = new SqlParameter("@GroupJoinID", SqlDbType.Int);
                para[13].Direction = ParameterDirection.Input;
                para[13].Value = customer.GroupJoinID;

                para[14] = new SqlParameter("@Leader", SqlDbType.Int);
                para[14].Direction = ParameterDirection.Input;
                para[14].Value = customer.Leader;

                para[15] = new SqlParameter("@Payer", SqlDbType.Int);
                para[15].Direction = ParameterDirection.Input;
                para[15].Value = customer.Payer;

                para[16] = new SqlParameter("@Createby", SqlDbType.Int);
                para[16].Direction = ParameterDirection.Input;
                para[16].Value = customer.Createby;

                //THONG TIN DAT PHONG
                para[17] = new SqlParameter("@ActivityR", SqlDbType.NVarChar, 20);
                para[17].Direction = ParameterDirection.Input;
                para[17].Value = reservation.Activity;

                para[18] = new SqlParameter("@ReservationCode", SqlDbType.NVarChar, 50);
                para[18].Direction = ParameterDirection.Input;
                para[18].Value = reservation.ReservationCode;

                para[19] = new SqlParameter("@ReservationType", SqlDbType.Int);
                para[19].Direction = ParameterDirection.Input;
                para[19].Value = reservation.ReservationType;

                para[20] = new SqlParameter("@Payment_Type_ID", SqlDbType.Int);
                para[20].Direction = ParameterDirection.Input;
                para[20].Value = reservation.Payment_Type_ID;

                para[21] = new SqlParameter("@ArrivalDate", SqlDbType.NVarChar, 20);
                para[21].Direction = ParameterDirection.Input;
                para[21].Value = reservation.ArrivalDate;

                para[22] = new SqlParameter("@Leave_Date ", SqlDbType.NVarChar, 20);
                para[22].Direction = ParameterDirection.Input;
                para[22].Value = reservation.LeaveDate;

                para[23] = new SqlParameter("@Number_People", SqlDbType.Int);
                para[23].Direction = ParameterDirection.Input;
                para[23].Value = reservation.Adult;

                para[24] = new SqlParameter("@Number_Children", SqlDbType.Int);
                para[24].Direction = ParameterDirection.Input;
                para[24].Value = reservation.Children;

                para[25] = new SqlParameter("@Holiday", SqlDbType.Int);
                para[25].Direction = ParameterDirection.Input;
                para[25].Value = reservation.Holiday;

                para[26] = new SqlParameter("@KhungGio", SqlDbType.Int);
                para[26].Direction = ParameterDirection.Input;
                para[26].Value = reservation.KhungGio;

                para[27] = new SqlParameter("@Price", SqlDbType.Float);
                para[27].Direction = ParameterDirection.Input;
                para[27].Value = reservation.Price;

                para[28] = new SqlParameter("@Tax", SqlDbType.Float);
                para[28].Direction = ParameterDirection.Input;
                para[28].Value = reservation.Tax;

                para[29] = new SqlParameter("@Deposit", SqlDbType.Float);
                para[29].Direction = ParameterDirection.Input;
                para[29].Value = reservation.Deposit;

                para[30] = new SqlParameter("@Discount", SqlDbType.Float);
                para[30].Direction = ParameterDirection.Input;
                para[30].Value = reservation.Discount;

                para[31] = new SqlParameter("@Deduction", SqlDbType.Float);
                para[31].Direction = ParameterDirection.Input;
                para[31].Value = reservation.Deduction;

                para[32] = new SqlParameter("@Note", SqlDbType.NVarChar, 200);
                para[32].Direction = ParameterDirection.Input;
                para[32].Value = reservation.Note;

                para[33] = new SqlParameter("@Reason", SqlDbType.NVarChar, 200);
                para[33].Direction = ParameterDirection.Input;
                para[33].Value = reservation.Reason;

                para[34] = new SqlParameter("@CreateResBy", SqlDbType.Int);
                para[34].Direction = ParameterDirection.Input;
                para[34].Value = reservation.CreateResBy;


                para[35] = new SqlParameter("@CusID", SqlDbType.Int);
                para[35].Direction = ParameterDirection.Input;
                para[35].Value = customer.ID;
                para[36] = new SqlParameter("@ResID", SqlDbType.Int);
                para[36].Direction = ParameterDirection.Input;
                para[36].Value = reservation.ID;

                para[37] = new SqlParameter("@ModifyBy", SqlDbType.Int);
                para[37].Direction = ParameterDirection.Input;
                para[37].Value = customer.Modifyby;
                para[38] = new SqlParameter("@ModifyResBy", SqlDbType.Int);
                para[38].Direction = ParameterDirection.Input;
                para[38].Value = reservation.ModifyResby;




                //OUTPUT
                para[39] = new SqlParameter("@RetCode", SqlDbType.Int);
                para[39].Direction = ParameterDirection.Output;
                para[39].Value = DBNull.Value;
                para[40] = new SqlParameter("@RetMesg", SqlDbType.NVarChar, 100);
                para[40].Direction = ParameterDirection.Output;
                para[40].Value = DBNull.Value;

                ds = helper.ExecuteQuery("sp_ReservationRoom_Create", para, CommandType.StoredProcedure);

                RetCode = Int32.Parse(para[39].Value.ToString());
                RetMesg = para[40].Value.ToString();
            }
            catch (Exception ex)
            {
                ds = null;
                RetCode = -1;
                RetMesg = ex.Message;
                throw ex;
            }
            return ds;
        }
        #endregion

        #region Insert Friend reservation
        public DataSet InsertFriend(CustomerModel customer, ref int RetCode, ref string RetMesg)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlParameter[] para = new SqlParameter[21];

                //THONG TIN KH
                para[0] = new SqlParameter("@ActivityC", SqlDbType.NVarChar, 20);
                para[0].Direction = ParameterDirection.Input;
                para[0].Value = customer.Activity;

                para[1] = new SqlParameter("@FullName", SqlDbType.NVarChar, 50);
                para[1].Direction = ParameterDirection.Input;
                para[1].Value = customer.FullName;

                para[2] = new SqlParameter("@Sex", SqlDbType.Int);
                para[2].Direction = ParameterDirection.Input;
                para[2].Value = customer.Sex;

                para[3] = new SqlParameter("@DOB", SqlDbType.NVarChar, 20);
                para[3].Direction = ParameterDirection.Input;
                para[3].Value = customer.DOB;

                para[4] = new SqlParameter("@IdentityNumber", SqlDbType.NVarChar, 20);
                para[4].Direction = ParameterDirection.Input;
                para[4].Value = customer.IdentityNumber;

                para[5] = new SqlParameter("@CitizenshipCode", SqlDbType.Int);
                para[5].Direction = ParameterDirection.Input;
                para[5].Value = customer.CitizenshipCode;

                para[6] = new SqlParameter("@Company", SqlDbType.NVarChar, 150);
                para[6].Direction = ParameterDirection.Input;
                para[6].Value = customer.Company;

                para[7] = new SqlParameter("@Address", SqlDbType.NVarChar, 200);
                para[7].Direction = ParameterDirection.Input;
                para[7].Value = customer.Address;

                para[8] = new SqlParameter("@Email", SqlDbType.NVarChar, 100);
                para[8].Direction = ParameterDirection.Input;
                para[8].Value = customer.Email;

                para[9] = new SqlParameter("@Mobile", SqlDbType.NVarChar, 20);
                para[9].Direction = ParameterDirection.Input;
                para[9].Value = customer.Mobile;

                para[10] = new SqlParameter("@HotelCode", SqlDbType.NVarChar, 50);
                para[10].Direction = ParameterDirection.Input;
                para[10].Value = customer.HotelCode;

                para[11] = new SqlParameter("@RoomID", SqlDbType.Int);
                para[11].Direction = ParameterDirection.Input;
                para[11].Value = customer.RoomID;

                para[12] = new SqlParameter("@GroupID", SqlDbType.Int);
                para[12].Direction = ParameterDirection.Input;
                para[12].Value = customer.GroupID;

                para[13] = new SqlParameter("@GroupJoinID", SqlDbType.Int);
                para[13].Direction = ParameterDirection.Input;
                para[13].Value = customer.GroupJoinID;

                para[14] = new SqlParameter("@Leader", SqlDbType.Int);
                para[14].Direction = ParameterDirection.Input;
                para[14].Value = customer.Leader;

                para[15] = new SqlParameter("@Payer", SqlDbType.Int);
                para[15].Direction = ParameterDirection.Input;
                para[15].Value = customer.Payer;

                para[16] = new SqlParameter("@Createby", SqlDbType.Int);
                para[16].Direction = ParameterDirection.Input;
                para[16].Value = customer.Createby;                

                para[17] = new SqlParameter("@ReserCode", SqlDbType.NVarChar, 50);
                para[17].Direction = ParameterDirection.Input;
                para[17].Value = customer.ReserCode;

                para[18] = new SqlParameter("@ID", SqlDbType.NVarChar, 50);
                para[18].Direction = ParameterDirection.Input;
                para[18].Value = customer.ID;

                //OUTPUT
                para[19] = new SqlParameter("@RetCode", SqlDbType.Int);
                para[19].Direction = ParameterDirection.Output;
                para[19].Value = DBNull.Value;
                para[20] = new SqlParameter("@RetMesg", SqlDbType.NVarChar, 100);
                para[20].Direction = ParameterDirection.Output;
                para[20].Value = DBNull.Value;

                ds = helper.ExecuteQuery("sp_ReservationRoom_Friend", para, CommandType.StoredProcedure);

                RetCode = Int32.Parse(para[19].Value.ToString());
                RetMesg = para[20].Value.ToString();
                
            }
            catch (Exception ex)
            {
                ds = null;
                RetCode = -1;
                RetMesg = ex.Message;
                throw ex;
            }
            return ds;
        }
        #endregion

        /// <summary>
        /// ReserCancel
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        #region ReserCancel
        public bool ReserCancel(ReservationRoomModel resroom, ref int RetCode, ref string RetMesg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[3];
                para[0] = new SqlParameter("@ReserCode", SqlDbType.NVarChar, 20);
                para[0].Direction = ParameterDirection.Input;
                para[0].Value = resroom.ReservationCode;

                //OUTPUT
                para[1] = new SqlParameter("@RetCode", SqlDbType.Int);
                para[1].Direction = ParameterDirection.Output;
                para[1].Value = DBNull.Value;
                para[2] = new SqlParameter("@RetMesg", SqlDbType.NVarChar, 100);
                para[2].Direction = ParameterDirection.Output;
                para[2].Value = DBNull.Value;

                helper.ExecuteNonQuery("sp_ReservationRoom_Cancel", para, CommandType.StoredProcedure);

                RetCode = Int32.Parse(para[1].Value.ToString());
                RetMesg = para[2].Value.ToString();

                result = true;
            }
            catch (Exception ex)
            {
                RetCode = -1;
                RetMesg = ex.Message;

                return result;
            }

            return result;
        }
        #endregion


        #region GEN CODE
        public bool GenResCode(ReservationRoomModel obj, ref string RetMesg)
        {
            bool result = false;
            try
            {
                SqlParameter[] para = new SqlParameter[3];
                para[0] = new SqlParameter("@Activity", SqlDbType.NVarChar, 50);
                para[0].Direction = ParameterDirection.Input;
                para[0].Value = obj.Activity;
                para[1] = new SqlParameter("@HotelCode", SqlDbType.NVarChar, 200);
                para[1].Direction = ParameterDirection.Input;
                para[1].Value = obj.HotelCode;

                para[2] = new SqlParameter("@ReturnCode", SqlDbType.NVarChar, 100);
                para[2].Direction = ParameterDirection.Output;
                para[2].Value = DBNull.Value;

                helper.ExecuteNonQuery("sp_GenCode", para, CommandType.StoredProcedure);

                RetMesg = para[2].Value.ToString(); ;

                result = true;
            }
            catch (Exception ex)
            {
                RetMesg = ex.Message;

                return result;
            }

            return result;
        }
        #endregion
    }
}