using System;
using System.Data.Common;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Common.DB;
using ZoomLa.SQLDAL;
using ZoomLa.SQLDAL.SQL;

namespace Common.MB
{


    public class M_FZ_Server : M_Base
    {
        public int id { get; set; }
        public string alias { get; set; }
        public string address { get; set; }
        public string uname { get; set; }
        public string upwd { get; set; }
        public string remind { get; set; }
        public DateTime cdate { get; set; } = DateTime.Now;
        public string rdp { get; set; } = "";
        public override string GetPK() { return "id"; }
        public override string TbName { get { return "ZL_FZ_Server"; } }
        public override string[,] FieldList()
        {
            string[,] Tablelist = {
                {"id","Int","4"},
                {"address","NVarChar","100"},
                {"uname","NVarChar","50"},
                {"upwd","NVarChar","50"},
                {"remind","NVarChar","100"},
                {"cdate","DateTime","8"},
                {"rdp","NVarChar","8000"},
                {"alias","NVarChar","50"}
            };
            return Tablelist;
        }
        public override SqlParameter[] GetParameters()
        {
            SqlParameter[] sp = GetSP();
            sp[0].Value = this.id;
            sp[1].Value = this.address;
            sp[2].Value = this.uname;
            sp[3].Value = this.upwd;
            sp[4].Value = this.remind;
            sp[5].Value = this.cdate;
            sp[6].Value = this.rdp;
            sp[7].Value = this.alias;
            return sp;
        }
        public M_FZ_Server GetModelFromReader(DbDataReader rdr)
        {
            M_FZ_Server model = new M_FZ_Server();
            model.id = ConvertToInt(rdr["id"]);
            model.address = ConverToStr(rdr["address"]);
            model.uname = ConverToStr(rdr["uname"]);
            model.upwd = ConverToStr(rdr["upwd"]);
            model.remind = ConverToStr(rdr["remind"]);
            model.cdate = ConvertToDate(rdr["cdate"]);
            model.rdp = ConverToStr(rdr["rdp"]);
            model.alias = ConverToStr(rdr["alias"]);
            rdr.Close();
            return model;
        }
    }

    public class B_FZ_Server
    {
        private M_FZ_Server initMod = new M_FZ_Server();
        public string TbName = "", PK = "";
        public B_FZ_Server()
        {
            TbName = initMod.TbName;
            PK = initMod.PK;
        }
        public DataTable Sel()
        {
            return DBCenter.Sel(TbName, "", PK + " DESC");
        }
        public M_FZ_Server SelReturnModel(int ID)
        {
            using (DbDataReader reader = DBCenter.SelReturnReader(TbName, PK, ID))
            {
                if (reader.Read())
                {
                    return initMod.GetModelFromReader(reader);
                }
                else
                {
                    return null;
                }
            }
        }
        public int Insert(M_FZ_Server model)
        {
            return DBCenter.Insert(model);
        }
        public bool UpdateByID(M_FZ_Server model)
        {
            return DBCenter.UpdateByID(model, model.id);
        }
        public void Del(string ids)
        {
            if (string.IsNullOrEmpty(ids)) { return; }
            SafeSC.CheckIDSEx(ids);
            DBCenter.DelByIDS(TbName, PK, ids);
        }
        public PageSetting SelPage(int cpage, int psize, Com_Filter filter)
        {
            string where = "1=1 ";
            List<SqlParameter> sp = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(filter.skey))
            {

            }
            PageSetting setting = PageSetting.Single(cpage, psize, TbName, PK, where, PK + " DESC", sp);
            DBCenter.SelPage(setting);
            return setting;
        }
    }

}
