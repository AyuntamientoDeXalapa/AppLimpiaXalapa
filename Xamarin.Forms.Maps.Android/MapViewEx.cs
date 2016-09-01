using System;

using Android.Content;
using Android.Gms.Maps;
using Android.Views;

namespace Xamarin.Forms.Maps.Android
{
    /// <summary>
    /// The extended MapView class.
    /// </summary>
    public class MapViewEx : MapView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapViewEx"/> class.
        /// </summary>
        /// <param name="context">The current execution context.</param>
        public MapViewEx(Context context)
            : base(context)
        {
        }

        /// <summary>
        /// Event that is raised when the current control is touched.
        /// </summary>
        public event EventHandler<TouchEventArgs> TouchEvent;


        /// <summary>
        /// Dispatches the touch event of the current control.
        /// </summary>
        /// <param name="e">A <see cref="MotionEvent"/> with arguments of the event.</param>
        /// <returns><c>true</c> if the event was processed; <c>false</c> otherwise.</returns>
        public override bool DispatchTouchEvent(MotionEvent e)
        {
            this.TouchEvent?.Invoke(this, new TouchEventArgs(false, e));
            return base.DispatchTouchEvent(e);
        }
    }
}