using Android.App;
using Android.Content.PM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Content;
using Microsoft.Maui.Authentication;

namespace IMS_Mobile.Platforms.Android
{
    [Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
    [IntentFilter(
         new[] { Intent.ActionView },
         Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
         DataScheme = "imsmobile")]
    public class WebAuthenticationCallbackActivity : WebAuthenticatorCallbackActivity
    {
    }
}
