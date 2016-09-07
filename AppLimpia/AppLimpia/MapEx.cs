using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace AppLimpia
{
    /// <summary>
    /// The extended Map control.
    /// </summary>
    public class MapEx : Map
    {
        /// <summary>
        /// The bind-able property indicating whether the current user position is shown on the map.
        /// </summary>
        public static readonly BindableProperty ShowUserPositionProperty = BindableProperty.Create(
            "ShowUserPosition",
            typeof(bool),
            typeof(MapEx),
            true);

        /// <summary>
        /// The bind-able property for the current user position.
        /// </summary>
        public static readonly BindableProperty UserPositionProperty = BindableProperty.Create(
            "UserPosition",
            typeof(Position),
            typeof(MapEx),
            default(Position));

        /// <summary>
        /// The pins on the current map.
        /// </summary>
        private ObservableCollection<MapExPin> pins; 

        /// <summary>
        /// Initializes a new instance of the <see cref="MapEx"/> class.
        /// </summary>
        public MapEx()
        {
            // Override the base value
            this.IsShowingUser = false;

            // TODO: Remove
            Device.OnPlatform(Android: () => { }, iOS: () => { });

            this.pins = new ObservableCollection<MapExPin>();

            // TODO: Remove
            Device.OnPlatform(
                WinPhone: () => { },
                Android: () => { },
                iOS: () => { },
                Default: () => this.pins.CollectionChanged += this.OnCollectionChanged);
        }

        /// <summary>
        /// Gets the collection of pins of the current map.
        /// </summary>
        public new IList<MapExPin> Pins => this.pins;

        /// <summary>
        /// Gets or sets a value indicating whether the current user position is shown on the map.
        /// </summary>
        public bool ShowUserPosition
        {
            get
            {
                return (bool)this.GetValue(MapEx.ShowUserPositionProperty);
            }

            set
            {
                this.SetValue(MapEx.ShowUserPositionProperty, value);

                // TODO: Remove
                Device.OnPlatform(Android: () => { }, iOS: () => { });
            }
        }

        /// <summary>
        /// Gets or sets the current user position.
        /// </summary>
        public Position UserPosition
        {
            get
            {
                return (Position)this.GetValue(MapEx.UserPositionProperty);
            }

            set
            {
                this.SetValue(MapEx.UserPositionProperty, value);
            }
        }

        /// <summary>
        /// Gets the distance between two positions.
        /// </summary>
        /// <param name="first">The <see cref="Position"/> to get distance from.</param>
        /// <param name="second">The <see cref="Position"/> to get distance to.</param>
        /// <returns>The distance between <paramref name="first"/> and <paramref name="second"/> in meters.</returns>
        public static double GetDistance(Position first, Position second)
        {
            // Translate coordinates to radians
            var firstLatitude = (Math.PI / 180.0) * first.Latitude;
            var firstLongitude = (Math.PI / 180.0) * first.Longitude;
            var secondLatitude = (Math.PI / 180.0) * second.Latitude;
            var secondLongitude = (Math.PI / 180.0) * second.Longitude;

            // Earth radius in meters
            const double R = 6371e3;

            // Calculate the distance between points
            var x = (firstLongitude - secondLongitude) * Math.Cos((firstLatitude + secondLatitude) / 2);
            var y = firstLatitude - secondLatitude;
            return Math.Sqrt((x * x) + (y * y)) * R;
        }

        /// <summary>
        /// Centers the map on the required point.
        /// </summary>
        /// <param name="center">The point to center map on.</param>
        public void CenterMap(Position center)
        {
            MessagingCenter.Send(this, "CenterMap", center);
        }

        /// <summary>
        /// Handles the CollectionChanged event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="NotifyCollectionChangedEventArgs"/> with arguments of the event.</param>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        // The element was added to the collection
                        var index = e.NewStartingIndex;
                        foreach (var element in e.NewItems)
                        {
                            base.Pins.Insert(index++, ((MapExPin)element).Pin);
                        }

                        break;
                    }

                case NotifyCollectionChangedAction.Remove:
                    {
                        // The element was removed from the collection
                        var index = e.OldStartingIndex;
                        // ReSharper disable once UnusedVariable
                        foreach (var element in e.OldItems)
                        {
                            base.Pins.RemoveAt(index++);
                        }

                        break;
                    }

                case NotifyCollectionChangedAction.Replace:
                    {
                        // The collection item was replaced
                        var index = e.NewStartingIndex;
                        foreach (var element in e.NewItems)
                        {
                            base.Pins[index++] = ((MapExPin)element).Pin;
                        }

                        break;
                    }

                case NotifyCollectionChangedAction.Move:
                    {
                        // The item was moved in the collection
                        var item = base.Pins[e.OldStartingIndex];
                        base.Pins.RemoveAt(e.OldStartingIndex);
                        base.Pins.Insert(e.NewStartingIndex, item);
                        break;
                    }

                case NotifyCollectionChangedAction.Reset:
                    base.Pins.Clear();
                    break;
            }
        }
    }
}
