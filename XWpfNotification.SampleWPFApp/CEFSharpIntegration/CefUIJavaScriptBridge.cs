using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Controls;
using System.Windows.Media.Animation;
using CefSharp;

namespace XWpfNotification.SampleWPFApp.CEFSharpIntegration
{
    public class CefUIJavaScriptBridge
    {
        public CefUINotification BridgedNotification { get; private set; } = null;
        public WindowsNotificationCommon.WindowsNotificationShowPositionType ShowPosition { get; set; } = WindowsNotificationCommon.NOTIFICATION_SHOW_POSITION_TYPE;
        public string HandlerName { get; private set; } = "cefSharpNotiHandler";



        public CefUIJavaScriptBridge(CefUINotification ownerNoti)
        {
            if (ownerNoti == null) { throw new ArgumentNullException("ownerNoti"); }
            BridgedNotification = ownerNoti;
        }



        private void AnimateShow(WindowsNotificationCommon.WindowsNotificationArea notiArea, TimeSpan notiShowDuration, Action showCompletedAction = null, Action closeCompletedAction = null)
        {
            TimeSpan StartDuration = TimeSpan.FromMilliseconds(500);
            IEasingFunction StartEaseFunc = new CubicEase();

            DoubleAnimation aniStartY = new DoubleAnimation();
            aniStartY.From = BridgedNotification.NotificationUILocation.Y;
            aniStartY.To = notiArea.Boundary.Y;
            aniStartY.Duration = StartDuration;
            aniStartY.FillBehavior = FillBehavior.HoldEnd;
            aniStartY.EasingFunction = StartEaseFunc;

            DoubleAnimation aniStartX = new DoubleAnimation();
            aniStartX.From = BridgedNotification.NotificationUILocation.X;
            aniStartX.To = notiArea.Boundary.X;
            aniStartX.Duration = StartDuration;
            aniStartX.FillBehavior = FillBehavior.HoldEnd;
            aniStartX.EasingFunction = StartEaseFunc;
            aniStartX.Completed += (s, e) =>
            {
                DoubleAnimation aniHold = new DoubleAnimation();
                aniHold.From = BridgedNotification.Opacity;
                aniHold.To = BridgedNotification.Opacity;
                aniHold.Duration = notiShowDuration;
                aniHold.FillBehavior = FillBehavior.HoldEnd;
                aniHold.Completed += (ss, ee) =>
                {
                    if (BridgedNotification.IsAutoClose)
                    {
                        App.NotiOverlay.Dispatcher.Invoke(new Action(() =>
                        {
                            App.NotiOverlay.RemoveNotification(BridgedNotification);
                        }));
                    }
                };
                BridgedNotification.BeginAnimation(Control.OpacityProperty, aniHold);
            };
            BridgedNotification.BeginAnimation(Canvas.LeftProperty, aniStartX);
            BridgedNotification.BeginAnimation(Canvas.TopProperty, aniStartY);
        }



        public void InvokeLoadComplete()
        {
            string script = String.Format("showMessage('{0}', '{1}', '{2}');", BridgedNotification.NotificationTitle.Replace("'", "").Replace("\"", ""), BridgedNotification.NotificationMessageBody.Replace("'", "").Replace("\"", ""), BridgedNotification.NotificationTime.ToString("MM-dd HH:mm:ss"));

            WindowsNotificationCommon.WindowsNotificationArea notiArea;
            App.NotiOverlay.Dispatcher.Invoke(new Action(() =>
            {
                BridgedNotification.ExecuteJavaScriptAsync(script);
                notiArea = App.NotiOverlay.CreateNewArea(BridgedNotification, ShowPosition);
                BridgedNotification.NotificationUILocation = notiArea.ShowAnimationStartPoint;
                BridgedNotification.Opacity = 1.0;
                AnimateShow(notiArea, WindowsNotificationCommon.NOTIFICATION_SHOW_DURATION, null, null);
            }), System.Windows.Threading.DispatcherPriority.Render);
        }

        public void InvokeRemoveNotification()
        {
            App.NotiOverlay.Dispatcher.Invoke(new Action(() =>
            {
                App.NotiOverlay.RemoveNotification(BridgedNotification);
            }), System.Windows.Threading.DispatcherPriority.Render);
        }
    }
}
