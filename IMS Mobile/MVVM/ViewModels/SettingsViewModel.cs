using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using IMS_Mobile.DB;
using IMS_Mobile.MVVM.Models;
using IMS_Mobile.MVVM.Views;
using IMS_Mobile.Converters;
using IMS_Mobile.Popups;
using IMS_Mobile.Service;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;

namespace IMS_Mobile.MVVM.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private readonly UserPreferencesRepository _userPreferencesRepository;
        private readonly SupabaseAuthService _authService;

        private UserPreferences _userPreferences = null!;
        private string _displayName = string.Empty;
        private Theme _selectedTheme;
        private Currency _selectedCurrency;
        private string _avatar = string.Empty;

        public SettingsPage? SettingsPage { get; set; }

        public SettingsViewModel(UserPreferencesRepository userPreferencesRepository, SupabaseAuthService authService)
        {
            _userPreferencesRepository = userPreferencesRepository;
            _authService = authService;

            LoadUserPreferences();

            SaveCommand = new Command(async () => await SavePreferences());
            ResetCommand = new Command(async () => await ResetToDefaults());
            ChangeAvatarCommand = new Command(async () => await ChangeAvatar());
            SelectAvatarCommand = new Command<string>(async (avatarPath) => await SelectAvatar(avatarPath));
        }

        public UserPreferences UserPreferences
        {
            get => _userPreferences;
            set
            {
                _userPreferences = value;
                OnPropertyChanged();
            }
        }

        public string DisplayName
        {
            get => _displayName;
            set
            {
                _displayName = value;
                OnPropertyChanged();
            }
        }

        public Theme SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                if (_selectedTheme != value)
                {
                    _selectedTheme = value;
                    OnPropertyChanged();
                    // Apply theme immediately when changed
                    ApplyTheme(value);
                }
            }
        }

        public Currency SelectedCurrency
        {
            get => _selectedCurrency;
            set
            {
                _selectedCurrency = value;
                OnPropertyChanged();
            }
        }

        public string Avatar
        {
            get => _avatar;
            set
            {
                _avatar = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand ResetCommand { get; }
        public ICommand ChangeAvatarCommand { get; }
        public ICommand SelectAvatarCommand { get; }

        public List<string> AvatarOptions { get; } = new List<string>
        {
            "avatar1.png",
            "avatar2.png",
            "avatar3.png",
            "avatar4.png",
            "avatar5.png",
            "avatar6.png",
            "avatar7.png",
            "avatar8.png",
            "avatar9.png",
            "avatar10.png"
        };

        private async void LoadUserPreferences()
        {
            try
            {
                UserPreferences = _userPreferencesRepository.GetUserPreferences();
                DisplayName = UserPreferences.DisplayName;
                SelectedTheme = UserPreferences.Theme;
                SelectedCurrency = UserPreferences.Currency;
                Avatar = UserPreferences.Avatar;

                // Sync user email from Supabase if not already set
                if (string.IsNullOrEmpty(UserPreferences.UserEmail))
                {
                    await SyncUserEmailFromSupabase();
                }
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(100);
                    var snackbar = Snackbar.Make(
                        message: $"Failed to load preferences: {ex.Message}",
                        duration: TimeSpan.FromSeconds(3),
                        visualOptions: new SnackbarOptions
                        {
                            BackgroundColor = Colors.Red,
                            TextColor = Colors.White,
                            CornerRadius = 10,
                        },
                        anchor: SettingsPage
                    );
                    await snackbar.Show();
                });
            }
        }

        private async Task SavePreferences()
        {
            try
            {
                UserPreferences.DisplayName = DisplayName;
                UserPreferences.Theme = SelectedTheme;
                UserPreferences.Currency = SelectedCurrency;
                UserPreferences.Avatar = Avatar;

                _userPreferencesRepository.SaveUserPreferences(UserPreferences);

                // Update the global currency converter
                GlobalCurrencyConverter.UpdateCurrency(SelectedCurrency);

                // Update flyout header
                if (Application.Current?.Windows[0].Page is AppShell shell)
                {
                    await shell.UpdateFlyoutHeaderAsync();
                }

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(100);
                    var snackbar = Snackbar.Make(
                        message: "Settings saved successfully!",
                        duration: TimeSpan.FromSeconds(2),
                        visualOptions: new SnackbarOptions
                        {
                            BackgroundColor = Colors.LightGreen,
                            TextColor = Colors.White,
                            CornerRadius = 10,
                        },
                        anchor: SettingsPage
                    );
                    await snackbar.Show();
                });
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(100);
                    var snackbar = Snackbar.Make(
                        message: $"Failed to save preferences: {ex.Message}",
                        duration: TimeSpan.FromSeconds(3),
                        visualOptions: new SnackbarOptions
                        {
                            BackgroundColor = Colors.Red,
                            TextColor = Colors.White,
                            CornerRadius = 10,
                        },
                        anchor: SettingsPage
                    );
                    await snackbar.Show();
                });
            }
        }

        private async Task ResetToDefaults()
        {
            try
            {
                DisplayName = "";
                SelectedTheme = Theme.Light;
                SelectedCurrency = Currency.USD;
                Avatar = "";

                UserPreferences.DisplayName = DisplayName;
                UserPreferences.Theme = SelectedTheme;
                UserPreferences.Currency = SelectedCurrency;
                UserPreferences.Avatar = Avatar;

                _userPreferencesRepository.SaveUserPreferences(UserPreferences);

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Task.Delay(100);
                    var snackbar = Snackbar.Make(
                        message: "Settings reset to defaults!",
                        duration: TimeSpan.FromSeconds(2),
                        visualOptions: new SnackbarOptions
                        {
                            BackgroundColor = Colors.LightGreen,
                            TextColor = Colors.White,
                            CornerRadius = 10,
                        },
                        anchor: SettingsPage
                    );
                    await snackbar.Show();
                });
            }
            catch (Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Task.Delay(100);
                    var snackbar = Snackbar.Make(
                        message: $"Failed to reset preferences: {ex.Message}",
                        duration: TimeSpan.FromSeconds(3),
                        visualOptions: new SnackbarOptions
                        {
                            BackgroundColor = Colors.Red,
                            TextColor = Colors.White,
                            CornerRadius = 10,
                        },
                        anchor: SettingsPage
                    );
                    await snackbar.Show();
                });
            }
        }

        private async Task ChangeAvatar()
        {
            try
            {
                var avatarPicker = new AvatarPickerPopup(this);
                await Application.Current!.Windows[0].Page!.ShowPopupAsync(avatarPicker);
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(100);
                    var snackbar = Snackbar.Make(
                        message: $"Failed to change avatar: {ex.Message}",
                        duration: TimeSpan.FromSeconds(3),
                        visualOptions: new SnackbarOptions
                        {
                            BackgroundColor = Colors.Red,
                            TextColor = Colors.White,
                            CornerRadius = 10,
                        },
                        anchor: SettingsPage
                    );
                    await snackbar.Show();
                });
            }
        }

        private async Task SelectAvatar(string avatarPath)
        {
            try
            {
                Avatar = avatarPath;

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Task.Delay(100);
                    var snackbar = Snackbar.Make(
                        message: "Avatar updated successfully!",
                        duration: TimeSpan.FromSeconds(2),
                        visualOptions: new SnackbarOptions
                        {
                            BackgroundColor = Colors.LightGreen,
                            TextColor = Colors.White,
                            CornerRadius = 10,
                        },
                        anchor: SettingsPage
                    );
                    await snackbar.Show();
                });
            }
            catch (Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Task.Delay(100);
                    var snackbar = Snackbar.Make(
                        message: $"Failed to update avatar: {ex.Message}",
                        duration: TimeSpan.FromSeconds(3),
                        visualOptions: new SnackbarOptions
                        {
                            BackgroundColor = Colors.Red,
                            TextColor = Colors.White,
                            CornerRadius = 10,
                        },
                        anchor: SettingsPage
                    );
                    await snackbar.Show();
                });
            }
        }

        private async Task SyncUserEmailFromSupabase()
        {
            try
            {
                var userEmail = await _authService.GetUserEmailAsync();
                if (!string.IsNullOrEmpty(userEmail))
                {
                    UserPreferences.UserEmail = userEmail;
                    _userPreferencesRepository.SaveUserPreferences(UserPreferences);
                }
            }
            catch (Exception)
            {
                // Silently fail - user email sync is not critical
            }
        }

        private void ApplyTheme(Theme theme)
        {
            try
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (Application.Current != null)
                    {
                        Application.Current.UserAppTheme = theme switch
                        {
                            Theme.Light => AppTheme.Light,
                            Theme.Dark => AppTheme.Dark,
                            _ => AppTheme.Light
                        };
                    }
                });
            }
            catch (Exception ex)
            {
                // Log error but don't show to user as it's not critical
                System.Diagnostics.Debug.WriteLine($"Failed to apply theme: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
