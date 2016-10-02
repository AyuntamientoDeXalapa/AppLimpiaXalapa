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
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the user name (login) for registration.
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Gets or sets the user password.
        /// </summary>
        public string Password { private get; set; }

        /// <summary>
        /// Gets or sets the user password confirmation.
        /// </summary>
        public string PasswordConfirm { private get; set; }

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
            var isEmail = Regex.IsMatch(
                this.Login,
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

            // Prepare the data to be send to the server
            var registrationForm = new Json.JsonObject
                                       {
                                               { "username", this.Login.ToLower() },
                                               { "name", this.FullName },
                                               { "password", this.Password }
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
        private async void ProcessRegistrationResults(JsonValue result)
        {
            // The register user ID
            // TODO: Change to OAUTH token
            this.UserId = result.GetItemOrDefault("id").GetStringValueOrDefault(string.Empty);
            Settings.Instance.SetValue(Settings.UserId, this.UserId);
            Debug.WriteLine("User ID = " + this.UserId);

            // End the registration process
            this.isRegistering = false;

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
