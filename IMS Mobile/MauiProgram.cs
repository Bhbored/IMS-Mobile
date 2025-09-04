using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using IMS_Mobile.DB;
using IMS_Mobile.MVVM.Models;
using IMS_Mobile.MVVM.ViewModels;
using IMS_Mobile.MVVM.Views;
using IMS_Mobile.Service;
using Microsoft.Extensions.Logging;
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
            builder.Services.AddSingleton<SupabaseAuthService>();
            builder.Services.AddSingleton<LoadingPage>();
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<SignUpPage>();
            builder.Services.AddSingleton<LoadingViewModel>();
            builder.Services.AddSingleton<LoginViewModel>();
            builder.Services.AddSingleton<SignUpViewModel>();
            // Register the database
            builder.Services.AddSingleton<BaseRepository<Transaction>>();
            builder.Services.AddSingleton<BaseRepository<Product>>();
            builder.Services.AddSingleton<BaseRepository<Contact>>();
            builder.Services.AddSingleton<BaseRepository<TransactionProductItem>>();
            //ViewModels
            builder.Services.AddSingleton<HomeVM>();
            builder.Services.AddSingleton<ContactsVM>();
            builder.Services.AddSingleton<InventoryVM>();
            builder.Services.AddSingleton<ReportsVM>();
            builder.Services.AddSingleton<ResetPasswordPage>();

            // Configure Supabase
           
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
