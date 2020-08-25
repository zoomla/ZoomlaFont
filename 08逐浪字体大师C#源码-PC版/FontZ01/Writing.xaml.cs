using System;
using System.Collections.Generic;
using System.Linq;
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
using CefSharp;
using CefSharp.Wpf;
using FontZ01.BLL;
using FontZ01.BrowserHandler;

namespace FontZ01
{
    /// <summary>
    /// Writing.xaml 的交互逻辑
    /// </summary>
    public partial class Writing : Window
    {
        string basePath = AppDomain.CurrentDomain.BaseDirectory;
       public ChromiumWebBrowser webView = null;

        public Writing(string Chinese)
        {
            InitializeComponent();
            webView = new ChromiumWebBrowser();
            webView.BrowserSettings.WebSecurity = CefState.Disabled;//允许跨域,否则无法加载file:\\\
            this.Content = webView;
            this.WindowState = WindowState.Normal;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;//对于旧式绑定需要设置这个值才能迁移
            CefSharpSettings.WcfEnabled = true;
            //js调用C#方法实现在浏览器弹出字体官网
            webView.JavascriptObjectRepository.Register("wpf", new JavaScriptInteractionObj(webView), isAsync: true, options: BindingOptions.DefaultBinder);

            webView.MenuHandler = new ContextMenuHandler();//浏览器右键菜单事件处理
            webView.KeyboardHandler = new KeyboardHandler(); //键盘消息事件处理
            webView.LifeSpanHandler = new LifeSpanHandler(); //弹出窗口处理事件
            Load(Chinese);
        }

        public void Load(string Chinese)
        {
            switch (Chinese)
            {
                case "中":
                    webView.Address = basePath + @"Writing\write.html";
                    break;
                case "国":
                    webView.Address = basePath + @"Writing\write.html#/font1";
                    break;
                case "梦":
                    webView.Address = basePath + @"Writing\write.html#/font2";
                    break;
                case "逐":
                    webView.Address = basePath + @"Writing\write.html#/font3";
                    break;
                case "浪":
                    webView.Address = basePath + @"Writing\write.html#/font4";
                    break;
                case "人":
                    webView.Address = basePath + @"Writing\write.html#/font5";
                    break;
                case "font_suoyin":
                    webView.Address = basePath + @"Writing\write.html#/font_suoyin";
                    break;
            }
        }
    }
}
