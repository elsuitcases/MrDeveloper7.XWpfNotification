using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace XWpfNotification
{
    public sealed class WpfUINotification : WindowsNotification
    {
        private Grid LayoutRootGrid = null;

        public WpfUINotification(string groupID, string notificationID, string title, string messageBody, DateTime messageTime) 
            : base(WindowsNotificationCommon.WindowsNotificationUIType.WindowsUI, groupID, notificationID, title, messageBody, messageTime)
        {
            LayoutRootGrid = new Grid();
            LayoutRootGrid.BeginInit();
            LayoutRootGrid.Name = "LayoutRoot";
            LayoutRootGrid.Margin = new Thickness(0, 0, 0, 0);
            LayoutRootGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
            LayoutRootGrid.VerticalAlignment = VerticalAlignment.Stretch;
            LayoutRootGrid.Background = BACKGROUND_COLOR;
            Foreground = FOREGROUND_COLOR;
            LayoutRootGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25) });
            LayoutRootGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength() });
            LayoutRootGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25) });

            TextBlock tbTitle = new TextBlock();
            tbTitle.BeginInit();
            tbTitle.Name = "tbTitle";
            tbTitle.SetValue(Grid.RowProperty, 0);
            tbTitle.Margin = new Thickness(5, 5, 5, 5);
            tbTitle.HorizontalAlignment = HorizontalAlignment.Stretch;
            tbTitle.VerticalAlignment = VerticalAlignment.Center;
            tbTitle.TextAlignment = TextAlignment.Left;
            tbTitle.TextWrapping = TextWrapping.NoWrap;
            tbTitle.FontSize = 12;
            tbTitle.FontWeight = FontWeights.Bold;
            tbTitle.Text = NotificationTitle;
            tbTitle.EndInit();
            LayoutRootGrid.Children.Add(tbTitle);

            TextBlock tbMessageBody = new TextBlock();
            tbMessageBody.BeginInit();
            tbMessageBody.Name = "tbMessageBody";
            tbMessageBody.SetValue(Grid.RowProperty, 1);
            tbMessageBody.Margin = new Thickness(5, 5, 5, 5);
            tbMessageBody.HorizontalAlignment = HorizontalAlignment.Stretch;
            tbMessageBody.VerticalAlignment = VerticalAlignment.Center;
            tbMessageBody.TextAlignment = TextAlignment.Left;
            tbMessageBody.TextWrapping = TextWrapping.Wrap;
            tbMessageBody.FontSize = 14;
            tbMessageBody.FontWeight = FontWeights.Normal;
            tbMessageBody.Text = NotificationMessageBody;
            tbMessageBody.EndInit();
            LayoutRootGrid.Children.Add(tbMessageBody);

            TextBlock tbMessageTime = new TextBlock();
            tbMessageTime.BeginInit();
            tbMessageTime.Name = "tbMessageTime";
            tbMessageTime.SetValue(Grid.RowProperty, 2);
            tbMessageTime.Margin = new Thickness(5, 5, 5, 5);
            tbMessageTime.HorizontalAlignment = HorizontalAlignment.Right;
            tbMessageTime.VerticalAlignment = VerticalAlignment.Center;
            tbMessageTime.TextAlignment = TextAlignment.Justify;
            tbMessageTime.TextWrapping = TextWrapping.NoWrap;
            tbMessageTime.FontSize = 10;
            tbMessageTime.FontWeight = FontWeights.Normal;
            tbMessageTime.Text = NotificationTime.ToString();
            tbMessageTime.EndInit();
            LayoutRootGrid.Children.Add(tbMessageTime);

            LayoutRootGrid.EndInit();
            Content = LayoutRootGrid;
        }



        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            LayoutRootGrid.Background = FOREGROUND_COLOR;
            Foreground = BACKGROUND_COLOR;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            LayoutRootGrid.Background = BACKGROUND_COLOR;
            Foreground = FOREGROUND_COLOR;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            MessageBox.Show(String.Format("[{0}] {1}\n- Location = {2}", NotificationTime, NotificationMessageBody, NotificationUILocation), String.Format("[{0}][{1}] {2}", NotificationGroupID, NotificationID, NotificationTitle), MessageBoxButton.OK, MessageBoxImage.Information);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            LayoutRootGrid.Background = FOREGROUND_COLOR;
            Foreground = BACKGROUND_COLOR;
        }
    }
}
