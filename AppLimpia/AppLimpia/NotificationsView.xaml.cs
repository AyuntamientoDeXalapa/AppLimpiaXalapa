using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppLimpia
{
    /// <summary>
    /// Interaction logic for NotificationsView.xaml.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotificationsView
    {
        /// <summary>
        /// The current binding context.
        /// </summary>
        private NotificationsViewModel currentBindingContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationsView"/> class.
        /// </summary>
        public NotificationsView()
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
                this.currentBindingContext.Navigation = null;

                // Remove events handler
                this.currentBindingContext.ErrorReported -= this.OnErrorReported;
            }

            // Set up the event handling from the binding context
            this.currentBindingContext = this.BindingContext as NotificationsViewModel;
            if (this.currentBindingContext != null)
            {
                // Set up navigation context
                this.currentBindingContext.Navigation = this.Navigation;

                // Set up event handlers
                this.currentBindingContext.ErrorReported += this.OnErrorReported;
            }

            // Call the base member
            base.OnBindingContextChanged();
        }

        /// <summary>
        /// Handles the ErrorReported event of ViewModel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="EventArgs"/> with arguments of the event.</param>
        private void OnErrorReported(object sender, ErrorReportEventArgs e)
        {
            this.DisplayAlert("Error", e.Exception.ToString(), "OK");
        }
   }
}
