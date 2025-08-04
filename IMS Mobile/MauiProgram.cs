using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using IMS_Mobile.DB;
using IMS_Mobile.MVVM.Models;
using IMS_Mobile.MVVM.ViewModels;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;
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
                .UseMauiCommunityToolkitCore()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    //fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    //fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
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


            return builder.Build();
        }
    }
}
