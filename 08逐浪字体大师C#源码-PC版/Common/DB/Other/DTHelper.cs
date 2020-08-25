using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ZoomLa.SQLDAL.Other
{
    public class DTHelper
    {
        /// <summary>
        /// DataTable分页
        /// </summary>
        /// <param name="dt">需分页的DataTable</param>
        /// <returns>需要显示的数据</returns>
        public static DataTable PageDT(DataTable dt, int cpage, int psize)
        {
            DataTable result = dt.Clone();
            if (psize < 1) psize = 10;
            if (cpage < 1) cpage = 1;
            int pageStart = psize * (cpage - 1);//0
            int pageEnd = psize * cpage;//10
            if (pageStart > dt.Rows.Count)
            {
                return result;
            }
            for (int i = pageStart; i < pageEnd && i < dt.Rows.Count; i++)
            {
                DataRow dr = result.NewRow();
                foreach (DataColumn dc in dt.Columns)
                {
                    dr[dc.ColumnName] = dt.Rows[i][dc.ColumnName];
                }
                result.Rows.Add(dr);
            }
            return result;
        }
        #region XML序列化与反序列化
        /// <summary>
        /// 将DataTable序列化为XML字符串返回,必须有TableName
        /// </summary> 
        /// <param name="pDt">需要序列化的DataTable</param> 
        /// <returns>序列化的DataTable</returns> 
        public static string SerializeDT(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(sb);
            XmlSerializer serializer = new XmlSerializer(typeof(DataTable));
            serializer.Serialize(writer, dt);
            XmlDocument xmlDoc = new XmlDocument();
            writer.Close();
            return sb.ToString();
        }
        /// <summary>
        ///  DataTable序列化,XML持久化存在本地,必须有TableName
        /// (推荐使用Json)
        /// 1,DataTable修改后存XML体积会加倍,比JSON大10倍
        /// 2,普通状态下也比Json大5倍
        /// </summary>
        /// <param name="dt">需要序列化的DataTable</param>
        /// <param name="path">物理路径</param>
        public static string SerializeDT(DataTable dt, string path)
        {
            if (path.StartsWith("/")) { throw new Exception("请指定物理路径"); }
            StringBuilder sb = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(sb);
            XmlSerializer serializer = new XmlSerializer(typeof(DataTable));
            serializer.Serialize(writer, dt);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(sb.ToString());//如果需要持久化存储,可如此
            xmlDoc.Save(path);
            writer.Close();
            return sb.ToString();
        }
        /// <summary> 
        /// 反序列化DataTable 
        /// </summary> 
        /// <param name="xmlStr">Xml字符串</param> 
        /// <returns>DataTable</returns> 
        public static DataTable DeserializeFromXML(string xmlStr)
        {
            StringReader strReader = new StringReader(xmlStr);
            XmlReader xmlReader = XmlReader.Create(strReader);
            XmlSerializer serializer = new XmlSerializer(typeof(DataTable));
            DataTable dt = serializer.Deserialize(xmlReader) as DataTable;
            return dt;
        }
        /// <summary> 
        /// 从物理路径反序列化DataTable 
        /// </summary> 
        /// <param name="xmlPath">序列化的DataTable的物理路径</param> 
        /// <returns>DataTable</returns> 
        public static DataTable DeserializeFromPath(string xmlPPath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPPath);
            return DeserializeFromXML(xmlDoc.InnerXml);
        }
        #endregion
    }
}
