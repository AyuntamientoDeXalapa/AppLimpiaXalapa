using System;
using System.IO;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace AppLimpia.Droid
{
    // Resolve ambiguous types
    using SecureSettings = Android.Provider.Settings.Secure;

    /// <summary>
    /// Defines the main activity for the Application.
    /// </summary>
    [Activity(
        Label = "Xalapa Limpia",
        Icon = "@drawable/icon",
        MainLauncher = true, 
        ScreenOrientation = ScreenOrientation.Portrait, 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        LaunchMode = LaunchMode.SingleTask)]
    [IntentFilter(new[] { "android.intent.action.VIEW" },
        Categories = new[] { "android.intent.category.DEFAULT", "android.intent.category.BROWSABLE" },
        DataScheme = "mx.gob.xalapa.limpia")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Class is created by the OS")]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        /// <summary>
        /// The view to show at the application start.
        /// </summary>
        internal const string StartView = "StartView";

        /// <summary>
        /// The notification view identifier.
        /// </summary>
        internal const string NotificationsView = "NotificationsView";

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
            Settings.Instance = new SettingsAndroid();

            // Setup exception handler
            AppDomain.CurrentDomain.UnhandledException += MainActivity.CurrentDomainOnUnhandledException;
            this.DisplayCrashReport();

            // Get the device id
            var deviceId = SecureSettings.GetString(this.ContentResolver, SecureSettings.AndroidId);

            // Initialize the application
            // ReSharper disable once UseObjectOrCollectionInitializer
            var application = new App();
            application.CurrentCultureInfo = MainActivity.GetCurrentCultureInfo();
            application.DeviceId = $"{Xamarin.Forms.Device.OS}:{deviceId}";
            application.LaunchUriDelegate = this.LaunchUri;

            // Register for GCM
            var intent = new Intent(this, typeof(RegistrationIntentService));
            this.StartService(intent);

            // Prepare the current intent
            this.LoadApplication(application);
        }

        /// <summary>
        /// Handles the new intent that arrives when the current activity is on top.
        /// </summary>
        /// <param name="intent">The new intent arrived to the current activity.</param>
        protected override void OnNewIntent(Intent intent)
        {
            this.Intent = intent;
            base.OnNewIntent(intent);
        }

        /// <summary>
        /// Handles the resuming of the current activity.
        /// </summary>
        protected override void OnResume()
        {
            MainActivity.HandleIntent(Xamarin.Forms.Application.Current as App, this.Intent);
            base.OnResume();
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

        /// <summary>
        /// Handles the Intent by the current application.
        /// </summary>
        /// <param name="application">The current instance of the application.</param>
        /// <param name="intent">The Intent to be handles by the current application.</param>
        private static void HandleIntent(App application, Intent intent)
        {
            // If no application exists
            if (application == null)
            {
                return;
            }

            // If the current intent is a view action
            if (intent.Action.Equals(Intent.ActionView))
            {
                // If intent have data
                if (intent.Data != null)
                {
                    // Resume the login process
                    var uri = new Uri(intent.DataString);
                    application.HandleUri(uri);
                }
            }
        }

        /// <summary>
        /// Handles the UnhandledException event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="UnhandledExceptionEventArgs"/> with arguments of the event.</param>
        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var newExc = new Exception("CurrentDomainOnUnhandledException", e.ExceptionObject as Exception);
            MainActivity.LogUnhandledException(newExc);
        }

        /// <summary>
        /// Logs the unhandled exception to the log file.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        private static void LogUnhandledException(Exception exception)
        {
            try
            {
                // Write exception data to log
                const string ErrorFileName = "Fatal.log";
                var libraryPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                var errorFilePath = Path.Combine(libraryPath, ErrorFileName);
                var errorMessage = $"Time: {DateTime.Now}\r\nError: Unhandled Exception\r\n{exception}";
                File.WriteAllText(errorFilePath, errorMessage);

                // Log unhandled exception
                System.Diagnostics.Debug.WriteLine("Unhandled exception");
                System.Diagnostics.Debug.WriteLine(exception.ToString());
            }
            catch
            {
                // Ignored
            }
        }

        /// <summary>
        /// Displays the crash report of the last unhandled exception.
        /// </summary>
        private void DisplayCrashReport()
        {
            // If error file does not exist
            const string ErrorFileName = "Fatal.log";
            var libraryPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var errorFilePath = Path.Combine(libraryPath, ErrorFileName);
            if (!File.Exists(errorFilePath))
            {
                return;
            }

            // Show the last exception data
            var errorText = File.ReadAllText(errorFilePath);
            new AlertDialog.Builder(this).SetPositiveButton(
                "Limpiar",
                (sender, args) =>
                    {
                        File.Delete(errorFilePath);
                    }).SetNegativeButton(
                "Cerrar",
                (sender, args) =>
                    {
                        // User pressed Close.
                    }).SetMessage(errorText).SetTitle("Crash Report").Show();
        }

        /// <summary>
        /// Launches the application associated with the specified URI.
        /// </summary>
        /// <param name="uriToLaunch">The URI to launch.</param>
        private void LaunchUri(Uri uriToLaunch)
        {
            var browserIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(uriToLaunch.ToString()));
            this.StartActivity(browserIntent);
        }
    }
}
