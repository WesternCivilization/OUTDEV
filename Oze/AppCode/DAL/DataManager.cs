using Oze.AppCode.BLL;
using System;
using System.Data;
using System.Data.SqlClient;

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
	
	public class DataManager
	{        
        #region Các thuộc tính

        #region ConnString

        private string m_ConnectionString;
        public int m_CommandTimeout = Convert.ToInt32(CConfig.SQL_COMMAND_TIMEOUT);

        /// <summary>
        /// Chuỗi thiết lập kết nối tới CSDL
        /// </summary>
        public string ConnString
        {
            get { return m_ConnectionString; }
            set { m_ConnectionString = value; }
        }

        #endregion

        #endregion

		#region public DataManager()
		public DataManager()
		{
            //tam thoi de fix connstr nhu nay. Sau lam nhieu server thi phai truyen bien vao
            this.m_ConnectionString = CConfig.CONNECTIONSTRING_OZEGENERAL;
		}
        #endregion

        #region public static SqlConnection GetConnection()

        /// <summary>
        /// Tạo kết nối đến CSDL
        /// </summary>
        /// <returns>Đối tượng SqlConnection kết nối đến CSDL</returns>
        public static SqlConnection GetConnection()
        {
            string connString = CConfig.CONNECTIONSTRING_OZEGENERAL;
            SqlConnection sqlConn = new SqlConnection(connString);
            return sqlConn;
        }

        #endregion


        #region ExecuteQuery (...)

        #region public static DataSet ExecuteQuery ( sqlQuery )

        /// <summary>
        /// Thực thi các câu lệnh SQL
        /// </summary>
        /// <param name="sqlQuery">Câu lệnh truy vấn</param>
        /// <returns>Đối tượng DataSet chứa kết quả truy vấn</returns>
        public static DataSet ExecuteQuery ( string sqlQuery )
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

		#region public static DataSet ExecuteQuery ( sqlQuery, paramArr )

		/// <summary>
		/// Thực thi các câu lệnh SQL
		/// </summary>
		/// <param name="sqlQuery">Câu lệnh SQL</param>
		/// <param name="paramArr">Mảng chứa các tham số trong câu lệnh</param>
		/// <returns>Đối tượng DataSet chứa kết quả truy vấn</returns>
		public static DataSet ExecuteQuery ( string sqlQuery, IDbDataParameter[] paramArr )
		{
			return DataManager.ExecuteQuery(sqlQuery, paramArr, CommandType.Text);
		}
				
		#endregion

		#region public static DataSet ExecuteQuery ( sqlQuery, sqlCT )

		public static DataSet ExecuteQuery( string sqlQuery, CommandType sqlCT )
		{
			return ExecuteQuery(sqlQuery, null, sqlCT);
		}

		#endregion

		#region public static DataSet ExecuteQuery ( sqlQuery, paramArr, sqlCT )
		/// <summary>
		/// Thực thi các câu lệnh SQL
		/// </summary>
		/// <param name="sqlQuery">Câu lệnh SQL</param>
		/// <param name="paramArr">Mảng chứa các tham số trong câu lệnh</param>
		/// <param name="sqlCT" >Kiểu Store Procedure hay kiểu text</param>
		/// <returns>Đối tượng DataSet chứa kết quả truy vấn</returns>
		public static DataSet ExecuteQuery ( string sqlQuery, IDbDataParameter[] paramArr, CommandType sqlCT)
		{
            DataSet sqlDS = new DataSet();

            //Tạo kết nối tới CSDL
            SqlConnection sqlConn = GetConnection();
            //Mở kết nối
            sqlConn.Open();
            try
            {
                //Tạo đối tượng dbCommand với câu lệnh SQL truyền vào và thiết lập tham số
                SqlCommand sqlCmd = CreateDbCommand(sqlQuery, sqlConn, paramArr, sqlCT);
                //sqlCmd.CommandTimeout = sqlConn.ConnectionTimeout;
                sqlCmd.CommandTimeout = 1500;
                SqlDataAdapter sqlDA = new SqlDataAdapter();
                sqlDA.SelectCommand = sqlCmd;
                sqlDA.Fill(sqlDS);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally {
                sqlConn.Close();
            }
            return sqlDS;
		}

		#endregion

		#endregion

		#region ExecuteReader (...)

		#region SqlDataReader ExecuteReader(sqlQuery, sqlConn)

		public static SqlDataReader ExecuteReader(string sqlQuery, SqlConnection sqlConn)
		{
			return ExecuteReader(sqlQuery, sqlConn, null);			
		}

		#endregion

		#region SqlDataReader ExecuteReader(sqlQuery, sqlConn, sqlCT)
		
		public static SqlDataReader ExecuteReader(string sqlQuery, SqlConnection sqlConn, CommandType sqlCT)
		{
			return ExecuteReader(sqlQuery, sqlConn, null, sqlCT);
		}

		#endregion

		#region SqlDataReader ExecuteReader(sqlQuery, sqlConn, paramArr)

		public static SqlDataReader ExecuteReader(string sqlQuery, SqlConnection sqlConn, IDbDataParameter[] paramArr)
		{
			return ExecuteReader(sqlQuery, sqlConn, paramArr, CommandType.Text);
		}

		#endregion

		#region SqlDataReader ExecuteReader(sqlQuery, sqlConn, paramArr, sqlCT)

		/// <summary>
		/// Lấy về DataReader
		/// </summary>
		/// <param name="sqlQuery">Câu truy vấn lấy dữ liệu</param>
		/// <param name="sqlConn">Đối tượng kết nối cần sử dụng</param>
		/// <param name="paramArr">Mảng chứa các tham số truyền cho câu truy vấn </param>
		/// <param name="sqlCT">Kiểu truy vấn: Text hoặc Stored Procedure</param>
		/// <returns>Đối tượng DataReader chứa dữ liệu cần lấy</returns>
		public static SqlDataReader ExecuteReader(string sqlQuery, SqlConnection sqlConn, IDbDataParameter[] paramArr, CommandType sqlCT)
		{
			try
			{
			
				SqlCommand sqlCmd = CreateDbCommand(sqlQuery, sqlConn, paramArr, sqlCT);
				SqlDataReader sqlDR = sqlCmd.ExecuteReader();
				sqlCmd.Dispose();

				return sqlDR;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		#endregion

		#endregion

		#region ExecuteNonQuery (...)

		#region public static void ExecuteNonQuery(string sqlQuery)
		/// <summary>
		/// Thực hiện các thao tác Insert, Update và Delete dữ liệu trong CSDL
		/// với các câu lệnh truy vấn SQL không sử dụng tham số
		/// </summary>
		/// <param name="sqlQuery">Câu lệnh truy vấn SQL</param>
		public static void ExecuteNonQuery(string sqlQuery)
		{
			ExecuteNonQuery(sqlQuery, null);
		}
		#endregion

		#region public static void ExecuteNonQuery( sqlQuery, paramArr )

		/// <summary>
		/// Thực hiện các thao tác Insert, Update và Delete dữ liệu trong CSDL
		/// với các câu lệnh truy vấn SQL có sử dụng tham số
		/// </summary>
		/// <param name="sqlQuery">Câu lệnh truy vấn SQL</param>
		/// <param name="paramArr">Mảng tham số truyền vào cho câu lệnh SQL</param>
		public static void ExecuteNonQuery(string sqlQuery, IDbDataParameter[] paramArr)
		{
			DataManager.ExecuteNonQuery(sqlQuery, paramArr, CommandType.Text);
		}

		#endregion

		#region public static void ExecuteNonQuery(sqlQuery, sqlCT)

		public static void ExecuteNonQuery(string sqlQuery, CommandType sqlCT)
		{
			ExecuteNonQuery(sqlQuery, null, sqlCT);
		}

		#endregion

		#region public static void ExecuteNonQuery( sqlQuery, paramArr, sqlCT )
		/// <summary>
		/// Thực hiện các thao tác Insert, Update và Delete dữ liệu trong CSDL
		/// với các câu lệnh truy vấn SQL có sử dụng tham số
		/// </summary>
		/// <param name="sqlQuery">Câu lệnh truy vấn SQL</param>
		/// <param name="paramArr">Mảng tham số truyền vào cho câu lệnh SQL</param>
		/// <param name="sqlCT" >Kiểu Store Procedure hay kiểu Text</param>
		public static void ExecuteNonQuery(string sqlQuery, IDbDataParameter[] paramArr, CommandType sqlCT)
        {
            //Tạo kết nối tới CSDL
            SqlConnection sqlConn = GetConnection();
            //Mở kết nối
            sqlConn.Open();

            try
            {
                //Tạo đối tượng dbCommand với câu lệnh SQL truyền vào và thiết lập tham số
                SqlCommand sqlCmd = CreateDbCommand(sqlQuery, sqlConn, paramArr, sqlCT);
                //Thực thi câu lệnh truy vấn
                sqlCmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
            finally {
                sqlConn.Close();
            }
		}


		#endregion

		#endregion

		#region SqlCommand CreateDbCommand( sqlQuery, sqlConn, paramArr, sqlCT )

		/// <summary>
		/// Tạo một đối tượng DbCommand
		/// </summary>
		/// <param name="sqlQuery">Câu lệnh truy vấn SQL</param>
		/// <param name="paramArr">Mảng chứa các tham số dùng trong câu lệnh</param>
		/// <param name="conn">Kết nối</param>
		/// <param name="sqlCT">Kiểu truy vấn: câu lệnh hay stored procedure</param>
		/// <returns>Đối tượng DbCommand</returns>
		public static SqlCommand CreateDbCommand(string sqlQuery, SqlConnection conn, IDbDataParameter[] paramArr, CommandType sqlCT)
		{
			SqlCommand sqlCmd = new SqlCommand(sqlQuery, conn);
			sqlCmd.CommandType = sqlCT;
				
			if (paramArr != null)
			{
				if (paramArr.Length > 0)
				{
					foreach (SqlParameter param in paramArr)
					{
						sqlCmd.Parameters.Add(param);
					}
				}
			}
			return sqlCmd;
		}

		#endregion

	}
}
