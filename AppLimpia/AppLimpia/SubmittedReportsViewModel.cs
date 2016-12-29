using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Windows.Input;

using AppLimpia.Json;

using Xamarin.Forms;

namespace AppLimpia
{
    /// <summary>
    /// The ViewModel for the my reports view.
    /// </summary>
    public class SubmittedReportsViewModel : ViewModelBase
    {
        /// <summary>
        /// The dictionary of submitted reports for faster search.
        /// </summary>
        private readonly Dictionary<string, IncidentReport> reportsDictionary;

        /// <summary>
        /// A value indicating whether the user have any submitted reports.
        /// </summary>
        private bool haveReports;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubmittedReportsViewModel"/> class.
        /// </summary>
        /// <param name="submittedReports">My reports collection.</param>
        public SubmittedReportsViewModel(ObservableCollection<IncidentReport> submittedReports)
        {
            // Save reports
            this.HaveReports = submittedReports.Count > 0;
            this.SubmittedReports = submittedReports;
            this.reportsDictionary = new Dictionary<string, IncidentReport>();
            foreach (var report in this.SubmittedReports)
            {
                this.reportsDictionary.Add(report.Id, report);
            }

            // Update the reports from server
            this.UpdateReports();

            // Setup commands
            this.CloseCommand = new Command(this.Close);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user have any submitted reports.
        /// </summary>
        public bool HaveReports
        {
            get
            {
                return this.haveReports;
            }

            // ReSharper disable once MemberCanBePrivate.Global
            // Justification = Used by data binding
            set
            {
                this.SetProperty(ref this.haveReports, value, nameof(this.HaveReports));
            }
        }

        /// <summary>
        /// Gets the collection of submitted reports.
        /// </summary>
        public ObservableCollection<IncidentReport> SubmittedReports { get; }

        /// <summary>
        /// Gets the close command.
        /// </summary>
        public ICommand CloseCommand { get; }

        /// <summary>
        /// Updates the submitted reports from the server.
        /// </summary>
        private void UpdateReports()
        {
            this.IsBusy = true;
            WebHelper.SendAsync(
                Uris.GetGetReportsUri(),
                null,
                this.ProcessUpdateReportsResult,
                () => this.IsBusy = false);
        }

        /// <summary>
        /// Processes the reports update returned by the server.
        /// </summary>
        /// <param name="result">The reports update result.</param>
        private void ProcessUpdateReportsResult(JsonValue result)
        {
            // Process the HAL
            string nextUri;
            var reports = WebHelper.ParseHalCollection(result, "reportes", out nextUri);

            // Parse my reports data
            foreach (var report in reports)
            {
                // Parse reports update data
                this.HaveReports = true;

                // Get my report field
                var id = report.GetItemOrDefault("id").GetStringValueOrDefault(null);
                var dateString =
                    report.GetItemOrDefault("fecha").GetItemOrDefault("date").GetStringValueOrDefault(string.Empty);
                var dropPoint = report.GetItemOrDefault("montonera").GetStringValueOrDefault(null);
                var incident = report.GetItemOrDefault("incidencia").GetStringValueOrDefault(null);
                var status = report.GetItemOrDefault("status").GetStringValueOrDefault(string.Empty);

                // Parse notification date
                DateTime date;
                var parseResult = DateTime.TryParseExact(
                    dateString,
                    "yyyy-MM-dd HH:mm:ss.ffffff",
                    null,
                    System.Globalization.DateTimeStyles.None,
                    out date);

                // If all fields present
                if (!string.IsNullOrEmpty(id) && parseResult && !string.IsNullOrEmpty(dropPoint)
                    && !string.IsNullOrEmpty(incident) && !string.IsNullOrEmpty(status))
                {
                    // If report already exists
                    if (this.reportsDictionary.ContainsKey(id))
                    {
                        this.reportsDictionary[id].Status = status;
                    }
                    else
                    {
                        // Add to submitted reports commection
                        var reportObject = new IncidentReport(id)
                                               {
                                                   Date = date.ToLocalTime(),
                                                   DropPoint = dropPoint,
                                                   Type = incident,
                                                   Status = status
                                               };
                        this.SubmittedReports.Add(reportObject);
                        this.reportsDictionary.Add(id, reportObject);
                    }
                }
            }

            // If new page is present
            if (nextUri != null)
            {
                // Get the favorites from the server
                WebHelper.SendAsync(
                    new Uris.UriMethodPair(new Uri(nextUri), HttpMethod.Get),
                    null,
                    this.ProcessUpdateReportsResult,
                    () => this.IsBusy = false);
            }
            else
            {
                // Hide the progress indicator
                this.IsBusy = false;
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
