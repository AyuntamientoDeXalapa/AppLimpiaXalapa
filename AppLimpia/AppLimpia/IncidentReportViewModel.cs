using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using AppLimpia.Json;
using System.Windows.Input;

using Xamarin.Forms;
using System.Threading;

namespace AppLimpia
{
    /// <summary>
    /// The ViewModel for the IncidentReport view.
    /// </summary>
    public class IncidentReportViewModel : ViewModelBase
    {
        /// <summary>
        /// The drop point to report incident.
        /// </summary>
        private MapExPin pin;

        /// <summary>
        /// The incident types dictionary.
        /// </summary>
        private Dictionary<string, string> typesDictionary;

        /// <summary>
        /// The selected incident type.
        /// </summary>
        private int incidentTypeIndex;

        /// <summary>
        /// The encoded report photo.
        /// </summary>
        private string reportPhoto;

        /// <summary>
        /// The report identifier
        /// </summary>
        private string reportId;

        /// <summary>
        /// Initializes a new instance of the <see cref="IncidentReportViewModel"/> class.
        /// </summary>
        public IncidentReportViewModel()
        {
            // Get incident types from the server
            this.typesDictionary = new Dictionary<string, string>();
            this.IncidentTypes = new ObservableCollection<string>();
            this.GetIncidentTypes();

            // Setup commands
            this.ReportIncidentCommand = new Command(this.ReportIncident);
        }

        /// <summary>
        /// Gets or sets the drop point for the incident report.
        /// </summary>
        public MapExPin Pin
        {
            get
            {
                return this.pin;
            }

            set
            {
                this.SetProperty(ref this.pin, value, nameof(this.Pin));
            }
        }

        /// <summary>
        /// Gets the incident types collection.
        /// </summary>
        public ObservableCollection<string> IncidentTypes { get; }

        /// <summary>
        /// Gets or sets the selected incident type.
        /// </summary>
        public int IncidentTypeIndex
        {
            get
            {
                return this.incidentTypeIndex;
            }

            set
            {
                this.SetProperty(ref this.incidentTypeIndex, value, nameof(this.IncidentTypeIndex));
            }
        }

        /// <summary>
        /// Gets the current report identifier.
        /// </summary>
        public string ReportId
        {
            get
            {
                return this.reportId;
            }

            private set
            {
                this.SetProperty(ref this.reportId, value, nameof(this.ReportId));
            }
        }

        /// <summary>
        /// Gets the report incident command.
        /// </summary>
        public ICommand ReportIncidentCommand { get; private set; }

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
        /// Sets the incident report photo.
        /// </summary>
        /// <param name="stream">The stream containing the incident report photo.</param>
        public void SetIncidentReportPhoto(Stream stream)
        {
            // If new photo is present
            if (stream != null)
            {
                // Encode photo
                var data = new byte[stream.Length];
                stream.Position = 0;
                stream.Read(data, 0, data.Length);
                this.reportPhoto = Convert.ToBase64String(data);

                // Reset the stream position
                stream.Position = 0;
            }
            else
            {
                // Clear photo
                this.reportPhoto = null;
            }
        }

        /// <summary>
        /// Gets the incident types from the server.
        /// </summary>
        private void GetIncidentTypes()
        {
            // Get the incident report types from the server
            var task = WebHelper.GetAsync(new Uri(Uris.GetIncidentTypes));

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
            if (type == "StringCollection")
            {
                // Parse string collection
                this.ParseStringCollection(json);
            }
            else if (type == "Report")
            {
                // Parse report data
                this.ParseReportData(json);
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
        private void ParseStringCollection(JsonValue json)
        {
            // Parse the string collection
            var strings = json.GetItemOrDefault("strings", null) as JsonArray;
            if (strings == null)
            {
                return;
            }

            // Parse each feature
            Debug.WriteLine("Strings: {0}", strings.Count);
            foreach (var notification in strings)
            {
                // Get the string fields
                var id = notification.GetItemOrDefault("id").GetStringValueOrDefault(null);
                var text = notification.GetItemOrDefault("string").GetStringValueOrDefault(null);

                // Parse string data
                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(text))
                {
                    this.IncidentTypes.Add(text);
                    this.typesDictionary.Add(text, id);
                }
            }
        }

        /// <summary>
        /// Parsed the report data returned by the server.
        /// </summary>
        /// <param name="json">The data in JSON format.</param>
        private void ParseReportData(JsonValue json)
        {
            // Get the report id
            var id = json.GetItemOrDefault("id").GetStringValueOrDefault(null);
            if (!string.IsNullOrEmpty(id))
            {
                this.ReportId = id;
            }
        }

        /// <summary>
        /// Reports the incident.
        /// </summary>
        private void ReportIncident()
        {
            // Prepare report data
            var report = new JsonObject { { "type", "Report" }, { "droppoint", this.pin.Id } };

            // Save the incident type
            var incidentType = this.IncidentTypes[this.incidentTypeIndex];
            var incidentId = this.typesDictionary[incidentType];
            report.Add("incident", incidentId);

            // Add photo if any
            if (!string.IsNullOrEmpty(this.reportPhoto))
            {
                report.Add("photo", this.reportPhoto);
            }

            // Send the report to the server
            var task = WebHelper.PostAsync(new Uri(Uris.SubmitReport), report);

            // Show the favorites on the map
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            var continuation = task.ContinueWith(this.ParseServerData, scheduler);

            // Close the current view
            continuation.ContinueWith(
                _ => this.Navigation.PopModalAsync(),
                default(CancellationToken),
                TaskContinuationOptions.OnlyOnRanToCompletion,
                scheduler);
        }
    }
}
