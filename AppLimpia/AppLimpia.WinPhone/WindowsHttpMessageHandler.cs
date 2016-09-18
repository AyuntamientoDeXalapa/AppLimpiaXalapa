using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Windows.Web.Http.Filters;

namespace AppLimpia.WinPhone
{
    /// <summary>
    /// The Windows HTTP protocol message handler.
    /// </summary>
    public class WindowsHttpMessageHandler : HttpMessageHandler
    {
        /// <summary>
        /// The undefined protocol version.
        /// </summary>
        private static readonly Version NoVersion = new Version(0, 0);

        /// <summary>
        /// The 1.0 protocol version.
        /// </summary>
        private static readonly Version Version10 = new Version(1, 0);

        /// <summary>
        /// The 1.1 protocol version.
        /// </summary>
        private static readonly Version Version11 = new Version(1, 1);

        /// <summary>
        /// The HTTP communication client.
        /// </summary>
        private readonly Windows.Web.Http.HttpClient client;

        /// <summary>
        /// A value indicating whether the current object is disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsHttpMessageHandler"/> class.
        /// </summary>
        /// <param name="httpFilter">The HTTP filter to use for handling response messages.</param>
        public WindowsHttpMessageHandler(IHttpFilter httpFilter = null)
        {
            if (httpFilter == null)
            {
                httpFilter = new HttpBaseProtocolFilter();
            }

            // Initialize the HTTP client
            this.client = new Windows.Web.Http.HttpClient(httpFilter);
            this.disposed = false;
        }

        /// <summary>
        /// Sends the asynchronous request to the server.
        /// </summary>
        /// <param name="request">The request to be send.</param>
        /// <param name="cancellationToken">The operation cancellation token.</param>
        /// <returns>The Task representing the asynchronous operation.</returns>
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Validate that the object is no disposed
                this.EnsureNotDisposed();

                // Execute the request
                var convertedRequest =
                    await WindowsHttpMessageHandler.ConvertRequest(request, cancellationToken).ConfigureAwait(false);
                var response =
                    await this.client.SendRequestAsync(convertedRequest).AsTask(cancellationToken).ConfigureAwait(false);
                var convertedResponse =
                    await WindowsHttpMessageHandler.ConvertResponse(response, cancellationToken).ConfigureAwait(false);

                // Return the converted response
                return convertedResponse;
            }
            catch (Exception ex)
            {
                throw new WebException(ex.Message, ex, WebExceptionStatus.ConnectFailure, null);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> if disposing; <c>false</c> - if called by finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            // Dispose the managed resources
            if (disposing && !this.disposed)
            {
                this.client.Dispose();
                this.disposed = true;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Converts the request message to be send.
        /// </summary>
        /// <param name="request">The request message to be transformed.</param>
        /// <param name="cancellationToken">The operation cancellation token.</param>
        /// <returns>The Task representing the asynchronous operation.</returns>
        private static async Task<Windows.Web.Http.HttpRequestMessage> ConvertRequest(
            HttpRequestMessage request,
            // ReSharper disable once UnusedParameter.Local
            CancellationToken cancellationToken)
        {
            // Create the request message
            var converted = new Windows.Web.Http.HttpRequestMessage
            {
                Method = new Windows.Web.Http.HttpMethod(request.Method.Method),
                Content = await WindowsHttpMessageHandler.ConvertContent(request.Content).ConfigureAwait(false),
                RequestUri = request.RequestUri
            };

            // Copy request headers
            WindowsHttpMessageHandler.CopyHeaders(request.Headers, converted.Headers);

            // Copy request properties
            foreach (var property in request.Properties)
            {
                converted.Properties.Add(property);
            }

            // Return the transformed request
            return converted;
        }

        /// <summary>
        /// Converts the request message that was send.
        /// </summary>
        /// <param name="request">The request message to be transformed.</param>
        /// <param name="cancellationToken">The operation cancellation token.</param>
        /// <returns>The Task representing the asynchronous operation.</returns>
        private static async Task<HttpRequestMessage> ConvertRequest(
            Windows.Web.Http.HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // Convert the request message
            var converted = new HttpRequestMessage
            {
                Method = new HttpMethod(request.Method.Method),
                RequestUri = request.RequestUri,
                Content = await WindowsHttpMessageHandler.ConvertContent(request.Content, cancellationToken)
                    .ConfigureAwait(false)
            };

            // Copy request headers
            foreach (var header in request.Headers)
            {
                converted.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            // Copy request properties
            foreach (var prop in request.Properties)
            {
                converted.Properties.Add(prop);
            }

            // Return the transformed request
            return converted;
        }

        /// <summary>
        /// Copies the headers.
        /// </summary>
        /// <param name="source">The source to copy headers from.</param>
        /// <param name="destination">The destination to copy headers to.</param>
        private static void CopyHeaders(
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> source,
            IDictionary<string, string> destination)
        {
            var headers = from kvp in source
                          from val in kvp.Value
                          select new KeyValuePair<string, string>(kvp.Key, val);
            foreach (var header in headers)
            {
                destination.Add(header);
            }
        }

        /// <summary>
        /// Converts the convent from System.Net.Http.HttpContent to Windows.Web.Http.IHttpContent.
        /// </summary>
        /// <param name="content">The System.Net.Http.HttpContent to convert.</param>
        /// <returns>Converted Windows.Web.Http.IHttpContent.</returns>
        private static async Task<Windows.Web.Http.IHttpContent> ConvertContent(HttpContent content)
        {
            // If no content to convert
            if (content == null)
            {
                return null;
            }

            // Convert the content
            var stream = await content.ReadAsStreamAsync().ConfigureAwait(false);
            var converted = new Windows.Web.Http.HttpStreamContent(stream.AsInputStream());

            // Copy content headers
            WindowsHttpMessageHandler.CopyHeaders(content.Headers, converted.Headers);

            // Return the converted content
            return converted;
        }

        /// <summary>
        /// Converts the convent from Windows.Web.Http.IHttpContent to System.Net.Http.HttpContent.
        /// </summary>
        /// <param name="content">The Windows.Web.Http.IHttpContent to convert.</param>
        /// <param name="cancellationToken">The operation cancellation token.</param>
        /// <returns>Converted System.Net.Http.HttpContent.</returns>
        private static async Task<HttpContent> ConvertContent(
            Windows.Web.Http.IHttpContent content,
            CancellationToken cancellationToken)
        {
            // If no content to convert
            if (content == null)
            {
                return null;
            }

            // Convert the content
            var stream = await content.ReadAsInputStreamAsync().AsTask(cancellationToken).ConfigureAwait(false);
            var converted = new StreamContent(stream.AsStreamForRead());

            // Copy content headers
            foreach (var header in content.Headers)
            {
                converted.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            // Return the converted content
            return converted;
        }

        /// <summary>
        /// Converts the received response message.
        /// </summary>
        /// <param name="response">The request message to be transformed.</param>
        /// <param name="cancellationToken">The operation cancellation token.</param>
        /// <returns>The Task representing the asynchronous operation.</returns>
        private static async Task<HttpResponseMessage> ConvertResponse(
            Windows.Web.Http.HttpResponseMessage response,
            CancellationToken cancellationToken)
        {
            // Convert the response
            var converted = new HttpResponseMessage((HttpStatusCode)(int)response.StatusCode)
            {
                ReasonPhrase = response.ReasonPhrase,
                RequestMessage = await WindowsHttpMessageHandler
                    .ConvertRequest(response.RequestMessage, cancellationToken)
                    .ConfigureAwait(false),
                Content = await WindowsHttpMessageHandler.ConvertContent(response.Content, cancellationToken)
                    .ConfigureAwait(false),
                Version = WindowsHttpMessageHandler.GetVersion(response.Version)
           };

            // Copy headers
            foreach (var header in response.Headers)
            {
                converted.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            // Return the converted message
            return converted;
        }

        /// <summary>
        /// COnverts the version constant.
        /// </summary>
        /// <param name="version">The version to convert.</param>
        /// <returns>The converted version.</returns>
        private static Version GetVersion(Windows.Web.Http.HttpVersion version)
        {
            switch (version)
            {
                case Windows.Web.Http.HttpVersion.None:
                    return WindowsHttpMessageHandler.NoVersion;

                case Windows.Web.Http.HttpVersion.Http10:
                    return WindowsHttpMessageHandler.Version10;

                case Windows.Web.Http.HttpVersion.Http11:
                    return WindowsHttpMessageHandler.Version11;

                default:
                    throw new ArgumentOutOfRangeException(nameof(version));
            }
        }

        /// <summary>
        /// Ensures that the current object is not disposed.
        /// </summary>
        private void EnsureNotDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(WindowsHttpMessageHandler));
            }
        }
    }
}
