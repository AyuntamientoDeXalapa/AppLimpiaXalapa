using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Android;
using Android.App;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Provider;

using AppLimpia.Media;

using Java.Lang;

namespace AppLimpia.Droid
{
    // Resolve ambiguous types
    using Environment = Android.OS.Environment;
    using Exception = System.Exception;
    using Uri = Android.Net.Uri;

    /// <summary>
    /// Class MediaPickerActivity.
    /// </summary>
    [Activity]
    internal class MediaPickerActivity : Activity
    {
        /// <summary>
        /// The media path bundle variable name.
        /// </summary>
        internal const string ExtraPath = "path";

        /// <summary>
        /// The media type bundle variable name.
        /// </summary>
        internal const string ExtraType = "type";

        /// <summary>
        /// The operation identifier bundle variable name.
        /// </summary>
        internal const string ExtraId = "id";

        /// <summary>
        /// The action bundle variable name.
        /// </summary>
        internal const string ExtraAction = "action";

        /// <summary>
        /// The value indicating whether the current operation was tasked bundle variable name.
        /// </summary>
        internal const string ExtraTasked = "tasked";

        /// <summary>
        /// The media file name bundle variable name.
        /// </summary>
        private const string MediaFileExtraName = "MediaFile";

        /// <summary>
        /// The requested action.
        /// </summary>
        private string action;

        /// <summary>
        /// The operation description.
        /// </summary>
        private string description;

        /// <summary>
        /// The current operation identifier.
        /// </summary>
        private int id;

        /// <summary>
        /// The value indicating whether the current media is a photo.
        /// </summary>
        private bool isPhoto;

        /// <summary>
        /// The media destination path.
        /// </summary>
        private Uri path;

        /// <summary>
        /// The media duration limit in seconds.
        /// </summary>
        private int durationLimit;

        /// <summary>
        /// The value indicating whether the current operation was tasked.
        /// </summary>
        private bool tasked;

        /// <summary>
        /// The media title.
        /// </summary>
        private string title;

        /// <summary>
        /// The media type.
        /// </summary>
        private string type;

        /// <summary>
        /// The event that is raised when the media was picked.
        /// </summary>
        internal static event EventHandler<MediaPickedEventArgs> MediaPicked;

        /// <summary>
        /// Called to retrieve per-instance state from an activity before being killed
        /// so that the state can be restored in <c><see cref="M:Android.App.Activity.OnCreate(Android.OS.Bundle)" /></c>
        /// or <c><see cref="M:Android.App.Activity.OnRestoreInstanceState(Android.OS.Bundle)" /></c> (the 
        /// <c><see cref="T:Android.OS.Bundle" /></c> populated by this method will be passed to both).
        /// </summary>
        /// <param name="outState">Bundle in which to place your saved state.</param>
        protected override void OnSaveInstanceState(Bundle outState)
        {
            // Save the state of the current activity
            outState.PutBoolean("ran", true);
            outState.PutString(MediaStore.MediaColumns.Title, this.title);
            outState.PutString(MediaStore.Images.ImageColumns.Description, this.description);
            outState.PutInt(MediaPickerActivity.ExtraId, this.id);
            outState.PutString(MediaPickerActivity.ExtraType, this.type);
            outState.PutString(MediaPickerActivity.ExtraAction, this.action);
            outState.PutInt(MediaStore.ExtraDurationLimit, this.durationLimit);
            outState.PutBoolean(MediaPickerActivity.ExtraTasked, this.tasked);

            // Store the image path if any
            if (this.path != null)
            {
                outState.PutString(MediaPickerActivity.ExtraPath, this.path.Path);
            }

            base.OnSaveInstanceState(outState);
        }

        /// <summary>
        /// Called when the activity is starting.
        /// </summary>
        /// <param name="savedInstanceState">
        ///   If the activity is being re-initialized after previously being shut down then this Bundle contains
        ///   the data it most recently supplied in <see cref="OnSaveInstanceState"/>. Otherwise it is <c>null</c>.
        /// </param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            // Call the base member
            base.OnCreate(savedInstanceState);

            // Take the state from the saved instance or an intent
            var b = savedInstanceState ?? this.Intent.Extras;
            var ran = b.GetBoolean("ran", false);
            this.title = b.GetString(MediaStore.MediaColumns.Title);
            this.description = b.GetString(MediaStore.Images.ImageColumns.Description);
            this.tasked = b.GetBoolean(MediaPickerActivity.ExtraTasked);
            this.id = b.GetInt(MediaPickerActivity.ExtraId, 0);
            this.type = b.GetString(MediaPickerActivity.ExtraType);

            // Parse media type
            if (this.type == "image/*")
            {
                this.isPhoto = true;
            }

            // Get the requested action
            this.action = b.GetString(MediaPickerActivity.ExtraAction);
            Intent pickIntent = null;
            try
            {
                // Create the media pick intent
                pickIntent = new Intent(this.action);
                if (this.action == Intent.ActionPick)
                {
                    pickIntent.SetType(this.type);
                }
                else
                {
                    // Setup the video properties for video
                    pickIntent.PutExtra(MediaStore.ExtraVideoQuality, 1);
                    if (!this.isPhoto)
                    {
                        this.durationLimit = b.GetInt(MediaStore.ExtraDurationLimit, 0);
                        if (this.durationLimit != 0)
                        {
                            pickIntent.PutExtra(MediaStore.ExtraDurationLimit, this.durationLimit);
                        }
                    }

                    // If the activity was not run
                    if (!ran)
                    {
                        this.path = MediaPickerActivity.GetOutputMediaFile(
                            this,
                            b.GetString(MediaPickerActivity.ExtraPath),
                            this.title,
                            this.isPhoto);
                        this.Touch();
                        pickIntent.PutExtra(MediaStore.ExtraOutput, this.path);
                    }
                    else
                    {
                        // Get the media location
                        this.path = Uri.Parse(b.GetString(MediaPickerActivity.ExtraPath));
                    }
                }

                // If the activity was not run before for the current operation
                if (!ran)
                {
                    // Check camera permission
                    if (global::Android.OS.Build.VERSION.Release == "6.0")
                    {
                        if (this.CheckSelfPermission(Manifest.Permission.Camera)
                            != Android.Content.PM.Permission.Granted)
                        {
                            this.RequestPermissions(new[] { Manifest.Permission.Camera }, 1);
                        }
                    }

                    // Start the media operation
                    this.StartActivityForResult(pickIntent, this.id);
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                MediaPickerActivity.RaiseOnMediaPicked(new MediaPickedEventArgs(this.id, ex));
            }
            finally
            {
                // Cleanup
                pickIntent?.Dispose();
            }
        }

        /// <summary>
        /// Called when an activity you launched exits, giving you the requestCode you started it with, the
        /// resultCode it returned, and any additional data from it.
        /// </summary>
        /// <param name="requestCode">
        ///   The integer request code originally supplied to startActivityForResult(), allowing you to identify who
        ///   this result came from.
        /// </param>
        /// <param name="resultCode">
        ///   The integer result code returned by the child activity through its setResult().
        /// </param>
        /// <param name="data">
        ///   An Intent, which can return result data to the caller (various data can be attached to Intent "extras").
        /// </param>
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            // If the operation was tasked
            if (this.tasked)
            {
                // Get the media from task result
                var future = resultCode == Result.Canceled
                                 ? MediaPickerActivity.TaskFromResult(new MediaPickedEventArgs(requestCode, true))
                                 : MediaPickerActivity.GetMediaFileAsync(
                                     this,
                                     requestCode,
                                     this.action,
                                     this.isPhoto,
                                     ref this.path,
                                     data?.Data);

                // Finish the current activity
                this.Finish();
                future.ContinueWith(t => MediaPickerActivity.RaiseOnMediaPicked(t.Result));
            }
            else
            {
                // If the task was canceled
                if (resultCode == Result.Canceled)
                {
                    this.SetResult(Result.Canceled);
                }
                else
                {
                    // Return the media resule
                    var resultData = new Intent();
                    resultData.PutExtra(MediaPickerActivity.MediaFileExtraName, data?.Data);
                    resultData.PutExtra(MediaPickerActivity.ExtraPath, this.path);
                    resultData.PutExtra("isPhoto", this.isPhoto);
                    resultData.PutExtra(MediaPickerActivity.ExtraAction, this.action);

                    this.SetResult(Result.Ok, resultData);
                }

                // Finish the current activity
                this.Finish();
            }
        }

        /// <summary>
        /// Gets the media file asynchronous.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="requestCode">The request operation code.</param>
        /// <param name="action">The action to perform.</param>
        /// <param name="isPhoto">A value indicating whenter the operation is performed on a photo.</param>
        /// <param name="path">The media path.</param>
        /// <param name="data">The media data URI.</param>
        /// <returns>A <see cref="Task"/> represening the asyncroneous operation.</returns>
        private static Task<MediaPickedEventArgs> GetMediaFileAsync(
            Context context,
            int requestCode,
            string action,
            bool isPhoto,
            ref Uri path,
            Uri data)
        {
            // If requested to take a photo
            Task<Tuple<string, bool>> pathFuture;
            Action<bool> dispose = null;
            string originalPath = null;
            if (action != Intent.ActionPick)
            {
                originalPath = path.Path;

                // Not all camera apps respect EXTRA_OUTPUT, some will instead return a content or file uri from data.
                if ((data != null) && (data.Path != originalPath))
                {
                    // Move the camera output
                    originalPath = data.ToString();
                    var currentPath = path.Path;
                    pathFuture =
                        MediaPickerActivity.TryMoveFileAsync(context, data, path, isPhoto)
                            .ContinueWith(t => new Tuple<string, bool>(t.Result ? currentPath : null, false));
                }
                else
                {
                    pathFuture = MediaPickerActivity.TaskFromResult(new Tuple<string, bool>(path.Path, false));
                }
            }
            else if (data != null)
            {
                // Pick the file from  media galery
                originalPath = data.ToString();
                path = data;
                pathFuture = MediaPickerActivity.GetFileForUriAsync(context, path, isPhoto);
            }
            else
            {
                // Nothing to perform
                pathFuture = MediaPickerActivity.TaskFromResult<Tuple<string, bool>>(null);
            }

            return pathFuture.ContinueWith(
                t =>
                    {
                        // If selected file exists
                        var resultPath = t.Result.Item1;
                        if ((resultPath != null) && File.Exists(t.Result.Item1))
                        {
                            // Return the selected media
                            if (t.Result.Item2)
                            {
                                dispose = d => File.Delete(resultPath);
                            }

                            var mf = new MediaFile(resultPath, () => File.OpenRead(t.Result.Item1), dispose);
                            return new MediaPickedEventArgs(requestCode, false, mf);
                        }

                        // Return an error
                        return new MediaPickedEventArgs(
                                   requestCode,
                                   new FileNotFoundException("Media file not found", originalPath));
                    });
        }

        /// <summary>
        /// Tries the move file asynchronous.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="url">The original file to move.</param>
        /// <param name="path">The file destination.</param>
        /// <param name="isPhoto">A value indicating whenter the operation is performed on a photo.</param>
        /// <returns>A <see cref="Task"/> represening the asyncroneous operation.</returns>
        private static Task<bool> TryMoveFileAsync(Context context, Uri url, Uri path, bool isPhoto)
        {
            var moveTo = MediaPickerActivity.GetLocalPath(path);
            return MediaPickerActivity.GetFileForUriAsync(context, url, isPhoto).ContinueWith(
                t =>
                    {
                        // If nothing to move
                        if (t.Result.Item1 == null)
                        {
                            return false;
                        }

                        // Move the file to required destination
                        File.Delete(moveTo);
                        File.Move(t.Result.Item1, moveTo);

                        // If the schema is content
                        if (url.Scheme == "content")
                        {
                            // Delete content file
                            context.ContentResolver.Delete(url, null, null);
                        }

                        return true;
                    },
                TaskScheduler.Default);
        }

        /// <summary>
        /// Gets the output media file.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="subdir">The sub-directory name to place file.</param>
        /// <param name="name">The file name.</param>
        /// <param name="isPhoto">A value indicating whenter the operation is performed on a photo.</param>
        /// <returns>The Uri of the output media file.</returns>
        private static Uri GetOutputMediaFile(Context context, string subdir, string name, bool isPhoto)
        {
            // Get the media file name
            subdir = subdir ?? string.Empty;
            if (string.IsNullOrWhiteSpace(name))
            {
                name = MediaPickerActivity.GetMediaFileWithPath(isPhoto, subdir, string.Empty, name);
            }

            var mediaType = isPhoto ? Environment.DirectoryPictures : Environment.DirectoryMovies;
            using (var mediaStorageDir = new Java.IO.File(context.GetExternalFilesDir(mediaType), subdir))
            {
                // If directory does not exists
                if (!mediaStorageDir.Exists())
                {
                    // If create directory failed
                    if (!mediaStorageDir.Mkdirs())
                    {
                        throw new IOException(
                                  "Couldn't create directory, have you added the WRITE_EXTERNAL_STORAGE permission?");
                    }

                    // Ensure this media doesn't show up in gallery apps
                    using (var nomedia = new Java.IO.File(mediaStorageDir, ".nomedia"))
                    {
                        nomedia.CreateNewFile();
                    }
                }

                // Return the media file URI
                return
                    Uri.FromFile(
                        new Java.IO.File(
                            MediaPickerActivity.GetUniqueMediaFileWithPath(
                                isPhoto,
                                mediaStorageDir.Path,
                                name,
                                File.Exists)));
            }
        }

        /// <summary>
        /// FIX for URI path for new Gallery Picker Error.
        /// </summary>
        /// <param name="uriPath">The URI to fix.</param>
        /// <returns>The fixed URI path.</returns>
        private static Uri FixUri(string uriPath)
        {
            // Remove /ACTUAL
            if (uriPath.Contains("/ACTUAL"))
            {
                uriPath = uriPath.Substring(0, uriPath.IndexOf("/ACTUAL", StringComparison.Ordinal));
            }

            // If URI is a content URI
            var pattern = Java.Util.Regex.Pattern.Compile("(content://media/.*\\d)");
            if (uriPath.Contains("content"))
            {
                // Return the mathced URI
                var matcher = pattern.Matcher(uriPath);
                if (matcher.Find())
                {
                    return Uri.Parse(matcher.Group(1));
                }

                // Invalid URI
                throw new IllegalArgumentException("Cannot handle this URI");
            }

            // Nothing to fix
            return null;
        }

        /// <summary>
        /// Creates a Task for the result.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="result">The result to create task from.</param>
        /// <returns>The task with the specified result.</returns>
        private static Task<T> TaskFromResult<T>(T result)
        {
            var source = new TaskCompletionSource<T>();
            source.SetResult(result);
            return source.Task;
        }

        /// <summary>
        /// Gets the output file with folder.
        /// </summary>
        /// <param name="isPhoto">A value indicating whenter the operation is performed on a photo.</param>
        /// <param name="folder">The root folder.</param>
        /// <param name="subdir">The subdir.</param>
        /// <param name="name">The file name.</param>
        /// <returns>The full file path.</returns>
        private static string GetMediaFileWithPath(bool isPhoto, string folder, string subdir, string name)
        {
            // If no name is provided
            if (string.IsNullOrWhiteSpace(name))
            {
                // Create a unique file name
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                if (isPhoto)
                {
                    name = "IMG_" + timestamp + ".jpg";
                }
                else
                {
                    name = "VID_" + timestamp + ".mp4";
                }
            }

            // Get the file extension
            var ext = Path.GetExtension(name);
            if (ext == string.Empty)
            {
                ext = isPhoto ? ".jpg" : ".mp4";
            }

            // Return the media file name
            name = Path.GetFileNameWithoutExtension(name);
            var newFolder = Path.Combine(folder ?? string.Empty, subdir ?? string.Empty);
            return Path.Combine(newFolder, name + ext);
        }

        /// <summary>
        /// Gets the unique file folder.
        /// </summary>
        /// <param name="isPhoto">A value indicating whenter the operation is performed on a photo.</param>
        /// <param name="folder">The root folder.</param>
        /// <param name="name">The file name.</param>
        /// <param name="checkExists">The check existance delegate.</param>
        /// <returns>The unique full file path.</returns>
        private static string GetUniqueMediaFileWithPath(
            bool isPhoto,
            string folder,
            string name,
            Func<string, bool> checkExists)
        {
            // Separate filename and extension
            var ext = Path.GetExtension(name);
            name = Path.GetFileNameWithoutExtension(name);
            if (string.IsNullOrEmpty(ext))
            {
                ext = isPhoto ? ".jpg" : "mp4";
            }

            // Generate the unique file name
            var newName = name + ext;
            var i = 1;
            while (checkExists(Path.Combine(folder, newName)))
            {
                newName = name + "_" + (i++) + ext;
            }

            return Path.Combine(folder, newName);
        }

        /// <summary>
        /// Gets the file for URI asynchronous.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <param name="uri">The file URI.</param>
        /// <param name="isPhoto">A value indicating whenter the operation is performed on a photo.</param>
        /// <returns>A <see cref="Task"/> represening the asyncroneous operation.</returns>
        // ReSharper disable once UnusedParameter.Local
        private static Task<Tuple<string, bool>> GetFileForUriAsync(Context context, Uri uri, bool isPhoto)
        {
            var tcs = new TaskCompletionSource<Tuple<string, bool>>();

            // Fix the content URI
            var fixedUri = MediaPickerActivity.FixUri(uri.Path);
            if (fixedUri != null)
            {
                uri = fixedUri;
            }

            if (uri.Scheme == "file")
            {
                // Return the media file
                tcs.SetResult(new Tuple<string, bool>(new System.Uri(uri.ToString()).LocalPath, false));
            }
            else if (uri.Scheme == "content")
            {
                // Return the content file
                Task.Factory.StartNew(
                    () =>
                        {
                            ICursor cursor = null;
                            try
                            {
                                // Get the content from resolver
                                cursor = context.ContentResolver.Query(uri, null, null, null, null);
                                if ((cursor == null) || !cursor.MoveToNext())
                                {
                                    // No data returned
                                    tcs.SetResult(new Tuple<string, bool>(null, false));
                                }
                                else
                                {
                                    // Parse the content data
                                    int column = cursor.GetColumnIndex(MediaStore.MediaColumns.Data);
                                    string contentPath = null;
                                    if (column != -1)
                                    {
                                        contentPath = cursor.GetString(column);
                                    }

                                    tcs.SetResult(new Tuple<string, bool>(contentPath, false));
                                }
                            }
                            finally
                            {
                                if (cursor != null)
                                {
                                    cursor.Close();
                                    cursor.Dispose();
                                }
                            }
                        },
                    CancellationToken.None,
                    TaskCreationOptions.None,
                    TaskScheduler.Default);
            }
            else
            {
                // Not a file not content
                tcs.SetResult(new Tuple<string, bool>(null, false));
            }

            return tcs.Task;
        }

        /// <summary>
        /// Gets the local path.
        /// </summary>
        /// <param name="uri">The file URI.</param>
        /// <returns>The locel file URI.</returns>
        private static string GetLocalPath(Uri uri)
        {
            return new System.Uri(uri.ToString()).LocalPath;
        }

        /// <summary>
        /// Raises the <see cref="MediaPicked"/> event.
        /// </summary>
        /// <param name="e">The <see cref="MediaPickedEventArgs"/> instance containing the event data.</param>
        private static void RaiseOnMediaPicked(MediaPickedEventArgs e)
        {
            MediaPickerActivity.MediaPicked?.Invoke(null, e);
        }

        /// <summary>
        /// Touches this instance.
        /// </summary>
        private void Touch()
        {
            if (this.path.Scheme != "file")
            {
                return;
            }

            File.Create(MediaPickerActivity.GetLocalPath(this.path)).Close();
        }
        
        /// <summary>
        /// The media picked event arguments.
        /// </summary>
        internal class MediaPickedEventArgs : EventArgs
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MediaPickedEventArgs"/> class.
            /// </summary>
            /// <param name="id">The operation identifier.</param>
            /// <param name="error">The error.</param>
            public MediaPickedEventArgs(int id, Exception error)
            {
                if (error == null)
                {
                    throw new ArgumentNullException(nameof(error));
                }

                this.RequestId = id;
                this.Error = error;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="MediaPickedEventArgs"/> class.
            /// </summary>
            /// <param name="id">The operation identifier.</param>
            /// <param name="isCanceled">A value indicating whether the operation was canceled.</param>
            /// <param name="media">The media file.</param>
            public MediaPickedEventArgs(int id, bool isCanceled, MediaFile media = null)
            {
                this.RequestId = id;
                this.IsCanceled = isCanceled;
                if (!this.IsCanceled && media == null)
                {
                    throw new ArgumentNullException(nameof(media));
                }

                this.Media = media;
            }

            /// <summary>
            /// Gets the request identifier.
            /// </summary>
            public int RequestId { get; }

            /// <summary>
            /// Gets a value indicating whether the operation is canceled.
            /// </summary>
            public bool IsCanceled { get; }

            /// <summary>
            /// Gets the error.
            /// </summary>
            public Exception Error { get; }

            /// <summary>
            /// Gets the media file.
            /// </summary>
            public MediaFile Media { get; }
        }
    }
}