using System;

namespace AppLimpia.Json
{
    /// <summary>
    /// The extension methods for getting the JSON values.
    /// </summary>
    internal static class JsonExtensions
    {
        /// <summary>
        /// Gets a value of an element with the specified name, or default value if the element does not exists.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to get an element with specified name.</param>
        /// <param name="name">The name of the element to get.</param>
        /// <returns>
        ///   A <see cref="JsonValue"/> with the specified name, or default value if the element with specified
        ///   name does not exists.
        /// </returns>
        public static JsonValue GetItemOrDefault(this JsonValue value, string name)
        {
            var obj1 = value as JsonObject;
            JsonValue item;
            if ((obj1 != null) && obj1.TryGetValue(name, out item))
            {
                return item;
            }

            return null;
        }

        /// <summary>
        /// Gets a value of an element with the specified name, or default value if the element does not exists.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to get an element with specified name.</param>
        /// <param name="name">The name of the element to get.</param>
        /// <param name="defaultValue">A value to return if the element with specified name does not exists.</param>
        /// <returns>
        ///   A <see cref="JsonValue"/> with the specified name, or default value if the element with specified
        ///   name does not exists.
        /// </returns>
        public static JsonValue GetItemOrDefault(this JsonValue value, string name, JsonValue defaultValue)
        {
            var obj1 = value as JsonObject;
            JsonValue item;
            if ((obj1 != null) && obj1.TryGetValue(name, out item))
            {
                return item;
            }

            return defaultValue;
        }

        /// <summary>
        /// Gets a value of an element at the specified index, or default value if the element does not exists.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to get an element at the specified index.</param>
        /// <param name="index">The index of the element to get.</param>
        /// <returns>
        ///   A <see cref="JsonValue"/> at the specified index, or default value if the element at the specified index
        ///   does not exists.
        /// </returns>
        public static JsonValue GetItemOrDefault(this JsonValue value, int index)
        {
            var arr1 = value as JsonArray;
            if ((arr1 != null) && (index >= 0) && (index < arr1.Count))
            {
                return arr1[index];
            }

            return null;
        }

        /// <summary>
        /// Gets a value of an element at the specified index, or default value if the element does not exists.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to get an element at the specified index.</param>
        /// <param name="index">The index of the element to get.</param>
        /// <param name="defaultValue">A value to return if the element at the specified index does not exists.</param>
        /// <returns>
        ///   A <see cref="JsonValue"/> at the specified index, or default value if the element at the specified index
        ///   does not exists.
        /// </returns>
        public static JsonValue GetItemOrDefault(this JsonValue value, int index, JsonValue defaultValue)
        {
            var arr1 = value as JsonArray;
            if ((arr1 != null) && (index >= 0) && (index < arr1.Count))
            {
                return arr1[index];
            }

            return defaultValue;
        }

        /// <summary>
        /// Gets a value of this <see cref="JsonValue"/> as a <see cref="System.Int16"/>, or default value if the
        /// <see cref="JsonValue"/> can not be converted to <see cref="System.Int16"/>.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to convert to <see cref="System.Int16"/>.</param>
        /// <returns>
        ///   A <see cref="System.Int16"/> with the value of this <see cref="JsonValue"/> if the <see cref="JsonValue"/>
        ///   is a number value, or default value if the <see cref="JsonValue"/> is not a number.
        /// </returns>
        public static short GetShortValueOrDefault(this JsonValue value)
        {
            return ((value != null) && value.IsNumber) ? value.GetShortValue() : default(short);
        }

        /// <summary>
        /// Gets a value of this <see cref="JsonValue"/> as a <see cref="System.Int16"/>, or default value if the
        /// <see cref="JsonValue"/> can not be converted to <see cref="System.Int16"/>.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to convert to <see cref="System.Int16"/>.</param>
        /// <param name="defaultValue">A value to return if the <see cref="JsonValue"/> is not a number.</param>
        /// <returns>
        ///   A <see cref="System.Int16"/> with the value of this <see cref="JsonValue"/> if the <see cref="JsonValue"/>
        ///   is a number value, or default value if the <see cref="JsonValue"/> is not a number.
        /// </returns>
        public static short GetShortValueOrDefault(this JsonValue value, short defaultValue)
        {
            return ((value != null) && value.IsNumber) ? value.GetShortValue() : defaultValue;
        }

        /// <summary>
        /// Gets a value of this <see cref="JsonValue"/> as a <see cref="System.Int32"/>, or default value if the
        /// <see cref="JsonValue"/> can not be converted to <see cref="System.Int32"/>.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to convert to <see cref="System.Int32"/>.</param>
        /// <returns>
        ///   A <see cref="System.Int32"/> with the value of this <see cref="JsonValue"/> if the <see cref="JsonValue"/>
        ///   is a number value, or default value if the <see cref="JsonValue"/> is not a number.
        /// </returns>
        public static int GetIntValueOrDefault(this JsonValue value)
        {
            return ((value != null) && value.IsNumber) ? value.GetIntValue() : default(int);
        }

        /// <summary>
        /// Gets a value of this <see cref="JsonValue"/> as a <see cref="System.Int32"/>, or default value if the
        /// <see cref="JsonValue"/> can not be converted to <see cref="System.Int32"/>.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to convert to <see cref="System.Int32"/>.</param>
        /// <param name="defaultValue">A value to return if the <see cref="JsonValue"/> is not a number.</param>
        /// <returns>
        ///   A <see cref="System.Int32"/> with the value of this <see cref="JsonValue"/> if the <see cref="JsonValue"/>
        ///   is a number value, or default value if the <see cref="JsonValue"/> is not a number.
        /// </returns>
        public static int GetIntValueOrDefault(this JsonValue value, int defaultValue)
        {
            return ((value != null) && value.IsNumber) ? value.GetIntValue() : defaultValue;
        }

        /// <summary>
        /// Gets a value of this <see cref="JsonValue"/> as a <see cref="System.Int64"/>, or default value if the
        /// <see cref="JsonValue"/> can not be converted to <see cref="System.Int64"/>.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to convert to <see cref="System.Int64"/>.</param>
        /// <returns>
        ///   A <see cref="System.Int64"/> with the value of this <see cref="JsonValue"/> if the <see cref="JsonValue"/>
        ///   is a number value, or default value if the <see cref="JsonValue"/> is not a number.
        /// </returns>
        public static long GetLongValueOrDefault(this JsonValue value)
        {
            return ((value != null) && value.IsNumber) ? value.GetLongValue() : default(long);
        }

        /// <summary>
        /// Gets a value of this <see cref="JsonValue"/> as a <see cref="System.Int64"/>, or default value if the
        /// <see cref="JsonValue"/> can not be converted to <see cref="System.Int64"/>.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to convert to <see cref="System.Int64"/>.</param>
        /// <param name="defaultValue">A value to return if the <see cref="JsonValue"/> is not a number.</param>
        /// <returns>
        ///   A <see cref="System.Int64"/> with the value of this <see cref="JsonValue"/> if the <see cref="JsonValue"/>
        ///   is a number value, or default value if the <see cref="JsonValue"/> is not a number.
        /// </returns>
        public static long GetLongValueOrDefault(this JsonValue value, long defaultValue)
        {
            return ((value != null) && value.IsNumber) ? value.GetLongValue() : defaultValue;
        }

        /// <summary>
        /// Gets a value of this <see cref="JsonValue"/> as a <see cref="System.Single"/>, or default value if the
        /// <see cref="JsonValue"/> can not be converted to <see cref="System.Single"/>.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to convert to <see cref="System.Single"/>.</param>
        /// <returns>
        ///   A <see cref="System.Single"/> with the value of this <see cref="JsonValue"/> if the <see cref="JsonValue"/>
        ///   is a number value, or default value if the <see cref="JsonValue"/> is not a number.
        /// </returns>
        public static float GetSingleValueOrDefault(this JsonValue value)
        {
            return ((value != null) && value.IsNumber) ? value.GetSingleValue() : default(float);
        }

        /// <summary>
        /// Gets a value of this <see cref="JsonValue"/> as a <see cref="System.Single"/>, or default value if the
        /// <see cref="JsonValue"/> can not be converted to <see cref="System.Single"/>.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to convert to <see cref="System.Single"/>.</param>
        /// <param name="defaultValue">A value to return if the <see cref="JsonValue"/> is not a number.</param>
        /// <returns>
        ///   A <see cref="System.Single"/> with the value of this <see cref="JsonValue"/> if the <see cref="JsonValue"/>
        ///   is a number value, or default value if the <see cref="JsonValue"/> is not a number.
        /// </returns>
        public static float GetSingleValueOrDefault(this JsonValue value, float defaultValue)
        {
            return ((value != null) && value.IsNumber) ? value.GetSingleValue() : defaultValue;
        }

        /// <summary>
        /// Gets a value of this <see cref="JsonValue"/> as a <see cref="System.Double"/>, or default value if the
        /// <see cref="JsonValue"/> can not be converted to <see cref="System.Double"/>.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to convert to <see cref="System.Double"/>.</param>
        /// <returns>
        ///   A <see cref="System.Double"/> with the value of this <see cref="JsonValue"/> if the <see cref="JsonValue"/>
        ///   is a number value, or default value if the <see cref="JsonValue"/> is not a number.
        /// </returns>
        public static double GetDoubleValueOrDefault(this JsonValue value)
        {
            return ((value != null) && value.IsNumber) ? value.GetDoubleValue() : default(double);
        }

        /// <summary>
        /// Gets a value of this <see cref="JsonValue"/> as a <see cref="System.Double"/>, or default value if the
        /// <see cref="JsonValue"/> can not be converted to <see cref="System.Double"/>.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to convert to <see cref="System.Double"/>.</param>
        /// <param name="defaultValue">A value to return if the <see cref="JsonValue"/> is not a number.</param>
        /// <returns>
        ///   A <see cref="System.Double"/> with the value of this <see cref="JsonValue"/> if the <see cref="JsonValue"/>
        ///   is a number value, or default value if the <see cref="JsonValue"/> is not a number.
        /// </returns>
        public static double GetDoubleValueOrDefault(this JsonValue value, double defaultValue)
        {
            return ((value != null) && value.IsNumber) ? value.GetDoubleValue() : defaultValue;
        }

        /// <summary>
        /// Gets a value of this <see cref="JsonValue"/> as a <see cref="System.String"/>, or default value if the
        /// <see cref="JsonValue"/> can not be converted to <see cref="System.String"/>.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to convert to <see cref="System.String"/>.</param>
        /// <returns>
        ///   A <see cref="System.String"/> with the value of this <see cref="JsonValue"/> if the <see cref="JsonValue"/>
        ///   is a number value, or default value if the <see cref="JsonValue"/> is not a number.
        /// </returns>
        public static string GetStringValueOrDefault(this JsonValue value)
        {
            return ((value != null) && value.IsString) ? value.GetStringValue() : default(string);
        }

        /// <summary>
        /// Gets a value of this <see cref="JsonValue"/> as a <see cref="System.String"/>, or default value if the
        /// <see cref="JsonValue"/> can not be converted to <see cref="System.String"/>.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to convert to <see cref="System.String"/>.</param>
        /// <param name="defaultValue">A value to return if the <see cref="JsonValue"/> is not a number.</param>
        /// <returns>
        ///   A <see cref="System.String"/> with the value of this <see cref="JsonValue"/> if the <see cref="JsonValue"/>
        ///   is a number value, or default value if the <see cref="JsonValue"/> is not a number.
        /// </returns>
        public static string GetStringValueOrDefault(this JsonValue value, string defaultValue)
        {
            return ((value != null) && value.IsString) ? value.GetStringValue() : defaultValue;
        }

        /// <summary>
        /// Gets a value of this <see cref="JsonValue"/> as a <see cref="System.String"/>, or default value if the
        /// <see cref="JsonValue"/> can not be converted to <see cref="System.String"/>.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to convert to <see cref="System.String"/>.</param>
        /// <returns>
        ///   A <see cref="System.String"/> with the value of this <see cref="JsonValue"/> if the <see cref="JsonValue"/>
        ///   is a number value, or default value if the <see cref="JsonValue"/> is not a number.
        /// </returns>
        public static bool GetBooleanValueOrDefault(this JsonValue value)
        {
            return (value != null) && value.IsBoolean && value.GetBooleanValue();
        }

        /// <summary>
        /// Gets a value of this <see cref="JsonValue"/> as a <see cref="System.String"/>, or default value if the
        /// <see cref="JsonValue"/> can not be converted to <see cref="System.String"/>.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to convert to <see cref="System.String"/>.</param>
        /// <param name="defaultValue">A value to return if the <see cref="JsonValue"/> is not a number.</param>
        /// <returns>
        ///   A <see cref="System.String"/> with the value of this <see cref="JsonValue"/> if the <see cref="JsonValue"/>
        ///   is a number value, or default value if the <see cref="JsonValue"/> is not a number.
        /// </returns>
        public static bool GetBooleanValueOrDefault(this JsonValue value, bool defaultValue)
        {
            return ((value != null) && value.IsBoolean) ? value.GetBooleanValue() : defaultValue;
        }
    }
}
