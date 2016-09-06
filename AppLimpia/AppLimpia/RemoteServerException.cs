using System;

namespace AppLimpia
{
    /// <summary>
    /// Exception that is thrown when the error is reported by the server.
    /// </summary>
    public class RemoteServerException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteServerException"/> class.
        /// </summary>
        /// <param name="title">The error title.</param>
        /// <param name="message">The error message.</param>
        public RemoteServerException(string title, string message)
            : base(message)
        {
            this.Title = title;
        }

        /// <summary>
        /// Gets the error title.
        /// </summary>
        public string Title { get; }
    }
}
