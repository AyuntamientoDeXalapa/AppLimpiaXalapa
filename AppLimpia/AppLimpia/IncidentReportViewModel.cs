using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Input;

using AppLimpia.Json;
using AppLimpia.Media;
using AppLimpia.Properties;

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
        /// The report photo data.
        /// </summary>
        private Stream reportPhotoData;

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
            this.IncidentTypeIndex = -1;
            this.IncidentTypes = new ObservableCollection<string>();
            this.GetIncidentTypes();

            // Setup commands
            this.TakePhotoCommand = new Command(this.TakePhoto);
            this.ReportIncidentCommand = new Command(this.ReportIncident);
            this.CancelCommand = new Command(this.Cancel);
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
        public int IncidentTypeIndex
        {
            get
            {
                return this.incidentTypeIndex;
            }

            // ReSharper disable once MemberCanBePrivate.Global
            // Justification = Used by data binding
            set
            {
                this.SetProperty(ref this.incidentTypeIndex, value, nameof(this.IncidentTypeIndex));
            }
        }

        /// <summary>
        /// Gets or sets he report photo data.
        /// </summary>
        public Stream ReportPhotoData
        {
            get
            {
                return this.reportPhotoData;
            }

            // ReSharper disable once MemberCanBePrivate.Global
            // Justification = Used by data binding
            set
            {
                this.SetProperty(ref this.reportPhotoData, value, nameof(this.ReportPhotoData));
            }
        }

        /// <summary>
        /// Gets the take photo command.
        /// </summary>
        public ICommand TakePhotoCommand { get; private set; }

        /// <summary>
        /// Gets the report incident command.
        /// </summary>
        public ICommand ReportIncidentCommand { get; private set; }

        /// <summary>
        /// Gets the cancel command.
        /// </summary>
        public ICommand CancelCommand { get; private set; }

        /// <summary>
        /// Takes the photo from the device camera.
        /// </summary>
        private void TakePhoto()
        {
            // If the camera available
            var picker = MediaPicker.Instance;
            if ((picker != null) && picker.IsCameraAvailable)
            {
                // Take the photo
                var options = new CameraMediaStorageOptions();
                var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
                var task = picker.TakePhotoAsync(options);
                if (task != null)
                {
                    // Process taken photo
                    task.ContinueWith(this.OnPhotoChoosen, scheduler);
                }
                else
                {
                    App.DisplayAlert(
                        Localization.ErrorDialogTitle,
                        Localization.ErrorCameraForbidden,
                        Localization.ErrorDialogDismiss);
                }
            }
            else
            {
                App.DisplayAlert(
                    Localization.ErrorDialogTitle,
                    Localization.ErrorCameraUnavailable,
                    Localization.ErrorDialogDismiss);
            }
        }

        /// <summary>
        /// Handles when the photo was chosen.
        /// </summary>
        /// <param name="task">The asynchronous choose operation.</param>
        private void OnPhotoChoosen(Task<MediaFile> task)
        {
            // If the photo was taken
            if (task.Status == TaskStatus.RanToCompletion)
            {
                // Resize and save report photo data
                var mediaFile = task.Result;
                this.ReportPhotoData = MediaPicker.Instance.ResizeImage(mediaFile.Source, 800, 800);
            }
            else if (task.IsFaulted)
            {
                App.DisplayAlert(
                    Localization.ErrorDialogTitle,
                    task.Exception.InnerException.Message,
                    Localization.ErrorDialogDismiss);
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
            this.IsBusy = true;
            WebHelper.SendAsync(
                Uris.GetGetIncidentTypesUri(),
                null,
                this.ProcessGetIncidentTypesResult,
                () => this.IsBusy = false);
        }

        /// <summary>
        /// Parsed the incident type data returned by the server.
        /// </summary>
        /// <param name="result">The incident type data returned by the server.</param>
        private void ProcessGetIncidentTypesResult(JsonValue result)
        {
            // Process the HAL
            string nextUri;
            var types = WebHelper.ParseHalCollection(result, "incidencias", out nextUri);

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
                WebHelper.SendAsync(
                    new Uris.UriMethodPair(new Uri(nextUri), HttpMethod.Get),
                    null,
                    this.ProcessGetIncidentTypesResult,
                    () => this.IsBusy = false);
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

            // Validate that the notification type is valid
            if (this.IncidentTypeIndex < 0)
            {
                App.DisplayAlert(
                    Localization.ErrorDialogTitle,
                    Localization.ErrorInvalidIncidentType,
                    Localization.ErrorDialogDismiss);
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
            report.Add(new StringContent(this.pin.Id), "montonera");
            if (this.incidentTypeIndex != -1)
            {
                report.Add(new StringContent(this.IncidentTypes[this.incidentTypeIndex]), "incidencia");
            }

            // Is report photo is specified
            if (this.reportPhotoData != null)
            {
                // Add photo to the report
                var imageContent = new StreamContent(this.reportPhotoData);
                imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                report.Add(imageContent, "imagen", "imagen.jpg");
            }

            // Send the report to the server
            this.IsBusy = true;
            WebHelper.SendAsync(
                Uris.GetSubmitReportUri(),
                report,
                this.ProcessReportIncidentResult,
                () => this.IsBusy = false,
                timeout: TimeSpan.FromMinutes(5));
        }

        /// <summary>
        /// Parsed the new incident data returned by the server.
        /// </summary>
        /// <param name="result">The new incident data returned by the server.</param>
        private void ProcessReportIncidentResult(JsonValue result)
        {
            // Get the new incident report id
            var id = result.GetItemOrDefault("id").GetStringValueOrDefault(null);
            var status = result.GetItemOrDefault("status").GetStringValueOrDefault(string.Empty);

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

        /// <summary>
        /// Cancels the register task.
        /// </summary>
        private async void Cancel()
        {
            // Return to main view
            await this.Navigation.PopModalAsync();
        }
    }
}
