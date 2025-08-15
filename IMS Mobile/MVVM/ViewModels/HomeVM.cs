using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Transaction = IMS_Mobile.MVVM.Models.Transaction;
using IMS_Mobile.MVVM.Models;
using IMS_Mobile.Popups;
using CommunityToolkit.Maui.Extensions;
using IMS_Mobile.DB;

namespace IMS_Mobile.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class HomeVM : INotifyPropertyChanged
    {
        #region Properties
        public ObservableCollection<Transaction> Transactions { get; set; } = new ObservableCollection<Transaction>();
        public ObservableCollection<Transaction> FilteredTransactions { get; set; } = new ObservableCollection<Transaction>();
        #endregion

        #region Pagination
        public int PageIndex
        {
            get => _pageIndex;
            set
            {
                _pageIndex = value;
                OnPropertyChanged();
            }
        }

        private int _pageIndex = 1;
        private List<Transaction> _currentFilteredList = new List<Transaction>();

        public void incrementPageIndex()
        {
            var maxPages = (int)Math.Ceiling((double)_currentFilteredList.Count / 10);
            if (PageIndex < maxPages)
            {
                PageIndex++;
                ApplyPagination();
            }
        }

        public void decrementPageIndex()
        {
            if (PageIndex > 1)
            {
                PageIndex--;
                ApplyPagination();
            }
        }

        private void ApplyPagination()
        {
            var maxPages = (int)Math.Ceiling((double)_currentFilteredList.Count / 10);

            if (PageIndex > maxPages && maxPages > 0)
                PageIndex = maxPages;
            if (PageIndex < 1)
                PageIndex = 1;

            var paginatedTransactions = _currentFilteredList
                .Skip((PageIndex - 1) * 10)
                .Take(10)
                .ToList();

            FilteredTransactions.Clear();
            foreach (var transaction in paginatedTransactions)
            {
                FilteredTransactions.Add(transaction);
            }
        }

        public void BackToFirstPage()
        {
            PageIndex = 1;
            ApplyPagination();
        }

        public void BackToLastPage()
        {
            var maxPages = (int)Math.Ceiling((double)_currentFilteredList.Count / 10);
            PageIndex = maxPages > 0 ? maxPages : 1;
            ApplyPagination();
        }

        public ICommand PreviousPage => new Command(() =>
        {
            decrementPageIndex();
        });
        public ICommand NextPage => new Command(() =>
        {
            incrementPageIndex();
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

        #region Filtering
        public void FilterByDateRange(DateTime startDate, DateTime endDate)
        {
            _currentFilteredList = Transactions
                .Where(t => t.CreatedDate.Date >= startDate.Date && t.CreatedDate.Date <= endDate.Date)
                .OrderByDescending(t => t.CreatedDate)
                .ToList();
            PageIndex = 1;
            ApplyPagination();
        }

        public void FilterBuy()
        {
            _currentFilteredList = Transactions
                .Where(t => t.Type == "buy")
                .OrderByDescending(t => t.CreatedDate)
                .ToList();
            PageIndex = 1;
            ApplyPagination();
        }

        public void FilterSell()
        {
            _currentFilteredList = Transactions
                .Where(t => t.Type == "sell")
                .OrderByDescending(t => t.CreatedDate)
                .ToList();
            PageIndex = 1;
            ApplyPagination();
        }

        public void ShowAllTransactions()
        {
            _currentFilteredList = Transactions
                .OrderByDescending(t => t.CreatedDate)
                .ToList();
            PageIndex = 1;
            ApplyPagination();
        }

        public ICommand BuyFilterCommand => new Command(() =>
        {
            FilterBuy();
        });
        public ICommand SellFilterCommand => new Command(() =>
        {
            FilterSell();
        });
        public ICommand AllFilterCommand => new Command(() =>
        {
            ShowAllTransactions();
        });
        #endregion

        #region Commands
        public ICommand DetailsPopup => new Command<Transaction>((transaction) =>
        {
            var products = transaction.Products;
            var items = new List<TransactionProductItem>(products);
            AppShell.Current.ShowPopupAsync(new TransactionDetails(items));
        });
        #endregion

        #region Tasks
        public Task LoadTransactionsAsync()
        {
            var transactions = App.TransactionRepository.GetItemsWithChildren();
            Transactions.Clear();
            FilteredTransactions.Clear();
            foreach (var transaction in transactions)
            {
                Transactions.Add(transaction);
            }
            ShowAllTransactions();
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
