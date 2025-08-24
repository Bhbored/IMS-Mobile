using IMS_Mobile.DB;
using IMS_Mobile.MVVM.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Syncfusion.Licensing;
using System.Diagnostics;
using IMS_Mobile.MVVM.Models;
using Contact = IMS_Mobile.MVVM.Models.Contact;
using IMS_Mobile.Service;

namespace IMS_Mobile
{
    public partial class App : Application
    {
        #region DI
        public static BaseRepository<Transaction>? TransactionRepository { get; set; }
        public static BaseRepository<Product>? ProductRepository { get; set; }
        public static BaseRepository<Contact>? ContactRepository { get; set; }
        public static BaseRepository<TransactionProductItem>? TransactionProductItemRepository { get; set; }
        public static HomeVM? homeVM { get; set; }
        public static ContactsVM? contactsVM { get; set; }
        public static InventoryVM? inventoryVM { get; set; }
        public static ReportsVM? reportsVM { get; set; }

        private readonly Supabase.Client _supabaseClient;
        private readonly SyncService _syncService;
        #endregion

        public App(BaseRepository<Transaction> _transaction, BaseRepository<Product> _productrepo,
            BaseRepository<Contact> _contactrepo, BaseRepository<TransactionProductItem> _transactionProductItemRepo
            , HomeVM _vm, ContactsVM _contactVM, InventoryVM _inventoryVM, ReportsVM _reportsVM,
            Supabase.Client supabaseClient, SyncService syncService)
        {

            InitializeComponent();
            SyncfusionLicenseProvider.RegisterLicense
           ("Ngo9BigBOggjHTQxAR8/V1JEaF5cXmRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXlceHRTQ2ZYWUN/XkFWYEk=");
            TransactionRepository = _transaction;
            ProductRepository = _productrepo;
            ContactRepository = _contactrepo;
            TransactionProductItemRepository = _transactionProductItemRepo;
            homeVM = _vm;
            contactsVM = _contactVM;
            inventoryVM = _inventoryVM;
            reportsVM = _reportsVM;
            _supabaseClient = supabaseClient;
            _syncService = syncService;
            InitializeSupabase();
        }



        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
        private async void InitializeSupabase()
        {
            try
            {
                await _supabaseClient.InitializeAsync();
                Console.WriteLine("Supabase initialized successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing Supabase: {ex.Message}");
            }
        }

        #region delete db
        public void DiposeCurrentDB()
        {
            ProductRepository.Dispose();
            TransactionRepository.Dispose();
            ContactRepository.Dispose();
            TransactionProductItemRepository.Dispose();
        }
        #endregion
    }
}