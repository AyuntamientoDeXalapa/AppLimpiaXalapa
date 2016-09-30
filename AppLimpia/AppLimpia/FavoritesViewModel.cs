using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using AppLimpia.Json;

using Xamarin.Forms;

namespace AppLimpia
{
    /// <summary>
    /// The ViewModel for the Favorites view.
    /// </summary>
    public class FavoritesViewModel : ViewModelBase
    {
        /// <summary>
        /// The primary favorite.
        /// </summary>
        private FavoriteWrapper primaryFavorite;

        /// <summary>
        /// Initializes a new instance of the <see cref="FavoritesViewModel"/> class.
        /// </summary>
        /// <param name="favorites">The user favorites.</param>
        /// <param name="primaryFavorite">The primary user favorite.</param>
        public FavoritesViewModel(IEnumerable<MapExPin> favorites, MapExPin primaryFavorite)
        {
            // Get the favorites collection
            this.Favorites = new ObservableCollection<FavoriteWrapper>();
            foreach (var favorite in favorites)
            {
                // Create the favorite wrapper for UI
                var wrapper = new FavoriteWrapper(favorite, this);
                this.Favorites.Add(wrapper);
                wrapper.PropertyChanged += this.OnFavoritePropertyChanged;

                // If the favorite is a primary favorite
                if (favorite == primaryFavorite)
                {
                    wrapper.IsPrimary = true;
                }
            }

            // Setup commands
            this.CancelCommand = new Command(this.Cancel);
        }

        /// <summary>
        /// Gets the favorite pins.
        /// </summary>
        public ObservableCollection<FavoriteWrapper> Favorites { get; }

        /// <summary>
        /// Gets or sets the primary favorite.
        /// </summary>
        public FavoriteWrapper PrimaryFavorite
        {
            get
            {
                return this.primaryFavorite;
            }

            set
            {
                // Set the current primary favorite as not favorite
                if (this.primaryFavorite != null)
                {
                    this.primaryFavorite.IsPrimary = false;
                }

                // Change the primary favorite
                this.SetProperty(ref this.primaryFavorite, value, nameof(this.PrimaryFavorite));
                Debug.WriteLine("New primary = {0}", this.primaryFavorite?.Label);
            }
        }

        /// <summary>
        /// Gets the cancel command.
        /// </summary>
        public ICommand CancelCommand { get; private set; }

        /// <summary>
        /// Handles the PropertyChanged event of favorite.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="PropertyChangedEventArgs"/> with arguments of the event.</param>
        private void OnFavoritePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var favorite = sender as FavoriteWrapper;
            if (favorite != null)
            {
                // If the favorite is a new primary
                if (e.PropertyName == FavoriteWrapper.IsPrimaryProperty.PropertyName)
                {
                    if (favorite.IsPrimary)
                    {
                        // Change the primary favorite
                        this.PrimaryFavorite = favorite;
                    }
                }
            }
        }

        /// <summary>
        /// Cancels the user data update task.
        /// </summary>
        private void Cancel()
        {
            // Return to login view
            this.Navigation.PopModalAsync();
        }

        /// <summary>
        /// The favorite pin wrapper.
        /// </summary>
        public class FavoriteWrapper : BindableObject
        {
            /// <summary>
            /// The is primary property of the map pin.
            /// </summary>
            // ReSharper disable once MemberCanBePrivate.Global
            public static readonly BindableProperty IsPrimaryProperty = BindableProperty.Create(
                "IsPrimary",
                typeof(bool),
                typeof(FavoriteWrapper),
                false);

            /// <summary>
            /// The favorite map pin.
            /// </summary>
            private readonly MapExPin favorite;

            /// <summary>
            /// The reference to the current view model.
            /// </summary>
            private readonly FavoritesViewModel viewModel;

            /// <summary>
            /// Initializes a new instance of the <see cref="FavoriteWrapper"/> class.
            /// </summary>
            /// <param name="favorite">The favorite map pin.</param>
            /// <param name="viewModel">The reference to the current view model.</param>
            public FavoriteWrapper(MapExPin favorite, FavoritesViewModel viewModel)
            {
                // Store the current favorite
                this.favorite = favorite;
                this.viewModel = viewModel;

                // Setup commands
                this.SetAsPrimaryCommand = new Command(this.SetAsPrimary);
                this.RemoveFavoriteCommand = new Command(this.RemoveFavorite);
            }

            /// <summary>
            /// Gets the pin for the currently wrapped favorite.
            /// </summary>
            public MapExPin Pin => this.favorite;

            /// <summary>
            /// Gets the label of the current <see cref="MapExPin"/>.
            /// </summary>
            public string Label => this.favorite.Label;

            /// <summary>
            /// Gets the set as primary command.
            /// </summary>
            public ICommand SetAsPrimaryCommand { get; }

            /// <summary>
            /// Gets the set as primary command.
            /// </summary>
            public ICommand RemoveFavoriteCommand { get; }

            /// <summary>
            /// Gets or sets a value indicating whether the current favorite is a primary favorite.
            /// </summary>
            public bool IsPrimary
            {
                get
                {
                    return (bool)this.GetValue(FavoriteWrapper.IsPrimaryProperty);
                }

                set
                {
                    this.SetValue(FavoriteWrapper.IsPrimaryProperty, value);
                }
            }

            /// <summary>
            /// Sets the current favorite as primary.
            /// </summary>
            private void SetAsPrimary()
            {
                // If the current view model is busy do nothing
                if (this.viewModel.IsBusy)
                {
                    return;
                }

                // If the current favorite is primary
                if (this.IsPrimary)
                {
                    return;
                }

                // Set the drop point as primary on the server
                this.viewModel.IsBusy = true;
                var uri = $"{Uris.SetPrimaryFavorite}?id={this.favorite.Id}";
                if (Settings.Instance.Contains(Settings.UserId))
                {
                    var uid = Settings.Instance.GetValue(Settings.UserId, string.Empty);
                    uri = $"{Uris.SetPrimaryFavorite}?uid={uid}&id={this.favorite.Id}";
                }

                var task = WebHelper.GetAsync(new Uri(uri));

                // Parse the server response
                var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
                var continuation = task.ContinueWith(this.ParseServerData, scheduler);

                // Update pin status
                continuation.ContinueWith(
                    _ =>
                        {
                            this.IsPrimary = true;
                            this.viewModel.IsBusy = false;
                        },
                    default(CancellationToken),
                    TaskContinuationOptions.OnlyOnRanToCompletion,
                    scheduler);

                // Setup error handling
                continuation.ContinueWith(
                    t =>
                        {
                            this.ParseTaskError(t);
                            this.viewModel.IsBusy = false;
                        },
                    default(CancellationToken),
                    TaskContinuationOptions.OnlyOnFaulted,
                    scheduler);
            }

            /// <summary>
            /// Removes the current favorite from the favorites list.
            /// </summary>
            private void RemoveFavorite()
            {
                // If the current view model is busy do nothing
                if (this.viewModel.IsBusy)
                {
                    return;
                }

                // Remove the drop points from favorites on the server
                this.viewModel.IsBusy = true;
                var uri = $"{Uris.RemoveFavorites}?id={this.favorite.Id}";
                if (Settings.Instance.Contains(Settings.UserId))
                {
                    var uid = Settings.Instance.GetValue(Settings.UserId, string.Empty);
                    uri = $"{Uris.RemoveFavorites}?uid={uid}&id={this.favorite.Id}";
                }

                var task = WebHelper.GetAsync(new Uri(uri));

                // Parse the server response
                var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
                var continuation = task.ContinueWith(this.ParseServerData, scheduler);

                // Update pin status
                continuation.ContinueWith(
                    _ =>
                        {
                            this.OnFavoriteRemoved();
                            this.viewModel.IsBusy = false;
                        },
                    default(CancellationToken),
                    TaskContinuationOptions.OnlyOnRanToCompletion,
                    scheduler);

                // Setup error handling
                continuation.ContinueWith(
                    t =>
                        {
                            this.ParseTaskError(t);
                            this.viewModel.IsBusy = false;
                        },
                    default(CancellationToken),
                    TaskContinuationOptions.OnlyOnFaulted,
                    scheduler);
            }

            /// <summary>
            /// Called when the current favorite was removed from the server.
            /// </summary>
            private void OnFavoriteRemoved()
            {
                // Remove the favorite from view model
                this.favorite.Type = MapPinType.DropPoint;
                if (this.viewModel.Favorites.Remove(this))
                {
                    // If the current favorite is a primary one
                    if (this.viewModel.PrimaryFavorite == this)
                    {
                        this.viewModel.PrimaryFavorite = null;
                    }
                }
            }

            /// <summary>
            /// Parses the server data response.
            /// </summary>
            /// <param name="task">A task that represents the asynchronous server operation.</param>
            private void ParseServerData(Task<Json.JsonValue> task)
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
                    App.DisplayAlert("Error", ex.ToString(), "OK");
                }
            }

            /// <summary>
            /// Parsed the data returned by the server.
            /// </summary>
            /// <param name="json">The data in JSON format.</param>
            private void ParseJson(Json.JsonValue json)
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
    }
}
