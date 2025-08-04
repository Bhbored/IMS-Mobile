using CommunityToolkit.Maui.Views;
using IMS_Mobile.MVVM.Models;
using IMS_Mobile.MVVM.ViewModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace IMS_Mobile.Popups;

public partial class AddProductPopup : Popup, INotifyPropertyChanged
{
    private Product newProduct = new Product();

    public AddProductPopup(InventoryVM vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
    public Product NewProduct
    {
        get => newProduct;
        set
        {
            newProduct = value;
            OnPropertyChanged();
        }
    }
    private void CloseButton_Clicked(object sender, EventArgs e)
    {
        CloseAsync();
    }

    private void CancelButton_Clicked(object sender, EventArgs e)
    {
        CloseAsync();
    }

    private void AddProductButton_Clicked(object sender, EventArgs e)
    {
        if (BindingContext is InventoryVM vm)
        {
            NewProduct = new Product();
            NewProduct.Name = ProductNameEntry.Text;
            NewProduct.Price = double.TryParse(PriceEntry.Text, out var price) ? price : 0;
            NewProduct.Cost = double.TryParse(CostEntry.Text, out var cost) ? cost : 0;
            NewProduct.stock = int.TryParse(StockEntry.Text, out var quantity) ? quantity : 0;
            OnPropertyChanged(nameof(NewProduct));
            vm.AddProduct(NewProduct);
        }
        CloseAsync();
    }
    #region INotifyPropertyChanged Implementation
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}