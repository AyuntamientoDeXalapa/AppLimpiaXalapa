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
        /// Asynchronously gets the data from the server with the GET method.
        /// </summary>
        /// <param name="uri">The server URI to retrieve data.</param>
        /// <param name="action">An action to be performed on successful remote operation.</param>
        public static void GetAsync(Uri uri, Action<JsonValue> action)
        {
            // Get the task
            var task = WebHelper.GetAsync(uri);

            // Setup continuation
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            task.ContinueWith(
                t => action(t.Result),
                default(CancellationToken),
                TaskContinuationOptions.OnlyOnRanToCompletion,
                scheduler);

            // Setup error handling
            task.ContinueWith(
                WebHelper.ParseTaskError,
                default(CancellationToken),
                TaskContinuationOptions.OnlyOnFaulted,
                scheduler);
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
                return await WebHelper.SendAsync(request);
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

                // Prepare the request content
                var builder = new StringBuilder();
                Json.Json.Write(content, builder);
                Debug.WriteLine("Request: " + builder);
                request.Content = new StringContent(builder.ToString(), Encoding.UTF8, "application/json");

                // Get server response
                return await WebHelper.SendAsync(request);
            }
        }

        /// <summary>
        /// Asynchronously sends the request to the server.
        /// </summary>
        /// <param name="request">The request to be send to the server.</param>
        /// <returns>>A task that represents the asynchronous operation.</returns>
        private static async Task<JsonValue> SendAsync(HttpRequestMessage request)
        {
            // Get server response
            // ReSharper disable once UseObjectOrCollectionInitializer
            var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(30);
            try
            {
                using (var response = await httpClient.SendAsync(request))
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
            catch (HttpRequestException ex)
            {
                // Operation has timed out
                // TODO: Localize
                throw new TimeoutException("No connectivity", ex);
            }
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
                if (ex is TimeoutException)
                {
                    // TODO: Localize
                    App.DisplayAlert(
                        "Error",
                        "No se pudo conectar con el servidor. Por favor verifica que esta conectado al Internet o intenta más tarde.",
                        "OK");
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
