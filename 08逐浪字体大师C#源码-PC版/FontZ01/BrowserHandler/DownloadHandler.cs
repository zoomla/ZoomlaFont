using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;

namespace FontZ01.BrowserHandler
{
    /// <summary>
    /// 下载事件处理
    /// </summary>
    public class DownloadHandler : IDownloadHandler
    {
        public void OnBeforeDownload(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
        {
            if (!callback.IsDisposed)
            {
                using (callback)
                {
                    callback.Continue(@"C:\Users\" +
                            System.Security.Principal.WindowsIdentity.GetCurrent().Name +
                            @"\Downloads\" +
                            downloadItem.SuggestedFileName,
                        showDialog: true);
                }
            }
        }

        public void OnDownloadUpdated(IWebBrowser chromiumWebBrowser, IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
        {
            //throw new NotImplementedException();
        }
    }
}
