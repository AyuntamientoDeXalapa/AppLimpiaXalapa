using System;
using System.ComponentModel;
using System.Diagnostics;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace AppLimpia
{
    /// <summary>
    /// Interaction logic for MainView.xaml.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainView
    {
        /// <summary>
        /// The current binding context.
        /// </summary>
        private MainViewModel currentBindingContext;

        /// <summary>
        /// The map pins synchronizer.
        /// </summary>
        private CollectionSynchronizer<MapExPin> pinsSynchronizer;

        /// <summary>
        /// A value indicating whether to update the map when the user location changes.
        /// </summary>
        private bool update;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainView"/> class.
        /// </summary>
        public MainView()
        {
            // Parse XAML content
            this.InitializeComponent();
            if (this.Resources == null)
            {
                this.Resources = new ResourceDictionary();
            }

            // Copy application resource dictionary
            foreach (var resource in Application.Current.Resources)
            {
                this.Resources.Add(resource.Key, resource.Value);
            }

            // Center the map on Xalapa
            this.update = true;
            this.MapView.MoveToRegion(
                MapSpan.FromCenterAndRadius(new Position(19.5266, -96.9238), Distance.FromMiles(0.3)));
        }

        /// <summary>
        /// Handles the BindingContextChanged event.
        /// </summary>
        protected override void OnBindingContextChanged()
        {
            // Remove the event handling from the binding context
            if (this.currentBindingContext != null)
            {
                this.pinsSynchronizer.Dispose();
                this.pinsSynchronizer = null;
                this.currentBindingContext.Navigation = null;
                
                // Remove events handler
                this.MapView.Pins.Clear();
                this.currentBindingContext.PropertyChanged -= this.OnPropertyChanged;
                this.currentBindingContext.ErrorReported -= this.OnErrorReported;
            }

            // Set up the event handling from the binding context
            this.currentBindingContext = this.BindingContext as MainViewModel;
            if (this.currentBindingContext != null)
            {
                // Set up navigation context
                this.currentBindingContext.Navigation = this.Navigation;

                // Set up event handlers
                this.pinsSynchronizer = new CollectionSynchronizer<MapExPin>(
                    this.currentBindingContext.Pins,
                    this.MapView.Pins);
                this.currentBindingContext.PropertyChanged += this.OnPropertyChanged;
                this.currentBindingContext.ErrorReported += this.OnErrorReported;

                // The position is not the default one
                if (this.currentBindingContext.UserPosition != default(Position))
                {
                    // Move to the selected position
                    this.OnPropertyChanged(
                        this.currentBindingContext,
                        new PropertyChangedEventArgs(nameof(MainViewModel.UserPosition)));
                }
            }

            // Call the base member
            base.OnBindingContextChanged();
        }

        /// <summary>
        /// Handles the PropertyChanged event of ViewModel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="PropertyChangedEventArgs"/> with arguments of the event.</param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // If the user position was changed
            if (e.PropertyName == nameof(MainViewModel.UserPosition))
            {
                if (this.update)
                {
                    // Center the map on the current user position
                    this.update = false;
                    this.MapView.CenterMap(this.currentBindingContext.UserPosition);
                }
            }
            else if (e.PropertyName == nameof(MainViewModel.MapCenterCoordinates))
            {
                // Center the map on the required position
                this.MapView.CenterMap(this.currentBindingContext.MapCenterCoordinates);
            }
        }

        /// <summary>
        /// Handles the ErrorReported event of ViewModel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="EventArgs"/> with arguments of the event.</param>
        private void OnErrorReported(object sender, ErrorReportEventArgs e)
        {
            Debug.WriteLine(e.Exception.ToString());
            this.DisplayAlert(
                "Error",
                "No se pudo conectar con el servidor. Por favor verifica que esta conectado al Internet o intenta más tarde.",
                "OK");
        }

        /// <summary>
        /// Handles the Tapped event of MoreCommand button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="EventArgs"/> with arguments of the event.</param>
        private void OnMoreCommandsTapped(object sender, EventArgs e)
        {
            this.MoreCommands.IsVisible = !this.MoreCommands.IsVisible;
            this.BoxViewMoreCommands.IsVisible = this.MoreCommands.IsVisible;
        }

        /// <summary>
        /// Handles the Tapped event of any command button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="EventArgs"/> with arguments of the event.</param>
        private void OnCommandTapped(object sender, EventArgs e)
        {
            // Hide more commands view
            this.MoreCommands.IsVisible = false;
            this.BoxViewMoreCommands.IsVisible = this.MoreCommands.IsVisible;
        }
    }
}
