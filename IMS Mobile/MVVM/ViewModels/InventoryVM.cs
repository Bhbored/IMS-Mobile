using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using IMS_Mobile.MVVM.Views;
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
        private bool _isRefreshing = false;
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
        public InventoryPage? InventoryPageInstance { get; set; }
        public Product ClonedProduct { get; set; } = new Product();
        public IList<object> SelectedProducts { get; set; } = [];

        public InventoryPage InventoryPage
        {
            get => InventoryPageInstance;
            set
            {
                InventoryPageInstance = value;
                OnPropertyChanged();
            }
        }
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }
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
                    Toast.Make($"{product.Name} added successfully", duration: ToastDuration.Short).Show();
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
        public async void EditProduct(Product product)
        {
            try
            {
                if (product != null && Products.Any(p => p.Id == product.Id))
                {
                    var productToClone = App.ProductRepository.GetItems().FirstOrDefault(p => p.Id == product.Id);
                    ClonedProduct = CloneProduct(productToClone);
                    App.ProductRepository.UpdateItem(product);
                    await LoadDB();
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await Task.Delay(100);
                        var snackbar = Snackbar.Make(
                            message: $"Edited {product.Name} Successfully",
                            action: () => UndoEdit(ClonedProduct),
                            actionButtonText: "UNDO",
                            duration: TimeSpan.FromSeconds(3),
                            visualOptions: new CommunityToolkit.Maui.Core.SnackbarOptions
                            {
                                BackgroundColor = Colors.Green,
                                TextColor = Colors.White
                            },
                            anchor: InventoryPageInstance
                        );
                        await snackbar.Show();
                    });
                }
                else
                {
                    await Toast.Make("Product does not exist or is null", duration: ToastDuration.Short).Show();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Edit product error: {ex.Message}");
                await Toast.Make("Error updating product", duration: ToastDuration.Short).Show();
            }
        }

        private Product CloneProduct(Product product)
        {

            ClonedProduct = new Product
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Cost = product.Cost,
                stock = product.stock
            };
            return ClonedProduct;
        }

        public async void UndoEdit(Product editedProduct)
        {
            if (ClonedProduct != null)
            {
                try
                {

                    App.ProductRepository.DeleteItem(editedProduct);
                    App.ProductRepository.InsertItem(ClonedProduct);

                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await LoadDB();
                        await Toast.Make($"{ClonedProduct.Name} edit undone", duration: ToastDuration.Short).Show();
                        OnPropertyChanged(nameof(FilteredProducts));
                    });


                    ClonedProduct = new Product();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Undo error: {ex.Message}");
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await Toast.Make("Failed to undo edit", duration: ToastDuration.Short).Show();
                    });
                }
            }
            else
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Toast.Make("No previous state to undo", duration: ToastDuration.Short).Show();
                });
            }
        }

        public async void UndoDelete(Product editedProduct)
        {
            if (ClonedProduct != null)
            {
                try
                {
                    App.ProductRepository.InsertItem(ClonedProduct);

                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await LoadDB();
                        await Toast.Make($"{ClonedProduct.Name} Delete undone", duration: ToastDuration.Short).Show();
                        OnPropertyChanged(nameof(FilteredProducts));
                    });

                    ClonedProduct = new Product();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Undo error: {ex.Message}");
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await Toast.Make("Failed to undo Delete", duration: ToastDuration.Short).Show();
                    });
                }
            }
            else
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Toast.Make("No previous state to undo", duration: ToastDuration.Short).Show();
                });
            }
        }
        public async void DeleteProduct(Product product)
        {
            try
            {
                if (product != null && Products.Any(p => p.Id == product.Id))
                {
                    var productToClone = App.ProductRepository.GetItems().FirstOrDefault(p => p.Id == product.Id);
                    ClonedProduct = CloneProduct(productToClone);
                    App.ProductRepository.DeleteItem(product);
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await LoadDB();
                        OnPropertyChanged(nameof(FilteredProducts));
                        await Task.Delay(100);
                        var snackbar = Snackbar.Make(
                            message: $"Edited {product.Name} Successfully",
                            action: () => UndoDelete(ClonedProduct),
                            actionButtonText: "UNDO",
                            duration: TimeSpan.FromSeconds(3),
                            visualOptions: new CommunityToolkit.Maui.Core.SnackbarOptions
                            {
                                BackgroundColor = Colors.Green,
                                TextColor = Colors.White
                            },
                            anchor: InventoryPageInstance
                        );
                        await snackbar.Show();
                    });
                }
                else
                {
                    await Toast.Make("Product does not exist or is null", duration: ToastDuration.Short).Show();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Delete product error: {ex.Message}");
                await Toast.Make("Error deleting product", duration: ToastDuration.Short).Show();
            }
        }

        #endregion

        #region commands
        public ICommand NewProductPop => new Command(async () =>
        {

            await AppShell.Current.ShowPopupAsync(new AddProductPopup(this));
        });
        public ICommand EditProductPop => new Command<Product>(async (Product) =>
        {
            await AppShell.Current.ShowPopupAsync(new EditProdutcPopup(this, Product));
        });

        public ICommand RefreshContactsCommand => new Command(async () => await RefreshProducts());

        public ICommand DeleteProductPop => new Command<Product>(async (Product) =>
        {
            await AppShell.Current.ShowPopupAsync(new DeleteProduct(this, Product));
        });


        #endregion

        #region Tasks
        private async Task RefreshProducts()
        {
            try
            {
                IsRefreshing = true;
                await Task.Delay(1000);
                await LoadDB();
                IsRefreshing = false;

            }
            catch (Exception ex)
            {
                IsRefreshing = false;
            }
        }

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