using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IMS_Mobile.MVVM.Views;
using IMS_Mobile.Service;

namespace IMS_Mobile.MVVM.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly SupabaseAuthService _authService;
        private readonly SyncService _syncService;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _isBusy;

        public LoginViewModel(SupabaseAuthService authService, SyncService syncService)
        {
            _authService = authService;
            _syncService = syncService;
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var session = await _authService.SignInAsync(Email, Password);
                if (session != null)
                {
                    // If currently offline, inform the user about limited functionality
                    if (_authService.IsOfflineSessionActive)
                    {
                        await Shell.Current.DisplayAlert("Offline", "You're offline. Changes won't sync until you're online.", "OK");
                    }
                    await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
                }
                else
                {
                    ErrorMessage = "Invalid credentials";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Login failed. Please try again.";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task SignUpAsync()
        {
            await Shell.Current.GoToAsync(nameof(SignUpPage));
        }
    }
}