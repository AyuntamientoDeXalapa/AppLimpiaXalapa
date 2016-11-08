using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
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
        /// The date and time of the current incident report.
        /// </summary>
        private DateTime? reportDate;

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
        /// The event that is called when a new incident report is created.
        /// </summary>
        public event EventHandler<IncidentReport> OnIncidentReportCreated;

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
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Used in data binding")]
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
        /// Gets the report incident command.
        /// </summary>
        public ICommand ReportIncidentCommand { get; private set; }

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
            // If the current view model is busy do nothing
            if (this.IsBusy)
            {
                return;
            }

            // Get the incident types from the server
            WebHelper.GetAsync(new Uri(Uris.GetIncidentTypes), this.ParseIncidentTypesData, () => this.IsBusy = false);
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
                // TODO: Remove when fixed
                if (id == null)
                {
                    id = Guid.NewGuid().ToString();
                }

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
                WebHelper.GetAsync(new Uri(nextUri), this.ParseIncidentTypesData, () => this.IsBusy = false);
            }
            else
            {
                // Hide the progress indicator
                this.IsBusy = false;
            }
        }

        /// <summary>
        /// Reports the incident.
        /// </summary>
        private void ReportIncident()
        {
            // If the current view model is busy do nothing
            if (this.IsBusy)
            {
                return;
            }

            // Save the report date
            if (!this.reportDate.HasValue)
            {
                this.reportDate = DateTime.UtcNow;
            }

            // Prepare report data
            // ReSharper disable once UseObjectOrCollectionInitializer
            var report = new MultipartFormDataContent();
            report.Add(new StringContent(this.reportDate.Value.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'")), "fecha");
            report.Add(new StringContent(this.pin.Id), "montonera");
            if (this.incidentTypeIndex != -1)
            {
                report.Add(new StringContent(this.IncidentTypes[this.incidentTypeIndex]), "incidencia");
            }

            // TODO: Remove when they are retrieved from the server
            var uid = $"Usuario de {Device.OS}";
            if (Settings.Instance.Contains(Settings.UserId))
            {
                uid = Settings.Instance.GetValue(Settings.UserId, string.Empty);
            }

            report.Add(new StringContent(uid), "usuario");
            report.Add(new StringContent(((App)Application.Current).DeviceId), "device");

            // Add image if any
            if (this.reportPhoto != null)
            {
                var imageContent = new ByteArrayContent(this.reportPhoto);
                imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                report.Add(new ByteArrayContent(this.reportPhoto), "imagen", "imagen.jpg");
            }

            // Send the report to the server
            this.IsBusy = true;
            WebHelper.PostAsync(
                new Uri(Uris.SubmitReport),
                report,
                this.ParseNewIncidentData,
                () => this.IsBusy = false);
        }

        /// <summary>
        /// Parsed the new incident data returned by the server.
        /// </summary>
        /// <param name="json">The new incident data returned by the server.</param>
        private void ParseNewIncidentData(JsonValue json)
        {
            // Get the new incident report id
            var id = json.GetItemOrDefault("id").GetStringValueOrDefault(null);
            var status = json.GetItemOrDefault("status").GetStringValueOrDefault(string.Empty);

            if (!string.IsNullOrEmpty(id))
            {
                // Create a new incident report
                var date = this.reportDate ?? DateTime.UtcNow;
                var report = new IncidentReport(id)
                                 {
                                     Date = date.ToLocalTime(),
                                     DropPoint = this.pin.Label,
                                     Type = this.IncidentTypes[this.incidentTypeIndex],
                                     Status = status
                };

                // Raise incident reported created event
                this.OnIncidentReportCreated?.Invoke(this, report);
            }

            // End the submission process
            this.IsBusy = false;

            // Close the current view
            this.Navigation.PopModalAsync();
        }
    }
}
