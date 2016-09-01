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
                var nativeMap = (MKMapView)this.Control;
                nativeMap.GetViewForAnnotation = null;
                var pins = (ObservableCollection<MapExPin>)((MapEx)e.OldElement).Pins;
                pins.CollectionChanged -= this.OnCollectionChanged;
            }

            // Register new event handlers
            if (e.NewElement != null)
            {
                var nativeMap = (MKMapView)this.Control;
                nativeMap.GetViewForAnnotation = this.GetViewForAnnotation;
                var pins = (ObservableCollection<MapExPin>)((MapEx)e.NewElement).Pins;
                pins.CollectionChanged += this.OnCollectionChanged;
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

            // This method is called when a property of a map is changed
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
            MKAnnotationView annotationView = mapView.DequeueReusableAnnotation(customPin.Id);
            System.Diagnostics.Debug.WriteLine("GetViewForAnnotation({0})", annotationView);
            if (annotationView == null)
            {
                // Create a new annotation
                annotationView = new MKAnnotationView(annotation, customPin.Id);
                var icon = customPin.Type == MapPinType.Vehicle
                               ? "Vehicle.png"
                               : (customPin.Type == MapPinType.Favorite ? "Favorite.png" : "DropPoint.png");
                annotationView.Image = UIImage.FromFile(icon);
                annotationView.CenterOffset = new CGPoint(0, 0);
            }

            // Return the annotation
            annotationView.CanShowCallout = true;
            annotationView.DetailCalloutAccessoryView = this.DetailViewForAnnotation(anno);
            return annotationView;
        }

        /// <summary>
        /// Gets the detail view for annotation display.
        /// </summary>
        /// <param name="annotation">The annotation to display.</param>
        /// <returns>The annotation detail view to display.</returns>
        private UIView DetailViewForAnnotation(MKPointAnnotation annotation)
        {
            // Create a new view for annotation details
            // ReSharper disable once UseObjectOrCollectionInitializer
            var view = new UIView();
            view.BackgroundColor = UIColor.Green;
            view.TranslatesAutoresizingMaskIntoConstraints = false;

            // Create the label
            // ReSharper disable once UseObjectOrCollectionInitializer
            var label = new UILabel();
            label.Text = annotation.Title;
            label.TranslatesAutoresizingMaskIntoConstraints = false;
            label.Font = UIFont.SystemFontOfSize(20);
            view.AddSubview(label);

            // Create the button
            var button = new UIButton(UIButtonType.Custom);
            button.SetTitle("Reportar", UIControlState.Normal);
            button.TranslatesAutoresizingMaskIntoConstraints = false;
            button.TouchUpInside += (s, e) => new UIAlertView("Touch2", "TouchUpInside handled", null, "OK", null).Show();
            view.AddSubview(button);

            ////NSDictionary* views = NSDictionaryOfVariableBindings(label, button);
            NSDictionary views = NSDictionary.FromObjectsAndKeys(
                new NSObject[] { label, button },
                new NSObject[] { new NSString("label"), new NSString("button") });
            

            ////[view addConstraints:[NSLayoutConstraint constraintsWithVisualFormat:@"H:|[label]|" options:0 metrics:nil views:views]];
            var constraint1 = NSLayoutConstraint.FromVisualFormat("H:|[label]|", 0, null, views);
            view.AddConstraints(constraint1);
             
            ////[view addConstraint:[NSLayoutConstraint constraintWithItem:button attribute:NSLayoutAttributeCenterX relatedBy:NSLayoutRelationEqual toItem:view attribute:NSLayoutAttributeCenterX multiplier:1 constant:0]];
            var constraint2 = NSLayoutConstraint.Create(
                button,
                NSLayoutAttribute.CenterX,
                NSLayoutRelation.Equal,
                view,
                NSLayoutAttribute.CenterX,
                1,
                0);
            view.AddConstraint(constraint2);

            ////[view addConstraints:[NSLayoutConstraint constraintsWithVisualFormat:@"V:|[label]-[button]|" options:0 metrics:nil views:views]];
            var constraint3 = NSLayoutConstraint.FromVisualFormat("V:|[label]-[button]|", 0, null, views);
            view.AddConstraints(constraint3);

            ////var widthConstraint = NSLayoutConstraint.Create(
            ////    view,
            ////    NSLayoutAttribute.Width,
            ////    NSLayoutRelation.Equal,
            ////    null,
            ////    NSLayoutAttribute.NoAttribute,
            ////    1,
            ////    100);
            ////view.AddConstraint(widthConstraint);

            ////var heightConstraint = NSLayoutConstraint.Create(
            ////    view,
            ////    NSLayoutAttribute.Height,
            ////    NSLayoutRelation.Equal,
            ////    null,
            ////    NSLayoutAttribute.NoAttribute,
            ////    1,
            ////    100);
            ////view.AddConstraint(heightConstraint);

            // Return the created view
            return view;
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
            System.Diagnostics.Debug.WriteLine("OnPinPropertyChanged({0})", e.PropertyName);

            ////var pin = (MapExPin)sender;
            ////var marker = this.markers?.FirstOrDefault(m => m.Id.Equals((string)pin.InternalId));
            ////if (marker == null)
            ////{
            ////    // Remove event handler
            ////    pin.PropertyChanged -= this.OnPinPropertyChanged;
            ////    return;
            ////}

            ////// If pin label changed
            ////if (e.PropertyName == MapExPin.LabelProperty.PropertyName)
            ////{
            ////    marker.Title = pin.Label;
            ////}

            ////// If the address property changed
            ////if (e.PropertyName == MapExPin.AddressProperty.PropertyName)
            ////{
            ////    marker.Snippet = pin.Address;
            ////}

            ////// If the position changed
            ////if (e.PropertyName == MapExPin.PositionProperty.PropertyName)
            ////{
            ////    marker.Position = new LatLng(pin.Position.Latitude, pin.Position.Longitude);
            ////}
        }
    }
}
