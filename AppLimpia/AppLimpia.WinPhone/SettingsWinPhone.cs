using System;

using Windows.Storage;

namespace AppLimpia.WinPhone
{
    /// <summary>
    /// Windows phone <see cref="Settings"/> implementation.
    /// </summary>
    public class SettingsWinPhone : Settings
    {
        /// <summary>
        /// The lock object provided for synchronous access.
        /// </summary>
        private readonly object locker = new object();

        /// <summary>
        /// Gets the application data container for the current application.
        /// </summary>
        private static ApplicationDataContainer AppSettings => ApplicationData.Current.LocalSettings;

        /// <summary>
        /// Gets the current value of the provided setting.
        /// </summary>
        /// <typeparam name="T">Value type to get.</typeparam>
        /// <param name="key">The setting name.</param>
        /// <param name="defaultValue">The default value if not set.</param>
        /// <returns>The value for the provided setting; or the <paramref name="defaultValue"/> if not set.</returns>
        public override T GetValue<T>(string key, T defaultValue = default(T))
        {
            // Synchronize calls
            object value;
            lock (this.locker)
            {
                // If data type is a decimal
                if (typeof(T) == typeof(decimal))
                {
                    // If the setting value exists
                    string savedDecimal;
                    if (SettingsWinPhone.AppSettings.Values.ContainsKey(key))
                    {
                        savedDecimal = Convert.ToString(SettingsWinPhone.AppSettings.Values[key]);
                    }
                    else
                    {
                        // Use the default value
                        savedDecimal = defaultValue == null ? default(decimal).ToString() : defaultValue.ToString();
                    }

                    value = Convert.ToDecimal(savedDecimal, System.Globalization.CultureInfo.InvariantCulture);
                    return (T)value;
                }

                // If data type is a date time
                if (typeof(T) == typeof(DateTime))
                {
                    // If the setting value exists
                    string savedTime = null;
                    if (SettingsWinPhone.AppSettings.Values.ContainsKey(key))
                    {
                        savedTime = Convert.ToString(SettingsWinPhone.AppSettings.Values[key]);
                    }

                    // If value is empty
                    if (string.IsNullOrWhiteSpace(savedTime))
                    {
                        value = defaultValue;
                    }
                    else
                    {
                        var ticks = Convert.ToInt64(savedTime, System.Globalization.CultureInfo.InvariantCulture);
                        value = new DateTime(ticks, DateTimeKind.Utc);
                    }

                    return (T)value;
                }

                // If the setting value exists
                if (SettingsWinPhone.AppSettings.Values.ContainsKey(key))
                {
                    var tempValue = SettingsWinPhone.AppSettings.Values[key];
                    if (tempValue != null)
                    {
                        value = (T)tempValue;
                    }
                    else
                    {
                        value = defaultValue;
                    }
                }
                else
                {
                    // Use the default value
                    value = defaultValue;
                }
            }

            // Return the retrieved value
            return (value != null) ? (T)value : defaultValue;
        }

        /// <summary>
        /// Adds or updates a value of the provided setting.
        /// </summary>
        /// <typeparam name="T">Value type to set.</typeparam>
        /// <param name="key">The setting name.</param>
        /// <param name="value">The value to set.</param>
        /// <returns><c>true</c> if value was add or updated.</returns>
        public override bool SetValue<T>(string key, T value)
        {
            return this.InternalSetValue(key, value);
        }

        /// <summary>
        /// Checks whether the setting with the specified name exists.
        /// </summary>
        /// <param name="key">The setting name.</param>
        /// <returns><c>true</c> if the setting with the specified name exists; otherwise <c>false</c>.</returns>
        public override bool Contains(string key)
        {
            // Synchronize calls
            lock (this.locker)
            {
                try
                {
                    return SettingsWinPhone.AppSettings.Values.ContainsKey(key);
                }
                catch (Exception)
                {
                    // Ignored
                }

                return false;
            }
        }

        /// <summary>
        /// Removes a value of the provided setting.
        /// </summary>
        /// <param name="key">The setting name.</param>
        public override void Remove(string key)
        {
            // Synchronize calls
            lock (this.locker)
            {
                // If the setting value exists
                if (SettingsWinPhone.AppSettings.Values.ContainsKey(key))
                {
                    SettingsWinPhone.AppSettings.Values.Remove(key);
                }
            }
        }

        /// <summary>
        /// Clear all application settings.
        /// </summary>
        public override void Clear()
        {
            // Synchronize calls
            lock (this.locker)
            {
                try
                {
                    SettingsWinPhone.AppSettings.Values.Clear();
                }
                catch (Exception)
                {
                    // Ignored
                }
            }
        }

        /// <summary>
        /// Adds or updates a value of the provided setting.
        /// </summary>
        /// <param name="key">The setting name.</param>
        /// <param name="value">The value to set.</param>
        /// <returns><c>true</c> if value was add or updated.</returns>
        private bool InternalSetValue(string key, object value)
        {
            // Synchronize calls
            bool valueChanged = false;
            lock (this.locker)
            {
                if (value is decimal)
                {
                    // Convert to string
                    value = Convert.ToString(
                        Convert.ToDecimal(value),
                        System.Globalization.CultureInfo.InvariantCulture);
                }
                else if (value is DateTime)
                {
                    value = Convert.ToString(
                        Convert.ToDateTime(value).ToUniversalTime().Ticks,
                        System.Globalization.CultureInfo.InvariantCulture);
                }

                // If the setting value exists
                if (SettingsWinPhone.AppSettings.Values.ContainsKey(key))
                {
                    // If the value has changed
                    if (!SettingsWinPhone.AppSettings.Values[key].Equals(value))
                    {
                        // Store key new value
                        SettingsWinPhone.AppSettings.Values[key] = value;
                        valueChanged = true;
                    }
                }
                else
                {
                    // Create a new setting value
                    SettingsWinPhone.AppSettings.CreateContainer(key, ApplicationDataCreateDisposition.Always);
                    SettingsWinPhone.AppSettings.Values[key] = value;
                    valueChanged = true;
                }
            }

            return valueChanged;
        }
    }
}
