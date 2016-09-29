using System;

using AppLimpia.Media;

using Foundation;
using UIKit;

#region Generated Code
// To suppress the StyleCop warning
namespace AppLimpia.iOS
#endregion
{
    /// <summary>
    /// Defines the application delegate for the current application.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Class is created by the OS")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "PartialTypeWithSinglePart", Justification = "Generated code")]
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        /// <summary>
        /// Handles the FinishedLaunching event of the application delegate.
        /// </summary>
        /// <param name="app">The current application.</param>
        /// <param name="options">The application options.</param>
        /// <returns><c>true</c> - if the application was launched; <c>false</c> otherwise.</returns>
        /// <remarks>
        ///   <para>
        ///     This method is invoked when the application has loaded and is ready to run. In this
        ///     method you should instantiate the window, load the UI into it and then make the window
        ///     visible.
        ///   </para>
        ///   <para>
        ///     You have 17 seconds to return from this method, or iOS will terminate your application.
        ///   </para>
        /// </remarks>
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            // Initialize the platform dependent components
            Xamarin.Forms.Forms.Init();
            Xamarin.FormsMaps.Init();
            MediaPicker.Instance = new Media.MediaPickerIOS();
            Settings.Instance = new SettingsIOS();

            // Initialize the application
            // ReSharper disable once UseObjectOrCollectionInitializer
            var application = new App();
            application.CurrentCultureInfo = AppDelegate.GetCurrentCultureInfo();
            this.LoadApplication(application);
            return base.FinishedLaunching(app, options);
        }

        /// <summary>
        /// Gets the current culture for application display.
        /// </summary>
        /// <returns>The culture to use for application resources.</returns>
        private static System.Globalization.CultureInfo GetCurrentCultureInfo()
        {
            // Set the fallback language
            var netLanguage = "en";
            var prefLanguageOnly = "en";

            // If preferred language is set
            if (NSLocale.PreferredLanguages.Length > 0)
            {
                var pref = NSLocale.PreferredLanguages[0];
                prefLanguageOnly = pref.Substring(0, 2);
                if (prefLanguageOnly == "pt")
                {
                    // Get the correct Portuguese language
                    pref = pref == "pt" ? "pt-BR" : "pt-PT";
                }

                netLanguage = pref.Replace("_", "-");
            }

            // Create the culture info for the current application
            System.Globalization.CultureInfo ci;
            try
            {
                ci = new System.Globalization.CultureInfo(netLanguage);
            }
            catch
            {
                ci = new System.Globalization.CultureInfo(prefLanguageOnly);
            }

            return ci;
        }
    }
}
