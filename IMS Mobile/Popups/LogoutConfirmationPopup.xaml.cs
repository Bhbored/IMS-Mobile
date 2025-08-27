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

    public LogoutConfirmationPopup(SupabaseAuthService authService)
    {
        InitializeComponent();
        BindingContext = this;
        _authService = authService;
    }
    private void OnCancelClicked(object sender, EventArgs e)
    {
        CloseAsync();
    }
    public ICommand Logout => new Command(async () =>
    {
        await logout();
    });
    [RelayCommand]
    public async Task logout()
    {
        await _authService.SignOutAsync();
        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }
}