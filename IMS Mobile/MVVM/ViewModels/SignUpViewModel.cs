using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

        public SignUpViewModel(SupabaseAuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            if (IsBusy) return;

            if (string.IsNullOrEmpty(Password) || Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match";
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var session = await _authService.SignUpAsync(Email, Password);
                if (session != null)
                {
                    await Shell.Current.DisplayAlert("Success", "Account created successfully!", "OK");
                    await Shell.Current.GoToAsync($"//{nameof(IMS_Mobile.MVVM.Views.HomePage)}");
                }
                else
                {
                    ErrorMessage = "Registration failed. Check your email for confirmation or try signing in.";
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