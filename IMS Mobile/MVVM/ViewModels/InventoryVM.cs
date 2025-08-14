using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using IMS_Mobile.MVVM.Views;
using IMS_Mobile.Popups;
using IMS_Mobile.Service;
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


        #region fields
        private bool _isRefreshing = false;
        private ObservableCollection<Product> filteredProducts = new();
        private ObservableCollection<Product> cartItems = new ObservableCollection<Product>();
        public int cartitems = 0;
        public double cartvalue = 0.0;
        private ObservableCollection<Product> finalCartItems = new ObservableCollection<Product>();
        private static SellProducts? sellProductsPage;


        #endregion

        #region Properties

        public int CartItemsCount
        {
            get => cartitems;
            set
            {
                cartitems = value;
                OnPropertyChanged();
            }
        }
        public double CartValue
        {
            get => cartvalue;
            set
            {
                cartvalue = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Product> CartItems
        {
            get => cartItems;
            set
            {
                cartItems = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Product> FinalCartItems
        {
            get => finalCartItems;
            set
            {
                finalCartItems = value;
                OnPropertyChanged();
            }
        }
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
        public static SellProducts SellProductsPage
        {
            get => sellProductsPage;
            set
            {
                sellProductsPage = value;
            }
        }
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
        public async void AddProduct(Product product)
        {
            try
            {
                if (product != null && !Products.Any(p => p.Name == product.Name))
                {
                    App.ProductRepository.InsertItem(product);
                    await LoadDB();
                    await ModernToastService.ShowSuccess($"{product.Name} added successfully");
                }
                else
                {
                    await ModernToastService.ShowError("Product already exists or is null");
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
                    await ModernToastService.ShowError("Product does not exist or is null");

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Edit product error: {ex.Message}");
                await ModernToastService.ShowInfo("Error updating product");

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
                        await ModernToastService.ShowSuccess($"{ClonedProduct.Name} edit undone");
                        OnPropertyChanged(nameof(FilteredProducts));
                    });


                    ClonedProduct = new Product();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Undo error: {ex.Message}");
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await ModernToastService.ShowSuccess("Failed to undo edit");
                    });
                }
            }
            else
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await ModernToastService.ShowError("No previous state to undo");
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
                        await ModernToastService.ShowSuccess($"{ClonedProduct.Name} Delete undone");
                        OnPropertyChanged(nameof(FilteredProducts));
                    });

                    ClonedProduct = new Product();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Undo error: {ex.Message}");
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await ModernToastService.ShowError("Failed to undo Delete");
                    });
                }
            }
            else
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await ModernToastService.ShowInfo("No previous state to undo");
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
                    await ModernToastService.ShowError("Product does not exist or is null");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Delete product error: {ex.Message}");
                await ModernToastService.ShowInfo("Error deleting product");
            }
        }
        public void UpdateCart(Product product, bool isChecked)
        {
            if (isChecked && !CartItems.Any(p => p.Id == product.Id))
            {
                CartItems.Add(product);

            }
            else
            {
                var itemToRemove = CartItems.FirstOrDefault(p => p.Id == product.Id);
                if (itemToRemove != null && CartItems.Any(p => p.Id == product.Id))
                {
                    CartItems.Remove(itemToRemove);
                }
            }

            CartItemsCount = CartItems.Count;
            CartValue = CartItems.Sum(p => p.Price);

            OnPropertyChanged(nameof(CartItemsCount));
            OnPropertyChanged(nameof(CartValue));
            OnPropertyChanged(nameof(CartItems));
        }
        public void FinalizeCart()
        {
            if (CartItems.Count > 0)
            {
                FinalCartItems.Clear();
                foreach (var item in CartItems)
                {
                    FinalCartItems.Add(item);
                }

            }

        }
        public void ClearCart()
        {
            if (FinalCartItems.Count > 0)
            {
               
                CartItems.Clear();
                finalCartItems.Clear();
                foreach (Product c in FilteredProducts)
                {
                    c.IsChecked = false;
                    c.Quantity = 1;
                }
                CartItemsCount = 0;
                CartValue = 0.00;
                OnPropertyChanged(nameof(FilteredProducts));
                OnPropertyChanged(nameof(CartItemsCount));
                OnPropertyChanged(nameof(CartValue));
                OnPropertyChanged(nameof(CartItems));
            }
        }
        public void RemoveFromCart(Product product)
        {
            if (product != null)
            {
                var itemToRemove = FinalCartItems.FirstOrDefault(p => p.Id == product.Id);
                if (itemToRemove != null)
                {
                    CartItems.Remove(itemToRemove);
                    FinalCartItems.Remove(itemToRemove);
                    OnPropertyChanged(nameof(CartItemsCount));
                    OnPropertyChanged(nameof(FinalCartItems));
                    CartItemsCount = CartItems.Count;
                    CartValue = CartItems.Sum(p => p.Price);
                    OnPropertyChanged(nameof(CartValue));
                    OnPropertyChanged(nameof(CartItems));
                    var productToUpdate = FilteredProducts.FirstOrDefault(p => p.Id == itemToRemove.Id);
                    productToUpdate.IsChecked = false;
                    OnPropertyChanged(nameof(FilterProducts));
                }
            }
        }
        private void IncreaseQuantity(Product product)
        {
            if (product != null && product.Quantity < product.stock)
            {
                product.Quantity++;
                UpdateCartTotals();
            }
        }

        private void DecreaseQuantity(Product product)
        {
            if (product != null && product.Quantity >= 1)
            {
                product.Quantity--;
                UpdateCartTotals();
            }
            else if (product != null && product.Quantity > product.stock)
            {
                product.Quantity = product.stock;
                product.Quantity--;
                OnPropertyChanged(nameof(product.Quantity));
                OnPropertyChanged(nameof(FinalCartItems));
                UpdateCartTotals();
            }
        }

        public void UpdateCartTotals()
        {

            CartItemsCount = FinalCartItems.Count;
            CartValue = FinalCartItems.Sum(p => p.Price * p.Quantity);
            OnPropertyChanged(nameof(CartItemsCount));
            OnPropertyChanged(nameof(CartValue));
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
        public ICommand RemoveFromCartCommand => new Command<Product>((product) =>
        {
            RemoveFromCart(product);
        });

        public ICommand IncreaseQuantityCommand =>new Command<Product>((product) =>
        {
            IncreaseQuantity(product);
        });
        public ICommand DecreaseQuantityCommand=> new Command<Product>((product) =>
        {
            DecreaseQuantity(product);
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