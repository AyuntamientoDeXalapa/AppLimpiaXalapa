using System;
using System.Diagnostics;
using System.Net.Http;
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
            this.UserName = string.Empty;
            this.FullName = string.Empty;
            this.Password = string.Empty;
            this.PasswordConfirm = string.Empty;
            this.NotificationTypeIndex = -1;

            // Setup commands
            this.RegisterCommand = new Command(this.Register);
            this.CancelCommand = new Command(this.Cancel);
        }

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
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the user password.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        // Justification = Used by data binding
        public string Password { get; set; }

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
                App.DisplayAlert(
                    Localization.ErrorDialogTitle,
                    Localization.ErrorInvalidFullName,
                    Localization.DialogDismiss);
                return;
            }

            // Validate that the user name is a valid email
            var login = this.UserName.Trim().ToLower();
            var isEmail = Regex.IsMatch(
                login,
                @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
                RegexOptions.IgnoreCase);
            if (!isEmail)
            {
                App.DisplayAlert(
                    Localization.ErrorDialogTitle,
                    Localization.ErrorInvalidUserLogin,
                    Localization.DialogDismiss);
                return;
            }

            // Validate that the password is present
            if (this.Password.Length <= 2)
            {
                App.DisplayAlert(
                    Localization.ErrorDialogTitle,
                    Localization.ErrorInvalidPassword,
                    Localization.DialogDismiss);
                return;
            }

            // Validate that the password and confirmation coincide
            if (this.Password != this.PasswordConfirm)
            {
                App.DisplayAlert(
                    Localization.ErrorDialogTitle,
                    Localization.ErrorInvalidConfirmation,
                    Localization.DialogDismiss);
                return;
            }

            // Validate that the notification type is valid
            if (this.NotificationTypeIndex < 0)
            {
                App.DisplayAlert(
                    Localization.ErrorDialogTitle,
                    Localization.ErrorInvalidNotificationTypes,
                    Localization.DialogDismiss);
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
        private async void ProcessRegistrationResults(JsonValue result)
        {
            // Return to login view
            await this.Navigation.PopModalAsync();

            // Process with login operation
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
