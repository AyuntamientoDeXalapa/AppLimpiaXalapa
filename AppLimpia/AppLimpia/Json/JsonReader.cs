using System;
using System.IO;
using System.Text;

namespace AppLimpia.Json
{
    /// <summary>
    /// Represents a reader that reads data in JSON format.
    /// </summary>
    internal class JsonReader : IDisposable
    {
        /// <summary>
        /// The end of data as returned by the <see cref="TextReader"/>.
        /// </summary>
        private const int EndOfData = -1;

        /// <summary>
        /// Whether to leave the underlying stream open after the value was written.
        /// </summary>
        private readonly bool leaveOpen;

        /// <summary>
        /// The text reader containing the JSON data.
        /// </summary>
        private TextReader textReader;

        /// <summary>
        /// The current token in the input data.
        /// </summary>
        private int currentToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonReader"/> class for the JSON data string.
        /// </summary>
        /// <param name="jsonString">The <see cref="System.String"/> containing the JSON data.</param>
        /// <exception cref="ArgumentNullException"><paramref name="jsonString"/> is <c>null</c>.</exception>
        public JsonReader(string jsonString)
        {
            if (jsonString == null)
            {
                throw new ArgumentNullException("jsonString");
            }

            // Initialize a new string reader
            this.textReader = new StringReader(jsonString);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonReader"/> class for the JSON data stream.
        /// </summary>
        /// <param name="jsonStream">The <see cref="Stream"/> containing the JSON data.</param>
        /// <exception cref="ArgumentNullException"><paramref name="jsonStream"/> is <c>null</c>.</exception>
        public JsonReader(Stream jsonStream)
            : this(jsonStream, Encoding.UTF8, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonReader"/> class for the JSON data stream with specified
        /// encoding.
        /// </summary>
        /// <param name="jsonStream">The <see cref="Stream"/> containing the JSON data.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <exception cref="ArgumentNullException">
        ///   <paramref name="jsonStream"/> is <c>null</c>
        ///   or
        ///   <paramref name="encoding"/> is <c>null</c>.
        /// </exception>
        public JsonReader(Stream jsonStream, Encoding encoding)
            // ReSharper disable IntroduceOptionalParameters.Global
            : this(jsonStream, encoding, false)
            // ReSharper restore IntroduceOptionalParameters.Global
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonReader"/> class for the JSON data stream with specified
        /// encoding and option to leave a stream open.
        /// </summary>
        /// <param name="jsonStream">The <see cref="Stream"/> containing the JSON data.</param>
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
        public JsonReader(Stream jsonStream, Encoding encoding, bool leaveOpen)
        {
            if (jsonStream == null)
            {
                throw new ArgumentNullException("jsonStream");
            }

            if (encoding == null)
            {
                throw new ArgumentNullException("encoding");
            }

            // Initialize a new stream reader
            this.textReader = new StreamReader(jsonStream, encoding);
            this.leaveOpen = leaveOpen;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonReader"/> class for the JSON text reader.
        /// </summary>
        /// <param name="textReader">The <see cref="TextReader"/> containing the JSON data.</param>
        /// <exception cref="ArgumentNullException"><paramref name="textReader"/> is <c>null</c>.</exception>
        public JsonReader(TextReader textReader)
            // ReSharper disable IntroduceOptionalParameters.Global
            : this(textReader, false)
            // ReSharper restore IntroduceOptionalParameters.Global
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonReader"/> class for the JSON <see cref="TextReader"/>
        /// with an option to leave a stream open.
        /// </summary>
        /// <param name="textReader">The <see cref="TextReader"/> containing the JSON data.</param>
        /// <param name="leaveOpen">
        ///   <c>true</c> to leave the stream open after the value was written;
        ///   <c>false</c> to close the stream.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="textReader"/> is <c>null</c>.</exception>
        public JsonReader(TextReader textReader, bool leaveOpen)
        {
            if (textReader == null)
            {
                throw new ArgumentNullException("textReader");
            }

            // Store the provided text reader
            this.textReader = textReader;
            this.leaveOpen = leaveOpen;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <remarks>
        ///   Call <see cref="Dispose"/> when you are finished using the <see cref="JsonReader"/>. The 
        ///   <see cref="Dispose"/> method leaves the <see cref="JsonReader"/> in an unusable state. After calling 
        ///   <see cref="Dispose"/>, you must release all references to the <see cref="JsonReader"/> so the garbage
        ///   collector can reclaim the memory that the <see cref="JsonReader"/> was occupying.
        /// </remarks>
        public void Dispose()
        {
            // If the stream should be closed
            if ((this.textReader != null) && !this.leaveOpen)
            {
                this.textReader.Dispose();
            }

            this.textReader = null;
        }

        /// <summary>
        /// Reads the JSON value.
        /// </summary>
        /// <returns>A <see cref="JsonValue"/> that was serialized.</returns>
        /// <exception cref="FormatException">
        ///   JSON serialized object contains an invalid token
        ///   or
        ///   JSON serialized object is invalid
        ///   or
        ///   Unexpected end of string while parsing a token
        ///   or
        ///   Data contains tokens after las value.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///   The JSON value was already read.
        /// </exception>
        /// <remarks>
        ///   <para>
        ///     This method read a single JSON value. If the provided data contains more than one JSON value
        ///     the <see cref="FormatException"/> is thrown.
        ///   </para>
        /// </remarks>
        public JsonValue Read()
        {
            // The value was already read
            if (this.textReader == null)
            {
                throw new InvalidOperationException("Value was already read");
            }

            // Skip the leading white spaces
            this.currentToken = this.textReader.Read();
            if (this.IsWhitespace(this.currentToken))
            {
                this.ReadNonWhitespaceToken(true);
            }

            // Read the root JSON value
            var root = this.ReadValue();

            // Skip the trailing white spaces leaving the last token
            var consumeToken = false;
            while (this.IsWhitespace(this.currentToken))
            {
                if (consumeToken)
                {
                    this.textReader.Read();
                }

                this.currentToken = this.textReader.Peek();
                consumeToken = true;
            }

            // Validate then the JSON data is properly ended
            if (this.currentToken != JsonReader.EndOfData)
            {
                throw new FormatException(
                    string.Format("Expected an end of data but found '{0}'", (char)this.currentToken));
            }

            // Close the stream
            this.Dispose();

            // Return the value read
            return root;
        }

        /// <summary>
        /// Reads the JSON value.
        /// </summary>
        /// <returns>A <see cref="JsonValue"/> that was serialized.</returns>
        /// <exception cref="FormatException">
        ///   JSON serialized object contains an invalid token
        ///   or
        ///   JSON serialized object is invalid
        ///   or
        ///   Unexpected end of string while parsing a token
        ///   or
        ///   Data contains tokens after las value.
        /// </exception>
        private JsonValue ReadValue()
        {
            if (this.currentToken == '{')
            {
                // Read a JSON object value
                return this.ReadObjectValue();
            }

            if (this.currentToken == '[')
            {
                // Read a JSON array value
                return this.ReadArrayValue();
            }

            if ((this.currentToken == '+') || (this.currentToken == '-') ||
                ((this.currentToken >= '0') && (this.currentToken <= '9')))
            {
                // Read a JSON number value
                return this.ReadNumberValue();
            }

            if (this.currentToken == '"')
            {
                // Read a JSON string value
                return this.ReadStringValue();
            }

            if ((this.currentToken == 't') || (this.currentToken == 'f') || (this.currentToken == 'n'))
            {
                // Read a JSON literal value
                return this.ReadLiteralValue();
            }

            // Unexpected token found
            throw new FormatException(string.Format("Unexpected token '{0}'", (char)this.currentToken));
        }

        /// <summary>
        /// Reads the JSON object value.
        /// </summary>
        /// <returns>An object <see cref="JsonValue"/> read.</returns>
        /// <exception cref="FormatException">
        ///   An object in a serialized JSON object is in invalid format
        ///   or
        ///   Unexpected end of string while parsing the current token.
        /// </exception>
        private JsonValue ReadObjectValue()
        {
            // Validate caller
            System.Diagnostics.Debug.Assert(this.currentToken == '{', "This function must be called for an object");

            // Consume the token
            this.ReadNonWhitespaceToken(true);

            // Read key value pairs
            var result = new JsonObject();
            while (this.currentToken != '}')
            {
                // If the key is not a string
                if (this.currentToken != '"')
                {
                    throw new FormatException("Object key must be a string.");
                }

                // Read the key
                var key = this.ReadString();

                // Read colon token
                this.ReadNonWhitespaceToken(false);
                if (this.currentToken != ':')
                {
                    throw new FormatException(
                        string.Format("Invalid object format. Expected a ':' but found '{0}'", (char)this.currentToken));
                }

                // Read JSON value
                this.ReadNonWhitespaceToken(true);
                var value = this.ReadValue();

                // If the key is already present
                if (result.ContainsKey(key))
                {
                    throw new FormatException(string.Format("Key '{0}' is already present in the object", key));
                }

                // Add value
                result.Add(key, value);

                // Read comma or closing curly bracket symbol
                this.ReadNonWhitespaceToken(false);
                if (this.currentToken == ',')
                {
                    this.ReadNonWhitespaceToken(true);
                    if (this.currentToken == '}')
                    {
                        throw new FormatException("Invalid object format. Expected a value after ','");
                    }
                }
                else if (this.currentToken != '}')
                {
                    throw new FormatException(
                        string.Format(
                            "Invalid object format. Expected a ',' or '}}' but found '{0}'",
                            (char)this.currentToken));
                }
            }

            // Return the parsed object
            this.ReadNextToken();
            return result;
        }

        /// <summary>
        /// Reads the JSON array value.
        /// </summary>
        /// <returns>An array <see cref="JsonValue"/> read.</returns>
        /// <exception cref="FormatException">
        ///   An array in a serialized JSON object is in invalid format
        ///   or
        ///   Unexpected end of string while parsing the current token.
        /// </exception>
        private JsonValue ReadArrayValue()
        {
            // Validate caller
            System.Diagnostics.Debug.Assert(this.currentToken == '[', "This function must be called for an array");

            // Consume the token
            this.ReadNonWhitespaceToken(true);

            // Read key value pairs
            var result = new JsonArray();
            while (this.currentToken != ']')
            {
                // Read JSON value
                var value = this.ReadValue();
                result.Add(value);

                // Read comma or closing curly bracket symbol
                this.ReadNonWhitespaceToken(false);
                if (this.currentToken == ',')
                {
                    this.ReadNonWhitespaceToken(true);
                    if (this.currentToken == ']')
                    {
                        throw new FormatException("Invalid array format. Expected a value after ','");
                    }
                }
                else if (this.currentToken != ']')
                {
                    throw new FormatException(
                        string.Format(
                            "Invalid array format. Expected a ',' or ']' but found '{0}'",
                            (char)this.currentToken));
                }
            }

            // Return the parsed object
            this.ReadNextToken();
            return result;
        }

        /// <summary>
        /// Reads the JSON number value.
        /// </summary>
        /// <returns>A number <see cref="JsonValue"/> read.</returns>
        /// <exception cref="FormatException">
        ///   A number in a serialized JSON object is in invalid format
        ///   or
        ///   Unexpected end of string while parsing the current token.
        /// </exception>
        private JsonValue ReadNumberValue()
        {
            // Validate caller
            System.Diagnostics.Debug.Assert(
                (this.currentToken == '+') || (this.currentToken == '-') ||
                ((this.currentToken >= '0') && (this.currentToken <= '9')),
                "This function must be called for a numbers");

            // Read the integer part of the number
            int sign;
            var intPart = this.ReadInteger(out sign);
            double? doublePart = null;

            // Read the rest of the number as double
            if ((this.currentToken >= '0') && (this.currentToken <= '9'))
            {
                doublePart = intPart;
                while ((this.currentToken >= '0') && (this.currentToken <= '9'))
                {
                    doublePart = (doublePart.Value * 10) + (this.currentToken - '0');
                    this.ReadNextToken();
                }
            }

            // Read the decimal part of the number
            if (this.currentToken == '.')
            {
                var floatPart = this.ReadFloat();
                doublePart = (doublePart ?? intPart) + floatPart;
            }

            // Invert the sign if required
            if (sign == -1)
            {
                intPart = -intPart;
                doublePart = -doublePart;
            }

            // Read the exponent part of the number
            if ((this.currentToken == 'E') || (this.currentToken == 'e'))
            {
                // Read the exponent
                this.ReadNextToken();
                var exponent = this.ReadInteger(out sign);
                doublePart = (doublePart ?? intPart) * Math.Pow(10.0, exponent * sign);
            }

            // Return the parsed integer
            return doublePart.HasValue ? new JsonNumber(doublePart.Value) : new JsonNumber(intPart);
        }

        /// <summary>
        /// Reads an integer value.
        /// </summary>
        /// <param name="sign">A sign of the integer value.</param>
        /// <returns>An integer value read.</returns>
        /// <exception cref="System.FormatException">
        ///   A number in a serialized JSON object is in invalid format
        ///   or
        ///   Unexpected end of string while parsing the current token.
        /// </exception>
        private long ReadInteger(out int sign)
        {
            // If the symbol is a sign
            sign = 1;
            var value = 0L;
            if ((this.currentToken == '+') || (this.currentToken == '-'))
            {
                // Of number is negative
                if (this.currentToken == '-')
                {
                    sign = -1;
                }

                this.ReadNextToken();
            }

            // If the next symbol in the stream is not a digit
            if ((this.currentToken < '0') || (this.currentToken > '9'))
            {
                if (this.currentToken != -1)
                {
                    throw new FormatException(
                        string.Format(
                            "Invalid number format. Expected a digit but found '{0}'",
                            (char)this.currentToken));
                }

                throw new FormatException("Unexpected end of string while parsing a number");
            }

            // While we reading digits
            try
            {
                while ((this.currentToken >= '0') && (this.currentToken <= '9'))
                {
                    // Process the next digit
                    value = checked((value * 10) + (this.currentToken - '0'));
                    this.ReadNextToken();
                }
            }
            catch (OverflowException)
            {
                // Handled by the next method
            }

            return value;
        }

        /// <summary>
        /// Reads an float part value.
        /// </summary>
        /// <returns>An integer value read.</returns>
        /// <exception cref="System.FormatException">
        ///   A number in a serialized JSON object is in invalid format
        ///   or
        ///   Unexpected end of string while parsing the current token.
        /// </exception>
        private double ReadFloat()
        {
            // This function mus be called only for float part
            System.Diagnostics.Debug.Assert(this.currentToken == '.', "This function must be called for a float part");

            // Parse the float part
            var value = 0.0;
            var exponent = 0.1;
            this.ReadNextToken();

            // If the next symbol is not a digit
            if ((this.currentToken < '0') || (this.currentToken > '9'))
            {
                if (this.currentToken != -1)
                {
                    throw new FormatException(
                        string.Format(
                            "Invalid number format. Expected a digit but found '{0}'",
                            (char)this.currentToken));
                }

                throw new FormatException("Unexpected end of string while parsing a number");
            }

            // Parse float point part
            while ((this.currentToken >= '0') && (this.currentToken <= '9'))
            {
                // Process the next digit
                value += (this.currentToken - '0') * exponent;
                exponent *= 0.1;
                this.ReadNextToken();
            }

            return value;
        }

        /// <summary>
        /// Reads the JSON string value.
        /// </summary>
        /// <returns>A string <see cref="JsonValue"/> read.</returns>
        /// <exception cref="FormatException">
        ///   A string in a serialized JSON object is in invalid format
        ///   or
        ///   Unexpected end of string while parsing the current token.
        /// </exception>
        private JsonValue ReadStringValue()
        {
            return new JsonString(this.ReadString());
        }

        /// <summary>
        /// Reads the string value.
        /// </summary>
        /// <returns>A <see cref="System.String"/> read.</returns>
        /// <exception cref="FormatException">
        ///   A string in a serialized JSON object is in invalid format
        ///   or
        ///   Unexpected end of string while parsing the current token.
        /// </exception>
        private string ReadString()
        {
            // Validate caller
            System.Diagnostics.Debug.Assert(this.currentToken == '"', "This function must be called for a string");

            // Read all of the symbol until the end of a string
            var buffer = new StringBuilder();
            this.ReadNextToken();
            while ((this.currentToken != '"') && (this.currentToken != -1))
            {
                // If an escape character found
                if (this.currentToken == '\\')
                {
                    // Read symbol following an escape character
                    this.ReadNextToken();
                    switch (this.currentToken)
                    {
                        case '"':
                        case '\\':
                        case '/':
                            // Insert quotation, inverse solidus or solidus character
                            buffer.Append((char)this.currentToken);
                            break;
                        case 'b':
                            // Insert a backspace character
                            buffer.Append('\b');
                            break;
                        case 'f':
                            // Insert a form feed character
                            buffer.Append('\f');
                            break;
                        case 'n':
                            // Insert a line feed character
                            buffer.Append('\n');
                            break;
                        case 'r':
                            // Insert a carriage return character
                            buffer.Append('\r');
                            break;
                        case 't':
                            // Insert a tabulation character
                            buffer.Append('\t');
                            break;
                        case 'u':
                            // Parse code point
                            var codePoint = this.ReadCodePoint();
                            if (codePoint.HasValue)
                            {
                                buffer.Append(codePoint);
                            }
                            else
                            {
                                // Process character
                                continue;
                            }

                            break;
                    }
                }
                else
                {
                    buffer.Append((char)this.currentToken);
                }

                this.ReadNextToken();
            }

            // Check that a string is ending with a quote
            if (this.currentToken != '"')
            {
                throw new FormatException("Unexpected end of string while parsing a string");
            }

            // Skip closing quotation symbol
            this.ReadNextToken();
            return buffer.ToString();
        }

        /// <summary>
        /// Reads the JSON string code point.
        /// </summary>
        /// <returns>A string code point read.</returns>
        /// <exception cref="FormatException">
        ///   A string in a serialized JSON object is in invalid format
        ///   or
        ///   Unexpected end of string while parsing the current token.
        /// </exception>
        private char? ReadCodePoint()
        {
            var codePoint = 0;
            for (var i = 0; i < 4; i++)
            {
                // Read and parse a hexadecimal digit
                var symbol = this.ReadNextToken();
                if ((symbol >= '0') && (symbol <= '9'))
                {
                    codePoint = (codePoint << 4) | ((symbol - '0') & 0xF);
                }
                else if ((symbol >= 'a') && (symbol <= 'f'))
                {
                    codePoint = (codePoint << 4) | ((symbol - 'a' + 10) & 0xF);
                }
                else if ((symbol >= 'A') && (symbol <= 'F'))
                {
                    codePoint = (codePoint << 4) | ((symbol - 'A' + 10) & 0xF);
                }
                else
                {
                    // Invalid code point symbol
                    return null;
                }
            }

            // Return the parsed code point
            return (char)codePoint;
        }

        /// <summary>
        /// Reads the JSON literal value.
        /// </summary>
        /// <returns>A literal <see cref="JsonValue"/> read.</returns>
        /// <exception cref="FormatException">
        ///   A literal in a serialized JSON object is in invalid format
        ///   or
        ///   Unexpected end of string while parsing the current token.
        /// </exception>
        private JsonValue ReadLiteralValue()
        {
            // Validate caller
            System.Diagnostics.Debug.Assert(
                (this.currentToken == 't') || (this.currentToken == 'f') || (this.currentToken == 'n'),
                "This function must be called for a literal");

            // Parse a literal
            switch (this.currentToken)
            {
                case 't':
                    // Parse 'true' literal
                    if ((this.ReadNextToken() == 'r') && (this.ReadNextToken() == 'u') && (this.ReadNextToken() == 'e'))
                    {
                        this.ReadNextToken();
                        return new JsonBoolean(true);
                    }

                    break;
                case 'f':
                    // Parse 'false' literal
                    if ((this.ReadNextToken() == 'a') && (this.ReadNextToken() == 'l') && (this.ReadNextToken() == 's') &&
                        (this.ReadNextToken() == 'e'))
                    {
                        this.ReadNextToken();
                        return new JsonBoolean(false);
                    }

                    break;
                case 'n':
                    // Parse 'null' literal
                    if ((this.ReadNextToken() == 'u') && (this.ReadNextToken() == 'l') && (this.ReadNextToken() == 'l'))
                    {
                        this.ReadNextToken();
                        return null;
                    }

                    break;
            }

            // Literal was not parsed
            throw new FormatException(string.Format("Unexpected token '{0}'", (char)this.currentToken));
        }

        /// <summary>
        /// Reads the next non-whitespace token from the input data.
        /// </summary>
        /// <param name="consume">
        ///   <c>true</c> to consume the current token;
        ///   <c>false</c> to leave the token in place.
        /// </param>
        /// <remarks>
        ///   <para>
        ///     This methods skips white spaces until the first non-whitespace character.
        ///   </para>
        /// </remarks>
        private void ReadNonWhitespaceToken(bool consume)
        {
            // Consume the current token
            if (consume)
            {
                this.ReadNextToken();
            }

            // Read the next non-whitespace token
            while (this.IsWhitespace(this.currentToken))
            {
                this.currentToken = this.textReader.Read();
            }
        }

        /// <summary>
        /// Reads the next token from the input data.
        /// </summary>
        /// <returns>The token read form the input data.</returns>
        private int ReadNextToken()
        {
            this.currentToken = this.textReader.Read();
            return this.currentToken;
        }

        /// <summary>
        /// Checks whether the token is a white space character.
        /// </summary>
        /// <param name="token">The token to check.</param>
        /// <returns>
        ///   <c>true</c> - if <paramref name="token"/> is a whitespace character;
        ///   <c>false</c> otherwise.
        /// </returns>
        private bool IsWhitespace(int token)
        {
            return (token == ' ') || (token == '\t') || (token == '\r') || (token == '\n');
        }
    }
}
