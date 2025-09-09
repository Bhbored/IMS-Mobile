

using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using IMS_Mobile.MVVM.ViewModels;
using IMS_Mobile.Popups;
using IMS_Mobile.Service;
using System.Diagnostics;

namespace IMS_Mobile.MVVM.Views;

public partial class HomePage : ContentPage
{
    private static readonly IList<string> ActiveFilter = new List<string> { "ActiveFilterButtonStyle" };
    private static readonly IList<string> InActiveFilter = new List<string> { "FilterButtonStyle" };
    public static HomePage? Current { get; private set; }

    private bool _isConnected;
    private bool _hasShownOfflineAlert = false;
    public HomePage(HomeVM vm)
    {
        InitializeComponent();
        BindingContext = vm;
        Current = this;

        // Set the HomePage reference in the ViewModel
        vm.HomePage = this;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is HomeVM vm)
        {
            // Always reload transactions to ensure fresh data for new accounts
            await vm.LoadTransactionsAsync();
            ResetAllFilters();
        }
    }
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        _isConnected = NetworkHelper.IsConnected();

        if (!_isConnected && !_hasShownOfflineAlert)
        {
            _hasShownOfflineAlert = true;
            await Task.Delay(500);
            try
            {
                var popup = new OfflineAlertPopup();
                await this.ShowPopupAsync(popup);
            }
            catch (Exception)
            {
                await this.DisplayAlert("⚠️Offline Mode", "You're offline. Changes will be saved locally and synced when you're back online.", "OK");
            }
        }
    }

    #region styling logic
    private void Button_Clicked(object sender, EventArgs e)
    {
        var btn = sender as Button;
        if (btn == null) return;

        ResetAllFilters();

        switch (btn.AutomationId)
        {
            case "filter1":
                Filter11.StyleClass = ActiveFilter;
                break;
            case "filter2":
                Filter22.StyleClass = ActiveFilter;
                break;
            case "filter3":
                Filter33.StyleClass = ActiveFilter;
                break;

        }
    }

    public void ResetAllFilters()
    {
        Filter11.StyleClass = InActiveFilter;
        Filter22.StyleClass = InActiveFilter;
        Filter33.StyleClass = InActiveFilter;
    }
    #endregion


    private void Button_Clicked_1(object sender, EventArgs e)
    {
        if (BindingContext is HomeVM vm)
        {
            AppShell.Current.ShowPopupAsync(new DatePickerPopup(vm));
        }

    }

}