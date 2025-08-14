using IMS_Mobile.MVVM.Models;
using IMS_Mobile.MVVM.ViewModels;
using Syncfusion.Maui.Core.Hosting;
namespace IMS_Mobile.MVVM.Views;

public partial class SellProducts : ContentPage
{
	public SellProducts(InventoryVM vm)
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
        if (BindingContext is InventoryVM vm)
        {
            vm.FilterProducts();
        }
    }

    private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        var checkBox = sender as CheckBox;
        var product = checkBox.BindingContext as Product;
        var vm = BindingContext as InventoryVM;

        if (vm != null && product != null)
        {
            vm.UpdateCart(product, e.Value); 
        }
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        bottomSheet.Show();
        var vm = BindingContext as InventoryVM;
        vm.FinalizeCart();
    }
}