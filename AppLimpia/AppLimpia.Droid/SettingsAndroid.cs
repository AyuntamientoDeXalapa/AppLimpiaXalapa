using System;

using Android.App;
using Android.Content;
using Android.Preferences;

namespace AppLimpia.Droid
{
    /// <summary>
    /// Android <see cref="Settings"/> implementation.
    /// </summary>
    public class SettingsAndroid : Settings
    {
        /// <summary>
        /// The lock object provided for synchronous access.
        /// </summary>
        private readonly object locker = new object();

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
            lock (this.locker)
            {
                using (var sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Application.Context))
                {
                    return this.GetValueCore(sharedPreferences, key, defaultValue);
                }
            }
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
            // Get the desired value type
            var typeOf = typeof(T);
            if (typeOf.IsGenericType && typeOf.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                typeOf = Nullable.GetUnderlyingType(typeOf);
            }

            var typeCode = Type.GetTypeCode(typeOf);
            return this.SetValue(key, value, typeCode);
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
                using (var sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Application.Context))
                {
                    return sharedPreferences.Contains(key);
                }
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
                using (var sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Application.Context))
                {
                    using (var sharedPreferencesEditor = sharedPreferences.Edit())
                    {
                        sharedPreferencesEditor.Remove(key);
                        sharedPreferencesEditor.Commit();
                    }
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
                using (var sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Application.Context))
                {
                    using (var sharedPreferencesEditor = sharedPreferences.Edit())
                    {
                        sharedPreferencesEditor.Clear();
                        sharedPreferencesEditor.Commit();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the current value of the provided setting.
        /// </summary>
        /// <typeparam name="T">Value type to get.</typeparam>
        /// <param name="sharedPreferences">The shared references.</param>
        /// <param name="key">The setting name.</param>
        /// <param name="defaultValue">The default value if not set.</param>
        /// <returns>The value for the provided setting; or the <paramref name="defaultValue"/> if not set.</returns>
        private T GetValueCore<T>(ISharedPreferences sharedPreferences, string key, T defaultValue)
        {
            // Get the desired value type
            var typeOf = typeof(T);
            if (typeOf.IsGenericType && typeOf.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                typeOf = Nullable.GetUnderlyingType(typeOf);
            }

            // Get the stored value
            object value;
            var typeCode = Type.GetTypeCode(typeOf);
            switch (typeCode)
            {
                case TypeCode.Decimal:
                    var savedDecimal = sharedPreferences.GetString(key, string.Empty);
                    value = Convert.ToDecimal(savedDecimal, System.Globalization.CultureInfo.InvariantCulture);
                    break;

                case TypeCode.Boolean:
                    value = sharedPreferences.GetBoolean(key, Convert.ToBoolean(defaultValue));
                    break;

                case TypeCode.Int64:
                    value = sharedPreferences.GetLong(
                        key,
                        Convert.ToInt64(defaultValue, System.Globalization.CultureInfo.InvariantCulture));
                    break;

                case TypeCode.String:
                    value = sharedPreferences.GetString(key, Convert.ToString(defaultValue));
                    break;

                case TypeCode.Double:
                    var savedDouble = sharedPreferences.GetString(key, string.Empty);
                    value = Convert.ToDouble(savedDouble, System.Globalization.CultureInfo.InvariantCulture);
                    break;

                case TypeCode.Int32:
                    value = sharedPreferences.GetInt(
                        key,
                        Convert.ToInt32(defaultValue, System.Globalization.CultureInfo.InvariantCulture));
                    break;

                case TypeCode.Single:
                    value = sharedPreferences.GetFloat(
                        key,
                        Convert.ToSingle(defaultValue, System.Globalization.CultureInfo.InvariantCulture));
                    break;

                case TypeCode.DateTime:
                    if (sharedPreferences.Contains(key))
                    {
                        var ticks = sharedPreferences.GetLong(key, 0);
                        value = new DateTime(ticks, DateTimeKind.Utc);
                    }
                    else
                    {
                        return defaultValue;
                    }

                    break;

                default:
                    if (defaultValue is Guid)
                    {
                        Guid outGuid;
                        Guid.TryParse(sharedPreferences.GetString(key, Guid.Empty.ToString()), out outGuid);
                        value = outGuid;
                    }
                    else
                    {
                        throw new ArgumentException($"Value of type {typeOf.FullName} is not supported.");
                    }

                    break;
            }

            // Return the retrieved value
            return (value != null) ? (T)value : defaultValue;
        }

        /// <summary>
        /// Adds or updates a value of the provided setting.
        /// </summary>
        /// <param name="key">The setting name.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="typeCode">The value type.</param>
        /// <returns><c>true</c> if value was add or updated.</returns>
        private bool SetValue(string key, object value, TypeCode typeCode)
        {
            // Synchronize calls
            lock (this.locker)
            {
                using (var sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Application.Context))
                {
                    using (var sharedPreferencesEditor = sharedPreferences.Edit())
                    {
                        switch (typeCode)
                        {
                            case TypeCode.Decimal:
                                sharedPreferencesEditor.PutString(
                                    key,
                                    Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture));
                                break;

                            case TypeCode.Boolean:
                                sharedPreferencesEditor.PutBoolean(key, Convert.ToBoolean(value));
                                break;

                            case TypeCode.Int64:
                                sharedPreferencesEditor.PutLong(
                                    key,
                                    Convert.ToInt64(value, System.Globalization.CultureInfo.InvariantCulture));
                                break;

                            case TypeCode.String:
                                sharedPreferencesEditor.PutString(key, Convert.ToString(value));
                                break;

                            case TypeCode.Double:
                                sharedPreferencesEditor.PutString(
                                    key,
                                    Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture));
                                break;

                            case TypeCode.Int32:
                                sharedPreferencesEditor.PutInt(
                                    key,
                                    Convert.ToInt32(value, System.Globalization.CultureInfo.InvariantCulture));
                                break;

                            case TypeCode.Single:
                                sharedPreferencesEditor.PutFloat(
                                    key,
                                    Convert.ToSingle(value, System.Globalization.CultureInfo.InvariantCulture));
                                break;

                            case TypeCode.DateTime:
                                sharedPreferencesEditor.PutLong(key, Convert.ToDateTime(value).ToUniversalTime().Ticks);
                                break;

                            default:
                                if (value is Guid)
                                {
                                    sharedPreferencesEditor.PutString(key, ((Guid)value).ToString());
                                }
                                else
                                {
                                    throw new ArgumentException(
                                              $"Value of type {value.GetType().Name} is not supported.");
                                }

                                break;
                        }

                        // Commit changes
                        sharedPreferencesEditor.Commit();
                    }
                }
            }

            // Return success
            return true;
        }
    }
}
