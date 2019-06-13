using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using CefSharp;
using XWpfNotification;
using XWpfNotification.SampleWPFApp.CEFSharpIntegration;

namespace XWpfNotification.SampleWPFApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        static public WindowsNotificationCoverage NotiOverlay;
        static internal System.Windows.Forms.NotifyIcon AppNotifyIcon = null;
        static internal System.Windows.Forms.ContextMenuStrip AppNotifyIcon_ContextMenuStrip = null;



        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            InitializeCefSharp();

            App.NotiOverlay = new WindowsNotificationCoverage();
            App.NotiOverlay.Loaded += (s, eargs) =>
            {
                if (App.NotiOverlay.IsDrivingTest)
                {
                    Random rndGroupID = new Random();
                    int g = 1001;
                    int i = 1;
                    System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
                    timer.Interval = TimeSpan.FromMilliseconds(400);
                    timer.Tick += (ss, eeargs) =>
                    {
                        WpfUINotification noti = new WpfUINotification(String.Format("group{0}", g), String.Format("{0}_{1}", g, i), "Windows Notification 제목", String.Format("[{0}] {1}번째 Windows Notification 메시지 입니다.......", g, i), DateTime.Now);
                        noti.Width = 320;
                        noti.Height = 100;
                        noti.Show(App.NotiOverlay, WindowsNotificationCommon.NOTIFICATION_SHOW_DURATION, WindowsNotificationCommon.WindowsNotificationShowPositionType.TopRight, true);
                        g = rndGroupID.Next(1001, 1010);
                        i++;
                        if (i == 101) { timer.Stop(); }
                        System.Threading.Thread.Sleep(1);
                    };
                    timer.Start();
                }
            };
            NotiOverlay.IsDrivingTest = true;

            MainWindow = NotiOverlay;

            InitializeAppNotifyIcon();
            MainWindow.Show();

            if (!Cef.IsInitialized)
            {
                #region [CEFSharp 연동 UI Notification 호출하기]
                string urlUIPage = System.IO.Path.Combine(new System.IO.FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location).DirectoryName, @"CEFSharpIntegration\XWpfNotification.html");
                int groupId = 1001;
                int notiId = 1;
                System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(400);
                timer.Tick += (s, args) =>
                {
                    CefUINotification cefNoti = new CefUINotification(groupId.ToString(), notiId.ToString(), notiId.ToString() + " Notification 제목", "[GROUP " + groupId.ToString() + "][" + notiId.ToString() + "] CEFSharp과 연동된 Windows Notification 메시지 입니다.", DateTime.Now, urlUIPage, 350, 100, null);
                    
                    cefNoti.Show(App.NotiOverlay, WindowsNotificationCommon.NOTIFICATION_SHOW_DURATION, WindowsNotificationCommon.WindowsNotificationShowPositionType.BottomCenter, true);

                    groupId+=10;
                    notiId++;
                    if (notiId == 21) { timer.Stop(); }
                    //System.Threading.Thread.Sleep(1);
                };
                timer.Start();
                #endregion
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Cef.Shutdown();
            if (App.NotiOverlay != null)
            {
                App.NotiOverlay.Close();
                App.NotiOverlay = null;
            }
            if (AppNotifyIcon_ContextMenuStrip != null)
            {
                AppNotifyIcon_ContextMenuStrip.Items.Clear();
                AppNotifyIcon_ContextMenuStrip.Dispose();
                AppNotifyIcon_ContextMenuStrip = null;
            }
            if (AppNotifyIcon != null)
            {
                AppNotifyIcon.Visible = false;
                AppNotifyIcon.Dispose();
                AppNotifyIcon = null;
            }
            base.OnExit(e);
        }



        private void InitializeAppNotifyIcon()
        {
            AppNotifyIcon = new System.Windows.Forms.NotifyIcon();
            AppNotifyIcon.Text = "XWpfNotification.SampleWPFApp";
            AppNotifyIcon.Icon = System.Drawing.SystemIcons.Shield;

            AppNotifyIcon_ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            AppNotifyIcon_ContextMenuStrip.Items.Add(new System.Windows.Forms.ToolStripSeparator()
            {
                Visible = true
            });
            AppNotifyIcon_ContextMenuStrip.Items.Add("종료", System.Drawing.SystemIcons.Error.ToBitmap(), (sender, e) =>
            {
                Application.Current.Shutdown();
            });

            AppNotifyIcon.ContextMenuStrip = AppNotifyIcon_ContextMenuStrip;
            AppNotifyIcon.Visible = true;
        }



        static internal bool InitializeCefSharp()
        {
            if (CefSharp.Cef.IsInitialized) { return true; }

            Cef.EnableHighDPISupport();
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            CefSharpSettings.ShutdownOnExit = true;
            CefSharpSettings.SubprocessExitIfParentProcessClosed = true;

            CefSharp.Wpf.CefSettings settings = new CefSharp.Wpf.CefSettings();
            settings.AcceptLanguageList = "ko-KR,ko";
            settings.Locale = "ko-KR";
            settings.LogSeverity = CefSharp.LogSeverity.Default;
            settings.MultiThreadedMessageLoop = true;
            settings.PersistSessionCookies = true;
            settings.PersistUserPreferences = true;
            settings.WindowlessRenderingEnabled = true;
            settings.CefCommandLineArgs.Add("enable-media-stream", "1");

            return CefSharp.Cef.Initialize(settings);
        }
    }
}
