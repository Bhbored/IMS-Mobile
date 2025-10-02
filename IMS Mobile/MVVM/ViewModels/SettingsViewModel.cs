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
using System.Threading.Tasks;

namespace IMS_Mobile.MVVM.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private readonly UserPreferencesRepository _userPreferencesRepository;
        private readonly SupabaseAuthService _authService;
        private readonly SyncService _syncService;

        private UserPreferences _userPreferences = null!;
        private string _displayName = string.Empty;
        private Theme _selectedTheme;
        private Currency _selectedCurrency;
        private string _avatar = string.Empty;

        public SettingsPage? SettingsPage { get; set; }

        public SettingsViewModel(UserPreferencesRepository userPreferencesRepository, SupabaseAuthService authService, SyncService syncService)
        {
            _userPreferencesRepository = userPreferencesRepository;
            _authService = authService;
            _syncService = syncService;

            LoadUserPreferences();

            SaveCommand = new Command(async () => await SavePreferences());
            ResetCommand = new Command(async () => await ResetToDefaults());
            ChangeAvatarCommand = new Command(async () => await ChangeAvatar());
            SelectAvatarCommand = new Command<string>(async (avatarPath) => await SelectAvatar(avatarPath));
            DeleteUserDataCommand = new Command(async () => await DeleteUserData());
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
                    _ = ApplyTheme(value);
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
        public ICommand DeleteUserDataCommand { get; }

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
                GlobalCurrencyConverter.UpdateCurrency(SelectedCurrency);
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
            }
        }

        private Task ApplyTheme(Theme theme)
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
            return Task.CompletedTask;
        }

        private async Task DeleteUserData()
        {
            try
            {
                bool confirmed = await Application.Current!.Windows[0].Page!.DisplayAlert(
                    "Delete All Data",
                    "Are you sure you want to delete all your data? This action cannot be undone.\n\nThis will delete:\n• All contacts\n• All products\n• All transactions\n• All transaction history\n\nYour account will remain active, but all data will be permanently removed.",
                    "Delete All Data",
                    "Cancel");

                if (!confirmed)
                    return;

                bool finalConfirmed = await Application.Current.Windows[0].Page!.DisplayAlert(
                    "Final Confirmation",
                    "This is your final warning. All your data will be permanently deleted and cannot be recovered. Are you absolutely sure?",
                    "Yes, Delete Everything",
                    "Cancel");

                if (!finalConfirmed)
                    return;

                var loadingSnackbar = Snackbar.Make(
                    message: "Deleting all data...",
                    duration: TimeSpan.FromSeconds(10),
                    visualOptions: new SnackbarOptions
                    {
                        BackgroundColor = Colors.Orange,
                        TextColor = Colors.White,
                        CornerRadius = 10,
                    },
                    anchor: SettingsPage
                );
                await loadingSnackbar.Show();

                await _syncService.ClearLocalData();
                await _syncService.ManualSyncToSupabase();

                _ = loadingSnackbar.Dismiss();

                DisplayName = "";
                SelectedTheme = Theme.Light;
                SelectedCurrency = Currency.USD;
                Avatar = "";

                var successSnackbar = Snackbar.Make(
                    message: "All data has been successfully deleted!",
                    duration: TimeSpan.FromSeconds(3),
                    visualOptions: new SnackbarOptions
                    {
                        BackgroundColor = Colors.Green,
                        TextColor = Colors.White,
                        CornerRadius = 10,
                    },
                    anchor: SettingsPage
                );
                await successSnackbar.Show();
            }
            catch (Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    var errorSnackbar = Snackbar.Make(
                        message: $"An error occurred: {ex.Message}",
                        duration: TimeSpan.FromSeconds(3),
                        visualOptions: new SnackbarOptions
                        {
                            BackgroundColor = Colors.Red,
                            TextColor = Colors.White,
                            CornerRadius = 10,
                        },
                        anchor: SettingsPage
                    );
                    await errorSnackbar.Show();
                });
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
