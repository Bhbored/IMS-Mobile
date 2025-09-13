using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Transaction = IMS_Mobile.MVVM.Models.Transaction;
using Product = IMS_Mobile.MVVM.Models.Product;
using TransactionProductItem = IMS_Mobile.MVVM.Models.TransactionProductItem;

namespace IMS_Mobile.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class AnalyticsVM : INotifyPropertyChanged
    {
        public ObservableCollection<Transaction> CashFlowTransactions
        {
            get => cashFlowTransactions;
            set
            {
                cashFlowTransactions = value;
                OnPropertyChanged();
            }
        }
        private List<Transaction> _allCashFlowTransactions = new List<Transaction>();

        private ObservableCollection<Transaction> cashFlowTransactions = new ObservableCollection<Transaction>();

        public int PageIndex { get; set; } = 1;


        #region Pagination Logic

        private void ApplyPagination()
        {
            var maxPages = (int)Math.Ceiling((double)_allCashFlowTransactions.Count / 10);

            if (PageIndex > maxPages && maxPages > 0)
                PageIndex = maxPages;
            if (PageIndex < 1)
                PageIndex = 1;

            var paginatedCashFlow = _allCashFlowTransactions
                .Skip((PageIndex - 1) * 10)
                .Take(10)
                .ToList();

            CashFlowTransactions.Clear();

            foreach (var transaction in paginatedCashFlow)
            {
                CashFlowTransactions.Add(transaction);
            }

            OnPropertyChanged(nameof(PageIndex));
            OnPropertyChanged(nameof(CashFlowTransactions));
        }

        public void IncrementPageIndex()
        {
            var maxPages = (int)Math.Ceiling((double)_allCashFlowTransactions.Count / 10);
            if (PageIndex < maxPages)
            {
                PageIndex++;
                ApplyPagination();
            }
        }

        public void DecrementPageIndex()
        {
            if (PageIndex > 1)
            {
                PageIndex--;
                ApplyPagination();
            }
        }

        public void BackToFirstPage()
        {
            PageIndex = 1;
            ApplyPagination();
        }

        public void BackToLastPage()
        {
            var maxPages = (int)Math.Ceiling((double)_allCashFlowTransactions.Count / 10);
            PageIndex = maxPages > 0 ? maxPages : 1;
            ApplyPagination();
        }

        public ICommand PreviousPage => new Command(() =>
        {
            DecrementPageIndex();
        });

        public ICommand NextPage => new Command(() =>
        {
            IncrementPageIndex();
        });

        public ICommand FirstPage => new Command(() =>
        {
            BackToFirstPage();
        });

        public ICommand LastPage => new Command(() =>
        {
            BackToLastPage();
        });
        #endregion

        public async Task RefreshDataAsync()
        {
            _allCashFlowTransactions.Clear();
            var dbTransaction = App.TransactionRepository?.GetItems() ?? new List<Transaction>();
            await Task.Delay(100);
            foreach (var item in dbTransaction)
            {
                _allCashFlowTransactions.Add(item);
            }
            _allCashFlowTransactions = _allCashFlowTransactions
                .OrderByDescending(t => t.CreatedDate)
                .ToList();
            ApplyPagination();
        }

        #region Pyramid Chart Data

        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();
        public ObservableCollection<TransactionProductItem> TransactionItems { get; set; } = new ObservableCollection<TransactionProductItem>();
        public ObservableCollection<object> TopSellingProducts { get; set; } = new ObservableCollection<object>();

        private void LoadMockProducts()
        {
            var random = new Random();
            var productNames = new[]
            {
                "iPhone 15 Pro", "Samsung Galaxy S24", "MacBook Pro M3", "Dell XPS 13", "iPad Air",
                "Surface Pro 9", "AirPods Pro", "Sony WH-1000XM5", "Nintendo Switch", "PlayStation 5",
                "Xbox Series X", "Apple Watch Ultra", "Samsung Galaxy Watch", "Fitbit Versa", "Garmin Fenix",
                "Canon EOS R5", "Sony A7 IV", "Nikon Z9", "GoPro Hero 12", "DJI Mini 4 Pro"
            };

            Products.Clear();

            for (int i = 0; i < 20; i++)
            {
                var product = new Product
                {
                    Id = i + 1,
                    Name = productNames[i],
                    Price = random.Next(50, 2000),
                    Cost = random.Next(30, 1500),
                    stock = random.Next(0, 100),
                    CreatedDate = DateTime.Now.AddDays(-random.Next(0, 365))
                };

                Products.Add(product);
            }
        }

        private void LoadMockTransactionItems()
        {
            var random = new Random();
            TransactionItems.Clear();

            for (int i = 0; i < 100; i++)
            {
                var productId = random.Next(1, 21);
                var quantity = random.Next(1, 10);
                var product = Products.FirstOrDefault(p => p.Id == productId);

                var transactionItem = new TransactionProductItem
                {
                    Id = i + 1,
                    Name = product?.Name ?? $"Product {productId}",
                    Price = product?.Price ?? random.Next(50, 2000),
                    Cost = product?.Cost ?? random.Next(30, 1500),
                    Quantity = quantity,
                    TransactionId = random.Next(1, 50)
                };

                TransactionItems.Add(transactionItem);
            }
        }

        private void CalculateTopSellingProducts()
        {
            var topProducts = TransactionItems
                .GroupBy(item => item.Name)
                .Select(group => new
                {
                    Name = group.Key,
                    TotalQuantity = group.Sum(item => item.Quantity),
                    TotalRevenue = group.Sum(item => item.TotalPrice)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(8)
                .ToList();

            TopSellingProducts.Clear();

            foreach (var product in topProducts)
            {
                TopSellingProducts.Add(new
                {
                    Name = product.Name,
                    Value = product.TotalQuantity,
                    Revenue = product.TotalRevenue
                });
            }
        }

        public void LoadPyramidChartData()
        {
            LoadMockProducts();
            LoadMockTransactionItems();
            CalculateTopSellingProducts();
        }

        #endregion



        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
