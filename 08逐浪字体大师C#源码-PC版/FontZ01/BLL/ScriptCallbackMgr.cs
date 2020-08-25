using System;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;
using CefSharp;
using CefSharp.Wpf;
using System.Web;
using Common.Helper;

namespace FontZ01.BLL
{
    public class ScriptCallbackMgr
    {
        private ChromiumWebBrowser chrome;
        public ScriptCallbackMgr(ChromiumWebBrowser _chrome)
        {
            chrome = _chrome;
        }

        public async void Download(string uri) 
        {
            try
            {
                //简单处理url
                if (string.IsNullOrWhiteSpace(uri)) return;
                uri = HttpUtility.UrlDecode(uri);

                //存在url中没有文件名称的问题
                string fileName = Path.GetFileName(uri);//文件名

                string path = Path.Combine(ConfigHelper.APPInfo.ImgSavePath, fileName);
                path = Common.Commons.NewImgName(path);

                FileInfo fi = new FileInfo(path);
                //检查目录
                if (!fi.Directory.Exists)
                {
                    fi.Directory.Create();
                }
                //检查文件(同名的可能性极小，但还是存在)
                if (fi.Exists)
                {
                    //文件名+毫秒+扩展名
                    fileName = Path.GetFileNameWithoutExtension(fileName) + DateTime.Now.Millisecond + Path.GetExtension(fileName);
                    path = Path.Combine(ConfigHelper.APPInfo.ImgSavePath, fileName);
                }

                //获取图片资源
                //HttpClientHelper client = new HttpClientHelper();
                //Task.Run(client.GetStreamAsync(uri));
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.ExpectContinue = false;
                HttpResponseMessage response = await client.GetAsync(uri);
                Stream stream = await response.Content.ReadAsStreamAsync();

                FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
                await stream.CopyToAsync(fs);
                fs.Flush();
                fs.Close();
                stream.Close();

                //调用浏览器的方法
                chrome.ExecuteScriptAsync("DownloadMsg();");
            }
            catch (Exception ex)
            {
                chrome.ExecuteScriptAsync($"console.log('{ex.Message}');");
            }
        }

        public void GoConfigPage()
        {
            string uri = "file:///" + AppDomain.CurrentDomain.BaseDirectory + "assest\\setting.html";
            chrome.Load(uri);
        }

        public string GetConfig()
        {
            string value = JsonConvert.SerializeObject(ConfigHelper.APPInfo);
            return value;
        }

        public string GetConfig(string configName)
        {
            string value = ConfigHelper.APPInfo.GetValue(configName).ToString();
            return value;
        }

        public bool UpdateConfig(string data)
        {
            APPConfigInfo config = JsonConvert.DeserializeObject<APPConfigInfo>(data);

            //由于客户端无法修改Dev的值，所以不能信任该值，应该信任从配置文件来的值
            config.Dev = ConfigHelper.APPInfo.Dev;
            ConfigHelper.APPInfo = config;
            ConfigHelper.Update();
            return true;
        }

        //private string GetPath(string defname)
        //{
        //    SaveFileDialog dialog = new SaveFileDialog();
        //    dialog.FileName = defname;
        //    var result = dialog.ShowDialog();
        //    if (result == DialogResult.OK)
        //    {
        //        return dialog.FileName;
        //    }
        //    else
        //    {
        //        return string.Empty;
        //    }
        //}
    }
}
