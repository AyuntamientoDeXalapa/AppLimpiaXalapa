using System;
using System.Net;

using AppLimpia.Json;

namespace AppLimpia
{
    /// <summary>
    /// Exception that is thrown when the error is reported by the server.
    /// </summary>
    internal sealed class RemoteServerException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteServerException"/> class.
        /// </summary>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="title">The error title.</param>
        /// <param name="message">The error message.</param>
        /// <param name="errorResponse">The full error response received from the server.</param>
        public RemoteServerException(HttpStatusCode statusCode, string title, string message, JsonValue errorResponse)
            : base(message)
        {
            this.StatusCode = statusCode;
            this.Title = title;
            this.ErrorResponse = errorResponse;
        }

        /// <summary>
        /// Gets the HTTP status code.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Gets the error title.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the full error response received from the server.
        /// </summary>
        internal JsonValue ErrorResponse { get; }
    }
}
