using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;

namespace Common.Helper
{
    public class ServiceHelper
    {
        public static bool IsExist(string serviceName)
        {
            ServiceController[] serviceList = ServiceController.GetServices();
            foreach (ServiceController service in serviceList)
            {
                if (service.ServiceName == serviceName)
                    return true;
            }
            return false;
        }

    }
}
