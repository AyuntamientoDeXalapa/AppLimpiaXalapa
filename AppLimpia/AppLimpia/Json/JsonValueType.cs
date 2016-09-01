using System;

namespace AppLimpia.Json
{
    /// <summary>
    /// The type of the <see cref="JsonValue"/>.
    /// </summary>
    internal enum JsonValueType
    {
        /// <summary>
        /// The <see cref="JsonValue"/> is an object.
        /// </summary>
        Object,
        
        /// <summary>
        /// The <see cref="JsonValue"/> is an array.
        /// </summary>
        Array,

        /// <summary>
        /// The <see cref="JsonValue"/> is a number.
        /// </summary>
        Number,

        /// <summary>
        /// The <see cref="JsonValue"/> is a string.
        /// </summary>
        String,

        /// <summary>
        /// The <see cref="JsonValue"/> is a "true" literal.
        /// </summary>
        True,

        /// <summary>
        /// The <see cref="JsonValue"/> is a "false" literal.
        /// </summary>
        False
    }
}
