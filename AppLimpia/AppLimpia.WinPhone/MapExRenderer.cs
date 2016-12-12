using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Windows.Devices.Geolocation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.WinRT;
using Xamarin.Forms.Platform.WinRT;

[assembly: ExportRenderer(typeof(AppLimpia.MapEx), typeof(AppLimpia.WinPhone.MapExRenderer))]

namespace AppLimpia.WinPhone
{
    // Resolve ambiguous types
    using Point = Windows.Foundation.Point;

    /// <summary>
    /// A renderer for <see cref="MapEx"/> control.
    /// </summary>
    public class MapExRenderer : MapRenderer
    {
        /// <summary>
        /// The circle for drawing user position on the map.
        /// </summary>
        private Ellipse userPositionCircle;

        /// <summary>
        /// The <see cref="Geolocator"/> service used for current position detection.
        /// </summary>
        private Geolocator geolocator;

        /// <summary>
        /// The map annotation.
        /// </summary>
        private MapExAnnotation annotation;
        
        /// <summary>
        /// Handles the ElementChanged event.
        /// </summary>
        /// <param name="e">A <see cref="ElementChangedEventArgs{Map}"/> with arguments of the event.</param>
        protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            // Call the base member
            base.OnElementChanged(e);

            // Unsubscribe from events
            if (e.OldElement != null)
            {
                Xamarin.Forms.MessagingCenter.Unsubscribe<MapEx>(this, "CenterMap");
                Xamarin.Forms.MessagingCenter.Unsubscribe<MapEx>(this, "CheckLocationService");
                if (this.Control != null)
                {
                    this.Control.MapTapped -= this.OnMapTapped;
                }

                var pins = (ObservableCollection<MapExPin>)((MapEx)e.OldElement).Pins;
                pins.CollectionChanged -= this.OnCollectionChanged;

                // Remove the location service
                if (this.geolocator != null)
                {
                    this.geolocator.PositionChanged -= this.GeolocatorOnPositionChanged;
                    this.geolocator = null;
                }
            }

            // If new element is present
            if (e.NewElement != null)
            {
                // Subscribe to events
                System.Diagnostics.Debug.Assert(this.Control != null, "Control is not created");
                Xamarin.Forms.MessagingCenter.Subscribe(this, "CenterMap", new Action<MapEx, Position>(this.CenterMap));
                Xamarin.Forms.MessagingCenter.Subscribe(
                    this,
                    "CheckLocationService",
                    new Action<MapEx, TaskCompletionSource<bool>>(MapExRenderer.CheckLocationService));
                this.Control.MapTapped += this.OnMapTapped;

                var pins = (ObservableCollection<MapExPin>)((MapEx)e.NewElement).Pins;
                pins.CollectionChanged += this.OnCollectionChanged;

                // Load pins already resent in collection
                foreach (var pin in pins)
                {
                    this.AddPin(pin);
                }

                // Update the user position
                this.UpdateUserPosition();
            }
        }

        /// <summary>
        /// Handles the PropertyChanged event of Element.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="PropertyChangedEventArgs"/> with arguments of the event.</param>
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Call the base member
            base.OnElementPropertyChanged(sender, e);

            // If ShowUserPosition property is changed
            if (e.PropertyName == MapEx.ShowUserPositionProperty.PropertyName)
            {
                this.UpdateUserPosition();
            }
        }

        /// <summary>
        /// Checks whether the location service is available.
        /// </summary>
        /// <param name="sender">The rendered <see cref="MapEx"/> element.</param>
        /// <param name="completionSource">The completion source for the asynchronous operation.</param>
        private static void CheckLocationService(MapEx sender, TaskCompletionSource<bool> completionSource)
        {
            // Get the location service
            var locationService = new Geolocator();
            completionSource.TrySetResult(
                (locationService.LocationStatus != PositionStatus.NotAvailable)
                && (locationService.LocationStatus != PositionStatus.Disabled));
        }

        /// <summary>
        /// Centers the map on the required point.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="center">The point to center map on.</param>
        private async void CenterMap(MapEx sender, Position center)
        {
            // Move the map center to the required position
            if (this.Control != null)
            {
                var geopoint =
                    new Geopoint(new BasicGeoposition { Latitude = center.Latitude, Longitude = center.Longitude });
                await this.Control.TrySetViewAsync(geopoint);
            }
        }

        /// <summary>
        /// Updates the user position.
        /// </summary>
        private void UpdateUserPosition()
        {
            // If showing the current user position
            var map = (MapEx)this.Element;
            if (map.ShowUserPosition)
            {
                // Create a new location service if none
                if (this.geolocator == null)
                {
                    this.geolocator = new Geolocator();
                }

                // If GPS service is available
                if ((this.geolocator.LocationStatus != PositionStatus.NotAvailable) &&
                    (this.geolocator.LocationStatus != PositionStatus.Disabled))
                {
                    // Subscribe to position updates
                    this.geolocator.ReportInterval = 30000; // Half minute
                    this.geolocator.DesiredAccuracy = PositionAccuracy.High;
                    this.geolocator.PositionChanged += this.GeolocatorOnPositionChanged;
                }
            }
            else
            {
                // Remove the location service
                this.geolocator.PositionChanged -= this.GeolocatorOnPositionChanged;
                this.geolocator = null;

                // Remove the user mark control
                if ((this.userPositionCircle != null) && this.Control.Children.Contains(this.userPositionCircle))
                {
                    // Remove the user position circle
                    this.Control.Children.Remove(this.userPositionCircle);
                    this.userPositionCircle = null;
                }
            }
        }

        /// <summary>
        /// Handles the PositionChanged event of <see cref="Geolocator"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="EventArgs"/> with arguments of the event.</param>
        private async void GeolocatorOnPositionChanged(Geolocator sender, PositionChangedEventArgs e)
        {
            // Run on UI thread
            var dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
            await dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    () => this.ShowUserPosition(e.Position.Coordinate, false));
        }

        /// <summary>
        /// Shows the user position on the map.
        /// </summary>
        /// <param name="userCoordinate">The current user position.</param>
        /// <param name="center"><c>true</c> to center the map on user position; <c>false</c> to leave map.</param>
        private void ShowUserPosition(Geocoordinate userCoordinate, bool center)
        {
            // If the position is not accurate
            if (userCoordinate.Accuracy > 100.0f)
            {
                return;
            }

            // Get the point for user coordinates
            var geopoint =
                new Geopoint(
                    new BasicGeoposition
                        {
                            Latitude = userCoordinate.Point.Position.Latitude,
                            Longitude = userCoordinate.Point.Position.Longitude
                        });

            // Create the graphics element to show user position
            if (this.userPositionCircle == null)
            {
                this.userPositionCircle = new Ellipse
                                              {
                                                  Stroke = new SolidColorBrush(Colors.White),
                                                  Fill = new SolidColorBrush(Colors.Blue),
                                                  StrokeThickness = 2.0,
                                                  Height = 20.0,
                                                  Width = 20.0,
                                                  Opacity = 50.0
                                              };
            }

            // Update current position of the control
            ((MapEx)this.Element).UserPosition = new Position(
                userCoordinate.Point.Position.Latitude,
                userCoordinate.Point.Position.Longitude);

            // Set the user position properties
            MapControl.SetLocation(this.userPositionCircle, geopoint);
            MapControl.SetNormalizedAnchorPoint(this.userPositionCircle, new Point(0.5, 0.5));

            // Add the user position circle if none
            if (!this.Control.Children.Contains(this.userPositionCircle))
            {
                this.Control.Children.Add(this.userPositionCircle);
            }

            // If center on user position
            if (center)
            {
                this.Control.Center = geopoint;
            }
        }

        /// <summary>
        /// Handles the CollectionChanged event of pins collection.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="EventArgs"/> with arguments of the event.</param>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (MapExPin pin in e.NewItems)
                    {
                        this.AddPin(pin);
                    }

                    break;

                case NotifyCollectionChangedAction.Move:
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (MapExPin pin in e.OldItems)
                    {
                        this.RemovePin(pin);
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (MapExPin pin in e.OldItems)
                    {
                        this.RemovePin(pin);
                    }

                    foreach (MapExPin pin in e.NewItems)
                    {
                        this.AddPin(pin);
                    }

                    break;

                case NotifyCollectionChangedAction.Reset:
                    this.ClearPins();
                    break;
            }
        }

        /// <summary>
        /// Adds a pin to the current map.
        /// </summary>
        /// <param name="pin">A pin to add.</param>
        private void AddPin(MapExPin pin)
        {
            // Insert the push pin to the map
            var pushPin = new MapExPushPin(pin);
            this.Control.Children.Add(pushPin);
            pushPin.Tapped += this.OnPushPinTapped;
        }

        /// <summary>
        /// Removes a pin from the current map.
        /// </summary>
        /// <param name="pin">A pin to remove.</param>
        private void RemovePin(MapExPin pin)
        {
            var pushPin = this.Control.Children.FirstOrDefault(
                c =>
                    {
                        var pPin = c as MapExPushPin;
                        return (pPin != null) && pPin.Pin.Equals(pin);
                    });
            if (pushPin != null)
            {
                ((MapExPushPin)pushPin).Tapped -= this.OnPushPinTapped;
                this.Control.Children.Remove(pushPin);
            }

            // If the annotation is for the removed pin
            if ((this.annotation != null) && this.annotation.Pin.Equals(pin))
            {
                this.Control.Children.Remove(this.annotation);
                this.annotation = null;
            }
        }

        /// <summary>
        /// Removes all pins from the current map.
        /// </summary>
        private void ClearPins()
        {
            // Remove all pins
            var pins = this.Control.Children.Where(c => c is MapExPushPin);
            foreach (var pin in pins)
            {
                ((MapExPushPin)pin).Tapped -= this.OnPushPinTapped;
                this.Control.Children.Remove(pin);
            }

            // Remove the map annotation
            if (this.annotation != null)
            {
                this.Control.Children.Remove(this.annotation);
            }
        }

        /// <summary>
        /// Handles the MapTapped event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="MapInputEventArgs"/> with arguments of the event.</param>
        private void OnMapTapped(MapControl sender, MapInputEventArgs e)
        {
            // If annotation exists
            if (this.annotation != null)
            {
                this.Control.Children.Remove(this.annotation);
                this.annotation = null;
            }
        }

        /// <summary>
        /// Handles the Tapped event of PushPin.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="TappedRoutedEventArgs"/> with arguments of the event.</param>
        private void OnPushPinTapped(object sender, TappedRoutedEventArgs e)
        {
            // If annotation exists
            if (this.annotation != null)
            {
                this.Control.Children.Remove(this.annotation);
                this.annotation = null;
            }

            // Add the annotation for the tapped pin
            this.annotation = new MapExAnnotation(((MapExPushPin)sender).Pin);
            this.Control.Children.Add(this.annotation);
        }
    }
}
