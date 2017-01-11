using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Oze.AppCode.BLL;
using Oze.AppCode.DAL;
using Oze.Models;

namespace Oze.Controllers
{
    public class ReservationRoom1Controller : Controller
    {
        CDatabaseLam db = new CDatabaseLam();
        CDatabase db1 = new CDatabase();
        
        #region GET: /Booking/
        public ActionResult Index()
        {
            string rescode = "";

            //Lấy list phong
            TempData["room"] = new SelectList(db1.GetList_Room(1), "Value", "Name");
            //Lấy list country
            TempData["country"] = new SelectList(db1.GetList_Country(1), "Value", "Name", 238);

            if (true)
            {
                try
                {
                    if (Session[CConfig.SESSION_USERID] != null && Session[CConfig.SESSION_HOTELCODE].ToString() != null)
                    {
                        ReservationRoomModel reservation = new ReservationRoomModel();
                        reservation.HotelCode = Session[CConfig.SESSION_HOTELCODE].ToString();
                        reservation.Activity = "RESERVATIONCODE";

                        if (db.GenResCode(reservation, ref rescode))
                        {
                            if (rescode.Length < 1)
                            {
                                TempData["ResCode"] = reservation.HotelCode + "000001";
                            }
                            else
                                TempData["ResCode"] = rescode;
                        }
                        else TempData["ResCode"] = "";
                    }
                    TempData.Keep("ResCode");
                }
                catch (Exception ex)
                {
                    rescode = ex.Message;
                }
            }
            return View();
        }
        #endregion

        #region Lưu thông tin đặt Phòng
        public JsonResult ReservationSave(CustomerModel customer, ReservationRoomModel reservation)
        {
            object[] message = new object[4];
            int code = -1;
            string mess = "";
            DataSet ds = new DataSet();
            try
            {
                if (customer != null && reservation != null)
                {
                    if (Session[CConfig.SESSION_USERID] != null && Session[CConfig.SESSION_HOTELCODE].ToString() != null)
                    {
                        //THONG TIN KHACH HANG
                        customer.Activity = "INSERT";
                        customer.Createby = Int32.Parse(Session[CConfig.SESSION_USERID].ToString());
                        customer.HotelCode = Session[CConfig.SESSION_HOTELCODE].ToString();
                        //THONG TIN DAT PHONG
                        reservation.Activity = "INSERT";
                        reservation.CreateResBy = Int32.Parse(Session[CConfig.SESSION_USERID].ToString());
                        reservation.HotelCode = Session[CConfig.SESSION_HOTELCODE].ToString();
                        reservation.Tax = 10;
                        reservation.Deduction = 2000000;

                        ds = db.InsertReservationRoom(customer, reservation, ref code, ref mess);
                        message[0] = mess;
                        message[1] = code;
                        
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            DataTable dt = ds.Tables[0];
                            //Chuyển data dạng bảng -> object nhận lại ở ajax
                            List<object> resultCus = new List<object>(); 
                            List<object> resultRes = new List<object>();


                            foreach (DataRow rw in dt.AsEnumerable())
                            {

                                #region Bảng chứ thông tin KH vừa thêm
                                resultCus.Add(new
                                {
                                    ID = rw["CusID"].ToString().Trim(),
                                    FullName = rw["Name"].ToString().Trim(),
                                    Sex = rw["Sex"].ToString().Trim(),
                                    //DOB = rw["DOB"].ToString().Trim(),
                                    DOB = DateTime.Parse(rw["DOB"].ToString().Trim()).ToString("dd/MM/yyyy"),
                                    IdentityNumber = rw["IdentifyNumber"].ToString().Trim(),
                                    CitizenshipCode = rw["CitizenshipCode"].ToString().Trim(),
                                    Address = rw["Address"].ToString().Trim(),
                                    Email = rw["Email"].ToString().Trim(),
                                    Mobile = rw["Mobile"].ToString().Trim(),
                                    Company = rw["Company"].ToString().Trim(),
                                    RoomID = rw["RoomID"].ToString().Trim(),
                                    GroupID = rw["Doan_ID"].ToString().Trim(),
                                    GroupJoinID = rw["Gop_Doan_ID"].ToString().Trim(),
                                    Leader = rw["Leader"].ToString().Trim(),
                                    Payer = rw["Payer"].ToString().Trim(),
                                    ReserCode = rw["BookingCode"].ToString().Trim()
                                });

                                #endregion

                                #region Bảng chứ thông tin Đặt phòng vừa thêm
                                resultRes.Add(new
                                {
                                    ID = rw["ResID"].ToString().Trim(),
                                    CustomerName = rw["Name"].ToString().Trim(),
                                    ReservationCode = rw["BookingCode"].ToString().Trim(),
                                    ReservationType = rw["Reservation_Type_ID"].ToString().Trim(),
                                    Payment_Type_ID = rw["Payment_Type_ID"].ToString().Trim(),
                                    //ArrivalDate = rw["Arrive_Date"].ToString().Trim(),
                                    //LeaveDate = rw["Leave_Date"].ToString().Trim(),
                                    ArrivalDate = DateTime.Parse(rw["Arrive_Date"].ToString().Trim()).ToString("dd/MM/yyyy HH:mm"),
                                    LeaveDate = DateTime.Parse(rw["Leave_Date"].ToString().Trim()).ToString("dd/MM/yyyy HH:mm"),
                                    Adult = rw["Number_People"].ToString().Trim(),
                                    Children = rw["Number_Children"].ToString().Trim(),
                                    Holiday = rw["Holiday"].ToString().Trim(),
                                    KhungGio = rw["KhungGio"].ToString().Trim(),
                                    Price = rw["Price"].ToString().Trim(),
                                    Deposit = rw["Deposit"].ToString().Trim(),
                                    Discount = rw["Discount"].ToString().Trim(),
                                    Note = rw["Note"].ToString().Trim()
                                    
                                });
                                
                                #endregion
                            }

                            message[2] = resultCus;
                            message[3] = resultRes;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message[0] = ex.Message;
                message[1] = -1;
            }

            return Json(new { mess = message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Lưu thông tin đặt Phòng
        public JsonResult ReservationEdit(CustomerModel customer, ReservationRoomModel reservation)
        {
            object[] message = new object[4];
            int code = -1;
            string mess = "";
            DataSet ds = new DataSet();
            try
            {
                if (customer != null && reservation != null)
                {
                    if (Session[CConfig.SESSION_USERID] != null && Session[CConfig.SESSION_HOTELCODE].ToString() != null)
                    {
                        //THONG TIN KHACH HANG
                        customer.Activity = "UPDATE";
                        customer.Modifyby = Int32.Parse(Session[CConfig.SESSION_USERID].ToString());
                        customer.HotelCode = Session[CConfig.SESSION_HOTELCODE].ToString();
                        //THONG TIN DAT PHONG
                        reservation.Activity = "UPDATE";
                        reservation.ModifyResby = Int32.Parse(Session[CConfig.SESSION_USERID].ToString());
                        reservation.HotelCode = Session[CConfig.SESSION_HOTELCODE].ToString();
                        reservation.Tax = 10;
                        reservation.Deduction = 2000000;

                        db.InsertReservationRoom(customer, reservation, ref code, ref mess);
                        message[0] = mess;
                        message[1] = code;
                    }
                }
            }
            catch (Exception ex)
            {
                message[0] = ex.Message;
                message[1] = -1;
            }

            return Json(new { mess = message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Add bạn cùng phòng
        public JsonResult ReservationAddFriend(CustomerModel customer)
        {
            object[] message = new object[3];
            int code = -1;
            string mess = "";
            DataSet ds = new DataSet();
            try
            {
                if (customer != null)
                {
                    if (Session[CConfig.SESSION_USERID] != null && Session[CConfig.SESSION_HOTELCODE].ToString() != null)
                    {
                        //THONG TIN KHACH HANG
                        customer.Activity = "INSERT";
                        customer.Createby = Int32.Parse(Session[CConfig.SESSION_USERID].ToString());
                        customer.HotelCode = Session[CConfig.SESSION_HOTELCODE].ToString();

                        ds = db.InsertFriend(customer, ref code, ref mess);
                        message[0] = mess;
                        message[1] = code;
                        //Bảng chứ thông tin Friend vừa thêm
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            DataTable dt = ds.Tables[0];
                            //Chuyển data dạng bảng -> object nhận lại ở ajax
                            List<object> result = new List<object>();
                            foreach (DataRow rw in dt.AsEnumerable())
                            {
                                result.Add(new
                                {
                                    ID = rw["ID"].ToString().Trim(),
                                    FullName = rw["Name"].ToString().Trim(),
                                    Sex = rw["Sex"].ToString().Trim(),
                                    //DOB = rw["DOB"].ToString().Trim(),
                                    DOB = DateTime.Parse(rw["DOB"].ToString().Trim()).ToString("dd/MM/yyyy"),
                                    IdentityNumber = rw["IdentifyNumber"].ToString().Trim(),
                                    CitizenshipCode = rw["CitizenshipCode"].ToString().Trim(),
                                    Address = rw["Address"].ToString().Trim(),
                                    Email = rw["Email"].ToString().Trim(),
                                    Mobile = rw["Mobile"].ToString().Trim(),
                                    Company = rw["Company"].ToString().Trim(),
                                    RoomID = rw["RoomID"].ToString().Trim(),
                                    GroupID = rw["Doan_ID"].ToString().Trim(),
                                    GroupJoinID = rw["Gop_Doan_ID"].ToString().Trim(),
                                    Leader = rw["Leader"].ToString().Trim(),
                                    Payer = rw["Payer"].ToString().Trim(),
                                    ReserCode = rw["ReserCode"].ToString().Trim()
                                });
                            }
                            message[2] = result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message[0] = ex.Message;
                message[1] = -1;
            }

            return Json(new { mess = message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Get thông tin bạn cùng Phòng --> để show hoặc sửa (Click vào 1 tên KH trong lst bạn cùng Phòng)
        public JsonResult ReservationGetFriend(CustomerModel customer)
        {
            object[] message = new object[3];
            int code = -1;
            string mess = "";
            DataSet ds = new DataSet();
            try
            {
                if (customer != null)
                {
                    if (Session[CConfig.SESSION_USERID] != null && Session[CConfig.SESSION_HOTELCODE].ToString() != null)
                    {
                        //THONG TIN KHACH HANG
                        customer.Activity = "SEARCH";

                        ds = db.InsertFriend(customer, ref code, ref mess);
                        message[0] = mess;
                        message[1] = code;
                        //Bảng chứ thông tin Friend vừa thêm
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            DataTable dt = ds.Tables[0];
                            //Chuyển data dạng bảng -> object nhận lại ở ajax
                            List<object> result = new List<object>();
                            foreach (DataRow rw in dt.AsEnumerable())
                            {
                                result.Add(new
                                {
                                    ID = rw["ID"].ToString().Trim(),
                                    FullName = rw["Name"].ToString().Trim(),
                                    Sex = rw["Sex"].ToString().Trim(),
                                    //DOB = rw["DOB"].ToString().Trim(),
                                    DOB = DateTime.Parse(rw["DOB"].ToString().Trim()).ToString("dd/MM/yyyy"),
                                    IdentityNumber = rw["IdentifyNumber"].ToString().Trim(),
                                    CitizenshipCode = rw["CitizenshipCode"].ToString().Trim(),
                                    Address = rw["Address"].ToString().Trim(),
                                    Email = rw["Email"].ToString().Trim(),
                                    Mobile = rw["Mobile"].ToString().Trim(),
                                    Company = rw["Company"].ToString().Trim(),
                                    RoomID = rw["RoomID"].ToString().Trim(),
                                    GroupID = rw["Doan_ID"].ToString().Trim(),
                                    GroupJoinID = rw["Gop_Doan_ID"].ToString().Trim(),
                                    Leader = rw["Leader"].ToString().Trim(),
                                    Payer = rw["Payer"].ToString().Trim(),
                                    ReserCode = rw["ReserCode"].ToString().Trim()
                                });
                            }
                            message[2] = result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message[0] = ex.Message;
                message[1] = -1;
            }

            return Json(new { mess = message }, JsonRequestBehavior.AllowGet);
        }
        #endregion 
        
        #region Sửa thông tin bạn cùng Phòng

        public JsonResult ReservationEditFriend(CustomerModel customer)
        {
            object[] message = new object[3];
            int code = -1;
            string mess = "";
            DataSet ds = new DataSet();
            try
            {
                if (customer != null)
                {
                    if (Session[CConfig.SESSION_USERID] != null && Session[CConfig.SESSION_HOTELCODE].ToString() != null)
                    {
                        //THONG TIN KHACH HANG
                        customer.Activity = "UPDATE";
                        customer.Modifyby = Int32.Parse(Session[CConfig.SESSION_USERID].ToString());
                        customer.HotelCode = Session[CConfig.SESSION_HOTELCODE].ToString();

                        ds = db.InsertFriend(customer, ref code, ref mess);
                        message[0] = mess;
                        message[1] = code;
                        //Bảng chứ thông tin Friend vừa thêm
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            DataTable dt = ds.Tables[0];
                            //Chuyển data dạng bảng -> object nhận lại ở ajax
                            List<object> result = new List<object>();
                            foreach (DataRow rw in dt.AsEnumerable())
                            {
                                result.Add(new
                                {
                                    ID = rw["ID"].ToString().Trim(),
                                    FullName = rw["Name"].ToString().Trim(),
                                    Sex = rw["Sex"].ToString().Trim(),
                                    //DOB = rw["DOB"].ToString().Trim(),
                                    DOB = DateTime.Parse(rw["DOB"].ToString().Trim()).ToString("dd/MM/yyyy"),
                                    IdentityNumber = rw["IdentifyNumber"].ToString().Trim(),
                                    CitizenshipCode = rw["CitizenshipCode"].ToString().Trim(),
                                    Address = rw["Address"].ToString().Trim(),
                                    Email = rw["Email"].ToString().Trim(),
                                    Mobile = rw["Mobile"].ToString().Trim(),
                                    Company = rw["Company"].ToString().Trim(),
                                    RoomID = rw["RoomID"].ToString().Trim(),
                                    GroupID = rw["Doan_ID"].ToString().Trim(),
                                    GroupJoinID = rw["Gop_Doan_ID"].ToString().Trim(),
                                    Leader = rw["Leader"].ToString().Trim(),
                                    Payer = rw["Payer"].ToString().Trim(),
                                    ReserCode = rw["ReserCode"].ToString().Trim()
                                });
                            }
                            message[2] = result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message[0] = ex.Message;
                message[1] = -1;
            }

            return Json(new { mess = message }, JsonRequestBehavior.AllowGet);
        }

        #endregion //Sửa thông tin bạn cùng Phòng

        #region Hủy đặt Phòng (Hủy theo ReserCode)

        public JsonResult ReservationCancel(ReservationRoomModel resroomcancel)
        {
            object[] message = new object[2];
            int code = -1;
            string mess = "";
            bool result = false;
            try
            {
                if (resroomcancel != null)
                {
                    if (Session[CConfig.SESSION_USERID] != null && Session[CConfig.SESSION_HOTELCODE].ToString() != null)
                    {
                        //THONG TIN KHACH HANG
                        resroomcancel.ModifyResby = Int32.Parse(Session[CConfig.SESSION_USERID].ToString());

                        result = db.ReserCancel(resroomcancel, ref code, ref mess);

                        message[0] = mess;
                        message[1] = code;
                    }
                }
            }
            catch (Exception ex)
            {
                message[0] = ex.Message;
                message[1] = -1;
            }

            return Json(new { mess = message }, JsonRequestBehavior.AllowGet);
        }

        #endregion //Hủy đặt Phòng (Hủy theo ReserCode)
    }
}