using System;

namespace AppLimpia.Json
{
    /// <summary>
    /// Represents the settings for writing data in JSON format by the <see cref="JsonWriter"/> class.
    /// </summary>
    internal sealed class JsonWriterSettings
    {
        /// <summary>
        /// The character used for indention.
        /// </summary>
        private char indentChar;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonWriterSettings"/> class with default values.
        /// </summary>
        /// <seealso cref="Default"/>.
        public JsonWriterSettings()
        {
            this.indentChar = ' ';
            this.ForceAscii = true;
        }

        /// <summary>
        /// Gets the default settings for the <see cref="JsonWriter"/>. By default the <see cref="JsonWriter"/>
        /// writes data in UTF8 encoding without byte order mask. The encoded data does not contains insignificant
        /// whitespace.
        /// </summary>
        public static JsonWriterSettings Default
        {
            get
            {
                return new JsonWriterSettings();
            }
        }

        /// <summary>
        /// Gets the indented settings for the <see cref="JsonWriter"/>. With this settings the <see cref="JsonWriter"/>
        /// writes data in UTF8 encoding without byte order mask. The encoded values are indented and each value is placed
        /// on a separate line.
        /// </summary>
        public static JsonWriterSettings Indented
        {
            get
            {
                return new JsonWriterSettings
                           {
                               Formatted = true,
                               LineBreakBeforeBrace = true,
                               IndentChar = ' ',
                               IndentCharCount = 4
                           };
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="JsonWriter"/> forces the encoded data to contain only
        /// ASCII symbols.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///      If this value is <c>true</c> then the non-ASCII symbols are encoded as code points.
        ///   </para>
        /// </remarks>
        public bool ForceAscii { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="JsonWriter"/> formats the output.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     If this value is set to <c>true</c>, <see cref="JsonWriter"/> places each value on a separate line and
        ///     adds an indention for each value.
        ///   </para>
        /// </remarks>
        public bool Formatted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="JsonWriter"/> adds a line break before opening brace.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     This value is used only when the <see cref="Formatted"/> is set to <c>true</c>.
        ///   </para>
        /// </remarks>
        public bool LineBreakBeforeBrace { get; set; }

        /// <summary>
        /// Gets or sets the character used for indention.
        /// </summary>
        /// <exception cref="ArgumentException">New indention character is not a whitespace.</exception>
        /// <remarks>
        ///   <para>
        ///     This value is used only when the <see cref="Formatted"/> is set to <c>true</c>.
        ///   </para>
        /// </remarks>
        public char IndentChar
        {
            get
            {
                return this.indentChar;
            }

            set
            {
                if ((value != ' ') && (value != '\t') && (value != '\r') && (value != '\n'))
                {
                    throw new ArgumentException("New indention character is not a whitespace");
                }

                this.indentChar = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount of <see cref="IndentChar"/> used for each level of indention.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     This value is used only when the <see cref="Formatted"/> is set to <c>true</c>.
        ///   </para>
        /// </remarks>
        public int IndentCharCount { get; set; }
    }
}
