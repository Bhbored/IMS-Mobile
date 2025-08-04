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
}