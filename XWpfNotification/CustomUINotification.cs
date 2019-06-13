using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XWpfNotification
{
    public class CustomUINotification : WindowsNotification
    {
        public object UserState { get; set; } = null;

        public CustomUINotification(string groupID, string notificationID, string title, string messageBody, DateTime messageTime, System.Windows.UIElement uiContent, object userState = null)
            : base(WindowsNotificationCommon.WindowsNotificationUIType.CustomContentUI, groupID, notificationID, title, messageBody, messageTime)
        {
            HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
            VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch;
            Content = uiContent;
            UserState = userState;
        }
    }
}
