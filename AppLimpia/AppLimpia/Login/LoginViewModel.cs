using System;
using System.Threading.Tasks;
using System.Windows.Input;

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
            this.RestorePasswordCommand = new Command(this.RestorePassword);
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
        public ICommand RestorePasswordCommand { get; private set; }

        /// <summary>
        /// Logs in the user with the provided credentials.
        /// </summary>
        private void Login()
        {
            // Login the user
            System.Diagnostics.Debug.WriteLine("{0}:{1}", this.UserName, this.Password);

            // Show the main view
            App.ShowMainView();
        }

        /// <summary>
        /// Logs in the user with the specified provider.
        /// </summary>
        /// <param name="provider">User credentials provider.</param>
        private void LoginWith(string provider)
        {
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
        private void RestorePassword()
        {
            // Create the register completion source
            System.Diagnostics.Debug.WriteLine("Restore password");

            // Show Register view
            var view = new RecoverPasswordView();
            this.Navigation.PushModalAsync(view);
        }
    }
}
