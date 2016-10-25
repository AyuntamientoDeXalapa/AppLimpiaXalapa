using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Windows.Input;

using AppLimpia.Json;

using Xamarin.Forms;

namespace AppLimpia.Login
{
    /// <summary>
    /// The ViewModel for the ChangePassword view.
    /// </summary>
    public class ChangePasswordViewModel : ViewModelBase
    {
        /// <summary>
        /// Gets or sets the user name (login) for registration.
        /// </summary>
        private string login;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangePasswordViewModel"/> class.
        /// </summary>
        public ChangePasswordViewModel()
        {
            // Setup default values
            this.Login = Settings.Instance.GetValue(Settings.UserName, string.Empty);
            this.PasswordCurrent = string.Empty;
            this.PasswordNew = string.Empty;
            this.PasswordConfirm = string.Empty;

            // Setup commands
            this.ChangePasswordCommand = new Command(this.ChangePassword);
            this.CancelCommand = new Command(this.Cancel);
        }

        /// <summary>
        /// Gets or sets the user name (login) for registration.
        /// </summary>
        public string Login
        {
            get
            {
                return this.login;
            }

            // ReSharper disable once MemberCanBePrivate.Global, Justification = Used by data binding
            set
            {
                this.SetProperty(ref this.login, value, nameof(this.Login));
            }
        }

        /// <summary>
        /// Gets or sets the user current password.
        /// </summary>
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global, Justification = Used by data binding
        // ReSharper disable once MemberCanBePrivate.Global, Justification = Used by data binding
        public string PasswordCurrent { private get; set; }

        /// <summary>
        /// Gets or sets the user new password.
        /// </summary>
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global, Justification = Used by data binding
        // ReSharper disable once MemberCanBePrivate.Global, Justification = Used by data binding
        public string PasswordNew { private get; set; }

        /// <summary>
        /// Gets or sets the user password confirmation.
        /// </summary>
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global, Justification = Used by data binding
        // ReSharper disable once MemberCanBePrivate.Global, Justification = Used by data binding
        public string PasswordConfirm { private get; set; }

        /// <summary>
        /// Gets the change password command.
        /// </summary>
        public ICommand ChangePasswordCommand { get; private set; }

        /// <summary>
        /// Gets the cancel command.
        /// </summary>
        public ICommand CancelCommand { get; private set; }

        /// <summary>
        /// Performs the user password change.
        /// </summary>
        private void ChangePassword()
        {
            // If already changing password
            if (this.IsBusy)
            {
                return;
            }

            // Validate that the current password is present
            if (this.PasswordCurrent.Length <= 2)
            {
                // TODO: Localize
                App.DisplayAlert("Error", "Debe de proporcionar una contraseña actual valida", "OK");
                return;
            }

            // Validate that the new password is present
            if (this.PasswordNew.Length <= 2)
            {
                // TODO: Localize
                App.DisplayAlert("Error", "Debe de proporcionar una contraseña nueva valida", "OK");
                return;
            }

            // Validate that the password and confirmation coincide
            if (this.PasswordNew != this.PasswordConfirm)
            {
                // TODO: Localize
                App.DisplayAlert("Error", "La contraseña y su confirmacion no coinciden", "OK");
                return;
            }

            // Prepare the data to be send to the server
            var passwordChangeForm = new Json.JsonObject
                                       {
                                               { "username", this.login },
                                               { "old_password", this.PasswordCurrent },
                                               { "password", this.PasswordNew }
                                       };
            var builder = new StringBuilder();
            Json.Json.Write(passwordChangeForm, builder);
            Debug.WriteLine("Request: " + builder);
            var request = new StringContent(builder.ToString(), Encoding.UTF8, "application/json");

            // Send request to the server
            this.IsBusy = true;
            WebHelper.PatchAsync(
                new Uri(Uris.ChangePassword),
                request,
                this.ProcessChangePasswordResults,
                () => this.IsBusy = false);
        }

        /// <summary>
        /// Processes the registration result returned by the server.
        /// </summary>
        /// <param name="result">The registration result.</param>
        private async void ProcessChangePasswordResults(JsonValue result)
        {
            // Report password change to the user
            // TODO: Localize
            await App.DisplayAlert("Cambiar contraseña", "Su contraseña fue cambiada con exito", "OK");

            // End the password change process
            this.IsBusy = false;

            // Return to login view
            await this.Navigation.PopModalAsync();
        }

        /// <summary>
        /// Cancels the register task.
        /// </summary>
        private void Cancel()
        {
            // Return to login view
            this.Navigation.PopModalAsync();
        }
    }
}
