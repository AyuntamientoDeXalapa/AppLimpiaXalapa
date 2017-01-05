using System;

using Xamarin.Forms;

namespace AppLimpia
{
    /// <summary>
    /// The extended Picker control.
    /// </summary>
    public class PickerEx : Picker
    {
        /// <summary>
        /// The color of the text in the current picker.
        /// </summary>
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
            "TextColor",
            typeof(Color),
            typeof(PickerEx),
            Color.Default);

        /// <summary>
        /// The text of the placeholder in the current picker.
        /// </summary>
        public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
            "Placeholder",
            typeof(string),
            typeof(PickerEx));

        /// <summary>
        /// The color of the placeholder in the current picker.
        /// </summary>
        public static readonly BindableProperty PlaceholderColorProperty = BindableProperty.Create(
            "PlaceholderColor",
            typeof(Color),
            typeof(PickerEx),
            Color.Default);

        /// <summary>
        /// Gets or sets the color of the text in the current picker.
        /// </summary>
        public Color TextColor
        {
            get
            {
                return (Color)this.GetValue(PickerEx.TextColorProperty);
            }

            set
            {
                this.SetValue(PickerEx.TextColorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the text of the placeholder in the current picker.
        /// </summary>
        public string Placeholder
        {
            get
            {
                return (string)this.GetValue(PickerEx.PlaceholderProperty);
            }

            set
            {
                this.SetValue(PickerEx.PlaceholderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the placeholder in the current picker.
        /// </summary>
        public Color PlaceholderColor
        {
            get
            {
                return (Color)this.GetValue(PickerEx.PlaceholderColorProperty);
            }

            set
            {
                this.SetValue(PickerEx.PlaceholderColorProperty, value);
            }
        }
    }
}
