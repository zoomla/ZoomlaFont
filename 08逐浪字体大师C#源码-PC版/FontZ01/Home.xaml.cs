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

namespace FontZ01
{
    /// <summary>
    /// Home.xaml 的交互逻辑
    /// </summary>
    public partial class Home : Window
    {
        public CefSettings settings = null;
        public Home()
        {
            InitializeComponent();
            //参数设置
            settings = new CefSettings() { LogSeverity = LogSeverity.Disable, IgnoreCertificateErrors = true }; ;
            Cef.Initialize(settings);
        }

        /// <summary>
        /// 显示智图界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Zhitu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow(settings);
            main.ShowDialog();
        }

        /// <summary>
        /// 显示公司官网
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Zoomla_Click(object sender, RoutedEventArgs e)
        {
            Test test = new Test(settings);
            test.ShowDialog();
        }

        private void MainForm_Click(object sender, RoutedEventArgs e)
        {
            MainForm mForm = new MainForm();
            mForm.Show();
        }

        private void MarkDown_Click(object sender, RoutedEventArgs e)
        {
            MarkDown md = new MarkDown(settings);
            md.ShowDialog();
        }
    }
}