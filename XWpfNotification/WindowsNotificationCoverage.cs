using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace XWpfNotification
{
    public class WindowsNotificationCoverage : Window
    {
        static private List<WindowsNotificationCommon.WindowsNotificationArea> NotificationAreas = new List<WindowsNotificationCommon.WindowsNotificationArea>();
        static public readonly System.Collections.Concurrent.ConcurrentQueue<WindowsNotificationCommon.WindowsNotificationArea> AreasQueues = new System.Collections.Concurrent.ConcurrentQueue<WindowsNotificationCommon.WindowsNotificationArea>(NotificationAreas);
        static private object lockerSticker = new object();

        private Canvas sticker = null;
        private Random RandomLocation = new Random();

        public bool IsDrivingTest { get; set; } = false;



        public WindowsNotificationCoverage() : base()
        {
            BeginInit();

            Name = "winNotificationCoverage";
            AllowDrop = true;
            AllowsTransparency = true;
            ResizeMode = ResizeMode.NoResize;
            ShowInTaskbar = false;
            Topmost = true;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;
            Background = Brushes.Transparent;
            BorderBrush = Brushes.Red;
            BorderThickness = new Thickness(3, 3, 3, 3);
            Margin = new Thickness(0, 0, 0, 0);
            Padding = new Thickness(0, 0, 0, 0);

            Unloaded += (sender, e) =>
            {
                NotificationAreas.Clear();
                sticker.Children.Clear();
                sticker = null;
                RandomLocation = null;
            };

            sticker = new Canvas();
            sticker.BeginInit();
            sticker.Name = "sticker";
            sticker.Margin = new Thickness(0, 0, 0, 0);
            sticker.HorizontalAlignment = HorizontalAlignment.Stretch;
            sticker.VerticalAlignment = VerticalAlignment.Stretch;
            sticker.Background = Brushes.Transparent;
            sticker.EndInit();

            Content = sticker;

            EndInit();
        }



        private void AnimateShow(WindowsNotification noti, WindowsNotificationCommon.WindowsNotificationArea notiArea, TimeSpan showDuration, bool autoClose)
        {
            TimeSpan StartDuration = TimeSpan.FromMilliseconds(400);
            IEasingFunction StartEaseFunc = new CubicEase();

            DoubleAnimation aniStartY = new DoubleAnimation();
            aniStartY.From = noti.NotificationUILocation.Y;
            aniStartY.To = notiArea.Boundary.Y;
            aniStartY.Duration = StartDuration;
            aniStartY.FillBehavior = FillBehavior.HoldEnd;
            aniStartY.EasingFunction = StartEaseFunc;

            DoubleAnimation aniStartX = new DoubleAnimation();
            aniStartX.From = noti.NotificationUILocation.X;
            aniStartX.To = notiArea.Boundary.X;
            aniStartX.Duration = StartDuration;
            aniStartX.FillBehavior = FillBehavior.HoldEnd;
            aniStartX.EasingFunction = StartEaseFunc;
            aniStartX.Completed += (s, e) =>
            {
                DoubleAnimation aniHold = new DoubleAnimation();
                aniHold.From = noti.Opacity;
                aniHold.To = noti.Opacity;
                aniHold.Duration = showDuration;
                aniHold.FillBehavior = FillBehavior.HoldEnd;
                aniHold.Completed += (ss, ee) =>
                {
                    if (autoClose)
                    {
                        RemoveNotification(noti);
                    }
                };
                noti.BeginAnimation(OpacityProperty, aniHold);
            };
            noti.BeginAnimation(Canvas.LeftProperty, aniStartX);
            noti.BeginAnimation(Canvas.TopProperty, aniStartY);
        }

        private WindowsNotificationCommon.WindowsNotificationArea CreateFirstNewArea(WindowsNotification noti, WindowsNotificationCommon.WindowsNotificationShowPositionType showPositionType)
        {
            WindowsNotificationCommon.WindowsNotificationArea CreatingArea = new WindowsNotificationCommon.WindowsNotificationArea();
            CreatingArea.NotificationGroupID = noti.NotificationGroupID;
            CreatingArea.NotificationID = noti.NotificationID;
            CreatingArea.NotificationType = noti.NotificationType;
            CreatingArea.ShowPositionType = showPositionType;

            Point ptInitial = GetInitialShowPositionByShowPositionType(showPositionType, noti.Width, noti.Height);
            CreatingArea.Boundary = new Rect()
            {
                Width = noti.Width,
                Height = noti.Height,
                X = ptInitial.X,
                Y = ptInitial.Y
            };

            CreatingArea.CreatedTime = DateTime.Now;

            return CreatingArea;
        }

        private Point GetInitialShowPositionByShowPositionType(WindowsNotificationCommon.WindowsNotificationShowPositionType showPositionType, double notificationWidth, double notificationHeight)
        {
            double InitialX_TopRightOrBottomRight = SystemParameters.WorkArea.Width - notificationWidth;
            double InitialX_BottomCenter = (SystemParameters.WorkArea.Width / 2) - (notificationWidth / 2);
            double InitialY_TopRight = WindowsNotificationCommon.NOTIFICATION_GAP;
            double InitialY_BottomCenterOrBottomRight = SystemParameters.WorkArea.Height - notificationHeight - WindowsNotificationCommon.NOTIFICATION_GAP;

            Point InitialShowPosition = new Point();

            switch (showPositionType)
            {
                case WindowsNotificationCommon.WindowsNotificationShowPositionType.BottomCenter:
                    InitialShowPosition.X = InitialX_BottomCenter;
                    InitialShowPosition.Y = InitialY_BottomCenterOrBottomRight;
                    break;

                case WindowsNotificationCommon.WindowsNotificationShowPositionType.BottomRight:
                    InitialShowPosition.X = InitialX_TopRightOrBottomRight;
                    InitialShowPosition.Y = InitialY_BottomCenterOrBottomRight;
                    break;

                case WindowsNotificationCommon.WindowsNotificationShowPositionType.Random:
                    InitialShowPosition.X = (double)RandomLocation.Next(0, (int)(SystemParameters.WorkArea.Width - notificationWidth - WindowsNotificationCommon.NOTIFICATION_GAP));
                    InitialShowPosition.Y = (double)RandomLocation.Next(0, (int)(SystemParameters.WorkArea.Height - notificationHeight - WindowsNotificationCommon.NOTIFICATION_GAP));
                    break;

                case WindowsNotificationCommon.WindowsNotificationShowPositionType.TopRight:
                    InitialShowPosition.X = InitialX_TopRightOrBottomRight;
                    InitialShowPosition.Y = InitialY_TopRight;
                    break;

                default:
                    InitialShowPosition.X = 0.0;
                    InitialShowPosition.Y = 0.0;
                    break;
            }

            return InitialShowPosition;
        }



        public WindowsNotificationCommon.WindowsNotificationArea CreateNewArea(WindowsNotification noti, WindowsNotificationCommon.WindowsNotificationShowPositionType showPositionType)
        {
            WindowsNotificationCommon.WindowsNotificationArea CreatingArea;

            Point ptInitial = GetInitialShowPositionByShowPositionType(showPositionType, noti.Width, noti.Height);

            if (!Monitor.TryEnter(NotificationAreas))
                Monitor.Wait(NotificationAreas);

            if (NotificationAreas.Count < 1)
            {
                #region <첫 번째 Notification 또는 현재 비어있는 경우>
                CreatingArea = CreateFirstNewArea(noti, showPositionType);
                #endregion
            }
            else if (NotificationAreas.Exists(a => a.NotificationGroupID == noti.NotificationGroupID && a.ShowPositionType == showPositionType))
            {
                #region <존재하는 Notification 그룹>
                WindowsNotificationCommon.WindowsNotificationArea ExistedGroupArea = NotificationAreas.Where(a => a.NotificationGroupID == noti.NotificationGroupID && a.ShowPositionType == showPositionType).OrderBy(a => a.CreatedTime).LastOrDefault();
                CreatingArea = new WindowsNotificationCommon.WindowsNotificationArea();
                CreatingArea.NotificationGroupID = ExistedGroupArea.NotificationGroupID;
                CreatingArea.NotificationID = noti.NotificationID;
                CreatingArea.NotificationType = noti.NotificationType;
                CreatingArea.ShowPositionType = showPositionType;
                CreatingArea.Boundary = ExistedGroupArea.Boundary;
                CreatingArea.CreatedTime = DateTime.Now;
                #endregion
            }
            else
            {
                #region <다른 그룹의 Notification(들)만 존재하는 경우>
                WindowsNotificationCommon.WindowsNotificationArea LastDifferentGroupArea;

                if (NotificationAreas.Where(a => a.NotificationGroupID != noti.NotificationGroupID && a.ShowPositionType == showPositionType).Count() > 0)
                {
                    LastDifferentGroupArea = NotificationAreas.Where(a => a.NotificationGroupID != noti.NotificationGroupID && a.ShowPositionType == showPositionType).OrderBy(a => a.CreatedTime).GroupBy(a => a.NotificationGroupID).LastOrDefault().FirstOrDefault();
                }
                else
                {
                    return CreatingArea = CreateFirstNewArea(noti, showPositionType);
                }

                CreatingArea = new WindowsNotificationCommon.WindowsNotificationArea();
                CreatingArea.NotificationGroupID = noti.NotificationGroupID;
                CreatingArea.NotificationID = noti.NotificationID;
                CreatingArea.NotificationType = noti.NotificationType;
                CreatingArea.ShowPositionType = showPositionType;

                CreatingArea.Boundary = new Rect()
                {
                    Width = noti.Width,
                    Height = noti.Height,
                    X = LastDifferentGroupArea.Boundary.X,
                    Y = LastDifferentGroupArea.Boundary.Y + LastDifferentGroupArea.Boundary.Height + WindowsNotificationCommon.NOTIFICATION_GAP
                };

                if (NotificationAreas.Where(a => a.Boundary.X == ptInitial.X && a.Boundary.Y == ptInitial.Y).Count() == 0)
                {
                    CreatingArea.Boundary.X = ptInitial.X;
                    CreatingArea.Boundary.Y = ptInitial.Y;
                }

                CreatingArea.CreatedTime = DateTime.Now;

                if (showPositionType == WindowsNotificationCommon.WindowsNotificationShowPositionType.BottomCenter)
                {
                    if (CreatingArea.Boundary.Y < 0)
                    {
                        CreatingArea.Boundary.Y = ptInitial.Y;
                    }
                }
                else if (showPositionType == WindowsNotificationCommon.WindowsNotificationShowPositionType.BottomRight)
                {
                    if (CreatingArea.Boundary.Y < 0)
                    {
                        CreatingArea.Boundary.X = LastDifferentGroupArea.Boundary.X - noti.Width - WindowsNotificationCommon.NOTIFICATION_GAP;
                        CreatingArea.Boundary.Y = ptInitial.Y;
                        if (CreatingArea.Boundary.X < 0)
                        {
                            CreatingArea.Boundary.X = ptInitial.X;
                        }
                    }
                }
                else if (showPositionType == WindowsNotificationCommon.WindowsNotificationShowPositionType.TopRight)
                {
                    if (CreatingArea.Boundary.Y > (SystemParameters.PrimaryScreenHeight - noti.Height - WindowsNotificationCommon.NOTIFICATION_GAP))
                    {
                        CreatingArea.Boundary.X = LastDifferentGroupArea.Boundary.X - noti.Width - WindowsNotificationCommon.NOTIFICATION_GAP;
                        CreatingArea.Boundary.Y = ptInitial.Y;
                        if (CreatingArea.Boundary.X < 0)
                        {
                            CreatingArea.Boundary.X = ptInitial.X;
                        }
                    }
                }
                #endregion
            }

            // 최초 위치 (애니메이션 시작 위치)
            Point ShowAnimationStartPoint = new Point();
            switch (showPositionType)
            {
                case WindowsNotificationCommon.WindowsNotificationShowPositionType.BottomCenter:
                    ShowAnimationStartPoint.X = (SystemParameters.PrimaryScreenWidth / 2) - (noti.Width / 2);
                    ShowAnimationStartPoint.Y = CreatingArea.Boundary.Height;
                    break;

                case WindowsNotificationCommon.WindowsNotificationShowPositionType.BottomRight:
                    ShowAnimationStartPoint.X = SystemParameters.PrimaryScreenWidth;
                    ShowAnimationStartPoint.Y = CreatingArea.Boundary.Y;
                    break;

                case WindowsNotificationCommon.WindowsNotificationShowPositionType.Random:
                    ShowAnimationStartPoint.X = SystemParameters.PrimaryScreenWidth;
                    ShowAnimationStartPoint.Y = SystemParameters.PrimaryScreenHeight;
                    break;

                case WindowsNotificationCommon.WindowsNotificationShowPositionType.TopRight:
                    ShowAnimationStartPoint.X = SystemParameters.PrimaryScreenWidth;
                    ShowAnimationStartPoint.Y = CreatingArea.Boundary.Y;
                    break;

                default:
                    ShowAnimationStartPoint.X = SystemParameters.PrimaryScreenWidth;
                    ShowAnimationStartPoint.Y = SystemParameters.PrimaryScreenHeight;
                    break;
            }
            CreatingArea.ShowAnimationStartPoint = ShowAnimationStartPoint;

            if (Dispatcher.CheckAccess())
            {
                NotificationAreas.Add(CreatingArea);
            }
            else
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    NotificationAreas.Add(CreatingArea);
                }));
            }

            Monitor.Pulse(NotificationAreas);
            Monitor.Exit(NotificationAreas);

            return CreatingArea;
        }

        public void RemoveNotification(WindowsNotification noti)
        {
            RemoveNotification(noti, null);
        }

        public void RemoveNotification(WindowsNotification noti, Action completedAction)
        {
            if (sticker.Children.Contains(noti))
            {
                DoubleAnimation aniFadeOut = new DoubleAnimation();
                aniFadeOut.From = Opacity;
                aniFadeOut.To = 0.0;
                aniFadeOut.Duration = TimeSpan.FromMilliseconds(250);
                aniFadeOut.FillBehavior = FillBehavior.Stop;
                aniFadeOut.Completed += (s, e) =>
                {
                    sticker.Children.Remove(noti);
                    if (!Monitor.TryEnter(NotificationAreas))
                        Monitor.Wait(NotificationAreas);
                    NotificationAreas.RemoveAll(a => a.NotificationGroupID == noti.NotificationGroupID && a.NotificationID == noti.NotificationID);
                    Monitor.Pulse(NotificationAreas);
                    Monitor.Exit(NotificationAreas);

                    if (completedAction != null)
                    {
                        completedAction.Invoke();
                    }
                    noti = null;
                    if (NotificationAreas.Count < 1) { Hide(); }
                };

                if (Dispatcher.CheckAccess())
                {
                    noti.BeginAnimation(OpacityProperty, aniFadeOut);
                }
                else
                {
                    Dispatcher.Invoke(new Action(() => {
                        noti.BeginAnimation(OpacityProperty, aniFadeOut);
                    }), System.Windows.Threading.DispatcherPriority.Render);
                }
            }
        }

        public void ShowNotification(WindowsNotification noti, TimeSpan showDuration, WindowsNotificationCommon.WindowsNotificationShowPositionType showPositionType, bool autoClose = true)
        {
            if ((!IsActive) || (!IsEnabled) || (!IsVisible))
            {
                IsEnabled = true;
                Visibility = Visibility.Visible;
                Topmost = true;
                Show();
                Activate();
            }

            WindowsNotificationCommon.WindowsNotificationArea NotiArea = CreateNewArea(noti, showPositionType);

            noti.NotificationUILocation = NotiArea.ShowAnimationStartPoint;
            sticker.Children.Add(noti);

            if (Dispatcher.CheckAccess())
            {
                AnimateShow(noti, NotiArea, showDuration, autoClose);
            }
            else
            {
                Dispatcher.Invoke(new Action<WindowsNotification, WindowsNotificationCommon.WindowsNotificationArea, TimeSpan, bool>(AnimateShow), System.Windows.Threading.DispatcherPriority.Render, noti, NotiArea, showDuration, autoClose);
            }
        }
    }
}
