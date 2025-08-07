using CommunityToolkit.Maui.Views;
using IMS_Mobile.MVVM.Models;
using IMS_Mobile.MVVM.ViewModels;
using System.ComponentModel;

namespace IMS_Mobile.Popups;

public partial class EditProdutcPopup : Popup, INotifyPropertyChanged
{
	private Product EditedProduct = new Product();
    public EditProdutcPopup(InventoryVM vm, Product product)
	{
		InitializeComponent();
		BindingContext = vm;
        EditedProduct = product;
        FillProductDetails(EditedProduct);
    }

	private void CancelButton_Clicked(object sender, EventArgs e)
	{
		CloseAsync();
    }

    public void FillProductDetails(Product product)
    {
        if (product != null)
        {
            ProductNameEntry.Text = product.Name;
            PriceEntry.Text = product.Price.ToString();
            CostEntry.Text = product.Cost.ToString();
            StockEntry.Text = product.stock.ToString();

        }
    }
    private void EditProductButton_Clicked(object sender, EventArgs e)
    {

        if(EditedProduct !=null)
        {
            EditedProduct.Name = ProductNameEntry.Text;
            EditedProduct.Price = double.TryParse(PriceEntry.Text, out double price) ? price : 0;
            EditedProduct.Cost = double.TryParse(CostEntry.Text, out double cost) ? cost : 0;
            EditedProduct.stock = int.TryParse(StockEntry.Text, out int stock) ? stock : 0;
            if(BindingContext is InventoryVM vm)
            {
               vm.EditProduct(EditedProduct);
            }
           
            CloseAsync();
        }


    }

}