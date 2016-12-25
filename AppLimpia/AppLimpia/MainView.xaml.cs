using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AppLimpia.Properties;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace AppLimpia
{
    /// <summary>
    /// Interaction logic for MainView.xaml.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainView
    {
        /// <summary>
        /// The current binding context.
        /// </summary>
        private MainViewModel currentBindingContext;

        /// <summary>
        /// The map pins synchronizer.
        /// </summary>
        private CollectionSynchronizer<MapExPin> pinsSynchronizer;

        /// <summary>
        /// A value indicating whether to update the map when the user location changes.
        /// </summary>
        private bool update;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainView"/> class.
        /// </summary>
        public MainView()
        {
            // Parse XAML content
            this.InitializeComponent();
            if (this.Resources == null)
            {
                this.Resources = new ResourceDictionary();
            }

            // Copy application resource dictionary
            foreach (var resource in Application.Current.Resources)
            {
                this.Resources.Add(resource.Key, resource.Value);
            }

            // Center the map on Xalapa
            this.update = true;
            this.MapView.MoveToRegion(
                MapSpan.FromCenterAndRadius(new Position(19.5266, -96.9238), Distance.FromMiles(0.3)));
        }

        /// <summary>
        /// Handles the Appearing event.
        /// </summary>
        protected override void OnAppearing()
        {
            // Check the location service status
            var task = this.MapView.CheckLocationService();
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            task.ContinueWith(
                t =>
                    {
                        // If location service is not available
                        if (!t.Result)
                        {
                            App.DisplayAlert(
                                Localization.ErrorDialogTitle,
                                Localization.ErrorNoLocationService,
                                Localization.ErrorDialogDismiss);
                        }
                    },
                default(CancellationToken),
                TaskContinuationOptions.OnlyOnRanToCompletion,
                scheduler);

            // If tutorial was not shown
            if (!Settings.Instance.Contains(Settings.TutorialShown))
            {
                this.ShowTutorial();
                Settings.Instance.SetValue(Settings.TutorialShown, 1);
            }

            // Call the base member
            base.OnAppearing();
        }

        /// <summary>
        /// Handles the BindingContextChanged event.
        /// </summary>
        protected override void OnBindingContextChanged()
        {
            // Remove the event handling from the binding context
            if (this.currentBindingContext != null)
            {
                this.pinsSynchronizer.Dispose();
                this.pinsSynchronizer = null;
                this.currentBindingContext.Navigation = null;
                
                // Remove events handler
                this.MapView.Pins.Clear();
                this.currentBindingContext.PropertyChanged -= this.OnPropertyChanged;
                this.currentBindingContext.ErrorReported -= this.OnErrorReported;
            }

            // Set up the event handling from the binding context
            this.currentBindingContext = this.BindingContext as MainViewModel;
            if (this.currentBindingContext != null)
            {
                // Set up navigation context
                this.currentBindingContext.Navigation = this.Navigation;

                // Set up event handlers
                this.pinsSynchronizer = new CollectionSynchronizer<MapExPin>(
                    this.currentBindingContext.Pins,
                    this.MapView.Pins);
                this.currentBindingContext.PropertyChanged += this.OnPropertyChanged;
                this.currentBindingContext.ErrorReported += this.OnErrorReported;

                // The position is not the default one
                if (this.currentBindingContext.UserPosition != default(Position))
                {
                    // Move to the selected position
                    this.OnPropertyChanged(
                        this.currentBindingContext,
                        new PropertyChangedEventArgs(nameof(MainViewModel.UserPosition)));
                }
            }

            // Call the base member
            base.OnBindingContextChanged();
        }

        /// <summary>
        /// Handles the PropertyChanged event of ViewModel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="PropertyChangedEventArgs"/> with arguments of the event.</param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // If the user position was changed
            if (e.PropertyName == nameof(MainViewModel.UserPosition))
            {
                if (this.update)
                {
                    // Center the map on the current user position
                    this.update = false;
                    this.MapView.CenterMap(this.currentBindingContext.UserPosition);
                }
            }
            else if (e.PropertyName == nameof(MainViewModel.MapCenterCoordinates))
            {
                // Center the map on the required position
                this.MapView.CenterMap(this.currentBindingContext.MapCenterCoordinates);
            }
        }

        /// <summary>
        /// Handles the ErrorReported event of ViewModel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="EventArgs"/> with arguments of the event.</param>
        private void OnErrorReported(object sender, ErrorReportEventArgs e)
        {
            Debug.WriteLine(e.Exception.ToString());
            this.DisplayAlert(
                "Error",
                "No se pudo conectar con el servidor. Por favor verifica que esta conectado al Internet o intenta más tarde.",
                "OK");
        }

        /// <summary>
        /// Handles the Tapped event of MoreCommand button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="EventArgs"/> with arguments of the event.</param>
        private void OnMoreCommandsTapped(object sender, EventArgs e)
        {
            this.MoreCommands.IsVisible = !this.MoreCommands.IsVisible;
            this.BoxViewMoreCommands.IsVisible = this.MoreCommands.IsVisible;

            // If more commands are hidden
            if (!this.MoreCommands.IsVisible)
            {
                return;
            }

            // Color commands
            var index = 0;
            foreach (var command in this.MoreCommands.Children.Skip(1))
            {
                // If command is visible
                if ((command is Grid) && command.IsVisible)
                {
                    // If row is odd
                    index++;
                    command.BackgroundColor = (index % 2 == 1)
                                                  ? (Color)this.Resources["AccentGray"]
                                                  : (Color)this.Resources["MainGray"];
                }
            }
        }

        /// <summary>
        /// Handles the Tapped event of any command button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="EventArgs"/> with arguments of the event.</param>
        private void OnCommandTapped(object sender, EventArgs e)
        {
            // Hide more commands view
            this.MoreCommands.IsVisible = false;
            this.BoxViewMoreCommands.IsVisible = this.MoreCommands.IsVisible;
        }

        /// <summary>
        /// Shows the tutorial on the first time launch.
        /// </summary>
        private void ShowTutorial()
        {
            // Fade in the tutorial
            this.TutorialContainer.IsVisible = true;
            this.TutorialContainer.InputTransparent = false;
            this.TutorialView.FadeTo(1);

            // Fill in slides
            // ReSharper disable once UseObjectOrCollectionInitializer
            var slides = new ObservableCollection<string>();
            slides.Add(Device.OnPlatform("Tutorial-01.png", "tutorial_01.png", "Assets/Tutorial/Tutorial-01.png"));
            slides.Add(Device.OnPlatform("Tutorial-02.png", "tutorial_02.png", "Assets/Tutorial/Tutorial-02.png"));
            slides.Add(Device.OnPlatform("Tutorial-03.png", "tutorial_03.png", "Assets/Tutorial/Tutorial-03.png"));
            slides.Add(Device.OnPlatform("Tutorial-04.png", "tutorial_04.png", "Assets/Tutorial/Tutorial-04.png"));
            slides.Add(Device.OnPlatform("Tutorial-05.png", "tutorial_05.png", "Assets/Tutorial/Tutorial-05.png"));
            slides.Add(Device.OnPlatform("Tutorial-06.png", "tutorial_06.png", "Assets/Tutorial/Tutorial-06.png"));
            slides.Add(Device.OnPlatform("Tutorial-07.png", "tutorial_07.png", "Assets/Tutorial/Tutorial-07.png"));
            slides.Add(Device.OnPlatform("Tutorial-08.png", "tutorial_08.png", "Assets/Tutorial/Tutorial-08.png"));
            slides.Add(Device.OnPlatform("Tutorial-09.png", "tutorial_09.png", "Assets/Tutorial/Tutorial-09.png"));
            slides.Add(Device.OnPlatform("Tutorial-10.png", "tutorial_10.png", "Assets/Tutorial/Tutorial-10.png"));
            this.TutorialSlides.ItemsSource = slides;

            // Set up slide changed handler
            this.TutorialSlides.ItemSelected += this.OnTutorialSlidesItemSelected;
        }

        /// <summary>
        /// Handles the ItemSelected event of TutorialSlides.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="SelectedItemChangedEventArgs"/> with arguments of the event.</param>
        private void OnTutorialSlidesItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            // Get the tutorial slides
            var slides = this.TutorialSlides.ItemsSource as ObservableCollection<string>;
            if (slides == null)
            {
                return;
            }

            // If last slide is selected
            if (e.SelectedItem.Equals(slides.LastOrDefault()))
            {
                // Show start button
                this.TutorialLastSlideButtonContainer.IsVisible = true;
                this.TutorialLastSlideButtonContainer.InputTransparent = false;
                this.TutorialLastSlideButton.FadeTo(1);
            }
            else
            {
                // Hide start button
                this.TutorialLastSlideButtonContainer.InputTransparent = true;
                this.TutorialLastSlideButton.FadeTo(0);
            }
        }

        /// <summary>
        /// Handles the Clicked event of close tutorial button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="EventArgs"/> with arguments of the event.</param>
        private async void OnCloseTutorialClicked(object sender, EventArgs e)
        {
            // Fade out the tutorial
            await this.TutorialView.FadeTo(0);
            this.TutorialContainer.IsVisible = true;
            this.TutorialContainer.InputTransparent = false;
        }
    }
}
