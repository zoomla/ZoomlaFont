using CefSharp;
using CefSharp.Wpf;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using System.Xml;
using FontZ01.BLL;
using FontZ01.Commons;
using FontZ01.Model.RDP;
using ZoomLa.SQLDAL;
using FontZ01.BrowserHandler;
using Common.Helper;
using Common.MB;

namespace FontZ01
{
    //JS的方法首字母需要小写,Lower Case Function Calls in JavaScript Land
    public partial class MainWindow : WindowBase
    {
        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        CefSharp.Wpf.ChromiumWebBrowser webView = null;
        public CefSettings setting = null;
        public MainWindow(CefSettings settings)
        {
            //try
            //{
            InitializeComponent();
            setting = settings;
            this.Loaded += (sender, e) =>
            {
                    //this.YesButton.Content = "确 定";
                    //this.YesButton.Width = 60;
                    //this.NoButton.Content = "取 消";
                    //this.NoButton.Width = 60;
                    //this.YesButton.Click += (ss, ee) =>{MessageBox.Show("确定");};
                    //this.NoButton.Click += (ss, ee) =>{MessageBox.Show("取消");};
                    //this.YesButton.Visibility = Visibility.Hidden;
                    //this.NoButton.Visibility = Visibility.Hidden;
                };
            //-------------------------------------------初始化
            string startUrl = "loading.html?url=app.html#/";
            //startUrl = "app.html#/welcome";

            ConfigHelper.LoadXML();
            DBCenter.DB = ZoomLa.SQLDAL.SQL.SqlBase.CreateHelper("mssql");
            if (!string.IsNullOrEmpty(ConfigHelper.APPInfo.connstr))
            {
                ZoomLa.SQLDAL.DBCenter.DB.ConnectionString = ConfigHelper.APPInfo.connstr;
                Common.DB.SqlHelper.ConnectionString = ConfigHelper.APPInfo.connstr;
            }
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show("检测到配置错误,请先完成配置,原因:" + ex.Message);
                //startUrl = "wait.html?url=app.html#/config";
            }
            try { FZHelper.LoadXml(); } catch { }
            //-------------------------------------------配置Chrome
            //var setting = new CefSettings() { LogSeverity = LogSeverity.Disable, IgnoreCertificateErrors = true };
            //CefSharp.Cef.Initialize(setting);
            webView = new CefSharp.Wpf.ChromiumWebBrowser();
            webView.BrowserSettings.WebSecurity = CefState.Disabled;//允许跨域,否则无法加载file:\\\
                                                                    //this.browserContainer.Content = webView;
                                                                    //this.Content = webView;
            this.Content = webView;
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;//对于旧式绑定需要设置这个值才能迁移
                                                                   //webView.RegisterJsObject("wpf", new JavaScriptInteractionObj(webView));

            // webView.Dock = DockStyle.Fill;
            webView.DownloadHandler = new DownloadHandler();
            webView.BrowserSettings.DefaultEncoding = "utf-8"; //解决乱码

            webView.MenuHandler = new ContextMenuHandler();  //浏览器右键菜单事件处理
            webView.KeyboardHandler = new KeyboardHandler(); //键盘消息事件处理
            webView.LifeSpanHandler = new LifeSpanHandler(); //弹出窗口处理事件

            CefSharpSettings.WcfEnabled = true;
            webView.JavascriptObjectRepository.Register("wpf", new JavaScriptInteractionObj(webView), isAsync: true, options: BindingOptions.DefaultBinder);

            //js通过chrome对象调用C#函数-实现智图专用的自动存储功能
            webView.JavascriptObjectRepository.Register("chrome", new ScriptCallbackMgr(webView), true, new BindingOptions() { CamelCaseJavascriptNames = false });

            //webView.Address = "http://www.z01.com";//几种加载方式
            //webView.Address = @"file:///" + basePath + "/assest/html/" + startUrl;
            webView.Address = @"file:///" + basePath + "/assest/" + startUrl;
            webView.Loaded += WebView_Loaded;
            //webView.LoadingStateChanged += wb_OnLoadingStateChange;
            //-----当加载链接错误时调用
            webView.LoadError += (sender, args) =>
            {
                    // Don't display an error for downloaded files.
                    if (args.ErrorCode == CefErrorCode.Aborted) { return; }
                var errorBody = string.Format("<html><body bgcolor=\"white\"><h2>Failed to load URL {0} with error {1} ({2}).</h2></body></html>",
                                              args.FailedUrl, args.ErrorText, args.ErrorCode);
                    //args.Frame.LoadStringForUrl(errorBody, args.FailedUrl);
                };
            //}
            //catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        //-----------------------------------
        //该事件早于Jquery等加载,所以不可在其中写JQuery事件,或需要延时执行
        private void WebView_Loaded(object sender, RoutedEventArgs e)
        {
            //var script = "alert('sadfasdf');";//$('body').css('background-color','red');
            //webView.ExecuteScriptAsync(script);
            //ExecuteJSAndReturnData_Btn_Click(null, null);
            //webView.ShowDevTools();//[debug]
        }
        //void wb_OnLoadingStateChange(object sender, LoadingStateChangedEventArgs e)
        //{
        //    if (e.IsLoading == false)//加载完成时为false,可能会执行多次
        //    {

        //    }
        //}
        //-----------------------------------
        private void CloseCef_Btn_Click(object sender, RoutedEventArgs e)
        {
            Cef.Shutdown();
        }
        private void DevTool_Btn_Click(object sender, RoutedEventArgs e)
        {
            webView.ShowDevTools();
        }
        private void ExecuteJS_Btn_Click(object sender, RoutedEventArgs e)
        {
            var script = "document.body.style.backgroundColor='red';";
            webView.ExecuteScriptAsync(script);
        }
        private void ExecuteJSAndReturnData_Btn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("function tempFunction() {");
            sb.AppendLine("     var w = window.innerWidth;");
            sb.AppendLine("     var h = window.innerHeight;");
            sb.AppendLine("");
            sb.AppendLine("     return w*h;");
            sb.AppendLine("}");
            sb.AppendLine("tempFunction();");
            // function EvaluateScriptAsync() only returns simple data types (ints, bools, string). 
            //if you need to return a complex object, you need to convert it to JSON first and send the object back as a string.
            var task = webView.EvaluateScriptAsync(sb.ToString());
            task.ContinueWith(t =>
            {
                if (!t.IsFaulted)
                {
                    var response = t.Result;
                    if (response.Success == true)
                    {
                        MessageBox.Show(response.Result.ToString());
                    }
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
    public class JavaScriptInteractionObj
    {
        B_FZ_Server serverBll = new B_FZ_Server();
        CefSharp.Wpf.ChromiumWebBrowser webView = null;
        public JavaScriptInteractionObj(ChromiumWebBrowser view)
        {

            webView = view;
        }
        public string ErrorFunction()
        {
            return null;
        }
        //-----------------------------------sys操作
        public void sys_close()
        {

        }
        public void sys_openurl(string url)
        {
            System.Diagnostics.Process.Start(url);
        }
        public void sys_opendev()
        {
            webView.ShowDevTools();
        }
        public void sys_outToExcel(string html, string name)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "文本文件|*.txt";
            dialog.FilterIndex = 0;
            dialog.FileName = "用户信息.xls";
            if (dialog.ShowDialog() == true)
            {
                IOHelper.WriteFile(dialog.FileName, html);
                MessageBox.Show("保存成功");
            }

        }
        private void ProcCmdWithExit(string cmd)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.StandardInput.WriteLine(cmd);
            p.StandardInput.WriteLine("exit");
        }
        //-----------------------------------API
        #region FZ用户
        public string user_list(bool reload)
        {
            if (reload) { FZHelper.LoadXml(); }
            return JsonConvert.SerializeObject(FZHelper.UserDT);
        }
        public string user_get(string name)
        {
            FZHelper.UserDT.DefaultView.RowFilter = "Name ='" + name.Trim() + "'";
            DataTable udt = FZHelper.UserDT.DefaultView.ToTable();
            if (udt.Rows.Count < 1) { function.WriteErrMsg("用户[" + name + "]不存在"); }
            string json = JsonConvert.SerializeObject(udt);
            FZHelper.UserDT.DefaultView.RowFilter = "";
            return json;
        }
        public bool user_update(string json)
        {
            DataTable users = JsonConvert.DeserializeObject<DataTable>(json);
            DataRow user = users.Rows[0];
            user["Pass"] = StringHelper.MD5(user["UserPwd"].ToString());
            FZHelper.User_Update(user);
            return true;
        }
        public bool user_add(string json)
        {
            DataTable users = JsonConvert.DeserializeObject<DataTable>(json);
            users.Columns.Add(new DataColumn("Pass", typeof(string)));
            users.Rows[0]["Pass"] = StringHelper.MD5(users.Rows[0]["UserPwd"].ToString());
            FZHelper.User_Add(users);
            return true;
        }
        public bool user_del(string name) { return FZHelper.User_Del(name); }
        public bool user_change(string name, string status)
        {
            XmlNode user = FZHelper.User_GetNode(name);
            user.SelectSingleNode("Option[@Name='Enabled']").InnerText = status;
            FZHelper.xmldoc.Save(FZHelper.xmlPath);
            return true;
        }
        #endregion
        #region server
        public string server_list()
        {
            return JsonConvert.SerializeObject(serverBll.Sel());
        }
        public string server_get(int id)
        {
            M_FZ_Server serverMod = new M_FZ_Server();
            if (id > 0)
            {
                serverMod = serverBll.SelReturnModel(id);
            }
            return JsonConvert.SerializeObject(serverMod);
        }
        public bool server_add(string modelStr)
        {
            M_FZ_Server model = JsonConvert.DeserializeObject<M_FZ_Server>(modelStr);
            if (model.id > 0)
            {
                serverBll.UpdateByID(model);
            }
            else
            {
                serverBll.Insert(model);
            }
            return true;
        }
        public bool server_del(int id) { serverBll.Del(id.ToString()); return true; }
        public void server_connect(int id)
        {
            M_FZ_Server serverMod = serverBll.SelReturnModel(id);
            string templateStr = Properties.Resources.TemplateRDP;
            //用DataProtection加密密码,并转化成二进制字符串
            var upwdStr = BitConverter.ToString(DataProtection.ProtectData(Encoding.Unicode.GetBytes(serverMod.upwd), ""));
            upwdStr = upwdStr.Replace("-", "");
            string rdpFileName = serverMod.address.Replace(":", "_");
            //替换模板里面的关键字符串,生成当前的drp字符串
            var NewStr = templateStr.Replace("{#address}", serverMod.address).Replace("{#username}", serverMod.uname).Replace("{#password}", upwdStr);
            //将drp保存到文件，并放在程序目录下，等待使用
            StreamWriter sw = new StreamWriter(rdpFileName);
            sw.Write(NewStr);
            sw.Close();
            //利用CMD命令调用MSTSC
            ProcCmdWithExit("mstsc " + rdpFileName);
        }
        #endregion
        #region sites

        public string sites_get()
        {
            JObject obj = new JObject();
            obj.Add("name", ConfigHelper.APPInfo.sitesServiceName);
            if (ServiceHelper.IsExist(ConfigHelper.APPInfo.sitesServiceName))
            {
                obj.Add("status", "已安装");
            }
            else
            {
                obj.Add("status", "未安装");
            }
            return JsonConvert.SerializeObject(obj);
        }
        public string sites_install()
        {
            //启动命令行并开始执行
            //C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe D:\CMSService\CMSService.exe /u
            //net start CMSService
            M_APIResult ret = new M_APIResult();
            try
            {
                string servicePath = function.VToP("/Config/Install/");
                if (ServiceHelper.IsExist(ConfigHelper.APPInfo.sitesServiceName)) { throw new Exception("检测到服务已安装,不可重复操作"); }
                if (!Directory.Exists(servicePath)) { throw new Exception("[" + servicePath + "]目录不存在"); }
                //string cmds = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe D:\CMSService\CMSService.exe";
                Process.Start(servicePath + "Install.bat");
            }
            catch (Exception ex) { ret.retmsg = ex.Message; ret.retcode = M_APIResult.Failed; }
            return ret.ToString();
        }
        public string sites_unInstall()
        {
            M_APIResult ret = new M_APIResult();
            try
            {
                string servicePath = function.VToP("/Config/Install/");
                if (!ServiceHelper.IsExist(ConfigHelper.APPInfo.sitesServiceName)) { throw new Exception("服务未安装,不需要卸载"); }
                if (!Directory.Exists(servicePath)) { throw new Exception("[" + servicePath + "]目录不存在"); }
                //string cmds = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe D:\CMSService\CMSService.exe";
                Process.Start(servicePath + "UnInstall.bat");
            }
            catch (Exception ex) { ret.retmsg = ex.Message; ret.retcode = M_APIResult.Failed; }
            return ret.ToString();
        }
        #endregion
        public string group_list()
        {
            return JsonConvert.SerializeObject(FZHelper.GroupDT);
        }
        public string config_get()
        {
            return JsonConvert.SerializeObject(ConfigHelper.APPInfo);
        }
        public bool config_update(string json)
        {
            M_APIResult ret = new M_APIResult();
            try
            {
                APPConfigInfo model = JsonConvert.DeserializeObject<APPConfigInfo>(json);
                ConfigHelper.APPInfo.fzdir = model.fzdir;
                ConfigHelper.APPInfo.connstr = model.connstr;
                ConfigHelper.Update();
                ZoomLa.SQLDAL.DBCenter.DB.ConnectionString = ConfigHelper.APPInfo.connstr;
                Common.DB.SqlHelper.ConnectionString = ConfigHelper.APPInfo.connstr;
                FZHelper.LoadXml();
            }
            catch (Exception ex) { ret.retmsg = ex.Message; ret.retcode = M_APIResult.Failed; }
            return true;
        }
        public void fz_restart() { FZHelper.FZ_RestartServices(); }
        public bool fz_backup()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(FZHelper.xmlPath);
            doc.Save(function.VToP("/Config/FileZilla Server.xml"));
            return true;
        }


    }
}
