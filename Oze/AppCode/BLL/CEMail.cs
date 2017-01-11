using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oze.AppCode.BLL
{
    public class CEMail
    {
        const string SMTP_SERVER = "10.26.248.20";                        //
        const string SMTP_PORT = "25";                                    //
        const string SMTP_USER = "fptsecurities";                         // "monitor";               
        const string SMTP_PASS = "fpts1307";                              // "fpts@123";              
        const string SMTP_FROM = "Stock5G<fptsecurities@fpts.com.vn>";    // "monitor@fpts.com.vn";
        const char CHAR_SPLITOR = ',';
        static int m_intMinuteCountBegin = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
        static bool m_blnFirst = true;

        /// <summary>
        /// 2016-03-02 14:28:28 ngocta2
        /// send email cho nhieu nguoi , voi chu ky 
        /// VD: interval=3 >>> co the call function nay lien tuc nhung that su chi co 3 phut moi send mail 1 lan
        /// </summary>
        /// <param name="strListTo">ngocta2@fps.com.vn,hantv@fps.com.vn,thuyntt@fps.com.vn,lammn@fps.com.vn</param>
        /// <param name="strSubject">string subject</param>
        /// <param name="strBody">string body</param>
        /// <param name="intIntervalInMinute">3</param>
        /// <returns></returns>
        public static bool SendEmails(string strListTo, string strSubject, string strBody, int intIntervalInMinute)
        {
            try
            {
                int intMinuteCountNow = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
                if (intMinuteCountNow - m_intMinuteCountBegin >= intIntervalInMinute || m_blnFirst)
                {
                    m_blnFirst = false;
                    m_intMinuteCountBegin = intMinuteCountNow;
                    CEMail.SendEmails(strListTo, strSubject, strBody);
                    
                }
                return true;
            }
            catch (Exception ex)
            {
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strListTo"></param>
        /// <param name="strSubject"></param>
        /// <param name="strBody"></param>
        /// <returns></returns>
        public static bool SendEmails(string strListTo, string strSubject, string strBody)
        {
            try
            {
                string[] arrTo = strListTo.Split(CHAR_SPLITOR);
                foreach (string strTo in arrTo)
                {
                    SendEmail(strTo, strSubject, strBody);
                }
                return true;
            }
            catch (Exception ex)
            {
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
                return false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="strTo"></param>
        /// <param name="strSubject"></param>
        /// <param name="strBody"></param>
        /// <returns></returns>
        public static bool SendEmail(string strTo, string strSubject, string strBody)
        {
            try
            {
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                System.Net.Mail.SmtpClient VSmtpMail = new System.Net.Mail.SmtpClient();
                mail.From = new System.Net.Mail.MailAddress(SMTP_FROM);
                mail.To.Add(new System.Net.Mail.MailAddress(strTo));
                mail.BodyEncoding = System.Text.Encoding.UTF8;
                mail.Subject = strSubject;
                mail.Body = strBody;
                mail.IsBodyHtml = true;
                VSmtpMail = new System.Net.Mail.SmtpClient();
                VSmtpMail.Host = SMTP_SERVER;
                VSmtpMail.Port = Convert.ToInt32(SMTP_PORT);
                VSmtpMail.Credentials = new System.Net.NetworkCredential(SMTP_USER, SMTP_PASS);
                VSmtpMail.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
                return false;
            }
        }
    }
}
