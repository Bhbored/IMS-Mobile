using IMS_Mobile.DB;
using IMS_Mobile.MVVM.Models;
using IMS_Mobile.MVVM.ViewModels;
using Microsoft.Extensions.Logging;
using Contact = IMS_Mobile.MVVM.Models.Contact;

namespace IMS_Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CurrentApp { get; private set; }
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
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

            //ViewModels
            builder.Services.AddSingleton<HomeVM>();

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "appdb.db3");
            builder.Services.AddSingleton<AppDbContext>(sp =>
                new AppDbContext(dbPath));
            builder.Services.AddScoped(typeof(IRepository<Product>), typeof(Repository<Product>));
            builder.Services.AddScoped(typeof(IRepository<Contact>), typeof(Repository<Contact>));
            builder.Services.AddScoped(typeof(IRepository<Category>), typeof(Repository<Category>));
            builder.Services.AddScoped(typeof(IRepository<Transaction>), typeof(Repository<Transaction>));
            var app = builder.Build();
            CurrentApp = app;
            return app;
        }
    }
}
