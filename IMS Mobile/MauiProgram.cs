using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using IMS_Mobile.DB;
using IMS_Mobile.MVVM.Models;
using IMS_Mobile.MVVM.ViewModels;
using IMS_Mobile.MVVM.Views;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;
using Syncfusion.Maui.Toolkit.Hosting;
using Supabase;
using System.Reflection;
using Contact = IMS_Mobile.MVVM.Models.Contact;
using IMS_Mobile.Service;

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

            // Configure Supabase
            var supabaseUrl = "https://leuyksaxpnppatlpitav.supabase.co"; 
            var supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImxldXlrc2F4cG5wcGF0bHBpdGF2Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTYwNDkyMzcsImV4cCI6MjA3MTYyNTIzN30.98ho7Ne_WOj_ihRcIyQDsDp_lzQzRVGFajLh5r7W8pc";
            var options = new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true

            };

            var supabase = new Supabase.Client(supabaseUrl, supabaseKey, options);

            // Register Supabase client as a singleton
            builder.Services.AddSingleton(supabase);

            // Register your sync service
            builder.Services.AddSingleton<SyncService>();

            return builder.Build();
        }
    }
}
