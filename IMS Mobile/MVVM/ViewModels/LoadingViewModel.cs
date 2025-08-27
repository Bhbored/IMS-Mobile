using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IMS_Mobile.MVVM.Views;
using IMS_Mobile.Service;

namespace IMS_Mobile.MVVM.ViewModels
{
    public partial class LoadingViewModel : ObservableObject
    {
        private readonly SupabaseAuthService _authService;

        public LoadingViewModel(SupabaseAuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        public async Task CheckAuthenticationAsync()
        {
            try
            {
                bool isAuthenticated = await _authService.InitializeAsync();

                if (isAuthenticated && _authService.IsUserAuthenticated)
                {
                    if (_authService.IsOfflineSessionActive)
                    {
                        await Shell.Current.DisplayAlert("⚠️Offline", "You're offline. Changes won't sync until you're online.", "OK");
                    }
                    await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
                }
                else
                {
                    await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                }
            }
            catch
            {
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
        }
    }
}