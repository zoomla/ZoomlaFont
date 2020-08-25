using MJTop.Data;
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
using FontZ01.Commons;

namespace FontZ01
{
    /// <summary>
    /// Data_Management.xaml 的交互逻辑
    /// </summary>
    public partial class Data_Management : Window
    {
        public Data_Management()
        {
            InitializeComponent();
        }

        private void GV_DBConfigs_Loaded(object sender, RoutedEventArgs e)
        {
            Collapsed();
        }

        /// <summary>
        /// 添加连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        DBForm dbForm = null;
        private void linkAdd_Click(object sender, RoutedEventArgs e)
        {
            dbForm = new DBForm(OPType.新建);
            bool? dia = dbForm.ShowDialog();
            if (dia == true)
            {
                RefreshListView();
                Collapsed();
            }
        }

        private void Collapsed()
        {
            GV_DBConfigs.Columns[0].Visibility = Visibility.Collapsed;
        }


        /// <summary>
        /// 刷新列表
        /// </summary>
        private void RefreshListView()
        {
            var data = ConfigUtils.SelectAll();
            if (data != null)
            {
                data.ForEach(item =>
                {
                    string num = string.Empty;
                    for (int i = 0; i < item.Pwd.Length; i++)
                    {
                        num += "*";
                    }
                    item.Pwd = num;
                });
                this.GV_DBConfigs.ItemsSource = data;
            }
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkEdit_Click(object sender, RoutedEventArgs e)
        {
            if (GV_DBConfigs.SelectedItem == null)
            {
                MessageBox.Show("请选择连接！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DBCHMConfig dc = GV_DBConfigs.SelectedItem as DBCHMConfig;
            int Id = dc.Id;
            DBForm dbForm = new DBForm(OPType.编辑, Id);
            var diaResult = dbForm.ShowDialog();
            if (diaResult == true)
            {
                RefreshListView();
                Collapsed();
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkRemove_Click(object sender, RoutedEventArgs e)
        {
            if (GV_DBConfigs.SelectedItem == null)
            {
                MessageBox.Show("请选择连接！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show("确定要删除该连接吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation) == MessageBoxResult.OK)
            {
                DBCHMConfig dc = GV_DBConfigs.SelectedItem as DBCHMConfig;
                int Id = dc.Id;
                ConfigUtils.Delete(Id);

                RefreshListView();
                Collapsed();
            }
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void linkClone_Click(object sender, RoutedEventArgs e)
        {
            if (GV_DBConfigs.SelectedItem == null)
            {
                MessageBox.Show("请选择连接！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DBCHMConfig dc = GV_DBConfigs.SelectedItem as DBCHMConfig;
            int Id = dc.Id;
            DBForm dbForm = new DBForm(OPType.克隆, Id);
            var diaResult = dbForm.ShowDialog();
            if (diaResult == true)
            {
                RefreshListView();
                Collapsed();
            }
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnConnect_Click(object sender, RoutedEventArgs e)

        {
            if (GV_DBConfigs.SelectedItem == null)
            {
                MessageBox.Show("请选择连接！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DBCHMConfig dc = GV_DBConfigs.SelectedItem as DBCHMConfig;
            int Id = dc.Id;
            DBCHMConfig config = ConfigUtils.Get(Id);


            if ((DBType)Enum.Parse(typeof(DBType), config.DBType) == DBType.SqlServer && !dc.Uid.Equals("sa", StringComparison.OrdinalIgnoreCase))
            {
                var dia = MessageBox.Show("非超级管理员的账号，可能因权限不足，查询不出表结构信息，确定要继续吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (dia == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            try
            {
                this.DialogResult = true;
                DBUtils.Instance = DBMgr.UseDB((DBType)Enum.Parse(typeof(DBType), config.DBType), config.ConnString, 300);
                ConfigUtils.UpLastModified(Id);
            }
            catch (Exception ex)
            {
                LogUtils.LogError("连接数据库失败", Developer.SysDefault, ex, config);
                MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        /// <summary>
        /// 窗体初始化获取连接数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Initialized(object sender, EventArgs e)
        {
            RefreshListView();
        }

        private void GV_DBConfigs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BtnConnect_Click(sender, e);
            //代表已经正常选中
            FormUtils.IsOK_Close = true;
            this.Close();
        }
    }
}
