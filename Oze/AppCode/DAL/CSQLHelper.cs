using Oze.AppCode.BLL;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace Oze.AppCode.DAL
{
    /// <summary>
	/// REUSE
	/// Created: 2016-10-14 10:56:41 ThichPV
	/// Description: Tạo mới và quản lý
	///  - Tất cả các hàm tạo mới ở đây đều phải được thống nhất và qua review mới được cóp vào
	///  - Nếu hàm nào không qua khâu trên sẽ bị xóa, vì không quản lý được. 
	///  - (các hàm phải được comment đầy đủ, ghi rõ chức năng, các biến truyền vào, kiểu và chức năng của nó làm gì)
	///  - (các kiểu mẫu của comment hàm có thể tham khảo hàm đầu tiên GetConfigurationValue(string ConfigurationKey))
	///  - Thanks,
	/// </summary>
    /// 
    public class CSQLHelper
    {
        private string m_ConnectionString;
        private SqlConnection m_Connection;
        private SqlCommand m_Command = new SqlCommand();
        private SqlDataAdapter m_DataAdapter;
        private SqlDataReader m_DataReader;
        private double m_dblDuration;
        public int m_CommandTimeout = Convert.ToInt32(CConfig.SQL_COMMAND_TIMEOUT);

        #region Public properties

        public string ConnString
        {
            get { return m_ConnectionString; }
            set { m_ConnectionString = value; }
        }

        public SqlConnection Connection
        {
            get { return m_Connection; }
            set { m_Connection = value; }
        }

        public SqlCommand Command
        {
            get { return m_Command; }
            set { m_Command = value; }
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
        
		#region public DataManager()
        public CSQLHelper(string strConnectionString)
	    {
            /*  LAMMN: 07/11/2016
             *  Check connectstring -> return chuỗi connectstring
             */

            System.Data.Common.DbConnectionStringBuilder builder = new System.Data.Common.DbConnectionStringBuilder();

            builder.ConnectionString = strConnectionString;
            string database = builder["database"] as string;
            if(database == "xxxx") //Nếu là chuỗi kết nối tới các member
            {
                if (HttpContext.Current.Session[CConfig.SESSION_HOTELGROUPCODE] != null)
                {
                    string HotelGroup = HttpContext.Current.Session[CConfig.SESSION_HOTELGROUPCODE].ToString().Trim();
                    strConnectionString = strConnectionString.Replace("xxxx", HotelGroup);
                }
                //else //hết phiên làm việc
                //{
                //    HttpContext.Current.Response.Redirect("~/Accounts/Login", false);
                //}
            }

            this.m_ConnectionString = strConnectionString;
            this.m_dblDuration = 0;
		}
        #endregion
        
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

        #region ExecuteQuery (...)

        #region public DataSet ExecuteQuery ( sqlQuery )

        /// <summary>
        /// Thực thi các câu lệnh SQL
        /// </summary>
        /// <param name="sqlQuery">Câu lệnh truy vấn</param>
        /// <returns>Đối tượng DataSet chứa kết quả truy vấn</returns>
        public DataSet ExecuteQuery ( string sqlQuery )
		{
			try
			{
				DataSet sqlDS = ExecuteQuery(sqlQuery, null);
				return sqlDS;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
				
		#endregion

		#region public DataSet ExecuteQuery ( sqlQuery, paramArr )

		/// <summary>
		/// Thực thi các câu lệnh SQL
		/// </summary>
		/// <param name="sqlQuery">Câu lệnh SQL</param>
		/// <param name="paramArr">Mảng chứa các tham số trong câu lệnh</param>
		/// <returns>Đối tượng DataSet chứa kết quả truy vấn</returns>
		public DataSet ExecuteQuery ( string sqlQuery, SqlParameter[] paramArr )
		{
            try
			{
                DataSet ds = ExecuteQuery(sqlQuery, paramArr, CommandType.Text);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
		}
				
		#endregion

		#region public DataSet ExecuteQuery ( sqlQuery, sqlCT )

		public DataSet ExecuteQuery( string sqlQuery, CommandType sqlCT )
		{
            try
            {
			    DataSet ds = ExecuteQuery(sqlQuery, null, sqlCT);
                return ds;
            }
			catch (Exception ex)
			{
				throw ex;
			}
		}

		#endregion

		#region public DataSet ExecuteQuery ( sqlQuery, paramArr, sqlCT )
		/// <summary>
		/// Thực thi các câu lệnh SQL
		/// </summary>
		/// <param name="sqlQuery">Câu lệnh SQL</param>
		/// <param name="paramArr">Mảng chứa các tham số trong câu lệnh</param>
		/// <param name="sqlCT" >Kiểu Store Procedure hay kiểu text</param>
		/// <returns>Đối tượng DataSet chứa kết quả truy vấn</returns>
		public DataSet ExecuteQuery ( string sqlQuery, SqlParameter[] paramArr, CommandType sqlCT)
		{
            DataSet functionReturnValue = null;
            if (_OpenConnection() == false)           // Open Connection
                return null;

            DataSet sqlDS123 = new DataSet();

            //Tạo đối tượng dbCommand với câu lệnh SQL truyền vào và thiết lập tham số
            SqlCommand sqlCmd = CreateDbCommand(sqlQuery, paramArr, sqlCT);

            try
            {
                /*Log before execute*/
                CLog.LogSQL(GetParamInfo(), sqlQuery);

                sqlCmd.CommandTimeout = m_CommandTimeout;
                m_DataAdapter = new SqlDataAdapter();
                m_DataAdapter.SelectCommand = sqlCmd;
                m_DataAdapter.Fill(sqlDS123);
                sqlCmd.Parameters.Clear();
                functionReturnValue = sqlDS123;
            }
            catch (Exception ex)
            {
                /*Log error execute*/
                CLog.LogError(GetParamInfo(), ex.Message);

                functionReturnValue = null;
                throw ex;
            }
            finally {
                _CloseConnection();
            }
            return functionReturnValue;
		}

		#endregion

		#endregion

		#region ExecuteReader (...)

        #region public SqlDataReader ExecuteReader(sqlQuery, sqlConn)

        public SqlDataReader ExecuteReader(string sqlQuery)
		{
			return ExecuteReader(sqlQuery, null);			
		}

		#endregion

        #region public SqlDataReader ExecuteReader(sqlQuery, sqlConn, sqlCT)

        public SqlDataReader ExecuteReader(string sqlQuery, CommandType sqlCT)
		{
			return ExecuteReader(sqlQuery, null, sqlCT);
		}

		#endregion

        #region public SqlDataReader ExecuteReader(sqlQuery, sqlConn, paramArr)

        public SqlDataReader ExecuteReader(string sqlQuery, SqlParameter[] paramArr)
		{
			return ExecuteReader(sqlQuery, paramArr, CommandType.Text);
		}

		#endregion

        #region public SqlDataReader ExecuteReader(sqlQuery, paramArr, sqlCT)

        /// <summary>
		/// Lấy về DataReader
		/// </summary>
		/// <param name="sqlQuery">Câu truy vấn lấy dữ liệu</param>
		/// <param name="sqlConn">Đối tượng kết nối cần sử dụng</param>
		/// <param name="paramArr">Mảng chứa các tham số truyền cho câu truy vấn </param>
		/// <param name="sqlCT">Kiểu truy vấn: Text hoặc Stored Procedure</param>
		/// <returns>Đối tượng DataReader chứa dữ liệu cần lấy</returns>
		public SqlDataReader ExecuteReader(string sqlQuery, SqlParameter[] paramArr, CommandType sqlCT)
		{
            if ((_OpenConnection() == false))           // Open Connection
                return null;

            //Tạo đối tượng dbCommand với câu lệnh SQL truyền vào và thiết lập tham số
            SqlCommand sqlCmd = CreateDbCommand(sqlQuery, paramArr, sqlCT);

			try
			{			
                /*Log before execute*/
                CLog.LogSQL(GetParamInfo(),sqlQuery);

                m_DataReader = sqlCmd.ExecuteReader();
                sqlCmd.Parameters.Clear();
				sqlCmd.Dispose();
			}
			catch (Exception ex)
			{
                /*Log error execute*/
                CLog.LogError(GetParamInfo(), ex.Message);

                m_DataReader = null;
				throw ex;
			}
            finally
            {
                _CloseConnection();
            }
            return m_DataReader;
            
		}

		#endregion

		#endregion

		#region ExecuteNonQuery (...)

        #region public bool ExecuteNonQuery(string sqlQuery)
        /// <summary>
		/// Thực hiện các thao tác Insert, Update và Delete dữ liệu trong CSDL
		/// với các câu lệnh truy vấn SQL không sử dụng tham số
		/// </summary>
		/// <param name="sqlQuery">Câu lệnh truy vấn SQL</param>
        public bool ExecuteNonQuery(string sqlQuery)
		{
            bool functionReturnValue = false;
            try
            {
                ExecuteNonQuery(sqlQuery, null);
                functionReturnValue = true;
            }
            catch(Exception ex)
            {
                functionReturnValue = false;
                throw ex;
            }
            return functionReturnValue;
			
		}
		#endregion

        #region public bool ExecuteNonQuery( sqlQuery, paramArr )

        /// <summary>
		/// Thực hiện các thao tác Insert, Update và Delete dữ liệu trong CSDL
		/// với các câu lệnh truy vấn SQL có sử dụng tham số
		/// </summary>
		/// <param name="sqlQuery">Câu lệnh truy vấn SQL</param>
		/// <param name="paramArr">Mảng tham số truyền vào cho câu lệnh SQL</param>
        public bool ExecuteNonQuery(string sqlQuery, SqlParameter[] paramArr)
		{			
            bool functionReturnValue = false;
            try
            {
                ExecuteNonQuery(sqlQuery, paramArr, CommandType.Text);
                functionReturnValue = true;
            }
            catch (Exception ex)
            {
                functionReturnValue = false;
                throw ex;
            }
            return functionReturnValue;
		}

		#endregion

        #region public bool ExecuteNonQuery(sqlQuery, sqlCT)

        public bool ExecuteNonQuery(string sqlQuery, CommandType sqlCT)
		{
            bool functionReturnValue = false;
            try
            {
                ExecuteNonQuery(sqlQuery, null, sqlCT);
                functionReturnValue = true;
            }
            catch (Exception ex)
            {
                functionReturnValue = false;
                throw ex;
            }
            return functionReturnValue;			
		}

		#endregion

        #region public bool ExecuteNonQuery( sqlQuery, paramArr, sqlCT )
        /// <summary>
		/// Thực hiện các thao tác Insert, Update và Delete dữ liệu trong CSDL
		/// với các câu lệnh truy vấn SQL có sử dụng tham số
		/// </summary>
		/// <param name="sqlQuery">Câu lệnh truy vấn SQL</param>
		/// <param name="paramArr">Mảng tham số truyền vào cho câu lệnh SQL</param>
		/// <param name="sqlCT" >Kiểu Store Procedure hay kiểu Text</param>
		public bool ExecuteNonQuery(string sqlQuery, SqlParameter[] paramArr, CommandType sqlCT)
        {
            bool functionReturnValue = false;
            if ((_OpenConnection() == false))           // Open Connection
                return false;

            //Tạo đối tượng dbCommand với câu lệnh SQL truyền vào và thiết lập tham số
            SqlCommand sqlCmd = CreateDbCommand(sqlQuery, paramArr, sqlCT);

            try
            {
                /*Log before execute*/
                CLog.LogSQL(GetParamInfo(), sqlQuery);

                //Thực thi câu lệnh truy vấn
                sqlCmd.ExecuteNonQuery();
                sqlCmd.Parameters.Clear();
                functionReturnValue = true;
            }
            catch (SqlException ex)
            {
                /*Log error execute*/
                CLog.LogError(GetParamInfo(), ex.Message);

                functionReturnValue = false;
                throw ex;
            }
            finally {
                _CloseConnection();
            }
            return functionReturnValue;
		}


		#endregion

		#endregion

        #region public SqlCommand CreateDbCommand( sqlQuery, paramArr, sqlCT )

        /// <summary>
		/// Tạo một đối tượng DbCommand
		/// </summary>
		/// <param name="sqlQuery">Câu lệnh truy vấn SQL</param>
		/// <param name="paramArr">Mảng chứa các tham số dùng trong câu lệnh</param>
		/// <param name="conn">Kết nối</param>
		/// <param name="sqlCT">Kiểu truy vấn: câu lệnh hay stored procedure</param>
		/// <returns>Đối tượng DbCommand</returns>
		public SqlCommand CreateDbCommand(string sqlQuery, SqlParameter[] paramArr, CommandType sqlCT)
		{
            m_Command.CommandText = sqlQuery;
            m_Command.CommandType = sqlCT;
            m_Command.Connection = m_Connection;
			if (paramArr != null)
			{
				if (paramArr.Length > 0)
				{
					foreach (SqlParameter param in paramArr)
					{
                        m_Command.Parameters.Add(param);
					}
				}
			}
            return m_Command;
		}

		#endregion
    }
}
