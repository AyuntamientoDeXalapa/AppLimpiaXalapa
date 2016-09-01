﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using AppLimpia.Json;
using AppLimpia.Login;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using System.Globalization;

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

        private List<MyReportsViewModel.IncidentReport> myReports; 

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            // Initialize variables
            this.havePosition = false;
            this.haveFavorites = false;

            // Initialize collections
            this.pinsDictionary = new Dictionary<string, MapExPin>();
            this.favoriteDropPoints = new List<MapExPin>();
            this.Pins = new ObservableCollection<MapExPin>();
            this.myReports = new List<MyReportsViewModel.IncidentReport>();
            
            // Create commands
            this.SearchDropPointsCommand = new Command(this.SearchDropPoints);
            this.LocateVehicleForPrimaryFavoriteCommand = new Command(
                () => this.OnPinVehicleLocationRequested(this.primaryFavorite, EventArgs.Empty),
                () => this.primaryFavorite != null);
            this.ShowFavoritesCommand = new Command(this.ShowFavorites);
            this.ShowNotificationsCommand = new Command(this.ShowNotifications);
            this.ShowMyReportsCommand = new Command(this.ShowMyReports, () => this.myReports.Count > 0);
            this.LogoutCommand = new Command(this.Logout);
        }

        /// <summary>
        /// The event that is raised when a ViewModel is reporting a error.
        /// </summary>
        public event EventHandler<ErrorReportEventArgs> ErrorReported;

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
        /// Gets the show my reports command.
        /// </summary>
        public ICommand ShowMyReportsCommand { get; }

        /// <summary>
        /// Gets the logout command.
        /// </summary>
        public ICommand LogoutCommand { get; }

        /// <summary>
        /// Initializes the current view model.
        /// </summary>
        public void Initialize()
        {
            // Load the user favorites
            this.GetUserFavorites();

            // TODO: Move to after login
            this.GetMyReports();
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

            // Get the favorites from the server
            var task = WebHelper.GetAsync(new Uri(Uris.GetFavorites));

            // Show the favorites on the map
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            var continuation = task.ContinueWith(this.ParseServerData, scheduler);
            continuation.ContinueWith(t => this.UpdateStatus(this.havePosition, true), scheduler);
        }

        /// <summary>
        /// Gets the nearest drop points from the server.
        /// </summary>
        private void GetNearestPoints()
        {
            // Get the drop points from the server
            System.Diagnostics.Debug.Assert(this.havePosition, "Mush have user position");
            var uri = $"{Uris.GetNearest}?lat={this.userPosition.Latitude}&lon={this.userPosition.Longitude}";
            var task = WebHelper.GetAsync(new Uri(uri));

            // Show the drop points on the map
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            task.ContinueWith(this.ParseServerData, scheduler);
        }

        /// <summary>
        /// Adds the drop point to favorites.
        /// </summary>
        /// <param name="id">The id of drop point to add.</param>
        private void AddToFavorite(string id)
        {
            // Add the drop points to favorites on the server
            var uri = $"{Uris.AddFavorites}?id={id}";
            var task = WebHelper.GetAsync(new Uri(uri));

            // Parse the server response
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            var continuation = task.ContinueWith(this.ParseServerData, scheduler);

            // Update pin status
            continuation.ContinueWith(
                _ => this.SetFavoriteStatus(id, true),
                default(CancellationToken),
                TaskContinuationOptions.OnlyOnRanToCompletion,
                scheduler);

            // Setup error handling
            continuation.ContinueWith(
                this.ParseTaskError,
                default(CancellationToken),
                TaskContinuationOptions.OnlyOnFaulted,
                scheduler);
        }

        /// <summary>
        /// Removes the drop point from favorites.
        /// </summary>
        /// <param name="id">The id of drop point to remove.</param>
        private void RemoveFromFavorite(string id)
        {
            // Remove the drop points from favorites on the server
            var uri = $"{Uris.RemoveFavorites}?id={id}";
            var task = WebHelper.GetAsync(new Uri(uri));

            // Parse the server response
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            var continuation = task.ContinueWith(this.ParseServerData, scheduler);

            // Update pin status
            continuation.ContinueWith(
                _ => this.SetFavoriteStatus(id, false),
                default(CancellationToken),
                TaskContinuationOptions.OnlyOnRanToCompletion,
                scheduler);

            // Setup error handling
            continuation.ContinueWith(
                this.ParseTaskError,
                default(CancellationToken),
                TaskContinuationOptions.OnlyOnFaulted,
                scheduler);
        }

        /// <summary>
        /// Locates the vehicle for specified drop point.
        /// </summary>
        /// <param name="id">The id of drop point to locate vehicle.</param>
        private void LocateVehicleForDropPoint(string id)
        {
            // Remove the drop points from favorites on the server
            var uri = $"{Uris.LocateVehicle}?id={id}";
            var task = WebHelper.GetAsync(new Uri(uri));

            // Parse the server response
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            var continuation = task.ContinueWith(this.ParseServerData, scheduler);

            // Show vehicle pin on the map
            continuation.ContinueWith(
                _ => this.ShowVehiclePosition(),
                default(CancellationToken),
                TaskContinuationOptions.OnlyOnRanToCompletion,
                scheduler);

            // Setup error handling
            continuation.ContinueWith(
                this.ParseTaskError,
                default(CancellationToken),
                TaskContinuationOptions.OnlyOnFaulted,
                scheduler);
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
            // Add the drop points to favorites on the server
            var task = WebHelper.GetAsync(new Uri(Uris.GetReports));

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
        /// Parses the server data response.
        /// </summary>
        /// <param name="task">A task that represents the asynchronous server operation.</param>
        private void ParseServerData(Task<JsonValue> task)
        {
            // Parse the server response
            // NOTE: If the server data was not retrieved the exception will be propagated
            this.ParseJson(task.Result);
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
                    pin.Type = MapPinType.Favorite;
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
                pin.Label = "Montonera " + properties.GetItemOrDefault("name").GetStringValueOrDefault(string.Empty);
                pin.Address = string.Empty; // pin.Type == MapPinType.DropPoint ? "Montonera" : "Favorita";
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
                MyReportsViewModel.IncidentReportStatus status;
                result &= Enum.TryParse(statusString, out status);

                // Add the report returned from the server
                if (!string.IsNullOrEmpty(id) && result && !string.IsNullOrEmpty(dropPoint) &&
                    !string.IsNullOrEmpty(incident))
                {
                    var reportObject = new MyReportsViewModel.IncidentReport
                                           {
                                               Id = id,
                                               Date = date,
                                               DropPoint = dropPoint,
                                               Type = incident,
                                               Status = status
                                           };
                    this.myReports.Add(reportObject);
                }
            }

            // Update command status
            ((Command)this.ShowMyReportsCommand).ChangeCanExecute();
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
                // If current vehicle is loaded
                if (this.currentVehicle != null)
                {
                    // Update vehicle position
                    this.GetVehiclePosition();
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
            this.havePosition = newHavePosition;
            this.haveFavorites = newHaveFavorites;

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
                    this.GetNearestPoints();
                }
            }

            // If the user moved from last update position
            if (this.havePosition && this.haveFavorites)
            {
                if (MapEx.GetDistance(this.userPosition, this.lastUpdatePosition) >= 50)
                {
                    this.GetNearestPoints();
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
                }
                else
                {
                    // Remove pin from favorites
                    pin.Type = MapPinType.DropPoint;
                    this.favoriteDropPoints.Remove(pin);
                }
            }
        }

        /// <summary>
        /// Sets the favorite status of map pin.
        /// </summary>
        /// <param name="favorite">The favorite drop point to set as primary.</param>
        private void SetPrimaryFavorite(MapExPin favorite)
        {
            // Set the drop point as primary favorite
            this.primaryFavorite = favorite;

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
                // TODO: Improve
                viewModel.PropertyChanged += (s, ea) =>
                    {
                        // Create a new incident report
                        if (ea.PropertyName == nameof(IncidentReportViewModel.ReportId))
                        {
                            var report = new MyReportsViewModel.IncidentReport
                                             {
                                                 Id = viewModel.ReportId,
                                                 Date = DateTime.Now,
                                                 DropPoint = viewModel.Pin.Label,
                                                 Type = viewModel.IncidentTypes[viewModel.IncidentTypeIndex],
                                                 Status = MyReportsViewModel.IncidentReportStatus.Received
                                             };
                            this.myReports.Add(report);
                            ((Command)this.ShowMyReportsCommand).ChangeCanExecute();
                        }
                    };

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
                this.GetNearestPoints();
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
                                this.pinsDictionary.Remove(item.Pin.Id);
                                this.favoriteDropPoints.Remove(item.Pin);
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
        /// Shows the my reports view.
        /// </summary>
        private void ShowMyReports()
        {
            // Show notifications view
            var viewModel = new MyReportsViewModel(this.myReports);
            var view = new MyReportsView { BindingContext = viewModel };
            this.Navigation.PushModalAsync(view);
        }

        /// <summary>
        /// Logs out the current user.
        /// </summary>
        private void Logout()
        {
            // Deactivate current view model
            this.IsActive = false;

            // Return to login view
            var viewModel = new LoginViewModel();
            var view = new LoginView { BindingContext = viewModel };
            App.ReplaceMainView(view);
        }
    }
}
