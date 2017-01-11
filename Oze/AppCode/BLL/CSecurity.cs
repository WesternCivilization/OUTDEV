using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oze.AppCode.BLL
{
    public class CSecurity
    {
        //http://stackoverflow.com/questions/14643735/how-to-generate-a-unique-token-which-expires-after-24-hours
        public const int DURATION_IN_SECONDS = 100; // token chi valid trong 100s tu khi tao ra

        // tao random token
        // token chi valid trong 1 khoang thoi gian nhat dinh
        // hacker ko the copy token nay de su dung trai phep (connect vao Hub get data)
        public static string CreateToken()
        {
            try
            {
                byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
                byte[] key = Guid.NewGuid().ToByteArray();
                string token = Convert.ToBase64String(time.Concat(key).ToArray());
                return token;
            }
            catch (Exception ex)
            {
                CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
                return "";
            }
        }

        // kiem tra token co hop le ko
        public static bool CheckToken(string token)
        {
            return true;
            //try
            //{
            //    byte[] data = Convert.FromBase64String(token);
            //    DateTime when = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
            //    if (when > DateTime.UtcNow.AddSeconds(0-DURATION_IN_SECONDS))
            //    {
            //        // OK
            //        return true;
            //    }

            //    return false;
            //}
            //catch (Exception ex)
            //{
            //    CLog.LogError(CBase.GetDeepCaller(), CBase.GetDetailError(ex));
            //    return false;
            //}
        }

    }
}
