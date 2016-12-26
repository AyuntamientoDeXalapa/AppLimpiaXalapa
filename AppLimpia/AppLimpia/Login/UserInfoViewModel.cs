using System;
using System.Windows.Input;

using AppLimpia.Json;
using AppLimpia.Properties;

using Xamarin.Forms;

namespace AppLimpia.Login
{
    /// <summary>
    /// The ViewModel for the User data view.
    /// </summary>
    public class UserInfoViewModel : ViewModelBase
    {
        /// <summary>
        /// The notification type index.
        /// </summary>
        private int notificationTypeIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInfoViewModel"/> class.
        /// </summary>
        public UserInfoViewModel()
        {
            // Setup default values
            this.NotificationTypeIndex = -1;

            // Setup commands
            this.SaveCommand = new Command(this.Save);
            this.CancelCommand = new Command(this.Cancel);

            // Gets the user data from the server
            this.GetUserInfo();
        }

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
        public ICommand SaveCommand { get; private set; }

        /// <summary>
        /// Gets the cancel command.
        /// </summary>
        public ICommand CancelCommand { get; private set; }

        /// <summary>
        /// Gets the user informarion from the server.
        /// </summary>
        private void GetUserInfo()
        {
            // If already loading user infomation
            if (this.IsBusy)
            {
                return;
            }

            // Send request to the server
            this.IsBusy = true;
            WebHelper.SendAsync(
                Uris.GetGetUserInfoUri(),
                null,
                this.ProcessGetUserDataResult,
                () => this.IsBusy = false);
        }

        /// <summary>
        /// Processes the get user informarion data result returned by the server.
        /// </summary>
        /// <param name="result">The save user data result.</param>
        private void ProcessGetUserDataResult(JsonValue result)
        {
            // End the load user data process
            this.IsBusy = false;

            // Get the current user notification types
            var notificationTypes = result.GetItemOrDefault("notification_types").GetIntValueOrDefault(-1);
            this.NotificationTypeIndex = notificationTypes;
        }

        /// <summary>
        /// Saves the user data.
        /// </summary>
        private void Save()
        {
            // If already saving information
            if (this.IsBusy)
            {
                return;
            }

            // Validate that the notification type is valid
            if (this.NotificationTypeIndex < 0)
            {
                // TODO: Localize
                App.DisplayAlert("Error", "Debe de selecionar el tipo de notificación", "OK");
                return;
            }

            // Prepare the data to be send to the server
            var request = new Json.JsonObject { { "notification_types", this.NotificationTypeIndex } };

            // Send request to the server
            this.IsBusy = true;
            WebHelper.SendAsync(
                Uris.GetUpdateUserInfoUri(),
                request.AsHttpContent(),
                this.ProcessSaveResult,
                () => this.IsBusy = false);
        }

        /// <summary>
        /// Processes the save user data result returned by the server.
        /// </summary>
        /// <param name="result">The save user data result.</param>
        private void ProcessSaveResult(JsonValue result)
        {
            // End the save user data process
            this.IsBusy = false;

            // Return to login view
            this.Navigation.PopModalAsync();
        }

        /// <summary>
        /// Cancels the user data update task.
        /// </summary>
        private void Cancel()
        {
            // Return to login view
            this.Navigation.PopModalAsync();
        }
    }
}
