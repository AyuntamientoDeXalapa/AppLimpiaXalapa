﻿using System;
using System.Net.Http;

namespace AppLimpia
{
    /// <summary>
    /// The URI for the server requests.
    /// </summary>
    internal static class Uris
    {
        /// <summary>
        /// The base server URI.
        /// </summary>
        public const string Server = "http://limpia.xalapa.gob.mx/";

        /// <summary>
        /// The locate vehicle for drop points URI.
        /// </summary>
        public const string LocateVehicle = Uris.Server + "applimpia-fake/locatevehicle.php";

        /// <summary>
        /// The get vehicle position URI.
        /// </summary>
        public const string GetVehiclePosition = Uris.Server + "applimpia-fake/lastknownposition.php";

        /// <summary>
        /// The get notifications URI.
        /// </summary>
        public const string GetNotifications = Uris.Server + "api/notificaciones";

        /// <summary>
        /// The get incident types URI.
        /// </summary>
        public const string GetIncidentTypes = Uris.Server + "api/catalogos/incidencias";

        /// <summary>
        /// The get reports URI.
        /// </summary>
        public const string GetReports = Uris.Server + "api/reportes";

        /// <summary>
        /// The user login URI.
        /// </summary>
        public const string Login = Uris.Server + "api/sesiones";

        /// <summary>
        /// The user registration URI.
        /// </summary>
        public const string Register = Uris.Server + "api/usuarios";

        /// <summary>
        /// The recover password URI.
        /// </summary>
        public const string RecoverPassword = Uris.Server + "api/usuarios/recover";

        /// <summary>
        /// The change password URI.
        /// </summary>
        public const string ChangePassword = Uris.Server + "api/usuarios/change";

        /// <summary>
        /// Gets the login URI.
        /// </summary>
        /// <returns>The login URI.</returns>
        public static UriMethodPair GetLoginUri()
        {
            return new UriMethodPair(new Uri($"{Uris.Server}api/sesiones"), HttpMethod.Post);
        }

        /// <summary>
        /// Gets the authorization URI for the specified OAUTH provider.
        /// </summary>
        /// <param name="provider">The OAUTH provider.</param>
        /// <returns>The authorization URI for the specified OAUTH provider.</returns>
        public static UriMethodPair GetAuthorizationUri(string provider)
        {
            return new UriMethodPair(new Uri($"{Uris.Server}api/sesiones/{provider}"), HttpMethod.Get);
        }

        /// <summary>
        /// Gets the change push token URI.
        /// </summary>
        /// <returns>The change push token URI.</returns>
        public static UriMethodPair GetChangePushTokenUri()
        {
            return new UriMethodPair(new Uri($"{Uris.Server}api/sesiones/this"), new HttpMethod("PATCH"));
        }

        /// <summary>
        /// Gets the logout URI.
        /// </summary>
        /// <returns>The logout URI.</returns>
        public static UriMethodPair GetLogoutUri()
        {
            return new UriMethodPair(new Uri($"{Uris.Server}api/sesiones"), HttpMethod.Delete);
        }

        /// <summary>
        /// Gets the submit report URI.
        /// </summary>
        /// <returns>The submit report URI.</returns>
        public static UriMethodPair GetSubmitReportUri()
        {
            return new UriMethodPair(new Uri($"{Uris.Server}api/reportes"), HttpMethod.Post);
        }

        /// <summary>
        /// Gets the refresh access token URI.
        /// </summary>
        /// <returns>The refresh access token URI.</returns>
        public static UriMethodPair GetRefreshTokenUri()
        {
            return new UriMethodPair(new Uri($"{Uris.Server}api/tokens"), HttpMethod.Post);
        }

        /// <summary>
        /// Gets the get user information URI.
        /// </summary>
        /// <returns>The update user information URI.</returns>
        public static UriMethodPair GetGetUserInfoUri()
        {
            return new UriMethodPair(new Uri($"{Uris.Server}api/usuarios/this"), HttpMethod.Get);
        }

        /// <summary>
        /// Gets the update user information URI.
        /// </summary>
        /// <returns>The update user information URI.</returns>
        public static UriMethodPair GetUpdateUserInfoUri()
        {
            return new UriMethodPair(new Uri($"{Uris.Server}api/usuarios/this"), new HttpMethod("PATCH"));
        }

        /// <summary>
        /// Gets the get reports URI.
        /// </summary>
        /// <returns>The get reports URI.</returns>
        public static UriMethodPair GetGetReportsUri()
        {
            return new UriMethodPair(new Uri($"{Uris.Server}api/reportes"), HttpMethod.Get);
        }

        /// <summary>
        /// Gets the find nearest drop points URI.
        /// </summary>
        /// <param name="longitude">The longitude to search drop points.</param>
        /// <param name="latitude">The latitude to search drop points.</param>
        /// <param name="distance">The distance in km of the serach radius.</param>
        /// <returns>The find nearest drop points URI.</returns>
        public static UriMethodPair GetFindNearestDropPointsUri(double longitude, double latitude, double distance)
        {
            return
                new UriMethodPair(
                    new Uri(
                        $"{Uris.Server}api/montoneras?longitude={longitude}&latitude={latitude}&distance={distance}"),
                    HttpMethod.Get);
        }

        /// <summary>
        /// Gets the get favorites URI.
        /// </summary>
        /// <returns>The get favorites URI.</returns>
        public static UriMethodPair GetGetFavoritesUri()
        {
            return new UriMethodPair(new Uri($"{Uris.Server}api/favoritos"), HttpMethod.Get);
        }

        /// <summary>
        /// Gets the add to favorite URI.
        /// </summary>
        /// <returns>The add to favorite URI.</returns>
        public static UriMethodPair GetAddToFavoritesUri()
        {
            return new UriMethodPair(new Uri($"{Uris.Server}api/favoritos"), HttpMethod.Post);
        }

        /// <summary>
        /// Gets the remove from favorites URI.
        /// </summary>
        /// <returns>The remove from favorites URI.</returns>
        public static UriMethodPair GetRemoveFromFavoritesUri()
        {
            return new UriMethodPair(new Uri($"{Uris.Server}api/favoritos"), HttpMethod.Delete);
        }

        /// <summary>
        /// Gets the locate vehicle URI.
        /// </summary>
        /// <param name="dropPoint">The drop point to locate vehicle for.</param>
        /// <returns>The locate vehicle URI.</returns>
        public static UriMethodPair GetLocateVehicleUri(string dropPoint)
        {
            return new UriMethodPair(new Uri($"{Uris.Server}api/montoneras/{dropPoint}?expand=true"), HttpMethod.Get);
        }

        /// <summary>
        /// Gets the get incident types URI.
        /// </summary>
        /// <returns>Teh get incident types URI.</returns>
        public static UriMethodPair GetGetIncidentTypesUri()
        {
            return new UriMethodPair(new Uri($"{Uris.Server}api/catalogos/incidencias"), HttpMethod.Get);
        }

        /// <summary>
        /// Represents the URI and method pair.
        /// </summary>
        internal sealed class UriMethodPair
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="UriMethodPair"/> class.
            /// </summary>
            /// <param name="uri">The URI for the request.</param>
            /// <param name="method">The method for the request.</param>
            public UriMethodPair(Uri uri, HttpMethod method)
            {
                this.Uri = uri;
                this.Method = method;
            }

            /// <summary>
            /// Gets the URI of the current URI-Method pair.
            /// </summary>
            public Uri Uri { get; private set; }

            /// <summary>
            /// Gets the HTTP method of the current URI-Method pair.
            /// </summary>
            public HttpMethod Method { get; private set; }
        }
    }
}
