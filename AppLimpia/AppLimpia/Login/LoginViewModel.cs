using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

using AppLimpia.Json;

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
        /// A value indicating whether the login process is in progress.
        /// </summary>
        private bool isLoggingIn;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginViewModel"/> class.
        /// </summary>
        public LoginViewModel()
        {
            // Reset user name and password
            this.userName = string.Empty;
            this.Password = string.Empty;
            this.isLoggingIn = false;

            // Setup commands
            this.LoginCommand = new Command(this.Login);
            this.RegisterCommand = new Command(this.Register);
            this.LoginWithCommand = new Command(par => this.LoginWith((string)par));
            this.RecoverPasswordCommand = new Command(this.RecoverPassword);
        }

        /// <summary>
        /// Gets or sets the user name of the user to log.
        /// </summary>
        public string UserName
        {
            get
            {
                return this.userName;
            }

            set
            {
                this.SetProperty(ref this.userName, value, nameof(this.UserName));
            }
        }

        /// <summary>
        /// Gets or sets the password for the user to log.
        /// </summary>
        public string Password { private get; set; }

        /// <summary>
        /// Gets the login command.
        /// </summary>
        public ICommand LoginCommand { get; private set; }

        /// <summary>
        /// Gets the login with command.
        /// </summary>
        public ICommand LoginWithCommand { get; private set; }

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
            if (this.isLoggingIn)
            {
                return;
            }

            // Validate that the user name is a valid email
            // TODO: Change to email or phone number
            var isEmail = Regex.IsMatch(
                this.UserName,
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

            // Prepare the data to be send to the server
            var user = System.Net.WebUtility.UrlEncode(this.UserName);
            var password = System.Net.WebUtility.UrlEncode(this.Password);

            // Send request to the server
            this.isLoggingIn = true;
            WebHelper.GetAsync(
                new Uri($"{Uris.Login}?username={user.ToLower()}&password={password}"),
                this.ProcessLoginResults,
                () => this.isLoggingIn = false);
        }

        /// <summary>
        /// Processes the login result returned by the server.
        /// </summary>
        /// <param name="result">The login result.</param>
        private void ProcessLoginResults(JsonValue result)
        {
            // The register user ID
            // TODO: Change to OAUTH token
            var userId = result.GetItemOrDefault("id").GetStringValueOrDefault(string.Empty);
            Settings.Instance.SetValue(Settings.UserId, userId);
            Debug.WriteLine("User ID = " + userId);

            // End the registration process
            this.isLoggingIn = false;

            // Show the main view
            App.ShowMainView();
        }

        /// <summary>
        /// Logs in the user with the specified provider.
        /// </summary>
        /// <param name="provider">User credentials provider.</param>
        private void LoginWith(string provider)
        {
            // TODO: Localize
            App.DisplayAlert("Error", "Esta función todavia no esta disponible", "OK");
            return;

            // Create the OAUTH login completion source
            var completionSource = new TaskCompletionSource<bool>();

            // Show OAUTH view
            var viewModel = new OauthViewModel(provider, completionSource);
            var view = new OauthView { BindingContext = viewModel };
            this.Navigation.PushModalAsync(view);

            // Set the continuation options
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            completionSource.Task.ContinueWith(t => this.ProcessOauthResult(t, viewModel), scheduler);
        }

        /// <summary>
        /// Processes the OAUTH login task result.
        /// </summary>
        /// <param name="oauthTask">The OAUTH login task.</param>
        /// <param name="viewModel">The OAUTH view model.</param>
        private void ProcessOauthResult(Task<bool> oauthTask, OauthViewModel viewModel)
        {
            // If task is canceled do nothing
            if (oauthTask.IsCanceled)
            {
                System.Diagnostics.Debug.WriteLine("OAUTH Canceled");
                return;
            }

            // Process OAUTH result
            System.Diagnostics.Debug.WriteLine("OAUTH Done");
            System.Diagnostics.Debug.WriteLine("UserId = " + viewModel.UserId);
            System.Diagnostics.Debug.WriteLine("UserKey = " + viewModel.UserKey);
            System.Diagnostics.Debug.WriteLine("Confirm = " + viewModel.Confirm);
            System.Diagnostics.Debug.WriteLine("Error = " + viewModel.Error);

            // If OAUTH was successful
            if (string.IsNullOrEmpty(viewModel.Error))
            {
                // Show the main view
                App.ShowMainView();
            }
            else
            {
                // Display error
                App.DisplayAlert("Error", viewModel.Error, "OK");
            }
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
            System.Diagnostics.Debug.WriteLine("Register task done");
            if (registerTask.IsCanceled)
            {
                System.Diagnostics.Debug.WriteLine("Register canceled");
                return;
            }

            // Show the main view
            App.ShowMainView();
        }

        /// <summary>
        /// Restores the user password.
        /// </summary>
        private void RecoverPassword()
        {
            // Create the register completion source
            System.Diagnostics.Debug.WriteLine("Restore password");

            // Show Register view
            var viewModel = new RecoverPasswordViewModel();
            var view = new RecoverPasswordView { BindingContext = viewModel };
            this.Navigation.PushModalAsync(view);
        }
    }
}
