using System;
using System.Globalization;
using System.Reflection;

using Windows.Networking.PushNotifications;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.System.Profile;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http.Filters;

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
            Xamarin.FormsMaps.Init("tQm6TJ6as98ILjiGz0qY7g");
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

            // Setup push notifications channel
            MainPage.GetPushToken();

            // ReSharper disable once UseObjectOrCollectionInitializer
            var application = new AppLimpia.App();
            application.CurrentCultureInfo = CultureInfo.CurrentUICulture;
            application.DeviceId = $"{Xamarin.Forms.Device.OS}:{MainPage.GetDeviceId()}";
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
        /// Gets the device unique identifier.
        /// </summary>
        /// <returns>The device unique identifier.</returns>
        private static string GetDeviceId()
        {
            var token = HardwareIdentification.GetPackageSpecificToken(null);
            var hardwareId = token.Id;

            var hasher = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            var hashed = hasher.HashData(hardwareId);

            return CryptographicBuffer.EncodeToHexString(hashed);
        }

        /// <summary>
        /// Gets the push notification token for the current application.
        /// </summary>
        private static void GetPushToken()
        {
            // Get the channel for notifications
            var channelOperation = PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
            channelOperation.AsTask().ContinueWith(
                task =>
                    {
                        // Send token to the server
                        var token = task.Result.Uri.ToString();
                        AppLimpia.App.SetPushToken($"{Xamarin.Forms.Device.OS}:{token}");
                    });
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
