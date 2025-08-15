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
    public Contact Contact { get; set; } = new Contact();
    public ObservableCollection<Transaction> Transactions { get; set; } = new ObservableCollection<Transaction>();

    public ContactDetailsPage(Contact contact)
    {
        InitializeComponent();
        Contact = contact;
        BindingContext = this;
        FillTransaction();

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


}

