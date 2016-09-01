using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using AppLimpia.Media;

using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

namespace AppLimpia.WinPhone
{
    /// <summary>
    /// Windows phone <see cref="MediaPicker"/> implementation.
    /// </summary>
    public sealed class MediaPickerWinPhone : MediaPicker
    {
        /// <summary>
        /// The view of the application.
        /// </summary>
        private CoreApplicationView view;

        /// <summary>
        /// The task completion source.
        /// </summary>
        private TaskCompletionSource<MediaFile> completionSource; 

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaPickerWinPhone"/> class.
        /// </summary>
        public MediaPickerWinPhone()
        {
            this.IsCameraAvailable = true;
            this.IsPhotosSupported = true;
        }

        /// <summary> 
        /// Gets a value indicating whether this instance is camera available. 
        /// </summary> 
        public override bool IsCameraAvailable { get; }

        /// <summary> 
        /// Gets a value indicating whether this instance is photos supported. 
        /// </summary> 
        public override bool IsPhotosSupported { get; }

        /// <summary> 
        /// Takes the picture. 
        /// </summary> 
        /// <param name="options">The storage options.</param> 
        /// <returns>Task representing the asynchronous operation.</returns>
        public override Task<MediaFile> TakePhotoAsync(CameraMediaStorageOptions options)
        {
            // Create a new instance of the file picker
            var filePicker = new FileOpenPicker
                                 {
                                     SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                                     ViewMode = PickerViewMode.Thumbnail
                                 };

            // Filter to include a sample subset of file types 
            filePicker.FileTypeFilter.Clear();
            filePicker.FileTypeFilter.Add(".bmp");
            filePicker.FileTypeFilter.Add(".png");
            filePicker.FileTypeFilter.Add(".jpeg");
            filePicker.FileTypeFilter.Add(".jpg");

            // If the choose photo operation is in progress
            var source = new TaskCompletionSource<MediaFile>();
            if (Interlocked.CompareExchange(ref this.completionSource, source, null) != null)
            {
                throw new InvalidOperationException("Only one operation can be active at a time");
            }

            // Choose the photo
            this.view = CoreApplication.GetCurrentView();
            filePicker.PickSingleFileAndContinue();
            this.view.Activated += this.OnViewActivated;
            return this.completionSource.Task;
        }

        /// <summary>
        /// Handles the Activated event of application view.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="IActivatedEventArgs"/> with arguments of the event.</param>
        private async void OnViewActivated(CoreApplicationView sender, IActivatedEventArgs e)
        {
            var args = e as FileOpenPickerContinuationEventArgs;
            if (args != null)
            {
                // Get the completion source
                var source = Interlocked.Exchange(ref this.completionSource, null);

                // Remove the event handler
                this.view.Activated -= this.OnViewActivated;

                // If no file is selected
                if (args.Files.Count == 0)
                {
                    source?.SetCanceled();
                    return;
                }

                // Get the opened file
                var storageFile = args.Files[0];
                Stream stream;
                using (var storageStream = await storageFile.OpenAsync(FileAccessMode.Read))
                {
                    // Convert the storage stream to IO.Stream
                    var bytes = new byte[storageStream.Size];
                    using (var reader = new DataReader(storageStream.GetInputStreamAt(0)))
                    {
                        await reader.LoadAsync((uint)bytes.Length);
                        reader.ReadBytes(bytes);
                    }

                    stream = new MemoryStream(bytes);
                }

                // Create the media file data
                var file = new MediaFile(storageFile.Path, () => stream, b => { });
                source?.SetResult(file);
            }
        }
    }
}
