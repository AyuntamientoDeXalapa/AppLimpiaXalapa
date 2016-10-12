using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;

using AppLimpia.Json;

using Xamarin.Forms;

namespace AppLimpia.Login
{
    /// <summary>
    /// The ViewModel for the Recover password view.
    /// </summary>
    public class RecoverPasswordViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecoverPasswordViewModel"/> class.
        /// </summary>
        public RecoverPasswordViewModel()
        {
            // Setup default values
            this.IsBusy = false;
            this.Login = string.Empty;

            // Setup commands
            this.RecoverPasswordCommand = new Command(this.RecoverPassword);
            this.CancelCommand = new Command(this.Cancel);
        }

        /// <summary>
        /// Gets or sets the user name (login) for password recovery.
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Gets the register command.
        /// </summary>
        public ICommand RecoverPasswordCommand { get; private set; }

        /// <summary>
        /// Gets the cancel command.
        /// </summary>
        public ICommand CancelCommand { get; private set; }

        /// <summary>
        /// Performs the user password recovery.
        /// </summary>
        private void RecoverPassword()
        {
            // If already recovering password
            if (this.IsBusy)
            {
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

            // Prepare the data to be send to the server
            var passwordRecoveryForm = new Json.JsonObject
                                       {
                                               { "username", login },
                                               { "name", "XXX" },
                                               { "password", "***" }
                                       };
            var builder = new StringBuilder();
            Json.Json.Write(passwordRecoveryForm, builder);
            Debug.WriteLine("Request: " + builder);
            var request = new StringContent(builder.ToString(), Encoding.UTF8, "application/json");

            // Send request to the server
            this.IsBusy = true;
            WebHelper.PutAsync(
                new Uri(Uris.RecoverPassword),
                request,
                this.ProcessPasswordRecoveryResults,
                () => this.IsBusy = false);
        }

        /// <summary>
        /// Processes the password recovery result returned by the server.
        /// </summary>
        /// <param name="result">The password recovery result.</param>
        private async void ProcessPasswordRecoveryResults(JsonValue result)
        {
            // Show user information
            // TODO: Localize
            await App.DisplayAlert(
                "Recuperar contraseña",
                "Si el nombre de usuario coincide con un usuario existente, Usted va a recibir una nueva contraseña por su correo",
                "OK");

            // End the recovery process
            this.IsBusy = false;

            // Return to login view
            await this.Navigation.PopModalAsync();
        }

        /// <summary>
        /// Cancels the register task.
        /// </summary>
        private async void Cancel()
        {
            // Return to login view
            await this.Navigation.PopModalAsync();
        }
    }
}
