namespace IMS_Mobile.MVVM.Views;
using IMS_Mobile.MVVM.Models;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Transactions;
using System.Windows.Input;
using Contact = Models.Contact;
using Transaction = Models.Transaction;

public partial class ContactDetailsPage : ContentPage
{
    public Contact Contact { get; set; }= new Contact();
    public ObservableCollection<Transaction> Transactions { get; set; } = new ObservableCollection<Transaction>();

    public ContactDetailsPage(Contact contact)
    {
        InitializeComponent();
        Contact = contact;
        BindingContext = this;
        //FillTransaction();
        GenerateTestTransactions();
    }
    public void FillTransaction()
    {
        var contactTransactions = App.TransactionRepository.GetItemsWithChildren()
        .Where(x => x.ContactId == Contact.Id)
        .ToList();
        Transactions = new ObservableCollection<Transaction>(contactTransactions);
    }

    protected override bool OnBackButtonPressed()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Navigation.PopAsync();
            
        });
        return true; 
    }
    protected override void OnDisappearing()
    {
        OnBackButtonPressed();
        base.OnDisappearing();
        Debug.WriteLine($"remove form stack  {Navigation.NavigationStack.Count}");
    }

    #region Test data
    public void GenerateTestTransactions()
    {
        var random = new Random();

        var productNames = new[]
        {
            "iPhone 15 Pro", "MacBook Air", "AirPods Pro", "iPad Mini", "Apple Watch",
            "Samsung Galaxy", "Dell XPS Laptop", "Sony Headphones", "Logitech Keyboard", "HP Monitor",
            "Canon Camera", "JBL Speaker", "Microsoft Surface", "Google Pixel", "Lenovo Tablet"
        };

        // Generate 10 test transactions
        for (int i = 0; i < 10; i++)
        {
            var transaction = new Transaction
            {
                ContactId = 1, // Assuming default contact ID
                Type = "Sell", // Always sell as requested
                IsPaid = random.Next(2) == 0, // 50% chance of being paid/unpaid
                CreatedDate = DateTime.Now.AddDays(-random.Next(30)), // Random date within last 30 days
                totalamount = Math.Round(random.Next(100, 2000) + random.NextDouble(), 2)
            };

            // Generate products for this transaction
            var productCount = random.Next(1, 4);
            var products = new List<TransactionProductItem>();

            for (int j = 0; j < productCount; j++)
            {
                var productName = productNames[random.Next(productNames.Length)];
                var quantity = random.Next(1, 5);
                var price = Math.Round(random.Next(50, 800) + random.NextDouble(), 2);
                var cost = Math.Round(price * (0.6 + random.NextDouble() * 0.3), 2); // 60-90% of price

                var product = new TransactionProductItem
                {
                    Name = productName,
                    Quantity = quantity,
                    Price = price,
                    Cost = cost,
                    TransactionId = transaction.Id // This will be set after transaction is saved
                };

                products.Add(product);
            }

            transaction.Products = products;

            // TODO: Save transaction to database
            // App.TransactionRepository.SaveItemWithChildren(transaction);
            Transactions.Add(transaction);
        }
    }
    #endregion
}

 