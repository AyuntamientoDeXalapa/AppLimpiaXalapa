using System;
using System.Collections.Generic;

namespace AppLimpia.Json
{
    /// <summary>
    /// An array of <see cref="JsonValue"/>.
    /// </summary>
    /// <seealso cref="JsonValue"/>
    /// <seealso cref="JsonObject"/>
    /// <seealso cref="JsonNumber"/>
    /// <seealso cref="JsonString"/>
    /// <seealso cref="JsonBoolean"/>
    internal sealed class JsonArray : JsonValue, IList<JsonValue>
    {
        /// <summary>
        /// Values stored in the <see cref="JsonArray"/>.
        /// </summary>
        private readonly List<JsonValue> values;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonArray"/> class.
        /// </summary>
        public JsonArray()
        {
            this.values = new List<JsonValue>();
        }

        /// <summary>
        /// Gets the data type of the current <see cref="JsonValue"/>.
        /// </summary>
        public override JsonValueType ValueType
        {
            get
            {
                return JsonValueType.Array;
            }
        }

        /// <summary>
        /// Gets the number of values contained in the <see cref="JsonArray"/>.
        /// </summary>
        public int Count
        {
            get
            {
                return this.values.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="JsonArray"/> is read-only.
        /// </summary>
        bool ICollection<JsonValue>.IsReadOnly
        {
            get
            {
                return ((ICollection<JsonValue>)this.values).IsReadOnly;
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
        public override JsonValue this[int index]
        {
            get
            {
                return this.values[index];
            }

            set
            {
                this.values[index] = value;
            }
        }

        /// <summary>
        /// Adds the specified value to the <see cref="JsonArray"/>.
        /// </summary>
        /// <param name="item">The value to add.</param>
        /// <exception cref="System.NotSupportedException">
        ///   The <see cref="JsonArray"/> is read-only.
        /// </exception>
        public void Add(JsonValue item)
        {
            this.values.Add(item);
        }

        /// <summary>
        /// Removes all values from the <see cref="JsonArray"/>.
        /// </summary>
        /// <exception cref="System.NotSupportedException">
        ///   The <see cref="JsonArray"/> is read-only.
        /// </exception>
        public void Clear()
        {
            this.values.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="JsonArray"/> contains a specific value.
        /// </summary>
        /// <param name="item">The value to locate in the <see cref="JsonArray"/>.</param>
        /// <returns><b>true</b> if value is found in the <see cref="JsonArray"/>; otherwise, <b>false</b>.</returns>
        public bool Contains(JsonValue item)
        {
            return this.values.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="JsonArray"/> to an <see cref="System.Array"/>, starting at a
        /// particular Array index.
        /// </summary>
        /// <param name="array">
        ///   The one-dimensional <see cref="System.Array"/> that is the destination of the elements copied from
        ///   <see cref="JsonObject"/>. The <see cref="System.Array"/> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <exception cref="System.ArgumentNullException">
        ///   <paramref name="array"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///   <paramref name="arrayIndex"/> is less than 0.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        ///   The number of elements in the source <see cref="JsonArray"/> is greater than the available space
        ///   from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
        /// </exception>
        public void CopyTo(JsonValue[] array, int arrayIndex)
        {
            this.values.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<JsonValue> GetEnumerator()
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
        /// Determines the index of a specific value in the <see cref="JsonArray"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="JsonArray"/>.</param>
        /// <returns>The index of item if found in the list; otherwise, -1.</returns>
        public int IndexOf(JsonValue item)
        {
            return this.values.IndexOf(item);
        }

        /// <summary>
        /// Inserts a value to the <see cref="JsonArray"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The value to insert into the <see cref="JsonArray"/>.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///   <paramref name="index"/> is not a valid index in the <see cref="JsonArray"/>.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        ///   The <see cref="JsonArray"/> is read-only.
        /// </exception>
        public void Insert(int index, JsonValue item)
        {
            this.values.Insert(index, item);
        }

        /// <summary>
        /// Removes the first occurrence of a specific value from the <see cref="JsonArray"/>.
        /// </summary>
        /// <param name="item">The value to remove from the <see cref="JsonArray"/>.</param>
        /// <returns>
        ///   <b>true</b> if value was successfully removed from the <see cref="JsonArray"/>;
        ///   otherwise, <b>false</b>.
        /// </returns>
        /// <exception cref="System.NotSupportedException">
        ///   The <see cref="JsonArray"/> is read-only.
        /// </exception>
        public bool Remove(JsonValue item)
        {
            return this.values.Remove(item);
        }

        /// <summary>
        /// Removes the <see cref="JsonArray"/> value at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the value to remove.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///   <paramref name="index"/> is not a valid index in the <see cref="JsonArray"/>.
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        ///   The <see cref="JsonArray"/> is read-only.
        /// </exception>
        public void RemoveAt(int index)
        {
            this.values.RemoveAt(index);
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
            // If parameter is null or not a JsonArray return false
            var other = obj as JsonArray;
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
            var count = this.values.Count;
            for (var i = 0; i < count; i++)
            {
                if (this.values[i] != other.values[i])
                {
                    return false;
                }
            }

            // Arrays are equal
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
            return string.Format("Array (Count = {0})", this.Count);
        }
    }
}
