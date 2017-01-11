using Oze.AppCode.BLL;
using Oze.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;

namespace Oze.AppCode.DAL
{
    public class CRightPermissions
    {
        CSQLHelper helper = new CSQLHelper(CConfig.CONNECTIONSTRING_OZEGENERAL);
        private SqlParameter[] para;

        public DataTable tbUserHasPermission()
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            int UserID = -1;
            try
            {
                if (HttpContext.Current.Session[CConfig.SESSION_USERID] == null)
                {
                    HttpContext.Current.Response.Redirect("/Accounts/Login");
                }
                else
                {
                    UserID = Convert.ToInt32(HttpContext.Current.Session[CConfig.SESSION_USERID]);
                }

                if (HttpContext.Current.Session[CConfig.SESSION_RIGHT_PERMISSION] != null)
                {
                    dt = (DataTable)HttpContext.Current.Session[CConfig.SESSION_RIGHT_PERMISSION];
                }
                else
                {
                    para = new SqlParameter[1];
                    para[0] = new SqlParameter("@UserID", SqlDbType.Int);
                    para[0].Direction = ParameterDirection.Input;
                    para[0].Value = UserID;
                    ds = helper.ExecuteQuery(CConfig.SP_GET_RIGHT_PERMISSION, para, CommandType.StoredProcedure);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        dt = ds.Tables[0];
                        HttpContext.Current.Session[CConfig.SESSION_RIGHT_PERMISSION] = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                dt = null;
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }
            return dt;
        }

        public List<AccessRightModels> UserHasPermission()
        {
            List<AccessRightModels> _buildList = new List<AccessRightModels>();
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            int UserID = -1;
            try
            {
                if (HttpContext.Current.Session[CConfig.SESSION_USERID] == null)
                {
                    HttpContext.Current.Response.Redirect("/Accounts/Login");
                }
                else
                {
                    UserID = Convert.ToInt32(HttpContext.Current.Session[CConfig.SESSION_USERID]);
                }

                if (HttpContext.Current.Session[CConfig.SESSION_RIGHT_PERMISSION] != null)
                {
                    dt = (DataTable)HttpContext.Current.Session[CConfig.SESSION_RIGHT_PERMISSION];
                }
                else
                {
                    para = new SqlParameter[1];
                    para[0] = new SqlParameter("@UserID", SqlDbType.Int);
                    para[0].Direction = ParameterDirection.Input;
                    para[0].Value = UserID;
                    ds = helper.ExecuteQuery(CConfig.SP_GET_RIGHT_PERMISSION, para, CommandType.StoredProcedure);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        dt = ds.Tables[0];
                        HttpContext.Current.Session[CConfig.SESSION_RIGHT_PERMISSION] = dt;
                    }
                }
                if (dt.Rows.Count > 0)
                {
                    _buildList.Add(new AccessRightModels
                    {
                        RuleID = Convert.ToInt32(dt.Rows[0]["RuleID"].ToString()),
                        ModelName = dt.Rows[0]["ModelID"].ToString(),
                        Read = Convert.ToBoolean(dt.Rows[0]["Read"].ToString()),
                        Write = Convert.ToBoolean(dt.Rows[0]["Write"].ToString()),
                        Create = Convert.ToBoolean(dt.Rows[0]["Create"].ToString()),
                        Delete = Convert.ToBoolean(dt.Rows[0]["Delete"].ToString())
                    });

                }
            }
            catch (Exception ex)
            {
                _buildList = null;
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }

            return _buildList;
        }

        public bool checkPermission(DataTable dt, string str, string action)
        {
            bool result = false;
            DataRow[] dtSelect = new DataRow[dt.Rows.Count];
            try
            {
                dtSelect = dt.Select("Model = '" + str + "'");
                foreach (DataRow row in dtSelect)
                {
                    if (row[action].ToString() == "1")
                    {
                        if (result == false)
                            result = true;
                        else
                            break;
                    }
                }
                //if (dt.Select("Model = '"+ str +"'")[0][action].ToString() == "True")
            }
            catch (Exception ex)
            {
                result = false;
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }
            return result;
        }

    }
}