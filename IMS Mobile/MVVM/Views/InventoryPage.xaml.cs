using CommunityToolkit.Maui.Alerts;
using IMS_Mobile.MVVM.Models;
using IMS_Mobile.MVVM.ViewModels;
using System.Collections.ObjectModel;

namespace IMS_Mobile.MVVM.Views;

public partial class InventoryPage : ContentPage
{
	public InventoryPage(InventoryVM vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
	protected override void OnAppearing()
	{
		base.OnAppearing();
		if (BindingContext is InventoryVM vm)
		{
			vm.LoadDB();
		}
    }

    private void autocomplete_SelectionChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
    {
		if(BindingContext is InventoryVM vm)
		{
			vm.FilterProducts();
        }
    }
    public void showSnackBar(Product product)
    {
        if (BindingContext is InventoryVM vm)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Snackbar.Make(
                    $"{product.Name} updated successfully",
                    () => vm.UndoEdit(product),
                    "UNDO",
                    TimeSpan.FromSeconds(3))
                .Show();
            });
        }
    }
}