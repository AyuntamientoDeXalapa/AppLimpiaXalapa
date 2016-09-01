using System;
using System.Threading.Tasks;
using System.Windows.Input;

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
        /// Initializes a new instance of the <see cref="RegisterViewModel"/> class.
        /// </summary>
        /// <param name="completionSource">The register task completion source.</param>
        public RegisterViewModel(TaskCompletionSource<bool> completionSource)
        {
            // Store the completion source
            this.completionSource = completionSource;

            // Setup default values
            this.UserEmail = string.Empty;
            this.Password = string.Empty;
            this.PasswordConfirm = string.Empty;

            // Setup commands
            this.RegisterCommand = new Command(this.Register);
            this.CancelCommand = new Command(this.Cancel);
        }

        /// <summary>
        /// Gets or sets the user email for registration.
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>
        /// Gets or sets the user password.
        /// </summary>
        public string Password { private get; set; }

        /// <summary>
        /// Gets or sets the user password confirmation.
        /// </summary>
        public string PasswordConfirm { private get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the user phone number.
        /// </summary>
        public string PhoneNumber { get; set; }

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
        private async void Register()
        {
            // Return to login view
            //await this.Navigation.PopModalAsync();

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
