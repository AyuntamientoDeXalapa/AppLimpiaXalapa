using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppLimpia
{
    /// <summary>
    /// The localization extension helper for XAML localization.
    /// </summary>
    [ContentProperty("Text")]
    public class LocalizeExtension : IMarkupExtension
    {
        /// <summary>
        /// The current culture.
        /// </summary>
        private readonly CultureInfo currentCulture;

        //const string ResourceId = "UsingResxLocalization.Resx.AppResources";

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizeExtension"/> class.
        /// </summary>
        public LocalizeExtension()
        {
            this.currentCulture = ((App)App.Current).CurrentCultureInfo;
        }

        /// <summary>
        /// Gets or sets the text to be localized.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Returns an object that is provided as the value of the target property for this markup extension.
        /// </summary>
        /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
        /// <returns>The object value to set on the property where the extension is applied.</returns>
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            // If no value to be localized
            if (this.Text == null)
            {
                return string.Empty;
            }

            // Get the resource manager
            var manager = AppLimpia.Properties.Localization.ResourceManager;

            // Get the requested string
            var localization = manager.GetString(this.Text, this.currentCulture);

            // If no translation is present
            if (localization == null)
            {
                localization = $"Key '{this.Text}' not found";
            }

            // Return the localized string
            return localization;
        }
    }
}
