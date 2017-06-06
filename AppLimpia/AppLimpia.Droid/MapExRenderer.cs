using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Views;
using Android.Widget;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;
using Xamarin.Forms.Platform.Android;

using View = Xamarin.Forms.View;

[assembly: ExportRenderer(typeof(AppLimpia.MapEx), typeof(AppLimpia.Droid.MapExRenderer))]

namespace AppLimpia.Droid
{
    /// <summary>
    /// A renderer for <see cref="MapEx"/> control.
    /// </summary>
    public class MapExRenderer : MapRenderer, GoogleMap.IInfoWindowAdapter, IOnMapReadyCallback
    {
        /// <summary>
        /// The instance of the current native map.
        /// </summary>
        private GoogleMap map;

        /// <summary>
        /// Markers present on the current map.
        /// </summary>
        private List<Marker> markers;

        /// <summary>
        /// The currently selected marker.
        /// </summary>
        private Marker selectedMarker;

        /// <summary>
        /// The currently selected markers annotation.
        /// </summary>
        private Android.Views.View selectedAnnotation;

        /// <summary>
        /// This method is called when the Google map is ready to be displayed.
        /// </summary>
        /// <param name="googleMap">The instance of the <see cref="GoogleMap"/>.</param>
        public void OnMapReady(GoogleMap googleMap)
        {
            // Setup the Google map instance
            this.map = googleMap;
            this.map.MyLocationChange += this.OnLocationChange;
            this.map.SetInfoWindowAdapter(this);

            // Load pins already resent in collection
            var pins = ((MapEx)this.Element).Pins;
            foreach (var pin in pins)
            {
                this.AddPin(pin);
            }
        }

        /// <summary>
        /// Provides a custom info window for a marker.
        /// </summary>
        /// <param name="marker">The marker for which an info window is being populated.</param>
        /// <returns>A custom info window for marker.</returns>
        public Android.Views.View GetInfoWindow(Marker marker)
        {
            return null;
        }

        /// <summary>
        /// Provides custom contents for the default info window frame of a marker.
        /// </summary>
        /// <param name="marker">The marker for which an info window is being populated.</param>
        /// <returns>A custom view to display as contents in the info window for marker.</returns>
        public Android.Views.View GetInfoContents(Marker marker)
        {
            // Get the layout inflater
            var inflater =
                Android.App.Application.Context.GetSystemService(Context.LayoutInflaterService) as
                Android.Views.LayoutInflater;
            if (inflater != null)
            {
                // Get the custom pin
                var customPin =
                    (this.Element as MapEx)?.Pins?.FirstOrDefault(p => marker.Id.Equals((string)p.InternalId));
                if (customPin == null)
                {
                    throw new Exception("Custom pin not found");
                }

                // Inflate the view for the pin
                if (customPin.Type != MapPinType.Vehicle)
                {
                    var useTouch = Build.VERSION.SdkInt >= BuildVersionCodes.N;
                    var view = inflater.Inflate(Resource.Layout.drop_point_annotation, null);

                    // Set annotation title
                    var textViewTitle = view?.FindViewById<TextView>(Resource.Id.title);
                    if (textViewTitle != null)
                    {
                        textViewTitle.Text = customPin.Label;
                    }

                    // Set annotation snippet
                    var textViewSnippet = view?.FindViewById<TextView>(Resource.Id.snippet);
                    if (textViewSnippet != null)
                    {
                        textViewSnippet.Text = customPin.Address;
                    }

                    // Set toggle favorite button event
                    var toggleFavorite = view?.FindViewById<Android.Widget.ImageButton>(Resource.Id.buttonFavoriteToggle);
                    if (toggleFavorite != null)
                    {
                        if (useTouch)
                        {
                            toggleFavorite.Touch += (s, e) =>
                                {
                                    if (e.Event.Action == MotionEventActions.Up)
                                    {
                                        marker.HideInfoWindow();
                                        customPin.ToggleFavoriteCommand.Execute(null);
                                    }
                                };
                        }
                        else
                        {
                            toggleFavorite.Click += (s, e) =>
                                {
                                    marker.HideInfoWindow();
                                    customPin.ToggleFavoriteCommand.Execute(null);
                                };
                        }

                        // Change the image if required
                        if (customPin.IsFavorite)
                        {
                            toggleFavorite.SetImageResource(Resource.Drawable.remove_from_favorites);
                        }
                    }

                    // Set locate vehicle button event
                    var locateVehicle = view?.FindViewById<Android.Widget.Button>(Resource.Id.buttonLocateVehicle);
                    if (locateVehicle != null)
                    {
                        if (useTouch)
                        {
                            locateVehicle.Touch += (s, e) =>
                                {
                                    if (e.Event.Action == MotionEventActions.Up)
                                    {
                                        marker.HideInfoWindow();
                                        customPin.LocateVehicleCommand.Execute(null);
                                    }
                                };
                        }
                        else
                        {
                            locateVehicle.Click += (s, e) =>
                                {
                                    marker.HideInfoWindow();
                                    customPin.LocateVehicleCommand.Execute(null);
                                };
                        }
                    }

                    // Set report button event
                    var report = view?.FindViewById<Android.Widget.Button>(Resource.Id.buttonReport);
                    if (report != null)
                    {
                        if (useTouch)
                        {
                            report.Touch += (s, e) =>
                                {
                                    if (e.Event.Action == MotionEventActions.Up)
                                    {
                                        marker.HideInfoWindow();
                                        customPin.ReportIncidentCommand.Execute(null);
                                    }
                                };
                        }
                        else
                        {
                            report.Click += (s, e) =>
                                {
                                    marker.HideInfoWindow();
                                    customPin.ReportIncidentCommand.Execute(null);
                                };
                        }
                    }

                    // Return the generated view
                    return this.SetView(marker, view);
                }
            }

            // Show the default annotation
            return null;
        }

        /// <summary>
        /// Handles the ElementChanged event.
        /// </summary>
        /// <param name="e">A <see cref="ElementChangedEventArgs{TElement}"/> with arguments of the event.</param>
        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            // Call the base member
            base.OnElementChanged(e);

            // If old element is present
            if (e.OldElement != null)
            {
                // Remove event handler
                Xamarin.Forms.MessagingCenter.Unsubscribe<MapEx>(this, "CenterMap");
                Xamarin.Forms.MessagingCenter.Unsubscribe<MapEx>(this, "CheckLocationService");

                var pins = (ObservableCollection<MapExPin>)((MapEx)e.OldElement).Pins;
                pins.CollectionChanged -= this.OnCollectionChanged;
                this.map.MyLocationChange -= this.OnLocationChange;
                ((MapViewEx)this.Control).TouchEvent -= this.OnTouch;

                // Remove the reference to the current map
                this.map = null;
            }

            // If new element is present
            if (e.NewElement != null)
            {
                // Setup event handlers
                var element = (MapEx)e.NewElement;
                Xamarin.Forms.MessagingCenter.Subscribe(this, "CenterMap", new Action<MapEx, Position>(this.CenterMap));
                Xamarin.Forms.MessagingCenter.Subscribe(
                    this,
                    "CheckLocationService",
                    new Action<MapEx, TaskCompletionSource<bool>>(MapExRenderer.CheckLocationService));
                var pins = (ObservableCollection<MapExPin>)element.Pins;
                pins.CollectionChanged += this.OnCollectionChanged;

                // Show user position if required
                element.IsShowingUser = element.ShowUserPosition;

                // Add event handler
                var control = (MapViewEx)this.Control;
                control.GetMapAsync(this);
                control.TouchEvent += this.OnTouch;
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
                var element = (MapEx)this.Element;
                element.IsShowingUser = element.ShowUserPosition;
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
            var manager =
                Android.App.Application.Context.GetSystemService(Context.LocationService) as
                    Android.Locations.LocationManager;
            if (manager == null)
            {
                completionSource.TrySetCanceled();
                return;
            }

            completionSource.TrySetResult(manager.IsProviderEnabled(Android.Locations.LocationManager.GpsProvider));
        }

        /// <summary>
        /// Gets the resource ID for the provided pin type.
        /// </summary>
        /// <param name="pinType">Pin type to get image resource.</param>
        /// <returns>Image resource identifier.</returns>
        private static int GetResourceId(MapPinType pinType)
        {
            switch (pinType)
            {
                case MapPinType.DropPoint:
                    return Resource.Drawable.pin_drop_point;
                case MapPinType.Favorite:
                    return Resource.Drawable.pin_favorite;
                case MapPinType.PrimaryFavorite:
                    return Resource.Drawable.pin_primary_favorite;
                default:
                    return Resource.Drawable.pin_vehicle;
            }
        }

        /// <summary>
        /// Centers the map on the required point.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="center">The point to center map on.</param>
        private void CenterMap(MapEx sender, Position center)
        {
            // Move the map center to the required position
            if ((this.Control != null) && (this.map != null))
            {
                var cameraPosition = new CameraPosition.Builder()
                    .Target(new LatLng(center.Latitude, center.Longitude))
                    .Zoom(this.map.CameraPosition.Zoom)
                    .Tilt(this.map.CameraPosition.Tilt)
                    .Bearing(this.map.CameraPosition.Bearing)
                    .Build();
                this.map.AnimateCamera(CameraUpdateFactory.NewCameraPosition(cameraPosition));
            }
        }

        /// <summary>
        /// Sets the currently selected marker and annotation.
        /// </summary>
        /// <param name="marker">The currently selected marker.</param>
        /// <param name="annotation">The currently selected annotation.</param>
        /// <returns>The annotation to be returned.</returns>
        private Android.Views.View SetView(Marker marker, Android.Views.View annotation)
        {
            this.selectedMarker = marker;
            this.selectedAnnotation = annotation;
            return this.selectedAnnotation;
        }

        /// <summary>
        /// Handles the LocationChange event of sender.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="GoogleMap.MyLocationChangeEventArgs"/> with arguments of the event.</param>
        private void OnLocationChange(object sender, GoogleMap.MyLocationChangeEventArgs e)
        {
            // If accuracy is too high
            if (e.Location.HasAccuracy && (e.Location.Accuracy >= 50))
            {
                return;
            }

            // Report the new map coordinates
            ((MapEx)this.Element).UserPosition = new Position(e.Location.Latitude, e.Location.Longitude);
        }

        /// <summary>
        /// Handles the Touch event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="Android.Views.View.TouchEventArgs"/> with arguments of the event.</param>
        private void OnTouch(object sender, TouchEventArgs e)
        {
            // If marker and annotation are present
            if ((this.selectedMarker != null) && this.selectedMarker.IsInfoWindowShown && (this.map != null) &&
                (this.selectedAnnotation != null))
            {
                // Get a marker position on the screen
                var point = this.map.Projection.ToScreenLocation(this.selectedMarker.Position);

                // Adjust motion event
                // 20 - offset between the default InfoWindow bottom edge and it's content bottom edge
                var scale = this.Context.Resources.DisplayMetrics.Density;
                var bottomOffsetPixels = (20 * scale) + 0.5f;
                var eventCopy = MotionEvent.Obtain(e.Event);
                eventCopy.OffsetLocation(
                    -point.X + (this.selectedAnnotation.Width / 2),
                    -point.Y + this.selectedAnnotation.Height + bottomOffsetPixels);

                // Dispatch the event to the info window
                this.selectedAnnotation.DispatchTouchEvent(eventCopy);
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
            // If map is not present
            if (this.map == null)
            {
                return;
            }

            // Create a new marker collection
            if (this.markers == null)
            {
                this.markers = new List<Marker>();
            }

            // Setup market options
            var markerOptions = new MarkerOptions();
            markerOptions.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
            markerOptions.Anchor(0.5f, 0.5f);
            markerOptions.SetTitle(pin.Label);
            markerOptions.SetSnippet(pin.Address);
            markerOptions.InfoWindowAnchor(0.5f, 0.5f);

            // Setup marker icon
            var icon = MapExRenderer.GetResourceId(pin.Type);
            markerOptions.SetIcon(BitmapDescriptorFactory.FromResource(icon));

            // Add marker to the map
            var marker = this.map.AddMarker(markerOptions);
            this.markers.Add(marker);
            pin.InternalId = marker.Id;

            // Add event handlers
            pin.PropertyChanged += this.OnPinPropertyChanged;
        }

        /// <summary>
        /// Removes a pin from the current map.
        /// </summary>
        /// <param name="pin">A pin to remove.</param>
        private void RemovePin(MapExPin pin)
        {
            // If map is not present
            if (this.map == null)
            {
                return;
            }

            // Find the marker to remove
            var marker = this.markers?.FirstOrDefault(m => m.Id.Equals((string)pin.InternalId));
            if (marker != null)
            {
                // Remove the pin form the map
                marker.Remove();
                this.markers.Remove(marker);
            }

            // Remove event handler
            pin.PropertyChanged -= this.OnPinPropertyChanged;
        }

        /// <summary>
        /// Removes all pins from the current map.
        /// </summary>
        private void ClearPins()
        {
            // If map is not present
            if (this.map == null)
            {
                return;
            }

            // If mo markers
            if (this.markers == null)
            {
                return;
            }

            // Remove all markers
            foreach (var marker in this.markers)
            {
                marker.Remove();
            }

            // Remove all markers
            this.markers.Clear();
        }

        /// <summary>
        /// Handles the ProportyChanged event of map pin.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="EventArgs"/> with arguments of the event.</param>
        private void OnPinPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Get the pin and corresponding marker
            var pin = (MapExPin)sender;
            var marker = this.markers?.FirstOrDefault(m => m.Id.Equals((string)pin.InternalId));
            if (marker == null)
            {
                // Remove event handler
                pin.PropertyChanged -= this.OnPinPropertyChanged;
                return;
            }

            // If pin label changed
            if (e.PropertyName == MapExPin.LabelProperty.PropertyName)
            {
                marker.Title = pin.Label;
            }

            // If the address property changed
            if (e.PropertyName == MapExPin.AddressProperty.PropertyName)
            {
                marker.Snippet = pin.Address;
            }

            // If the position changed
            if (e.PropertyName == MapExPin.PositionProperty.PropertyName)
            {
                marker.Position = new LatLng(pin.Position.Latitude, pin.Position.Longitude);
            }

            // If the type changed
            if (e.PropertyName == MapExPin.TypeProperty.PropertyName)
            {
                // Change the marker icon
                var icon = MapExRenderer.GetResourceId(pin.Type);
                marker.SetIcon(BitmapDescriptorFactory.FromResource(icon));

                // If the marker info window is shown
                if (marker.IsInfoWindowShown)
                {
                    marker.ShowInfoWindow();
                }
            }
        }
    }
}
