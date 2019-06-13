using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Media;
using CefSharp;
using CefSharp.Wpf;

namespace XWpfNotification.SampleWPFApp.CEFSharpIntegration
{
    public class CefUINotification : CustomUINotification
    {
        private ChromiumWebBrowser WebView { get; set; } = null;
        private CefUIJavaScriptBridge JSBridge { get; set; } = null;
        
        public bool IsAutoClose { get; private set; } = false;
        public string UIPageURL { get; private set; } = String.Empty;



        public CefUINotification(string groupID, string notificationID, string title, string messageBody, DateTime messageTime, string urlNotificationUIPage, double webWidth, double webHeight, object userState = null)
            : base(groupID, notificationID, title, messageBody, messageTime, null, userState)
        {
            if (String.IsNullOrEmpty(urlNotificationUIPage)) { throw new ArgumentNullException("urlNotificationUIPage"); }
            UIPageURL = urlNotificationUIPage;

            Width = webWidth;
            Height = webHeight;
            UserState = userState;

            Unloaded += (s, e) =>
            {
                if (WebView != null)
                {
                    if (WebView.GetBrowser() != null) WebView.GetBrowser().CloseBrowser(true);
                    WebView.Dispose();
                    GC.Collect();
                }
                JSBridge = null;
            };

            JSBridge = new CefUIJavaScriptBridge(this);
            WebView = CreateCefSharpWebView("about:blank");
            WebView.IsBrowserInitializedChanged += (s, e) =>
            {
                if ((bool)e.NewValue == true)
                {
                    WebView.Address = UIPageURL;
                }
            };
            WebView.JavascriptObjectRepository.Register(
                JSBridge.HandlerName,
                JSBridge,
                false,
                new BindingOptions()
                {
                    CamelCaseJavascriptNames = false
                });
            Content = WebView;
        }



        public void ExecuteJavaScriptAsync(string script)
        {
            WebView.ExecuteScriptAsyncWhenPageLoaded(script);
        }

        public override void Show(WindowsNotificationCoverage overlay, TimeSpan showDuration, WindowsNotificationCommon.WindowsNotificationShowPositionType showPositionType, bool autoClose = true)
        {
            if (overlay == null) { return; }

            JSBridge.ShowPosition = showPositionType;
            IsAutoClose = autoClose;
            Opacity = 0.0;
            ((System.Windows.Controls.Canvas)overlay.Content).Children.Add(this);
        }



        static private ChromiumWebBrowser CreateCefSharpWebView(string url)
        {
            bool isCefInitialized = CefSharp.Cef.IsInitialized;
            if (!isCefInitialized) isCefInitialized = App.InitializeCefSharp();
            if (isCefInitialized)
            {
                CefSharp.BrowserSettings settings = new CefSharp.BrowserSettings();
                settings.ApplicationCache = CefSharp.CefState.Default;
                settings.DefaultEncoding = "UTF-8";
                settings.FileAccessFromFileUrls = CefSharp.CefState.Enabled;
                settings.ImageLoading = CefSharp.CefState.Enabled;
                settings.Javascript = CefSharp.CefState.Enabled;
                settings.JavascriptAccessClipboard = CefSharp.CefState.Enabled;
                settings.JavascriptCloseWindows = CefSharp.CefState.Enabled;
                settings.JavascriptDomPaste = CefSharp.CefState.Enabled;
                settings.Plugins = CefSharp.CefState.Enabled;
                settings.UniversalAccessFromFileUrls = CefSharp.CefState.Enabled;
                settings.WebSecurity = CefSharp.CefState.Disabled;

                ChromiumWebBrowser webview = new ChromiumWebBrowser(url);
                webview.BeginInit();
                webview.Margin = new Thickness(0, 0, 0, 0);
                webview.HorizontalAlignment = HorizontalAlignment.Stretch;
                webview.VerticalAlignment = VerticalAlignment.Stretch;
                webview.Background = Brushes.Transparent;
                webview.BrowserSettings = settings;
                webview.EndInit();

                return webview;
            }
            else
            {
                throw new Exception("CEF 초기화 실패!");
            }
        }
    }
}
