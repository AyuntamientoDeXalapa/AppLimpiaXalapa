using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppLimpia.Login
{
    /// <summary>
    /// Interaction logic for StartPage.xaml.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StartView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartView"/> class.
        /// </summary>
        public StartView()
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
    }
}
