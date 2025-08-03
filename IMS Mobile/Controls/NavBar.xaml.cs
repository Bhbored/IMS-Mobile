using Microsoft.Maui;
using System.Windows.Input;

namespace IMS_Mobile.Controls;

public partial class NavBar : ContentView
{
    public NavBar()
    {
        InitializeComponent();
        BindingContext = this;
        UpdateActiveTab();
    }

    protected override void OnParentSet()
    {
        base.OnParentSet();
        UpdateActiveTab();
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        UpdateActiveTab();
    }

    public void UpdateActiveTab()
    {
        var current = Shell.Current?.CurrentState?.Location?.ToString();

        // Reset all
        SetInactive(homeBorder, homeLabel);
        SetInactive(invBorder, invLabel);
        SetInactive(contactBorder, contactLabel);
        SetInactive(reportsBorder, reportsLabel);

        // Set active
        switch (current)
        {
            case "/HomePage": SetActive(homeBorder, homeLabel); break;
            case "/InventoryPage": SetActive(invBorder, invLabel); break;
            case "/ContactsPage": SetActive(contactBorder, contactLabel); break;
            case "/ReportsPage": SetActive(reportsBorder, reportsLabel); break;
        }
    }

    private void SetActive(Border border, Label label)
    {
        border.StyleClass = new[] { "ActiveNavigationItemStyle" };
        label.StyleClass = new[] { "ActiveNavigationTextStyle" };
    }

    private void SetInactive(Border border, Label label)
    {
        border.StyleClass = new[] { "NavigationItemStyle" };
        label.StyleClass = new[] { "NavigationTextStyle" };
    }

    #region Commands
    public ICommand HomeCommand => new Command(async () => await Shell.Current.GoToAsync("//HomePage"));
    public ICommand InvCommand => new Command(async () => await Shell.Current.GoToAsync("//InventoryPage"));
    public ICommand ContactCommand => new Command(async () => await Shell.Current.GoToAsync("//ContactsPage"));
    public ICommand ReportsCommand => new Command(async () => await Shell.Current.GoToAsync("//ReportsPage"));
    #endregion
}