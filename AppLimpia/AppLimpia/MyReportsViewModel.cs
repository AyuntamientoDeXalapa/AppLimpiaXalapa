using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using AppLimpia.Json;

using Xamarin.Forms;

namespace AppLimpia
{
    /// <summary>
    /// The ViewModel for the my reports view.
    /// </summary>
    public class MyReportsViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MyReportsViewModel"/> class.
        /// </summary>
        /// <param name="myReports">My reports collection.</param>
        public MyReportsViewModel(IEnumerable<IncidentReport> myReports)
        {
            this.MyReports = new ObservableCollection<IncidentReport>(myReports);

            // Update the report statuses from server
            this.GetReportStatuses();

            // Setup commands
            this.CloseCommand = new Command(this.Close);
        }

        /// <summary>
        /// Gets the collection of notifications.
        /// </summary>
        public ObservableCollection<IncidentReport> MyReports { get; }

        /// <summary>
        /// Gets the close command.
        /// </summary>
        public ICommand CloseCommand { get; }

        /// <summary>
        /// The event that is raised when a ViewModel is reporting a error.
        /// </summary>
        public event EventHandler<ErrorReportEventArgs> ErrorReported;

        /// <summary>
        /// Reports an error.
        /// </summary>
        /// <param name="args">A <see cref="ErrorReportEventArgs"/> with arguments of the event.</param>
        public void ReportError(ErrorReportEventArgs args)
        {
            this.ErrorReported?.Invoke(this, args);
        }

        /// <summary>
        /// Gets the incident types from the server.
        /// </summary>
        private void GetReportStatuses()
        {
            // Prepare status request data
            var statusReport = new JsonObject { { "type", "StatusReport" } };
            var ids = new JsonArray();
            statusReport.Add("ids", ids);

            // Add report ids
            foreach (var report in this.MyReports)
            {
                ids.Add(report.Id);
            }

            // Get the report statuses from the server
            var task = WebHelper.PostAsync(new Uri(Uris.GetReportStatus), statusReport);

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
            if (type == "StatusReport")
            {
                // Parse status report
                this.ParseStatusReport(json);
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
        /// Parsed the string collection data returned by the server.
        /// </summary>
        /// <param name="json">The data in JSON format.</param>
        private void ParseStatusReport(JsonValue json)
        {
            // Parse the status report
            var statuses = json.GetItemOrDefault("statuses", null) as JsonArray;
            if (statuses == null)
            {
                return;
            }

            // Parse each feature
            Debug.WriteLine("Statuses: {0}", statuses.Count);
            foreach (var reportedStatus in statuses)
            {
                // Get the string fields
                var id = reportedStatus.GetItemOrDefault("id").GetStringValueOrDefault(null);
                var statusString = reportedStatus.GetItemOrDefault("status").GetStringValueOrDefault(null);

                // Parse report status
                IncidentReportStatus newStatus;
                var result = Enum.TryParse(statusString, out newStatus);

                // Parse string data
                Debug.WriteLine("{0} => {1}", id, result ? newStatus.ToString() : "null");
                if (!string.IsNullOrEmpty(id) && result)
                {
                    // Update report status
                    // TODO: Optimize
                    foreach (var myReport in this.MyReports)
                    {
                        if (myReport.Id == id)
                        {
                            myReport.Status = newStatus;
                        }
                    }
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
