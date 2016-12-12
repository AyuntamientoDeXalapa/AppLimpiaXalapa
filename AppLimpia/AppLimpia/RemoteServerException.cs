using System;
using System.Net;

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
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="title">The error title.</param>
        /// <param name="message">The error message.</param>
        public RemoteServerException(HttpStatusCode statusCode, string title, string message)
            : base(message)
        {
            this.StatusCode = statusCode;
            this.Title = title;
        }

        /// <summary>
        /// Gets the HTTP status code.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Gets the error title.
        /// </summary>
        public string Title { get; }
    }
}
