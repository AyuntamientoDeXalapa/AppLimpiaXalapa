using System;

namespace AppLimpia.Json
{
    /// <summary>
    /// A JSON boolean value.
    /// </summary>
    /// <seealso cref="JsonValue"/>
    /// <seealso cref="JsonObject"/>
    /// <seealso cref="JsonArray"/>
    /// <seealso cref="JsonNumber"/>
    /// <seealso cref="JsonString"/>
    internal sealed class JsonBoolean : JsonValue
    {
        /// <summary>
        /// The boolean data of the current <see cref="JsonValue"/>.
        /// </summary>
        private readonly bool boolData;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonBoolean"/> class for the specified boolean value.
        /// </summary>
        /// <param name="booleanValue">The boolean value for the current <see cref="JsonValue"/>.</param>
        public JsonBoolean(bool booleanValue)
        {
            this.boolData = booleanValue;
        }

        /// <summary>
        /// Gets the data type of the current <see cref="JsonValue"/>.
        /// </summary>
        public override JsonValueType ValueType
        {
            get
            {
                return this.boolData ? JsonValueType.True : JsonValueType.False;
            }
        }

        /// <summary>
        /// Gets a value of this <see cref="JsonValue"/> as a <see cref="System.Boolean"/>.
        /// </summary>
        /// <returns>A <see cref="System.Boolean"/> with the value of this <see cref="JsonValue"/>.</returns>
        /// <exception cref="System.InvalidOperationException">
        ///   <see cref="JsonValue"/> is not a <see cref="System.Boolean"/>.
        /// </exception>
        /// <seealso cref="JsonValue.IsBoolean"/>
        public override bool GetBooleanValue()
        {
            // Return boolean value
            return this.boolData;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="JsonValue"/>.</returns>
        public override int GetHashCode()
        {
            return this.boolData.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="JsonValue"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="JsonValue"/>.</returns>
        public override string ToString()
        {
            return this.boolData ? "true" : "false";
        }
    }
}
