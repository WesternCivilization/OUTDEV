using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;

namespace Oze.AppCode.BLL
{
    public class Validate
    {
        /// <summary>
        /// 2016-11-11 15:08:18 NamLD
        /// validate chuỗi
        /// </summary>
        /// <param name="str">chuỗi truyền vào</param>
        /// <param name="maxLength">độ dài tối đa của chuỗi</param>
        /// <param name="minLength">độ dài tối thiểu của chuỗi</param>
        /// <param name="columnName">tên cột</param>
        /// <param name="isNull">có được để trống?</param>
        /// <param name="error">danh sach loi</param>
        /// <returns></returns>
        public static string validStr(string str, int maxLength, int minLength, string columnName, bool isNull, List<string> error) {
            string result = "";
            try
            {
                if (str.Trim() == "" && !isNull)
                {
                    result = "Không để trống " + columnName;
                }
                else if ((str.Trim() != "" && str.Trim().Length > maxLength) || (str.Trim() != "" && str.Trim().Length < minLength))
                {
                    result = "Độ dài " + columnName + ": " + minLength + " - " + maxLength + " ký tự";
                    error.Add(result);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
            return result;
        }
        /// <summary>
        /// 2016-11-11 15:52:53 NamLD 
        /// validate chuỗi số
        /// </summary>
        /// <param name="str">chuỗi truyền vào</param>
        /// <param name="maxLength">độ dài tối đa của chuỗi</param>
        /// <param name="minLength">độ dài tối thiểu của chuỗi</param>
        /// <param name="columnName">tên cột</param>
        /// <param name="isNull">có được để trống?</param>
        /// <param name="error">danh sach loi</param>
        /// <returns></returns>
        public static string validStrNumber(string str, int maxLength, int minLength, string columnName, bool isNull, List<string> error)
        {
            string result = "";
            
            try
            {
                Regex regex = new Regex("^[0-9]{" + minLength + "," + maxLength + "}$");
                if (str.Trim() == "" && !isNull)
                {
                    result = "Không để trống " + columnName;
                }
                else if (str.Trim() != "")
                {
                    Match match = regex.Match(str);
                    if (!match.Success)
                    {
                        if (minLength == maxLength)
                        {
                            result = columnName + " là chuỗi số có độ dài: " + minLength + " ký tự";
                        }
                        else
                        {
                            result = columnName + " là chuỗi số có độ dài: " + minLength + " - " + maxLength + " ký tự";
                        }
                    }
                }
                error.Add(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            return result;
        }
        /// <summary>
        /// 2016-11-11 16:00:04 NamLD
        /// kiem tra email
        /// </summary>
        /// <param name="str">chuỗi truyền vào</param>
        /// <param name="columnName">tên cột</param>
        /// <param name="isNull">có được để trống?</param>
        /// <param name="error">danh sach loi</param>
        /// <returns></returns>
        public static string validEmail(string str, string columnName, bool isNull, List<string> error)
        {
            string result = "";
            try
            {
                try
                {
                    if (str.Trim() == "" && !isNull)
                    {
                        result = "Không để trống " + columnName;
                    }
                    else if (str.Trim() != "")
                    {
                        MailAddress m = new MailAddress(str);
                    }
                }
                catch (Exception)
                {
                    result = "Lỗi định dạng " + columnName;
                }
                error.Add(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            return result;
        }
        /// <summary>
        /// 2016-11-11 16:32:08 NamLD
        /// validate DateTime
        /// </summary>
        /// <param name="strDatetime"></param>
        /// <param name="columnName"></param>
        /// <param name="isNull"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static string validDatetime(string str, string columnName, bool isNull, List<string> error) {
            string result = "";
            try
            {
                if (str.Trim() == "" && !isNull)
                {
                    result = "Không để trống " + columnName;
                }
                else if (str.Trim() != "")
                {
                    DateTime temp;
                    if (!DateTime.TryParse(str, out temp))
                    {
                        result = "Lỗi định dạng " + columnName;
                    }
                }
                error.Add(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
    }
}