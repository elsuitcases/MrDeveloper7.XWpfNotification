using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;

namespace XWpfNotification
{
    static public class WindowsNotificationCommon
    {
        /// <summary>
        /// Notification 간격 기본값
        /// </summary>
        public const double NOTIFICATION_GAP = 10;

        /// <summary>
        /// Notification 가시 시간 기본값
        /// </summary>
        static public readonly TimeSpan NOTIFICATION_SHOW_DURATION = TimeSpan.FromMilliseconds(5000);

        /// <summary>
        /// Notification 표시 위치 유형 기본값
        /// </summary>
        static public readonly WindowsNotificationShowPositionType NOTIFICATION_SHOW_POSITION_TYPE = WindowsNotificationShowPositionType.TopRight;



        /// <summary>
        /// Windows Notification 오버레이 영역 - 단일 Windows Notification에 대한 ID 정보와 오버레이 위치 및 크기에 대한 구조체 입니다.
        /// </summary>
        public struct WindowsNotificationArea
        {
            /// <summary>
            /// Windows Notification 그룹 ID
            /// </summary>
            public string NotificationGroupID;

            /// <summary>
            /// Windows Notification ID
            /// </summary>
            public string NotificationID;

            /// <summary>
            /// Windows Notification 유형
            /// </summary>
            public WindowsNotificationUIType NotificationType;

            /// <summary>
            /// Windows Notification 표시 위치 유형
            /// </summary>
            public WindowsNotificationShowPositionType ShowPositionType;

            /// <summary>
            /// Windows Notification 바운더리 - 위치 및 크기
            /// </summary>
            public Rect Boundary;

            /// <summary>
            /// Windows Notification 오버레이 영역 생성 일시
            /// </summary>
            public DateTime CreatedTime;

            /// <summary>
            /// Windows Notification 표시 애니메이션 시작 지점 Point
            /// </summary>
            public Point ShowAnimationStartPoint;
        }



        /// <summary>
        /// Windows Notification UI 유형
        /// </summary>
        public enum WindowsNotificationUIType
        {
            /// <summary>
            /// 미리 정의된 Windows UI 컨트롤형 토스트
            /// </summary>
            WindowsUI_Toast = 1,

            /// <summary>
            /// 미리 정의된 Windows UI 컨트롤형 Notification
            /// </summary>
            WindowsUI = 2,

            /// <summary>
            /// 사용자 정의 컨텐트 컨트롤형 Notification
            /// </summary>
            CustomContentUI = 3
        }

        /// <summary>
        /// Windows Notification 표시 위치 유형
        /// </summary>
        public enum WindowsNotificationShowPositionType
        {
            /// <summary>
            /// 무작위
            /// </summary>
            Random = 0,

            /// <summary>
            /// 상단 오른쪽
            /// </summary>
            TopRight = 1,

            /// <summary>
            /// 하단 오른쪽
            /// </summary>
            BottomRight = 2,

            /// <summary>
            /// 하단 가운데
            /// </summary>
            BottomCenter = 3
        }
    }
}
