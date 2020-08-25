using System;
using System.Data;
using System.Xml;
using System.Data.SqlClient;
using System.Collections;
using System.Configuration;
using System.Web;
using System.Collections.Generic;
using System.Diagnostics;
using ZoomLa.SQLDAL.SQL;
using System.Data.Common;

namespace ZoomLa.SQLDAL
{
    public sealed class SqlHelper
    {
        /// <summary>
        /// 联接查询,统一使用左连接
        /// </summary>
        /// <param name="fields">字段名,主表为A,次表为B</param>
        /// <param name="mtable">主表名</param>
        /// <param name="stable">次表名</param>
        public static DataTable JoinQuery(string fields, string mtable, string stable, string on, string where = "", string order = "", SqlParameter[] sp = null)
        {
            string sql = "SELECT {0} FROM {1} A LEFT JOIN {2} B ON {3} ";
            if (!string.IsNullOrEmpty(where))
            {
                sql += " WHERE " + where;
            }
            if (!string.IsNullOrEmpty(order))
            {
                if (!order.ToUpper().Contains("ORDER BY ")) { order = " ORDER BY " + order; }
                sql += order;
            }
            sql = string.Format(sql, fields, mtable, stable, on);
            return ExecuteTable(CommandType.Text, sql, sp);
        }
        public static int ObjectToInt32(object o) { return DataConvert.CLng(o); }
        public static DataSet ExecuteDataSet(CommandType type,string sql,SqlParameter[] sp=null)
        {
            DataTable dt = DBCenter.DB.ExecuteTable(new SqlModel(sql, sp));
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            return ds;
        }
        public static object ExecuteScalar(CommandType type,string sql,SqlParameter[] sp=null)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            if (sp != null) { list.AddRange(sp); }
            return DBCenter.DB.ExecuteScalar(new SqlModel(sql,list.ToArray()));
        }
        public static DbDataReader ExecuteReader(CommandType type,string cmd,SqlParameter[] sp = null)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            if (sp != null) { list.AddRange(sp); }
            return DBCenter.DB.ExecuteReader(new SqlModel(cmd,list.ToArray()));
        }
        public static DataTable ExecuteTable(string cmd, SqlParameter[] sp = null)
        {
            return ExecuteTable(CommandType.Text,cmd,sp);
        }
        public static DataTable ExecuteTable(CommandType type,string cmd,SqlParameter[] sp = null)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            if (sp != null) { list.AddRange(sp); }
            return DBCenter.DB.ExecuteTable(new SqlModel(cmd,sp));
        }
        public static int ExecuteNonQuery(CommandType type, string cmd, SqlParameter[] sp = null)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            if (sp != null) { list.AddRange(sp); }
            DBCenter.DB.ExecuteNonQuery(new SqlModel(cmd, sp));
            return 1;
        }
        public static bool ExecuteSql(string cmd, SqlParameter[] sp=null)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            if (sp != null) { list.AddRange(sp); }
            DBCenter.DB.ExecuteNonQuery(new SqlModel(cmd, sp));
            return true;
        }

        #region old
        public static string GetTablecolumn(string tablename, string columnname)
        {
            string ConnectionString = DBCenter.DB.ConnectionString;
            string[] connect1 = ConnectionString.Split(new string[] { "Initial Catalog=" }, StringSplitOptions.RemoveEmptyEntries);
            string[] connect = connect1[1].Split(new string[] { ";User ID=" }, StringSplitOptions.RemoveEmptyEntries);
            string dataname = connect[0];

            string truetablename = "";
            string truedataname = "";
            string connstr = "";

            if (tablename.IndexOf(".") > -1)
            {
                string[] columnlist = tablename.Split(new string[] { "." }, StringSplitOptions.None);
                if (columnlist.Length == 3)
                {
                    truetablename = columnlist[columnlist.Length - 1];
                    truedataname = columnlist[0];
                }
                if (truedataname == dataname)
                {
                    connstr = ConnectionString;
                }
            }
            else
            {
                connstr = ConnectionString;
            }

            string Columtxt = "";
            string strSql = "SELECT value FROM ::fn_listextendedproperty ('MS_Description','user','dbo','table','" + truetablename + "','column','" + columnname + "')";
            using (SqlConnection conn = new SqlConnection(connstr))
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = strSql;
                cmd.CommandTimeout = 60;
                cmd.CommandType = CommandType.Text;
                Columtxt = (string)cmd.ExecuteScalar();
            }
            return Columtxt;
        }
        /// <summary>
        /// 读取sql server数据库结构 输出表名
        /// </summary>
        /// <param name="sType">结构类型 Table表 Views视图</param>
        /// <returns></returns>
        public static DataTable GetSchemaTable2(string ConnectionString)
        {
            DataTable dt;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                try
                {
                    dt = conn.GetSchema("TABLES");
                }
                catch
                {
                    dt = null;
                }
                finally
                {
                    conn.Close();
                }
            }
            dt.DefaultView.Sort = "TABLE_NAME asc";
            //TABLE_NAME
            DataTable newtable = new DataTable();
            newtable.Columns.Add("TABLE_NAME");
            newtable.Columns.Add("TABLENAME");
            string[] connect1 = ConnectionString.Split(new string[] { "Initial Catalog=" }, StringSplitOptions.RemoveEmptyEntries);
            string[] connect = connect1[1].Split(new string[] { ";User ID=" }, StringSplitOptions.RemoveEmptyEntries);
            string dataname = connect[0];
            foreach (DataRow row in dt.Rows)
            {
                newtable.Rows.Add(row["TABLE_NAME"], row["TABLE_NAME"]);
            }
            return newtable;
        }

        public static DataTable GetSchemaTable(string ConnectionString)
        {
            return GetSchemaTable(ConnectionString, "dbo");
        }
        /// <summary>
        /// 读取sql server数据库结构 输出表名
        /// </summary>
        /// <param name="sType">结构类型 Table表 Views视图</param>
        /// <param name="sType">架构名称,默认为dbo</param>
        /// <returns></returns>
        public static DataTable GetSchemaTable(string ConnectionString, string schema)
        {
            if (string.IsNullOrEmpty(schema)) schema = "dbo";
            schema = schema.Trim();
            DataTable dt;
            DataTable newtable = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    dt = conn.GetSchema("TABLES");
                    dt.DefaultView.Sort = "TABLE_NAME asc";
                    newtable.Columns.Add("TABLE_NAME");
                    newtable.Columns.Add("TABLENAME");
                    string[] connect1 = ConnectionString.Split(new string[] { "Initial Catalog=" }, StringSplitOptions.RemoveEmptyEntries);
                    string[] connect = connect1[1].Split(new string[] { ";User ID=" }, StringSplitOptions.RemoveEmptyEntries);
                    string dataname = connect[0];

                    foreach (DataRow row in dt.Rows)
                    {
                        newtable.Rows.Add(dataname + "." + schema + "." + row["TABLE_NAME"], row["TABLE_NAME"]);
                    }
                }
                catch
                {
                    dt = null;
                }
                finally
                {
                    conn.Close();
                }
            }
            //TABLE_NAME
            return newtable;
        }
        #endregion
    }
    public class Sql
    {
        public static bool ExeSql(string sql)
        {
            return SqlHelper.ExecuteSql(sql);
        }
        public static DbDataReader SelReturnReader(string tbname, string where, SqlParameter[] sp = null)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            if (sp != null) { list.AddRange(sp); }
            return DBCenter.SelReturnReader(tbname, DealWhere(where), "", list);
        }
        public static DbDataReader SelReturnReader(string tbname, string pk, int id)
        {
            return DBCenter.SelReturnReader(tbname,pk,id); 
        }
        public static bool Del(string tbname, int id)
        {
            return DBCenter.Del(tbname, "ID", id);
        }
        public static bool Del(string tbname, string where, SqlParameter[] sp = null)
        {
            return DBCenter.DelByWhere(tbname, DealWhere(where), DealSP(sp));
        }
        public static DataTable Sel(string tbname, string pk,int id)
        {
            return DBCenter.Sel(tbname,pk+"="+id);
        }
        public static DataTable Sel(string tbname, string where = "", SqlParameter[] sp = null)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            if (sp != null) { list.AddRange(sp); }
            return DBCenter.Sel(tbname, DealWhere(where), "", list);
        }
        public static DataTable Sel(string tbname, string where, string order, SqlParameter[] sp=null)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            if (sp != null) { list.AddRange(sp); }
            return DBCenter.Sel(tbname, DealWhere(where),DealOrder(order), list);
        }
        public static string DealWhere(string where)
        {
            if (!string.IsNullOrEmpty(where)) { where = where.ToUpper().Replace("WHERE ", ""); }
            return where;
        }
        public static string DealOrder(string order)
        {
            if (string.IsNullOrEmpty(order)) { return ""; }
            order = order.ToUpper().Replace("ORDER BY ","");
            return order;
        }
        public static List<SqlParameter> DealSP(SqlParameter[] sp)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            if (sp != null) { list.AddRange(sp); }
            return list;

        }



        /// <summary>
        /// 根据标识ID更新
        /// </summary>
        /// <param name="strTableName">表名</param>
        /// <param name="PK">标识列</param>
        /// <param name="ID">标识列的值</param>
        /// <param name="strSet">赋值</param>
        public static bool UpdateByID(string strTableName, string PK, int ID, string strSet, SqlParameter[] sp)
        {
            string strSql = "UPDATE " + strTableName + " SET " + strSet + " WHERE " + PK + "=" + ID;
            return SqlHelper.ExecuteSql(strSql, sp);
        }
        public static bool UpdateByIDs(string strTableName, string PK, string IDs, string strSet, SqlParameter[] sp)
        {
            string strSql = "UPDATE " + strTableName + " SET " + strSet + " WHERE " + PK + "='" + IDs + "'";
            return SqlHelper.ExecuteSql(strSql, sp);
        }
        /// <summary>
        /// 返回值是命令影响的行数,非ID
        /// </summary>
        public static int insert(string tableName, SqlParameter[] mf, string paras, string filed)
        {
            string sqlStr = "INSERT INTO " + tableName + "(" + filed + ")" + " VALUES(" + paras + ")";
            SqlParameter[] cmdParams = mf;
            return SqlHelper.ExecuteNonQuery(CommandType.Text, sqlStr, cmdParams);
        }
        public static int insertID(string tableName, SqlParameter[] mf, string paras, string filed)
        {
            string sqlStr = "INSERT INTO " + tableName + "(" + filed + ")" + " VALUES(" + paras + ");SELECT @@identity;";
            try
            {
                return Convert.ToInt32(SqlHelper.ExecuteScalar(CommandType.Text, sqlStr, mf));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + ":" + sqlStr);
            }
        }
    }
}