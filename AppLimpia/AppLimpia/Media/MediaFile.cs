using System;
using System.IO;

namespace AppLimpia.Media
{
    /// <summary> 
    /// Class MediaFile. This class cannot be inherited.
    /// </summary> 
    public sealed class MediaFile : IDisposable
    {
        /// <summary> 
        /// The media file path.
        /// </summary> 
        private readonly string path;

        /// <summary> 
        /// The data stream getter.
        /// </summary> 
        private readonly Func<Stream> streamGetter;

        /// <summary> 
        /// The dispose method.
        /// </summary> 
        private readonly Action<bool> dispose;

        /// <summary> 
        /// The value indicating whether the current <see cref="MediaFile"/> is disposed.
        /// </summary> 
        private bool isDisposed;

        /// <summary> 
        /// Initializes a new instance of the <see cref="MediaFile" /> class. 
        /// </summary> 
        /// <param name="path">The media file path.</param> 
        /// <param name="streamGetter">The data stream getter.</param> 
        /// <param name="dispose">The dispose method.</param> 
        public MediaFile(string path, Func<Stream> streamGetter, Action<bool> dispose = null)
        {
            this.path = path;
            this.streamGetter = streamGetter;
            this.dispose = dispose;
        }

        /// <summary> 
        /// Finalizes an instance of the <see cref="MediaFile" /> class. 
        /// </summary> 
        ~MediaFile()
        {
            this.Dispose(false);
        }

        /// <summary> 
        /// Gets the path of the current <see cref="MediaFile"/>. 
        /// </summary> 
        /// <exception cref="ObjectDisposedException">The current <see cref="MediaFile"/> is disposed.</exception> 
        public string Path
        {
            get
            {
                if (this.isDisposed)
                {
                    throw new ObjectDisposedException(nameof(MediaFile));
                }

                return this.path;
            }
        }

        /// <summary> 
        /// Gets the stream. 
        /// </summary> 
        /// <exception cref="ObjectDisposedException">The current <see cref="MediaFile"/> is disposed.</exception> 
        public Stream Source
        {
            get
            {
                if (this.isDisposed)
                {
                    throw new ObjectDisposedException(nameof(MediaFile));
                }

                return this.streamGetter();
            }
        }

        /// <summary> 
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources. 
        /// </summary> 
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary> 
        /// Releases unmanaged and - optionally - managed resources. 
        /// </summary> 
        /// <param name="disposing">
        ///   <c>true</c> to release both managed and unmanaged resources;
        ///   <c>false</c> to release only unmanaged resources.
        /// </param> 
        private void Dispose(bool disposing)
        {
            // If the object is already disposed
            if (this.isDisposed)
            {
                return;
            }

            // Dispose the current object
            this.isDisposed = true;
            this.dispose?.Invoke(disposing);
        }
    }
}
