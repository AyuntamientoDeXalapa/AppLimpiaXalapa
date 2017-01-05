using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AppLimpia.Media;

using AVFoundation;

using Foundation;

using UIKit;

#region Generated Code
// To suppress the StyleCop warning
namespace AppLimpia.iOS.Media
#endregion
{
    /// <summary>
    /// iOS <see cref="MediaPicker"/> implementation.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public sealed class MediaPickerIOS : MediaPicker
    {
        /// <summary>
        /// The type image.
        /// </summary>
        internal const string TypeImage = "public.image";

        /////// <summary>
        /////// The type movie.
        /////// </summary>
        ////internal const string TypeMovie = "public.movie";

        /// <summary>
        /// The media picker delegate.
        /// </summary>
        private UIImagePickerControllerDelegate pickerDelegate;

        /// <summary>
        /// The user interface popover controller.
        /// </summary>
        private UIPopoverController popover;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaPickerIOS"/> class.
        /// </summary>
        public MediaPickerIOS()
        {
            // Get available media types
            this.IsCameraAvailable =
                UIImagePickerController.IsSourceTypeAvailable(UIImagePickerControllerSourceType.Camera);
            var availableCameraMedia =
                UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.Camera) ?? new string[0];
            var availableLibraryMedia =
                UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary)
                ?? new string[0];

            // Get if photos are supported
            foreach (var type in availableCameraMedia.Concat(availableLibraryMedia))
            {
                if (type == MediaPickerIOS.TypeImage)
                {
                    this.IsPhotosSupported = true;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is camera available.
        /// </summary>
        /// <value><c>true</c> if this instance is camera available; otherwise, <c>false</c>.</value>
        public override bool IsCameraAvailable { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is photos supported.
        /// </summary>
        /// <value><c>true</c> if this instance is photos supported; otherwise, <c>false</c>.</value>
        public override bool IsPhotosSupported { get; }

        /////// <summary>
        /////// Select a picture from library.
        /////// </summary>
        /////// <param name="options">The storage options.</param> 
        /////// <returns>Task representing the asynchronous operation.</returns>
        ////public Task<MediaFile> SelectPhotoAsync(CameraMediaStorageOptions options)
        ////{
        ////    // If photos are not supported
        ////    if (!this.IsPhotosSupported)
        ////    {
        ////        throw new NotSupportedException();
        ////    }

        ////    // Get the image from pictures
        ////    return this.GetMediaAsync(UIImagePickerControllerSourceType.PhotoLibrary, MediaPickerIOS.TypeImage);
        ////}

        /// <summary> 
        /// Takes the picture. 
        /// </summary> 
        /// <param name="options">The storage options.</param> 
        /// <returns>Task representing the asynchronous operation.</returns>
        public override Task<MediaFile> TakePhotoAsync(CameraMediaStorageOptions options)
        {
            // If photos are not supported
            if (!this.IsPhotosSupported)
            {
                throw new NotSupportedException();
            }

            // If camera is not supported
            if (!this.IsCameraAvailable)
            {
                throw new NotSupportedException();
            }

            // If the camera permission is not determined
            var status = AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Video);
            if (status == AVAuthorizationStatus.NotDetermined)
            {
                var granted = AVCaptureDevice.RequestAccessForMediaTypeAsync(AVMediaType.Video);
                granted.Wait();
                if (!granted.Result)
                {
                    return null;
                }
            }

            // If camera is restricted
            if ((status == AVAuthorizationStatus.Denied) || (status == AVAuthorizationStatus.Restricted))
            {
                return null;
            }

            // Take the camera photo
            MediaPickerIOS.VerifyCameraOptions(options);
            return this.GetMediaAsync(UIImagePickerControllerSourceType.Camera, MediaPickerIOS.TypeImage, options);
        }

        /// <summary>
        /// Resizes the image to have the specified maximum sizes.
        /// </summary>
        /// <param name="source">The source image to resize.</param>
        /// <param name="maxWidth">The max width of the image.</param>
        /// <param name="maxHeight">The max height of the image.</param>
        /// <returns>The resized image data.</returns>
        public override Stream ResizeImage(Stream source, int maxWidth, int maxHeight)
        {
            // Load the bitmap
            using (var originalBitmap = new UIKit.UIImage(NSData.FromStream(source)))
            {
                // If the image does not need to be resized
                var originalWidth = originalBitmap.Size.Width;
                var originalHeight = originalBitmap.Size.Height;
                if ((originalWidth <= maxWidth) && (originalHeight <= maxHeight))
                {
                    return source;
                }

                // Calculate the resized image size
                var resizedHeight = (float)originalHeight;
                var resizedWidth = (float)originalWidth;

                // If height is greater than the maximum height
                if (resizedHeight > maxHeight)
                {
                    resizedHeight = maxHeight;
                    var factor = (float)originalHeight / maxHeight;
                    resizedWidth = (float)originalWidth / factor;
                }

                // If the width is greater than the maximum width
                if (resizedWidth > maxWidth)
                {
                    resizedWidth = maxWidth;
                    var factor = (float)originalWidth / maxWidth;
                    resizedHeight = (float)originalHeight / factor;
                }

                // Resize the image
                UIGraphics.BeginImageContext(new SizeF(resizedWidth, resizedHeight));
                originalBitmap.Draw(new RectangleF(0, 0, resizedWidth, resizedHeight));
                var resizedBitmap = UIGraphics.GetImageFromCurrentImageContext();
                UIGraphics.EndImageContext();

                // Save the rescaled image
                var imageData = resizedBitmap.AsJPEG(0.95f).ToArray();
                resizedBitmap.Dispose();
                return new MemoryStream(imageData);
            }
        }

        /// <summary>
        /// Setups the controller.
        /// </summary>
        /// <param name="mediaDelegate">The media picker delegate.</param>
        /// <param name="sourceType">Media source type.</param>
        /// <param name="mediaType">Media type.</param>
        /// <param name="options">The options.</param>
        /// <returns>The configured media picker controller.</returns>
        private static MediaPickerController SetupController(
            MediaPickerDelegate mediaDelegate,
            UIImagePickerControllerSourceType sourceType,
            string mediaType,
            CameraMediaStorageOptions options = null)
        {
            // Create the media picker
            var picker = new MediaPickerController(mediaDelegate)
            {
                MediaTypes = new[] { mediaType },
                SourceType = sourceType
            };

            // If the image is from camera
            if (sourceType == UIImagePickerControllerSourceType.Camera)
            {
                if ((mediaType == MediaPickerIOS.TypeImage) && (options != null))
                {
                    // Configure the camera
                    picker.CameraDevice = MediaPickerIOS.GetCameraDevice(options.DefaultCamera);
                    picker.CameraCaptureMode = UIImagePickerControllerCameraCaptureMode.Photo;
                }
            }

            return picker;
        }

        /// <summary>
        /// Gets the camera device.
        /// </summary>
        /// <param name="device">The camera device to get.</param>
        /// <returns>The requested camera.</returns>
        private static UIImagePickerControllerCameraDevice GetCameraDevice(CameraDevice device)
        {
            switch (device)
            {
                case CameraDevice.Front:
                    return UIImagePickerControllerCameraDevice.Front;
                case CameraDevice.Rear:
                    return UIImagePickerControllerCameraDevice.Rear;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Verifies the camera options.
        /// </summary>
        /// <param name="options">The camera options.</param>
        private static void VerifyCameraOptions(CameraMediaStorageOptions options)
        {
            // If options are not specified
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            // If path is rooted
            if ((options.Directory != null) && Path.IsPathRooted(options.Directory))
            {
                // ReSharper disable once LocalizableElement
                throw new ArgumentException("Directory must be a relative path", nameof(options));
            }

            // Validate the camera options
            if (!Enum.IsDefined(typeof(CameraDevice), options.DefaultCamera))
            {
                throw new ArgumentException("Camera is not a member of CameraDevice");
            }
        }

        /// <summary>
        /// Gets the media asynchronous.
        /// </summary>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="mediaType">Type of the media.</param>
        /// <param name="options">The options.</param>
        /// <returns>Task representing the asynchronous operation.</returns>
        private Task<MediaFile> GetMediaAsync(
            UIImagePickerControllerSourceType sourceType,
            string mediaType,
            CameraMediaStorageOptions options = null)
        {
            // Get the active window
            var window = UIApplication.SharedApplication.KeyWindow;
            if (window == null)
            {
                throw new InvalidOperationException("There's no current active window");
            }

            // Get the view controller
            var viewController = window.RootViewController;

#if __IOS_10__
        if (viewController == null || (viewController.PresentedViewController != null && viewController.PresentedViewController.GetType() == typeof(UIAlertController)))
        {
            window =
                UIApplication.SharedApplication.Windows.OrderByDescending(w => w.WindowLevel)
                    .FirstOrDefault(w => w.RootViewController != null);

            if (window == null)
            {
                throw new InvalidOperationException("Could not find current view controller");
            }

            viewController = window.RootViewController;
        }
#endif

            // Get the root view controller
            while (viewController.PresentedViewController != null)
            {
                viewController = viewController.PresentedViewController;
            }

            // Create a new media picker delegate
            var newDelegate = new MediaPickerDelegate(viewController, sourceType, options);
            var operationResult = Interlocked.CompareExchange(ref this.pickerDelegate, newDelegate, null);
            if (operationResult != null)
            {
                throw new InvalidOperationException("Only one operation can be active at at time");
            }

            // Setup the view controller
            var picker = MediaPickerIOS.SetupController(newDelegate, sourceType, mediaType, options);

            // If the image is from photo library
            if ((UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
                && (sourceType == UIImagePickerControllerSourceType.PhotoLibrary))
            {
                // Create a photo choosing popover
                newDelegate.Popover = new UIPopoverController(picker)
                {
                    Delegate = new MediaPickerPopoverDelegate(newDelegate, picker)
                };
                newDelegate.DisplayPopover();
            }
            else
            {
                // Show the media picker view
                viewController.PresentViewController(picker, true, null);
            }

            // Get the media
            return newDelegate.Task.ContinueWith(
                t =>
                {
                    // Dispose popover if any
                    if (this.popover != null)
                    {
                        this.popover.Dispose();
                        this.popover = null;
                    }

                    // Release the media picker delegate
                    Interlocked.Exchange(ref this.pickerDelegate, null);
                    return t;
                }).Unwrap();
        }
    }
}
