using System;
using System.IO;
using System.Text;

namespace AppLimpia.Json
{
    /// <summary>
    /// Represents a writer that writes data in JSON format.
    /// </summary>
    internal class JsonWriter : IDisposable
    {
        /// <summary>
        /// The default encoding to use.
        /// </summary>
        private static readonly Encoding DefaultEncoding = new UTF8Encoding(false, true);

        /// <summary>
        /// Whether to leave the underlying stream open after the value was written.
        /// </summary>
        private readonly bool leaveOpen;

        /// <summary>
        /// The text reader containing the JSON data.
        /// </summary>
        private TextWriter textWriter;

        /// <summary>
        /// The current indent level.
        /// </summary>
        private int indentLevel;

        /// <summary>
        /// Weather to write a line break.
        /// </summary>
        private bool writeLineBreak;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonWriter"/> class for writing to the <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="jsonString">The <see cref="StringBuilder"/> to write data to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="jsonString"/> is <c>null</c>.</exception>
        public JsonWriter(StringBuilder jsonString)
            : this(jsonString, JsonWriterSettings.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonWriter"/> class for writing to the <see cref="Stream"/>.
        /// </summary>
        /// <param name="jsonStream">The <see cref="Stream"/> to write data to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="jsonStream"/> is <c>null</c>.</exception>
        public JsonWriter(Stream jsonStream)
            : this(jsonStream, JsonWriterSettings.Default, JsonWriter.DefaultEncoding, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonWriter"/> class for writing to the <see cref="Stream"/>
        /// with specified encoding.
        /// </summary>
        /// <param name="jsonStream">The <see cref="Stream"/> to write data to.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="jsonStream"/> is <c>null</c>
        ///   or
        ///   <paramref name="encoding"/> is <c>null</c>.
        /// </exception>
        public JsonWriter(Stream jsonStream, Encoding encoding)
            : this(jsonStream, JsonWriterSettings.Default, encoding, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonWriter"/> class for writing to the <see cref="Stream"/>
        /// with specified encoding and option to leave a stream open.
        /// </summary>
        /// <param name="jsonStream">The <see cref="Stream"/> to write data to.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="leaveOpen">
        ///   <c>true</c> to leave the stream open after the value was written;
        ///   <c>false</c> to close the stream.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="jsonStream"/> is <c>null</c>
        ///   or
        ///   <paramref name="encoding"/> is <c>null</c>.
        /// </exception>
        public JsonWriter(Stream jsonStream, Encoding encoding, bool leaveOpen)
            : this(jsonStream, JsonWriterSettings.Default, encoding, leaveOpen)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonWriter"/> class for writing to the <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="textWriter">The <see cref="TextWriter"/> to write data to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="textWriter"/> is <c>null</c>.</exception>
        public JsonWriter(TextWriter textWriter)
            : this(textWriter, JsonWriterSettings.Default, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonWriter"/> class for writing to the <see cref="TextWriter"/>
        /// and option to leave a stream open.
        /// </summary>
        /// <param name="textWriter">The <see cref="TextWriter"/> to write data to.</param>
        /// <param name="leaveOpen">
        ///   <c>true</c> to leave the stream open after the value was written;
        ///   <c>false</c> to close the stream.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="textWriter"/> is <c>null</c>.</exception>
        public JsonWriter(TextWriter textWriter, bool leaveOpen)
            : this(textWriter, JsonWriterSettings.Default, leaveOpen)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonWriter"/> class for writing to the 
        /// <see cref="StringBuilder"/> with specified settings.
        /// </summary>
        /// <param name="jsonString">The <see cref="StringBuilder"/> to write data to.</param>
        /// <param name="settings">The settings for writing data in JSON format.</param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="jsonString"/> is <c>null</c>
        ///   or
        ///   <paramref name="settings"/> is <c>null</c>.
        /// </exception>
        public JsonWriter(StringBuilder jsonString, JsonWriterSettings settings)
        {
            if (jsonString == null)
            {
                throw new ArgumentNullException("jsonString");
            }

            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            // Initialize a new string reader
            this.textWriter = new StringWriter(jsonString);
            this.Settings = settings;
            this.leaveOpen = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonWriter"/> class for writing to the <see cref="Stream"/>
        /// with specified settings.
        /// </summary>
        /// <param name="jsonStream">The <see cref="Stream"/> to write data to.</param>
        /// <param name="settings">The settings for writing data in JSON format.</param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="jsonStream"/> is <c>null</c>
        ///   or
        ///   <paramref name="settings"/> is <c>null</c>.
        /// </exception>
        public JsonWriter(Stream jsonStream, JsonWriterSettings settings)
            : this(jsonStream, settings, JsonWriter.DefaultEncoding, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonWriter"/> class for writing to the <see cref="Stream"/>
        /// with specified settings and encoding.
        /// </summary>
        /// <param name="jsonStream">The <see cref="Stream"/> to write data to.</param>
        /// <param name="settings">The settings for writing data in JSON format.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="jsonStream"/> is <c>null</c>
        ///   or
        ///   <paramref name="settings"/> is <c>null</c>
        ///   or
        ///   <paramref name="encoding"/> is <c>null</c>.
        /// </exception>
        public JsonWriter(Stream jsonStream, JsonWriterSettings settings, Encoding encoding)
            // ReSharper disable IntroduceOptionalParameters.Global
            : this(jsonStream, settings, encoding, false)
            // ReSharper restore IntroduceOptionalParameters.Global
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonWriter"/> class for writing to the <see cref="Stream"/>
        /// with specified settings, encoding and option to leave a stream open.
        /// </summary>
        /// <param name="jsonStream">The <see cref="Stream"/> to write data to.</param>
        /// <param name="settings">The settings for writing data in JSON format.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="leaveOpen">
        ///   <c>true</c> to leave the stream open after the value was written;
        ///   <c>false</c> to close the stream.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="jsonStream"/> is <c>null</c>
        ///   or
        ///   <paramref name="settings"/> is <c>null</c>
        ///   or
        ///   <paramref name="encoding"/> is <c>null</c>.
        /// </exception>
        public JsonWriter(Stream jsonStream, JsonWriterSettings settings, Encoding encoding, bool leaveOpen)
        {
            if (jsonStream == null)
            {
                throw new ArgumentNullException("jsonStream");
            }

            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            if (encoding == null)
            {
                throw new ArgumentNullException("encoding");
            }

            // Initialize a new stream reader
            this.textWriter = new StreamWriter(jsonStream, encoding);
            this.Settings = settings;
            this.leaveOpen = leaveOpen;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonWriter"/> class for writing to the <see cref="TextWriter"/>
        /// with specified settings.
        /// </summary>
        /// <param name="textWriter">The <see cref="TextWriter"/> to write data to.</param>
        /// <param name="settings">The settings for writing data in JSON format.</param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="textWriter"/> is <c>null</c>
        ///   or
        ///   <paramref name="settings"/> is <c>null</c>.
        /// </exception>
        public JsonWriter(TextWriter textWriter, JsonWriterSettings settings)
            // ReSharper disable IntroduceOptionalParameters.Global
            : this(textWriter, settings, false)
            // ReSharper restore IntroduceOptionalParameters.Global
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonWriter"/> class for writing to the <see cref="TextWriter"/>
        /// with specified settings and option to leave a stream open.
        /// </summary>
        /// <param name="textWriter">The <see cref="TextWriter"/> to write data to.</param>
        /// <param name="settings">The settings for writing data in JSON format.</param>
        /// <param name="leaveOpen">
        ///   <c>true</c> to leave the stream open after the value was written;
        ///   <c>false</c> to close the stream.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="textWriter"/> is <c>null</c>
        ///   or
        ///   <paramref name="settings"/> is <c>null</c>.
        /// </exception>
        public JsonWriter(TextWriter textWriter, JsonWriterSettings settings, bool leaveOpen)
        {
            if (textWriter == null)
            {
                throw new ArgumentNullException("textWriter");
            }

            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            // Store the provided text reader
            this.textWriter = textWriter;
            this.Settings = settings;
            this.leaveOpen = leaveOpen;
        }

        /// <summary>
        /// Gets the settings for writing data in JSON format.
        /// </summary>
        public JsonWriterSettings Settings { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <remarks>
        ///   Call <see cref="Dispose"/> when you are finished using the <see cref="JsonWriter"/>. The 
        ///   <see cref="Dispose"/> method leaves the <see cref="JsonWriter"/> in an unusable state. After calling 
        ///   <see cref="Dispose"/>, you must release all references to the <see cref="JsonWriter"/> so the garbage
        ///   collector can reclaim the memory that the <see cref="JsonWriter"/> was occupying.
        /// </remarks>
        public void Dispose()
        {
            // If the stream should be closed
            if ((this.textWriter != null) && !this.leaveOpen)
            {
                this.textWriter.Dispose();
            }

            this.textWriter = null;
        }

        /// <summary>
        /// Writes the JSON value.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to write.</param>
        /// <exception cref="InvalidOperationException">
        ///   The JSON value was already written.
        /// </exception>
        /// <remarks>
        ///   <para>
        ///     This method writes a single JSON value and finalizes the data. Subsequent calls to this method throws
        ///     <see cref="InvalidOperationException"/>.
        ///   </para>
        /// </remarks>
        public void Write(JsonValue value)
        {
            // The value was already written
            if (this.textWriter == null)
            {
                throw new InvalidOperationException("Value was already written");
            }

            // Write the JSON root value
            this.WriteValue(value);
            this.textWriter.Flush();

            // Close the stream
            this.Dispose();
        }

        /// <summary>
        /// Writes the JSON value.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to write.</param>
        private void WriteValue(JsonValue value)
        {
            // If the value is not null
            if (value != null)
            {
                switch (value.ValueType)
                {
                    case JsonValueType.Object:
                        this.WriteObjectValue(value);
                        break;
                    case JsonValueType.Array:
                        this.WriteArrayValue(value);
                        break;
                    case JsonValueType.Number:
                        this.WriteNumberValue(value);
                        break;
                    case JsonValueType.String:
                        this.WriteStringValue(value);
                        break;
                    case JsonValueType.True:
                        // Write JSON true literal
                        this.textWriter.Write("true");
                        break;
                    case JsonValueType.False:
                        // Write JSON false literal
                        this.textWriter.Write("false");
                        break;
                }
            }
            else
            {
                // Write JSON null literal
                this.textWriter.Write("null");
            }
        }

        /// <summary>
        /// Writes the indent for the current level.
        /// </summary>
        private void WriteIndent()
        {
            // If the output is formatted
            if (this.Settings.Formatted)
            {
                // Write the indention of the current value
                var indent = this.Settings.IndentCharCount * this.indentLevel;
                for (var i = 0; i < indent; i++)
                {
                    this.textWriter.Write(this.Settings.IndentChar);
                }
            }
        }

        /// <summary>
        /// Writes the JSON object value.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to write.</param>
        private void WriteObjectValue(JsonValue value)
        {
            // Validate caller
            System.Diagnostics.Debug.Assert(
                value.ValueType == JsonValueType.Object,
                "This function must be called for an object");

            // Write object header
            var objectValue = (JsonObject)value;
            if (this.Settings.LineBreakBeforeBrace && this.writeLineBreak && (objectValue.Count > 0))
            {
                this.textWriter.WriteLine();
                this.WriteIndent();
            }

            this.textWriter.Write('{');

            // Write object values
            this.indentLevel++;
            var first = true;
            var oldWriteLineBreak = this.writeLineBreak;
            this.writeLineBreak = true;
            foreach (var kvp in objectValue)
            {
                // Write comma after a value
                if (!first)
                {
                    this.textWriter.Write(',');
                }

                // Format the output
                if (this.Settings.Formatted)
                {
                    this.textWriter.WriteLine();
                }

                // Write object key
                this.WriteIndent();
                this.WriteString(kvp.Key);
                this.textWriter.Write(':');
                if (this.Settings.Formatted)
                {
                    this.textWriter.Write(' ');
                }

                // Write object value
                this.WriteValue(kvp.Value);
                first = false;
            }

            // Format the output
            this.writeLineBreak = oldWriteLineBreak;
            if (objectValue.Count > 0)
            {
                if (this.Settings.Formatted)
                {
                    this.textWriter.WriteLine();
                }

                // Write object footer
                this.indentLevel--;
                this.WriteIndent();
            }
            else
            {
                this.indentLevel--;
            }

            this.textWriter.Write('}');
        }

        /// <summary>
        /// Writes the JSON array value.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to write.</param>
        private void WriteArrayValue(JsonValue value)
        {
            // Validate caller
            System.Diagnostics.Debug.Assert(
                value.ValueType == JsonValueType.Array,
                "This function must be called for an array");

            // Write array header
            var arrayValue = (JsonArray)value;
            if (this.Settings.LineBreakBeforeBrace && this.writeLineBreak && (arrayValue.Count > 0))
            {
                this.textWriter.WriteLine();
                this.WriteIndent();
            }

            this.textWriter.Write('[');

            // Write array values
            this.indentLevel++;
            var first = true;
            var oldWriteLineBreak = this.writeLineBreak;
            this.writeLineBreak = false;
            foreach (var val in arrayValue)
            {
                // Write comma after a value
                if (!first)
                {
                    this.textWriter.Write(',');
                }

                // Format the output
                if (this.Settings.Formatted)
                {
                    this.textWriter.WriteLine();
                }

                // Write value
                this.WriteIndent();
                this.WriteValue(val);
                first = false;
            }

            // Format the output
            this.writeLineBreak = oldWriteLineBreak;
            if (arrayValue.Count > 0)
            {
                if (this.Settings.Formatted)
                {
                    this.textWriter.WriteLine();
                }

                // Write array footer
                this.indentLevel--;
                this.WriteIndent();
            }
            else
            {
                this.indentLevel--;
            }

            this.textWriter.Write(']');
        }

        /// <summary>
        /// Writes the JSON number value.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to write.</param>
        private void WriteNumberValue(JsonValue value)
        {
            // Validate caller
            System.Diagnostics.Debug.Assert(
                value.ValueType == JsonValueType.Number,
                "This function must be called for a numbers");

            // Write JSON number value
            this.textWriter.Write(value.ToString());
        }

        /// <summary>
        /// Writes the JSON string value.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to write.</param>
        private void WriteStringValue(JsonValue value)
        {
            // Validate caller
            System.Diagnostics.Debug.Assert(
                value.ValueType == JsonValueType.String,
                "This function must be called for a string");

            // Write JSON number value
            this.WriteString((string)value);
        }

        /// <summary>
        /// Writes the string value.
        /// </summary>
        /// <param name="value">The <see cref="System.String"/> to write.</param>
        private void WriteString(string value)
        {
            // Write opening quote
            this.textWriter.Write('"');

            // Write string character by character
            foreach (var character in value)
            {
                // Parse special characters
                switch (character)
                {
                    case '"':
                        // Insert quotation character
                        this.textWriter.Write("\\\"");
                        break;
                    case '\\':
                        // Insert inverse solidus character
                        this.textWriter.Write("\\\\");
                        break;
                    case '/':
                        // Insert solidus character
                        this.textWriter.Write("\\/");
                        break;
                    case '\b':
                        // Insert a backspace character
                        this.textWriter.Write("\\b");
                        break;
                    case '\f':
                        // Insert a form feed character
                        this.textWriter.Write("\\f");
                        break;
                    case '\n':
                        // Insert a line feed character
                        this.textWriter.Write("\\n");
                        break;
                    case '\r':
                        // Insert a carriage return character
                        this.textWriter.Write("\\r");
                        break;
                    case '\t':
                        // Insert a tabulation character
                        this.textWriter.Write("\\t");
                        break;
                    default:
                        // If character is not ASCII
                        if (this.Settings.ForceAscii && (character >= 128))
                        {
                            this.textWriter.Write("\\u{0:x4}", (int)character);
                        }
                        else
                        {
                            this.textWriter.Write(character);
                        }

                        break;
                }
            }

            // Write closing quote
            this.textWriter.Write('"');
        }
    }
}
