using System;

namespace AppLimpia
{
    /// <summary>
    /// The application notification.
    /// </summary>
    public class Notification
    {
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
