namespace IMS_Mobile.MVVM.Views;

using CommunityToolkit.Maui.Extensions;
using IMS_Mobile.MVVM.Models;
using IMS_Mobile.Popups;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Transactions;
using System.Windows.Input;
using Contact = Models.Contact;
using Transaction = Models.Transaction;
using CommunityToolkit.Maui.Views;

public partial class ContactDetailsPage : ContentPage
{
    public Contact Contact { get; set; } = new Contact();
    public ObservableCollection<Transaction> Transactions { get; set; } = new ObservableCollection<Transaction>();

    private List<Transaction> _allTransactions = new List<Transaction>();
    private int _currentIndex = 0;
    private const int _pageSize = 5;
    private bool _isLoading = false;
    private bool _isPopupShowing = false;

    public ContactDetailsPage(Contact contact, List<Transaction> transactions)
    {
        InitializeComponent();
        Contact = contact;
        BindingContext = this;
        LoadInitialTransactions(transactions);
    }

    public void LoadInitialTransactions(List<Transaction> transactions)
    {
        _allTransactions = transactions;
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
        if (!_isPopupShowing)
        {
            OnBackButtonPressed();
        }
        base.OnDisappearing();
        Debug.WriteLine($"remove form stack  {Navigation.NavigationStack.Count}");
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        if (_isPopupShowing) return;
        try
        {
            _isPopupShowing = true;

            var result = await DisplayPromptAsync(
                "Reduce Credit Score",
                $"Current Score: {Contact.CreditScore:F1}\n\nEnter reduction amount:",
                "Confirm",
                "Cancel",
                "Enter amount...",
                keyboard: Keyboard.Numeric,
                initialValue: "");

            if (!string.IsNullOrEmpty(result))
            {
                if (double.TryParse(result, out double reduction) && reduction > 0)
                {
                    var newScore = Contact.CreditScore - reduction;
                    Contact.CreditScore = newScore;
                    App.ContactRepository.UpdateItem(Contact);
                }
                else if (!string.IsNullOrEmpty(result))
                {
                    await DisplayAlert("⚠️ Invalid Input", "Please enter a valid number greater than 0", "OK");
                }
            }
        }
        catch (Exception ex)
        {

            await DisplayAlert("❌ Error", "⚠️ Unable to show input dialog.\n\nPlease try again.", "OK");
        }
        finally
        {
            _isPopupShowing = false;
        }
    }
}
