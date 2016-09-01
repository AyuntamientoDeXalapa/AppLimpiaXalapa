using System;
using System.ComponentModel;

using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;

namespace AppLimpia.WinPhone
{
    /// <summary>
    /// Represent the annotation on the map control.
    /// </summary>
    public class MapExAnnotation : ContentControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapExAnnotation"/> class.
        /// </summary>
        /// <param name="pin">The map pin to show annotation for.</param>
        internal MapExAnnotation(MapExPin pin)
        {
            if (pin == null)
            {
                throw new ArgumentNullException();
            }

            // Load the content template
            this.Pin = pin;
            this.ContentTemplate = this.GetDataTemplate();

            // Set the data context
            this.Content = pin;
            this.DataContext = pin;

            // Update the pin location
            this.UpdateLocation();

            // Subscribe to events
            this.Loaded += this.OnLoaded;
            this.Unloaded += this.OnUnloaded;
        }

        /// <summary>
        /// Gets the map pin to show.
        /// </summary>
        internal MapExPin Pin { get; }

        /// <summary>
        /// Gets the data template for the current pin.
        /// </summary>
        /// <returns>The data template for the current pin.</returns>
        private DataTemplate GetDataTemplate()
        {
            var key = $"{this.Pin.Type}AnnotationTemplate";
            if (!Application.Current.Resources.ContainsKey(key))
            {
                key = "AnnotationTemplate";
            }

            return Application.Current.Resources[key] as DataTemplate;
        }

        /// <summary>
        /// Handles the Loaded event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="EventArgs"/> with arguments of the event.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Subscribe to events (NOTE: For some reasons this method is called twice)
            this.Pin.PropertyChanged -= this.OnPinPropertyChanged;
            this.Pin.PropertyChanged += this.OnPinPropertyChanged;
        }

        /// <summary>
        /// Handles the Unloaded event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="EventArgs"/> with arguments of the event.</param>
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // Remove event handlers
            this.Pin.PropertyChanged -= this.OnPinPropertyChanged;
        }

        /// <summary>
        /// Handles the ProportyChanged event of map pin.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="EventArgs"/> with arguments of the event.</param>
        private void OnPinPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // If the position changed
            if (e.PropertyName == MapExPin.PositionProperty.PropertyName)
            {
                this.UpdateLocation();
            }
        }

        /// <summary>
        /// Updates the pin location on the map.
        /// </summary>
        private void UpdateLocation()
        {
            var anchor = new Windows.Foundation.Point(0.5, 1);
            var location =
                new Geopoint(
                    new BasicGeoposition
                    {
                        Latitude = this.Pin.Position.Latitude,
                        Longitude = this.Pin.Position.Longitude
                    });
            MapControl.SetLocation(this, location);
            MapControl.SetNormalizedAnchorPoint(this, anchor);
        }
    }
}
