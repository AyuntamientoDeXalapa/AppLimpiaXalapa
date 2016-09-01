using System;
using System.Collections.Generic;

namespace AppLimpia.Json
{
    /// <summary>
    /// A name value collection of <see cref="JsonValue"/>.
    /// </summary>
    /// <seealso cref="JsonValue"/>
    /// <seealso cref="JsonArray"/>
    /// <seealso cref="JsonNumber"/>
    /// <seealso cref="JsonString"/>
    /// <seealso cref="JsonBoolean"/>
    internal sealed class JsonObject : JsonValue, IDictionary<string, JsonValue>
    {
        /// <summary>
        /// Names and values pairs stored in the <see cref="JsonObject"/>.
        /// </summary>
        private readonly Dictionary<string, JsonValue> values;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonObject"/> class.
        /// </summary>
        public JsonObject()
        {
            this.values = new Dictionary<string, JsonValue>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the data type of the current <see cref="JsonValue"/>.
        /// </summary>
        public override JsonValueType ValueType
        {
            get
            {
                return JsonValueType.Object;
            }
        }

        /// <summary>
        /// Gets the number of values contained in the <see cref="JsonObject"/>
        /// </summary>
        public int Count
        {
            get
            {
                return this.values.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="JsonObject"/> is read-only.
        /// </summary>
        bool ICollection<KeyValuePair<string, JsonValue>>.IsReadOnly
        {
            get
            {
                return ((ICollection<KeyValuePair<string, JsonValue>>)this.values).IsReadOnly;
            }
        }

        /// <summary>
        /// Gets an <see cref="ICollection{T}"/> containing the names of the values in the <see cref="JsonObject"/>.
        /// </summary>
        public ICollection<string> Keys
        {
            get
            {
                return this.values.Keys;
            }
        }

        /// <summary>
        /// Gets an <see cref="ICollection{T}"/> containing the values in the <see cref="JsonObject"/>.
        /// </summary>
        public ICollection<JsonValue> Values
        {
            get
            {
                return this.values.Values;
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
        public override JsonValue this[string name]
        {
            get
            {
                return this.values[name];
            }

            set
            {
                this.values[name] = value;
            }
        }

        /// <summary>
        /// Adds the specified name and value to the <see cref="JsonObject"/>.
        /// </summary>
        /// <param name="name">The name of the element to add.</param>
        /// <param name="value">The value of the element to add.</param>
        /// <exception cref="System.ArgumentNullException">
        ///   <paramref name="name"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///   A value with the same name already exists in the <see cref="JsonObject"/>.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        ///   The <see cref="JsonObject"/> is read-only.
        /// </exception>
        public void Add(string name, JsonValue value)
        {
            this.values.Add(name, value);
        }

        /// <summary>
        /// Adds an item to the <see cref="ICollection{T}"/>
        /// </summary>
        /// <param name="item">The object to add to the <see cref="ICollection{T}"/>.</param>
        /// <exception cref="System.NotSupportedException">
        ///   The <see cref="ICollection{T}"/> is read-only.
        /// </exception>
        void ICollection<KeyValuePair<string, JsonValue>>.Add(KeyValuePair<string, JsonValue> item)
        {
            ((ICollection<KeyValuePair<string, JsonValue>>)this.values).Add(item);
        }

        /// <summary>
        /// Removes all values from the <see cref="JsonObject"/>.
        /// </summary>
        /// <exception cref="System.NotSupportedException">
        ///   The <see cref="JsonObject"/> is read-only.
        /// </exception>
        public void Clear()
        {
            this.values.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="ICollection{T}"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="ICollection{T}"/>.</param>
        /// <returns>
        ///   <b>true</b> if item is found in the <see cref="ICollection{T}"/>; otherwise, <b>false</b>.
        /// </returns>
        bool ICollection<KeyValuePair<string, JsonValue>>.Contains(KeyValuePair<string, JsonValue> item)
        {
            return ((ICollection<KeyValuePair<string, JsonValue>>)this.values).Contains(item);
        }

        /// <summary>
        /// Determines whether the <see cref="JsonObject"/> contains a value with specified name.
        /// </summary>
        /// <param name="name">The name of a value to locate in the <see cref="JsonObject"/>.</param>
        /// <returns>
        ///   <b>true</b> if the <see cref="JsonObject"/> contains a value with the specified name;
        ///   otherwise, <b>false</b>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///   <paramref name="name"/> is <c>null</c>.
        /// </exception>
        public bool ContainsKey(string name)
        {
            return this.values.ContainsKey(name);
        }

        /// <summary>
        /// Copies the elements of the <see cref="ICollection{T}"/> to an <see cref="System.Array"/>, starting at a
        /// particular Array index.
        /// </summary>
        /// <param name="array">
        ///   The one-dimensional <see cref="System.Array"/> that is the destination of the elements copied from
        ///   <see cref="ICollection{T}"/>. The <see cref="System.Array"/> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <exception cref="System.ArgumentNullException">
        ///   <paramref name="array"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///   <paramref name="arrayIndex"/> is less than 0.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///   The number of elements in the source <see cref="ICollection{T}"/> is greater than the available space
        ///   from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
        /// </exception>
        void ICollection<KeyValuePair<string, JsonValue>>.CopyTo(
            KeyValuePair<string, JsonValue>[] array,
            int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, JsonValue>>)this.values).CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<KeyValuePair<string, JsonValue>> GetEnumerator()
        {
            return this.values.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.values.GetEnumerator();
        }

        /// <summary>
        /// Removes the value with the specified name from the <see cref="JsonObject"/>.
        /// </summary>
        /// <param name="name">The name of the value to remove.</param>
        /// <returns>
        ///   <b>true</b> if the element is successfully found and removed; otherwise, <b>false</b>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        ///   <paramref name="name"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        ///   The <see cref="JsonObject"/> is read-only.
        /// </exception>
        public bool Remove(string name)
        {
            return this.values.Remove(name);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ICollection{T}"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="ICollection{T}"/>.</param>
        /// <returns>
        ///   <b>true</b> if item was successfully removed from the <see cref="ICollection{T}"/>;
        ///   otherwise, <b>false</b>. This method also returns <b>false</b> if item is not
        ///   found in the original <see cref="ICollection{T}"/>.
        /// </returns>
        /// <exception cref="System.NotSupportedException">
        ///   The <see cref="JsonObject"/> is read-only.
        /// </exception>
        bool ICollection<KeyValuePair<string, JsonValue>>.Remove(KeyValuePair<string, JsonValue> item)
        {
            return ((ICollection<KeyValuePair<string, JsonValue>>)this.values).Remove(item);
        }

        /// <summary>
        /// Gets the value associated with the specified name.
        /// </summary>
        /// <param name="name">The name of the value to get.</param>
        /// <param name="value">
        ///   When this method returns, contains the value associated with the specified name,
        ///   if the name is found; otherwise, <b>null</b>. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        ///   <b>true</b> if the <see cref="JsonObject"/> contains an element with the specified name;
        ///   otherwise, <b>false</b>.
        /// </returns>
        public bool TryGetValue(string name, out JsonValue value)
        {
            return this.values.TryGetValue(name, out value);
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
            // If parameter is null or not a JsonObject return false
            var other = obj as JsonObject;
            if ((object)other == null)
            {
                return false;
            }

            // Compare values count
            if (this.values.Count != other.values.Count)
            {
                return false;
            }

            // Compare values
            foreach (var key in this.values.Keys)
            {
                // If key does not exists
                if (!other.values.ContainsKey(key))
                {
                    return false;
                }

                // Compare values with the same key
                if (this.values[key] != other.values[key])
                {
                    return false;
                }
            }

            // Objects are equal
            return true;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="JsonValue"/>.</returns>
        public override int GetHashCode()
        {
            return this.values.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="JsonValue"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="JsonValue"/>.</returns>
        public override string ToString()
        {
            return string.Format("Object (Count = {0})", this.Count);
        }
    }
}
