using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefSharp;

namespace FontZ01.BrowserHandler
{
    /// <summary>
    /// 键盘按键事件处理
    /// </summary>
    class KeyboardHandler : IKeyboardHandler
    {
        public bool OnKeyEvent(IWebBrowser chromiumWebBrowser, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey)
        {
            //123是F12的键码
            //if (ConfigHelper.globalConfig.Dev && type == KeyType.RawKeyDown && windowsKeyCode == 123)
            //{
            //    chromiumWebBrowser.ShowDevTools();
            //}
            return false;
        }

        public bool OnPreKeyEvent(IWebBrowser chromiumWebBrowser, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey, ref bool isKeyboardShortcut)
        {
            return false;
        }
    }
}
