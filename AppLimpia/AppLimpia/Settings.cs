using System;

namespace AppLimpia
{
    /// <summary>
    /// The application settings cross platform implementation.
    /// </summary>
    public abstract class Settings
    {
        /// <summary>
        /// The user identifier setting name.
        /// </summary>
        public const string UserId = "UserId";

        /// <summary>
        /// The user name setting name.
        /// </summary>
        public const string UserName = "UserName";

        /// <summary>
        /// The access token setting name.
        /// </summary>
        public const string AccessToken = "AccessToken";

        /// <summary>
        /// The access token expires setting name.
        /// </summary>
        public const string AccessTokenExpires = "AccessTokenExpires";

        /// <summary>
        /// The refresh token setting name.
        /// </summary>
        public const string RefreshToken = "RefreshToken";

        /// <summary>
        /// The push notification token setting name.
        /// </summary>
        public const string PushToken = "PushToken";

        /// <summary>
        /// Gets or sets the instance of the current <see cref="Settings"/> implementation.
        /// </summary>
        public static Settings Instance { get; set; }

        /// <summary>
        /// Gets the current value of the provided setting.
        /// </summary>
        /// <typeparam name="T">Value type to get.</typeparam>
        /// <param name="key">The setting name.</param>
        /// <param name="defaultValue">The default value if not set.</param>
        /// <returns>The value for the provided setting; or the <paramref name="defaultValue"/> if not set.</returns>
        public abstract T GetValue<T>(string key, T defaultValue = default(T));

        /// <summary>
        /// Adds or updates a value of the provided setting.
        /// </summary>
        /// <typeparam name="T">Value type to set.</typeparam>
        /// <param name="key">The setting name.</param>
        /// <param name="value">The value to set.</param>
        /// <returns><c>true</c> if value was add or updated.</returns>
        public abstract bool SetValue<T>(string key, T value);

        /// <summary>
        /// Checks whether the setting with the specified name exists.
        /// </summary>
        /// <param name="key">The setting name.</param>
        /// <returns><c>true</c> if the setting with the specified name exists; otherwise <c>false</c>.</returns>
        public abstract bool Contains(string key);

        /// <summary>
        /// Removes a value of the provided setting.
        /// </summary>
        /// <param name="key">The setting name.</param>
        public abstract void Remove(string key);

        /// <summary>
        /// Clear all application settings.
        /// </summary>
        public abstract void Clear();
    }
}
