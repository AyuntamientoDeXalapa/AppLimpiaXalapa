using System;

namespace AppLimpia.Media
{
    /// <summary>
    ///     Enum CameraDevice
    /// </summary>
    public enum CameraDevice
    {
        /// <summary>
        ///     The rear
        /// </summary>
        Rear,

        /// <summary>
        ///     The front
        /// </summary>
        Front
    }

    /// <summary>
    /// 
    /// </summary>
    public class CameraMediaStorageOptions
    {
        /// <summary>
        ///     Gets or sets the directory.
        /// </summary>
        /// <value>The directory.</value>
        public string Directory { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the maximum pixel dimension.
        /// </summary>
        /// <value>The maximum pixel dimension.</value>
        public int? MaxPixelDimension { get; set; }

        /// <summary>
        ///     Gets or sets the percent quality.
        /// </summary>
        /// <value>The percent quality.</value>
        public int? PercentQuality { get; set; }

        /// <summary>
        ///     Gets or sets the default camera.
        /// </summary>
        /// <value>The default camera.</value>
        public CameraDevice DefaultCamera { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [save media on capture].
        /// </summary>
        /// <value><c>true</c> if [save media on capture]; otherwise, <c>false</c>.</value>
        public bool SaveMediaOnCapture { get; set; }
    }
}
