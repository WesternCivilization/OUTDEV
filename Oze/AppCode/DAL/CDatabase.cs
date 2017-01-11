using oze.data;
using StoredProcedures;
using Oze.AppCode.BLL;
using Oze.Models;
using ServiceStack.OrmLite.SqlServer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using ServiceStack.OrmLite;
using System.Linq;
using System.Web.Script.Serialization;

namespace Oze.AppCode.DAL
{
    public class CDatabase
    {
        IOzeConnectionFactory _connectionData;
        //Tạo kết nối cho database dùng chung
        CSQLHelper helper = new CSQLHelper(CConfig.CONNECTIONSTRING_OZEGENERAL);
        //Tạo kết nối cho database của từng cụm khách sạn
        //CSQLHelper helperClient = new CSQLHelper(CConfig.CONNECTIONSTRING_OZMEMBER);
        CSQLHelper helperClient = new CSQLHelper(CConfig.CONNECTIONSTRING_OZEGENERAL);
        //Khởi tạo parameter 
        private SqlParameter[] param;

        const string TEMPLATE_JSON_CUSTOMER_BOOKING = "{\"pageCount\":\"(pageCount)\",\"data\":[(data)]}";
        const string TEMPLATE_JSON_CUSTOMER_BOOKING_INFO = "{\"CustomerName\":\"(CustomerName)\",\"BookingCode\":\"(BookingCode)\",\"ReservationType\":\"(ReservationType)\",\"ArrivalDate\":\"(ArrivalDate)\",\"LeaveDate\":\"(LeaveDate)\",\"ReservationStatus\":\"(ReservationStatus)\"}";

        public CDatabase()
        {
            _connectionData = new OzeConnectionFactory(CConfig.CONNECTIONSTRING_OZEGENERAL, SqlServerOrmLiteDialectProvider.Instance);

        }

        #region "ThichPV - 22/10/2016"    

        /// <summary>
        /// Gán Phòng cho Booking
        /// </summary>
        /// <param name="HotelCode"></param>
        /// <param name="BookingCode"></param>
        /// <param name="RoomID"></param>
        /// <returns>Trả về thông báo</returns>
        public List<string> Create_RoomforReservationRoom(tbl_Reservation_Room_tbl_Room_Rel model, int UserID)
        {
            //Gán phòng cho booking
            List<string> json = new List<string>();
            using (var db = _connectionData.OpenDbConnection())
            {
                try
                {
                    long id = 0;
                    using (var tran = db.OpenTransaction())//if need
                    {
                        var obj = new tbl_Reservation_Room_tbl_Room_Rel();
                        obj.Id = model.Id;
                        obj.BookingCode = model.BookingCode;
                        obj.HotelCode = model.HotelCode;
                        obj.New_Room_ID = model.New_Room_ID;
                        //obj.Old_Room_ID = model.Old_Room_ID;
                        var querys = db.From<tbl_Reservation_Room_tbl_Room_Rel>().Where(e => e.Id == obj.Id);
                        var objUpdate = db.Select(querys).SingleOrDefault();
                        //Kiểm tra xem booking đã gán phòng chưa
                        if (objUpdate != null) //Đã gán rồi thì update
                        {
                            var query = db.From<tbl_Reservation_Room_tbl_Room_Rel>().Where(e => e.BookingCode == model.BookingCode).Where(e => e.New_Room_ID == model.New_Room_ID);
                            if (db.Select(query).Count() == 0) //Check phòng được gán lại đã gán cho booking này chưa
                            {
                                objUpdate.Old_Room_ID = objUpdate.New_Room_ID;
                                objUpdate.New_Room_ID = obj.New_Room_ID;

                                id = db.Update(objUpdate);
                            }
                            else
                                json.Add("Phòng đã được gán. Chọn phòng khác");
                        }
                        else //Chưa đc gán thì thêm mới
                        {
                            id = db.Insert(obj, selectIdentity: true);
                        }
                        tran.Commit();
                    }

                    if (id != 0)
                    {
                        json = GetReservationRoom_Detail(model.BookingCode, model.HotelCode, UserID);
                    }
                }
                catch (Exception ex)
                {
                    json.Add("Gán phòng không thành công!");
                    CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
                }
            }
            return json;
        }

        public List<view_GetRoomForRoomType> Getview_GetRoomForRoomType(string RoomTypeID)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<view_GetRoomForRoomType>().Where(e => e.RoomType_ID == int.Parse(RoomTypeID));
                return db.Select(query).ToList();
            }
        }

        /// <summary>
        /// Đổi phòng/Ngày
        /// </summary>
        /// <param name="ReserModel"></param>
        /// <param name="Rel_Model"></param>
        /// <returns>string Message Success/Error</returns>
        public string Transfer_SetRoomForBooking(tbl_Reservation_Room ReserModel, tbl_Reservation_Room_tbl_Room_Rel Rel_Model)
        {
            string message = "";
            int result1 = 0;
            int result2 = 0;
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Reservation_Room_tbl_Room_Rel>().Where(e => e.Id == Rel_Model.Id);
                var query1 = db.From<tbl_Reservation_Room>().Where(e => e.BookingCode == ReserModel.BookingCode);
                var isUpdate = db.Select(query).SingleOrDefault();
                var isUpdateReser = db.Select(query1).SingleOrDefault();

                if (isUpdate == null) //Booking này đã được gán phòng chưa
                {
                    message = "Đặt phòng chưa được gán. Bạn không được đổi phòng";
                }
                else
                {
                    if(isUpdateReser != null) //Update thông tin đặt phòng
                    {
                        isUpdateReser.Holiday = ReserModel.Holiday;
                        isUpdateReser.KhungGio = ReserModel.KhungGio;
                        isUpdateReser.Price = ReserModel.Price;
                        isUpdateReser.Arrive_Date = ReserModel.Arrive_Date;
                        isUpdateReser.Leave_Date = ReserModel.Leave_Date;
                        isUpdateReser.Number_People = ReserModel.Number_People;
                        isUpdateReser.Number_Children = ReserModel.Number_Children;
                        isUpdateReser.Payment_Type_ID = ReserModel.Payment_Type_ID;
                        isUpdateReser.Deduction = ReserModel.Deduction;
                        isUpdateReser.Deposit = ReserModel.Deposit;

                        result1 = db.Update(isUpdateReser);
                    }
                    if (isUpdate != null) // Update thông tin chuyển phòng
                    {
                        isUpdate.Old_Room_ID = isUpdate.New_Room_ID;
                        isUpdate.New_Room_ID = Rel_Model.New_Room_ID;
                        result2 = db.Update(isUpdate);
                    }
                    if(result1 != 0 && result2 !=0)
                    {
                        message = "Đổi phòng thành công!";
                    }
                    else
                    {
                        message = "Đổi phòng không thành công!";
                    }
                }

                return message;
            }
        }

        public tbl_Reservation_Room_tbl_Room_Rel GetReservation_Room_tbl_Room_RelByID(string id)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Reservation_Room_tbl_Room_Rel>().Where(e => e.Id == int.Parse(id));
                return db.Select(query).SingleOrDefault();
            }
        }
        public tbl_Reservation_Room GetReservation_RoomByID(string id)
        {
            using (var db = _connectionData.OpenDbConnection())
            {
                var query = db.From<tbl_Reservation_Room>().Where(e => e.Id == int.Parse(id));
                return db.Select(query).SingleOrDefault();
            }
        }

        /// <summary>
        /// Get list menu cho vào dropdownlist
        /// </summary>
        /// <returns></returns>
        public List<DefineCategoryListModel> GetList_DefineCategory(string Type, int UserID)
        {
            List<DefineCategoryListModel> _buildList = new List<DefineCategoryListModel>();
            DataSet ds = new DataSet();
            try
            {
                param = new SqlParameter[2];
                param[0] = new SqlParameter("@Type", SqlDbType.NVarChar,100);
                param[0].Direction = ParameterDirection.Input;
                param[0].Value = Type;
                param[1] = new SqlParameter("@UserID", SqlDbType.Int);
                param[1].Direction = ParameterDirection.Input;
                param[1].Value = UserID;
                ds = helperClient.ExecuteQuery(CConfig.SP_GET_CATEGORY, param, CommandType.StoredProcedure);
                {
                    foreach (DataRow _row in ds.Tables[0].Rows)
                    {
                        _buildList.Add(new DefineCategoryListModel
                        {
                            ID = Convert.ToInt32(_row["ID"].ToString()),
                            Value = _row["Value"].ToString(),
                            Name = _row["Name"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _buildList = null;
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }

            return _buildList;
        }

        /// <summary>
        /// Lay danh sach loai hinh khach dat phong 
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public List<DefineCategoryListModel> GetList_ReservationType(int UserID)
        {
            List<DefineCategoryListModel> _buildList = new List<DefineCategoryListModel>();
            DataSet ds = new DataSet();
            try
            {
                param = new SqlParameter[1];
                param[0] = new SqlParameter("@UserID", SqlDbType.Int);
                param[0].Direction = ParameterDirection.Input;
                param[0].Value = UserID;
                ds = helperClient.ExecuteQuery(CConfig.SP_GET_RESERVATION_TYPE, param, CommandType.StoredProcedure);
                {
                    foreach (DataRow _row in ds.Tables[0].Rows)
                    {
                        _buildList.Add(new DefineCategoryListModel
                        {
                            Value = _row["Value"].ToString(),
                            Name = _row["Name"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _buildList = null;
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }

            return _buildList;
        }

        /// <summary>
        /// Lay danh sach loai dat phong
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public List<DefineCategoryListModel> GetList_RoomType(int UserID)
        {
            List<DefineCategoryListModel> _buildList = new List<DefineCategoryListModel>();
            DataSet ds = new DataSet();
            try
            {
                param = new SqlParameter[1];
                param[0] = new SqlParameter("@UserID", SqlDbType.Int);
                param[0].Direction = ParameterDirection.Input;
                param[0].Value = UserID;
                ds = helperClient.ExecuteQuery(CConfig.SP_GET_ROOM_TYPE, param, CommandType.StoredProcedure);
                {
                    foreach (DataRow _row in ds.Tables[0].Rows)
                    {
                        _buildList.Add(new DefineCategoryListModel
                        {
                            Value = _row["Value"].ToString(),
                            Name = _row["Name"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _buildList = null;
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }

            return _buildList;
        }

        /// <summary>
        /// Lay danh sach Trang thai dat phong
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public List<DefineCategoryListModel> GetList_ReservationStatus(int UserID)
        {
            List<DefineCategoryListModel> _buildList = new List<DefineCategoryListModel>();
            DataSet ds = new DataSet();
            try
            {
                param = new SqlParameter[1];
                param[0] = new SqlParameter("@UserID", SqlDbType.Int);
                param[0].Direction = ParameterDirection.Input;
                param[0].Value = UserID;
                ds = helperClient.ExecuteQuery(CConfig.SP_GET_ROOM_STATUS, param, CommandType.StoredProcedure);
                {
                    foreach (DataRow _row in ds.Tables[0].Rows)
                    {
                        _buildList.Add(new DefineCategoryListModel
                        {
                            Value = _row["Value"].ToString(),
                            Name = _row["Name"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _buildList = null;
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }

            return _buildList;
        }

        /// <summary>
        /// Lay danh sach phong
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public List<DefineCategoryListModel> GetList_Room(int UserID)
        {
            List<DefineCategoryListModel> _buildList = new List<DefineCategoryListModel>();
            DataSet ds = new DataSet();
            try
            {
                param = new SqlParameter[1];
                param[0] = new SqlParameter("@UserID", SqlDbType.Int);
                param[0].Direction = ParameterDirection.Input;
                param[0].Value = UserID;
                ds = helper.ExecuteQuery(CConfig.SP_GET_ROOM, param, CommandType.StoredProcedure);
                {
                    foreach (DataRow _row in ds.Tables[0].Rows)
                    {
                        _buildList.Add(new DefineCategoryListModel
                        {
                            Value = _row["Value"].ToString(),
                            Name = _row["Name"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _buildList = null;
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }

            return _buildList;
        }

        /// <summary>
        /// Lay danh sach quốc gia, quốc tịch
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public List<DefineCategoryListModel> GetList_Country(int UserID)
        {
            List<DefineCategoryListModel> _buildList = new List<DefineCategoryListModel>();
            DataSet ds = new DataSet();
            try
            {
                param = new SqlParameter[1];
                param[0] = new SqlParameter("@UserID", SqlDbType.Int);
                param[0].Direction = ParameterDirection.Input;
                param[0].Value = UserID;
                ds = helper.ExecuteQuery(CConfig.SP_GET_COUNTRY, param, CommandType.StoredProcedure);
                {
                    foreach (DataRow _row in ds.Tables[0].Rows)
                    {
                        _buildList.Add(new DefineCategoryListModel
                        {
                            Value = _row["Value"].ToString(),
                            Name = _row["Name"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _buildList = null;
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }

            return _buildList;
        }


        public string getObjectCustomer_Booking(ReservationRoomModel mdReservationRoom,string HotelCode, int UserID, int CurrentPage, int NumInPage)
        {
            string strJSON = "";
            string strJSONChild = "";
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            param = new SqlParameter[12];
            param[0] = new SqlParameter("@BookingCode", SqlDbType.NVarChar,50);
            param[0].Direction = ParameterDirection.Input;
            param[0].Value = mdReservationRoom.ReservationCode;
            param[1] = new SqlParameter("@ReservationType", SqlDbType.Int);
            param[1].Direction = ParameterDirection.Input;
            param[1].Value = mdReservationRoom.ReservationType;
            param[2] = new SqlParameter("@ReservationStatus", SqlDbType.Int);
            param[2].Direction = ParameterDirection.Input;
            param[2].Value = mdReservationRoom.ReservationStatus;
            param[3] = new SqlParameter("@RoomID", SqlDbType.Int);
            param[3].Direction = ParameterDirection.Input;
            param[3].Value = mdReservationRoom.RoomID;
            param[4] = new SqlParameter("@RoomType", SqlDbType.Int);
            param[4].Direction = ParameterDirection.Input;
            param[4].Value = mdReservationRoom.RoomTypeID;
            param[5] = new SqlParameter("@Arrive_Date", SqlDbType.NVarChar,15);
            param[5].Direction = ParameterDirection.Input;
            param[5].Value = mdReservationRoom.ArrivalDate;
            param[6] = new SqlParameter("@Leave_Date", SqlDbType.NVarChar,15);
            param[6].Direction = ParameterDirection.Input;
            param[6].Value = mdReservationRoom.LeaveDate;
            param[7] = new SqlParameter("@CustomerName", SqlDbType.NVarChar,200);
            param[7].Direction = ParameterDirection.Input;
            param[7].Value = mdReservationRoom.CustomerName;
            param[8] = new SqlParameter("@PageNumber", SqlDbType.Int);
            param[8].Direction = ParameterDirection.Input;
            param[8].Value = CurrentPage;
            param[9] = new SqlParameter("@RowspPage", SqlDbType.Int);
            param[9].Direction = ParameterDirection.Input;
            param[9].Value = NumInPage;
            param[10] = new SqlParameter("@UserID", SqlDbType.Int);
            param[10].Direction = ParameterDirection.Input;
            param[10].Value = UserID;
            param[11] = new SqlParameter("@HotelCode", SqlDbType.NVarChar,50);
            param[11].Direction = ParameterDirection.Input;
            param[11].Value = HotelCode;
            ds = helper.ExecuteQuery(CConfig.SP_GET_RESERVATION_ROOM, param, CommandType.StoredProcedure);
            //Them tong so trang tim kiem vao obj
            
            //result.Add(ds.Tables[0].Rows[0]["TOTAL_PAGE"].ToString());
            //Them data vao obj
            dt = ds.Tables[1];
            
                                                    
            foreach (DataRow rw in dt.AsEnumerable())
            {
                StringBuilder sb = new StringBuilder(TEMPLATE_JSON_CUSTOMER_BOOKING_INFO);
                sb.Replace("(CustomerName)", rw["CustomerName"].ToString());
                sb.Replace("(BookingCode)", rw["BookingCode"].ToString());
                sb.Replace("(ReservationType)", rw["Reservation_Type_Name"].ToString());
                sb.Replace("(ArrivalDate)", rw["Arrive_Date"].ToString());
                sb.Replace("(LeaveDate)", rw["Leave_Date"].ToString());
                sb.Replace("(ReservationStatus)", rw["Room_Status_Name"].ToString());
                strJSONChild += sb.ToString() + ",";
            }
            string data = strJSONChild == ""? strJSONChild:strJSONChild.Substring(0, strJSONChild.Length - 1);
            strJSON = TEMPLATE_JSON_CUSTOMER_BOOKING.Replace("(pageCount)", ds.Tables[0].Rows[0]["TOTAL_PAGE"].ToString())
                                                    .Replace("(data)", data);

                //result.Add(pageCount = ds.Tables[0].Rows[0]["TOTAL_PAGE"].ToString());
            return strJSON;
        }

        public List<string> GetReservationRoom_Detail(string BookingCode, string HotelCode, int UserID)
        {
            List<string> json = new List<string>();
            List<DetailBooking> result = new List<DetailBooking>();
            List<DetailBooking> result1 = new List<DetailBooking>();
            List<Customer_Booking> result2 = new List<Customer_Booking>();
            DataSet ds = new DataSet();
            try
            {
                param = new SqlParameter[3];
                param[0] = new SqlParameter("@BookingCode", SqlDbType.NVarChar, 50);
                param[0].Direction = ParameterDirection.Input;
                param[0].Value = BookingCode;
                param[1] = new SqlParameter("@HotelCode", SqlDbType.NVarChar, 50);
                param[1].Direction = ParameterDirection.Input;
                param[1].Value = HotelCode;
                param[2] = new SqlParameter("@UserID", SqlDbType.Int);
                param[2].Direction = ParameterDirection.Input;
                param[2].Value = UserID;
                ds = helper.ExecuteQuery(CConfig.SP_GET_RESERVATION_ROOM_DETAIL, param, CommandType.StoredProcedure);
               
                json.Add(CBase.convertDataTableToStringJSON(ds.Tables[0]));
                json.Add(CBase.convertDataTableToStringJSON(ds.Tables[1]));
                json.Add(CBase.convertDataTableToStringJSON(ds.Tables[2]));
                
            }
            catch (Exception ex)
            {
                result = null;
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }

            return json;
        }
        
        public List<GroupTypeModels> GetGroupTypeList(int UserID)
        {
            List<GroupTypeModels> _buildList = new List<GroupTypeModels>();
            DataSet ds = new DataSet();
            try
            {
                param = new SqlParameter[1];
                param[0] = new SqlParameter("@UserID", SqlDbType.Int);
                param[0].Direction = ParameterDirection.Input;
                param[0].Value = UserID;
                ds = helper.ExecuteQuery(CConfig.SP_GET_GROUPTYPE,param, CommandType.StoredProcedure);
                {
                    foreach (DataRow _row in ds.Tables[0].Rows)
                    {
                        _buildList.Add(new GroupTypeModels
                        {
                            Id = Convert.ToInt32(_row["ID"].ToString().Trim()),
                            NameVN = _row["NameVN"].ToString().Trim(),
                            NameEN = _row["NameEN"].ToString().Trim(),
                            Order = Convert.ToInt32(_row["Order"].ToString().Trim())
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _buildList = null;
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }

            return _buildList;
        }

        public List<GroupTypeModels> GetListGroupTypeforGroup(string SysHotelCode)
        {
            List<GroupTypeModels> _buildList = new List<GroupTypeModels>();
            DataSet ds = new DataSet();
            try
            {
                param = new SqlParameter[1];
                param[0] = new SqlParameter("@SysHotelCode", SqlDbType.NVarChar,50);
                param[0].Direction = ParameterDirection.Input;
                param[0].Value = SysHotelCode;
                ds = helper.ExecuteQuery(CConfig.SP_GET_GROUPS, param, CommandType.StoredProcedure);
                if(ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow _row in ds.Tables[0].Rows)
                    {
                        _buildList.Add(new GroupTypeModels
                        {
                            Id = Convert.ToInt32(_row["ID"].ToString().Trim()),
                            NameVN = _row["GroupType"].ToString().Trim()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _buildList = null;
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }

            return _buildList;
        }
        /// <summary>
        /// Lấy danh sách các Group
        /// </summary>
        /// <param name="SysHotelCode"></param>
        /// <returns></returns>
        public List<GroupDetailModel> GetListGroup(string SysHotelCode)
        {
            List<GroupDetailModel> _buildList = new List<GroupDetailModel>();
            DataSet ds = new DataSet();
            try
            {
                param = new SqlParameter[1];
                param[0] = new SqlParameter("@SysHotelCode", SqlDbType.NVarChar,50);
                param[0].Direction = ParameterDirection.Input;
                param[0].Value = SysHotelCode;
                ds = helper.ExecuteQuery(CConfig.SP_GET_GROUPS, param, CommandType.StoredProcedure);
                {
                    foreach (DataRow _row in ds.Tables[1].Rows)
                    {
                        _buildList.Add(new GroupDetailModel
                        {
                            ID = Convert.ToInt32(_row["ID"].ToString()),
                            GroupName = _row["GroupName"].ToString(),
                            GroupType = _row["GroupType"].ToString()==""? -1 : Convert.ToInt32(_row["GroupType"].ToString()),
                            GroupTypeName = _row["GroupTypeName"].ToString(),
                            SysHotelCode = _row["SysHotelCode"].ToString(),
                            SysHotelName = _row["SysHotelName"].ToString(),
                            Description = _row["Description"].ToString(),
                            Status = Convert.ToInt32(_row["Status"].ToString()) == 1 ? Status.Open : Status.Close,
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _buildList = null;
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }

            return _buildList;
        }

        /// <summary>
        /// Lấy thông tin 1 group quyền
        /// </summary>
        /// <param name="GroupID"></param>
        /// <returns></returns>
        public GroupModels GetGroupByID(int GroupID)
        {
            GroupModels Group = new GroupModels();
            DataSet ds = new DataSet();
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@ID", SqlDbType.Int);
                para[0].Direction = ParameterDirection.Input;
                para[0].Value = GroupID;
                ds = helper.ExecuteQuery(CConfig.SP_GET_GROUPS_BY_ID, para, CommandType.StoredProcedure);
                {
                    foreach (DataRow _row in ds.Tables[0].Rows)
                    {
                        Group.Id = Convert.ToInt32(_row["ID"].ToString());
                        Group.GroupName = _row["GroupName"].ToString();
                        Group.GroupType = Convert.ToInt32(_row["GroupType"].ToString());
                        Group.Description = _row["Description"].ToString();
                        Group.SysHotelCode = _row["SysHotelCode"].ToString();
                        Group.Status = Convert.ToInt32(_row["Status"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Group = null;
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }

            return Group;
        }

        /// <summary>
        /// Tạo nhóm quyền
        /// </summary>
        /// <param name="Group"></param>
        /// <returns></returns>
        public DataSet CreateGroup(GroupDetailModel Group)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlParameter[] para = new SqlParameter[6];
                para[0] = new SqlParameter("@GroupName", SqlDbType.NVarChar,100);
                para[0].Direction = ParameterDirection.Input;
                para[0].Value = Group.GroupName;
                para[1] = new SqlParameter("@GroupType", SqlDbType.NVarChar, 100);
                para[1].Direction = ParameterDirection.Input;
                para[1].Value = Group.GroupType;
                para[2] = new SqlParameter("@Description", SqlDbType.NVarChar,200);
                para[2].Direction = ParameterDirection.Input;
                para[2].Value = Group.Description;
                para[3] = new SqlParameter("@SysHotelCode", SqlDbType.NVarChar,50);
                para[3].Direction = ParameterDirection.Input;
                para[3].Value = Group.SysHotelCode;
                para[4] = new SqlParameter("@Status", SqlDbType.Int);
                para[4].Direction = ParameterDirection.Input;
                para[4].Value = Group.Status;
                para[5] = new SqlParameter("@UserID", SqlDbType.Int);
                para[5].Direction = ParameterDirection.Input;
                para[5].Value = Group.UserID;

                ds = helper.ExecuteQuery(CConfig.SP_CREATE_GROUPS, para, CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                ds = null;
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }

            return ds;
        }

        public DataSet CreateAccessRight (int GroupID, string RuleID, string Read, string Write, string Create, string Delete)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlParameter[] para = new SqlParameter[6];
                para[0] = new SqlParameter("@GroupID", SqlDbType.Int);
                para[0].Direction = ParameterDirection.Input;
                para[0].Value = GroupID;
                para[1] = new SqlParameter("@RuleID", SqlDbType.VarChar, 8000);
                para[1].Direction = ParameterDirection.Input;
                para[1].Value = RuleID;
                para[2] = new SqlParameter("@Read", SqlDbType.VarChar, 8000);
                para[2].Direction = ParameterDirection.Input;
                para[2].Value = Read;
                para[3] = new SqlParameter("@Write", SqlDbType.VarChar, 8000);
                para[3].Direction = ParameterDirection.Input;
                para[3].Value = Write;
                para[4] = new SqlParameter("@Create", SqlDbType.VarChar, 8000);
                para[4].Direction = ParameterDirection.Input;
                para[4].Value = Create;
                para[5] = new SqlParameter("@Delete", SqlDbType.VarChar, 8000);
                para[5].Direction = ParameterDirection.Input;
                para[5].Value = Delete;

                ds = helper.ExecuteQuery(CConfig.SP_UPDATE_ACCESSRIGHT, para, CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                ds = null;
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }

            return ds;
        }

        /// <summary>
        /// Lấy danh sách các luật cho từng nhóm quyền
        /// </summary>
        /// <param name="GroupID"></param>
        /// <returns></returns>
        public List<AccessRightModels> GetAccessRights(int GroupID)
        {
            List<AccessRightModels> _buildList = new List<AccessRightModels>();
            DataSet ds = new DataSet();
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@GroupID", SqlDbType.Int);
                param[0].Direction = ParameterDirection.Input;
                param[0].Value = GroupID;

                ds = helper.ExecuteQuery(CConfig.SP_GET_ACCESSRIGHT, param, CommandType.StoredProcedure);
                {
                    foreach (DataRow _row in ds.Tables[0].Rows)
                    {
                        _buildList.Add(new AccessRightModels
                        {
                            RuleID = _row["RuleID"].ToString() == "" ? 0 : Convert.ToInt32(_row["RuleID"].ToString()),
                            GroupID = _row["GroupID"].ToString() == "" ? 0 : Convert.ToInt32(_row["GroupID"].ToString()),
                            GroupName = _row["GroupName"].ToString(),
                            ModelID = _row["ModelID"].ToString() == "" ? 0 : Convert.ToInt32(_row["ModelID"].ToString()),
                            ModelName = _row["Model"].ToString(),
                            Read = _row["Read"].ToString() == "" ? false: Convert.ToBoolean(_row["Read"].ToString()),
                            Write = _row["Write"].ToString() == "" ? false : Convert.ToBoolean(_row["Write"].ToString()),
                            Create = _row["Create"].ToString() == "" ? false : Convert.ToBoolean(_row["Create"].ToString()),
                            Delete = _row["Delete"].ToString() == "" ? false : Convert.ToBoolean(_row["Delete"].ToString())
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _buildList = null;
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }

            return _buildList;
        }

        /// <summary>
        /// Lấy danh sách menu được phân quyền vào nhóm
        /// </summary>
        /// <param name="SysGroupID"></param>
        /// <returns></returns>
        public List<SysGroupMenu_RelModels> GetListGroupMenu(int SysGroupID)
        {
            List<SysGroupMenu_RelModels> _buildList = new List<SysGroupMenu_RelModels>();
            DataSet ds = new DataSet();
            try
            {
                param = new SqlParameter[1];
                param[0] = new SqlParameter("@GroupID", SqlDbType.Int);
                param[0].Direction = ParameterDirection.Input;
                param[0].Value = SysGroupID;
                ds = helper.ExecuteQuery(CConfig.SP_GET_GROUPSMENU_REL, param, CommandType.StoredProcedure);
                {
                    foreach (DataRow _row in ds.Tables[0].Rows)
                    {
                        _buildList.Add(new SysGroupMenu_RelModels
                        {
                            ID = Convert.ToInt32(_row["ID"].ToString()),
                            SysGroupID = Convert.ToInt32(_row["SysGroupID"].ToString()),
                            SysMenuID = Convert.ToInt32(_row["SysMenuID"].ToString()),
                            Name = _row["MenuName"].ToString(),
                            SysGroupName = _row["GroupName"].ToString(),
                            Level = Convert.ToInt32(_row["Level"].ToString())
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _buildList = null;
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }

            return _buildList;
        }

        /// <summary>
        /// Get list menu cho vào dropdownlist
        /// </summary>
        /// <returns></returns>
        public List<MenusModel> GetListMenu()
        {
            List<MenusModel> _buildList = new List<MenusModel>();
            DataSet ds = new DataSet();
            try
            {
                ds = helper.ExecuteQuery(CConfig.SP_GET_ALL_MENUS_LIST, CommandType.StoredProcedure);
                {
                    foreach (DataRow _row in ds.Tables[0].Rows)
                    {
                        _buildList.Add(new MenusModel
                        {
                            ID = Convert.ToInt32(_row["ID"].ToString()),
                            Name = _row["Name"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _buildList = null;
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }

            return _buildList;
        }

        public DataSet CreateGroupMenuRel(SysGroupMenu_RelModels GroupMenu)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlParameter[] para = new SqlParameter[3];
                para[0] = new SqlParameter("@GroupID", SqlDbType.Int);
                para[0].Direction = ParameterDirection.Input;
                para[0].Value = GroupMenu.SysGroupID;
                para[1] = new SqlParameter("@MenuID", SqlDbType.Int);
                para[1].Direction = ParameterDirection.Input;
                para[1].Value = GroupMenu.SysMenuID;
                para[2] = new SqlParameter("@UserID", SqlDbType.Int);
                para[2].Direction = ParameterDirection.Input;
                para[2].Value = GroupMenu.UserID;

                ds = helper.ExecuteQuery(CConfig.SP_CREATE_GROUPSMENU_REL, para, CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                ds = null;
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }

            return ds;
        }

        public DataSet DeleteGroupMenuRel(int ID, int UserID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlParameter[] para = new SqlParameter[2];
                para[0] = new SqlParameter("@ID", SqlDbType.Int);
                para[0].Direction = ParameterDirection.Input;
                para[0].Value = ID;
                para[1] = new SqlParameter("@UserID", SqlDbType.Int);
                para[1].Direction = ParameterDirection.Input;
                para[1].Value = UserID;

                ds = helper.ExecuteQuery(CConfig.SP_DELETE_GROUPSMENU_REL, para, CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                ds = null;
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }

            return ds;
        }

        #endregion

        #region "TrungND"
        /*TrungND 21/10/16 lấy toàn bộ thông tin của khách sạn*/
        public DataSet GetAllHotels()
        {
            DataSet ds = new DataSet();
            try
            {
                if (helper._OpenConnection())
	            {
                    ds = helper.ExecuteQuery(CConfig.SP_HOTELS_GETALL,null, CommandType.StoredProcedure);
                    helper._CloseConnection();
	            }
            }
            catch (Exception)
            {
                ds = null;
            }

            return ds;
        }
        /*TrungND 21/10/16 lấy toàn bộ thông tin của khách sạn*/
        public DataSet GetAllSysUser()
        {
            DataSet ds = new DataSet();
            try
            {
                if (helper._OpenConnection())
                {
                    ds = helper.ExecuteQuery(CConfig.SP_SYSUSER_GETALLUSER, null, CommandType.StoredProcedure);
                    helper._CloseConnection();
                }
            }
            catch (Exception)
            {
                ds = null;
            }

            return ds;
        }

        /*TrungND 21/10/16 Tạo tài khoản SysUser*/
        public DataSet CreateSysUser(SysUserModel obj )
        {
            DataSet ds = new DataSet();
            try
            {
                SqlParameter[] para = new SqlParameter[15];
                para[0] = new SqlParameter("@UserName", SqlDbType.NVarChar, 50);
                para[0].Direction = ParameterDirection.Input;
                para[0].Value = obj.UserName;
                para[1] = new SqlParameter("@Password", SqlDbType.NVarChar, 50);
                para[1].Direction = ParameterDirection.Input;
                para[1].Value = obj.Password;
                para[2] = new SqlParameter("@IdentityNumber", SqlDbType.NVarChar,20);
                para[2].Direction = ParameterDirection.Input;
                para[2].Value = obj.IdentityNumber;
                para[3] = new SqlParameter("@FullName", SqlDbType.NVarChar, 50);
                para[3].Direction = ParameterDirection.Input;
                para[3].Value = obj.FullName;
                para[4] = new SqlParameter("@Address", SqlDbType.NVarChar, 200);
                para[4].Direction = ParameterDirection.Input;
                para[4].Value = obj.Address;
                para[5] = new SqlParameter("@Email", SqlDbType.NVarChar, 50);
                para[5].Direction = ParameterDirection.Input;
                para[5].Value = obj.Email;

                para[6] = new SqlParameter("@Mobile", SqlDbType.VarChar, 12);
                para[6].Direction = ParameterDirection.Input;
                para[6].Value = obj.Mobile;
                para[7] = new SqlParameter("@FirstLogin", SqlDbType.Int);
                para[7].Direction = ParameterDirection.Input;
                para[7].Value = 1;
                
                para[8] = new SqlParameter("@Department", SqlDbType.Int);
                para[8].Direction = ParameterDirection.Input;
                para[8].Value = obj.Department;

                para[9] = new SqlParameter("@ParentID", SqlDbType.Int);
                para[9].Direction = ParameterDirection.Input;
                para[9].Value = obj.ParentID;

                para[10] = new SqlParameter("@SysHotelID", SqlDbType.Int);
                para[10].Direction = ParameterDirection.Input;
                para[10].Value = obj.SysHotelID;

                para[11] = new SqlParameter("@Status", SqlDbType.Int);
                para[11].Direction = ParameterDirection.Input;
                para[11].Value = obj.Status;

                para[12] = new SqlParameter("@IsActive", SqlDbType.Int);
                para[12].Direction = ParameterDirection.Input;
                para[12].Value = obj.IsActive;

                para[13] = new SqlParameter("@Createby", SqlDbType.Int);
                para[13].Direction = ParameterDirection.Input;
                para[13].Value = obj.Createby;

                para[14] = new SqlParameter("@CreateDate", SqlDbType.DateTime);
                para[14].Direction = ParameterDirection.Input;
                para[14].Value = obj.CreateDate;

                //para[13] = new SqlParameter("@Modifyby", SqlDbType.DateTime);
                //para[13].Direction = ParameterDirection.Input;
                //para[13].Value = obj.@Modifyby;

                //para[14] = new SqlParameter("@ModifyDate", SqlDbType.DateTime);
                //para[14].Direction = ParameterDirection.Input;
                //para[14].Value = obj.ModifyDate;

                if (helper._OpenConnection())
                {
                    ds = helper.ExecuteQuery(CConfig.SP_SYSUSER_INSERT, para, CommandType.StoredProcedure);
                    helper._CloseConnection();
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds;
        }

        /*TrungND 28/10/16 Chỉnh sửa tài khoản SysUser*/
        public bool UpdateSysUser(SysUserModel obj)
        {
            bool kq = false;
            try
            {
                SqlParameter[] para = new SqlParameter[14];
                para[0] = new SqlParameter("@ID", SqlDbType.Int);
                para[0].Direction = ParameterDirection.Input;
                para[0].Value = obj.ID;
                para[1] = new SqlParameter("@Password", SqlDbType.NVarChar, 50);
                para[1].Direction = ParameterDirection.Input;
                para[1].Value = obj.Password;
                para[2] = new SqlParameter("@IdentityNumber", SqlDbType.NVarChar, 20);
                para[2].Direction = ParameterDirection.Input;
                para[2].Value = obj.IdentityNumber;
                para[3] = new SqlParameter("@FullName", SqlDbType.NVarChar, 50);
                para[3].Direction = ParameterDirection.Input;
                para[3].Value = obj.FullName;
                para[4] = new SqlParameter("@Address", SqlDbType.NVarChar, 200);
                para[4].Direction = ParameterDirection.Input;
                para[4].Value = obj.Address;
                para[5] = new SqlParameter("@Email", SqlDbType.NVarChar, 50);
                para[5].Direction = ParameterDirection.Input;
                para[5].Value = obj.Email;

                para[6] = new SqlParameter("@Mobile", SqlDbType.VarChar, 12);
                para[6].Direction = ParameterDirection.Input;
                para[6].Value = obj.Mobile;

                para[7] = new SqlParameter("@Department", SqlDbType.Int);
                para[7].Direction = ParameterDirection.Input;
                para[7].Value = obj.Department;

                para[8] = new SqlParameter("@ParentID", SqlDbType.Int);
                para[8].Direction = ParameterDirection.Input;
                para[8].Value = obj.ParentID;

                para[9] = new SqlParameter("@SysHotelID", SqlDbType.Int);
                para[9].Direction = ParameterDirection.Input;
                para[9].Value = obj.SysHotelID;

                para[10] = new SqlParameter("@Status", SqlDbType.Int);
                para[10].Direction = ParameterDirection.Input;
                para[10].Value = obj.Status;

                para[11] = new SqlParameter("@IsActive", SqlDbType.Int);
                para[11].Direction = ParameterDirection.Input;
                para[11].Value = obj.IsActive;

                para[12] = new SqlParameter("@Modifyby", SqlDbType.Int);
                para[12].Direction = ParameterDirection.Input;
                para[12].Value = obj.Modifyby;

                para[13] = new SqlParameter("@ModifyDate", SqlDbType.DateTime);
                para[13].Direction = ParameterDirection.Input;
                para[13].Value = obj.ModifyDate;

                //para[13] = new SqlParameter("@Modifyby", SqlDbType.DateTime);
                //para[13].Direction = ParameterDirection.Input;
                //para[13].Value = obj.@Modifyby;

                //para[14] = new SqlParameter("@ModifyDate", SqlDbType.DateTime);
                //para[14].Direction = ParameterDirection.Input;
                //para[14].Value = obj.ModifyDate;
                if (helper._OpenConnection())
                {
                    kq = helper.ExecuteNonQuery(CConfig.SP_SYSUSER_UPDATESYSUSER, para, CommandType.StoredProcedure);
                    helper._CloseConnection();
                }
               
            }
            catch (Exception ex)
            {
                return kq;
                throw ex;
            }

            return kq;
        }

        public bool DeleteSysUser(string id)
        {
            bool kq = false;
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@ID", SqlDbType.Int);
                para[0].Direction = ParameterDirection.Input;
                para[0].Value = id;

                if (helper._OpenConnection())
                {
                    kq = helper.ExecuteNonQuery(CConfig.SP_SYSUSER_DELETESYSUSER, para, CommandType.StoredProcedure);
                    helper._CloseConnection();
                }
                
            }
            catch (Exception)
            {
            }
            return kq;
        }

        /*TrungND 21/10/16 lấy 1 số thông tin của các user*/
        public DataSet GetInforSysUser()
        {
            DataSet ds = new DataSet();
            try
            {
                if (helper._OpenConnection())
                {
                    ds = helper.ExecuteQuery(CConfig.SP_SYSUSER_GETINFORUSER, null, CommandType.StoredProcedure);
                    helper._CloseConnection();
                }
            }
            catch (Exception)
            {
                ds = null;
            }

            return ds;
        }

        /*TrungND 21/10/16 lấy 1 số thông tin của 1 user*/
        public DataSet GetInforSysUserByID(string id)
        {
            DataSet ds = new DataSet();
            try
            {
                if (helper._OpenConnection())
                {
                    SqlParameter[] para = new SqlParameter[1];
                    para[0] = new SqlParameter("@ID", SqlDbType.Int);
                    para[0].Direction = ParameterDirection.Input;
                    para[0].Value = id;

                    if (helper._OpenConnection())
                    {
                        ds = helper.ExecuteQuery(CConfig.SP_SYSUSER_DETAILSYSUSER, para, CommandType.StoredProcedure);
                        helper._CloseConnection();
                    }
                    
                }
            }
            catch (Exception ex)
            {
                ds = null;
            }

            return ds;
        }

        /*TrungND 21/10/16 lấy 1 số thông tin của 1 user*/
        public DataSet GetAllGroupsByUser(string username)
        {
            DataSet ds = new DataSet();
            try
            {
                if (helper._OpenConnection())
                {
                    SqlParameter[] para = new SqlParameter[1];
                    para[0] = new SqlParameter("@UserName", SqlDbType.VarChar,20);
                    para[0].Direction = ParameterDirection.Input;
                    para[0].Value = username;

                    if (helper._OpenConnection())
                    {
                        ds = helper.ExecuteQuery(CConfig.SP_ACCOUNT_GETGROUPSBYUSER, para, CommandType.StoredProcedure);
                        helper._CloseConnection();
                    }
                    
                }
            }
            catch (Exception ex)
            {
                ds = null;
            }

            return ds;
        }

        #region "TrungND - 21/11/2016 Desc: Room Hotel"
        /*TrungND GetAll Room 21/11/2016*/
        public List<RoomModel> GetAllRoom()
        {
            List<RoomModel> list = new List<RoomModel>();
            try
            {
                if (helper._OpenConnection())
                {
                    DataSet ds = new DataSet();

                    if (helper._OpenConnection())
                    {
                        ds = helperClient.ExecuteQuery(CConfig.SP_SYSROOM_GETALLROOM, CommandType.StoredProcedure);
                        helperClient._CloseConnection();
                    }
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RoomModel obj = new RoomModel();
                            obj.ID = Int32.Parse(ds.Tables[0].Rows[i]["ID"].ToString());
                            obj.Name = ds.Tables[0].Rows[i]["Name"].ToString();
                            obj.Postion = ds.Tables[0].Rows[i]["Postion"].ToString();
                            obj.Floor = ds.Tables[0].Rows[i]["Floor"].ToString();
                            obj.RoomType_ID = Int32.Parse(ds.Tables[0].Rows[i]["RoomType_ID"].ToString());
                            obj.RoomLevel_ID = Int32.Parse(ds.Tables[0].Rows[i]["RoomLevel_ID"].ToString());
                            obj.Building_ID = Int32.Parse(ds.Tables[0].Rows[i]["Building_ID"].ToString());
                            obj.Temp = ds.Tables[0].Rows[i]["Temp"].ToString();
                            obj.Active = Int32.Parse(ds.Tables[0].Rows[i]["Active"].ToString());

                            list.Add(obj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return list;
            }

            return list;
        }

        /*TrungND Create Room 28/11/2016*/
        public string CreateRooms(RoomModel obj, ref string rs)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlParameter[] para = new SqlParameter[8];
                para[0] = new SqlParameter("@Name", SqlDbType.NVarChar, 100);
                para[0].Direction = ParameterDirection.Input;
                para[0].Value = obj.Name;
                para[1] = new SqlParameter("@Floor", SqlDbType.NVarChar, 50);
                para[1].Direction = ParameterDirection.Input;
                para[1].Value = obj.Floor;
                para[2] = new SqlParameter("@Postion", SqlDbType.NVarChar, 50);
                para[2].Direction = ParameterDirection.Input;
                para[2].Value = obj.Postion;
                para[3] = new SqlParameter("@RoomType_ID", SqlDbType.Int);
                para[3].Direction = ParameterDirection.Input;
                para[3].Value = obj.RoomType_ID;
                para[4] = new SqlParameter("@RoomLevel_ID", SqlDbType.Int);
                para[4].Direction = ParameterDirection.Input;
                para[4].Value = obj.RoomLevel_ID;
                para[5] = new SqlParameter("@Building_ID", SqlDbType.Int);
                para[5].Direction = ParameterDirection.Input;
                para[5].Value = obj.Building_ID;
                para[6] = new SqlParameter("@Temp", SqlDbType.VarChar, 100);
                para[6].Direction = ParameterDirection.Input;
                para[6].Value = obj.Temp;
                para[7] = new SqlParameter("@Active", SqlDbType.Int);
                para[7].Direction = ParameterDirection.Input;
                para[7].Value = obj.Active;

                if (helper._OpenConnection())
                {
                    dt = helperClient.ExecuteQuery(CConfig.SP_SYSROOM_CREATEROOM, para, CommandType.StoredProcedure).Tables[0];
                    helperClient._CloseConnection();
                }
                
                rs = dt.Rows[0]["iCode"].ToString();
                if (dt.Rows[0]["iCode"].ToString().Equals("1"))
                {
                    return dt.Rows[0]["sMess"].ToString();
                }
                return dt.Rows[0]["sMess"].ToString();
            }
            catch (Exception ex)
            {
                return "Thêm mới phòng thất bại: " + ex;
            }
        }
        #endregion

         /*TrungND update Room name 29/11/2016*/
        public bool UpdateRoomsName(RoomModel obj,ref string mess ,ref int rs)
        {
            bool kq = false;
            try
            {
                SqlParameter[] para = new SqlParameter[4];
                para[0] = new SqlParameter("@ID", SqlDbType.Int);
                para[0].Direction = ParameterDirection.Input;
                para[0].Value = obj.ID;
                para[1] = new SqlParameter("@Name", SqlDbType.NVarChar, 100);
                para[1].Direction = ParameterDirection.Input;
                para[1].Value = obj.Name;
                para[2] = new SqlParameter("@RetCode", SqlDbType.Int);
                para[2].Direction = ParameterDirection.Output;
                para[2].Value = DBNull.Value;
                para[3] = new SqlParameter("@RetMesg", SqlDbType.NVarChar, 200);
                para[3].Direction = ParameterDirection.Output;
                para[3].Value = DBNull.Value;

                if (helper._OpenConnection())
                {
                    kq = helperClient.ExecuteNonQuery(CConfig.SP_ROOM_UPDATE_ROOMNAME, para, CommandType.StoredProcedure);
                    helperClient._CloseConnection();
                }
                
                mess = para[3].Value.ToString();
                rs = Int32.Parse(para[2].Value.ToString());
                
                return kq;
            }
            catch (Exception ex)
            {
                return kq;
            }
        }
             
        /*TrungND delete Room name 29/11/2016*/
        public bool DeleteRoomsID(int id,ref string mess)
        {
            bool kq = false;
            try
            {
                SqlParameter[] para = new SqlParameter[3];
                para[0] = new SqlParameter("@ID", SqlDbType.Int);
                para[0].Direction = ParameterDirection.Input;
                para[0].Value = id;
                para[1] = new SqlParameter("@RetCode", SqlDbType.Int);
                para[1].Direction = ParameterDirection.Output;
                para[1].Value = DBNull.Value;
                para[2] = new SqlParameter("@RetMesg", SqlDbType.NVarChar, 200);
                para[2].Direction = ParameterDirection.Output;
                para[2].Value = DBNull.Value;

                if (helper._OpenConnection())
                {
                    kq = helperClient.ExecuteNonQuery(CConfig.SP_ROOM_DELETE_ROOM, para, CommandType.StoredProcedure);
                    helperClient._CloseConnection();
                }
                
                mess = para[2].Value.ToString();
                int rs = Int32.Parse(para[2].Value.ToString());
                if (rs == -1)
                {
                    return false;
                }
                return kq;
            }
            catch (Exception ex)
            {
                return kq;
            }
        }


        ///*TrungND Create RoomType 12/12/2016*/
        public void CreateRoomType(RoomTypeModel obj, ref string roomtype, ref int retcode, ref string retmesg)
        {
            bool kq = false;

            try
            {
                SqlParameter[] para = new SqlParameter[9];
                para[0] = new SqlParameter("@Name", SqlDbType.NVarChar, 100);
                para[0].Direction = ParameterDirection.Input;
                para[0].Value = obj.Name;
                para[1] = new SqlParameter("@Code", SqlDbType.NVarChar, 100);
                para[1].Direction = ParameterDirection.Input;
                para[1].Value = obj.Code;
                para[2] = new SqlParameter("@DouldBed", SqlDbType.Int);
                para[2].Direction = ParameterDirection.Input;
                para[2].Value = obj.DouldBed;
                para[3] = new SqlParameter("@SingBed", SqlDbType.Int);
                para[3].Direction = ParameterDirection.Input;
                para[3].Value = obj.SingBed;
                para[4] = new SqlParameter("@UserLimit", SqlDbType.Int);
                para[4].Direction = ParameterDirection.Input;
                para[4].Value = obj.UserLimit;
                para[5] = new SqlParameter("@Note", SqlDbType.NVarChar,2000);
                para[5].Direction = ParameterDirection.Input;
                para[5].Value = obj.Note;
                para[6] = new SqlParameter("@RetCode", SqlDbType.Int);
                para[6].Direction = ParameterDirection.Output;
                para[6].Value = null;
                para[7] = new SqlParameter("@RetMess", SqlDbType.NVarChar, 200);
                para[7].Direction = ParameterDirection.Output;
                para[7].Value = null;
                para[8] = new SqlParameter("@RetRoomTypeCode", SqlDbType.VarChar, 100);
                para[8].Direction = ParameterDirection.Output;
                para[8].Value = null;

                if (helperClient._OpenConnection())
                {
                    kq = helperClient.ExecuteNonQuery(CConfig.SP_ROOMTYPE_CREATE, para, CommandType.StoredProcedure);
                    helperClient._CloseConnection();
                }

                if (kq)
                {
                    roomtype = para[8].Value.ToString();
                    retcode = Int32.Parse(para[6].Value.ToString());
                    retmesg = para[7].Value.ToString();
                    return;
                }
            }
            catch (Exception ex)
            {
                retcode = -1;
                retmesg = ex.Message;
                return;
            }
        }

        ///*TrungND Create và up date Phụ trội được dùng chung 1 hàm*/
        //public void CreateAndUpdatePhutroi(PhuTroiModel obj, ref int retcode, ref string retmesg)
        //{
        //    bool kq = false;

        //    try
        //    {
        //        SqlParameter[] para = new SqlParameter[9];
        //        para[0] = new SqlParameter("@Name", SqlDbType.NVarChar, 100);
        //        para[0].Direction = ParameterDirection.Input;
        //        para[0].Value = obj.Name;
        //        para[1] = new SqlParameter("@ReservationType", SqlDbType.VarChar, 50);
        //        para[1].Direction = ParameterDirection.Input;
        //        para[1].Value = obj.ReservationType;
        //        para[2] = new SqlParameter("@CodePhuTroi", SqlDbType.VarChar,100);
        //        para[2].Direction = ParameterDirection.Input;
        //        para[2].Value = obj.CodePhuTroi;
        //        para[3] = new SqlParameter("@TotalHours", SqlDbType.Int);
        //        para[3].Direction = ParameterDirection.Input;
        //        para[3].Value = obj.TotalHours;
        //        para[4] = new SqlParameter("@NumberPeople", SqlDbType.Int);
        //        para[4].Direction = ParameterDirection.Input;
        //        para[4].Value = obj.NumberPeople;
        //        para[5] = new SqlParameter("@Price", SqlDbType.Float);
        //        para[5].Direction = ParameterDirection.Input;
        //        para[5].Value = obj.Price;
        //        para[5] = new SqlParameter("@RoomTypeCode", SqlDbType.NVarChar, 100);
        //        para[5].Direction = ParameterDirection.Input;
        //        para[5].Value = obj.RoomTypeCode;
        //        para[6] = new SqlParameter("@RetMess", SqlDbType.NVarChar,200);
        //        para[6].Direction = ParameterDirection.Output;
        //        para[6].Value = null;
        //        para[7] = new SqlParameter("@RetCode", SqlDbType.VarChar, 100);
        //        para[7].Direction = ParameterDirection.Output;
        //        para[7].Value = null;
        //        para[8] = new SqlParameter("@RetPhuTroiCode", SqlDbType.VarChar, 100);
        //        para[8].Direction = ParameterDirection.Output;
        //        para[8].Value = null;

        //        if (helper._OpenConnection())
        //        {
        //            kq = helperClient.ExecuteNonQuery(CConfig.SP_PHUTROI_CREATE, para, CommandType.StoredProcedure);
        //            helperClient._CloseConnection();
        //        }

        //        if (kq)
        //        {
        //            string codephutroi = para[8].Value.ToString();
        //            retcode = Int32.Parse(para[7].Value.ToString());
        //            retmesg = para[6].Value.ToString();
        //            return;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        retcode = -1;
        //        retmesg = ex.Message;
        //        return;
        //    }
        //}

        ///*TrungND Create và update giá phòng theo hạng phòng được dùng chung 1 hàm*/
        //public void CreateAndUpdateRoomPrice(RoomPriceModel obj, ref int retcode, ref string retmesg)
        //{
        //    bool kq = false;

        //    try
        //    {
        //        SqlParameter[] para = new SqlParameter[9];
        //        para[0] = new SqlParameter("@Code", SqlDbType.NVarChar, 100);
        //        para[0].Direction = ParameterDirection.Input;
        //        para[0].Value = obj.Code;
        //        para[1] = new SqlParameter("@SysHotelCode", SqlDbType.VarChar, 50);
        //        para[1].Direction = ParameterDirection.Input;
        //        para[1].Value = obj.SysHotelCode;
        //        para[2] = new SqlParameter("@ReservationType", SqlDbType.VarChar, 50);
        //        para[2].Direction = ParameterDirection.Input;
        //        para[2].Value = obj.ReservationType;
        //        para[3] = new SqlParameter("@DatetimeType", SqlDbType.VarChar, 50);
        //        para[3].Direction = ParameterDirection.Input;
        //        para[3].Value = obj.DatetimeType;
        //        para[4] = new SqlParameter("@TotalHours", SqlDbType.Int);
        //        para[4].Direction = ParameterDirection.Input;
        //        para[4].Value = obj.TotalHours;
        //        para[5] = new SqlParameter("@NumberPeople", SqlDbType.Int);
        //        para[5].Direction = ParameterDirection.Input;
        //        para[5].Value = obj.NumberPeople;
        //        para[6] = new SqlParameter("@Price", SqlDbType.Float);
        //        para[6].Direction = ParameterDirection.Input;
        //        para[6].Value = obj.Price;
        //        para[7] = new SqlParameter("@RoomTypeCode", SqlDbType.NVarChar, 100);
        //        para[7].Direction = ParameterDirection.Input;
        //        para[7].Value = obj.RoomTypeCode;
        //        para[8] = new SqlParameter("@RetMess", SqlDbType.NVarChar, 200);
        //        para[8].Direction = ParameterDirection.Output;
        //        para[8].Value = null;
        //        para[9] = new SqlParameter("@RetCode", SqlDbType.VarChar, 100);
        //        para[9].Direction = ParameterDirection.Output;
        //        para[9].Value = null;
        //        para[8] = new SqlParameter("@RetRoomPriceCode", SqlDbType.VarChar, 100);
        //        para[8].Direction = ParameterDirection.Output;
        //        para[8].Value = null;

        //        if (helper._OpenConnection())
        //        {
        //            kq = helperClient.ExecuteNonQuery(CConfig.SP_PHUTROI_CREATE, para, CommandType.StoredProcedure);
        //            helperClient._CloseConnection();
        //        }

        //        if (kq)
        //        {
        //            string codephutroi = para[8].Value.ToString();
        //            retcode = Int32.Parse(para[7].Value.ToString());
        //            retmesg = para[6].Value.ToString();
        //            return;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        retcode = -1;
        //        retmesg = ex.Message;
        //        return;
        //    }
        //}

        /*TrungND set hạng phòng và giá cho phòng*/
        public void SetRoomTypeForRoom(RoomModel obj, ref int retcode, ref string retmesg)
        {
            bool kq = false;

            try
            {
                SqlParameter[] para = new SqlParameter[4];
                para[0] = new SqlParameter("@ID", SqlDbType.NVarChar, 100);
                para[0].Direction = ParameterDirection.Input;
                para[0].Value = obj.ID;
                para[1] = new SqlParameter("@RoomTypeCode", SqlDbType.VarChar, 50);
                para[1].Direction = ParameterDirection.Input;
                para[1].Value = obj.RoomTypeCode;
                para[2] = new SqlParameter("@RetCode", SqlDbType.Int);
                para[2].Direction = ParameterDirection.Output;
                para[2].Value = null;
                para[3] = new SqlParameter("@RetMesg", SqlDbType.NVarChar, 200);
                para[3].Direction = ParameterDirection.Output;
                para[3].Value = null;

                if (helper._OpenConnection())
                {
                    kq = helperClient.ExecuteNonQuery(CConfig.SP_ROOM_UPDATE_ROOMTYPE, para, CommandType.StoredProcedure);
                    helperClient._CloseConnection();
                }

                if (kq)
                {
                    retcode = Int32.Parse(para[2].Value.ToString());
                    retmesg = para[3].Value.ToString();
                    return;
                }
            }
            catch (Exception ex)
            {
                retcode = -1;
                retmesg = ex.Message;
                return;
            }
        }


        #endregion
        #region "LAMLT  -12/07/2016"
        public DataSet GetAllSupplier()
        {
            DataSet ds = new DataSet();
            try
            {
                if (helper._OpenConnection())
                {
                    ds = helper.ExecuteQuery(CConfig.SP_SUPPLIER_GETALLUSER, null, CommandType.StoredProcedure);
                    helper._CloseConnection();
                }
            }
            catch (Exception)
            {
                ds = null;
            }

            return ds;
        }
       
        public DataSet CreateSupplier(SupplierModel obj)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlParameter[] para = new SqlParameter[7];
                para[0] = new SqlParameter("@Name", SqlDbType.NVarChar, 50);
                para[0].Direction = ParameterDirection.Input;
                para[0].Value = obj.Name;
                para[1] = new SqlParameter("@ContactName", SqlDbType.NVarChar, 50);
                para[1].Direction = ParameterDirection.Input;
                para[1].Value = obj.ContactName;
                para[2] = new SqlParameter("@Address", SqlDbType.NVarChar, 20);
                para[2].Direction = ParameterDirection.Input;
                para[2].Value = obj.Address;
                para[3] = new SqlParameter("@Email", SqlDbType.NVarChar, 50);
                para[3].Direction = ParameterDirection.Input;
                para[3].Value = obj.Email;
                para[4] = new SqlParameter("@Mobile", SqlDbType.NVarChar, 200);
                para[4].Direction = ParameterDirection.Input;
                para[4].Value = obj.Mobile;
                para[5] = new SqlParameter("@Note", SqlDbType.NVarChar, 50);
                para[5].Direction = ParameterDirection.Input;
                para[5].Value = obj.Note;

                para[6] = new SqlParameter("@CreateDate", SqlDbType.DateTime);
                para[6].Direction = ParameterDirection.Input;
                para[6].Value = obj.CreateDate;

                ds = helper.ExecuteQuery(CConfig.SP_SUPPLIER_INSERT, para, CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds;
        }

        public bool UpdateSupplier(SupplierModel obj)
        {
            bool kq = false;
            try
            {
                SqlParameter[] para = new SqlParameter[8];
                para[0] = new SqlParameter("@ID", SqlDbType.Int);
                para[0].Direction = ParameterDirection.Input;
                para[0].Value = obj.ID;
              
                para[1] = new SqlParameter("@Name", SqlDbType.NVarChar, 50);
                para[1].Direction = ParameterDirection.Input;
                para[1].Value = obj.Name;

                para[2] = new SqlParameter("@ContactName", SqlDbType.NVarChar, 50);
                para[2].Direction = ParameterDirection.Input;
                para[2].Value = obj.ContactName;

                para[3] = new SqlParameter("@Address", SqlDbType.NVarChar, 20);
                para[3].Direction = ParameterDirection.Input;
                para[3].Value = obj.Address;

                para[4] = new SqlParameter("@Email", SqlDbType.NVarChar, 50);
                para[4].Direction = ParameterDirection.Input;
                para[4].Value = obj.Email;

                para[5] = new SqlParameter("@Mobile", SqlDbType.NVarChar, 200);
                para[5].Direction = ParameterDirection.Input;
                para[5].Value = obj.Mobile;

                para[6] = new SqlParameter("@Note", SqlDbType.NVarChar, 50);
                para[6].Direction = ParameterDirection.Input;
                para[6].Value = obj.Note;

                para[7] = new SqlParameter("@CreateDate", SqlDbType.DateTime);
                para[7].Direction = ParameterDirection.Input;
                para[7].Value = obj.CreateDate;

               
                kq = helper.ExecuteNonQuery(CConfig.SP_SUPPLIER_INSERT, para, CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                return kq;
                throw ex;
            }

            return kq;
        }

        public bool DeleteSupplier(string id)
        {
            bool kq = false;
            try
            {
                SqlParameter[] para = new SqlParameter[1];
                para[0] = new SqlParameter("@ID", SqlDbType.Int);
                para[0].Direction = ParameterDirection.Input;
                para[0].Value = id;

                kq = helper.ExecuteNonQuery(CConfig.SP_SUPPLIER_DELETESUPPLIER, para, CommandType.StoredProcedure);
                helper._CloseConnection();
            }
            catch (Exception)
            {
            }
            return kq;
        }

        public DataSet GetInforSupplier()
        {
            DataSet ds = new DataSet();
            try
            {
                if (helper._OpenConnection())
                {
                    ds = helper.ExecuteQuery(CConfig.SP_SUPPLIER_GETINFORUSER, null, CommandType.StoredProcedure);
                    helper._CloseConnection();
                }
            }
            catch (Exception)
            {
                ds = null;
            }

            return ds;
        }

        public DataSet GetInforSupplierById(string id)
        {
            DataSet ds = new DataSet();
            try
            {
                if (helper._OpenConnection())
                {
                    SqlParameter[] para = new SqlParameter[1];
                    para[0] = new SqlParameter("@ID", SqlDbType.Int);
                    para[0].Direction = ParameterDirection.Input;
                    para[0].Value = id;

                    ds = helper.ExecuteQuery(CConfig.SP_SUPPLIER_DETAILSUPPLIER, para, CommandType.StoredProcedure);
                    helper._CloseConnection();
                }
            }
            catch (Exception ex)
            {
                ds = null;
            }

            return ds;
        }
        #endregion
    }
}