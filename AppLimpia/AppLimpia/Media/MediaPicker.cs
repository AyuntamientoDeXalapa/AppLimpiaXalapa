using System;
using System.Threading.Tasks;

namespace AppLimpia.Media
{
    /// <summary>
    /// The MediaPicker cross platform implementation.
    /// </summary>
    public abstract class MediaPicker
    {
        /// <summary>
        /// Gets or sets the instance of the current <see cref="MediaPicker"/> implementation.
        /// </summary>
        public static MediaPicker Instance { get; set; }

        /// <summary> 
        /// Gets a value indicating whether this instance is camera available. 
        /// </summary> 
        public abstract bool IsCameraAvailable { get; }

        /// <summary> 
        /// Gets a value indicating whether this instance is photos supported. 
        /// </summary> 
        public abstract bool IsPhotosSupported { get; }

        /// <summary> 
        /// Takes the picture. 
        /// </summary> 
        /// <param name="options">The storage options.</param> 
        /// <returns>Task representing the asynchronous operation.</returns>
        public abstract Task<MediaFile> TakePhotoAsync(CameraMediaStorageOptions options);
    }
}
