using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using IMS_Mobile.MVVM.Models;
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
using Contact = IMS_Mobile.MVVM.Models.Contact;
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
        private bool isBuy = false;


        #endregion

        #region Properties
        public bool IsBuy
        {
            get => isBuy;
            set
            {
                isBuy = value;
                OnPropertyChanged();
            }
        }
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
                FilteredProducts.Clear();
                foreach (var p in Products)
                    FilteredProducts.Add(p);
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

                FilteredProducts.Clear();
                foreach (var p in filteredList.DistinctBy(x => x.Id))
                    FilteredProducts.Add(p);
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
            if (IsBuy == true)
            {
                CartValue = CartItems.Sum(p => p.Cost);
            }
            else
            {
                CartValue = CartItems.Sum(p => p.Price);
            }
            OnPropertyChanged(nameof(IsBuy));
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
            if (FinalCartItems.Count > 0 || FilteredProducts.Any(x => x.IsChecked == true))
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
                    OnPropertyChanged(nameof(FilteredProducts));
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
            if (IsBuy == true)
            {
                CartValue = FinalCartItems.Sum(p => p.Cost * p.Quantity);
            }
            else
            {
                CartValue = FinalCartItems.Sum(p => p.Price * p.Quantity);
            }

            OnPropertyChanged(nameof(CartItemsCount));
            OnPropertyChanged(nameof(CartValue));
        }
        public void SellCash()
        {
            if (FinalCartItems.Count > 0)
            {
                var transaction = new Transaction
                {

                    totalamount = CartValue,
                    Type = "sell",
                    IsPaid = true,
                    Products = new List<TransactionProductItem>()
                };
                foreach (var item in FinalCartItems)
                {
                    transaction.Products.Add(new TransactionProductItem
                    {
                        Name = item.Name,
                        Quantity = item.Quantity,
                        Price = item.Price,
                    });
                }
                var productnames = transaction.Products.Select(x => x.Name).ToList();
                foreach (var productname in productnames)
                {
                    var temproduct = App.ProductRepository
                        .GetItems()
                        .FirstOrDefault(x => x.Name == productname);
                    if (temproduct != null)
                    {
                        temproduct.stock -= transaction.Products
                      .FirstOrDefault(x => x.Name == productname)
                      .Quantity;
                        App.ProductRepository.UpdateItem(temproduct);
                    }
                }
                App.TransactionRepository.InsertItemWithChildren(transaction, true);
                ClearCart();
                LoadDB();
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(100);
                    var snackbar = Snackbar.Make(
                    message: "Cash Transaction Added successfully",
                    duration: TimeSpan.FromSeconds(2),
                    visualOptions: new SnackbarOptions
                    {
                        BackgroundColor = Colors.Green,
                        TextColor = Colors.White,
                        CornerRadius = 10,

                    },
                    anchor: sellProductsPage
                );
                    await snackbar.Show();
                });

            }
            else
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(100);
                    var snackbar = Snackbar.Make(
                    message: "Add Items To Cart First",
                    duration: TimeSpan.FromSeconds(2),
                    visualOptions: new SnackbarOptions
                    {
                        BackgroundColor = Colors.Red,
                        TextColor = Colors.White,
                        CornerRadius = 10,

                    },
                    anchor: sellProductsPage
                );
                    await snackbar.Show();
                });
            }
        }
        public void SellCredit(Contact contact)
        {
            try
            {
                if (FinalCartItems.Count > 0)
                {
                    var contactid = App.ContactRepository.GetItems().Where(x => x.Name == contact.Name).FirstOrDefault();
                    var transaction = new Transaction
                    {
                        totalamount = CartValue,
                        Type = "sell",
                        IsPaid = false,
                        ContactId = contactid.Id,
                        Products = new List<TransactionProductItem>()
                    };
                    foreach (var item in FinalCartItems)
                    {
                        transaction.Products.Add(new TransactionProductItem
                        {
                            Name = item.Name,
                            Quantity = item.Quantity,
                            Price = item.Price,
                        });
                    }
                    var productnames = transaction.Products.Select(x => x.Name).ToList();
                    foreach (var productname in productnames)
                    {
                        var temproduct = App.ProductRepository
                            .GetItems()
                            .FirstOrDefault(x => x.Name == productname);
                        if (temproduct != null)
                        {
                            temproduct.stock -= transaction.Products
                          .FirstOrDefault(x => x.Name == productname)
                          .Quantity;
                            App.ProductRepository.UpdateItem(temproduct);
                        }
                    }
                    contactid.TotalPurchases += transaction.totalamount;
                    contactid.CreditScore += transaction.totalamount;
                    App.TransactionRepository.InsertItemWithChildren(transaction);
                    App.ContactRepository.UpdateItem(contactid);
                    ClearCart();
                    LoadDB();
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await Task.Delay(100);
                        var snackbar = Snackbar.Make(
                        message: "Credit Transaction Added successfully",
                        duration: TimeSpan.FromSeconds(2),
                        visualOptions: new SnackbarOptions
                        {
                            BackgroundColor = Colors.Green,
                            TextColor = Colors.White,
                            CornerRadius = 10,
                        },
                        anchor: sellProductsPage
                    );
                        await snackbar.Show();
                    });
                }
            }
            catch (Exception ex)
            {
                App.Current.MainPage.DisplayAlert($"WE CAN'T BECAUSE{ex.Message}", "OK", "CANCEL");
            }


        }
        public async void OpenContactSearchPopup()
        {
            if (FinalCartItems.Count > 0)
            {
                await AppShell.Current.ShowPopupAsync(new ContactSearchPopup(this));
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(100);
                    var snackbar = Snackbar.Make(
                    message: "Add Items To Cart First",
                    duration: TimeSpan.FromSeconds(2),
                    visualOptions: new SnackbarOptions
                    {
                        BackgroundColor = Colors.Red,
                        TextColor = Colors.White,
                        CornerRadius = 10,
                    },
                    anchor: sellProductsPage
                );
                    await snackbar.Show();
                });
            }

        }
        public void BuyProducts()
        {
            if (FinalCartItems.Count > 0)
            {
                var transaction = new Transaction
                {

                    totalamount = CartValue,
                    Type = "buy",
                    IsPaid = true,
                    Products = new List<TransactionProductItem>()
                };
                foreach (var item in FinalCartItems)
                {
                    transaction.Products.Add(new TransactionProductItem
                    {
                        Name = item.Name,
                        Quantity = item.Quantity,
                        Cost = item.Cost,
                    });
                }
                var productnames = transaction.Products.Select(x => x.Name).ToList();
                foreach (var productname in productnames)
                {
                    var temproduct = App.ProductRepository
                        .GetItems()
                        .FirstOrDefault(x => x.Name == productname);
                    if (temproduct != null)
                    {
                        temproduct.stock += transaction.Products
                      .FirstOrDefault(x => x.Name == productname)
                      .Quantity;
                        App.ProductRepository.UpdateItem(temproduct);
                    }
                }
                App.TransactionRepository.InsertItemWithChildren(transaction, true);
                ClearCart();
                LoadDB();
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(100);
                    var snackbar = Snackbar.Make(
                    message: "Buy Transaction Added successfully",
                    duration: TimeSpan.FromSeconds(2),
                    visualOptions: new SnackbarOptions
                    {
                        BackgroundColor = Colors.Green,
                        TextColor = Colors.White,
                        CornerRadius = 10,

                    },
                    anchor: sellProductsPage
                );
                    await snackbar.Show();
                });

            }
            else
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(100);
                    var snackbar = Snackbar.Make(
                    message: "Add Items To Cart First",
                    duration: TimeSpan.FromSeconds(2),
                    visualOptions: new SnackbarOptions
                    {
                        BackgroundColor = Colors.Red,
                        TextColor = Colors.White,
                        CornerRadius = 10,

                    },
                    anchor: sellProductsPage
                );
                    await snackbar.Show();
                });
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
        public ICommand RemoveFromCartCommand => new Command<Product>((product) =>
        {
            RemoveFromCart(product);
        });

        public ICommand IncreaseQuantityCommand => new Command<Product>((product) =>
        {
            IncreaseQuantity(product);
        });
        public ICommand DecreaseQuantityCommand => new Command<Product>((product) =>
        {
            DecreaseQuantity(product);
        });
        public ICommand RefresCartItems => new Command(async () => await RefreshCart());
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
        private async Task RefreshCart()
        {
            try
            {
                IsRefreshing = true;
                await Task.Delay(1000);
                ClearCart();
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