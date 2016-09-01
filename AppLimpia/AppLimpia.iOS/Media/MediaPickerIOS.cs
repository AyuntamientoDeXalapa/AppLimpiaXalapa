﻿using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AppLimpia.Media;

using UIKit;

#region Generated Code
// To suppress the StyleCop warning
namespace AppLimpia.iOS.Media
#endregion
{
    /// <summary>
    /// iOS <see cref="MediaPicker"/> implementation.
    /// </summary>
    public sealed class MediaPickerIOS : MediaPicker
    {
        /// <summary>
        /// The type image
        /// </summary>
        internal const string TypeImage = "public.image";

        /// <summary>
        /// The type movie
        /// </summary>
        internal const string TypeMovie = "public.movie";

        /// <summary>
        /// The _picker delegate
        /// </summary>
        private UIImagePickerControllerDelegate _pickerDelegate;

        /// <summary>
        /// The _popover
        /// </summary>
        private UIPopoverController _popover;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaPickerIOS"/> class.
        /// </summary>
        public MediaPickerIOS()
        {
            IsCameraAvailable = UIImagePickerController.IsSourceTypeAvailable(UIImagePickerControllerSourceType.Camera);

            var availableCameraMedia = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.Camera)
                                        ?? new string[0];
            var availableLibraryMedia =
                UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary) ?? new string[0];

            foreach (var type in availableCameraMedia.Concat(availableLibraryMedia))
            {
                if (type == TypeImage)
                {
                    IsPhotosSupported = true;
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

        /// <summary>
        /// Select a picture from library.
        /// </summary>
        /// <param name="options">The storage options.</param>
        /// <returns>Task&lt;IMediaFile&gt;.</returns>
        /// <exception cref="NotSupportedException"></exception>
        public Task<MediaFile> SelectPhotoAsync(CameraMediaStorageOptions options)
        {
            if (!IsPhotosSupported)
            {
                throw new NotSupportedException();
            }

            return GetMediaAsync(UIImagePickerControllerSourceType.PhotoLibrary, TypeImage);
        }

        /// <summary>
        /// Takes the picture.
        /// </summary>
        /// <param name="options">The storage options.</param>
        /// <returns>Task&lt;IMediaFile&gt;.</returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
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

            // Get media data
            MediaPickerIOS.VerifyCameraOptions(options);
            return this.GetMediaAsync(UIImagePickerControllerSourceType.Camera, MediaPickerIOS.TypeImage, options);
        }

        /// <summary>
        /// Gets the media asynchronous.
        /// </summary>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="mediaType">Type of the media.</param>
        /// <param name="options">The options.</param>
        /// <returns>Task&lt;MediaFile&gt;.</returns>
        /// <exception cref="InvalidOperationException">
        /// There's no current active window
        /// or
        /// Could not find current view controller
        /// or
        /// Only one operation can be active at at time
        /// </exception>
        private Task<MediaFile> GetMediaAsync(
            UIImagePickerControllerSourceType sourceType,
            string mediaType,
            CameraMediaStorageOptions options = null)
        {
            var window = UIApplication.SharedApplication.KeyWindow;
            if (window == null)
            {
                throw new InvalidOperationException("There's no current active window");
            }

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
            while (viewController.PresentedViewController != null)
            {
                viewController = viewController.PresentedViewController;
            }

            var ndelegate = new MediaPickerDelegate(viewController, sourceType, options);
            var od = Interlocked.CompareExchange(ref _pickerDelegate, ndelegate, null);
            if (od != null)
            {
                throw new InvalidOperationException("Only one operation can be active at at time");
            }

            var picker = SetupController(ndelegate, sourceType, mediaType, options);

            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad
                && sourceType == UIImagePickerControllerSourceType.PhotoLibrary)
            {
                ndelegate.Popover = new UIPopoverController(picker)
                {
                    Delegate = new MediaPickerPopoverDelegate(ndelegate, picker)
                };
                ndelegate.DisplayPopover();
            }
            else
            {
                viewController.PresentViewController(picker, true, null);
            }

            return ndelegate.Task.ContinueWith(
                t =>
                {
                    if (_popover != null)
                    {
                        _popover.Dispose();
                        _popover = null;
                    }

                    Interlocked.Exchange(ref _pickerDelegate, null);
                    return t;
                }).Unwrap();
        }

        /// <summary>
        /// Setups the controller.
        /// </summary>
        /// <param name="mpDelegate">The mp delegate.</param>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="mediaType">Type of the media.</param>
        /// <param name="options">The options.</param>
        /// <returns>MediaPickerController.</returns>
        private static MediaPickerController SetupController(
            MediaPickerDelegate mpDelegate,
            UIImagePickerControllerSourceType sourceType,
            string mediaType,
            CameraMediaStorageOptions options = null)
        {
            var picker = new MediaPickerController(mpDelegate) { MediaTypes = new[] { mediaType }, SourceType = sourceType };

            if (sourceType == UIImagePickerControllerSourceType.Camera)
            {
                if (mediaType == TypeImage && options is CameraMediaStorageOptions)
                {
                    picker.CameraDevice = GetCameraDevice(((CameraMediaStorageOptions)options).DefaultCamera);
                    picker.CameraCaptureMode = UIImagePickerControllerCameraCaptureMode.Photo;
                }
            }

            return picker;
        }

        /// <summary>
        /// Gets the UI camera device.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <returns>UIImagePickerControllerCameraDevice.</returns>
        /// <exception cref="NotSupportedException"></exception>
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
        /// <param name="options">The options.</param>
        /// <exception cref="ArgumentException">options.Camera is not a member of CameraDevice</exception>
        private static void VerifyCameraOptions(CameraMediaStorageOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }
            if (options.Directory != null && Path.IsPathRooted(options.Directory))
            {
                throw new ArgumentException("options.Directory must be a relative path", "options");
            }


            if (!Enum.IsDefined(typeof(CameraDevice), options.DefaultCamera))
            {
                throw new ArgumentException("options.Camera is not a member of CameraDevice");
            }
        }
    }
}
