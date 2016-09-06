using System;

using Android.App;
using Android.Content.PM;
using Android.OS;

namespace AppLimpia.Droid
{
    /// <summary>
    /// Defines the main activity for the Application.
    /// </summary>
    [Activity(Label = "AppLimpia", Icon = "@drawable/icon", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Class is created by the OS")]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        /// <summary>
        /// Handles the creation of the current <see cref="MainActivity"/>.
        /// </summary>
        /// <param name="bundle">The saved instance state.</param>
        protected override void OnCreate(Bundle bundle)
        {
            // Call the base member
            base.OnCreate(bundle);

            // Initialize the platform dependent components
            Xamarin.FormsMaps.Init(this, bundle);
            Xamarin.Forms.Forms.Init(this, bundle);
            Media.MediaPicker.Instance = new MediaPickerDroid();

            // Initialize the application
            // ReSharper disable once UseObjectOrCollectionInitializer
            var application = new App();
            application.CurrentCultureInfo = MainActivity.GetCurrentCultureInfo();
            this.LoadApplication(application);
        }

        /// <summary>
        /// Gets the current culture for application display.
        /// </summary>
        /// <returns>The culture to use for application resources.</returns>
        private static System.Globalization.CultureInfo GetCurrentCultureInfo()
        {
            var androidLocale = Java.Util.Locale.Default;
            var netLanguage = androidLocale.ToString().Replace("_", "-");
            return new System.Globalization.CultureInfo(netLanguage);
        }
    }
}
