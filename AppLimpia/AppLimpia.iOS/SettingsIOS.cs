using System;

using Foundation;

#region Generated Code
// To suppress the StyleCop warning
namespace AppLimpia.iOS
#endregion
{
    /// <summary>
    /// iOS <see cref="Settings"/> implementation.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class SettingsIOS : Settings
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
                // If the key does not exists
                var defaults = NSUserDefaults.StandardUserDefaults;
                if (defaults.ValueForKey(new NSString(key)) == null)
                {
                    return defaultValue;
                }

                // Get the desired value type
                var typeOf = typeof(T);
                if (typeOf.IsGenericType && (typeOf.GetGenericTypeDefinition() == typeof(Nullable<>)))
                {
                    typeOf = Nullable.GetUnderlyingType(typeOf);
                }

                // Get the stored value
                object value;
                var typeCode = Type.GetTypeCode(typeOf);
                switch (typeCode)
                {
                    case TypeCode.Decimal:
                        var savedDecimal = defaults.StringForKey(key);
                        value = Convert.ToDecimal(savedDecimal, System.Globalization.CultureInfo.InvariantCulture);
                        break;

                    case TypeCode.Boolean:
                        value = defaults.BoolForKey(key);
                        break;

                    case TypeCode.Int64:
                        var savedInt64 = defaults.StringForKey(key);
                        value = Convert.ToInt64(savedInt64, System.Globalization.CultureInfo.InvariantCulture);
                        break;

                    case TypeCode.Double:
                        value = defaults.DoubleForKey(key);
                        break;

                    case TypeCode.String:
                        value = defaults.StringForKey(key);
                        break;

                    case TypeCode.Int32:
                        value = (int)defaults.IntForKey(key);
                        break;

                    case TypeCode.Single:
                        value = defaults.FloatForKey(key);
                        break;

                    case TypeCode.DateTime:
                        var savedTime = defaults.StringForKey(key);
                        if (string.IsNullOrWhiteSpace(savedTime))
                        {
                            value = defaultValue;
                        }
                        else
                        {
                            var ticks = Convert.ToInt64(savedTime, System.Globalization.CultureInfo.InvariantCulture);
                            value = new DateTime(ticks, DateTimeKind.Utc);
                        }

                        break;

                    default:
                        if (defaultValue is Guid)
                        {
                            var outGuid = Guid.Empty;
                            var savedGuid = defaults.StringForKey(key);
                            if (string.IsNullOrWhiteSpace(savedGuid))
                            {
                                value = outGuid;
                            }
                            else
                            {
                                Guid.TryParse(savedGuid, out outGuid);
                                value = outGuid;
                            }
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
            if (typeOf.IsGenericType && (typeOf.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                typeOf = Nullable.GetUnderlyingType(typeOf);
            }

            // Set the value
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
                var defaults = NSUserDefaults.StandardUserDefaults;
                var keyString = new NSString(key);
                var setting = defaults.ValueForKey(keyString);
                return setting != null;
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
                var defaults = NSUserDefaults.StandardUserDefaults;
                try
                {
                    var keyString = new NSString(key);
                    if (defaults.ValueForKey(keyString) != null)
                    {
                        defaults.RemoveObject(key);
                        defaults.Synchronize();
                    }
                }
                catch (Exception)
                {
                    // Ignored
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
                var defaults = NSUserDefaults.StandardUserDefaults;
                try
                {
                    defaults.RemovePersistentDomain(NSBundle.MainBundle.BundleIdentifier);
                    defaults.Synchronize();
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
        /// <param name="typeCode">The value type.</param>
        /// <returns><c>true</c> if value was add or updated.</returns>
        private bool SetValue(string key, object value, TypeCode typeCode)
        {
            // Synchronize calls
            lock (this.locker)
            {
                var defaults = NSUserDefaults.StandardUserDefaults;
                switch (typeCode)
                {
                    case TypeCode.Decimal:
                        defaults.SetString(Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture), key);
                        break;
                    case TypeCode.Boolean:
                        defaults.SetBool(Convert.ToBoolean(value), key);
                        break;
                    case TypeCode.Int64:
                        defaults.SetString(Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture), key);
                        break;
                    case TypeCode.Double:
                        defaults.SetDouble(Convert.ToDouble(value, System.Globalization.CultureInfo.InvariantCulture), key);
                        break;
                    case TypeCode.String:
                        defaults.SetString(Convert.ToString(value), key);
                        break;
                    case TypeCode.Int32:
                        defaults.SetInt(Convert.ToInt32(value, System.Globalization.CultureInfo.InvariantCulture), key);
                        break;
                    case TypeCode.Single:
                        defaults.SetFloat(Convert.ToSingle(value, System.Globalization.CultureInfo.InvariantCulture), key);
                        break;
                    case TypeCode.DateTime:
                        defaults.SetString(Convert.ToString(Convert.ToDateTime(value).ToUniversalTime().Ticks), key);
                        break;
                    default:
                        if (value is Guid)
                        {
                            defaults.SetString(((Guid)value).ToString(), key);
                        }
                        else
                        {
                            throw new ArgumentException($"Value of type {value.GetType().Name} is not supported.");
                        }

                        break;
                }

                // Save value
                try
                {
                    defaults.Synchronize();
                }
                catch (Exception)
                {
                    // Return error
                    return false;
                }
            }

            // Return success
            return true;
        }
    }
}