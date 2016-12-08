using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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
            false);

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
        private readonly ObservableCollection<MapExPin> pins; 

        /// <summary>
        /// Initializes a new instance of the <see cref="MapEx"/> class.
        /// </summary>
        public MapEx()
        {
            // Override the base value
            this.IsShowingUser = false;
            this.pins = new ObservableCollection<MapExPin>();
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
            }
        }

        /// <summary>
        /// Gets or sets the current user position.
        /// </summary>
        public Position UserPosition
        {
            // ReSharper disable once UnusedMember.Global
            // Justification = Used by data binding
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
        /// Checks whether the location service is active.
        /// </summary>
        /// <returns>The task representing the asynchronous operation.</returns>
        public Task<bool> CheckLocationService()
        {
            var completionSource = new TaskCompletionSource<bool>();
            MessagingCenter.Send(this, "CheckLocationService", completionSource);
            return completionSource.Task;
        }
    }
}
