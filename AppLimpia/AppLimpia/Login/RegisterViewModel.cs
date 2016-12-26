using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

using AppLimpia.Json;

using Xamarin.Forms;

namespace AppLimpia.Login
{
    /// <summary>
    /// The ViewModel for the Register view.
    /// </summary>
    public class RegisterViewModel : ViewModelBase
    {
        /// <summary>
        /// The register task completion source.
        /// </summary>
        private readonly TaskCompletionSource<bool> completionSource;

        /// <summary>
        /// A value indicating whether the registration process is in progress.
        /// </summary>
        private bool isRegistering;

        /// <summary>
        /// The notification type index.
        /// </summary>
        private int notificationTypeIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterViewModel"/> class.
        /// </summary>
        /// <param name="completionSource">The register task completion source.</param>
        public RegisterViewModel(TaskCompletionSource<bool> completionSource)
        {
            // Store the completion source
            this.completionSource = completionSource;
            this.isRegistering = false;

            // Setup default values
            this.Login = string.Empty;
            this.FullName = string.Empty;
            this.Password = string.Empty;
            this.PasswordConfirm = string.Empty;
            this.NotificationTypeIndex = -1;

            // Setup commands
            this.RegisterCommand = new Command(this.Register);
            this.CancelCommand = new Command(this.Cancel);
        }

        /// <summary>
        /// Gets the registered user identifier.
        /// </summary>
        public string UserId { get; private set; }

        /// <summary>
        /// Gets or sets the full user name.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        // Justification = Used by data binding
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the user name (login) for registration.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        // Justification = Used by data binding
        public string Login { get; set; }

        /// <summary>
        /// Gets or sets the user password.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        // Justification = Used by data binding
        public string Password { private get; set; }

        /// <summary>
        /// Gets or sets the user password confirmation.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        // Justification = Used by data binding
        public string PasswordConfirm { private get; set; }

        /// <summary>
        /// Gets or sets the notification type index.
        /// </summary>
        public int NotificationTypeIndex
        {
            get
            {
                return this.notificationTypeIndex;
            }

            // ReSharper disable once MemberCanBePrivate.Global
            // Justification = Used by data binding
            set
            {
                this.SetProperty(ref this.notificationTypeIndex, value, nameof(this.NotificationTypeIndex));
            }
        }

        /// <summary>
        /// Gets the register command.
        /// </summary>
        public ICommand RegisterCommand { get; private set; }

        /// <summary>
        /// Gets the cancel command.
        /// </summary>
        public ICommand CancelCommand { get; private set; }

        /// <summary>
        /// Performs the user registration.
        /// </summary>
        private void Register()
        {
            // If already registering
            if (this.isRegistering)
            {
                return;
            }

            // Validate that the user full name is not empty
            if (this.FullName.Length < 2)
            {
                // TODO: Localize
                App.DisplayAlert("Error", "Debe de proporcionar un nombre completo valido", "OK");
                return;
            }

            // Validate that the user name is a valid email
            // TODO: Change to email or phone number
            var login = this.Login.Trim().ToLower();
            var isEmail = Regex.IsMatch(
                login,
                @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
                RegexOptions.IgnoreCase);
            if (!isEmail)
            {
                // TODO: Localize
                App.DisplayAlert("Error", "Debe de proporcionar un email valido", "OK");
                return;
            }

            // Validate that the password is present
            if (this.Password.Length <= 2)
            {
                // TODO: Localize
                App.DisplayAlert("Error", "Debe de proporcionar una contraseña valida", "OK");
                return;
            }

            // Validate that the password and confirmation coincide
            if (this.Password != this.PasswordConfirm)
            {
                // TODO: Localize
                App.DisplayAlert("Error", "La contraseña y su confirmacion no coinciden", "OK");
                return;
            }

            // Validate that the notification type is valid
            if (this.NotificationTypeIndex < 0)
            {
                // TODO: Localize
                App.DisplayAlert("Error", "Debe de selecionar el tipo de notificación", "OK");
                return;
            }

            // Prepare the data to be send to the server
            var registrationForm = new Json.JsonObject
                                       {
                                               { "username", login },
                                               { "name", this.FullName.Trim() },
                                               { "password", this.Password },
                                               { "notification_types", this.NotificationTypeIndex }
                                       };
            var builder = new StringBuilder();
            Json.Json.Write(registrationForm, builder);
            Debug.WriteLine("Request: " + builder);
            var request = new StringContent(builder.ToString(), Encoding.UTF8, "application/json");

            // Send request to the server
            this.isRegistering = true;
            WebHelper.PostAsync(
                new Uri(Uris.Register),
                request,
                this.ProcessRegistrationResults,
                () => this.isRegistering = false);
        }

        /// <summary>
        /// Processes the registration result returned by the server.
        /// </summary>
        /// <param name="result">The registration result.</param>
        private void ProcessRegistrationResults(JsonValue result)
        {
            // The register user ID
            // TODO: Change to OAUTH token
            this.UserId = result.GetItemOrDefault("id").GetStringValueOrDefault(string.Empty);
            Settings.Instance.SetValue(Settings.UserName, this.Login);
            Settings.Instance.SetValue(Settings.UserId, this.UserId);
            Debug.WriteLine("User ID = " + this.UserId);

            ////// End the registration process
            ////this.isRegistering = false;

            ////// Return to login view
            ////await this.Navigation.PopModalAsync();

            ////// Signal the task completion
            ////this.completionSource.SetResult(true);

            // TODO: Remove when register returns valid tokens
            // Prepare the data to be send to the server
            var login = this.Login.Trim().ToLower();
            var deviceId = ((App)Application.Current).DeviceId;
            var loginForm = new Json.JsonObject
                                {
                                        { "username", login },
                                        { "password", this.Password },
                                        { "device", deviceId }
                                };

            // If push token exists
            var pushToken = ((App)Application.Current).PushToken;
            if (!string.IsNullOrEmpty(pushToken))
            {
                loginForm.Add("push_token", pushToken);
            }

            // Format data
            var builder = new StringBuilder();
            Json.Json.Write(loginForm, builder);
            Debug.WriteLine("Request: " + builder);
            var request = new StringContent(builder.ToString(), Encoding.UTF8, "application/json");

            // Send request to the server
            WebHelper.PostAsync(
                new Uri(Uris.Login),
                request,
                this.ProcessLoginResults,
                () => this.isRegistering = false);

        }

        /// <summary>
        /// Processes the login result returned by the server.
        /// </summary>
        /// <param name="result">The login result.</param>
        // TODO: Remove
        private async void ProcessLoginResults(JsonValue result)
        {
            // The register user ID
            // TODO: Remove USER ID
            var userId = result.GetItemOrDefault("user_id").GetStringValueOrDefault(string.Empty);
            var accessToken = result.GetItemOrDefault("access_token").GetStringValueOrDefault(string.Empty);
            var expiresIn = result.GetItemOrDefault("expires_in").GetIntValueOrDefault(24 * 60 * 60);
            var refreshToken = result.GetItemOrDefault("refresh_token").GetStringValueOrDefault(string.Empty);

            // End the login process
            this.isRegistering = false;

            // If all of the fields are returned
            var login = this.Login.Trim().ToLower();
            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
            {
                // Save data to settings
                Settings.Instance.SetValue(Settings.UserName, login);
                Settings.Instance.SetValue(Settings.UserId, userId);
                Settings.Instance.SetValue(Settings.AccessToken, accessToken);
                Settings.Instance.SetValue(Settings.AccessTokenExpires, DateTime.UtcNow.AddSeconds(expiresIn));
                Settings.Instance.SetValue(Settings.RefreshToken, refreshToken);

                // TODO: Remove after debugging
                Debug.WriteLine("User ID       = " + userId);
                Debug.WriteLine("Access Token  = " + accessToken);
                Debug.WriteLine("Refresh Token = " + refreshToken);
            }
            else
            {
                // TODO: Localize
                App.DisplayAlert("Error", "El servidor no ha regresado los datos de acceso al sistema", "OK");
                return;
            }

            // Return to login view
            await this.Navigation.PopModalAsync();

            // Signal the task completion
            this.completionSource.SetResult(true);
        }

        /// <summary>
        /// Cancels the register task.
        /// </summary>
        private async void Cancel()
        {
            // Return to login view
            await this.Navigation.PopModalAsync();

            // Signal task cancellation
            this.completionSource.SetCanceled();
        }
    }
}
