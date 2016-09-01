using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace AppLimpia.Login
{
    /// <summary>
    /// The ViewModel for the Start view.
    /// </summary>
    public class StartViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartViewModel"/> class.
        /// </summary>
        public StartViewModel()
        {
            this.StartCommand = new Command(this.Start);
        }

        /// <summary>
        /// Gets the start command of the current application.
        /// </summary>
        public ICommand StartCommand { get; private set; }

        /// <summary>
        /// Starts the application.
        /// </summary>
        private void Start()
        {
            // Get the login view
            var viewModel = new LoginViewModel();
            var view = new LoginView { BindingContext = viewModel };
            App.ReplaceMainView(view);
        }
    }
}
