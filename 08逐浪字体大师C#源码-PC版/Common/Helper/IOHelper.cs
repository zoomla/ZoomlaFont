using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Common.Helper
{
    public class IOHelper
    {
        public static void WriteFile(string ppath, string content)
        {
            string dir=Path.GetDirectoryName(ppath);
            if (!Directory.Exists(dir)) {Directory.CreateDirectory(dir); }
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            File.WriteAllBytes(ppath, bytes);
        }
        public static string MapPath(string vpath)
        {
            string ppath = AppDomain.CurrentDomain.BaseDirectory + vpath;
            ppath = ppath.Replace("/", "\\");
            return ppath;
        }
    }
}
