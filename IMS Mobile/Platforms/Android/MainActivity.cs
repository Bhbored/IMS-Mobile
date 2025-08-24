using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.Util;
using Microsoft.Maui;

[Activity(Theme = "@style/Maui.SplashTheme",
          MainLauncher = true,
          ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                                 ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density,
          ScreenOrientation = ScreenOrientation.Portrait)]
public class MainActivity : MauiAppCompatActivity
{
    public override Android.Content.Res.Resources Resources
    {
        get
        {
            var baseRes = base.Resources;
            var config = new Configuration(baseRes.Configuration);

            // Force no scaling from system “Display size / Text size”
            if (config.FontScale != 1.0f)
            {
                config.FontScale = 1.0f;
                var metrics = new DisplayMetrics();
                WindowManager?.DefaultDisplay?.GetMetrics(metrics);
                baseRes.UpdateConfiguration(config, metrics);
            }
            return baseRes;
        }
    }
}
