using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using IMS_Mobile.MVVM.ViewModels;
using IMS_Mobile.MVVM.Views;
using IMS_Mobile.Service;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IMS_Mobile.Popups;

public partial class LogoutConfirmationPopup : Popup
{
    private readonly SupabaseAuthService _authService;
    private readonly SyncService _syncService;
    public bool IsLoading { get; private set; } = false;
    public bool IsNotLoading { get; private set; } = true;

    public LogoutConfirmationPopup(SupabaseAuthService authService, SyncService syncService)
    {
        InitializeComponent();
        BindingContext = this;
        _authService = authService;
        _syncService = syncService;
        IsLoading = false;
    }
    private void OnCancelClicked(object sender, EventArgs e)
    {
        CloseAsync();
    }
    public ICommand Logout => new Command(async () =>
    {
        var connected = NetworkHelper.IsConnected();
        try
        {
            if (connected)
            {
                IsNotLoading = false;
                IsLoading = true;
                await logout();
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Attention ⚠️", "You're not Connected", "ok");
                await CloseAsync();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed becasue {ex.Message}");

        }
        finally
        {
            IsLoading = false;
            IsNotLoading = true;
        }


    });
    [RelayCommand]
    public async Task logout()
    {

        await _syncService.SyncToSupabase();
        await Task.Delay(1000);
        await App.StopConnection();
        await Task.Delay(1000);
        await _authService.SignOutAsync();
        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }
}