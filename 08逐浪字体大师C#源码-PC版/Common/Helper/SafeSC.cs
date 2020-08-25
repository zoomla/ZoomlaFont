using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public class SafeSC
    {
        public static void CheckIDSEx(string ids)
        {
            if (!CheckIDS(ids))
                throw new Exception("传入的" + ids + "不合法,请参照格式1,2,3");
        }
        /// <summary>
        /// false,未通过
        /// </summary>
        public static bool CheckIDS(string ids)
        {
            if (string.IsNullOrEmpty(ids)) { return false; }
            string[] strArray = ids.Split(new char[1]
      {
        ','
      }, StringSplitOptions.RemoveEmptyEntries);
            bool flag = true;
            int result = 0;
            for (int index = 0; index < strArray.Length; ++index)
            {
                if (!int.TryParse(strArray[index], out result))
                {
                    flag = false;
                    break;
                }
            }
            return flag;
        }
    }
}
