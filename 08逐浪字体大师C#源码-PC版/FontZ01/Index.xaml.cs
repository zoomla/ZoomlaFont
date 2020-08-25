using CefSharp;
using CefSharp.Wpf;
using ControlzEx.Standard;
using DevExpress.Xpf.Charts.Native;
using FontZ01.BrowserHandler;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;

namespace FontZ01
{
    /// <summary>
    /// Index.xaml 的交互逻辑
    /// </summary>
    public partial class Index : Window
    {
        /// <summary>
        /// 最小化控件
        /// </summary>
        WindowState ws;
        WindowState wsl;
        NotifyIcon notifyIcon;
        public CefSettings setting = new CefSettings();
        public ChromiumWebBrowser webView;
        public System.Timers.Timer timer = new System.Timers.Timer(1000);
        public System.Timers.Timer _timer = new System.Timers.Timer(7000);
        //ip地址
        public string IP = "";
        public string RAM = "";
        public Index()
        {
            Thread.Sleep(4000);
            InitializeComponent();
            ds = new System.Windows.Threading.DispatcherSynchronizationContext();
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuCounter.MachineName = ".";
            cpuCounter.NextValue();
        }
        PerformanceCounter cpuCounter;
        private void Window_Initialized(object sender, EventArgs e)
        {
            #region 得到电脑屏幕分辨率
            //this.Width = SystemParameters.WorkArea.Width / 1.5;//得到屏幕工作区域宽度
            //this.Height = SystemParameters.WorkArea.Height / 1.5;//得到屏幕工作区域高度
            #endregion

            #region 显示小图标
            icon();
            wsl = WindowState.Minimized;
            #endregion
        }

        #region 获取本机内存
        /// <summary>
        /// 获取本机内存
        /// </summary>
        /// <returns></returns>
        private static long GetPhisicalMemory()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(); //用于查询一些如系统信息的管理对象
            searcher.Query = new SelectQuery("Win32_PhysicalMemory", "", new string[] { "Capacity" });//设置查询条件
            ManagementObjectCollection collection = searcher.Get(); //获取内存容量
            ManagementObjectCollection.ManagementObjectEnumerator em = collection.GetEnumerator();
            long capacity = 0;
            while (em.MoveNext())
            {
                ManagementBaseObject baseObj = em.Current;
                if (baseObj.Properties["Capacity"].Value != null)
                {
                    try
                    {
                        capacity += long.Parse(baseObj.Properties["Capacity"].Value.ToString());
                    }
                    catch
                    {
                        Console.WriteLine("有错误发生！", "错误信息");
                        return 0;
                    }
                }
            }
            return capacity / 1024 / 1024 / 1024;
        }
        #endregion

        public string GetAddress(string ip)
        {
            return ip;
        }

        #region 最小化时任务栏出现小图标
        /// <summary>
        /// 最小化时任务栏出现小图标
        /// </summary>
        private void icon()
        {
            string Path = AppDomain.CurrentDomain.BaseDirectory + @"res\favicon.ico";
            if (File.Exists(Path))
            {
                this.notifyIcon = new NotifyIcon();
                this.notifyIcon.BalloonTipText = "逐浪字体大师"; //设置程序启动时显示的文本
                this.notifyIcon.Text = "逐浪字体大师";//最小化到托盘时，鼠标点击时显示的文本
                System.Drawing.Icon icon = new System.Drawing.Icon(Path);//程序图标     
                this.notifyIcon.Icon = icon;
                this.notifyIcon.Visible = true;

                //字体大师官网
                System.Windows.Forms.MenuItem ziti_com = new System.Windows.Forms.MenuItem("字体网");
                ziti_com.Click += new EventHandler(ShowOW_ziticom);
                //字体官网
                System.Windows.Forms.MenuItem ziti = new System.Windows.Forms.MenuItem("字体大师官网");
                ziti.Click += new EventHandler(ShowOW); 
                //github
                System.Windows.Forms.MenuItem zfont_github = new System.Windows.Forms.MenuItem("Github仓库");
                zfont_github.Click += new EventHandler(ShowOW_zfont_github);
                //右键菜单--打开菜单项
                System.Windows.Forms.MenuItem open = new System.Windows.Forms.MenuItem("显示主界面");
                open.Click += new EventHandler(ShowWindow);
                //右键菜单--退出菜单项
                System.Windows.Forms.MenuItem exit = new System.Windows.Forms.MenuItem("退出");
                exit.Click += new EventHandler(CloseWindow);
                //关联托盘控件
                System.Windows.Forms.MenuItem[] childen = new System.Windows.Forms.MenuItem[] { ziti_com, ziti, zfont_github, open, exit };
                notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(childen);

                notifyIcon.MouseDoubleClick += OnNotifyIconDoubleClick;
                this.notifyIcon.ShowBalloonTip(1000);
            }

        }

        /// <summary>
        /// 显示字体网
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowOW_ziticom(object sender, EventArgs e)
        {
            Process.Start("http://www.ziti163.com");
        }
        /// <summary>
        /// 进入github
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowOW_zfont_github(object sender, EventArgs e)
        {
            Process.Start("https://github.com/zoomla/ZoomlaFont");
        }
        /// <summary>
        /// 显示字体大师官网
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowOW(object sender, EventArgs e)
        {
            Process.Start("http://v.ziti163.com");
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseWindow(object sender, EventArgs e)
        {
            this.notifyIcon.Visible = false;
            this.Close();
            System.Windows.Application.Current.Shutdown();
        }

        /// <summary>
        /// 打开菜单栏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowWindow(object sender, EventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Visible;
            this.ShowInTaskbar = true;
            this.Activate();
        }

        private void OnNotifyIconDoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Minimized;
            this.WindowState = WindowState.Normal;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            ws = this.WindowState;
            if (ws == WindowState.Minimized)
            {
                this.Hide();
            }
        }

        #endregion

        #region 检测网络是否正常
        /// <summary>
        /// 检测网络是否正常
        /// </summary>
        /// <param name="StrIpOrDName"></param>
        /// <returns></returns>
        public static bool PingIpOrDomainName(string StrIpOrDName)
        {
            try
            {
                Ping objPingSender = new Ping();
                PingOptions objPinOptions = new PingOptions();
                objPinOptions.DontFragment = true;
                string data = "";
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                int intTimeout = 120;
                PingReply objPinReply = objPingSender.Send(StrIpOrDName, intTimeout, buffer, objPinOptions);
                string strInfo = objPinReply.Status.ToString();
                if (strInfo == "Success")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception) { return false; }
        }
        #endregion

        #region 窗体操作
        /// <summary>
        /// 窗体加载中操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetSubtitle();
            Color color = Color.FromRgb(25, 25, 25);
            this.Background = new SolidColorBrush(color);
            try
            {
                IP = XCLNetTools.Common.IPHelper.GetLocalIP();
                this.Iswangluo.Content = "网络：畅通";
            }
            catch (Exception ex)
            {
                this.Iswangluo.Content = "网络：未连接";
                System.Windows.MessageBox.Show("未检测出网络!", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            RAM = GetPhisicalMemory().ToString();
            GetAddress(IP);
            timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            this.Barrage.Text = subtitle[m];
            m += 1;
            _timer.AutoReset = true;
            _timer.Elapsed += _timer_Elapsed; ;
            _timer.Start();
        }

        #region 字幕链接
        /// <summary>
        /// 字幕链接
        /// </summary>
        public void SetSubtitle()
        {
            string[] Link = { "https://www.ziti163.com/Item/2708.aspx", "https://www.ziti163.com/Item/2707.aspx", "https://www.ziti163.com/Item/2706.aspx", "https://www.ziti163.com/Item/2705.aspx", "https://www.ziti163.com/Item/2704.aspx", "https://www.ziti163.com/Item/2700.aspx" };
            zimu zimu = new zimu { subtitle = subtitle[0], link = Link[0] };
            GetZimus.Add(zimu);
            zimu zimu1 = new zimu { subtitle = subtitle[1], link = Link[1] };
            GetZimus.Add(zimu1);
            zimu zimu2 = new zimu { subtitle = subtitle[2], link = Link[2] };
            GetZimus.Add(zimu2);
            zimu zimu3 = new zimu { subtitle = subtitle[3], link = Link[3] };
            GetZimus.Add(zimu3);
            zimu zimu4 = new zimu { subtitle = subtitle[4], link = Link[4] };
            GetZimus.Add(zimu4);
            zimu zimu5 = new zimu { subtitle = subtitle[5], link = Link[5] };
            GetZimus.Add(zimu5);
        }
        string[] subtitle = { "五年的时间-我完成了国内首发独一无二的海昏侯字体全套字库(2020/08/17)", "字库厂商蒙纳字体monotype介绍(2020/08/14)",
            "海昏侯考古结硕果:逐浪海昏侯汉简隶书现代字库发布(2020/08/05)", "逐浪唐寅行书体(首款唐伯虎原迹高清字库/唐伯虎天下第一风流行书体(2020/08/05)","为有鲁直报国强-逐浪粗颜楷精品字库全面发布(2020/08/05)",
            "助力夜宵经济+中国首款夜宵酒吧专用字体-逐浪海鲜排档撸串体发布(2020/08/05)"};
        public List<zimu> GetZimus = new List<zimu>();
        #endregion
        public int m = 0;
        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ds.Send(obj =>
            {
                this.Barrage.Text = subtitle[m];
                try
                {
                    XCLNetTools.Common.IPHelper.GetLocalIP();
                    this.Iswangluo.Content = "网络：畅通";
                }
                catch (Exception ex)
                {
                    this.Iswangluo.Content = "网络：未连接";
                }
                if (m >= subtitle.Length - 1)
                {
                    m = 0;
                }
                else
                {
                    m++;
                }
            }, null);
        }

        System.Threading.SynchronizationContext ds = null;
        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ds.Send(obj =>
            {
                if (string.IsNullOrEmpty(IP))
                    this.Information.Content = "CPU：" + Math.Round(cpuCounter.NextValue()) + " % " + "  内存：" + RAM + "G";
                else
                    this.Information.Content = "CPU：" + Math.Round(cpuCounter.NextValue()) + " % " + "  内存：" + RAM + "G" + "  当前IP：" + IP;
            }, null);
        }


        /// <summary>
        /// 最小化窗体事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;   
        }

        /// <summary>
        /// 关闭窗体事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cloes_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (System.Windows.MessageBox.Show("您确定关闭吗", "提示", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                this.notifyIcon.Visible = false;
                ShowInTaskbar = false;
                this.Close();
                System.Windows.Application.Current.Shutdown();
            }
        }
        /// <summary>
        /// 拖动窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        #endregion

        #region 字库创作
        /// <summary>
        /// 中文字库创作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Name_Click(object sender, RoutedEventArgs e)
        {
            Test test = new Test(setting);
            test.ShowDialog();
        }
        #endregion


        #region 我的字库
        /// <summary>
        /// 我的字库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void font_myfont_Click(object sender, RoutedEventArgs e)
        {
            MyFont test = new MyFont(setting);
            test.ShowDialog();
        }

        /// <summary>
        /// 英文字库创作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Name1_Click(object sender, RoutedEventArgs e)
        {
            MarkDown md = new MarkDown(setting);
            md.ShowDialog();
        }
        #endregion

        /// <summary>
        /// 字体素材下载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Font_website_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://www.ziti163.com/");
        }

        /// <summary>
        /// 字体素材下载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Font_network_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://www.ziti163.com/Item/2709.aspx");
        }
        
        /// <summary>
        /// 字库索引
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void font_list_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://f.ziti163.com/Class_8/Default");
        }

        /// <summary>
        /// 字库欣赏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void font_suoyin_Click(object sender, RoutedEventArgs e)
        {
            Writing writing = new Writing("font_suoyin");
            writing.ShowDialog();
        }

        /// <summary>
        /// 方言
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Fy_network_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://a.ziti163.com");
        }

        private void Font_unicode_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://www.ziti163.com/uni");
        }
        
        private void Font_stu_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.ziti163.com/Class_11/Default.aspx");
        } 
        
        private void Font_dict_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://zd.ziti163.com");
        }

        private void Font_webfont_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://v.ziti163.com/font/webfont");
        } 
        
        private void Font_bihua_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://www.ziti163.com/Item/2402.aspx");
        }
               
        private void font_Name1_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://www.ziti163.com/Item/2285.aspx");
        } 

        private void font_Name2_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://www.ziti163.com/Item/2483.aspx");
        }

        private void font_Name3_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://www.ziti163.com/Item/1942.aspx");
        }

        private void font_Name4_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://www.ziti163.com/Item/1929.aspx");
        }

        private void font_Name5_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://www.ziti163.com/Item/2708.aspx");
        }
        
        private void font_Name6_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://www.ziti163.com/Class_64/Default.aspx");
        }

        #region 汉字书写
        /// <summary>
        /// “中”字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Writing writing = new Writing("中");
            writing.ShowDialog();
        }

        /// <summary>
        /// 国
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Writing writing = new Writing("国");
            writing.ShowDialog();
        }

        /// <summary>
        /// 梦
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void meng_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Writing writing = new Writing("梦");
            writing.ShowDialog();
        }

        /// <summary>
        /// 逐
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Zhu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Writing writing = new Writing("逐");
            writing.ShowDialog();
        }

        /// <summary>
        /// 浪
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Lang_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Writing writing = new Writing("浪");
            writing.ShowDialog();
        }

        /// <summary>
        /// 人
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ren_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Writing writing = new Writing("人");
            writing.ShowDialog();
        }
        #endregion


        private void JaedRing_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("http://v.ziti163.com");
        }

        /// <summary>
        /// 字幕连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Barrage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string zi = this.Barrage.Text.Trim();
            zimu zimu = GetZimus.Find(i => i.subtitle == zi);
            Process.Start(zimu.link);
        }
    }
    public class zimu
    {
        public string subtitle { get; set; }
        public string link { get; set; }
    }
}
