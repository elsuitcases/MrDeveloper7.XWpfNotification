using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace XWpfNotification
{
    public abstract class WindowsNotification : ContentControl
    {
        static public readonly DependencyProperty NotificationUILocationProperty = DependencyProperty.Register("NotificationUILocation", typeof(Point), typeof(WindowsNotification), new PropertyMetadata(new Point(0.0, 0.0)));

        protected readonly Brush BACKGROUND_COLOR = Brushes.DarkSlateBlue;
        protected readonly Brush FOREGROUND_COLOR = Brushes.GhostWhite;
        protected readonly Brush BORDER_COLOR = Brushes.SlateBlue;
        protected readonly Thickness BORDER_THICKNESS = new Thickness(2, 2, 2, 2);

        public WindowsNotificationCommon.WindowsNotificationUIType NotificationType { get; private set; } = WindowsNotificationCommon.WindowsNotificationUIType.WindowsUI;
        public string NotificationGroupID { get; private set; } = String.Empty;
        public string NotificationID
        {
            get
            {
                return Name;
            }
            private set
            {
                Name = value;
            }
        }

        public string NotificationTitle { get; private set; } = "Windows Notification 제목";
        public string NotificationMessageBody { get; private set; } = "Windows Notification 메시지";
        public DateTime NotificationTime { get; private set; } = DateTime.Now;
        public Point NotificationUILocation
        {
            get
            {
                return (Point)GetValue(NotificationUILocationProperty);
            }
            set
            {
                SetValue(NotificationUILocationProperty, value);
            }
        }



        private Border Edges = new Border()
        {
            Margin = new Thickness(0, 0, 0, 0),
            Padding = new Thickness(2),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            CornerRadius = new CornerRadius(10),
            Background = Brushes.Transparent,
            BorderBrush = Brushes.Tomato,
            BorderThickness = new Thickness(0),
            Effect = new System.Windows.Media.Effects.DropShadowEffect()
            {
                BlurRadius = 20,
                Color = Colors.Black,
                Direction = -45,
                Opacity = 1.0,
                ShadowDepth = 5
            }
        };

        private Grid LayoutRootGrid = new Grid()
        {
            Name = "LayoutRoot",
            Margin = new Thickness(0, 0, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Background = Brushes.Transparent
        };

        /// <summary>
        /// Notification UI 컨텐트
        /// </summary>
        public new UIElement Content
        {
            get
            {
                return LayoutRootGrid.Children[0];
            }
            protected set
            {
                if (value == null) { return; }
                LayoutRootGrid.Children.Clear();
                LayoutRootGrid.Children.Add(value);
            }
        }



        public WindowsNotification(WindowsNotificationCommon.WindowsNotificationUIType notiType, string groupID, string notificationID, string title, string messageBody, DateTime messageTime) : base()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowsNotification), new FrameworkPropertyMetadata(typeof(WindowsNotification)));
            
            BeginInit();

            Binding LocationBindX = new Binding();
            LocationBindX.Source = this;
            LocationBindX.Path = new PropertyPath(Canvas.LeftProperty);
            LocationBindX.Mode = BindingMode.TwoWay;

            Binding LocationBindY = new Binding();
            LocationBindY.Source = this;
            LocationBindY.Path = new PropertyPath(Canvas.TopProperty);
            LocationBindY.Mode = BindingMode.TwoWay;

            MultiBinding LocationBind = new MultiBinding();
            LocationBind.Converter = new LocationPointConverter();
            LocationBind.Mode = BindingMode.TwoWay;
            LocationBind.Bindings.Add(LocationBindX);
            LocationBind.Bindings.Add(LocationBindY);
            BindingOperations.SetBinding(this, WindowsNotification.NotificationUILocationProperty, LocationBind);

            NotificationType = notiType;
            NotificationGroupID = groupID;
            NotificationTitle = title;
            NotificationMessageBody = messageBody;
            NotificationTime = messageTime;
            
            Name = String.Format("noti_{0}", notificationID);
            Margin = new Thickness(0, 0, 0, 0);
            Width = 400;
            Height = 100;
            Background = Brushes.Transparent;
            Opacity = 1.0;
            Visibility = Visibility.Visible;

            base.Content = Edges;
            Edges.Child = LayoutRootGrid;

            MouseEnter += (sender, e) =>
            {
                Cursor = System.Windows.Input.Cursors.Hand;
            };
            PreviewMouseMove += (sender, e) =>
            {
                Cursor = System.Windows.Input.Cursors.Hand;
            };
            MouseLeave += (sender, e) =>
            {
                Cursor = System.Windows.Input.Cursors.Arrow;
            };

            EndInit();
        }



        public virtual void Show(WindowsNotificationCoverage overlay, TimeSpan showDuration, WindowsNotificationCommon.WindowsNotificationShowPositionType showPositionType, bool autoClose = true)
        {
            if (overlay == null) { return; }
            overlay.ShowNotification(this, showDuration, showPositionType, autoClose);
        }



        static public void Show(WindowsNotificationCoverage overlay, WindowsNotification noti)
        {
            if ((overlay == null) || (noti == null)) { return; }
            overlay.ShowNotification(noti, WindowsNotificationCommon.NOTIFICATION_SHOW_DURATION, WindowsNotificationCommon.NOTIFICATION_SHOW_POSITION_TYPE, true);
        }

        static public void Show(WindowsNotificationCoverage overlay, WindowsNotification noti, TimeSpan showDuration, WindowsNotificationCommon.WindowsNotificationShowPositionType showPositionType, bool autoClose = true)
        {
            if ((overlay == null) || (noti == null)) { return; }
            overlay.ShowNotification(noti, showDuration, showPositionType, autoClose);
        }
    }
}
