using System;
using System.Threading.Tasks;

using AppLimpia.Media;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppLimpia
{
    /// <summary>
    /// Interaction logic for MainView.xaml.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IncidentReportView
    {
        /// <summary>
        /// The current binding context.
        /// </summary>
        private IncidentReportViewModel currentBindingContext;

        /// <summary>
        /// The incident types synchronizer.
        /// </summary>
        private CollectionSynchronizer<string> typesSynchronizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="IncidentReportView"/> class.
        /// </summary>
        public IncidentReportView()
        {
            // Parse XAML content
            this.InitializeComponent();
            if (this.Resources == null)
            {
                this.Resources = new ResourceDictionary();
            }

            // Copy application resource dictionary
            foreach (var resource in Application.Current.Resources)
            {
                this.Resources.Add(resource.Key, resource.Value);
            }
        }

        /// <summary>
        /// Handles the BindingContextChanged event.
        /// </summary>
        protected override void OnBindingContextChanged()
        {
            // Remove the event handling from the binding context
            if (this.currentBindingContext != null)
            {
                this.typesSynchronizer.Dispose();
                this.typesSynchronizer = null;
                this.currentBindingContext.Navigation = null;
            }

            // Set up the event handling from the binding context
            this.currentBindingContext = this.BindingContext as IncidentReportViewModel;
            if (this.currentBindingContext != null)
            {
                // Set up navigation context
                this.currentBindingContext.Navigation = this.Navigation;

                // Set up event handlers
                this.typesSynchronizer = new CollectionSynchronizer<string>(
                    this.currentBindingContext.IncidentTypes,
                    this.PickerIncidentTypes.Items);
            }

            // Call the base member
            base.OnBindingContextChanged();
        }

        /// <summary>
        /// Handles the TakePhoto event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="EventArgs"/> with arguments of the event.</param>
        private void OnTakePhoto(object sender, EventArgs e)
        {
            var picker = MediaPicker.Instance;
            if ((picker != null) && picker.IsCameraAvailable)
            {
                var options = new CameraMediaStorageOptions();
                var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
                picker.TakePhotoAsync(options).ContinueWith(this.OnPhotoChoosen, scheduler);
            }
            else
            {
                this.DisplayAlert("Error", "La aplicación no puede aceder a camara", "OK");
            }
        }

        /// <summary>
        /// Handles the Canceled event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="EventArgs"/> with arguments of the event.</param>
        private void OnCanceled(object sender, EventArgs e)
        {
            this.Navigation.PopModalAsync();
        }

        /// <summary>
        /// Handles when the photo was chosen.
        /// </summary>
        /// <param name="task">The asynchronous choose operation.</param>
        private void OnPhotoChoosen(Task<MediaFile> task)
        {
            // If the task failed
            if (task.IsFaulted)
            {
                this.DisplayAlert("Error", task.Exception.InnerException.ToString(), "OK");
            }
            else if (task.IsCanceled)
            {
#if DEBUG
                this.DisplayAlert("Canceled", "Canceled", "OK");
#endif
            }
            else
            {
                var mediaFile = task.Result;
                this.currentBindingContext?.SetIncidentReportPhoto(mediaFile.Source);

                this.Image1.Source = ImageSource.FromStream(() => mediaFile.Source);
                this.Image1.IsVisible = true;
            }
        }
    }
}
