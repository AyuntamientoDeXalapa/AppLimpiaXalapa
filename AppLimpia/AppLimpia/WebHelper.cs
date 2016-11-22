using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using AppLimpia.Json;

namespace AppLimpia
{
    /// <summary>
    /// The web requests helper class.
    /// </summary>
    internal static class WebHelper
    {
        /// <summary>
        /// The factory to use for creating HttpClient instances.
        /// </summary>
        private static Func<HttpClient> factory = () => new HttpClient();

        /// <summary>
        /// Parses the Hypertext Application Language collection received from the server.
        /// </summary>
        /// <param name="hal">The Hypertext Application Language server response.</param>
        /// <param name="collectionName">The name of the collection to retrieve.</param>
        /// <param name="nextUri">The URI of the next page or <c>null</c> if the last page.</param>
        /// <returns>The embedded collection data.</returns>
        public static JsonArray ParseHalCollection(JsonValue hal, string collectionName, out string nextUri)
        {
            // Get the data from the current response
            var array = new JsonArray();
            var collection = hal.GetItemOrDefault("_embedded").GetItemOrDefault(collectionName) as JsonArray;
            if (collection != null)
            {
                foreach (var item in collection)
                {
                    array.Add(item);
                }
            }
            else
            {
                throw new FormatException("No embedded collection data");
            }

            // Get the next page URI
            nextUri =
                hal.GetItemOrDefault("_links")
                    .GetItemOrDefault("next")
                    .GetItemOrDefault("href")
                    .GetStringValueOrDefault(null);
            return array;
        }

        /// <summary>
        /// Asynchronously gets the data from the server with the GET method.
        /// </summary>
        /// <param name="uri">The server URI to retrieve data.</param>
        /// <param name="action">An action to be performed on successful remote operation.</param>
        /// <param name="failAction">An action to be executed on failed request.</param>
        public static void GetAsync(Uri uri, Action<JsonValue> action, Action failAction = null)
        {
            // Add the nonce to negate caching
            var builder = new UriBuilder(uri);
            if (string.IsNullOrEmpty(builder.Query))
            {
                builder.Query = "nonce=" + Guid.NewGuid();
            }
            else
            {
                var query = builder.Query;
                if (query[0] == '?')
                {
                    query = query.Substring(1);
                }

                builder.Query = query + "&nonce=" + Guid.NewGuid();
            }

            var newUri = builder.Uri;
            Debug.WriteLine("Original URI: {0}", uri);
            Debug.WriteLine("Modified URI: {0}", newUri);

            // Get the task
            var task = WebHelper.GetAsync(newUri);

            // Setup continuation
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            task.ContinueWith(
                t => action(t.Result),
                default(CancellationToken),
                TaskContinuationOptions.OnlyOnRanToCompletion,
                scheduler);

            // Setup error handling
            var continuation = task.ContinueWith(
                WebHelper.ParseTaskError,
                default(CancellationToken),
                TaskContinuationOptions.OnlyOnFaulted,
                scheduler);

            // Setup error continuation
            if (failAction != null)
            {
                continuation.ContinueWith(t => failAction());
            }
        }

        /// <summary>
        /// Asynchronously sends the data from the server with the POST method.
        /// </summary>
        /// <param name="uri">The server URI to send data to.</param>
        /// <param name="content">The content to post to server.</param>
        /// <param name="action">An action to be performed on successful remote operation.</param>
        /// <param name="failAction">An action to be executed on failed request.</param>
        public static void PostAsync(Uri uri, HttpContent content, Action<JsonValue> action, Action failAction = null)
        {
            // Add the nonce to negate caching
            var builder = new UriBuilder(uri);
            if (string.IsNullOrEmpty(builder.Query))
            {
                builder.Query = "nonce=" + Guid.NewGuid();
            }
            else
            {
                var query = builder.Query;
                if (query[0] == '?')
                {
                    query = query.Substring(1);
                }

                builder.Query = query + "&nonce=" + Guid.NewGuid();
            }

            var newUri = builder.Uri;
            Debug.WriteLine("Original URI: {0}", uri);
            Debug.WriteLine("Modified URI: {0}", newUri);

            // Get the task
            var task = WebHelper.PostAsync(newUri, content);

            // Setup continuation
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            task.ContinueWith(
                t => action(t.Result),
                default(CancellationToken),
                TaskContinuationOptions.OnlyOnRanToCompletion,
                scheduler);

            // Setup error handling
            var continuation = task.ContinueWith(
                WebHelper.ParseTaskError,
                default(CancellationToken),
                TaskContinuationOptions.OnlyOnFaulted,
                scheduler);

            // Setup error continuation
            if (failAction != null)
            {
                continuation.ContinueWith(t => failAction());
            }
        }

        /// <summary>
        /// Asynchronously sends the data from the server with the PUT method.
        /// </summary>
        /// <param name="uri">The server URI to put data.</param>
        /// <param name="content">The content to put to server.</param>
        /// <param name="action">An action to be performed on successful remote operation.</param>
        /// <param name="failAction">An action to be executed on failed request.</param>
        public static void PutAsync(Uri uri, HttpContent content, Action<JsonValue> action, Action failAction = null)
        {
            // Add the nonce to negate caching
            var builder = new UriBuilder(uri);
            if (string.IsNullOrEmpty(builder.Query))
            {
                builder.Query = "nonce=" + Guid.NewGuid();
            }
            else
            {
                var query = builder.Query;
                if (query[0] == '?')
                {
                    query = query.Substring(1);
                }

                builder.Query = query + "&nonce=" + Guid.NewGuid();
            }

            var newUri = builder.Uri;
            Debug.WriteLine("Original URI: {0}", uri);
            Debug.WriteLine("Modified URI: {0}", newUri);

            // Get the task
            var task = WebHelper.PutAsync(newUri, content);

            // Setup continuation
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            task.ContinueWith(
                t => action(t.Result),
                default(CancellationToken),
                TaskContinuationOptions.OnlyOnRanToCompletion,
                scheduler);

            // Setup error handling
            var continuation = task.ContinueWith(
                WebHelper.ParseTaskError,
                default(CancellationToken),
                TaskContinuationOptions.OnlyOnFaulted,
                scheduler);

            // Setup error continuation
            if (failAction != null)
            {
                continuation.ContinueWith(t => failAction());
            }
        }

        /// <summary>
        /// Asynchronously sends the data from the server with the PATCH method.
        /// </summary>
        /// <param name="uri">The server URI to put data.</param>
        /// <param name="content">The content to send to server.</param>
        /// <param name="action">An action to be performed on successful remote operation.</param>
        /// <param name="failAction">An action to be executed on failed request.</param>
        public static void PatchAsync(Uri uri, HttpContent content, Action<JsonValue> action, Action failAction = null)
        {
            // Add the nonce to negate caching
            var builder = new UriBuilder(uri);
            if (string.IsNullOrEmpty(builder.Query))
            {
                builder.Query = "nonce=" + Guid.NewGuid();
            }
            else
            {
                var query = builder.Query;
                if (query[0] == '?')
                {
                    query = query.Substring(1);
                }

                builder.Query = query + "&nonce=" + Guid.NewGuid();
            }

            var newUri = builder.Uri;
            Debug.WriteLine("Original URI: {0}", uri);
            Debug.WriteLine("Modified URI: {0}", newUri);

            // Get the task
            var task = WebHelper.PatchAsync(newUri, content);

            // Get the task scheduler
            TaskScheduler scheduler;
            try
            {
                scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            }
            catch (InvalidOperationException)
            {
                scheduler = TaskScheduler.Current;
            }

            // Setup continuation
            task.ContinueWith(
                t => action(t.Result),
                default(CancellationToken),
                TaskContinuationOptions.OnlyOnRanToCompletion,
                scheduler);

            // Setup error handling
            var continuation = task.ContinueWith(
                WebHelper.ParseTaskError,
                default(CancellationToken),
                TaskContinuationOptions.OnlyOnFaulted,
                scheduler);

            // Setup error continuation
            if (failAction != null)
            {
                continuation.ContinueWith(t => failAction());
            }
        }

        /// <summary>
        /// Asynchronously gets the data from the server with the GET method.
        /// </summary>
        /// <param name="uri">The server URI to retrieve data.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task<JsonValue> GetAsync(Uri uri)
        {
            // Prepare the GET request
            using (var request = new HttpRequestMessage(HttpMethod.Get, uri))
            {
                // Send the GET request to the server
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (Settings.Instance.Contains(Settings.AccessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue(
                                                        "Bearer",
                                                        Settings.Instance.GetValue(Settings.AccessToken, string.Empty));
                }

                return await WebHelper.SendAsync(request, null);
            }
        }

        /// <summary>
        /// Asynchronously posts the data to the server with the POST method.
        /// </summary>
        /// <param name="uri">The server URI to post data.</param>
        /// <param name="content">The content to post to server.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task<JsonValue> PostAsync(Uri uri, HttpContent content)
        {
            // Prepare the POST request
            using (var request = new HttpRequestMessage(HttpMethod.Post, uri))
            {
                // Send the POST request to the server
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (Settings.Instance.Contains(Settings.AccessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue(
                                                        "Bearer",
                                                        Settings.Instance.GetValue(Settings.AccessToken, string.Empty));
                }

                // Prepare the request content
                request.Content = content;

                // Get server response
                return await WebHelper.SendAsync(request, null);
            }
        }

        /// <summary>
        /// Asynchronously sends the data to the server with the PUT method.
        /// </summary>
        /// <param name="uri">The server URI to put data.</param>
        /// <param name="content">The content to put to server.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task<JsonValue> PutAsync(Uri uri, HttpContent content)
        {
            // Prepare the PUT request
            using (var request = new HttpRequestMessage(HttpMethod.Put, uri))
            {
                // Send the PUT request to the server
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (Settings.Instance.Contains(Settings.AccessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue(
                                                        "Bearer",
                                                        Settings.Instance.GetValue(Settings.AccessToken, string.Empty));
                }

                // Prepare the request content
                request.Content = content;

                // Get server response
                return await WebHelper.SendAsync(request, null);
            }
        }

        /// <summary>
        /// Asynchronously sends the data to the server with the PUT method.
        /// </summary>
        /// <param name="uri">The server URI to put data.</param>
        /// <param name="content">The content to put to server.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task<JsonValue> PatchAsync(Uri uri, HttpContent content)
        {
            // Prepare the PUT request
            using (var request = new HttpRequestMessage(new HttpMethod("PATCH"), uri))
            {
                // Send the PUT request to the server
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (Settings.Instance.Contains(Settings.AccessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue(
                                                        "Bearer",
                                                        Settings.Instance.GetValue(Settings.AccessToken, string.Empty));
                }

                // Prepare the request content
                request.Content = content;

                // Get server response
                return await WebHelper.SendAsync(request, null);
            }
        }

        /// <summary>
        /// Asynchronously posts the data to the server with the POST method.
        /// </summary>
        /// <param name="uri">The server URI to post data.</param>
        /// <param name="content">The content to post to server.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task<JsonValue> PostAsync(Uri uri, JsonValue content)
        {
            // Prepare the POST request
            using (var request = new HttpRequestMessage(HttpMethod.Post, uri))
            {
                // Send the POST request to the server
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (Settings.Instance.Contains(Settings.AccessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue(
                                                        "Bearer",
                                                        Settings.Instance.GetValue(Settings.AccessToken, string.Empty));
                }

                // Prepare the request content
                var builder = new StringBuilder();
                Json.Json.Write(content, builder);
                Debug.WriteLine("Request: " + builder);
                request.Content = new StringContent(builder.ToString(), Encoding.UTF8, "application/json");

                // Get server response
                return await WebHelper.SendAsync(request, null);
            }
        }

        /// <summary>
        /// Asynchronously sends the data to the server and receives response with the specified method.
        /// </summary>
        /// <param name="uriMethod">The server URI-Method pair to use.</param>
        /// <param name="content">The content to send to the server.</param>
        /// <param name="action">An action to be performed on successful remote operation.</param>
        /// <param name="failAction">An action to be executed on failed request.</param>
        /// <param name="timeout">An timeout for the operation.</param>
        public static void SendAsync(
            Uris.UriMethodPair uriMethod,
            HttpContent content,
            Action<JsonValue> action,
            Action failAction = null,
            TimeSpan? timeout = null)
        {
            // Add the nonce to negate caching
            var uri = WebHelper.AppendNonce(uriMethod.Uri);

            // Get the task
            var task = WebHelper.SendAsync(uri, uriMethod.Method, content, null);

            // Setup continuation
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            task.ContinueWith(
                t => action(t.Result),
                default(CancellationToken),
                TaskContinuationOptions.OnlyOnRanToCompletion,
                scheduler);

            // Setup error handling
            var continuation = task.ContinueWith(
                WebHelper.ParseTaskError,
                default(CancellationToken),
                TaskContinuationOptions.OnlyOnFaulted,
                scheduler);

            // Setup error continuation
            if (failAction != null)
            {
                continuation.ContinueWith(t => failAction());
            }
        }

        /// <summary>
        /// Sets the new factory to use to create HttpClient instances.
        /// </summary>
        /// <param name="newFactory">The factory method to use.</param>
        public static void SetFactory(Func<HttpClient> newFactory)
        {
            // If factory is null
            if (newFactory == null)
            {
                throw new ArgumentNullException(nameof(newFactory));
            }

            // Setup the new factory
            WebHelper.factory = newFactory;
        }

        /// <summary>
        /// Asynchronously sends the data to the server and receives response with the specified method.
        /// </summary>
        /// <param name="uri">The server URI to use.</param>
        /// <param name="method">The HTTP Method to use.</param>
        /// <param name="content">The content to send to the server.</param>
        /// <param name="timeout">An timeout for the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private static async Task<JsonValue> SendAsync(Uri uri, HttpMethod method, HttpContent content, TimeSpan? timeout)
        {
            // Prepare the request
            using (var request = new HttpRequestMessage(method, uri))
            {
                // Set the request headers
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (Settings.Instance.Contains(Settings.AccessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue(
                                                        "Bearer",
                                                        Settings.Instance.GetValue(Settings.AccessToken, string.Empty));
                }

                // Set the request content
                if (content != null)
                {
                    Debug.WriteLine("Request: " + await content.ReadAsStringAsync());
                    request.Content = content;
                }

                // Get server response
                return await WebHelper.SendAsync(request, timeout);
            }
        }

        /// <summary>
        /// Asynchronously sends the request to the server.
        /// </summary>
        /// <param name="request">The request to be send to the server.</param>
        /// <param name="timeout">An timeout for the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private static async Task<JsonValue> SendAsync(HttpRequestMessage request, TimeSpan? timeout)
        {
            // Get the timeout
            var timeoutValue = timeout ?? TimeSpan.FromSeconds(30);

            // Get server response
            var httpClient = WebHelper.factory();
            httpClient.Timeout = timeoutValue;
            try
            {
                // Send the request to the server
                var socketTimeout = timeoutValue.Add(TimeSpan.FromSeconds(5));
                var response = await Task.Run(
                                   () =>
                                       {
                                           var cancelSource = new CancellationTokenSource();
                                           var requestTask = httpClient.SendAsync(request, cancelSource.Token);

                                           // If task timeout
                                           if (!requestTask.Wait(socketTimeout))
                                           {
                                               // Cancel the task
                                               // TODO: Localize
                                               cancelSource.Cancel();
                                               throw new TimeoutException("No connectivity");
                                           }

                                           // Return the result
                                           return requestTask.GetAwaiter().GetResult();
                                       }).ConfigureAwait(false);
                using (response)
                {
                    // If response is a error
                    if (!response.IsSuccessStatusCode)
                    {
                        // Parse the returned error description
                        var responseContent = await response.Content.ReadAsStringAsync();
                        Debug.WriteLine("Error: " + responseContent);
                        var problem = Json.Json.Read(responseContent);

                        // Read error description
                        var title = problem.GetItemOrDefault("title").GetStringValueOrDefault(null);
                        var detail = problem.GetItemOrDefault("detail").GetStringValueOrDefault(null);
                        throw new RemoteServerException(title, detail);
                    }

                    // Parse success error response
                    {
                        // If response body does not have content
                        if (response.StatusCode == HttpStatusCode.NoContent)
                        {
                            return null;
                        }

                        // Parse the returned JSON data
                        var responseContent = await response.Content.ReadAsStringAsync();
                        Debug.WriteLine("Response: " + responseContent);
                        return await Task.Run(() => Json.Json.Read(responseContent));
                    }
                }
            }
            catch (Exception ex) when ((ex is WebException) || (ex is HttpRequestException))
            {
                // Response cannot be received
                // TODO: Localize
                throw new TimeoutException("No connectivity", ex);
            }
        }

        /// <summary>
        /// Appends nonce to the provided URI.
        /// </summary>
        /// <param name="originalUri">The URI to append nonce.</param>
        /// <returns>URI with appended nonce.</returns>
        private static Uri AppendNonce(Uri originalUri)
        {
            // Get the URI query
            var builder = new UriBuilder(originalUri);
            if (string.IsNullOrEmpty(builder.Query))
            {
                builder.Query = "nonce=" + Guid.NewGuid();
            }
            else
            {
                // Append nonce to the existing query string
                var query = builder.Query;
                if (query[0] == '?')
                {
                    query = query.Substring(1);
                }

                builder.Query = query + "&nonce=" + Guid.NewGuid();
            }

            // Return the URI with nonce
            var newUri = builder.Uri;
            Debug.WriteLine("Original URI: {0}", originalUri);
            Debug.WriteLine("Modified URI: {0}", newUri);
            return newUri;
        }

        /// <summary>
        /// Parses the task that have failed to execute.
        /// </summary>
        /// <param name="task">A task that represents the failed asynchronous operation.</param>
        private static void ParseTaskError(Task task)
        {
            // The task should be faulted
            System.Diagnostics.Debug.Assert(task.Status == TaskStatus.Faulted, "Asynchronous task must be faulted.");

            // Report the error to the user
            foreach (var ex in task.Exception.Flatten().InnerExceptions)
            {
                // If the error is an not connected error
                if ((ex is TimeoutException) || (ex is TaskCanceledException))
                {
                    // TODO: Localize
                    App.DisplayAlert(
                        "Error",
                        "No se pudo conectar con el servidor. Por favor verifica que esta conectado al Internet o intenta más tarde.",
                        "OK");
                    Debug.WriteLine(ex.ToString());
                }
                else if (ex is RemoteServerException)
                {
                    var exception = (RemoteServerException)ex;
                    App.DisplayAlert(exception.Title, exception.Message, "OK");
                }
                else
                {
                    // TODO: Localize
                    App.DisplayAlert("Error", "El servidor marco el error pero no ha regresado ninguna detalle.", "OK");
                    Debug.WriteLine(ex.ToString());
                }
            }
        }
    }
}
