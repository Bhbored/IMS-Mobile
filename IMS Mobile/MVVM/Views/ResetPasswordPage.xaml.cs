using IMS_Mobile.Service;
using Microsoft.Maui.Controls;
using Supabase.Gotrue;
using static Supabase.Gotrue.Constants;

namespace IMS_Mobile.MVVM.Views;

[QueryProperty(nameof(Token), "token")]
[QueryProperty(nameof(Email), "email")]
[QueryProperty(nameof(Type), "type")]
public partial class ResetPasswordPage : ContentPage
{
    private readonly SupabaseAuthService _authService;

    public string Token { get; set; } = "";
    public string Email { get; set; } = "";
    public string Type { get; set; } = "";

    public ResetPasswordPage(SupabaseAuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (Type != "recovery" || string.IsNullOrWhiteSpace(Token) || string.IsNullOrWhiteSpace(Email))
        {
            await DisplayAlert("Error", "Invalid reset link.", "OK");
            return;
        }

        try
        {
            var sb = _authService.GetClient();

            if (sb.Auth.CurrentUser == null)
            {
                try
                {
                    await sb.Auth.VerifyOTP(Email, Token, EmailOtpType.Recovery);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"Failed to verify link: {ex.Message}", "OK");
                    return;
                }
            }

            ResetBtn.IsEnabled = true;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Unexpected error: {ex.Message}", "OK");
        }
    }


    private async void OnResetPasswordClicked(object sender, EventArgs e)
    {
        var newPw = NewPasswordEntry.Text?.Trim() ?? "";
        var confirm = ConfirmPasswordEntry.Text?.Trim() ?? "";

        if (string.IsNullOrEmpty(newPw))
        {
            await DisplayAlert("Error", "Enter a new password.", "OK");
            return;
        }

        if (newPw != confirm)
        {
            await DisplayAlert("Error", "Passwords do not match.", "OK");
            return;
        }

        try
        {
            var sb = _authService.GetClient();
            await sb.Auth.Update(new UserAttributes { Password = newPw });
            await DisplayAlert("Success", "Password updated.", "OK");
            await Shell.Current.GoToAsync("//LoginPage");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to reset password: {ex.Message}", "OK");
        }
    }
}
