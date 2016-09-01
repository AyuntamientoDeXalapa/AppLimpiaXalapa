using System;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

namespace AppLimpia
{
    /// <summary>
    /// The ViewModel for the User data view.
    /// </summary>
    public class UserDataViewModel : ViewModelBase
    {
        /// <summary>
        /// The update user data task completion source.
        /// </summary>
        private readonly TaskCompletionSource<bool> completionSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserDataViewModel"/> class.
        /// </summary>
        /// <param name="completionSource">The update user data task completion source.</param>
        public UserDataViewModel(TaskCompletionSource<bool> completionSource)
        {
            // Store the completion source
            this.completionSource = completionSource;

            // Setup default values
            this.Name = string.Empty;
            this.Phone = string.Empty;

            // Setup commands
            this.SaveCommand = new Command(this.Save);
            this.CancelCommand = new Command(this.Cancel);
        }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the user phone.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Gets the register command.
        /// </summary>
        public ICommand SaveCommand { get; private set; }

        /// <summary>
        /// Gets the cancel command.
        /// </summary>
        public ICommand CancelCommand { get; private set; }

        /// <summary>
        /// Saves the user data.
        /// </summary>
        private void Save()
        {
            // Signal the task completion
            this.completionSource.SetResult(true);

            // Return to login view
            this.Navigation.PopModalAsync();
        }

        /// <summary>
        /// Cancels the user data update task.
        /// </summary>
        private void Cancel()
        {
            // Signal task cancellation
            this.completionSource.SetCanceled();

            // Return to login view
            this.Navigation.PopModalAsync();
        }
    }
}
