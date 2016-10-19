using System;

using Android.App;
using Android.Content;
using Android.Gms.Gcm;
using Android.Gms.Gcm.Iid;
using Android.Util;

namespace AppLimpia.Droid
{
    /// <summary>
    /// The GCM registration service.
    /// </summary>
    [Service(Exported = false)]
    public class RegistrationIntentService : IntentService
    {
        /// <summary>
        /// The lock object to prevent concurrent operations.
        /// </summary>
        private static readonly object Locker = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationIntentService"/> class.
        /// </summary>
        public RegistrationIntentService()
            : base("RegistrationIntentService")
        {
        }

        /// <summary>
        /// Handled the action requested by intent.
        /// </summary>
        /// <param name="intent">The Intent to process.</param>
        protected override void OnHandleIntent(Intent intent)
        {
            try
            {
                // Prevent concurrent requests
                Log.Info("RegistrationIntentService", "Calling InstanceID.GetToken");
                lock (RegistrationIntentService.Locker)
                {
                    // Get the application instance ID
                    var instanceId = InstanceID.GetInstance(this);
                    var token = instanceId.GetToken("593931297162", GoogleCloudMessaging.InstanceIdScope, null);

                    // Send the registration token to the App server
                    Log.Info("RegistrationIntentService", "GCM Registration Token: " + token);
                    this.SendRegistrationToAppServer(token);
                    this.Subscribe(token);
                }
            }
            catch (Exception ex)
            {
                Log.Debug("RegistrationIntentService", "Failed to get a registration token");
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Sends the registration token to the App server.
        /// </summary>
        /// <param name="token">The registration token.</param>
        private void SendRegistrationToAppServer(string token)
        {
            // Add custom implementation here as needed.
        }

        void Subscribe(string token)
        {
            var pubSub = GcmPubSub.GetInstance(this);
            pubSub.Subscribe(token, "/topics/global", null);
        }
    }
}