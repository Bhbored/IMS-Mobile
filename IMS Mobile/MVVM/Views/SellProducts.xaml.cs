using IMS_Mobile.MVVM.ViewModels;

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
}