using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows.Input;

using AppLimpia.Json;

using Xamarin.Forms;

namespace AppLimpia
{
    /// <summary>
    /// The ViewModel for the Notifications view.
    /// </summary>
    public class NotificationsViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationsViewModel"/> class.
        /// </summary>
        public NotificationsViewModel()
        {
            // Get notifications from the server
            this.Notifications = new ObservableCollection<Notification>();
            this.GetNotifications();

            // Setup commands
            this.CloseCommand = new Command(this.Close);
        }

        /// <summary>
        /// Gets the collection of notifications.
        /// </summary>
        public ObservableCollection<Notification> Notifications { get; }

        /// <summary>
        /// Gets the close command.
        /// </summary>
        public ICommand CloseCommand { get; }

        /// <summary>
        /// Gets the notifications from the server.
        /// </summary>
        private void GetNotifications()
        {
            // Get the favorites from the server
            WebHelper.GetAsync(new Uri(Uris.GetNotifications), this.ParseNotificationData);
        }

        /// <summary>
        /// Parsed the notification data returned by the server.
        /// </summary>
        /// <param name="json">The notification data returned by the server.</param>
        private void ParseNotificationData(JsonValue json)
        {
            // Process the HAL
            string nextUri;
            var notifications = WebHelper.ParseHalCollection(json, "notificaciones_usuario", out nextUri);

            // Parse notification data
            foreach (var notification in notifications)
            {
                // Get the notification field
                var id = notification.GetItemOrDefault("id").GetStringValueOrDefault(null);
                var dateString =
                    notification.GetItemOrDefault("fecha")
                        .GetItemOrDefault("date")
                        .GetStringValueOrDefault(string.Empty);
                var message = notification.GetItemOrDefault("mensaje").GetStringValueOrDefault(null);

                // Parse notification date
                DateTime date;
                var result = DateTime.TryParseExact(
                    dateString,
                    "yyyy-MM-dd HH:mm:ss.ffffff",
                    null,
                    DateTimeStyles.None,
                    out date);

                // If all fields present
                if (!string.IsNullOrEmpty(id) && result && !string.IsNullOrEmpty(message))
                {
                    // Add new notification
                    var notificationObject = new Notification { Id = id, Date = date.ToLocalTime(), Message = message };
                    this.Notifications.Add(notificationObject);
                }
            }

            // If new page is present
            if (nextUri != null)
            {
                // Get the favorites from the server
                WebHelper.GetAsync(new Uri(nextUri), this.ParseNotificationData);
            }
        }

        /// <summary>
        /// Closes the current view model and associated view.
        /// </summary>
        private void Close()
        {
            this.Navigation.PopModalAsync();
        }
    }
}
