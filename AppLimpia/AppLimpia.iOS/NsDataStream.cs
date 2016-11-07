using System;
using System.IO;

using Foundation;

#region Generated Code
// To suppress the StyleCop warning
namespace AppLimpia.iOS
#endregion
{
    /// <summary>
    /// The NSData <see cref="Stream"/> wrapper.
    /// </summary>
    internal sealed unsafe class NsDataStream : UnmanagedMemoryStream
    {
        /// <summary>
        /// The NSData object.
        /// </summary>
        private readonly NSData data;

        /// <summary>
        /// Initializes a new instance of the <see cref="NsDataStream"/> class.
        /// </summary>
        /// <param name="data">The NSData object.</param>
        public NsDataStream(NSData data)
            : base((byte*)data.Bytes, (long)data.Length)
        {
            this.data = data;
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="UnmanagedMemoryStream" /> and optionally
        /// releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        ///   <c>true</c> to release both managed and unmanaged resources; 
        ///   <c>false</c> to release only unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            // Dispose the managed data
            if (disposing)
            {
                this.data.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
