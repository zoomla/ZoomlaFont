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
using FontZ01.BrowserHandler;

namespace FontZ01
{
    /// <summary>
    /// Test.xaml 的交互逻辑
    /// </summary>
    public partial class Test : Window
    {
        //定义浏览器对象
        public ChromiumWebBrowser ChromeBrowser;
        public CefSettings setting = null;

        public Test(CefSettings settings)
        {
            InitializeComponent();
            setting = settings;
            InitializeChromium();
        }
        /// <summary>
        /// 初始化浏览器并启动
        /// </summary>
        public void InitializeChromium()
        {
            ////参数设置
            //CefSettings settings = new CefSettings();
            //Cef.Initialize(settings);
            //创建实例
            ChromeBrowser = new ChromiumWebBrowser("http://v.ziti163.com");
            ChromeBrowser.RequestContext = new RequestContext();
            //添加控件
            this.Content = ChromeBrowser;
            this.WindowState = WindowState.Maximized;   //全屏打开
            ChromeBrowser.MenuHandler = new ContextMenuHandler();  //浏览器右键菜单事件处理
            ChromeBrowser.KeyboardHandler = new KeyboardHandler(); //键盘消息事件处理
            ChromeBrowser.LifeSpanHandler = new LifeSpanHandler(); //弹出窗口处理事件
        }
    }
}
