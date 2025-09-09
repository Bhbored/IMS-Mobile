using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using IMS_Mobile.DB;
using IMS_Mobile.MVVM.Models;
using IMS_Mobile.MVVM.Views;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace IMS_Mobile.MVVM.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private readonly UserPreferencesRepository _userPreferencesRepository;

        private UserPreferences _userPreferences;
        private string _displayName;
        private Theme _selectedTheme;
        private Currency _selectedCurrency;
        private string _avatar;

        public SettingsPage? SettingsPage { get; set; }

        public SettingsViewModel(UserPreferencesRepository userPreferencesRepository)
        {
            _userPreferencesRepository = userPreferencesRepository;

            LoadUserPreferences();

            SaveCommand = new Command(async () => await SavePreferences());
            ResetCommand = new Command(async () => await ResetToDefaults());
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
                _selectedTheme = value;
                OnPropertyChanged();
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

        private void LoadUserPreferences()
        {
            try
            {
                UserPreferences = _userPreferencesRepository.GetUserPreferences();
                DisplayName = UserPreferences.DisplayName;
                SelectedTheme = UserPreferences.Theme;
                SelectedCurrency = UserPreferences.Currency;
                Avatar = UserPreferences.Avatar;
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

                MainThread.BeginInvokeOnMainThread(async () =>
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
                MainThread.BeginInvokeOnMainThread(async () =>
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

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
