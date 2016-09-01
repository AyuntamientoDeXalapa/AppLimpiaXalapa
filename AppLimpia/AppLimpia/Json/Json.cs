using System;
using System.IO;
using System.Text;

namespace AppLimpia.Json
{
    /// <summary>
    /// The interface for reading and writing the data in JSON format.
    /// </summary>
    internal static class Json
    {
        /// <summary>
        /// Reads the JSON data from a <see cref="System.String"/>.
        /// </summary>
        /// <param name="jsonString">A <see cref="System.String"/> containing data in JSON format.</param>
        /// <returns>A <see cref="JsonValue"/> that was serialized in a <paramref name="jsonString"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="jsonString"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">
        ///   JSON serialized object contains an invalid token
        ///   or
        ///   JSON serialized object is invalid
        ///   or
        ///   Unexpected end of string while parsing a token.
        /// </exception>
        public static JsonValue Read(string jsonString)
        {
            using (var reader = new JsonReader(jsonString))
            {
                return reader.Read();
            }
        }

        /// <summary>
        /// Reads the JSON data from a <see cref="Stream"/>.
        /// </summary>
        /// <param name="jsonStream">A <see cref="System.String"/> containing data in JSON format.</param>
        /// <returns>A <see cref="JsonValue"/> that was serialized in a <paramref name="jsonStream"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="jsonStream"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">
        ///   JSON serialized object contains an invalid token
        ///   or
        ///   JSON serialized object is invalid
        ///   or
        ///   Unexpected end of string while parsing a token.
        /// </exception>
        public static JsonValue Read(Stream jsonStream)
        {
            using (var reader = new JsonReader(jsonStream))
            {
                return reader.Read();
            }
        }

        /// <summary>
        /// Reads the JSON data from a <see cref="Stream"/> with specified encoding.
        /// </summary>
        /// <param name="jsonStream">A <see cref="System.String"/> containing data in JSON format.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <returns>A <see cref="JsonValue"/> that was serialized in a <paramref name="jsonStream"/>.</returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="jsonStream"/> is <c>null</c>
        ///   or
        ///   <paramref name="encoding"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FormatException">
        ///   JSON serialized object contains an invalid token
        ///   or
        ///   JSON serialized object is invalid
        ///   or
        ///   Unexpected end of string while parsing a token.
        /// </exception>
        public static JsonValue Read(Stream jsonStream, Encoding encoding)
        {
            using (var reader = new JsonReader(jsonStream, encoding))
            {
                return reader.Read();
            }
        }

        /// <summary>
        /// Reads the JSON data from a <see cref="Stream"/> with specified encoding and option to leave a stream open.
        /// </summary>
        /// <param name="jsonStream">A <see cref="System.String"/> containing data in JSON format.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="leaveOpen">
        ///   <c>true</c> to leave the stream open after the value was written;
        ///   <c>false</c> to close the stream.
        /// </param>
        /// <returns>A <see cref="JsonValue"/> that was serialized in a <paramref name="jsonStream"/>.</returns>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="jsonStream"/> is <c>null</c>
        ///   or
        ///   <paramref name="encoding"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="FormatException">
        ///   JSON serialized object contains an invalid token
        ///   or
        ///   JSON serialized object is invalid
        ///   or
        ///   Unexpected end of string while parsing a token.
        /// </exception>
        public static JsonValue Read(Stream jsonStream, Encoding encoding, bool leaveOpen)
        {
            using (var reader = new JsonReader(jsonStream, encoding, leaveOpen))
            {
                return reader.Read();
            }
        }

        /// <summary>
        /// Reads the JSON data from a <see cref="TextReader"/>.
        /// </summary>
        /// <param name="jsonReader">A <see cref="TextReader"/> containing data in JSON format.</param>
        /// <returns>A <see cref="JsonValue"/> that was serialized in a <paramref name="jsonReader"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="jsonReader"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">
        ///   JSON serialized object contains an invalid token
        ///   or
        ///   JSON serialized object is invalid
        ///   or
        ///   Unexpected end of string while parsing a token.
        /// </exception>
        public static JsonValue Read(TextReader jsonReader)
        {
            using (var reader = new JsonReader(jsonReader))
            {
                return reader.Read();
            }
        }

        /// <summary>
        /// Reads the JSON data from a <see cref="TextReader"/> with option to leave a stream open.
        /// </summary>
        /// <param name="jsonReader">A <see cref="TextReader"/> containing data in JSON format.</param>
        /// <param name="leaveOpen">
        ///   <c>true</c> to leave the stream open after the value was written;
        ///   <c>false</c> to close the stream.
        /// </param>
        /// <returns>A <see cref="JsonValue"/> that was serialized in a <paramref name="jsonReader"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="jsonReader"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">
        ///   JSON serialized object contains an invalid token
        ///   or
        ///   JSON serialized object is invalid
        ///   or
        ///   Unexpected end of string while parsing a token.
        /// </exception>
        public static JsonValue Read(TextReader jsonReader, bool leaveOpen)
        {
            using (var reader = new JsonReader(jsonReader, leaveOpen))
            {
                return reader.Read();
            }
        }

        /// <summary>
        /// Writes the JSON data to a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to write.</param>
        /// <param name="jsonString">The <see cref="StringBuilder"/> to write data to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="jsonString"/> is <c>null</c>.</exception>
        public static void Write(JsonValue value, StringBuilder jsonString)
        {
            using (var writer = new JsonWriter(jsonString))
            {
                writer.Write(value);
            }
        }

        /// <summary>
        /// Writes the JSON data to a <see cref="Stream"/>.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to write.</param>
        /// <param name="jsonStream">The <see cref="Stream"/> to write data to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="jsonStream"/> is <c>null</c>.</exception>
        public static void Write(JsonValue value, Stream jsonStream)
        {
            using (var writer = new JsonWriter(jsonStream))
            {
                writer.Write(value);
            }
        }

        /// <summary>
        /// Writes the JSON data to a <see cref="Stream"/> with specified encoding.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to write.</param>
        /// <param name="jsonStream">The <see cref="Stream"/> to write data to.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="jsonStream"/> is <c>null</c>
        ///   or
        ///   <paramref name="encoding"/> is <c>null</c>.
        /// </exception>
        public static void Write(JsonValue value, Stream jsonStream, Encoding encoding)
        {
            using (var writer = new JsonWriter(jsonStream, encoding))
            {
                writer.Write(value);
            }
        }

        /// <summary>
        /// Writes the JSON data to a <see cref="Stream"/> with specified encoding and option to leave a stream open.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to write.</param>
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
        public static void Write(JsonValue value, Stream jsonStream, Encoding encoding, bool leaveOpen)
        {
            using (var writer = new JsonWriter(jsonStream, encoding, leaveOpen))
            {
                writer.Write(value);
            }
        }

        /// <summary>
        /// Writes the JSON data to a <see cref="TextWriter"/>.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to write.</param>
        /// <param name="textWriter">The <see cref="TextWriter"/> to write data to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="textWriter"/> is <c>null</c>.</exception>
        public static void Write(JsonValue value, TextWriter textWriter)
        {
            using (var writer = new JsonWriter(textWriter))
            {
                writer.Write(value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonWriter"/> class for writing to the <see cref="TextWriter"/>
        /// and option to leave a stream open.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to write.</param>
        /// <param name="textWriter">The <see cref="TextWriter"/> to write data to.</param>
        /// <param name="leaveOpen">
        ///   <c>true</c> to leave the stream open after the value was written;
        ///   <c>false</c> to close the stream.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="textWriter"/> is <c>null</c>.</exception>
        public static void Write(JsonValue value, TextWriter textWriter, bool leaveOpen)
        {
            using (var writer = new JsonWriter(textWriter, leaveOpen))
            {
                writer.Write(value);
            }
        }

        /// <summary>
        /// Writes the JSON data to a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to write.</param>
        /// <param name="jsonString">The <see cref="StringBuilder"/> to write data to.</param>
        /// <param name="settings">The settings for writing data in JSON format.</param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="jsonString"/> is <c>null</c>
        ///   or
        ///   <paramref name="settings"/> is <c>null</c>.
        /// </exception>
        public static void Write(JsonValue value, StringBuilder jsonString, JsonWriterSettings settings)
        {
            using (var writer = new JsonWriter(jsonString, settings))
            {
                writer.Write(value);
            }
        }

        /// <summary>
        /// Writes the JSON data to a <see cref="Stream"/>.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to write.</param>
        /// <param name="jsonStream">The <see cref="Stream"/> to write data to.</param>
        /// <param name="settings">The settings for writing data in JSON format.</param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="jsonStream"/> is <c>null</c>
        ///   or
        ///   <paramref name="settings"/> is <c>null</c>.
        /// </exception>
        public static void Write(JsonValue value, Stream jsonStream, JsonWriterSettings settings)
        {
            using (var writer = new JsonWriter(jsonStream, settings))
            {
                writer.Write(value);
            }
        }

        /// <summary>
        /// Writes the JSON data to a <see cref="Stream"/> with specified settings and encoding.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to write.</param>
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
        public static void Write(JsonValue value, Stream jsonStream, JsonWriterSettings settings, Encoding encoding)
        {
            using (var writer = new JsonWriter(jsonStream, settings, encoding))
            {
                writer.Write(value);
            }
        }

        /// <summary>
        /// Writes the JSON data to a <see cref="Stream"/> with specified settings, encoding and option to leave a
        /// stream open.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to write.</param>
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
        public static void Write(
            JsonValue value,
            Stream jsonStream,
            JsonWriterSettings settings,
            Encoding encoding,
            bool leaveOpen)
        {
            using (var writer = new JsonWriter(jsonStream, settings, encoding, leaveOpen))
            {
                writer.Write(value);
            }
        }

        /// <summary>
        /// Writes the JSON data to a <see cref="TextWriter"/> with specified settings.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to write.</param>
        /// <param name="textWriter">The <see cref="TextWriter"/> to write data to.</param>
        /// <param name="settings">The settings for writing data in JSON format.</param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="textWriter"/> is <c>null</c>
        ///   or
        ///   <paramref name="settings"/> is <c>null</c>.
        /// </exception>
        public static void Write(JsonValue value, TextWriter textWriter, JsonWriterSettings settings)
        {
            using (var writer = new JsonWriter(textWriter, settings))
            {
                writer.Write(value);
            }
        }

        /// <summary>
        /// Writes the JSON data to a <see cref="TextWriter"/> with specified settings and option to leave a stream
        /// open.
        /// </summary>
        /// <param name="value">The <see cref="JsonValue"/> to write.</param>
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
        public static void Write(JsonValue value, TextWriter textWriter, JsonWriterSettings settings, bool leaveOpen)
        {
            using (var writer = new JsonWriter(textWriter, settings, leaveOpen))
            {
                writer.Write(value);
            }
        }
    }
}
