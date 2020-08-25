using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;

namespace FontZ01.BrowserHandler
{
    public class LifeSpanHandler : ILifeSpanHandler
    {
        public bool DoClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
            return false;
            //throw new NotImplementedException();
        }

        public void OnAfterCreated(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
            //throw new NotImplementedException();
        }

        public void OnBeforeClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
            //throw new NotImplementedException();
        }

        public bool OnBeforePopup(IWebBrowser chromiumWebBrowser,
            IBrowser browser,
            IFrame frame,
            string targetUrl,
            string targetFrameName,
            WindowOpenDisposition targetDisposition,
            bool userGesture,
            IPopupFeatures popupFeatures,
            IWindowInfo windowInfo,
            IBrowserSettings browserSettings,
            ref bool noJavascriptAccess,
            out IWebBrowser newBrowser)
        {
            windowInfo.Width = 1024;
            windowInfo.Height = 768;
            newBrowser = null;
            var IWebBrowser = (IWebBrowser)chromiumWebBrowser;//在Form窗体内展现-拒绝弹出新窗口
            chromiumWebBrowser.Load(targetUrl);
            return true;//true:取消弹出窗口
            throw new NotImplementedException();
            //Form
            //windowInfo.Width = 1024;
            //windowInfo.Height = 768;
            //newBrowser = null;
            //return false;//true:取消弹出窗口
            //throw new NotImplementedException();
        }
    }
}
