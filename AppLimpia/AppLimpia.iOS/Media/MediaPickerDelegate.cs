using System;
using System.IO;
using System.Threading.Tasks;

using AppLimpia.Media;

using CoreGraphics;
using Foundation;
using UIKit;

#region Generated Code
// To suppress the StyleCop warning
namespace AppLimpia.iOS.Media
#endregion
{
    /// <summary>
    /// The media picker delegate that receives the captured media data.
    /// </summary>
    internal class MediaPickerDelegate : UIImagePickerControllerDelegate
    {
        /// <summary>
        /// The observer.
        /// </summary>
        private readonly NSObject observer;

        /// <summary>
        /// The camera storage options.
        /// </summary>
        private readonly CameraMediaStorageOptions options;

        /// <summary>
        /// The media capture source.
        /// </summary>
        private readonly UIImagePickerControllerSourceType source;

        /// <summary>
        /// The media capture task completion source.
        /// </summary>
        private readonly TaskCompletionSource<MediaFile> completionSource = new TaskCompletionSource<MediaFile>();

        /// <summary>
        /// The view controller.
        /// </summary>
        private readonly UIViewController viewController;

        /// <summary>
        /// The device orientation.
        /// </summary>
        private UIDeviceOrientation? orientation;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaPickerDelegate"/> class.
        /// </summary>
        /// <param name="viewController">The view controller.</param>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="options">The options.</param>
        internal MediaPickerDelegate(
            UIViewController viewController,
            UIImagePickerControllerSourceType sourceType,
            CameraMediaStorageOptions options)
        {
            // Setup the current media picker delegate
            this.viewController = viewController;
            this.source = sourceType;
            this.options = options ?? new CameraMediaStorageOptions();

            // Add the observer
            if (viewController != null)
            {
                UIDevice.CurrentDevice.BeginGeneratingDeviceOrientationNotifications();
                this.observer = NSNotificationCenter.DefaultCenter.AddObserver(
                    UIDevice.OrientationDidChangeNotification,
                    this.DidRotate);
            }
        }

        /// <summary>
        /// Gets or sets the popover.
        /// </summary>
        /// <value>The popover.</value>
        // ReSharper disable once MemberCanBePrivate.Global
        public UIPopoverController Popover { get; set; }

        /////// <summary>
        /////// Gets the associated view controller.
        /////// </summary>
        ////public UIView View => this.viewController.View;

        /// <summary>
        /// Gets the task representing the media capture operation.
        /// </summary>
        public Task<MediaFile> Task => this.completionSource.Task;

        /// <summary>
        /// Gets a value indicating whether the current instance represents the capture operation.
        /// </summary>
        private bool IsCapture => this.source == UIImagePickerControllerSourceType.Camera;

        /// <summary>
        /// Called when the media picking is finished.
        /// </summary>
        /// <param name="picker">The media picker controller.</param>
        /// <param name="info">The information about captured media.</param>
        public override void FinishedPickingMedia(UIImagePickerController picker, NSDictionary info)
        {
            // Get the captured media
            MediaFile mediaFile;
            switch ((NSString)info[UIImagePickerController.MediaType])
            {
                case MediaPickerIOS.TypeImage:
                    mediaFile = this.GetPictureMediaFile(info);
                    break;

                ////case MediaPickerIOS.TypeMovie:
                ////    mediaFile = this.GetMovieMediaFile(info);
                ////    break;

                default:
                    throw new NotSupportedException();
            }

            // Dismiss the current media picker
            this.Dismiss(picker, () => this.completionSource.TrySetResult(mediaFile));
        }

        /// <summary>
        /// Called when the media picking is canceled.
        /// </summary>
        /// <param name="picker">The media picker controller.</param>
        public override void Canceled(UIImagePickerController picker)
        {
            // Dismiss the current media picker
            this.Dismiss(picker, () => this.completionSource.TrySetCanceled());
        }

        /// <summary>
        /// Displays the popover.
        /// </summary>
        /// <param name="hideFirst"><c>true</c> to hide previous popover; <c>false</c> otherwise.</param>
        public void DisplayPopover(bool hideFirst = false)
        {
            // If no popover to show
            if (this.Popover == null)
            {
                return;
            }

            // Get the screen dimensions
            var screenWidth = UIScreen.MainScreen.Bounds.Width;
            var screenHeight = UIScreen.MainScreen.Bounds.Height;

            // Get the base width and height
            float width = 400;
            float height = 300;

            // If no orientation
            if (this.orientation == null)
            {
                // Get the device orientation
                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                if (MediaPickerDelegate.IsValidInterfaceOrientation(UIDevice.CurrentDevice.Orientation))
                {
                    this.orientation = UIDevice.CurrentDevice.Orientation;
                }
                else
                {
                    this.orientation = MediaPickerDelegate.GetDeviceOrientation(this.viewController.InterfaceOrientation);
                }
            }

            // Get the screen size based on orientation
            float x;
            float y;
            if ((this.orientation == UIDeviceOrientation.LandscapeLeft)
                || (this.orientation == UIDeviceOrientation.LandscapeRight))
            {
                y = (float)(screenWidth / 2) - (height / 2);
                x = (float)(screenHeight / 2) - (width / 2);
            }
            else
            {
                x = (float)(screenWidth / 2) - (width / 2);
                y = (float)(screenHeight / 2) - (height / 2);
            }

            // hide the previous popover
            if (hideFirst && this.Popover.PopoverVisible)
            {
                this.Popover.Dismiss(false);
            }

            // Show a new popover
            this.Popover.PresentFromRect(new CGRect(x, y, width, height), this.viewController.View, 0, true);
        }

        /// <summary>
        /// Gets the unique path to store media.
        /// </summary>
        /// <param name="type">The media type.</param>
        /// <param name="path">The path to store media.</param>
        /// <param name="name">The name of the media to store.</param>
        /// <returns>Unique path to store media.</returns>
        private static string GetUniquePath(string type, string path, string name)
        {
            // Get the media extension
            var isPhoto = type == MediaPickerIOS.TypeImage;
            var ext = Path.GetExtension(name);
            if (string.IsNullOrEmpty(ext))
            {
                ext = isPhoto ? ".jpg" : ".mp4";
            }

            // If file name exists
            name = Path.GetFileNameWithoutExtension(name);
            var fullName = name + ext;
            var i = 1;
            while (File.Exists(Path.Combine(path, fullName)))
            {
                fullName = name + "_" + (i++) + ext;
            }

            // Return the generated file name
            return Path.Combine(path, fullName);
        }

        /// <summary>
        /// Gets the output path to store media type.
        /// </summary>
        /// <param name="type">The media type.</param>
        /// <param name="path">The path to store media.</param>
        /// <param name="name">The name of the media to store.</param>
        /// <returns>Unique path to store media.</returns>
        private static string GetOutputPath(string type, string path, string name)
        {
            // Create the folder to store media
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), path);
            Directory.CreateDirectory(path);

            // Get the base media name
            if (string.IsNullOrWhiteSpace(name))
            {
                var timestamp = DateTime.Now.ToString("yyyMMdd_HHmmss");
                if (type == MediaPickerIOS.TypeImage)
                {
                    name = "IMG_" + timestamp + ".jpg";
                }
                else
                {
                    name = "VID_" + timestamp + ".mp4";
                }
            }

            // Return the media file
            return Path.Combine(path, MediaPickerDelegate.GetUniquePath(type, path, name));
        }

        /// <summary>
        /// Determines whether the interface orientation is valid.
        /// </summary>
        /// <param name="newOrientation">The new device orientation.</param>
        /// <returns><c>true</c> if the device orientation is valid; otherwise, <c>false</c>.</returns>
        private static bool IsValidInterfaceOrientation(UIDeviceOrientation newOrientation)
        {
            return (newOrientation != UIDeviceOrientation.FaceUp) && (newOrientation != UIDeviceOrientation.FaceDown)
                   && (newOrientation != UIDeviceOrientation.Unknown);
        }

        /// <summary>
        /// Determines whether two orientation are of the same kind.
        /// </summary>
        /// <param name="orientation1">The first orientation to check.</param>
        /// <param name="orientation2">The second orientation to check.</param>
        /// <returns><c>true</c> if both orientation are of the same kind; otherwise, <c>false</c>.</returns>
        private static bool IsSameOrientationKind(UIDeviceOrientation orientation1, UIDeviceOrientation orientation2)
        {
            if ((orientation1 == UIDeviceOrientation.FaceDown) || (orientation1 == UIDeviceOrientation.FaceUp))
            {
                return (orientation2 == UIDeviceOrientation.FaceDown) || (orientation2 == UIDeviceOrientation.FaceUp);
            }

            if ((orientation1 == UIDeviceOrientation.LandscapeLeft)
                || (orientation1 == UIDeviceOrientation.LandscapeRight))
            {
                return (orientation2 == UIDeviceOrientation.LandscapeLeft)
                       || (orientation2 == UIDeviceOrientation.LandscapeRight);
            }

            if ((orientation1 == UIDeviceOrientation.Portrait)
                || (orientation1 == UIDeviceOrientation.PortraitUpsideDown))
            {
                return (orientation2 == UIDeviceOrientation.Portrait)
                       || (orientation2 == UIDeviceOrientation.PortraitUpsideDown);
            }

            return false;
        }

        /// <summary>
        /// Gets the device orientation.
        /// </summary>
        /// <param name="newOrientation">The new device orientation.</param>
        /// <returns>The converted device orientation..</returns>
        private static UIDeviceOrientation GetDeviceOrientation(UIInterfaceOrientation newOrientation)
        {
            switch (newOrientation)
            {
                case UIInterfaceOrientation.LandscapeLeft:
                    return UIDeviceOrientation.LandscapeLeft;
                case UIInterfaceOrientation.LandscapeRight:
                    return UIDeviceOrientation.LandscapeRight;
                case UIInterfaceOrientation.Portrait:
                    return UIDeviceOrientation.Portrait;
                case UIInterfaceOrientation.PortraitUpsideDown:
                    return UIDeviceOrientation.PortraitUpsideDown;
                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Dismisses the current media picker.
        /// </summary>
        /// <param name="picker">The picker.</param>
        /// <param name="onDismiss">The action to perform when the current media picker is dismissed.</param>
        private void Dismiss(UIImagePickerController picker, Action onDismiss)
        {
            // If no view controller present
            if (this.viewController == null)
            {
                onDismiss();
            }
            else
            {
                // Remove the notification observer
                NSNotificationCenter.DefaultCenter.RemoveObserver(this.observer);
                UIDevice.CurrentDevice.EndGeneratingDeviceOrientationNotifications();

                // Dispose the observer
                this.observer.Dispose();

                // If popover is present
                if (this.Popover != null)
                {
                    // Dismiss the popover
                    this.Popover.Dismiss(true);
                    this.Popover.Dispose();
                    this.Popover = null;

                    // Call the provided delegate
                    onDismiss();
                }
                else
                {
                    // Dismiss the view controller
                    picker.DismissViewController(true, onDismiss);
                    picker.Dispose();
                }
            }
        }

        /// <summary>
        /// Called when the current device was rotated.
        /// </summary>
        /// <param name="notification">The device rotation notification.</param>
        private void DidRotate(NSNotification notification)
        {
            // Get the new device orientation
            var device = (UIDevice)notification.Object;
            if (!MediaPickerDelegate.IsValidInterfaceOrientation(device.Orientation) || (this.Popover == null))
            {
                return;
            }

            // If new orientation is of the same kind
            if (this.orientation.HasValue
                && MediaPickerDelegate.IsSameOrientationKind(this.orientation.Value, device.Orientation))
            {
                return;
            }

            // Get whether the view should be rotated
            if (UIDevice.CurrentDevice.CheckSystemVersion(6, 0))
            {
                if (!this.GetShouldRotate6(device.Orientation))
                {
                    return;
                }
            }
            else if (!this.GetShouldRotate(device.Orientation))
            {
                return;
            }

            // Get the current orientation
            var current = this.orientation;
            this.orientation = device.Orientation;
            if (current == null)
            {
                return;
            }

            // Display the new popover
            this.DisplayPopover(true);
        }

        /// <summary>
        /// Gets the value whether the current view should be rotated.
        /// </summary>
        /// <param name="deviceOrientation">The new orientation.</param>
        /// <returns><c>true</c> if the view should be rotated; <c>false</c> otherwise.</returns>
        private bool GetShouldRotate(UIDeviceOrientation deviceOrientation)
        {
            // Get the new orientation
            UIInterfaceOrientation newOrientation;
            switch (deviceOrientation)
            {
                case UIDeviceOrientation.LandscapeLeft:
                    newOrientation = UIInterfaceOrientation.LandscapeLeft;
                    break;

                case UIDeviceOrientation.LandscapeRight:
                    newOrientation = UIInterfaceOrientation.LandscapeRight;
                    break;

                case UIDeviceOrientation.Portrait:
                    newOrientation = UIInterfaceOrientation.Portrait;
                    break;

                case UIDeviceOrientation.PortraitUpsideDown:
                    newOrientation = UIInterfaceOrientation.PortraitUpsideDown;
                    break;

                default:
                    return false;
            }

            // Detect whether a view should be rotated
            return this.viewController.ShouldAutorotateToInterfaceOrientation(newOrientation);
        }

        /// <summary>
        /// Gets the value whether the current view should be rotated on IOS prior to version 6.
        /// </summary>
        /// <param name="deviceOrientation">The new orientation.</param>
        /// <returns><c>true</c> if the view should be rotated; <c>false</c> otherwise.</returns>
        private bool GetShouldRotate6(UIDeviceOrientation deviceOrientation)
        {
            // If auto rotate is disabled
            if (!this.viewController.ShouldAutorotate())
            {
                return false;
            }

            // Get the rotation mask
            UIInterfaceOrientationMask mask;
            switch (deviceOrientation)
            {
                case UIDeviceOrientation.LandscapeLeft:
                    mask = UIInterfaceOrientationMask.LandscapeLeft;
                    break;

                case UIDeviceOrientation.LandscapeRight:
                    mask = UIInterfaceOrientationMask.LandscapeRight;
                    break;

                case UIDeviceOrientation.Portrait:
                    mask = UIInterfaceOrientationMask.Portrait;
                    break;

                case UIDeviceOrientation.PortraitUpsideDown:
                    mask = UIInterfaceOrientationMask.PortraitUpsideDown;
                    break;

                default:
                    return false;
            }

            // Detect whether a view should be rotated
            return this.viewController.GetSupportedInterfaceOrientations().HasFlag(mask);
        }

        /// <summary>
        /// Gets the picture media file.
        /// </summary>
        /// <param name="info">The information about captured media.</param>
        /// <returns>The media file representing the captured media.</returns>
        private MediaFile GetPictureMediaFile(NSDictionary info)
        {
            // Get the image data
            var image = (UIImage)info[UIImagePickerController.EditedImage]
                        ?? (UIImage)info[UIImagePickerController.OriginalImage];

            // Get the output path
            var path = MediaPickerDelegate.GetOutputPath(
                MediaPickerIOS.TypeImage,
                this.options.Directory ?? (this.IsCapture ? string.Empty : "temp"),
                this.options.Name);

            // Write image as JPEG
            using (var fs = File.OpenWrite(path))
            {
                using (Stream s = new NsDataStream(image.AsJPEG()))
                {
                    s.CopyTo(fs);
                    fs.Flush();
                }
            }

            // Create the media file instance
            Action<bool> dispose = null;
            if (this.source != UIImagePickerControllerSourceType.Camera)
            {
                dispose = d => File.Delete(path);
            }

            // Return the captured media
            return new MediaFile(path, () => File.OpenRead(path), dispose);
        }

        /////// <summary>
        /////// Gets the movie media file.
        /////// </summary>
        /////// <param name="info">The information about captured media.</param>
        /////// <returns>The media file representing the captured media.</returns>
        ////private MediaFile GetMovieMediaFile(NSDictionary info)
        ////{
        ////    // Get the media URI and temp path
        ////    var url = (NSUrl)info[UIImagePickerController.MediaURL];
        ////    var path = MediaPickerDelegate.GetOutputPath(
        ////        MediaPickerIOS.TypeMovie,
        ////        this.options.Directory ?? (this.IsCapture ? string.Empty : "temp"),
        ////        this.options.Name ?? Path.GetFileName(url.Path));

        ////    // Copy file
        ////    File.Move(url.Path, path);
        ////    Action<bool> dispose = null;
        ////    if (this.source != UIImagePickerControllerSourceType.Camera)
        ////    {
        ////        dispose = d => File.Delete(path);
        ////    }

        ////    // Return the captured media data
        ////    return new MediaFile(path, () => File.OpenRead(path), dispose);
        ////}
    }
}
