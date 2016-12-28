using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

using AppLimpia.Json;
using AppLimpia.Properties;

using Xamarin.Forms;

namespace AppLimpia.Login
{
    /// <summary>
    /// The ViewModel for the Login view.
    /// </summary>
    public class LoginViewModel : ViewModelBase
    {
        /// <summary>
        /// The user name of the user to log.
        /// </summary>
        private string userName;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginViewModel"/> class.
        /// </summary>
        public LoginViewModel()
        {
            // Reset user name and password
            this.userName = string.Empty;
            this.Password = string.Empty;

            // Setup commands
            this.LoginCommand = new Command(this.Login);
            this.RegisterCommand = new Command(this.Register);
            this.LoginWithCommand = new Command(par => this.LoginWith((string)par));
            this.ResumeLoginWithCommand = new Command(par => this.ResumeLoginWith((Uri)par));
            this.RecoverPasswordCommand = new Command(this.RecoverPassword);
        }

        /// <summary>
        /// Gets or sets the user name (login) of the user to log.
        /// </summary>
        public string UserName
        {
            get
            {
                return this.userName;
            }

            // ReSharper disable once MemberCanBePrivate.Global
            // Justification = Used by data binding
            set
            {
                this.SetProperty(ref this.userName, value, nameof(this.UserName));
            }
        }

        /// <summary>
        /// Gets or sets the password for the user to log.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        // Justification = Used by data binding
        public string Password { get; set; }

        /// <summary>
        /// Gets the login command.
        /// </summary>
        public ICommand LoginCommand { get; private set; }

        /// <summary>
        /// Gets the login with command.
        /// </summary>
        public ICommand LoginWithCommand { get; private set; }

        /// <summary>
        /// Gets the resume login with command.
        /// </summary>
        public ICommand ResumeLoginWithCommand { get; private set; }

        /// <summary>
        /// Gets the register command.
        /// </summary>
        public ICommand RegisterCommand { get; private set; }

        /// <summary>
        /// Gets the restore password command.
        /// </summary>
        public ICommand RecoverPasswordCommand { get; private set; }

        /// <summary>
        /// Logs in the user with the provided credentials.
        /// </summary>
        private void Login()
        {
            // Login the user
            System.Diagnostics.Debug.WriteLine("{0}:{1}", this.UserName, this.Password);

            // If already logging in
            if (this.IsBusy)
            {
                return;
            }

            // Validate that the user name is a valid email
            var user = this.UserName.Trim().ToLower();
            var isEmail = Regex.IsMatch(
                user,
                @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
                RegexOptions.IgnoreCase);
            if (!isEmail)
            {
                App.DisplayAlert(
                    Localization.ErrorDialogTitle,
                    Localization.ErrorInvalidUserLogin,
                    Localization.ErrorDialogDismiss);
                return;
            }

            // Validate that the password is present
            if (this.Password.Length <= 2)
            {
                App.DisplayAlert(
                    Localization.ErrorDialogTitle,
                    Localization.ErrorInvalidPassword,
                    Localization.ErrorDialogDismiss);
                return;
            }

            // Prepare the data to be send to the server
            var deviceId = ((App)Application.Current).DeviceId;
            var request = new Json.JsonObject
                                {
                                        { "grant_type", "password" },
                                        { "username", user },
                                        { "password", this.Password },
                                        { "scope", "user submit-report" },
                                        { "device", deviceId }
                                };

            // If push token exists
            var pushToken = ((App)Application.Current).PushToken;
            if (!string.IsNullOrEmpty(pushToken))
            {
                request.Add("push_token", pushToken);
            }

            // Setup error handlers
            // - If session is already opened by another device, request user consent
            var handlers = new Dictionary<System.Net.HttpStatusCode, Action>
                               {
                                       { System.Net.HttpStatusCode.Conflict, () => this.RetryLogin(request) }
                               };

            // Send request to the server
            this.IsBusy = true;
            WebHelper.SendAsync(
                Uris.GetLoginUri(),
                request.AsHttpContent(),
                this.ProcessLoginResult,
                () => this.IsBusy = false,
                handlers);
        }

        /// <summary>
        /// Retries the login process after user consent to change session.
        /// </summary>
        /// <param name="request">The request to retry.</param>
        private async void RetryLogin(JsonObject request)
        {
            // Ask the user consent to override session
            var consent = await App.DisplayAlert(
                                    Localization.ConfirmationDialogTitle,
                                    Localization.ConfirmationSesionChange,
                                    Localization.ButtonConfirm,
                                    Localization.Cancel);

            // If the user consent received
            if (consent)
            {
                // Resent the request to the server
                request.Add("override", true);
                WebHelper.SendAsync(
                    Uris.GetLoginUri(),
                    request.AsHttpContent(),
                    this.ProcessLoginResult,
                    () => this.IsBusy = false);
            }
            else
            {
                // Cancel the task
                this.UserName = string.Empty;
                this.Password = string.Empty;
                this.OnPropertyChanged(nameof(this.Password));
                this.IsBusy = false;
            }
        }

        /// <summary>
        /// Processes the login result returned by the server.
        /// </summary>
        /// <param name="result">The login result.</param>
        private void ProcessLoginResult(JsonValue result)
        {
            // The register user ID
            // TODO: Remove USER ID
            var userId = result.GetItemOrDefault("user_id").GetStringValueOrDefault(string.Empty);
            var accessToken = result.GetItemOrDefault("access_token").GetStringValueOrDefault(string.Empty);
            var expiresIn = result.GetItemOrDefault("expires_in").GetIntValueOrDefault(24 * 60 * 60);
            var refreshToken = result.GetItemOrDefault("refresh_token").GetStringValueOrDefault(string.Empty);

            // End the login process
            this.IsBusy = false;

            // If all of the fields are returned
            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
            {
                // Save data to settings
                Settings.Instance.SetValue(Settings.UserId, userId);
                Settings.Instance.SetValue(Settings.AccessToken, accessToken);
                Settings.Instance.SetValue(Settings.AccessTokenExpires, DateTime.UtcNow.AddSeconds(expiresIn));
                Settings.Instance.SetValue(Settings.RefreshToken, refreshToken);

                // Save user name if any
                if (!string.IsNullOrWhiteSpace(this.userName))
                {
                    Settings.Instance.SetValue(Settings.UserName, this.userName.Trim().ToLower());
                }

                // TODO: Remove after debugging
                Debug.WriteLine("User ID       = " + userId);
                Debug.WriteLine("Access Token  = " + accessToken);
                Debug.WriteLine("Refresh Token = " + refreshToken);
            }
            else
            {
                // Report the server error to the user
                App.DisplayAlert(
                    Localization.ErrorDialogTitle,
                    Localization.ErrorNoLoginData,
                    Localization.ErrorDialogDismiss);
                return;
            }

            // Show the main view
            App.ShowMainView();
        }

        /// <summary>
        /// Logs in the user with the specified provider.
        /// </summary>
        /// <param name="provider">User credentials provider.</param>
        private void LoginWith(string provider)
        {
            // If already logging in
            if (this.IsBusy)
            {
                return;
            }

            // Remove OAUTH token
            Settings.Instance.Remove(Settings.OauthToken);

            // If launch URI delegate is provided
            if (((App)Application.Current).LaunchUriDelegate != null)
            {
                // Get the authorization URI from the server
                this.IsBusy = true;
                WebHelper.SendAsync(
                    Uris.GetAuthorizationUri(provider),
                    null,
                    this.ProcessLoginWithResult,
                    () => this.IsBusy = false);
            }
            else
            {
                // Display the not implemented error
                App.DisplayAlert(
                    Localization.ErrorDialogTitle,
                    Localization.ErrorNotImplemented,
                    Localization.ErrorDialogDismiss);
            }
        }

        /// <summary>
        /// Processes the login with result returned by the server.
        /// </summary>
        /// <param name="result">The login with result.</param>
        private void ProcessLoginWithResult(JsonValue result)
        {
            // Get the token and redirect URI
            var grantType = result.GetItemOrDefault("grant_type").GetStringValueOrDefault(string.Empty);
            var oauthToken = result.GetItemOrDefault("oauth_token").GetStringValueOrDefault(string.Empty);
            var authorizationUri = result.GetItemOrDefault("authorization_uri").GetStringValueOrDefault(string.Empty);

            // End the enter with process
            this.IsBusy = false;

            // If no redirect URI is not specified
            if (string.IsNullOrEmpty(authorizationUri))
            {
                return;
            }

            // Save OAUTH token
            var oauthState = new JsonObject { { "grant_type", grantType }, { "oauth_token", oauthToken } };
            var builder = new StringBuilder();
            Json.Json.Write(oauthState, builder);
            Settings.Instance.SetValue(Settings.OauthToken, builder.ToString());

            // Redirect to the authorization URI
            ((App)Application.Current).LaunchUriDelegate?.Invoke(new Uri(authorizationUri));
        }

        /// <summary>
        /// Resumes the log in with the specified provider.
        /// </summary>
        /// <param name="authorizationUri">The user authorization URI.</param>
        private void ResumeLoginWith(Uri authorizationUri)
        {
            // Get the saved OAUTH state
            var stateString = Settings.Instance.GetValue(Settings.OauthToken, "{}");
            var oauthState = (JsonObject)Json.Json.Read(stateString);
            Settings.Instance.Remove(Settings.OauthToken);

            // Get the query parameters from the URI
            Debug.WriteLine("ResumeLoginWith({0})", authorizationUri);
            var query = authorizationUri.Query;
            if ((query.Length > 0) && (query[0] == '?'))
            {
                query = query.Substring(1);
            }

            // Parse the keys and values
            var pairs = query.Split('&');
            var arguments = new Dictionary<string, string>();
            foreach (var pair in pairs)
            {
                var split = pair.Split('=');
                arguments.Add(
                    Uri.UnescapeDataString(split[0]),
                    split.Length > 1 ? Uri.UnescapeDataString(split[1]) : string.Empty);
            }

            // If error is present
            if (arguments.ContainsKey("error"))
            {
                // Report error to the user
                App.DisplayAlert(
                    Localization.ErrorDialogTitle,
                    Localization.ErrorCancelled,
                    Localization.ErrorDialogDismiss);
                return;
            }

            // Add the result to the saved state
            foreach (var kvp in arguments)
            {
                oauthState.Add(kvp.Key, kvp.Value);
            }

            // Add the device and push token
            var deviceId = ((App)Application.Current).DeviceId;
            var pushToken = ((App)Application.Current).PushToken;
            oauthState.Add("device", deviceId);
            if (!string.IsNullOrEmpty(pushToken))
            {
                oauthState.Add("push_token", pushToken);
            }

            // Setup error handlers
            // - If session is already opened by another device, request user consent
            var handlers = new Dictionary<System.Net.HttpStatusCode, Action>
                               {
                                       { System.Net.HttpStatusCode.Conflict, () => this.RetryLogin(oauthState) }
                               };

            // Send request to the server
            this.IsBusy = true;
            WebHelper.SendAsync(
                Uris.GetLoginUri(),
                oauthState.AsHttpContent(),
                this.ProcessLoginResult,
                () => this.IsBusy = false,
                handlers);
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        private void Register()
        {
            // Create the register completion source
            System.Diagnostics.Debug.WriteLine("Register");
            var completionSource = new TaskCompletionSource<bool>();

            // Show Register view
            var viewModel = new RegisterViewModel(completionSource);
            var view = new RegisterView { BindingContext = viewModel };
            this.Navigation.PushModalAsync(view);

            // Set the continuation options
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            completionSource.Task.ContinueWith(t => this.ProcessRegisterResult(t, viewModel), scheduler);
        }

        /// <summary>
        /// Processes the register login task result.
        /// </summary>
        /// <param name="registerTask">The register task.</param>
        /// <param name="viewModel">The register view model.</param>
        private void ProcessRegisterResult(Task<bool> registerTask, RegisterViewModel viewModel)
        {
            // If task is canceled do nothing
            if (registerTask.IsCanceled)
            {
                return;
            }

            // Login the current user
            this.UserName = viewModel.UserName;
            this.Password = viewModel.Password;
            this.Login();
        }

        /// <summary>
        /// Restores the user password.
        /// </summary>
        private void RecoverPassword()
        {
            // Show recover password view
            var viewModel = new RecoverPasswordViewModel();
            var view = new RecoverPasswordView { BindingContext = viewModel };
            this.Navigation.PushModalAsync(view);
        }
    }
}
