﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;

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
        /// Initializes a new instance of the <see cref="App"/> class. 
        /// </summary>
        public App()
        {
            // Parse the XAML file
            this.InitializeComponent();

            // Show the start page
            var startViewModel = new Login.StartViewModel();
            var startPage = new Login.StartView { BindingContext = startViewModel };
            this.MainPage = startPage;
        }

        /// <summary>
        /// Gets or sets the main view model.
        /// </summary>
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
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
                instance.MainPage = Device.OnPlatform<Page>(
                    new NavigationPage(mainView),
                    new NavigationPage(mainView),
                    mainView);

                // Android does not fire Appearing event so subscribe to Disappearing event
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
            return Application.Current.MainPage.DisplayAlert(title, message, cancel);
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
            return Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
        }

        /// <summary>
        /// Handles the application start event.
        /// </summary>
        protected override void OnStart()
        {
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