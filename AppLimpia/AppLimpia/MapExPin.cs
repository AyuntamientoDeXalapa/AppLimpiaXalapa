using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace AppLimpia
{
    /// <summary>
    /// The type of the map pin.
    /// </summary>
    public enum MapPinType
    {
        /// <summary>
        /// The map pin is a drop point.
        /// </summary>
        DropPoint,

        /// <summary>
        /// The map pin is a favorite drop point.
        /// </summary>
        Favorite,

        /// <summary>
        /// The map pin is a vehicle.
        /// </summary>
        Vehicle
    }

    /// <summary>
    /// The <see cref="MapEx"/> pin.
    /// </summary>
    public class MapExPin : BindableObject
    {
        /// <summary>
        /// The id property of the map pin.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public static readonly BindableProperty IdProperty = BindableProperty.Create(
            "Id",
            typeof(string),
            typeof(MapExPin));

        /// <summary>
        /// The type property of the map pin.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public static readonly BindableProperty TypeProperty = BindableProperty.Create(
            "Type",
            typeof(MapPinType),
            typeof(MapExPin),
            MapPinType.DropPoint);

        /// <summary>
        /// The position property of the map pin.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public static readonly BindableProperty PositionProperty = BindableProperty.Create(
            "Position",
            typeof(Position),
            typeof(MapExPin),
            default(Position));

        /// <summary>
        /// The label property of the map pin.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public static readonly BindableProperty LabelProperty = BindableProperty.Create(
            "Label",
            typeof(string),
            typeof(MapExPin));

        /// <summary>
        /// The address property of the map pin.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public static readonly BindableProperty AddressProperty = BindableProperty.Create(
            "Address",
            typeof(string),
            typeof(MapExPin));

        /// <summary>
        /// Initializes a new instance of the <see cref="MapExPin"/> class.
        /// </summary>
        public MapExPin()
        {
            // Setup command
            this.ToggleFavoriteCommand = new Command(() => this.FavoriteToggled?.Invoke(this, new EventArgs()));
            this.LocateVehicleCommand = new Command(() => this.VehicleLocationRequested?.Invoke(this, new EventArgs()));
            this.ReportIncidentCommand = new Command(() => this.IncidentReported?.Invoke(this, new EventArgs()));

            // TODO: Fix
            this.Pin = new Pin { Type = PinType.Generic };
            this.Pin.Clicked += (s, e) => this.RaiseTappedEvent(e);
        }

        /// <summary>
        /// The event that is raised when the current <see cref="MapExPin"/> is tapped.
        /// </summary>
        public event EventHandler Tapped;

        /// <summary>
        /// The event that is raised when the current <see cref="MapExPin"/> favorite status is toggled.
        /// </summary>
        public event EventHandler FavoriteToggled;

        /// <summary>
        /// The event that is raised when the vehicle location is requested for the current <see cref="MapExPin"/>.
        /// </summary>
        public event EventHandler VehicleLocationRequested;

        /// <summary>
        /// The event that is raised when the incident is reported for the current <see cref="MapExPin"/>.
        /// </summary>
        public event EventHandler IncidentReported;

        /// <summary>
        /// Gets the map pin.
        /// </summary>
        [Obsolete("Provided to preserve compatibility across platforms")]
        public Pin Pin { get; }

        /// <summary>
        /// Gets or sets the internal identifier of the current <see cref="MapExPin"/>.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     This field is used by the renderer and should not be modified by the application.
        ///   </para>
        /// </remarks>
        public object InternalId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the current <see cref="MapExPin"/>.
        /// </summary>
        public string Id
        {
            get
            {
                return (string)this.GetValue(MapExPin.IdProperty);
            }

            set
            {
                this.SetValue(MapExPin.IdProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the type of the current <see cref="MapExPin"/>.
        /// </summary>
        public MapPinType Type
        {
            get
            {
                return (MapPinType)this.GetValue(MapExPin.TypeProperty);
            }

            set
            {
                this.SetValue(MapExPin.TypeProperty, value);

                // ReSharper disable once ExplicitCallerInfoArgument
                this.OnPropertyChanged(nameof(this.IsFavorite));
            }
        }

        /// <summary>
        /// Gets whether the current <see cref="MapExPin"/> is a favorite.
        /// </summary>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Used in data binding")]
        public bool IsFavorite => this.Type == MapPinType.Favorite;

        /// <summary>
        /// Gets or sets the position of the current <see cref="MapExPin"/>.
        /// </summary>
        public Position Position
        {
            get
            {
                return (Position)this.GetValue(MapExPin.PositionProperty);
            }

            set
            {
                this.SetValue(MapExPin.PositionProperty, value);
                this.Pin.Position = value;
            }
        }

        /// <summary>
        /// Gets or sets the label of the current <see cref="MapExPin"/>.
        /// </summary>
        public string Label
        {
            get
            {
                return (string)this.GetValue(MapExPin.LabelProperty);
            }

            set
            {
                this.SetValue(MapExPin.LabelProperty, value);
                this.Pin.Label = value;
            }
        }

        /// <summary>
        /// Gets or sets the address of the current <see cref="MapExPin"/>.
        /// </summary>
        public string Address
        {
            get
            {
                return (string)this.GetValue(MapExPin.AddressProperty);
            }

            set
            {
                this.SetValue(MapExPin.AddressProperty, value);
                this.Pin.Address = value;
            }
        }

        /// <summary>
        /// Gets the toggle favorite command.
        /// </summary>
        public ICommand ToggleFavoriteCommand { get; }

        /// <summary>
        /// Gets the locate vehicle command.
        /// </summary>
        public ICommand LocateVehicleCommand { get; }

        /// <summary>
        /// Gets the report incident command.
        /// </summary>
        public ICommand ReportIncidentCommand { get; }

        /// <summary>
        /// Determines whether two specified <see cref="MapExPin"/> have the same value.
        /// </summary>
        /// <param name="a">The first <see cref="MapExPin"/> to compare, or null. </param>
        /// <param name="b">The second <see cref="MapExPin"/> to compare, or null. </param>
        /// <returns>
        ///   <c>true</c> if the value of <paramref name="a"/> is the same as the value of <paramref name="b"/>;
        ///   otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(MapExPin a, MapExPin b)
        {
            // If a and b are the same object
            if (object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If both objects are not null
            if (((object)a != null) && ((object)b != null))
            {
                return a.Equals(b);
            }

            // One object is null and another is not
            return false;
        }

        /// <summary>
        /// Determines whether two specified <see cref="MapExPin"/> have different values.
        /// </summary>
        /// <param name="a">The first <see cref="MapExPin"/> to compare, or null. </param>
        /// <param name="b">The second <see cref="MapExPin"/> to compare, or null. </param>
        /// <returns>
        ///  <c>true</c> if the value of <paramref name="a"/> is different from the value of <paramref name="b"/>;
        ///  otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(MapExPin a, MapExPin b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        ///   <b>true</b> if the specified object is equal to the current object;
        ///   otherwise, <b>false</b>.
        /// </returns>
        public override bool Equals(object obj)
        {
            // If parameter is null return false
            return obj != null && this.Equals(obj as MapExPin);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="MapExPin"/>.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = this.Position.GetHashCode();
                hash = (hash << 7) ^ this.Address?.GetHashCode() ?? 0;
                hash = (hash << 7) ^ this.Label?.GetHashCode() ?? 0;
                return hash;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="MapExPin"/> is equal to the current <see cref="MapExPin"/>.
        /// </summary>
        /// <param name="obj">The <see cref="MapExPin"/> to compare with the current object.</param>
        /// <returns>
        ///   <b>true</b> if the specified <see cref="MapExPin"/> is equal to the current <see cref="MapExPin"/>;
        ///   otherwise, <b>false</b>.
        /// </returns>
        // ReSharper disable once MemberCanBePrivate.Global
        public bool Equals(MapExPin obj)
        {
            // If parameter is null return false
            if ((object)obj == null)
            {
                return false;
            }

            // Compare value types
            return (this.Type == obj.Type) && (this.Position == obj.Position) && (this.Address == obj.Address) &&
                   (this.Label == obj.Label);
        }

        /// <summary>
        /// Raises the <see cref="Tapped"/> event.
        /// </summary>
        /// <param name="e">A <see cref="EventArgs"/> with arguments of the event.</param>
        public void RaiseTappedEvent(EventArgs e)
        {
            this.Tapped?.Invoke(this, e);
        }
    }
}
