using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using ZLCommon.Helper;
using ZLCommon.MB;
/*
 * 1,启动时初始化计时器,每分钟检测一次是否到期,到期则停止站点(每分钟重取一次数据)
 * 
 */ 
namespace IISControl
{
    public partial class IISServices : ServiceBase
    {
        public static B_CodeModel logBll = new B_CodeModel("ZL_FZ_Log");
        private Timer _timer = null;
        public IISServices()
        {
            InitializeComponent();
            _timer = new Timer();
            _timer.Interval = 1 * 60 * 1000;
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
            _timer.Start();
        }
        protected override void OnStart(string[] args)
        {
            //using (System.IO.StreamWriter sw = new System.IO.StreamWriter("C:\\log.txt", true))
            //{
            //    try {
            //        string names = "";
            //        sw.WriteLine(names);
            //        foreach (var site in IISHelper.iis.Sites)
            //        {
            //            names += site.Name + "|";
            //        }
            //        IISHelper.StopSite("italybees.com意蜂实业");
            //    }
            //    catch (Exception ex) { sw.WriteLine("出错:" + ex.Message); }
            //}

        }
        protected override void OnStop()
        {

        }
        protected void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            string siteName = "";
            try
            {
                foreach (DataRow dr in IISHelper.fzuserDT.Rows)
                {
                    siteName = DataConvert.CStr(dr["SiteInfo"]);
                    string date = DataConvert.CStr(dr["EndDate"]);
                    DateTime end = DateTime.Now;
                    if (!string.IsNullOrEmpty(siteName) && DateTime.TryParse(date, out end))
                    {
                        if (end < DateTime.Now && IISHelper.iis.Sites[siteName].State == ObjectState.Started)
                        {
                            IISHelper.StopSite(siteName);
                            IISHelper.WriteLog("已禁用" + siteName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                IISHelper.WriteLog(ex.Message,siteName);
            }
        }
    }
}
