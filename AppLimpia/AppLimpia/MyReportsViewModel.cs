using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        /// The dictionary of submitted reports for faster search.
        /// </summary>
        private readonly Dictionary<string, IncidentReport> reportsDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyReportsViewModel"/> class.
        /// </summary>
        /// <param name="myReports">My reports collection.</param>
        public MyReportsViewModel(IEnumerable<IncidentReport> myReports)
        {
            // Save reports
            this.MyReports = new ObservableCollection<IncidentReport>(myReports);
            this.reportsDictionary = new Dictionary<string, IncidentReport>();
            foreach (var report in this.MyReports)
            {
                this.reportsDictionary.Add(report.Id, report);
            }

            // Update the report statuses from server
            this.GetReportStatuses();

            // Setup commands
            this.CloseCommand = new Command(this.Close);
        }

        /// <summary>
        /// Gets the collection of submitted reports.
        /// </summary>
        public ObservableCollection<IncidentReport> MyReports { get; }

        /// <summary>
        /// Gets the close command.
        /// </summary>
        public ICommand CloseCommand { get; }

        /// <summary>
        /// Gets the incident report statuses from the server.
        /// </summary>
        private void GetReportStatuses()
        {
            WebHelper.GetAsync(new Uri(Uris.GetReports), this.ParseReportStatuses);
        }

        /// <summary>
        /// Parsed the my reports data returned by the server.
        /// </summary>
        /// <param name="json">The my reports data returned by the server.</param>
        private void ParseReportStatuses(JsonValue json)
        {
            // Process the HAL
            string nextUri;
            var reports = WebHelper.ParseHalCollection(json, "reportes", out nextUri);

            // Parse my reports data
            foreach (var report in reports)
            {
                // Get my report field
                var id = report.GetItemOrDefault("id").GetStringValueOrDefault(null);
                var status = report.GetItemOrDefault("status").GetStringValueOrDefault(string.Empty);

                // If report with the provided identifier exists and status is not empty
                if (this.reportsDictionary.ContainsKey(id) && !string.IsNullOrEmpty(status))
                {
                    // Update report status
                    Debug.WriteLine("{0}: New status = {1}", id, status);
                    this.reportsDictionary[id].Status = status;
                }
            }

            // If new page is present
            if (nextUri != null)
            {
                // Get the favorites from the server
                WebHelper.GetAsync(new Uri(nextUri), this.ParseReportStatuses);
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
