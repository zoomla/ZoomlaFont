using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

/// <summary>
/// 本文件是主题设置，本项目移除了主题，使用默认windows wpf主题
/// </summary>

namespace FontZ01
{
    public partial class WindowBase : Window
    {
        //下面几行是引用主题的现注释
        //public WindowBase()
        //{
        //    InitializeTheme();
        //    InitializeStyle();

        //    this.Loaded += delegate
        //    {
        //        InitializeEvent();
        //    };
        //}
        protected virtual void MinWin()
        {
            this.WindowState = WindowState.Minimized;
        }
        protected virtual void MaxWin()
        {
            this.WindowState = WindowState.Normal;
            this.WindowState = WindowState.Maximized;
        }
        public Button YesButton
        {
            get;
            set;
        }
        public Button NoButton
        {
            get;
            set;
        }
        private void InitializeEvent()
        {
            ControlTemplate baseWindowTemplate = (ControlTemplate)App.Current.Resources["BaseWindowControlTemplate"];

            Border borderTitle = (Border)baseWindowTemplate.FindName("borderTitle", this);
            Button closeBtn = (Button)baseWindowTemplate.FindName("btnClose", this);
            Button minBtn = (Button)baseWindowTemplate.FindName("btnMin", this);
            Button maxBtn=(Button)baseWindowTemplate.FindName("btnMax", this);
            YesButton = (Button)baseWindowTemplate.FindName("btnYes", this);
            NoButton = (Button)baseWindowTemplate.FindName("btnNo", this);

            minBtn.Click += delegate { MinWin(); };
            maxBtn.Click += delegate { MaxWin(); };
            closeBtn.Click += delegate { this.Close(); };

            borderTitle.MouseMove += delegate (object sender, MouseEventArgs e)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            };


        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
        }
        public Canvas GridContent
        {
            get;
            set;
        }
        private void InitializeStyle()
        {
            this.Style = (Style)App.Current.Resources["BaseWindowStyle"];
        }
        private void InitializeTheme()
        {
            string themeName = "Black";
            App.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(new Uri(string.Format("../Theme/{0}/WindowBaseStyle.xaml", themeName), UriKind.Relative)) as ResourceDictionary);
        }
        private bool _allowSizeToContent = false;
        /// <summary>
        /// 自定义属性，用于标记该窗体是否允许按内容适应，设此属性是为了解决最大化按钮当SizeToContent属性为WidthAndHeight时不能最大化，从而最大、最小化必须变更SizeToContent的值的问题
        /// </summary>
        public bool AllowSizeToContent
        {
            get
            {
                return _allowSizeToContent;
            }
            set
            {
                this.SizeToContent = (value ? SizeToContent.WidthAndHeight : SizeToContent.Manual);
                _allowSizeToContent = value;
            }
        }
    }
}
