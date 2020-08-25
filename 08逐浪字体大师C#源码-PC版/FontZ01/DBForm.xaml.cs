using MJTop.Data;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
    /// DBForm.xaml 的交互逻辑
    /// </summary>
    public partial class DBForm : Window
    {
        public DBForm()
            : this(OPType.新建)
        {

        }

        /// <summary>
        /// 当前操作类型
        /// </summary>
        public OPType OpType { get; private set; }

        public int Id { get; private set; }


        public DBForm(OPType opType, int? id = null)
        {
            InitializeComponent();
            this.OpType = opType;
            if ((this.OpType == OPType.编辑 || this.OpType == OPType.克隆) && !id.HasValue)
            {
                throw new ArgumentNullException(this.OpType + "操作必须传递要操作的Id！");
            }
            else
            {
                if (id.HasValue)
                {
                    cboDBType.Items.Clear();
                    foreach (var item in FormUtils.DictDBType)
                    {
                        cboDBType.Items.Add(item.Value.ToString());
                    }

                    this.Id = id.Value;
                    DBCHMConfig config = ConfigUtils.Get(id.Value);
                    TxtConnectName.Text = config.Name;
                    cboDBType.Text = config.DBType;
                    TxtHost.Text = config.Server;
                    TxtPort.Text = config.Port?.ToString();
                    TxtUName.Text = config.Uid;
                    TxtPwd.Password = config.Pwd;
                    cboDBName.Text = config.DBName;
                    txtConnTimeOut.Text = config.ConnTimeOut?.ToString();

                    if (this.OpType == OPType.克隆)
                    {
                        TxtConnectName.Text += "_Clone";
                    }

                    if (config.DBType == DBType.SQLite.ToString())
                    {
                        btnSelectFile.Visibility = Visibility.Visible;

                        TxtHost.IsEnabled = false;
                        TxtPort.IsEnabled = false;
                        TxtUName.IsEnabled = false;

                        //暂不支持 加密的 Sqlite数据库
                        TxtPwd.IsEnabled = false;
                    }
                }
                else
                {
                    btnSelectFile.Visibility = Visibility.Hidden;
                }

                if (string.IsNullOrWhiteSpace(txtConnTimeOut.Text))
                {
                    txtConnTimeOut.Text = "60";
                }
            }
            lblMsg.Content = string.Empty;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            if (OpType == OPType.新建)
            {
                foreach (var item in FormUtils.DictDBType)
                {
                    cboDBType.Items.Add(item.Value.ToString());
                }
                cboDBType.SelectedIndex = 0;
                string port;
                if (FormUtils.DictPort.TryGetValue(cboDBType.Text, out port))
                {
                    TxtPort.Text = port;
                }
                TxtHost.Text = "127.0.0.1";
                // TODO 设置默认用户名等
                SetUserNameByDbType();
            }
        }

        /// <summary>
        /// 根据数据库类型设置默认用户名等 扩展修改 2019-01-24 21:23
        /// </summary>
        private void SetUserNameByDbType()
        {
            btnSelectFile.Visibility = Visibility.Hidden;

            TxtHost.IsEnabled = true;
            TxtPort.IsEnabled = true;
            TxtUName.IsEnabled = true;

            labDBName.Content = "数据库";
            DBType dbtype = (DBType)Enum.Parse(typeof(DBType), cboDBType.Text.ToString());
            if (dbtype == DBType.SqlServer)
            {
                TxtUName.Text = "sa";
            }
            else if (dbtype == DBType.MySql)
            {
                TxtUName.Text = "root";
            }
            else if (dbtype == DBType.Oracle || dbtype == DBType.OracleDDTek)
            {
                TxtUName.Text = "scott";
                labDBName.Content = "服务名";
            }
            else if (dbtype == DBType.PostgreSql)
            {
                TxtUName.Text = "postgres";
            }
            else if (dbtype == DBType.DB2)
            {
                TxtUName.Text = "db2admin";
            }
            else if (dbtype == DBType.SQLite)
            {
                btnSelectFile.Visibility = Visibility.Visible;

                TxtHost.IsEnabled = false;
                TxtPort.IsEnabled = false;
                TxtUName.IsEnabled = false;

                //暂不支持 加密的 Sqlite数据库
                TxtPwd.IsEnabled = false;
            }
            else
            {
                TxtUName.Text = "";
            }
        }

        /// <summary>
        /// 连接/测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnTestConnect_Click(object sender, RoutedEventArgs e)
        {
            DBType type = (DBType)Enum.Parse(typeof(DBType), cboDBType.Text);
            try
            {
                if (type == DBType.Oracle && string.IsNullOrWhiteSpace(cboDBName.Text))
                {
                    throw new Exception("Oracle没有提供数据库名称查询支持，请输入服务名！");
                }

                string strDBName = cboDBName.Text;
                DBUtils.Instance = DBMgr.UseDB(type, TxtHost.Text,
                    string.IsNullOrWhiteSpace(TxtPort.Text) ? null : new int?(Convert.ToInt32(TxtPort.Text)),
                    strDBName, TxtUName.Text, TxtPwd.Password,
                    string.IsNullOrWhiteSpace(txtConnTimeOut.Text) ? 60 : Convert.ToInt32(txtConnTimeOut.Text), 300);
                var info = DBUtils.Instance.Info;

                cboDBName.Items.Clear();
                foreach (var dbName in info.DBNames)
                {
                    cboDBName.Items.Add(dbName);
                }
                cboDBName.SelectedItem = strDBName;
                this.Title = "连接服务器成功！";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TxtConnectName.Text))
                {
                    MessageBox.Show("请输入连接名", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (string.IsNullOrWhiteSpace(cboDBName.Text))
                {
                    MessageBox.Show("请输入数据库名称！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                DBType type = (DBType)Enum.Parse(typeof(DBType), cboDBType.Text);
                string connString = DBMgr.GetConnectionString(type, TxtHost.Text,
                    (string.IsNullOrWhiteSpace(TxtPort.Text) ? null : new Nullable<int>(Convert.ToInt32(TxtPort.Text))),
                    cboDBName.Text, TxtUName.Text, TxtPwd.Password,
                    (string.IsNullOrWhiteSpace(txtConnTimeOut.Text) ? 60 : Convert.ToInt32(txtConnTimeOut.Text))
                    );
                NameValueCollection nvc = new NameValueCollection();
                if (OpType == OPType.新建 || OpType == OPType.克隆)
                {
                    nvc.Add("Name", TxtConnectName.Text.Trim());
                    nvc.Add("DBType", cboDBType.Text.Trim());
                    nvc.Add("Server", TxtHost.IsEnabled ? TxtHost.Text.Trim() : string.Empty);
                    nvc.Add("Port", TxtPort.IsEnabled ? TxtPort.Text : string.Empty);
                    nvc.Add("DBName", cboDBName.Text.Trim());
                    nvc.Add("Uid", TxtUName.IsEnabled ? TxtUName.Text.Trim() : string.Empty);
                    nvc.Add("Pwd", TxtPwd.IsEnabled ? TxtPwd.Password : string.Empty);
                    nvc.Add("ConnTimeOut", txtConnTimeOut.IsEnabled ? txtConnTimeOut.Text : "60");
                    nvc.Add("ConnString", connString);
                    nvc.Add("Modified", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                    ConfigUtils.Save(nvc);
                }
                else if (OpType == OPType.编辑)
                {
                    nvc.Add("Id", Id.ToString());
                    nvc.Add("Name", TxtConnectName.Text.Trim());
                    nvc.Add("DBType", cboDBType.Text.Trim());
                    nvc.Add("Server", TxtHost.IsEnabled ? TxtHost.Text.Trim() : string.Empty);
                    nvc.Add("Port", TxtPort.IsEnabled ? TxtPort.Text : string.Empty);
                    nvc.Add("DBName", cboDBName.Text.Trim());
                    nvc.Add("Uid", TxtUName.IsEnabled ? TxtUName.Text.Trim() : string.Empty);
                    nvc.Add("Pwd", TxtPwd.IsEnabled ? TxtPwd.Password : string.Empty);
                    nvc.Add("ConnTimeOut", txtConnTimeOut.IsEnabled ? txtConnTimeOut.Text : "60");
                    nvc.Add("ConnString", connString);
                    nvc.Add("Modified", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    ConfigUtils.Save(nvc);
                }
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            this.DialogResult = false;
        }
    }
}
