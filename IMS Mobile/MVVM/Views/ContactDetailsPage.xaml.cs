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

    private List<Transaction> _allTransactions = new List<Transaction>();
    private int _currentIndex = 0;
    private const int _pageSize = 5;
    private bool _isLoading = false;

    public ContactDetailsPage(Contact contact, List<Transaction> transactions)
    {
        InitializeComponent();
        Contact = contact;
        BindingContext = this;
        LoadInitialTransactions(transactions);
    }

    public void LoadInitialTransactions(List<Transaction> transactions)
    {
        _allTransactions =transactions;
        LoadNextPage();
    }

    private void LoadNextPage()
    {
        if (_isLoading) return;

        _isLoading = true;

        var nextBatch = _allTransactions
            .Skip(_currentIndex)
            .Take(_pageSize)
            .ToList();

        foreach (var transaction in nextBatch)
        {
            Transactions.Add(transaction);
        }

        _currentIndex += _pageSize;
        _isLoading = false;
    }

    private void OnRemainingItemsThresholdReached(object sender, EventArgs e)
    {
        if (_currentIndex < _allTransactions.Count)
        {
            LoadNextPage();
        }
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
