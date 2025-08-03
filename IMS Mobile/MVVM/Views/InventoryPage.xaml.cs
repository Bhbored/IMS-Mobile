using IMS_Mobile.MVVM.ViewModels;

namespace IMS_Mobile.MVVM.Views;

public partial class InventoryPage : ContentPage
{
	public InventoryPage()
	{
		InitializeComponent();
		BindingContext = new InventoryVM();
	}
}