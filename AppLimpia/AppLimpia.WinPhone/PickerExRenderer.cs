using System;
using System.ComponentModel;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

using Xamarin.Forms;
using Xamarin.Forms.Platform.WinRT;

[assembly: ExportRenderer(typeof(AppLimpia.PickerEx), typeof(AppLimpia.WinPhone.PickerExRenderer))]

namespace AppLimpia.WinPhone
{
    /// <summary>
    /// A renderer for <see cref="MapEx"/> control.
    /// </summary>
    public class PickerExRenderer : PickerRenderer
    {
        /// <summary>
        /// A value indicating whether to handle the selection changed event.
        /// </summary>
        private bool handle = true;

        /// <summary>
        /// Handles the ElementChanged event.
        /// </summary>
        /// <param name="e">A <see cref="ElementChangedEventArgs{Picker}"/> with arguments of the event.</param>
        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            // Call the base member
            base.OnElementChanged(e);

            // If new element is present
            if (e.NewElement != null)
            {
                // Subscribe to events
                // NOTE: This is a hack because binding to attached property throws an exception in Xamarin
                System.Diagnostics.Debug.Assert(this.Control != null, "Control is not created");
                this.Control.DropDownClosed += this.OnDropDownClosed;
                this.Control.SelectionChanged += this.OnSelectionChanged;

                // Configure the control
                this.UpdatePlaceholderText();
                this.UpdateTextColor();
                this.UpdateForecolor();
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

            // If text color is changed
            if (e.PropertyName == PickerEx.TextColorProperty.PropertyName)
            {
                this.UpdateTextColor();
            }
            else if (e.PropertyName == PickerEx.PlaceholderProperty.PropertyName)
            {
                this.UpdatePlaceholderText();
            }
            else if (e.PropertyName == PickerEx.PlaceholderColorProperty.PropertyName)
            {
                this.UpdateForecolor();
            }
        }

        /// <summary>
        /// Gets the Brush for the specified color.
        /// </summary>
        /// <param name="color">The Color to create brush.</param>
        /// <returns>The created brush.</returns>
        private static Brush ColorToBrush(Xamarin.Forms.Color color)
        {
            var winColor = Windows.UI.Color.FromArgb(
                (byte)(color.A * 255.0),
                (byte)(color.R * 255.0),
                (byte)(color.G * 255.0),
                (byte)(color.B * 255.0));
            return new SolidColorBrush(winColor);
        }

        /// <summary>
        /// Handles the DropDownClosed event of Control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="object"/> with arguments of the event.</param>
        private void OnDropDownClosed(object sender, object e)
        {
            if (this.handle)
            {
                this.UpdateForecolor();
            }

            this.handle = true;
        }

        /// <summary>
        /// Handles the SelectionChanged event of Control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="SelectionChangedEventArgs"/> with arguments of the event.</param>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combobox = sender as ComboBox;
            System.Diagnostics.Debug.Assert(combobox != null, "Invalid sender");
            this.handle = !combobox.IsDropDownOpen;
            this.UpdateForecolor();
        }

        /// <summary>
        /// Sets the text of the placeholder for the current control.
        /// </summary>
        private void UpdatePlaceholderText()
        {
            var element = (PickerEx)this.Element;
            if (element.Placeholder != null)
            {
                this.Control.PlaceholderText = element.Placeholder;
            }
            else
            {
                this.Control.ClearValue(ComboBox.PlaceholderTextProperty);
            }
        }

        /// <summary>
        /// Sets the text color for the current control.
        /// </summary>
        private void UpdateTextColor()
        {
            this.Control.Foreground = PickerExRenderer.ColorToBrush(((PickerEx)this.Element).TextColor);
        }

        /// <summary>
        /// Updates the fore color based on the selection.
        /// </summary>
        private void UpdateForecolor()
        {
            // If control exists
            if (this.Control != null)
            {
                var element = (PickerEx)this.Element;
                var color = this.Control.SelectedIndex >= 0 ? element.TextColor : element.PlaceholderColor;
                this.Control.Foreground = PickerExRenderer.ColorToBrush(color);
            }
        }
    }
}
