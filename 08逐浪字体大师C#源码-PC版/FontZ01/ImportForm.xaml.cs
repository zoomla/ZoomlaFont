using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml;
using TryOpenXml.Dtos;
using TryOpenXml.Text;
using FontZ01.Commons;

namespace FontZ01
{
    /// <summary>
    /// ImportForm.xaml 的交互逻辑
    /// </summary>
    public partial class ImportForm : Window
    {
        public ImportForm()
        {
            InitializeComponent();
        }

        private void BtnBrow_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialo = new OpenFileDialog
            {
                //支持 pdm，实体类注释产生的xml文档，dbchm导出的xml文件
                Filter = "支持文件类型(*.pdm;*.xml)|*.pdm;*.xml|PowerDesigner文件(*.pdm)|*.pdm|xml文件|*.xml",
                Multiselect = true
            };
            if (openFileDialo.ShowDialog() == true)
            {
                txtMulItem.Text = string.Join("\r\n", openFileDialo.FileNames);
            }
        }

        private void BtnUpdateDisplayName_Click(object sender, RoutedEventArgs e)
        {
            if (DBUtils.Instance == null)
            {
                MessageBox.Show("更新批注，需连接数据库，请切换到要更新批注的数据库！", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtMulItem.Text))
            {
                MessageBox.Show("请先选择批注数据文件！", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return;
            }

            string[] paths = txtMulItem.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string ph in paths)
            {
                string extName = System.IO.Path.GetExtension(ph).ToLower();
                try
                {
                    if (File.Exists(ph))
                    {
                        if (extName == ".pdm")
                        {
                            UpdateCommentByPDM(ph);
                        }
                        else if (extName == ".xml")
                        {
                            UpdateCommentByXML(ph);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "出错", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
            }

            MessageBox.Show("更新表列批注完成！", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            FormUtils.IsOK_Close = true;
            this.Close();

        }

        void UpdateCommentByXML(string path)

        {
            var xmlContent = File.ReadAllText(path, Encoding.UTF8);
            if (xmlContent.Contains("ArrayOfTableDto"))
            {
                //通过 dbchm 导出的 XML文件 来更新 表列批注

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlContent);

                var dbName = doc.DocumentElement.GetAttribute("databaseName");

                if (!DBUtils.Instance.Info.DBName.Equals(dbName, StringComparison.OrdinalIgnoreCase))
                {
                    if (MessageBox.Show("检测到数据库名称不一致，确定要继续吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.Cancel)
                    {
                        return;
                    }
                }

                var lstDTO = XmlUtils.Deserialize(typeof(List<TableDto>), xmlContent) as List<TableDto>;

                foreach (var tabInfo in lstDTO)
                {
                    if (DBUtils.Instance.Info.IsExistTable(tabInfo.TableName) && !string.IsNullOrWhiteSpace(tabInfo.Comment))
                    {
                        DBUtils.Instance.Info.SetTableComment(tabInfo.TableName, tabInfo.Comment);
                    }

                    foreach (var colInfo in tabInfo.Columns)
                    {
                        if (DBUtils.Instance.Info.IsExistColumn(tabInfo.TableName, colInfo.ColumnName) && !string.IsNullOrWhiteSpace(colInfo.Comment))
                        {
                            DBUtils.Instance.Info.SetColumnComment(tabInfo.TableName, colInfo.ColumnName, colInfo.Comment);
                        }
                    }
                }
            }
            else
            {

                //通过 有 VS 生成的 实体类库 XML文档文件 来更新 表列批注

                XmlAnalyze analyze = new XmlAnalyze(path);

                var data = analyze.Data;

                foreach (var item in data)
                {
                    if (DBUtils.Instance.Info.IsExistTable(item.Key.Key) && !string.IsNullOrWhiteSpace(item.Key.Value))
                    {
                        DBUtils.Instance.Info.SetTableComment(item.Key.Key, item.Key.Value);
                    }

                    foreach (var colKV in item.Value)
                    {
                        if (DBUtils.Instance.Info.IsExistColumn(item.Key.Key, colKV.Key) && !string.IsNullOrWhiteSpace(colKV.Value))
                        {
                            DBUtils.Instance.Info.SetColumnComment(item.Key.Key, colKV.Key, colKV.Value);
                        }
                    }
                }

            }
        }

        /// <summary>
        /// 通过pdm文件更新批注
        /// </summary>
        /// <param name="path"></param>
        void UpdateCommentByPDM(string path)
        {
            var lstTabs = GetTables(path);
            var dbInfo = DBUtils.Instance?.Info;
            foreach (var tab in lstTabs)
            {
                string tab_Comment = tab.Name;
                if (!string.IsNullOrWhiteSpace(tab_Comment)
                    && !tab.Code.Equals(tab_Comment, StringComparison.OrdinalIgnoreCase))
                {
                    dbInfo.SetTableComment(tab.Code, tab_Comment);
                }
                var lstCols = tab.Columns;
                foreach (var col in lstCols)
                {
                    string col_Comment = col.Name;
                    if (!string.IsNullOrWhiteSpace(col_Comment)
                        && !col.Code.Equals(col_Comment, StringComparison.OrdinalIgnoreCase))
                    {
                        dbInfo.SetColumnComment(tab.Code, col.Code, col_Comment);
                    }
                }
            }
        }

        static IList<Export.PdmModels.TableInfo> GetTables(params string[] pdmPaths)
        {
            List<Export.PdmModels.TableInfo> lstTables = new List<Export.PdmModels.TableInfo>();
            var pdmReader = new Export.PDM.PdmReader();
            foreach (string path in pdmPaths)
            {
                if (File.Exists(path))
                {
                    var models = pdmReader.ReadFromFile(path);
                    lstTables.AddRange(models.Tables);
                }
            }
            lstTables = lstTables.OrderBy(t => t.Code).ToList();
            return lstTables;
        }
    }
}
