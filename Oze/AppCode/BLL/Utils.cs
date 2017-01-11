using System;
using System.Web;
using System.Web.UI.WebControls;
using System.IO;
using System.Data;
using System.Data.SqlClient;

namespace Oze.AppCode.BLL
{
    public class Utils
    {
        //#region public Utils()
        //public Utils()
        //{
        //    //
        //    // TODO: Add constructor logic here
        //    //
        //}
        //#endregion

        //#region DateTime Process

        //#region public static string ConvertStrDate (string strDate)
        ///// <summary>
        ///// Chuyển định dạng của ngày tháng thành mm/dd/yyyy
        ///// </summary>
        ///// <param name="strDate"></param>
        ///// <returns></returns>
        //public static string ConvertStrDate(string strDate)
        //{
        //    string strTemDate = strDate.Substring(3, 2);
        //    string strValue;

        //    strValue = strTemDate + "/";
        //    strTemDate = strDate.Substring(0, 2);
        //    strValue += strTemDate + "/";
        //    strTemDate = strDate.Substring(6, 4);
        //    strValue += strTemDate;

        //    return strValue;
        //}
        //#endregion

        //#region public static string ConvertToEnDate (string strDate)
        //public static string ConvertToEnDate(string strDate)
        //{
        //    string strTemDate = strDate.Substring(3, 2);
        //    string strValue;

        //    strValue = strTemDate + "/";
        //    strTemDate = strDate.Substring(0, 2);
        //    strValue += strTemDate + "/";
        //    strTemDate = strDate.Substring(6, 4);
        //    strValue += strTemDate;

        //    return strValue;
        //}
        //#endregion

        //#region public static string ConvertToEnDateNew (string date)
        ///// <summary>
        ///// Chuyển ngày tháng kiểu dd/mm/yyyy sang kiểu mm/dd/yyyy
        ///// </summary>
        ///// <param name="date">Ngày tháng kiểu dd/mm/yyyy</param>
        ///// <returns>Ngày tháng kiểu mm/dd/yyyy</returns>
        ///// <remarks>
        //public static string ConvertToEnDateNew(string date)
        //{
        //    string[] partsOfDateArray = date.Split(@"/".ToCharArray());
        //    string result, day, month, year;
        //    day = "0" + partsOfDateArray[0];
        //    day = day.Substring(day.Length - 2);
        //    month = "0" + partsOfDateArray[1];
        //    month = month.Substring(month.Length - 2);
        //    year = partsOfDateArray[2];
        //    result = month + "/" + day + "/" + year;
        //    return result;
        //}
        //#endregion

        //#region public static string ConvertToVNDateNew (string date)
        ///// <summary>
        ///// Chuyển ngày tháng kiểu mm/dd/yyyy sang kiểu dd/mm/yyyy
        ///// </summary>
        ///// <param name="date">Ngày tháng kiểu mm/dd/yyyy</param>
        ///// <returns>Ngày tháng kiểu dd/mm/yyyy- ANHNV3</returns> 
        ///// <remarks>
        //public static string ConvertToVNDateNew(string date)
        //{
        //    string[] partsOfDateArray = date.Split(@"/".ToCharArray());
        //    string result, day, month, year;
        //    month = "0" + partsOfDateArray[0];
        //    month = month.Substring(month.Length - 2);
        //    day = "0" + partsOfDateArray[1];
        //    day = day.Substring(day.Length - 2);
        //    year = partsOfDateArray[2];
        //    year = year.Substring(0, 4);
        //    result = day + "/" + month + "/" + year;
        //    return result;
        //}

        ///// <summary>
        ///// Lay dinh dang date dd/MM/yyy hh:mm
        ///// </summary>
        ///// <param name="str"></param>
        ///// <returns></returns>
        //public static string getDateString(string str)
        //{
        //    string strDate;
        //    string strTime;
        //    try
        //    {
        //        DateTime dt;
        //        dt = DateTime.Parse(str); //Convert.ToDateTime(str);
        //        strTime = (dt.Hour < 10 ? "0" + dt.Hour.ToString() : dt.Hour.ToString()) + ":" + (dt.Minute < 10 ? "0" + dt.Minute.ToString() : dt.Minute.ToString());
        //        strDate = string.Format("{0:dd/MM/yyyy}", dt) + " " + strTime;
        //        //return strDate;
        //    }
        //    catch
        //    {
        //        strDate = "UnKnow";
        //    }
        //    return strDate;
        //}
        ///// <summary>
        ///// Lay dinh dang date dd/MM/yyy 
        ///// </summary>
        ///// <param name="str"></param>
        ///// <returns></returns>
        //public static string getDateString_ddMMyyyy(string str)
        //{
        //    string strDate;

        //    try
        //    {
        //        DateTime dt;
        //        dt = DateTime.Parse(str); //Convert.ToDateTime(str);
        //        strDate = string.Format("{0:dd/MM/yyyy}", dt);
        //        //return strDate;
        //    }
        //    catch
        //    {
        //        strDate = "UnKnow";
        //    }
        //    return strDate;
        //}
        ///// <summary>
        ///// Lay datetime from chuoi text
        ///// </summary>
        ///// <param name="strddMMYYYY"></param>
        ///// <returns></returns>
        //public static DateTime getDateFromDDMMYYYY(string strddMMYYYY)
        //{
        //    string strDate = "";
        //    string strTime;
        //    DateTime dt = DateTime.Now;
        //    strTime = (dt.Hour < 10 ? "0" + dt.Hour.ToString() : dt.Hour.ToString()) + ":" + (dt.Minute < 10 ? "0" + dt.Minute.ToString() : dt.Minute.ToString());

        //    string[] arr = strddMMYYYY.Split('/');
        //    try
        //    {
        //        if (arr.Length > 2)
        //        {
        //            strDate = arr[1] + "/" + arr[0] + "/" + arr[2];
        //            dt = Convert.ToDateTime(strDate + " " + strTime);
        //        }

        //    }
        //    catch
        //    {

        //    }
        //    return dt;
        //}

        ///// <summary>
        ///// /
        ///// </summary>
        ///// <param name="isCheck"></param>
        ///// <returns></returns>
        //public static string getCheckbox(string isCheck)
        //{
        //    if (isCheck.ToLower() == "true" || isCheck.ToLower() == "1")
        //    { return "<input type='checkbox'  checked disabled = true>"; }
        //    else return "<input type='checkbox' disabled = true>";
        //}

        //#endregion

        //#region CheckFormatDateTime
        //private static bool CheckFormat()
        //{
        //    //	
        //    //			string strFormat = dtFormat.ShortDatePattern;
        //    //			CultureInfo ciInfo = new CultureInfo("en-US");
        //    //			sting strFromat = ciInfo.DateTimeFormat;
        //    //			if ((strFormat == "M/d/yyyy") || (strFormat == "MM/dd/yyyy")) return true;
        //    return false;
        //    //			DateTime dt = new DateTime(DateTime.Now.Year,12,1,2,2,3);
        //    //			string strFormat = dt.GetDateTimeFormats();
        //    //			if (strFormat == "dd/MM/yyyy") return false;


        //}
        //#endregion

        //#region public static string ConvertStringDate (string strDate)
        ///// <summary>
        ///// Chuyển định dạng của ngày tháng thành mm/dd/yyyy
        ///// </summary>
        ///// <param name="strDate"></param>
        ///// <returns></returns>
        //public static string ConvertStringDate(string strDate)
        //{
        //    string CharJoin = strDate.Substring(2, 1);
        //    string[] strTempDate = strDate.Split(Convert.ToChar(CharJoin));
        //    string strValue;
        //    strValue = strTempDate[1] + CharJoin;
        //    strValue += strTempDate[0] + CharJoin;
        //    strValue += strTempDate[2];
        //    return strValue;
        //}
        //public static string ConvertToEngDate(string strDate)
        //{
        //    string strTemDate = strDate.Substring(3, 2);
        //    string strValue;

        //    strValue = strTemDate + "/";
        //    strTemDate = strDate.Substring(0, 2);
        //    strValue += strTemDate + "/";
        //    strTemDate = strDate.Substring(6, 4);
        //    strValue += strTemDate;

        //    return strValue;
        //}
        //#endregion
        //#endregion

        //#region Counter (Đếm số lượt truy cập)

        //#region public int GetTotalHitsFromDB()

        ///// <summary>
        ///// Lấy về biến TotalHits từ CSDL
        ///// </summary>
        ///// <returns>Giá trị cần lấy về</returns>
        //public int GetTotalHitsFromDB(string strConnectString)
        //{            
        //    int result = 0;
        //    CSQLHelper datamgr = new CSQLHelper(strConnectString);
        //    if (datamgr._OpenConnection() == false)
        //        return result;
        //    try
        //    {
        //        string sqlQuery = "SELECT TotalHits FROM hit_tbl_Statistic";
        //        //SqlConnection sqlConn = DataManager.GetConnection();
        //        //sqlConn.Open();

        //        SqlDataReader sqlDR = datamgr.ExecuteReader(sqlQuery);
        //        if (sqlDR.HasRows)
        //        {
        //            while (sqlDR.Read())
        //            {
        //                result = sqlDR.GetInt32(0);
        //            }
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        result = 0;
        //    }
        //    finally
        //    {
        //        datamgr._CloseConnection();
        //    }
        //    return result;
        //}

        //#endregion

        //#region public bool UpdateTotalHitsToDB(int currentHits)

        ///// <summary>
        ///// Cập nhật biến TotalHits vào CSDL
        ///// </summary>
        ///// <param name="currentHits">Giá trị cần cập nhật</param>
        //public bool UpdateTotalHitsToDB(int currentHits, string strConnectString)
        //{
        //    bool result = false;
        //    CSQLHelper datamgr = new CSQLHelper(strConnectString);
        //    if (datamgr._OpenConnection() == false)
        //        return result;

        //    try
        //    {
        //        string sqlQuery = "UPDATE hit_tbl_Statistic SET TotalHits = " + currentHits.ToString();
        //        datamgr.ExecuteNonQuery(sqlQuery);
        //        result = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        result = false;
        //    }
        //    finally
        //    {
        //        datamgr._CloseConnection();
        //    }
        //    return result;
        //}

        //#endregion

        //#endregion

        //#region Injection
        //public static string ReplaceText(string strText)
        //{
        //    strText = strText.Replace("'", "''");
        //    strText = strText.Replace("<", "");
        //    strText = strText.Replace(">", "");
        //    return strText;
        //}
        //#endregion

        //#region public void addPopUpCalendarToImage (...)

        //public void addPopUpCalendarToImage(System.Web.UI.WebControls.Image image, TextBox textbox, string datetimeFormat, System.Web.UI.WebControls.CustomValidator cvCheckDate)
        //{
        //    if (Utils.UserDate().ToUpper() != Utils.SystemDate().ToUpper())
        //        cvCheckDate.ClientValidationFunction = "validateFromDate_vi_VN";
        //    else
        //        cvCheckDate.ClientValidationFunction = "validateFromDate_vi_EN";
        //    cvCheckDate.ControlToValidate = textbox.ID;
        //    cvCheckDate.CssClass = "LabelError";
        //    image.Attributes.Add("onclick", "popUpCalendar(document.forms[0]." + textbox.ID + ", document.forms[0]." + textbox.ID + ", '" + datetimeFormat + "'); return false;");
        //}

        //#endregion

        //#region Upload File

        //#region public static string UploadImageFromLocal(...)
        ///// <summary>
        ///// Modified by MinhTQ
        ///// TODO: Tra ve string upload de upload file len server vao thu muc xac dinh trong web.config
        ///// Copyright (c) Fpt Software Solutions, FEB - 2005
        ///// </summary>
        ///// <param name="rawFileName"></param>
        ///// <returns></returns>
        //public static string UploadImageFromLocal(string rawFileName, string filename)
        //{
        //    string strFileName = rawFileName;
        //    filename = filename.Replace(@"/", "_");

        //    //string last = strFileName.Substring(0,strFileName.Length - 3);
        //    string last = Path.GetFileName(strFileName.Substring(strFileName.Length - 3));

        //    strFileName = filename + "." + last;//Path.GetFileName(strFileName);			
        //    //Upload file vao thu muc da config tren server			
        //    string uploadPath = CConfig.UPLOAD_FILE;
        //    uploadPath = HttpContext.Current.Server.MapPath(uploadPath);

        //    string uploadFileName = uploadPath + strFileName.Trim();
        //    return uploadFileName;
        //}
        //#endregion

        //#region public static string UploadImageFromAdv(...)
        ///// <summary>
        ///// Modified by MinhTQ
        ///// TODO: Tra ve string upload de upload file len server vao thu muc xac dinh trong web.config
        ///// Copyright (c) Fpt Software Solutions, FEB - 2005
        ///// </summary>
        ///// <param name="rawFileName"></param>
        ///// <returns></returns>
        //public static string UploadImageFromAdv(string rawFileName)
        //{
        //    //Lay ve ten File image can upload
        //    string strFileName = rawFileName;
        //    strFileName = Path.GetFileName(strFileName);

        //    //Xu ly: them ngay thang upload vao ten file
        //    string dateUpload = DateTime.Now.ToShortDateString();
        //    string[] infoArray = dateUpload.Split(new char[1] { '/' });
        //    dateUpload = string.Join("", infoArray);

        //    //Upload file vao thu muc da config tren server			
        //    string uploadPath = CConfig.UPLOAD_FILE_AVD;
        //    uploadPath = HttpContext.Current.Server.MapPath(uploadPath);

        //    string uploadFileName = uploadPath + dateUpload.Trim() + strFileName.Trim();
        //    return uploadFileName;
        //}
        //#endregion

        //#region public static string UploadFileFromProduct(...)
        ///// <summary>
        ///// Modified by MinhTQ
        ///// TODO: Tra ve string upload de upload file len server vao thu muc xac dinh trong web.config
        ///// Copyright (c) Fpt Software Solutions, FEB - 2005
        ///// </summary>
        ///// <param name="rawFileName"></param>
        ///// <returns></returns>
        //public static string UploadFileFromProduct(string rawFileName)
        //{
        //    //Lay ve ten File image can upload
        //    string strFileName = rawFileName;
        //    strFileName = Path.GetFileName(strFileName);

        //    //Xu ly: them ngay thang upload vao ten file
        //    string dateUpload = DateTime.Now.ToShortDateString();
        //    string[] infoArray = dateUpload.Split(new char[1] { '/' });
        //    dateUpload = string.Join("", infoArray);

        //    //Upload file vao thu muc da config tren server			
        //    string uploadPath = CConfig.UPLOAD_SMALL_IMG;
        //    uploadPath = HttpContext.Current.Server.MapPath(uploadPath);

        //    string uploadFileName = uploadPath + dateUpload.Trim() + strFileName.Trim();
        //    return uploadFileName;
        //}
        //#endregion

        //#region public static string UploadFileFromFashion(...)
        ///// <summary>
        ///// Modified by HaDM
        ///// TODO: Tra ve string upload de upload file len server vao thu muc xac dinh trong web.config
        ///// Copyright (c) Fpt Software Solutions, FEB - 2005
        ///// </summary>
        ///// <param name="rawFileName"></param>
        ///// <returns></returns>
        //public static string UploadFileFromFashion(string rawFileName)
        //{
        //    //Lay ve ten File image can upload
        //    string strFileName = rawFileName;
        //    strFileName = Path.GetFileName(strFileName);

        //    //Xu ly: them ngay thang upload vao ten file
        //    string dateUpload = DateTime.Now.ToShortDateString();
        //    string[] infoArray = dateUpload.Split(new char[1] { '/' });
        //    dateUpload = string.Join("", infoArray);

        //    //Upload file vao thu muc da config tren server			
        //    string uploadPath = CConfig.UPLOAD_FASHIONLARGERIMG;
        //    uploadPath = HttpContext.Current.Server.MapPath(uploadPath);

        //    string uploadFileName = uploadPath + dateUpload.Trim() + strFileName.Trim();
        //    return uploadFileName;
        //}
        //#endregion

        //#region public static string UploadNewsFashionImg(...)
        ///// <summary>
        ///// Modified by HaDM
        ///// TODO: Tra ve string upload de upload file len server vao thu muc xac dinh trong web.config
        ///// Copyright (c) Fpt Software Solutions, FEB - 2005
        ///// </summary>
        ///// <param name="rawFileName"></param>
        ///// <returns></returns>
        //public static string UploadNewsFashionImg(string rawFileName)
        //{
        //    //Lay ve ten File image can upload
        //    string strFileName = rawFileName;
        //    strFileName = Path.GetFileName(strFileName);

        //    //Xu ly: them ngay thang upload vao ten file
        //    string dateUpload = DateTime.Now.ToShortDateString();
        //    string[] infoArray = dateUpload.Split(new char[1] { '/' });
        //    dateUpload = string.Join("", infoArray);

        //    //Upload file vao thu muc da config tren server			
        //    string uploadPath = CConfig.UPLOAD_FASHIONLARGERIMG;
        //    uploadPath = HttpContext.Current.Server.MapPath(uploadPath);

        //    string uploadFileName = uploadPath + dateUpload.Trim() + strFileName.Trim();
        //    return uploadFileName;
        //}
        //#endregion

        //#region public static string UploadFileFromMember(...)
        ///// <summary>
        ///// Modified by HaDM
        ///// TODO: Tra ve string upload de upload file len server vao thu muc xac dinh trong web.config
        ///// Copyright (c) Fpt Software Solutions, FEB - 2005
        ///// </summary>
        ///// <param name="rawFileName"></param>
        ///// <returns></returns>
        //public static string UploadFileFromMember(string rawFileName)
        //{
        //    //Lay ve ten File image can upload
        //    string strFileName = rawFileName;
        //    strFileName = Path.GetFileName(strFileName);

        //    //Xu ly: them ngay thang upload vao ten file
        //    string dateUpload = DateTime.Now.ToShortDateString();
        //    string[] infoArray = dateUpload.Split(new char[1] { '/' });
        //    dateUpload = string.Join("", infoArray);

        //    //Upload file vao thu muc da config tren server			
        //    string uploadPath = CConfig.UPLOAD_FASHIONLARGERIMG;
        //    uploadPath = HttpContext.Current.Server.MapPath(uploadPath);

        //    string uploadFileName = uploadPath + dateUpload.Trim() + strFileName.Trim();
        //    return uploadFileName;
        //}
        //#endregion

        //#region public static string UploadFileHTML(...)
        ///// <summary>
        ///// Modified by MinhTQ
        ///// TODO: Tra ve string upload de upload file len server vao thu muc xac dinh trong web.config
        ///// Copyright (c) Fpt Software Solutions, FEB - 2005
        ///// </summary>
        ///// <param name="rawFileName"></param>
        ///// <returns></returns>
        //public static string UploadFileHTML(string rawFileName)
        //{
        //    //Lay ve ten File image can upload
        //    string strFileName = rawFileName;
        //    strFileName = Path.GetFileName(strFileName);

        //    //Xu ly: them ngay thang upload vao ten file
        //    //			string dateUpload = DateTime.Now.ToShortDateString();
        //    //			string[] infoArray = dateUpload.Split(new char[1]{'/'});
        //    //			dateUpload = string.Join("",infoArray);

        //    //Upload file vao thu muc da config tren server			
        //    string uploadPath = CConfig.UPLOAD_FILE_HTML;
        //    uploadPath = HttpContext.Current.Server.MapPath(uploadPath);

        //    string uploadFileName = uploadPath + strFileName.Trim();
        //    return uploadFileName;
        //}
        //public static string UploadFileIMG(string rawFileName)
        //{
        //    //Lay ve ten File image can upload
        //    string strFileName = rawFileName;
        //    strFileName = Path.GetFileName(strFileName);

        //    //Xu ly: them ngay thang upload vao ten file
        //    //string dateUpload = DateTime.Now.ToShortDateString();
        //    //string[] infoArray = dateUpload.Split(new char[1]{'/'});
        //    //dateUpload = string.Join("",infoArray);

        //    //Upload file vao thu muc da config tren server			
        //    string uploadPath = CConfig.UPLOAD_FILE_HTML;
        //    uploadPath = HttpContext.Current.Server.MapPath(uploadPath);

        //    string uploadFileName = uploadPath + strFileName.Trim();
        //    return uploadFileName;
        //}
        //#endregion

        //#region public static string UploadNewsFile(string rawFileName)
        //public static string UploadNewsFile(string rawFileName)
        //{
        //    //Lay ve ten File image can upload
        //    string strFileName = rawFileName;
        //    strFileName = Path.GetFileName(strFileName);

        //    //Xu ly: them ngay thang upload vao ten file
        //    //			string dateUpload = DateTime.Now.ToShortDateString();
        //    //			string[] infoArray = dateUpload.Split(new char[1]{'/'});
        //    //			dateUpload = string.Join("",infoArray);

        //    //Upload file vao thu muc da config tren server			
        //    string uploadPath = CConfig.UPLOAD_FILE_HTML;
        //    uploadPath = HttpContext.Current.Server.MapPath(uploadPath);

        //    string uploadFileName = uploadPath + strFileName.Trim();
        //    return uploadFileName;
        //}
        //#endregion

        //#region public static string UploadBanner(...)
        ///// <summary>
        ///// Modified by MinhTQ
        ///// TODO: Tra ve string upload de upload file len server vao thu muc xac dinh trong web.config
        ///// Copyright (c) Fpt Software Solutions, FEB - 2005
        ///// </summary>
        ///// <param name="rawFileName"></param>
        ///// <returns></returns>
        //public static string UploadBanner(string rawFileName)
        //{
        //    //Lay ve ten File image can upload
        //    string strFileName = rawFileName;
        //    strFileName = Path.GetFileName(strFileName);

        //    //Upload file vao thu muc da config tren server			
        //    string uploadPath = CConfig.UPLOAD_SMALL_IMG;
        //    uploadPath = HttpContext.Current.Server.MapPath(uploadPath);

        //    string uploadFileName = uploadPath + strFileName.Trim();
        //    return uploadFileName;
        //}

        //#endregion

        //#region public static string UploadReportFileName(...)
        ///// <summary>
        ///// Modified by MinhTQ
        ///// TODO: Tra ve string upload de upload file len server vao thu muc xac dinh trong web.config
        ///// Copyright (c) Fpt Software Solutions, FEB - 2005
        ///// </summary>
        ///// <param name="rawFileName"></param>
        ///// <returns></returns>	

        //public static string UploadReportFileName(string rawFileName, string stringfileName)
        //{
        //    //Lay ve ten File image can upload
        //    string strFileName = rawFileName;

        //    //string last = strFileName.Substring(0,strFileName.Length - 3);
        //    string last = Path.GetFileName(strFileName.Substring(strFileName.Length - 3));

        //    strFileName = stringfileName + "." + last;//Path.GetFileName(strFileName);			

        //    //Upload file vao thu muc da config tren server			
        //    string uploadPath = CConfig.UPLOAD_FILE_REPORT;
        //    uploadPath = HttpContext.Current.Server.MapPath(uploadPath);

        //    string uploadFileName = uploadPath + strFileName.Trim();
        //    return uploadFileName;
        //}
        //public static string UploadReportFileName123(string rawFileName, string stringfileName)
        //{
        //    //Lay ve ten File image can upload
        //    string strFileName = rawFileName;

        //    //string last = strFileName.Substring(0,strFileName.Length - 3);
        //    string last = Path.GetFileName(strFileName.Substring(strFileName.Length - 3));

        //    strFileName = stringfileName + "." + last;//Path.GetFileName(strFileName);			

        //    //Upload file vao thu muc da config tren server			
        //    string uploadPath = CConfig.UPLOAD_FILE_REPORT;
        //    uploadPath = HttpContext.Current.Server.MapPath(uploadPath);

        //    string uploadFileName = uploadPath + strFileName.Trim();
        //    return uploadFileName;
        //}
        //#endregion

        //#endregion

        //#region UserDate()
        //public static string UserDate()
        //{
        //    string strUserDate = Utils.InterfaceDate().ToLower();
        //    return strUserDate;
        //}
        //#endregion

        //#region SystemDate()
        //public static string SystemDate()
        //{
        //    string strDate = CConfig.FORMAT_DATETIME_1;
        //    return strDate;
        //}
        //#endregion

        //#region InterfaceDate()
        ////Dùng cho Calendar
        //public static string InterfaceDate()
        //{
        //    string strDate = CConfig.FORMAT_DATETIME_1;
        //    return strDate;
        //}
        //#endregion

        //#region ConvertSystemDate()
        //public static string ConvertSystemDate(string txtDate)
        //{
        //    string dDate;
        //    if (Utils.UserDate().ToUpper() != Utils.SystemDate().ToUpper())
        //    {
        //        dDate = Utils.ConvertToEnDate(txtDate);
        //    }
        //    else
        //    {
        //        dDate = txtDate;
        //    }
        //    return dDate;
        //}
        //#endregion

        //#region CreateDirectory(string FolderPath)
        ///// <summary>
        ///// Created by BinhTD on 13/07/2005
        ///// TODO: Tao mot thu muc voi duong dan truyen vao
        ///// </summary>
        ///// <param name="FolderPath"></param>
        ///// <returns></returns>
        //public static bool CreateDirectory(string FolderPath)
        //{
        //    System.IO.Directory.CreateDirectory(FolderPath);
        //    return true;
        //}
        //#endregion

        //#region WriteToTextFile(string FileName, string FileContents)
        ///// <summary>
        ///// Created by BinhTD on 13/07/2005
        ///// TODO: Tao File Text va Truyen du lieu vao file
        ///// </summary>
        ///// <param name="FileName">Ten file</param>
        ///// <param name="FileContents">Noi dung dua vao file</param>
        ///// <returns></returns>
        //public static bool WriteToTextFile(string FileName, string FileContents)
        //{
        //    FileStream fs; // Writes the FileContents to a file
        //    System.IO.StreamWriter sw; // Streams the text data to the FileStream object.

        //    // Create a file to hold the output. (existing files will be overwritten)
        //    //fs = new FileStream(FileName, FileMode.Create, FileAccess.Write);
        //    fs = new FileStream(FileName, FileMode.Append, FileAccess.Write);
        //    sw = new StreamWriter(fs);

        //    sw.WriteLine(FileContents);

        //    sw.Flush();

        //    // Close the output file.
        //    sw.Close();
        //    fs.Close();

        //    return true;

        //}
        //#endregion

        //#region Check Valid Date
        ///// <summary>
        ///// Kiem tra xem ngay nhap vao co vuot qua kha nang luu tru cua Data Base 
        ///// Create By THOPD
        ///// </summary>
        ///// <param name="date">ngay can kiem tra</param>
        ///// <returns>true: hop le</returns>
        //public static bool CheckValidDate(string date)
        //{
        //    DateTime compareDate = new DateTime(2079, 1, 1);
        //    DateTime lowerDate = new DateTime(1900, 1, 1);
        //    if (date.Trim().Length == 0) return true;
        //    DateTime currentDate = Convert.ToDateTime(Utils.ConvertSystemDate(date));
        //    if ((currentDate >= compareDate) || (currentDate < lowerDate)) return false;
        //    return true;
        //}
        //#endregion

        //#region public void CheckAccessRight(string role)
        ///// <summary>
        ///// Kiểm tra xem người dùng hiện tại có quyền vào trang hay không
        /////		1. Nếu người duyệt chưa đăng nhập, chuyển trang hiện tại 
        /////		tới trang đăng nhập
        /////		2. Nếu người duyệt đã đăng nhập nhưng không có quyền vào trang,
        /////		chuyển trang hiện tại tới trang thông báo
        ///// </summary>
        ///// <param name="role">Quyền cần kiểm tra</param>
        //public void CheckAccessRight(string role)
        //{
        //    //try
        //    //{
        //    //    HttpContext hc = HttpContext.Current;
        //    //    if (!hc.User.Identity.IsAuthenticated)
        //    //    {
        //    //        //Hien thi trang thai Guest
        //    //        hc.Response.Redirect(GlobalVariables.projectPath + "?dir=Login", true);
        //    //    }
        //    //    else
        //    //    {
        //    //        System.Security.Principal.IPrincipal user = hc.User;
        //    //        SitePrincipal sp = new SitePrincipal(user.Identity.Name);
        //    //        if (!sp.IsInRole(role))
        //    //            hc.Response.Redirect(GlobalVariables.projectPath + "WebPage/UnderContruction/UnAuthentication.aspx", true);
        //    //    }
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    throw ex;
        //    //}
        //}
        //#endregion

        //#region public string UploadFileFromLocal(...)
        ///// <summary>
        ///// Modified by MinhTQ
        ///// TODO: Tra ve string upload de upload file len server vao thu muc xac dinh trong web.config
        ///// Copyright (c) Fpt Software Solutions, FEB - 2005
        ///// </summary>
        ///// <param name="rawFileName"></param>
        ///// <returns></returns>
        //public string UploadFileFromLocal(string rawFileName)
        //{
        //    ////Lay ve ten File image can upload
        //    //string strFileName = rawFileName;
        //    //strFileName = Path.GetFileName(strFileName);

        //    ////Xu ly: them ngay thang upload vao ten file
        //    //string dateUpload = DateTime.Now.ToString("ddMMyyyy_hhmm");
        //    ////			string[] infoArray = dateUpload.Split(new char[1]{'/'});
        //    ////			dateUpload = string.Join("",infoArray);

        //    ////Upload file vao thu muc da config tren server			
        //    //string uploadPath = ConfigurationHelper.GetConfigValue("UploadPath");
        //    //uploadPath = HttpContext.Current.Server.MapPath(uploadPath);

        //    //string uploadFileName = uploadPath + dateUpload.Trim() + "_" + strFileName.Trim();
        //    //return uploadFileName;
        //    return null;
        //}
        //#endregion

        //#region ShowMessageBox
        //public static void ShowMessageBox(System.Web.UI.Page pPage, string pMessage)
        //{
        //    //string strScript = "<script language=JavaScript>";
        //    //strScript += "alert('" + pMessage + "');";
        //    //strScript += "</script>";

        //    //if (!pPage.IsStartupScriptRegistered("clientScript"))
        //    //    pPage.RegisterStartupScript("clientScript", strScript);

        //}
        //#endregion

        //#region ProcessString
        ///// <summary>
        ///// Cắt ngắn đoạn text
        ///// </summary>
        ///// <param name="input"></param>
        ///// <param name="characterLimit"></param>
        ///// <returns></returns>
        //public static string Truncate2(string input, int characterLimit)
        //{
        //    string output = Truncate(input, characterLimit);
        //    if (output.Trim().Length == 0)
        //    {
        //        output = "<br />";
        //    }
        //    return output;
        //}

        ///// <summary>
        ///// Cắt ngắn đoạn text
        ///// </summary>
        ///// <param name="input"></param>
        ///// <param name="characterLimit"></param>
        ///// <returns></returns>
        //public static string Truncate(string input, int characterLimit)
        //{
        //    string output = input;

        //    // Check if the string is longer than the allowed amount
        //    // otherwise do nothing
        //    if (output.Length > characterLimit && characterLimit > 0)
        //    {

        //        // cut the string down to the maximum number of characters
        //        output = output.Substring(0, characterLimit);

        //        // Check if the character right after the truncate point was a space
        //        // if not, we are in the middle of a word and need to remove the rest of it
        //        if (input.Substring(output.Length, 1) != " ")
        //        {
        //            int LastSpace = output.LastIndexOf(" ");

        //            // if we found a space then, cut back to that space
        //            if (LastSpace != -1)
        //            {
        //                output = output.Substring(0, LastSpace);
        //            }
        //        }
        //        // Finally, add the "..."
        //        output += "...";
        //    }
        //    return output;
        //}
        //public static string getNumberFormat(string vValue, int n)
        //{

        //    string strFormat = "{0:N" + n.ToString() + "}";
        //    double dValue = 0;
        //    double.TryParse(vValue, out dValue);
        //    if (dValue == 0 && vValue.Trim() != "") return vValue;
        //    return string.Format(strFormat, dValue);
        //}
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="vValue"></param>
        ///// <param name="n"></param>
        ///// <returns></returns>
        //public static double getNumberFormat_Mul(string vValue, int n, int mul)
        //{
        //    string strFormat = "{0:N" + n.ToString() + "}";
        //    double dValue = 0;
        //    double.TryParse(vValue, out dValue);
        //    //return string.Format(strFormat, dValue * mul);
        //    return dValue;
        //}
        ///// <summary>
        ///// Kiem tra dataset khoong bi null va chua table co index la tableIndex
        ///// </summary>
        ///// <param name="ds"></param>
        ///// <param name="tableIndex"></param>
        ///// <returns></returns>
        //public static bool DataSetIsNotNull(DataSet ds, int tableIndex)
        //{

        //    return ((ds != null) && (ds.Tables.Count > tableIndex) && (ds.Tables[tableIndex] != null) && ds.Tables[tableIndex].Rows.Count > 0);
        //}

        //private static string GetSpace(int level)
        //{
        //    string temp = "&nbsp;&nbsp;&nbsp;";
        //    string retVal = "";
        //    if (level >= 1)
        //    {
        //        for (int i = 2; i <= level; i++)
        //        {
        //            retVal += temp;
        //        }
        //    }
        //    return retVal;
        //}

        ///// <summary>
        ///// Tra lai chuoi so theo format;
        ///// </summary>
        ///// <param name="strFormat"></param>
        ///// <param name="vValue"></param>
        ///// <returns></returns>
        //public static string GetStrNumber(string vValue, int n)
        //{
        //    string strFormat = "{0:N" + n.ToString() + "}";
        //    double dValue = 0;
        //    double.TryParse(vValue, out dValue);
        //    if (dValue == 0 || vValue.Trim() == "") return "-";
        //    return String.Format(strFormat, dValue);
        //}
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="vValue"></param>
        ///// <returns></returns>
        //public static Int64 GetInt64FromString(string vValue)
        //{
        //    Int64 iValue = 0;
        //    Int64.TryParse(vValue, out iValue);
        //    return iValue;
        //}
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="vValue"></param>
        ///// <returns></returns>
        //public static double GetdoubleFromString(string vValue)
        //{
        //    double iValue = 0;
        //    double.TryParse(vValue, out iValue);
        //    return iValue;
        //}
        //#endregion

        
    }
}