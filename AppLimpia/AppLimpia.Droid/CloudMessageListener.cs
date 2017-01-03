using System;

using Android.App;
using Android.Content;
using Android.Gms.Gcm;
using Android.Media;
using Android.OS;
using Android.Util;

namespace AppLimpia.Droid
{
    /// <summary>
    /// The cloud message listener service.
    /// </summary>
    [Service(Exported = false), IntentFilter(new[] { "com.google.android.c2dm.intent.RECEIVE" })]
    // ReSharper disable once UnusedMember.Global Justification = Created by OS
    public class CloudMessageListener : GcmListenerService
    {
        /// <summary>
        /// Handles new message from cloud.
        /// </summary>
        /// <param name="from">Message sender.</param>
        /// <param name="data">Message body.</param>
        public override void OnMessageReceived(string from, Bundle data)
        {
            var message = data.GetString("message");
            Log.Debug("CloudMessageListener", "From:    " + from);
            Log.Debug("CloudMessageListener", "Message: " + message);
            this.ShowNotification(message);
        }

        /// <summary>
        /// Shows the notification in th notification tray.
        /// </summary>
        /// <param name="message">The notification message to show.</param>
        private void ShowNotification(string message)
        {
            // If the application is active
            var instance = Xamarin.Forms.Application.Current as App;
            if (instance?.IsActive == true)
            {
                // Show a message
                Xamarin.Forms.Device.BeginInvokeOnMainThread(
                    () =>
                        App.DisplayAlert(
                            Properties.Localization.NotificationDialogTitle,
                            message,
                            Properties.Localization.DialogDismiss));
                return;
            }

            // Launch the application on notification click
            var intent = new Intent(this, typeof(MainActivity));
            intent.SetAction(Intent.ActionView);
            intent.PutExtra(MainActivity.StartView, MainActivity.NotificationsView);
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

            // Create a new notification
            var sound = RingtoneManager.GetDefaultUri(RingtoneType.Notification);
            var notificationBuilder =
                new Android.App.Notification.Builder(this).SetSmallIcon(Resource.Drawable.icon)
                    .SetContentTitle("Xalapa Limpia")
                    .SetContentText(message)
                    .SetSound(sound)
                    .SetAutoCancel(true)
                    .SetContentIntent(pendingIntent);

            // Show notification in the notification tray
            var notificationManager = (NotificationManager)this.GetSystemService(Context.NotificationService);
            notificationManager.Notify(0, notificationBuilder.Build());
        }
    }
}
