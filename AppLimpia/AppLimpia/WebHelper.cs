using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using AppLimpia.Json;
using AppLimpia.Properties;

namespace AppLimpia
{
    /// <summary>
    /// The web requests helper class.
    /// </summary>
    internal static class WebHelper
    {
        /// <summary>
        /// The lock object provided for synchronous access.
        /// </summary>
        private static readonly object Locker = new object();

        /// <summary>
        /// The factory to use for creating HttpClient instances.
        /// </summary>
        private static Func<HttpClient> factory = () => new HttpClient();

        /// <summary>
        /// The task representing the refresh operation.
        /// </summary>
        private static TaskCompletionSource<string> refreshOperation;

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
                t => WebHelper.ParseTaskError(t, null),
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
                t => WebHelper.ParseTaskError(t, null),
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
                t => WebHelper.ParseTaskError(t, null),
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
                t => WebHelper.ParseTaskError(t, null),
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
        /// <param name="errorHandlers">Error handlers for different types of server errors.</param>
        /// <param name="timeout">An timeout for the operation.</param>
        public static void SendAsync(
            Uris.UriMethodPair uriMethod,
            HttpContent content,
            Action<JsonValue> action,
            Action failAction = null,
            Dictionary<HttpStatusCode, Action> errorHandlers = null,
            TimeSpan? timeout = null)
        {
            // Add the nonce to negate caching
            var uri = WebHelper.AppendNonce(uriMethod.Uri);

            // Get the task
            var task = WebHelper.SendAsync(uri, uriMethod.Method, content, timeout);

            // Get the task scheduler
            TaskScheduler scheduler;
            var setupContinuation = true;
            try
            {
                scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            }
            catch (InvalidOperationException)
            {
                scheduler = TaskScheduler.Current;
                setupContinuation = false;
            }

            // Setup continuation
            task.ContinueWith(
                t => action(t.Result),
                default(CancellationToken),
                TaskContinuationOptions.OnlyOnRanToCompletion,
                scheduler);

            // Setup error handling
            Task continuation;
            if (setupContinuation)
            {
                continuation = task.ContinueWith(
                    t => WebHelper.ParseTaskError(t, errorHandlers),
                    default(CancellationToken),
                    TaskContinuationOptions.OnlyOnFaulted,
                    scheduler);
            }
            else
            {
                continuation = task;
            }

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
        private static async Task<JsonValue> SendAsync(
            Uri uri,
            HttpMethod method,
            HttpContent content,
            TimeSpan? timeout)
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
                var response = await WebHelper.SendRequest(httpClient, request, socketTimeout);
                try
                {
                    // If not authorized exception and refresh token exists
                    if ((response.StatusCode == HttpStatusCode.Unauthorized)
                        && !string.IsNullOrEmpty(Settings.Instance.GetValue(Settings.RefreshToken, string.Empty)))
                    {
                        // Refresh the access token
                        var newToken = await WebHelper.RefreshToken(httpClient);

                        // Prepare the modified request
                        using (var newRequest = new HttpRequestMessage(request.Method, WebHelper.AppendNonce(request.RequestUri)))
                        {
                            newRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newToken);
                            newRequest.Content = request.Content;
                            foreach (var accepts in request.Headers.Accept)
                            {
                                newRequest.Headers.Accept.Add(accepts);
                            }

                            // Send request with new authorization
                            response.Dispose();
                            response = await WebHelper.SendRequest(httpClient, newRequest, socketTimeout);
                        }
                    }

                    // Parse the server response
                    return await WebHelper.ParseResponse(response);
                }
                finally
                {
                    response.Dispose();
                }
            }
            catch (Exception ex) when ((ex is WebException) || (ex is HttpRequestException))
            {
                // Response cannot be received
                throw new TimeoutException(Localization.ErrorNotConnected, ex);
            }
        }

        /// <summary>
        /// Send request to the server.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> to permorm the send operation.</param>
        /// <param name="request">The <see cref="HttpRequestMessage"/> to send to the server.</param>
        /// <param name="timeout">The timeout for the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private static async Task<HttpResponseMessage> SendRequest(
            HttpClient httpClient,
            HttpRequestMessage request,
            TimeSpan timeout)
        {
            return await Task.Run(
                       () =>
                           {
                               // Send request to the server
                               var cancelSource = new CancellationTokenSource();
                               var requestTask = httpClient.SendAsync(request, cancelSource.Token);

                               // If task fails an aggregate exception is thrown
                               try
                               {
                                   // If task timeout
                                   if (!requestTask.Wait(timeout))
                                   {
                                       // Cancel the task
                                       cancelSource.Cancel();
                                       throw new TimeoutException(Localization.ErrorNotConnected);
                                   }
                               }
                               catch (AggregateException ex)
                               {
                                   // Get the first exception from an aggregate
                                   throw ex.InnerExceptions.First();
                               }

                               // Return the result
                               return requestTask.GetAwaiter().GetResult();
                           }).ConfigureAwait(false);
        }

        /// <summary>
        /// Parses the response received from the server.
        /// </summary>
        /// <param name="response">The response received from the server.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private static async Task<JsonValue> ParseResponse(HttpResponseMessage response)
        {
            // If response is a error
            if (!response.IsSuccessStatusCode)
            {
                // Parse the returned error description
                var responseContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("Error ({0}): {1}", response.StatusCode, responseContent);
                Debug.WriteLine("Response: " + responseContent);
                var problem = Json.Json.Read(responseContent);

                // Read error description
                var title = problem.GetItemOrDefault("title").GetStringValueOrDefault(null);
                var detail = problem.GetItemOrDefault("detail").GetStringValueOrDefault(null);
                throw new RemoteServerException(response.StatusCode, title, detail);
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
                Debug.WriteLine("From: " + response.RequestMessage.RequestUri);
                Debug.WriteLine("Response: " + responseContent);
                return await Task.Run(() => Json.Json.Read(responseContent));
            }
        }

        /// <summary>
        /// Refreshes the expired access token.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> to permorm the send operation.</param>
        /// <returns>A task that represents the asynchronous refresh operation.</returns>
        private static Task<string> RefreshToken(HttpClient httpClient)
        {
            // Start the token ferfesh operation
            lock (WebHelper.Locker)
            {
                if (WebHelper.refreshOperation == null)
                {
                    WebHelper.refreshOperation = new TaskCompletionSource<string>();
                    var task = WebHelper.InternalRefreshToken(httpClient);

                    // Return the refreshed token
                    task.ContinueWith(
                        t =>
                        {
                            // Parse refresh operation task result
                            if (t.IsFaulted)
                            {
                                Debug.Assert(t.Exception?.InnerExceptions != null, "Task is not faulted");
                                WebHelper.refreshOperation.SetException(t.Exception?.InnerExceptions);
                            }
                            else
                            {
                                WebHelper.refreshOperation.SetResult(t.Result);
                            }

                            // Remove reference
                            WebHelper.refreshOperation = null;
                        });
                }
            }

            // Return the refresh operation
            return WebHelper.refreshOperation.Task;
        }

        /// <summary>
        /// Refreshes the expired access token.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> to perform the send operation.</param>
        /// <returns>A task that represents the asynchronous refresh operation.</returns>
        private static async Task<string> InternalRefreshToken(HttpClient httpClient)
        {
            // Prepare the refresh request
            var endpoint = Uris.GetRefreshTokenUri();
            var refreshRequest = new JsonObject
                                        {
                                                { "grant_type", "refresh_token" },
                                                { "refresh_token", Settings.Instance.GetValue(Settings.RefreshToken, string.Empty) }
                                        };
            using (var request = new HttpRequestMessage(endpoint.Method, WebHelper.AppendNonce(endpoint.Uri)))
            {
                // Setup the request
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Content = refreshRequest.AsHttpContent();

                // Send the request to the server
                var socketTimeout = httpClient.Timeout.Add(TimeSpan.FromSeconds(5));
                var response = await WebHelper.SendRequest(httpClient, request, socketTimeout);
                using (response)
                {
                    // Parse the server response
                    var responseContent = await WebHelper.ParseResponse(response);

                    // Parse the new access token data
                    var accessToken = responseContent.GetItemOrDefault("access_token").GetStringValueOrDefault(string.Empty);
                    var expiresIn = responseContent.GetItemOrDefault("expires_in").GetIntValueOrDefault(24 * 60 * 60);

                    // If all of the fields are returned
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        // Update access token
                        Settings.Instance.SetValue(Settings.AccessToken, accessToken);
                        Settings.Instance.SetValue(Settings.AccessTokenExpires, DateTime.UtcNow.AddSeconds(expiresIn));
                        return accessToken;
                    }

                    // Report error
                    throw new RemoteServerException(
                              HttpStatusCode.Unauthorized,
                              Localization.ErrorDialogTitle,
                              Localization.ErrorUnauthorized);
                }
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

                // If query already contain nonce
                var index = query.IndexOf("nonce=", StringComparison.Ordinal);
                if (index >= 0)
                {
                    query = query.Substring(0, index);
                    builder.Query = query + "nonce=" + Guid.NewGuid();
                }
                else
                {
                    builder.Query = query + "&nonce=" + Guid.NewGuid();
                }
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
        /// <param name="errorHandlers">Error handlers for different types of server errors.</param>
        private static void ParseTaskError(Task task, Dictionary<HttpStatusCode, Action> errorHandlers)
        {
            // The task should be faulted
            System.Diagnostics.Debug.Assert(task.Status == TaskStatus.Faulted, "Asynchronous task must be faulted.");

            // Report the error to the user
            foreach (var ex in task.Exception.Flatten().InnerExceptions)
            {
                try
                {
                    // If the error is a not connected error
                    if ((ex is TimeoutException) || (ex is TaskCanceledException))
                    {
                        App.DisplayAlert(
                            Localization.ErrorDialogTitle,
                            Localization.ErrorNotConnected,
                            Localization.ErrorDialogDismiss);
                        Debug.WriteLine(ex.ToString());
                    }
                    else if (ex is RemoteServerException)
                    {
                        // If handler for the current error is setup
                        var exception = (RemoteServerException)ex;
                        if (errorHandlers?.ContainsKey(exception.StatusCode) == true)
                        {
                            errorHandlers[exception.StatusCode].Invoke();
                        }
                        else
                        {
                            App.DisplayAlert(exception.Title, exception.Message, "OK");
                            Debug.WriteLine($"{exception.Title}: {exception.Message}");
                        }
                    }
                    else
                    {
                        App.DisplayAlert(
                            Localization.ErrorDialogTitle,
                            Localization.ErrorUnknownServerError,
                            Localization.ErrorDialogDismiss);
                        Debug.WriteLine(ex.ToString());
                    }
                }
                catch (Exception)
                {
                    // Ignored
                    // This is thrown when updating push token when application is not a foreground application
                }
            }
        }
    }
}
