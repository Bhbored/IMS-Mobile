using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using IMS_Mobile.MVVM.ViewModels;
using IMS_Mobile.MVVM.Views;
using IMS_Mobile.Service;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IMS_Mobile.Popups;

public partial class LogoutConfirmationPopup : Popup
{
    private readonly SupabaseAuthService _authService;
    private readonly SyncService _syncService;

    public LogoutConfirmationPopup(SupabaseAuthService authService, SyncService syncService)
    {
        InitializeComponent();
        BindingContext = this;
        _authService = authService;
        _syncService = syncService;
    }
    private void OnCancelClicked(object sender, EventArgs e)
    {
        CloseAsync();
    }
    public ICommand Logout => new Command(async () =>
    {
        var connected = NetworkHelper.IsConnected();
        if (connected)
        {
            await logout();
        }
        else
        {
            await App.Current.MainPage.DisplayAlert("Attention ⚠️", "You're not Connected", "ok");
            await CloseAsync();
        }

    });
    [RelayCommand]
    public async Task logout()
    {
        await _syncService.SyncToSupabase();
        await Task.Delay(1000);
        _syncService.ClearLocalData();
        await _authService.SignOutAsync();
        App.StopConnection();
        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }
}