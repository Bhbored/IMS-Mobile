

using IMS_Mobile.MVVM.ViewModels;

namespace IMS_Mobile.MVVM.Views;

public partial class HomePage : ContentPage
{
	public HomePage(HomeVM vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }
   
}