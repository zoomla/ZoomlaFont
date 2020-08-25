using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;


namespace Common
{
    //下载图片自动命名的方法
    public class Commons
    {
        public static string NewImgName(string filename)
        {
            string str = string.Empty;
            string timeStr = DateTime.Now.ToString("yyyyMMddHHmmss", DateTimeFormatInfo.InvariantInfo);
            //if (string.IsNullOrEmpty(filename))
            //{
            //    str = timeStr+".jpg";
            //}
            //else
            //{
            int index = filename.LastIndexOf(".");
            str = filename.Substring(0, index) + "_" + timeStr + filename.Substring(index);
            //}
            return str;
        }
    }
}
