using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Common.Helper
{
    public static class StringHelper
    {
        public static string Base64StringDecode(string input)
        {
            byte[] bytes = Convert.FromBase64String(input);
            return Encoding.UTF8.GetString(bytes);
        }
        public static string Base64StringEncode(string input)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        }
        public static string MD5(string input)
        {
            using (MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider())
            {
                return BitConverter.ToString(provider.ComputeHash(Encoding.UTF8.GetBytes(input))).Replace("-", "").ToLower();
            }
        }
        public static string SubStr(object obj, int len = 60, string sub = "...")
        {
            string str = obj.ToString();
            if (string.IsNullOrEmpty(str) || str.Length < len)
            {
                return str;
            }
            return (str.Substring(0, len) + sub);
        }
    }
}
