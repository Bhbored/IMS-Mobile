using IMS_Mobile.MVVM.ViewModels;

namespace IMS_Mobile.MVVM.Views;

public partial class SignUpPage : ContentPage
{
	public SignUpPage(SignUpViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}