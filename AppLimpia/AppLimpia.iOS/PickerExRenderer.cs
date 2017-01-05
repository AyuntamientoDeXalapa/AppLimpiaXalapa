using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

using Foundation;

using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(AppLimpia.PickerEx), typeof(AppLimpia.iOS.PickerExRenderer))]

#region Generated Code
// To suppress the StyleCop warning
namespace AppLimpia.iOS
#endregion
{
    /// <summary>
    /// A renderer for <see cref="PickerEx"/> control.
    /// </summary>
    public class PickerExRenderer : PickerRenderer
    {
        /// <summary>
        /// The default text color.
        /// </summary>
        private UIColor defaultTextColor;

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
                System.Diagnostics.Debug.Assert(this.Control != null, "Control is not created");

                // Save the default values
                this.defaultTextColor = this.Control.TextColor;

                // Configure the control
                this.UpdateTextColor();
                this.UpdatePlaceholder();

                // FIX: Xamarin updates placeholder on every collection change
                ((ObservableCollection<string>)e.NewElement.Items).CollectionChanged +=
                    (s, _) => { this.UpdatePlaceholder(); };
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
                this.UpdatePlaceholder();
            }
            else if (e.PropertyName == PickerEx.PlaceholderColorProperty.PropertyName)
            {
                this.UpdatePlaceholder();
            }
        }

        /// <summary>
        /// Sets the text and the color of the placeholder for the current control.
        /// </summary>
        private void UpdatePlaceholder()
        {
            // Get the placeholder text and color
            var element = (PickerEx)this.Element;
            if (element.Placeholder == null)
            {
                return;
            }

            // Update the placeholder text
            var targetColor = element.PlaceholderColor;
            var color = (element.IsEnabled && !targetColor.Equals(Color.Default)) ? targetColor : Color.Gray;
            this.Control.AttributedPlaceholder = new NSAttributedString(
                                                     element.Placeholder,
                                                     this.Control.Font,
                                                     color.ToUIColor());
        }

        /// <summary>
        /// Sets the text color for the current control.
        /// </summary>
        private void UpdateTextColor()
        {
            // Get the text olor
            var element = (PickerEx)this.Element;
            var textColor = this.Element.TextColor;
            if (!element.IsEnabled || textColor.Equals(Color.Default))
            {
                this.Control.TextColor = this.defaultTextColor;
                return;
            }

            this.Control.TextColor = textColor.ToUIColor();
        }
    }
}
