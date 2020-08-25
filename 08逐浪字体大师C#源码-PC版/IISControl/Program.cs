using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using ZLCommon.DB;
using ZLCommon.Helper;
//必须放置在ZLManager的根目录执行(需要读其配置文件)
namespace IISControl
{
    static class Program
    {
        static void Main()
        {
            ConfigHelper.LoadXML();
            SqlHelper.ConnectionString = ConfigHelper.APPInfo.connstr;
            IISHelper.fzuserDT = SqlHelper.ExecuteTable("SELECT * FROM ZL_FZ_User");
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new IISServices() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
