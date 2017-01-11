using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Xml;
using Oze.AppCode.BLL;

/*
 * Stored procedure execution is faster when you pass parameters by position (the order in which the parameters are declared in the stored procedure) rather than by name.
 * When using objects pooled by MTS, acquire resources as late as possible and release them as soon as possible. As such, you should create objects as late as possible, and destroy them as early as possible to free resources.
 * Do not open data connections using a specific user's credentials. Connections that have been opened using such credentials cannot be pooled and reused, thus losing the benefits of connection pooling.
 * Explicitly close ADO Recordset and Connection objects to insure that connections are promptly returned to the connection pool for use by other processes.
 */
namespace Oze.AppCode.DAL
{
    public class CSQL
    {
        private string m_ConnectionString;
        private int m_CommandTimeout = Convert.ToInt32(CConfig.SQL_COMMAND_TIMEOUT);
        private SqlCommand m_Command = new SqlCommand();
        private SqlConnection m_Connection;        
        private SqlDataAdapter m_DataAdapter;
        private SqlDataReader m_DataReader;
        private double m_dblDuration;
        //===============================================================================================================

        public CSQL(string strConnectionString)
        {
            this.m_ConnectionString = strConnectionString;
            this.m_dblDuration = 0;
        }
        #region Public properties
        public SqlParameterCollection Parameters
        {
            get
            {
                m_Command.Parameters.Clear();
                return m_Command.Parameters;
            }
        }

        public SqlCommand Command
        {
            get { return m_Command; }
            set { m_Command = value; }
        }

        public SqlConnection Connection
        {
            get { return m_Connection; }
            set { m_Connection = value; }
        }

        public SqlDataReader DataReader
        {
            get { return m_DataReader; }
            set { m_DataReader = value; }
        }

        public int ExecDuration
        {
            get { return Convert.ToInt32(this.m_dblDuration); }
        }
        #endregion

        #region Public methods

        #region Open_Close Conn
        //Ham nay thuc hien viec ket noi voi database
        //COMMAND => The time in seconds to wait for the command to execute. The default is 30 seconds.
        public bool _OpenConnection()
        {
            bool functionReturnValue = false;

            try
            {
                if (m_Connection == null || m_Connection.State == ConnectionState.Closed) // 2015-07-16 09:33:40 ngocta2 => bug: open 2 connection, close 1
                {
                    // CHUA OPEN roi thi tao new connection
                    m_Connection = new SqlConnection(this.m_ConnectionString);
                    m_Connection.Open();
                    functionReturnValue = true;
                }
                else
                {
                    if (m_Connection.State == ConnectionState.Open)
                    {
                        // OPEN roi thi ko tao connection nua
                        functionReturnValue = true;
                    }
                }
            }
            catch (Exception ex)
            {
                functionReturnValue = false;
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }
            return functionReturnValue;
        }

        //Ham nay dung de dong 1 connection den database
        public void _CloseConnection()
        {
            try
            {
                if ((m_Connection.State == ConnectionState.Open))
                {
                    m_Connection.Close();
                    m_Connection.Dispose();
                    m_Command.Dispose();
                }
            }
            catch (Exception ex)
            {
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }
            finally
            {
            }
        }
        #endregion

        #region GetParamInfo For Wirte log
        /// <summary>
        /// log
        /// </summary>
        /// <param name="Params"></param>
        /// <returns></returns>
        public string GetParamInfo()
        {
            SqlParameterCollection Params = this.m_Command.Parameters;

            int i = 0;
            string s = "\r\n";
            for (i = 0; i <= Params.Count - 1; i++)
                s += "\t" + Params[i].ParameterName + "='" + Params[i].Value + "',\r\n";

            if (_OpenConnection() == false)           // Open Connection
                return null;

            //<add name="CONNECTION_STRING_50" connectionString="server=10.26.248.50;UID=sa;PWD=sa;database=EzTransfer;Connect Timeout=30;Pooling=false;"  providerName="System.Data.SqlClient"/>
            System.Data.Common.DbConnectionStringBuilder builder = new System.Data.Common.DbConnectionStringBuilder();
            builder.ConnectionString = this.Connection.ConnectionString;
            string strServer = builder["server"] as string;
            string strDatabase = builder["database"] as string;
            s += CConfig.CHAR_TAB + "--" + strServer + "=>" + strDatabase + CConfig.CHAR_CRLF;

            return s;
        }
        #endregion

        //===============================================================================================================

        #region Execute
        /// <summary>
        /// ExecuteQuery
        /// Thuc hien 1 cau lenh Insert, Update, Delete, hay 1 procedure = cach viet cau lenh "exec sp_fgsd"
        /// Input: cau lenh sql
        /// Output: Boolean (thanh cong hay khong ?)
        /// </summary>
        /// <param name="SQL">delect * from tbl_test</param>
        /// <param name="SQL">exec sp_fgsd</param>
        /// <returns></returns>
        public bool ExecuteQuery(string myQuery)
        {
            bool functionReturnValue = false;
            if ((_OpenConnection() == false))           // Open Connection
                return false;

            m_Command.CommandTimeout = this.m_CommandTimeout;
            m_Command.Connection = m_Connection;
            m_Command.CommandText = myQuery;
            m_Command.CommandType = CommandType.Text;

            try
            {
                CLog.LogSQL(CBase.GetDeepCaller(), myQuery);    // log SQL

                DateTime dtBegin = DateTime.Now; // duration                
                m_Command.ExecuteNonQuery();
                this.m_dblDuration = DateTime.Now.Subtract(dtBegin).TotalMilliseconds; // duration
                functionReturnValue = true;
            }
            catch (Exception ex)
            {
                functionReturnValue = false;
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }
            finally
            {
                _CloseConnection();     // Close Connection
            }
            return functionReturnValue;
        }
        /// <summary>
        /// ExecuteSP
        /// Thuc hien 1 store procedure Insert, Update, Delete
        /// Input: 1. spName: ten store
        ///        2. myParamArray() : mang cac tham so truyen va
        /// Output: Boolean (thanh cong hay khong ?)
        /// </summary>
        /// <param name="SPname">sp_DELETE_ALL</param>
        /// <returns></returns>
        public bool ExecuteSP(string SPname)
        {
            bool functionReturnValue = false;
            if ((_OpenConnection() == false))           // Open Connection
                return false;

            m_Command.Connection = m_Connection;
            m_Command.CommandText = SPname;
            m_Command.CommandType = CommandType.StoredProcedure;

            try
            {
                CLog.LogSQL(CBase.GetDeepCaller(), SPname + this.GetParamInfo());       // log SQL

                DateTime dtBegin = DateTime.Now; // duration                
                m_Command.ExecuteNonQuery();
                this.m_dblDuration = DateTime.Now.Subtract(dtBegin).TotalMilliseconds; // duration
                m_Command.Parameters.Clear();
                functionReturnValue = true;
            }
            catch (Exception ex)
            {
                functionReturnValue = false;
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }
            finally
            {
                _CloseConnection();     // Close Connection
            }
            return functionReturnValue;
        }
        #endregion

        #region GetDatatable
        /// <summary>
        /// GetDatatableFromQuery
        /// Thuc hien 1 cau query tra ve 1 datatable
        /// Input: cau lenh Sql
        /// Output: datatable
        /// </summary>
        /// <param name="SQL">select * from tbl_test</param>
        /// <returns></returns>
        public DataTable GetDatatableFromQuery(string myQuery)
        {
            DataTable functionReturnValue = null;
            DataTable table = new DataTable();

            if ((_OpenConnection() == false))           // Open Connection
                return null;

            try
            {
                CLog.LogSQL(CBase.GetDeepCaller(), myQuery);    // log SQL

                m_Command.Connection = m_Connection;
                m_Command.CommandText = myQuery;
                m_Command.CommandType = CommandType.Text;

                DateTime dtBegin = DateTime.Now; // duration    
                m_DataAdapter = new SqlDataAdapter(m_Command);
                m_DataAdapter.Fill(table);
                this.m_dblDuration = DateTime.Now.Subtract(dtBegin).TotalMilliseconds; // duration
                functionReturnValue = table;
                //throw new Exception("ngocta2 gay loi");
            }
            catch (Exception ex)
            {
                functionReturnValue = null;
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }
            finally
            {
                _CloseConnection();     // Close Connection
            }
            return functionReturnValue;
        }

        /// <summary>
        /// GetDataTableFromSP
        /// Thuc hien 1 Store procedure tra ve 1 datatable
        /// Input: 1. ds parameters
        ///        2. tên sp
        /// Output: datatable
        /// </summary>
        /// <param name="SPname">sp_select</param>
        /// <returns></returns>
        public DataTable GetDataTableFromSP(string SPname)
        {
            DataTable functionReturnValue = null;
            DataTable table = new DataTable();

            if ((_OpenConnection() == false))           // Open Connection
                return null;

            try
            {
                CLog.LogSQL(CBase.GetDeepCaller(), SPname + this.GetParamInfo());    // log SQL

                m_Command.Connection = m_Connection;
                m_Command.CommandText = SPname;
                m_Command.CommandType = CommandType.StoredProcedure;

                DateTime dtBegin = DateTime.Now; // duration    
                m_DataAdapter = new SqlDataAdapter(m_Command);
                m_DataAdapter.Fill(table);
                this.m_dblDuration = DateTime.Now.Subtract(dtBegin).TotalMilliseconds; // duration
                m_Command.Parameters.Clear();
                functionReturnValue = table;
            }
            catch (Exception ex)
            {
                functionReturnValue = null;
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }
            finally
            {
                _CloseConnection();     // Close Connection
            }
            return functionReturnValue;

        }
        #endregion

        #region GetDataset
        /// <summary>
        /// Thuc hien 1 cau query tra ve 1 dataset
        /// Input: 1.cau lenh Sql
        /// Output: dataset
        /// </summary>
        /// <param name="SQL">select * from tbl_1 select * from tbl_2</param>
        /// <returns></returns>
        public DataSet GetDatasetFromQuery(string myQuery)
        {
            DataSet functionReturnValue = null;
            if ((_OpenConnection() == false))           // Open Connection
                return null;

            DataSet ds = new DataSet();
            try
            {
                CLog.LogSQL(CBase.GetDeepCaller(), myQuery);    // log SQL 

                m_Command.Connection = m_Connection;
                m_Command.CommandText = myQuery;
                m_Command.CommandType = CommandType.Text;

                DateTime dtBegin = DateTime.Now; // duration  
                m_DataAdapter = new SqlDataAdapter(m_Command);
                m_DataAdapter.Fill(ds);
                this.m_dblDuration = DateTime.Now.Subtract(dtBegin).TotalMilliseconds; // duration
                functionReturnValue = ds;
            }
            catch (Exception ex)
            {
                functionReturnValue = null;
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }
            finally
            {
                _CloseConnection();     // Close Connection
            }
            return functionReturnValue;
        }
        /// <summary>
        /// Thuc hien 1 cau query tra ve 1 dataset
        /// Input: 
        ///         1. Ten store procedure
        /// Output: dataset
        /// </summary>
        /// <returns></returns>
        public DataSet GetDatasetFromSP(string SPname)
        {
            DataSet functionReturnValue = null;
            if ((_OpenConnection() == false))           // Open Connection
                return null;
            DataSet ds = new DataSet();

            try
            {
                CLog.LogSQL(CBase.GetDeepCaller(), SPname + this.GetParamInfo());    // log SQL

                m_Command.Connection = m_Connection;
                m_Command.CommandText = SPname;
                m_Command.CommandType = CommandType.StoredProcedure;
                DateTime dtBegin = DateTime.Now; // duration  
                m_DataAdapter = new SqlDataAdapter(m_Command);
                m_DataAdapter.Fill(ds);
                this.m_dblDuration = DateTime.Now.Subtract(dtBegin).TotalMilliseconds; // duration
                m_Command.Parameters.Clear();
                functionReturnValue = ds;
            }
            catch (Exception ex)
            {
                functionReturnValue = null;
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }
            finally
            {
                _CloseConnection();     // Close Connection
            }
            return functionReturnValue;
        }
        #endregion

        #region GetDataReader
        /// <summary>
        /// Thuc hien 1 câu lệnh sql tra ve 1 DataReader
        /// Input: 
        ///       1. câu lệnh sql
        /// Output: DataReader
        /// </summary>
        /// <param name="SQL">select * from tbl_test</param>
        /// <returns></returns>
        public SqlDataReader GetDataReaderFromQuery(string myQuery)
        {
            if ((_OpenConnection() == false))           // Open Connection
                return null;

            m_Command.Connection = m_Connection;
            m_Command.CommandText = myQuery;
            m_Command.CommandType = CommandType.Text;
            try
            {
                CLog.LogSQL(CBase.GetDeepCaller(), myQuery);    // log SQL

                DateTime dtBegin = DateTime.Now; // duration  
                m_DataReader = m_Command.ExecuteReader();
                this.m_dblDuration = DateTime.Now.Subtract(dtBegin).TotalMilliseconds; // duration
                return m_DataReader;
            }
            catch (Exception ex)
            {
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
                return null;
            }
            finally
            {
                //_CloseConnection(); // _CloseConnection thi error, phai goi CloseConnection tu ben ngoai (Caller)
            }
        }

        /// <summary>
        /// Thuc hien 1 store procedure (co tham so) tra ve 1 DataReader
        /// Input: 
        ///       1. spName: ten store
        ///       2. myParamArray() : danh sach cac tham so 
        /// Output: DataReader
        /// </summary>
        /// <param name="SPname">sp_SELECT</param>
        /// <returns></returns>
        public SqlDataReader GetDataReaderFromSP(string SPname)
        {
            if ((_OpenConnection() == false))           // Open Connection
                return null;

            m_Command.Connection = m_Connection;
            m_Command.CommandText = SPname;
            m_Command.CommandType = CommandType.StoredProcedure;
            try
            {
                CLog.LogSQL(CBase.GetDeepCaller(), SPname + this.GetParamInfo());    // log SQL

                DateTime dtBegin = DateTime.Now; // duration  
                m_DataReader = m_Command.ExecuteReader();
                this.m_dblDuration = DateTime.Now.Subtract(dtBegin).TotalMilliseconds; // duration
                m_Command.Parameters.Clear();
                return m_DataReader;
            }
            catch (Exception ex)
            {
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
                return null;
            }
            finally
            {
                //_CloseConnection(); // _CloseConnection thi error, phai goi CloseConnection tu ben ngoai (Caller)
            }
        }
        #endregion

        #endregion

        #region Excel Methods

        OleDbConnection objExcelConn;
        OleDbCommand objCmd;

        /// <summary>
        /// Hàm thực hiện mở connection
        /// </summary>
        public bool _OpenConnection_Excel(string filename)
        {
            try
            {
                string strExcelConn = "Provider=Microsoft.Jet.OleDb.4.0;" + "data source=" + filename + ";" + "Extended Properties=Excel 8.0;";
                objExcelConn = new OleDbConnection(strExcelConn);
                objExcelConn.Open();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw (ex);
            }
        }
        /// <summary>
        /// Hàm thực hiện đóng connection
        /// </summary>
        public void _CloseConnection_Excel()
        {
            if ((objExcelConn.State == ConnectionState.Open))
            {
                objExcelConn.Close();
            }
        }
        /// <summary>
        /// Thuc hien get 1 datatable từ file excel
        /// Input: 1. file excel
        ///        2. câu lệnh sql
        /// Output: datatable 
        /// </summary>
        public DataTable OpenDataTableFromExcel(string filename, string myQuery)
        {
            DataTable functionReturnValue = null;
            if ((_OpenConnection_Excel(filename) == false))
                return null;
            try
            {
                objCmd = new OleDbCommand(myQuery, objExcelConn);
                DataTable dt = new DataTable();
                OleDbDataAdapter da = new OleDbDataAdapter(objCmd);
                da.Fill(dt);
                functionReturnValue = dt;

            }
            catch (Exception ex)
            {
                return null;
                throw (ex);
            }
            finally
            {
                _CloseConnection_Excel();
            }
            return functionReturnValue;
        }
        /// <summary>
        /// </summary>
        public bool ImportEachRow(string sql)
        {
            bool functionReturnValue = false;
            m_Command.CommandText = sql;
            m_Command.CommandType = CommandType.Text;
            try
            {
                m_Command.ExecuteNonQuery();
                functionReturnValue = true;
            }
            catch (Exception e)
            {
                functionReturnValue = false;
                throw (e);
            }
            return functionReturnValue;
        }

        #endregion
    }     
}
