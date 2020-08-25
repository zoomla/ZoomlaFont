using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FontZ01.Commons
{
    public class function
    {
        public static string MapPath(string vpath)
        {
            string ppath = AppDomain.CurrentDomain.BaseDirectory + vpath;
            ppath = ppath.Replace("/", "\\");
            return ppath;
        }
        public static string VToP(string vpath) 
        {
            return MapPath(vpath);
        }
        public static void WriteErrMsg(string msg) 
        {
            MessageBox.Show(msg);
        }
    }
}
