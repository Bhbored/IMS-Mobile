using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using IMS_Mobile.DB;
using IMS_Mobile.MVVM.Models;
using IMS_Mobile.MVVM.ViewModels;
using IMS_Mobile.MVVM.Views;
using IMS_Mobile.Service;
using Microsoft.Extensions.Logging;
using Plugin.AdMob;
using Plugin.AdMob.Configuration;
using Supabase;
using Syncfusion.Maui.Core.Hosting;
using Syncfusion.Maui.Toolkit.Hosting;
using System.Reflection;
using Contact = IMS_Mobile.MVVM.Models.Contact;

namespace IMS_Mobile
{
    public static class MauiProgram
    {

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureSyncfusionCore()
                .ConfigureSyncfusionToolkit()
                .UseMauiCommunityToolkitCore()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Poppins-Bold.ttf", "poppinbold");
                    fonts.AddFont("Poppins-Regular.ttf", "poppinregular");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            //ads 
            builder.UseAdMob();
            AdConfig.UseTestAdUnitIds = true;   
            builder.Services.AddSingleton<SupabaseAuthService>();
            builder.Services.AddSingleton<LoadingPage>();
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<SignUpPage>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<Analytics>();
            builder.Services.AddTransient<GeneratedReport>();
            builder.Services.AddSingleton<LoadingViewModel>();
            builder.Services.AddSingleton<LoginViewModel>();
            builder.Services.AddSingleton<SignUpViewModel>();
            // Register the database
            builder.Services.AddTransient<BaseRepository<Transaction>>();
            builder.Services.AddTransient<BaseRepository<Product>>();
            builder.Services.AddTransient<BaseRepository<Contact>>();
            builder.Services.AddTransient<BaseRepository<TransactionProductItem>>();
            builder.Services.AddTransient<UserPreferencesRepository>();
            //ViewModels
            builder.Services.AddTransient<HomeVM>();
            builder.Services.AddTransient<AnalyticsVM>();
            builder.Services.AddTransient<ContactsVM>();
            builder.Services.AddTransient<InventoryVM>();
            builder.Services.AddTransient<ReportsVM>();
            builder.Services.AddTransient<AnalyticsVM>();
            builder.Services.AddTransient<GeneratedReportVM>();
            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddSingleton<ResetPasswordPage>();

            var options = new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true

            };
        
            var supabase = new Supabase.Client(supabaseUrl, supabaseKey, options);
            builder.Services.AddSingleton(supabase);
            builder.Services.AddSingleton<SyncService>();


            var app = builder.Build();
            ServiceLocator.Services = app.Services;
            return app;
        }
    }
}
