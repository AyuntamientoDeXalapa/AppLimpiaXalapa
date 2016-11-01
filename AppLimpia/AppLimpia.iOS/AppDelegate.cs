using System;
using System.Diagnostics;
using System.IO;

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

            // Setup exception handler
            AppDomain.CurrentDomain.UnhandledException += AppDelegate.CurrentDomainOnUnhandledException;
            AppDelegate.DisplayCrashReport();

            // Request notification permission
            var pushSettings =
                UIUserNotificationSettings.GetSettingsForTypes(
                    UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                    new NSSet());
            UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);

            // Get the device identifier
#if !TARGET_IPHONE_SIMULATOR
            var deviceUuid = UIDevice.CurrentDevice.IdentifierForVendor;
#else
            var deviceUuid = new NSUuid("CA90424A-5E89-468E-BC7B-9DE7D82FC02D");
#endif

            // Initialize the application
            // ReSharper disable once UseObjectOrCollectionInitializer
            var application = new App();
            application.CurrentCultureInfo = AppDelegate.GetCurrentCultureInfo();
            application.DeviceId = $"{Xamarin.Forms.Device.OS}:{deviceUuid.AsString()}";
            this.LoadApplication(application);
            return base.FinishedLaunching(app, options);
        }

        /// <summary>
        /// Called to tell the delegate the types of local and remote notifications that can be used to get the
        /// user’s attention.
        /// </summary>
        /// <param name="application">The application object that registered the user notification settings.</param>
        /// <param name="notificationSettings">
        ///   The user’s specified notification settings for your application. The settings in this object may be
        ///   different than the ones you originally requested.
        /// </param>
        public override void DidRegisterUserNotificationSettings(
            UIApplication application,
            UIUserNotificationSettings notificationSettings)
        {
            // Register for remote notifications
            UIApplication.SharedApplication.RegisterForRemoteNotifications();
        }

        /// <summary>
        /// Tells the delegate that the application successfully registered with Apple Push Notification service.
        /// </summary>
        /// <param name="application">
        ///   The application object that initiated the remote-notification registration process.
        /// </param>
        /// <param name="deviceToken">A token that identifies the device to APNs.</param>
        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            /*
             * let tokenChars = UnsafePointer<CChar>(deviceToken.bytes)
             * var tokenString = ""
             * 
             * for i in 0..<deviceToken.length {
             *     tokenString += String(format: "%02.2hhx", arguments: [tokenChars[i]])
             * }
             * 
             * print("Device Token:", tokenString)
             */

            // Get current device token
            var token = deviceToken.Description;
            if (!string.IsNullOrWhiteSpace(token))
            {
                token = token.Trim('<').Trim('>');
            }

            // Report error to user
            Debug.WriteLine("Notifications token: " + token);
            ////new UIAlertView("Registered for push notifications", token, null, "OK", null).Show();
        }

        /// <summary>
        /// Sent to the delegate when Apple Push Notification service cannot successfully complete the registration
        /// process.
        /// </summary>
        /// <param name="application">
        ///   The application object that initiated the remote-notification registration process.
        /// </param>
        /// <param name="error">An NSError object that encapsulates information why registration did not succeed.</param>
        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            // Report error to user
            new UIAlertView("Error de registrar con servicio de notificaciones", error.LocalizedDescription, null, "OK", null).Show();
            Debug.WriteLine("Error registering push notifications: " + error.LocalizedDescription);
        }

        /// <summary>
        /// Tells the application that a remote notification arrived .
        /// </summary>
        /// <param name="application">The application object that received the remote notification.</param>
        /// <param name="userInfo">A dictionary that contains information related to the remote notification.</param>
        public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        {
            Debug.WriteLine("Received remote notification");
        }

        /// <summary>
        /// Gets the current culture for application display.
        /// </summary>
        /// <returns>The culture to use for application resources.</returns>
        private static System.Globalization.CultureInfo GetCurrentCultureInfo()
        {
            // Set the fall back language
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

        /// <summary>
        /// Handles the UnhandledException event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="UnhandledExceptionEventArgs"/> with arguments of the event.</param>
        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var newExc = new Exception("CurrentDomainOnUnhandledException", e.ExceptionObject as Exception);
            AppDelegate.LogUnhandledException(newExc);
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
                var libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Resources);
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
        private static void DisplayCrashReport()
        {
            // If error file does not exist
            const string ErrorFileName = "Fatal.log";
            var libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Resources);
            var errorFilePath = Path.Combine(libraryPath, ErrorFileName);
            if (!File.Exists(errorFilePath))
            {
                return;
            }

            // Show the last exception data
            var errorText = File.ReadAllText(errorFilePath);
            var alertView = new UIAlertView("Crash Report", errorText, null, "Cerrar", "Limpiar")
                                {
                                    UserInteractionEnabled = true
                                };
            alertView.Clicked += (sender, args) =>
                {
                    if (args.ButtonIndex != 0)
                    {
                        File.Delete(errorFilePath);
                    }
                };
            alertView.Show();
        }
    }
}
