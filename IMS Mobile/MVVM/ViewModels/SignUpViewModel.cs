using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IMS_Mobile.MVVM.Views;
using IMS_Mobile.Service;

namespace IMS_Mobile.MVVM.ViewModels
{
    public partial class SignUpViewModel : ObservableObject
    {
        private readonly SupabaseAuthService _authService;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _confirmPassword = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _emailError = string.Empty;

        [ObservableProperty]
        private string _passwordError = string.Empty;

        [ObservableProperty]
        private string _confirmPasswordError = string.Empty;

        public SignUpViewModel(SupabaseAuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            ErrorMessage = string.Empty;
            EmailError = string.Empty;
            PasswordError = string.Empty;
            ConfirmPasswordError = string.Empty;

            try
            {
                var email = (Email ?? string.Empty).Trim();
                var password = Password ?? string.Empty;
                var confirm = ConfirmPassword ?? string.Empty;

                if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
                {
                    EmailError = "Enter a valid email address.";
                }

                bool hasMinLen = password.Length >= 6;
                bool hasLetter = password.Any(char.IsLetter);
                bool hasDigit = password.Any(char.IsDigit);
                if (!hasMinLen || !hasLetter || !hasDigit)
                {
                    PasswordError = "Password must be 6+ chars and include letters and numbers.";
                }

                if (password != confirm)
                {
                    ConfirmPasswordError = "Passwords do not match.";
                }

                if (!string.IsNullOrEmpty(EmailError) || !string.IsNullOrEmpty(PasswordError) || !string.IsNullOrEmpty(ConfirmPasswordError))
                {
                    return;
                }

                var (session, error) = await _authService.SignUpWithResultAsync(email, password);
                if (error != null)
                {
                    if (error.Contains("exists", StringComparison.OrdinalIgnoreCase))
                    {
                        EmailError = error;
                    }
                    else
                    {
                        ErrorMessage = error;
                    }
                    return;
                }

                if (session != null)
                {
                    await Shell.Current.DisplayAlert("Success", "Account created successfully!", "OK");
                    await Task.Delay(100);
                    await App.RecreateRepositories();
                    await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Confirm Your Email", "We sent a confirmation email. Please verify to complete sign up.", "OK");
                    await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Registration failed. Please try again.";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}