using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppLimpia.Login
{
    /// <summary>
    /// Interaction logic for LoginView.xaml.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OauthView
    {
        /// <summary>
        /// The current binding context.
        /// </summary>
        private OauthViewModel currentBindingContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="OauthView"/> class.
        /// </summary>
        public OauthView()
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

            // Setup event handlers
            this.OauthWebView.Navigating += this.OnNavigating;
            this.OauthWebView.Navigated += this.OnNavigated;
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
            }

            // Set up the event handling from the binding context
            this.currentBindingContext = this.BindingContext as OauthViewModel;
            if (this.currentBindingContext != null)
            {
                // Set up navigation context
                this.currentBindingContext.Navigation = this.Navigation;
                this.OauthWebView.Source = this.currentBindingContext.Uri;
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

            // Cancel the OAUTH operation
            this.currentBindingContext.CancelCommand.Execute(null);
            return true;
        }

        /// <summary>
        /// Handles the Navigating event of OauthWebView.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="WebNavigatingEventArgs"/> with arguments of the event.</param>
        private void OnNavigating(object sender, WebNavigatingEventArgs e)
        {
            if (this.currentBindingContext?.OnNavigating(e.Url) == false)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Handles the Navigated event of OauthWebView.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="WebNavigatedEventArgs"/> with arguments of the event.</param>
        private void OnNavigated(object sender, WebNavigatedEventArgs e)
        {
            this.currentBindingContext?.OnNavigated(e.Url);
        }
    }
}
