using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;

namespace FontZ01.Commons
{
    public class ServicesHelper
    {
        /// <summary>
        /// 停止服务
        /// </summary>
        public static bool StopService(string ServiceName)
        {
            ServiceController service = new ServiceController(ServiceName);
            try
            {
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped);
                return true;
            }
            catch (Exception ex)
            {
                function.WriteErrMsg(ServiceName + "操作失败,原因:" + ex.Message);
                return false;
            }

        }
        /// <summary>
        /// 开启服务
        /// </summary>
        public static bool StartService(string ServiceName)
        {
            ServiceController service = new ServiceController(ServiceName);
            try
            {
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running);
                return true;
            }
            catch (Exception ex)
            {
                function.WriteErrMsg(ServiceName + "操作失败,原因:" + ex.Message);
                return false;
            }

        }
        public static bool RestartService(string ServiceName)
        {
            try
            {
                StopService(ServiceName);
                StartService(ServiceName);
                return true;
            }
            catch (Exception ex) { function.WriteErrMsg(ServiceName + "操作失败,原因:" + ex.Message); return false; }
        }
    }
}
