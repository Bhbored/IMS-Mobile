using CommunityToolkit.Maui.Extensions;
using IMS_Mobile.MVVM.Views;
using IMS_Mobile.Popups;
using IMS_Mobile.Service;
using IMS_Mobile.DB;
using IMS_Mobile.MVVM.Models;

namespace IMS_Mobile
{
    public partial class AppShell : Shell
    {
        private readonly SupabaseAuthService _authService;
        private readonly SyncService _syncService;

        public AppShell(SupabaseAuthService authService, SyncService syncService)
        {
            InitializeComponent();
            Routing.RegisterRoute("HomePage", typeof(HomePage));
            Routing.RegisterRoute("ContactsPage", typeof(ContactsPage));
            Routing.RegisterRoute("InventoryPage", typeof(InventoryPage));
            Routing.RegisterRoute("ReportsPage", typeof(ReportsPage));
            Routing.RegisterRoute("ContactDetailsPage", typeof(ContactDetailsPage));
            Routing.RegisterRoute(nameof(LoadingPage), typeof(LoadingPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(SignUpPage), typeof(SignUpPage));
            Routing.RegisterRoute(nameof(ResetPasswordPage), typeof(ResetPasswordPage));
            Routing.RegisterRoute(nameof(Analytics), typeof(Analytics));
            Routing.RegisterRoute(nameof(GeneratedReport), typeof(GeneratedReport));
            _authService = authService;
            _syncService = syncService;      
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await UpdateFlyoutHeaderAsync();
        }

        private void MenuItem_Clicked(object sender, EventArgs e)
        {
            Shell.Current.ShowPopupAsync(new LogoutConfirmationPopup(_authService, _syncService));
        }

        public async Task UpdateFlyoutHeaderAsync()
        {
            try
            {
                var userPreferences = App.UserPreferencesRepository?.GetUserPreferences();
                var userEmail = await _authService.GetUserEmailAsync();

                if (userPreferences != null && !string.IsNullOrEmpty(userPreferences.Avatar))
                {
                    userAvatar.Source = userPreferences.Avatar;
                }
                else
                {
                    userAvatar.Source = "user.png";
                }

                if (userPreferences != null && !string.IsNullOrEmpty(userPreferences.DisplayName))
                {
                    userNameLabel.Text = userPreferences.DisplayName;
                }
                else if (!string.IsNullOrEmpty(userEmail))
                {
                    userNameLabel.Text = userEmail.Split('@')[0]; 
                }
                else
                {
                    userNameLabel.Text = "User";
                }

                if (userPreferences != null && !string.IsNullOrEmpty(userPreferences.UserEmail))
                {
                    userEmailLabel.Text = userPreferences.UserEmail;
                }
                else if (!string.IsNullOrEmpty(userEmail))
                {
                    userEmailLabel.Text = userEmail;
                }
                else
                {
                    userEmailLabel.Text = "user@example.com";
                }
            }
            catch (Exception ex)
            {
                userAvatar.Source = "user.png";
                userNameLabel.Text = "User";
                userEmailLabel.Text = "user@example.com";
            }
        }

    }
}
