using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using AppLimpia.Json;

using Xamarin.Forms;

namespace AppLimpia
{
    /// <summary>
    /// The ViewModel for the IncidentReport view.
    /// </summary>
    public class IncidentReportViewModel : ViewModelBase
    {
        /// <summary>
        /// The incident types dictionary.
        /// </summary>
        private readonly Dictionary<string, string> typesDictionary;

        /// <summary>
        /// The drop point to report incident.
        /// </summary>
        private MapExPin pin;
        
        /// <summary>
        /// The selected incident type.
        /// </summary>
        private int incidentTypeIndex;

        /// <summary>
        /// The encoded report photo.
        /// </summary>
        private byte[] reportPhoto;

        /// <summary>
        /// The report identifier.
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
        /// Reports an error.
        /// </summary>
        /// <param name="args">A <see cref="ErrorReportEventArgs"/> with arguments of the event.</param>
        public void ReportError(ErrorReportEventArgs args)
        {
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
                this.reportPhoto = new byte[stream.Length];
                stream.Position = 0;
                stream.Read(this.reportPhoto, 0, this.reportPhoto.Length);

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
            // Get the incident types from the server
            WebHelper.GetAsync(new Uri(Uris.GetIncidentTypes), this.ParseIncidentTypesData);
        }

        /// <summary>
        /// Parsed the incident type data returned by the server.
        /// </summary>
        /// <param name="json">The incident type data returned by the server.</param>
        private void ParseIncidentTypesData(JsonValue json)
        {
            // Process the HAL
            string nextUri;
            var types = WebHelper.ParseHalCollection(json, "incidencias", out nextUri);

            // Parse incident types data
            foreach (var type in types)
            {
                // Get the incident type field
                var id = type.GetItemOrDefault("id").GetStringValueOrDefault(null);
                var incidentType = type.GetItemOrDefault("incidencia").GetStringValueOrDefault(null);

                // If all fields present
                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(incidentType))
                {
                    // If incident type does not exists
                    if (!this.typesDictionary.ContainsKey(incidentType))
                    {
                        // Add new incident type
                        this.IncidentTypes.Add(incidentType);
                        this.typesDictionary.Add(incidentType, id);
                    }
                }
            }

            // If new page is present
            if (nextUri != null)
            {
                // Get the favorites from the server
                WebHelper.GetAsync(new Uri(nextUri), this.ParseIncidentTypesData);
            }
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
            // ReSharper disable once UseObjectOrCollectionInitializer
            var report = new MultipartFormDataContent();
            report.Add(new StringContent(DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'")), "fecha");
            report.Add(new StringContent(this.pin.Id), "montonera");
            if (this.incidentTypeIndex != -1)
            {
                report.Add(new StringContent(this.IncidentTypes[this.incidentTypeIndex]), "incidencia");
            }

            // TODO: Remove when they are retrieved from the server
            report.Add(new StringContent($"Usuario de {Device.OS}"), "usuario");
            report.Add(new StringContent($"Dispositivo: {Device.OS}"), "device");

            // Add image if any
            if (this.reportPhoto != null)
            {
                var imageContent = new ByteArrayContent(this.reportPhoto);
                imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                report.Add(new ByteArrayContent(this.reportPhoto), "imagen", "imagen.jpg");
            }

            // Send the report to the server
            WebHelper.PostAsync(new Uri(Uris.SubmitReport), report, this.ParseNewIncidentData);
        }

        /// <summary>
        /// Parsed the new incident data returned by the server.
        /// </summary>
        /// <param name="json">The new incident data returned by the server.</param>
        private void ParseNewIncidentData(JsonValue json)
        {
            // Get the new incident report id
            var id = json.GetItemOrDefault("id").GetStringValueOrDefault(null);
            if (!string.IsNullOrEmpty(id))
            {
                this.ReportId = id;
            }

            // Close the current view
            this.Navigation.PopModalAsync();
        }
    }
}
