using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Common.MB
{
    /*
     * BLL层的静态公用方法类
     */ 
    public class BLLCommon
    {
        public static StringBuilder ueditorMin = new StringBuilder();//简洁版编辑器(不允许上传图片,附件)
        public static StringBuilder ueditorMinEx = new StringBuilder();//简洁版编辑器(带上传附件)
        public static StringBuilder ueditorMid = new StringBuilder();//标准版
        public static StringBuilder ueditorBar = new StringBuilder();//贴吧等使用,带表情,视图,多图片和代码
        public static StringBuilder ueditorNom = new StringBuilder();//聊天等地使用,带表情(无全屏)
        //public static StringBuilder ueFormula = new StringBuilder();//用于教师
        static BLLCommon() 
        {
            ueditorMin.AppendLine(" toolbars : [['FullScreen','Undo', 'Redo','Bold', 'Italic', 'NumberedList', 'BulletedList',   'Smiley', 'ShowBlocks', 'Maximize', 'underline', 'fontborder', 'strikethrough', '|', 'justifyleft', 'justifycenter', 'justifyright', 'justifyjustify']]");
            ueditorMinEx.AppendLine(" toolbars : [['FullScreen','Undo', 'Redo','Bold', 'Italic', 'NumberedList', 'BulletedList',   'Smiley', 'ShowBlocks', 'Maximize', 'underline', 'fontborder', 'strikethrough', '|', 'justifyleft', 'justifycenter', 'justifyright', 'justifyjustify','simpleupload','insertimage','attachment']]");
            ueditorMid.AppendLine("toolbars : [['fullscreen','|', 'undo', 'redo', '|',");
            ueditorMid.AppendLine("'bold', 'italic', 'underline', 'fontborder', 'strikethrough', 'superscript', 'subscript', 'removeformat', 'formatmatch', 'autotypeset', 'blockquote', 'pasteplain', '|', 'forecolor', 'backcolor', 'insertorderedlist', 'insertunorderedlist', 'selectall', 'cleardoc', '|',");
            ueditorMid.AppendLine("'rowspacingtop', 'rowspacingbottom', 'lineheight', '|',");
            ueditorMid.AppendLine(" 'customstyle', 'paragraph', 'fontfamily', 'fontsize', '|',");
            ueditorMid.AppendLine(" 'directionalityltr', 'directionalityrtl', 'indent', '|','simpleupload','insertimage','emotion','attachment','map',");
            ueditorMid.AppendLine(" 'justifyleft', 'justifycenter', 'justifyright', 'justifyjustify', '|', 'touppercase', 'tolowercase', '|',");
            ueditorMid.AppendLine(" 'link', 'unlink', 'anchor', '|', 'pagebreak','|','template', 'horizontal', 'date', 'time', '|', 'spechars', 'inserttable', 'deletetable', 'insertparagraphbeforetable', 'insertrow', 'deleterow', 'insertcol', 'deletecol', 'mergecells', 'mergeright', 'mergedown', 'splittocells', 'splittorows', 'splittocols', 'kityformula','|', 'print', 'preview', 'searchreplace']]");
            ueditorBar.AppendLine(" toolbars : [['FullScreen','Undo', 'Redo','Bold', 'Italic', 'NumberedList', 'BulletedList',   'Smiley', 'ShowBlocks', 'Maximize', 'underline', 'fontborder', 'strikethrough', '|', 'justifyleft', 'justifycenter', 'justifyright', 'justifyjustify','insertcode','simpleupload','insertimage','insertvideo','emotion','attachment','map']]");
            ueditorNom.AppendLine(" toolbars : [['Undo', 'Redo','Bold', 'Italic', 'NumberedList', 'BulletedList',   'Smiley', 'ShowBlocks', 'Maximize', 'underline', 'fontborder', 'strikethrough', '|', 'justifyleft', 'justifycenter', 'justifyright', 'justifyjustify','emotion']]");
            //-----
        }
        #region M_Base
        /// <summary>
        /// 获取字段窜
        /// </summary>
        public static string GetFields(M_Base model)
        {
            string str = string.Empty, PK = model.PK.ToLower();
            string[,] strArr =model.FieldList();
            for (int i = 0; i < strArr.GetLength(0); i++)
            {
                if (strArr[i, 0].ToLower() != PK)
                {
                    str += "[" + strArr[i, 0] + "],";
                }
            }
            return str.Substring(0, str.Length - 1);
        }
        /// <summary>
        /// 获取参数串
        /// </summary>
        public static string GetParas(M_Base model)
        {
            string str = string.Empty, PK = model.PK.ToLower();
            string[,] strArr = model.FieldList();
            for (int i = 0; i < strArr.GetLength(0); i++)
            {
                if (strArr[i, 0].ToLower() != PK)
                {
                    str += "@" + strArr[i, 0] + ",";
                }
            }
            return str.Substring(0, str.Length - 1);
        }
        /// <summary>
        /// 获取字段=参数(Update)
        /// </summary>
        public static string GetFieldAndPara(M_Base model)
        {
            string str = string.Empty, PK = model.PK.ToLower();
            string[,] strArr = model.FieldList();
            for (int i = 0; i < strArr.GetLength(0); i++)
            {
                if (strArr[i, 0].ToLower() != PK)
                {
                    str += "[" + strArr[i, 0] + "]=@" + strArr[i, 0] + ",";
                }
            }
            return str.Substring(0, str.Length - 1);
        }
        #endregion
        #region DataRow
        public static string GetParas(DataRow dr, string pk = "id")
        {
            DataTable dt = dr.Table;
            string str = string.Empty;
            foreach (DataColumn col in dt.Columns)
            {
                if (col.ColumnName.Equals(pk, StringComparison.CurrentCultureIgnoreCase)) continue;
                str += "@" + col.ColumnName + ",";
            }
            return str.TrimEnd(',');
        }
        public static string GetFields(DataRow dr, string pk = "id")
        {
            DataTable dt = dr.Table;
            string str = string.Empty;
            foreach (DataColumn col in dt.Columns)
            {
                if (col.ColumnName.Equals(pk, StringComparison.CurrentCultureIgnoreCase)) continue;
                str += "[" + col.ColumnName + "],";
            }
            return str.TrimEnd(',');
        }
        public static string GetFieldAndPara(DataRow dr, string pk = "id")
        {
            DataTable dt = dr.Table;
            string str = string.Empty;
            foreach (DataColumn col in dt.Columns)
            {
                if (col.ColumnName.Equals(pk, StringComparison.CurrentCultureIgnoreCase)) continue;
                str += "[" + col.ColumnName + "]=@" + col.ColumnName + ",";
            }
            return str.TrimEnd(',');
        }
        public static SqlParameter[] GetParameters(DataRow dr, string pk = "id")
        {
            DataTable dt = dr.Table;
            List<SqlParameter> listSP = new List<SqlParameter>();
            foreach (DataColumn col in dt.Columns)
            {
                if (col.ColumnName.Equals(pk, StringComparison.CurrentCultureIgnoreCase)) continue;
                listSP.Add(new SqlParameter(col.ColumnName, dr[col.ColumnName]));
            }
            return listSP.ToArray();
        }
        #endregion
        //-----
       
    }
    public class Com_Filter
    {
        public string rids = "";
        public string ids = "";
        public string uids = "";
        public string nids = "";//节点
        public string gids = "";//会员组
        public string status = "";
        public string type = "-100";
        public string skey = "";
        //关键词搜索(必须完全匹配)
        public string skey_match = "";
        public string uname = "";
        public string orderBy = "";
        public int storeId = -100;
        //因为父級是从0开始
        public int pid = -100;
        /// <summary>
        /// 是否包含回收站数据
        /// </summary>
        public bool isRecycle = false;
        //是否必须为已审核数据
        public bool isAudited = false;
        public string addon = "";
        public string mode = "";
        //开始与结束日期筛选
        public string sdate = "";
        public string edate = "";
    }
    public abstract class Base
    {
        public object GetValue(string fieldName)
        {
            return GetType().GetProperty(fieldName).GetValue(this);
        }
    }
    public class GlobalConfig : Base
    {
        public string ImgSavePath { get; set; }
        public bool Dev { get; set; }
    }

}
