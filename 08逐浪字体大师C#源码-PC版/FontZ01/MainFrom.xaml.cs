using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FontZ01.Commons;
using MJTop.Data;
using System.ComponentModel;
using System.IO;
using System.Diagnostics;
using TryOpenXml.Dtos;
using System.Windows.Forms;
using FontZ01.CHM;
using DevExpress.Xpf.Editors.Helpers;

namespace FontZ01
{
    /// <summary>
    /// MianForm.xaml 的交互逻辑
    /// </summary>
    public partial class MainForm : Window
    {
        // TODO 已选择的表
        private string selectedTableDesc = "已选择{0}张表";

        public BackgroundWorker bgWork = new BackgroundWorker();
        System.Threading.SynchronizationContext ds = null;
        public MainForm()
        {
            InitializeComponent();
            ds = new System.Windows.Threading.DispatcherSynchronizationContext();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {

            lblTip.Content = string.Empty;

            // 开启 报告进度更新
            bgWork.WorkerReportsProgress = true;

            // 注册 任务执行事件
            bgWork.DoWork += BgWork_DoWork;

            // 注册 报告进度更新事件
            bgWork.ProgressChanged += BgWork_ProgressChanged;

            //初始化窗体
            InitMain();
            // IsReadOnly();
        }

        private void BgWork_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // 设置 进度位置
            ds.Send(obj =>
            {
                Prog.Value = e.ProgressPercentage;
                if (e.UserState != null)
                {
                    lblMsg.Content = "操作失败！";
                    lblMsg.Foreground = new SolidColorBrush(Colors.Red);

                    Exception ex = e.UserState as Exception;
                    if (ex != null)
                    {
                        LogUtils.LogError("DBCHM执行出错", Developer.MJ, ex);
                        var diaRes = System.Windows.MessageBox.Show("很抱歉，执行过程出现错误，出错原因：\r\n" + e.UserState.ToString() + "\r\n\r\n是否打开错误日志目录？", "程序执行出错", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);
                        if (diaRes == MessageBoxResult.Yes)
                        {
                            string dir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");
                            Process.Start(dir);
                        }
                    }
                }
            }, null);
        }

        private void BgWork_DoWork(object sender, DoWorkEventArgs e)
        {
            if (FormUtils.ProgArg != null)
            {
                ds.Send(obj =>
                {
                    lblMsg.Content = string.Empty;

                    Prog.Maximum = FormUtils.ProgArg.MaxNum;
                    FormUtils.ProgArg.ExecAct.Invoke();

                    if (string.IsNullOrWhiteSpace(lblMsg.Content.ToString()))
                    {
                        lblMsg.Content = "操作已完成！";
                        lblMsg.Foreground = new SolidColorBrush(Colors.Green);
                    }

                    FormUtils.ProgArg = null;
                }, null);

            }
        }

        private void InitMain()
        {
            Data_Management window = new Data_Management();
            var diaRes = window.ShowDialog();
            if (diaRes == true || FormUtils.IsOK_Close)
            {
                List<IsCheck> che = new List<IsCheck>();
                foreach (var item in DBUtils.Instance?.Info.TableNames)
                {
                    IsCheck check = new IsCheck
                    {
                        Table_Name = item,
                        IsChecked = false
                    };
                    che.Add(check);
                }
                checedkListBox.ItemsSource = che;
                FormUtils.IsOK_Close = false;
            }
            else
            {
                return;
            }
            if (checedkListBox.Items.Count > 0)//默认选择第一张表
            {
                checedkListBox.SelectedIndex = 0;
                // LabCurrTabName.Content = checedkListBox.SelectedItem.ToString();
                TxtCurrTabComment.Text = DBUtils.Instance?.Info?.TableComments[LabCurrTabName.Content.ToString()];
            }
            else  //无数据表时，清空 Gird列表
            {
                GV_ColComments.Items.Clear();
            }
            if (!string.IsNullOrWhiteSpace(DBUtils.Instance?.Info?.DBName))
            {
                this.Title = DBUtils.Instance?.Info?.DBName + "(" + DBUtils.Instance.DBType.ToString() + ")" + "DBCHM v" + Assembly.GetExecutingAssembly().GetName().Version.ToString().Replace(".0.0", "");
            }

            //Sqlite 数据库自身 不支持 数据库批注 功能
            if (DBUtils.Instance != null && DBUtils.Instance.DBType == MJTop.Data.DBType.SQLite)
            {
                TxtCurrTabComment.IsEnabled = false;
                BtnSaveGridData.IsEnabled = false;
                lblTip.Content = DBUtils.Instance.DBType + "数据库不支持批注功能！";
                lblTip.Foreground = new SolidColorBrush(Colors.Red);
                GV_ColComments.Columns[1].IsReadOnly = true;
            }
            else
            {
                TxtCurrTabComment.IsEnabled = true;
                BtnSaveGridData.IsEnabled = true;
                lblTip.Content = string.Empty;
                lblTip.Foreground = new SolidColorBrush(Colors.Green);
            }
        }

        public void IsReadOnly()
        {
            if (DBUtils.Instance != null && DBUtils.Instance.DBType == DBType.SQLite)
            {
                GV_ColComments.Columns[1].IsReadOnly = true;
            }
            else
            {
                GV_ColComments.Columns[1].IsReadOnly = false;
            }
        }

        private void IsChecks_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.CheckBox box = sender as System.Windows.Controls.CheckBox;
            for (int i = 0; i < checedkListBox.Items.Count; i++)
            {
                IsCheck check = checedkListBox.Items[i] as IsCheck;
                if (check != null)
                {
                    if (check.Table_Name == box.Content.ToString())
                    {
                        string it = list.Find(item => item == box.Content.ToString());
                        if (string.IsNullOrEmpty(it))
                        {
                            list.Add(box.Content.ToString());
                        }
                        else
                        {
                            list.Remove(box.Content.ToString());
                        }
                        checedkListBox.SelectedIndex = i;
                        this.Number.Content = string.Format(selectedTableDesc, list.Count);
                        return;
                    }
                }
            }
        }

        public List<string> list = new List<string>();
        private void checedkListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (checedkListBox.SelectedItems.Count > 0)
            {
                Prog.Value = 0;
                lblMsg.Content = string.Empty;
                GV_ColComments.ItemsSource = null;
                GV_ColComments.Items.Clear();
                IsCheck check = checedkListBox.SelectedItem as IsCheck;
                if (check != null)
                {
                    LabCurrTabName.Content = check.Table_Name;
                    TxtCurrTabComment.Text = DBUtils.Instance?.Info?.TableComments[LabCurrTabName.Content.ToString()];

                    var columnInfos = DBUtils.Instance?.Info?.GetColumns(LabCurrTabName.Content.ToString());

                    if (columnInfos != null)
                    {
                        List<Display> displays = new List<Display>();

                        foreach (var colInfo in columnInfos)
                        {
                            Display dis = new Display
                            {
                                ColumnName = colInfo.ColumnName,
                                TypeName = colInfo.TypeName,
                                Length = colInfo.Length,
                                DeText = colInfo.DeText
                            };
                            displays.Add(dis);
                        }
                        GV_ColComments.ItemsSource = displays;
                    }
                    else
                    {
                        DBUtils.Instance?.Info?.Refresh();
                    }
                }
            }
        }

        /// <summary>
        /// 模糊搜索数据表名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            string strName = TxtTabName.Text.Trim().ToLower();
            var lstTableName = DBUtils.Instance?.Info?.TableNames;
            list.Clear();
            checedkListBox.ItemsSource = null;
            checedkListBox.Items.Clear();
            this.Number.Content = string.Format(selectedTableDesc, 0);

            if (!string.IsNullOrWhiteSpace(strName))
            {
                lstTableName.ForEach(t =>
                {
                    if (strName.Contains(","))
                    {
                        // TODO 多个关键词模糊匹配
                        Dictionary<string, string> tableDic = new Dictionary<string, string>();
                        string[] keys = strName.Split(',');
                        foreach (string key in keys)
                        {
                            if (string.IsNullOrWhiteSpace(key))
                                continue;
                            if (t.ToLower().Contains(key) && !tableDic.ContainsKey(t))
                            {
                                tableDic.Add(t, t);
                            }
                        }
                        if (null != tableDic || tableDic.Count > 0)
                        {
                            foreach (KeyValuePair<string, string> kvp in tableDic)
                            {
                                IsCheck check = new IsCheck
                                {
                                    IsChecked = false,
                                    Table_Name = kvp.Key
                                };
                                checedkListBox.Items.Add(check);
                            }
                        }
                    }
                    else
                    {
                        // TODO 单个关键词模糊匹配
                        if (t.ToLower().Contains(strName))
                        {
                            IsCheck check = new IsCheck
                            {
                                IsChecked = false,
                                Table_Name = t
                            };
                            checedkListBox.Items.Add(check);
                        }
                    }
                });
            }
            else//默认所有数据表
            {
                List<IsCheck> che = new List<IsCheck>();
                foreach (var item in DBUtils.Instance?.Info.TableNames)
                {
                    IsCheck check = new IsCheck
                    {
                        Table_Name = item,
                        IsChecked = false
                    };
                    che.Add(check);
                }
                checedkListBox.ItemsSource = che;
            }

            if (checedkListBox.Items.Count > 0)//默认选择第一张表
            {
                checedkListBox.SelectedIndex = 0;
                // LabCurrTabName.Content = checedkListBox.SelectedItem.ToString();
                TxtCurrTabComment.Text = DBUtils.Instance?.Info?.TableComments[LabCurrTabName.Content.ToString()];
            }
        }


        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSaveGridData_Click(object sender, RoutedEventArgs e)
        {
            //设置要 滚动条 对应的执行方法，以及滚动条最大值
            FormUtils.ProgArg = new ProgressArg(() =>
            {
                bool? blRes = DBUtils.Instance?.Info?.SetTableComment(LabCurrTabName.Content.ToString(), TxtCurrTabComment.Text.Replace("'", "`"));
                if (blRes.HasValue)
                {
                    if (blRes.Value)
                    {
                        bgWork.ReportProgress(1);
                    }
                    else
                    {
                        bgWork.ReportProgress(1 + GV_ColComments.Items.Count, new Exception("执行更新 表描述过程中，出现异常！（" + LabCurrTabName.Content.ToString() + "） "));
                        return;
                    }
                }
                else
                {
                    bgWork.ReportProgress(1 + GV_ColComments.Items.Count, new Exception("执行更新 表描述过程中，出现未知异常！（" + LabCurrTabName.Content.ToString() + "） "));
                    return;
                }

                for (int j = 0; j < GV_ColComments.Items.Count - 1; j++)
                {
                    Display info = GV_ColComments.Items[j] as Display;
                    string columnName = info.ColumnName;
                    string colComment = info.DeText;
                    if (!string.IsNullOrEmpty(colComment))
                    {
                        blRes = DBUtils.Instance?.Info?.SetColumnComment(LabCurrTabName.Content.ToString(), columnName, colComment);
                    }

                    if (string.IsNullOrEmpty(colComment))
                    {
                        bgWork.ReportProgress(1 + (1 + j));
                    }
                    else
                    {
                        if (blRes.HasValue)
                        {
                            if (blRes.Value)
                            {
                                bgWork.ReportProgress(1 + (1 + j));
                            }
                            else
                            {
                                bgWork.ReportProgress(1 + GV_ColComments.Items.Count, new Exception("执行更新 列描述过程中，出现异常！（" + columnName + "） "));
                                return;
                            }
                        }
                        else
                        {
                            bgWork.ReportProgress(1 + GV_ColComments.Items.Count, new Exception("执行更新 列描述过程中，出现未知异常！（" + columnName + "）  "));
                            return;
                        }
                    }
                }

            }, 1 + GV_ColComments.Items.Count);

            bgWork.RunWorkerAsync();
        }
        /// <summary>
        /// 数据连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbConnect_Click(object sender, RoutedEventArgs e)
        {
            lblTip.Content = string.Empty;
            InitMain();
            System.Windows.MessageBox.Show("已完成！", "提示", MessageBoxButton.OK);
        }
        /// <summary>
        /// 重新获取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbRefresh_Click(object sender, EventArgs e)
        {
            DBUtils.Instance?.Info?.Refresh();
            TextBox_TextChanged(sender, e);

            // TODO 重置
            this.Check.IsChecked = false;
            this.Number.Content = string.Format(selectedTableDesc, 0);
            System.Windows.MessageBox.Show("已完成！", "提示", MessageBoxButton.OK);
        }
        /// <summary>
        /// pdm上传
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbPDMUpload_Click(object sender, EventArgs e)
        {
            ImportForm pdmForm = new ImportForm();
            bool? dirRes = pdmForm.ShowDialog();
            if (dirRes == true || FormUtils.IsOK_Close)
            {
                FormUtils.IsOK_Close = false;
                TextBox_TextChanged(sender, e);
            }
        }

        /// <summary>
        /// chm文件根路径
        /// </summary>
        private string dirPath = string.Empty;
        /// <summary>
        /// CHM 文件绝对路径
        /// </summary>
        private string chm_path = string.Empty;
        /// <summary>
        /// 默认html
        /// </summary>
        private string defaultHtml = "数据库表目录.html";
        /// <summary>
        /// 索引文件路径
        /// </summary>
        private string indexHtmlpath = string.Empty;

        #region chm文档导出
        /// <summary>
        /// chm文档导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbBuild_Click(object sender, EventArgs e)
        {
            // TODO 导出chm
            ExportToChm();
        }

        private void ExportToChm()
        {
            #region 使用 HTML Help Workshop 的 hhc.exe 编译 ,先判断系统中是否已经安装有  HTML Help Workshop 

            string hhcPath = string.Empty;

            if (!ConfigUtils.CheckInstall("HTML Help Workshop", "hhc.exe", out hhcPath))
            {
                string htmlhelpPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "htmlhelp.exe");
                if (File.Exists(htmlhelpPath))
                {
                    if (System.Windows.MessageBox.Show("导出CHM文档需安装 HTML Help Workshop ，是否现在安装？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Asterisk) == MessageBoxResult.OK)
                    {
                        var proc = Process.Start(htmlhelpPath);
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("导出CHM文档需安装 HTML Help Workshop ，您未安装 HTML Help Workshop！", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    return;
                }
            }

            #endregion


            SaveFileDialog saveDia = new SaveFileDialog();
            saveDia.Filter = "(*.chm)|*.chm";
            saveDia.Title = "另存文件为";
            saveDia.CheckPathExists = true;
            saveDia.AddExtension = true;
            saveDia.AutoUpgradeEnabled = true;
            saveDia.DefaultExt = ".chm";
            saveDia.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveDia.OverwritePrompt = true;
            saveDia.ValidateNames = true;
            saveDia.FileName = DBUtils.Instance.Info.DBName + "表结构信息.chm";
            if (saveDia.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                chm_path = saveDia.FileName;

                System.Diagnostics.Process process;
                if (IsExistProcess(System.IO.Path.GetFileName(saveDia.FileName), out process))
                {
                    var dia = System.Windows.MessageBox.Show("文件已打开，导出前需关闭，是否继续？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                    if (dia == MessageBoxResult.OK)
                    {
                        process.Kill();
                    }
                }

                try
                {
                    //创建临时文件夹,存在则删除，防止已经存在的文件 会导致生成出来的chm 有问题
                    dirPath = Path.Combine(ConfigUtils.AppPath, DBUtils.Instance.DBType + "_" + DBUtils.Instance.Info.DBName);
                    if (ZetaLongPaths.ZlpIOHelper.DirectoryExists(dirPath))
                    {
                        ZetaLongPaths.ZlpIOHelper.DeleteDirectory(dirPath, true);
                    }
                    ZetaLongPaths.ZlpIOHelper.CreateDirectory(dirPath);
                    ConfigUtils.AddSecurityControll2Folder(dirPath);
                }
                catch (Exception ex)
                {
                    LogUtils.LogError("文件目录创建出错", Developer.SysDefault, ex, dirPath);
                    System.Windows.MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                //设置要 进度条 对应的执行方法，以及进度条最大值
                FormUtils.ProgArg = new ProgressArg(() =>
                {
                    try
                    {
                        List<TableDto> tableDtos = DBInstanceTransToDto();

                        //生成数据库目录文件
                        indexHtmlpath = Path.Combine(dirPath, defaultHtml);
                        //ChmHtmlHelper.CreateDirHtml("数据库表目录", DBUtils.Instance.Info.TableComments, indexHtmlpath);
                        ChmHtmlHelper.CreateDirHtml("数据库表目录", tableDtos, indexHtmlpath);

                        string structPath = Path.Combine(dirPath, "表结构");
                        if (!ZetaLongPaths.ZlpIOHelper.DirectoryExists(structPath))
                        {
                            ZetaLongPaths.ZlpIOHelper.CreateDirectory(structPath);
                        }

                        bgWork.ReportProgress(2);

                        //生成每张表列结构的html
                        //ChmHtmlHelper.CreateHtml(DBUtils.Instance.Info.TableInfoDict, structPath);
                        ChmHtmlHelper.CreateHtml(tableDtos, structPath);

                        bgWork.ReportProgress(3);

                        ChmHelp c3 = new ChmHelp();
                        c3.HHCPath = hhcPath;
                        c3.DefaultPage = defaultHtml;
                        c3.Title = Path.GetFileName(chm_path);
                        c3.ChmFileName = chm_path;
                        c3.SourcePath = dirPath;
                        c3.Compile();

                        bgWork.ReportProgress(4);

                        if (System.Windows.MessageBox.Show("生成CHM文档成功，是否打开？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                        {
                            Process.Start(saveDia.FileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogUtils.LogError("DBCHM执行出错", Developer.MJ, ex);
                        bgWork.ReportProgress(4, ex);
                    }

                }, 4);

                bgWork.RunWorkerAsync();
            }
        }

        private List<TableDto> DBInstanceTransToDto()
        {
            List<TableDto> tables = new System.Collections.Generic.List<TableDto>();

            // TODO 查询数据库表集合
            System.Collections.Specialized.NameValueCollection dict_tabs = DBUtils.Instance.Info.TableComments;

            // TODO 根据选择的表名进行导出相关处理
            List<string> checkedTableNames = new List<string>();
            list.Sort();
            foreach (var item in checedkListBox.Items)
            {
                IsCheck check = item as IsCheck;
                string Name = list.Find(h => h == check.Table_Name);
                if (!string.IsNullOrEmpty(Name))
                {
                    // TODO 选中的表名
                    string checkedTableName = Name;
                    checkedTableNames.Add(checkedTableName);
                }
            }


            //foreach (var item in list)
            //{
            //    if (checedkListBox.Items.Contains(item) == true)
            //    {
            //        // TODO 选中的表名
            //        string checkedTableName = item;
            //        checkedTableNames.Add(checkedTableName);
            //    }
            //}
            if (checkedTableNames.Count == 0)
            {
                //TODO 未选择指定表，默认全部表处理
                for (var j = 0; j < checedkListBox.Items.Count; j++)
                {
                    IsCheck check = checedkListBox.Items[j] as IsCheck;
                    string checkedTableName = check.Table_Name;
                    checkedTableNames.Add(checkedTableName);
                }
            }

            int i = 1; // 计数器
            foreach (var tableName in checkedTableNames)
            {
                TableDto tableDto = new TableDto();

                tableDto.TableOrder = i + ""; // 序号
                tableDto.TableName = tableName; // 表名
                tableDto.Comment = (!string.IsNullOrWhiteSpace(dict_tabs[tableName]) ? dict_tabs[tableName] : ""); // 表注释（说明）

                // TODO 查询数据库表字段集合
                var columns = new System.Collections.Generic.List<ColumnDto>();
                var dictTabs = DBUtils.Instance.Info.TableInfoDict;
                MJTop.Data.TableInfo tabInfo = null;
                if (dictTabs.Case == MJTop.Data.KeyCase.Lower)
                {
                    tabInfo = dictTabs[tableName.ToLower()];
                }
                else
                {
                    tabInfo = dictTabs[tableName.ToUpper()];
                }
                // TODO 添加数据字段行,循环数据库表字段集合
                foreach (var col in tabInfo.Colnumns)
                {
                    ColumnDto columnDto = new ColumnDto();

                    columnDto.ColumnOrder = col.Colorder + ""; // 序号
                    columnDto.ColumnName = col.ColumnName; // 列名
                    columnDto.ColumnTypeName = col.TypeName; // 数据类型
                    columnDto.Length = (col.Length.HasValue ? col.Length.Value.ToString() : ""); // 长度
                    columnDto.Scale = (col.Scale.HasValue ? col.Scale.Value.ToString() : ""); // 小数位
                    columnDto.IsPK = (col.IsPK ? "√" : ""); // 主键
                    columnDto.IsIdentity = (col.IsIdentity ? "√" : ""); // 自增
                    columnDto.CanNull = (col.CanNull ? "√" : ""); // 允许空
                    columnDto.DefaultVal = (!string.IsNullOrWhiteSpace(col.DefaultVal) ? col.DefaultVal : ""); // 默认值
                    columnDto.Comment = (!string.IsNullOrWhiteSpace(col.DeText) ? col.DeText : ""); // 列注释（说明）

                    columns.Add(columnDto);
                }
                tableDto.Columns = columns; // 数据库表字段集合赋值

                tables.Add(tableDto);

                i++;
            }
            return tables;
        }

        #endregion

        public bool IsExistProcess(string fileName, out Process process)
        {
            var procs = Process.GetProcessesByName("hh");
            foreach (var proc in procs)
            {
                if (proc.MainWindowTitle.Equals(fileName))
                {
                    process = proc;
                    return true;
                }
            }
            process = null;
            return false;
        }

        #region word文档导出
        /// <summary>
        /// word文档导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsWordExp_Click(object sender, RoutedEventArgs e)
        {
            // TODO 导出word
            ExportToWord();
        }

        private void ExportToWord()
        {
            #region 引用Microsoft.Office.Interop.Word.dll导出word文档方法弃用，改为引用Aspose.Words.dll方法导出word文档
            //FormUtils.ShowProcessing("正在导出数据字典Word文档，请稍等......", this, arg =>
            //{
            //    try
            //    {
            //        System.Collections.Generic.List<TableDto> tableDtos = DBInstanceTransToDto();
            //        TryOpenXml.Text.WordUtils.ExportWordByMicrosoftOfficeInteropWord(DBUtils.Instance.Info.DBName, tableDtos);

            //        MessageBox.Show("生成数据库字典Word文档成功！", "操作提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            //    }
            //    catch (Exception ex)
            //    {
            //        LogUtils.LogError("DBCHM执行出错", Developer.MJ, ex);
            //    }

            //}, null); 
            #endregion

            string fileName = string.Empty;
            SaveFileDialog saveDia = new SaveFileDialog();
            saveDia.Filter = "Word files (*.doc)|*.doc";
            saveDia.Title = "另存文件为";
            saveDia.CheckPathExists = true;
            saveDia.AddExtension = true;
            saveDia.AutoUpgradeEnabled = true;
            saveDia.DefaultExt = ".doc";
            saveDia.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveDia.OverwritePrompt = true;
            saveDia.ValidateNames = true;
            saveDia.FileName = DBUtils.Instance.Info.DBName + "表结构信息.doc";
            if (saveDia.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //System.Diagnostics.Process process;
                //if (IsExistProcess(Path.GetFileName(saveDia.FileName), out process))
                //{
                //    var dia = MessageBox.Show("文件已打开，导出前需关闭，是否继续？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                //    if (dia == DialogResult.OK)
                //    {
                //        process.Kill();
                //    }
                //}

                try
                {
                    System.Collections.Generic.List<TableDto> tableDtos = DBInstanceTransToDto();
                    TryOpenXml.Text.WordUtils.ExportWordByAsposeWords(saveDia.FileName, DBUtils.Instance.Info.DBName, tableDtos);

                    if (System.Windows.MessageBox.Show("生成数据库字典Word文档成功，是否打开？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        Process.Start(saveDia.FileName);
                    }
                }
                catch (Exception ex)
                {
                    LogUtils.LogError("DBCHM执行出错", Developer.MJ, ex);
                }
            }
        }
        #endregion

        #region excel文档导出
        /// <summary>
        /// excel文档导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsExcelExp_Click(object sender, RoutedEventArgs e)
        {
            // TODO 导出excel
            ExportToExcel();
        }

        private void ExportToExcel()
        {
            string fileName = string.Empty;
            SaveFileDialog saveDia = new SaveFileDialog();
            saveDia.Filter = "Excel files (*.xlsx)|*.xlsx";
            saveDia.Title = "另存文件为";
            saveDia.CheckPathExists = true;
            saveDia.AddExtension = true;
            saveDia.AutoUpgradeEnabled = true;
            saveDia.DefaultExt = ".xlsx";
            saveDia.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveDia.OverwritePrompt = true;
            saveDia.ValidateNames = true;
            saveDia.FileName = DBUtils.Instance.Info.DBName + "表结构信息.xlsx";
            if (saveDia.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //System.Diagnostics.Process process;
                //if (IsExistProcess(Path.GetFileName(saveDia.FileName), out process))
                //{
                //    var dia = MessageBox.Show("文件已打开，导出前需关闭，是否继续？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                //    if (dia == DialogResult.OK)
                //    {
                //        process.Kill();
                //    }
                //}

                try
                {
                    List<TableDto> tableDtos = DBInstanceTransToDto();
                    TryOpenXml.Text.ExcelUtils.ExportExcelByEpplus(saveDia.FileName, DBUtils.Instance.Info.DBName, tableDtos);

                    if (System.Windows.MessageBox.Show("生成数据库字典Excel文档成功，是否打开？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        Process.Start(saveDia.FileName);
                    }
                }
                catch (Exception ex)
                {
                    LogUtils.LogError("DBCHM执行出错", Developer.MJ, ex);
                }
            }
        }
        #endregion

        #region pdf文档导出
        /// <summary>
        /// pdf文档导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsPdfExp_Click(object sender, RoutedEventArgs e)
        {
            // TODO 导出pdf
            ExportToPdf();
        }

        private void ExportToPdf()
        {
            string fileName = string.Empty;
            SaveFileDialog saveDia = new SaveFileDialog();
            saveDia.Filter = "Text documents (.pdf)|*.pdf";
            saveDia.Title = "另存文件为";
            saveDia.CheckPathExists = true;
            saveDia.AddExtension = true;
            saveDia.AutoUpgradeEnabled = true;
            saveDia.DefaultExt = ".pdf";
            saveDia.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveDia.OverwritePrompt = true;
            saveDia.ValidateNames = true;
            saveDia.FileName = DBUtils.Instance.Info.DBName + "表结构信息.pdf";
            if (saveDia.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //System.Diagnostics.Process process;
                //if (IsExistProcess(Path.GetFileName(saveDia.FileName), out process))
                //{
                //    var dia = MessageBox.Show("文件已打开，导出前需关闭，是否继续？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                //    if (dia == DialogResult.OK)
                //    {
                //        process.Kill();
                //    }
                //}

                try
                {
                    // TODO 中文ttf字体库文件（微软雅黑）
                    string baseFontPath = System.Windows.Forms.Application.StartupPath + "\\Fonts\\msyh.ttf";
                    System.Collections.Generic.List<TableDto> tableDtos = DBInstanceTransToDto();
                    TryOpenXml.Text.PdfUtils.ExportPdfByITextSharp(saveDia.FileName, baseFontPath, DBUtils.Instance.Info.DBName, tableDtos);

                    if (System.Windows.MessageBox.Show("生成数据库字典PDF文档成功，是否打开？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        Process.Start(saveDia.FileName);
                    }
                }
                catch (Exception ex)
                {
                    LogUtils.LogError("DBCHM执行出错", Developer.MJ, ex);
                }
            }
        }
        #endregion

        #region html文档导出
        /// <summary>
        /// html文档导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsHtmlExp_Click(object sender, RoutedEventArgs e)
        {
            ExportToHtml();
        }

        private void ExportToHtml()
        {
            string fileName = string.Empty;
            SaveFileDialog saveDia = new SaveFileDialog();
            saveDia.Filter = "html files (*.html)|*.html";
            saveDia.Title = "另存文件为";
            saveDia.CheckPathExists = true;
            saveDia.AddExtension = true;
            saveDia.AutoUpgradeEnabled = true;
            saveDia.DefaultExt = ".html";
            saveDia.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveDia.OverwritePrompt = true;
            saveDia.ValidateNames = true;
            saveDia.FileName = DBUtils.Instance.Info.DBName + "表结构信息.html";
            if (saveDia.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    System.Collections.Generic.List<TableDto> tableDtos = DBInstanceTransToDto();
                    TryOpenXml.Text.HtmlUtils.ExportHtml(saveDia.FileName, DBUtils.Instance.Info.DBName, tableDtos);

                    if (System.Windows.MessageBox.Show("生成数据库字典HTML文档成功，是否打开？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        System.Diagnostics.Process.Start(saveDia.FileName);
                    }
                }
                catch (Exception ex)
                {
                    LogUtils.LogError("DBCHM执行出错", Developer.MJ, ex);
                }
            }
        }
        #endregion

        #region xml文档导出
        /// <summary>
        /// xml文档导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsXmlExp_Click(object sender, RoutedEventArgs e)
        {
            // TODO 导出xml
            ExportToXml();
        }

        private void ExportToXml()
        {
            string fileName = string.Empty;
            SaveFileDialog saveDia = new SaveFileDialog();
            saveDia.Filter = "XML files (*.xml)|*.xml";
            saveDia.Title = "另存文件为";
            saveDia.CheckPathExists = true;
            saveDia.AddExtension = true;
            saveDia.AutoUpgradeEnabled = true;
            saveDia.DefaultExt = ".xml";
            saveDia.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveDia.OverwritePrompt = true;
            saveDia.ValidateNames = true;
            saveDia.FileName = DBUtils.Instance.Info.DBName + "表结构信息.xml";
            if (saveDia.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    System.Collections.Generic.List<TableDto> tableDtos = DBInstanceTransToDto();
                    TryOpenXml.Text.XmlUtils.ExportXml(saveDia.FileName, DBUtils.Instance.Info.DBName, tableDtos);

                    if (System.Windows.MessageBox.Show("生成数据库字典XML文档成功，是否打开？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        System.Diagnostics.Process.Start(saveDia.FileName);
                    }
                }
                catch (Exception ex)
                {
                    LogUtils.LogError("DBCHM执行出错", Developer.MJ, ex);
                }
            }
        }
        #endregion

        #region
        /// <summary>
        /// md导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsMarkDownExp_Click(object sender, RoutedEventArgs e)
        {
            ExportToMarkDown();
        }

        private void ExportToMarkDown()

        {
            string fileName = string.Empty;
            SaveFileDialog saveDia = new SaveFileDialog();
            saveDia.Filter = "markdown files (*.md)|*.md";
            saveDia.Title = "另存文件为";
            saveDia.CheckPathExists = true;
            saveDia.AddExtension = true;
            saveDia.AutoUpgradeEnabled = true;
            saveDia.DefaultExt = ".md";
            saveDia.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveDia.OverwritePrompt = true;
            saveDia.ValidateNames = true;
            saveDia.FileName = DBUtils.Instance.Info.DBName + "表结构信息.md";
            if (saveDia.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    List<TableDto> tableDtos = DBInstanceTransToDto();

                    TryOpenXml.Text.MarkDownUtils.Export(saveDia.FileName, DBUtils.Instance.Info.DBName, tableDtos);

                    if (System.Windows.MessageBox.Show("生成数据库字典markdown文档成功，是否打开？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        Process.Start(saveDia.FileName);
                    }
                }
                catch (Exception ex)
                {
                    LogUtils.LogError("DBCHM执行出错", Developer.MJ, ex);
                }
            }
        }
        #endregion

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            this.TxtTabName.Text = string.Empty;
            System.Windows.Controls.CheckBox ck = sender as System.Windows.Controls.CheckBox;
            if (ck.IsChecked == true)
            {
                list.Clear();
                List<IsCheck> isc = new List<IsCheck>();
                foreach (var item in checedkListBox.Items)
                {
                    IsCheck check = item as IsCheck;
                    list.Add(check.Table_Name);
                    IsCheck ic = new IsCheck
                    {
                        Table_Name = check.Table_Name,
                        IsChecked = true
                    };
                    isc.Add(ic);
                }
                checedkListBox.ItemsSource = null;
                checedkListBox.ItemsSource = isc;
            }
            else
            {
                list.Clear();
                List<IsCheck> isc = new List<IsCheck>();
                foreach (var item in checedkListBox.Items)
                {
                    IsCheck check = item as IsCheck;
                    IsCheck ic = new IsCheck
                    {
                        Table_Name = check.Table_Name,
                        IsChecked = false
                    };
                    isc.Add(ic);
                }
                checedkListBox.ItemsSource = null;
                checedkListBox.ItemsSource = isc;
            }
            this.Number.Content = string.Format(selectedTableDesc, list.Count);
        }


        /// <summary>
        /// 关于
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutBox box = new AboutBox();
            box.ShowDialog();
        }

        private void checedkListBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
                e.Handled = true;
        }
    }
}
