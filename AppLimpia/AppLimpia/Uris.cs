﻿using System;

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
        /// The get favorites URI.
        /// </summary>
        public const string GetFavorites = Uris.Server + "applimpia-fake/getfavorites.php";
        
        /// <summary>
        /// The get favorites URI.
        /// </summary>
        public const string AddFavorites = Uris.Server + "applimpia-fake/addfavorites.php";

        /// <summary>
        /// The get favorites URI.
        /// </summary>
        public const string RemoveFavorites = Uris.Server + "applimpia-fake/removefavorites.php";

        /// <summary>
        /// The set primary favorite URI.
        /// </summary>
        public const string SetPrimaryFavorite = Uris.Server + "applimpia-fake/setprimary.php";

        /// <summary>
        /// The get nearest drop points URI.
        /// </summary>
        public const string GetNearest = Uris.Server + "applimpia-fake/getnearest.php";

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
        /// The submit report URI.
        /// </summary>
        public const string SubmitReport = Uris.Server + "api/reportes";

        /// <summary>
        /// The get report status URI.
        /// </summary>
        public const string GetReportStatus = Uris.Server + "applimpia-fake/getreportstatus.php";

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
        /// The set push token URI.
        /// </summary>
        public const string SetPushToken = Uris.Server + "api/sesiones/push";

        /// <summary>
        /// The OAUTH start URI.
        /// </summary>
        public const string OauthStart = Uris.Server + "applimpia-fake/oauth2_start.php";

        /// <summary>
        /// The OAUTH done URI.
        /// </summary>
        public const string OauthDone = Uris.Server + "applimpia-fake/oauth2_done.php";
    }
}
