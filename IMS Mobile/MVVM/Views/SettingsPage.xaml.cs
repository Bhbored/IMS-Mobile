namespace IMS_Mobile.MVVM.Views;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
        SetupPickers();
    }

    private void SetupPickers()
    {
        // Set default selections
        themePicker.SelectedIndex = 0;
        currencyPicker.SelectedIndex = 0;
    }



    private void OnChangeAvatarClicked(object sender, EventArgs e)
    {
        // Handle avatar change
    }

    private void OnDisplayNameChanged(object sender, TextChangedEventArgs e)
    {
        // Handle display name change
    }

    private void OnThemeChanged(object sender, EventArgs e)
    {
        // Handle theme change
    }


    private void OnCurrencyChanged(object sender, EventArgs e)
    {
        // Handle currency change
    }

    private void OnSaveClicked(object sender, EventArgs e)
    {
        // Handle save
    }

    private void OnResetClicked(object sender, EventArgs e)
    {
        // Handle reset
    }
}