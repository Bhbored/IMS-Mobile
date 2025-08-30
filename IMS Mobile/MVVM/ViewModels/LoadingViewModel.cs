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