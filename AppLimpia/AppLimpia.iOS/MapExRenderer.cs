using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

using CoreGraphics;
using CoreLocation;

using Foundation;

using MapKit;
using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(AppLimpia.MapEx), typeof(AppLimpia.iOS.MapExRenderer))]

#region Generated Code
// To suppress the StyleCop warning
namespace AppLimpia.iOS
#endregion
{
    /// <summary>
    /// A renderer for <see cref="MapEx"/> control.
    /// </summary>
    public class MapExRenderer : MapRenderer
    {
        /// <summary>
        /// Handles the ElementChanged event.
        /// </summary>
        /// <param name="e">A <see cref="ElementChangedEventArgs{TElement}"/> with arguments of the event.</param>
        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            // Call the base member
            base.OnElementChanged(e);

            // Remove event handlers
            if (e.OldElement != null)
            {
                Xamarin.Forms.MessagingCenter.Unsubscribe<MapEx>(this, "CenterMap");

                var nativeMap = (MKMapView)this.Control;
                nativeMap.GetViewForAnnotation = null;
                var pins = (ObservableCollection<MapExPin>)((MapEx)e.OldElement).Pins;
                pins.CollectionChanged -= this.OnCollectionChanged;
            }

            // Register new event handlers
            if (e.NewElement != null)
            {
                // Setup event handlers
                Xamarin.Forms.MessagingCenter.Subscribe(this, "CenterMap", new Action<MapEx, Position>(this.CenterMap));

                var nativeMap = (MKMapView)this.Control;
                nativeMap.GetViewForAnnotation = this.GetViewForAnnotation;
                nativeMap.CalloutAccessoryControlTapped += this.OnCalloutAccessoryControlTapped;
                var pins = (ObservableCollection<MapExPin>)((MapEx)e.NewElement).Pins;
                pins.CollectionChanged += this.OnCollectionChanged;

                nativeMap.DidUpdateUserLocation += this.OnUpdateUserLocation;

                // Show user position if required
                var element = (MapEx)e.NewElement;
                element.IsShowingUser = element.ShowUserPosition;
                MapExRenderer.CheckLocationService(element);
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
                MapExRenderer.CheckLocationService(element);
            }
        }

        /// <summary>
        /// Checks whether the location service is available.
        /// </summary>
        /// <param name="element">The rendered <see cref="MapEx"/> element.</param>
        private static void CheckLocationService(MapEx element)
        {
            // If location is requested
            if (element.ShowUserPosition)
            {
                // If the location service is not allowed
                var status = CLLocationManager.Status;
                if ((status == CLAuthorizationStatus.Denied) || (status == CLAuthorizationStatus.Restricted))
                {
                    element.PositionServiceAvailable = false;
                }
                else
                {
                    element.PositionServiceAvailable = true;
                }
            }
            else
            {
                element.PositionServiceAvailable = false;
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
            if (this.Control != null)
            {
                var nativeMap = (MKMapView)this.Control;
                nativeMap.SetCenterCoordinate(new CLLocationCoordinate2D(center.Latitude, center.Longitude), true);
            }
        }

        /// <summary>
        /// Gets the view for annotation display.
        /// </summary>
        /// <param name="mapView">The map view control.</param>
        /// <param name="annotation">The annotation to display.</param>
        /// <returns>The annotation view to display.</returns>
        private MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            // If annotation is a user location do nothing
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (annotation is MKUserLocation)
            {
                return null;
            }

            // Get the custom pin for annotation
            var anno = annotation as MKPointAnnotation;
            if (anno == null)
            {
                return null;
            }

            var customPin = (this.Element as MapEx)?.Pins?.FirstOrDefault(p => p.InternalId.Equals(anno));
            if (customPin == null)
            {
                throw new Exception("Custom pin not found");
            }

            // If no annotation can be reused
            var reuseId = customPin.Type.ToString();
            MKAnnotationView annotationView = mapView.DequeueReusableAnnotation(reuseId);
            if (annotationView == null)
            {
                // Create a new annotation
                annotationView = new MKAnnotationView(annotation, reuseId);
                var icon = "Vehicle.png";
                switch (customPin.Type)
                {
                    case MapPinType.DropPoint:
                        icon = "DropPoint.png";
                        break;
                    case MapPinType.Favorite:
                        icon = "Favorite.png";
                        break;
                    case MapPinType.PrimaryFavorite:
                        icon = "PrimaryFavorite.png";
                        break;
                }
                
                annotationView.Image = UIImage.FromFile(icon);
                annotationView.CenterOffset = new CGPoint(0, 0);
            }

            // Return the annotation
            annotationView.Annotation = annotation;
            annotationView.CanShowCallout = true;

            // If the annotation is not a vehicle
            if (customPin.Type != MapPinType.Vehicle)
            {
                // Set the left call-out views
                var favorite = new UIButton(UIButtonType.Custom);
                var favoriteImage = customPin.Type == MapPinType.DropPoint
                                        ? "AddToFavorites.png"
                                        : "RemoveFromFavorites.png";
                var image = UIImage.FromFile(favoriteImage);
                favorite.Frame = new CGRect(0, 0, image.Size.Width, image.Size.Height);
                favorite.SetImage(UIImage.FromFile(favoriteImage), UIControlState.Normal);
                annotationView.LeftCalloutAccessoryView = favorite;

                // Set details view
                annotationView.DetailCalloutAccessoryView = this.DetailViewForAnnotation(customPin);
            }

            // Return the provided annotation
            return annotationView;
        }

        /// <summary>
        /// Gets the detail view for annotation display.
        /// </summary>
        /// <param name="customPin">The custom pin to create annotation for.</param>
        /// <returns>The annotation detail view to display.</returns>
        // ReSharper disable once MemberCanBeMadeStatic.Local
        private UIView DetailViewForAnnotation(MapExPin customPin)
        {
            // Create a new view for annotation details
            // ReSharper disable once UseObjectOrCollectionInitializer
            var view = new UIView();
            view.BackgroundColor = UIColor.White;
            view.TranslatesAutoresizingMaskIntoConstraints = false;

            // Create the locate vehicle button
            // ReSharper disable once UseObjectOrCollectionInitializer
            var labelSchedule = new UILabel();
            labelSchedule.Text = customPin.Address;
            labelSchedule.TranslatesAutoresizingMaskIntoConstraints = false;
            view.AddSubview(labelSchedule);

            // Create the locate vehicle button
            var buttonLocate = new UIButton(UIButtonType.Custom);
            buttonLocate.SetTitle("Ubicar camion", UIControlState.Normal);
            buttonLocate.SetTitleColor(UIColor.Blue, UIControlState.Normal);
            buttonLocate.TranslatesAutoresizingMaskIntoConstraints = false;
            buttonLocate.TouchUpInside += (s, e) => customPin.LocateVehicleCommand.Execute(null);
            view.AddSubview(buttonLocate);

            // Create the report button
            var buttonReport = new UIButton(UIButtonType.Custom);
            buttonReport.SetTitle("Reportar", UIControlState.Normal);
            buttonReport.SetTitleColor(UIColor.Blue, UIControlState.Normal);
            buttonReport.TranslatesAutoresizingMaskIntoConstraints = false;
            buttonReport.TouchUpInside += (s, e) => customPin.ReportIncidentCommand.Execute(null);
            view.AddSubview(buttonReport);

            // Create sub view dictionary
            NSDictionary views = NSDictionary.FromObjectsAndKeys(
                new NSObject[] { labelSchedule, buttonLocate, buttonReport },
                new NSObject[] { new NSString("labelSchedule"), new NSString("buttonLocate"), new NSString("buttonReport") });

            // Add view constraints
            view.AddConstraints(NSLayoutConstraint.FromVisualFormat("H:|[labelSchedule]|", 0, null, views));
            view.AddConstraints(NSLayoutConstraint.FromVisualFormat("H:|[buttonLocate]|", 0, null, views));
            view.AddConstraints(NSLayoutConstraint.FromVisualFormat("H:|[buttonReport]|", 0, null, views));
            view.AddConstraints(
                NSLayoutConstraint.FromVisualFormat("V:|[labelSchedule]-[buttonLocate]-[buttonReport]|", 0, null, views));

            // Return the created view
            return view;
        }

        /// <summary>
        /// Handles the CalloutAccessoryControlTapped event of MKMapView.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="MKMapViewAccessoryTappedEventArgs"/> with arguments of the event.</param>
        private void OnCalloutAccessoryControlTapped(object sender, MKMapViewAccessoryTappedEventArgs e)
        {
            // If annotation is a user location do nothing
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (e.View.Annotation is MKUserLocation)
            {
                return;
            }

            // Get the custom pin for annotation
            var anno = e.View.Annotation as MKPointAnnotation;
            if (anno == null)
            {
                return;
            }

            // Toggle pin favorite status
            var customPin = (this.Element as MapEx)?.Pins?.FirstOrDefault(p => p.InternalId.Equals(anno));
            customPin?.ToggleFavoriteCommand.Execute(null);
        }

        /// <summary>
        /// Handles the DidUpdateUserLocation event of MKMapView.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="MKUserLocationEventArgs"/> with arguments of the event.</param>
        private void OnUpdateUserLocation(object sender, MKUserLocationEventArgs e)
        {
            // Report the new map coordinates
            if (e.UserLocation.Location != null)
            {
                // If accuracy is too high
                if (e.UserLocation.Location.HorizontalAccuracy >= 100)
                {
                    return;
                }

                // Update the user location
                var location = e.UserLocation.Location.Coordinate;
                ((MapEx)this.Element).UserPosition = new Position(location.Latitude, location.Longitude);
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
            // Prepare a new annotation for the map
            // ReSharper disable once UseObjectOrCollectionInitializer
            var annotation = new MKPointAnnotation();
            annotation.Title = string.IsNullOrEmpty(pin.Label) ? "NO DATA" : pin.Label;
            annotation.Subtitle = string.IsNullOrEmpty(pin.Address) ? string.Empty : pin.Address;
            annotation.SetCoordinate(new CLLocationCoordinate2D(pin.Position.Latitude, pin.Position.Longitude));
            pin.InternalId = annotation;

            // Add the annotation to the map
            ((MKMapView)this.Control).AddAnnotation(annotation);

            // Add event handlers
            pin.PropertyChanged += this.OnPinPropertyChanged;
        }

        /// <summary>
        /// Removes a pin from the current map.
        /// </summary>
        /// <param name="pin">A pin to remove.</param>
        private void RemovePin(MapExPin pin)
        {
            // Find the annotation to remove
            if (pin.InternalId != null)
            {
                ((MKMapView)this.Control).RemoveAnnotation((IMKAnnotation)pin.InternalId);
            }

            // Remove event handler
            pin.PropertyChanged -= this.OnPinPropertyChanged;
        }

        /// <summary>
        /// Removes all pins from the current map.
        /// </summary>
        private void ClearPins()
        {
            // Remove all annotations
            var annotations = ((MKMapView)this.Control).Annotations.ToList();
            foreach (var annotation in annotations)
            {
                if (annotation is MKPointAnnotation)
                {
                    ((MKMapView)this.Control).RemoveAnnotation(annotation);
                }
            }
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
            var annotation = pin.InternalId as MKPointAnnotation;
            if (annotation == null)
            {
                // Remove event handler
                pin.PropertyChanged -= this.OnPinPropertyChanged;
                return;
            }

            // If pin label changed
            if (e.PropertyName == MapExPin.LabelProperty.PropertyName)
            {
                annotation.Title = pin.Label;
            }

            // If the address property changed
            if (e.PropertyName == MapExPin.AddressProperty.PropertyName)
            {
                annotation.Subtitle = pin.Address;
            }

            // If the position changed
            if (e.PropertyName == MapExPin.PositionProperty.PropertyName)
            {
                annotation.SetCoordinate(new CLLocationCoordinate2D(pin.Position.Latitude, pin.Position.Longitude));
            }
            
            // If the type changed
            if (e.PropertyName == MapExPin.TypeProperty.PropertyName)
            {
                // Reload the marker
                var nativeMap = this.Control as MKMapView;
                if (nativeMap != null)
                {
                    var isSelected = object.ReferenceEquals(nativeMap.SelectedAnnotations.FirstOrDefault(), annotation);
                    nativeMap.RemoveAnnotation(annotation);
                    nativeMap.AddAnnotation(annotation);

                    // Select the new annotation
                    if (isSelected)
                    {
                        nativeMap.SelectAnnotation(annotation, false);
                    }
                }
            }
        }
    }
}
