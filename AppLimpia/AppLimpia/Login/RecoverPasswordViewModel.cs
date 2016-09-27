using System;
using System.Windows.Input;

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
        /// Performs the user registration.
        /// </summary>
        private async void RecoverPassword()
        {
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
