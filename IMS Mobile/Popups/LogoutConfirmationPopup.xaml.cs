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
                await Application.Current!.MainPage!.DisplayAlert("Attention ⚠️", "You're not Connected", "ok");
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
        // 1. Sync local changes to Supabase first
        await _syncService.SyncToSupabase();
        await Task.Delay(1000);

        // 2. Stop connections and wipe all tables (this clears everything)
        await App.StopConnection();
        await Task.Delay(1000);

        // 3. Sign out from auth service
        await _authService.SignOutAsync();

        // 4. Clear ViewModel references
        App.homeVM = null;
        App.contactsVM = null;
        App.inventoryVM = null;
        App.reportsVM = null;

        // 5. Navigate to login
        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }
}