using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IMS_Mobile.MVVM.Views;
using IMS_Mobile.Service;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Maui.Storage;
using System.Diagnostics;

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
                    bool isOnline = NetworkHelper.IsConnected();

                    if (isTokenValid)
                    {
                        if (isOnline)
                        {
                            try
                            {
                                bool isAuthenticated = await App.AuthService.InitializeAsync();
                                if (isAuthenticated)
                                {
                                    await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
                                    return;
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"AuthService init failed: {ex.Message}");
                            }
                        }
                        else
                        {
                            App.AuthService.HydrateOfflineSession(accessToken, refreshToken);
                            if (App.AuthService.IsUserAuthenticated)
                            {
                                await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
                                return;
                            }
                        }
                    }
                    else if (isOnline)
                    {
                        try
                        {
                            bool isAuthenticated = await App.AuthService.InitializeAsync();
                            if (isAuthenticated)
                            {
                                await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"AuthService init failed: {ex.Message}");
                        }
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