using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppLimpia
{
    /// <summary>
    /// Interaction logic for UserDataPage.xaml.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserDataView
    {
        /// <summary>
        /// The current binding context.
        /// </summary>
        private UserDataViewModel currentBindingContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserDataView"/> class.
        /// </summary>
        public UserDataView()
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
            }

            // Set up the event handling from the binding context
            this.currentBindingContext = this.BindingContext as UserDataViewModel;
            if (this.currentBindingContext != null)
            {
                // Set up navigation context
                this.currentBindingContext.Navigation = this.Navigation;
            }

            // Call the base member
            base.OnBindingContextChanged();
        }
    }
}
