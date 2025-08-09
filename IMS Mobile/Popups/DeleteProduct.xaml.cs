using CommunityToolkit.Maui.Views;
using IMS_Mobile.MVVM.Models;
using IMS_Mobile.MVVM.ViewModels;

namespace IMS_Mobile.Popups;

public partial class DeleteProduct : Popup
{
	public Product Product { get; set; }= new Product();
    public DeleteProduct(InventoryVM vm, Product product)
	{
		InitializeComponent();
		BindingContext = vm;
		Product = product;
    }
	private void DeleteButton_Clicked(object sender, EventArgs e)
	{
		if (BindingContext is InventoryVM vm)
		{
			vm.DeleteProduct(Product);
			CloseAsync();
		}
    }
	private void CancelButton_Clicked(object sender, EventArgs e)
	{
		CloseAsync();
    }

}