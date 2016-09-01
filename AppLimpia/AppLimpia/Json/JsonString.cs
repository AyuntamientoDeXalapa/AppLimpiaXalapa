using System;

namespace AppLimpia.Json
{
    /// <summary>
    /// A JSON string value.
    /// </summary>
    /// <seealso cref="JsonValue"/>
    /// <seealso cref="JsonObject"/>
    /// <seealso cref="JsonArray"/>
    /// <seealso cref="JsonNumber"/>
    /// <seealso cref="JsonBoolean"/>
    internal sealed class JsonString : JsonValue
    {
        /// <summary>
        /// The string data of the current <see cref="JsonValue"/>.
        /// </summary>
        private readonly string stringData;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonString"/> class for the specified string value.
        /// </summary>
        /// <param name="stringValue">The string value for the current <see cref="JsonString"/>.</param>
        public JsonString(string stringValue)
        {
            this.stringData = stringValue;
        }

        /// <summary>
        /// Gets the data type of the current <see cref="JsonValue"/>.
        /// </summary>
        public override JsonValueType ValueType
        {
            get
            {
                return JsonValueType.String;
            }
        }

        /// <summary>
        /// Gets a value of this <see cref="JsonValue"/> as a <see cref="System.String"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> with the value of this <see cref="JsonValue"/>.</returns>
        /// <exception cref="System.InvalidOperationException">
        ///   <see cref="JsonValue"/> is not a <see cref="System.String"/>
        /// </exception>
        /// <seealso cref="JsonValue.IsString"/>
        public override string GetStringValue()
        {
            // Return string value
            return this.stringData;
        }

        /// <summary>
        /// Determines whether the specified <see cref="JsonValue"/> is equal to the current <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="obj">The <see cref="JsonValue"/> to compare with the current object.</param>
        /// <returns>
        ///   <b>true</b> if the specified <see cref="JsonValue"/> is equal to the current <see cref="JsonValue"/>;
        ///   otherwise, <b>false</b>.
        /// </returns>
        public override bool Equals(JsonValue obj)
        {
            // If parameter is null return false
            var other = obj as JsonString;
            if ((object)other == null)
            {
                return false;
            }

            // Compare values
            return this.stringData == other.stringData;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="JsonValue"/>.</returns>
        public override int GetHashCode()
        {
            return this.stringData.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="JsonValue"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="JsonValue"/>.</returns>
        public override string ToString()
        {
            return this.stringData;
        }
    }
}
