using System;
using System.ComponentModel;
using System.IO;

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

                // Remove events handler
                this.PickerIncidentTypes.Items.Clear();
                this.currentBindingContext.PropertyChanged -= this.OnPropertyChanged;
            }

            // Set up the event handling from the binding context
            this.currentBindingContext = this.BindingContext as IncidentReportViewModel;
            if (this.currentBindingContext != null)
            {
                // Set up navigation context
                this.currentBindingContext.Navigation = this.Navigation;

                // Set up event handlers
                this.currentBindingContext.PropertyChanged += this.OnPropertyChanged;
                this.typesSynchronizer = new CollectionSynchronizer<string>(
                    this.currentBindingContext.IncidentTypes,
                    this.PickerIncidentTypes.Items);
            }

            // Call the base member
            base.OnBindingContextChanged();
        }

        /// <summary>
        /// Called when the back button is pressed.
        /// </summary>
        /// <returns><c>true</c> if the button press was handled; <c>false</c> to handle the button by OS.</returns>
        protected override bool OnBackButtonPressed()
        {
            // If no binding context
            if (this.currentBindingContext == null)
            {
                return base.OnBackButtonPressed();
            }

            // Cancel the register operation
            this.currentBindingContext.CancelCommand.Execute(null);
            return true;
        }

        /// <summary>
        /// Handles the PropertyChanged event of ViewModel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="PropertyChangedEventArgs"/> with arguments of the event.</param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // If the report photo data was changed
            if (e.PropertyName == nameof(IncidentReportViewModel.ReportPhotoData))
            {
                // If the image data is available
                if (this.currentBindingContext.ReportPhotoData != null)
                {
                    // Copy image data for display
                    this.ImageReportPhoto.Source =
                        ImageSource.FromStream(() => new MemoryStream(this.currentBindingContext.ReportPhotoData));
                    this.ImageReportPhoto.IsVisible = true;
                }
                else
                {
                    this.ImageReportPhoto.Source = null;
                    this.ImageReportPhoto.IsVisible = false;
                }
            }
        }
    }
}
