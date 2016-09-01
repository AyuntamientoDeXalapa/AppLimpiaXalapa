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
            this.LoadApplication(new App());
        }
    }
}
