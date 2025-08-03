using IMS_Mobile.DB;
using IMS_Mobile.MVVM.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Syncfusion.Licensing;
using System.Diagnostics;

namespace IMS_Mobile
{
    public partial class App : Application
    {
        #region DI
        public static HomeVM homeVM { get; set; }
        #endregion
        public App(HomeVM _vm)
        {
            InitializeComponent();
            SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JEaF5cXmRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXlceHRTQ2ZYWUN/XkFWYEk=");
            homeVM = _vm;
        }

        protected override async void OnStart()
        {
            base.OnStart();

            try
            {
                var dbContext = MauiProgram.CurrentApp.Services.GetService<AppDbContext>();
                if (dbContext != null)
                {
                    await dbContext.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Migration failed: {ex.Message}");
            }
        }
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}