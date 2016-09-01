using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
        /// <returns>A task that represents the asynchronous get operation.</returns>
        public static async Task<JsonValue> GetAsync(Uri uri)
        {
            // Prepare the GET request
            var httpClient = new HttpClient();
            using (var request = new HttpRequestMessage(HttpMethod.Get, uri))
            {
                // Send the GET request to the server
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                using (var response = await httpClient.SendAsync(request))
                {
                    // Ensure a success operation
                    response.EnsureSuccessStatusCode();

                    // Parse the returned JSON data
                    var content = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine("Response: " + content);
                    return await Task.Run(() => Json.Json.Read(content));
                }
            }
        }

        /// <summary>
        /// Asynchronously posts the data to the server with the POST method.
        /// </summary>
        /// <param name="uri">The server URI to post data.</param>
        /// <param name="content">The content to post to server.</param>
        /// <returns>A task that represents the asynchronous get operation.</returns>
        public static async Task<JsonValue> PostAsync(Uri uri, JsonValue content)
        {
            // Prepare the GET request
            var httpClient = new HttpClient();
            using (var request = new HttpRequestMessage(HttpMethod.Post, uri))
            {
                // Send the GET request to the server
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Prepare the request content
                var builder = new StringBuilder();
                Json.Json.Write(content, builder);
                Debug.WriteLine("Request: " + builder);
                request.Content = new StringContent(builder.ToString(), Encoding.UTF8, "application/json");

                // Get server response
                using (var response = await httpClient.SendAsync(request))
                {
                    // Ensure a success operation
                    response.EnsureSuccessStatusCode();

                    // Parse the returned JSON data
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine("Response: " + responseContent);
                    return await Task.Run(() => Json.Json.Read(responseContent));
                }
            }
        }

        /// <summary>
        /// Asynchronously gets the data from the server with the GET method.
        /// </summary>
        /// <param name="uri">The server URI to retrieve data.</param>
        /// <returns>A task that represents the asynchronous get operation.</returns>
        public static async Task<JsonValue> GetServerDataOld(Uri uri)
        {
            // Create the web request
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Accept = "application/json";
            request.Method = "GET";

            // Send the request to the server and wait for the response
            using (var response = await request.GetResponseAsync())
            {
                // Get a stream representation of the HTTP web response
                using (var stream = response.GetResponseStream())
                {
                    // Parse the returned JSON data
                    // ReSharper disable once AccessToDisposedClosure
                    var jsonDoc = await Task.Run(() => Json.Json.Read(stream));
                    return jsonDoc;
                }
            }
        }
    }
}
