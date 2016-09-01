using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppLimpia
{
    /// <summary>
    /// Interaction logic for MyReportsView.xaml.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyReportsView
    {
        /// <summary>
        /// The current binding context.
        /// </summary>
        private MyReportsViewModel currentBindingContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyReportsView"/> class.
        /// </summary>
        public MyReportsView()
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
            this.currentBindingContext = this.BindingContext as MyReportsViewModel;
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
