using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ZLCommon.MB;

namespace IISControl
{
    public class IISHelper
    {
        public static DataTable fzuserDT = new DataTable();
        public static ServerManager iis = new ServerManager();
        private static B_CodeModel logBll=new B_CodeModel("ZL_FZ_Log");
        public static ObjectState StopSite(string siteName)
        {
            return iis.Sites[siteName].Stop();
        }
        public static ObjectState StartSite(string siteName)
        {
            return iis.Sites[siteName].Start();
        }
        public static ObjectState RestartSite(string siteName)
        {
            iis.Sites[siteName].Stop();
            return iis.Sites[siteName].Start();
        }
        public static void WriteLog(string msg, string remind = "")
        {
            DataRow dr = logBll.NewModel();
            dr["CDate"] = DateTime.Now;
            dr["ExMessage"] = msg;
            dr["Remind"] = remind;
            logBll.Insert(dr);
        }
    }
}
