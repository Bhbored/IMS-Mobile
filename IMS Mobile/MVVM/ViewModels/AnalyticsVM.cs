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
        #region Properties
        public ObservableCollection<Transaction> CashFlowTransactions
        {
            get => cashFlowTransactions;
            set
            {
                cashFlowTransactions = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Product> Products
        {
            get => products;
            set
            {
                products = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<TransactionProductItem> TransactionItems
        {
            get => transactionItems;
            set
            {
                transactionItems = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<object> TopSellingProducts
        {
            get => topSellingProducts;
            set
            {
                topSellingProducts = value;
                OnPropertyChanged();
            }
        }
        public int PageIndex
        {
            get => pageIndex;
            set
            {
                pageIndex = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Fields
        private List<Transaction> _allCashFlowTransactions = new List<Transaction>();
        private ObservableCollection<Transaction> cashFlowTransactions = new ObservableCollection<Transaction>();
        private ObservableCollection<Product> products = new ObservableCollection<Product>();
        private ObservableCollection<TransactionProductItem> transactionItems = new ObservableCollection<TransactionProductItem>();
        private ObservableCollection<object> topSellingProducts = new ObservableCollection<object>();
        private int pageIndex = 1;

        #endregion    

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
            Products.Clear();
            TransactionItems.Clear();
            var dbProducts = App.ProductRepository?.GetItems() ?? new List<Product>();
            await Task.Delay(100);
            foreach (var item in dbProducts)
            {
                Products.Add(item);
            }
            var dbTransactionItems = App.TransactionProductItemRepository!
                .GetItems()
                .Where(x => x.Cost == 0)
                .ToList();
            foreach (var item in dbTransactionItems)
            {
                TransactionItems.Add(item);

            }
            OnPropertyChanged(nameof(Products));
            OnPropertyChanged(nameof(TransactionItems));
            CalculateTopSellingProducts();
        }

        #region Pyramid Chart Data

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

        #endregion



        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
