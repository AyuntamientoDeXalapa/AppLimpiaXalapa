using System;
using System.Threading.Tasks;

using AppLimpia.Media;

using Foundation;

using UIKit;

#region Generated Code
// To suppress the StyleCop warning
namespace AppLimpia.iOS.Media
#endregion
{
    /// <summary>
    /// Class media picker controller.
    /// </summary>
    public sealed class MediaPickerController : UIImagePickerController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaPickerController"/> class.
        /// </summary>
        /// <param name="mediaDelegate">The media picker delegate.</param>
        internal MediaPickerController(MediaPickerDelegate mediaDelegate)
        {
            base.Delegate = mediaDelegate;
        }

        /// <summary>
        /// Gets or sets the media picker delegate.
        /// </summary>
        public override NSObject Delegate
        {
            get
            {
                return base.Delegate;
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets the result asynchronous.
        /// </summary>
        /// <returns>Task representing the asynchronous operation.</returns>
        // ReSharper disable once UnusedMember.Global
        public Task<MediaFile> GetResultAsync()
        {
            return ((MediaPickerDelegate)this.Delegate).Task;
        }
    }
}
