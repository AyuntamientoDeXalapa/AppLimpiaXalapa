using System;
using System.Globalization;
using System.Reflection;

using Windows.UI.Xaml.Navigation;
using Windows.Web.Http.Filters;

#if !DEBUG
#error Generate map authentication token https://msdn.microsoft.com/en-us/library/dn741528.aspx
#endif

namespace AppLimpia.WinPhone
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            // Initialize the platform dependent components
            Xamarin.FormsMaps.Init(string.Empty);
            Media.MediaPicker.Instance = new MediaPickerWinPhone();
            Settings.Instance = new SettingsWinPhone();

            // Initialize the application
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;

            // Change the resource manager
            var manager = new WindowsRuntimeResourceManager(
                typeof(AppLimpia.Properties.Localization).FullName,
                typeof(AppLimpia.Properties.Localization).GetTypeInfo().Assembly);
            AppLimpia.Properties.Localization.SetResourceManager(manager);

            // Setup factory
            AppLimpia.WebHelper.SetFactory(this.HttpClientFactory);

            // ReSharper disable once UseObjectOrCollectionInitializer
            var application = new AppLimpia.App();
            application.CurrentCultureInfo = CultureInfo.CurrentUICulture;
            this.LoadApplication(application);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        /// <summary>
        /// Creates the instance of the <see cref="System.Net.Http.HttpClient"/>.
        /// </summary>
        /// <returns>A new instance of the <see cref="System.Net.Http.HttpClient"/>.</returns>
        private System.Net.Http.HttpClient HttpClientFactory()
        {
            var filter = new HttpBaseProtocolFilter();
            var client = new System.Net.Http.HttpClient(new WindowsHttpMessageHandler(filter));
            return client;
        }
    }
}
