using IMS_Mobile.Service;
using Supabase.Gotrue;

namespace IMS_Mobile.MVVM.Views;

public partial class ResetPasswordPage : ContentPage
{
    private string _resetToken;
    private readonly SupabaseAuthService _authService;

    public ResetPasswordPage(SupabaseAuthService authService) 
    {
        InitializeComponent();
        _authService = authService;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Get token from navigation parameters
        var uri = Shell.Current.CurrentState.Location;
        if (!string.IsNullOrEmpty(uri.Query))
        {
            var query = uri.Query.TrimStart('?');
            var queryParams = System.Web.HttpUtility.ParseQueryString(query);
            _resetToken = queryParams["token"] ?? string.Empty;
        }
    }

    private async void OnResetPasswordClicked(object sender, EventArgs e)
    {
        var newPassword = NewPasswordEntry.Text;
        var confirmPassword = ConfirmPasswordEntry.Text;

        if (string.IsNullOrEmpty(_resetToken))
        {
            await DisplayAlert("Error", "Invalid reset link", "OK");
            return;
        }

        if (string.IsNullOrEmpty(newPassword))
        {
            await DisplayAlert("Error", "Please enter a new password", "OK");
            return;
        }

        if (newPassword != confirmPassword)
        {
            await DisplayAlert("Error", "Passwords do not match", "OK");
            return;
        }

        try
        {
            // Use injected service
            var supabase = _authService.GetClient();

            // Update password - correct method name
            await supabase.Auth.Update(new UserAttributes
            {
                Password = newPassword
            });

            await DisplayAlert("Success", "Password updated successfully!", "OK");
            await Shell.Current.GoToAsync($"//LoginPage");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to reset password: {ex.Message}", "OK");
        }
    }
}