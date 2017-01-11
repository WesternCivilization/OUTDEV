using System.Configuration;
using System.Web;

namespace Oze.AppCode.BLL
{
    public class CConfig
    {
        public static string NO_LOG_SET = "0";
        public static string SINGLE_THREAD = "0";

        public static string TEMPLATE_LOG_DATA = "(Time)|(Title)|(Detail)";
        public static string LOG_EXT = ".txt";
        /*Tạo các chuỗi kết nối*/
        public static string CONNECTIONSTRING_OZEGENERAL = ConfigurationManager.ConnectionStrings["ConnectionString_OzeGeneral"].ConnectionString;        
        public static string CONNECTIONSTRING_OZMEMBER = ConfigurationManager.ConnectionStrings["ConnectionString_OzeMember"].ConnectionString;
        //public static string CONNECTIONSTRING_OZEGENERAL_SERVER = ConfigurationManager.ConnectionStrings["ConnectionString_OzeGeneral_Server"].ConnectionString; /*NamLD 11/10/2016 10:50:56*/

        /*Tạo các Link Setting*/
        public static string BASE_APP_NAME = ConfigurationManager.AppSettings["BASE_APP_NAME"].ToString();
        public static string BASE_LOG_ERROR = ConfigurationManager.AppSettings["BASE_LOG_ERROR"].ToString();
        public static string BASE_LOG_SQL = ConfigurationManager.AppSettings["BASE_LOG_SQL"].ToString();
        public static string BASE_LOG_MULTI_THREAD = ConfigurationManager.AppSettings["BASE_LOG_MULTI_THREAD"].ToString();
        public static string BASE_LOG_PATH_ERROR = ConfigurationManager.AppSettings["BASE_LOG_PATH_ERROR"].ToString();
        public static string BASE_LOG_PATH_SQL = ConfigurationManager.AppSettings["BASE_LOG_PATH_SQL"].ToString();
        public static string BASE_LOG_PATH_TEXT = ConfigurationManager.AppSettings["BASE_LOG_PATH_TEXT"].ToString();
        public static string BASE_LOG_PATH_EX = ConfigurationManager.AppSettings["BASE_LOG_PATH_EX"].ToString();
        public static string SQL_COMMAND_TIMEOUT = "0";
        public static string BASE_TEMPLATE_LOG_EX_FILENAME = ConfigurationManager.AppSettings["BASE_TEMPLATE_LOG_EX_FILENAME"].ToString();
        public static string UPLOAD_FILE = ConfigurationManager.AppSettings["Upload_file"].ToString();

        public static string UPLOAD_FILE_AVD = ConfigurationManager.AppSettings["AdvImagePath"].ToString();
        public static string UPLOAD_SMALL_IMG = ConfigurationManager.AppSettings["UploadSmallImg"].ToString();
        public static string UPLOAD_FASHIONLARGERIMG = ConfigurationManager.AppSettings["UploadFashionLargeImg"].ToString();
        public static string UPLOAD_FILE_HTML = ConfigurationManager.AppSettings["UploadFileHtml"].ToString();
        public static string UPLOAD_FILE_REPORT = ConfigurationManager.AppSettings["UploadReport"].ToString();



        public const string PATTERN_FORMAT_NUMBER = "N00";           //10,352,772
        public const string PATTERN_FORMAT_NUMBER3 = "F02";           //772.00
        public const string PATTERN_FORMAT_NUMBER4 = "F03";           //772.000
        public const string PATTERN_FORMAT_NUMBER5 = "{0:C}";           // $1,234.00
        public const string PATTERN_FORMAT_NUMBER8 = "{0:N}";           // 100.00
        public const string CultureInfo_US = "en-US";
        public const string CultureInfo_VN = "vi-VN";
        public const string FORMAT_DATETIME_1 = "dd/MM/yyyy HH:mm";// (24/10/2012 15:36)
        public const string FORMAT_DATETIME_2 = "dd/MM/yyyy";// (24/10/2012)
        public const string FORMAT_TIME_1 = "HH:mm:ss";// (11:30:00)
        public const string FORMAT_TIME_2 = "dd/MM/yyyy HH:mm:ss";// (24/10/2012 11:30:00)

        public const string CHAR_CRLF = "\r\n";
        public const string CHAR_TAB = "\t";

        /*Hotels*/
        public static string SP_HOTELS_GETALL = "sp_Hotels_GetAll";

        /*SysUser*/
        public static string SP_SYSUSER_GETALLUSER = "sp_SysUser_GetAllUser";
        public static string SP_SYSUSER_INSERT = "sp_SysUsers_Insert";
        public static string SP_SYSUSER_GETINFORUSER = "sp_SysUser_GetInforUser";
        public static string SP_SYSUSER_DETAILSYSUSER = "sp_Account_DetailSysUser";
        public static string SP_SYSUSER_UPDATESYSUSER = "sp_Account_UpdateSysUser";
        public static string SP_ACCOUNT_GETGROUPSBYUSER = "sp_Account_GetGroupsByUser";
        public static string SP_SYSUSER_DELETESYSUSER = "sp_Account_DeleteSysUser";

        /*TrungND Room*/
        public static string SP_SYSROOM_GETALLROOM = "sp_Room_GetAllRoom";
        public static string SP_SYSROOM_CREATEROOM = "sp_Create_Rooms";
        public static string SP_ROOM_UPDATE_ROOMNAME = "sp_Room_Update_RoomName";
        public static string SP_ROOM_DELETE_ROOM = "sp_Room_Delete_Room";
        public static string SP_ROOM_UPDATE_ROOMTYPE = "sp_Room_Update_RoomType";

        /*TrungND RoomType*/
        public static string SP_ROOMTYPE_CREATE = "sp_RoomType_Create";
        /*TrungND Phụ trội*/
        public static string SP_PHUTROI_CREATE = "sp_PhuTroi_Create";


        /*Session*/
        public static string SESSIONMESS = "mess";
        public static string SESSION_USERID = "userid";
        public static string SESSION_USERNAME = "username";
        public static string SESSION_FULLNAME = "fullname";
        public static string SESSION_HOTELCODE = "hotelcode";
        public static string SESSION_HOTELNAME = "hotelname";
        public static string SESSION_PW = "pw";
        public static string SESSION_MENU_LIST = "menulist";
        public static string SESSION_RIGHT_PERMISSION = "right_permission";
        public static string SESSION_HOTELGROUPCODE = "HGCode";
        public static string SESSION_HOTELID = "hotelid";
        public static string SESSION_GROUPCODE = "GROUPCODE";

        /*ThichPV - */
        public static string SP_GET_GROUPS = "sp_Get_Groups"; 
        public static string SP_GET_GROUPS_BY_ID = "sp_Get_Groups_ByID";
        public static string SP_CREATE_GROUPS = "sp_Create_Groups";
        public static string SP_ACCESSRIGHT_CREATE = "sp_AccessRight_Create";
        public static string SP_GET_ACCESSRIGHT = "sp_Get_AccessRight";
        public static string SP_GET_GROUPTYPE = "sp_GroupType_GetAll";
        public static string SP_CREATE_GROUPSMENU_REL = "sp_Create_GroupsMenuRel";
        public static string SP_DELETE_GROUPSMENU_REL = "sp_Delete_GroupMenuRel";
        public static string SP_GET_GROUPSMENU_REL = "sp_Get_GroupsMenuRel";
        public static string SP_GET_ALL_MENUS_LIST ="sp_Get_All_Menus_list";
        public static string SP_GET_RIGHT_PERMISSION = "sp_right_permission";
        public static string SP_UPDATE_ACCESSRIGHT = "sp_Update_AccessRight";
        public static string SP_GET_CATEGORY = "sp_Get_Category";
        public static string SP_GET_RESERVATION_TYPE = "sp_Get_Reservation_Type";
        public static string SP_GET_ROOM_STATUS = "sp_Get_Room_Status";
        public static string SP_GET_ROOM_TYPE = "sp_Get_Room_Type";
        public static string SP_GET_ROOM = "sp_Get_Room";
        public static string SP_GET_COUNTRY = "sp_Get_Country";
        public static string SP_GET_RESERVATION_ROOM = "sp_Get_Customer_Booking";
        public static string SP_GET_RESERVATION_ROOM_DETAIL = "sp_Get_Customer_Booking_Detail";

        /*2016-10-24 09:55:44 NamLD
         * List Menu
         * list hotel
         */
        public static string SP_MENU_GET_BY_USER = "sp_Menu_Get_By_User";
        public static string SP_HOTELS_GET_LIST = "sp_Hotels_Get_List";
        public static string SP_HOTELS_INSERT = "sp_Hotels_Insert";
        public static string SP_HOTELS_UPDATE = "sp_Hotels_Update";
        public static string SP_HOTELS_DELETE = "sp_Hotels_Delete";


        /*LamMN*/
        public static string SP_ACCOUNT_LOGIN = "sp_Account_Login";
        public static string SP_ACCOUNT_REGISTER = "sp_Account_Register";
        public static string SP_ACCOUNT_GET_INFO = "sp_Account_GetInfomation";
        public static string SP_ACCOUNT_UPDATE_PW = "sp_Account_Update_PW";
        public static string SP_TERRITORIES = "sp_Territories";
        public static string SP_USERDETAIL_UPDATE = "sp_Account_UserUpdate"; //update thông tin user (user cập nhật)

        /**
         * 2016-11-15 10:06:21 NamLD
         * cac thong bao tra ket qua
         */
        public static string MESSAGE_INSERT_SUCCESS = "Thêm mới thành công";
        public static string MESSAGE_INSERT_FAIL = " đã tồn tại";
        //public static string MESSAGE_INSERT_SUCCESS = "Thêm thành công";
        //public static string MESSAGE_INSERT_SUCCESS = "Thêm thành công";

        /*Supplier*/
        public static string SP_SUPPLIER_GETALLUSER = "sp_Supplier_GetAllUser";
        public static string SP_SUPPLIER_INSERT = "sp_Supplier_Insert";
        public static string SP_SUPPLIER_GETINFORUSER = "sp_Supplier_GetInforUser";
        public static string SP_SUPPLIER_DETAILSUPPLIER = "sp_Supplier_DetailSupplier";
        public static string SP_SUPPLIER_UPDATESUPPLIER = "sp_Supplier_UpdateSupplier";
        public static string SP_SUPPLIER_DELETESUPPLIER = "sp_Supplier_DeleteSupplier";
    }
}