using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace Oze.AppCode.BLL
{
    public class MD5
    {
        public static byte[] encryptData(string data)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5Hasher = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashedBytes;
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            hashedBytes = md5Hasher.ComputeHash(encoder.GetBytes(data));
            return hashedBytes;
        }
        public static string md5(string data)
        {
            return BitConverter.ToString(encryptData(data)).Replace("-", "").ToLower();
        }
        public static string GetRand()
        {
            Random ran = new Random();
            string sRan = ran.Next(0, 9).ToString() + ran.Next(0, 9).ToString() + ran.Next(0, 9).ToString() + ran.Next(0, 9).ToString() + ran.Next(0, 9).ToString() + ran.Next(0, 9).ToString();
            //Session["#randvm"] = sRan;
            return sRan;
        }
        private static string keyValue = "film-gdata";
        public static string EncPwd(string password)
        {
            string encryptPassword = password + keyValue;
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(encryptPassword);
            HashAlgorithm hash = new MD5CryptoServiceProvider();
            byte[] hashBytes = hash.ComputeHash(passwordBytes);
            encryptPassword = Convert.ToBase64String(passwordBytes);
            return encryptPassword;
        }
        public static string GetRandomForToNumber(int codeCount)
        {
            string allChar = "0,1,2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z";
            string[] allCharArray = allChar.Split(',');
            string randomCode = "";
            int temp = -1;
            Random rand = new Random();
            for (int i = 0; i < codeCount; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * ((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(36);
                if (temp != -1 && temp == t)
                {
                    return GetRandomForToNumber(codeCount);
                }
                temp = t;
                randomCode += allCharArray[t];
            }
            return randomCode;
        }
        internal System.Collections.Generic.IEnumerable<byte> ComputeHash(byte[] originalBytes)
        {
            throw new NotImplementedException();
        }
    }

}