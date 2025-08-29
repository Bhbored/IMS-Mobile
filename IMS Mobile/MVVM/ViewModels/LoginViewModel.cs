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

        [ObservableProperty]
        private string _emailError = string.Empty;

        [ObservableProperty]
        private string _passwordError = string.Empty;

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
            EmailError = string.Empty;
            PasswordError = string.Empty;

            try
            {
                // Normalize inputs
                var email = (Email ?? string.Empty).Trim();
                var password = Password ?? string.Empty;

                // Basic client-side validation
                if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
                {
                    EmailError = "Enter a valid email address.";
                }
                if (string.IsNullOrWhiteSpace(password))
                {
                    PasswordError = "Password is required.";
                }
                if (!string.IsNullOrEmpty(EmailError) || !string.IsNullOrEmpty(PasswordError))
                {
                    return;
                }

                var session = await _authService.SignInAsync(email, password);
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
                    // For security, we can't reliably distinguish between unknown email and wrong password
                    // Provide contextual guidance next to fields instead
                    PasswordError = "Incorrect email or password. Tap 'Forgot Password?' to reset.";
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

        [RelayCommand]
        private async Task ForgotPasswordAsync()
        {
            if (IsBusy) return;
            try
            {
                if (string.IsNullOrWhiteSpace(Email))
                {
                    await Shell.Current.DisplayAlert("Reset Password", "Enter your email first.", "OK");
                    return;
                }
                IsBusy = true;
                var ok = await _authService.SendPasswordResetEmailAsync(Email);
                if (ok)
                {
                    await Shell.Current.DisplayAlert("Email Sent", "If an account exists for that email, you'll receive reset instructions.", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Couldn't send reset email. Try again later.", "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}