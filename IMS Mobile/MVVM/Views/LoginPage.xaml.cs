using IMS_Mobile.MVVM.ViewModels;

namespace IMS_Mobile.MVVM.Views;

public partial class LoginPage : ContentPage
{
	public static LoginPage? Current { get; set; }
	public LoginPage(LoginViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
	public void CloseKeybord()
	{
		PasswordEntry.Unfocus();
	}
}