using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

using AppLimpia.Json;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppLimpia
{
    /// <summary>
    /// Defines the main Xamarin.Froms application.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class App
    {
        /// <summary>
        /// A value indicating whether the alert dialog is shown.
        /// </summary>
        private static bool alertShown;

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class. 
        /// </summary>
        public App()
        {
            // Parse the XAML file
            this.InitializeComponent();

            // Get the stored user identifier
            var accessToken = string.Empty;
            if (Settings.Instance.Contains(Settings.AccessToken))
            {
                accessToken = Settings.Instance.GetValue(Settings.UserId, string.Empty);
                Debug.WriteLine("Access Token  = " + accessToken);
            }

            // If user is not logged in
            if (string.IsNullOrEmpty(accessToken))
            {
                // Show the login page
                var startViewModel = new Login.LoginViewModel();
                var startPage = new Login.LoginView { BindingContext = startViewModel };
                this.MainPage = startPage;
            }
            else
            {
                App.ShowMainView();
            }
        }

        /// <summary>
        /// Gets or sets the current culture info for the current application.
        /// </summary>
        internal CultureInfo CurrentCultureInfo
        {
            get
            {
                return AppLimpia.Properties.Localization.Culture;
            }

            set
            {
                Debug.WriteLine("New culture {0}", value);
                AppLimpia.Properties.Localization.Culture = value;
            }
        }

        /// <summary>
        /// Gets or sets the device unique identifier.
        /// </summary>
        internal string DeviceId { get; set; }

        /// <summary>
        /// Gets the device push notification token.
        /// </summary>
        internal string PushToken { get; private set; }

        /// <summary>
        /// Gets or sets the launch URI delegate.
        /// </summary>
        internal Action<Uri> LaunchUriDelegate { get; set; }

        /// <summary>
        /// Gets or sets the main view model.
        /// </summary>
        private MainViewModel MainViewModel { get; set; }

        /// <summary>
        /// Replaces the main view of the current application.
        /// </summary>
        /// <param name="mainView">The main view to be shown.</param>
        internal static void ReplaceMainView(Page mainView)
        {
            // Get the current application instance
            var instance = Application.Current as App;
            if (instance != null)
            {
                instance.MainViewModel = null;
                instance.MainPage = mainView;
            }
        }

        /// <summary>
        /// Shows the main application view.
        /// </summary>
        internal static void ShowMainView()
        {
            // Get the current application instance
            var instance = Application.Current as App;
            if (instance != null)
            {
                // Initialize the main page
                instance.MainViewModel = new MainViewModel { IsActive = true };
                var mainView = new MainView { BindingContext = instance.MainViewModel };
                instance.MainPage = mainView;

                // Android does not fire Appearing event
                if (Device.OS != TargetPlatform.Android)
                {
                    // Initialize the main view model when the main view is loaded
                    mainView.Appearing += instance.OnMainViewAppearing;
                }
                else
                {
                    // Initialize the main view model
                    Device.BeginInvokeOnMainThread(() => instance.MainViewModel.Initialize());
                }
            }
        }

        /// <summary>
        /// Presents an alert dialog to the application user with a single cancel button.
        /// </summary>
        /// <param name="title">The title of the alert dialog.</param>
        /// <param name="message">The body text of the alert dialog.</param>
        /// <param name="cancel">Text to be displayed on the 'Cancel' button.</param>
        /// <returns>A <see cref="Task"/> representing an alert dialog.</returns>
        internal static Task DisplayAlert(string title, string message, string cancel)
        {
            // If alert is not shown
            if (!App.alertShown)
            {
                // Display alert to the user
                try
                {
                    App.alertShown = true;
                    var task = Application.Current.MainPage.DisplayAlert(title, message, cancel);
                    task.ContinueWith(_ => App.alertShown = false);
                    return task;
                }
                catch (Exception)
                {
                    App.alertShown = false;
                    throw;
                }
            }

            Debug.WriteLine("DisplayAlert(\"{0}\", \"{1}\") called twice", title, message);
            return null;
        }

        /// <summary>
        /// Presents an alert dialog to the application user with an accept and a cancel button.
        /// </summary>
        /// <param name="title">The title of the alert dialog.</param>
        /// <param name="message">The body text of the alert dialog.</param>
        /// <param name="accept">Text to be displayed on the 'Accept' button.</param>
        /// <param name="cancel">Text to be displayed on the 'Cancel' button.</param>
        /// <returns>A <see cref="Task"/> representing an alert dialog.</returns>
        internal static Task<bool> DisplayAlert(string title, string message, string accept, string cancel)
        {
            // If alert is not shown
            if (!App.alertShown)
            {
                // Display alert to the user
                try
                {
                    App.alertShown = true;
                    var task = Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
                    task.ContinueWith(_ => App.alertShown = false);
                    return task;
                }
                catch (Exception)
                {
                    App.alertShown = false;
                    throw;
                }
            }

            Debug.WriteLine("DisplayAlert(\"{0}\", \"{1}\") called twice", title, message);
            return null;
        }

        /// <summary>
        /// Sets the push notification token used for the current application.
        /// </summary>
        /// <param name="pushToken">The device push notification token.</param>
        internal static void SetPushToken(string pushToken)
        {
            // If push token have not been changed
            Debug.WriteLine("New push token available: " + pushToken);
            var currentPushToken = Settings.Instance.GetValue(Settings.PushToken, string.Empty);
            if (string.Compare(currentPushToken, pushToken, StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                // Do nothing
                return;
            }

            // Get the current application instance
            var instance = Application.Current as App;
            if (instance != null)
            {
                // Set the push token
                instance.PushToken = pushToken;
            }

            // If user is logged in
            if (Settings.Instance.Contains(Settings.AccessToken))
            {
                // Prepare the data to be send to the server
                var request = new Json.JsonObject
                                  {
                                          { "username", Settings.Instance.GetValue(Settings.UserName, string.Empty) },
                                          { "push_token", pushToken }
                                  };

                // Send request to the server
                Debug.WriteLine("Update push token on server");
                WebHelper.SendAsync(
                    Uris.GetChangePushTokenUri(),
                    request.AsHttpContent(),
                    _ => Settings.Instance.SetValue(Settings.PushToken, pushToken));
            }
        }

        /// <summary>
        /// Handles the application start event.
        /// </summary>
        protected override void OnStart()
        {
            Debug.WriteLine($"Device ID: {this.DeviceId}");
        }

        /// <summary>
        /// Handles the application sleep event.
        /// </summary>
        protected override void OnSleep()
        {
            // Stop the timer restarts
            Debug.WriteLine("OnSleep");
            if (this.MainViewModel != null)
            {
                this.MainViewModel.IsActive = false;
            }
        }

        /// <summary>
        /// Handles the application resume event.
        /// </summary>
        protected override void OnResume()
        {
            // Restart the timer
            Debug.WriteLine("OnResume");
            if (this.MainViewModel != null)
            {
                this.MainViewModel.IsActive = true;
            }
        }

        /// <summary>
        /// Handles the Appearing event of MainView.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="EventArgs"/> with arguments of the event.</param>
        private void OnMainViewAppearing(object sender, EventArgs e)
        {
            // Unregister event
            ((Page)sender).Appearing -= this.OnMainViewAppearing;

            // Initialize the main view model
            this.MainViewModel.Initialize();
        }
    }
}
