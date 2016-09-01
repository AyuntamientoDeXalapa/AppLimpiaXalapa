using System;

using AppLimpia.Media;

using Foundation;
using UIKit;

#region Generated Code
// To suppress the StyleCop warning
namespace AppLimpia.iOS
#endregion
{
    /// <summary>
    /// Defines the application delegate for the current application.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Class is created by the OS")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "PartialTypeWithSinglePart", Justification = "Generated code")]
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        /// <summary>
        /// Handles the FinishedLaunching event of the application delegate.
        /// </summary>
        /// <param name="app">The current application.</param>
        /// <param name="options">The application options.</param>
        /// <returns><c>true</c> - if the application was launched; <c>false</c> otherwise.</returns>
        /// <remarks>
        ///   <para>
        ///     This method is invoked when the application has loaded and is ready to run. In this
        ///     method you should instantiate the window, load the UI into it and then make the window
        ///     visible.
        ///   </para>
        ///   <para>
        ///     You have 17 seconds to return from this method, or iOS will terminate your application.
        ///   </para>
        /// </remarks>
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            // Initialize the platform dependent components
            Xamarin.Forms.Forms.Init();
            Xamarin.FormsMaps.Init();
            MediaPicker.Instance = new Media.MediaPickerIOS();

            // Initialize the application
            this.LoadApplication(new App());
            return base.FinishedLaunching(app, options);
        }
    }
}
