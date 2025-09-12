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

                var email = (Email ?? string.Empty).Trim();
                var password = Password ?? string.Empty;

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

                    if (!_authService.IsOfflineSessionActive && NetworkHelper.IsConnected() == true)
                    {
                        LoginPage.Current?.CloseKeybord();
                        await App.RecreateRepositories();
                        await Task.Delay(100);
                        await _syncService.ClearLocalData();
                        await Task.Delay(1000);
                        await _syncService.SyncFromSupabase();
                        await Task.Delay(1000);
                    }

                    App.RecreateViewModels();

                    // Reset HomeVM to initial state for new account
                    App.homeVM?.ResetToInitialState();

                    // Update flyout header with user info
                    if (Application.Current?.MainPage is AppShell shell)
                    {
                        await shell.UpdateFlyoutHeaderAsync();
                    }

                    await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
                }
                else
                {

                    PasswordError = "Incorrect email or password. Tap 'Forgot Password?' to reset.";
                }
            }
            catch (Exception)
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
                if (ok.Ok)
                {
                    await Shell.Current.DisplayAlert("Email Sent", "If an account exists for that email, you'll receive reset instructions.", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", $"Couldn't send reset email.\n{ok.Error}", "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
        #region Google OAuth Login
        [RelayCommand]
        private async Task LoginWithGoogleAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            ErrorMessage = string.Empty;
            EmailError = string.Empty;
            PasswordError = string.Empty;
            try
            {
                var redirectUri = "imsmobile://login-callback";
                var result = await _authService.SignInWithGoogleAsync(redirectUri);
                if (result.Session != null)
                {
                    if (!_authService.IsOfflineSessionActive && NetworkHelper.IsConnected() == true)
                    {
                        LoginPage.Current?.CloseKeybord();
                        await App.RecreateRepositories();
                        await Task.Delay(100);
                        await _syncService.ClearLocalData();
                        await Task.Delay(1000);
                        await _syncService.SyncFromSupabase();
                        await Task.Delay(1000);
                    }
                    App.RecreateViewModels();
                    App.homeVM?.ResetToInitialState();
                    if (Application.Current?.MainPage is AppShell shell)
                    {
                        await shell.UpdateFlyoutHeaderAsync();
                    }
                    await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
                }
                else
                {
                    ErrorMessage = string.IsNullOrEmpty(result.Error) ? "Login failed. Please try again." : result.Error;
                }
            }
            catch
            {
                ErrorMessage = "Login failed. Please try again.";
            }
            finally
            {
                IsBusy = false;
            }
        }
        #endregion

    }
}