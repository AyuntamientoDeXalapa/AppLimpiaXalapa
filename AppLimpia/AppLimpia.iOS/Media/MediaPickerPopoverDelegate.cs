using System;

using UIKit;

#region Generated Code
// To suppress the StyleCop warning
namespace AppLimpia.iOS.Media
#endregion
{
    /// <summary>
    /// The media picker popover delegate.
    /// </summary>
    internal class MediaPickerPopoverDelegate : UIPopoverControllerDelegate
    {
        /// <summary>
        /// The media picker delegate.
        /// </summary>
        private readonly MediaPickerDelegate pickerDelegate;

        /// <summary>
        /// The media picker controller.
        /// </summary>
        private readonly UIImagePickerController picker;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaPickerPopoverDelegate"/> class.
        /// </summary>
        /// <param name="pickerDelegate">The media picker delegate.</param>
        /// <param name="picker">The media picker controller.</param>
        internal MediaPickerPopoverDelegate(MediaPickerDelegate pickerDelegate, UIImagePickerController picker)
        {
            this.pickerDelegate = pickerDelegate;
            this.picker = picker;
        }

        /// <summary>
        /// Gets a value determining whether the current pop-up should be dismissed.
        /// </summary>
        /// <param name="controller">The popover controller.</param>
        /// <returns><c>true</c> to dismiss the current pop-up; <c>false</c> otherwise.</returns>
        public override bool ShouldDismiss(UIPopoverController controller)
        {
            return true;
        }

        /// <summary>
        /// Called when the current pop-up is dismissed.
        /// </summary>
        /// <param name="controller">The popover controller.</param>
        public override void DidDismiss(UIPopoverController controller)
        {
            this.pickerDelegate.Canceled(this.picker);
        }
    }
}
