using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
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
        /// The event that is raised when a ViewModel is reporting a error.
        /// </summary>
        public event EventHandler<ErrorReportEventArgs> ErrorReported;

        /// <summary>
        /// Gets the collection of notifications.
        /// </summary>
        public ObservableCollection<Notification> Notifications { get; }

        /// <summary>
        /// Gets the close command.
        /// </summary>
        public ICommand CloseCommand { get; }

        /// <summary>
        /// Reports an error.
        /// </summary>
        /// <param name="args">A <see cref="ErrorReportEventArgs"/> with arguments of the event.</param>
        public void ReportError(ErrorReportEventArgs args)
        {
            this.ErrorReported?.Invoke(this, args);
        }

        /// <summary>
        /// Gets the notifications from the server.
        /// </summary>
        private void GetNotifications()
        {
            // Get the favorites from the server
            var task = WebHelper.GetAsync(new Uri(Uris.GetNotifications));

            // Show the favorites on the map
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            task.ContinueWith(this.ParseServerData, scheduler);
        }

        /// <summary>
        /// Parses the server data response.
        /// </summary>
        /// <param name="task">A task that represents the asynchronous server operation.</param>
        private void ParseServerData(Task<JsonValue> task)
        {
            // If the task competed
            System.Diagnostics.Debug.Assert(task.IsCompleted, "Asynchronous task must be completed.");
            if (task.Status == TaskStatus.RanToCompletion)
            {
                this.ParseJson(task.Result);
            }
            else
            {
                // Since task can not be canceled, the only other result is that the task failed
                foreach (var ex in task.Exception.InnerExceptions)
                {
                    this.ReportError(new ErrorReportEventArgs(ex));
                }
            }
        }

        /// <summary>
        /// Parsed the data returned by the server.
        /// </summary>
        /// <param name="json">The data in JSON format.</param>
        private void ParseJson(JsonValue json)
        {
            // If the data is a GeoJson format
            var type = json.GetItemOrDefault("type").GetStringValueOrDefault(string.Empty);
            if (type == "NotificationCollection")
            {
                // Parse notifications collection
                this.ParseNotificationCollection(json);
            }
            else
            {
                // Show the error reported by the server
                // TODO: Add proper exception handling
                var sb = new StringBuilder();
                Json.Json.Write(json, sb);
                this.ReportError(new ErrorReportEventArgs(new Exception(sb.ToString())));
            }
        }

        /// <summary>
        /// Parsed the notifications data returned by the server.
        /// </summary>
        /// <param name="geoJson">The data in GeoJson format.</param>
        private void ParseNotificationCollection(JsonValue geoJson)
        {
            // Parse the notifications collection
            var notifications = geoJson.GetItemOrDefault("notifications", null) as JsonArray;
            if (notifications == null)
            {
                return;
            }

            // Parse each feature
            Debug.WriteLine("Notifications: {0}", notifications.Count);
            foreach (var notification in notifications)
            {
                // Get the notification field
                var id = notification.GetItemOrDefault("id").GetStringValueOrDefault(null);
                var dateString = notification.GetItemOrDefault("date").GetStringValueOrDefault(string.Empty);
                var message = notification.GetItemOrDefault("message").GetStringValueOrDefault(null);

                // Parse notification data
                DateTime date;
                var result = DateTime.TryParseExact(
                    dateString,
                    "dd/MM/yyyy HH:mm:ss",
                    null,
                    DateTimeStyles.None,
                    out date);
                if (!string.IsNullOrEmpty(id) && result && !string.IsNullOrEmpty(message))
                {
                    this.Notifications.Add(new Notification { Id = id, Date = date.ToLocalTime(), Message = message });
                }
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
