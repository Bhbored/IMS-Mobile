using IMS_Mobile.DB;
using IMS_Mobile.MVVM.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Syncfusion.Licensing;
using System.Diagnostics;
using IMS_Mobile.MVVM.Models;
using Contact = IMS_Mobile.MVVM.Models.Contact;

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
        #endregion

        public App(BaseRepository<Transaction> _transaction, BaseRepository<Product> _productrepo,
            BaseRepository<Contact> _contactrepo, BaseRepository<TransactionProductItem> _transactionProductItemRepo
            , HomeVM _vm)
        {
            InitializeComponent();
            SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JEaF5cXmRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXlceHRTQ2ZYWUN/XkFWYEk=");
            TransactionRepository = _transaction;
            ProductRepository = _productrepo;
            ContactRepository = _contactrepo;
            TransactionProductItemRepository = _transactionProductItemRepo;
            homeVM = _vm;
            //GenerateTestDataAsync();
        }

       

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        #region test data injection
        private void GenerateTestDataAsync()
        {
            try
            {
                var random = new Random();
                var types = new[] { "buy", "sell" };
                var productNames = new[] { "Laptop", "Phone", "Tablet", "Headphones", "Mouse", "Keyboard", "Monitor", "Speaker", "Camera", "Printer" };

                for (int i = 1; i <= 50; i++)
                {
                    var transaction = new Transaction
                    {
                        totalamount = Math.Round(random.NextDouble() * 1000 + 50, 2),
                        Type = types[random.Next(types.Length)],
                        IsPaid = random.Next(2) == 1,
                        CreatedDate = DateTime.Now.AddDays(-random.Next(30)),
                        ContactId = 0
                    };

                    var productCount = random.Next(1, 6);
                    for (int j = 0; j < productCount; j++)
                    {
                        transaction.Products.Add(new TransactionProductItem
                        {
                            Name = productNames[random.Next(productNames.Length)],
                            Price = Math.Round(random.NextDouble() * 200 + 20, 2),
                            Quantity = random.Next(1, 10),
                            Cost = Math.Round(random.NextDouble() * 150 + 10, 2)
                        });
                    }
                    i++;
                    TransactionRepository.InsertItemWithChildren(transaction, true);
                }

                Debug.WriteLine("Test data generated successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Test data generation failed: {ex.Message}");
            }
        }
        #endregion
    }
}