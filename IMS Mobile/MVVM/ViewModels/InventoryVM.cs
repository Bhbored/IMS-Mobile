using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using IMS_Mobile.Popups;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Product = IMS_Mobile.MVVM.Models.Product;

namespace IMS_Mobile.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class InventoryVM : INotifyPropertyChanged
    {
        private ObservableCollection<Product> filteredProducts = new();

        #region fields
        #endregion

        #region Properties
        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();
        public ObservableCollection<Product> FilteredProducts
        {
            get
            {
                return filteredProducts;
            }

            set
            {
                filteredProducts = value;
                OnPropertyChanged();
            }
        }
        public IList<object> SelectedProducts { get; set; } = [];
        #endregion

        #region Methods
        public void FilterProducts()
        {
            if (SelectedProducts.Count == 0)
            {
                FilteredProducts = new ObservableCollection<Product>(Products);
            }
            else
            {
                var filteredList = new List<Product>();

                foreach (var selectedItem in SelectedProducts)
                {
                    if (selectedItem is Product selectedProduct)
                    {
                        var matchingProduct = Products.FirstOrDefault(p => p.Name == selectedProduct.Name);
                        if (matchingProduct != null)
                        {
                            filteredList.Add(matchingProduct);
                        }
                    }
                }

                FilteredProducts = new ObservableCollection<Product>(filteredList);
            }
        }
        public void AddProduct(Product product)
        {
            try
            {
                if (product != null && !Products.Any(p => p.Name == product.Name))
                {
                   App.ProductRepository.InsertItem(product);
                    LoadDB();
                    Toast.Make($"{product.Name} added successfully",duration: ToastDuration.Short).Show();
                }
                else
                {
                    Toast.Make("Product already exists or is null", duration: ToastDuration.Short).Show();
                }
            }
            catch (Exception ex)
            {
                
                Debug.WriteLine($"Error adding product: {ex.Message}");
            }

        }
        #endregion

        #region commands
        public ICommand NewProductPop => new Command(async() => { 
        
        await AppShell.Current.ShowPopupAsync(new AddProductPopup(this));
        });

        #endregion

        #region Tasks

        public Task LoadDB()
        {
            var dbProdutcs = App.ProductRepository.GetItems();
            Products.Clear();
            FilteredProducts.Clear();
            foreach (var item in dbProdutcs)
            {
                Products.Add(item);
            }
            FilteredProducts = new ObservableCollection<Product>(Products);
            return Task.CompletedTask;
        }
        #endregion
        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}