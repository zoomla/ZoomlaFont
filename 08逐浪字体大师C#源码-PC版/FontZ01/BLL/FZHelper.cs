using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using Common.DB;
using Common.Helper;
using Common.MB;
using FontZ01.Commons;

namespace FontZ01.BLL
{
    //用XML中的信息操作后,需要重启FZ服务(能否通知FZ重读XML)
    public static class FZHelper
    {
        public static string xmlPath { get { return ConfigHelper.APPInfo.fzdir + @"\FileZilla Server.xml"; } }
        public static XmlDocument xmldoc = new XmlDocument();
        public static DataTable UserDT = null;
        public static DataTable GroupDT = null;
        //重新加载XML,更新信息后|初始化时
        public static void LoadXml()
        {
            xmldoc.Load(xmlPath);
            UserDT = User_Sel();
            GroupDT = Group_Sel();
        }
        //加载最新的配置文件
        public static void LoadNewstXml()
        {
            xmldoc.Load(xmlPath);
        }
        #region 用户组
        public static string[] Group_Field = ("Bypass server userlimit|User Limit|IP Limit|Enabled|Comments|ForceSsl|8plus3"
                   + "").Split('|');
        public static DataTable Group_Sel()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Name", typeof(string)));

            foreach (string field in User_fields)
            {
                dt.Columns.Add(new DataColumn(field, typeof(string)));
            }
            XmlNodeList nodes = xmldoc.SelectNodes("//FileZillaServer/Groups/Group");
            foreach (XmlNode user in nodes)
            {
                DataRow dr = dt.NewRow();
                dr["Name"] = user.Attributes["Name"].InnerText;
                foreach (string field in User_fields)
                {
                    dr[field] = GetNodeValue(user, "Option[@Name='" + field + "']");
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        #endregion
        #region 用户管理
        static B_CodeModel  userBll = new B_CodeModel("ZL_FZ_User");
        //需要更新入XML中的字段
        public static string[] User_fields = ("Pass|Group|Bypass server userlimit|User Limit|IP Limit|Enabled|Comments|ForceSsl|8plus3").Split('|');
        public static string[] User_Extfields = "Name|CDate|Enddate|SiteInfo|UserPwd".Split('|');
        public static DataTable User_Sel()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Name", typeof(string)));
            dt.Columns.Add(new DataColumn("Dir", typeof(string)));//用户目录信息(现仅需一个)
            foreach (string field in User_fields)
            {
                dt.Columns.Add(new DataColumn(field, typeof(string)));
            }
            XmlNodeList nodes = xmldoc.SelectNodes("//FileZillaServer/Users/User");
            foreach (XmlNode user in nodes)
            {
                DataRow dr = dt.NewRow();
                dr["Name"] = user.Attributes["Name"].InnerText;
                dr["Dir"] = "";
                XmlNode perNode = user.SelectSingleNode("Permissions/Permission");
                if (perNode != null) { dr["Dir"] = perNode.Attributes["Dir"].Value; }
                foreach (string field in User_fields)
                {
                    dr[field] = GetNodeValue(user, "Option[@Name='" + field + "']");
                }
                dt.Rows.Add(dr);
            }
            //再混合DataTable中的数据
            {
                foreach (string field in User_Extfields)
                {
                    if (!dt.Columns.Contains(field)) { dt.Columns.Add(new DataColumn(field, typeof(string))); }
                }
                DataTable extdt = SqlHelper.ExecuteTable("SELECT * FROM ZL_FZ_User");
                foreach (DataRow dr in extdt.Rows)
                {
                    DataRow[] drs = dt.Select("Name='" + dr["Name"] + "'");
                    if (drs.Length > 0)
                    {
                        foreach (string field in User_Extfields)
                        {
                            drs[0][field] = dr[field];
                        }
                    }
                }
            }//extdt end;
            return dt;
        }
        public static DataRow User_GetByName(string name)
        {
            name = name.Trim();
            DataRow[] drs = UserDT.Select("Name='" + name + "'");
            return drs.Length > 0 ? drs[0] : null;
        }
        //更新用户,不提供用户名修改功能
        public static void User_Update(DataRow dr)
        {
            //将值更新回xml
            LoadNewstXml();
            //--------------------
            XmlNode user = User_GetNode(dr["Name"].ToString());
            if (user == null) { throw new Exception("用户[" + dr["Name"] + "]不存在"); }
            foreach (string field in User_fields)
            {
                if (dr.Table.Columns.Contains(field))
                {
                    XmlNode node = user.SelectSingleNode("Option[@Name='" + field + "']");
                    user.SelectSingleNode("Option[@Name='" + field + "']").InnerText = dr[field].ToString();
                }
            }
            {
                //目录权限
                XmlNode perNode = user.SelectSingleNode("Permissions/Permission");
                perNode.Attributes["Dir"].Value = dr["Dir"].ToString();
            }
            xmldoc.Save(xmlPath);
            //--------------------
            //有可能XML中数据,而数据库中无数据,所以判断是插入还是更新
            DataTable extdt = User_BeforeUR(dr.Table);
            string name=extdt.Rows[0]["Name"].ToString();
            if (userBll.IsExist("Name", name)) { userBll.UpdateByID(extdt.Rows[0], "Name"); }
            else { userBll.Insert(extdt.Rows[0]); }
            LoadXml();
        }
        /// <summary>
        /// 添加新用户,用户名不能同名,不能为空,其他不限
        /// </summary>
        public static void User_Add(DataTable dt)
        {
            LoadNewstXml();
            {
                XmlNode users = xmldoc.SelectSingleNode("//FileZillaServer/Users");
                XmlNode user = User_DRToNode(dt.Rows[0]);
                if (user == null) { function.WriteErrMsg("用户信息为空"); return; }
                XmlNode node = xmldoc.ImportNode(user, true);
                users.AppendChild(node);
                xmldoc.Save(xmlPath);
            }
            //--------更新数据库信息
            {
                DataTable extdt = User_BeforeUR(dt);
                userBll.Insert(extdt.Rows[0]);
            }
            //--------重加载
            LoadXml();
        }
        public static bool User_Del(string name)
        {
            try
            {
                LoadNewstXml();
                XmlNode user = User_GetNode(name);
                if (user == null) { function.WriteErrMsg("用户[" + name + "]不存在"); return false; }
                XmlNode users = xmldoc.SelectSingleNode("//FileZillaServer/Users");
                users.RemoveChild(user);
                xmldoc.Save(xmlPath);
                //----------
                userBll.DelByWhere("Name=@name", new List<SqlParameter>() { new SqlParameter("name", name) });
                //----------
                LoadXml();
                return true;
            }
            catch (Exception ex) { function.WriteErrMsg(ex.Message); return false; }
        }
        public static XmlNode User_GetNode(string name)
        {
            XmlNode user = xmldoc.SelectSingleNode("//FileZillaServer/Users/User[@Name='" + name + "']");
            return user;
        }
        /// <summary>
        /// 更新时从FZ,新建时从XML,dr需要重命名
        /// </summary>
        private static XmlNode User_DRToNode(DataRow dr, string from = "model")
        {
            DataTable dt = dr.Table;
            if (dt.Columns.Contains("UserLimit")) { dt.Columns["UserLimit"].ColumnName = "User Limit"; }
            if (dt.Columns.Contains("IPLimit")) { dt.Columns["IPLimit"].ColumnName = "IP Limit"; }
            //dr中的值可能并不完全,所以需要从模型或目标文件中取值再修改
            XmlNode user = null;
            switch (from)
            {
                case "fz":
                    user = FZHelper.User_GetNode(dr["name"].ToString());
                    break;
                default:
                    user = ConfigHelper.Model_New("User");
                    break;
            }
            foreach (string field in User_fields)
            {
                if (dr.Table.Columns.Contains(field))
                {
                    user.SelectSingleNode("Option[@Name='" + field + "']").InnerText = DataConvert.CStr(dr[field]);
                }
                user.Attributes["Name"].Value = dr["Name"].ToString();
            }
            //用户目录处理
            {
                XmlNode perNode = user.SelectSingleNode("Permissions/Permission");
                perNode.Attributes["Dir"].Value = dr["Dir"].ToString();
            }
            return user;
        }
        //在插入数据库前对数据进行预处理,过滤字段,设定初始值等
        private static DataTable User_BeforeUR(DataTable dt)
        {
            foreach (string field in User_Extfields)
            {
                if (!dt.Columns.Contains(field)) { dt.Columns.Add(new DataColumn(field, typeof(string))); }
            }
            DataTable extdt = dt.DefaultView.ToTable(false, User_Extfields);
            DataRow dr = extdt.Rows[0];
            if (string.IsNullOrEmpty(DataConvert.CStr(dr["CDate"]))) { extdt.Rows[0]["CDate"] = DateTime.Now.ToString(); }
            if (string.IsNullOrEmpty(DataConvert.CStr(dr["Name"]))) { function.WriteErrMsg("数据未指定用户名,无法添加|更新"); return null; }
            return extdt;
        }
        #endregion
        #region 用户目录权限

        #endregion
        #region FZ操作
        public static void FZ_RestartServices()
        {
            //可交由服务中实现
            ServicesHelper.RestartService("FileZilla Server");
        }
        #endregion
        //获取节点下子节点中的值
        private static string GetNodeValue(XmlNode node, string xmlpath)
        {
            if (node == null) { return ""; }
            XmlNode n = node.SelectSingleNode(xmlpath);
            if (n == null) { return ""; }
            else { return n.InnerText; }
        }
        /// <summary>
        /// 为xml节点新增属性
        /// </summary>
        private static XmlAttribute CreateNodeAttribute(XmlDocument doc, String name, String value)
        {
            XmlAttribute attribute = doc.CreateAttribute(name, null);
            attribute.Value = value;
            return attribute;
        }



    }

}
