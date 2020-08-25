using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontZ01.Commons
{

    public class Display
    {
        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 字段长度
        /// </summary>
        public long? Length { get; set; }
        /// <summary>
        /// 列说明
        /// </summary>
        public string DeText { get; set; }
    }

    public class IsCheck
    {
        public string Table_Name { get; set; }
        public bool? IsChecked { get; set; }
    }
}
