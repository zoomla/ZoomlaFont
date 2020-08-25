using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Common.DB;
using Common.Models;

namespace Common.Helper
{
    public static class ConfigHelper
    {
        private static string globalPath = AppDomain.CurrentDomain.BaseDirectory + @"\Config\global.config";
        private static string modelPath = AppDomain.CurrentDomain.BaseDirectory + @"\Config\model.config";
        private static object Lockobj = new object();
        public static APPConfigInfo APPInfo = null;
        public static void LoadXML()
        {
            using (Stream stream = new FileStream(globalPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(APPConfigInfo));
                APPInfo = (APPConfigInfo)serializer.Deserialize(stream);
            }
            modelxml.Load(modelPath);
        }
        public static void Update()
        {
            lock (Lockobj)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(APPConfigInfo));
                using (Stream stream = new FileStream(globalPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                {
                    XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                    namespaces.Add("", "");//加上这句,去掉根节点多余的属性，否则会自动在根节点上加两个属性
                    serializer.Serialize(stream, APPInfo, namespaces);
                    stream.Close(); stream.Dispose();
                    LoadXML();
                }
            }
        }
        //--------------------------------Model
        private static XmlDocument modelxml= new XmlDocument();
        public static XmlNode Model_New(string name)
        {
            XmlNode node = modelxml.SelectSingleNode("//model/" + name);
            return node;
        }
    }
        public abstract class Base
    {
        public object GetValue(string fieldName)
        {
            return GetType().GetProperty(fieldName).GetValue(this);
        }
    }
    public class APPConfigInfo : Base
    {
        //SQL数据库连接字符串
        public string connstr = "";
        public string fzdir = "";
        public string sitesServiceName = "ZoomlaCMS_Sites";
        public string ImgSavePath { get; set; }
        public bool Dev { get; set; }
    }

}
