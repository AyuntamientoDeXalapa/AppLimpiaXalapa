using System;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace AppLimpia.WinPhone
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App
    {
        /// <summary>
        /// The transaction collection.
        /// </summary>
        private TransitionCollection transitions;

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class. 
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            // Create the root frame
            this.CreateRootFrame(e);
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when the application is activated by some means other than normal launching.
        /// </summary>
        /// <param name="args">Event data for the event.</param>
        protected override void OnActivated(IActivatedEventArgs args)
        {
            // If application is activated by protocol
            if (args.Kind == ActivationKind.Protocol)
            {
                // Create the root frame
                this.CreateRootFrame(args);

                // Resume the login process
                var application = Xamarin.Forms.Application.Current as AppLimpia.App;
                if (application != null)
                {
                    var loginViewModel = application.MainPage?.BindingContext as Login.LoginViewModel;
                    var protocolArgs = args as ProtocolActivatedEventArgs;
                    if (protocolArgs != null)
                    {
                        loginViewModel?.ResumeLoginWithCommand?.Execute(protocolArgs.Uri);
                    }
                }

                // Ensure that the current window is active
                Window.Current.Activate();
            }

            base.OnActivated(args);
        }

        /// <summary>
        /// Creates the root frame for the current application.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        private void CreateRootFrame(IActivatedEventArgs e)
        {
            // Get the current root frame
            var rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                // ReSharper disable once UseObjectOrCollectionInitializer
                rootFrame = new Frame();
                rootFrame.CacheSize = 1;

                // Initialize the Xamarin.Forms application
                Xamarin.Forms.Forms.Init(e);
                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.OnRootFrameFirstNavigated;

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                var arg = (e as LaunchActivatedEventArgs)?.Arguments;
                if (!rootFrame.Navigate(typeof(MainPage), arg))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
        }

        /// <summary>
        /// Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private async void OnRootFrameFirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            if (rootFrame != null)
            {
                rootFrame.ContentTransitions = this.transitions
                                               ?? new TransitionCollection { new NavigationThemeTransition() };
                rootFrame.Navigated -= this.OnRootFrameFirstNavigated;
            }

            // Exclude the status bar
            ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
            await StatusBar.GetForCurrentView().HideAsync();
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}