using IMS_Mobile.MVVM.ViewModels;

namespace IMS_Mobile.MVVM.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}