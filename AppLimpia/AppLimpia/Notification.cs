using System;

namespace AppLimpia
{
    /// <summary>
    /// The application notification.
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// No notifications for the current user.
        /// </summary>
        public const int NoNotification = 0;

        /// <summary>
        /// Distance notification.
        /// </summary>
        public const int DistanceNotifications = 1;

        /// <summary>
        /// Notifications one day before.
        /// </summary>
        public const int DayBeforeNotifications = 2;

        /// <summary>
        /// Gets or sets the identifier of the current.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Gets or sets the notification date and time.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the notification text.
        /// </summary>
        public string Message { get; set; }
    }
}
