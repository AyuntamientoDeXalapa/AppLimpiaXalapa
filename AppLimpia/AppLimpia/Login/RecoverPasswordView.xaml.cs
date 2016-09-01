using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppLimpia.Login
{
    /// <summary>
    /// Interaction logic for RegisterView.xaml.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecoverPasswordView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecoverPasswordView"/> class. 
        /// </summary>
        public RecoverPasswordView()
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

        private void Button_OnClicked(object sender, EventArgs e)
        {
            this.Navigation.PopModalAsync();
        }
    }
}
