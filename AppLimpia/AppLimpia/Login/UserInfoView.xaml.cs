using System;

using AppLimpia.Properties;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppLimpia.Login
{
    /// <summary>
    /// Interaction logic for UserDataPage.xaml.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserInfoView
    {
        /// <summary>
        /// The current binding context.
        /// </summary>
        private UserInfoViewModel currentBindingContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInfoView"/> class.
        /// </summary>
        public UserInfoView()
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

            // Fill notification types combobox
            this.PickerNotificationTypes.Items.Add(Localization.NoNotification);
            this.PickerNotificationTypes.Items.Add(Localization.DistanceNotifications);
            this.PickerNotificationTypes.Items.Add(Localization.DayBeforeNotifications);
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
            this.currentBindingContext = this.BindingContext as UserInfoViewModel;
            if (this.currentBindingContext != null)
            {
                // Set up navigation context
                this.currentBindingContext.Navigation = this.Navigation;
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
    }
}
