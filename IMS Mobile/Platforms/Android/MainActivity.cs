using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Microsoft.Maui;

namespace IMS_Mobile;

[Activity(Theme = "@style/Maui.SplashTheme",
          MainLauncher = true,
          ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                                 ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density,
          ScreenOrientation = ScreenOrientation.Portrait)]
[IntentFilter(
    new[] { Intent.ActionView },
    Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
    DataScheme = "imsmobile",
    DataHost = "reset-password")]
public class MainActivity : MauiAppCompatActivity
{
    public override Android.Content.Res.Resources Resources
    {
        get
        {
            var baseRes = base.Resources;
            var config = new Configuration(baseRes.Configuration);
            if (config.FontScale != 1.0f)
            {
                config.FontScale = 1.0f;
                var metrics = new Android.Util.DisplayMetrics();
                WindowManager?.DefaultDisplay?.GetMetrics(metrics);
                baseRes.UpdateConfiguration(config, metrics);
            }
            return baseRes;
        }
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        var data = Intent?.DataString;
        if (!string.IsNullOrEmpty(data))
            Microsoft.Maui.Controls.Application.Current?.SendOnAppLinkRequestReceived(new Uri(data));
    }

    protected override void OnNewIntent(Intent? intent)
    {
        base.OnNewIntent(intent);
        var data = intent?.DataString;
        if (!string.IsNullOrEmpty(data))
            Microsoft.Maui.Controls.Application.Current?.SendOnAppLinkRequestReceived(new Uri(data));
    }
}
