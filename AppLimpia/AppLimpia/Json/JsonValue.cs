using System;

namespace AppLimpia.Json
{
    /// <summary>
    /// A JSON value. Value can be an object, an array, a number, a string, or 'true' or 'false' literals.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     A type of a current value can be determined by using Is* properties of this class: <see cref="IsObject"/>,
    ///     <see cref="IsArray"/>, <see cref="IsNumber"/>, <see cref="IsString"/> and <see cref="IsBoolean"/>.
    ///   </para>
    /// </remarks>
    /// <seealso cref="JsonObject"/>
    /// <seealso cref="JsonArray"/>
    /// <seealso cref="JsonNumber"/>
    /// <seealso cref="JsonString"/>
    internal abstract class JsonValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonValue"/> class for the specified value type.
        /// </summary>
        internal JsonValue()
        {
        }

        /// <summary>
        /// Gets the data type of the current <see cref="JsonValue"/>.
        /// </summary>
        public abstract JsonValueType ValueType
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether a current <see cref="JsonValue"/> is a <see cref="JsonObject"/>.
        /// </summary>
        /// <seealso cref="JsonObject"/>
        public bool IsObject
        {
            get
            {
                return this.ValueType == JsonValueType.Object;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a current <see cref="JsonValue"/> is a <see cref="JsonArray"/>.
        /// </summary>
        /// <seealso cref="JsonArray"/>
        public bool IsArray
        {
            get
            {
                return this.ValueType == JsonValueType.Array;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a current <see cref="JsonValue"/> is a number.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     Numbers in JSON are signed but not typed. An integer number can be represented as 
        ///     <see cref="System.Int16"/>, <see cref="System.Int32"/> or <see cref="System.Int64"/>.
        ///     A float point number can be represented as <see cref="System.Single"/> or <see cref="System.Double"/>.
        ///     The conversion operators allow for conversions between integer and float point formats without
        ///     restrictions.
        ///   </para>
        ///   <para>
        ///     It should be noted that <c>IsNumber</c> property returns <c>true</c> for both integer and float
        ///     numbers. To determine whether a number is an integer of a float number use 
        ///     <see cref="JsonNumber.IsFloatNumber"/> property.
        ///   </para>
        /// </remarks>
        /// <seealso cref="GetShortValue"/>
        /// <seealso cref="GetIntValue"/>
        /// <seealso cref="GetLongValue"/>
        /// <seealso cref="GetSingleValue"/>
        /// <seealso cref="GetDoubleValue"/>
        public bool IsNumber
        {
            get
            {
                return this.ValueType == JsonValueType.Number;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a current <see cref="JsonValue"/> is a <see cref="System.String"/>.
        /// </summary>
        /// <seealso cref="GetStringValue"/>
        public bool IsString
        {
            get
            {
                return this.ValueType == JsonValueType.String;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a current <see cref="JsonValue"/> is a <see cref="System.Boolean"/>.
        /// </summary>
        /// <seealso cref="GetBooleanValue"/>
        public bool IsBoolean
        {
            get
            {
                return (this.ValueType == JsonValueType.True) || (this.ValueType == JsonValueType.False);
            }
        }

        /// <summary>
        /// Gets or sets the value of an element with the specified name.
        /// </summary>
        /// <param name="name">The name of the element to get or set.</param>
        /// <returns>The value with the specified name.</returns>
        /// <exception cref="System.ArgumentNullException">
        ///   <paramref name="name"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">
        ///   The value is retrieved and <paramref name="name"/> is not found.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        ///   The property is set and the <see cref="JsonObject"/> is read-only.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        ///   The property is not called on <see cref="JsonObject"/>.
        /// </exception>
        /// <remarks>
        ///   <para>
        ///     This indexer allows retrieval of values from a <see cref="JsonObject"/>by its name with out casting to
        ///     <see cref="JsonObject"/>. If this function is called on other class it will throw
        ///     <see cref="System.InvalidOperationException"/>.
        ///   </para>
        /// </remarks>
        /// <seealso cref="JsonObject.Item(string)"/>
        public virtual JsonValue this[string name]
        {
            get
            {
                throw new InvalidOperationException("JsonValue is not an object");
            }

            set
            {
                throw new InvalidOperationException("JsonValue is not an object");
            }
        }

        /// <summary>
        /// Gets or sets the value of an element at specified index.
        /// </summary>
        /// <param name="index">The index of the element to get or set.</param>
        /// <returns>The value at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///   <paramref name="index"/> is not a valid index in the <see cref="JsonArray"/>.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        ///   The property is set and the <see cref="JsonArray"/> is read-only.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        ///   The property is not called on <see cref="JsonArray"/>.
        /// </exception>
        /// <remarks>
        ///   <para>
        ///     This indexer allows retrieval of values from a <see cref="JsonArray"/> with out casting to
        ///     <see cref="JsonArray"/>. If this function is called on other class it will throw
        ///     <see cref="System.InvalidOperationException"/>.
        ///   </para>
        /// </remarks>
        /// <seealso cref="JsonArray.Item(int)"/>
        public virtual JsonValue this[int index]
        {
            get
            {
                throw new InvalidOperationException("JsonValue is not an array");
            }

            set
            {
                throw new InvalidOperationException("JsonValue is not an array");
            }
        }

        /// <summary>
        /// Enables explicit casts from an instance of type <see cref="JsonValue"/> to <see cref="System.Int16"/>.
        /// </summary>
        /// <param name="value">An instance of type <see cref="JsonValue"/> to cast.</param>
        /// <returns>A <see cref="System.Int16"/> value.</returns>
        /// <exception cref="System.InvalidCastException">
        ///   <see cref="JsonValue"/> is not a number.
        /// </exception>
        /// <exception cref="System.OverflowException">
        ///   Converting <see cref="JsonValue"/> to <see cref="System.Int16"/> resulted in overflow.
        /// </exception>
        /// <seealso cref="IsNumber"/>
        /// <seealso cref="GetShortValue"/>
        public static explicit operator short(JsonValue value)
        {
            return value.GetShortValue();
        }

        /// <summary>
        /// Enables explicit casts from an instance of type <see cref="JsonValue"/> to <see cref="System.Int32"/>.
        /// </summary>
        /// <param name="value">An instance of type <see cref="JsonValue"/> to cast.</param>
        /// <returns>A <see cref="System.Int32"/> value.</returns>
        /// <exception cref="System.InvalidCastException">
        ///   <see cref="JsonValue"/> is not a number.
        /// </exception>
        /// <exception cref="System.OverflowException">
        ///   Converting <see cref="JsonValue"/> to <see cref="System.Int32"/> resulted in overflow.
        /// </exception>
        /// <seealso cref="IsNumber"/>
        /// <seealso cref="GetIntValue"/>
        public static explicit operator int(JsonValue value)
        {
            return value.GetIntValue();
        }

        /// <summary>
        /// Enables explicit casts from an instance of type <see cref="JsonValue"/> to <see cref="System.Int64"/>.
        /// </summary>
        /// <param name="value">An instance of type <see cref="JsonValue"/> to cast.</param>
        /// <returns>A <see cref="System.Int64"/> value.</returns>
        /// <exception cref="System.InvalidCastException">
        ///   <see cref="JsonValue"/> is not a number.
        /// </exception>
        /// <exception cref="System.OverflowException">
        ///   Converting <see cref="JsonValue"/> to <see cref="System.Int64"/> resulted in overflow.
        /// </exception>
        /// <seealso cref="IsNumber"/>
        /// <seealso cref="GetLongValue"/>
        public static explicit operator long(JsonValue value)
        {
            return value.GetLongValue();
        }

        /// <summary>
        /// Enables explicit casts from an instance of type <see cref="JsonValue"/> to <see cref="System.Single"/>.
        /// </summary>
        /// <param name="value">An instance of type <see cref="JsonValue"/> to cast.</param>
        /// <returns>A <see cref="System.Single"/> value.</returns>
        /// <exception cref="System.InvalidCastException">
        ///   <see cref="JsonValue"/> is not a number.
        /// </exception>
        /// <exception cref="System.OverflowException">
        ///   Converting <see cref="JsonValue"/> to <see cref="System.Single"/> resulted in overflow.
        /// </exception>
        /// <seealso cref="IsNumber"/>
        /// <seealso cref="GetSingleValue"/>
        public static explicit operator float(JsonValue value)
        {
            return value.GetSingleValue();
        }

        /// <summary>
        /// Enables explicit casts from an instance of type <see cref="JsonValue"/> to <see cref="System.Double"/>.
        /// </summary>
        /// <param name="value">An instance of type <see cref="JsonValue"/> to cast.</param>
        /// <returns>A <see cref="System.Double"/> value.</returns>
        /// <exception cref="System.InvalidCastException">
        ///   <see cref="JsonValue"/> is not a number.
        /// </exception>
        /// <seealso cref="IsNumber"/>
        /// <seealso cref="GetDoubleValue"/>
        public static explicit operator double(JsonValue value)
        {
            return value.GetDoubleValue();
        }

        /// <summary>
        /// Enables explicit casts from an instance of type <see cref="JsonValue"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="value">An instance of type <see cref="JsonValue"/> to cast.</param>
        /// <returns>A <see cref="System.String"/> value.</returns>
        /// <exception cref="System.InvalidCastException">
        ///   <see cref="JsonValue"/> is not a <see cref="System.String"/>.
        /// </exception>
        /// <seealso cref="IsString"/>
        /// <seealso cref="GetStringValue"/>
        public static explicit operator string(JsonValue value)
        {
            return value.GetStringValue();
        }

        /// <summary>
        /// Enables explicit casts from an instance of type <see cref="JsonValue"/> to <see cref="System.Boolean"/>.
        /// </summary>
        /// <param name="value">An instance of type <see cref="JsonValue"/> to cast.</param>
        /// <returns>A <see cref="System.Boolean"/> value.</returns>
        /// <exception cref="System.InvalidCastException">
        ///   <see cref="JsonValue"/> is not a <see cref="System.Boolean"/>.
        /// </exception>
        /// <seealso cref="IsBoolean"/>
        /// <seealso cref="GetBooleanValue"/>
        public static explicit operator bool(JsonValue value)
        {
            return value.GetBooleanValue();
        }

        /// <summary>
        /// Enables implicit casts from an instance of type <see cref="System.Int16"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">An instance of type <see cref="System.Int16"/> to cast.</param>
        /// <returns>A <see cref="JsonValue"/> value.</returns>
        public static implicit operator JsonValue(short value)
        {
            return new JsonNumber(value);
        }

        /// <summary>
        /// Enables implicit casts from an instance of type <see cref="System.Int32"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">An instance of type <see cref="System.Int32"/> to cast.</param>
        /// <returns>A <see cref="JsonValue"/> value.</returns>
        /// <seealso cref="IsNumber"/>
        /// <seealso cref="GetIntValue"/>
        public static implicit operator JsonValue(int value)
        {
            return new JsonNumber(value);
        }

        /// <summary>
        /// Enables implicit casts from an instance of type <see cref="System.Int64"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">An instance of type <see cref="System.Int64"/> to cast.</param>
        /// <returns>A <see cref="JsonValue"/> value.</returns>
        /// <seealso cref="IsNumber"/>
        /// <seealso cref="GetLongValue"/>
        public static implicit operator JsonValue(long value)
        {
            return new JsonNumber(value);
        }

        /// <summary>
        /// Enables implicit casts from an instance of type <see cref="System.Single"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">An instance of type <see cref="System.Single"/> to cast.</param>
        /// <returns>A <see cref="JsonValue"/> value.</returns>
        /// <seealso cref="IsNumber"/>
        /// <seealso cref="GetSingleValue"/>
        public static implicit operator JsonValue(float value)
        {
            return new JsonNumber(value);
        }

        /// <summary>
        /// Enables explicit casts from an instance of type <see cref="System.Double"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">An instance of type <see cref="System.Double"/> to cast.</param>
        /// <returns>A <see cref="JsonValue"/> value.</returns>
        /// <seealso cref="IsNumber"/>
        /// <seealso cref="GetDoubleValue"/>
        public static implicit operator JsonValue(double value)
        {
            return new JsonNumber(value);
        }

        /// <summary>
        /// Enables implicit casts from an instance of type <see cref="System.String"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">An instance of type <see cref="System.String"/> to cast.</param>
        /// <returns>A <see cref="JsonValue"/> value.</returns>
        /// <seealso cref="IsString"/>
        /// <seealso cref="GetStringValue"/>
        public static implicit operator JsonValue(string value)
        {
            return new JsonString(value);
        }

        /// <summary>
        /// Enables implicit casts from an instance of type <see cref="System.Boolean"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">An instance of type <see cref="System.Boolean"/> to cast.</param>
        /// <returns>A <see cref="JsonValue"/> value.</returns>
        /// <seealso cref="IsBoolean"/>
        /// <seealso cref="GetBooleanValue"/>
        public static implicit operator JsonValue(bool value)
        {
            return new JsonBoolean(value);
        }

        /// <summary>
        /// Determines whether two specified <see cref="JsonValue"/> have the same value.
        /// </summary>
        /// <param name="a">The first <see cref="JsonValue"/> to compare, or null. </param>
        /// <param name="b">The second <see cref="JsonValue"/> to compare, or null. </param>
        /// <returns>
        ///   <c>true</c> if the value of <paramref name="a"/> is the same as the value of <paramref name="b"/>;
        ///   otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(JsonValue a, JsonValue b)
        {
            // If a and b are the same object
            if (object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If both objects are not null
            if (((object)a != null) && ((object)b != null))
            {
                return a.Equals(b);
            }

            // One object is null and another is not
            return false;
        }

        /// <summary>
        /// Determines whether two specified <see cref="JsonValue"/> have different values.
        /// </summary>
        /// <param name="a">The first <see cref="JsonValue"/> to compare, or null. </param>
        /// <param name="b">The second <see cref="JsonValue"/> to compare, or null. </param>
        /// <returns>
        ///  <c>true</c> if the value of <paramref name="a"/> is different from the value of <paramref name="b"/>;
        ///  otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(JsonValue a, JsonValue b)
        {
            return !(a == b);
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
        /// <seealso cref="IsNumber"/>
        public virtual short GetShortValue()
        {
            throw new InvalidCastException("JsonValue is not a number");
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
        /// <seealso cref="IsNumber"/>
        public virtual int GetIntValue()
        {
            throw new InvalidCastException("JsonValue is not a number");
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
        /// <seealso cref="IsNumber"/>
        public virtual long GetLongValue()
        {
            throw new InvalidCastException("JsonValue is not a number");
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
        /// <seealso cref="IsNumber"/>
        public virtual float GetSingleValue()
        {
            throw new InvalidCastException("JsonValue is not a number");
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
        /// <seealso cref="IsNumber"/>
        public virtual double GetDoubleValue()
        {
            throw new InvalidCastException("JsonValue is not a number");
        }

        /// <summary>
        /// Gets a value of this <see cref="JsonValue"/> as a <see cref="System.String"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> with the value of this <see cref="JsonValue"/>.</returns>
        /// <exception cref="System.InvalidOperationException">
        ///   <see cref="JsonValue"/> is not a <see cref="System.String"/>
        /// </exception>
        /// <seealso cref="IsString"/>
        public virtual string GetStringValue()
        {
            throw new InvalidCastException("JsonValue is not a string");
        }

        /// <summary>
        /// Gets a value of this <see cref="JsonValue"/> as a <see cref="System.Boolean"/>.
        /// </summary>
        /// <returns>A <see cref="System.Boolean"/> with the value of this <see cref="JsonValue"/>.</returns>
        /// <exception cref="System.InvalidOperationException">
        ///   <see cref="JsonValue"/> is not a <see cref="System.Boolean"/>.
        /// </exception>
        /// <seealso cref="IsBoolean"/>
        public virtual bool GetBooleanValue()
        {
            throw new InvalidCastException("JsonValue is not a boolean");
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        ///   <b>true</b> if the specified object is equal to the current object;
        ///   otherwise, <b>false</b>.
        /// </returns>
        public override bool Equals(object obj)
        {
            // If parameter is null return false
            return obj != null && this.Equals(obj as JsonValue);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="JsonValue"/>.</returns>
        public override int GetHashCode()
        {
            // GetHashCode is overridden in derived classes
            return 0;
        }

        /// <summary>
        /// Determines whether the specified <see cref="JsonValue"/> is equal to the current <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="obj">The <see cref="JsonValue"/> to compare with the current object.</param>
        /// <returns>
        ///   <b>true</b> if the specified <see cref="JsonValue"/> is equal to the current <see cref="JsonValue"/>;
        ///   otherwise, <b>false</b>.
        /// </returns>
        public virtual bool Equals(JsonValue obj)
        {
            // If parameter is null return false
            if ((object)obj == null)
            {
                return false;
            }

            // Compare value types
            return this.ValueType == obj.ValueType;
        }
    }
}
