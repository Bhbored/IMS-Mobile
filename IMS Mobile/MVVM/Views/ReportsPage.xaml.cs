using IMS_Mobile.MVVM.ViewModels;

namespace IMS_Mobile.MVVM.Views;

public partial class ReportsPage : ContentPage
{
	public ReportsPage()
	{
		InitializeComponent();
		BindingContext = new ReportsVM();
	}
}