using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using AppLimpia.Json;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using System.Globalization;
using System.Text;

using AppLimpia.Properties;

namespace AppLimpia
{
    /// <summary>
    /// The ViewModel for the Main view.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// The pins to be shown on the <see cref="Map"/>.
        /// </summary>
        private readonly Dictionary<string, MapExPin> pinsDictionary;

        /// <summary>
        /// The list of users favorite drop points.
        /// </summary>
        private readonly List<MapExPin> favoriteDropPoints;

        /// <summary>
        /// The primary favorite drop point.
        /// </summary>
        private MapExPin primaryFavorite;

        /// <summary>
        /// The currenly monitored drop point.
        /// </summary>
        private string currentDropPoint;

        /// <summary>
        /// The currently shown vehicle pin.
        /// </summary>
        private MapExPin currentVehicle;

        /// <summary>
        /// The current user position.
        /// </summary>
        private Position userPosition;

        /// <summary>
        /// The user position of the last update.
        /// </summary>
        private Position lastUpdatePosition;

        /// <summary>
        /// The current map center coordinates.
        /// </summary>
        private Position mapCenterCoordinates;

        /// <summary>
        /// A value indicating whether a current user is logged in.
        /// </summary>
        private bool userLoggedIn;

        /// <summary>
        /// A value indicating whether a current user can change password.
        /// </summary>
        private bool canChangePassword;

        /// <summary>
        /// A value indicating whether a current user position is loaded.
        /// </summary>
        private bool havePosition;

        /// <summary>
        /// A value indicating whether the favorites are loaded.
        /// </summary>
        private bool haveFavorites;

        /// <summary>
        /// A value indicating whether the current view model is active.
        /// </summary>
        private bool isActive;

        private ObservableCollection<IncidentReport> myReports;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            // Initialize variables
            this.userLoggedIn = Settings.Instance.Contains(Settings.AccessToken);
            this.canChangePassword = Settings.Instance.Contains(Settings.UserName);
            this.havePosition = false;
            this.haveFavorites = false;

            // Initialize collections
            this.pinsDictionary = new Dictionary<string, MapExPin>();
            this.favoriteDropPoints = new List<MapExPin>();
            this.Pins = new ObservableCollection<MapExPin>();
            this.myReports = new ObservableCollection<IncidentReport>();

            // Create commands
            this.SearchDropPointsCommand = new Command(this.SearchDropPoints);
            this.LocateVehicleForPrimaryFavoriteCommand =
                new Command(
                    () => this.OnPinVehicleLocationRequested(this.primaryFavorite, EventArgs.Empty),
                    () => this.primaryFavorite != null);
            this.ShowFavoritesCommand = new Command(this.ShowFavorites);
            this.ShowNotificationsCommand = new Command(this.ShowNotifications);
            this.ShowReportsCommand = new Command(this.ShowReports);
            this.ChangeUserInfoCommand = new Command(this.ChangeUserInfo);
            this.ChangePasswordCommand = new Command(this.ChangePassword);
            this.LoginCommand = new Command(this.Login);
            this.LogoutCommand = new Command(this.Logout);
        }

        /// <summary>
        /// The event that is raised when a ViewModel is reporting a error.
        /// </summary>
        public event EventHandler<ErrorReportEventArgs> ErrorReported;

        /// <summary>
        /// Gets or sets a value indicating whether a current user is logged in.
        /// </summary>
        public bool UserLoggedIn
        {
            get
            {
                return this.userLoggedIn;
            }

            set
            {
                this.SetProperty(ref this.userLoggedIn, value, nameof(this.UserLoggedIn));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a current user can change password.
        /// </summary>
        public bool CanChangePassword
        {
            get
            {
                return this.canChangePassword;
            }

            set
            {
                this.SetProperty(ref this.canChangePassword, value, nameof(this.CanChangePassword));
            }
        }

        /// <summary>
        /// Gets or sets the current user position.
        /// </summary>
        public Position UserPosition
        {
            get
            {
                return this.userPosition;
            }

            set
            {
                // If the user position changed
                if (this.SetProperty(ref this.userPosition, value, nameof(this.UserPosition)))
                {
                    Debug.WriteLine("New UserPosition: lat={0}; lon={1}", value.Latitude, value.Longitude);
                    this.UpdateStatus(true, this.haveFavorites);
                }
            }
        }

        /// <summary>
        /// Gets or sets the current map center coordinates.
        /// </summary>
        public Position MapCenterCoordinates
        {
            get
            {
                return this.mapCenterCoordinates;
            }

            set
            {
                // Update the map center coordinates
                this.mapCenterCoordinates = value;
                this.OnPropertyChanged(nameof(this.MapCenterCoordinates));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the current view model is active.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return this.isActive;
            }

            set
            {
                // If the view model is active
                if (this.SetProperty(ref this.isActive, value, nameof(this.IsActive)))
                {
                    if (this.isActive)
                    {
                        // Start the update timer
                        Device.StartTimer(TimeSpan.FromMinutes(1), this.TimerUpdate);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the pins to be shown on <see cref="Map"/>.
        /// </summary>
        public ObservableCollection<MapExPin> Pins { get; }

        /// <summary>
        /// Gets the search drop points command.
        /// </summary>
        public ICommand SearchDropPointsCommand { get; }

        /// <summary>
        /// Gets the locate vehicle for primary favorite command.
        /// </summary>
        public ICommand LocateVehicleForPrimaryFavoriteCommand { get; }

        /// <summary>
        /// Gets the show notifications command.
        /// </summary>
        public ICommand ShowFavoritesCommand { get; }

        /// <summary>
        /// Gets the show notifications command.
        /// </summary>
        public ICommand ShowNotificationsCommand { get; }

        /// <summary>
        /// Gets the show reports command.
        /// </summary>
        public ICommand ShowReportsCommand { get; }

        /// <summary>
        /// Gets the change user info command.
        /// </summary>
        public ICommand ChangeUserInfoCommand { get; }

        /// <summary>
        /// Gets the change password command.
        /// </summary>
        public ICommand ChangePasswordCommand { get; }

        /// <summary>
        /// Gets the login command.
        /// </summary>
        public ICommand LoginCommand { get; }

        /// <summary>
        /// Gets the logout command.
        /// </summary>
        public ICommand LogoutCommand { get; }

        /// <summary>
        /// Initializes the current view model.
        /// </summary>
        public void Initialize()
        {
            // If user is logged in
            if (Settings.Instance.Contains(Settings.AccessToken))
            {
                // Load the user favorites
                this.GetUserFavorites();

                // TODO: Move to after login
                ////this.GetMyReports();
            }
            else
            {
                // Favorites will be loaded when user logs in
                this.UpdateStatus(this.havePosition, true);
            }
        }

        /// <summary>
        /// Reports an error.
        /// </summary>
        /// <param name="args">A <see cref="ErrorReportEventArgs"/> with arguments of the event.</param>
        public void ReportError(ErrorReportEventArgs args)
        {
            this.ErrorReported?.Invoke(this, args);
        }

        /// <summary>
        /// Gets the user favorites from the server.
        /// </summary>
        private void GetUserFavorites()
        {
            // Clear the favorites
            this.haveFavorites = false;

            // Send request to the server
            WebHelper.SendAsync(Uris.GetGetFavoritesUri(), null, this.ProcessGetUserFavoritesResult);
        }

        /// <summary>
        /// Processes the search for the nearest points result returned by the server.
        /// </summary>
        /// <param name="result">The search result.</param>
        private void ProcessGetUserFavoritesResult(JsonValue result)
        {
            // Get the embedded data
            var collection = result.GetItemOrDefault("_embedded").GetItemOrDefault("favoritos") as JsonArray;
            if (collection == null)
            {
                return;
            }

            // Process each drop point in the collection
            this.ProcessDropPointData(collection);

            // Update status
            this.UpdateStatus(this.havePosition, true);
        }

        /// <summary>
        /// Seaches the nearest drop points near the user position.
        /// </summary>
        private void FindNearestDropPoints()
        {
            // Send request to the server
            System.Diagnostics.Debug.Assert(this.havePosition, "Mush have user position");
            WebHelper.SendAsync(
                Uris.GetFindNearestDropPointsUri(this.userPosition.Longitude, this.userPosition.Latitude, 0.2),
                null,
                this.ProcessFindNearestDropPointsResult);
        }

        /// <summary>
        /// Processes the search for the nearest points result returned by the server.
        /// </summary>
        /// <param name="result">The search result.</param>
        private void ProcessFindNearestDropPointsResult(JsonValue result)
        {
            // Get the embedded data
            var collection = result.GetItemOrDefault("_embedded").GetItemOrDefault("montoneras") as JsonArray;
            if (collection == null)
            {
                return;
            }

            // Process each drop point in the collection
            this.ProcessDropPointData(collection);
        }

        /// <summary>
        /// Processes the drop points data received from the server.
        /// </summary>
        /// <param name="collection">The drop points data received from the server.</param>
        private void ProcessDropPointData(JsonArray collection)
        {
            // Process each drop point in the collection
            foreach (var item in collection)
            {
                // Get the id and subtype properties
                var id = item.GetItemOrDefault("id").GetStringValueOrDefault(null);
                var subtype = item.GetItemOrDefault("subtype").GetStringValueOrDefault(string.Empty);
                if (id == null)
                {
                    continue;
                }

                // Get the coordinates
                var coordinates = item.GetItemOrDefault("coordenadas", null) as JsonArray;
                var lon = coordinates.GetItemOrDefault(0).GetDoubleValueOrDefault(double.NaN);
                var lat = coordinates.GetItemOrDefault(1).GetDoubleValueOrDefault(double.NaN);

                // If coordinates are not present
                if (double.IsNaN(lat) || double.IsNaN(lon))
                {
                    return;
                }

                // If the pin already exists
                MapExPin pin;
                if (this.pinsDictionary.TryGetValue(id, out pin))
                {
                    // Update the pin
                    Debug.WriteLine("Replace: " + id);
                    pin.Position = new Position(lat, lon);
                }
                else
                {
                    // Create a new pin
                    Debug.WriteLine("New: " + id);
                    pin = new MapExPin
                              {
                                  Id = id,
                                  Position = new Position(lat, lon),
                                  Type = MapPinType.DropPoint,
                                  Label = string.Empty,
                                  Address = string.Empty
                              };

                    // Setup event handlers
                    pin.FavoriteToggled += this.OnPinFavoriteToggled;
                    pin.VehicleLocationRequested += this.OnPinVehicleLocationRequested;
                    pin.IncidentReported += this.OnPinIncidentReported;

                    // Add the pin to the map
                    this.pinsDictionary.Add(id, pin);
                    this.Pins.Add(pin);
                }

                // If the pin is a favorite
                if (string.Compare(subtype, "favorito", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    // Add the pin to the favorites
                    pin.Type = MapPinType.Favorite;
                    this.favoriteDropPoints.Add(pin);
                }
                else if (string.Compare(subtype, "favorito principal", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    // Add the pin to the favorites
                    pin.Type = MapPinType.PrimaryFavorite;
                    this.favoriteDropPoints.Add(pin);
                    this.SetPrimaryFavorite(pin);
                }

                // Update the pin parameters
                var name = item.GetItemOrDefault("name").GetStringValueOrDefault(string.Empty);
                pin.Label = $"{Localization.DropPointTitle} {name}";
                pin.Address = item.GetItemOrDefault("turno").GetStringValueOrDefault(string.Empty);
            }
        }

        /// <summary>
        /// Adds the drop point to favorites.
        /// </summary>
        /// <param name="id">The id of drop point to add.</param>
        private void AddToFavorite(string id)
        {
            // If the current view model is busy do nothing
            if (this.IsBusy)
            {
                return;
            }

            // Prepare the data to be send to the server
            var request = new Json.JsonObject { { "montonera", id } };

            // Send request to the server
            this.IsBusy = true;
            WebHelper.SendAsync(
                Uris.GetAddToFavoritesUri(),
                request.AsHttpContent(),
                _ =>
                    {
                        this.SetFavoriteStatus(id, true);
                        this.IsBusy = false;
                    },
                () => this.IsBusy = false);
        }

        /// <summary>
        /// Removes the drop point from favorites.
        /// </summary>
        /// <param name="id">The id of drop point to remove.</param>
        private void RemoveFromFavorite(string id)
        {
            // If the current view model is busy do nothing
            if (this.IsBusy)
            {
                return;
            }

            // Prepare the data to be send to the server
            var request = new Json.JsonObject { { "montonera", id } };

            // Send request to the server
            this.IsBusy = true;
            WebHelper.SendAsync(
                Uris.GetRemoveFromFavoritesUri(),
                request.AsHttpContent(),
                _ =>
                    {
                        this.SetFavoriteStatus(id, false);
                        this.IsBusy = false;
                    },
                () => this.IsBusy = false);
        }

        /// <summary>
        /// Locates the vehicle for specified drop point.
        /// </summary>
        /// <param name="dropPoint">The id of drop point to locate vehicle.</param>
        private void LocateVehicleForDropPoint(string dropPoint)
        {
            // If the current view model is busy do nothing
            if (this.IsBusy)
            {
                return;
            }

            // Send request to the server
            WebHelper.SendAsync(
                Uris.GetLocateVehicleUri(dropPoint),
                null,
                v =>
                    {
                        this.ProcessLocateVehicleResult(v, true);
                        this.IsBusy = false;
                    },
                () => this.IsBusy = false);
        }

        /// <summary>
        /// Updates the vehicle position for specified drop point.
        /// </summary>
        /// <param name="dropPoint">The id of drop point to update vehicle position.</param>
        private void UpdateVehiclePosition(string dropPoint)
        {
            // Send request to the server
            WebHelper.SendAsync(
                Uris.GetLocateVehicleUri(dropPoint),
                null,
                v => this.ProcessLocateVehicleResult(v, false));
        }

        /// <summary>
        /// Processes the locate vehicle result returned by the server.
        /// </summary>
        /// <param name="result">The locate vehicle result.</param>
        /// <param name="center"><c>true</c> to center map on the vehicle; <c>false</c> to leave map uncahnged.</param>
        private void ProcessLocateVehicleResult(JsonValue result, bool center)
        {
            // Get the id of the drop point
            var dropPoint = result.GetItemOrDefault("id").GetStringValueOrDefault(string.Empty);

            // Get the vehicle id
            var vehicle = result.GetItemOrDefault("vehiculo");
            var vehicleId = vehicle.GetItemOrDefault("id").GetStringValueOrDefault(string.Empty);

            // Get the vehicle coordinates
            var coordinates = vehicle.GetItemOrDefault("coordinates", null) as JsonArray;
            var lon = coordinates.GetItemOrDefault(0).GetDoubleValueOrDefault(double.NaN);
            var lat = coordinates.GetItemOrDefault(1).GetDoubleValueOrDefault(double.NaN);

            // If vehicle data is not present
            if (!string.IsNullOrEmpty(vehicleId) && !double.IsNaN(lat) && !double.IsNaN(lon))
            {
                // If the pin already exists
                MapExPin pin;
                if (this.pinsDictionary.TryGetValue(vehicleId, out pin))
                {
                    // Update the pin
                    Debug.WriteLine("Replace: " + vehicleId);
                    pin.Position = new Position(lat, lon);
                }
                else
                {
                    // Create a new pin
                    Debug.WriteLine("New: " + vehicleId);
                    pin = new MapExPin
                              {
                                  Id = vehicleId,
                                  Position = new Position(lat, lon),
                                  Type = MapPinType.Vehicle,
                                  Label = string.Empty,
                                  Address = string.Empty
                              };

                    // Replace vehicle if any
                    if (this.currentVehicle != null)
                    {
                        this.pinsDictionary.Remove(this.currentVehicle.Id);
                        this.Pins.Remove(this.currentVehicle);
                    }

                    // Save the current vehicle
                    this.currentVehicle = pin;

                    // Add the pin to the map
                    this.pinsDictionary.Add(vehicleId, pin);
                    this.Pins.Add(pin);
                }

                // Update the pin parameters
                pin.Label = vehicle.GetItemOrDefault("number").GetStringValueOrDefault(string.Empty);
                pin.Address = vehicle.GetItemOrDefault("location").GetStringValueOrDefault(string.Empty);

                // Start vehicle position updates
                this.currentDropPoint = dropPoint;

                // Center map on vehicle
                if (center)
                {
                    this.MapCenterCoordinates = this.currentVehicle.Position;
                }
            }
            else
            {
                // TODO: Localize
                App.DisplayAlert("Error", "La unidad asignada a ruta no cuenta con servicio de localización", "OK");
            }
        }

        /// <summary>
        /// Gets the vehicle updated position from the server.
        /// </summary>
        private void GetVehiclePosition()
        {
            // Get the drop points from the server
            System.Diagnostics.Debug.Assert(this.currentVehicle != null, "Vehicle must be loaded");
            Debug.WriteLine("Updating vehicle positions");
            var uri = $"{Uris.GetVehiclePosition}?id={this.currentVehicle.Id}";
            var task = WebHelper.GetAsync(new Uri(uri));

            // Parse the server response
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            var continuation = task.ContinueWith(this.ParseServerData, scheduler);

            // Setup error handling
            continuation.ContinueWith(
                this.ParseTaskError,
                default(CancellationToken),
                TaskContinuationOptions.OnlyOnFaulted,
                scheduler);
        }

        /// <summary>
        /// Loads my reports from the server.
        /// </summary>
        private void GetMyReports()
        {
            // Get the submitted incident reports from the server
            // TODO: Move to Login event
            WebHelper.GetAsync(new Uri(Uris.GetReports), this.ParseMyReportsData);
        }

        /// <summary>
        /// Parsed the my reports data returned by the server.
        /// </summary>
        /// <param name="json">The my reports data returned by the server.</param>
        private void ParseMyReportsData(JsonValue json)
        {
            // Process the HAL
            string nextUri;
            var reports = WebHelper.ParseHalCollection(json, "reportes", out nextUri);

            var uid = string.Empty;
            if (Settings.Instance.Contains(Settings.UserId))
            {
                uid = Settings.Instance.GetValue(Settings.UserId, string.Empty);
            }

            // Parse my reports data
            foreach (var report in reports)
            {
                // Get my report field
                var id = report.GetItemOrDefault("id").GetStringValueOrDefault(null);
                var dateString =
                    report.GetItemOrDefault("fecha").GetItemOrDefault("date").GetStringValueOrDefault(string.Empty);
                var dropPoint = report.GetItemOrDefault("montonera").GetStringValueOrDefault(null);
                var incident = report.GetItemOrDefault("incidencia").GetStringValueOrDefault(null);
                var user = report.GetItemOrDefault("usuario").GetStringValueOrDefault(string.Empty);
                var status = report.GetItemOrDefault("status").GetStringValueOrDefault(string.Empty);

                // TODO: Move this check to server
                if (string.IsNullOrEmpty(uid) || (user == uid))
                {
                    // Parse notification date
                    DateTime date;
                    var result = DateTime.TryParseExact(
                        dateString,
                        "yyyy-MM-dd HH:mm:ss.ffffff",
                        null,
                        DateTimeStyles.None,
                        out date);

                    // If all fields present
                    if (!string.IsNullOrEmpty(id) && result && !string.IsNullOrEmpty(dropPoint)
                        && !string.IsNullOrEmpty(incident) && !string.IsNullOrEmpty(status))
                    {
                        // Add my report
                        var reportObject = new IncidentReport(id)
                                               {
                                                   Date = date.ToLocalTime(),
                                                   DropPoint = dropPoint,
                                                   Type = incident,
                                                   Status = status
                                               };
                        this.myReports.Add(reportObject);
                    }
                }
            }

            // If new page is present
            if (nextUri != null)
            {
                // Get the favorites from the server
                WebHelper.GetAsync(new Uri(nextUri), this.ParseMyReportsData);
            }
        }

        /// <summary>
        /// Parses the server data response.
        /// </summary>
        /// <param name="task">A task that represents the asynchronous server operation.</param>
        private JsonValue ParseServerData(Task<JsonValue> task)
        {
            // Parse the server response
            // NOTE: If the server data was not retrieved the exception will be propagated
            this.ParseJson(task.Result);
            return task.Result;
        }

        /// <summary>
        /// Parses the task that have failed to execute.
        /// </summary>
        /// <param name="task">A task that represents the failed asynchronous operation.</param>
        private void ParseTaskError(Task task)
        {
            // The task should be faulted
            System.Diagnostics.Debug.Assert(task.Status == TaskStatus.Faulted, "Asynchronous task must be faulted.");

            // Report the error to the user
            foreach (var ex in task.Exception.Flatten().InnerExceptions)
            {
                this.ReportError(new ErrorReportEventArgs(ex));
            }
        }

        /// <summary>
        /// Parsed the data returned by the server.
        /// </summary>
        /// <param name="json">The data in JSON format.</param>
        private void ParseJson(JsonValue json)
        {
            // If the data is a GeoJson format
            var type = json.GetItemOrDefault("type").GetStringValueOrDefault(string.Empty);
            if ((type == "Feature") || (type == "FeatureCollection"))
            {
                // Parse the GeoJson
                this.ParseGeoJson(json);
            }
            else if (type == "ReportCollection")
            {
                // Parse reports data
                this.ParseReportCollection(json);
            }
            else
            {
                // If the result is an error
                var error = json.GetItemOrDefault("error").GetStringValueOrDefault(string.Empty);
                if (!string.IsNullOrEmpty(error))
                {
                    // Report error to application
                    throw new Exception(error);
                }
            }
        }

        /// <summary>
        /// Parsed the GeoJson data returned by the server.
        /// </summary>
        /// <param name="geoJson">The data in GeoJson format.</param>
        private void ParseGeoJson(JsonValue geoJson)
        {
            // If the root is a feature
            var type = geoJson.GetItemOrDefault("type").GetStringValueOrDefault(string.Empty);
            if (type == "Feature")
            {
                this.ParseGeoJsonFeature(geoJson);
            }

            // Parse the features collection
            var features = geoJson.GetItemOrDefault("features", null) as JsonArray;
            if (features == null)
            {
                return;
            }

            // Parse each feature
            Debug.WriteLine("Features: {0}", features.Count);
            foreach (var feature in features)
            {
                // Validate the type
                type = feature.GetItemOrDefault("type", null).GetStringValueOrDefault(string.Empty);
                if (type == "Feature")
                {
                    this.ParseGeoJsonFeature(feature);
                }
            }
        }

        /// <summary>
        /// Pareses the GeoJson feature.
        /// </summary>
        /// <param name="feature">The GeoJson feature.</param>
        private void ParseGeoJsonFeature(JsonValue feature)
        {
            // Get the id and subtype properties
            var id = feature.GetItemOrDefault("id").GetStringValueOrDefault(null);
            var subtype = feature.GetItemOrDefault("subtype").GetStringValueOrDefault("DropPoint");
            if (id == null)
            {
                return;
            }

            // Get the geometry property
            var geometry = feature.GetItemOrDefault("geometry", null);
            if (geometry == null)
            {
                Debug.WriteLine("No geometry for '{0}'", id);
                return;
            }

            // Parse the geometry
            var geometryType = geometry.GetItemOrDefault("type", null).GetStringValueOrDefault(string.Empty);
            if (geometryType != "Point")
            {
                Debug.WriteLine("Geometry of '{0}' is not a point", id);
                return;
            }

            // Get the coordinates
            var coordinates = geometry.GetItemOrDefault("coordinates", null) as JsonArray;
            var lon = coordinates.GetItemOrDefault(0).GetDoubleValueOrDefault(double.NaN);
            var lat = coordinates.GetItemOrDefault(1).GetDoubleValueOrDefault(double.NaN);

            // If coordinates are not present
            if (double.IsNaN(lat) || double.IsNaN(lon))
            {
                return;
            }

            // If the pin already exists
            MapExPin pin;
            if (this.pinsDictionary.TryGetValue(id, out pin))
            {
                // Update the pin
                Debug.WriteLine("Replace: " + id);
                pin.Position = new Position(lat, lon);
            }
            else
            {
                // Create a new pin
                Debug.WriteLine("New: " + id);
                pin = new MapExPin
                          {
                              Id = id,
                              Position = new Position(lat, lon),
                              Type = MapPinType.DropPoint,
                              Label = string.Empty,
                              Address = string.Empty
                          };

                // If the pin is a favorite
                if (string.Compare(subtype, "Favorite", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    // Add the pin to the favorites
                    pin.Type = MapPinType.Favorite;
                    this.favoriteDropPoints.Add(pin);
                }
                else if (string.Compare(subtype, "Primary Favorite", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    // Add the pin to the favorites
                    pin.Type = MapPinType.PrimaryFavorite;
                    this.favoriteDropPoints.Add(pin);
                    this.SetPrimaryFavorite(pin);
                }
                else if (string.Compare(subtype, "Vehicle", StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    pin.Type = MapPinType.Vehicle;

                    // Replace vehicle if any
                    if (this.currentVehicle != null)
                    {
                        this.pinsDictionary.Remove(this.currentVehicle.Id);
                        this.Pins.Remove(this.currentVehicle);
                    }

                    // Save the current vehicle
                    this.currentVehicle = pin;
                }

                // If pin is not a vehicle pin
                if (pin.Type != MapPinType.Vehicle)
                {
                    // Add the clicked event handler
                    // TODO: Remove
                    pin.Tapped += (s, e) => App.DisplayAlert("Information", "pin.Tapped", "OK");
                }

                // Setup event handlers
                pin.FavoriteToggled += this.OnPinFavoriteToggled;
                pin.VehicleLocationRequested += this.OnPinVehicleLocationRequested;
                pin.IncidentReported += this.OnPinIncidentReported;

                // Add the pin to the map
                this.pinsDictionary.Add(id, pin);
                this.Pins.Add(pin);
            }

            // Update the pin parameters
            var properties = feature.GetItemOrDefault("properties");
            if (pin.Type == MapPinType.Vehicle)
            {
                pin.Label = properties.GetItemOrDefault("description").GetStringValueOrDefault(string.Empty);
                pin.Address = properties.GetItemOrDefault("location").GetStringValueOrDefault(string.Empty);
            }
            else
            {
                // TODO: Localize
                pin.Label = "Punto de recolección "
                            + properties.GetItemOrDefault("name").GetStringValueOrDefault(string.Empty);
                pin.Address = "Lun, Mie, Vie (Turno: " + properties.GetItemOrDefault("turno").GetStringValue() + ")";
            }
        }

        /// <summary>
        /// Parsed the JSON reports collection data returned by the server.
        /// </summary>
        /// <param name="json">The data in JSON format.</param>
        private void ParseReportCollection(JsonValue json)
        {
            // Parse the report collection
            var reports = json.GetItemOrDefault("reports", null) as JsonArray;
            if (reports == null)
            {
                return;
            }

            // Parse each report
            Debug.WriteLine("Reports: {0}", reports.Count);
            foreach (var report in reports)
            {
                // Get the report field
                var id = report.GetItemOrDefault("id").GetStringValueOrDefault(null);
                var dateString = report.GetItemOrDefault("date").GetStringValueOrDefault(string.Empty);
                var dropPoint = report.GetItemOrDefault("droppoint").GetStringValueOrDefault(null);
                var incident = report.GetItemOrDefault("incident").GetStringValueOrDefault(null);
                var statusString = report.GetItemOrDefault("status").GetStringValueOrDefault(null);

                // Parse date and status

                // Parse report status
                DateTime date;
                var result = DateTime.TryParseExact(
                    dateString,
                    "dd/MM/yyyy HH:mm:ss",
                    null,
                    DateTimeStyles.None,
                    out date);

                // Add the report returned from the server
                if (!string.IsNullOrEmpty(id) && result && !string.IsNullOrEmpty(dropPoint)
                    && !string.IsNullOrEmpty(incident))
                {
                    var reportObject = new IncidentReport(id)
                                           {
                                               Date = date,
                                               DropPoint = dropPoint,
                                               Type = incident,
                                               Status = string.Empty
                                           };
                    this.myReports.Add(reportObject);
                }
            }
        }

        /// <summary>
        /// Updates the current and vehicle positions.
        /// </summary>
        /// <returns><c>true</c> to restart timer; <c>false</c> to cancel timer.</returns>
        private bool TimerUpdate()
        {
            // If the application is active
            Debug.WriteLine("TimerUpdate");
            if (this.isActive)
            {
                // If drop point is monitored
                if (!string.IsNullOrEmpty(this.currentDropPoint))
                {
                    this.UpdateVehiclePosition(this.currentDropPoint);
                }
            }

            // Return the active status of the current view model
            return this.isActive;
        }

        /// <summary>
        /// Updates the status of the current view model.
        /// </summary>
        /// <param name="newHavePosition">A value indicating whether a current user position is loaded.</param>
        /// <param name="newHaveFavorites">A value indicating whether the favorites are loaded.</param>
        private void UpdateStatus(bool newHavePosition, bool newHaveFavorites)
        {
            // Update the status
            var change = (this.havePosition != newHavePosition) || (this.haveFavorites != newHaveFavorites);
            this.havePosition |= newHavePosition;
            this.haveFavorites |= newHaveFavorites;

            ////// If favorites are loaded
            ////if (this.haveFavorites)
            ////{
            ////    // Show favorites on the map
            ////    foreach (var favorite in this.favoriteDropPoints)
            ////    {
            ////        this.Pins.Add(favorite);
            ////    }
            ////}

            // If the position is detected and favorites are loaded
            if (this.havePosition && this.haveFavorites && change)
            {
                // Calculate the minimum distance between the favorite drop point and the current user position
                var min = double.PositiveInfinity;
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var point in this.favoriteDropPoints)
                {
                    // If the distance is shorter than the minimum
                    var distance = MapEx.GetDistance(this.UserPosition, point.Position);
                    if (distance < min)
                    {
                        min = distance;
                    }
                }

                // If minimum distance is greater than the threshold
                this.lastUpdatePosition = this.userPosition;
                if (min > 100)
                {
                    this.FindNearestDropPoints(); //.GetNearestPoints();
                }
            }

            // If the user moved from last update position
            if (this.havePosition && this.haveFavorites)
            {
                if (MapEx.GetDistance(this.userPosition, this.lastUpdatePosition) >= 50)
                {
                    this.FindNearestDropPoints(); //.GetNearestPoints();
                    this.lastUpdatePosition = this.userPosition;
                }
            }
        }

        /// <summary>
        /// Sets the favorite status of map pin.
        /// </summary>
        /// <param name="id">The identifier of a map pin.</param>
        /// <param name="isFavorite"><c>true</c> to set as favorite; <c>false</c> to remove from favorite.</param>
        private void SetFavoriteStatus(string id, bool isFavorite)
        {
            // Get the map pin by id
            MapExPin pin;
            if (this.pinsDictionary.TryGetValue(id, out pin))
            {
                if (isFavorite)
                {
                    // Add pin to favorites
                    pin.Type = MapPinType.Favorite;
                    this.favoriteDropPoints.Add(pin);

                    // If the favorite is the first one
                    if (this.favoriteDropPoints.Count == 1)
                    {
                        // Set as primary favorite
                        this.SetPrimaryFavorite(pin);
                    }
                }
                else
                {
                    // Remove pin from favorites
                    pin.Type = MapPinType.DropPoint;
                    this.favoriteDropPoints.Remove(pin);

                    // Remove favorite from primary favorites
                    if (this.primaryFavorite == pin)
                    {
                        this.primaryFavorite = null;

                        // Set the new primary id
                        var newPrimary = this.favoriteDropPoints.FirstOrDefault();
                        if (newPrimary != null)
                        {
                            this.SetPrimaryFavorite(newPrimary);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the favorite status of map pin.
        /// </summary>
        /// <param name="favorite">The favorite drop point to set as primary.</param>
        private void SetPrimaryFavorite(MapExPin favorite)
        {
            // Unset the current primary favorite
            if (this.primaryFavorite != null)
            {
                this.primaryFavorite.Type = MapPinType.Favorite;
            }

            // Set the drop point as primary favorite
            this.primaryFavorite = favorite;
            if (this.primaryFavorite != null)
            {
                this.primaryFavorite.Type = MapPinType.PrimaryFavorite;
            }

            // Update commands
            ((Command)this.LocateVehicleForPrimaryFavoriteCommand).ChangeCanExecute();
        }

        /// <summary>
        /// Shown the vehicle position on the map.
        /// </summary>
        private void ShowVehiclePosition()
        {
            // If vehicle pin is present
            Debug.WriteLine("{0} - {1}", this.currentVehicle, this.currentVehicle?.Position);
            if (this.currentVehicle != null)
            {
                // Center map on vehicle
                this.MapCenterCoordinates = this.currentVehicle.Position;
            }
        }

        /// <summary>
        /// Handles the FavoriteToggled event of map pin.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="EventArgs"/> with arguments of the event.</param>
        private void OnPinFavoriteToggled(object sender, EventArgs e)
        {
            // If pin is specified
            var pin = sender as MapExPin;
            if (pin != null)
            {
                // If pin should be added to favorites
                if (!pin.IsFavorite)
                {
                    // Add pin to favorites
                    this.AddToFavorite(pin.Id);
                }
                else
                {
                    // Remove pin from favorites
                    this.RemoveFromFavorite(pin.Id);
                }
            }
        }

        /// <summary>
        /// Handles the VehicleLocationRequested event of map pin.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="EventArgs"/> with arguments of the event.</param>
        private void OnPinVehicleLocationRequested(object sender, EventArgs e)
        {
            // If pin is specified
            var pin = sender as MapExPin;
            if (pin != null)
            {
                // Locate vehicle for drop point
                this.LocateVehicleForDropPoint(pin.Id);
            }
        }

        /// <summary>
        /// Handles the IncidentReported event of map pin.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="EventArgs"/> with arguments of the event.</param>
        private void OnPinIncidentReported(object sender, EventArgs e)
        {
            // If pin is specified
            var pin = sender as MapExPin;
            if (pin != null)
            {
                // Setup view model
                var viewModel = new IncidentReportViewModel { Pin = pin };

                // Subscribe to incident report created event
                viewModel.OnIncidentReportCreated += (s, report) => this.myReports.Add(report);

                // Show incident report view
                var view = new IncidentReportView { BindingContext = viewModel };
                this.Navigation.PushModalAsync(view);
            }
        }

        /// <summary>
        /// Search the nearest drop points.
        /// </summary>
        private void SearchDropPoints()
        {
            // If the current location is detected
            if (this.havePosition)
            {
                // Center the map on current position
                this.MapCenterCoordinates = this.userPosition;

                // Search nearest drop points
                this.FindNearestDropPoints(); //.GetNearestPoints();
            }
        }

        /// <summary>
        /// Shows the favorites view.
        /// </summary>
        private void ShowFavorites()
        {
            try
            {
                // Setup view model
                var viewModel = new FavoritesViewModel(this.favoriteDropPoints, this.primaryFavorite);
                // TODO: Improve
                viewModel.PropertyChanged += (s, e) =>
                    {
                        // Change the primary favorite
                        if (e.PropertyName == nameof(FavoritesViewModel.PrimaryFavorite))
                        {
                            this.SetPrimaryFavorite(viewModel.PrimaryFavorite?.Pin);
                        }
                    };
                viewModel.Favorites.CollectionChanged += (s, e) =>
                    {
                        // Remove the favorite
                        if (e.Action == NotifyCollectionChangedAction.Remove)
                        {
                            foreach (var item in e.OldItems.Cast<FavoritesViewModel.FavoriteWrapper>())
                            {
                                this.SetFavoriteStatus(item.Pin.Id, false);
                            }
                        }
                    };

                // Show favorites view
                var view = new FavoritesView { BindingContext = viewModel };
                this.Navigation.PushModalAsync(view);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Shows the notifications view.
        /// </summary>
        private void ShowNotifications()
        {
            // Show notifications view
            var viewModel = new NotificationsViewModel();
            var view = new NotificationsView { BindingContext = viewModel };
            this.Navigation.PushModalAsync(view);
        }

        /// <summary>
        /// Shows the reports view.
        /// </summary>
        private void ShowReports()
        {
            // Show submitted reports view
            var viewModel = new SubmittedReportsViewModel(this.myReports);
            var view = new SubmittedReportsView { BindingContext = viewModel };
            this.Navigation.PushModalAsync(view);
        }

        /// <summary>
        /// Changes the user information of the currently logged user.
        /// </summary>
        private void ChangeUserInfo()
        {
            // Show change user info view
            var viewModel = new Login.UserInfoViewModel();
            var view = new Login.UserInfoView { BindingContext = viewModel };
            this.Navigation.PushModalAsync(view);
        }

        /// <summary>
        /// Changes the password of the currently logged user.
        /// </summary>
        private void ChangePassword()
        {
            // If logged in with email and password
            if (Settings.Instance.Contains(Settings.UserName))
            {
                // Show change password view
                var viewModel = new Login.ChangePasswordViewModel();
                var view = new Login.ChangePasswordView { BindingContext = viewModel };
                this.Navigation.PushModalAsync(view);
            }
            else
            {
                App.DisplayAlert(
                    Localization.ErrorDialogTitle,
                    Localization.ErrorCanNotChangePassword,
                    Localization.ErrorDialogDismiss);
            }
        }

        /// <summary>
        /// Logs in the current user.
        /// </summary>
        private void Login()
        {
            this.LoginInternal();
        }

        /// <summary>
        /// Logs in the current user.
        /// </summary>
        /// <returns>A task representing the login operation.</returns>
        private Task LoginInternal()
        {
            // If the user already logged in
            if (!Settings.Instance.Contains(Settings.AccessToken))
            {
                // Create the login completion source
                var completionSource = new TaskCompletionSource<bool>();

                // Show Login view
                var viewModel = new Login.LoginViewModel(completionSource);
                var view = new Login.LoginView { BindingContext = viewModel };
                this.Navigation.PushModalAsync(view);

                // Set the continuation options
                var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
                return completionSource.Task.ContinueWith(this.ProcessLoginResult, scheduler);
            }

            // Report error
            App.DisplayAlert(
                Localization.ErrorDialogTitle,
                Localization.ErrorAlreadyLoggedIn,
                Localization.ErrorDialogDismiss);
            return null;
        }

        /// <summary>
        /// Processes the login result.
        /// </summary>
        /// <param name="loginTask">The login task.</param>
        private void ProcessLoginResult(Task<bool> loginTask)
        {
            // If user is logged in
            Debug.WriteLine("Login status = {0}", loginTask.Status);
            if (loginTask.Status == TaskStatus.RanToCompletion)
            {
                // Set the user as logged in
                this.UserLoggedIn = true;
                this.CanChangePassword = Settings.Instance.Contains(Settings.UserName);
                this.Initialize();
            }
        }

        /// <summary>
        /// Logs out the current user.
        /// </summary>
        private void Logout()
        {
            // If the user is not logged in
            if (Settings.Instance.Contains(Settings.AccessToken))
            {
                // Deactivate current view model
                this.IsActive = false;

                // Prepare the data to be send to the server
                var deviceId = ((App)Application.Current).DeviceId;
                var request = new Json.JsonObject { { "device", deviceId } };

                // Setup error handlers
                // - If session is already closed by the server, close the sesion on the client
                var handlers = new Dictionary<System.Net.HttpStatusCode, Action>
                                   {
                                           { System.Net.HttpStatusCode.Unauthorized, () => this.ProcessLogoutResult(null) }
                                   };

                // Send request to the server
                this.IsBusy = true;
                WebHelper.SendAsync(
                    Uris.GetLogoutUri(),
                    request.AsHttpContent(),
                    this.ProcessLogoutResult,
                    () => this.IsBusy = false,
                    handlers);
            }
            else
            {
                App.DisplayAlert(
                    Localization.ErrorDialogTitle,
                    Localization.ErrorNotLoggedIn,
                    Localization.ErrorDialogDismiss);
            }
        }

        /// <summary>
        /// Processes the logout result returned by the server.
        /// </summary>
        /// <param name="result">The logout result.</param>
        private void ProcessLogoutResult(JsonValue result)
        {
            // End the logout process
            this.IsBusy = false;

            // Remove all favorites
            this.SetPrimaryFavorite(null);
            foreach (var favorite in this.favoriteDropPoints)
            {
                favorite.Type = MapPinType.DropPoint;
            }

            // Reset favorites status
            this.favoriteDropPoints.Clear();

            // Logout user
            this.UserLoggedIn = false;
            this.CanChangePassword = false;
            Settings.Instance.Clear();
        }
    }
}
