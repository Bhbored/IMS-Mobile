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
                // Get user preferences from local database
                var userPreferences = App.UserPreferencesRepository?.GetUserPreferences();
                System.Diagnostics.Debug.WriteLine($"UserPreferences: {userPreferences?.DisplayName}, {userPreferences?.UserEmail}, {userPreferences?.Avatar}");

                // Get current user email from Supabase auth
                var userEmail = await _authService.GetUserEmailAsync();
                System.Diagnostics.Debug.WriteLine($"UserEmail from Supabase: {userEmail}");

                // Update avatar
                if (userPreferences != null && !string.IsNullOrEmpty(userPreferences.Avatar))
                {
                    userAvatar.Source = userPreferences.Avatar;
                    System.Diagnostics.Debug.WriteLine($"Setting avatar to: {userPreferences.Avatar}");
                }
                else
                {
                    userAvatar.Source = "user.png"; // Default avatar
                    System.Diagnostics.Debug.WriteLine("Setting avatar to default: user.png");
                }

                // Update display name (prefer user preferences, fallback to email prefix)
                if (userPreferences != null && !string.IsNullOrEmpty(userPreferences.DisplayName))
                {
                    userNameLabel.Text = userPreferences.DisplayName;
                    System.Diagnostics.Debug.WriteLine($"Setting name to: {userPreferences.DisplayName}");
                }
                else if (!string.IsNullOrEmpty(userEmail))
                {
                    userNameLabel.Text = userEmail.Split('@')[0]; // Use email prefix as name
                    System.Diagnostics.Debug.WriteLine($"Setting name to email prefix: {userEmail.Split('@')[0]}");
                }
                else
                {
                    userNameLabel.Text = "User";
                    System.Diagnostics.Debug.WriteLine("Setting name to default: User");
                }

                // Update email (prefer user preferences, fallback to Supabase user)
                if (userPreferences != null && !string.IsNullOrEmpty(userPreferences.UserEmail))
                {
                    userEmailLabel.Text = userPreferences.UserEmail;
                    System.Diagnostics.Debug.WriteLine($"Setting email to: {userPreferences.UserEmail}");
                }
                else if (!string.IsNullOrEmpty(userEmail))
                {
                    userEmailLabel.Text = userEmail;
                    System.Diagnostics.Debug.WriteLine($"Setting email to Supabase: {userEmail}");
                }
                else
                {
                    userEmailLabel.Text = "user@example.com";
                    System.Diagnostics.Debug.WriteLine("Setting email to default: user@example.com");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateFlyoutHeaderAsync error: {ex.Message}");
                // Fallback to default values if anything fails
                userAvatar.Source = "user.png";
                userNameLabel.Text = "User";
                userEmailLabel.Text = "user@example.com";
            }
        }

        //await Shell.Current.GoToAsync("//products");
    }
}
