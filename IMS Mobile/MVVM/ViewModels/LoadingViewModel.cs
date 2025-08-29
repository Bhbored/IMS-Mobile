using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IMS_Mobile.MVVM.Views;
using IMS_Mobile.Service;
using System.IdentityModel.Tokens.Jwt;

namespace IMS_Mobile.MVVM.ViewModels
{
    public partial class LoadingViewModel : ObservableObject
    {
        public LoadingViewModel()
        {
            // No auth service needed - handled in App.xaml.cs
        }

        [RelayCommand]
        public async Task CheckAuthenticationAsync()
        {
            try
            {
                var accessToken = await SecureStorage.GetAsync("access_token");
                var refreshToken = await SecureStorage.GetAsync("refresh_token");

                if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
                {
                    var jwtToken = new JwtSecurityToken(accessToken);
                    bool isTokenValid = DateTime.UtcNow < jwtToken.ValidTo;

                    if (isTokenValid)
                    {
                        bool isOnline = NetworkHelper.IsConnected();

                        if (!isOnline)
                        {
                            await Shell.Current.DisplayAlert("⚠️Offline Mode", "You're offline. Changes will be saved locally and synced when you're back online.", "OK");
                        }

                        await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
                        return;
                    }
                }

                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
            catch
            {
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }
        }
    }
}