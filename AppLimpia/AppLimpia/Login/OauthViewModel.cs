using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

namespace AppLimpia.Login
{
    /// <summary>
    /// The ViewModel for the OAUTH view.
    /// </summary>
    public class OauthViewModel : ViewModelBase
    {
        /// <summary>
        /// The OAUTH task completion source.
        /// </summary>
        private readonly TaskCompletionSource<bool> completionSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="OauthViewModel"/> class for the specified user
        /// credentials provider.
        /// </summary>
        /// <param name="provider">The user credentials provider.</param>
        /// <param name="completionSource">he OAUTH task completion source.</param>
        public OauthViewModel(string provider, TaskCompletionSource<bool> completionSource)
        {
            // Store the initial URI
            this.Uri = Uris.OauthStart + "?provider=" + WebUtility.UrlEncode(provider);
            this.completionSource = completionSource;

            // Setup default values
            this.UserId = string.Empty;
            this.UserKey = string.Empty;
            this.Confirm = false;
            this.Error = string.Empty;

            // Setup commands
            this.CancelCommand = new Command(this.Cancel);
        }

        /// <summary>
        /// Gets the currently displayed URI.
        /// </summary>
        public string Uri { get; private set; }

        /// <summary>
        /// Gets the user identifier.
        /// </summary>
        public string UserId { get; private set; }

        /// <summary>
        /// Gets the user authentication key.
        /// </summary>
        public string UserKey { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the user confirmation is required to continue.
        /// </summary>
        public bool Confirm { get; private set; }

        /// <summary>
        /// Gets the error text.
        /// </summary>
        public string Error { get; private set; }

        /// <summary>
        /// Gets the cancel command.
        /// </summary>
        public ICommand CancelCommand { get; private set; }

        /// <summary>
        /// Called when navigation is started.
        /// </summary>
        /// <param name="uri">The URI of the navigation.</param>
        /// <returns><c>true</c> to continue navigation; <c>false</c> to cancel navigation.</returns>
        public bool OnNavigating(string uri)
        {
            System.Diagnostics.Debug.WriteLine("Navigating to: " + uri);
            if (uri.StartsWith(Uris.OauthDone, StringComparison.CurrentCultureIgnoreCase))
            {
                // Parse the data
                var args = uri.Substring(uri.IndexOf('?') + 1).Split('&');
                this.ParseResponse(args);

                // Return to login view
                this.Navigation.PopModalAsync();

                // Result the login process
                this.completionSource.SetResult(true);

                // Cancel the navigation
                return false;
            }

            return true;
        }

        /// <summary>
        /// Called when navigation is done.
        /// </summary>
        /// <param name="uri">The URI of the navigation.</param>
        public void OnNavigated(string uri)
        {
            // Change the current URI
            System.Diagnostics.Debug.WriteLine("Navigated to: " + uri);
            this.Uri = uri;
        }

        /// <summary>
        /// Parses the server response.
        /// </summary>
        /// <param name="args">The response arguments.</param>
        private void ParseResponse(string[] args)
        {
            // For each argument
            foreach (var arg in args)
            {
                // Get argument name and value
                var index = arg.IndexOf('=');
                var name = arg.Substring(0, index);
                var value = arg.Substring(index + 1);

                // Parse response argument
                switch (name.ToLower())
                {
                    case "uid":
                        this.UserId = value;
                        break;
                    case "key":
                        this.UserKey = value;
                        break;
                    case "confirm":
                        this.Confirm = true;
                        break;
                    case "error":
                        this.Error = value;
                        break;
                }
            }
        }

        /// <summary>
        /// Cancels the OAUTH login.
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
