using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FontZ01.BLL;
using FontZ01.BrowserHandler;


namespace FontZ01
{
    /// <summary>
    /// MarkDown.xaml 的交互逻辑
    /// </summary>
    public partial class MarkDown : Window
    {
        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        ChromiumWebBrowser webView = null;
        public CefSettings setting = null;
        public MarkDown(CefSettings settings)
        {
            InitializeComponent();

            try
            {
                setting = settings;
                
                //string startUrl = "index.html"

                webView = new ChromiumWebBrowser(basePath + @"Markdown\index.html");
                // webView = new ChromiumWebBrowser("http://127.0.0.1:8080");
                webView.BrowserSettings.WebSecurity = CefState.Disabled;//允许跨域,否则无法加载file:\\\

                this.Content = webView;
                CefSharpSettings.LegacyJavascriptBindingEnabled = true;//对于旧式绑定需要设置这个值才能迁移

                CefSharpSettings.WcfEnabled = true;
                webView.JavascriptObjectRepository.Register("wpf", new JavaScriptInteractionObj(webView), isAsync: true, options: BindingOptions.DefaultBinder);


                //                                                       //webView.RegisterJsObject("wpf", new JavaScriptInteractionObj(webView));
                //webView.DownloadHandler = new DownloadHandler();
                //webView.BrowserSettings.DefaultEncoding = "utf-8";
                webView.MenuHandler = new ContextMenuHandler();//浏览器右键菜单事件处理
                webView.KeyboardHandler = new KeyboardHandler(); //键盘消息事件处理
                webView.LifeSpanHandler = new LifeSpanHandler(); //弹出窗口处理事件 

                //CefSharpSettings.WcfEnabled = true;
                //webView.JavascriptObjectRepository.Register("wpf", new JavaScriptInteractionObj(webView), isAsync: true, options: BindingOptions.DefaultBinder);

                ////js通过chrome对象调用C#函数-实现智图专用的自动存储功能
                //webView.JavascriptObjectRepository.Register("chrome", new ScriptCallbackMgr(webView), true, new BindingOptions() { CamelCaseJavascriptNames = false });
                //webView.Address = basePath + @"Markdown\" + startUrl;
                
                //webView.Address = "http://192.168.1.25:8080";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
