using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;
using System.IO;
using System.Globalization;     //language
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Linq;
using System.Data;

namespace Oze.AppCode.BLL
{
    public class CBase
    {

        /// <summary>
        /// ngocta2 2:53 PM 5/19/2014
        /// convert du lieu , thuong dung voi data select tu db ra
        /// http://weblogs.asp.net/psheriff/archive/2013/01/29/building-collections-of-entity-classes-using-a-datareader.aspx
        ///while (rdr.Read())
        ///{
        ///    entity = new Product();
        ///    // ProductId is a NOT NULL field
        ///    entity.ProductId        = Convert.ToInt32(rdr["ProductId"]);
        ///    // Strings automatically convert to "" if null.
        ///    entity.ProductName      = rdr["ProductName"].ToString();
        ///    // nullable values
        ///    entity.IntroductionDate = CBase.ConvertTo<DateTime>(rdr["IntroductionDate"],default(DateTime));
        ///    entity.Cost             = CBase.ConvertTo<decimal>(rdr["Cost"], default(decimal));
        ///    entity.Price            = CBase.ConvertTo<decimal>(rdr["Price"], default(decimal));
        ///    entity.IsDiscontinued   = CBase.ConvertTo<bool>( rdr["IsDiscontinued"], default(bool));
        ///    ret.Add(entity);
        ///}
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T ConvertTo<T>(object value, T defaultValue) where T : struct
        {
            if (value.Equals(DBNull.Value))
                return defaultValue;
            else
                return (T)value;
        }

        /// <summary>
        /// /// VB function : Right
        /// </summary>
        /// <param name="value"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Right(string value, int length)
        {
            return value.Substring(value.Length - length);
        }

        /// <summary>
        /// VB function : Left
        /// </summary>
        /// <param name="s"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string Left(string s, int len)
        {
            if (s == null)
                return s;
            else if (len == 0 || s.Length == 0)
                return "";
            else if (s.Length <= len)
                return s;
            else
                return s.Substring(0, len);
        }


        // replace 1 string nhieu lan
        public static string Repeat(string value, int count)
        {
            return new StringBuilder().Insert(0, value, count).ToString();
        }

        /// <summary>
        /// check SQL injection: ANSI String Value
        /// </summary>
        /// <param name="sData"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static object FieldCheck(string sData)
        {
            object functionReturnValue = null;
            if (System.Convert.IsDBNull(sData))
                functionReturnValue = "Null";
            else
                if (string.IsNullOrEmpty(sData.Trim()))
                    functionReturnValue = "Null";
                else
                    functionReturnValue = "'" + sData.Trim().Replace("'", "''") + "'";
            return functionReturnValue;
        }

        /// <summary>
        /// check SQL injection: Unicode String Value
        /// </summary>
        /// <param name="sData"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static object FieldUnicodeCheck(string sData)
        {
            object functionReturnValue = null;
            if (System.Convert.IsDBNull(sData))
                functionReturnValue = "Null";
            else
                if (string.IsNullOrEmpty(sData.Trim()))
                    functionReturnValue = "Null";
                else
                    functionReturnValue = "N'" + sData.Trim().Replace("'", "''") + "'";
            return functionReturnValue;
        }

        /// <summary>
        /// check SQL injection: Number Value
        /// </summary>
        /// <param name="sData"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static object ValueCheck(string sData)
        {
            return sData;
            object functionReturnValue = null;
            if (System.Convert.IsDBNull(sData))
                functionReturnValue = "Null";
            else
                if (System.Convert.IsDBNull(sData))
                    functionReturnValue = sData.Trim().Replace(",", "");
                else
                    functionReturnValue = "Null";
            return functionReturnValue;
        }

        //truy van URL 
        // strURL = http://liveprice.fpts.com.vn/monitor.asp?k=MW2_SRV_HTML_STOCK_INFO_1_ABT
        // strURL = https://172.16.0.11/mw/4g/monitor.asp?k=MW2_SRV_HTML_STOCK_INFO_1_ABT
        public static string GetURLResponse(string strURL)
        {
            try
            {
                string s = strURL.ToLower();
                if (s.Substring(0, 8) == "https://") return GetURLResponseHTTPS(strURL);
                if (s.Substring(0, 7) == "http://") return GetURLResponseHTTP(strURL);
                return "";
            }
            catch (Exception ex)
            {
                // log error
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
                return "";
            }
        }


        //truy van URL dung WebRequest
        public static string GetURLResponseHTTP(string strURL)
        {
            try
            {
                WebRequest request = WebRequest.Create(strURL);
                WebResponse response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                return responseFromServer;
            }
            catch (Exception ex)
            {
                // log error
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
                return "";
            }
        }

        //truy van URL dung WebClient
        public static string GetURLResponseHTTP2(string strURL)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    string responseFromServer = client.DownloadString(strURL);
                    return responseFromServer;
                }
            }
            catch (Exception ex)
            {
                // log error
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
                return "";
            }
        }

        public static string GetURLResponseHTTPS(string strURL)
        {

            try
            {
                WebRequest request = WebRequest.Create(strURL);
                request.Proxy = null;
                request.Credentials = CredentialCache.DefaultCredentials;

                //allows for validation of SSL certificates      
                ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                return responseFromServer;
            }
            catch (System.Exception ex)
            {
                // log error
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
                return "";
            }

        }

        //for testing purpose only, accept any dodgy certificate... 
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public static string GetCaller(int level = 2)
        {
            var m = new StackTrace().GetFrame(level).GetMethod();

            if (m.DeclaringType == null) return ""; //9:33 AM 6/18/2014 Exception Details: System.NullReferenceException: Object reference not set to an instance of an object.

            // .Name is the name only, .FullName includes the namespace
            var className = m.DeclaringType.FullName;

            //the method/function name you are looking for.
            var methodName = m.Name;

            //returns a composite of the namespace, class and method name.
            return className + "->" + methodName;
        }

        // current function name
        public static string GetCurrentMethod()
        {
            return System.Reflection.MethodBase.GetCurrentMethod().Name;
        }

        // .net 4.0 ko ghi chi tiet duoc error
        // .net 4.5 ghi chi tiet den loi tai dong nao
        public static string GetDetailError(Exception ex)
        {
            string strDetailError = "" +
                "\r\nSource\t\t= " + ex.Source +
                "\r\nTargetSite\t= " + ex.TargetSite +
                "\r\nMessage\t\t= " + ex.Message +
                "\r\nStackTrace\t= " + ex.StackTrace;
            return strDetailError;
                
        }
        /// <summary>
        /// 3=>0 = Microsoft.Samples.AspNetRouteIntegration.Service->HelloWorld=>BaseLib.CSQL->ExecuteSP=>BaseLib.CBase->GetDeepCaller=>BaseLib.CBase->GetCaller=>
        /// 3=>2 = Microsoft.Samples.AspNetRouteIntegration.Service->HelloWorld=>BaseLib.CSQL->ExecuteSP=>
        /// </summary>
        /// <returns></returns>
        public static string GetDeepCaller()
        {
            string strCallerName = "";
            for (int i = 3; i >= 2; i--)
                strCallerName += GetCaller(i) + "=>";

            //returns a composite of the namespace, class and method name.
            return strCallerName;
        }

        //thuyntt 10:44 AM 19/05/2014: format number and date
        //Or even better... (thanks Anon) 
        public static Boolean IsNumeric(string stringToTest)
        {
            double result;
            return double.TryParse(stringToTest, out result);
        }

        //thuyntt 10:44 AM 19/05/2014
        //"dd/MM/yyyy hh:mm";// (24/10/2012 15:36)
        public static string FormatDate(DateTime dt)
        {
            return dt.ToString(CConfig.FORMAT_DATETIME_1);
        }

        //thuyntt 10:44 AM 19/05/2014
        //"dd/MM/yyyy";// (24/10/2012)
        public static string FormatDate2(DateTime dt)
        {
            return dt.ToString(CConfig.FORMAT_DATETIME_2);
        }

        //thuyntt 10:44 AM 19/05/2014
        //"dd/MM/yyyy hh:mm";// (24/10/2012 15:36)
        public static string FormatTime1(DateTime dt)
        {
            return dt.ToString(CConfig.FORMAT_TIME_1);
        }

        //ngocta2 10:41 AM 8/1/2014
        // monitor.redis.viewkeyvalue
        public static string FormatTime2(DateTime dt)
        {
            return dt.ToString(CConfig.FORMAT_TIME_2);
        }

        //thuyntt 10:44 AM 19/05/2014
        // 12345 => 12,345
        public static string FormatNumber(decimal dec32)
        {
            CultureInfo ci = new CultureInfo(CConfig.CultureInfo_US);
            return dec32.ToString(CConfig.PATTERN_FORMAT_NUMBER, ci);
        }

        //thuyntt 10:44 AM 19/05/2014
        // if value < 0 => (value) : -123,45 -> (123,45): for decimal: added by ThuyNT 2013.04.04
        public static string MyFormatNumber(decimal dec32)
        {
            string rs;
            rs = "(" + FormatNumber(dec32) + ")";
            if (dec32 < 0)
            {
                return rs.ToString().Replace("-", "");
            }
            else
            {
                return FormatNumber(dec32);
            }
        }

        //thuyntt 10:44 AM 19/05/2014
        // 12345 => 12,345
        public static string FormatNumber2(object obj)
        {
            if (obj == null) return "0";
            if (IsNumeric(obj.ToString()) == false) return "0";

            CultureInfo ci = new CultureInfo(CConfig.CultureInfo_US);
            return Convert.ToDecimal(obj).ToString(CConfig.PATTERN_FORMAT_NUMBER, ci);
        }

        //thuyntt 10:44 AM 19/05/2014
        // 0,05 => 0.05
        public static string FormatNumber3(object obj)
        {
            if (obj == null) return "0";
            //if (IsNumeric(obj.ToString()) == false) return "0";

            CultureInfo ci = new CultureInfo(CConfig.CultureInfo_US);
            return Convert.ToDouble(obj).ToString(CConfig.PATTERN_FORMAT_NUMBER3, ci);
        }

        //thuyntt 10:44 AM 19/05/2014
        // 0,045 => 0.045
        public static string FormatNumber4(object obj)
        {
            if (obj == null) return "0";
            //if (IsNumeric(obj.ToString()) == false) return "0";

            CultureInfo ci = new CultureInfo(CConfig.CultureInfo_US);
            return Convert.ToDouble(obj).ToString(CConfig.PATTERN_FORMAT_NUMBER4, ci);
        }

        //thuyntt 10:44 AM 19/05/2014
        // 1921.39 => $1,921.39
        public static string FormatNumber5(object obj)
        {
            if (obj == null) return "0";
            return string.Format(CConfig.PATTERN_FORMAT_NUMBER5, Convert.ToDecimal(obj));
        }

        //thuyntt 10:44 AM 19/05/2014
        //4,800 = >4,800.000
        public static string FormatNumber6(decimal dec32)
        {
            string specifier = "0,0.000";
            if (dec32 == 0) return "0.000";
            if (dec32 < 10) return string.Format(CConfig.PATTERN_FORMAT_NUMBER5, dec32.ToString("0.000"));
            return string.Format(CConfig.PATTERN_FORMAT_NUMBER5, dec32.ToString(specifier));
        }

        //thuyntt 10:44 AM 19/05/2014
        //0.02686667 ==> 0.03
        public static string FormatNumber7(object obj)
        {
            if (obj == null) return "0";
            return string.Format(CultureInfo.InvariantCulture, "{0:0.00}", obj);
        }

        //thuyntt 10:44 AM 19/05/2014
        //0.2686667 ==> 0.3 and 35 => 35.0
        public static string FormatNumber10(object obj)
        {
            if (obj == null) return "0";
            if (Convert.ToDecimal(obj) > 1000) return string.Format(CultureInfo.InvariantCulture, "{0:0,0.0}", obj);
            return string.Format(CultureInfo.InvariantCulture, "{0:0.0}", obj);

        }

        //thuyntt 10:44 AM 19/05/2014
        //1234 => 1,234.00
        public static string FormatNumber8(object obj)
        {
            if (obj == null) return "0";
            return string.Format(CConfig.PATTERN_FORMAT_NUMBER8, Convert.ToDecimal(obj));
        }

        //thuyntt 10:44 AM 19/05/2014
        //1234 => 1,234.0
        public static string FormatNumber9(object obj)
        {
            if (obj == null) return "0";
            else if (Convert.ToDouble(obj) < 10.0) return String.Format(CultureInfo.InvariantCulture, "0:0.0", obj);
            else return String.Format(CultureInfo.InvariantCulture, "0:0,0.0", obj);
        }

        /// <summary>
        /// dangth
        /// Convert array char to string 
        /// </summary>
        public static string Array2String(char[] data)
        {
            string tmp = new string(data);
            return tmp;
        }

        /// <summary>
        /// Convert int to time
        /// </summary>
        /// <param name="data">91500</param>
        /// <returns>9:15:00</returns>
        public static string ConvertIntToTime(int data)
        {
            string temp = data.ToString();
            if (temp.Length == 6)
                return temp.Substring(0, 2) + ":" + temp.Substring(2, 2) + ":" + temp.Substring(4, 2);
            else if (temp.Length == 5)
                return temp.Substring(0, 1) + ":" + temp.Substring(1, 2) + ":" + temp.Substring(3, 2);
            else
                return "";
        }
        /// <summary>
        /// Convert time to int
        /// </summary>
        /// <param name="data">9:15:00</param>
        /// <returns>91500</returns>
        public static int ConvertTimeToInt(string data)
        {
            if (data == null || data == "")
                return 0;
            else
                return Convert.ToInt32(data.Replace(":", ""));
        }

        ///// <summary>
        ///// Send Msg to monitor
        ///// </summary>
        ///// <param name="AppName"></param>
        ///// <param name="Mgs"></param>
        //public static void Monitor_SendMsg(string AppName, string Msg)
        //{
        //    try
        //    {
        //        var pairs = new List<KeyValuePair<string, string>>
        //    {
        //        new KeyValuePair<string, string>("type", "msg"),
        //        new KeyValuePair<string, string>("msg", Msg),
        //    };
        //        Monitor_Send(AppName, pairs);
        //    }
        //    catch (Exception ex)
        //    {
        //        CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
        //    }
        //}
        ///// <summary>
        ///// Send Progress to monitor
        ///// </summary>
        ///// <param name="AppName"></param>
        ///// <param name="Current"></param>
        ///// <param name="Max"></param>
        //public static void Monitor_SendProgress(string AppName, string Current, string Max)
        //{
        //    try
        //    {
        //        var pairs = new List<KeyValuePair<string, string>>
        //    {
        //        new KeyValuePair<string, string>("type", "progress"),
        //        new KeyValuePair<string, string>("current", Current),
        //        new KeyValuePair<string, string>("max", Max),
        //    };
        //        Monitor_Send(AppName, pairs);
        //    }
        //    catch (Exception ex)
        //    {
        //        CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
        //    }
        //}
        ///// <summary>
        ///// Send Status to monitor
        ///// </summary>
        ///// <param name="AppName"></param>
        ///// <param name="Current"></param>
        ///// <param name="Max"></param>
        ///// <param name="Msg"></param>
        ///// <param name="Note"></param>
        //public static void Monitor_SendStatus(string AppName, string Current, string Max, string Msg, string Note)
        //{
        //    try
        //    {
        //        List<KeyValuePair<string, string>> pairs = new List<KeyValuePair<string, string>>
        //    {
        //        new KeyValuePair<string, string>("type", "status"),
        //        new KeyValuePair<string, string>("current", Current),
        //        new KeyValuePair<string, string>("max", Max),
        //        new KeyValuePair<string, string>("msg", Msg),
        //        new KeyValuePair<string, string>("note", Note),
        //    };
        //        Monitor_Send(AppName, pairs);
        //    }
        //    catch (Exception ex)
        //    {
        //        CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
        //    }
        //}

        //private static void Monitor_Send(string AppName, List<KeyValuePair<string, string>> pairs)
        //{
        //    try
        //    {
        //        HttpClient client = new HttpClient();

        //        FormUrlEncodedContent content = new FormUrlEncodedContent(pairs);

        //        var response = client.PostAsync(CConfig.MONITOR_URL.Replace("<!AppName>", AppName), content).Result;

        //        if (response.IsSuccessStatusCode)
        //        {

        //        }
        //        response.Dispose();
        //    }
        //    catch (Exception ex)
        //    {
        //        CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
        //    }
        //}

        //public static void Send_Test(string AppName, string Current, string Max, string Msg, string Note)
        //{
        //    using (WebClient client = new WebClient())
        //    {
        //        System.Collections.Specialized.NameValueCollection reqparm = new System.Collections.Specialized.NameValueCollection();
        //        reqparm.Add("type", "status");
        //        reqparm.Add("current", Current);
        //        reqparm.Add("max", Max);
        //        reqparm.Add("msg", Msg);
        //        reqparm.Add("note", Note);
        //        byte[] responsebytes = client.UploadValues(CConfig.MONITOR_URL.Replace("<!AppName>", AppName), "POST", reqparm);
        //        string responsebody = Encoding.UTF8.GetString(responsebytes);
        //    }
        //}

        /// <summary>
        /// 2015-05-04 11:11:26 ngocta2
        /// convert char array => string
        /// </summary>
        /// <param name="objValue"></param>
        /// <returns></returns>
        public static string Char2String(object objValue)
        {
            try
            {
                if (objValue == null) return "null";

                string strValue = "";
                if (objValue.GetType().ToString() ==  "System.Char[]")
                {
                    char[] arrchrData = (char[])objValue;
                    string strTemp = new string(arrchrData);
                    strValue = strTemp.Trim();
                }
                else
                    strValue = objValue.ToString();

                return strValue;
            }
            catch (Exception ex)
            {
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
                return null;
            }
        }


        public static void ResizeArray<T>(ref T[] arr)
        {
            try
            {
                if (arr == null)
                    Array.Resize(ref arr, 1); // tang size array  
                else
                    Array.Resize(ref arr, arr.Length + 1); // tang size array       
            }
            catch (Exception ex)
            {
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            }
        }

        // 3.1 => true
        // 3.0 => false
        public static bool IsFloatingPointNumber(decimal d)
        {
            return !((d % 1) == 0);
        }

        /// <summary>
        /// CBase.Right("QuoteFeederHOSELib.BLL.CQuoteFeederHOSELib->Thread_PRS_SECURITY","QuoteFeederHOSELib.BLL.CQuoteFeederHOSELib->Thread_PRS_SECURITY".Length -42-2)
        ///"Thread_PRS_SECURITY"
        ///"QuoteFeederHOSELib.BLL.CQuoteFeederHOSELib->Thread_PRS_SECURITY".IndexOf ("->")
        ///42
        ///CBase.GetCaller(1)
        ///"QuoteFeederHOSELib.BLL.CQuoteFeederHOSELib->Thread_PRS_SECURITY"
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentFunctionName(string strDeepCaller)
        {
            try
            {
                //return System.Reflection.MethodBase.GetCurrentMethod().Name;

                //StackTrace st = new StackTrace();
                //StackFrame sf = st.GetFrame(0);
                //MethodBase currentMethodName = sf.GetMethod();
                //string ssss = currentMethodName.ToString();

                strDeepCaller = CBase.Right(strDeepCaller, strDeepCaller.Length - strDeepCaller.IndexOf("->") - 2);
                return strDeepCaller;
            }
            catch (Exception ex)
            {
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
                return "";
            }
        }

        /// <summary>
        /// tool HNX msg queue
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary>
        /// tool HNX msg queue
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }


        //http://stackoverflow.com/questions/19549312/how-to-compress-json-responses
        public static string lzw_compress(string s)
        {
            var dict = new Dictionary<string, int>();
            char[] data = s.ToCharArray();
            var output = new List<char>();
            char currChar;
            string phrase = data[0].ToString();
            int code = 256;

            for (var i = 1; i < data.Length; i++)
            {
                currChar = data[i];
                var temp = phrase + currChar;
                if (dict.ContainsKey(temp))
                    phrase += currChar;
                else
                {
                    if (phrase.Length > 1)
                        output.Add((char)dict[phrase]);
                    else
                        output.Add((char)phrase[0]);
                    dict[phrase + currChar] = code;
                    code++;
                    phrase = currChar.ToString();
                }
            }

            if (phrase.Length > 1)
                output.Add((char)dict[phrase]);
            else
                output.Add((char)phrase[0]);

            return new string(output.ToArray());
        }

        /*
         * Created by: LamMN
         * Created date: 18/03/2015
         * Desc: Sinh mat khau tu dong
         * iPassLen = 8
         * radom 3 chuỗi sẽ không bao giờ bị dính lỗi toàn số hoặc toàn chữ 
         */
        public static string GeneratePassword(int iPassLen)
        {
            //string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@#$%*";
            string charnums = "OPQRSTUVWXYZ0123456789ABCDEFGHIJKLMN";
            string chars = "OPQRSTUVWXYZABCDEFGHIJKLMN";
            string nums = "01234567899876543210";
            Random random = new Random();
            string result = new string(
                                 Enumerable.Repeat(chars, iPassLen - 6)
                                         .Select(s => s[random.Next(s.Length)])
                                         .ToArray());
            result += new string(
                             Enumerable.Repeat(charnums, iPassLen - 4)
                                     .Select(s => s[random.Next(s.Length)])
                                     .ToArray());
            result += new string(
                             Enumerable.Repeat(nums, iPassLen - 6)
                                     .Select(s => s[random.Next(s.Length)])
                                     .ToArray());
            return result;
        }
        public static string convertDataTableToStringJSON(DataTable dt)
        {
            System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return serializer.Serialize(rows);
        }
    }

    //==================================================================================================================================================================================================================================================

    public static class CSBase
    {
        /// <summary>
        /// 
        /// http://stackoverflow.com/questions/1415140/can-my-enums-have-friendly-names
        /// http://stackoverflow.com/questions/6096299/extension-methods-must-be-defined-in-a-non-generic-static-class
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Enum2String(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr =
                           Attribute.GetCustomAttribute(field,
                             typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                    else
                    {
                        return field.Name;
                    }
                }
            }
            return null;
        }
        
    }
}
