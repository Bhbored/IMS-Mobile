using IMS_Mobile.MVVM.Models;
using IMS_Mobile.MVVM.ViewModels;
using Microsoft.Maui.Controls;
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
    private void CloseBottomSheet(object sender, EventArgs e)
    {
        var vm = BindingContext as InventoryVM;
        vm.ClearCart();
        bottomSheet.Close();
    }

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        var entry = sender as Entry;
        var product = entry.BindingContext as Product;
        var vm = BindingContext as InventoryVM;

        if (product != null && !string.IsNullOrEmpty(e.NewTextValue))
        {
            if (string.IsNullOrWhiteSpace(e.NewTextValue))
                return;

            if (int.TryParse(e.NewTextValue, out int newQuantity))
            {
                if (newQuantity > product.stock)
                {
                    entry.Text = product.stock.ToString();
                    entry.CursorPosition = entry.Text.Length;
                    product.Quantity = product.stock;
                }
                else if (newQuantity < 1)
                {
                    entry.Text = "1";
                    entry.CursorPosition = entry.Text.Length;
                    product.Quantity = 1;
                }
                else
                {
                    product.Quantity = newQuantity;
                }

                vm?.UpdateCartTotals();
            }
            else
            {
                entry.Text = product.Quantity.ToString();
                entry.CursorPosition = entry.Text.Length;
            }
        }
    }

    private void SellCash(object sender, EventArgs e)
    {
        var  vm = BindingContext as InventoryVM;
        if (vm != null)
        {
            vm.SellCash();
            bottomSheet.Close();
        }
    }
}