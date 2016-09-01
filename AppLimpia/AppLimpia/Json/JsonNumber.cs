using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace AppLimpia.Json
{
    /// <summary>
    /// A JSON number value.
    /// </summary>
    /// <seealso cref="JsonValue"/>
    /// <seealso cref="JsonObject"/>
    /// <seealso cref="JsonArray"/>
    /// <seealso cref="JsonString"/>
    /// <seealso cref="JsonBoolean"/>
    internal sealed class JsonNumber : JsonValue
    {
        /// <summary>
        /// The value indicating whether a current number is a float point number.
        /// </summary>
        private readonly bool isFloatNumber;

        /// <summary>
        /// The number data of the current <see cref="JsonNumber"/>.
        /// </summary>
        private readonly NumberData numberData;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonNumber"/> class for the specified short value.
        /// </summary>
        /// <param name="shortValue">The short value for the current <see cref="JsonNumber"/>.</param>
        public JsonNumber(short shortValue)
            : this((long)shortValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonNumber"/> class for the specified int value.
        /// </summary>
        /// <param name="intValue">The int value for the current <see cref="JsonNumber"/>.</param>
        public JsonNumber(int intValue)
            : this((long)intValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonNumber"/> class for the specified long value.
        /// </summary>
        /// <param name="longValue">The long value for the current <see cref="JsonNumber"/>.</param>
        public JsonNumber(long longValue)
        {
            this.isFloatNumber = false;
            this.numberData.IntegerValue = longValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonNumber"/> class for the specified float value.
        /// </summary>
        /// <param name="floatValue">The float value for the current <see cref="JsonNumber"/>.</param>
        public JsonNumber(float floatValue)
            : this((double)floatValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonNumber"/> class for the specified double value.
        /// </summary>
        /// <param name="doubleValue">The double value for the current <see cref="JsonNumber"/>.</param>
        public JsonNumber(double doubleValue)
        {
            this.isFloatNumber = true;
            this.numberData.FloatValue = doubleValue;
        }

        /// <summary>
        /// Gets the data type of the current <see cref="JsonValue"/>.
        /// </summary>
        public override JsonValueType ValueType
        {
            get
            {
                return JsonValueType.Number;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current <see cref="JsonNumber"/> is a float point number.
        /// </summary>
        public bool IsFloatNumber
        {
            get
            {
                return this.isFloatNumber;
            }
        }

        /// <summary>
        /// Gets a value of this <see cref="JsonValue"/> as a <see cref="System.Int16"/>.
        /// </summary>
        /// <returns>A <see cref="System.Int16"/> with the value of this <see cref="JsonValue"/>.</returns>
        /// <exception cref="System.InvalidOperationException">
        ///   <see cref="JsonValue"/> is not a number.
        /// </exception>
        /// <exception cref="System.OverflowException">
        ///   Converting <see cref="JsonValue"/> to <see cref="System.Int16"/> resulted in overflow.
        /// </exception>
        /// <seealso cref="JsonValue.IsNumber"/>
        public override short GetShortValue()
        {
            return checked((short)this.GetLongValue());
        }

        /// <summary>
        /// Gets a value of this <see cref="JsonValue"/> as a <see cref="System.Int32"/>.
        /// </summary>
        /// <returns>A <see cref="System.Int32"/> with the value of this <see cref="JsonValue"/>.</returns>
        /// <exception cref="System.InvalidOperationException">
        ///   <see cref="JsonValue"/> is not a number.
        /// </exception>
        /// <exception cref="System.OverflowException">
        ///   Converting <see cref="JsonValue"/> to <see cref="System.Int32"/> resulted in overflow.
        /// </exception>
        /// <seealso cref="JsonValue.IsNumber"/>
        public override int GetIntValue()
        {
            return checked((int)this.GetLongValue());
        }

        /// <summary>
        /// Gets a value of this <see cref="JsonValue"/> as a <see cref="System.Int64"/>.
        /// </summary>
        /// <returns>A <see cref="System.Int64"/> with the value of this <see cref="JsonValue"/>.</returns>
        /// <exception cref="System.InvalidOperationException">
        ///   <see cref="JsonValue"/> is not a number.
        /// </exception>
        /// <exception cref="System.OverflowException">
        ///   Converting <see cref="JsonValue"/> to <see cref="System.Int64"/> resulted in overflow.
        /// </exception>
        /// <seealso cref="JsonValue.IsNumber"/>
        public override long GetLongValue()
        {
            // Return long value
            return this.isFloatNumber ? checked((long)this.numberData.FloatValue) : this.numberData.IntegerValue;
        }

        /// <summary>
        /// Gets a value of this <see cref="JsonValue"/> as a <see cref="System.Single"/>.
        /// </summary>
        /// <returns>A <see cref="System.Single"/> with the value of this <see cref="JsonValue"/>.</returns>
        /// <exception cref="System.InvalidOperationException">
        ///   <see cref="JsonValue"/> is not a number.
        /// </exception>
        /// <exception cref="System.OverflowException">
        ///   Converting <see cref="JsonValue"/> to <see cref="System.Single"/> resulted in overflow.
        /// </exception>
        /// <seealso cref="JsonValue.IsNumber"/>
        public override float GetSingleValue()
        {
            // Check double value for overflow
            var value = this.GetDoubleValue();
            if ((value < float.MinValue) || (value > float.MaxValue))
            {
                throw new OverflowException();
            }

            return (float)value;
        }

        /// <summary>
        /// Gets a value of this <see cref="JsonValue"/> as a <see cref="System.Double"/>.
        /// </summary>
        /// <returns>A <see cref="System.Double"/> with the value of this <see cref="JsonValue"/>.</returns>
        /// <exception cref="System.InvalidOperationException">
        ///   <see cref="JsonValue"/> is not a number.
        /// </exception>
        /// <exception cref="System.OverflowException">
        ///   Converting <see cref="JsonValue"/> to <see cref="System.Double"/> resulted in overflow.
        /// </exception>
        /// <seealso cref="JsonValue.IsNumber"/>
        public override double GetDoubleValue()
        {
            // Return double value
            return this.isFloatNumber ? this.numberData.FloatValue : this.numberData.IntegerValue;
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
            var other = obj as JsonNumber;
            if ((object)other == null)
            {
                return false;
            }

            // Compare values
            if (this.isFloatNumber != other.isFloatNumber)
            {
                return false;
            }

            if (this.isFloatNumber)
            {
                return this.numberData.FloatValue.Equals(other.numberData.FloatValue);
            }

            return this.numberData.IntegerValue == other.numberData.IntegerValue;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="JsonValue"/>.</returns>
        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyMemberInGetHashCode
            return this.isFloatNumber
                       ? this.numberData.FloatValue.GetHashCode()
                       : this.numberData.IntegerValue.GetHashCode();
            // ReSharper restore NonReadonlyMemberInGetHashCode
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="JsonValue"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="JsonValue"/>.</returns>
        public override string ToString()
        {
            return this.isFloatNumber
                       ? this.numberData.FloatValue.ToString(CultureInfo.InvariantCulture)
                       : this.numberData.IntegerValue.ToString();
        }

        /// <summary>
        /// The number data of the <see cref="JsonNumber"/> class.
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        private struct NumberData
        {
            /// <summary>
            /// The integer value data of the <see cref="JsonNumber"/>.
            /// </summary>
            [FieldOffset(0)]
            public long IntegerValue;

            /// <summary>
            /// The float point value data of the <see cref="JsonNumber"/>.
            /// </summary>
            [FieldOffset(0)]
            public double FloatValue;
        }
    }
}
