using System;

using Android.App;
using Android.Content;
using Android.Gms.Gcm.Iid;

namespace AppLimpia.Droid
{
    /// <summary>
    /// Lister to receive Token refresh request.
    /// </summary>
    [Service(Exported = false), IntentFilter(new[] { "com.google.android.gms.iid.InstanceID" })]
    // ReSharper disable once UnusedMember.Global Justification = Created by OS
    public class TokenRefreshListener : InstanceIDListenerService
    {
        /// <summary>
        /// Handles the token refresh request.
        /// </summary>
        public override void OnTokenRefresh()
        {
            // Refresh the service token
            var intent = new Intent(this, typeof(RegistrationIntentService));
            this.StartService(intent);
        }
    }
}
