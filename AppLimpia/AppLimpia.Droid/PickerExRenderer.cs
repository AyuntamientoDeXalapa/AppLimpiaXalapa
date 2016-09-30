using System;
using System.ComponentModel;

using Android.Content.Res;
using Android.Text;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(AppLimpia.PickerEx), typeof(AppLimpia.Droid.PickerExRenderer))]

namespace AppLimpia.Droid
{
    /// <summary>
    /// A renderer for <see cref="MapEx"/> control.
    /// </summary>
    public class PickerExRenderer : PickerRenderer
    {
        /// <summary>
        /// The default text color.
        /// </summary>
        private ColorStateList defaultTextColor;

        /// <summary>
        /// The default hint text color.
        /// </summary>
        private ColorStateList defaultHintTextColor;

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

                // Configure the control
                this.Control.ClearFocus();
                this.UpdatePlaceholderText();
                this.UpdateTextColor();
                this.UpdatePlaceholderColor();
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
            else if (e.PropertyName == PickerEx.PlaceholderTextProperty.PropertyName)
            {
                this.UpdatePlaceholderText();
            }
            else if (e.PropertyName == PickerEx.PlaceholderColorProperty.PropertyName)
            {
                this.UpdatePlaceholderColor();
            }
        }

        /// <summary>
        /// Sets the text of the placeholder for the current control.
        /// </summary>
        private void UpdatePlaceholderText()
        {
            var element = (PickerEx)this.Element;
            element.Title = element.PlaceholderText;
        }

        /// <summary>
        /// Sets the text color for the current control.
        /// </summary>
        private void UpdateTextColor()
        {
            // Get the default hint text color
            if (this.defaultTextColor == null)
            {
                this.defaultTextColor = this.Control.TextColors;
            }

            // Set the new hint text color
            var textColor = ((PickerEx)this.Element).TextColor;
            this.Control.SetTextColor(textColor.ToAndroidPreserveDisabled(this.defaultTextColor));
        }

        /// <summary>
        /// Updates the placeholder color for the current control.
        /// </summary>
        private void UpdatePlaceholderColor()
        {
            // Get the default hint text color
            if (this.defaultHintTextColor == null)
            {
                this.defaultHintTextColor = this.Control.HintTextColors;
            }

            // Set the new hint text color
            var placeholderColor = ((PickerEx)this.Element).PlaceholderColor;
            this.Control.SetHintTextColor(placeholderColor.ToAndroidPreserveDisabled(this.defaultHintTextColor));
        }
    }
}